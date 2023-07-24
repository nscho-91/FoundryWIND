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
using WeifenLuo.WinFormsUI.Docking;
using ezTools; 

namespace ezAxis
{
    public partial class DI_Ajin : DockContent, DIO_Mom
    {
        string m_id;
        Log m_log;
        ezGrid m_grid;

        int m_nDI0 = 0; 
        int m_lDI = 0;
        int m_nModule0 = 0;
        int[] m_aModule = null;
        uint[] m_aData = null; 
        DIO_Ajin_ID[] m_di = null;

        uint[] m_aComp = new uint[16];

        CPoint m_cpDraw = new CPoint(10, 50);

        bool m_bRunThread = false;
        Thread m_thread;

        public DI_Ajin()
        {
            InitializeComponent();
        }

        public void Init(string id, LogView logView)
        {
            this.TabText = m_id = id;
            m_aComp[0] = 1; 
            for (int n = 1; n < 16; n++) m_aComp[n] = 2 * m_aComp[n - 1];
            m_log = new Log(m_id, logView);
            m_grid = new ezGrid(m_id, grid, m_log, false); 
            grid.Hide();
            RunGrid(eGrid.eRegRead); 
            RunGrid(eGrid.eInit);

            m_aModule = new int[m_lDI];
            m_aData = new uint[6 * m_lDI];
            for (int n = 0; n < m_lDI; n++) m_aModule[n] = m_nModule0 + n;

            comboDI.Items.Clear();
            for (int n = 0; n < m_lDI; n++) comboDI.Items.Add(n.ToString());

            m_di = new DIO_Ajin_ID[96 * m_lDI];
            for (int n = 0; n < 96 * m_lDI; n++) m_di[n] = new DIO_Ajin_ID("DI", n + m_nDI0); 

            m_thread = new Thread(new ThreadStart(RunThread)); 
            m_thread.Start();
        }

        public void ThreadStop()
        {
            if (m_bRunThread) 
            { 
                m_bRunThread = false; 
                m_thread.Join(); 
            }
            for (int n = 0; n < 96 * m_lDI; n++) WriteOutputBit(n + m_nDI0, false);
        }

        protected override string GetPersistString()
        {
            return GetType().ToString() + "," + m_id;
        }

        public bool IsPersistString(string str)
        {
            return GetPersistString() == str;
        }

        private void checkSetup_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkSetup.Checked) grid.Hide();
            else grid.Show();
            Invalidate();
        }

        private void DI_Ajin_Paint(object sender, PaintEventArgs e)
        {
            Graphics dc = e.Graphics; 
            CPoint cp = new CPoint();
            if (checkSetup.Checked) return;
            if (comboDI.SelectedIndex < 0) return;
            cp = m_cpDraw;
            dc.DrawString("INPUT", this.Font, Brushes.Blue, cp.x, cp.y);
            int nDI0 = 96 * comboDI.SelectedIndex; 
            for (int n = 0; n < 48; n++) 
            { 
                cp.y += 18;
                m_di[n + nDI0].Draw(dc, GetInputBit(m_nDI0 + n + nDI0), cp); 
            }
            cp = m_cpDraw; cp.x += (((Control)sender).Size.Width - 20) / 2;
            dc.DrawString("INPUT", this.Font, Brushes.Blue, cp.x, cp.y);
            for (int n = 48; n < 96; n++) 
            {
                cp.y += 18;
                m_di[n + nDI0].Draw(dc, GetInputBit(m_nDI0 + n + nDI0), cp);
            }
        }

        private void comboDI_SelectedIndexChanged(object sender, EventArgs e)
        {
            Invalidate(false); 
        }

        private void DI_Ajin_MouseDown(object sender, MouseEventArgs e)
        {
            if (checkSetup.Checked) return;
            if (comboDI.SelectedIndex < 0) return;
            if (e.Button == MouseButtons.Right)
            {
                int nDI0 = 96 * comboDI.SelectedIndex;
                for (int n = 0; n < 96; n++)
                {
                    if (m_di[n + nDI0].IsClick(e.X, e.Y)) m_di[n + nDI0].ShowEditBox();
                }
            }
            Invalidate(false);
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate); 
            RunGrid(eGrid.eRegWrite); 
        }

        void RunGrid(eGrid eMode, ezJob job = null)
        {
            m_grid.Update(eMode, job);
            m_grid.Set(ref m_nModule0, "Module", "First", "First Module Index");
            m_grid.Set(ref m_lDI, "Module", "Count", "# of DI Page");
            m_grid.Set(ref m_nDI0, "DI", "First", "First DI Index"); 
            m_grid.Refresh();
        }

        void RunThread()
        {
            m_bRunThread = true; 
            Thread.Sleep(3000);
            while (m_bRunThread)
            {
                Thread.Sleep(10);
                int lDI = m_aModule.Length; 
                for (int n = 0; n < lDI; n++)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        CAXD.AxdiReadInportWord(m_aModule[n], i, ref m_aData[6 * n + i]);
                    }
                }
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!checkSetup.Checked) Invalidate(false);
        }

        public bool GetInputBit(int n)
        {
            n -= m_nDI0;
            if (n < 0) return false;
            int nIndex = n / 16; 
            if (nIndex >= 6 * m_lDI) return false;
            return ((m_aData[nIndex] & m_aComp[n % 16]) != 0);
        }

        public bool GetOutputBit(int n)
        {
            return false;
        }

        public void WriteOutputBit(int n, bool bOn)
        {
        }

        public void SetDICaption(int nDI, string str)
        {
            nDI -= m_nDI0; 
            if ((nDI < 0) || (nDI >= 96 * m_lDI)) return;
            m_di[nDI].SetCaption(str);
        }

        public void SetDOCaption(int nDO, string str)
        {
        }

        private void DI_Ajin_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

    }
}
