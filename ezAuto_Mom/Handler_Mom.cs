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


namespace ezAutoMom
{
    public partial class Handler_Mom : DockContent
    {
        public enum eError
        {
            Home,
            HomeWTR,
            HomeAligner,
            HomeVision,
            HomeLoadport,
            AlignerError,
            eMCReset,
            eIonizer,
            FFU,
            FDCError_CDA,            
            FDCError_Vacuum,         
            FDCError_VisionPressure, 
            FDCError_EFEMPressure,   
            FDCError_Temp,           
            FDCError_Elect,          
            FDCError_Vacuum1,        
            FDCError_Vacuum2         
        }

        public string m_id;
        public Log m_log;
        public Control_Mom m_control;
        public Work_Mom m_work;
        public Light_Mom m_light;
        public Display_Mom m_display;
        public HW_Aligner_Mom m_aligner;
        public int m_nLoadPort = 2;
        
        public bool m_LostLPInfo = false;
        public bool m_bEnableStart = true;
        public bool m_bStageVacuumDone = false;
        public bool m_bFDCUse = false;
        public int m_bFDCITV_Val = 1000;
        public int m_nFDCModuleNum = -1;
        public bool m_bFFUUse = false;  
        public int m_nFFUModuleNum = -1;
        public bool[] m_bDontStartLoading = new bool[3] { false, false, false };
        public enum eProcAutoUnload
        {
            GetLPInfo,
            LPLoadWait,
            CheckLPSlot,
            VisionHome,
            VisionUnload,
            VisionWaitDone,
            StartWTR,
            Idle,
            AlignerReady
        }
        public eProcAutoUnload m_procAutoUnload = eProcAutoUnload.VisionHome;
        public bool m_bPossibleAutoRecover = true;
        protected int m_nLPNumForRecovery = -1;

        public Handler_Mom()
        {
            InitializeComponent();
        }

        public virtual void Init(string id, object auto)
        {
            this.Text = m_id = id;
            m_log = new Log(id, ((Auto_Mom)auto).ClassLogView()); 
            m_control = ((Auto_Mom)auto).ClassControl();
            m_work = ((Auto_Mom)auto).ClassWork();
            m_light = ((Auto_Mom)auto).ClassLight();
            m_display = ((Auto_Mom)auto).ClassDisplay(); 
        }

        public virtual int GetMCResetIONUM()
        {
            return -1;
        }

        public virtual void SetRecoverPortNum(int nLP)
        {
            m_nLPNumForRecovery = nLP;
        }

        public virtual void ShowChild() { }
        public virtual void UnloadStrip() { }
        public virtual void RunEmergency() { }
        public virtual void BoatBlow(bool bBlow) { }
        public virtual void RunInit() { }
        public virtual void RunStart() { }
        public virtual void RunStop() { }
        public virtual void RunRecover() { }
        public virtual void RunLoaderVac(bool bVac) { }
        public virtual bool RunStageVac(bool bVac) { return false; }
        public virtual void CheckFinish() { }
        public virtual void RunTestRecovery() { }
        public virtual bool RunResetPos() { return false; }
        public virtual bool SendHostError(int nCode, string str) { return false; }
        public virtual bool CheckRecoveryStop() { return false; }
        public virtual bool CheckUnloadOK() { return false; }
        public virtual bool CheckStrip() { return false; }
        public virtual bool IsWaferOut() { return false; }
        public virtual void LoaderVac(bool bVac) { }
        public virtual void RunInkChange() { }
        public virtual bool GetHostStart() { return false; }
        protected virtual void RunTimer() { }
        public virtual void LotStart(InfoCarrier infoCarrier) { }
        public virtual void LotEnd(InfoCarrier infoCarrier) { }
        
        public virtual Info_Carrier ClassCarrier(int nIndex) { return null; }
        public virtual int GetRunStepState() { return 0; }
        public virtual HW_LoadPort_Mom ClassLoadPort(int nIndex) { return null; }
        public virtual HW_RFID_Mom ClassRFID(int nIndex) { return null; }
        public virtual HW_Aligner_Mom ClassAligner() { return null; }
        public virtual HW_WTR_Mom ClassWTR() { return null; }
        public virtual HW_FDC_Mom ClassFDC() { return null; }
        public virtual HW_VisionWorks_Mom ClassVisionWorks() { return null; }
        public virtual HW_ImageVS_Mom ClassImageVS() { return null; }
        public virtual HW_Picker_Mom ClassPicker(int nPicker) { return null; }
        public virtual HW_LoadEV ClassLoadEV(int nEV) { return null; }
        public virtual HW_BackSide_Mom ClassBackSide() { return null; }
		public virtual Object ClassLoader() { return null; }
        public virtual HW_PowerMeter ClassPowerMeter() { return null; }
        public virtual HW_FFU_Mom ClassFFU() { return null; } 
        public virtual bool IsWaferExistInEQP() { return false; }
        public virtual bool IsWaferInfoExistInEQP() { return false; }  
        private void timer_Tick(object sender, EventArgs e)
        {
            RunTimer(); 
        }

        private void Handler_Mom_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}

