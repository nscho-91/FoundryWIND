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
    public partial class HW_LoadPort_Mom : Form
    {
        public enum eState
        {
            Init, //처음
            Home, //홈잡는중
            Ready, //홈끝났음
            Placed, //캐리어 올라가있음
            CSTIDRead,  //카세트 아이디 읽기
            RunDocking,                         //KJWAuto 추가.
            RunLoad, //도킹, 문열고, 매핑
            LoadDone, //매핑까지 끝
            RunUnload, // 언로드 중
            UnloadDone, //언로드 완료
            Error, //타임 아웃이나 기타 에러
            WaitProcessEnd,
        }

        public enum eUploadCycle
        {
            None = 1,
            ReadCarrierID,
            DelCarrierInfo,
            SetCarrierComplete,
            SetLPReadyToUnload,
        }
        public eUploadCycle m_eUploadCycle = eUploadCycle.None;

        public bool m_bUseOHT = false; // 자동화 유무
        public bool m_bOHTAuto = false; // 자동화 상태
        public bool m_bRFIDUse = false;
        public bool m_bUseBCR = false;
        public bool m_bMSB = false;
        public int m_nSlimSlot = -1;
        public bool m_bUseSlimSlot = false;
        protected bool m_bUseMultiPort = false;
        protected int m_diRingPlacment = -1;
        public bool m_bRnRStart = false;
        public bool m_bReload = false;

        protected string m_id;
        protected Auto_Mom m_auto;
        Size[] m_sz = new Size[2];
        bool m_bLED = true;
        protected int m_nID = 0;
        protected Log m_log;
        protected ezGrid m_grid;
        protected Control_Mom m_control;
        Handler_Mom m_handler;
        protected XGem300_Mom m_xGem;
        protected Work_Mom m_work; 
        public Info_Carrier m_infoCarrier;
        protected eState m_eState = eState.Init; 
        protected bool m_bRunThread = false;
        protected bool m_bChangePlace = false; // ing 180221
        protected bool m_bPrevPlace = false; // ing 180221
        Thread m_thread;
        public int m_nDynamic=0;
        public int m_nStatic=0;
        public bool m_bAging=false;
        public bool m_bManual= false;
        public HW_WTR_Mom m_wtr=null;
        public int m_msHome = 40000;
        public MarsLog m_logMars;
        public bool m_bCheckParticle = false;
        public HW_LoadPort_Mom()
        {
            InitializeComponent();
            m_sz[0] = m_sz[1] = this.Size; 
            m_sz[0].Height = 26;

            for (int i = 1; i <= 25; i++) comboSlot.Items.Add(i.ToString());
            comboSlot.SelectedIndex = 0;
        }

        public virtual void Init(int nID, Auto_Mom auto)
        {
            m_nID = nID; 
            m_id = m_nID.ToString("LoadPort0"); 
            m_auto = auto; 
            checkView.Text = m_id;
            m_infoCarrier = new Info_Carrier(nID); 
            m_log = new Log(m_id, m_auto.m_logView, "LoadPort");
            m_grid = new ezGrid(m_id, grid, m_log, false);
            m_control = m_auto.ClassControl(); 
            m_handler = m_auto.ClassHandler();
            m_xGem = m_auto.ClassXGem();
            m_work = m_auto.ClassWork();
            m_wtr = m_handler.ClassWTR();
            m_thread = new Thread(new ThreadStart(RunThread)); m_thread.Start();
            m_logMars = m_auto.ClassMarsLog(); 
        }

        public virtual void ThreadStop()
        {
            if (m_bRunThread) { m_bRunThread = false; m_thread.Join(); }
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

        public virtual void RunGrid(eGrid eMode)
        {
            m_grid.Update(eMode, null);
        }

        private void checkView_CheckedChanged(object sender, EventArgs e)
        {
            m_handler.ShowChild();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            m_bLED = !m_bLED;
            UpdateDI(labelCoverClose, IsCoverOpen(false) == eHWResult.OK);
            UpdateDI(labelCoverOpen, IsCoverOpen(true) == eHWResult.OK);
            RunTimer(m_bLED); 
        }

        void UpdateDI(Label label, bool bOn)
        {
            if (bOn) label.ForeColor = Color.Red;
            else label.ForeColor = Color.Black;
        }

        protected virtual void RunTimer(bool bLED) { }
        public virtual eHWResult DoorOpen() { return eHWResult.Error; } 
        public virtual eHWResult DoorClose() { return eHWResult.Error; } 
        protected virtual eHWResult IsCoverOpen(bool bOpen) { return eHWResult.Error; }
        protected virtual eHWResult RunCover(bool bOpen) { return eHWResult.Error; }
        protected virtual eHWResult IsLoad(bool bLoad) { return eHWResult.Error; }
        protected virtual eHWResult IsCarrierReady() { return eHWResult.Error; }
        public virtual eHWResult RunLoad(bool bLoad) { return eHWResult.Error; }
        protected virtual eHWResult RunMapping() { return eHWResult.Error; }
        public virtual eHWResult RunUndocking() { return eHWResult.Error; }
        public virtual eHWResult SetMappingData(string strMaps) { return eHWResult.Error; }
        public virtual bool NeedWTRMapping() { return false; }
        public virtual bool CheckState_OtherLP(int m_nLPNum, string strCalc, eState State) { return false; }

        public virtual bool IsDoorOpen() { return false; }
        public virtual bool IsDoorAxisMove() { return false; }
        public virtual bool IsDoorOpenPos() { return false; }
        public virtual Wafer_Size.eSize CheckCarrier() { return Wafer_Size.eSize.Empty; }
        public virtual bool MoveSlot(int nType, int iSlot, bool bWait) { return false; }

        protected virtual void ProcInit() { }
        protected virtual void ProcHome() { }
        protected virtual void ProcCSTIDRead() { }
        protected virtual void ProcDocking() { }
        protected virtual void ProcWaitProcessEnd() { }
        protected virtual void ProcReady() { }
        protected virtual void ProcLoad() { }
        protected virtual void ProcDone() { }
        protected virtual void ProcUnload() { }

        void RunThread() 
        {
            m_bRunThread = true; Thread.Sleep(5000);
            while (m_bRunThread)
            {
                Thread.Sleep(10);
                switch (m_eState)
                {
                    case eState.Init: ProcInit(); break;
                    case eState.Home:
                        m_infoCarrier.m_eState = Info_Carrier.eState.Init; // ing 171106
                        ProcHome(); 
                        m_auto.SetCSTLoadOK(m_nID, false);
                        break;
                    case eState.Ready: ProcReady(); break;
                    case eState.CSTIDRead: ProcCSTIDRead(); break;
                    case eState.RunDocking: ProcDocking(); break;
                    case eState.RunLoad: ProcLoad(); break;
                    case eState.LoadDone:  break;
                    case eState.RunUnload: ProcUnload(); break;
                    case eState.WaitProcessEnd: ProcWaitProcessEnd();
                        break;
                    case eState.Error: 
                        m_work.SetError(eAlarm.Warning, m_log, (int)0); 
                        SetState(eState.Init);
                        break;
                }
            }
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate); 
            RunGrid(eGrid.eRegWrite);
        }

        private void buttonCover_Click(object sender, EventArgs e)
        {
            if (IsCoverOpen(true) == eHWResult.OK) 
            {
                RunCover(false); 
            }
            else 
            {
                RunCover(true); 
            } 
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            if (IsLoad(true) == eHWResult.OK)
            {
                if (m_wtr.RunGoHome() == eHWResult.Error) { return; } // BHJ 190304 add
                RunLoad(false);
            }
            else if (IsLoad(false) == eHWResult.OK)
            {
                if (IsCarrierReady() == eHWResult.OK)
                {
                    if (m_wtr.RunGoHome() == eHWResult.Error) { return; } // BHJ 190304 add
                    RunLoad(true);
                }
            }
        }

        private void buttonMap_Click(object sender, EventArgs e)
        {
            RunMapping(); 
        }

        private void buttonHome_Click(object sender, EventArgs e)
        {
            SetHome(); 
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            Reset(); 
        }

        public virtual void Reset()
        {
            if (m_eState == eState.Error)
                m_eState = eState.Ready;
        }

        public eHWResult SetHome()
        {
            if ((m_eState == eState.RunLoad) || (m_eState == eState.RunUnload))
            {
                m_log.Popup("Can't Start Home When State is " + m_eState.ToString());
                return eHWResult.Error;
            }
            SetState(eState.Home);
            return eHWResult.OK;
        }

        public eHWResult SetUnload()
        {
            if (m_eState != eState.LoadDone) return eHWResult.Error;
            SetState(eState.RunUnload); 
            return eHWResult.OK;
        }

        public eState GetState()
        {
            return m_eState;
        }
        
        public void SetState(eState state)
        {
            m_log.Add("SetState : " + m_eState.ToString() + " to " + state.ToString());
            m_eState = state;
        }

        public virtual bool IsPlaced()
        {
            return CheckCarrier() < Wafer_Size.eSize.Empty;
        }

        public virtual bool IsDocking()
        {
            return m_eState == eState.RunLoad;
        }

        public bool IsOpen()
        {
            return m_eState == eState.RunLoad;
        }

        public virtual bool IsCoverClose() { return false; }

        private void grid_Click(object sender, EventArgs e)
        {

        }

        private void btnLiftUP_Click(object sender, EventArgs e)
        {
            RunLiftUp();
        }

        private void btnLiftDown_Click(object sender, EventArgs e)
        {
            RunLiftDown();
        }

        private void buttonLoadPortMove_Click(object sender, EventArgs e)
        {
            int nSlot = comboSlot.SelectedIndex;
            MoveSlot(0, nSlot, true);
        }

        public virtual eHWResult RunLiftUp() { return eHWResult.Error; }
        public virtual eHWResult RunLiftDown() { return eHWResult.Error; }
        public virtual eHWResult RunLEDAuto() { return eHWResult.Error; }
        public virtual eHWResult RunLEDManual() { return eHWResult.Error; }
        public virtual eHWResult RunLEDLoad() { return eHWResult.Error; }
        public virtual eHWResult RunLEDUnload() { return eHWResult.Error; }
        public virtual eHWResult RunChange300Pos() { return eHWResult.Error; }
        public virtual eHWResult RunChangeRingPos() { return eHWResult.Error; }
        public bool UseMultiPort() { return m_bUseMultiPort; }

        private void btn300Pos_Click(object sender, EventArgs e)
        {
            RunChange300Pos();
        }

        private void btnRingPos_Click(object sender, EventArgs e)
        {
            RunChangeRingPos();
        }

        public virtual void SetAutoLoading()
        {
        }
    }
}
