using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using ezTools; 

namespace ezAutoMom
{
    public partial class ezView : DockContent, ezView_Mom
    {
        public string m_id;
        public int m_nID; 
        Auto_Mom m_auto; 
        Log m_log;
        public ezImgView m_imgView;

        bool m_bShift = false;
        bool m_bCtrl = false;
        bool m_bAlt = false;
        bool m_bLBD = false;
        CPoint m_cpLBD, m_cpMove;

        public ezView(string id, int nID, Auto_Mom auto)
        {
            InitializeComponent();
            this.TabText = m_id = id; m_nID = nID; m_auto = auto;
            m_log = new Log(m_id, m_auto.ClassLogView());
            m_imgView = new ezImgView(id, 1, m_auto.ClassLogView(), this.Font);
            menuPopup.Hide();
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        public void ThreadStop()
        {

        }

        protected override string GetPersistString()
        {
            return GetType().ToString() + "," + m_id;
        }

        public bool IsPersistString(string str)
        {
            return GetPersistString() == str; 
        }

        private void View_Paint(object sender, PaintEventArgs e)
        {
            Graphics dc = e.Graphics;
            m_imgView.Draw(dc, this.Size);
            m_auto.Draw(dc, m_nID, m_imgView); 
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (m_imgView.HasImage()) return;
            base.OnPaintBackground(e);
        }

        private void View_Resize(object sender, EventArgs e)
        {
            Invalidate(); 
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            m_imgView.Zoom(e.Delta, new CPoint(e.Location));
            SetTabText(m_cpMove); Invalidate();
        }

        void SetTabText(CPoint cpWin)
        {
            int nGV; 
            CPoint cpImg = m_imgView.GetImgPos(cpWin);
            nGV = m_imgView.m_pImg.GetGV(cpImg);
            this.TabText = m_id + nGV.ToString(" 000 ") + m_imgView.m_fZoom.ToString("0.00 ") + cpImg.ToString();
        }

        private void View_MouseDown(object sender, MouseEventArgs e)
        {
            CPoint cpDown = new CPoint(e.Location);
            if (e.Button == MouseButtons.Left)
            {
                m_bLBD = true; m_cpLBD = cpDown; menuPopup.Hide();
                if (m_bShift) m_auto.CheckLBD(m_nID, m_imgView.GetImgPos(cpDown), m_bCtrl); 
            }
            if (e.Button == MouseButtons.Right)
            {
                menuPopup.Location = e.Location;
                menuPopup.Show();
            }
            Invalidate();
        }

        private void View_MouseUp(object sender, MouseEventArgs e)
        {
            CPoint cpUp = new CPoint(e.Location);
            if (m_imgView == null) return; 
            if (e.Button == MouseButtons.Left)
            {
                m_bLBD = false;
                if (m_auto.IsEdit(m_nID)) m_auto.CheckMove(m_nID, m_imgView.GetImgPos(cpUp), true);
            }
            Invalidate();
        }

        private void View_MouseMove(object sender, MouseEventArgs e)
        {
            CPoint cpMove = new CPoint(e.Location);
            if (m_imgView == null) return;
            Focus(); 
            if (m_auto.IsEdit(m_nID)) m_auto.CheckMove(m_nID, m_imgView.GetImgPos(cpMove), false);
            else if (m_bLBD) m_imgView.Shift(cpMove - m_cpMove); 
            m_cpMove = cpMove; SetTabText(m_cpMove); Invalidate(false); 
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "BMP Files (*.bmp)|*.bmp|BMP Gray (*.bmp)|*.bmp|JPG Files (*.jpg)|*.jpg|JPG Gray (*.jpg)|*.jpg";
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            m_imgView.FileOpen(dlg.FileName, (dlg.FilterIndex % 2) == 0); menuPopup.Hide(); Invalidate();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            menuPopup.Hide();
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "BMP Files (*.bmp)|*.bmp|JPG Files (*.jpg)|*.jpg";
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            m_imgView.FileSave(dlg.FileName); 
        }
        
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            menuPopup.Hide();
            if (m_imgView.m_pImg == null) return;
            if (!m_imgView.m_pImg.HasImage()) return;
            m_imgView.m_pImg.Clear();
        }

        private void grabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_auto.Grab(m_nID);
        }

        private void View_KeyDown(object sender, KeyEventArgs e)
        {
            m_bAlt = e.Alt; m_bCtrl = e.Control; m_bShift = e.Shift;
        }

        private void View_KeyUp(object sender, KeyEventArgs e)
        {
            m_bAlt = e.Alt; m_bCtrl = e.Control; m_bShift = e.Shift;
        }

        public ezImg ClassImage()
        {
            return m_imgView.m_pImg;
        }

        public ezImgView ClassImageView()
        {
            return m_imgView;
        }

        public void InvalidView(bool bInvalidate)
        {
            Invalidate(bInvalidate); 
        }

        private void View_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

    }
}
