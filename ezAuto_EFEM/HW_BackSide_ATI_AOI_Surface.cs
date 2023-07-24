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
    public class HW_BackSide_ATI_AOI_Surface
    {
        public string m_id;

        const int c_nBlock = 32;
        HW_BackSide_ATI_AOI_Surface_Blob[] m_aoiBlob = new HW_BackSide_ATI_AOI_Surface_Blob[c_nBlock];

        bool m_bUse = false;
        double[] m_fGVSigma = new double[2] { 2, 3 }; 
        int m_nBlobSize = 30;
        int m_nMergeSize = 100;
        int m_nRotateMode = 0;
        public int m_nMerge = 10;
        public string m_strCode = "000";
        string m_strShape;
        string[] m_strShapes = new string[Enum.GetNames(typeof(eShape)).Length];
        string m_strColor;
        string[] m_strColors;
        Color[] m_colors = new Color[] { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Purple, Color.Pink, Color.Orange, Color.Ivory, Color.Chocolate };
        public Color m_color = Color.Red;
        CPoint m_cpCenter = new CPoint(0, 0);
        CPoint m_szImg = new CPoint(0, 0);
        double m_dR = 0;
        public MinMax[] m_aEdge = null;
        eShape m_shape = eShape.EveryThing;
        enum eShape { EveryThing, Polygon, Line };

        Log m_log;
        ezImg m_img;
        HW_BackSide_ATI_AOIData m_data; 

        public int m_nIsland = 0;
        public ArrayList m_aIsland = new ArrayList();

        CPoint[] m_cpSclaeBarRange = new CPoint[2];
        CPoint[,] m_cpSclaeBar = new CPoint[2, 2];
        CPoint m_cpT, m_cpB, m_cpR, m_cpL;
        double m_dResolution;

        public HW_BackSide_ATI_AOI_Surface()
        {
        }

        public void Init(string id, HW_BackSide_ATI_AOIData data, Log log)
        {
            m_id = id;
            m_data = data;
            m_log = log;
            for (int y = 0; y < c_nBlock; y++) 
            {
                m_aoiBlob[y] = new HW_BackSide_ATI_AOI_Surface_Blob();
                m_aoiBlob[y].Init(m_id + y.ToString(), m_log); 
            }
            m_strColors = new string[m_colors.Length];
            m_strColor = m_colors[0].ToString();
            for (int n = 0; n < m_strColors.Length; n++)
            {
                m_strColors[n] = m_colors[n].ToString();
            }
            for (int n = 0; n < m_strShapes.Length; n++)
            {
                m_strShapes[n] = Enum.GetNames(typeof(eShape))[n];
            }
            m_strShape = eShape.EveryThing.ToString();

        }

        public void ThreadStop()
        {
            for (int y = 0; y < c_nBlock; y++) m_aoiBlob[y].ThreadStop(); 
        }

        public void RunGrid(ezGrid grid, eGrid eMode, ezJob job = null)
        {
            grid.Set(ref m_bUse, m_id, "Use", "Use Inspection");
            grid.Set(ref m_fGVSigma[0], m_id, "GVMin", "Small Value more Sensitive (Sigma)");
            grid.Set(ref m_fGVSigma[1], m_id, "GVMax", "Small Value more Sensitive (Sigma)");
            grid.Set(ref m_nBlobSize, m_id, "BlobSize", "Minimum Size (pixel)");
            grid.Set(ref m_nMerge, m_id, "Merge", "Merge Distance (pixel)");
            grid.Set(ref m_nMergeSize, m_id, "MergeSize", "Minimum Size (pixel)");
            grid.Set(ref m_strShape, m_strShapes, m_id, "Shape", "Shape");
            grid.Set(ref m_strColor, m_strColors, m_id, "Color", "Draw Color");
            grid.Set(ref m_strCode, m_id, "Code", "Defect Code");
        }

        public void Draw(Graphics dc, ezImgView view, bool bDraw)
        {
            if (bDraw == false) return;
            for (int n = 0; n < m_strColors.Length; n++)
            {
                if (m_strColor == m_colors[n].ToString())
                {
                    m_color = m_colors[n];
                }
            }
            try
            {
                for (int n = 0; n < m_aIsland.Count; n++)
                {
                    ((azBlob.Island)m_aIsland[n]).DrawRect(dc, view, m_color, bDraw);
                }
                DrawScaleBar(dc, view, bDraw);
            }
            catch (Exception ex)
            {
                m_log.Add(ex.Message);
            }
        }

        public void DrawScaleBar(Graphics dc, ezImgView view, bool bDraw)
        {
            //view.DrawLine(dc, Color.Green, m_cpT, m_cpR);
            //view.DrawLine(dc, Color.Green, m_cpR, m_cpB);
            //view.DrawLine(dc, Color.Green, m_cpB, m_cpL);
            //view.DrawLine(dc, Color.Green, m_cpL, m_cpT);
            if (m_dResolution == 0) return;
            view.DrawLine(dc, Color.Green, m_cpSclaeBar[0 ,0], m_cpSclaeBar[0, 1]);
            view.DrawLine(dc, Color.Green, m_cpSclaeBar[1, 0], m_cpSclaeBar[1, 1]);
            view.DrawLine(dc, Color.Green, (m_cpSclaeBar[0, 0] + m_cpSclaeBar[0, 1]) / 2, (m_cpSclaeBar[1, 0] + m_cpSclaeBar[1, 1]) / 2);
            view.DrawString(dc, m_data.m_fResolution.ToString("Resolution = 0.00") + "um", (m_cpSclaeBar[0, 0] + m_cpSclaeBar[1, 1]) / 2, new SolidBrush(Color.Black));
        }

        public bool Inspect(ezImg img, double dR, double gvSigma, int nRotateMode)
        {
            m_dResolution = 0;
            if (m_fGVSigma[0] <= 0) return false; 
            ezStopWatch sw = new ezStopWatch();
            m_nIsland = 0;
            m_aIsland.Clear();
            m_nRotateMode = nRotateMode;
            m_img = img;
            if (!m_bUse) return false;
            if (!m_img.HasImage()) return true;
            m_log.Add(m_id + " - Start Blob Inspection.");
            m_cpCenter = m_data.m_cpCenter;
            m_dR = dR;
            if (CheckRange())
            {
                m_log.Popup("HW_BackSide_ATI_AOI_Blob CheckRange Error !!"); 
                return true;
            }
            ReAllocate();
            CalcEdge(); 
            CPoint szROI = new CPoint(m_szImg.x / c_nBlock, m_szImg.y / c_nBlock);
            for (int y = 0; y < c_nBlock; y++) 
            {
                m_aoiBlob[y].Inspect(m_img, new CPoint(0, y * szROI.y), c_nBlock, m_aEdge, m_fGVSigma, gvSigma, m_nBlobSize, m_nMerge, m_nRotateMode, m_strCode); 
            }
            int nRun = c_nBlock;
            while (nRun > 0)
            {
                Thread.Sleep(1);
                nRun = 0;
                for (int y = 0; y < c_nBlock; y++) if (m_aoiBlob[y].m_bRunBlob) nRun++;
            }
            for (int y = 1; y < c_nBlock; y++) m_aoiBlob[y].MergeIsland(m_aoiBlob[y - 1]); 
            for (int y = 0; y < c_nBlock; y++) CopyIsland(m_aoiBlob[y]);

            for (int n = 0; n < Enum.GetNames(typeof(eShape)).Length; n++)
            {
                if (m_strShape == Enum.GetNames(typeof(eShape))[n])
                {
                    m_shape = (eShape)Enum.Parse(typeof(eShape), m_strShape);
                }
            }
            switch (m_shape)
            {
                case eShape.EveryThing:
                    break;

                case eShape.Polygon:
                    for (int n = 0; n < m_aIsland.Count; n++)
                    {
                        if ((((azBlob.Island)m_aIsland[n]).m_nSize / ((azBlob.Island)m_aIsland[n]).m_cp0.GetL(((azBlob.Island)m_aIsland[n]).m_cp1)) <= 7)
                        {
                            m_aIsland.RemoveAt(n);
                            n--;
                            m_nIsland--;
                        }
                    }
                    break;

                case eShape.Line:
                    for (int n = 0; n < m_aIsland.Count; n++)
                    {
                        if ((((azBlob.Island)m_aIsland[n]).m_nSize / ((azBlob.Island)m_aIsland[n]).m_cp0.GetL(((azBlob.Island)m_aIsland[n]).m_cp1)) > 7)
                        {
                            m_aIsland.RemoveAt(n);
                            n--;
                            m_nIsland--;
                        }
                    }
                    break;
            }
//            m_log.Add(m_id + " - Blob Inspect Time : " + sw.Check().ToString());
            return false;
        }

        public bool InspectPM(ezImg img, double dR, double gvSigma, CPoint[] cpScaleBarRagne)
        {
            m_dResolution = 0;
            if (m_fGVSigma[0] <= 0) return false;
            ezStopWatch sw = new ezStopWatch();
            m_cpSclaeBarRange = cpScaleBarRagne;
            m_nIsland = 0;
            m_aIsland.Clear();
            m_img = img;
            if (!m_bUse) return false;
            if (!m_img.HasImage()) return true;
            m_log.Add(m_id + " - Start Blob Inspection.");
            m_cpCenter = m_data.m_cpCenter;
            m_dR = dR;
            if (CheckRange())
            {
                m_log.Popup("HW_BackSide_ATI_AOI_Blob CheckRange Error !!");
                return true;
            }
            ReAllocate();
            CalcEdge();
            CPoint szROI = new CPoint(m_szImg.x / c_nBlock, m_szImg.y / c_nBlock);
            for (int y = 0; y < c_nBlock; y++)
            {
                m_aoiBlob[y].Inspect(m_img, new CPoint(0, y * szROI.y), c_nBlock, m_aEdge, m_fGVSigma, gvSigma, m_nBlobSize, m_nMerge, m_nRotateMode, m_strCode);
            }
            int nRun = c_nBlock;
            while (nRun > 0)
            {
                Thread.Sleep(1);
                nRun = 0;
                for (int y = 0; y < c_nBlock; y++) if (m_aoiBlob[y].m_bRunBlob) nRun++;
            }
            for (int y = 1; y < c_nBlock; y++) m_aoiBlob[y].MergeIsland(m_aoiBlob[y - 1]);
            for (int y = 0; y < c_nBlock; y++) CopyIsland(m_aoiBlob[y]);
            if (CalcScale())
            {
                m_log.Popup("Backside PM Fail !!");
            }
            return false;
        }


        void ReAllocate()
        {
            if (m_szImg != m_img.m_szImg)
            {
                m_szImg = m_img.m_szImg;
                m_aEdge = new MinMax[m_szImg.y];
            }
        }

        bool CheckRange()
        {
            if (m_cpCenter.x - m_dR < 0) return true; 
            if (m_cpCenter.y - m_dR < 0) return true;
            if (m_cpCenter.x + m_dR >= m_img.m_szImg.x) return true;
            if (m_cpCenter.y + m_dR >= m_img.m_szImg.x) return true; 
            return  false; 
        }

        void CalcEdge()
        {
            double dR2 = m_dR * m_dR;
            for (int y = 0; y < m_szImg.y; y++)
            {
                m_aEdge[y].Min = 0;
                m_aEdge[y].Max = -1;
                double dY = y - m_cpCenter.y;
                double dX2 = dR2 - dY * dY;
                if (dX2 > 1)
                {
                    int dX = (int)Math.Sqrt(dX2);
                    m_aEdge[y].Min = m_cpCenter.x - dX;
                    m_aEdge[y].Max = m_cpCenter.x + dX;
                }
            }
        }

        void CopyIsland(HW_BackSide_ATI_AOI_Surface_Blob aoi)
        {
            for (ushort n = 0; n < aoi.m_nIsland[1]; n++)
            {
                azBlob.Island islandBlob = ((azBlob.Island)aoi.m_aIsland[n]);
                if (islandBlob.m_bValid && (islandBlob.m_nSize > m_nMergeSize))
                {
                    azBlob.Island island = new azBlob.Island();
                    island.m_nRoateMode = m_nRotateMode;
                    island.Copy((azBlob.Island)aoi.m_aIsland[n]);
                    m_aIsland.Add(island); 
                    m_nIsland++;
                }
            }
        }

        public void ClearInvalidIsland()
        {
            if (!m_bUse) return; // 170321
            ArrayList aIsland = new ArrayList();
            foreach (azBlob.Island island in m_aIsland)
            {
                if (island.m_bValid) aIsland.Add(island);
            }
            m_aIsland = aIsland;
            m_nIsland = m_aIsland.Count;
        }

        public bool CalcScale()
        {
            int nScaleBar = -1;
            int nMax, nMaxT, nMaxB, nMaxR, nMaxL;
            double d1, d2, d3, d4, dDistance, dGradient, dSigma;
            nMax = nMaxT = nMaxR = 0;
            nMaxL = m_img.m_szImg.x - 1;
            nMaxB = m_img.m_szImg.y - 1;
            azBlob.Island scaleIsland = new azBlob.Island();
            scaleIsland.m_cp0 = m_cpSclaeBarRange[0];
            scaleIsland.m_cp1 = m_cpSclaeBarRange[1];
            for (int n = 0; n < m_aIsland.Count; n++)
            {
                if (((azBlob.Island)m_aIsland[n]).IsInside(scaleIsland))
                {
                    if (nMax < ((azBlob.Island)m_aIsland[n]).m_aPosition.Count)
                    {
                        nMax = ((azBlob.Island)m_aIsland[n]).m_aPosition.Count;
                        nScaleBar = n;
                    }
                }
            }
            if (nScaleBar < 0)
            {
                m_log.Popup("Can not Find Scale Bar !! Please Check Scale Bar Range !!");
                return true;
            }
            for (int n = 0; n < ((azBlob.Island)m_aIsland[nScaleBar]).m_aPosition.Count; n++)
            {
                CPoint cp = (CPoint)(((azBlob.Island)m_aIsland[nScaleBar]).m_aPosition[n]);
                if (nMaxT < cp.y)
                {
                    nMaxT = cp.y;
                    m_cpT = cp;
                }
                if (nMaxB > cp.y)
                {
                    nMaxB = cp.y;
                    m_cpB = cp;
                }
                if (nMaxR < cp.x)
                {
                    nMaxR = cp.x;
                    m_cpR = cp;
                }
                if (nMaxL > cp.x)
                {
                    nMaxL = cp.x;
                    m_cpL = cp;
                }
            }
            if (m_cpT.x < m_cpB.x)
            {
                dGradient = (double)((m_cpT.y - m_cpL.y) / (m_cpT.x - m_cpL.x));
                m_cpR.y = m_cpB.y + (int)((m_cpR.x - m_cpB.x) * dGradient);
                d1 = CalDistance(m_cpB.x, m_cpB.y, m_cpT.x, m_cpT.y, m_cpL.x, m_cpL.y);
                d2 = CalDistance(m_cpR.x, m_cpR.y, m_cpT.x, m_cpT.y, m_cpL.x, m_cpL.y);
                d3 = CalDistance(m_cpT.x, m_cpT.y, m_cpB.x, m_cpB.y, m_cpR.x, m_cpR.y);
                d4 = CalDistance(m_cpL.x, m_cpL.y, m_cpB.x, m_cpB.y, m_cpR.x, m_cpR.y);
                m_cpSclaeBar[0, 0] = new CPoint(m_cpT.x, m_cpT.y);
                m_cpSclaeBar[0, 1] = new CPoint(m_cpT.x + 5, m_cpT.y + (int)(dGradient * 5));
                m_cpSclaeBar[1, 0] = new CPoint(m_cpR.x, m_cpR.y);
                m_cpSclaeBar[1, 1] = new CPoint(m_cpR.x + 5, m_cpR.y + (int)(dGradient * 5));
            }
            else
            {
                dGradient = (double)((m_cpL.y - m_cpB.y) / (m_cpL.x - m_cpB.x));
                m_cpR.y = m_cpT.y + (int)((m_cpR.x - m_cpT.x) * dGradient);
                d1 = CalDistance(m_cpB.x, m_cpB.y, m_cpT.x, m_cpT.y, m_cpR.x, m_cpR.y);
                d2 = CalDistance(m_cpL.x, m_cpL.y, m_cpT.x, m_cpT.y, m_cpR.x, m_cpR.y);
                d3 = CalDistance(m_cpT.x, m_cpT.y, m_cpB.x, m_cpB.y, m_cpL.x, m_cpL.y);
                d4 = CalDistance(m_cpR.x, m_cpR.y, m_cpB.x, m_cpB.y, m_cpL.x, m_cpL.y);
            }
            dDistance = (double)((d1 + d2 + d3 + d4) / 4);
            dSigma = Math.Sqrt((d1 - dDistance) * (d1 - dDistance) + (d2 - dDistance) * (d2 - dDistance) + (d3 - dDistance) * (d3 - dDistance) + (d4 - dDistance) * (d4 - dDistance));
            m_dResolution = 1000 * m_data.m_fScaleBar / dDistance;
            m_dResolution = Math.Round(m_dResolution, 2);
            if (dSigma > 50 || m_data.m_fResolution > m_dResolution + m_dResolution / 100 || m_data.m_fResolution < m_dResolution - m_dResolution / 100)
            {
                m_log.Popup("The resolution has changed too much through PM. Please check result of PM inspection.");
                m_log.Popup("Original Resolution : " + m_data.m_fResolution.ToString("0.00") + " Measured Resolution Now : " + m_dResolution.ToString("0.00"));
                return true;
            }
            m_log.Popup("Resolution Has Changed : " + m_data.m_fResolution.ToString("0.00") + "->" + m_dResolution.ToString("0.00"));
            m_data.m_fResolution = m_dResolution;
            return false;
        }
        
        double CalDistance(int x, int y, int x1, int y1, int x2, int y2)
        {
            double dDist = 0.0;
            if (x1 == x2) return Math.Abs(x1 - x2);
            double a;
            double b = -1;
            double c;

            a = (y2 - y1) / (x2 - x1);
            c = (x2 * y1 - x1 * y2) / (x2 - x1);

            dDist = Math.Abs(a*x+b*y+c) / Math.Sqrt(a*a+b*b);

            /*
            double bunja = Math.Abs(x * (y2 - y1) + y * (x1 - x2) - (x1 * y2) + (x2 * y2));
            double bunmo = Math.Sqrt((y2 - y1) * (y2 - y1) + (x1 - x2) * (x1 - x2));
            dDist = bunja / bunmo;
            */
            return dDist;
        }
    }
}
