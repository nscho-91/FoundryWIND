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
    class HW_BackSide_ATI_AOI_Sobel
    {
        const double c_nLine = 7.0;
        public string m_id;
        Log m_log;
        HW_BackSide_ATI_AOIData m_data; 

        ezImg m_imgDown;
        ezImg m_imgSobel; 
        CPoint m_cpCenter = new CPoint();
        CPoint m_szImg = new CPoint();
        double m_dR = 0;
        MinMax[] m_aEdge = null;

        bool m_bUse = false;
        bool m_bFileSave = false;
        MinMax m_nGV;
        //int m_nBlobGV = 100;
        int m_nBlobSize = 4;
        public int m_nBlobMerge = 20;
        int m_nMergeSize = 100;
        int m_nAveOffset = 1;
        int m_nRotateMode = 0;
        public string m_strCode = "000";
        string m_strShape;
        string[] m_strShapes = new string[Enum.GetNames(typeof(eShape)).Length];
        string m_strColor;
        string[] m_strColors;
        Color[] m_colors = new Color[] { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Purple, Color.Pink, Color.Orange, Color.Ivory, Color.Chocolate };
        public Color m_color = Color.Red;
        eShape m_shape = eShape.EveryThing;
        enum eShape { EveryThing, Polygon, Line };

        azBlob[] m_blob = new azBlob[2]; // 0, 90도 검사를 나누기위해서, m_blob의 Doblob이 검사 결과에 영향을 줌. 복사해서 쓰는데도 영향을 줌 이상함.
        public int m_nIsland = 0;
        //public ArrayList[] m_aIsland = new ArrayList[2];
        public ArrayList m_aIsland = new ArrayList();

        public HW_BackSide_ATI_AOI_Sobel()
        {
        }

        public void Init(string id, HW_BackSide_ATI_AOIData data, Log log)
        {
            m_id = id;
            m_data = data; 
            m_log = log;
            m_imgSobel = new ezImg(id + "_Sobel", log);
            m_blob[0] = new azBlob(m_id + "_0", log);
            m_blob[1] = new azBlob(m_id + "_0", log);
            m_strColors = new string[m_colors.Length];
            m_strColor = m_colors[0].ToString();
            m_nGV = new MinMax(100, 256, m_log);
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
        }

        public void RunGrid(ezGrid grid, eGrid eMode, ezJob job = null)
        {
            grid.Set(ref m_bUse, m_id, "Use", "Use Inspection");
            grid.Set(ref m_nGV, m_id, "GV", "BlobGV");
            //grid.Set(ref m_nBlobGV, m_id, "GV", "Blob GV");
            grid.Set(ref m_nAveOffset, m_id, "Ave", "Offset of Average");
            grid.Set(ref m_nBlobSize, m_id, "BlobSize", "Minimum Blob Size (Pixel)");
            grid.Set(ref m_nBlobMerge, m_id, "Merge", "Blob Merge Size (Pixel)");
            grid.Set(ref m_nMergeSize, m_id, "MergeSize", "Minimum Size (pixel)");            
            grid.Set(ref m_bFileSave, m_id, "FileSave", "Image FileSave");
            grid.Set(ref m_strShape, m_strShapes, m_id, "Shape", "Shape");
            grid.Set(ref m_strColor, m_strColors, m_id, "Color", "Draw Color");
            grid.Set(ref m_strCode, m_id, "Code", "Defect Code");
            //if ((eMode == eGrid.eJobOpen) || (eMode == eGrid.eRegRead)) m_bFileSave = false; 
        }

        public void Draw(Graphics dc, ezImgView view, bool bDraw)
        {
            if (bDraw == false) return;
            for (int n = 0; n < m_strColors.Length; n++)
            {
                if (m_strColor == m_colors[n].ToString())
                {
                    m_color = m_colors[n];
                    break;
                }
            }
            try
            {
                foreach (azBlob.Island island in m_aIsland)
                {
                    island.DrawRect(dc, view, m_color, bDraw);
                }
            }
            catch (Exception ex)
            {
                m_log.Add(ex.Message);
            }
        }

        public bool Inspect(double dR, double fAngle, int nRotate)
        {
            if (m_data.m_nDown == 0) return false;
            ezStopWatch sw = new ezStopWatch();
            m_nRotateMode = nRotate;
            m_imgDown = m_data.m_imgDown;//축소된 이미지로 처리
            m_aIsland.Clear();
            if (!m_bUse) return false;
            m_imgDown.FileSave("D:\\Before.bmp");
            m_cpCenter = m_data.m_cpCenterDown;
            m_dR = dR;
            m_data.CalcEdge(m_dR);
            m_aEdge = m_data.m_aEdgeDown; 
            if (!m_imgDown.HasImage()) return true;
            m_log.Add(m_id + " - Start Sobel Inspection.");
            ReAllocate(m_imgDown.m_szImg);

            Array.Clear(m_imgSobel.m_aBuf, 0, m_szImg.x * m_szImg.y);
            m_data.RunCalcThread(CalcSobel); 
            RunAve(); // 161128
            RunBlob(); //surface 검사,절대gv

            m_log.Add(m_id + " - AOI Sobel : " + sw.Check().ToString());

            if (m_bFileSave)
            {
                //m_imgDown.RotateImage(m_imgDown, m_imgDown, "180");
                m_imgDown.m_fAngle = fAngle;
                m_imgDown.FileSave("d:\\Down.bmp");
                m_imgSobel.FileSave("d:\\Sobel.bmp");
            }

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
            return false;
        }

        void ReAllocate(CPoint szImg)
        {
            if (m_szImg == szImg) return; 
            m_szImg = szImg;
            m_imgSobel.ReAllocate(szImg, 1);
        }

        void CalcSobel(int nIndex, int y)
        {
            if (m_aEdge[y].Max < 0) return;
            MinMax mm = new MinMax();
            int nMargin = 3;
            if (y - nMargin < 0 || y - nMargin > m_aEdge.Length - 1 || y + nMargin < 0 || y + nMargin > m_aEdge.Length - 1) return; // ing 161128
            mm.Min = Math.Max(m_aEdge[y - nMargin].Min, m_aEdge[y + nMargin].Min);
            mm.Max = Math.Min(m_aEdge[y - nMargin].Max, m_aEdge[y + nMargin].Max); 
            if (mm.Max < 0) return; 
            for (int x = mm.Min; x <= mm.Max; x++)
            {
                m_imgSobel.m_aBuf[y, x] = (byte)CalcSobelXY(x, y); 
            }
        }

        int[,] m_aSobelX = new int[7, 7]
        {
            { -1, -1, -1, 0, 1, 1, 1 },
            { -1, -1, -1, 0, 1, 1, 1 },
            { -1, -1, -2, 0, 2, 1, 1 },
            { -1, -2, -2, 0, 2, 2, 1 },
            { -1, -1, -2, 0, 2, 1, 1 },
            { -1, -1, -1, 0, 1, 1, 1 },
            { -1, -1, -1, 0, 1, 1, 1 }
        };
        int[,] m_aSobelY = new int[7, 7]
        {
            { -1, -1, -1, -1, -1, -1, -1 },
            { -1, -1, -1, -2, -1, -1, -1 },
            { -1, -1, -2, -2, -2, -1, -1 },
            {  0,  0,  0,  0,  0,  0,  0 },
            {  1,  1,  2,  2,  2,  1,  1 },
            {  1,  1,  1,  2,  1,  1,  1 },
            {  1,  1,  1,  1,  1,  1,  1 }
        };
        byte CalcSobelXY(int x0, int y0)
        {
            int nSobelX = 0;
            int nSobelY = 0;
            for (int y = 0, yp = y0 - 3; y < 7; y++, yp++)
            {
                for (int x = 0, xp = x0 - 3; x < 7; x++, xp++)
                {
                    nSobelY += (m_aSobelY[y, x] * m_imgDown.m_aBuf[yp, xp]);
                    nSobelX += (m_aSobelX[y, x] * m_imgDown.m_aBuf[yp, xp]);
                }
            }
            return (byte)(Math.Abs(nSobelX) + Math.Abs(nSobelY));
        }

        void RunAve()
        {
            int x, y, n, nGV;
            unsafe
            {
                for (y = m_nAveOffset; y < m_imgSobel.m_szImg.y - m_nAveOffset; y++)
                {
                    x = m_nAveOffset;
                    while (x < m_imgSobel.m_szImg.x - m_nAveOffset)
                    {
                        nGV = n = 0;
                        byte* pSrc = (byte*)m_imgSobel.GetIntPtr(y, x);
                        byte* pAve = (byte*)m_imgSobel.GetIntPtr(y - m_nAveOffset, x - m_nAveOffset);
                        while (n < (2 * m_nAveOffset + 1) * (2 * m_nAveOffset + 1))
                        {
                            nGV += *pAve;
                            if (*pSrc > 0) nGV += 0;
                            if (nGV > 0) nGV += 0;
                            pAve++;
                            n++;
                            if (n % (2 * m_nAveOffset + 1) == 0) pAve += m_imgSobel.m_szImg.x - (2 * m_nAveOffset + 1);
                        }
                        *pSrc = (byte)(nGV / n);
                        x++;
                    }
                }
            }
        }

        void RunBlob()
        {
            m_nIsland = 0;
            m_blob[m_nRotateMode].DoBlob(eBlob.eArea, m_imgSobel, new CPoint(0, 0), m_imgSobel.m_szImg, m_nGV.Min, m_nGV.Max, true);
            for (int n = 1; n <= m_blob[m_nRotateMode].GetNumLand(); n++)
            {
                if (m_blob[m_nRotateMode].GetIsland(n).GetSize(eBlob.eArea) > m_nBlobSize)
                {
                    azBlob.Island island = new azBlob.Island();
                    island.Copy(m_blob[m_nRotateMode].GetIsland(n));
                    ReSizeIsland(island);//원 사이즈로 돌아감
                    island.m_strCode = m_strCode; // ing 170321
                    island.m_nRoateMode = m_nRotateMode;
                    m_aIsland.Add(island);
                    m_nIsland++; 
                }
            }
            while (MergeIsland()) ; 
        }

        void ReSizeIsland(azBlob.Island island)
        {
            CPoint cp;
            island.m_cp0 *= m_data.m_nDown;
            island.m_cp1.x += 1;
            island.m_cp1.y += 1;
            island.m_cp1 *= m_data.m_nDown;
            island.m_nSize *= m_data.m_nDown;
            for (int n = 0; n < island.m_aPosition.Count; n++ )
            {
                cp.x = ((CPoint)island.m_aPosition[n]).x * m_data.m_nDown;
                cp.y = ((CPoint)island.m_aPosition[n]).y * m_data.m_nDown;
                island.m_aPosition[n] = cp;
            }
        }

        bool MergeIsland()
        {
            bool bMerge = false;
            for (int n = 0; n < m_nIsland; n++) if (((azBlob.Island)m_aIsland[n]).m_bValid)
            {
                azBlob.Island island = (azBlob.Island)m_aIsland[n];
                for (int m = n + 1; m < m_nIsland; m++) if (((azBlob.Island)m_aIsland[m]).m_bValid)
                {
                    if (island.Merge((azBlob.Island)m_aIsland[m], m_nBlobMerge)) bMerge = true;
                }
            }
            ClearInvalidIsland(); 
            return bMerge;
        }

        public void ClearInvalidIsland()
        {
            if (!m_bUse) return; // ing 170321
            ArrayList aIsland = new ArrayList();
            foreach (azBlob.Island island in m_aIsland)
            {
                if (island.m_bValid && island.m_nSize > m_nMergeSize) aIsland.Add(island); // ing 170318
            }
            m_aIsland = aIsland;
            m_nIsland = m_aIsland.Count;
        }

    }
}
