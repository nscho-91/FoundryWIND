using System;
using System.Collections; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ezAutoMom;
using ezTools;

namespace ezAuto_EFEM
{
    public enum eOrientation { R0, R90, R180, R270 };
    
    class HW_BackSide_ATI_AOI_Edge
    {
        HW_BackSide_ATI_AOIData m_data; 
        ArrayList m_aEdge = new ArrayList();

        double[] m_aCos = new double[361];
        double[] m_aSin = new double[361]; 

        string m_id;
        int m_nGV = 0; 
        public int[] m_aGV = null;
        bool m_bDiviaion = false;
        bool m_bWhiteWafer = true;
        bool m_bDraw = false;
        bool m_bNotchFind90 = true;
        bool m_bScaleError = false;
        int m_nNotchHeight = 50;
        public CPoint m_cpShift = new CPoint(0, 0);
        CPoint m_cpShiftLimit = new CPoint(300, 300);
        public bool m_bOffCenter = false;
        public eOrientation m_eOrientation = 0;
        string m_strOrientation = eOrientation.R0.ToString();
        string[] m_strOrientations = new string[Enum.GetNames(typeof(eOrientation)).Length];
        Log m_log;
        ezImg m_img = null;
        CPoint m_szImg;

        public void Init(string id, HW_BackSide_ATI_AOIData data, Log log)
        {
            m_id = id;
            m_data = data; 
            m_log = log;
            for (int n = 0; n <= 360; n++)
            {
                m_aCos[n] = Math.Cos(Math.PI * n / 180);
                m_aSin[n] = Math.Sin(Math.PI * n / 180); 
            }
            for (int n = 0; n < m_strOrientations.Length; n++)
            {
                m_strOrientations[n] = Enum.GetNames(typeof(eOrientation))[n];
            }
            m_strOrientation = eOrientation.R0.ToString();
        }

        public void RunGrid(ezGrid grid, eGrid eMode, ezJob job = null)
        {
            grid.Set(ref m_strOrientation, m_strOrientations, m_id, "Orientation", "Notch Orientation");
            grid.Set(ref m_bDiviaion, m_id, "Deviation", "Use a Deviation for Finding Edge");
            grid.Set(ref m_nGV, m_id, "EdgeGV", "Edge Find GV (1 ~ 255)");
            //grid.Set(ref m_dGV, m_id, "FrameDiffGV", "Differenctial GV For Find Frame(1 ~ 255)");
            grid.Set(ref m_nAveRange, m_id, "AveRange", "Frame Edge Range Of Average");
            grid.Set(ref m_cpShiftLimit, m_id, "ShiftCenter", "Limit of Shift Between Wafer Center and Frame Center (um)");
            grid.Set(ref m_nNotchHeight, m_id, "Notch Height", "Notch Height");
            grid.Set(ref m_bNotchFind90, m_id, "Notch90", "On 90 Dgree Rotated Wafer, Refind a Notch Position");
            grid.Set(ref m_bScaleError, m_id, "ScaleErr", "True = X, Y Scale Id Different, False = X, Y Scale Is Same (Different Notch Find Algorism)");
            grid.Set(ref m_bDraw, m_id, "DrawMap", "Draw Die Map");
            if ((eMode == eGrid.eJobOpen) || (eMode == eGrid.eRegRead)) m_bDraw = false;
        }

        public bool FindEdge(ezImg img)
        {
            ezStopWatch sw = new ezStopWatch();
            m_img = img;
            m_szImg = m_img.m_szImg; 
            m_aEdge.Clear();
            FindEdge(new CPoint(0, 1), new CPoint(1, 0));
            FindEdge(new CPoint(0, -1), new CPoint(1, 0));
            FindEdge(new CPoint(1, 0), new CPoint(0, 1));
            FindEdge(new CPoint(-1, 0), new CPoint(0, 1));
            m_log.Add("FindEdge Inspect Time : " + sw.Check().ToString());
            return false;
        }

        //const int c_nR = 6000;
        public int c_nR = 4000;
        public int c_dR = 10; 
        void FindEdge(CPoint cpDir, CPoint cpFor)
        {
            int dR = (int)(c_nR / Math.Sqrt(2) + 200); 
            CPoint cpPeak = new CPoint(m_szImg.x / 2, m_szImg.y / 2);
            cpPeak += (cpDir * c_nR); 
            FindEdge(ref cpPeak, cpDir);
            cpFor *= c_dR; 
            CPoint cp = cpPeak;
            for (int n = 0; n < dR; n += c_dR)
            {
                cp += cpFor;
                FindEdge(ref cp, cpDir); 
            }
            cp = cpPeak;
            for (int n = 0; n < dR; n += c_dR)
            {
                cp -= cpFor;
                FindEdge(ref cp, cpDir);
            }
        }

        void FindEdge(ref CPoint cp, CPoint cpDir)
        {
            int nGV = m_aGV[cp.x]; 
            if (m_bWhiteWafer)
            {
                while (IsValid(cp) && (GetGV(cp) > nGV)) cp += cpDir;
                while (IsValid(cp) && (GetGV(cp) < nGV)) cp -= cpDir;
            }
            else
            {
                while (IsValid(cp) && (GetGV(cp) < nGV)) cp += cpDir;
                while (IsValid(cp) && (GetGV(cp) > nGV)) cp -= cpDir;
            }
            if (IsValid(cp)) m_aEdge.Add(cp);
        }

        byte GetGV(CPoint cp)
        {
            return GetGV(cp.x, cp.y); 
        }

        byte GetGV(int xp, int yp)
        {
            int nGV = 0;
            for (int y = yp - 2; y <= yp + 2; y += 2)
            {
                for (int x = xp - 2; x <= xp + 2; x += 2)
                {
                    nGV += m_img.m_aBuf[y, x];
                }
            }
            return (byte)(nGV / 9);
        }

        void FindEdge(double fAngle)
        {
            int x, y, nSum, nOrgSum, nMax, nIndex;
            CPoint cpEdge;
            nSum = nMax = nIndex = 0;
            byte[] aGV = new byte[m_data.m_nRange];
            byte[,] aAve = new byte[2, m_data.m_nRange - 1];
            byte[] aDiff = new byte[m_data.m_nRange - 1];
            for (int n = 0; n < aGV.Length; n++)
            {
                x = m_data.m_cpManualCenter.x + (int)((m_data.m_nManualR - (m_data.m_nRange / 2) + n) * Math.Cos(fAngle));
                y = m_data.m_cpManualCenter.y + (int)((m_data.m_nManualR - (m_data.m_nRange / 2) + n) * Math.Sin(fAngle));
                aGV[n] = m_img.GetGV(new CPoint(x, y));
                nSum += aGV[n];
            }
            nOrgSum = nSum;
            for (int n = 0; n < aAve.Length / 2; n++)
            {
                nSum -= aGV[n];
                aAve[0, n] = (byte)((nOrgSum - nSum) / (n + 1));
                aAve[1, n] = (byte)(nSum / (aGV.Length - (n + 1)));
            }
            for (int n = 0; n < aDiff.Length; n++)
            {
                aDiff[n] = (byte)Math.Abs(aAve[0, n] - aAve[1, n]);
                if (nMax < aDiff[n])
                {
                    nMax = aDiff[n];
                    nIndex = n;
                }
            }
            if (nIndex < m_data.m_nRange * 0.2 || nIndex > m_data.m_nRange * 0.8) return;
            x = m_data.m_cpManualCenter.x + (int)((m_data.m_nManualR - (m_data.m_nRange / 2) + nIndex) * Math.Cos(fAngle));
            y = m_data.m_cpManualCenter.y + (int)((m_data.m_nManualR - (m_data.m_nRange / 2) + nIndex) * Math.Sin(fAngle));
            cpEdge = new CPoint(x, y);
            m_aEdge.Add(cpEdge);
        }

        bool IsValid(CPoint cp)
        {
            if ((cp.x < 2) || (cp.y < 2)) return false;
            if (cp.x >= m_szImg.x - 2) return false;
            if (cp.y >= m_szImg.y - 2) return false; 
            return true; 
        }

        public bool FindNotch()
        {
            double nSum, nAvg, nDistance, nMin;
            int n, nNotch = 0;
            if (m_aEdge.Count < 1) return true;
            nSum = nAvg = 0;
            foreach (CPoint cp in m_aEdge)
            {
                nSum += cp.GetL(m_data.m_cpCenter);
            }
            nAvg = nSum / m_aEdge.Count;
            nMin = ((CPoint)m_aEdge[0]).GetL(m_data.m_cpCenter);
            for (n = 0; n < m_aEdge.Count; n++)
            {
                nDistance = ((CPoint)m_aEdge[n]).GetL(m_data.m_cpCenter);
                if (nDistance > nAvg + 200 || nDistance < nAvg - 200)
                {
                    m_aEdge.RemoveAt(n);
                    n--;
                    continue;
                }
                if (nDistance < nMin)
                {
                    nMin = nDistance;
                    nNotch = n;
                }
            }
            m_data.m_cpNotch = (CPoint)m_aEdge[nNotch];
            m_data.m_cpDiffNotchAndCenter = m_data.m_cpCenter - m_data.m_cpNotch;
            return false;
        }

        public bool FindNotch2()
        {
            if (m_bScaleError) return FindNotch3();
            double fMax = 0f;
            double fSum = 0f, fAvg = 0f;
            int nCnt = 10, nNotch = 0;
            double fDis;

            foreach (CPoint cp in m_aEdge)
            {
                fSum += cp.GetL(m_data.m_cpCenter);
            }
            fAvg = fSum / m_aEdge.Count;


            for (int i = 0; i < m_aEdge.Count; i++)
            {

                fDis = ((CPoint)m_aEdge[i]).GetL(m_data.m_cpCenter);
                if (fDis > fAvg + 200 || fDis < fAvg - 200||Math.Abs(m_data.m_dR - ((CPoint)m_aEdge[i]).GetL(m_data.m_cpCenter)) > m_nNotchHeight)
                {
                    m_aEdge.RemoveAt(i);
                    i--;
                    continue;
                }
                if (i + nCnt >= m_aEdge.Count)
                    continue;

                //for (int j = 0; j < nCnt; j++)
                {
                    fSum = Math.Abs(m_data.m_dR - ((CPoint)m_aEdge[i]).GetL(m_data.m_cpCenter)) +
                           Math.Abs(m_data.m_dR - ((CPoint)m_aEdge[i + 1]).GetL(m_data.m_cpCenter)) +
                           Math.Abs(m_data.m_dR - ((CPoint)m_aEdge[i + 2]).GetL(m_data.m_cpCenter)) +
                    Math.Abs(m_data.m_dR - ((CPoint)m_aEdge[i + 3]).GetL(m_data.m_cpCenter)) +
                    Math.Abs(m_data.m_dR - ((CPoint)m_aEdge[i + 4]).GetL(m_data.m_cpCenter)) +
                    Math.Abs(m_data.m_dR - ((CPoint)m_aEdge[i + 5]).GetL(m_data.m_cpCenter)) +
                    Math.Abs(m_data.m_dR - ((CPoint)m_aEdge[i + 6]).GetL(m_data.m_cpCenter)) +
                    Math.Abs(m_data.m_dR - ((CPoint)m_aEdge[i + 7]).GetL(m_data.m_cpCenter)) +
                    Math.Abs(m_data.m_dR - ((CPoint)m_aEdge[i + 8]).GetL(m_data.m_cpCenter)) +
                    Math.Abs(m_data.m_dR - ((CPoint)m_aEdge[i + 9]).GetL(m_data.m_cpCenter));
                }
                if (fMax < fSum && fSum < m_nNotchHeight * nCnt)
                {
                    fMax = fSum;
                    nNotch = i + 4;
                    m_data.m_cpNotch = (CPoint)m_aEdge[nNotch];
                    m_data.m_cpDiffNotchAndCenter = m_data.m_cpCenter - m_data.m_cpNotch;
                }
            }
            m_log.Add("Find Notch : " + m_data.m_cpNotch.ToPoint());
            return false;
        }

        public bool FindNotch3()
        {
            double fMax = 0f;
            int a = 0;
            double fSum = 0f, fAvg = 0f, fAvgTemp = 0f, fSumTemp = 0f;
            int nCnt = 10, nNotch = 0;
            double fDis;

            foreach (CPoint cp in m_aEdge)
            {
                fSum += cp.GetL(m_data.m_cpCenter);
            }
            fAvg = fSum / m_aEdge.Count;

            for (int n = 100; n < 200; n++)
            {
                fSumTemp += ((CPoint)m_aEdge[n]).GetL(m_data.m_cpCenter);
            }

            for (int i = 0; i < m_aEdge.Count; i++)
            {
                fDis = ((CPoint)m_aEdge[i]).GetL(m_data.m_cpCenter);
                if (fDis > fAvg + 200 || fDis < fAvg - 200 || Math.Abs(m_data.m_dR - ((CPoint)m_aEdge[i]).GetL(m_data.m_cpCenter)) > m_nNotchHeight)
                {
                    m_aEdge.RemoveAt(i);
                    i--;
                    continue;
                }
                if (i + nCnt >= m_aEdge.Count)
                    continue;
            }
            try
            {
                for (int i = 0; i < m_aEdge.Count; i++)
                {
                    a = i;
                    if (((CPoint)m_aEdge[i]).x == 6230)
                    {
                        a = 0;
                    }

                    fSumTemp = fSumTemp - ((CPoint)m_aEdge[(i + 100) % m_aEdge.Count]).GetL(m_data.m_cpCenter) + ((CPoint)m_aEdge[(i + 200) % m_aEdge.Count]).GetL(m_data.m_cpCenter);
                    fAvgTemp = fSumTemp / 100;
                    //for (int j = 0; j < nCnt; j++)
                    {
                        fSum = 0;
                        fSum += fAvgTemp - ((CPoint)m_aEdge[i]).GetL(m_data.m_cpCenter);
                        fSum += fAvgTemp - ((CPoint)m_aEdge[(i + 1) % m_aEdge.Count]).GetL(m_data.m_cpCenter);
                        fSum += fAvgTemp - ((CPoint)m_aEdge[(i + 2) % m_aEdge.Count]).GetL(m_data.m_cpCenter);
                        fSum += fAvgTemp - ((CPoint)m_aEdge[(i + 3) % m_aEdge.Count]).GetL(m_data.m_cpCenter);
                        fSum += fAvgTemp - ((CPoint)m_aEdge[(i + 4) % m_aEdge.Count]).GetL(m_data.m_cpCenter);
                        fSum += fAvgTemp - ((CPoint)m_aEdge[(i + 5) % m_aEdge.Count]).GetL(m_data.m_cpCenter);
                        fSum += fAvgTemp - ((CPoint)m_aEdge[(i + 6) % m_aEdge.Count]).GetL(m_data.m_cpCenter);
                        fSum += fAvgTemp - ((CPoint)m_aEdge[(i + 7) % m_aEdge.Count]).GetL(m_data.m_cpCenter);
                        fSum += fAvgTemp - ((CPoint)m_aEdge[(i + 8) % m_aEdge.Count]).GetL(m_data.m_cpCenter);
                        fSum += fAvgTemp - ((CPoint)m_aEdge[(i + 9) % m_aEdge.Count]).GetL(m_data.m_cpCenter);
                    }
                    if (fMax < fSum)
                    {
                        fMax = fSum;
                        nNotch = i + 4;
                        m_data.m_cpNotch = (CPoint)m_aEdge[nNotch % m_aEdge.Count];
                        m_data.m_cpDiffNotchAndCenter = m_data.m_cpCenter - m_data.m_cpNotch;
                    }
                }
            }
            catch
            {
                m_log.Add(a.ToString());
            }
            m_log.Add("Find Notch : " + m_data.m_cpNotch.ToPoint());
            return false;
        }

        public void DrawEdge(Graphics dc, ezImgView view, bool bDraw)
        {
            try
            {
                if (m_img == null) return;
                if (m_img.m_bNew) return;
                if (bDraw && m_bDraw) foreach (CPoint cp in m_aEdge) DrawCross(dc, view, Color.GreenYellow, cp, 7);
                if (bDraw) foreach (CPoint cp in m_aEdgeFrame) DrawCross(dc, view, Color.Yellow, cp, 7);
                DrawCross(dc, view, Color.Yellow, m_data.m_cpCenterFrame, 20);
                DrawCross(dc, view, Color.Red, m_data.m_cpCenter, 20);
                DrawCircle(dc, view, Color.Red, m_data.m_cpCenter, m_data.m_dR);
                if (m_data.m_bSetup) DrawCircle(dc, view, Color.Red, m_data.m_cpCenter, m_data.m_dR + m_nOffset);
                //if (m_data.m_bSetup) DrawCircle(dc, view, Color.Red, m_data.m_cpCenter, m_data.m_dR + m_nOffset + m_nRangeEnd);
                if (m_data.m_bSetup) DrawCircle(dc, view, Color.Blue, m_data.m_cpManualCenter, m_data.m_nManualR - (m_data.m_nRange / 2));
                if (m_data.m_bSetup) DrawCircle(dc, view, Color.Blue, m_data.m_cpManualCenter, m_data.m_nManualR + (m_data.m_nRange / 2));
                DrawCross(dc, view, Color.Red, m_data.m_cpNotch, 20);
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

        void DrawCircle(Graphics dc, ezImgView view, Color color, CPoint cpCenter, double dR)
        {
            CPoint cp0 = new CPoint(cpCenter.x + (int)dR, cpCenter.y); 
            CPoint cp1 = new CPoint(); 
            for (int nAngle = 1; nAngle <= 360; nAngle++)
            {
                cp1.x = cpCenter.x + (int)(dR * m_aCos[nAngle]);
                cp1.y = cpCenter.y + (int)(dR * m_aSin[nAngle]);
                view.DrawLine(dc, color, cp0, cp1); 
                cp0 = cp1; 
            }
        }

        public void DrawDie(Graphics dc, ezImgView view, Color color, bool bDraw)
        {
            int x, y;
            if (bDraw && m_bDraw)
            {
                for (y = 0; y < m_data.m_dieMap.m_aDie.GetLength(0); y++)
                {
                    for (x = 0; x < m_data.m_dieMap.m_aDie.GetLength(1); x++)
                    {
                        if (m_data.m_dieMap.m_aDie[y, x] == null) break;
                        if (m_data.m_dieMap.m_aDie[y, x].m_bEnable)
                        {
                            view.DrawLine(dc, color, m_data.m_dieMap.m_aDie[y, x].m_cpLT, m_data.m_dieMap.m_aDie[y, x].m_cpRT);
                            view.DrawLine(dc, color, m_data.m_dieMap.m_aDie[y, x].m_cpRT, m_data.m_dieMap.m_aDie[y, x].m_cpRB);
                            view.DrawLine(dc, color, m_data.m_dieMap.m_aDie[y, x].m_cpRB, m_data.m_dieMap.m_aDie[y, x].m_cpLB);
                            view.DrawLine(dc, color, m_data.m_dieMap.m_aDie[y, x].m_cpLB, m_data.m_dieMap.m_aDie[y, x].m_cpLT);
                            view.DrawString(dc, m_data.m_dieMap.m_aDie[y, x].m_cpIndex.ToString(), new CPoint((m_data.m_dieMap.m_aDie[y, x].m_cpLT.x + m_data.m_dieMap.m_aDie[y, x].m_cpRB.x) / 2, (m_data.m_dieMap.m_aDie[y, x].m_cpLT.y + m_data.m_dieMap.m_aDie[y, x].m_cpRB.y) / 2));
                        }
                        if (m_data.m_dieMap.m_aDie[y, x].m_bEnable && m_data.m_dieMap.m_aDie[y, x].m_aDefect.Count > 0)
                        {
                            view.DrawRectangle(dc, Color.FromArgb(30, 255, 0, 0), m_data.m_dieMap.m_aDie[y, x].m_cpLT, m_data.m_dieMap.m_aDie[y, x].m_cpRT, m_data.m_dieMap.m_aDie[y, x].m_cpRB, m_data.m_dieMap.m_aDie[y, x].m_cpLB);
                        }
                    }
                }
            }
        }

        static readonly object m_csLock = new object();
        public bool Inspect(ezImg img, int nRotateMode)
        {
            double nAngle = 0;
            m_bOffCenter = false;
            m_log.Add(m_id + " - Start Edge Inspection.");
            m_data.m_cpCenter = new CPoint(0, 0);
            ezStopWatch sw = new ezStopWatch();
            m_img = img;
            m_szImg = m_img.m_szImg;
            for (int n = 0; n < Enum.GetNames(typeof(eOrientation)).Length; n++)
            {
                if (m_strOrientation == Enum.GetNames(typeof(eOrientation))[n])
                {
                    m_eOrientation = (eOrientation)Enum.Parse(typeof(eOrientation), m_strOrientation);
                }
            }
            if (m_bDiviaion)//간격사이 검출
            {
                m_aEdge.Clear();
                while (nAngle < 360)
                {
                    FindEdge(nAngle * Math.PI / 180);
                    nAngle += m_data.m_fAnglePeriod;
                }
                FindCenter();
                if (nRotateMode == 0) FindNotch2();
                FindCenter();
                if (nRotateMode == 1)
                {
                    m_data.m_cpNotch = m_data.m_cpCenter - m_data.m_cpDiffNotchAndCenter;
                }
                m_img.m_bNew = false;
            }
            else
            {
                m_data.m_aGV = CalcGV();
                lock (m_csLock)
                {
                    FindEdge(img);
                    FindCenter();
                    if (nRotateMode == 0) FindNotch2();
                    FindCenter();
                    if (nRotateMode == 1)
                    {
                        if (!m_bNotchFind90) m_data.m_cpNotch = m_data.m_cpCenter - m_data.m_cpDiffNotchAndCenter;
                        else FindNotch2();
                    }
                    m_img.m_bNew = false;
                }
            }

            if (nRotateMode == 0)
            {
                m_aEdgeFrame.Clear();
                CalcCenterFrame();

                if (m_cpShift.x > m_cpShiftLimit.x || m_cpShift.y > m_cpShiftLimit.y)
                {
                    m_bOffCenter = true;
                    m_log.Popup("Limit Is Over Between Wafer Center and Frame Center !! " + m_cpShift.ToString());
                }
            }

            m_log.Add(m_id + " - AOI Edge : " + sw.Check().ToString());
            if (m_data.m_cpCenter.x < m_img.m_szImg.x * 0.4 || m_data.m_cpCenter.x > m_img.m_szImg.x * 0.6 
                || m_data.m_cpCenter.y < m_img.m_szImg.y * 0.4 || m_data.m_cpCenter.y > m_img.m_szImg.y * 0.6)
            {
                m_log.Popup("Can not Find Center of Wafer !!");
                return true;
            }
            if ((m_data.m_cpCenter.x + m_data.m_dR) > m_szImg.x - 1 || (m_data.m_cpCenter.y + m_data.m_dR) > m_szImg.y - 1
                || (m_data.m_cpCenter.x - m_data.m_dR) < 0 || (m_data.m_cpCenter.y - m_data.m_dR) < 0)
            {
                m_log.Popup("Wafer Position Is Wrong !!");
                return true;
            }
            return false;
        }

        public void CheckWrongNotchOrientation(int nRotateMode)
        {
            m_data.m_fTheta = GetAngle(m_data.m_cpCenter, m_data.m_cpNotch);
            m_data.m_fTheta = (m_data.m_fTheta + 360) % 360;
            if (nRotateMode == 0)
            {
                switch (m_eOrientation)
                {
                    //Wrong notch Orientation
                    case eOrientation.R0:
                        if ((360 - m_data.m_fTheta) > 15 && (360 - m_data.m_fTheta) < 345)
                        {
                            m_log.Popup("Wrong Notch Orientation (" + (360 - m_data.m_fTheta).ToString("0.0") + ")");
                        }
                        break;
                    case eOrientation.R90:
                    case eOrientation.R180:
                    case eOrientation.R270:
                        if (Math.Abs(((int)m_eOrientation * 90) - (360 - m_data.m_fTheta)) > 15)
                        {
                            m_log.Popup("Wrong Notch Orientation (" + (360 - m_data.m_fTheta).ToString("0.0") + " dgree)");
                        }
                        break;
                }
            }
            m_log.Add("Notch Rotate : " + m_data.m_fTheta.ToString("0.000"));
            //m_data.RelocateDie();
            SetDie();
        }

        int[] CalcGV()
        {
            if ((m_aGV == null) || (m_aGV.Count() < m_szImg.x))
            {
                m_aGV = new int[m_szImg.x]; 
            }

            if (m_nGV > 0)
            {
                for (int x = 0; x < m_szImg.x; x++) m_aGV[x] = m_nGV;
                return m_aGV; 
            }

            CPoint szDown = m_data.m_imgDown.m_szImg;
            int nR = 50; 
            int nGVL = CalcGV(new CPoint(nR, szDown.y / 2 - nR), nR);
            int nGVR = CalcGV(new CPoint(szDown.x - nR, szDown.y / 2 - nR), nR);
            int nGVT = CalcGV(new CPoint(szDown.x / 2 - nR, szDown.y - nR), nR);
            int nGVB = CalcGV(new CPoint(szDown.x / 2 - nR, nR), nR);
            int nGVC = AveGV(szDown);

            m_bWhiteWafer = (nGVC > (nGVT + nGVB) / 2);
            m_data.m_bWhiteWafer = m_bWhiteWafer; 

            int nGVLR = (nGVL + nGVR) / 2;
            int nGVTB = (nGVT + nGVB) / 2;
            int xM = m_szImg.x / 2; 
            for (int x = 0; x <= xM; x++)
            {
                m_aGV[x] = (x * nGVTB + (xM - x) * nGVLR) / xM;
                m_aGV[m_szImg.x - x - 1] = m_aGV[x]; 
            }
            return m_aGV; 
        }

        int AveGV(CPoint szDown)
        {
            int xc = szDown.x / 2;
            int yc = szDown.y / 2;
            double fSum = 0;
            for (int y = -10; y <= 10; y++)
            {
                for (int x = -10; x <= 10; x++)
                {
                    fSum += m_data.m_imgDown.m_aBuf[yc + y, xc + x]; 
                }
            }
            return (int)(fSum / 21 / 21); 
        }

        int CalcGV(CPoint cp0, int nR)
        {
            int[] aHisto = new int[256];
            Array.Clear(aHisto, 0, 256);
            for (int y = cp0.y - nR; y < cp0.y + nR; y++)
            {
                for (int x = cp0.x - nR; x < cp0.x + nR; x++)
                {
                    aHisto[m_data.m_imgDown.m_aBuf[y, x]]++;
                }
            }

            int nGVAve = 0;
            int nGVCount = 0;
            for (int n = 0; n < 250; n++)
            {
                nGVAve += (n * aHisto[n]);
                nGVCount += aHisto[n];
            }
            if (nGVCount == 0) return 100;
            nGVAve /= nGVCount;

            int nHistoMax = 0;
            int nGVMin = 0;
            for (int n = 0; n < nGVAve; n++)
            {
                if (nHistoMax < aHisto[n])
                {
                    nHistoMax = aHisto[n];
                    nGVMin = n;
                }
            }
            nHistoMax = 0;
            int nGVMax = 0;
            for (int n = nGVAve; n < 230; n++)
            {
                if (nHistoMax < aHisto[n])
                {
                    nHistoMax = aHisto[n];
                    nGVMax = n;
                }
            }
            return (nGVMin + nGVMax) / 2;
        }

        #region FindCenter
        double[,] m_XY = new double[4, 4] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
        double[,] m_a = new double[3, 3] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
        double[] m_b = new double[3] { 0, 0, 0 };

        void FindCenter()
        {
            DetectEdge(m_aEdge);
            GaussEllimination(ref m_data.m_cpCenter, ref m_data.m_dR); 
        }

        bool DetectEdge(ArrayList aEdge)
        {
            for (int y = 0; y < 4; y++) for (int x = 0; x < 4; x++) m_XY[x, y] = 0;
            for (int y = 0; y < 3; y++)
            {
                m_b[y] = 0;
                for (int x = 0; x < 3; x++) m_a[x, y] = 0;
            }
            foreach (CPoint cp in aEdge)
            {
                double x = cp.x;
                double y = cp.y;
                m_XY[0, 0]++;
                m_XY[0, 1] += y;
                m_XY[1, 0] += x;
                m_XY[0, 2] += (y * y);
                m_XY[1, 1] += (x * y);
                m_XY[2, 0] += (x * x);
                m_XY[0, 3] += (y * y * y);
                m_XY[1, 2] += (x * y * y);
                m_XY[2, 1] += (x * x * y);
                m_XY[3, 0] += (x * x * x);
            }
            m_a[0, 0] = m_XY[2, 0];
            m_a[1, 1] = m_XY[0, 2];
            m_a[2, 2] = m_XY[0, 0];
            m_a[1, 0] = m_a[0, 1] = m_XY[1, 1];
            m_a[2, 0] = m_a[0, 2] = m_XY[1, 0];
            m_a[2, 1] = m_a[1, 2] = m_XY[0, 1];
            m_b[0] = -m_XY[3, 0] - m_XY[1, 2];
            m_b[1] = -m_XY[2, 1] - m_XY[0, 3];
            m_b[2] = -m_XY[2, 0] - m_XY[0, 2];
            return false;
        }

        bool GaussEllimination(ref CPoint cpCenter, ref double dR)
        {
            for (int k = 0; k < 3; k++)
            {
                if (m_a[k, k] == 0)
                {
                    for (int i = k + 1; i < 3; i++)
                    {
                        if (m_a[i, k] != 0) continue;
                        for (int j = 0; j < 3; j++) Swap(ref m_a[k, j], ref m_a[i, j]);
                        Swap(ref m_b[k], ref m_b[i]);
                        break;
                    }
                }
                if (m_a[k, k] == 0) return true;
                for (int i = k + 1; i < 3; i++)
                {
                    m_a[i, k] /= m_a[k, k];
                    for (int j = k + 1; j < 3; j++) m_a[i, j] -= (m_a[i, k] * m_a[k, j]);
                    m_b[i] -= (m_a[i, k] * m_b[k]);
                }
            }

            double[] dResult = new double[3] { 0, 0, 0 };
            for (int i = 2; i >= 0; i--)
            {
                double dSum = 0;
                for (int j = i + 1; j < 3; j++) dSum += (m_a[i, j] * dResult[j]);
                dResult[i] = (m_b[i] - dSum) / m_a[i, i];
            }
            dResult[0] = -0.5 * dResult[0];
            dResult[1] = -0.5 * dResult[1];
            dResult[2] = Math.Sqrt(dResult[0] * dResult[0] + dResult[1] * dResult[1] - dResult[2]);

            cpCenter.x = (int)dResult[0];
            cpCenter.y = (int)dResult[1];
            dR = dResult[2];

            return false;
        }

        void Swap(ref double a, ref double b)
        {
            double c = a;
            a = b;
            b = c;
        }
        #endregion

        double GetAngle(CPoint cp0, CPoint cp1)
        {
            int dx = cp1.x - cp0.x;
            int dy = cp1.y - cp0.y;
            double rad = Math.Atan2(dx, dy);
            return ((rad * 180 / Math.PI) + 180) * -1; // Because Notch Is 6 o'clock
        }

        void SetDie()
        {
            // ing
            int x, y;
            double fTheta;
            RPoint rp0, rpCenter, rpLT, rpRT, rpLB, rpRB;
            fTheta = m_data.m_fTheta * Math.PI / 180;
            rpCenter = new RPoint(m_data.m_cpCenter);
            if (m_data.m_dieMap.m_aDie == null) return;
            rp0 = rpCenter - new RPoint(m_data.m_dieMap.m_rpPitch.x / m_data.m_fResolution * m_data.m_dieMap.m_cpMap.x / 2, -m_data.m_dieMap.m_rpPitch.y / m_data.m_fResolution * m_data.m_dieMap.m_cpMap.y / 2);
            for (y = 0; y < m_data.m_dieMap.m_aDie.GetLength(0); y++)
            {
                for (x = 0; x < m_data.m_dieMap.m_aDie.GetLength(1); x++)
                {
                    if (m_data.m_dieMap.m_aDie[y, x] == null) break;
                    m_data.m_dieMap.m_aDie[y, x].m_defType = eDefType.None;
                    m_data.m_dieMap.m_aDie[y, x].m_aDefect.Clear();
                    rpLT = rp0 + new RPoint(x * m_data.m_dieMap.m_rpPitch.x / m_data.m_fResolution, y * - (m_data.m_dieMap.m_rpPitch.y/ m_data.m_fResolution));
                    rpRT = rpLT + new RPoint(m_data.m_dieMap.m_rpPitch.x / m_data.m_fResolution, 0);
                    rpLB = rpLT + new RPoint(0, -m_data.m_dieMap.m_rpPitch.y / m_data.m_fResolution);
                    rpRB = rpLT + new RPoint(m_data.m_dieMap.m_rpPitch.x / m_data.m_fResolution, - (m_data.m_dieMap.m_rpPitch.y/ m_data.m_fResolution));
                    rpLT = new RPoint(((rpLT.x - rpCenter.x) * Math.Cos(fTheta)) - ((rpLT.y - rpCenter.y) * Math.Sin(fTheta)) + rpCenter.x, (rpLT.x - rpCenter.x) * Math.Sin(fTheta) + ((rpLT.y - rpCenter.y) * Math.Cos(fTheta)) + rpCenter.y);
                    rpRT = new RPoint(((rpRT.x - rpCenter.x) * Math.Cos(fTheta)) - ((rpRT.y - rpCenter.y) * Math.Sin(fTheta)) + rpCenter.x, (rpRT.x - rpCenter.x) * Math.Sin(fTheta) + ((rpRT.y - rpCenter.y) * Math.Cos(fTheta)) + rpCenter.y);
                    rpLB = new RPoint(((rpLB.x - rpCenter.x) * Math.Cos(fTheta)) - ((rpLB.y - rpCenter.y) * Math.Sin(fTheta)) + rpCenter.x, (rpLB.x - rpCenter.x) * Math.Sin(fTheta) + ((rpLB.y - rpCenter.y) * Math.Cos(fTheta)) + rpCenter.y);
                    rpRB = new RPoint(((rpRB.x - rpCenter.x) * Math.Cos(fTheta)) - ((rpRB.y - rpCenter.y) * Math.Sin(fTheta)) + rpCenter.x, (rpRB.x - rpCenter.x) * Math.Sin(fTheta) + ((rpRB.y - rpCenter.y) * Math.Cos(fTheta)) + rpCenter.y);
                    m_data.m_dieMap.m_aDie[y, x].m_cpLT = rpLT.ToCPoint();
                    m_data.m_dieMap.m_aDie[y, x].m_cpRT = rpRT.ToCPoint();
                    m_data.m_dieMap.m_aDie[y, x].m_cpLB = rpLB.ToCPoint();
                    m_data.m_dieMap.m_aDie[y, x].m_cpRB = rpRB.ToCPoint();
                    // 원을 벗어나는 다이는 사용안함으로
                    //if (m_data.m_cpCenter.GetL(m_data.m_dieMap.m_aDie[y, x].m_cpLT) > m_data.m_dR) m_data.m_dieMap.m_aDie[y, x].m_bEnable = false;
                    //if (m_data.m_cpCenter.GetL(m_data.m_dieMap.m_aDie[y, x].m_cpRT) > m_data.m_dR) m_data.m_dieMap.m_aDie[y, x].m_bEnable = false;
                    //if (m_data.m_cpCenter.GetL(m_data.m_dieMap.m_aDie[y, x].m_cpLB) > m_data.m_dR) m_data.m_dieMap.m_aDie[y, x].m_bEnable = false;
                    //if (m_data.m_cpCenter.GetL(m_data.m_dieMap.m_aDie[y, x].m_cpRB) > m_data.m_dR) m_data.m_dieMap.m_aDie[y, x].m_bEnable = false;
                    m_data.m_dieMap.m_aDie[y, x].m_cpIndex.x = x - (m_data.m_dieMap.m_cpMap.x / 2);
                    m_data.m_dieMap.m_aDie[y, x].m_cpIndex.y = (m_data.m_dieMap.m_cpMap.y / 2) - y;
                }
            }
        }

        public int GetEdge()
        {
            CPoint pt1 = (CPoint)m_aEdge[0];
            int nLimit = pt1.y - m_data.m_cpCenter.y;
            return nLimit;
        }
        public double GetNotchPos()
        {
            return m_data.m_fTheta;
        }

        ArrayList m_aEdgeFrame = new ArrayList();

        void CalcCenterFrame()
        {
            m_data.m_cpCenterFrame.Set(0, 0);
            m_aEdgeFrame.Clear();

            FindEdgeFrame(45, 60, 0.2, 1);
            FindEdgeFrame(300, 315, 0.2, 1);
            FindEdgeFrame(125, 140, 0.2, -1);
            FindEdgeFrame(225, 240, 0.2, -1);

            double dR = 0;
            DetectEdge(m_aEdgeFrame);
            GaussEllimination(ref m_data.m_cpCenterFrame, ref dR);

            m_cpShift = m_data.m_cpCenter - m_data.m_cpCenterFrame;
            m_cpShift *= m_data.m_fResolution;
            m_log.Add("dCenter = " + m_cpShift.ToString());
        }

        int m_nOffset = 800;
        const int m_nRangeEnd = 500;
        int m_nAveRange = 30;
        double[] m_aDiff = new double[m_nRangeEnd];
        void FindEdgeFrame(double angleStart, double angleEnd, double anglePeriod, int nDirectionX)
        {
            double nGVCurrent, nGVNext, dMax;
            int x, y, n, nCount;
            m_aDiff.Initialize();
            for (double angleN = angleStart; angleN < angleEnd; angleN += anglePeriod)
            {
                nGVCurrent = nGVNext = nCount = 0;
                x = (int)(m_data.m_cpCenter.x + ((m_data.m_dR + m_nOffset) * Math.Cos(angleN * Math.PI / 180)));
                y = (int)(m_data.m_cpCenter.y + ((m_data.m_dR + m_nOffset) * Math.Sin(angleN * Math.PI / 180)));
                for (nCount = 0; nCount < m_nRangeEnd; nCount++)
                {
                    for (n = 0; n < m_nAveRange; n++)
                    {
                        nGVCurrent += (int)GetGV(x - (n * nDirectionX), y);
                        nGVNext += (int)GetGV(x + (n * nDirectionX), y);
                    }
                    nGVCurrent /= m_nAveRange;
                    nGVNext /= m_nAveRange;
                    m_aDiff[nCount] = nGVNext - nGVCurrent;
                    if (nDirectionX > 0) x++;
                    else x--;
                }
                dMax = 0;
                for (n = 0; n < m_nRangeEnd; n++)
                {
                    if (dMax < m_aDiff[n]) 
                    {
                        x = (int)(m_data.m_cpCenter.x + ((m_data.m_dR + m_nOffset) * Math.Cos(angleN * Math.PI / 180))) + (n * nDirectionX);
                        dMax = m_aDiff[n];
                    }
                }
                if (IsValidFrame(x, y) && dMax > 0) m_aEdgeFrame.Add(new CPoint(x, y));
            }

        }

        bool IsValidFrame(int x, int y)
        {
            if ((x < 10) || (y < 10)) return false;
            if (x >= m_szImg.x - 10) return false;
            if (y >= m_szImg.y - 10) return false;
            return true;
        }

    }
}
