using System;
using System.Collections; 
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading; 
using System.Threading.Tasks;
using System.Drawing;
using ezAutoMom;
using ezTools; 

namespace ezAuto_EFEM
{
    public class HW_BackSide_ATI_AOI_Chipping
    {
        enum eID
        {
            Off,
            On,
            Wafer,
            Find
        }

        public string m_id;

        CPoint m_cpCenter = new CPoint(0, 0);
        double m_dR = 0;

        MinMax m_mmR = new MinMax(-100, 30, null);
        int[] m_aGV = null; 

        Log m_log;
        ezImg m_img;

        const int c_nFFT = 16384;
        int m_nHighPass = c_nFFT / 16; 

        double[] m_aCos = new double[c_nFFT];
        double[] m_aSin = new double[c_nFFT];

        CPoint[] m_cpEdge = new CPoint[c_nFFT];
        double[] m_rEdge = new double[c_nFFT];
        double[] m_rEdgeAve = new double[c_nFFT];
        RPoint[] m_rpFFT = new RPoint[c_nFFT]; 
        byte[,] m_aBuf = null;
        eID[,] m_aWafer = null; 

        HW_BackSide_ATI_AOIData m_data;
        ArrayList m_aChipping = new ArrayList();
        public ArrayList m_arrDefect = new ArrayList();
        double m_dScoreNotch = 90;
        double m_dScoreMax = 0;
        double[] m_arrNotch = new double[60];
        double[] m_arrNotchTemp = new double[60];
        double[] m_arrTemp = new double[60];
        ippTools m_ippTools;
        CPoint m_cpNotchSize = new CPoint(40, 44);
        public string m_strCode = "000";

        bool m_bWhiteWafer = true; 
        bool m_bUse = true;

        public HW_BackSide_ATI_AOI_Chipping()
        {
        }

        public void Init(string id, HW_BackSide_ATI_AOIData data, Log log)
        {
            m_id = id;
            m_data = data;
            m_log = log;
            for (int n = 0; n < c_nFFT; n++)
            {
                m_aCos[n] = Math.Cos(2 * Math.PI * n / c_nFFT);
                m_aSin[n] = Math.Sin(2 * Math.PI * n / c_nFFT);
            }
            for (int n = 0; n < m_arrNotch.Length; n++) // ing 170302
            {
                m_log.m_reg.Read("NotchPattern" + n.ToString(), ref m_arrNotch[n]);
            }
            m_ippTools = new ippTools(); // ing 170302
        }

        public void ThreadStop()
        {
        }

        public void RunGrid(ezGrid grid, eGrid eMode, ezJob job = null)
        {
            grid.Set(ref m_bUse, m_id, "Use", "Use"); 
            grid.Set(ref m_mmR, m_id, "ROI", "Find Edge Range (pixel)");
            grid.Set(ref m_minChipping, m_id, "MinSize", "Minimum Chipping Size (pixel)");
            grid.Set(ref m_dScoreNotch, m_id, "NotchScore", "Notch Score (Pattern Matching)");
            grid.Set(ref m_strCode, m_id, "Code", "Defect Code");
        }

        public void DrawChipping(Graphics dc, ezImgView view, bool bDraw)
        {
            try
            {
                if (m_img == null) return;
                if (m_img.m_bNew) return;
                if (bDraw)
                {
                    foreach (CPoint cp in m_cpEdge) DrawCross(dc, view, Color.GreenYellow, cp, 1);
                    foreach (HW_BackSide_ATI_AOI_Chipping_Dat dat in m_aChipping) DrawChipping(dc, view, Color.Red, dat); 
                }
            }
            catch (Exception ex)
            {
                m_log.Add(ex.Message);
            }
        }

        void DrawCross(Graphics dc, ezImgView view, Color color, CPoint cp, int nLine)
        {
            view.DrawLine(dc, color, cp + new CPoint(-nLine, 0), cp + new CPoint(nLine, 0));
            view.DrawLine(dc, color, cp + new CPoint(0, -nLine), cp + new CPoint(0, nLine));
        }

        void DrawChipping(Graphics dc, ezImgView view, Color color, HW_BackSide_ATI_AOI_Chipping_Dat dat)
        {
            int nLine = 4 * dat.m_nWidth;
            CPoint cp = dat.m_cpEdge; 
            view.DrawLine(dc, color, cp - new CPoint(-nLine, -nLine), cp + new CPoint(-nLine, nLine));
            view.DrawLine(dc, color, cp - new CPoint(-nLine, nLine), cp + new CPoint(nLine, nLine));
            view.DrawLine(dc, color, cp - new CPoint(nLine, nLine), cp + new CPoint(nLine, -nLine));
            view.DrawLine(dc, color, cp - new CPoint(nLine, -nLine), cp + new CPoint(-nLine, -nLine));
            view.DrawString(dc, " h = " + dat.m_nHeight.ToString() + ", w = " + dat.m_nWidth.ToString(), cp - new CPoint(-nLine, -nLine), Brushes.Red); 
        }

        public void Clear()
        {
            for (int n = 0; n < m_cpEdge.Length; n++)
            {
                m_cpEdge[n] = new CPoint(0, 0);
            }
            m_aChipping.Clear();
            m_arrDefect.Clear();
        }

        public bool Inspect(ezImg img)
        {
            ezStopWatch sw = new ezStopWatch();
            m_img = img;
            m_dR = m_data.m_dR;
            m_cpCenter = m_data.m_cpCenter; 
            m_aGV = m_data.m_aGV;
            m_bWhiteWafer = m_data.m_bWhiteWafer;
            m_dScoreMax = 0;
            Clear(); 
            ReAllocateBuf();
            Array.Clear(m_aWafer, 0, c_nFFT * m_mmR.Range());
            if (!m_bUse) return false;
            m_log.Add(m_id + " - Start Chipping Inspection.");
            for (int n = 0; n < c_nFFT; n++) MakeImage(n); 
            for (int n = 0; n < c_nFFT; n++) FindEdge(n);
            SaveFile("D:\\Chipping.dat");
            HighPass();
            SaveFile("D:\\Chipping1.dat");
            FindChipping(); 
            m_log.Add(m_id + " - AOI Chipping : " + sw.Check().ToString());
            return false;
        }

        void SaveFile(string strFile) //forget
        {
            StreamWriter sw = new StreamWriter(new FileStream(strFile, FileMode.Create));
            for (int n = 0; n < c_nFFT; n++)
            {
                int nDat = (int)Math.Round(m_rEdge[n]); 
                sw.WriteLine(nDat.ToString());
            }
            sw.Close(); 
        }

        void ReAllocateBuf()
        {
            if ((m_aBuf != null) && (m_aBuf.Length == (c_nFFT * m_mmR.Range()))) return;
            m_aBuf = new byte[c_nFFT, m_mmR.Range()];
            m_aWafer = new eID[c_nFFT, m_mmR.Range()]; 
        }

        void MakeImage(int nID)
        {
            for (int n = m_mmR.Min, i = 0; n < m_mmR.Max; n++, i++)
            {
                double r = m_dR + n;
                int x = m_cpCenter.x + (int)Math.Round(r * m_aCos[nID]);
                int y = m_cpCenter.y + (int)Math.Round(r * m_aSin[nID]);
                m_aBuf[nID, i] = GetGV(x, y);
                if (m_bWhiteWafer ^ (m_aBuf[nID, i] < m_aGV[x])) m_aWafer[nID, i] = eID.On;
                else m_aWafer[nID, i] = eID.Off; 
            }
            for (int n = m_mmR.Min, i = 0; n < m_mmR.Max; n++, i++)
            {
                if (m_aWafer[nID, i] == eID.On) m_aWafer[nID, i] = eID.Wafer;
                else return; 
            }
        }

        byte GetGV(int x0, int y0)
        {
            int nGV = 0;
            for (int y = y0 - 2; y <= y0 + 2; y += 2)
            {
                for (int x = x0 - 2; x <= x0 + 2; x += 2)
                {
                    nGV += m_img.m_aBuf[y, x];
                }
            }
            return (byte)(nGV / 9);
        }

        void FindEdge(int nID) 
        {
            for (int i = 0; i < m_mmR.Range(); i++) CheckWafer(nID, i);
            for (int n = m_mmR.Max - 1, i = m_mmR.Range() - 1; n >= m_mmR.Min; n--, i--)
            {
                if (m_aWafer[nID, i] == eID.Wafer)
                {
                    double r = m_dR + n;
                    m_rEdge[nID] = r;
                    m_cpEdge[nID].x = m_cpCenter.x + (int)Math.Round(r * m_aCos[nID]);
                    m_cpEdge[nID].y = m_cpCenter.y + (int)Math.Round(r * m_aSin[nID]);
                    return; 
                }
            }
        }

        void CheckWafer(int nID, int i)
        {
            if (m_aWafer[nID, i] != eID.On) return;
            eID eid = FindWafer(nID, i);
            FillWafer(nID, i, eid); 
        }
        
        Stack m_aStack = new Stack();

        eID FindWafer(int cpy, int cpx)
        {
            int x, y, x0, y0;
            int xl = 0;
            int xr = m_mmR.Range() - 1;
            m_aWafer[cpy, cpx] = eID.Find;
            m_aStack.Clear();
            while (true)
            {
                x0 = cpx;
                y0 = cpy;
                if (cpx > xl)
                {
                    x = x0 - 1;
                    y = y0;
                    if (m_aWafer[y, x] == eID.Wafer) return eID.Wafer; 
                    if (m_aWafer[y, x] == eID.On)
                    {
                        m_aWafer[y, x] = eID.Find;
                        m_aStack.Push(new CPoint(x0, y0));
                        cpx--;
                        continue;
                    }
                }
                if (true)
                {
                    x = x0;
                    y = (y0 + c_nFFT - 1) % c_nFFT;
                    if (m_aWafer[y, x] == eID.Wafer) return eID.Wafer;
                    if (m_aWafer[y, x] == eID.On)
                    {
                        m_aWafer[y, x] = eID.Find;
                        m_aStack.Push(new CPoint(x0, y0));
                        cpy = (cpy + c_nFFT - 1) % c_nFFT;
                        continue;
                    }
                }
                if (true)
                {
                    x = x0;
                    y = (y0 + c_nFFT + 1) % c_nFFT;
                    if (m_aWafer[y, x] == eID.Wafer) return eID.Wafer;
                    if (m_aWafer[y, x] == eID.On)
                    {
                        m_aWafer[y, x] = eID.Find;
                        m_aStack.Push(new CPoint(x0, y0));
                        cpy = (cpy + c_nFFT + 1) % c_nFFT;
                        continue;
                    }
                }
                if (cpx < xr)
                {
                    x = x0 + 1;
                    y = y0;
                    if (m_aWafer[y, x] == eID.Wafer) return eID.Wafer;
                    if (m_aWafer[y, x] == eID.On)
                    {
                        m_aWafer[y, x] = eID.Find;
                        m_aStack.Push(new CPoint(x0, y0));
                        cpx++;
                        continue;
                    }
                }
                if (m_aStack.Count == 0) break;
                CPoint cp0 = (CPoint)m_aStack.Pop();
                cpx = cp0.x;
                cpy = cp0.y;
            }
            return eID.Off;
        }

        void FillWafer(int cpy, int cpx, eID eid)
        {
            int x, y, x0, y0;
            int xl = 0;
            int xr = m_mmR.Range() - 1;
            m_aWafer[cpy, cpx] = eid;
            m_aStack.Clear();
            while (true)
            {
                x0 = cpx;
                y0 = cpy;
                if (cpx > xl)
                {
                    x = x0 - 1;
                    y = y0;
                    if (m_aWafer[y, x] == eID.Find)
                    {
                        m_aWafer[y, x] = eid;
                        m_aStack.Push(new CPoint(x0, y0));
                        cpx--;
                        continue;
                    }
                }
                if (true)
                {
                    x = x0;
                    y = (y0 + c_nFFT - 1) % c_nFFT;
                    if (m_aWafer[y, x] == eID.Find)
                    {
                        m_aWafer[y, x] = eid;
                        m_aStack.Push(new CPoint(x0, y0));
                        cpy = (cpy + c_nFFT - 1) % c_nFFT; 
                        continue;
                    }
                }
                if (true)
                {
                    x = x0;
                    y = (y0 + c_nFFT + 1) % c_nFFT;
                    if (m_aWafer[y, x] == eID.Find)
                    {
                        m_aWafer[y, x] = eid;
                        m_aStack.Push(new CPoint(x0, y0));
                        cpy = (cpy + c_nFFT + 1) % c_nFFT; 
                        continue;
                    }
                }
                if (cpx < xr)
                {
                    x = x0 + 1;
                    y = y0;
                    if (m_aWafer[y, x] == eID.Find)
                    {
                        m_aWafer[y, x] = eid;
                        m_aStack.Push(new CPoint(x0, y0));
                        cpx++;
                        continue;
                    }
                }
                if (m_aStack.Count == 0) break;
                CPoint cp0 = (CPoint)m_aStack.Pop();
                cpx = cp0.x;
                cpy = cp0.y;
            }
        }

        void HighPass()
        {
            double fSum = 0;
            int nDiv = 2 * m_nHighPass + 1;
            for (int n = -m_nHighPass; n <= m_nHighPass; n++) fSum += m_rEdge[(n + c_nFFT) % c_nFFT];
            for (int n = 0; n < c_nFFT; n++)
            {
                m_rEdgeAve[n] = fSum / nDiv;
                fSum += m_rEdge[(n + m_nHighPass + 1 + c_nFFT) % c_nFFT];
                fSum -= m_rEdge[(n - m_nHighPass + c_nFFT) % c_nFFT];
            }
            for (int n = 0; n < c_nFFT; n++) m_rEdge[n] = m_rEdgeAve[n] - m_rEdge[n];
        }

        CPoint m_minChipping = new CPoint(5, 7); 

        void FindChipping()
        {
            for (int n = 0; n < c_nFFT; n++) FindChipping(n); 
        }

        void FindChipping(int n0)
        {
            int nMax = n0; 
            double yMax = m_rEdge[n0]; 
            if (m_rEdge[n0] < m_minChipping.y) return;
            int nl = n0 - 1;
            while ((m_rEdge[(nl + c_nFFT) % c_nFFT] >= m_minChipping.y) && ((n0 - nl) < c_nFFT))
            {
                if (yMax < m_rEdge[(nl + c_nFFT) % c_nFFT])
                {
                    nMax = (nl + c_nFFT) % c_nFFT; 
                    yMax = m_rEdge[nMax];
                }
                nl--; 
            }
            int nr = n0 + 1;
            while ((m_rEdge[(nr + c_nFFT) % c_nFFT] >= m_minChipping.y) && ((nr - nl) < c_nFFT))
            {
                if (yMax < m_rEdge[(nr + c_nFFT) % c_nFFT])
                {
                    nMax = (nr + c_nFFT) % c_nFFT;
                    yMax = m_rEdge[nMax];
                }
                nr++; 
            }

            HW_BackSide_ATI_AOI_Chipping_Dat dat = new HW_BackSide_ATI_AOI_Chipping_Dat();
            dat.m_nHeight = (int)Math.Round(yMax);

            double yMid = yMax / 2;
            if (yMid < m_minChipping.y) yMid = m_minChipping.y; 
            int il = nMax - 1;
            while (m_rEdge[(il + c_nFFT) % c_nFFT] >= yMid) il--;
            int ir = nMax + 1;
            while (m_rEdge[(ir + c_nFFT) % c_nFFT] >= yMid) ir++;
            dat.m_nWidth = ir - il;

            nMax = (ir + il) / 2; 
            dat.m_cpEdge.x = m_cpCenter.x + (int)Math.Round(m_data.m_dR * m_aCos[nMax]);
            dat.m_cpEdge.y = m_cpCenter.y + (int)Math.Round(m_data.m_dR * m_aSin[nMax]);

            for (int n = 0; n < m_arrTemp.Length; n++)
            {
                m_arrTemp[n] = (int)m_rEdge[((nr + nl - m_arrTemp.Length) / 2 + c_nFFT + n) % c_nFFT];
            }

            for (int n = nl; n <= nr; n++)
            {
                m_rEdge[(n + c_nFFT) % c_nFFT] = 0;
            }
            double dScore;
            dScore = m_ippTools.ippiCrossCorrSame_NormLevel(m_arrTemp, m_arrNotch, new CPoint(m_arrTemp.Length, 1), new CPoint(m_arrNotch.Length, 1));
            if ((dat.m_cpEdge.GetL(m_data.m_cpNotch) > 100 || dScore < m_dScoreNotch) && dat.m_nWidth > m_minChipping.x)
            {
                if (dScore > m_dScoreNotch && dScore > m_dScoreMax)
                {
                    m_dScoreMax = dScore;
                    m_data.m_cpNotch = dat.m_cpEdge;
                    m_log.Add("Notch Score = " + dScore.ToString("0.00"));
                    return;
                }
                m_aChipping.Add(dat);
                azBlob.Island def = new azBlob.Island();
                def.m_cp0 = dat.m_cpEdge;// -m_data.m_dieMap.m_rpPitch.ToCPoint() - new CPoint(dat.m_nWidth, dat.m_nHeight);
                def.m_cp1 = dat.m_cpEdge;// +m_data.m_dieMap.m_rpPitch.ToCPoint() + new CPoint(dat.m_nWidth, dat.m_nHeight);
                def.m_strCode = m_strCode;
                def.m_nSize = dat.m_nWidth;
                CPoint cpOffset = new CPoint((int)(m_data.m_dieMap.m_rpPitch.x / m_data.m_fResolution), (int)(m_data.m_dieMap.m_rpPitch.y / m_data.m_fResolution));
                def.m_aPosition.Add(dat.m_cpEdge);
                def.m_aPosition.Add(new CPoint(dat.m_cpEdge.x - cpOffset.x, dat.m_cpEdge.y - cpOffset.y));
                def.m_aPosition.Add(new CPoint(dat.m_cpEdge.x - cpOffset.x, dat.m_cpEdge.y + cpOffset.y));
                def.m_aPosition.Add(new CPoint(dat.m_cpEdge.x + cpOffset.x, dat.m_cpEdge.y - cpOffset.y));
                def.m_aPosition.Add(new CPoint(dat.m_cpEdge.x + cpOffset.x, dat.m_cpEdge.y + cpOffset.y));
                def.m_aPosition.Add(new CPoint(dat.m_cpEdge.x, dat.m_cpEdge.y + cpOffset.y));
                def.m_aPosition.Add(new CPoint(dat.m_cpEdge.x, dat.m_cpEdge.y - cpOffset.y));
                def.m_aPosition.Add(new CPoint(dat.m_cpEdge.x + cpOffset.x, dat.m_cpEdge.y));
                def.m_aPosition.Add(new CPoint(dat.m_cpEdge.x - cpOffset.x, dat.m_cpEdge.y));
                cpOffset.x = (int)(cpOffset.x * 1.2);
                cpOffset.y = (int)(cpOffset.y * 1.5);
                def.m_aPosition.Add(new CPoint(dat.m_cpEdge.x - cpOffset.x, dat.m_cpEdge.y - cpOffset.y));
                def.m_aPosition.Add(new CPoint(dat.m_cpEdge.x - cpOffset.x, dat.m_cpEdge.y + cpOffset.y));
                def.m_aPosition.Add(new CPoint(dat.m_cpEdge.x + cpOffset.x, dat.m_cpEdge.y - cpOffset.y));
                def.m_aPosition.Add(new CPoint(dat.m_cpEdge.x + cpOffset.x, dat.m_cpEdge.y + cpOffset.y));
                def.m_aPosition.Add(new CPoint(dat.m_cpEdge.x, dat.m_cpEdge.y + cpOffset.y));
                def.m_aPosition.Add(new CPoint(dat.m_cpEdge.x, dat.m_cpEdge.y - cpOffset.y));
                def.m_aPosition.Add(new CPoint(dat.m_cpEdge.x + cpOffset.x, dat.m_cpEdge.y));
                def.m_aPosition.Add(new CPoint(dat.m_cpEdge.x - cpOffset.x, dat.m_cpEdge.y));
//                def.m_aPosition.Add(new CPoint(cpCenter.x - ));
                m_arrDefect.Add(def);
                m_log.Add("Chipping Height = " + dat.m_nHeight.ToString() + ", Width = " + dat.m_nWidth + ", Notch Score = " + dScore.ToString("0.00"));
            }
            else if (dScore >= m_dScoreNotch)
            {
                m_log.Add("Notch Score = " + dScore.ToString("0.00"));
            }
            if ((dat.m_cpEdge.GetL(m_data.m_cpNotch) < 100))
            {
                Array.Copy(m_arrTemp, m_arrNotchTemp, m_arrTemp.Length);      
            }
        }

        public bool SaveNotchArr(string strFile)
        {
            //if (m_aChipping.Count != 1) return true;
            Array.Copy(m_arrNotchTemp, m_arrNotch, m_arrNotchTemp.Length);
            StreamWriter sw;
            try
            {
                sw = new StreamWriter(new FileStream(strFile, FileMode.Create));
                for (int n = 0; n < m_arrNotch.Length; n++)
                {
                    try
                    {
                        sw.WriteLine(m_arrNotch[n].ToString());
                    }
                    catch
                    {
                        sw.Close();
                        m_log.Popup("Notch Pattern File Saving Fail !! - " + strFile);
                        return true;
                    }
                }
            }
            catch
            {
                m_log.Popup("Notch Pattern File Saving Fail !! - " + strFile);
                return true;
            }
            //for (int n = 0; n < m_arrNotch.Length; n++)
            //{
                //m_log.m_reg.Write("NotchPattern" + n.ToString(), m_arrNotch[n]);         
            //}
            sw.Close();
            return false;
        }

        public bool ReadNotchArr(string strFile)
        {
            string strLine = "";
            StreamReader sr;
            try
            {
                sr = new StreamReader(strFile);
                for (int n = 0; n < m_arrNotch.Length; n++)
                {
                    try
                    {
                        strLine = sr.ReadLine();
                        m_arrNotch[n] = Convert.ToDouble(strLine);
                    }
                    catch
                    {
                        sr.Close();
                        m_log.Popup("Notch Pattern File Reading Fail !! - " + strFile);
                        return true;
                    }
                }
            }
            catch
            {
                m_log.Popup("Notch Pattern File Reading Fail !! - " + strFile);
                return true;
            }
            sr.Close();
            return false;
        }

    }

    struct HW_BackSide_ATI_AOI_Chipping_Dat
    {
        public int m_nWidth;
        public int m_nHeight;
        public CPoint m_cpEdge;
    }
}
