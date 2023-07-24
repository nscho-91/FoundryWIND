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

    public partial class HW_Aligner_Mom : Form
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

        protected enum eProcess
        {
            PreProcess,
            Align,
            BCR,
            OCR,
            Edge, 
            Rotate,
        }
        protected eProcess m_eRun = eProcess.PreProcess;

        public string m_strAlignModel = "Align";
        public bool m_bEnableAlign = true;
        public bool m_bRotate180 = false;   //KDG 161025 Add Ring Wafer 180 Rotate
        public bool m_bWTRShiftUse = false; // BHJ 190304 add
        
        protected string m_id;
        protected string m_sError; 
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
        public MarsLog m_logMars;
        protected bool m_bRunThread = false;
        Thread m_thread;

        public int m_msHome = 30000;

        public int m_nTryBCR = 1;
        public double m_dPeriod = 1;
        public int m_nBinaryGV = 125;
        public bool m_bUseImgBinary = false;
        
        public HW_Aligner_Mom()
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
            m_log = new Log(m_id, m_auto.ClassLogView(), "Aligner");
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
            m_logMars = m_auto.ClassMarsLog(); 
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
            for (int nLP = 0; nLP < m_auto.ClassHandler().m_nLoadPort; nLP++)
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

        public virtual void JobSave(ezJob job)
        {

        }

        public virtual void JobOpen(ezJob job)
        {

        }

        public virtual void ShowDlg(Form parent, ref CPoint cpShow)
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

        private void buttonRotate_Click(object sender, EventArgs e)
        {
            RunRotate(m_recipe.m_fAngleAlign);
        }

        private void buttonHome_Click(object sender, EventArgs e)
        {
            SetHome();
        }

        private void buttonVacOn_Click(object sender, EventArgs e)
        {
            RunVac(true);
        }

        private void buttonVacOff_Click(object sender, EventArgs e)
        {
            RunVac(false);
        }

        private void buttonBCR_Click(object sender, EventArgs e)
        {
            if (m_infoWafer == null) m_infoWafer = new Info_Wafer(0, 0, 0, m_log);
            m_infoWafer.m_wafer = m_recipe.m_wafer;
            m_recipe.SetInfoWafer(m_infoWafer, null);
            m_infoWafer.m_bUseBCR = true;
            Thread thread = new Thread(new ThreadStart(ProcBCR));
            thread.Start();
            thread.Join();
        }

        private void buttonOCR_Click(object sender, EventArgs e)
        {
            if (m_infoWafer == null) m_infoWafer = new Info_Wafer(0, 0, 0, m_log);
            m_infoWafer.m_wafer = m_recipe.m_wafer;
            m_recipe.SetInfoWafer(m_infoWafer, null);
            m_infoWafer.m_bUseOCR = true;
            Thread thread = new Thread(new ThreadStart(ProcOCR));
            thread.Start();
            thread.Join();
        }

        private void timerLabel_Tick(object sender, EventArgs e)
        {
            if (CheckWaferExist() == eHWResult.On)
            {
                labelWafer.ForeColor = Color.Red;
            }
            else
            {
                labelWafer.ForeColor = Color.Black;
            }
            labelError.Text = m_sError;
            RunTimer_200ms();
        }

        public eState GetState()
        {
            return m_eState;
        }

        public eHWResult SetHome()
        {
            if ((m_eState == eState.Run) && (m_work.GetState() ==  eWorkRun.Run))
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
                Thread.Sleep(500); // ing 170401
                if (CheckWaferExist() != eHWResult.Off)
                {
                    m_log.Popup("Wafer Checked !!");
                    m_work.SetError(eAlarm.Error, m_log, 0);
                    return eHWResult.Error;
                }
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
                m_infoWafer.Locate = Info_Wafer.eLocate.Aligner;
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

        public virtual eHWResult CheckWaferExist(bool bVac = false) { return eHWResult.Off; }
        public virtual eHWResult RunLift(bool bUp) { return eHWResult.Error; }   //KDG 161025 Add
        public virtual void Grab() { }
        public virtual double GetAlignAngle() { return 0.0; }
        public virtual int GetWaferShape() { return -1;  }

        public virtual void Draw(Graphics dc, ezImgView imgView) { }
        public virtual void InvalidView() { }

        public virtual bool RunVac(bool bVac) { return false; }
        public virtual bool RunLifterVac(bool bVac) { return false; }       //KDG 161229 Add
        public virtual void LotStart() { }
        public virtual void LotEnd() { } // ing 170208
        
        protected virtual void ProcHome() { }
        protected virtual void ProcRunReady() { }
        protected virtual void ProcPreProcess() { }
        protected virtual void ProcAlign() { }
        protected virtual void TestRun() { }
        protected virtual void ProcBCR() { }
        protected virtual void ReadBCR() { } // ing 170401
        protected virtual void ProcOCR() { }
        protected virtual void ReadOCR() { } // ing 170401
        protected virtual void ProcEdge() { }
        protected virtual void ProcRotate() { }
        protected virtual void ProcRotateRF(Info_Wafer.eAngleRF eAngle) { }
        protected virtual void RunRotate(double fAngle) { }
        protected virtual void ProcError() { }
        public virtual eHWResult UnloadLift() { return eHWResult.OK; }

        protected virtual void RunTimer_200ms() { }

        protected void SetState(eState state) 
        {
            if (state == m_eState) return;
            m_log.Add("SetState : " + m_eState.ToString() + " to " + state.ToString());
            m_eState = state;
            if (m_eState == eState.Run)
            {
                SetProcess(eProcess.PreProcess);
            }
        }

        protected virtual void SetProcess(eProcess run)
        {
            m_log.Add("Set Run Process : " + m_eRun.ToString() + " to " + run.ToString());
            m_log.WriteLog("Sequence", "[Aligner] Set Run Process : " + m_eRun.ToString() + " to " + run.ToString());  //230724 nscho 
            m_eRun = run;
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
                        //if (!m_bEnableAlign)
                        //{
                            if (m_infoWafer.m_eProcCurrent < Info_Wafer.eProc.Align)
                            {
                                if (m_infoWafer.m_eProcCurrent == Info_Wafer.eProc.BCR)
                                {
                                    ReadBCR(); // MSB의 필름위 OCR읽는 기능도 ReadBCR에 들어있음.
                                }
                                if (m_infoWafer.m_wafer.IsRingFrame()) ProcRotateRF((Info_Wafer.eAngleRF)(m_infoWafer.m_eProcCurrent));
                                SetState(eState.Done);
                                break;
                            }
                            //else
                            //{
                            //    SetState(eState.Done);
                            //    break;
                            //}
                        //}
                        switch (m_eRun)
                        {
                            case eProcess.PreProcess:
                                ProcPreProcess();
                                break;

                            case eProcess.Align:
                                ProcAlign();
                                break;

                            case eProcess.BCR:
                                ProcBCR();
                                break;

                            case eProcess.OCR:
                                ProcOCR();
                                break;

                            case eProcess.Edge:
                                ProcEdge();
                                break;

                            case eProcess.Rotate:
                                ProcRotate();
                                break;
                        }
                        break;

                    case eState.Done:
                        break;

                    case eState.Error: ProcError();
                        break;
                }
            }
        }

        private void buttonAlign_Click(object sender, EventArgs e)
        {
            Info_Wafer infowafer = m_infoWafer;
            if (m_infoWafer == null)
            {
                m_infoWafer = new Info_Wafer(0, 0, 0, m_log);
                m_infoWafer.m_wafer = m_recipe.m_wafer;
                m_recipe.SetInfoWafer(m_infoWafer, null); 
            }
//            SetRun(m_infoWafer);
            TestRun(); //forget Hynix
            m_infoWafer = infowafer; 
        }


        public virtual bool LiterSafety()
        {
            if (m_strAlignModel == "EBR")
            {
                if (RunLift(false) == eHWResult.Error)
                    return true; 
                else
                    return false; 
            }
            else
                return false; 
        }

        public virtual ezView ClassView()
        {
            return null;
        }
    }
}
