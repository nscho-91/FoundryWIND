using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ezTools; 

namespace ezAxis
{
    public partial class DIO_Ajin_ID : Form
    {
        string m_id, m_strID;
        int m_nID;
        CPoint m_cp = new CPoint(0, 0); 
        Brush[,] m_brush = new Brush[2, 2]; 

        public DIO_Ajin_ID(string id, int nID)
        {
            InitializeComponent();
            m_nID = nID; m_id = id + m_nID.ToString("00"); m_strID = m_id; 
            m_brush[0, 0] = new SolidBrush(Color.FromArgb(200, 200, 200));
            m_brush[0, 1] = new SolidBrush(Color.FromArgb(100, 100, 100));
            m_brush[1, 0] = new SolidBrush(Color.FromArgb(200, 100, 100));
            m_brush[1, 1] = new SolidBrush(Color.FromArgb(255, 0, 0)); 
        }

        private void button_Click(object sender, EventArgs e)
        {
            m_strID = text.Text; Close(); 
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

    }
}
