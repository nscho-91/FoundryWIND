using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using ezAutoMom;
using ezTools;

namespace ezAuto_EFEM
{
    public class HW_Aligner_EBR_EBR_Concentric
    {
        string m_id;
        Auto_Mom m_auto;
        HW_Aligner_EBR_AOI m_AOI;
        HW_Aligner_EBR_EBR_Graph m_EBRGraph;
        Log m_log;

        ezView m_viewAlign;
        ezImg m_imgAlign;
        CPoint m_szAlign = new CPoint(0, 0); 

        ezImg m_imgAve = null;
        ezImg m_imgSub = null;
        int[] m_aSumY;

        int m_nLength = 30;
        int m_lAveY = 200;
        int m_nArea = 50;
        int m_nThreshold = 100;

        bool m_bDraw = false;
        int[] m_aCount = null;
        public ArrayList m_arrSurface = new ArrayList();
        MinMax m_mmImg;

        public HW_Aligner_EBR_EBR_Concentric()
        {
        }

        public void Init(string id, Auto_Mom auto, ezView viewAlign, HW_Aligner_EBR_AOI AOI, HW_Aligner_EBR_EBR_Graph EBRGraph, Log log)
        {
            m_id = id;
            m_auto = auto;
            m_AOI = AOI;
            m_EBRGraph = EBRGraph; 
            m_viewAlign = viewAlign;
            m_log = log;
            m_imgAve = new ezImg(m_id, m_log);
            m_imgSub = new ezImg(m_id, m_log); 
        }

        public void ThreadStop()
        {

        }

        public void RunGrid(ezGrid rGrid, eGrid eMode)
        {
            rGrid.Set(ref m_nGV, m_id, "dGV", "delta GV (0 ~ 255)");
            rGrid.Set(ref m_nLength, m_id, "ROI", "Inspect ROI Width (Pixel)");
            rGrid.Set(ref m_nArea, m_id, "Area", "Defect Area (Pixel)");
            rGrid.Set(ref m_nThreshold, m_id, "Y_Threshold", "Y Size Threashold");
        }

        public void DrawDefect(Graphics dc)
        {
            try
            {
                foreach (Defect def in m_arrSurface)
                {
                    if (!m_bDraw) return;
                    m_viewAlign.m_imgView.DrawString(dc, "Surface : " + def.m_nSize, def.m_cp0);
                    m_viewAlign.m_imgView.DrawLine(dc, Color.Red, new CPoint(def.m_cp0.x, def.m_cp0.y), new CPoint(def.m_cp0.x, def.m_cp1.y));
                    m_viewAlign.m_imgView.DrawLine(dc, Color.Red, new CPoint(def.m_cp0.x, def.m_cp1.y), new CPoint(def.m_cp1.x, def.m_cp1.y));
                    m_viewAlign.m_imgView.DrawLine(dc, Color.Red, new CPoint(def.m_cp1.x, def.m_cp1.y), new CPoint(def.m_cp1.x, def.m_cp0.y));
                    m_viewAlign.m_imgView.DrawLine(dc, Color.Red, new CPoint(def.m_cp1.x, def.m_cp0.y), new CPoint(def.m_cp0.x, def.m_cp0.y));
                }
            }
            catch (Exception ex)
            {
                m_log.Add(ex.Message);
            }
        }

        public void Inspect(MinMax mmBevel, bool bTrim = false)
        {
            ezStopWatch sw = new ezStopWatch();
            m_bDraw = false;
            m_arrSurface.Clear();
            m_mmImg = mmBevel;
            m_imgAlign = m_viewAlign.m_imgView.m_pImg;
            m_szAlign = m_imgAlign.m_szImg;
            int nROI = ((m_mmImg.Range() + m_nLength + 10) / 48 + 1) * 48;
            m_aSumY = new int[nROI];
            m_aCount = new int[m_szAlign.y / c_nBand + 1]; 
            m_imgAve.ReAllocate(new CPoint(nROI, m_imgAlign.m_szImg.y), 1);
            m_imgSub.ReAllocate(new CPoint(nROI, m_imgAlign.m_szImg.y), 1);
            m_mmImg.Max += 5;
            m_mmImg.Min = m_mmImg.Max - nROI;
            MakeAveY(m_mmImg.Min, m_mmImg.Max);

            CalcCount(m_mmImg.Min, bTrim);

            MergeDefect();
            //RepositionDefect();

            MakeSub(m_mmImg.Min, m_mmImg.Max);

            m_log.Add("Concentric : " + sw.Check().ToString());
            m_bDraw = true;
        }

        void MakeAveY(int x0, int x1)
        {
            int nDiv = 0; 
            for (int y = -m_lAveY; y <= m_lAveY; y++)
            {
                MakeSumY(x0, x1, y);
                nDiv++;
            }
            for (int y = 0; y < m_szAlign.y; y++)
            {
                for (int x = x0, ix = 0; x < x1; x++, ix++)
                {
                    m_imgAve.m_aBuf[y, ix] = (byte)(m_aSumY[ix] / nDiv);
                }
                MakeSubY(x0, x1, y - m_lAveY);
                MakeSumY(x0, x1, y + m_lAveY + 1);
            }
        }

        void MakeSumY(int x0, int x1, int y)
        {
            if (y < 0) y += m_szAlign.y;
            y = y % m_szAlign.y;
            for (int x = x0, ix = 0; x < x1; x++, ix++) m_aSumY[ix] += m_imgAlign.m_aBuf[y, x];
        }

        void MakeSubY(int x0, int x1, int y)
        {
            if (y < 0) y += m_szAlign.y;
            y = y % m_szAlign.y;
            for (int x = x0, ix = 0; x < x1; x++, ix++) m_aSumY[ix] -= m_imgAlign.m_aBuf[y, x];
        }

        int m_nGV = 20;
        bool m_bTest = false; //forget
        int c_nBand = 40; 

        void MakeSub(int x0, int x1)
        {
            if (m_bTest == false) return; 
            for (int y = 0; y < m_szAlign.y; y++)
            {
                for (int x = x0, ix = 0; x < x1; x++, ix++)
                {
                    m_imgSub.m_aBuf[y, ix] = (byte)Math.Abs((int)m_imgAlign.m_aBuf[y, x] - m_imgAve.m_aBuf[y, ix]);
                    if (m_imgSub.m_aBuf[y, ix] > m_nGV) m_imgAlign.m_aBuf[y, x] = 255;
                    else m_imgAlign.m_aBuf[y, x] = 0; 
                }
                int xp = m_aCount[y / c_nBand];
                if (xp >= m_szAlign.x) xp = m_szAlign.x - 1;
                m_imgAlign.m_aBuf[y, xp] = 255;
            }
        }

        int c_nNotchSize = 700;
        void CalcCount(int x0, bool bTrim = false)
        {
            int xs = 0;
            Array.Clear(m_aCount, 0, m_szAlign.y / c_nBand + 1); 
            for (int y = 0; y < m_szAlign.y; y++)
            {
                if (m_AOI.IsNotch(new CPoint(0, y))) continue;
                int yBand = y / c_nBand;
                if (!bTrim) xs = m_EBRGraph.GetBevelEdge(y, m_szAlign.y) - (int)(m_nLength * 1.5);
                else xs = m_EBRGraph.GetTrimEdge(y, m_szAlign.y) - m_nLength;
                for (int x = xs, ix = xs - x0, n = 0; n < m_nLength; x++, ix++, n++)
                {
                    if (y > m_AOI.m_cpNotch.y - c_nNotchSize && y < m_AOI.m_cpNotch.y + c_nNotchSize) continue;
                    if ((byte)Math.Abs((int)m_imgAlign.m_aBuf[y, x] - m_imgAve.m_aBuf[y, ix]) > m_nGV) m_aCount[yBand]++;
                }
                if (m_bTest)
                {
                    m_imgAlign.m_aBuf[y, xs] = 255;
                    m_imgAlign.m_aBuf[y, xs + m_nLength] = 255; 
                }
            }
            for (int n = 0; n < m_aCount.Length; n++)
            {
                if (m_aCount[n] > m_nArea)
                {
                    Defect def = new Defect();
                    def.m_cp0.x = m_mmImg.Min;
                    def.m_cp1.x = m_mmImg.Max;
                    def.m_cp0.y = (n * c_nBand - c_nBand + m_imgAlign.m_szImg.y) % m_imgAlign.m_szImg.y;
                    def.m_cp1.y = (n * c_nBand + c_nBand) % m_imgAlign.m_szImg.y;
                    def.m_nSize = m_aCount[n];
                    m_arrSurface.Add(def);
                }
            }
        }

        void MergeDefect() // ing 170228
        {
            int n;
            if (m_arrSurface.Count < 2) return;
            for (n = 1; n < m_arrSurface.Count; n++)
            {
                if (n < 1) n = 1;
                if (Math.Abs(((Defect)m_arrSurface[n - 1]).m_cp1.y - ((Defect)m_arrSurface[n]).m_cp0.y) < c_nBand * 2)
                {
                    ((Defect)m_arrSurface[n - 1]).m_nSize += ((Defect)m_arrSurface[n]).m_nSize;
                    if (((Defect)m_arrSurface[n - 1]).m_cp0.x > ((Defect)m_arrSurface[n]).m_cp0.x) ((Defect)m_arrSurface[n - 1]).m_cp0.x = ((Defect)m_arrSurface[n]).m_cp0.x;
                    if (((Defect)m_arrSurface[n - 1]).m_cp1.x < ((Defect)m_arrSurface[n]).m_cp1.x) ((Defect)m_arrSurface[n - 1]).m_cp1.x = ((Defect)m_arrSurface[n]).m_cp1.x;
                    ((Defect)m_arrSurface[n - 1]).m_cp1.y = ((Defect)m_arrSurface[n]).m_cp1.y;
                    ((Defect)m_arrSurface[n - 1]).m_fAngle = ((360 - ((((Defect)m_arrSurface[n - 1]).m_cp0.y + ((Defect)m_arrSurface[n - 1]).m_cp1.y) / 2 - m_AOI.m_cpNotch.y + m_imgAlign.m_szImg.y)) % m_imgAlign.m_szImg.y) * 360.0 / m_imgAlign.m_szImg.y; // ing 170426
                    m_arrSurface.RemoveAt(n);
                    n--;
                }
            }
            for (n = 0; n < m_arrSurface.Count; n++)
            {
                if (((Defect)m_arrSurface[n]).m_cp1.y - ((Defect)m_arrSurface[n]).m_cp0.y < m_nThreshold)
                {
                    m_arrSurface.RemoveAt(n);
                    n--;
                    continue;
                }
            }
            for (n = 0; n < m_arrSurface.Count; n++) m_log.Add("Surface[" + n.ToString() + "] Position : " + ((Defect)m_arrSurface[n]).m_cp0.y.ToString());
        }

        public void RepositionDefect()
        {
            int n;
            if (m_arrSurface.Count < 1) return;
            for (n = 0; n < m_arrSurface.Count; n++)
            {
                if (n < 0) n = 0;
                if (((Defect)m_arrSurface[n]).m_cp1.y - ((Defect)m_arrSurface[n]).m_cp0.y < 2)
                {
                    m_arrSurface.RemoveAt(n);
                    n--;
                    if (m_arrSurface.Count < 1) return;
                    continue;
                }
                else
                {
                    ((Defect)m_arrSurface[n]).m_cp0.y *= c_nBand;
                    ((Defect)m_arrSurface[n]).m_cp1.y *= c_nBand;
                    ((Defect)m_arrSurface[n]).m_nSize *= c_nBand;
                    ((Defect)m_arrSurface[n]).m_fAngle *= c_nBand;
                    m_log.Add("Surface[" + n.ToString() + "] Position : " + ((Defect)m_arrSurface[n]).m_cp0.y.ToString());
                }
            }
        }
    }
}
