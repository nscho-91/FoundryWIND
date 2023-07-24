using System;
using System.Drawing;
using System.Collections; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ezTools 
{
    public class ezImgView 
    {
        const double c_fMaxZoom = 8; 

        public string m_id;
        public ezImg[] m_aImg;
        public ezImg m_pImg;
        public ezImg m_pImg_Big;
        public int m_nImg = 0;
        public double m_fZoom = 1;
        public Font m_font; 
        
        public RPoint m_rpImg = new RPoint(0, 0);
        CPoint m_szImg = new CPoint(1, 1); 
        CPoint m_szWin = new CPoint(1, 1); 
        Log m_log;
        int m_yShiftWin = 0;
        IntPtr m_hBitmap = new IntPtr(0);

        public ezImgView(string id, int nImg, LogView logView, Font font)
        {
            int n;
            m_id = id; m_nImg = nImg; m_font = font; 
            m_log = new Log(m_id, logView);
            m_aImg = new ezImg[nImg];
            for (n = 0; n < m_nImg; n++) m_aImg[n] = new ezImg(m_id + n.ToString("00"), m_log);
            m_pImg = m_aImg[0];
            
        }

        ~ezImgView()
        {
            DeleteObject(m_hBitmap);
        }

        public bool HasImage()
        {
            if (m_pImg == null) return false;
            return m_pImg.HasImage(); 
        }

        public void MakeBigImage()
        {
            CPoint cpbig = new CPoint(1376*5,270000/5);
            m_pImg_Big = new ezImg("big", m_log);
            m_pImg_Big.ReAllocate(cpbig, 1);

            CPoint cpSrc = new CPoint();
            CPoint cpDst = new CPoint();
            CPoint size = new CPoint(1376, 270000 / 5);
            for (int i = 0; i < 5; i++) {
                cpSrc = new CPoint(0,54000*i);
                cpDst = new CPoint(1376*i,0);
                m_pImg_Big.Copy(m_pImg, cpSrc, cpDst, size);
            }
                
        }

        public CPoint GetImgPos(CPoint cpWin)
        {
            cpWin.y = m_szWin.y - cpWin.y - m_yShiftWin;
            cpWin /= m_fZoom; 
            return m_rpImg.ToCPoint() + cpWin; 
        }

        public CPoint GetWinPos(CPoint cpImg)
        {
            RPoint rpWin = (new RPoint(cpImg) - m_rpImg) * m_fZoom + new RPoint(m_fZoom, m_fZoom);
            CPoint cpWin = rpWin.ToCPoint();  
            cpWin.y = m_szWin.y - cpWin.y - m_yShiftWin;
            return cpWin;
        }

        public CPoint GetWinPos(int x, int y)
        {
            return GetWinPos(new CPoint(x, y));
        }

        public void Draw(Graphics dc, Size szWin)
        {
            m_szWin = new CPoint((Point)szWin); 
            if (!HasImage()) return;
            FixPos(); 
            IntPtr hdc = dc.GetHdc();
            SetStretchBltMode(hdc, 0x03);
            cpp_StretchDIBits(m_pImg.m_szImg.x, m_pImg.m_szImg.y, 8 * m_pImg.m_nByte, hdc, 0, 0, CalcWinRect().Width, CalcWinRect().Height, 
                CalcImgRect().X, CalcImgRect().Y, CalcImgRect().Width, CalcImgRect().Height, m_pImg.GetIntPtr(0, 0), 0, 0x00CC0020);
            dc.ReleaseHdc(hdc);           

            
        }

        public void Draw(Graphics dc)
        {
            m_fZoom = 1; 
            m_szWin = new CPoint(m_pImg.m_szImg);
            if (!HasImage()) return;
            IntPtr hdc = dc.GetHdc();
            SetStretchBltMode(hdc, 0x03);
            cpp_StretchDIBits(m_pImg.m_szImg.x, m_pImg.m_szImg.y, 8 * m_pImg.m_nByte, hdc, 0, 0, m_pImg.m_szImg.x, m_pImg.m_szImg.y,
                0, 0, m_pImg.m_szImg.x, m_pImg.m_szImg.y, m_pImg.GetIntPtr(0, 0), 0, 0x00CC0020);
            dc.ReleaseHdc(hdc);
        }

        public void DrawLine(Graphics dc, Color color, CPoint cp0, CPoint cp1)
        {
            Pen pen = new Pen(color);
            Point p0 = GetWinPos(cp0).ToPoint();
            Point p1 = GetWinPos(cp1).ToPoint();
            dc.DrawLine(pen, p0, p1); 
        }

        public void DrawLines(Graphics dc, Color color, ArrayList listPoint)
        {
            if (listPoint.Count == 0) return; 
            Pen pen = new Pen(color);
            Point[] points = new Point[listPoint.Count];
            for (int n = 0; n < listPoint.Count; n++ )
            {
                points[n] = GetWinPos((CPoint)listPoint[n]).ToPoint(); 
            }
            dc.DrawLines(pen, points); 
        }

        public void DrawRectangle(Graphics dc, Color color, CPoint cp0, CPoint cp1)
        {
            ArrayList list = new ArrayList(); 
            list.Add(cp0);
            list.Add(new CPoint(cp0.x , cp1.y));
            list.Add(cp1);
            list.Add(new CPoint(cp1.x, cp0.y));
            list.Add(cp0);
            DrawLines(dc, color, list); 
        }

        public void DrawRectangle(Graphics dc, Color color, CPoint cp0, CPoint cp1, CPoint cp2, CPoint cp3)
        {
            Brush brush = new SolidBrush(color);
            Point[] aPoint = new Point[4];
            aPoint[0] = GetWinPos(cp0).ToPoint();
            aPoint[1] = GetWinPos(cp1).ToPoint();
            aPoint[2] = GetWinPos(cp2).ToPoint();
            aPoint[3] = GetWinPos(cp3).ToPoint();
            dc.FillPolygon(brush, aPoint);
        }

        public void DrawCircle(Graphics dc, Color color, CPoint cp0, CPoint cp1)
        {
            Pen pen = new Pen(color);
            Point p0 = GetWinPos(cp0).ToPoint();
            Point p1 = GetWinPos(cp1).ToPoint();
            dc.DrawEllipse(pen, p0.X, p0.Y, p1.X - p0.X, p1.Y - p0.Y);
        }

        public void DrawString(Graphics dc, string str, CPoint cp)
        {
            Point p = GetWinPos(cp).ToPoint();
            dc.DrawString(str, this.m_font, Brushes.Black, p); 
        }

        public void DrawString(Graphics dc, string str, CPoint cp, Brush brush)
        {
            Point p = GetWinPos(cp).ToPoint();
            dc.DrawString(str, this.m_font, brush, p);
        }

        Rectangle CalcWinRect()
        {
            CPoint szImg;
            szImg = m_pImg.m_szImg * m_fZoom;
            if (szImg.x > m_szWin.x) szImg.x = m_szWin.x;
            if (szImg.y > m_szWin.y) szImg.y = m_szWin.y;
            return szImg.ToRectangle(); 
        }

        Rectangle CalcImgRect()
        {
            return new Rectangle((int)m_rpImg.x, (int)m_rpImg.y, m_szImg.x, m_szImg.y); 
        }

        public void Shift(CPoint cpShift)
        {
            if (!HasImage()) return;
            RPoint rp = new RPoint(cpShift); 
            m_rpImg.x -= (rp.x / m_fZoom);
            m_rpImg.y += (rp.y / m_fZoom);
            FixPos(); 
        }

        public void Zoom(int nDelta, CPoint cpWin)
        {
            if (!HasImage()) return;
            CPoint cpImg = GetImgPos(cpWin);
            if (nDelta > 0) m_fZoom *= 1.25; else m_fZoom *= 0.8;
            if (m_fZoom > c_fMaxZoom) { m_fZoom = c_fMaxZoom; return; } 
            FixZoom();
            if (nDelta < 0) CheckMinZoom();
            cpWin.y = m_szWin.y - cpWin.y; 
            m_rpImg = new RPoint(cpImg) - (new RPoint(cpWin) / m_fZoom);
            FixPos(); 
        }

        void FixPos()
        {
            CPoint szWin = (m_szWin / m_fZoom);
            CPoint szImg = m_pImg.m_szImg - m_rpImg.ToCPoint();
            m_yShiftWin = (int)(m_szWin.y - m_pImg.m_szImg.y * m_fZoom);
            if (m_yShiftWin < 0) m_yShiftWin = 0;
            if (szImg.x < szWin.x) m_rpImg.x = m_pImg.m_szImg.x - szWin.x;
            if (szImg.y < szWin.y) m_rpImg.y = m_pImg.m_szImg.y - szWin.y;
            if (m_rpImg.x < 0) m_rpImg.x = 0;
            if (m_rpImg.y < 0) m_rpImg.y = 0;
            szImg = m_pImg.m_szImg - m_rpImg.ToCPoint();
            if (szImg.x > szWin.x) m_szImg.x = szWin.x; else m_szImg.x = szImg.x;
            if (szImg.y > szWin.y) m_szImg.y = szWin.y; else m_szImg.y = szImg.y; 
        }

        void FixZoom() 
        {
            int nZoom = 0; 
            if (m_fZoom > 1)
            {
                while (m_fZoom > 1) { nZoom++; m_fZoom *= 0.8; }
                m_fZoom = 1;
                while (nZoom > 0) { nZoom--; m_fZoom *= 1.25; }
            }
            else
            {
                while (m_fZoom < 1) { nZoom++; m_fZoom *= 1.25; }
                m_fZoom = 1;
                while (nZoom > 0) { nZoom--; m_fZoom *= 0.8; }
            }
        }
        
        void CheckMinZoom()
        {
            double fZoom;
            fZoom = Math.Min(1.0 * m_szWin.x / m_pImg.m_szImg.x, 1.0 * m_szWin.y / m_pImg.m_szImg.y);
            if (m_fZoom < fZoom) m_fZoom = fZoom; 
        }

        public void MaxZoomOut()
        {
            m_fZoom = 0;
            CheckMinZoom(); 
        }

        public bool FileOpen(string strFile, bool bGray = false)
        {
            if (m_pImg == null) return true;
            
            m_pImg.FileOpen(strFile, bGray);
            MakeBigImage();
            return true;
        }

        public void FileSave(string strFile)
        {
            if (m_pImg == null) return; 
            m_pImg.FileSave(strFile); 
        }

        public bool IsDrawText()
        {
            return m_fZoom > 0.2; 
        }

        //////////////// DllIport

        [DllImport("gdi32.dll")] 
        public static extern int SetBkColor(IntPtr hdc, int crColor); 

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)] 
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC); 
    
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)] 
        public static extern bool DeleteDC(IntPtr hdc); 
   
        [DllImport("gdi32.dll", ExactSpelling = true)] 
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject); 
    
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)] 
        public static extern bool DeleteObject(IntPtr hObject); 
    
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)] 
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hObject, int width, int height); 
    
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hObjSource, int nXSrc, int nYSrc, Int32 dwRop);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)] 
        public static extern bool StretchBlt(IntPtr hObject, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, IntPtr hObjSource, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, Int32 dwRop); 
    
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)] 
        public static extern bool SetStretchBltMode(IntPtr hObject, int nStretchMode); 

        [DllImport("ezCpp.dll")]
        public static extern void cpp_StretchDIBits(int biWidth, int biHeight, int biBitCount, IntPtr hdc, int XDest, int YDest, int nDestWidth, int nDestHight, int XSrc, int YSrc, int nSrcWidth, int nSrcHeight, IntPtr lpBits, uint iUsage, Int32 dwRop);
    }
}
