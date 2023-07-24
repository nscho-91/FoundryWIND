using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using PropertyGridEx;
using ezAutoMom;
using ezTools;

namespace ezAutoMom
{
    public partial class Light_Mom : DockContent
    {
        protected const int c_nLightCh = 20;
        protected const int c_nLightMode = 16;

        public string m_id;
        protected string[] m_strMode = new string[c_nLightMode];
        public bool m_bLightOn, m_bOn, m_bReset;
        public int m_nCh = 12, m_nMode = 1;
        int[] m_nHour = new int[3] { 0, 0, 10000 };
        public int[,] m_aPower = new int[c_nLightMode, c_nLightCh];
        public Light_Data[] m_aSetup = new Light_Data[c_nLightCh];

        public Log m_log;
        public ezGrid m_grid;
        ezStopWatch m_swHour = new ezStopWatch();
        Size m_szGrid;

        public EventHandler evhUpdateData = null;

        public Light_Mom()
        {
            InitializeComponent();
            m_szGrid = this.Size - grid.Size;
        }

        public void Init(string id, object auto)
        {
            int n;
            this.Text = m_id = id;
            m_log = new Log(id, ((Auto_Mom)auto).m_logView);
            m_grid = new ezGrid(id, grid, m_log, false);
            m_bLightOn = m_bOn = false;
            for (n = 0; n < c_nLightCh; n++) m_aSetup[n] = new Light_Data(n, false, m_log);
            for (n = 0; n < c_nLightMode; n++) m_strMode[n] = "Mode" + n.ToString("00");
            RunGrid(eGrid.eRegRead, null);
            for (n = 0; n < m_nMode; n++) comboMode.Items.Add(m_strMode[n]);
            if (m_nMode >= 0) comboMode.SelectedIndex = 0;
            RunGrid(eGrid.eInit, null);
        }

        public virtual void ThreadStop()
        {
            int n;
            for (n = 0; n < m_nCh; n++) LightOn(n, false);
        }

        protected override string GetPersistString()
        {
            return GetType().ToString() + "," + m_id;
        }

        public bool IsPersistString(string str)
        {
            return GetPersistString() == str;
        }

        public virtual void LightOn(int nID, bool bOn)
        {
            if (m_bLightOn == bOn) return; m_bLightOn = bOn;
            if (m_bLightOn) { 
                m_swHour.Start(); 
                if (m_nHour[1] > m_nHour[2]) 
                    m_log.Popup("Hour"); 
            }
            else
            {
                m_nHour[0] += (int)m_swHour.Check();
                while (m_nHour[0] > 3600000)
                {
                    m_bReset = true; m_nHour[1]++; m_nHour[0] -= 3600000; m_bReset = false;
                }
            }
        }

        private void buttonReconnect_Click(object sender, EventArgs e)
        {
            ReConnect();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to Reset?", "LED Time Reset", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            m_nHour[0] = m_nHour[1] = 0; RunGrid(eGrid.eInit, null);
        }

        private void checkOn_Click(object sender, EventArgs e)
        {
            RunGrid(eGrid.eInit, null);
            LightOn(comboMode.SelectedIndex, checkOn.Checked);
        }

        private void checkSetup_Click(object sender, EventArgs e)
        {
            RunGrid(eGrid.eInit, null);
        }

        private void comboMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            RunGrid(eGrid.eInit, null);
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate, null); RunGrid(eGrid.eRegWrite, null);
            UpdateData();
        }

        void RunGrid(eGrid eMode, ezJob job)
        {
            if (m_grid == null) return;
            m_grid.Update(eMode, job);
            RunGridSetup(checkSetup.Checked, eMode, job);
            RunGridLight(checkSetup.Checked, eMode, job);
            m_grid.Refresh();
        }

        protected virtual void RunGridSetup(bool bChecked, eGrid eMode, ezJob job)
        {
            int n, nMode;
            if ((eMode == eGrid.eJobOpen) || (eMode == eGrid.eJobSave)) return;
            if (!bChecked && (eMode <= eGrid.eUpdate)) return;
            m_grid.Set(ref m_nMode, "Mode", "Count", "# of Total Mode");
            if ((eMode == eGrid.eRegRead) || (eMode == eGrid.eRegWrite)) nMode = c_nLightMode; else nMode = m_nMode;
            for (n = 0; n < nMode; n++) m_grid.Set(ref m_strMode[n], "Mode", "Mode" + n.ToString("00"), "Mode ID");
            m_grid.Set(ref m_nCh, "Channel", "Count", "Total Channel Count");
            if (m_nCh > c_nLightCh) m_nCh = c_nLightCh;
            for (n = 0; n < m_nCh; n++) m_grid.Set(ref m_aSetup[n], "Channel", n.ToString("Ch00"), "Channel Setting");
        }

        protected virtual void RunGridLight(bool bChecked, eGrid eMode, ezJob job)
        {
            int n, m;
            if (bChecked && (eMode <= eGrid.eUpdate)) return;
            if ((eMode == eGrid.eRegRead) || (eMode == eGrid.eRegWrite))
            {
                for (n = 0; n < c_nLightMode; n++) for (m = 0; m < c_nLightCh; m++)
                    m_grid.Set(ref m_aPower[n, m], m_strMode[n], m.ToString("Ch00"), "LED Power 0~100%");
            }
            if (eMode == eGrid.eJobSave || eMode == eGrid.eJobOpen) // ing 161013
            {
                for (n = 0; n < c_nLightMode; n++) for (m = 0; m < c_nLightCh; m++) if (m_aSetup[m].m_bUse)
                    m_grid.Set(ref m_aPower[n, m], m_strMode[n], m.ToString("Ch00"), "LED Power 0~100%");
            }
            if (comboMode.SelectedIndex >= 0)
            {
                n = comboMode.SelectedIndex;
                for (m = 0; m < c_nLightCh; m++) if (m_aSetup[m].m_bUse)
                    m_grid.Set(ref m_aPower[n, m], m_strMode[n], m.ToString("Ch00"), "LED Power 0~100%");
            }
        }

        public void JobOpen(ezJob job)
        {
            RunGrid(eGrid.eJobOpen, job); RunGrid(eGrid.eInit, null);
        }

        public void JobSave(ezJob job)
        {
            RunGrid(eGrid.eJobSave, job);
        }

        public virtual void ReConnect() { }
        public virtual void ChangePower(int nCh, int nMode) { }
        void InitString(string id, int nID, string str) { }
        void SetAlarm(int nError, string id) { }

        private void Light_Mom_Resize(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            Size sz = control.Size;
            if (control.Text == m_id) { grid.Size = control.Size - m_szGrid; }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (m_bReset) RunGrid(eGrid.eInit, null);
        }

        private void Light_Mom_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        //kw Grid 변경 이벤트 받으면 호출 해줌
        public void UpdateData()
        {
            if (evhUpdateData != null)
                evhUpdateData(this, null);
        }

        //kw 전체 사용 채널 개수를 넘겨줌
        public int GetUseChannelCount()
        {
            int nCount = 0;
            for (int n = 0; n < c_nLightCh; n++) 
            {
                if( m_aSetup[n].bUse )
                    nCount++;
            }
            return nCount;
        }

        // kw nUseIdx 번째로 사용중인 채널의 라벨을 넘겨줌
        public string GetLabel(int nUseIdx)
        {
            int nIdx = 0;

            for (int n = 0; n < c_nLightCh; n++)
            {
                if (m_aSetup[n].bUse && nUseIdx == nIdx++)
                    return m_aSetup[n].ID;
            }

            return "";
        }
    }
}
