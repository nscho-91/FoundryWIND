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
using ezAutoMom;
using ezTools;

namespace ezAutoMom
{
    public partial class HW_WTR_Mom : Form
    {
        public enum eState
        {
            Init,
            Home,
            Ready,
            Run,
            AutoUnload,
            Error,
            Pause,
        }

        public enum eArm
        {
            Lower,
            Upper,
            Single,
            Both,
        }
        protected eArm m_eArmMode = eArm.Both;
        string[] m_strArmMode = Enum.GetNames(typeof(eArm));
        protected Wafer_Size[] m_wsArm = new Wafer_Size[2];

        protected enum eTeaching
        {
            LoadPort0,
            LoadPort1,
            LoadPort2,
            Aligner,
            Vision,
            BackSide,
            None
        }
        Info_Wafer.eLocate m_eGetLoc = Info_Wafer.eLocate.LoadPort;

        protected int[,] m_nTeaching = new int[(int)Wafer_Size.eSize.Empty, (int)eTeaching.None];
        //[4inch, 5inch, 6inch, 200mm, 300mm, 300mm_RF][LP0, LP1, LP2, AL, VW, BS]

        enum eProcMode
        {
            ATI,
            BackSide90
        }

        public enum eError_Mom
        {
            LPOpen,
            CarrierCheck,
            ArmClose,
            GetLoadPort,
            PutLoadPort,
            GetAligner,
            PutAligner,
            AlingerExist,
            GetVision,
            PutVision,
            VisionReady,
            VisionWaferInfo,
            VisionExist,
            GetBackside,
            PutBackside,
            BacksideExsit,
            PutterTwoArm,
            DcollExist,
            LPClose, 
            End
        } 


        eProcMode m_eProcMode = eProcMode.ATI;
        string[] m_strProcModes = Enum.GetNames(typeof(eProcMode));

        protected string m_id;
        protected string m_sError;
        public bool m_bCycleStop = false;
        Size[] m_sz = new Size[2];
        protected Log m_log;
        protected ezGrid m_grid;
        protected Auto_Mom m_auto;
        protected Control_Mom m_control;
        protected XGem300_Mom m_xGem;
        protected Work_Mom m_work;
        protected Handler_Mom m_handler;
        protected HW_Aligner_Mom m_aligner = null;
        protected HW_VisionWorks_Mom m_vision = null;
        protected HW_BackSide_Mom m_backSide = null;
        protected Recipe_Mom m_recipe = null;
        int m_indexCarrier = -1;
        protected Info_Carrier[] m_infoCarrier = new Info_Carrier[3] { null, null, null };
        protected HW_LoadPort_Mom[] m_loadPort = new HW_LoadPort_Mom[3] { null, null, null };
        protected Info_Wafer[] m_InfoWafer = new Info_Wafer[2] { null, null };
        protected eState m_eState = eState.Init;
        protected int m_nLoadPort = 2;

        protected eHWResult[] m_waferExist = new eHWResult[2] { eHWResult.Off, eHWResult.Off };       //Wafer Check {Lower Arm, Upper Arm}

        protected bool m_bRunThread = false;
        Thread m_thread;

        bool m_bRotateVision = false;
        protected string m_sSpeed = ""; // ing 170626

        public int m_msHome = 2000;
        public int m_msWaitWTRShift = 2*60*1000;
        public double m_dShiftLimit = 10; // BHJ 190305 add
        public MarsLog m_logMars; 

        public HW_WTR_Mom()
        {
            InitializeComponent();
            m_sz[0] = m_sz[1] = this.Size;
            m_sz[0].Height = 26;
        }

        void SetAlarm(eAlarm alarm, eError_Mom eErr)
        {
            SetState(eState.Error);
            m_work.SetError(alarm, m_log, (int)eErr);
            if (m_xGem == null) return;
            m_xGem.SetAlarm(m_id, (int)eErr);
        } 

        public virtual void Init(string id, Auto_Mom auto)
        {
            m_id = id;
            m_auto = auto;
            m_sz[0] = m_sz[1] = this.Size;
            m_sz[0].Height = 26;
            m_log = new Log(m_id, m_auto.ClassLogView(), "WTR");
            m_grid = new ezGrid(m_id, grid, m_log, false);
            m_control = m_auto.ClassControl();
            m_xGem = m_auto.ClassXGem();
            m_work = m_auto.ClassWork();
            m_handler = m_auto.ClassHandler();
            m_aligner = m_handler.ClassAligner();
            m_vision = m_handler.ClassVisionWorks();
            m_backSide = m_handler.ClassBackSide();
            m_recipe = m_auto.ClassRecipe();
            m_nLoadPort = m_handler.m_nLoadPort;
            InitString();
            for (int n = 0; n < m_nLoadPort; n++)
            {
                m_infoCarrier[n] = m_handler.ClassCarrier(n);
                m_loadPort[n] = m_handler.ClassLoadPort(n);
            }
            m_wsArm[0] = new Wafer_Size(m_log);
            m_wsArm[1] = new Wafer_Size(m_log);
            m_thread = new Thread(new ThreadStart(RunThread)); m_thread.Start();
            InitComboBox();
            m_logMars = m_auto.ClassMarsLog();
        }

        void InitString()
        {
            InitString((int)eError_Mom.LPOpen, "Load port Does Not Open. Check Loadport Open Sensor");
            InitString((int)eError_Mom.CarrierCheck, "Cassette Error , Please Check Wafer Size & Casstte");
            InitString((int)eError_Mom.ArmClose, "Arm Close Error");
            InitString((int)eError_Mom.GetLoadPort, "Get LoadPort Error");
            InitString((int)eError_Mom.PutLoadPort, "Put LoadPort Error");
            InitString((int)eError_Mom.GetAligner, "Get Aligner Error");
            InitString((int)eError_Mom.PutAligner, "Put Aligner Error");
            InitString((int)eError_Mom.AlingerExist, "Aligner Wafer Exist Error");
            InitString((int)eError_Mom.GetVision, "Get Vision Error");
            InitString((int)eError_Mom.PutVision, "Put Vision Error");
            InitString((int)eError_Mom.VisionReady, "Wafer Stage And Lifter is Not Ready Pos or Vision Ready Signal Does Not Turn On");
            InitString((int)eError_Mom.VisionWaferInfo, "Wafer Information on Vision Was Losted");
            InitString((int)eError_Mom.VisionExist, "Wafer Does Not Exist On The Vision Stage");
            InitString((int)eError_Mom.GetBackside, "Get Backside Error");
            InitString((int)eError_Mom.PutBackside, "Put Backside Error");
            InitString((int)eError_Mom.BacksideExsit, "Bacskide Wafer Exist Error");
            InitString((int)eError_Mom.PutterTwoArm, "Putter Does Not Use TwoArm ");
            InitString((int)eError_Mom.DcollExist, "Dcoll data not Exist");      //jky 200904 add
            InitString((int)eError_Mom.LPClose, "Load port Does Open. Check Loadport Open Sensor");
        }

        void InitString(int eErr, string str) 
        {
            m_log.AddString(str);
            if (m_xGem == null) return;
            m_xGem.AddALID(m_id, (int)eErr, str);
        } 

        void InitComboBox()
        {
            comboArm.Items.Clear();
            comboArm.Items.Add(eArm.Lower.ToString());
            comboArm.Items.Add(eArm.Upper.ToString());
            comboArm.SelectedIndex = 0;

            comboPosition.Items.Clear();
            for (int i = 0; i < m_nLoadPort; i++) comboPosition.Items.Add(((eTeaching)i).ToString());
            comboPosition.Items.Add(eTeaching.Aligner.ToString());
            comboPosition.Items.Add(eTeaching.Vision.ToString());
            comboPosition.Items.Add(eTeaching.BackSide.ToString());
            comboPosition.SelectedIndex = 0;

            for (int i = 1; i <= 25; i++) comboSlot.Items.Add(i.ToString());
            comboSlot.SelectedIndex = 0;

            for (int i = 0; i < (int)(Wafer_Size.eSize.Empty); i++)
            {
                comboWaferSize.Items.Add(((Wafer_Size.eSize)i).ToString());
            }
            comboWaferSize.SelectedIndex = 0;

        }

        protected virtual void RunGrid(eGrid eMode)
        {
            m_grid.Update(eMode, null);
            RunGridArmMode(m_grid, eMode);
            RunGridArmEnable(m_grid, eMode, eArm.Lower, "LowerArm");
            RunGridArmEnable(m_grid, eMode, eArm.Upper, "UpperArm");
            RunGridProcMode(m_grid, eMode);
        }

        void RunGridProcMode(ezGrid rGrid, eGrid eMode)
        {
            string strProcMode = m_eProcMode.ToString();
            rGrid.Set(ref strProcMode, m_strProcModes, "Mode", "Proc", "Process Mode");
            int n = 0;
            foreach (string str in m_strProcModes)
            {
                if (str == strProcMode)
                {
                    if ((int)m_eProcMode != n) rGrid.m_bInitGrid = true;
                    m_eProcMode = (eProcMode)n;
                    return;
                }
                n++;
            }
        }

        void RunGridArmMode(ezGrid rGrid, eGrid eMode)
        {
            string strArm = m_eArmMode.ToString();
            rGrid.Set(ref strArm, m_strArmMode, "Mode", "UseArm", "Use Arm Mode");
            int n = 0;
            foreach (string str in m_strArmMode)
            {
                if (str == strArm)
                {
                    if ((int)m_eArmMode != n) rGrid.m_bInitGrid = true;
                    m_eArmMode = (eArm)n;
                    return;
                }
                n++;
            }
        }

        void RunGridArmEnable(ezGrid rGrid, eGrid eMode, eArm eArmMode, string strGroup)
        {
            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++)
            {
                string strSize = ((Wafer_Size.eSize)n).ToString();
                rGrid.Set(ref m_wsArm[(int)eArmMode].m_bEnable[n], strGroup, strSize, "Enable Whole Wafer Size");
            }
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            eArm eArm0 = m_eArmMode;
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate);
            RunGrid(eGrid.eRegWrite);
            if (m_grid.m_bInitGrid) RunGrid(eGrid.eInit);
        }

        public virtual void ThreadStop()
        {
            if (m_bRunThread)
            {
                m_bRunThread = false;
                m_thread.Join();
            }
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

        public eState GetState()
        {
            return m_eState;
        }

        public Info_Wafer GetInfoWafer(eArm nArm)
        {
            if (nArm == eArm.Single)
            {
                if (m_InfoWafer[(int)eArm.Lower] != null) return m_InfoWafer[(int)eArm.Lower];
                if (m_InfoWafer[(int)eArm.Upper] != null) return m_InfoWafer[(int)eArm.Upper];
                return null;
            }
            return m_InfoWafer[(int)nArm];
        }

        public void SetInfoWafer(eArm nArm, Info_Wafer infoWafer)
        {
            m_InfoWafer[(int)nArm] = infoWafer;
            if (infoWafer == null) return;
            if (nArm == eArm.Upper) m_InfoWafer[(int)nArm].Locate = Info_Wafer.eLocate.WTR_Upper;
            else m_InfoWafer[(int)nArm].Locate = Info_Wafer.eLocate.WTR_Lower;
        }

        static readonly object m_csLock = new object();

        protected Info_Wafer GetLoadPortWafer()
        {
            //if (RunWTRMapping() == eHWResult.Error) SetState(eState.Error); // ing 20171029 WTR Mapping 시컨스 변경
            lock (m_csLock)
            {
                if (m_indexCarrier < 0)
                {
                    m_indexCarrier = GetIndexCarrier();
                }
                if (m_indexCarrier < 0) return null;
                return m_infoCarrier[m_indexCarrier].GetLoadWafer();
            }
        }

        int GetIndexCarrier()
        {
            int nIndex = -1;
            int nFirstLot = 0;
            for (int n = 0; n < m_nLoadPort; n++)
            {
                if ((m_infoCarrier[n] != null) && (m_infoCarrier[n].m_eState == Info_Carrier.eState.Ready))
                {
                    if (nIndex == -1) nIndex = n;
                    for (int i = 0; i < m_infoCarrier[n].m_lWafer; i++)
                    {
                        if (m_infoCarrier[n].m_infoWafer[i].State == Info_Wafer.eState.Run) nFirstLot++;
                    }
                }
            }
            if (nIndex >= 0) return nIndex;
            return nIndex;

        }

        public void ClearIndexCarrier()
        {
            m_indexCarrier = -1;
        }

        //KDG 161007 아래 함수와 같은 내용이라 삭제
        /*
        public  virtual eHWResult IsWaferExist(eArm nArm) 
        {
            return m_waferExist[(int)nArm]; 
        }
        */

        public virtual eHWResult CheckWaferExist(eArm nArm) { return eHWResult.Off; }
        protected virtual void RunVac(eArm nArm, bool bVac) { }
        protected virtual eHWResult RunGrip(eArm nArm, bool bOn) { return eHWResult.Off; }
        protected virtual eHWResult RunHome() { return eHWResult.Error; }
        protected virtual eHWResult AfterHome() { return eHWResult.Error; }
        public virtual bool IsArmClose(eArm nArm, bool bWait = false) { return false; } // ing 161018
        protected virtual void ErrorReset() { }
        protected virtual void ProcError() { }
        protected virtual eHWResult RunGetMotion(eArm nArm, int nPos, int nSlot, double mmShift = 0) { return eHWResult.Error; }
        protected virtual eHWResult RunPutMotion(eArm nArm, int nPos, int nSlot, double mmShift = 0) { return eHWResult.Error; }
        protected virtual eHWResult RunGetExtendMotion(eArm nArm, int nPos, int nSlot) { return eHWResult.Error; }
        protected virtual eHWResult RunGetLPSLimSlot(eArm nArm, int nPos, int nSlot) { return eHWResult.Error; }
        protected virtual eHWResult RunPutLPSLimSlot(eArm nArm, int nPos, int nSlot) { return eHWResult.Error; }
        protected virtual eHWResult RunGetRetractMotion(eArm nArm, int nPos, int nSlot) { return eHWResult.Error; }
        protected virtual eHWResult RunPutExtendMotion(eArm nArm, int nPos, int nSlot) { return eHWResult.Error; }
        protected virtual eHWResult RunPutRetractMotion(eArm nArm, int nPos, int nSlot) { return eHWResult.Error; }
        protected virtual eHWResult RunGetReady(eArm nArm, int nPos, int nSlot) { return eHWResult.Error; }
        protected virtual eHWResult RunPutReady(eArm nArm, int nPos, int nSlot) { return eHWResult.Error; }
        protected virtual eHWResult RunWTRMapping() { return eHWResult.OK; }
        protected virtual eHWResult RunWTRMapping(int nLP) { return eHWResult.OK; } // ing 20171029 WTR Mapping 시컨스 변경
        public virtual eHWResult RunGoHome() { return eHWResult.Error; } // BHJ 190304 add 


        protected virtual void SetProcQue(Info_Wafer infoWafer)
        {
            infoWafer.m_queProc.Clear(); // ing 170401
            if (m_eProcMode == eProcMode.BackSide90)
            {
                if (m_recipe.m_bUseVision)
                {
                    infoWafer.m_queProc.Enqueue(Info_Wafer.eProc.Align);
                    infoWafer.m_queProc.Enqueue(Info_Wafer.eProc.Vision);
                    if (m_recipe.m_bUseBackSide) infoWafer.m_queProc.Enqueue(Info_Wafer.eProc.BackSide);
                }
                else
                {
                    if (infoWafer.m_bUseAligner) infoWafer.m_queProc.Enqueue(Info_Wafer.eProc.Align);
                    else infoWafer.m_queProc.Enqueue(Info_Wafer.eProc.BCR);
                    if (m_recipe.m_bUseBackSide) infoWafer.m_queProc.Enqueue(Info_Wafer.eProc.BackSide);
                }
            }
            else
            {
                infoWafer.m_queProc.Enqueue(Info_Wafer.eProc.Align);
                if (m_recipe.m_bUseVision) infoWafer.m_queProc.Enqueue(Info_Wafer.eProc.Vision);
                if (m_recipe.m_bUseBackSide) infoWafer.m_queProc.Enqueue(Info_Wafer.eProc.BackSide);
            }
            infoWafer.m_queProc.Enqueue(Info_Wafer.eProc.LoadPort);
            infoWafer.m_eProcNext = (Info_Wafer.eProc)infoWafer.m_queProc.Dequeue(); // ing 170327
        }

        public eHWResult SetHome()
        {
            if ((m_eState == eState.Run) || (m_eState == eState.Error))
            {
                m_log.Popup("Can't Start Home When State is " + m_eState.ToString());
                return eHWResult.Error;
            }
            SetState(eState.Home);
            return eHWResult.OK;
        }

        public void StartAutoUnload()
        {
            SetState(eState.AutoUnload);
        }

        public eHWResult SetPause()
        {
            if (m_eState != eState.Run)
            {
                m_log.Popup("Can't Pause WTR When State is " + m_eState.ToString());
                return eHWResult.Error;
            }
            SetState(eState.Pause);
            return eHWResult.OK;
        }

        public eHWResult SetContinue()
        {
            if (m_eState != eState.Pause)
            {
                m_log.Popup("Can't Continue WTR When State is " + m_eState.ToString());
                return eHWResult.Error;
            }
            SetState(eState.Run);
            return eHWResult.OK;
        }

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
                        ClearIndexCarrier();
                        if (m_logMars != null) m_logMars.AddFunctionLog("WTR", "InitialIze", MarsLog.eStatus.Start, "", MarsLog.eMateral.Wafer, null); 
                        if (RunHome() == eHWResult.OK)
                        {
                            if (AfterHome() == eHWResult.Error)
                            {
                                m_log.WriteLog("Sequence", "[" + m_id + "Done]" + " Home FAIL");
                                SetState(eState.Error);
                            }
                            m_log.WriteLog("Sequence", "[" + m_id + "Done]" + " Home PASS");
                            SetState(eState.Ready);
                        }
                        else 
                        {
                            SetState(eState.Error);
                        }
                        if (m_logMars != null) m_logMars.AddFunctionLog("WTR", "InitialIze", MarsLog.eStatus.End, "", MarsLog.eMateral.Wafer, null);
                        break;

                    case eState.Ready:
                        if (m_loadPort[0].GetState() == HW_LoadPort_Mom.eState.LoadDone) if (RunWTRMapping(0) == eHWResult.Error) SetState(eState.Error); // ing 20171029 WTR Mapping 시컨스 변경
                        if (m_loadPort[1].GetState() == HW_LoadPort_Mom.eState.LoadDone) if (RunWTRMapping(1) == eHWResult.Error) SetState(eState.Error); // ing 20171029 WTR Mapping 시컨스 변경
                        m_bCycleStop = false;
                        switch (m_work.GetState())
                        {
                            case eWorkRun.Run: SetState(eState.Run); break;
                        }
                        break;
                    case eState.Run:
                        if (m_loadPort[0].GetState() == HW_LoadPort_Mom.eState.LoadDone) if (RunWTRMapping(0) == eHWResult.Error) SetState(eState.Error); // ing 20171029 WTR Mapping 시컨스 변경
                        if (m_loadPort[1].GetState() == HW_LoadPort_Mom.eState.LoadDone) if (RunWTRMapping(1) == eHWResult.Error) SetState(eState.Error); // ing 20171029 WTR Mapping 시컨스 변경
                        if (m_eArmMode == eArm.Both)
                            StateRunBoth();
                        else
                            StateRunSingle(m_eArmMode);
                        break;
                    case eState.AutoUnload:
                        StateAutoUnload();
                        break;
                    case eState.Error: ProcError();
                        break;
                }
            }
        }

        void StateAutoUnload()
        {
            if (m_work.GetState() != eWorkRun.AutoUnload)
            {
                SetState(eState.Ready);
                return;
            }

            if (RunAutoUnload(eArm.Lower)) return;
            if (RunAutoUnload(eArm.Upper)) return;
            if (m_aligner.InfoWafer != null)
            {
                if ((m_aligner.InfoWafer != null) && (m_aligner.GetState() == HW_Aligner_Mom.eState.Done || m_aligner.GetState() == HW_Aligner_Mom.eState.Ready))
                {
                    RunGet(eTeaching.Aligner, GetArmGet(m_eArmMode, m_aligner.InfoWafer));
                    return;
                }
                else return; // ing 161022
            }
            if (m_backSide.InfoWafer != null)
            {
                if ((m_backSide.InfoWafer != null) && (m_backSide.GetState() == HW_BackSide_Mom.eState.Done || m_backSide.GetState() == HW_BackSide_Mom.eState.Ready))
                {
                    RunGet(eTeaching.BackSide, GetArmGet(m_eArmMode, m_backSide.InfoWafer));
                    return;
                }
                else return; // ing 161022
            }

            if (m_vision.IsWaferExist() == eHWResult.On || m_vision.m_bManualWaferExist)
            {
                if (m_vision.IsWFStgReady() == eHWResult.OK && m_vision.IsLifterUp() == true && m_vision.IsVSReadySignal() == true && m_vision.IsWaferExistLoad() == true || m_vision.m_bManualWaferExist)
                {
                    Thread.Sleep(15000);        // Lifter up time check; 
                    RunGet(eTeaching.Vision, GetArmGet(m_eArmMode, m_vision.InfoWafer));
                    m_vision.m_bManualWaferExist = false;
                }
                return;
            }

            for (int n = 0; n < m_nLoadPort; n++)
            {
                foreach (Info_Wafer wafer in m_loadPort[n].m_infoCarrier.m_infoWafer)
                {
                    if (wafer == null) continue;
                    if (wafer.m_eAngleRF != Info_Wafer.eAngleRF.R0 && wafer.m_wafer.IsRingFrame() && wafer.m_bRotate180)
                    {
                        RunGet(eTeaching.LoadPort0, GetArmGet(m_eArmMode, wafer), wafer);
                        return;
                    }
                }
            }
            m_handler.m_LostLPInfo = false;                 //190516 SDH ADD
            m_work.m_run.Stop();
        }

        bool RunAutoUnload(eArm nArm) // ing modify 161026
        {
            int nLoadPort;
            if (GetInfoWafer(nArm) == null) return false;
            GetInfoWafer(nArm).m_queProc.Clear(); // ing 170401
            if (GetInfoWafer(nArm).m_bRotate180 && GetInfoWafer(nArm).m_wafer.IsRingFrame() && GetInfoWafer(nArm).m_eAngleRF != Info_Wafer.eAngleRF.R0)
            {
                if (m_aligner.InfoWafer == null)
                {
                    nLoadPort = GetInfoWafer(nArm).m_nLoadPort;
                    GetInfoWafer(nArm).m_eProcNext = Info_Wafer.eProc.Rotate0;
                    //GetInfoWafer(nArm).m_queProc.Enqueue(Info_Wafer.eProc.Rotate0); // ing 170401
                    GetInfoWafer(nArm).m_queProc.Enqueue(Info_Wafer.eProc.LoadPort); // ing 170401
                    RunPut(eTeaching.Aligner, nArm);
                    return true;
                }
                else
                {
                    if (nArm == eArm.Lower)
                    {
                        m_log.Add("AutoUnload LoadPort RF180 !!");
                        return false;
                    }
                    nLoadPort = GetInfoWafer(nArm).m_nLoadPort;
                    RunPut(eTeaching.LoadPort0, nArm);
                    CheckUnloadFinish(nLoadPort); // ing 170401
                    return false;
                }
            }
            else
            {
                nLoadPort = GetInfoWafer(nArm).m_nLoadPort;
                RunPut(eTeaching.LoadPort0, nArm);
                CheckUnloadFinish(nLoadPort);
                return true;
            }
        }

        void CheckUnloadFinish(int nLoadPort)
        {
            if (m_infoCarrier[nLoadPort].IsWaferOut() || m_auto.ClassHandler().IsWaferExistInEQP() || m_auto.ClassHandler().IsWaferOut())
                return;
            if (RunHome() != eHWResult.OK)
            {
                SetState(eState.Error);
                return;
            }
            m_loadPort[nLoadPort].SetUnload();
        }

        bool RunPut(eArm nArmPut)
        {
            Info_Wafer.eLocate eLocNext = GetNextLocate(GetInfoWafer(nArmPut));
            switch (eLocNext)
            {
                case Info_Wafer.eLocate.LoadPort:
                    RunPut(eTeaching.LoadPort0, nArmPut);
                    return false;
                case Info_Wafer.eLocate.Aligner:
                    if (m_aligner.InfoWafer != null) return true;
                    RunPut(eTeaching.Aligner, nArmPut);
                    return false;
                case Info_Wafer.eLocate.Vision:
                    //if (m_vision.InfoWafer != null) return true; 
                    if (m_vision.IsWaferExist() == eHWResult.Off)
                    {
                        if (m_vision.GetState() == HW_VisionWorks_Mom.eState.LoadWait)
                        {
                            m_vision.SetLoad(m_InfoWafer[(int)nArmPut]);
                        }
                        if (m_vision.GetState() == HW_VisionWorks_Mom.eState.Ready)
                        {
                            RunPut(eTeaching.Vision, nArmPut);
                        }
                    }
                    return false;
                case Info_Wafer.eLocate.BackSide:
                    if (m_backSide.InfoWafer != null) return true;
                    RunPut(eTeaching.BackSide, nArmPut);
                    return false;
            }
            return true;
        }

        void StateRunBoth()
        {
            if (!m_work.IsRun())
            {
                SetState(eState.Ready);
                return;
            }
            if ((GetInfoWafer(eArm.Lower) != null) && (GetInfoWafer(eArm.Upper) != null))  //둘다 있을떄
            {
                if (GetNextLocate(GetInfoWafer(eArm.Lower)) == m_eGetLoc)
                {
                    RunPut(eArm.Lower);
                    return;
                }
                if (GetNextLocate(GetInfoWafer(eArm.Upper)) == m_eGetLoc)
                {
                    RunPut(eArm.Upper);
                    return;
                }
                if (RunPut(eArm.Lower) == false) return;
                if (RunPut(eArm.Upper) == false) return;
                m_log.Popup("Can't Put Wafer !!");
                return;
            }
            if ((GetInfoWafer(eArm.Lower) != null) || (GetInfoWafer(eArm.Upper) != null))  // 둘중 하나라도 있을떄
            {
                eArm nArmPut, nArmGet;
                if (GetInfoWafer(eArm.Lower) != null)
                {
                    nArmPut = eArm.Lower;
                    nArmGet = eArm.Upper;
                }
                else
                {
                    nArmPut = eArm.Upper;
                    nArmGet = eArm.Lower;
                }
                Info_Wafer.eLocate eLocNext = GetNextLocate(GetInfoWafer(nArmPut));
                switch (eLocNext)
                {
                    case Info_Wafer.eLocate.LoadPort:
                        RunPut(eTeaching.LoadPort0, nArmPut);
                        return;
                    case Info_Wafer.eLocate.Aligner:
                        if (m_aligner.InfoWafer != null)
                        {
                            if (RunGet(eTeaching.Aligner, nArmGet)) return;
                        }
                        if (m_eState != eState.Run) return;

                        if (m_aligner.IsReady(GetInfoWafer(nArmPut)))  //KDG Add Aligner State??
                        {
                            RunPut(eTeaching.Aligner, nArmPut);
                            RotateVision();
                        }
                        return;
                    case Info_Wafer.eLocate.Vision:
                        if (m_vision.IsWaferExist() == eHWResult.On)
                        {
                            if ((m_vision.InfoWafer != null) && m_vision.InfoWafer.m_bRunWait == false)
                            {
                                int nPos = m_nTeaching[(int)m_vision.InfoWafer.m_wafer.m_eSize, (int)eTeaching.Vision];
                                RunGetReady(nArmGet, nPos, 0);
                                m_vision.InfoWafer.m_bRunWait = true;
                            }

                            if ((m_vision.InfoWafer != null) && (m_vision.GetState() == HW_VisionWorks_Mom.eState.Done))
                            {
                                if (RunGet(eTeaching.Vision, nArmGet)) return;
                            }
                        }
                        if (m_eState != eState.Run) return;
                        if (m_vision.IsWaferExist() == eHWResult.Off)
                        {
                            if (m_vision.GetState() == HW_VisionWorks_Mom.eState.LoadWait)
                            {
                                m_vision.SetLoad(m_InfoWafer[(int)nArmPut]);
                            }
                            if (m_vision.GetState() == HW_VisionWorks_Mom.eState.Ready)
                            {
                                RunPut(eTeaching.Vision, nArmPut);
                            }
                        }
                        return;
                    case Info_Wafer.eLocate.BackSide:
                        if (m_backSide.InfoWafer != null)
                        {
                            if (RunGet(eTeaching.BackSide, nArmGet)) return;
                        }
                        if (m_eState != eState.Run) return;
                        RunPut(eTeaching.BackSide, nArmPut);
                        return;
                }
            }
            else    //둘다 없을떄
            {
                Info_Wafer infoWafer = GetLoadPortWafer();
                if (!m_bCycleStop && (infoWafer != null))
                {
                    RunGet(eTeaching.LoadPort0, eArm.Upper, infoWafer);
                    return;
                }
                if ((m_aligner.InfoWafer != null) && (m_aligner.GetState() == HW_Aligner_Mom.eState.Done))
                {
                    RunGet(eTeaching.Aligner, eArm.Lower);
                    return;
                }
                if ((m_vision.InfoWafer != null) && m_vision.InfoWafer.m_bRunWait == false)
                {
                    int nPos = m_nTeaching[(int)m_vision.InfoWafer.m_wafer.m_eSize, (int)eTeaching.Vision];
                    RunGetReady(eArm.Upper, nPos, 0);
                    m_vision.InfoWafer.m_bRunWait = true;
                }
                if ((m_vision.InfoWafer != null) && (m_vision.GetState() == HW_VisionWorks_Mom.eState.Done) && (m_vision.IsWaferExist() == eHWResult.On))
                {
                    RunGet(eTeaching.Vision, eArm.Upper);
                    return;
                }
                if ((m_backSide.InfoWafer != null) && (m_backSide.GetState() == HW_BackSide_Mom.eState.Done))
                {
                    RunGet(eTeaching.BackSide, eArm.Upper);
                    return;
                }
            }
        }

        void StateRunSingle(eArm nArm)
        {
            if (!m_work.IsRun())
            {
                SetState(eState.Ready);
                return;
            }
            if (GetInfoWafer(nArm) != null)
            {
                eArm nArmGet = GetArmOn(nArm);
                switch (GetNextLocate(GetInfoWafer(nArmGet)))
                {
                    case Info_Wafer.eLocate.Aligner:
                        if (m_aligner.IsReady(GetInfoWafer(nArmGet)))
                        {
                            RunPut(eTeaching.Aligner, nArmGet);
                            return;
                        }
                        break;
                    case Info_Wafer.eLocate.Vision:
                        if (m_vision.IsWaferExist() == eHWResult.Off)
                        {
                            if (m_vision.GetState() == HW_VisionWorks_Mom.eState.LoadWait)
                            {
                                m_vision.SetLoad(m_InfoWafer[(int)nArmGet]);
                            }
                            if (m_vision.GetState() == HW_VisionWorks_Mom.eState.Ready)
                            {
                                RunPut(eTeaching.Vision, nArmGet);
                            }
                            return;
                        }
                        break;
                    case Info_Wafer.eLocate.BackSide:
                        if (m_backSide.IsReady(GetInfoWafer(nArmGet)))
                        {
                            RunPut(eTeaching.BackSide, nArmGet);
                            return;
                        }
                        break;
                    case Info_Wafer.eLocate.LoadPort:
                        RunPut(eTeaching.LoadPort0, nArmGet);
                        return;
                }
            }
            else
            {
                if ((m_backSide.InfoWafer != null) && (m_backSide.GetState() == HW_BackSide_Mom.eState.Done) && IsNextLocateReady(m_backSide.InfoWafer))
                {
                    RunGet(eTeaching.BackSide, GetArmGet(nArm, m_backSide.InfoWafer));
                    return;
                }
                if (m_vision.IsWaferExist() == eHWResult.On)
                {
                    if ((m_vision.InfoWafer != null) && (m_vision.GetState() == HW_VisionWorks_Mom.eState.Done) && IsNextLocateReady(m_vision.InfoWafer))
                    {
                        RunGet(eTeaching.Vision, GetArmGet(nArm, m_vision.InfoWafer));
                        return;
                    }
                }
                if ((m_aligner.InfoWafer != null) && (m_aligner.GetState() == HW_Aligner_Mom.eState.Done) && IsNextLocateReady(m_aligner.InfoWafer))
                {
                    RunGet(eTeaching.Aligner, GetArmGet(nArm, m_aligner.InfoWafer));
                    return;
                }
                if (m_aligner.InfoWafer == null)
                {
                    if ((m_backSide.InfoWafer != null) && (m_backSide.InfoWafer.m_eAngleRF != Info_Wafer.eAngleRF.R0)) return;
                    if ((m_vision.InfoWafer != null) && (m_vision.InfoWafer.m_eAngleRF != Info_Wafer.eAngleRF.R0)) return;
                    Info_Wafer infoWafer = GetLoadPortWafer();
                    if (!m_bCycleStop && (infoWafer != null))
                    {
                        RunGet(eTeaching.LoadPort0, GetArmGet(nArm, infoWafer), infoWafer);
                        return;
                    }
                }
            }
        }

        eArm GetArmOn(eArm nArm)
        {
            if (nArm != eArm.Single && nArm != eArm.Both) return nArm;
            if (m_InfoWafer[(int)eArm.Lower] != null) return eArm.Lower;
            if (m_InfoWafer[(int)eArm.Upper] != null) return eArm.Upper;
            return nArm;
        }

        eArm GetArmGet(eArm nArm, Info_Wafer infoWafer)
        {
            if (nArm != eArm.Single && nArm != eArm.Both) return nArm;
            return eArm.Lower;
            //forget
            //            infoWafer.m_wafer.m_eSize
        }

        Info_Wafer.eLocate GetNextLocate(Info_Wafer infoWafer)
        {
            switch (infoWafer.m_eProcNext)
            {
                case Info_Wafer.eProc.BCR: // ing 170626
                case Info_Wafer.eProc.Align:
                case Info_Wafer.eProc.Rotate0:
                case Info_Wafer.eProc.Rotate90:
                case Info_Wafer.eProc.Rotate180:
                case Info_Wafer.eProc.Rotate270: 
                if (infoWafer.m_bPreAlignfail) return Info_Wafer.eLocate.LoadPort;  
                return Info_Wafer.eLocate.Aligner;
                case Info_Wafer.eProc.Vision: return Info_Wafer.eLocate.Vision;
                case Info_Wafer.eProc.BackSide: return Info_Wafer.eLocate.BackSide;
                case Info_Wafer.eProc.LoadPort:
                    if (!infoWafer.m_wafer.IsRingFrame() || infoWafer.m_eAngleRF == Info_Wafer.eAngleRF.R0) return Info_Wafer.eLocate.LoadPort;
                    infoWafer.m_queProc.Clear();
                    infoWafer.m_eProcNext = Info_Wafer.eProc.Rotate0;
                    infoWafer.m_queProc.Enqueue(Info_Wafer.eProc.LoadPort);
                    return Info_Wafer.eLocate.Aligner;
            }
            m_log.Popup("GetNextLocate Error");
            return Info_Wafer.eLocate.LoadPort;
        }

        bool IsNextLocateReady(Info_Wafer infoWafer)
        {
            Info_Wafer.eLocate eLoc = GetNextLocate(infoWafer);
            switch (eLoc)
            {
                case Info_Wafer.eLocate.LoadPort:
                    return true;
                case Info_Wafer.eLocate.Aligner:
                    return (m_aligner.InfoWafer == null);
                case Info_Wafer.eLocate.Vision:
                    return (m_vision.InfoWafer == null);
                case Info_Wafer.eLocate.BackSide:
                    return (m_backSide.InfoWafer == null);
            }
            m_log.Popup("IsNextLocateReady Error");
            return false;
        }

        void RotateVision()
        {
            if (m_bRotateVision) return;
            //forget Rotate to Vision
            m_bRotateVision = true;
        }

        bool RunGet(eTeaching nTeaching, eArm nArm, Info_Wafer infoWafer = null)
        {
            bool bResult = false;
            switch (nTeaching)
            {
                case eTeaching.LoadPort0:
                case eTeaching.LoadPort1:
                case eTeaching.LoadPort2:
                    string strEvent = "Get_Loadport" + infoWafer.m_nLoadPort.ToString();
                    m_swWTRMove.Start();
                    RunGetLoadPort(nArm, infoWafer);
                    SetProcQue(infoWafer);
                    if (m_swWTRMove.Check() > 100)
                        m_log.Add("MOVE TIME : LOADPORT -> ARM (" + m_swWTRMove.Check().ToString() + " ms)");
                    m_swWTRMove.Stop();
                    break;
                case eTeaching.Aligner:
                    m_swWTRMove.Start();
                    bResult = RunGetAligner(nArm);
                    if (m_swWTRMove.Check() > 100)
                        m_log.Add("MOVE TIME : Aligner -> ARM (" + m_swWTRMove.Check().ToString() + " ms)");
                    m_swWTRMove.Stop();
                    break;
                case eTeaching.Vision:
                    m_swWTRMove.Start();
                    bResult = RunGetVision(nArm);
                    if (m_swWTRMove.Check() > 100)
                        m_log.Add("MOVE TIME : STAGE -> ARM (" + m_swWTRMove.Check().ToString() + " ms)");
                    m_swWTRMove.Stop();
                    break;
            }
            return bResult;
        }
        /**
* @brief Loadport에서 Wafer를 Get하는 함수
* @param eArm nArm : Arm 위치 설정
* @param Info_Wafer : Wafer 정보
* @return 없음
* @note Patch-notes
* 날짜|작성자|설명|비고
* -|-|-|-
* 2023-07-24|조남수|Loadport에서 Get하는 로그 추가와 Slot이 시작하는 로그 추가|-
* @warning 없음
*/
        void RunGetLoadPort(eArm nArm, Info_Wafer infoWafer)
        {
            int nPos = m_nTeaching[(int)infoWafer.m_wafer.m_eSize, (int)eTeaching.LoadPort0 + infoWafer.m_nLoadPort];
            string sLog = String.Format("[WTRStart] Get LoadPort : {0}, Arm : {1}, Slot : {2}", infoWafer.m_nLoadPort + 1, nArm.ToString(), (infoWafer.m_nID + 1).ToString("00"));  //230724 nscho 
            m_log.WriteLog("Sequence", sLog);//230724 nscho
            if (infoWafer.m_bSendLotstartsigaal == true)
            {
                m_auto.ClassHandler().ClassVisionWorks().SendLotStart(infoWafer.m_sLotStartID);
                m_auto.ClassHandler().ClassAligner().LotStart();
            }
            if (m_loadPort[infoWafer.m_nLoadPort].m_bCheckParticle && !m_loadPort[infoWafer.m_nLoadPort].IsDoorOpenPos())
            {
                if (m_loadPort[infoWafer.m_nLoadPort].DoorOpen() == eHWResult.Error)
                {
                    SetAlarm(eAlarm.Warning, eError_Mom.LPOpen);
                    m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho 
                }
            } 
            if (!m_loadPort[infoWafer.m_nLoadPort].IsDoorOpenPos())        //Interlock LoadPort Door 오픈 Check
            {
                m_log.Popup("Load port Does Not Open. Check Loadport Open Sensor");
                SetAlarm(eAlarm.Warning, eError_Mom.LPOpen); 
                m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho 
                return;
            }
            if (m_loadPort[infoWafer.m_nLoadPort].CheckCarrier() != infoWafer.m_wafer.m_eSize)
            {
                m_log.Popup("Cassette Error , Please Check Wafer Size & Casstte ");
                SetAlarm(eAlarm.Warning, eError_Mom.CarrierCheck); 
                m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho 
                return;
            }
            try
            {
                infoWafer.m_idMaterial = m_infoCarrier[infoWafer.m_nLoadPort].m_strLotID + ":" + infoWafer.m_nSlot.ToString("00");
                if (m_logMars != null) m_logMars.AddTransferLog("WTR", "GET", MarsLog.eStatus.Start, infoWafer.m_idMaterial, MarsLog.eMateral.Wafer, m_infoCarrier[infoWafer.m_nLoadPort].m_strLotID, "LP" + (infoWafer.m_nLoadPort + 1).ToString(), infoWafer.m_nSlot, "WTR", (int)nArm + 1, null); 
                m_log.WriteLog("Sequence", "[Slot Start]" + 
                    " CarrierID : " + infoWafer.m_strCarrierID +
                    ", WaferID : " + infoWafer.WAFERID +
                    ", Slot : " + infoWafer.m_nSlot.ToString() +
                    ", Recipe : " + infoWafer.m_sRecipe
                    ); //230724 nscho 
                if (m_loadPort[infoWafer.m_nLoadPort].m_bUseSlimSlot && infoWafer.m_nID == 0)
                {
                    if (RunGetLPSLimSlot(nArm, m_loadPort[infoWafer.m_nLoadPort].m_nSlimSlot, 0) != eHWResult.OK)
                    {
                        SetState(eState.Error);
                        m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho 
                        return;
                    }
                }
                else
                {
                    if (RunGetMotion(nArm, nPos, infoWafer.m_nID) != eHWResult.OK)
                    {
                        SetState(eState.Error);
                        m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho 
                        return;
                    }
                }
            }
            finally
            {
                if (m_logMars != null) m_logMars.AddTransferLog("WTR", "GET", MarsLog.eStatus.End, infoWafer.m_idMaterial, MarsLog.eMateral.Wafer, m_infoCarrier[infoWafer.m_nLoadPort].m_strLotID, "LP" + (infoWafer.m_nLoadPort + 1).ToString(), infoWafer.m_nSlot, "WTR", (int)nArm + 1, null); 
            }
            if (!IsArmClose(nArm, true))       //KDG 160927 Add // ing 161018
            {
                m_log.Popup("Arm Close Error");
                m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho 
                SetAlarm(eAlarm.Warning, eError_Mom.ArmClose); 
                return;
            }
            if (m_loadPort[infoWafer.m_nLoadPort].m_bCheckParticle && m_loadPort[infoWafer.m_nLoadPort].IsDoorOpenPos())
            {
                if (m_loadPort[infoWafer.m_nLoadPort].DoorClose() == eHWResult.Error)
                {
                    SetAlarm(eAlarm.Warning, eError_Mom.LPOpen);
                    m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho 
                }
            } //211210 nscho Door Close
            //forget
            m_log.WriteLog("Sequence", "[WTRDone] Get LoadPort" + (infoWafer.m_nLoadPort + 1) + " PASS");  //230724 nscho 
            m_bRotateVision = false;
            infoWafer.State = Info_Wafer.eState.Run;
            SetInfoWafer(nArm, infoWafer);
            m_eGetLoc = Info_Wafer.eLocate.LoadPort;
        }
        /**
* @brief Aligner에서 Wafer를 Get하는 함수
* @param eArm nArm : Arm 위치 설정
* @return 없음
* @note Patch-notes
* 날짜|작성자|설명|비고
* -|-|-|-
* 2023-07-24|조남수|Aligner에서 Get하는 로그 추가|-
* @warning 없음
*/
        bool RunGetAligner(eArm nArm)
        {
            int nPos = m_nTeaching[(int)m_aligner.InfoWafer.m_wafer.m_eSize, (int)eTeaching.Aligner];

            if (m_aligner.GetState() != HW_Aligner_Mom.eState.Done && m_aligner.GetState() != HW_Aligner_Mom.eState.Ready) return true;
            //if (m_aligner.GetState() != HW_Aligner_Mom.eState.Done && m_aligner.GetState() != HW_Aligner_Mom.eState.Ready && m_aligner.LiterSafety()) return true;        //KDG 170110 Add Aligner Lifter
            m_aligner.RunVac(false);
            try
            {
                string sLog = String.Format("[WTRStart] Get Aligner, Arm : {0}, Slot : {1}", nArm.ToString(), (m_aligner.InfoWafer.m_nSlot).ToString("00")); //230724 nscho 
                m_log.WriteLog("Sequence", sLog);//230724 nscho
                if (m_logMars != null) m_logMars.AddTransferLog("WTR", "GET", MarsLog.eStatus.Start, m_aligner.InfoWafer.m_idMaterial, MarsLog.eMateral.Wafer, m_infoCarrier[m_aligner.InfoWafer.m_nLoadPort].m_strLotID, "Aligner", 1, "WTR", (int)nArm + 1, null);  

                if (RunGetMotion(nArm, nPos, 0) != eHWResult.OK)
                {
                    m_log.Popup("Get Aligner Error");
                    m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho
                    SetAlarm(eAlarm.Warning, eError_Mom.GetAligner); 
                    return true;
                }
            }
            finally
            {
                if (m_logMars != null) m_logMars.AddTransferLog("WTR", "GET", MarsLog.eStatus.End, m_aligner.InfoWafer.m_idMaterial, MarsLog.eMateral.Wafer, m_infoCarrier[m_aligner.InfoWafer.m_nLoadPort].m_strLotID, "Aligner", 1, "WTR", (int)nArm + 1, null); 
            }
            if (!IsArmClose(nArm, true))       //KDG 160927 Add 
            {
                m_log.Popup("Arm Close Error");
                m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho
                SetAlarm(eAlarm.Warning, eError_Mom.ArmClose); 
                return true;
            }
            m_bRotateVision = false;
            SetInfoWafer(nArm, m_aligner.InfoWafer);
            m_log.WriteLog("Sequence", "[WTRDone] Get Aligner PASS"); //230724 nscho 
            m_aligner.SetReady();
            m_eGetLoc = Info_Wafer.eLocate.Aligner;
            return false;
        }
        /**
* @brief Vision에서 Wafer를 Get하는 함수
* @param eArm nArm : Arm 위치 설정
* @return 없음
* @note Patch-notes
* 날짜|작성자|설명|비고
* -|-|-|-
* 2023-07-24|조남수|Vision에서 Get하는 로그 추가|-
* @warning 없음
*/
        bool RunGetVision(eArm nArm)
        {
            string sLog = String.Format("[WTRStart] Get Vision, Arm : {0}, Slot : {1}", nArm.ToString(), (m_vision.InfoWafer.m_nID + 1).ToString("00")); // 230724 nscho
            m_log.WriteLog("Sequence", sLog);//230724 nscho 
            if (m_vision.InfoWafer == null)
            {
                m_log.Popup("Wafer Information on Vision Was Losted !!");
                SetState(eState.Error); 
                return true;
            }
            int nPos = m_nTeaching[(int)m_vision.InfoWafer.m_wafer.m_eSize, (int)eTeaching.Vision];

            if (m_vision.GetState() != HW_VisionWorks_Mom.eState.Done && m_vision.GetState() != HW_VisionWorks_Mom.eState.LoadWait) return true;

            if (m_vision.IsWFStgReady() != eHWResult.OK || m_vision.IsLifterUp() != true || m_vision.IsVSReadySignal() != true)
            {
                m_log.Popup("Wafer Stage And Lifter is Not Ready Pos or Vision Ready Signal Does Not Turn On");
                m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho
                SetAlarm(eAlarm.Warning, eError_Mom.VisionReady); 
                return true;
            }
            if (m_vision.IsWaferExistLoad() != true && !m_vision.m_bManualWaferExist)
            {
                m_log.Popup("Wafer Does Not Exist On The Stage");
                m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho
                SetAlarm(eAlarm.Warning, eError_Mom.VisionExist); 
                return true;
            }

            try
            {
                if (m_logMars != null) m_logMars.AddTransferLog("WTR", "GET", MarsLog.eStatus.Start, m_vision.InfoWafer.m_idMaterial, MarsLog.eMateral.Wafer, m_infoCarrier[m_vision.InfoWafer.m_nLoadPort].m_strLotID, "Vision", 1, "WTR", (int)nArm + 1, null); 
                if (RunGetMotion(nArm, nPos, 0) != eHWResult.OK)
                {
                    m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho
                    SetState(eState.Error);
                    return true;
                }
            }
            finally
            {
                if (m_logMars != null) m_logMars.AddTransferLog("WTR", "GET", MarsLog.eStatus.End, m_vision.InfoWafer.m_idMaterial, MarsLog.eMateral.Wafer, m_infoCarrier[m_vision.InfoWafer.m_nLoadPort].m_strLotID, "Vision", 1, "WTR", (int)nArm + 1, null); 
            }
                if (!IsArmClose(nArm, true))       //KDG 160927 Add // ing 161018
                {
                    m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho
                    m_log.Popup("Arm Close Error");
                    SetAlarm(eAlarm.Warning, eError_Mom.ArmClose); 
                    return true;
                }
            //if (m_work.m_bXgemUSe) {
            //    m_auto.ClassXgem300().SetInspDone(m_InfoWafer[(int)nArm].m_nSlot);
            //}
            m_log.WriteLog("Sequence", "[WTRDone] Get Vision PASS"); //230724 nscho 
            m_bRotateVision = true;
            SetInfoWafer(nArm, m_vision.InfoWafer);
            m_vision.SetUnloadDone();
            m_eGetLoc = Info_Wafer.eLocate.Vision;
            return false;
        }

        bool RunGetBackSide(eArm nArm)
        {
            int nPos = m_nTeaching[(int)m_backSide.InfoWafer.m_wafer.m_eSize, (int)eTeaching.BackSide];
            if (m_backSide.GetState() != HW_BackSide_Mom.eState.Done && m_backSide.GetState() != HW_BackSide_Mom.eState.Ready) return true;
            if (!m_backSide.IsAxisOK()) { m_log.Popup("Backside Position Is Wrong !!"); return true; } //ing 161022
            m_backSide.RunVac(false);
            if (m_logMars != null) m_logMars.AddTransferLog("WTR", "GET", MarsLog.eStatus.Start, m_backSide.InfoWafer.m_idMaterial, MarsLog.eMateral.Wafer, m_infoCarrier[m_backSide.InfoWafer.m_nLoadPort].m_strLotID, "BacksideStage", 1, "WTR", (int)nArm + 1, null); 
            if (RunGetMotion(nArm, nPos, 0) != eHWResult.OK)
            {
                m_log.Popup("Get BackSide Error");
                SetAlarm(eAlarm.Warning, eError_Mom.GetBackside); 
                return true;
            }
            if (m_logMars != null) m_logMars.AddTransferLog("WTR", "GET", MarsLog.eStatus.End, m_backSide.InfoWafer.m_idMaterial, MarsLog.eMateral.Wafer, m_infoCarrier[m_backSide.InfoWafer.m_nLoadPort].m_strLotID, "BacksideStage", 1, "WTR", (int)nArm + 1, null); 
            if (!IsArmClose(nArm, true))       //KDG 160927 Add // ing 161018
            {
                m_log.Popup("Arm Close Error");
                SetAlarm(eAlarm.Warning, eError_Mom.ArmClose); 
                return true;
            }
            SetInfoWafer(nArm, m_backSide.InfoWafer);
            m_backSide.SetReady();
            m_eGetLoc = Info_Wafer.eLocate.BackSide;
            return false;
        }
        ezStopWatch m_swWTRMove = new ezStopWatch();

        void RunPut(eTeaching nTeaching, eArm nArm)
        {
            switch (nTeaching)
            {
                case eTeaching.LoadPort0:
                case eTeaching.LoadPort1:
                case eTeaching.LoadPort2:
                    string strEvent = "Put_Loadport" + m_InfoWafer[(int)nArm].m_nLoadPort.ToString();
                    m_swWTRMove.Start();
                    RunPutLoadPort(nArm);
                    if (m_swWTRMove.Check() > 100)
                        m_log.Add("MOVE TIME : ARM -> LOADPORT (" + m_swWTRMove.Check().ToString() + " ms)");
                    m_swWTRMove.Stop();
                    break;
                case eTeaching.Aligner:
                    m_swWTRMove.Start();
                    RunPutAligner(nArm);
                    if (m_swWTRMove.Check() > 100)
                        m_log.Add("MOVE TIME : ARM -> ALIGNER (" + m_swWTRMove.Check().ToString() + " ms)");
                    m_swWTRMove.Stop();
                    break;
                case eTeaching.Vision:
                    m_swWTRMove.Start();
                    RunPutVision(nArm, m_InfoWafer[(int)nArm]); //211012 nscho Wafer정보가 다른 경우 WTR의 정보로 LoadDone 시킴.
                    if (m_swWTRMove.Check() > 100)
                        m_log.Add("MOVE TIME : ARM -> STAGE (" + m_swWTRMove.Check().ToString() + " ms)");
                    m_swWTRMove.Stop();
                    break;
            }
        }
        bool Check_SlotIsRun(eArm nArm)
        {
            Info_Carrier infoCarrier = m_loadPort[m_InfoWafer[(int)nArm].m_nLoadPort].m_infoCarrier;
            int nSlot = m_InfoWafer[(int)nArm].m_nSlot;
            bool bRun = false;
            if (infoCarrier.m_infoWafer[nSlot - 1].State == Info_Wafer.eState.Run || infoCarrier.m_infoWafer[nSlot - 1].State == Info_Wafer.eState.Empty)
            {
                bRun = true;
            }
            else bRun = false;

            return bRun;
        }
        /**
* @brief Loadport에 Wafer를 Put하는 함수
* @param eArm nArm : Arm 위치 설정
* @return 없음
* @note Patch-notes
* 날짜|작성자|설명|비고
* -|-|-|-
* 2023-07-24|조남수|Loadport에 Put하는 로그 추가와 Slot검사가 끝난 로그 추가|-
* @warning 없음
*/
        void RunPutLoadPort(eArm nArm)
        {
            string sLog = String.Format("[WTRStart] Put LoadPort : {0}, Arm : {1}, CarrierID : {2}, Slot : {3} ", m_InfoWafer[(int)nArm].m_nLoadPort + 1, nArm.ToString(), m_InfoWafer[(int)nArm].m_strCarrierID.ToString(), (GetInfoWafer(nArm).m_nID + 1).ToString("00")); //230724 nscho
            m_log.WriteLog("Sequence", sLog); //230724 nscho
            int nPos = m_nTeaching[(int)m_InfoWafer[(int)nArm].m_wafer.m_eSize, (int)eTeaching.LoadPort0 + m_InfoWafer[(int)nArm].m_nLoadPort];
            if (m_loadPort[m_InfoWafer[(int)nArm].m_nLoadPort].m_bCheckParticle && !m_loadPort[m_InfoWafer[(int)nArm].m_nLoadPort].IsDoorOpenPos())
            {
                if (m_loadPort[m_InfoWafer[(int)nArm].m_nLoadPort].DoorOpen() == eHWResult.Error)
                {
                    m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho
                    SetAlarm(eAlarm.Warning, eError_Mom.LPOpen);
                }
            } //211210 nscho Door Open
            if (!m_loadPort[m_InfoWafer[(int)nArm].m_nLoadPort].IsDoorOpenPos())        //Interlock LoadPort Door 오픈 Check
            {
                m_log.Add("Load port Does Not Open. Check Loadport Open Sensor");
                m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho
                SetAlarm(eAlarm.Warning, eError_Mom.LPOpen); 
                return;
            }

            if (m_loadPort[m_InfoWafer[(int)nArm].m_nLoadPort].CheckCarrier() != m_InfoWafer[(int)nArm].m_wafer.m_eSize)
            {
                m_log.Popup("Cassette Error , Please Check Wafer Size & Casstte ");
                m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho
                SetAlarm(eAlarm.Warning, eError_Mom.CarrierCheck); 
                return;
            }
            if (!Check_SlotIsRun(nArm))
            {
                SetAlarm(eAlarm.Error, eError_Mom.PutLoadPort);
                m_log.Add("Slot state is not Run.");
                m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho
                m_log.Add("Slot Num : " + m_InfoWafer[(int)nArm].m_nSlot.ToString("00") + " Slot State : " + m_loadPort[m_InfoWafer[(int)nArm].m_nLoadPort].m_infoCarrier.m_infoWafer[m_InfoWafer[(int)nArm].m_nSlot - 1].State.ToString()); 
                return;
            }   
            try
            {
                if (m_logMars != null) m_logMars.AddTransferLog("WTR", "PUT", MarsLog.eStatus.Start, GetInfoWafer(nArm).m_idMaterial, MarsLog.eMateral.Wafer, m_infoCarrier[GetInfoWafer(nArm).m_nLoadPort].m_strLotID, "WTR", (int)nArm + 1, "LP" + (GetInfoWafer(nArm).m_nLoadPort + 1).ToString(), GetInfoWafer(nArm).m_nSlot, null); 
                if (m_loadPort[m_InfoWafer[(int)nArm].m_nLoadPort].m_bUseSlimSlot && GetInfoWafer(nArm).m_nID == 0)
                {
                    if (RunPutLPSLimSlot(nArm, m_loadPort[m_InfoWafer[(int)nArm].m_nLoadPort].m_nSlimSlot, 0) != eHWResult.OK)
                    {
                        m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho
                        SetState(eState.Error);
                        return;
                    }
                }
                else
                {
                    if (RunPutMotion(nArm, nPos, GetInfoWafer(nArm).m_nID) != eHWResult.OK)
                    {
                        m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho
                        SetState(eState.Error);
                        return;
                    }
                }
            }
            finally
            {
                if (m_logMars != null) m_logMars.AddTransferLog("WTR", "PUT", MarsLog.eStatus.End, GetInfoWafer(nArm).m_idMaterial, MarsLog.eMateral.Wafer, m_infoCarrier[GetInfoWafer(nArm).m_nLoadPort].m_strLotID, "WTR", (int)nArm + 1, "LP" + (GetInfoWafer(nArm).m_nLoadPort + 1).ToString(), GetInfoWafer(nArm).m_nSlot, null); 
            }
            if (!IsArmClose(nArm, true))       //KDG 160927 Add // ing 161018
            {
                m_log.Popup("Arm Close Error");
                m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho
                SetAlarm(eAlarm.Warning, eError_Mom.ArmClose); 
                return;
            }

            GetInfoWafer(nArm).Locate = Info_Wafer.eLocate.LoadPort;
            //if (m_work.IsRun())
            //{
            //    GetInfoWafer(nArm).State = Info_Wafer.eState.Done;
            //}
            //else 
            if (m_work.GetState() == eWorkRun.AutoUnload)
            {
                GetInfoWafer(nArm).State = Info_Wafer.eState.Exist;
            }
            else
                GetInfoWafer(nArm).State = Info_Wafer.eState.Done;

            while (m_work.GetState() != eWorkRun.Run && m_work.GetState() != eWorkRun.AutoUnload)
            {   
                Thread.Sleep(50);
                if (m_work.GetState() == eWorkRun.Error)
                {
                    m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho
                    return;
                }
            }

            //if (m_infoCarrier[GetInfoWafer(nArm).m_nLoadPort].CheckDone(m_bCycleStop) && !IsWaferSensorCheck()) //forget0927//sdh
            if (m_work.GetState() == eWorkRun.Run
                && m_infoCarrier[GetInfoWafer(nArm).m_nLoadPort].CheckDone(m_bCycleStop) && !m_auto.ClassHandler().IsWaferExistInEQP())  //KDG 161007 Modify 구현된 함수가 있어서 변경진행
            {
                if (GetInfoWafer(nArm).m_bUseVision)
                    m_vision.SendLotEnd(m_infoCarrier[GetInfoWafer(nArm).m_nLoadPort]);
                RunGoHome();   // BHJ 190304 add 
                m_loadPort[GetInfoWafer(nArm).m_nLoadPort].SetUnload();
                if ((m_auto.m_strWork == Auto_Mom.eWork.Sanken.ToString()) && (m_bCycleStop == true)) //kns20180209
                {
                    if (GetInfoWafer(nArm).m_nLoadPort == 0) CheckUnloadFinish(1);
                    if ((m_loadPort[0].GetState() == HW_LoadPort_Mom.eState.Ready) && (m_loadPort[1].GetState() == HW_LoadPort_Mom.eState.Ready))
                        m_bCycleStop = false;
                }
                else
                {
                    m_bCycleStop = false;
                }
            }

            if (m_auto.m_bXgemUse && (m_xGem.IsOnlineRemote() || m_xGem.IsOnlineLocal()) && m_work.GetState() == eWorkRun.Run)
                m_xGem.WaferEnd(m_InfoWafer[(int)nArm]);
            m_log.WriteLog("Sequence", "[Slot End]"
                   + " CarrierID : " + m_InfoWafer[(int)nArm].m_strCarrierID
                   + ", WaferID : " + m_InfoWafer[(int)nArm].m_strWaferID
                   + ", Slot : " + (GetInfoWafer(nArm).m_nID + 1).ToString("00")
                   ); // 230724 nscho 
            m_log.WriteLog("Sequence", "[WTRDone] Put LoadPort" + (m_InfoWafer[(int)nArm].m_nLoadPort + 1) + " PASS"); 
            m_bRotateVision = false;
            SetInfoWafer(nArm, null);
        }

        /**
* @brief Aligner에 Wafer를 Put하는 함수
* @param eArm nArm : Arm 위치 설정
* @return 없음
* @note Patch-notes
* 날짜|작성자|설명|비고
* -|-|-|-
* 2023-07-24|조남수|Aligner에 Put하는 로그 추가|-
* @warning 없음
*/
        void RunPutAligner(eArm nArm)
        {
            string sLog = String.Format("[WTRStart] Put Aligner, Arm : {0}, CarrierID : {1}, Slot : {2}", nArm.ToString(), m_InfoWafer[(int)nArm].m_strCarrierID, (m_InfoWafer[(int)nArm].m_nSlot).ToString("00")); // 230724 nscho
            m_log.WriteLog("Sequence", sLog);//230724 nscho
            int nPos = m_nTeaching[(int)m_InfoWafer[(int)nArm].m_wafer.m_eSize, (int)eTeaching.Aligner];

            if (m_aligner.GetState() != HW_Aligner_Mom.eState.Ready) return;
            //if (m_aligner.GetState() != HW_Aligner_Mom.eState.Ready && m_aligner.LiterSafety()) return;       //KDG 170110 Add Aligner Lifter
            if (m_aligner.CheckWaferExist() != eHWResult.Off)
            {
                m_log.Popup("Put Aligner Error : Aligner Wafer Exist Checked");
                m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho
                SetAlarm(eAlarm.Warning, eError_Mom.AlingerExist); 
                return;
            }

            // SET INFOWAFER
            m_aligner.InfoWafer = GetInfoWafer(nArm); 
            try
            {
                if (m_logMars != null) m_logMars.AddTransferLog("WTR", "PUT", MarsLog.eStatus.Start, GetInfoWafer(nArm).m_idMaterial, MarsLog.eMateral.Wafer, m_infoCarrier[GetInfoWafer(nArm).m_nLoadPort].m_strLotID, "WTR", (int)nArm + 1, "Aligner", 1, null); 
                if (RunPutMotion(nArm, nPos, 0) != eHWResult.OK)
                {
                    m_log.Popup("Put Aligner Error");
                    m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho
                    SetAlarm(eAlarm.Warning, eError_Mom.PutAligner); 
                    return;
                }
                if (!IsArmClose(nArm, true))       //KDG 160927 Add // ing 161018
                {
                    m_log.Popup("Arm Close Error");
                    m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho
                    SetAlarm(eAlarm.Warning, eError_Mom.ArmClose); 
                    return;
                }
                if (m_aligner.CheckWaferExist() == eHWResult.Off)
                {
                    m_log.Popup("Aligner Check Wafer Error");
                    m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho
                    SetAlarm(eAlarm.Warning, eError_Mom.AlingerExist); 
                    return;
                }
            }
            finally
            {
                if (m_logMars != null) m_logMars.AddTransferLog("WTR", "PUT", MarsLog.eStatus.End, GetInfoWafer(nArm).m_idMaterial, MarsLog.eMateral.Wafer, m_infoCarrier[GetInfoWafer(nArm).m_nLoadPort].m_strLotID, "WTR", (int)nArm + 1, "Aligner", 1, null); 
            }
            m_bRotateVision = false;
            m_aligner.SetRun(GetInfoWafer(nArm));
            SetInfoWafer(nArm, null);
            m_log.WriteLog("Sequence", "[WTRDone] Put Aligner PASS");  //230724 nscho 
            if (m_aligner.m_bEnableAlign && m_aligner.m_bWTRShiftUse) { HoldWTRShift(); }
        }

        bool m_bHoldWTRShift = false; // forget190219
        ezStopWatch m_swWTRShift = new ezStopWatch();
        void HoldWTRShift() // BHJ 190304 add
        {
            m_swWTRShift.Start();
            m_bHoldWTRShift = true;
            while (m_bHoldWTRShift && (m_swWTRShift.Check() < m_msWaitWTRShift)) ;
            if (m_swWTRShift.Check() >= m_msWaitWTRShift) m_log.Popup("Wait WTR Shift Timeout");
        }

        public void RunWTRShift(double mmShift) // BHJ 190304 add
        {
            //if (!m_bHoldWTRShift) return;
            int nPos = m_nTeaching[(int)Wafer_Size.eSize.mm300, (int)eTeaching.Aligner]; // forget190219
            try
            {
                if (m_logMars != null) m_logMars.AddTransferLog("WTR", "GET", MarsLog.eStatus.Start, m_aligner.InfoWafer.m_idMaterial, MarsLog.eMateral.Wafer, m_infoCarrier[m_aligner.InfoWafer.m_nLoadPort].m_strLotID, "Aligner", 1, "WTR", (int)eArm.Upper + 1, null); 
                if (RunGetMotion(eArm.Upper, nPos, 0, mmShift) != eHWResult.OK)
                {
                    m_log.Popup("WTR Shift : Get Aligner Error");
                    SetAlarm(eAlarm.Warning, eError_Mom.GetAligner); 
                    m_bHoldWTRShift = true;
                    return;
                }
            }
            finally
            {
                if (m_logMars != null) m_logMars.AddTransferLog("WTR", "GET", MarsLog.eStatus.End, m_aligner.InfoWafer.m_idMaterial, MarsLog.eMateral.Wafer, m_infoCarrier[m_aligner.InfoWafer.m_nLoadPort].m_strLotID, "Aligner", 1, "WTR", (int)eArm.Upper + 1, null); 
            }
            //if (m_aligner.RunVac(true)) return; // BHJ 190312 comment
            MarsLog.Datas datas = new MarsLog.Datas();
            datas.Add("Shift", "mm", Math.Round(mmShift, 2));
            try
            {
                if (m_logMars != null) m_logMars.AddTransferLog("WTR", "PUT", MarsLog.eStatus.Start, m_aligner.InfoWafer.m_idMaterial, MarsLog.eMateral.Wafer, m_infoCarrier[m_aligner.InfoWafer.m_nLoadPort].m_strLotID, "WTR", (int)eArm.Upper + 1, "Aligner", 1, datas); 
                if (RunPutMotion(eArm.Upper, nPos, 0) != eHWResult.OK)
                {
                    m_log.Popup("WTR Shift : Put Aligner Error");
                    SetAlarm(eAlarm.Warning, eError_Mom.PutAligner); 
                    m_bHoldWTRShift = true;
                    return;
                }
            }
            finally
            {
                if (m_logMars != null) m_logMars.AddTransferLog("WTR", "PUT", MarsLog.eStatus.End, m_aligner.InfoWafer.m_idMaterial, MarsLog.eMateral.Wafer, m_infoCarrier[m_aligner.InfoWafer.m_nLoadPort].m_strLotID, "WTR", (int)eArm.Upper + 1, "Aligner", 1, datas); 
            }

            m_bHoldWTRShift = false; // BHJ 190312 add
        }
        /**
* @brief Vision에 Wafer를 Put하는 함수
* @param eArm nArm : Arm 위치 설정
* @return 없음
* @note Patch-notes
* 날짜|작성자|설명|비고
* -|-|-|-
* 2023-07-24|조남수|Vision에 Put하는 로그 추가|-
* @warning 없음
*/
        void RunPutVision(eArm nArm, Info_Wafer m_infoWafer, bool bMSG = true) //211012 nscho Wafer정보가 다른 경우 WTR의 정보로 LoadDone 시킴.
        {
            int nPos = m_nTeaching[(int)m_InfoWafer[(int)nArm].m_wafer.m_eSize, (int)eTeaching.Vision];
            string sLog = String.Format("[WTRStart] Put Vision, Arm : {0}, CarrierID : {1}, Slot : {2}", nArm.ToString(), m_vision.InfoWafer.m_strCarrierID, (m_vision.InfoWafer.m_nID + 1).ToString("00")); // 230724 nscho 
            m_log.WriteLog("Sequence", sLog);//230724 nscho
            if (m_vision.IsWFStgReady() != eHWResult.OK || m_vision.IsLifterUp() != true || m_vision.IsVSReadySignal() != true)
            {
                m_log.Popup("Wafer Stage And Lifter is Not Ready Pos or Vision Ready Signal Does Not Turn On");
                m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho
                SetAlarm(eAlarm.Error, eError_Mom.VisionReady); 
                return;
            }
            if (m_vision.IsWaferExistLoad() != false)
            {
                m_log.Popup("Wafer Exist On The Stage");
                m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho
                SetAlarm(eAlarm.Warning, eError_Mom.VisionExist); 
                return;
            }
            try
            {
                if (m_logMars != null) m_logMars.AddTransferLog("WTR", "PUT", MarsLog.eStatus.Start, GetInfoWafer(nArm).m_idMaterial, MarsLog.eMateral.Wafer, m_infoCarrier[GetInfoWafer(nArm).m_nLoadPort].m_strLotID, "WTR", (int)nArm + 1, "Vision", 1, null); 
                if (RunPutMotion(nArm, nPos, 0) != eHWResult.OK)
                {
                    m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho
                    SetState(eState.Error);
                    return;
                }
            }
            finally
            {
                if (m_logMars != null) m_logMars.AddTransferLog("WTR", "PUT", MarsLog.eStatus.End, GetInfoWafer(nArm).m_idMaterial, MarsLog.eMateral.Wafer, m_infoCarrier[GetInfoWafer(nArm).m_nLoadPort].m_strLotID, "WTR", (int)nArm + 1, "Vision", 1, null); 
            } 
            if (!IsArmClose(nArm, true))       //KDG 160927 Add // ing 161018
            {
                m_log.Popup("Arm Close Error");
                m_log.WriteLog("Sequence", "[WTRDone] FAIL"); //230724 nscho
                SetAlarm(eAlarm.Warning, eError_Mom.ArmClose); 
                return;
            }
            //if (m_work.m_bXgemUSe) {
            //    m_auto.ClassXgem300().SetInspStart(m_InfoWafer[(int)nArm].m_nSlot);
            //}
            m_bRotateVision = true;
            m_vision.SetLoadDone(m_infoWafer, bMSG); //211012 nscho Wafer정보가 다른 경우 WTR의 정보로 LoadDone 시킴.
            if (m_auto.m_bXgemUse && (m_xGem.IsOnlineRemote() || m_xGem.IsOnlineLocal())) 
                m_xGem.WaferStart(m_InfoWafer[(int)nArm]); 
            SetInfoWafer(nArm, null);
            m_log.WriteLog("Sequence", "[WTRDone] Put Vision PASS");  //230724 nscho 
            m_vision.m_bWaferExist = true;
        }

        void RunPutBackSide(eArm nArm)
        {
            int nPos = m_nTeaching[(int)m_InfoWafer[(int)nArm].m_wafer.m_eSize, (int)eTeaching.BackSide];
            if (m_backSide.GetState() != HW_BackSide_Mom.eState.Ready) return;
            if (!m_backSide.IsAxisOK()) { m_log.Popup("Backside Position Is Wrong !!"); return; } //ing 161022
            if (m_backSide.CheckWaferExist() != eHWResult.Off)
            {
                m_log.Popup("Put BackSide Error : BackSide Wafer Exist Checked");
                SetAlarm(eAlarm.Warning, eError_Mom.BacksideExsit); 
                return;
            }
            try
            {
                if (m_logMars != null) m_logMars.AddTransferLog("WTR", "PUT", MarsLog.eStatus.Start, GetInfoWafer(nArm).m_idMaterial, MarsLog.eMateral.Wafer, m_infoCarrier[GetInfoWafer(nArm).m_nLoadPort].m_strLotID, "WTR", (int)nArm + 1, "BacksideStage", 1, null); 
                if (RunPutMotion(nArm, nPos, 0) != eHWResult.OK)
                {
                    m_log.Popup("Put BackSide Error");
                    SetAlarm(eAlarm.Warning, eError_Mom.PutBackside); 
                    return;
                }
            }
            finally 
            {
                if (m_logMars != null) m_logMars.AddTransferLog("WTR", "PUT", MarsLog.eStatus.End, GetInfoWafer(nArm).m_idMaterial, MarsLog.eMateral.Wafer, m_infoCarrier[GetInfoWafer(nArm).m_nLoadPort].m_strLotID, "WTR", (int)nArm + 1, "BacksideStage", 1, null); 
            }
            if (!IsArmClose(nArm, true))       //KDG 160927 Add // ing 161018
            {
                m_log.Popup("Arm Close Error");
                SetAlarm(eAlarm.Warning, eError_Mom.ArmClose); 
                return;
            }
            if (m_auto.m_bXgemUse && (m_xGem.IsOnlineRemote() || m_xGem.IsOnlineLocal()))
                m_xGem.WaferStart(m_InfoWafer[(int)nArm]);
            m_backSide.SetRun(GetInfoWafer(nArm));
            SetInfoWafer(nArm, null);
        }

        void buttonHome_Click(object sender, EventArgs e)
        {
            RunHome();
        }

        void buttonVacOn_Click(object sender, EventArgs e)
        {
            RunVac((eArm)(comboArm.SelectedIndex), true);
        }

        void buttonVacOff_Click(object sender, EventArgs e)
        {
            RunVac((eArm)(comboArm.SelectedIndex), false);
        }

        void buttonReset_Click(object sender, EventArgs e)
        {
            ErrorReset();
        }

        void buttonGet_Click(object sender, EventArgs e)
        {
            eArm nArm = (eArm)(comboArm.SelectedIndex);
            int nPos = (int)(eTeaching)(Enum.Parse(typeof(eTeaching), comboPosition.SelectedItem.ToString()));
            int nPoint = m_nTeaching[comboWaferSize.SelectedIndex, nPos];
            int nSlot = comboSlot.SelectedIndex;

            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++)
            {
                if (nPoint == m_nTeaching[n, (int)eTeaching.LoadPort0])
                {
                    if (!m_loadPort[0].IsDoorOpenPos())
                    {
                        m_work.m_run.SetError(eAlarm.Warning);
                        m_log.Popup("Loadport 0 Door Does Not Open");
                        return;
                    }
                }
                if (nPoint == m_nTeaching[n, (int)eTeaching.LoadPort1])
                {
                    if (!m_loadPort[1].IsDoorOpenPos())
                    {
                        m_work.m_run.SetError(eAlarm.Warning);
                        m_log.Popup("Loadport 1 Door Does Not Open");
                        return;
                    }
                }
            }
            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++)
            {
                if (nPoint == m_nTeaching[n, (int)eTeaching.Aligner])
                {
                    if (m_aligner.CheckWaferExist() == eHWResult.Off)
                    {
                        m_work.m_run.SetError(eAlarm.Warning);
                        m_log.Popup("Does Not Exist Wafer On Aligner");
                        return;
                    }
                }
            }
            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++)
            {
                if (nPoint == m_nTeaching[n, (int)eTeaching.Vision])
                {
                    if (m_vision.IsWFStgReady() != eHWResult.OK || m_vision.IsLifterUp() != true || m_vision.IsVSReadySignal() != true)
                    {

                        SetAlarm(eAlarm.Error, eError_Mom.VisionReady); 
                        return;
                    }
                    if (m_vision.IsWaferExistLoad() != true)
                    {
                        m_log.Popup("Wafer Does Not Exist On The Stage");
                        SetAlarm(eAlarm.Warning, eError_Mom.VisionExist); 
                        return;
                    }
                }
            }
            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++)
            {
                if (nPoint == m_nTeaching[n, (int)eTeaching.BackSide])
                {
                    if (m_backSide.CheckWaferExist() == eHWResult.Off)
                    {
                        m_work.m_run.SetError(eAlarm.Warning);
                        m_log.Popup("Does Not Exist Wafer On BackSide");
                        return;
                    }
                }
            }

            RunGetMotion(nArm, nPoint, nSlot);
        }

        void buttonPut_Click(object sender, EventArgs e)
        {
            eArm nArm = (eArm)(comboArm.SelectedIndex);
            int nPos = (int)(eTeaching)(Enum.Parse(typeof(eTeaching), comboPosition.SelectedItem.ToString()));
            int nPoint = m_nTeaching[comboWaferSize.SelectedIndex, nPos];
            int nSlot = comboSlot.SelectedIndex;
            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++)
            {
                if (nPoint == m_nTeaching[n, (int)eTeaching.LoadPort0])
                {
                    if (!m_loadPort[0].IsDoorOpenPos())
                    {
                        m_work.m_run.SetError(eAlarm.Warning);
                        m_log.Popup("Loadport 0 Door Does Not Open");
                        return;
                    }
                }
                if (nPoint == m_nTeaching[n, (int)eTeaching.LoadPort1])
                {
                    if (!m_loadPort[1].IsDoorOpenPos())
                    {
                        m_work.m_run.SetError(eAlarm.Warning);
                        m_log.Popup("Loadport 1 Door Does Not Open");
                        return;
                    }
                }
            }
            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++)
            {
                if (nPoint == m_nTeaching[n, (int)eTeaching.Aligner])
                {
                    if (m_aligner.CheckWaferExist() == eHWResult.On)
                    {
                        m_work.m_run.SetError(eAlarm.Warning);
                        m_log.Popup("Exist Wafer On Aligner");
                        return;
                    }
                }
            }
            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++)
            {
                if (nPoint == m_nTeaching[n, (int)eTeaching.Vision])
                {
                    if (m_vision.IsWFStgReady() != eHWResult.OK || m_vision.IsLifterUp() != true || m_vision.IsVSReadySignal() != true)
                    {
                        m_log.Popup("Wafer Stage And Lifter is Not Ready Pos or Vision Ready Signal Does Not Turn On");
                        SetAlarm(eAlarm.Error, eError_Mom.VisionReady);
                        return;
                    }
                    if (m_vision.IsWaferExistLoad() != true)
                    {
                        m_log.Popup("Wafer Does Not Exist On The Stage");
                        SetAlarm(eAlarm.Warning, eError_Mom.VisionExist);
                        return;
                    }
                }
            }
            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++)
            {
                if (nPoint == m_nTeaching[n, (int)eTeaching.BackSide])
                {
                    if (m_backSide.CheckWaferExist() == eHWResult.On)
                    {
                        m_work.m_run.SetError(eAlarm.Warning);
                        m_log.Popup("Exist Wafer On BackSide");
                        return;
                    }
                }
            }

            RunPutMotion(nArm, nPoint, nSlot);
        }

        private void btnGETEXRT_Click(object sender, EventArgs e)
        {
            //RunGetLPSLimSlot(eArm.Upper, 1, 6);
            eArm nArm = (eArm)(comboArm.SelectedIndex);
            int nPos = (int)(eTeaching)(Enum.Parse(typeof(eTeaching), comboPosition.SelectedItem.ToString()));
            int nPoint = m_nTeaching[comboWaferSize.SelectedIndex, nPos];
            int nSlot = comboSlot.SelectedIndex + 1;

            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++)
            {
                if (nPoint == m_nTeaching[n, (int)eTeaching.LoadPort0])
                {
                    if (!m_loadPort[0].IsDoorOpenPos())
                    {
                        m_work.m_run.SetError(eAlarm.Warning);
                        m_log.Popup("Loadport 0 Door Does Not Open");
                        return;
                    }
                }
                if (nPoint == m_nTeaching[n, (int)eTeaching.LoadPort1])
                {
                    if (!m_loadPort[1].IsDoorOpenPos())
                    {
                        m_work.m_run.SetError(eAlarm.Warning);
                        m_log.Popup("Loadport 1 Door Does Not Open");
                        return;
                    }
                }
            }
            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++)
            {
                if (nPoint == m_nTeaching[n, (int)eTeaching.Aligner])
                {
                    if (m_aligner.CheckWaferExist() == eHWResult.Off)
                    {
                        m_work.m_run.SetError(eAlarm.Warning);
                        m_log.Popup("Does Not Exist Wafer On Aligner");
                        return;
                    }
                }
            }
            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++)
            {
                if (nPoint == m_nTeaching[n, (int)eTeaching.Vision])
                {
                    if (m_vision.IsWFStgReady() != eHWResult.OK || m_vision.IsLifterUp() != true || m_vision.IsVSReadySignal() != true)
                    {
                        m_log.Popup("Wafer Stage And Lifter is Not Ready Pos or Vision Ready Signal Does Not Turn On");
                        SetState(eState.Error);
                        return;
                    }
                    if (m_vision.IsWaferExistLoad() != true)
                    {
                        m_log.Popup("Wafer Does Not Exist On The Stage");
                        SetState(eState.Error);
                        return;
                    }
                }
            }
            RunGetExtendMotion(nArm, nPoint, nSlot);
        }

        private void btnGETRETA_Click(object sender, EventArgs e)
        {
            eArm nArm = (eArm)(comboArm.SelectedIndex);
            int nPos = (int)(eTeaching)(Enum.Parse(typeof(eTeaching), comboPosition.SelectedItem.ToString()));
            int nPoint = m_nTeaching[comboWaferSize.SelectedIndex, nPos];
            int nSlot = comboSlot.SelectedIndex + 1;

            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++)
            {
                if (nPoint == m_nTeaching[n, (int)eTeaching.LoadPort0])
                {
                    if (!m_loadPort[0].IsDoorOpenPos())
                    {
                        m_work.m_run.SetError(eAlarm.Warning);
                        m_log.Popup("Loadport 0 Door Does Not Open");
                        return;
                    }
                }
                if (nPoint == m_nTeaching[n, (int)eTeaching.LoadPort1])
                {
                    if (!m_loadPort[1].IsDoorOpenPos())
                    {
                        m_work.m_run.SetError(eAlarm.Warning);
                        m_log.Popup("Loadport 1 Door Does Not Open");
                        return;
                    }
                }
            }
            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++)
            {
                if (nPoint == m_nTeaching[n, (int)eTeaching.Aligner])
                {
                    if (m_aligner.CheckWaferExist() == eHWResult.Off)
                    {
                        m_work.m_run.SetError(eAlarm.Warning);
                        m_log.Popup("Does Not Exist Wafer On Aligner");
                        return;
                    }
                }
            }
            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++)
            {
                if (nPoint == m_nTeaching[n, (int)eTeaching.Vision])
                {
                    if (m_vision.IsWFStgReady() != eHWResult.OK || m_vision.IsLifterUp() != true || m_vision.IsVSReadySignal() != true)
                    {
                        m_log.Popup("Wafer Stage And Lifter is Not Ready Pos or Vision Ready Signal Does Not Turn On");
                        SetAlarm(eAlarm.Error, eError_Mom.VisionReady);
                        return;
                    }
                    if (m_vision.IsWaferExistLoad() != true)
                    {
                        m_log.Popup("Wafer Does Not Exist On The Stage");
                        SetAlarm(eAlarm.Warning, eError_Mom.VisionExist);
                        return;
                    }
                }
            }
            RunGetRetractMotion(nArm, nPoint, nSlot);
        }

        private void btnPUTEXRT_Click(object sender, EventArgs e)
        {
            eArm nArm = (eArm)(comboArm.SelectedIndex);
            int nPos = (int)(eTeaching)(Enum.Parse(typeof(eTeaching), comboPosition.SelectedItem.ToString()));
            int nPoint = m_nTeaching[comboWaferSize.SelectedIndex, nPos];
            int nSlot = comboSlot.SelectedIndex + 1;
            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++)
            {
                if (nPoint == m_nTeaching[n, (int)eTeaching.LoadPort0])
                {
                    if (!m_loadPort[0].IsDoorOpenPos())
                    {
                        m_work.m_run.SetError(eAlarm.Warning);
                        m_log.Popup("Loadport 0 Door Does Not Open");
                        return;
                    }
                }
                if (nPoint == m_nTeaching[n, (int)eTeaching.LoadPort1])
                {
                    if (!m_loadPort[1].IsDoorOpenPos())
                    {
                        m_work.m_run.SetError(eAlarm.Warning);
                        m_log.Popup("Loadport 1 Door Does Not Open");
                        return;
                    }
                }
            }
            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++)
            {
                if (nPoint == m_nTeaching[n, (int)eTeaching.Aligner])
                {
                    if (m_aligner.CheckWaferExist() == eHWResult.On)
                    {
                        m_work.m_run.SetError(eAlarm.Warning);
                        m_log.Popup("Exist Wafer On Aligner");
                        return;
                    }
                }
            }
            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++)
            {
                if (nPoint == m_nTeaching[n, (int)eTeaching.Vision])
                {
                    if (m_vision.IsWFStgReady() != eHWResult.OK || m_vision.IsLifterUp() != true || m_vision.IsVSReadySignal() != true)
                    {
                        m_log.Popup("Wafer Stage And Lifter is Not Ready Pos or Vision Ready Signal Does Not Turn On");
                        SetAlarm(eAlarm.Error, eError_Mom.VisionReady);
                        return;
                    }
                    if (m_vision.IsWaferExistLoad() != true)
                    {
                        m_log.Popup("Wafer Does Not Exist On The Stage");
                        SetAlarm(eAlarm.Warning, eError_Mom.VisionExist);
                        return;
                    }
                }
            }
            RunPutExtendMotion(nArm, nPoint, nSlot);
        }

        private void btnPUTRETA_Click(object sender, EventArgs e)
        {
            eArm nArm = (eArm)(comboArm.SelectedIndex);
            int nPos = (int)(eTeaching)(Enum.Parse(typeof(eTeaching), comboPosition.SelectedItem.ToString()));
            int nPoint = m_nTeaching[comboWaferSize.SelectedIndex, nPos];
            int nSlot = comboSlot.SelectedIndex + 1;
            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++)
            {
                if (nPoint == m_nTeaching[n, (int)eTeaching.LoadPort0])
                {
                    if (!m_loadPort[0].IsDoorOpenPos())
                    {
                        m_work.m_run.SetError(eAlarm.Warning);
                        m_log.Popup("Loadport 0 Door Does Not Open");
                        return;
                    }
                }
                if (nPoint == m_nTeaching[n, (int)eTeaching.LoadPort1])
                {
                    if (!m_loadPort[1].IsDoorOpenPos())
                    {
                        m_work.m_run.SetError(eAlarm.Warning);
                        m_log.Popup("Loadport 1 Door Does Not Open");
                        return;
                    }
                }
            }
            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++)
            {
                if (nPoint == m_nTeaching[n, (int)eTeaching.Aligner])
                {
                    if (m_aligner.CheckWaferExist() == eHWResult.On)
                    {
                        m_work.m_run.SetError(eAlarm.Warning);
                        m_log.Popup("Exist Wafer On Aligner");
                        return;
                    }
                }
            }
            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++)
            {
                if (nPoint == m_nTeaching[n, (int)eTeaching.Vision])
                {
                    if (m_vision.IsWFStgReady() != eHWResult.OK || m_vision.IsLifterUp() != true || m_vision.IsVSReadySignal() != true)
                    {
                        m_log.Popup("Wafer Stage And Lifter is Not Ready Pos or Vision Ready Signal Does Not Turn On");
                        SetAlarm(eAlarm.Error, eError_Mom.VisionReady); 
                        return;
                    }
                    if (m_vision.IsWaferExistLoad() != true)
                    {
                        m_log.Popup("Wafer Does Not Exist On The Stage");
                        SetAlarm(eAlarm.Warning, eError_Mom.VisionExist); 
                        return;
                    }
                }
            }
            RunPutRetractMotion(nArm, nPoint, nSlot);
        }

        public virtual string GetSystemSpeed() // ing 170626
        {
            return m_sSpeed;
        }

        private void buttonGripOn_Click(object sender, EventArgs e)
        {
            RunGrip((eArm)(comboArm.SelectedIndex), true);
        }

        private void buttonGripOff_Click(object sender, EventArgs e)
        {
            RunGrip((eArm)(comboArm.SelectedIndex), false);
        }

        //KDG 161007 EFEM)Handler IsWaferExistInEQP()에서 구현되어 있음
        /*
        public bool IsWaferSensorCheck()        //20160928 SDH ADD
        {
            if(IsWaferExist(eArm.Upper) == eHWResult.On) return true;
            if(IsWaferExist(eArm.Lower) == eHWResult.On) return true;
            if(m_aligner.IsWaferExist() == eHWResult.On) return true;
            if (m_vision.IsWaferExist() == eHWResult.On) return true;

            return false;

        }
        */
    }
}
