using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ezAutoMom;
using ezTools;

namespace ezAuto_EFEM
{
    public class HW_Aligner_EBR_AOI
    {
        public string m_id;
        Log m_log;
        public ezImg m_img;

        public CPoint m_szImg = new CPoint(0, 0);

        public bool m_bInspChipping = false;
        public bool m_bSaveEdgeChipping = false;
        public bool m_bFindNotchNoAlign = false;
        public bool m_bTrimNotch = false;
        bool m_bDraw = false;
        public int m_nGV = 100;
        public int m_nTrimGV = 20;
        CPoint m_szNotch = new CPoint(245, 770);

        int m_yMax = 0;
        int m_nChippingSize = 5;
        int m_nMerge = 10;
        public int[] m_aEdge;
        public int[] m_aCircle; // ing 170227 FFT로 찾은 Edge결과 배열 추가 나중에 m_aEdge랑 비교해서 계산하면 됨.
        public int[] m_aChipping;
        public int[] m_aCalc;
        public int[] m_aTrimEdge;
        public int[] m_aCalcTrim;
        double[] m_aNotchSum;
        public ArrayList m_arrChipping = new ArrayList();
        double[] m_aNotchPattern = new double[80];
        double[] m_aDstPattern = new double[80];
        double m_dAveEdge = 0.0;

        public CPoint m_cpNotch = new CPoint(0, 0);
        public CPoint m_cpTrimNotch = new CPoint(0, 0);

        const int c_nFFT = 1024;

        double[] m_aData = new double[c_nFFT];
        double m_fFFT0 = 0;
        public RPoint m_xyFFT = new RPoint();
        public RPoint m_rtFFT = new RPoint();

        double[] m_aCos = new double[c_nFFT];
        double[] m_aSin = new double[c_nFFT];

        ippTools m_ippTools;
        public double m_dDiffNotch = 0.0;

        const int c_nThread = 40;
        Thread[] m_thread = new Thread[c_nThread];
        bool m_bRun = true;
        bool m_bAbsoluteGV = true;
        bool[] m_bRunEdge = new bool[c_nThread];
        int m_xAve = 10;
        int m_nDiff = 20;
        int[,] m_aAve = null;
        int[,] m_aDiff = null;
        double[,] m_aDiffSum = null;
        CPoint m_cpImg = new CPoint();

        public enum eInspect
        {
            WhiteWafer,
            BlackWafer,
        }
        eInspect m_eInspect = eInspect.BlackWafer;
        string m_sInspect = eInspect.BlackWafer.ToString();
        string[] m_sInspects = Enum.GetNames(typeof(eInspect));

        public HW_Aligner_EBR_AOI()
        {
        }

        public void Init(string id, Log log)
        {
            m_id = id;
            m_log = log;
            for (int n = 0; n < c_nFFT; n++)
            {
                m_aCos[n] = Math.Cos(2 * Math.PI * n / c_nFFT);
                m_aSin[n] = Math.Sin(2 * Math.PI * n / c_nFFT);
            }
            for (int n = 0; n < m_aNotchPattern.Length; n++)
            {
                m_log.m_reg.Read("NotchPattern" + n.ToString(), ref m_aNotchPattern[n]);
            }
            m_ippTools = new ippTools();

            for (int n = 0; n < c_nThread; n++)
            {
                m_thread[n] = new Thread(new ParameterizedThreadStart(RunThread));
                m_thread[n].Start(n);
        }
        }

        void InitArray(CPoint cpSize)
        {
            if (m_cpImg == cpSize) return;
            m_cpImg = cpSize;
            m_aAve = new int[m_cpImg.y / 10, m_cpImg.x + 1];
            m_aDiff = new int[m_cpImg.y / 10, m_cpImg.x + 1];
            m_aDiffSum = new double[m_cpImg.y / 10, 4 * m_xAve];
        }

        public void ThreadStop()
        {
            m_bRun = false;
            foreach (Thread thread in m_thread) thread.Join();
        }

        public void Draw(Graphics dc, ezImgView imgView)
        {
            if (m_img == null) return;
            if (m_img.m_bNew) return;
            DrawLines(dc, imgView, Color.Green, m_aCircle);
            DrawLines(dc, imgView, Color.Red, m_aEdge);
            DrawLines(dc, imgView, Color.Blue, m_aCalc);
            //DrawLines(dc, imgView, Color.OrangeRed, m_aTrimEdge);
            //DrawLines(dc, imgView, Color.DarkOrange, m_aCalcTrim);
            imgView.DrawLine(dc, Color.Black, new CPoint(0, m_cpNotch.y), new CPoint(m_cpNotch.x, m_cpNotch.y));
            if (!m_bAbsoluteGV) imgView.DrawLine(dc, Color.White, new CPoint(0, m_cpNotch.y), new CPoint(m_cpNotch.x, m_cpNotch.y));
            imgView.DrawLine(dc, Color.DarkOrange, new CPoint(0, m_cpTrimNotch.y), new CPoint(m_cpTrimNotch.x, m_cpTrimNotch.y));
            if (m_bTrimNotch && m_dDiffNotch > 0.1) imgView.DrawString(dc, m_dDiffNotch.ToString("0.00 um"), new CPoint(m_cpTrimNotch.x / 2, m_cpTrimNotch.y - 5));
            try
            {
                foreach (Defect def in m_arrChipping)
                {
                    if (!m_bDraw) return;
                    if (m_bInspChipping)
                    {
                        imgView.DrawString(dc, "Chipping : " + def.m_nSize, def.m_cp0);
                        imgView.DrawLine(dc, Color.Yellow, new CPoint(def.m_cp0.x, def.m_cp0.y), new CPoint(def.m_cp0.x, def.m_cp1.y));
                        imgView.DrawLine(dc, Color.Yellow, new CPoint(def.m_cp0.x, def.m_cp1.y), new CPoint(def.m_cp1.x, def.m_cp1.y));
                        imgView.DrawLine(dc, Color.Yellow, new CPoint(def.m_cp1.x, def.m_cp1.y), new CPoint(def.m_cp1.x, def.m_cp0.y));
                        imgView.DrawLine(dc, Color.Yellow, new CPoint(def.m_cp1.x, def.m_cp0.y), new CPoint(def.m_cp0.x, def.m_cp0.y));
                    }
                }
            }
            catch (Exception ex)
            {
                m_log.Add(ex.Message);
            }
        }

        void DrawLines(Graphics dc, ezImgView imgView, Color color, int[] aLine)
        {
            CPoint[] cp = new CPoint[2];
            cp[0] = new CPoint(aLine[0], 5);
            cp[1] = new CPoint(0, 0);
            for (int y = 15; y < m_szImg.y; y += 10)
            {
                cp[1].x = aLine[y / 10];
                cp[1].y = y;
                imgView.DrawLine(dc, color, cp[0], cp[1]);
                cp[0] = cp[1];
            }
        }

        public void RunGrid(ezGrid rGrid, eGrid eMode)
        {
            rGrid.Set(ref m_sInspect, m_sInspects, "Inpect", "Mode", "Inspect Mode");
            rGrid.Set(ref m_bAbsoluteGV, "Inpect", "AbsoluteGV", "Use Inspection Mode (true = AbsoluteGV, false = RelativeGV)");
            rGrid.Set(ref m_szNotch, "Notch", "Size", "Notch Size (Pixel)");
            rGrid.Set(ref m_bUsePattern, "Notch", "Pattern", "Use Pattern Matching");
            rGrid.Set(ref m_nMerge, "Chipping", "Merge", "Distance of Defect (Pixel)");
            for (int n = 0; n < m_sInspects.Length; n++)
            {
                if (m_sInspect == ((eInspect)n).ToString()) m_eInspect = (eInspect)n;
            }
        }

        public void RecipeGrid(ezGrid rGrid, eGrid eMode, ezJob job, bool bChipping = true, bool bEdge = true, bool bNotchImg = true)
        {
            rGrid.Set(ref m_nGV, "Align", "GV", "Edge Find GV (0 ~ 255)");
            rGrid.Set(ref m_nDiff, "Align", "Variation", "Variation Of GV Wafer Edge");
            if (bChipping)
            {
                rGrid.Set(ref m_bInspChipping, "Align", "InspChipping", "Use Chipping Inspection");
                rGrid.Set(ref m_nChippingSize, "Align", "ChippingSize", "Chipping Size (Pixel)");
            }
            if (bEdge) rGrid.Set(ref m_bSaveEdgeChipping, "Align", "EdgeImg", "Save Edge Chipping Image");
            if (bNotchImg) rGrid.Set(ref m_bFindNotchNoAlign, "Align", "GetImg", "Get Image Without Align");
        }

        void ReAllocate(CPoint szImg)
        {
            if (szImg == m_szImg) return;
            m_szImg = szImg;
            m_yMax = m_szImg.y / 10;
            m_aEdge = new int[m_yMax + 1];
            m_aCalc = new int[m_yMax + 1];
            m_aCircle = new int[m_yMax + 1];
            m_aChipping = new int[m_yMax + 1];
            Array.Clear(m_aCircle, 0, m_yMax);
            m_aTrimEdge = new int[m_yMax + 1];
            m_aCalcTrim = new int[m_yMax + 1];
        }

        public int Inspect(ezImg img, double dResolution = 3.5)
        {
            ezStopWatch sw = new ezStopWatch();
            m_img = img;
            m_cpNotch = new CPoint(0, 0);
            m_bDraw = false;
            m_arrChipping.Clear();
            ReAllocate(m_img.m_szImg);
            if (m_bAbsoluteGV) for (int y = 0; y < m_szImg.y; y += 10) m_aEdge[y / 10] = FindEdge(y);
            else
            {
                InitArray(img.m_szImg);
                for (int n = 0; n < c_nThread; n++) m_bRunEdge[n] = true;
                int nRun = c_nThread;
                while (nRun > 0)
                {
                    Thread.Sleep(1);
                    nRun = 0;
                    for (int n = 0; n < c_nThread; n++) if (m_bRunEdge[n]) nRun++;
                }
            }
            m_log.Add("Find Edge : " + sw.Check() + " ms");
            CalcEdge();
            CalcShift();
            CalcCircle();
            FindNotch();
            double nTotal = 0;
            int nCount = 0;
            for (int n = 0; n < m_aEdge.Length - 1; n++)
            {
                if (IsNotch(new CPoint(0, n * 10))) continue;
                nTotal += m_aEdge[n];
                nCount++;
            }
            m_dAveEdge = nTotal / nCount;

            if (m_bInspChipping)
            {
                MergeChipping(); 
                RepositionDefect();
                m_log.Add("Chipping Count : " + m_arrChipping.Count);
            }
            m_dDiffNotch = 0.0;
            if (m_bTrimNotch)
            {
                for (int y = 0; y < m_szImg.y; y += 10) m_aTrimEdge[y / 10] = FindTrimEdge(y);
                CalcTrimEdge();
                if (FindTrimNotch())
                {
                    m_log.Add("Can not Find Trim Notch !!");
                    return 0;
                }
                m_dDiffNotch = Math.Abs(m_cpTrimNotch.y - m_cpNotch.y) * dResolution;
                m_log.Add("Notch Diff : " + m_dDiffNotch.ToString("0.00 um"));
            }
            m_img.m_bNew = false;
            m_log.Add("Align Edge Inspect : " + sw.Check().ToString());
            m_bDraw = true;
            return 0;
        }

        public double GetOffsetAngle(double dAngle)
        {
            double dOffset = 0.0;
            int nIndex = 0;
            int nArrIndexNotch = m_cpNotch.y / 10;
            int nArrIndexEndPoint = (int)(m_cpNotch.y + (dAngle * m_img.m_szImg.y / 360)) / 10;
            for (int n = nArrIndexNotch; n < nArrIndexEndPoint; n++)
            {
                nIndex = n;
                if (IsNotch(new CPoint(0, n * 10))) nIndex = (m_cpNotch.y / 10) + (m_szNotch.y / 10);
                nIndex = nIndex % (m_aEdge.Length - 1);
                dOffset += ((double)(m_dAveEdge - m_aEdge[nIndex]) * 2 * Math.PI / (m_aEdge.Length - 1));
            }
            m_log.Add(dAngle.ToString() + " Dgree Offset : " + dOffset.ToString("0.000"));
            return dOffset;
        }

        void RunThread(object obj)
        {
            int nIndex = (int)obj;
            Thread.Sleep(5000);
            while (m_bRun)
            {
                Thread.Sleep(1);
                int nPeriod = m_cpImg.y / c_nThread;
                if (m_bRunEdge[nIndex])
                {
                    for (int y = nIndex * nPeriod; y < m_cpImg.y - (c_nThread - nIndex - 1) * nPeriod; y += 10)
                    {
                        CalcAve(y);
                        m_aEdge[y / 10] = (int)FindEdgeBlur(y / 10);
                    }
                    m_bRunEdge[nIndex] = false;
                }
            }
        }

        public int FindEdge(int y)
        {
            if (m_eInspect == eInspect.WhiteWafer)
            {
                for (int x = m_szImg.x - 10; x > 10; x--)
                {
                    if (m_img.m_aBuf[y, x] >= m_nGV) return x;
                } 
                return 0;
            }
            for (int x = m_szImg.x - 10; x > 10; x--)
            {
                if (m_img.m_aBuf[y, x] < m_nGV) return x;
            }
            return 0;
        }

        int FindTrimEdge(int y)
        {
            for (int x = m_aEdge[y / 10] - 15; x > 10; x--)
            {
                if (m_img.m_aBuf[y, x] > m_nTrimGV) return x;
            }
            return 0;
        }

        int FindBlurNotchX(int y)
        {
            int[] aAve = new int[m_cpImg.x];
            int[] aDiff = new int[m_cpImg.x];
            int[] aDiffSum = new int[4 * m_xAve];
            for (int x = m_xAve; x < m_cpImg.x - m_xAve; x++)
            {
                int x0 = x - m_xAve;
                if (x0 < 0) x0 = 0;
                int x1 = x + m_xAve;
                if (x1 >= m_cpImg.x) x1 = m_cpImg.x - 1;
                int nSum = 0;
                for (int ix = x0; ix <= x1; ix++) nSum += m_img.m_aBuf[y, ix];
                aAve[x] = nSum / (x1 - x0 + 1);
            }
            Array.Clear(m_aDiff, m_cpImg.x * y / 10, m_cpImg.x);
            for (int x = m_xAve; x < m_cpImg.x - m_xAve; x++)
            {
                aDiff[x] = aAve[x + m_xAve] - aAve[x - m_xAve];
            }

            int xp = 0;
            int vMid = m_nDiff;
            int xMax = xp;
            int vMax = 0;
            while (aDiff[xp] < vMid && xp < m_cpImg.x - m_xAve) xp++;
            while (aDiff[xp] >= vMid && xp < m_cpImg.x - m_xAve)
            {
                if (vMax < aDiff[xp])
                {
                    vMax = aDiff[xp];
                    xMax = xp;
                }
                xp++;
            }
            if (xMax < m_xAve) return 0;
            return xMax;
        }

        double FindEdgeBlur(int y)
        {
            int xp = 0;
            int vMid = m_nDiff;
            int xMax = xp;
            int vMax = 0;
            while (m_aDiff[y, xp] < vMid && xp < m_cpImg.x - m_xAve) xp++;
            while (m_aDiff[y, xp] >= vMid && xp < m_cpImg.x - m_xAve)
            {
                if (vMax < m_aDiff[y, xp])
                {
                    vMax = m_aDiff[y, xp];
                    xMax = xp;
                }
                xp++;
            }
            return FindEdge2(y, xMax); 
        }

        void CalcAve(int y)
        {
            for (int x = m_xAve; x < m_cpImg.x - m_xAve; x++)
            {
                int x0 = x - m_xAve;
                if (x0 < 0) x0 = 0;
                int x1 = x + m_xAve;
                if (x1 >= m_cpImg.x) x1 = m_cpImg.x - 1;
                int nSum = 0;
                for (int ix = x0; ix <= x1; ix++) nSum += m_img.m_aBuf[y, ix];
                m_aAve[y / 10, x] = nSum / (x1 - x0 + 1);
            }
            Array.Clear(m_aDiff, m_cpImg.x * y / 10, m_cpImg.x);
            for (int x = m_xAve; x < m_cpImg.x - m_xAve; x++)
            {
                m_aDiff[y / 10, x] = m_aAve[y / 10, x + m_xAve] - m_aAve[y / 10, x - m_xAve];
            }
        }


        double FindEdge2(int y, int xMax)
        {
            if (xMax < m_xAve) return 0.0; // ing 170612
            if ((m_aDiffSum == null) || (m_aDiffSum.Length < 2 * m_xAve)) Array.Clear(m_aDiffSum, y * 4 * m_xAve, 4 * m_xAve);
            for (int x = xMax - m_xAve, ix = 0; x <= xMax + m_xAve; x++, ix++) m_aDiffSum[y, ix] = FindEdgeSum(y, x);
            for (int x = xMax - m_xAve, ix = 0; x <= xMax + m_xAve; x++, ix++)
            {
                if (m_aDiffSum[y, ix] < 0 && ix > 0)
                {
                    if (m_aDiffSum[y, ix - 1] == m_aDiffSum[y, ix]) return 1;
                    double dx = m_aDiffSum[y, ix - 1] / (m_aDiffSum[y, ix - 1] - m_aDiffSum[y, ix]);
                    double fxMax = x - 1 + dx;
                    xMax = (int)Math.Round(fxMax);
                    for (int xp = xMax - 2 * m_xAve; xp <= xMax + 2 * m_xAve; xp++)
                    {
                        if (xp <= 0) xp = 1;
                        m_aDiff[y, xp] = 0;
                        m_aDiff[y, xp - 1] = 0;
                    }
                    return fxMax;
                }
            }
            return xMax;
        }

        double FindEdgeSum(int y, int xp)
        {
            double fSum = 0;
            if (xp < m_xAve) return 0;
            for (int x = xp - m_xAve; x < xp; x++) fSum -= m_aDiff[y, x];
            for (int x = xp + 1; x <= xp + m_xAve; x++) fSum += m_aDiff[y, x];
            return fSum;
        }

        void CalcEdge()
        {
            int yNotch = m_szNotch.y / 20;
            for (int y = 0; y < m_yMax; y++)
            {
                m_aCalc[y] = 100 - 2 * m_aEdge[y] + m_aEdge[(y - yNotch + m_yMax) % m_yMax] + m_aEdge[(y + yNotch) % m_yMax];
                if (m_aCalc[y] < 0) m_aCalc[y] = 0;
            }
        }

        void CalcTrimEdge()
        {
            int yNotch = m_szNotch.y / 20;
            for (int y = 0; y < m_yMax; y++)
            {
                m_aCalcTrim[y] = 100 - 2 * m_aTrimEdge[y] + m_aTrimEdge[(y - yNotch + m_yMax) % m_yMax] + m_aTrimEdge[(y + yNotch) % m_yMax];
                if (m_aCalcTrim[y] < 0) m_aCalcTrim[y] = 0;
            }
        }

        void FindNotch()
        {
            int xMax = 0;
            int y0 = 0;
            int y1 = 0;
            for (int y = 0; y < m_yMax; y++)
            {
                if (IsNotch(new CPoint(0, y * 10)) || m_cpNotch.y == 0)
                {
                    if (xMax < m_aCalc[y])
                    {
                        xMax = m_aCalc[y];
                        y0 = y;
                    }
                    if (xMax <= m_aCalc[y])
                    {
                        y1 = y;
                    }
                }
            }
            m_cpNotch.y = FindNotch((y0 + y1) / 2);
            if (m_bAbsoluteGV) m_cpNotch.x = FindEdge(GetY(m_cpNotch.y));
            else m_cpNotch.X = (int)FindBlurNotchX(GetY(m_cpNotch.y));
            m_log.Add("Notch : " + m_cpNotch.ToString());
        }

        bool FindTrimNotch()
        {
            int xMax = 0;
            int y0 = 0;
            int y1 = 0;
            for (int y = 0; y < m_yMax; y++)
            {
                if (IsNotch(new CPoint(0, y * 10)))
                {
                    if (xMax < m_aCalcTrim[y])
                    {
                        xMax = m_aCalcTrim[y];
                        y0 = y;
                    }
                    if (xMax <= m_aCalcTrim[y])
                    {
                        y1 = y;
                    }
                }
            }
            m_cpTrimNotch.y = FindTrimNotch((y0 + y1) / 2);
            if (m_cpTrimNotch.y < 0 || m_cpTrimNotch.y > m_img.m_szImg.y) return true;
            m_cpTrimNotch.x = FindTrimEdge(GetY(m_cpTrimNotch.y));
            m_log.Add("Trim Notch : " + m_cpTrimNotch.ToString());
            return false;
        }

        int FindNotch(int y0)
        {
            int tempY = 0;
            int yNotch = m_szNotch.y / 40;
            if ((m_aNotchSum == null) || (m_aNotchSum.Length < yNotch)) m_aNotchSum = new double[m_szNotch.y];
            for (int y = y0 - yNotch, iy = 0; y <= y0 + yNotch; y++, iy++)
            {
                tempY = y;
                if (tempY < 0) tempY += m_yMax;
                if (tempY > m_yMax - 1) tempY -= m_yMax;
                m_aNotchSum[iy] = NotchSum(tempY, yNotch);
            }
            for (int y = y0 - yNotch, iy = 0; y <= y0 + yNotch; y++, iy++)
            {
                tempY = y;
                if (tempY < 0) tempY += m_yMax;
                if (tempY > m_yMax - 1) tempY -= m_yMax;
                if (m_aNotchSum[iy] < 0 && iy > 0)
                {
                    double fl = m_aNotchSum[iy - 1];
                    double fr = m_aNotchSum[iy];
                    double dy = m_aNotchSum[iy - 1] / (m_aNotchSum[iy - 1] - m_aNotchSum[iy]);
                    return (int)(10 * (tempY - 1 + dy) + 5);
                }
            }
            return 10 * y0 + 5;
        }

        double NotchSum(int y0, int yNotch)
        {
            int tempY = 0;
            double fSum = 0;
            if (y0 - yNotch < 0) return 0;
            for (int y = y0 - yNotch; y < y0; y++)
            {
                tempY = y;
                if (tempY < 0) tempY += m_yMax;
                if (tempY > m_yMax - 1) tempY -= m_yMax;
                fSum -= m_aCalc[tempY];
            }
            for (int y = y0 + 1; y <= y0 + yNotch; y++)
            {
                tempY = y;
                if (tempY < 0) tempY += m_yMax;
                if (tempY > m_yMax - 1) tempY -= m_yMax;
                fSum += m_aCalc[tempY];
            }
            return fSum;
        }

        int FindTrimNotch(int y0)
        {
            int tempY = 0;
            int yNotch = m_szNotch.y / 40;
            if ((m_aNotchSum == null) || (m_aNotchSum.Length < yNotch)) m_aNotchSum = new double[m_szNotch.y];
            for (int y = y0 - yNotch, iy = 0; y <= y0 + yNotch; y++, iy++)
            {
                tempY = y;
                if (tempY < 0) tempY += m_yMax;
                if (tempY > m_yMax - 1) tempY -= m_yMax;
                m_aNotchSum[iy] = TrimNotchSum(tempY, yNotch);
            }
            for (int y = y0 - yNotch, iy = 0; y <= y0 + yNotch; y++, iy++)
            {
                tempY = y;
                if (tempY < 0) tempY += m_yMax;
                if (tempY > m_yMax - 1) tempY -= m_yMax;
                if (m_aNotchSum[iy] < 0 && iy > 0)
                {
                    double fl = m_aNotchSum[iy - 1];
                    double fr = m_aNotchSum[iy];
                    double dy = m_aNotchSum[iy - 1] / (m_aNotchSum[iy - 1] - m_aNotchSum[iy]);
                    return (int)(10 * (tempY - 1 + dy) + 5);
                }
            }
            return 10 * y0 + 5;
        }

        double TrimNotchSum(int y0, int yNotch)
        {
            int tempY = 0;
            double fSum = 0;
            if (y0 - yNotch < 0) return 0;
            for (int y = y0 - yNotch; y < y0; y++)
            {
                tempY = y;
                if (tempY < 0) tempY += m_aTrimEdge.Length;
                if (tempY > m_aTrimEdge.Length - 1) tempY -= m_aTrimEdge.Length;
                fSum -= m_aCalcTrim[tempY];
            }
            for (int y = y0 + 1; y <= y0 + yNotch; y++)
            {
                tempY = y;
                if (tempY < 0) tempY += m_aTrimEdge.Length;
                if (tempY > m_aTrimEdge.Length - 1) tempY -= m_aTrimEdge.Length;
                fSum += m_aCalcTrim[tempY];
            }
            return fSum;
        }

        bool m_bUsePattern = true;
        void MergeChipping()
        {
            int n, nMax = 0;
            double dMax = 0.0;
            if (m_arrChipping.Count < 2) return;
            for (n = 1; n < m_arrChipping.Count; n++)
            {
                if (n < 1) n = 1;
                if (Math.Abs(((Defect)m_arrChipping[n - 1]).m_cp1.y - ((Defect)m_arrChipping[n]).m_cp0.y) < m_nMerge)
                {
                    ((Defect)m_arrChipping[n - 1]).m_nSize += ((Defect)m_arrChipping[n]).m_nSize;
                    if (((Defect)m_arrChipping[n - 1]).m_cp0.x > ((Defect)m_arrChipping[n]).m_cp0.x) ((Defect)m_arrChipping[n - 1]).m_cp0.x = ((Defect)m_arrChipping[n]).m_cp0.x;
                    if (((Defect)m_arrChipping[n - 1]).m_cp1.x < ((Defect)m_arrChipping[n]).m_cp1.x) ((Defect)m_arrChipping[n - 1]).m_cp1.x = ((Defect)m_arrChipping[n]).m_cp1.x;
                    ((Defect)m_arrChipping[n - 1]).m_cp1.y = ((Defect)m_arrChipping[n]).m_cp1.y;
                    ((Defect)m_arrChipping[n - 1]).m_fAngle = (m_cpNotch.y - ((((Defect)m_arrChipping[n - 1]).m_cp0.y + ((Defect)m_arrChipping[n - 1]).m_cp1.y) / 2 * 10 + m_szImg.y) % m_szImg.y) * 360.0 / m_szImg.y; // ing 170426
                    m_arrChipping.RemoveAt(n);
                    n--;
                }
            }
            if (m_bUsePattern)
            {
                for (n = 0; n < m_arrChipping.Count; n++)
                {
                    int nCenter;
                    double dScore;
                    nCenter = (((Defect)m_arrChipping[n]).m_cp0.y + ((Defect)m_arrChipping[n]).m_cp1.y) / 2;
                    for (int index = 0; index < m_aDstPattern.Length - 1; index++)
                    {
                        m_aDstPattern[index] = m_aCalc[(nCenter - (m_aDstPattern.Length / 2) + m_yMax + index) % m_yMax];
                    }
                    dScore = m_ippTools.ippiCrossCorrSame_NormLevel(m_aDstPattern, m_aNotchPattern, new CPoint(m_aDstPattern.Length, 1), new CPoint(m_aNotchPattern.Length, 1));
                    if (dMax < dScore)
                    {
                        dMax = dScore;
                        nMax = n;
                    }
                }
                if (m_arrChipping.Count > 0) 
                {
                    m_cpNotch.y = (((Defect)m_arrChipping[nMax]).m_cp0.y + ((Defect)m_arrChipping[nMax]).m_cp1.y) / 2 * 10;
                    m_cpNotch.x = ((Defect)m_arrChipping[nMax]).m_cp0.x;
                    m_log.Add("First Notch Search : " + m_cpNotch.ToString() + ", Score : " + dMax.ToString("0.00"));
                }
            }
        }

        public double GetNotchAngle()
        {
            double radNotch = Math.PI * m_cpNotch.y / m_szImg.y;
            double dx = m_xyFFT.x * Math.Cos(radNotch) - m_xyFFT.y * Math.Sin(radNotch);
            double dy = m_xyFFT.y * Math.Cos(radNotch) + m_xyFFT.x * Math.Sin(radNotch);
            return 360.0 * (m_cpNotch.y + dx) / m_szImg.y;
//            return 360.0 * m_cpNotch.y / m_szImg.y;
        }

        int GetY(int y)
        {
            if (y < 0) return y + m_szImg.y;
            if (y >= m_szImg.y) return y - m_szImg.y;
            return y;
        }

        void CalcShift() 
        {
            int iMax = 0;
            int xMax = 0;
            for (int n = 0; n < m_yMax; n++)
            {
                if (xMax < m_aEdge[n])
                {
                    xMax = m_aEdge[n];
                    iMax = n;
                }
            }

            for (int n = 0; n < c_nFFT; n++) m_aData[n] = GetShiftEdge(n);

            double dmin, dmax;
            dmin = dmax = m_aData[0]; // BHJ 190315 add
            for (int x = 0; x < c_nFFT; x++)
            {
                m_fFFT0 += m_aData[x];
                m_xyFFT.x += m_aData[x] * m_aCos[x];
                m_xyFFT.y += -m_aData[x] * m_aSin[x];

                // min max calc 
                if (dmin > m_aData[x]) dmin = m_aData[x];
                if (dmax < m_aData[x]) dmax = m_aData[x];
            }
            m_fFFT0 /= c_nFFT;
            m_xyFFT.x /= c_nFFT;
            m_xyFFT.y /= c_nFFT;
            m_rtFFT.x = Math.Sqrt(m_xyFFT.x * m_xyFFT.x + m_xyFFT.y * m_xyFFT.y);
            m_rtFFT.y = 180 * Math.Atan2(m_xyFFT.y, m_xyFFT.x) / Math.PI;
            m_log.Add(string.Format("min = #{0:0.00}, max = #{1:0.00}, diff = #{2:0.00}", dmin, dmax, dmax - dmin),false); // 그래프상 최대 오차, 실제 이미지와 비율 존재 하는지?
            m_log.Add(string.Format("xyFFT = #{0:0.00}, rtFFT = #{1:0.00}", m_xyFFT, m_rtFFT));
            m_log.Add(string.Format("FFT0 = #{0:0.00}, MAX index = #{1}", m_fFFT0, (iMax * 360.0) / m_yMax));
            //m_log.Add("xyFFT = " + m_xyFFT.ToString() + ", rtFFT = " + m_rtFFT.ToString());
            //m_log.Add("FFT0 = " + m_fFFT0.ToString() + ", Max = " + ((iMax * 360.0) / m_yMax).ToString());
        }

        void CalcCircle()
        {
            for (int y = 0; y < m_yMax; y++)
            {
                m_aCircle[y] = (int)(m_fFFT0 + 2 * m_rtFFT.x * Math.Cos(Math.PI * (2.0 * y / m_yMax + m_rtFFT.y / 180)));
                if (!m_bInspChipping) continue;
                m_aChipping[y] = m_aCircle[y] - m_aEdge[y];
                if (y == 11790) y = 11790;
                if (m_aChipping[y] > m_nChippingSize && m_nChippingSize > 0)
                {
                    Defect def = new Defect();
                    def.m_cp0.x = m_aEdge[y];
                    def.m_cp1.x = m_aCircle[y];
                    def.m_cp0.y = def.m_cp1.y = y;
                    def.m_nSize = m_aChipping[y];
                    if (def.m_cp0.x > def.m_cp1.x)
                    {
                        def.m_cp1.x = m_aEdge[y];
                        def.m_cp0.x = m_aCircle[y];
                    }
                    m_arrChipping.Add(def);
                }
            }
        }

        double GetShiftEdge(int n)
        {
            double a = n * m_szImg.y / 10.0 / c_nFFT;
            int i = (int)a;
            return m_aEdge[i] + (a - i) * (m_aEdge[(i + 1) % m_yMax] - m_aEdge[i]);
        }

        public void RepositionDefect()
        {
            int n;
            if (m_arrChipping.Count < 1) return;
            for (n = 0; n < m_arrChipping.Count; n++)
            {
                if (n < 0) n = 0;
                if (((Defect)m_arrChipping[n]).m_cp1.y - ((Defect)m_arrChipping[n]).m_cp0.y < 2)
                {
                    m_arrChipping.RemoveAt(n);
                    n--;                    
                    if (m_arrChipping.Count < 1) return;
                    continue;
                }
                else
                {
                    ((Defect)m_arrChipping[n]).m_cp0.y *= 10;
                    ((Defect)m_arrChipping[n]).m_cp1.y *= 10;
                    ((Defect)m_arrChipping[n]).m_nSize *= 10;
                    m_log.Add("Chipping[" + n.ToString() + "] Position : " + ((Defect)m_arrChipping[n]).m_cp0.y.ToString());
                }
                if (IsNotch((Defect)m_arrChipping[n])) // ing test
                {
                    m_arrChipping.RemoveAt(n);
                    n--;
                    if (m_arrChipping.Count < 1) return;
                    continue;
                }
            }
        }

        public bool IsNotch(Defect def)
        {
            if (Math.Abs(def.m_cp1.y + def.m_cp0.y) / 2 < m_cpNotch.y + m_szNotch.y &&
                Math.Abs(def.m_cp1.y + def.m_cp0.y) / 2 > m_cpNotch.y - m_szNotch.y)
            {
                return true;
            }
            return false;
        }

        public bool IsNotch(CPoint cp)
        {
            if (cp.y < m_cpNotch.y + m_szNotch.y && cp.y > m_cpNotch.y - m_szNotch.y)
            {
                return true;
            }
            return false;
        }

        public void SetupNotchPattern() // ing 170302
        {
            int n;
            for (n = 0; n < m_aNotchPattern.Length; n++)
            {
                m_aNotchPattern[n] = m_aCalc[((m_cpNotch.y / 10) + m_yMax - (m_aNotchPattern.Length / 2) + n) % m_yMax];
                m_log.m_reg.Write("NotchPattern" + n.ToString(), m_aNotchPattern[n]);         
            }
        }

        public void SetupNotchDiff() // legacy bhj commented : HW_Aligner_EBRFix.RunAlignSetup()
        {
            int n;
            for (n = 0; n < m_aNotchPattern.Length; n++)
            {
                m_aNotchPattern[n] = m_aCircle[((m_cpNotch.y / 10) + m_yMax - (m_aNotchPattern.Length / 2) + n) % m_yMax] - m_aCalc[((m_cpNotch.y / 10) + m_yMax - (m_aNotchPattern.Length / 2) + n) % m_yMax];
                m_log.m_reg.Write("NotchPattern" + n.ToString(), m_aNotchPattern[n]);
            }
        }
    }
}
