using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ezTools;

namespace ezAuto_EFEM
{
    class HW_BackSide_ATI_AOI_WhiteBalance
    {
        public string m_id;
        int m_nRange = 7;
        int[,] m_aCount = null;
        int[,] m_aSum = null;
        double[,] m_aSum2 = null;
        int[] m_aAve = null;
        double[] m_aSigma = null;
        double[] m_aaAve = null;
        double[] m_aaSigma = null;
        CPoint m_szImg = new CPoint();
        double m_dR = 0;
        Log m_log;
        ezImg m_img;
        HW_BackSide_ATI_AOIData m_data;

        const int c_nThread = 48;
        bool m_bRun = false;
        bool[] m_bRunAve = new bool[c_nThread];
        bool[] m_bRunBalance = new bool[c_nThread];
        Thread[] m_thread = new Thread[c_nThread];

        double[,] m_aHisto = new double[c_nThread, 256];

        public HW_BackSide_ATI_AOI_WhiteBalance()
        {
        }

        public void Init(string id, HW_BackSide_ATI_AOIData data, Log log)
        {
            m_id = id;
            m_data = data;
            m_log = log;
            m_bRun = true;
            for (int n = 0; n < c_nThread; n++)
            {
                m_bRunAve[n] = false;
                m_bRunBalance[n] = false;
                m_thread[n] = new Thread(new ParameterizedThreadStart(RunThread));
                m_thread[n].Start(n);
            }
        }

        public void ThreadStop()
        {
            m_bRun = false;
            foreach (Thread thread in m_thread) thread.Join();
        }

        public void RunGrid(ezGrid grid, eGrid eMode, ezJob job = null)
        {
            grid.Set(ref m_nRange, m_id, "Ave", "Average Range (1 ~ )");
        }

        void RunThread(object obj)
        {
            int nIndex = (int)obj;
            Thread.Sleep(5000);
            while (m_bRun)
            {
                Thread.Sleep(10);
                if (m_bRunAve[nIndex])
                {
                    for (int y = nIndex; y < m_szImg.y; y += c_nThread) CalcAve(nIndex, y);
                    m_bRunAve[nIndex] = false;
                }
                if (m_bRunBalance[nIndex])
                {
                    for (int y = nIndex; y < m_szImg.y; y += c_nThread) WhiteBalance(nIndex, y);
                    m_bRunBalance[nIndex] = false;
                }
            }
        }

        public bool Inspect(ezImg img, double dR)
        {
            ezStopWatch sw = new ezStopWatch();
            m_img = img;
            m_dR = dR;
            m_data.CalcEdge(dR);
            if (!m_img.HasImage()) return true;
            m_log.Add(m_id + " - Start WhiteBalance Inspection.");
            ReAllocate(m_img.m_szImg);
            CalcAve();
            CalcDiff();
            if (m_nRange == 0) return false;
            WhiteBalance();
            m_log.Add(m_id + " - AOI White balance : " + sw.Check().ToString());
            return false;
        }

        void ReAllocate(CPoint sz)
        {
            if (m_szImg == sz) return; m_szImg = sz;
            m_aSum = new int[c_nThread, sz.x];
            m_aSum2 = new double[c_nThread, sz.x];
            m_aCount = new int[c_nThread, sz.x];
            m_aAve = new int[sz.x];
            m_aSigma = new double[sz.x];
            m_aaAve = new double[sz.x];
            m_aaSigma = new double[sz.x];
        }

        void CalcAve()
        {
            Array.Clear(m_aSum, 0, c_nThread * m_szImg.x);
            Array.Clear(m_aSum2, 0, c_nThread * m_szImg.x);
            Array.Clear(m_aCount, 0, c_nThread * m_szImg.x);

            for (int n = 0; n < c_nThread; n++) m_bRunAve[n] = true;
            Thread.Sleep(10);
            int nRun = c_nThread;
            while (nRun > 0)
            {
                Thread.Sleep(1);
                nRun = 0;
                for (int n = 0; n < c_nThread; n++) if (m_bRunAve[n]) nRun++;
            }
        }

        void CalcAve(int nIndex, int y)
        {
            double dR2 = m_dR * m_dR;
            m_data.m_aEdge[y].Min = 0;
            m_data.m_aEdge[y].Max = -1;
            double dY = y - m_data.m_cpCenter.y;
            double dX2 = dR2 - dY * dY;
            if (dX2 > 1)
            {
                int dX = (int)Math.Sqrt(dX2);
                m_data.m_aEdge[y].Min = m_data.m_cpCenter.x - dX;
                m_data.m_aEdge[y].Max = m_data.m_cpCenter.x + dX;
                if (m_data.m_aEdge[y].Min < 0 || m_data.m_aEdge[y].Max > m_img.m_szImg.x - 1) return; // ing 161123
                for (int x = m_data.m_aEdge[y].Min; x <= m_data.m_aEdge[y].Max; x++)
                {
                    byte v = m_img.m_aBuf[y, x];
                    m_aSum[nIndex, x] += v;
                    m_aSum2[nIndex, x] += (v * v);
                    m_aCount[nIndex, x]++;
                }
            }
        }

        void CalcDiff()
        {
            for (int n = 1; n < c_nThread; n++)
            {
                for (int x = 0; x < m_szImg.x; x++)
                {
                    m_aSum[0, x] += m_aSum[n, x];
                    m_aSum2[0, x] += m_aSum2[n, x];
                    m_aCount[0, x] += m_aCount[n, x];
                }
            }
            for (int x = 0; x < m_szImg.x; x++)
            {
                m_aAve[x] = 0;
                m_aSigma[x] = 0;
                if (m_aCount[0, x] != 0)
                {
                    m_aAve[x] = m_aSum[0, x] / m_aCount[0, x];
                    m_aSigma[x] = Math.Sqrt(m_aSum2[0, x] / m_aCount[0, x] - m_aAve[x] * m_aAve[x]);
                }
            }

            int nCount = 0;
            double fSum = 0;
            double fSigma = 0;
            for (int x = 0; x < m_szImg.x; x++)
            {
                if (m_aAve[x] > 0)
                {
                    nCount++;
                    fSum += m_aAve[x];
                    fSigma += m_aSigma[x];
                }
            }
            m_data.m_gvAve = fSum / nCount;
            m_data.m_gvSigma = fSigma / nCount;

            int xl = 0;
            while (m_aAve[xl] == 0)
            {
                xl++;
                if (xl >= m_aAve.Length - 1) break; // ing 161212
            }
            for (int n = 1; n <= m_nRange; n++)
            {
                m_aAve[xl - n] = m_aAve[xl];
                m_aSigma[xl - n] = m_aSigma[xl];
            }
            int xr = m_szImg.x - 1;
            while (m_aAve[xr] == 0) xr--;
            for (int n = 1; n <= m_nRange; n++)
            {
                m_aAve[xr + n] = m_aAve[xr];
                m_aSigma[xr + n] = m_aSigma[xr];
            }

            fSum = 0;
            fSigma = 0;
            for (int x = xl - m_nRange; x < xl + m_nRange; x++)
            {
                fSum += m_aAve[x];
                fSigma += m_aSigma[x];
            }
            int nTotal = 2 * m_nRange + 1;
            for (int x = xl; x <= xr; x++)
            {
                fSum += m_aAve[x + m_nRange];
                fSigma += m_aSigma[x + m_nRange];
                m_aaAve[x] = fSum / nTotal;
                m_aaSigma[x] = fSigma / nTotal;
                fSum -= m_aAve[x - m_nRange];
                fSigma -= m_aSigma[x - m_nRange];
            }
        }

        void WhiteBalance()
        {
            for (int n = 0; n < c_nThread; n++) m_bRunBalance[n] = true;
            Array.Clear(m_aHisto, 0, 256 * c_nThread);
            Thread.Sleep(10);
            int nRun = c_nThread;
            while (nRun > 0)
            {
                Thread.Sleep(1);
                nRun = 0;
                for (int n = 0; n < c_nThread; n++) if (m_bRunBalance[n]) nRun++;
            }
            m_log.Add("GV Ave = " + m_data.m_gvAve.ToString("0.0"));
            m_log.Add("GV Sigma = " + m_data.m_gvSigma.ToString("0.0"));
        }

        void WhiteBalance(int nIndex, int y)
        {
            for (int x = m_data.m_aEdge[y].Min; x <= m_data.m_aEdge[y].Max; x++)
            {
                double dSigma = (m_img.m_aBuf[y, x] - m_aaAve[x]) / m_aaSigma[x] * m_data.m_gvSigma / 2;
                //m_img.m_aBuf[y, x] = (byte)(m_data.m_gvAve + dSigma);
                m_img.m_aBuf[y, x] = (byte)((m_img.m_aBuf[y, x] + (2 * m_data.m_gvAve) + dSigma) / 3); // ing 170711
                m_aHisto[nIndex, m_img.m_aBuf[y, x]]++;
            }
        }

    }
}
