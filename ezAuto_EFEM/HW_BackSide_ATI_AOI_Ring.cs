using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ezTools;
using System.IO;
using System.Windows.Forms;



namespace ezAuto_EFEM
{
    class HW_BackSide_ATI_AOI_Ring
    {
        public string m_id;

        public int m_nSearchPosition = 3500;
        public int m_nSearchHeight = 200;
        public int m_nSearchWidth = 500;

        public string m_strCode = "000";
        string m_strColor;
        string[] m_strColors;
        Color[] m_colors = new Color[] { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Purple, Color.Pink, Color.Orange, Color.Ivory, Color.Chocolate };
        public Color m_color = Color.Yellow;

        public int m_nLineThickness = 30;
        int m_nLineLength = 120;    // 4의 배수

        int m_nIntensity = 50;

        bool m_bSave = false;
        bool m_bUse = true;
        Log m_log;

        CPoint m_cpCenter = new CPoint(0, 0);
        CPoint m_szImg = new CPoint(0, 0);
        HW_BackSide_ATI_AOIData m_data;
        int m_nDNum = 0;
        int m_nMin = 5;
        public ArrayList m_aPoint = new ArrayList();
        public ArrayList m_aSocre = new ArrayList();

        public HW_BackSide_ATI_AOI_Ring()
        {
        }

        public void Init(string id, HW_BackSide_ATI_AOIData data, Log log)
        {
            m_id = id;
            m_data = data;
            m_log = log;
            m_strColors = new string[m_colors.Length];
            m_strColor = m_colors[0].ToString();
            for (int n = 0; n < m_strColors.Length; n++)
            {
                m_strColors[n] = m_colors[n].ToString();
            }
        }

        public void DrawRing(Graphics dc, ezImgView view, bool bDraw)
        {
            int i = 0;
            if (bDraw == false || !IsDefect()) return;
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
                foreach (Rectangle rt in m_aPoint)
                {
                    view.DrawLine(dc, m_color, new CPoint(rt.Left, rt.Top), new CPoint(rt.Right, rt.Top));
                    view.DrawLine(dc, m_color, new CPoint(rt.Right, rt.Top), new CPoint(rt.Right, rt.Bottom));
                    view.DrawLine(dc, m_color, new CPoint(rt.Right, rt.Bottom), new CPoint(rt.Left, rt.Bottom));
                    view.DrawLine(dc, m_color, new CPoint(rt.Left, rt.Bottom), new CPoint(rt.Left, rt.Top));
                    view.DrawString(dc, "ring:" + m_aSocre[i++].ToString(), new CPoint(rt.Right, rt.Top));
                }
            }
            catch (Exception ex)
            {
                m_log.Add(ex.Message);
            }
        }

        public void RunGrid(ezGrid grid, eGrid eMode, ezJob job = null)
        {
            grid.Set(ref m_bUse, m_id, "Use", "Use");
            grid.Set(ref m_nSearchPosition, m_id, "Search Position(pxl)", "the Radius to search ringmark");
            grid.Set(ref m_nSearchHeight, m_id, "Search height(pxl)", "height of search box");
            grid.Set(ref m_nSearchWidth, m_id, "Search width(pxl)", "width of search box");

            grid.Set(ref m_nLineThickness, m_id, "Line Thickness(pxl)", "Thickness of ringmark");
            grid.Set(ref m_nLineLength, m_id, "Line Length(pxl)", "size of ringmark template (100)");

            grid.Set(ref m_nIntensity, m_id, "Intensity(GV)", "Intensity of matching (10~50)");
            grid.Set(ref m_bSave, m_id, "Save image", "save processing image");
            grid.Set(ref m_nMin, m_id, "Box", "Min of Box");
            grid.Set(ref m_strColor, m_strColors, m_id, "Color", "Draw Color");
            grid.Set(ref m_strCode, m_id, "Code", "Defect Code");
        }

        public bool Inspect(ezImg img)
        {
            if (!img.HasImage()) return true;
            if (!m_bUse) return false;
            ezStopWatch sw = new ezStopWatch();
            img = m_data.m_imgDown;
            m_szImg = img.m_szImg;            
            m_cpCenter = m_data.m_cpCenterDown;
            while( m_nSearchWidth % (4*m_data.m_nDown) != 0 )
            {
                m_nSearchWidth++;
            }
            while (m_nLineLength % (4 * m_data.m_nDown) != 0 )
            {
                m_nLineLength++;
            }
            ippTools ipp = new ippTools();
            CPoint cpTemplate = new CPoint(m_nLineLength / m_data.m_nDown, m_nLineLength / m_data.m_nDown);
            m_nDNum = 0;
            m_aPoint.Clear();
            m_aSocre.Clear();
            m_log.Add(m_id + " - Start RingMark Inspection.");
            if (m_nLineThickness < m_data.m_nDown) m_nLineThickness = m_data.m_nDown;
            ezImg rotateImg = new ezImg("RotateImg", m_log);
            ezImg srcImg = img;
            ezImg template = new ezImg("template", m_log);
            template.ReAllocate(cpTemplate, 1);

            MakeTemplate(template);
            FindRing0(img, template, m_cpCenter, ipp, 0);
            FindRing90(img, template, m_cpCenter, ipp, 0);

            rotateImg.ReAllocate(img);
            ipp.ippiRotate(rotateImg, rotateImg.m_szImg, new Rectangle(0, 0, rotateImg.m_szImg.x, rotateImg.m_szImg.y), srcImg, srcImg.m_szImg, new Rectangle(0, 0, srcImg.m_szImg.x, srcImg.m_szImg.y), -30, m_cpCenter.x, m_cpCenter.y, 4/*cubic*/);
            FindRing0(rotateImg, template, m_cpCenter, ipp, 30);
            FindRing90(rotateImg, template, m_cpCenter, ipp, -30);

            if (m_bSave)
                rotateImg.GetBitmap().Save("d:\\ring_rotateimage30.bmp");
 
            ipp.ippiRotate(rotateImg, rotateImg.m_szImg, new Rectangle(0, 0, rotateImg.m_szImg.x, rotateImg.m_szImg.y), srcImg, srcImg.m_szImg, new Rectangle(0, 0, srcImg.m_szImg.x, srcImg.m_szImg.y), -60, m_cpCenter.x, m_cpCenter.y, 4/*cubic*/);
            FindRing0(rotateImg, template, m_cpCenter, ipp, 60);
            FindRing90(rotateImg, template, m_cpCenter, ipp, -60);

            if (m_bSave)
                rotateImg.GetBitmap().Save("d:\\ring_rotateimage60.bmp");

            m_log.Add(m_id + " - AOI Ring : " + sw.Check().ToString());
            return false;
        }

        public void FindRing0(ezImg img, ezImg template, CPoint cpCenter, ippTools ipp, float dAngle)
        {
            // clip src image
            int nSearchHeight = m_nSearchHeight / m_data.m_nDown;
            int nSearchPosition = m_nSearchPosition / m_data.m_nDown;
            int nSearchWidth = m_nSearchWidth / m_data.m_nDown;

            ezImg srcImg = new ezImg("SrcImg", m_log);
            CPoint szClip = new CPoint(nSearchWidth, template.m_szImg.y + nSearchHeight * 2);
            CPoint cpMax, cpClipStart, cpDefect;
            Rectangle rect;
            int nMax, nMax1;
            srcImg.ReAllocate(szClip, 1);
            cpMax = new CPoint(0, 0);

            // top - center
            cpClipStart = new CPoint(cpCenter.x - nSearchWidth / 2, cpCenter.y - template.m_szImg.y / 2 + nSearchPosition);
            srcImg.Copy(img, cpClipStart, new CPoint(0, 0), szClip);
            nMax = nMax1 = doInspRingMark(srcImg, template, ipp, ref cpMax);

            // top - left
            cpClipStart = new CPoint(cpCenter.x - nSearchWidth / 2 - nSearchWidth, cpCenter.y - template.m_szImg.y / 2 + nSearchPosition);
            srcImg.Copy(img, cpClipStart, new CPoint(0, 0), szClip);
            nMax1 = doInspRingMark(srcImg, template, ipp, ref cpMax);
            if (nMax1 > nMax)
                nMax = nMax1;
            // top - right
            cpClipStart = new CPoint(cpCenter.x - nSearchWidth / 2 + nSearchWidth, cpCenter.y - template.m_szImg.y / 2 + nSearchPosition);
            srcImg.Copy(img, cpClipStart, new CPoint(0, 0), szClip);
            nMax1 = doInspRingMark(srcImg, template, ipp, ref cpMax);
            if (nMax1 > nMax)
                nMax = nMax1;

            if (nMax > m_nIntensity)
            {
                cpDefect = new CPoint(cpCenter.x * m_data.m_nDown, cpCenter.y * m_data.m_nDown + m_nSearchPosition);
                cpDefect = GetRotationPos(dAngle, cpDefect, cpCenter * m_data.m_nDown);
                rect = new Rectangle(cpDefect.x - m_nSearchWidth * 2, cpDefect.y - m_nSearchWidth * 2, m_nSearchWidth * 4, m_nSearchWidth * 4);
                m_aPoint.Add(rect);
                m_aSocre.Add(nMax);
                m_nDNum++;
            }
            
            // bot - center
            cpClipStart = new CPoint(cpCenter.x - nSearchWidth / 2, cpCenter.y - template.m_szImg.y / 2 - nSearchPosition);
            srcImg.Copy(img, cpClipStart, new CPoint(0, 0), szClip);
            nMax = nMax1 = doInspRingMark(srcImg, template, ipp, ref cpMax);

            // bot - left
            cpClipStart = new CPoint(cpCenter.x - nSearchWidth / 2 - nSearchWidth, cpCenter.y - template.m_szImg.y / 2 - nSearchPosition);
            srcImg.Copy(img, cpClipStart, new CPoint(0, 0), szClip);
            nMax1 = doInspRingMark(srcImg, template, ipp, ref cpMax);
            if (nMax1 > nMax)
                nMax = nMax1;

            // bot - right
            cpClipStart = new CPoint(cpCenter.x - nSearchWidth / 2 + nSearchWidth, cpCenter.y - template.m_szImg.y / 2 - nSearchPosition);
            srcImg.Copy(img, cpClipStart, new CPoint(0, 0), szClip);
            nMax1 = doInspRingMark(srcImg, template, ipp, ref cpMax);
            if (nMax1 > nMax)
                nMax = nMax1;

            if (nMax > m_nIntensity)
            {
                cpDefect = new CPoint(cpCenter.x * m_data.m_nDown, cpCenter.y * m_data.m_nDown - m_nSearchPosition);
                cpDefect = GetRotationPos(dAngle, cpDefect, cpCenter * m_data.m_nDown);
                rect = new Rectangle(cpDefect.x - m_nSearchWidth * 2, cpDefect.y - m_nSearchWidth * 2, m_nSearchWidth * 4, m_nSearchWidth * 4);
                m_aPoint.Add(rect);
                m_aSocre.Add(nMax);
                m_nDNum++;
            }
        }

        public void FindRing90(ezImg img, ezImg template, CPoint cpCenter, ippTools ipp, float dAngle)
        {
            int nSearchHeight = m_nSearchHeight / m_data.m_nDown;
            int nSearchPosition = m_nSearchPosition / m_data.m_nDown;
            int nSearchWidth = m_nSearchWidth / m_data.m_nDown;

            // clip src image
            ezImg srcImg = new ezImg("SrcImg", m_log);
            ezImg srcRotatedImg = new ezImg("RotatedSrcImg", m_log);
            CPoint szClip = new CPoint(template.m_szImg.y + nSearchHeight * 2, nSearchWidth);
            CPoint szRotateClip = new CPoint(nSearchWidth, template.m_szImg.y + nSearchHeight * 2);
            CPoint cpMax, cpClipStart, cpDefect;
            Rectangle rect;
            int nMax, nMax1;
            srcImg.ReAllocate(szClip, 1);
            srcRotatedImg.ReAllocate(szRotateClip, 1);
            cpMax = new CPoint(0, 0);

            // left - center
            cpClipStart = new CPoint(cpCenter.x - template.m_szImg.y / 2 - nSearchPosition, cpCenter.y - nSearchWidth / 2);
            srcImg.Copy(img, cpClipStart, new CPoint(0, 0), szClip);
            for (int y = 0; y < srcImg.m_szImg.y; y++)
            {
                for (int x = 0; x < srcImg.m_szImg.x; x++)
                {
                    srcRotatedImg.m_aBuf[x, y] = srcImg.m_aBuf[y, x];
                }
            }
            nMax = nMax1 = doInspRingMark(srcRotatedImg, template, ipp, ref cpMax);

            // left - top
            cpClipStart = new CPoint(cpCenter.x - template.m_szImg.y / 2 - nSearchPosition, cpCenter.y - nSearchWidth / 2 + nSearchWidth);
            srcImg.Copy(img, cpClipStart, new CPoint(0, 0), szClip);
            for (int y = 0; y < srcImg.m_szImg.y; y++)
            {
                for (int x = 0; x < srcImg.m_szImg.x; x++)
                {
                    srcRotatedImg.m_aBuf[x, y] = srcImg.m_aBuf[y, x];
                }
            }
            nMax1 = doInspRingMark(srcImg, template, ipp, ref cpMax);
            if (nMax1 > nMax)
                nMax = nMax1;

            // left - bottom
            cpClipStart = new CPoint(cpCenter.x - template.m_szImg.y / 2 - nSearchPosition, cpCenter.y - nSearchWidth / 2 - nSearchWidth);
            srcImg.Copy(img, cpClipStart, new CPoint(0, 0), szClip);
            for (int y = 0; y < srcImg.m_szImg.y; y++)
            {
                for (int x = 0; x < srcImg.m_szImg.x; x++)
                {
                    srcRotatedImg.m_aBuf[x, y] = srcImg.m_aBuf[y, x];
                }
            }
            nMax1 = doInspRingMark(srcImg, template, ipp, ref cpMax);
            if (nMax1 > nMax)
                nMax = nMax1;

            if (nMax > m_nIntensity)
            {
                cpDefect = new CPoint(cpCenter.x * m_data.m_nDown - m_nSearchPosition, cpCenter.y * m_data.m_nDown);
                cpDefect = GetRotationPos(dAngle, cpDefect, cpCenter * m_data.m_nDown);
                rect = new Rectangle(cpDefect.x - m_nSearchWidth * 2, cpDefect.y - m_nSearchWidth * 2, m_nSearchWidth * 4, m_nSearchWidth * 4);
                m_aPoint.Add(rect);
                m_aSocre.Add(nMax);
                m_nDNum++;
            }

            // right - center
            cpClipStart = new CPoint(cpCenter.x - template.m_szImg.y / 2 + nSearchPosition, cpCenter.y - nSearchWidth / 2);
            srcImg.Copy(img, cpClipStart, new CPoint(0, 0), szClip);
            for (int y = 0; y < srcImg.m_szImg.y; y++)
            {
                for (int x = 0; x < srcImg.m_szImg.x; x++)
                {
                    srcRotatedImg.m_aBuf[x, y] = srcImg.m_aBuf[y, x];
                }
            }
            nMax1 = nMax = doInspRingMark(srcRotatedImg, template, ipp, ref cpMax);

            // right - top
            cpClipStart = new CPoint(cpCenter.x - template.m_szImg.y / 2 + nSearchPosition, cpCenter.y - nSearchWidth / 2 + nSearchWidth);
            srcImg.Copy(img, cpClipStart, new CPoint(0, 0), szClip);
            for (int y = 0; y < srcImg.m_szImg.y; y++)
            {
                for (int x = 0; x < srcImg.m_szImg.x; x++)
                {
                    srcRotatedImg.m_aBuf[x, y] = srcImg.m_aBuf[y, x];
                }
            }
            nMax1 = doInspRingMark(srcImg, template, ipp, ref cpMax);
            if (nMax1 > nMax)
                nMax = nMax1;

            // right - bottom
            cpClipStart = new CPoint(cpCenter.x - template.m_szImg.y / 2 + nSearchPosition, cpCenter.y - nSearchWidth / 2 - nSearchWidth);
            srcImg.Copy(img, cpClipStart, new CPoint(0, 0), szClip);
            for (int y = 0; y < srcImg.m_szImg.y; y++)
            {
                for (int x = 0; x < srcImg.m_szImg.x; x++)
                {
                    srcRotatedImg.m_aBuf[x, y] = srcImg.m_aBuf[y, x];
                }
            }
            nMax1 = doInspRingMark(srcImg, template, ipp, ref cpMax);
            if (nMax1 > nMax)
                nMax = nMax1;

            if (nMax > m_nIntensity)
            {
                cpDefect = new CPoint(cpCenter.x * m_data.m_nDown + m_nSearchPosition, cpCenter.y * m_data.m_nDown);
                cpDefect = GetRotationPos(dAngle, cpDefect, cpCenter * m_data.m_nDown);
                rect = new Rectangle(cpDefect.x - m_nSearchWidth * 2, cpDefect.y - m_nSearchWidth * 2, m_nSearchWidth * 4, m_nSearchWidth * 4);
                m_aPoint.Add(rect);
                m_aSocre.Add(nMax);
                m_nDNum++;
            }

        }

        public int doInspRingMark(ezImg srcImg, ezImg template, ippTools ipp, ref CPoint cpMax)
        {
            if (m_bSave)
                srcImg.GetBitmap().Save("D:\\ring_input.bmp");

            ipp.ippiCrossCorrForRingmark(template, srcImg, new CPoint(0, 0), srcImg.m_szImg);

            int nW = m_nLineLength * 2 / m_data.m_nDown;
            int nH = m_nLineThickness * 2 / m_data.m_nDown;
            int nSum;
            int nAvg;
            int nMax = 0;
            cpMax = new CPoint(0, 0);

            // for speed to reduce time
            for (int y = 0; y < srcImg.m_szImg.y - nH; y++)
            {
                nSum = 0;
                nAvg = 0;

                for (int yy = 0; yy < nH; yy++)
                {
                    for (int xx = 0; xx < nW; xx++)
                    {
                        nSum += srcImg.m_aBuf[y + yy, xx];
                    }
                }
                if (nSum > 0) nAvg = nSum / (nW * nH);
                if (nAvg > nMax)
                {
                    nMax = nAvg;
                    cpMax = new CPoint(0, y);
                }

                for (int x = 1; x < srcImg.m_szImg.x - nW; x++)
                {
                    nAvg = 0;
                    for (int yy = 0; yy < nH; yy++)
                    {
                        nSum -= srcImg.m_aBuf[y + yy, x - 1];
                        nSum += srcImg.m_aBuf[y + yy, x + nW - 1];
                    }

                    if (nSum > 0) nAvg = nSum / (nW * nH);
                    if (nAvg > nMax)
                    {
                        nMax = nAvg;
                        cpMax = new CPoint(x, y);
                    }
                }
            }

            if (m_bSave)
                srcImg.GetBitmap().Save("d:\\ring_output.bmp");

            return nMax;
        }

        public void MakeTemplate(ezImg template)
        {
            // make template.
            for (int i = 0; i < template.m_szImg.y; i++)
            {
                for (int j = 0; j < template.m_szImg.x; j++)
                {
                    template.m_aBuf[i, j] = 255;
                }
            }

            int nStartY = template.m_szImg.y / 2 - (m_nLineThickness / 2 / m_data.m_nDown);

            for (int i = 0; i < m_nLineThickness / m_data.m_nDown; i++)
            {
                for (int j = 0; j < template.m_szImg.x; j++)
                {
                    template.m_aBuf[i + nStartY, j] = 0;
                }
            }

            if (m_bSave)
                template.GetBitmap().Save("D:\\ring_template.bmp");
     
        }

        public CPoint GetRotationPos(float fAngle, CPoint ptCurrent, CPoint cpCenter)
        {
            double fT = Math.PI * fAngle / 180;
            double fCos = Math.Cos(fT);
            double fSin = Math.Sin(fT);

            CPoint cpTemp = new CPoint(ptCurrent.x, ptCurrent.y);
            CPoint cpResult = new CPoint(0, 0);

            cpTemp -= cpCenter;

            cpResult.x = (int)(cpTemp.x * fCos + cpTemp.y * fSin);
            cpResult.y = (int)(cpTemp.y * fCos + cpTemp.x * fSin);

            cpResult += cpCenter;

            return cpResult;
        }

        public bool IsDefect()
        {
            if (m_nMin <= m_nDNum) return true;
            return false;
        }

        public void ClearDefect()
        {
            m_nDNum = 0;
        }
    }
}