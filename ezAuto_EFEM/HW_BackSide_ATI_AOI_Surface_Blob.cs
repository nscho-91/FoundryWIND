using System;
using System.Collections; 
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading; 
using System.Threading.Tasks;
using ezTools; 

namespace ezAuto_EFEM
{
    class HW_BackSide_ATI_AOI_Surface_Blob
    {
        public string m_id;

        const ushort c_nMark = 65500;
        int c_nBlock = 1; 

        public ushort[] m_nIsland = new ushort[2] { 0, 0 };
        public ArrayList m_aIsland = new ArrayList();

        ushort[,] m_aDst = null;
        Visit[,] m_aVisit;

        public CPoint m_cpROI = new CPoint(0, 0);
        CPoint m_szROI = new CPoint(0, 0);
        CPoint m_szImg = new CPoint(0, 0);

        Log m_log;

        ezImg m_img;
        MinMax m_nGV = new MinMax();
        public MinMax[] m_aEdge = null;
        public MinMax[] m_aRange = null;
        int m_nMinBlob = 0;
        int m_nMerge = 3; 
        double[] m_fGVSigma = new double[2] { 0, 0 };
        double m_gvSigma = 0;

        bool m_bRun = false;
        Thread m_thread = null;

        public bool m_bRunBlob = false;

        public string m_strCode = "000";
        public int m_nRotate = 0;
        
        public void Init(string id, Log log)
        {
            m_id = id;
            m_log = log;
            m_thread = new Thread(new ThreadStart(RunThread)); 
            m_thread.Start();
        }

        public void ThreadStop()
        {
            if (m_bRun) { m_bRun = false; m_thread.Join(); }
        }

        public bool Inspect(ezImg img, CPoint cpROI, int nBlock, MinMax[] aEdge, double[] fGVSigma, double gvSigma, int nMinBlob, int nMerge,int nRotate, string strCode = "")
        {
            m_img = img;
            m_cpROI = cpROI;
            c_nBlock = nBlock;
            m_aEdge = aEdge;
            m_fGVSigma[0] = fGVSigma[0];
            m_fGVSigma[1] = fGVSigma[1];
            m_gvSigma = gvSigma; 
            m_nMinBlob = nMinBlob;
            m_nMerge = nMerge; 
            m_nIsland[1] = 0;
            m_aIsland.Clear();
            ReAllocate();
            m_bRunBlob = true;
            m_nRotate = nRotate;
            m_strCode = strCode;
            return false; 
        }

        void ReAllocate()
        {
            if (m_szImg != m_img.m_szImg)
            {
                m_szImg = m_img.m_szImg;
                m_szROI = m_szImg;
                m_szROI /= c_nBlock;
                m_aRange = new MinMax[m_szImg.y]; 
                m_aDst = new ushort[m_szROI.y, m_szROI.x];
                m_aVisit = new Visit[m_szROI.y, m_szROI.x]; 
            }
        }

        void CalcEdge()
        {
            for (int y = 0; y < m_szImg.y; y++)
            {
                m_aRange[y] = m_aEdge[y];
                if (m_aEdge[y].Min > 1)
                {
                    if (m_aRange[y].Min < m_cpROI.x) m_aRange[y].Min = m_cpROI.x;
                    if (m_aRange[y].Max > m_cpROI.x + m_szROI.x) m_aRange[y].Max = m_cpROI.x + m_szROI.x;
                }
            }
        }

        void RunThread()
        {
            m_bRun = true;
            Thread.Sleep(5000);
            while (m_bRun)
            {
                Thread.Sleep(10);
                if (m_bRunBlob)
                {
                    ezStopWatch sw = new ezStopWatch();
                    for (int n = 0; n < c_nBlock; n++) DoBlob(n);
                    while (MergeIsland(0, m_nIsland[1])) ; 
                    m_bRunBlob = false; 
                }
            }
        }

        void DoBlob(int nBlock)
        {
            m_cpROI.x = nBlock * m_szROI.x; 
            CalcEdge();
            CalcLocalGV();
            InitData();
            DoBlob(); 
        }

        void DoBlob()
        {
            m_nIsland[0] = m_nIsland[1]; 
            for (int y = 0; y < m_szROI.y; y++)
            {
                for (int x = 0; x < m_szROI.x; x++)
                {
                    if (m_aDst[y, x] == c_nMark)
                    {
                        m_nIsland[1]++;
                        if (m_nIsland[1] >= c_nMark)
                        {
                            m_log.Popup("Blob Error : Too Many Island !!");
                            return;
                        }
                        m_aDst[y, x] = m_nIsland[1];
                        azBlob.Island island = GetIsland();
                        island.Init(x, y);
                        FindNeighbor(ref island, new CPoint(x, y), true);
                        island.CalcCenter();
                        island.Shift(m_cpROI);
                        if (island.m_nSize <= m_nMinBlob)
                        {
                            island.m_aPosition.RemoveRange(island.m_aPosition.Count - island.m_nSize, island.m_nSize);
                            m_nIsland[1]--;
                        }
                        island.m_nRoateMode = m_nRotate;
                        island.m_strCode = m_strCode; // ing 170321
                    }
                }
            }
            while (MergeIsland(m_nIsland[0], m_nIsland[1])) ; 
        }

        bool MergeIsland(int nIsland0, int nIsland1)
        {
            bool bMerge = false;
            for (int n = nIsland0; n < nIsland1; n++) if (((azBlob.Island)m_aIsland[n]).m_bValid)
            {
                azBlob.Island island = (azBlob.Island)m_aIsland[n];
                for (int m = n + 1; m < nIsland1; m++) if (((azBlob.Island)m_aIsland[m]).m_bValid)
                {
                    if (island.Merge((azBlob.Island)m_aIsland[m], m_nMerge)) bMerge = true; 
                }
            }
            return bMerge; 
        }

        public void MergeIsland(HW_BackSide_ATI_AOI_Surface_Blob aoiBlob0)
        {
            for (int n = 0; n < m_nIsland[1]; n++) if (((azBlob.Island)m_aIsland[n]).m_bValid)
            {
                azBlob.Island island = (azBlob.Island)m_aIsland[n];
                for (int m = 0; m < aoiBlob0.m_nIsland[1]; m++) 
                {
                    azBlob.Island island0 = (azBlob.Island)aoiBlob0.m_aIsland[m]; 
                    if (island0.m_bValid)
                    {
                        island.Merge(island0, m_nMerge); 
                    }
                }
            }
        }

        azBlob.Island GetIsland()
        {
            if (m_aIsland.Count < m_nIsland[1])
            {
                azBlob.Island island = new azBlob.Island();
                m_aIsland.Add(island); 
            }
            return (azBlob.Island)m_aIsland[m_nIsland[1] - 1];
        }

        void CalcLocalGV()
        {
            int nCount = 0; 
            double fSum = 0;
            for (int y = 0, yImg = m_cpROI.y; y < m_szROI.y; y++, yImg++)
            {
                for (int xImg = m_aRange[yImg].Min; xImg < m_aRange[yImg].Max; xImg++)
                {
                    nCount++; 
                    fSum += m_img.m_aBuf[yImg, xImg];
                }
            }
            if (nCount == 0) return; 
            double fAve = fSum / nCount;
            m_nGV.Min = (int)(fAve - m_gvSigma * m_fGVSigma[0]);
            m_nGV.Max = (int)(fAve + m_gvSigma * m_fGVSigma[1]); 
        }

        void InitData()
        {
            Array.Clear(m_aDst, 0, m_szROI.x * m_szROI.y);
            for (int y = 0, yImg = m_cpROI.y; y < m_szROI.y; y++, yImg++)
            {
                for (int xImg = m_aRange[yImg].Min; xImg < m_aRange[yImg].Max; xImg++)
                {
                    int x = xImg - m_cpROI.x;
                    if (m_img.m_aBuf[yImg, xImg] > m_nGV.Max)
                    {
                        m_aDst[y, x] = c_nMark;
                        //m_img.m_aBuf[yImg, xImg] = 255; 
                    }
                    else if (m_img.m_aBuf[yImg, xImg] < m_nGV.Min)
                    {
                        m_aDst[y, x] = c_nMark;
                        //m_img.m_aBuf[yImg, xImg] = 0;
                    }
                    m_aVisit[y, x].m_bVisit = false;
                }
            }
        }

        void FindNeighbor(ref azBlob.Island rIsland, CPoint cp, bool bAddPixel = false)
        {
            int x, y, x0, y0;
            rIsland.m_nSize++; m_aVisit[cp.y, cp.x].Set(true, cp);
            if (bAddPixel) rIsland.m_aPosition.Add(cp + m_cpROI); // ing 161203
            while (true)
            {
                x0 = cp.x; y0 = cp.y;
                if (cp.x > 0)
                {
                    x = x0 - 1; y = y0;
                    if (!m_aVisit[y, x].m_bVisit && (m_aDst[y, x] == c_nMark))
                    {
                        m_aDst[y, x] = m_aDst[y0, x0]; m_aVisit[y, x].Set(true, cp);
                        if (bAddPixel) rIsland.m_aPosition.Add(cp + m_cpROI); // ing 161203
                        cp.x--; rIsland.m_nSize++; rIsland.m_rpCenter.x += cp.x; rIsland.m_rpCenter.y += cp.y;
                        if (cp.x <= 0) cp.x = 0;
                        if (cp.x <= rIsland.m_cp0.x) rIsland.m_cp0.x = cp.x;
                        continue;
                    }
                }
                if (cp.x < (m_szROI.x - 1))
                {
                    x = x0 + 1; y = y0;
                    if (!m_aVisit[y, x].m_bVisit && (m_aDst[y, x] == c_nMark))
                    {
                        m_aDst[y, x] = m_aDst[y0, x0]; m_aVisit[y, x].Set(true, cp);
                        if (bAddPixel) rIsland.m_aPosition.Add(cp + m_cpROI); // ing 161203
                        cp.x++; rIsland.m_nSize++; rIsland.m_rpCenter.x += cp.x; rIsland.m_rpCenter.y += cp.y;
                        if (cp.x >= (m_szROI.x - 1)) cp.x = m_szROI.x - 1;
                        if (cp.x >= rIsland.m_cp1.x) rIsland.m_cp1.x = cp.x;
                        continue;
                    }
                }
                if (cp.y > 0)
                {
                    x = x0; y = y0 - 1;
                    if (!m_aVisit[y, x].m_bVisit && (m_aDst[y, x] == c_nMark))
                    {
                        m_aDst[y, x] = m_aDst[y0, x0]; m_aVisit[y, x].Set(true, cp);
                        if (bAddPixel) rIsland.m_aPosition.Add(cp + m_cpROI); // ing 161203
                        cp.y--; rIsland.m_nSize++; rIsland.m_rpCenter.x += cp.x; rIsland.m_rpCenter.y += cp.y;
                        if (cp.y <= 0) cp.y = 0;
                        if (cp.y <= rIsland.m_cp0.y) rIsland.m_cp0.y = cp.y;
                        continue;
                    }
                }
                if (cp.y < (m_szROI.y - 1))
                {
                    x = x0; y = y0 + 1;
                    if (!m_aVisit[y, x].m_bVisit && (m_aDst[y, x] == c_nMark))
                    {
                        m_aDst[y, x] = m_aDst[y0, x0]; m_aVisit[y, x].Set(true, cp);
                        if (bAddPixel) rIsland.m_aPosition.Add(cp + m_cpROI); // ing 161203
                        cp.y++; rIsland.m_nSize++; rIsland.m_rpCenter.x += cp.x; rIsland.m_rpCenter.y += cp.y;
                        if (cp.y >= (m_szROI.y - 1)) cp.y = m_szROI.y - 1;
                        if (cp.y >= rIsland.m_cp1.y) rIsland.m_cp1.y = cp.y;
                        continue;
                    }
                }
                if (cp != m_aVisit[y0, x0].m_cp) cp = m_aVisit[y0, x0].m_cp; else break;
            }
        }
    }

    struct Visit
    {
        public bool m_bVisit;
        public CPoint m_cp;

        public Visit(bool bVisit, CPoint cp)
        {
            m_bVisit = bVisit; m_cp = cp;
        }

        public void Set(bool bVisit, CPoint cp)
        {
            m_bVisit = bVisit; m_cp = cp;
        }

        public void Set(bool bVisit, int x, int y)
        {
            m_bVisit = bVisit; m_cp.x = x; m_cp.y = y;
        }
    }

}
