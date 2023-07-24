using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;
using ezAutoMom;
using ezTools;

namespace ezAutoMom
{
    public partial class HW_BackSide_Mom : Form
    {

        public enum eState
        {
            Init,
            Home,
            RunReady,
            Ready,
            Run,
            Done,
            Error,
        }
        protected eState m_eState = eState.Init;

//        protected enum eProcess
//        {
//            PreProcess,
//            Align,
//            BCR,
//            OCR,
//            Rotate
//        }
//        protected eProcess m_eRun = eProcess.PreProcess;

//        public string m_strAlignModel = "Align";
//        public bool m_bEnableBackSide = true;

        protected string m_id;
        protected string m_sError;
        public bool m_bEnable = false; 
        Size[] m_sz = new Size[2];

        protected Log m_log;
        protected ezGrid m_grid;
        protected Auto_Mom m_auto;
        protected Control_Mom m_control;
        protected XGem300_Mom m_xGem;
        protected Work_Mom m_work;
        protected Recipe_Mom m_recipe;
        Handler_Mom m_handler;
        protected Info_Wafer m_infoWafer = null;

        protected eHWResult m_waferExist = eHWResult.Off;

        public Wafer_Size m_wafer;

        protected bool m_bRunThread = false;
        Thread m_thread;

        public int m_msHome = 30000;

        public HW_BackSide_Mom()
        {
            InitializeComponent();
            m_sz[0] = m_sz[1] = this.Size;
            m_sz[0].Height = 26;
        }

        public virtual void Init(string id, Auto_Mom auto)
        {
            m_id = id;
            m_auto = auto;
            checkView.Text = m_id;
            m_log = new Log(m_id, m_auto.ClassLogView(), "BackSide");
            m_grid = new ezGrid(m_id, grid, m_log, false);
            m_wafer = new Wafer_Size(m_log);
            m_control = m_auto.ClassControl();
            m_xGem = m_auto.ClassXGem();
            m_work = m_auto.ClassWork();
            m_recipe = m_auto.ClassRecipe();
            m_handler = m_auto.ClassHandler();
            CheckEnableWaferSize();
            m_thread = new Thread(new ThreadStart(RunThread));
            m_thread.Start();
        }

        public virtual void ThreadStop()
        {
            if (m_bRunThread) { m_bRunThread = false; m_thread.Join(); }
        }

        public virtual IDockContent GetContentFromPersistString(string persistString)
        {
            return null;
        }

        public virtual void ShowAll(DockPanel dockPanel)
        {
        }

        protected void CheckEnableWaferSize()
        {
            for (int nLP = 0; nLP < 3; nLP++)
            {
                Info_Carrier infoCarrier = m_auto.ClassHandler().ClassCarrier(nLP);
                if (infoCarrier != null) m_wafer.AddEnable(infoCarrier.m_wafer);
            }
            m_wafer.InitString();
        }

        protected virtual void RunGrid(eGrid eMode)
        {
            m_grid.Update(eMode, null);
        }

        public void ShowDlg(Form parent, ref CPoint cpShow)
        {
            int nIndex;
            this.TopLevel = false;
            this.Parent = parent;
            this.Location = cpShow.ToPoint();

            if (checkView.Checked) nIndex = 1;
            else nIndex = 0;

            this.Size = m_sz[nIndex];
            cpShow.y += m_sz[nIndex].Height;
            Show();
        }

        private void checkView_CheckedChanged(object sender, EventArgs e)
        {
            m_handler.ShowChild();
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate);
            RunGrid(eGrid.eRegWrite);
        }

        public eState GetState()
        {
            return m_eState;
        }

        public eHWResult SetHome()
        {
            if ((m_eState == eState.Run))
            {
                m_log.Popup("Can't Start Home When State is " + m_eState.ToString());
                return eHWResult.Error;
            }
            SetState(eState.Home);
            return eHWResult.OK;
        }

        public eHWResult SetRun(Info_Wafer infoWafer)
        {
            if (infoWafer == null)
            {
                m_log.Popup("InfoWafer is Empty !!");
                return eHWResult.Error;
            }
            if (m_eState != eState.Ready)
            {
                m_log.Popup("Can't Start Run When State is " + m_eState.ToString());
                return eHWResult.Error;
            }
            if (CheckWaferExist() != eHWResult.On)
            {
                m_log.Popup("Wafer not Exist!!");
                return eHWResult.Error;
            }
            InfoWafer = infoWafer;
            SetState(eState.Run);
            return eHWResult.OK;
        }

        public eHWResult SetReady()
        {
            if (m_eState != eState.Done && m_eState != eState.Ready)
            {
                m_log.Popup("Can't Set Ready When State is " + m_eState.ToString());
                return eHWResult.Error;
            }
            if (CheckWaferExist() != eHWResult.Off)
            {
                m_log.Popup("Wafer Checked !!");
                return eHWResult.Error;
            }
            m_infoWafer = null;
            SetState(eState.RunReady);
            return eHWResult.OK;
        }

        public Info_Wafer InfoWafer
        {
            get { return m_infoWafer; }
            set
            {
                m_infoWafer = value;
                if (m_infoWafer == null) return;
                m_infoWafer.Locate = Info_Wafer.eLocate.BackSide;
            }
        }

        public eHWResult IsWaferExist()
        {
            return m_waferExist;
        }

        public virtual bool IsReady(Info_Wafer infoWafer)
        {
            return (InfoWafer == null);
        }

        public virtual eHWResult CheckWaferExist(bool bVac = true) { return eHWResult.Off; }
        public virtual void Grab(int nID) { }

        public virtual void JobSave(ezJob job) { }

        public virtual void JobOpen(ezJob job) { }

        public virtual void Draw(Graphics dc, ezImgView imgView) { }
        public virtual void InvalidView() { }

        public virtual bool RunVac(bool bVac, int msWait = 3000) { return false; }

        public virtual void RunBlow() { }

        protected virtual void ProcHome() { }
        protected virtual void ProcRunReady() { }
        protected virtual void ProcReady() { }
        protected virtual void ProcRun() { }
        protected virtual void ProcError() { }

//        protected virtual void RunTimer_200ms() { }

        protected void SetState(eState state)
        {
            if (state == m_eState) return;
            m_log.Add("SetState : " + m_eState.ToString() + " to " + state.ToString());
            m_eState = state;
        }

        void RunThread()
        {
            m_bRunThread = true;
            Thread.Sleep(5000);

            while (m_bRunThread)
            {
                Thread.Sleep(10);
                switch (m_eState)
                {
                    case eState.Init:
                        break;
                    case eState.Home:
                        ProcHome();
                        break;
                    case eState.RunReady:
                        ProcRunReady();
                        break;
                    case eState.Ready:
                        break;
                    case eState.Run:
                        ProcRun();
                        break;
                    case eState.Done:
                        break;
                    case eState.Error: 
                        ProcError();
                        break;
                }
            }
        }

        public virtual bool IsBackSideEnable()  //KDG Add BackSide Enable
        {
            return false;
        }

        public virtual void ShowCamera() { }

        private void buttonCamera_Click(object sender, EventArgs e)
        {
            ShowCamera();
        }

        public virtual bool IsAxisOK()
        {
            return true;
        }

        private void HW_BackSide_Mom_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            SetState(eState.Run);
        }

        private void buttonHome_Click(object sender, EventArgs e)
        {
            SetState(eState.Home);
        }

        public virtual ezView GetView(int nID)
        {
            return null;
        }

        public virtual void SavePM()
        {

        }
    }
}
