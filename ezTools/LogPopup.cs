using System;
using System.Collections; 
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ezTools
{
    public partial class LogPopup : DockContent
    {
        enum eColor
        {
            Red,
            Yellow,
            Green,
            Blue,
            White
        }
        string[] m_strColors = Enum.GetNames(typeof(eColor));
        string[,] m_strColor = new string[3, 2]; 

        const int c_nLogPopup = 12;

        bool m_bEnable = true;  //박상영20180123
        public string m_id, m_strFont = "Arial";
        public int m_nFont = 12;

        int m_nSec = 0;
        Queue m_aLogDat = new Queue();
        eLogLv m_eLogLv = eLogLv.Add;
        Color[,] m_color = new Color[3, 2];
        ImgPopup m_imgPopup;
        DockPanel m_dockPanel;

        public LogPopup(string id, string strModel, DockPanel dockPanel, ImgPopup imgPopup = null)
        {
            TabText = m_id = id;
            m_dockPanel = dockPanel;
            InitializeComponent();
            m_strColor[0, 0] = eColor.White.ToString();
            m_strColor[0, 1] = eColor.White.ToString();
            m_strColor[1, 0] = eColor.Green.ToString();
            m_strColor[1, 1] = eColor.White.ToString();
            m_strColor[2, 0] = eColor.Red.ToString();
            m_strColor[2, 1] = eColor.Yellow.ToString();
            SetColor(0, 0);
            SetColor(0, 1);
            m_imgPopup = imgPopup;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (m_nSec == 0) return;
            m_nSec--; this.Invalidate(); 
        }

        private void LogPopup_MouseEnter(object sender, EventArgs e)
        {
            m_nSec = 0; this.Invalidate();
        }

        private void LogPopup_FormClosing(object sender, FormClosingEventArgs e)
        {
            Control control = (Control)sender;
            e.Cancel = true; 
            this.Hide();
            if (m_imgPopup != null) m_imgPopup.Hide();
        }

        private void LogPopup_Paint(object sender, PaintEventArgs e)
        {
            int y = 5; int rgb; 
            Graphics dc = e.Graphics;
            Font font = new Font(m_strFont, m_nFont);
            if (m_nSec == 0)
            {
                this.BackColor = m_color[0, 0];
                m_eLogLv = eLogLv.Add; 
            }
            else this.BackColor = m_color[(int)m_eLogLv, m_nSec % 2];
            y = y - (2 * m_aLogDat.Count * m_nFont) + this.Size.Height;
            if (y > 5) y = 5; 
            foreach (LogDat logDat in m_aLogDat)
            {
                rgb = (int)logDat.GetPeriodSec(); if (rgb > 200) rgb = 200; 
                if (y > -m_nFont) dc.DrawString(logDat.GetLog(), font, new SolidBrush(Color.FromArgb(rgb, rgb, rgb)), 10, y);
                y += (2 * m_nFont);
            }     
        }

        public void Popup(LogDat logDat)
        {
            if (!m_bEnable) return; //박상영20180123
            m_aLogDat.Enqueue(logDat); 
            m_nSec = 60;
            if (m_eLogLv < logDat.m_eLogLv) m_eLogLv = logDat.m_eLogLv; 
            if (m_aLogDat.Count >= c_nLogPopup) m_aLogDat.Dequeue();
            if (this.IsHidden) this.Show(m_dockPanel);
        }

        public void RunGrid(ezGrid rGrid)
        {
            rGrid.Set(ref m_bEnable, "Popup", "Enable", "Enable Popup Log");    //박상영20180123
            rGrid.Set(ref m_strFont, "Popup", "Font", "Popup Font Name");
            rGrid.Set(ref m_nFont, "Popup", "Size", "Popup Font Size");
            rGrid.Set(ref m_strColor[1, 0], m_strColors, "Popup", "PopupColor0", "Popup Toggle Color");
            rGrid.Set(ref m_strColor[1, 1], m_strColors, "Popup", "PopupColor1", "Popup Toggle Color");
            rGrid.Set(ref m_strColor[2, 0], m_strColors, "Popup", "ErrorColor0", "Error Toggle Color");
            rGrid.Set(ref m_strColor[2, 1], m_strColors, "Popup", "ErrorColor1", "Error Toggle Color");
            SetColor(1, 0);
            SetColor(1, 1);
            SetColor(2, 0);
            SetColor(2, 1); 
        }

        void SetColor(int nLogLv, int nToggle)
        {
            for (int n = 0; n < m_strColors.Length; n++)
            {
                if (m_strColor[nLogLv, nToggle] == ((eColor)n).ToString())
                {
                    m_color[nLogLv, nToggle] = SetColor((eColor)n); 
                    return ; 
                }
            }
            m_color[nLogLv, nToggle] = SetColor(eColor.White); 
        }

        Color SetColor(eColor nColor)
        {
            switch (nColor)
            {
                case eColor.Blue: return Color.FromArgb(0, 0, 200);
                case eColor.Green: return Color.FromArgb(0, 200, 0);
                case eColor.Red: return Color.FromArgb(200, 0, 0);
                case eColor.Yellow: return Color.FromArgb(200, 200, 0); 
            }
            return Color.FromArgb(230, 230, 230);
        }
    }
}
