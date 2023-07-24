using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Printing;

namespace ezTools
{  
    public class ezSRP350 
    { 
        PrintDocument pPd; 
        string m_strLine;
        Font m_Font;
        int m_nCount;

        public void AddText(string strLine) 
        {
            m_strLine += strLine;
            m_nCount++; 
        } 

        void pPd_PrintPage(object sender, PrintPageEventArgs e) 
        { 
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            e.Graphics.DrawString(m_strLine, m_Font, Brushes.Black, 10, m_nCount);
        } 

        public void Print(Font font) 
        {
            m_Font = font;
            pPd = new PrintDocument();
            pPd.DefaultPageSettings.PrinterResolution.Kind = PrinterResolutionKind.High;
            pPd.PrintPage += new PrintPageEventHandler(pPd_PrintPage);
            pPd.Print();
        }
        public void ClearPrint()
        {
            m_strLine = "";
            m_nCount = 0;
        }

    }
}
