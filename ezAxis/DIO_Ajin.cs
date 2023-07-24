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
    public partial class DIO_Ajin : DockContent, DIO_Mom
    {
        Type[,] m_type = new Type[2, 3];

        string m_id;
        int m_nID, m_nID48;
        int[,] m_aModule = new int[2, 3] { { -1, -1, -1 }, { -1, -1, -1 } };
        int[,] m_aOffset = new int[2, 3] { { -1, -1, -1 }, { -1, -1, -1 } };
        uint[,] m_aData = new uint[2, 3] { { 0, 0, 0 }, { 0, 0, 0 } };
        uint[] m_aComp = new uint[16]; 
        CPoint m_cpDraw = new CPoint(10, 50);
        DIO_Ajin_ID[] m_di = new DIO_Ajin_ID[48];
        DO_Ajin_ID[] m_do = new DO_Ajin_ID[48]; 
        Log m_log;
        ezGrid m_grid;
        bool m_bRunThread = false;
        uint dwStatus;
        Thread m_thread;

        public DIO_Ajin(string id, int nID, LogView logView, int dyDraw = 18)
        {
            int n;
            m_nID = nID; m_nID48 = 48 * nID;
            m_dyDraw = dyDraw;
            InitializeComponent();
            this.TabText = m_id = id;
            m_aComp[0] = 1; for (n = 1; n < 16; n++) m_aComp[n] = 2 * m_aComp[n - 1];
            for (n = 0; n < 48;n++)
            {
                m_di[n] = new DIO_Ajin_ID("DI", n + m_nID48);
                m_do[n] = new DO_Ajin_ID("DO", n + m_nID48);
                m_do[n].m_cbWriteOutput += WriteOutputBit;
            }
            for (n = 0; n < 3; n++)
            {
                m_type[0, n] = new Type();
                m_type[1, n] = new Type(); 
            }
            m_log = new Log(m_id, logView); 
            m_grid = new ezGrid(m_id, grid, m_log, false); grid.Hide(); 
            RunGrid(eGrid.eRegRead); RunGrid(eGrid.eInit);
            m_thread = new Thread(new ThreadStart(RunThread)); m_thread.Start();
        }

        public void ThreadStop()
        {
            int n;
            if (m_bRunThread) { m_bRunThread = false; m_thread.Join(); }
            for (n = 0; n < 48; n++) WriteOutputBit(n + m_nID48, false); 
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

        int m_dyDraw = 18; 
        private void DIO_Ajin_Paint(object sender, PaintEventArgs e)
        {
            Graphics dc = e.Graphics; int n; CPoint cp = new CPoint();
            if (checkSetup.Checked) return;
            cp = m_cpDraw;
            dc.DrawString("INPUT", this.Font, Brushes.Blue, cp.x, cp.y);
            for (n = 0; n < 48; n++) { cp.y += m_dyDraw; m_di[n].Draw(dc, GetInputBit(n + m_nID48), cp); }
            cp = m_cpDraw; cp.x += (((Control)sender).Size.Width - 20) / 2;
            dc.DrawString("OUTPUT", this.Font, Brushes.Blue, cp.x, cp.y);
            for (n = 0; n < 48; n++) { cp.y += m_dyDraw; m_do[n].Draw(dc, GetOutputBit(n + m_nID48), cp); }
        }

        private void DIO_Ajin_MouseDown(object sender, MouseEventArgs e)
        {
            int n; 
            if (checkSetup.Checked) return;
            if (e.Button == MouseButtons.Left)
            {
                for (n = 0; n < 48; n++) if (m_do[n].IsClick(e.X, e.Y)) WriteOutputBit(n + m_nID48, !GetOutputBit(n + m_nID48));
            }
            if (e.Button == MouseButtons.Right)
            {
                for (n = 0; n < 48; n++) if (m_di[n].IsClick(e.X, e.Y)) m_di[n].ShowEditBox();
                for (n = 0; n < 48; n++) if (m_do[n].IsClick(e.X, e.Y)) m_do[n].ShowEditBox(); 
            }
            Invalidate(false); 
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate); RunGrid(eGrid.eRegWrite); 
        }

        void RunGrid(eGrid eMode, ezJob job = null)
        {
            int n; 
            string strInput = m_nID.ToString() + ".Input_";
            string strOutput = m_nID.ToString() + ".Output_";
            m_grid.Update(eMode, job);
            for (n = 0; n < 3;n++)
            {
                m_type[1, n].RunGridIO(m_grid, strInput + n.ToString(), "Type", "Input Module Type"); 
                m_grid.Set(ref m_aModule[1, n], strInput + n.ToString(), "Module", "Input Module Number");
                m_grid.Set(ref m_aOffset[1, n], strInput + n.ToString(), "Offset", "Input Offset Number");
            }
            for (n = 0; n < 3; n++)
            {
                m_type[0, n].RunGridIO(m_grid, strOutput + n.ToString(), "Type", "Output Module Type"); 
                m_grid.Set(ref m_aModule[0, n], strOutput + n.ToString(), "Module", "Output Module Number");
                m_grid.Set(ref m_aOffset[0, n], strOutput + n.ToString(), "Offset", "Output Offset Number");
            }
            m_grid.Refresh();
        }

        uint AJindiReadInportWord(int n)
        {
            if (m_type[1, n].IsUC()) return CAXD.AxdiReadInportWord(m_aModule[1, n], m_aOffset[1, n], ref m_aData[1, n]);
            else return CAND.AndiReadInportWord(m_aModule[1, n], m_aOffset[1, n], ref m_aData[1, n]);
        }

        uint AJindiReadOutportWord(int n)
        {
            if (m_type[1, n].IsUC()) return CAXD.AxdoReadOutportWord(m_aModule[0, n], m_aOffset[0, n], ref m_aData[0, n]);
            else return CAND.AndoReadOutportWord(m_aModule[0, n], m_aOffset[0, n], ref m_aData[0, n]);
        }

        void RunThread()
        {
            int n; 
            m_bRunThread = true; Thread.Sleep(3000);
            while (m_bRunThread)
            {
                Thread.Sleep(1);
                for (n = 0; n < 3; n++)
                {
                    if (m_aModule[1, n] >= 0) AJindiReadInportWord(n);
                    if (m_aModule[0, n] >= 0) AJindiReadOutportWord(n); 
                }
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!checkSetup.Checked) Invalidate(false);
        }

        public bool GetInputBit(int n)
        {
            n -= m_nID48; if ((n < 0) || (n >= 48)) return false;
            if ((m_aData[1, n / 16] & m_aComp[n % 16]) != 0) return true;
            return false;
        }

        public bool GetOutputBit(int n)
        {
            n -= m_nID48; if ((n < 0) || (n >= 48)) return false;
            if ((m_aData[0, n / 16] & m_aComp[n % 16]) != 0) return true;
            return false;
        }

        public void WriteOutputBit(int n, bool bOn)
        {
            uint nOn; 
            if ((n == 12) && bOn)
            {
                n = 12; 
            }
            n -= m_nID48; if ((n < 0) || (n >= 48)) return; 
            if (bOn) nOn = 1; else nOn = 0;
            if (m_type[0, n/16].IsUC()) CAXD.AxdoWriteOutportBit(m_aModule[0, n / 16], 16 * m_aOffset[0, n / 16] + n % 16, nOn);
            else CAND.AndoWriteOutportBit(m_aModule[0, n / 16], 16 * m_aOffset[0, n / 16] + n % 16, nOn);
        }

        public void SetDICaption(int nDI, string str)
        {
            nDI -= m_nID48; if ((nDI < 0) || (nDI >= 48)) return;
            m_di[nDI].SetCaption(str);
        }

        public void SetDOCaption(int nDO, string str)
        {
            nDO -= m_nID48; if ((nDO < 0) || (nDO >= 48)) return;
            m_do[nDO].SetCaption(str);
        }

        private void DIO_Ajin_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public bool IsAxisOpen()
        {
            if (CAXL.AxlOpen(7) == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                if (CAXD.AxdInfoIsDIOModule(ref dwStatus) == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                {
                    if (dwStatus == (uint)AXT_EXISTENCE.STATUS_EXIST)
                    {
                        return true;
                    }
                    else
                    {
                        m_log.Popup("Module does not exist.");
                        return false;
                    }
                }
                else
                {
                    m_log.Popup("AxdInfoIsDIOModule Error!!");
                    return false;
                }
            }
            else
            {
                m_log.Popup("EIP Device Open Error!");
                return false;
            }
        }
    }
}
