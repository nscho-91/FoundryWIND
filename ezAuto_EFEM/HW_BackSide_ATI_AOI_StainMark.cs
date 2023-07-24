using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using ezTools;

namespace ezAuto_EFEM
{
    public class HW_BackSide_ATI_AOI_StainMark
    {
        public string m_id;
        Log m_log;
        ezImg m_imgDown;
        //ezImg m_OriImg;

        CPoint m_cpCenter = new CPoint(0, 0);
        CPoint m_szImg = new CPoint(0, 0);
        CPoint m_cpStart = new CPoint(0, 0);
        CPoint m_cpEnd = new CPoint(0, 0);

        HW_BackSide_ATI_AOIData m_data;
        int m_nGV = 130;
        int m_nBlockSize = 200;
        int m_nCnt = 0;
        double m_fPercent = 1;
        double m_fArea = 0;
        double m_nResult = 0.0;
        int m_nOffset = 100;
        int m_nSum = 0;
        int m_nEqualization = 1;
        bool m_bSaveImg = false;
        bool m_bUse = true;
        double m_dR = 0.0;

        public string m_strCode = "000";
        string m_strColor;
        string[] m_strColors;
        Color[] m_colors = new Color[] { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Purple, Color.Pink, Color.Orange, Color.Ivory, Color.Chocolate };
        public Color m_color = Color.Orange;

        public ArrayList m_strList = new ArrayList();
        public ArrayList m_aIsland = new ArrayList();
        public ArrayList m_aMerge = new ArrayList();

        public HW_BackSide_ATI_AOI_StainMark()
        {

        }

        public void Init(string id, HW_BackSide_ATI_AOIData data, Log log)
        {
            m_id = id;
            m_data = data;
            m_log = log;
            m_strList = new ArrayList();
            m_strColors = new string[m_colors.Length];
            m_strColor = m_colors[0].ToString();
            for (int n = 0; n < m_strColors.Length; n++)
            {
                m_strColors[n] = m_colors[n].ToString();
            }
        }

        public void ThreadStop()
        {

        }

        public void RunGrid(ezGrid grid, eGrid eMode, ezJob job = null)
        {
            grid.Set(ref m_bUse, m_id, "Use", "Use");
            grid.Set(ref m_nEqualization, m_id, "Equalization", "The Number Of Area For Histogram Equaliztion");
            grid.Set(ref m_nGV, m_id, "GV", "Gray Value (0 ~ 255)");
            grid.Set(ref m_nBlockSize, m_id, "BlockSize", "Block Size");
            grid.Set(ref m_fPercent, m_id, "Percent", "Percent of Stain Mark");
            grid.Set(ref m_nOffset, m_id, "Offset", "Offset of Inspection Area");
            grid.Set(ref m_bSaveImg, m_id, "Save Image", "save processing image");
            grid.Set(ref m_strColor, m_strColors, m_id, "Color", "Draw Color");
            grid.Set(ref m_strCode, m_id, "Code", "Defect Code");
        }

        public void DrawStain(Graphics dc, ezImgView view, bool bDraw)
        {
            //if (m_OriImg == null || !bDraw) return;
            if (m_imgDown == null || !bDraw) return;
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
                foreach (azBlob.Island island in m_aMerge)
                {
                    view.DrawLine(dc, m_color, island.m_cp0, new CPoint(island.m_cp0.x, island.m_cp1.y));
                    view.DrawLine(dc, m_color, island.m_cp0, new CPoint(island.m_cp1.x, island.m_cp0.y));
                    view.DrawLine(dc, m_color, island.m_cp1, new CPoint(island.m_cp1.x, island.m_cp0.y));
                    view.DrawLine(dc, m_color, island.m_cp1, new CPoint(island.m_cp0.x, island.m_cp1.y));
                    view.DrawString(dc, "Stain Mark", island.m_cp0);
                }
                if (m_bSaveImg)
                {
                    foreach (azBlob.Island island in m_aIsland)
                    {
                        view.DrawLine(dc, m_color, island.m_cp0, new CPoint(island.m_cp0.x, island.m_cp1.y));
                        view.DrawLine(dc, m_color, island.m_cp0, new CPoint(island.m_cp1.x, island.m_cp0.y));
                        view.DrawLine(dc, m_color, island.m_cp1, new CPoint(island.m_cp1.x, island.m_cp0.y));
                        view.DrawLine(dc, m_color, island.m_cp1, new CPoint(island.m_cp0.x, island.m_cp1.y));
                    }
                    foreach (StainString stainString in m_strList)
                    {
                        view.DrawString(dc, stainString.m_strGV, stainString.m_cp);
                    }
                }
            }
            catch (Exception ex)
            {
                m_log.Add(ex.Message);
            }

        }

        public bool Inspect(ezImg img, double dR)
        {
            int n, m, nDownBlock;
            if (m_data.m_nDown == 0 || m_nBlockSize == 0) return true;
            ezStopWatch sw = new ezStopWatch();
            m_imgDown = m_data.m_imgDown;
            nDownBlock = m_nBlockSize / m_data.m_nDown;
            m_aIsland.Clear();
            m_aMerge.Clear();
            m_strList.Clear();
            m_cpCenter = m_data.m_cpCenterDown;
            if (!m_imgDown.HasImage()) return true;
            if (!m_bUse) return false;
            m_log.Add(m_id + " - Start StainMark Inspection.");
            m_dR = (dR - m_nOffset) / m_data.m_nDown;
            HistEqual(m_imgDown);
            //HistEqual(img);
            //m_data.MakeDownImage(img);
            //m_OriImg = m_data.m_imgDown;

            if (CenterNeiborGV())
            {
                m_log.Add(m_id + " - AOI Stain : No Inspection");
                return true;
            }
                
            if (m_bSaveImg)
                m_imgDown.FileSave("D:\\TEST.bmp");
                //m_OriImg.FileSave("D:\\TEST.bmp");

            m_cpStart = new CPoint(Convert.ToInt32(m_data.m_cpCenter.x / m_data.m_nDown - m_dR), Convert.ToInt32(m_data.m_cpCenter.y / m_data.m_nDown - m_dR));
            m_cpEnd = new CPoint(Convert.ToInt32(m_data.m_cpCenter.x / m_data.m_nDown + m_dR), Convert.ToInt32(m_data.m_cpCenter.y / m_data.m_nDown + m_dR));
            m_nSum = m_nCnt = 0;
            m_nResult = 0.0;
            m_fArea = dR * dR * 3.141592;
            for (int i = m_cpStart.y; i < m_cpEnd.y - nDownBlock; i += nDownBlock)
            {
                for (int j = m_cpStart.x; j < m_cpEnd.x - nDownBlock; j += nDownBlock)
                {
                    if (CalDis(new CPoint(j, i)) < m_dR)
                    {
                        for (m = -nDownBlock / 2; m < nDownBlock / 2; m++)
                        {
                            for (n = -nDownBlock / 2; n < nDownBlock / 2; n++)
                            {
                                m_nSum += m_imgDown.GetGV(new CPoint(j + n, i + m));
                                //m_nSum += m_OriImg.GetGV(new CPoint(j + n, i + m));
                            }
                        }

                        m_nSum = m_nSum / (nDownBlock * nDownBlock);//small box
                        if (m_nSum < m_nGV)
                        {
                            azBlob.Island island = new azBlob.Island();
                            StainString stainString;
                            island.m_cp0 = new CPoint(j * m_data.m_nDown, i * m_data.m_nDown);
                            island.m_cp1 = island.m_cp0 + new CPoint(nDownBlock * m_data.m_nDown, nDownBlock * m_data.m_nDown);
                            island.m_nSize = nDownBlock * m_data.m_nDown * nDownBlock * m_data.m_nDown;
                            island.m_strCode = m_strCode;
                            stainString.m_cp = (island.m_cp0 + island.m_cp1) / 2;
                            stainString.m_strGV = m_nSum.ToString();
                            m_aIsland.Add(island);
                            m_strList.Add(stainString);
                            m_nCnt++;
                        }
                    }
                }
            }
            m_aMerge.AddRange(m_aIsland);
            MergeIsland();
            RemoveIsland();
            m_nResult = 100.0 * (double)m_nCnt * (double)m_nBlockSize * (double)m_nBlockSize / m_fArea;
            //img = m_OriImg;
            m_log.Add(m_id + " - StainMark : " + m_nResult.ToString("0.00") + "%");
            m_log.Add(m_id + " - AOI Stain : " + sw.Check().ToString());
            return false;
        }

        bool MergeIsland()
        {
            bool bMerge = false;
            for (int n = 0; n < m_aMerge.Count; n++)
            {
                if (((azBlob.Island)m_aMerge[n]).m_bValid)
                {
                    azBlob.Island island = (azBlob.Island)m_aMerge[n];
                    for (int m = n + 1; m < m_aMerge.Count; m++)
                    {
                        if (((azBlob.Island)m_aMerge[m]).m_bValid)
                        {
                            if (island.Merge((azBlob.Island)m_aMerge[m], m_nBlockSize / 2)) bMerge = true;
                        }
                        if (((azBlob.Island)m_aMerge[n]).m_cp0.x > ((azBlob.Island)m_aMerge[m]).m_cp0.x)
                        {
                            // ing
                        }
                    }
                }
            }
            ClearInvalidIsland();
            return bMerge;
        }

        public void ClearInvalidIsland()
        {
            ArrayList aIsland = new ArrayList();
            foreach (azBlob.Island island in m_aMerge)
            {
                if (island.m_bValid && island.m_nSize > m_fArea * m_fPercent / 100) aIsland.Add(island);
            }
            m_aMerge = aIsland;
        }

        public void RemoveIsland()
        {
            ArrayList tempList = new ArrayList();
            ArrayList tempListString = new ArrayList();
            for (int n = 0; n < m_aMerge.Count; n++)
            {
                for (int m = 0; m < m_aIsland.Count; m++)
                {
                    if (((azBlob.Island)m_aMerge[n]).IsInside((azBlob.Island)m_aIsland[m], m_nBlockSize))
                    {
                        tempList.Add(m_aIsland[m]);
                        tempListString.Add(m_strList[m]);
                    }
                }
            }
            m_aIsland.Clear();
            m_strList.Clear();
            m_aIsland = tempList;
            m_strList = tempListString;
        }

        public double CalDis(CPoint pt)
        {
            int x1 = m_data.m_cpCenter.x / m_data.m_nDown;
            int y1 = m_data.m_cpCenter.y / m_data.m_nDown;
            int x2 = pt.x;
            int y2 = pt.y;
            double fDis = Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
            return fDis;
        }

        public void MeanFilter(ezImg img, int windowSize)
        {
            ezStopWatch sw = new ezStopWatch();
            int Height = img.m_szImg.y;
            int Width = img.m_szImg.x;

            if (windowSize % 2 == 0)
            {
                return;
            }
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    int sum = 0;
                    for (int g = -(windowSize - 1) / 2; g <= (windowSize - 1) / 2; g++)
                    {
                        for (int k = -(windowSize - 1) / 2; k <= (windowSize - 1) / 2; k++)
                        {
                            int a = i + g, b = j + k;
                            if (a < 0) a = 0;
                            if (a > Height - 1) a = Height - 1;
                            if (b < 0) b = 0;
                            if (b > Width - 1) b = Width - 1;
                            sum += img.GetGV(new CPoint(b, a));
                        }
                    }
                    img.m_aBuf[i, j] = (byte)(sum / (windowSize * windowSize));
                }
            }
            m_log.Add("MeanFilter : " + sw.Check().ToString());
        }
        public void HistEqual(ezImg img)
        {
            if (m_nEqualization < 0) m_nEqualization = 1;
            double[,] num = new double[m_nEqualization, 256];
            int Width = img.m_szImg.x;
            int Height = img.m_szImg.y;
            int[] nArea = new int[m_nEqualization];
            for (int n = 0; n < m_nEqualization;n++)
            {

            }
            object obj;
            byte[,] Data = new byte[Height, Width];
            obj = img.m_aBuf.Clone();
            Data = (byte[,])obj;

            for (int n = 0; n < m_nEqualization; n++)
            {
                Parallel.For(0, 256, new ParallelOptions { MaxDegreeOfParallelism = 4 }, i =>
                {
                    num[n, i] = 0;
                });
            }

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (CalDis(new CPoint(j, i)) < m_dR)
                    {
                        num[(j / (Width / m_nEqualization)) % m_nEqualization, Data[i, j]]++;
                        nArea[(j / (Width / m_nEqualization)) % m_nEqualization]++;
                    }
                }
            }

            double[,] newGray = new double[m_nEqualization, 256];

            for (int n = 0; n < m_nEqualization; n++)
            {
                double d = 0;
                Parallel.For(0, 256, new ParallelOptions { MaxDegreeOfParallelism = 4 }, i =>
                {
                    d += num[n, i];
                    //newGray[i] = n * 255 / (Math.PI * m_dR * m_dR);
                    //newGray[n, i] = d * 255 / (Height * Width / 4);
                    newGray[n, i] = d * 255 / nArea[n];
                });
            }

            Parallel.For(0, Height, new ParallelOptions { MaxDegreeOfParallelism = 4 }, i =>
            {
                for (int j = 0; j < Width; j++)
                {
                    if (CalDis(new CPoint(j, i)) < m_dR)
                    {
                        img.m_aBuf[i, j] = (byte)newGray[(j / (Width / m_nEqualization)) % m_nEqualization, Data[i, j]];
                    }
                }
            });
        }

        public bool CenterNeiborGV()
        {
            int SumLeft = 0, SumRight = 0;
            int nDownBlock = m_nBlockSize / m_data.m_nDown;
            //CPoint ptCenter = new CPoint(m_OriImg.m_szImg.x / 2, m_OriImg.m_szImg.y / 2);
            //Rectangle rectLeft = new Rectangle(ptCenter.x - m_OriImg.m_szImg.x / 4, ptCenter.y - nDownBlock, nDownBlock, nDownBlock);
            //Rectangle rectRight = new Rectangle(ptCenter.x + m_OriImg.m_szImg.x / 4, ptCenter.y - nDownBlock, nDownBlock, nDownBlock);
            CPoint ptCenter = new CPoint(m_imgDown.m_szImg.x / 2, m_imgDown.m_szImg.y / 2);
            Rectangle rectLeft = new Rectangle(ptCenter.x - m_imgDown.m_szImg.x / 4, ptCenter.y - nDownBlock, nDownBlock, nDownBlock);
            Rectangle rectRight = new Rectangle(ptCenter.x + m_imgDown.m_szImg.x / 4, ptCenter.y - nDownBlock, nDownBlock, nDownBlock);

            for (int i = rectLeft.Top; i < rectLeft.Bottom; i++)
            {
                for (int j = rectLeft.Left; j < rectLeft.Right; j++)
                {
                    SumLeft += m_imgDown.m_aBuf[i, j];
                }
            }
            SumLeft = SumLeft / (nDownBlock * nDownBlock);

            for (int i = rectRight.Top; i < rectRight.Bottom; i++)
            {
                for (int j = rectRight.Left; j < rectRight.Right; j++)
                {
                    SumRight += m_imgDown.m_aBuf[i, j];
                }
            }
            SumRight = SumRight / (nDownBlock * nDownBlock);


            if (SumLeft > 180 && SumRight > 180)
                return true;
            else
                return false;
        }

        public struct StainString
        {
            public string m_strGV;
            public CPoint m_cp;
        }
    }
}
