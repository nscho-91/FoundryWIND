using System;
using System.Collections; // ing 161203
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.IO;

namespace ezTools
{
    public enum eBlob { eArea, eX, eY, eL, eDistance, eNone };

    struct azBlob_Visit
    {
        public bool m_bVisit;
        public CPoint m_cp;

        public void Set(bool bVisit, CPoint cp)
        {
            m_bVisit = bVisit; m_cp = cp;
        }

        public void Set(bool bVisit, int x, int y)
        {
            m_bVisit = bVisit; m_cp.x = x; m_cp.y = y; 
        }
    }

    public struct azBlob_Island
    {
        public int m_nArea, m_nDistance;
        public CPoint m_cp0, m_cp1;
        public RPoint m_rpCenter;
        public ArrayList m_aPosition; // ing 161203
        public string m_strCode; // ing 170321
        public int m_nRotateMode;
            
        public int GetSize(eBlob n)
        {
            switch (n)
            {
                case eBlob.eX: return m_cp1.x - m_cp0.x;
                case eBlob.eY: return m_cp1.y - m_cp0.y;
                case eBlob.eL: return Math.Max(m_cp1.x - m_cp0.x, m_cp1.y - m_cp0.y);
                case eBlob.eDistance: return m_nDistance;
                default: return m_nArea; 
            }
        }

        public CPoint GetROI()
        {
            int x, y;
            x = m_cp1.x - m_cp0.x;
            y = m_cp1.y - m_cp0.y;
            if (x < 1) x = 1; if (y < 1) y = 1;
            return new CPoint(x, y); 
        }

        public void Clear(int x, int y)
        {
            m_nArea = 0; m_cp0 = m_cp1 = new CPoint(x, y); m_rpCenter = new RPoint(x, y); 
        }
        
        public void CalcCenter()
        {
            if (m_nArea <= 0) return;
            m_rpCenter /= m_nArea; 
        }

        public CPoint GetROICenter()
        {
            return new CPoint((m_cp0.x + m_cp1.x) / 2, (m_cp0.y + m_cp1.y) / 2);
        }

        public bool IsInside(azBlob_Island island, int nOffset = 0) // ing 170321
        {
            if (m_cp0.x - nOffset < island.m_cp0.x && m_cp1.x + nOffset > island.m_cp1.x && m_cp0.y - nOffset < island.m_cp0.y && m_cp1.y + nOffset > island.m_cp1.y)
            {
                return true;
            }
            return false;
        }
    }

    public class azBlob
    {
        public string m_id; 
        bool m_bFull = false;
        byte m_nIsland;
        byte[,] m_aDst = null;
        int[] m_nGV = new int[2];
        eBlob m_eBlob, m_eBlobSort;
        azBlob_Island[] m_aIsland = new azBlob_Island[256];
        azBlob_Visit[,] m_aVisit; 
        CPoint m_cp = new CPoint(0, 0);
        CPoint m_sz = new CPoint(0, 0); 
        Log m_log;
        ezImg m_img;
        public ArrayList m_aPosition = new ArrayList(); // ing 161203
        
        public azBlob(string id, Log log)
        {
            m_id = id;
            m_log = log;
        }

        public bool DoBlob(eBlob nBlob, ezImg img, CPoint cp, CPoint sz, int nGV0, int nGV1, bool bAddPixel = false)
        {
            int x, y; byte nIsland = 0;
            m_eBlob = nBlob; m_eBlobSort = eBlob.eNone; m_img = img; m_bFull = false;
            m_nIsland = 0; m_cp = cp; cp.FixSize(ref sz); 
            m_nGV[0] = nGV0; m_nGV[1] = nGV1; if (m_nGV[1] <= 0) m_nGV[1] = 256;
            if (!m_img.HasImage()) return true;
            if (m_img.CheckRange(ref cp, ref sz)) return true;
            ReAllocate(sz); InitData(); 
            for (y = 0; y < m_sz.y; y++) for (x = 0; x < m_sz.x; x++) if (m_aDst[y, x] == 255)
            {
                NextIsland(ref nIsland); if (m_nIsland < nIsland) m_nIsland = nIsland;
                m_aDst[y, x] = (byte)nIsland; m_aIsland[nIsland].Clear(x, y);
                FindNeighbor(ref m_aIsland[nIsland], new CPoint(x, y), bAddPixel); m_aIsland[nIsland].CalcCenter(); 
            }
            return false;
        }

        void ReAllocate(CPoint sz)
        {
            if (m_sz == sz) return; m_sz = sz;
            m_aVisit = new azBlob_Visit[sz.y, sz.x];
            m_aDst = new byte[sz.y, sz.x];
        }

        void InitData()
        {
            int n, x, y, w;  
            for (n = 0; n < 256; n++) m_aIsland[n].Clear(0, 0);
            for (y = 0; y < m_sz.y; y++) for (x = 0; x < m_sz.x; x++) m_aVisit[y, x].Set(false, x, y);
            unsafe
            {
                for (y = 0; y < m_sz.y; y++) 
                {
                    fixed (byte* pSrc = &m_img.m_aBuf[m_cp.y + y, m_cp.x], pDst = &m_aDst[y, 0]) 
                    {
                        for (x = 0, w = 0; x < m_sz.x; x++, w += m_img.m_nByte)
                        {
                            if ((*(pSrc + w) >= m_nGV[0]) && (*(pSrc + w) <= m_nGV[1])) *(pDst + x) = 255; else *(pDst + x) = 0;
                        }
                    }
                }
            }
        }

        void NextIsland(ref byte nIsland)
        {
            if (!m_bFull) { nIsland++; if (nIsland >= 250) m_bFull = true; return; }
            int n, iMin = 1, nMin = m_aIsland[1].GetSize(m_eBlob);
            for (n = 1; n <= 250; n++) if (m_aIsland[n].GetSize(m_eBlob) < nMin) { nMin = m_aIsland[n].GetSize(m_eBlob); iMin = n; }
            nIsland = (byte)iMin; 
        }

        void FindNeighbor(ref azBlob_Island rIsland, CPoint cp, bool bAddPixel = false)
        {
            int x, y, x0, y0;
            if (rIsland.m_aPosition == null) rIsland.m_aPosition = new ArrayList();
            rIsland.m_aPosition.Clear();
            rIsland.m_nArea++; m_aVisit[cp.y, cp.x].Set(true, cp);
            if (bAddPixel) rIsland.m_aPosition.Add(cp); // ing 161203
            while (true)
            {
                x0 = cp.x; y0 = cp.y;
                if (cp.x > 0)
                {
                    x = x0 - 1; y = y0;
                    if (!m_aVisit[y, x].m_bVisit && (m_aDst[y, x] == 255))
                    {
                        m_aDst[y, x] = m_aDst[y0, x0]; m_aVisit[y, x].Set(true, cp);
                        if (bAddPixel) rIsland.m_aPosition.Add(cp); // ing 161203
                        cp.x--; rIsland.m_nArea++; rIsland.m_rpCenter.x += cp.x; rIsland.m_rpCenter.y += cp.y;
                        if (cp.x <= 0) cp.x = 0;
                        if (cp.x <= rIsland.m_cp0.x) rIsland.m_cp0.x = cp.x;
                        continue; 
                    }
                }
                if (cp.x < (m_sz.x - 1))
                {
                    x = x0 + 1; y = y0;
                    if (!m_aVisit[y, x].m_bVisit && (m_aDst[y, x] == 255))
                    {
                        m_aDst[y, x] = m_aDst[y0, x0]; m_aVisit[y, x].Set(true, cp);
                        if (bAddPixel) rIsland.m_aPosition.Add(cp); // ing 161203
                        cp.x++; rIsland.m_nArea++; rIsland.m_rpCenter.x += cp.x; rIsland.m_rpCenter.y += cp.y;
                        if (cp.x >= (m_sz.x - 1)) cp.x = m_sz.x - 1;
                        if (cp.x >= rIsland.m_cp1.x) rIsland.m_cp1.x = cp.x;
                        continue;
                    }
                }
                if (cp.y > 0)
                {
                    x = x0; y = y0 - 1;
                    if (!m_aVisit[y, x].m_bVisit && (m_aDst[y, x] == 255))
                    {
                        m_aDst[y, x] = m_aDst[y0, x0]; m_aVisit[y, x].Set(true, cp);
                        if (bAddPixel) rIsland.m_aPosition.Add(cp); // ing 161203
                        cp.y--; rIsland.m_nArea++; rIsland.m_rpCenter.x += cp.x; rIsland.m_rpCenter.y += cp.y;
                        if (cp.y <= 0) cp.y = 0;
                        if (cp.y <= rIsland.m_cp0.y) rIsland.m_cp0.y = cp.y;
                        continue;
                    }
                }
                if (cp.y < (m_sz.y - 1))
                {
                    x = x0; y = y0 + 1;
                    if (!m_aVisit[y, x].m_bVisit && (m_aDst[y, x] == 255))
                    {
                        m_aDst[y, x] = m_aDst[y0, x0]; m_aVisit[y, x].Set(true, cp);
                        if (bAddPixel) rIsland.m_aPosition.Add(cp); // ing 161203
                        cp.y++; rIsland.m_nArea++; rIsland.m_rpCenter.x += cp.x; rIsland.m_rpCenter.y += cp.y;
                        if (cp.y >= (m_sz.y - 1)) cp.y = m_sz.y - 1;
                        if (cp.y >= rIsland.m_cp1.y) rIsland.m_cp1.y = cp.y;
                        continue;
                    }
                }
                if (cp != m_aVisit[y0, x0].m_cp) cp = m_aVisit[y0, x0].m_cp; else break;
            }
        }

        public void QuickSort(eBlob nBlob)
        {
            if (nBlob == m_eBlobSort) return;
            if (nBlob == eBlob.eDistance) CalcDistance();
            m_eBlobSort = nBlob; QuickSort(1, m_nIsland);
        }

        void QuickSort(int n0, int n1)
        {
            int i0, i1, nMid; azBlob_Island island;
            i0 = n0; i1 = n1; nMid = m_aIsland[(n0 + n1) / 2].GetSize(m_eBlobSort);
            do
            {
                while (m_aIsland[i0].GetSize(m_eBlobSort) > nMid) i0++;
                while (m_aIsland[i1].GetSize(m_eBlobSort) < nMid) i1--; 
                if (i0 <= i1)
                {
                    island = m_aIsland[i0]; m_aIsland[i0] = m_aIsland[i1]; m_aIsland[i1] = island;
                    i0++; i1--;
                }
            } while (i0 <= i1);
            if (n0 < i1) QuickSort(n0, i1);
            if (i0 < n1) QuickSort(i0, n1); 
        }

        public int GetMaxIndex(eBlob nBlob)
        {
            int i, iMax = 1, nMax = -1;
            for (i = 1; i <= m_nIsland; i++)
            {
                if (m_aIsland[i].GetSize(nBlob) > nMax)
                {
                    nMax = m_aIsland[i].GetSize(nBlob); iMax = i;
                }
            }
            return iMax; 
        }

        public RPoint GetMaxCenter(eBlob nBlob, int nDir)
        {
            return GetCenter(GetMaxIndex(nBlob), nDir);
        }

        public RPoint GetCenter(int nIsland)
        {
            return GetCenter(nIsland, 0); 
        }

        public RPoint GetCenter(int nIsland, int nDir)
        {
            int x, y, v, nCount, nMax; CPoint cp, sc;
            cp.x = m_sz.x / 2; cp.y = m_sz.y / 2; nCount = 0; nMax = -2000000000;
            if (nIsland > (int)m_nIsland) return new RPoint(m_cp + cp);
            if (nDir == 0) return m_aIsland[nIsland].m_rpCenter + new RPoint(m_cp);
            sc.x = (int)(3 * Math.Sin(Math.PI * nDir / 6));
            sc.y = (int)(3 * Math.Cos(Math.PI * nDir / 6));
            for (y = 0; y < m_sz.y; y++) for (x = 0; x < m_sz.x; x++) if (m_aDst[y, x] == nIsland)
            {
                v = x * sc.x + y * sc.y;
                if (v == nMax) { cp.x += x; cp.y += y; nCount++; }
                else if (v > nMax) { nMax = v; cp = new CPoint(x, y); nCount = 1; }
            }
            if (nCount > 0) cp /= nCount; 
            return new RPoint(m_cp + cp);
        }

        public int GetMaxSize(eBlob nBlob)
        {
            if (m_nIsland < 1) return 0;
            return GetSize(nBlob, GetMaxIndex(nBlob)); 
        }

        public int GetSize(eBlob nBlob, int nIsland)
        {
            if (nIsland > m_nIsland) return 0;
            int temp = m_aIsland[nIsland].GetSize(nBlob);
            return m_aIsland[nIsland].GetSize(nBlob); 
        }

        public CPoint GetROI(int nIsland)
        {
            if (nIsland > m_nIsland) return new CPoint(0, 0);
            return m_aIsland[nIsland].GetROI();
        }

        public azBlob_Island GetIsland(int nIsland)
        {
            return m_aIsland[nIsland];
        }

        void CalcDistance()
        {
            int n; CPoint cp, dp;
            cp = m_sz; cp /= 2; 
            for (n = 1; n <= m_nIsland; n++)
            {
                dp = cp - m_aIsland[n].m_rpCenter.ToCPoint(); dp.Square(dp); 
                m_aIsland[n].m_nDistance = (int)Math.Sqrt(dp.x + dp.y);
            }
        }
        public int GetNumLand()
        {
            return (int)m_nIsland;
        }

        public Rectangle GetBound(int nIndex)
        {
           	CPoint cp0, cp1;
            cp0 = m_aIsland[nIndex].m_cp0;
            cp1 = m_aIsland[nIndex].m_cp1;
            return new Rectangle(cp0.x, cp0.y, cp1.x, cp1.y);
        }

        public Rectangle GetBoundRelative(int nIndex)
        {
	        CPoint cp0, cp1;
            cp0 = m_aIsland[nIndex].m_cp0 + m_cp;
            cp1 = m_aIsland[nIndex].m_cp1 + m_cp;
            return new Rectangle(cp0.x, cp0.y, cp1.x, cp1.y);
        }

        public class Island
        {
            public bool m_bValid = true;
            public int m_nSize;
            public CPoint m_cp0;
            public CPoint m_cp1;
            public CPoint m_cpShift;
            public RPoint m_rpCenter;
            public ArrayList m_aPosition = new ArrayList(); // ing 161203
            public string m_strCode = "000";
            public int m_nRoateMode = 0;

            public Island()
            {
            }

            public void Init(int x, int y)
            {
                m_bValid = true;
                m_nSize = 0;
                m_cp0 = m_cp1 = new CPoint(x, y);
                m_rpCenter = new RPoint(x, y);
            }

            public void CalcCenter()
            {
                if (m_nSize <= 0) return;
                m_rpCenter /= m_nSize;
            }

            public void DrawRect(Graphics dc, ezImgView view, Color color, bool bDraw)
            {
                if (m_bValid == false || bDraw == false) return;
                CPoint cp1 = new CPoint(m_cp1.x + 1, m_cp1.y + 1);
                CPoint cp2 = new CPoint(m_cp0.x, m_cp1.y + 1);
                CPoint cp3 = new CPoint(m_cp1.x + 1, m_cp0.y);
                view.DrawLine(dc, color, m_cp0, cp2);
                view.DrawLine(dc, color, cp2, cp1);
                view.DrawLine(dc, color, cp1, cp3);
                view.DrawLine(dc, color, cp3, m_cp0);
                //view.DrawString(dc, ((int)m_rpCenter.x).ToString("") + "," + ((int)m_rpCenter.y).ToString(), m_cp0);
                view.DrawString(dc, m_strCode, m_cp0 + new CPoint(0, 10), new SolidBrush(color)); // ing 170321
            }

            public void Copy(Island island)
            {
                m_bValid = island.m_bValid;
                m_nSize = island.m_nSize;
                m_cp0 = island.m_cp0;
                m_cp1 = island.m_cp1;
                m_rpCenter = island.m_rpCenter;
                m_aPosition = island.m_aPosition; // ing 161203
                m_strCode = island.m_strCode; // ing 170321
                m_nRoateMode = island.m_nRoateMode;
            }

            public void Copy(azBlob_Island island)
            {
                m_bValid = true;
                m_nSize = island.GetSize(eBlob.eArea);
                m_cp0 = island.m_cp0;
                m_cp1 = island.m_cp1;
                m_rpCenter = island.m_rpCenter;
                m_aPosition = island.m_aPosition; // ing 161203
                m_strCode = island.m_strCode; // ing 170321
                m_nRoateMode = island.m_nRotateMode;
            }

            public void Shift(CPoint cpROI)
            {
                m_rpCenter.x += cpROI.x;
                m_rpCenter.y += cpROI.y;
                m_cp0 += cpROI;
                m_cp1 += cpROI;
            }

            public bool Merge(Island island, int nMerge)
            {
                if (m_bValid == false) return false;
                if (IsSeperate(island, nMerge)) return false;
                island.m_bValid = false;
                m_nSize += island.m_nSize;
                if (m_cp0.x > island.m_cp0.x) m_cp0.x = island.m_cp0.x;
                if (m_cp0.y > island.m_cp0.y) m_cp0.y = island.m_cp0.y;
                if (m_cp1.x < island.m_cp1.x) m_cp1.x = island.m_cp1.x;
                if (m_cp1.y < island.m_cp1.y) m_cp1.y = island.m_cp1.y;
                m_aPosition.AddRange(island.m_aPosition); // ing 161203
                island.m_strCode = m_strCode; // ing 170321
                m_nRoateMode = island.m_nRoateMode;
                return true;
            }

            public bool IsSeperate(Island island, int nMerge)
            {
                if (IsSeperate(m_cp0.x - nMerge, m_cp1.x + nMerge, island.m_cp0.x - nMerge, island.m_cp1.x + nMerge)) return true;
                if (IsSeperate(m_cp0.y - nMerge, m_cp1.y + nMerge, island.m_cp0.y - nMerge, island.m_cp1.y + nMerge)) return true;
                return false;
            }

            bool IsSeperate(int v00, int v01, int v10, int v11)
            {
                return (v00 > v11) || (v10 > v01);
            }

            public void CalcRotate(double fCos, double fSin, CPoint cpCenter, double dR)
            {
                CPoint cp = (m_cp0 + m_cp1) / 2;
                CPoint dp = cp - cpCenter;
                m_rpCenter.x = dp.x * fCos + dp.y * fSin;
                m_rpCenter.y = dp.y * fCos - dp.x * fSin;
                m_rpCenter *= (150 / dR);
            }

            public bool IsInside(azBlob.Island island, int nOffset = 0) // ing 170321
            {
                if (m_cp0.x - nOffset < island.m_cp0.x && m_cp1.x + nOffset > island.m_cp1.x && m_cp0.y - nOffset < island.m_cp0.y && m_cp1.y + nOffset > island.m_cp1.y) return true;
                return false;
            }
        }
    }
}
