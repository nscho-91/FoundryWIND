using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using ezTools; 

namespace ezAutoMom
{
    

    public enum eBoatType 
    { 
        eSingle, 
        eDouble 
    };

    public enum eLaser 
    { 
        eOff, 
        eMark, 
        eMatch, 
        eAlways 
    };

    public enum eLampState
    {
        eInit = 0,
        eReady = 1,
        eRunAll = 2,
        eRunhalf = 3,
        eRunNone = 4,
        eAlarm1 = 5,
        eAlarm2 = 6,
        eAlarm3 = 7,
        eReacovery = 8,
        eHome = 9,
        sdd
    }

    public partial class Work_Mom : DockContent
    {
        public class cRunData
        {
            public string sDate = "*";      //
            public string sLotID = "*";     //
            public string sPartNo = "*";     //
            public DateTime dInTime;        //
            public DateTime dOutTime;
            public string dTotalTime;
            public DateTime dRunTime;
            public string sRunTime = "*";
            public string sStopTime = "*";
            public string sUPEH = "0";
            public string sLoadQty = "0";     //
            public cRunData()
            {
                dRunTime = DateTime.Now;
                dInTime = DateTime.Now;
                dOutTime = DateTime.Now;

                sDate = String.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(DateTime.Today.ToString()));

            }
        }

        public class cStopData
        {
            public string sDate = "*";
            public string sErrorCode = "*";
            public string sLotID = "*";
            public string sStartTime = "*";
            public DateTime dStartTime;
            public string sEndTime = "*";
            public string sStopTime = "*";
            public string sContents = "*";

            public cStopData()
            {
                sDate = String.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(DateTime.Today.ToString()));
                dStartTime = DateTime.Now;
            }
        }

        public cRunData m_cRunData = null;
        public cStopData m_cStopData = null;
        public int[] m_diDoorFan = new int[15] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        public enum eFanAlarmPlace
        {
            EFEM_PC_Door_Fan = 0,
            EFEM_TOP_Door_Fan,
            EFEM_WTR_Door_Fan,
            EFEM_PC_Fan,
            EFEM_Illum_Fan,
            EFEM_WTR_Fan,
            EFEM_WTR2_Fan,
            VS_PC_Door_Fan,
            VS_TOP_Door_Fan,
            VS_BOT_Door_Fan,
            VS_PC_Fan,
            VS_Ilum1_Fan,
            VS_Illum2_Fan,
            EFEM_Aligner_Door_Fan,
        }

        enum eError { eStripPosCont, eStripPosTotal };
        public enum eBuzzer { Buzzer, Warning, Error, Home }; 
        public string m_strModel = "Model";
        public string m_strCustomer = "Customer";
        public string m_strLot = "LotID";
        public string m_strWorker = "Worker";
        public string m_strFilePath = "d:\\";
        public bool m_bMapDataUse = false;
        public bool m_bManualInspect = false;

        public bool m_bPickerSetDone = false;
        public bool m_bJobOpen = false;

        public bool m_bDoorLock = true, m_bDryRun = false, m_bUseMGZ, m_bCheckGV;
        public bool m_bManualRun = false;
        public bool m_bWithoutCommRun = false;
        public bool m_bRNR = false;
        public bool m_bAFVI = true;
        public bool m_bAFVIDemo = false; 
        public bool m_bCleanerSW, m_bCleanerAir, m_bCamThres = false;
        public bool m_bArms = false;
        public bool[] m_bCleanerUse = new bool[2] { true, true };
        public int m_nStrip = 0, m_nStripStart = 0, m_nCheckStart = 0, m_nThres = -1;
        public int[] m_nCycleStop = new int[2] { 0, 1 };
        public int[,] m_nCleaner = new int[2, 2] { { 0, 0 }, { 0, 0 } };
        public eBoatType m_eBoat = eBoatType.eDouble;
        public eLaser m_eLaser = eLaser.eMark;
        public double[] m_fStripCX = new double[2] { 77, 77 };
        public double[] m_fStripCY = new double[2] { 240, 240 };
        public double[] m_fStripCZ = new double[2] { 0.27, 0.27 };
        public double[] m_fMGZCX = new double[2] { 83, 83 };
        public double m_fStripDZ = 0;
        public double m_fChipCX = 10;
        public double m_fChipCY = 10; 
        public bool[] m_bUseBoat = new bool[2] { true, true };
        public bool[] m_bUsePaper = new bool[2] { true, false };        
        public bool m_bUseRoller = true; 
        public bool m_bInvalid = false;
        protected bool m_bEnableSlowMove = true;
        public bool m_bGoodSort = false; //kns20171222
        public string m_strDoorName = "";
        public int m_nLaserShutDown = 0;
        public bool bLightCurtainErr = false;           //190421 SDH ADD
        public int m_nBundleMax = 20;
        public bool m_bFastBamboo = true; 

        public bool m_bTestRun = false;
        public bool m_bXgemUSe = false;
        
        public int m_nStripTakt = 0, m_nTaktLot = 0, m_nCheckMark = 0; 
        public int[,] m_nBadCount = new int[2, 2] { { 0, 0 }, { 0, 0 } };
        public int[,] m_nStripPos = new int[3, 2] { { 0, 0 }, { 0, 0 }, { 0, 0 } };
        public double m_fTaktTime = 0, m_fTaktAve = 0, m_fTaktTot = 0;
        public bool[] bLPCommStatus = { false, false, false };
        ezStopWatch[] m_swTakt = new ezStopWatch[3];
        protected ezStopWatch m_swLamp = new ezStopWatch(); 

        protected string m_id;
        public Work_Run m_run = null;
        protected Log m_log;
        public ezGrid m_grid;
        public object m_auto;
        XGem300_Mom m_xGem;
        Size m_dzGrid; 
        public bool m_bBuzzerOff = false;
        protected bool m_bWorkerBuzzerOn = false;
        public LampSet_Mom m_Lamp;
        public Control_Mom m_control;

        string[] m_sRuns = new string[]
        {
            "Org",
            "2017",
        };
        string m_sRun = "Org"; 

        public Work_Mom()
        {
            InitializeComponent();
            m_dzGrid = this.Size - grid.Size; 
        }

        public virtual void Init(string id, object auto)
        {
            int n;
            this.Text = m_id = id; m_auto = auto; 
            m_log = new Log(id, ((Auto_Mom)auto).m_logView, "Work");
            m_grid = new ezGrid(id, grid, m_log, true);
            labelModel.Text = m_log.m_strModel;
            InitWorkRun(); 
            m_xGem = ((Auto_Mom)auto).ClassXGem();
            timerEnable.Enabled = true; 
            for (n = 0; n < 3; n++) m_swTakt[n] = new ezStopWatch();
            m_Lamp = new LampSet_Mom();
            m_control = new Control_Mom();
            m_cRunData = new cRunData();
            m_cStopData = new cStopData();
        }

        public virtual void ThreadStop()
        {
            m_run.ThreadStop(); 
        }

        public virtual void InitString() 
        {
            InitString(eError.eStripPosCont, "Too Many Continue Strip Pos Error !!");
            InitString(eError.eStripPosTotal, "Too Many Total Strip Pos Error !!");
        } 

        void InitString(eError eErr, string str)
        {
            m_log.AddString(str);
            if (m_xGem == null) return;
            m_xGem.AddALID(m_id, (int)eErr, str);
        } 

        void SetAlarm(eAlarm alarm, eError eErr)
        {
            SetError(alarm, m_log, (int)eErr);
            if (m_xGem == null) return;
            m_xGem.SetAlarm(m_id, (int)eErr);
        } 

        public virtual void JobOpen(ezJob job) { }
        public virtual void JobSave(ezJob job) { }

        private void buttonRunStart_Click(object sender, EventArgs e)
        {
            m_run.Run(); 
        }

        private void buttonRunPause_Click(object sender, EventArgs e)
        {
            m_run.Stop(); 
        }

        private void buttonRunReset_Click(object sender, EventArgs e)
        {
            m_run.Reset(); 
        }

        private void buttonRunHome_Click(object sender, EventArgs e)
        {
            m_run.Home(); 
        }

        private void buttonRunNew_Click(object sender, EventArgs e)
        {
            if (IsRun()) return;
            m_log.HidePopup(true);
            if (MessageBox.Show("New Lot ?", "New Lot", MessageBoxButtons.YesNo) != DialogResult.Yes) { m_log.HidePopup(false); return; }
            m_log.HidePopup(false);
            Clear(); ClearStrip(); m_bInvalid = true;
            ((Auto_Mom)m_auto).Clear(); m_log.Add("<New>");
        }
        private void buttonRunCycle_Click(object sender, EventArgs e)
        {
            m_nCycleStop[0] = 0; 
            RunCycleStop(); 
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate);
            RunGrid(eGrid.eRegWrite);
            if (m_grid.m_bInitGrid) RunGrid(eGrid.eInit); 
        }

        private void Work_Mom_Resize(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            Size sz = control.Size;
            if (control.Text == "Work_Mom") { grid.Size = control.Size - m_dzGrid; }
        }

        void InitWorkRun()
        {
            if (m_sRun == "2017")
            {
                Work_Run_2017 run2017 = new Work_Run_2017();
                run2017.Init(m_id, m_auto);
                m_run = run2017;
                return; 
            }
            Work_Run_Org runOrg = new Work_Run_Org();
            runOrg.Init(m_id, m_auto);
            m_run = runOrg;
        }

        public virtual void ModelGrid(ezGrid rGrid, eGrid eMode)
        {
            rGrid.Set(ref m_sRun, m_sRuns, "Work", "Run", "Work Run Mode"); 
        }

        public virtual bool GetDoorLockInState()
        {
            return false;
        }

        public virtual bool GetDoorLockOutState()
        {
            return false;
        }
        public virtual void SetDoorLockOff()
        {

        }

        public virtual void RunGrid(eGrid eMode, ezJob job = null)
        {
            m_grid.Update(eMode, job);
            m_grid.Set(ref m_strModel, "Work", "Model", "Model Name");
            m_grid.Set(ref m_strCustomer, "Work", "Customer", "Model Name");
            m_grid.Set(ref m_strLot, "Work", "Lot", "Model Name");
            m_grid.Set(ref m_nCycleStop[1], "CycleStop", "Count", "Cycle Stop Strip Count");
            m_grid.Set(ref m_fStripCX[0], "Strip", "Width", "Strip Width (mm)");
            m_grid.Set(ref m_fStripCY[0], "Strip", "Length", "Strip Length (mm)");
            m_grid.Set(ref m_fStripCZ[0], "Strip", "Thickness", "Strip Thickness (mm)");
            m_grid.Set(ref m_bDoorLock, "Option", "DoorLock", "Check Door Lock", ((Auto_Mom)m_auto).m_login.m_lvLogin < 2);           
            m_grid.Set(ref m_nStrip, "Time", "Strip", "Strip Index", true);
            m_grid.Set(ref m_nTaktLot, "Time", "Lot", "Lot Time (min)", true);
            m_grid.Set(ref m_fTaktTime, "Time", "Takt", "Takt Time (sec)", true);
            m_grid.Set(ref m_fTaktAve, "Time", "Ave", "Average Takt Time (sec)", true);
        }

        public virtual void SetError(eAlarm alarm, Log log, int iError)
        {
            if (m_run == null) return;
            m_run.SetError(alarm, log, iError); 
        }

        public bool IsRun()
        {
            if (m_run == null) return false; 
            return (m_run.GetState() == eWorkRun.Run);
        }

        public bool IsReady()
        {
            return (m_run.GetState() == eWorkRun.Ready);
        }

        public bool IsManual()
        {
            return m_run.IsManual(); 
        }

        public eWorkRun GetState()
        {
            return m_run.GetState(); 
        }

        public double GetSizeX(bool bDelta)
        {
	        if (bDelta) return m_fStripCX[0] - m_fStripCX[1]; else return m_fStripCX[0]; 
        }

        public double GetSizeY(bool bDelta)
        {
	        if (bDelta) return m_fStripCY[0] - m_fStripCY[1]; else return m_fStripCY[0];
        }

        public double GetSizeZ(bool bDelta)
        {
	        if (bDelta) return m_fStripCZ[0] - m_fStripCZ[1]; else return m_fStripCZ[0];
        }

        public double GetMGZSizeX(bool bDelta)
        {
	        if (bDelta) return m_fMGZCX[0] - m_fMGZCX[1]; else return m_fMGZCX[0];
        }

        public void GetRejectValue(int nID, ref int nBadCont, ref int nBadMax)
        {
            nBadCont = m_nBadCount[nID, 0];
            nBadMax = m_nBadCount[nID, 1]; 
        }

        public void StripPosError(int nID, bool bError)
        {
            if (!IsRun()) return;
            if (bError) { m_nStripPos[nID, 0]++; m_nStripPos[nID, 1]++; } else m_nStripPos[nID, 0] = 0;
            if ((m_nStripPos[2, 0] > 0) && (m_nStripPos[nID, 0] >= m_nStripPos[2, 0])) SetAlarm(eAlarm.Stop, eError.eStripPosCont);
            if ((m_nStripPos[2, 1] > 0) && (m_nStripPos[nID, 1] >= m_nStripPos[2, 1])) SetAlarm(eAlarm.Stop, eError.eStripPosTotal); 
        }

        public virtual void Clear()
        {
            m_nStrip = 0; 
            m_nStripPos[0, 0] = m_nStripPos[0, 1] = m_nStripPos[1, 0] = m_nStripPos[1, 1] = 0;
        }

        public bool IsUseMark()
        {
            return (m_eLaser != eLaser.eOff);
        }

        public bool IsUseBoat(int nID)
        {
            return m_bUseBoat[nID % 2];
        }

        public bool IsUsePaper(int nID)
        {
            return m_bUsePaper[nID % 2];
        }

        public int GetStrip()
        {
            return m_nStrip; 
        }

        public void ClearStrip()
        {
            m_nStrip = 0; m_fTaktTime = 0; m_fTaktAve = 0; m_nTaktLot = 0; m_fTaktTot = 0; 
        }

        public void IncStrip()
        {
            if (!IsRun() && (m_nStrip <= 0)) return;
            m_nStrip++;
            m_fTaktTime = m_fTaktAve = 0; 
            if (m_nStrip == 1) m_swTakt[0].Start();
            if (m_nStrip == m_nStripStart) 
            {
                m_fTaktTime = 0;
                m_swTakt[1].Start(); 
                m_swTakt[2].Start();
                m_nStripTakt = m_nStrip; 
            }
            if (m_nStrip > m_nStripTakt) 
            { 
                m_fTaktTime = 0.001 * m_swTakt[1].Check();
                m_swTakt[1].Start(); 
                m_nStripTakt = m_nStrip;
            }
            if (m_nStrip > m_nStripStart) 
            { 
                m_fTaktAve = 0.001 * m_swTakt[2].Check() / (m_nStrip - m_nStripStart); 
            }
            m_nTaktLot = (int)(m_swTakt[0].Check() / 60000); 
            if (m_nStrip == 0) 
                m_fTaktTot = 0; 
            else m_fTaktTot = 0.001 * m_swTakt[0].Check() / m_nStrip;
            m_fTaktTime = Math.Round(m_fTaktTime, 3); m_fTaktAve = Math.Round(m_fTaktAve, 3); m_fTaktTot = Math.Round(m_fTaktTot, 3); 
            m_bInvalid = true;
        }

        public virtual void RunCycleStop()
        {
            if (IsRun()) { m_nCycleStop[0] = 1; m_log.Add("Cycle Stop"); return; }
            if (IsReady()) { m_run.Run(); m_nCycleStop[0] = m_nCycleStop[1]; m_log.Add("Cycle Start"); return; }
        }

        public bool IsCycleStop()
        {
            switch (m_nCycleStop[0])
            {
                case 0: return false;
                case 1: m_nCycleStop[0]--; return true;
                default: m_nCycleStop[0]--; return false;
            }
        }

        public bool IsCheckMark(int nStrip)
        {
            if (m_nCheckMark == 0) return false;
            if ((m_nCheckMark == 1) || ((nStrip % m_nCheckMark) == 1)) return true;
            return false;
        }

        public string GetID()
        {
            return m_strModel + "_" + m_strCustomer + "_" + m_strLot;
        }

        private void timerInvalid_Tick(object sender, EventArgs e)
         {
            if (m_grid == null) return;
            if (m_bInvalid) { RunGrid(eGrid.eInit); m_bInvalid = false; }
        }

        private void timerEnable_Tick(object sender, EventArgs e)
        {
            if (m_auto == null) return;
            ((Auto_Mom)m_auto).RunLogin();
            RunTimer(); 
        }

        protected virtual void RunTimer()
        {
        }

        private void Work_Mom_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public bool IsSlowMove()
        {
            return m_bEnableSlowMove && IsDoorOpen(); 
        }

        public void RunLogin(int lvLogin)
        {
            grid.Enabled = (lvLogin > 0);
        }

        public virtual bool IsStartEnable(bool bPopup) { return false; }

        public virtual bool IsEMGError() { return true; }
        public virtual bool IsVacError() { return true; }
        public virtual bool IsCDAError() { return false; }

        public virtual bool IsDoorOpen() { return true; }
        public virtual bool IsFanAlarm() { return true; }

        public virtual void RunLamp() { }
        public virtual bool IsDI_Start() { return false; }
        public virtual bool IsDI_Stop() { return false; }
        public virtual bool IsDI_Reset() { return false; }
        public virtual bool IsDI_Home() { return false; }
        public virtual bool IsDI_PickerSet() { return false; }
        public virtual bool IsDI_BuzzerOff() { return false; }
        public virtual bool IsDI_DoorOpen() { return false; }

        public virtual void WorkerBuzzerOn() { m_bWorkerBuzzerOn = true; }
        public virtual void WorkerBuzzerOff() { m_bWorkerBuzzerOn = false; }

        public virtual void RunBuzzer(eBuzzer eID, bool bOn) { }
        public virtual bool IsBuzzerOn() { return false; }

        public virtual object GetManualJob() { return null; }
        public virtual object MakeUI() { return null; }
        public virtual object GetSSEMGEM() {return null; }
        public virtual bool IsLightCurtainError() { return false; }

        public void RunBuzzerOff() 
        {
            m_bBuzzerOff = true; 
            for (int n = 0; n<4; n++)
            {
                RunBuzzer((eBuzzer)n, false); 
            }
        }

        public virtual void RunDoorLock(bool bOn) { }

        public virtual void RunLightOn(int nID, bool bOn) { }

        public GroupBox GetGroupBox() { return groupBox1; } //박상영20180124
    }
}
