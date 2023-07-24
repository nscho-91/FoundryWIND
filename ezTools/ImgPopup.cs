using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ezTools
{
    public partial class ImgPopup : Form
    {
        bool m_bEnable = true;  //박상영20180123
        string m_id, m_strPath, m_strError;
        CPoint m_cp, m_sz;
        ezRegistry m_reg;
        Bitmap m_bmp = null;

        public ImgPopup(string id, string strModel)
        {
            InitializeComponent();
            m_id = id; m_strPath = "D:\\ErrorImg";
            m_reg = new ezRegistry(strModel, m_id);
            m_reg.Read("sz", ref m_sz); this.Size = (Size)m_sz.ToPoint();
            m_reg.Read("cp", ref m_cp); this.Location = m_cp.ToPoint();
        }

        private void ImgPopup_FormClosing(object sender, FormClosingEventArgs e)
        {
            Control control = (Control)sender;
            Point cp = control.Location;
            Size sz = control.Size;
            e.Cancel = true;
            if (control.Name == "ImgPopup")
            {
                m_cp = new CPoint(cp); m_reg.Write("cp", m_cp);
                m_sz = new CPoint((Point)sz); m_reg.Write("sz", m_sz);
            }
            this.Hide(); 
        }

        private void ImgPopup_Paint(object sender, PaintEventArgs e)
        {
            if (m_bmp == null) { Hide(); return; }
            Text = "ImgPopup - " + "[" + m_strError + "]";
            e.Graphics.DrawImage(m_bmp, 0, 0, Width, Height);
            e.Graphics.DrawString(m_strError, new Font("Arial", 20, FontStyle.Regular), new SolidBrush(Color.Red), new PointF(10, 10));
        }
        private void ImgPopup_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        public void RunGrid(ezGrid rGrid)
        {
            rGrid.Set(ref m_bEnable, "ErrorImg", "Enable", "Enable Popup Image");    //박상영20180123
            rGrid.Set(ref m_strPath, "ErrorImg", "Path", "Error Image Path");
        }

        public void Popup(string id, string strError)
        {
            if (!m_bEnable) return; //박상영20180123
            m_strError = strError;
            try { m_bmp = new Bitmap(m_strPath + "\\" + id + "_" + m_strError + ".bmp"); }
            catch { m_bmp = null; this.Hide(); return; }
            ShowForm();
        }

        public void ShowForm()
        {
            if (m_bmp != null) Show(); 
        }
    }
}
