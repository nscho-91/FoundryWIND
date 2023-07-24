using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading; 
using System.Threading.Tasks;
using System.Windows.Forms;
using ezTools; 

namespace ezAxis
{
    public partial class DO_Ajin_ID : Form
    {
        string m_id, m_strID;
        int m_nID;
        
        CPoint m_cp = new CPoint(0, 0);
        Brush[,] m_brush = new Brush[2, 2];

        bool m_bRun = false;
        Thread m_thread;

        public delegate void WriteOutput(int nDO, bool bOn);
        public event WriteOutput m_cbWriteOutput;

        public DO_Ajin_ID(string id, int nID)
        {
            InitializeComponent();
            m_nID = nID; 
            m_id = id + m_nID.ToString("00"); 
            m_strID = m_id;
            m_brush[0, 0] = new SolidBrush(Color.FromArgb(200, 200, 200));
            m_brush[0, 1] = new SolidBrush(Color.FromArgb(100, 100, 100));
            m_brush[1, 0] = new SolidBrush(Color.FromArgb(200, 100, 100));
            m_brush[1, 1] = new SolidBrush(Color.FromArgb(255, 0, 0));
        }

        void ThreadStop()
        {
            if (m_bRun) { m_bRun = false; m_thread.Join(); }
        }

        private void button_Click(object sender, EventArgs e)
        {
            m_strID = text.Text;
            ThreadStop(); 
            Close(); 
        }

        private void checkRepeat_CheckedChanged(object sender, EventArgs e)
        {
            if (m_bRun) ThreadStop(); 
            else
            {
                m_thread = new Thread(new ThreadStart(RunThread));
                m_thread.Start();
            }
        }

        private void DO_Ajin_ID_FormClosing(object sender, FormClosingEventArgs e)
        {
            ThreadStop(); 
        }

        public void Draw(Graphics dc, bool bOn, CPoint cp)
        {
            int nOn, nCap;
            m_cp = cp;
            if (bOn) nOn = 1; else nOn = 0;
            if (m_id == m_strID) nCap = 0; else nCap = 1;
            dc.DrawString(m_nID.ToString("00"), this.Font, Brushes.Gray, cp.x, cp.y);
            dc.DrawString(m_strID, this.Font, m_brush[nOn, nCap], cp.x + 30, cp.y);
        }

        public bool IsClick(int xp, int yp)
        {
            if ((xp < m_cp.x) || (xp > (m_cp.x + 100))) return false;
            if ((yp < m_cp.y) || (yp > (m_cp.y + 18))) return false;
            return true;
        }

        public void SetCaption(string strID)
        {
            m_strID = strID;
        }

        private void text_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue != 13) return;
            m_strID = text.Text; Close();
        }

        public void ShowEditBox()
        {
            text.Text = m_strID; ShowDialog();
        }

        void RunThread()
        {
            m_bRun = true; 
            while (m_bRun)
            {
                m_cbWriteOutput(m_nID, true);
                Thread.Sleep(1000);
                m_cbWriteOutput(m_nID, false);
                Thread.Sleep(1000);
            }
        }

    }
}
