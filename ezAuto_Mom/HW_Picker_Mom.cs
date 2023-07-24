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

namespace ezAutoMom
{
    public partial class HW_Picker_Mom : Form 
    {
        public string m_id;
        public bool m_bPaper, m_bLoad;
        public int m_msVac, m_msBlow, m_msEV, m_iVac;
        public int[] m_nShake = new int[2];
        public int[] m_msShake = new int[2];
        public int m_diUp, m_diDown, m_doDown, m_doBlow;
        public int[] m_diVac = new int[2];
        public int[] m_doVac = new int[2];

        Size[] m_sz = new Size[2];
        protected Info_Strip m_infoStrip = null; 
        public Log m_log;
        public ezGrid m_grid; 
        public Control_Mom m_control;
        public Handler_Mom m_handler;
        public Work_Mom m_work;
        public XGem300_Mom m_xGem;
        public bool m_bCheckview;   //박상영20180207

        public HW_Picker_Mom()
        {
            InitializeComponent(); 
            m_sz[0] = m_sz[1] = this.Size; m_sz[0].Height = 26;
            m_iVac = 0; m_nShake[0] = m_nShake[1] = 0;
            m_msShake[0] = m_msShake[1] = 100; m_msVac = 50; m_msBlow = 50; m_msEV = 300;
            m_diUp = m_diDown = m_doDown = m_doBlow = -1;
            m_diVac[0] = m_diVac[1] = m_doVac[0] = m_doVac[1] = -1;
        }

        public void Init(string id, object auto, string sGroup = "Handler")
        {
            m_id = id; checkView.Text = m_id; 
            m_log = new Log(id, ((Auto_Mom)auto).ClassLogView(), sGroup);
            m_grid = new ezGrid(id, grid, m_log, false);
            m_control = ((Auto_Mom)auto).ClassControl();
            m_handler = ((Auto_Mom)auto).ClassHandler();
            m_work = ((Auto_Mom)auto).ClassWork();
            m_xGem = ((Auto_Mom)auto).ClassXGem();
            RunGrid(eGrid.eRegRead); RunGrid(eGrid.eInit); 
        }

        private void checkView_CheckedChanged(object sender, EventArgs e)
        {
            m_handler.ShowChild(); 
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            LoadDown(m_iVac); LoadEVUp(null, null, false); 
        }

        private void buttonUnload_Click(object sender, EventArgs e)
        {
            UnloadDown(true); UnloadUp(); m_infoStrip = null; 
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate); RunGrid(eGrid.eRegWrite);
        }

        public void ShowDlg(Form parent, ref CPoint cpShow)
        {
            int nIndex;
            this.TopLevel = false; this.Parent = parent; this.Location = cpShow.ToPoint();
            if (checkView.Checked) nIndex = 1; else nIndex = 0;
            this.Size = m_sz[nIndex]; cpShow.y += m_sz[nIndex].Height;
            Show();

            m_bCheckview = checkView.Checked;   //박상영20180207
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            UpdateDI(labelDI_Down, m_diDown); UpdateDI(labelDI_Up, m_diUp);
            UpdateDI(labelDI_Vac0, m_diVac[0]); UpdateDI(labelDI_Vac1, m_diVac[1]);
            UpdateDO(labelDO_Down, m_doDown); UpdateDO(labelDO_Blow, m_doBlow);
            UpdateDO(labelDO_Vac0, m_doVac[0]); UpdateDO(labelDO_Vac1, m_doVac[1]); 
        }

        private void labelDO_Down_Click(object sender, EventArgs e)
        {
            RunDO(m_doDown); 
        }

        private void labelDO_Blow_Click(object sender, EventArgs e)
        {
            RunDO(m_doBlow); 
        }

        private void labelDO_Vac0_Click(object sender, EventArgs e)
        {
            RunDO(m_doVac[0]); 
        }

        private void labelDO_Vac1_Click(object sender, EventArgs e)
        {
            RunDO(m_doVac[1]); 
        }

        void UpdateDI(Label label, int DI) 
        {
            if (m_control.GetInputBit(DI)) label.ForeColor = Color.Red;
            else label.ForeColor = Color.Black;
        }

        void UpdateDO(Label label, int DO)
        {
            if (m_control.GetOutputBit(DO)) label.ForeColor = Color.Red;
            else label.ForeColor = Color.Black;
        }

        void RunDO(int DO) 
        {
            m_control.WriteOutputBit(DO, !m_control.GetOutputBit(DO));
        }

        public virtual void RunGrid(eGrid eMode) { }
        public virtual void RunReset() { }
        public virtual void RunInit() { }
        public virtual bool LoadDown(int iVac) { return false; }
        public virtual bool LoadUp(Info_Strip infoStrip, bool bUp = true) { return false; }
        public virtual bool LoadEVUp(Info_Strip infoStrip, HW_LoadEV_Mom loadEV, bool bPaper = false) { return false; }
        public virtual bool UnloadDown(bool bShake) { return false; }
        public virtual bool UnloadUp(bool bUp = true) { return false; }
        public virtual bool IsDown() { return false; }
        public virtual bool IsVac() { return false; }
        public virtual void RunVac(bool bVac) { }
        public virtual bool PickerDown() { return false; }
        public virtual bool PickerUp(bool bUp = true) { return false; }

        public bool WaitVac(bool bVac, int msWait) 
        {
            ezStopWatch sw = new ezStopWatch(); 
            while (sw.Check() < msWait)
            {
                if (IsVac() == bVac) return false;
                Thread.Sleep(1); 
            }
            return true; 
        }

        public bool PickerDown(int msDown) 
        {
            bool bError;
            bError = PickerDown();
            Thread.Sleep(msDown);
            return bError; 
        }

        private void HW_Picker_Mom_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public virtual void SetStrip(Info_Strip infoStrip)
        {
            m_infoStrip = infoStrip;
            if (m_infoStrip == null)
            {
                m_log.Add(m_id + " SetStrip Null");
            }
            else
            {
                m_infoStrip.m_sPos = m_id;
                m_log.Add(m_id + " : SetStrip " + m_infoStrip.m_nStrip.ToString());
            }
        }

        public virtual Info_Strip GetStrip()
        {
            return m_infoStrip;
        }
    }
}
