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
    public partial class DB96T2_Ajin : DockContent, DIO_Mom
    {
        string m_id;
        Log m_log;
        ezGrid m_grid;

        int m_lPage = 8;
        int m_lDI = 160;
        int m_lDO = 16; 
        int m_nModule0 = 1;
        int m_nDIO0 = 48; 

        int[] m_aModule = null;
        uint[] m_aDataDI = null;
        uint[] m_aDataDO = null;
        DIO_Ajin_ID[] m_di = null;
        DO_Ajin_ID[] m_do = null;

        uint[] m_aComp = new uint[16];
        CPoint m_cpDraw = new CPoint(10, 50);

        bool m_bRunThread = false;
        Thread m_thread;

        public DB96T2_Ajin()
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

            m_aModule = new int[m_lPage];
            m_aDataDI = new uint[10 * m_lPage];
            m_aDataDO = new uint[m_lPage];
            for (int n = 0; n < m_lPage; n++) m_aModule[n] = m_nModule0 + 2 * n;

            comboDI.Items.Clear();
            for (char ch = 'A'; ch <= 'H'; ch++) comboDI.Items.Add(ch.ToString());

            m_di = new DIO_Ajin_ID[m_lDI * m_lPage];
            for (int n = 0; n < m_lDI * m_lPage; n++) m_di[n] = new DIO_Ajin_ID("DI", n + m_nDIO0);

            m_do = new DO_Ajin_ID[m_lDO * m_lPage];
            for (int n = 0; n < m_lDO * m_lPage; n++) m_do[n] = new DO_Ajin_ID("DO", n + m_nDIO0);

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
            for (int n = 0; n < 16 * m_lDO * m_lPage; n++) WriteOutputBit(n + m_nDIO0, false);
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

        private void DB96T2_Ajin_Paint(object sender, PaintEventArgs e)
        {
            Graphics dc = e.Graphics;
            CPoint cp = new CPoint();
            if (checkSetup.Checked) return;
            if (comboDI.SelectedIndex < 0) return;
            cp = m_cpDraw;
            int nDI0 = m_lDI * comboDI.SelectedIndex;
            int nDO0 = m_lDO * comboDI.SelectedIndex;
            for (int x = 0; x < 5; x++)
            {
                cp = m_cpDraw; 
                cp.x += (((Control)sender).Size.Width - 20) * x / 5;
                dc.DrawString("INPUT", this.Font, Brushes.Blue, cp.x, cp.y);
                for (int n = 0; n < 32; n++, nDI0++)
                {
                    cp.y += 18;
                    m_di[nDI0].Draw(dc, GetInputBit(m_nDIO0 + nDI0), cp);
                }
                cp.y += 36;
                dc.DrawString("OUTPUT", this.Font, Brushes.Blue, cp.x, cp.y);
                for (int n = 0; n < 3; n++, nDO0++)
                {
                    cp.y += 18;
                    m_do[nDO0].Draw(dc, GetOutputBit(m_nDIO0 + nDO0), cp); 
                }
            }
        }

        private void comboDI_SelectedIndexChanged(object sender, EventArgs e)
        {
            Invalidate(false); 
        }

        private void DB96T2_Ajin_MouseDown(object sender, MouseEventArgs e)
        {
            int n;
            if (checkSetup.Checked) return;
            if (comboDI.SelectedIndex < 0) return;
            int nDI0 = m_lDI * comboDI.SelectedIndex;
            int nDO0 = m_lDO * comboDI.SelectedIndex;
            if (e.Button == MouseButtons.Left)
            {
                for (n = 0; n < 16; n++) if (m_do[nDO0 + n].IsClick(e.X, e.Y)) WriteOutputBit(m_nDIO0 + nDO0 + n, !GetOutputBit(m_nDIO0 + nDO0 + n));
            }
            if (e.Button == MouseButtons.Right)
            {
                for (n = 0; n < m_lDI; n++) if (m_di[nDI0 + n].IsClick(e.X, e.Y)) m_di[nDI0 + n].ShowEditBox();
                for (n = 0; n < m_lDO; n++) if (m_do[nDO0 + n].IsClick(e.X, e.Y)) m_do[nDO0 + n].ShowEditBox();
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
            m_grid.Set(ref m_lPage, "Page", "Count", "# of DIO Page");
            m_grid.Set(ref m_nDIO0, "DIO", "First", "First DIO Index"); 
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
                        CAXD.AxdiReadInportWord(m_aModule[n], i, ref m_aDataDI[10 * n + i]);
                    }
                    for (int i = 6; i < 10; i++)
                    {
                        CAXD.AxdiReadInportWord(m_aModule[n] + 1, i - 6, ref m_aDataDI[10 * n + i]);
                    }
                    CAXD.AxdoReadOutportWord(m_aModule[n] + 1, 4, ref m_aDataDO[n]); 
                }
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!checkSetup.Checked) Invalidate(false);
        }

        public bool GetInputBit(int n)
        {
            n -= m_nDIO0;
            if (n < 0) return false;
            int nIndex = n / 16;
            if (nIndex >= 10 * m_lPage) return false;
            return ((m_aDataDI[nIndex] & m_aComp[n % 16]) != 0);
        }

        public bool GetOutputBit(int n)
        {
            n -= m_nDIO0;
            if (n < 0) return false;
            int nIndex = n / 16;
            if (nIndex >= m_lPage) return false;
            return ((m_aDataDO[nIndex] & m_aComp[n % 16]) != 0);
        }

        public void WriteOutputBit(int n, bool bOn)
        {
            uint nOn;
            n -= m_nDIO0;
            if (n < 0) return;
            int nIndex = n / 16;
            if (nIndex >= m_lPage) return;
            if (bOn) nOn = 1; else nOn = 0;
            CAXD.AxdoWriteOutportBit(m_aModule[nIndex] + 1, 64 + n % 16, nOn);
        }

        public void SetDICaption(int n, string str)
        {
            n -= m_nDIO0;
            if (n < 0) return;
            if (n >= 160 * m_lPage) return;
            m_di[n].SetCaption(str);
        }

        public void SetDOCaption(int n, string str)
        {
            n -= m_nDIO0;
            if (n < 0) return;
            int nIndex = n / 16;
            if (nIndex >= m_lPage) return;
            m_do[n].SetCaption(str);
        }

        private void DB96T2_Ajin_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

    }
}
