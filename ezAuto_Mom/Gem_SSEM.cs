using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GEM_XGem300Pro;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using ezTools;

namespace ezAutoMom
{
    #region Class_Define
    public class cGemSetting
    {
        public string IP = "0.0.0.0";
        public string Port = "0";
        public string Mode = "Passive";
    }
    public class IniFile
    {

        [DllImport("kernel32")]     //ini파일 읽기
        private static extern int GetPrivateProfileString(String section, String key, String def, StringBuilder retVal, int size, String filePath);
        [DllImport("kernel32")]     //ini파일 쓰기
        private static extern long WritePrivateProfileString(String section, String key, String val, String filePath);

        public static void G_IniWriteValue(String Section, String Key, String Value, string avsPath)        /// ini파일에 쓰기
        {
            WritePrivateProfileString(Section, Key, Value, avsPath);
        }

        public static void G_IniWriteBoolValue(String Section, String Key, bool Value, string avsPath)        /// ini파일에 쓰기
        {
            WritePrivateProfileString(Section, Key, Convert.ToString(Value), avsPath);
        }

        public static void G_IniWriteIntValue(String Section, String Key, Int32 Value, string avsPath)        /// ini파일에 쓰기
        {
            WritePrivateProfileString(Section, Key, Convert.ToString(Value), avsPath);
        }

        public static String G_IniReadValue(String Section, String Key, string avsPath)                     /// ini파일에서 읽어 오기
        {
            StringBuilder temp = new StringBuilder(2000);
            int i = GetPrivateProfileString(Section, Key, "", temp, 2000, avsPath);
            return temp.ToString();
        }


        public static bool G_IniReadBoolValue(String Section, String Key, string avsPath)                     /// ini파일에서 읽어 오기
        {
            bool rt;
            string sValue;
            sValue = G_IniReadValue(Section, Key, avsPath);

            if ((sValue == "0") || (sValue == "-1") || (sValue == "") || (sValue == "False")) {
                rt = false;
            }
            else {
                rt = true;
            }
            return rt;
        }

        public static int G_IniReadIntValue(String Section, String Key, string avsPath)                     /// ini파일에서 읽어 오기
        {
            int rt;
            string sValue;
            sValue = G_IniReadValue(Section, Key, avsPath);
            if (sValue == "") {
                rt = -1;
            }
            else {
                rt = Convert.ToInt32(sValue);
            }
            return rt;
        }

    }
    public class cMapDataInfo
    {
        public enum eFNLOC : long   //FNLOC : flat/notch location in degrees clockwise from bottom, ex) 0, 90, 180, 270
        {
            Notch_Bottom = 0,
            Notch_Left = 90,
            Notch_Top = 180,
            Notch_Right = 270,
        }

        public enum eFFROT : long     //FFROT : film frame location in degrees clockwise from bottom, ex) 0, 90, 180, 270  
        {
            RingFrame_Bottom = 0,
            RingFrame_Left = 90,
            RingFrame_Top = 180,
            RingFrame_Right = 270,
        }

        public enum eORLOC : long    //ORLOC : Origin 좌료의 위치
        {
            CenterOfWafer = 0,
            UpperRight = 1,
            UpperLeft = 2,
            LowerLeft = 3,
            LowerRight = 4,
        }

//        string m_sRedLight = "0";
//        string m_sGreenLight = "0";
//        string m_sBlueLight = "0";
//        string m_sCoaxLight = "0";
//        string m_sSideLight = "0";

//        string WaferID = "";
//        Gem_SSEM.eIDTYPE IDType = Gem_SSEM.eIDTYPE.RingFrame;
//        eFNLOC FNLOC = eFNLOC.Notch_Bottom;
//        eFFROT FFROT = eFFROT.RingFrame_Bottom;
//        eORLOC ORLOC = eORLOC.LowerLeft;
//        int RPSEL = 0;
//        int REFP = 0;
//        string DUTMS = "";
//        float XDIES = 0;    // Die의 X 크기
//        float YDIES = 0;    // Die의 Y 크기
//        int ROWCT = 0;      // Wafer Map의 열 갯수
//        int COLCT = 0;      // Wafer Map의 행 갯수
//        int PRDCT = 0;      // Wafer에 전체 Die 갯수
//        int STRP = 0;       // 좌표 시작 Position
//        string MapData = "";// Wafer Map

    }
    public class cRecipe
    {
        public string[] sName = new string[100];
        public string[] sRedLight = new string[100];
        public string[] sBlueLight = new string[100];
        public string[] sGreenLight = new string[100];
        public string[] sCoaxLight = new string[100];
        public string[] sSideLight = new string[100];
        public string[] sAlignStep = new string[100];
        public string[] sAlignRange = new string[100];
        public string[] sOverlaySearchStart = new string[100];
        public string[] sOverlaySearchLength = new string[100];
        public string[] sOverlayThreshold = new string[100];
        public int nRecipeNum = 0;
    }
    #endregion

    public class Gem_SSEM
    {
        #region State Define
        public enum eXGemModuleState
        {
            INIT = 0,
            IDLE,
            SETUP,
            READY,
            EXCUTE,
        }
        public enum eCommunicateState
        {
            COMM_DISABLE = 1,
            COMM_WAITCR = 2,
            COMM_WAITDELAY = 3,
            COMM_WAITCRA = 4,
            COMM_COMMUNICATING = 5,
        }

        public enum eControlState
        {
            CONT_OFFLINE = 1,
            CONT_ATTEMPTONLINE = 2,
            CONT_HOSTOFFLINE = 3,
            CONT_LOCAL = 4,
            CONT_ONLINEREMOTE = 5,
        }

        public enum eCEID_NO : int
        {
            e_OFFLINE = 0,
            e_ONLINELOCAL = 1,
            e_ONLINEREMOTE = 2,
            e_EQPSTATUSCHANGE = 3,
            e_LP_LOADREQUEST,
            e_LP_LOADCOMPLETE,
            e_LP_CARRIERIDREAD,
            e_LP_SLOTMAP_WAITINGFORHOST,
            e_LP_SLOTMAP_VERIFICATIONOK,
            e_LP_UNLOADREQUEST,
            e_LP_UNLOADCOMPLETE,
            e_LP_ACCESSMODECHANGE,
            e_WorkStarted,              //EFEM에서 첫번째 Lot이 설비로 투입될때
            e_WorkCancel,               //TC에서 Work Cancel 했을때
            e_WorkAbort,                //TC에서 Abort
            e_WorkCompleted,            // 설비에서 마지막 Lot이 배출되어 카세트에 넣었을때
            e_WorkPause,               //TC 에서 Pause
            e_WorkResume,               //TC에서 Resume
            e_WaferStarted,
            e_WaferCompleted,
            e_ProcessStarted,
            e_ProcessEnded,
            //e_PRJob_Queued,
            //e_PRJob_WaitSTart,
            //e_PRJob_Processing,
            //e_PRJob_Completed,
            //e_CTRLJob_Queued,
            //e_CTRLJob_WaitSTart,
            //e_CTRLJob_Excuting,
            //e_CTRLJob_Completed,
            e_Rework,
            e_Scrap,
            e_Decall
        }

        public enum eECV_No
        {
            EQP_Initiate_Connection = 0,
            EstablishCommDelay,
            MaxSpoolTransmit,
            TimeFormat,
            InitControlState,
            OfflineSubstate,
            OnlineSubState,
            OnlineFailState,
            IPAddress,
            DeviceID,
            T3TimeOut,
            T5TimeOut,
            T6TimeOut,
            T7TimeOut,
            T8TimeOut,
        }

        public enum ePPIDChangeMode
        {
            Create = 1,
            Delete,
            Modify,
        }

        public enum eEQPState_SSEM
        {
            EQ_IDLE = 1,
            EQ_RUN,
            EQ_DOWN,
            EQ_PM,
        }

        public enum eVID_NO : int
        {
            v_MDLN = 0,
            v_SOFTREV,
            v_EQPID,
            v_CRST,             //control state
            v_EQ_STATUS,
            v_OldEQState,
            v_CurEQState,
            v_LotID,
            v_CarrierID,
            v_PanelID,
            v_PPID,
            v_RedLight,
            v_BlueLight,
            v_GreenLight,
            v_CoaxLight,
            v_SideLight,
            v_CDA,
            v_Vacuum,
            v_EQP_Pressure,
            v_EQP_Pressure2,
            v_EQP_Temp,
            v_ElecPanel_Temp,
            v_ALST,
            v_ALCD,
            v_ALID,
            v_ALTX,
            v_LOTID,
            v_SlotMap,
            v_ProcessSlotMap,
            v_CarrierType,
            v_PreRunFlag,
            v_CarrierQTY,
            v_SlotNo,
            v_ProductType,
            v_ProductId,
            v_OperatorID,
            v_PortNo,
            v_PortType,
            v_PortTransferState,
            v_PortAvailableState,
            v_PortAccessMode,
            v_ReworkSlotNo,
        }
        public enum eCRST_SSEM
        {
            Offline = 0,
            Local,
            Remote,
        }
        public enum eIDTYPE
        {
            Wafer = 0,
            WaferCassette = 1,
            RingFrame = 2,
        }

        public enum eERRCODE
        {
            NO_ERROR = 0,
            Unkown_Object,
            Unkown_TargetObjuctType,
            Unkown_ObjectInstance,
            Unkown_AttributeName,
            ReadOnly_AccessDenied,
            Unkown_ObjectType,
            Invalid_AttibuteValue,
            Syntax_Error,
            Verifacation_Error,
            Validataion_Error,
            ObjectIdentifier_InUse,
            ParametersImproperlySpecified,
            InsufficientParameters,
            Unsupported_Option,
            Busy,
        }

        public enum eALARMID
        {
            COMMNUNICATION_ERROR,
        }

        public enum eALCD
        {       //Alram Code Byte
            NO_ALRAM = 0,
            PERSONAL_SAFTY,
            EQIPMERNT_SAFTY,
            PARAMETER_Warning,
            PRAMETER_ERROR,
            IRRECOVERALBE_ERROR,
            EQPSTATE_WARNING,
            ATTENTION_FALG,
            DATA_INTEGRITY,
        }
        #region SYSTEMTIME
        public struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        [DllImport("kernel32")]
        public static extern int SetSystemTime(ref SYSTEMTIME lpSystemTime);

        #endregion

        public enum eRCMD
        {
            JOB_START = 1,
            JOB_STOP = 2,
            JOB_CANCLE = 3,
            JOB_ABORT = 4,
            JOB_PAUSE = 5,
            JOB_RESUME = 6,
            PPSELECT = 8,
            OPCALL = 10,
        }

        public class cWaferInfo
        {
            public string m_WaferID = "";
            public int m_nSlotNum = 0;
            public int m_nPortNum = 0;
            public cCMS.eProductType m_eProductType = cCMS.eProductType.WAFER;
        }

        public enum eSECSGEMSITE
        {
            SAMSUNG_EM = 0,
        }

        #endregion

        #region Class
        public class cPJ
        {
            public enum ePRJObState
            {
                Queued = 0,
                SettingUp,
                WaitingForStart,
                Processing,
                ProcessingComplete,
                Reserved,
                Pausing,
                Paused,
                Stopping,
                Aborting,
                Stopped,
                Aborted,
            }

            public string m_sPRJobID = "";
            public string m_sRecipeID = "";
            public string m_ePreRunFlag = "";
            public string m_SlotMap = "";
            public string m_SlotMapInsp = "";
            public ePRJObState PRJobState = ePRJObState.Stopped;
            public string[] m_sPanelID = new string[25] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            public string[] m_sSlotNo = new string[25] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            public string m_sAutoStart = "";
        }

        public class cCJ
        {
            public enum eCTRLJobState
            {
                Queued = 0,
                Selected,
                WaitingForStart,
                Excuting,
                Paused,
                Completed,
            }
            public string sCTRLJobID = "";
            public eCTRLJobState CTRLJobState = eCTRLJobState.Paused;
            public string sCarrierID = "";
            public string sPRJOBID = "";
            public string m_sAutoStart = "";
        }

        public class cCMS
        {
            public enum ePortType
            {
                IN_OUT_PORT = 1,
                INPUT_PORT,
                OUTPUT_PORT,
            }
            public enum eCarrierType
            {

                Cassette = 1,
                P_FOUP,
                Magazine,
                Tray,
                FOSB,
            }
            public enum eLPAvailalbeState
            {
                Empty = 0,
                FULL = 1,
            }
            public enum eLPTransferState
            {
                ReadyToLoad = 1,
                ReadyToProcess,
                Processing,
                ReadyToUnload,
            }
            public enum ePortAccessMode
            {
                Manual = 0,
                Auto,
            }
            public enum eCarrierIDState
            {
                ID_NOT_READ = 0,
                WAITING_FOR_HOST,
                ID_VERIFICATION_OK,
                ID_VERIFICATION_FAILED,
            }
            public enum eCarrierSlotMapState
            {
                SLOT_MAP_NOT_READ = 0,
                WAITING_FOR_HOST,
                SLOTMAP_VERIFICATION_OK,
                SLOTMAP_VERIFICATION_FAILED
            }
            public enum eProductType
            {
                QUAD = 1,
                PANEL = 2,
                WAFER = 3,
            }

            public ePortType m_ePortType = ePortType.IN_OUT_PORT;
            public eCarrierType m_eCarrierType = eCarrierType.FOSB;
            public eLPTransferState m_eLPTransferState = eLPTransferState.ReadyToLoad;
            public ePortAccessMode m_eAccessMode = ePortAccessMode.Auto;
            public eCarrierSlotMapState m_eSlotMapState = eCarrierSlotMapState.SLOT_MAP_NOT_READ;
            public eCarrierIDState m_eCarrierIDState = eCarrierIDState.ID_NOT_READ;
            public eProductType m_eProducType = eProductType.WAFER;

            public string CarrierID = "";
            public string LotID = "";
            public eLPAvailalbeState GetLPAvailableState()
            {
                switch (m_eLPTransferState) {
                    case eLPTransferState.ReadyToLoad:
                        return eLPAvailalbeState.Empty;
                    case eLPTransferState.Processing:
                    case eLPTransferState.ReadyToProcess:
                    case eLPTransferState.ReadyToUnload:
                    default:
                        return eLPAvailalbeState.FULL;
                }
            }
        }
        #endregion

        private XGem300ProNet m_XGem = null;
        private string m_sCfgPath = "C:\\AVIS\\INIT\\Gem300.cfg";
//        private string m_sLogPath = "C:\\AVIS\\Log\\";
        private string m_sCEIDPath = "C:\\AVIS\\INIT\\Gem300CEID.ini";
        private string m_sVIDPath = "C:\\AVIS\\INIT\\Gem300VID.ini";
        private string m_sECVPath = "C:\\AVIS\\INIT\\Gem300ECV.ini";
        public string m_sRecipePath = "C:\\AVIS\\INIT\\GEM300Recipe.ini";
        public string m_sMapDataPath = "C:\\AVIS\\INIT\\MapData.ini";
        private string m_sMLDN = "WIND";
        private string m_sSoftrev = "Ver 1.0";
        private cWaferInfo m_cWaferInfo = new cWaferInfo();
        private cRecipe m_cRecipeList = new cRecipe();
        private eSECSGEMSITE m_eSecsGemSite = eSECSGEMSITE.SAMSUNG_EM;
        private eEQPState_SSEM m_eEQPSTATE = eEQPState_SSEM.EQ_IDLE;

        public cGemSetting m_GemSetting = new cGemSetting();
        private long m_nCommState;
        private long m_nContState;
        private string m_OperatorID = "ATI";
        private string m_EQPID = "HUMT9884";
        private int m_nPortNum = 2;
        public int m_nWorkLP = 2;
        public int m_nWorkSlot = 0;
        private cPJ m_cPJ = new cPJ();
        private cCJ m_cCJ = new cCJ();
        private cCMS[] m_cCMS = new cCMS[2];
        public delegate void Delegate_OneString(string sMsg);
        public delegate void Delegate_Strings(string[] sMsg);
        public delegate void Delegate_2Strings(string sPPID, string[] sName, string[] sValue);
        public delegate void Delegate_2IntStrings(int a, int b, string s);
        public delegate void Delegate_Items(ListViewItem item);
        public delegate void Delegate_Int(int a);
        public event Delegate_OneString OnRecieveTerminalMsg;
        public event Delegate_Strings OnRecieveTerminalMultiMsg;
//        public event Delegate_OneString OnRecieveLogEvent;
//        public event Delegate_Strings OnRecipeDeleteEvent;
        public event Delegate_OneString OnRecipeSelected;
//        public event Delegate_2Strings OnRecipeChanged;
        public event Delegate_2IntStrings OnChangeList;
        public event Delegate_Items OnMakeList;
        public event Delegate_Int OnDeleteItem;


        private int[] m_nCEID = new int[Enum.GetNames(typeof(eCEID_NO)).Length];
        private long[] m_nVID = new long[Enum.GetNames(typeof(eVID_NO)).Length];
        private int[] m_nECV = new int[Enum.GetNames(typeof(eECV_No)).Length];
        Thread m_thread;
        protected bool m_bRunThread = false;
        private eMainCycle MainCycle = eMainCycle.IDLE;
        private int m_nTOTSMP = 0;
        private string m_sTRID = "";
        private int m_nDSPER = 0;
        private int m_nTraceNum = 0;
        private long[] m_nTraceVID = new long[Enum.GetNames(typeof(eVID_NO)).Length];
        private ezStopWatch m_swTrace = new ezStopWatch();
        private Auto_Mom m_Auto;
        private Handler_Mom m_Handler;

        public Gem_SSEM(Auto_Mom auto)
        {
            m_Auto = auto;
            m_Handler = m_Auto.ClassHandler();
            m_XGem = new XGem300ProNet();
            m_nPortNum = 2;
            SetEventHanle();
            LoadCEID();
            LoadVID();
            LoadECV();
            m_thread = new Thread(new ThreadStart(RunThread));
            m_thread.Start();
            m_cWaferInfo = new cWaferInfo();
            for (int n = 0; n < m_nPortNum; n++)
                m_cCMS[n] = new cCMS();
            m_cCMS[0].m_eCarrierType = cCMS.eCarrierType.Cassette;
            m_cCMS[1].m_eCarrierType = cCMS.eCarrierType.FOSB;

        }
        public void ThreadStop()
        {
            if (m_bRunThread) { m_bRunThread = false; m_thread.Join(); }
        }

        public void RunThread()
        {
            m_bRunThread = true;
            Thread.Sleep(5000);

            while (m_bRunThread) {
                Thread.Sleep(10);
                RunScenaio();
            }
        }

        public void SetMainCycle(eMainCycle eCycle, int nLP)
        {
            m_nWorkLP = nLP;
            MainCycle = eCycle;
        }
        public eMainCycle GetMainCycle()
        {
            return MainCycle;
        }
        public enum eMainCycle
        {
            None = 0,
            IDLE,
            SendReadyToLoad,
            Wait_CST_Placed,
            Wait_CarrierIDRead,
            WaitHost_CarrierIDPass,
            Wait_SlotMapForHost,
            WaitHost_SlotMapPass,
            Wait_MappingDone,
            Wait_SlotMapVerificationOK,
            WaitHost_PRJOBCreate,
            Wait_PRJobQueued,
            WaitHost_CTRLJOBCreate,
            Wait_CTRLJobQueued,
            SendProcessStart,
            Send_WaferMapInfo,
            Send_WaferMapDown,
            Wait_WaferStart,
            Wait_WaferEnd,
            Wait_CSTEnd,
            SendProcessEnd,
            Wait_ReadyToUnload,
            Wait_UnloadComplete,
            Stop,

        }

        bool[] m_bLoadReq = new bool[2] { false, false };
        int m_nInspWafer = 0;

        void RunScenaio()
        {
            Thread.Sleep(5000);
            while (m_bRunThread) {
                Thread.Sleep(10);
                if (m_Auto.ClassWork().m_bXgemUSe) {
                    switch (MainCycle) {
                        case eMainCycle.IDLE:
                            if (m_Auto.ClassWork().GetState() == eWorkRun.Run) {
                                SetEQPState(eEQPState_SSEM.EQ_RUN);
                                MainCycle = eMainCycle.SendReadyToLoad;
                                m_nWorkLP = 0;
                            }
                            break;
                        case eMainCycle.SendReadyToLoad:
                            for (int i = 0; i < m_nPortNum; i++) {
                                m_bLoadReq[i] = false;
                                if (!m_Handler.ClassLoadPort(i).IsPlaced()) {
                                    m_cCMS[i] = new cCMS();
                                    m_cCMS[0].m_eCarrierType = cCMS.eCarrierType.Cassette;
                                    m_cCMS[1].m_eCarrierType = cCMS.eCarrierType.FOSB;
                                    LoadRequest(i);
                                    m_bLoadReq[i] = true;
                                    MainCycle = eMainCycle.Wait_CST_Placed;
                                    Thread.Sleep(1000);
                                }
                            }
                            break;
                        case eMainCycle.Wait_CST_Placed:
                            for (int i = 0; i < m_nPortNum; i++) {
                                if (m_Handler.ClassLoadPort(i).IsPlaced() && m_bLoadReq[i] && m_Auto.IsCSTLoadOK(m_nPortNum)) {
                                    LoadComplete(i);
                                    m_nWorkLP = i;
                                    m_Handler.ClassLoadPort(i).SetState(HW_LoadPort_Mom.eState.CSTIDRead);
                                    MainCycle = eMainCycle.Wait_CarrierIDRead;
                                    Thread.Sleep(1000);
                                    break;
                                }
                            }
                            break;
                        case eMainCycle.Wait_CarrierIDRead:
                            if (m_Handler.ClassRFID(m_nWorkLP).GetCSTID() != "" && m_Handler.ClassRFID(m_nWorkLP).GetLOTID() != "") {
                                m_cCMS[m_nWorkLP].LotID = m_Handler.ClassRFID(m_nWorkLP).GetLOTID();
                                m_cCMS[m_nWorkLP].CarrierID = m_Handler.ClassRFID(m_nWorkLP).GetCSTID();
                                CarrierIDRead(m_nWorkLP, m_cCMS[m_nWorkLP].CarrierID, m_cCMS[m_nWorkLP].LotID);
                                MainCycle = eMainCycle.WaitHost_CarrierIDPass;
                            }
                            break;
                        case eMainCycle.WaitHost_CarrierIDPass:
                            if (m_cCMS[m_nWorkLP].m_eCarrierIDState == cCMS.eCarrierIDState.ID_VERIFICATION_OK) {
                                m_bMappingDone = false;
                                MainCycle = eMainCycle.Wait_MappingDone;
                            }
                            else if (m_cCMS[m_nWorkLP].m_eCarrierIDState == cCMS.eCarrierIDState.ID_VERIFICATION_FAILED) {
                            }
                            break;
                        case eMainCycle.Wait_MappingDone:
                            if (m_bMappingDone) {
                                m_bMappingDone = false;
                                SendSlotMapToHost(m_nWorkLP, m_cPJ.m_SlotMap);
                                MainCycle = eMainCycle.WaitHost_SlotMapPass;
                            }
                            break;
                        case eMainCycle.WaitHost_SlotMapPass:
                            if (m_cCMS[m_nWorkLP].m_eSlotMapState == cCMS.eCarrierSlotMapState.SLOTMAP_VERIFICATION_OK) {
                                SendEvent_SamSung_ELEC(eCEID_NO.e_LP_SLOTMAP_VERIFICATIONOK, m_nWorkLP);
                                m_cPJ = new cPJ();
                                MainCycle = eMainCycle.WaitHost_PRJOBCreate;
                            }
                            else if (m_cCMS[m_nWorkLP].m_eSlotMapState == cCMS.eCarrierSlotMapState.SLOTMAP_VERIFICATION_FAILED) {
                            }
                            break;
                        case eMainCycle.WaitHost_PRJOBCreate:
                            if (m_cPJ.m_sPRJobID != "") {
                                MainCycle = eMainCycle.Wait_PRJobQueued;
                            }
                            break;
                        case eMainCycle.Wait_PRJobQueued:
                            Thread.Sleep(1000);
                            m_cPJ.PRJobState = cPJ.ePRJObState.Queued;
                            //SendEvent_SamSung_ELEC(eCEID_NO.e_PRJob_Queued,m_nWorkLP);
                            MainCycle = eMainCycle.WaitHost_CTRLJOBCreate;
                            break;
                        case eMainCycle.WaitHost_CTRLJOBCreate:
                            if (m_cCJ.sCTRLJobID != "") {
                                MainCycle = eMainCycle.Wait_CTRLJobQueued;
                            }
                            break;
                        case eMainCycle.Wait_CTRLJobQueued:
                            Thread.Sleep(1000);
                            m_cCJ.CTRLJobState = cCJ.eCTRLJobState.Queued;
                            //SendEvent_SamSung_ELEC(eCEID_NO.e_CTRLJob_Queued,m_nWorkLP);
                            MainCycle = eMainCycle.SendProcessStart;
                            break;
                        case eMainCycle.SendProcessStart:
                            Thread.Sleep(1000);
                            WorkStart();
                            m_cCJ.CTRLJobState = cCJ.eCTRLJobState.Excuting;
                            //SendEvent_SamSung_ELEC(eCEID_NO.e_WorkStarted, m_nWorkLP);
                            Thread.Sleep(500);
                            ProcessStart();
                            m_nInspWafer = 3;
                            m_cPJ.PRJobState = cPJ.ePRJObState.Processing;
                            //SendEvent_SamSung_ELEC(eCEID_NO.e_ProcessStarted, m_nWorkLP);
//                            m_bWaferIDRead = false;
                            m_bInstStart = false;
                            MainCycle = eMainCycle.Wait_WaferStart;
                            break;
                        case eMainCycle.Wait_WaferStart:
                            Thread.Sleep(5000);
                            if (m_bInstStart) {
                                SendEvent_SamSung_ELEC(eCEID_NO.e_WaferStarted, m_nWorkLP);
                                MainCycle = eMainCycle.Wait_WaferEnd;
                                m_bInspEnd = false;
                                m_bInstStart = false;
                            }
                            else {
                                MainCycle = eMainCycle.Wait_CSTEnd;
                            }
                            break;
                        case eMainCycle.Wait_WaferEnd:
                            if (m_bInspEnd) {
                                m_bInspEnd = false;
                                SendEvent_SamSung_ELEC(eCEID_NO.e_WaferCompleted, m_nWorkLP);
                                m_nInspWafer++;
                                if (m_nInspWafer == 4) {
                                    MainCycle = eMainCycle.Wait_CSTEnd;
                                }
                                else {
                                    MainCycle = eMainCycle.Wait_WaferStart;
                                }
                            }
                            break;
                        case eMainCycle.Wait_CSTEnd:
                            Thread.Sleep(5000);
                            if (m_bCSTEnd) {
                                m_bCSTEnd = false;
                                ProcessEnd();
                                //SendEvent_SamSung_ELEC(eCEID_NO.e_ProcessEnded, m_nWorkLP);
                                MainCycle = eMainCycle.SendProcessEnd;
                            }
                            else
                                MainCycle = eMainCycle.Wait_WaferStart;
                            break;
                        case eMainCycle.SendProcessEnd:
                            SendDecallEvent();
                            Thread.Sleep(1000);
                            WorkComplete();
                            MainCycle = eMainCycle.Wait_ReadyToUnload;
                            break;
                        case eMainCycle.Wait_ReadyToUnload:
                            if (m_Handler.ClassLoadPort(m_nWorkLP).GetState() == HW_LoadPort_Mom.eState.Ready) {
                                UnLoadRequest(m_nWorkLP);
                                MainCycle = eMainCycle.Wait_UnloadComplete;
                            }
                            break;
                        case eMainCycle.Wait_UnloadComplete:
                            if (!(m_Handler.ClassLoadPort(m_nWorkLP).IsPlaced())) {
                                UnLoadComplete(m_nWorkLP);
                                MainCycle = eMainCycle.SendReadyToLoad;
                            }
                            break;
                        case eMainCycle.Stop:
                            break;
                    }
                }
            }

        }
        bool m_bMappingDone = false;
//        bool m_bWaferIDRead = false;
        bool m_bInstStart = false;
        bool m_bInspEnd = false;
        bool m_bCSTEnd = false;

        public void SetMappingDone(string sMapData)
        {
            m_cPJ.m_SlotMap = sMapData;
            m_bMappingDone = true;
        }
        public void SetWaferIDForMapDown(string ssWaferID, int nSlot)
        {
            m_cWaferInfo.m_WaferID = ssWaferID;
            m_cWaferInfo.m_nSlotNum = nSlot;

            MapDataInfoRequest(ssWaferID);
            Thread.Sleep(4000);
            if (m_bMapReq) {
                MapDataDownLoad(ssWaferID);
                m_bMapReq = false;
            }
            else {
                m_Auto.ClassWork().m_run.Stop();
            }
        }

        public void SetInspStart(int nSlot)
        {
            m_nWorkSlot = nSlot;
            m_bInstStart = true;
        }

        public void SetInspDone(int nSlot)
        {
            m_bInspEnd = true;
        }

        public void SetCSTEnd()
        {
            m_bCSTEnd = true;
        }

        private void SetEventHanle()
        {
            this.m_XGem.OnGEMCommStateChanged += new OnGEMCommStateChanged(OnGEMCommStateChanged);
            this.m_XGem.OnGEMControlStateChanged += new OnGEMControlStateChanged(OnGEMControlStateChanged);
            this.m_XGem.OnXGEMStateEvent += new OnXGEMStateEvent(OnXGEMStateEvent);
            this.m_XGem.OnGEMTerminalMessage += new OnGEMTerminalMessage(OnGEMTerminalMessageRecieve);
            this.m_XGem.OnGEMTerminalMultiMessage += new OnGEMTerminalMultiMessage(OnGEMTerminalMessageMultiRecieve);
            //this.m_XGem.OnGEMReqDateTime += new OnGEMReqDateTime(OnGEMReqDateTime);
            this.m_XGem.OnGEMReqGetDateTime += new OnGEMReqGetDateTime(OnGemReqGetDateTime);
            this.m_XGem.OnSECSMessageReceived += new OnSECSMessageReceived(OnSECSMessageReceived);
            this.m_XGem.OnGEMReqOnline += new OnGEMReqOnline(OnGEMReqOnline);
            this.m_XGem.OnGEMReqOffline += new OnGEMReqOffline(OnGEMReqOffline);
            this.m_XGem.OnGEMReqPPDelete += new OnGEMReqPPDelete(OnGEMReqPPDelete);
            this.m_XGem.OnGEMReqPPList += new OnGEMReqPPList(OnGEMReqPPList);
        }

        private void OnGEMReqPPDelete(long nMsgId, long nCount, string[] psPPID)
        {
            int nExist2 = 1;
            int nExist1 = 0;
            string sFile = m_sRecipePath;
            FileInfo fi = new FileInfo(sFile);
            string sSection = String.Format("Recipe Num");
            m_cRecipeList.nRecipeNum = IniFile.G_IniReadIntValue(sSection, "Num", sFile);
            for (int j = 0; j < nCount; j++) {
                if (psPPID[j].IndexOf(" ") >= 0)
                    psPPID[j] = psPPID[j].Substring(0, psPPID[j].IndexOf(" "));
                for (int i = 0; i < m_cRecipeList.nRecipeNum; i++) {
                    sSection = String.Format("Recipe" + i);
                    string temp = IniFile.G_IniReadValue(sSection, "Name", sFile);
                    if (psPPID[j] == temp)
                        nExist1 = 1;
                }
                if (nExist1 == 0)
                    nExist2 = 0;
            }
            if (nExist2 == 0) { //Recipe 없음
                m_XGem.GEMRspPPDelete(nMsgId, nCount, psPPID, 4);
            }
            else {
                for (int i = 0; i < psPPID.Length; i++) {
                    for (int j = 0; j < m_cRecipeList.nRecipeNum; j++) {
                        if (m_cRecipeList.sName[j] == psPPID[i]) {
                            OnDeleteItem(j);
                            //lvRecipe.Items[j].Remove();
                            //lvRecipe.Update();
                        }
                    }
                }
                SaveRecipe();
                LoadRecipe();
                m_XGem.GEMRspPPDelete(nMsgId, nCount, psPPID, 0);
            }
        }

        private void OnGEMReqPPList(long nMsgId)
        {
            string[] sPPids = new string[m_cRecipeList.nRecipeNum];
            for (int i = 0; i < m_cRecipeList.nRecipeNum; i++) {
                sPPids[i] = m_cRecipeList.sName[i];
            }
            m_XGem.GEMRspPPList(nMsgId, (long)m_cRecipeList.nRecipeNum, sPPids);
        }


        private void ChangeTime(long nObjectID, long nSysbyte)
        {
            long ret = 0;       // ret 1 = Error 상태  0 : 정상
            long nAckObjID = 0;
            string sSystemTime = "";
            m_XGem.GetStringItem(nObjectID, ref sSystemTime);
            if (sSystemTime.Length == 14) {
                SYSTEMTIME Systime = new SYSTEMTIME();
                Systime.wYear = Convert.ToUInt16(sSystemTime.Substring(0, 4));
                Systime.wDayOfWeek = 4;
                Systime.wMonth = Convert.ToUInt16(sSystemTime.Substring(4, 2));
                Systime.wDay = Convert.ToUInt16(sSystemTime.Substring(6, 2));
                Systime.wHour = Convert.ToUInt16(sSystemTime.Substring(8, 2));
                Systime.wMinute = Convert.ToUInt16(sSystemTime.Substring(10, 2));
                Systime.wSecond = Convert.ToUInt16(sSystemTime.Substring(12, 2));
                int i = SetSystemTime(ref Systime);
                //ret = 0;
            }
            m_XGem.MakeObject(ref nAckObjID);
            m_XGem.SetStringItem(nAckObjID, ret.ToString());

            m_XGem.SendSECSMessage(nAckObjID, 2, 32, nSysbyte);
            //m_XGem300.GEMRspDateTime(nMsgld, ret);
        }

        public void WorkStart()
        {
            SendEvent_SamSung_ELEC(eCEID_NO.e_WorkStarted, m_nWorkLP);
            //SendEvent_SamSung_ELEC(eCEID_NO.e_CTRLJob_Excuting, m_nWorkLP);
            m_cCJ.CTRLJobState = cCJ.eCTRLJobState.Excuting;
        }

        public void WorkComplete()
        {
            SendEvent_SamSung_ELEC(eCEID_NO.e_WorkCompleted, m_nWorkLP);
            m_cCJ.CTRLJobState = cCJ.eCTRLJobState.Completed;
            //SendEvent_SamSung_ELEC(eCEID_NO.e_CTRLJob_Completed, m_nWorkLP);
        }
        public void ProcessStart()
        {
            SendEvent_SamSung_ELEC(eCEID_NO.e_ProcessStarted, m_nWorkLP);
            //SendEvent_SamSung_ELEC(eCEID_NO.e_PRJob_Processing, m_nWorkLP);
            m_cPJ.PRJobState = cPJ.ePRJObState.Processing;
        }
        public void ProcessEnd()
        {
            SendEvent_SamSung_ELEC(eCEID_NO.e_ProcessEnded, m_nWorkLP);
            m_cPJ.PRJobState = cPJ.ePRJObState.ProcessingComplete;
            //SendEvent_SamSung_ELEC(eCEID_NO.e_PRJob_Completed);

        }
        private void SetSoftwareVersion()
        {
            SetSVValue(eVID_NO.v_MDLN, m_sMLDN);
            SetSVValue(eVID_NO.v_SOFTREV, m_sSoftrev);
        }
        #region VID_CEID_ECV
        private long LoadCEID()
        {
            string sFile = m_sCEIDPath;
            FileInfo fi = new FileInfo(sFile);
            if (fi.Exists == false) {
                InitCEID();
                return 0;
            }
            for (int i = 0; i < Enum.GetNames(typeof(eCEID_NO)).Length; i++) {
                string sSection = String.Format("Event" + " " + Enum.GetName(typeof(eCEID_NO), i));
                m_nCEID[i] = IniFile.G_IniReadIntValue(sSection, "Num", sFile);
            }
            return 1;
        }

        private void InitCEID()
        {
            int i = 0;
            for (i = 0; i < Enum.GetNames(typeof(eCEID_NO)).Length; i++) {
                m_nCEID[i] = -1;
            }
        }
        private void InitECV()
        {
            int i = 0;
            for (i = 0; i < Enum.GetNames(typeof(eECV_No)).Length; i++) {
                m_nECV[i] = -1;
            }
        }

        private long LoadVID()
        {
            string sFile = m_sVIDPath;
            FileInfo fi = new FileInfo(sFile);
            if (fi.Exists == false) {
                InitVID();
                return 0;
            }
            for (int i = 0; i < Enum.GetNames(typeof(eVID_NO)).Length; i++) {
                string sSection = String.Format("VID" + " " + Enum.GetName(typeof(eVID_NO), i));
                m_nVID[i] = IniFile.G_IniReadIntValue(sSection, "Num", sFile);
            }
            return 1;

        }

        private long LoadECV()
        {
            string sFile = m_sECVPath;
            FileInfo fi = new FileInfo(sFile);
            if (fi.Exists == false) {
                InitVID();
                return 0;
            }
            for (int i = 0; i < Enum.GetNames(typeof(eECV_No)).Length; i++) {
                string sSection = String.Format("ECV" + " " + Enum.GetName(typeof(eECV_No), i));
                m_nECV[i] = IniFile.G_IniReadIntValue(sSection, "Num", sFile);
            }
            return 1;

        }

        public void LoadRecipe()
        {
            string sFile = m_sRecipePath;
            FileInfo fi = new FileInfo(sFile);
            string sSection = String.Format("Recipe Num");
            m_cRecipeList.nRecipeNum = IniFile.G_IniReadIntValue(sSection, "Num", sFile);
            for (int i = 0; i < m_cRecipeList.nRecipeNum; i++) {
                sSection = String.Format("Recipe" + i);
                m_cRecipeList.sName[i] = IniFile.G_IniReadValue(sSection, "Name", sFile);
                m_cRecipeList.sRedLight[i] = IniFile.G_IniReadValue(sSection, "RedLight", sFile);
                m_cRecipeList.sBlueLight[i] = IniFile.G_IniReadValue(sSection, "BlueLight", sFile);
                m_cRecipeList.sGreenLight[i] = IniFile.G_IniReadValue(sSection, "GreenLight", sFile);
                m_cRecipeList.sCoaxLight[i] = IniFile.G_IniReadValue(sSection, "CoaxLight", sFile);
                m_cRecipeList.sSideLight[i] = IniFile.G_IniReadValue(sSection, "SideLight", sFile);
                m_cRecipeList.sAlignStep[i] = IniFile.G_IniReadValue(sSection, "AlignStep", sFile);
                m_cRecipeList.sAlignRange[i] = IniFile.G_IniReadValue(sSection, "AlignRange", sFile);
                m_cRecipeList.sOverlaySearchStart[i] = IniFile.G_IniReadValue(sSection, "OverlaySearchStart", sFile);
                m_cRecipeList.sOverlaySearchLength[i] = IniFile.G_IniReadValue(sSection, "OverlaySearchLenght", sFile);
                m_cRecipeList.sOverlayThreshold[i] = IniFile.G_IniReadValue(sSection, "OverlayThreshold", sFile);
            }
        }
        public void SetRecipeNum(int nNum)
        {
            m_cRecipeList.nRecipeNum = nNum;
        }

        private void SaveRecipe()
        {
            //string sFile = m_sRecipePath;
            //FileInfo fi = new FileInfo(sFile);
            //string sSection = String.Format("Recipe Num");
            //IniFile.G_IniWriteIntValue(sSection, "Num", m_cRecipeList.nRecipeNum, sFile);
            //for (int i = 0; i < m_cRecipeList.nRecipeNum; i++)
            //{
            //    sSection = String.Format("Recipe" + i);
            //    IniFile.G_IniWriteValue(sSection, "Name", lvRecipe.Items[i].SubItems[0].Text, sFile);
            //    IniFile.G_IniWriteIntValue(sSection, "RedLight", Convert.ToInt32(lvRecipe.Items[i].SubItems[1].Text), sFile);
            //    IniFile.G_IniWriteIntValue(sSection, "BlueLight", Convert.ToInt32(lvRecipe.Items[i].SubItems[2].Text), sFile);
            //    IniFile.G_IniWriteIntValue(sSection, "GreenLight", Convert.ToInt32(lvRecipe.Items[i].SubItems[3].Text), sFile);
            //    IniFile.G_IniWriteIntValue(sSection, "CoaxLight", Convert.ToInt32(lvRecipe.Items[i].SubItems[4].Text), sFile);
            //    IniFile.G_IniWriteIntValue(sSection, "SideLight", Convert.ToInt32(lvRecipe.Items[i].SubItems[5].Text), sFile);
            //    IniFile.G_IniWriteIntValue(sSection, "AlignStep", Convert.ToInt32(lvRecipe.Items[i].SubItems[6].Text), sFile);
            //    IniFile.G_IniWriteIntValue(sSection, "AlignRange", Convert.ToInt32(lvRecipe.Items[i].SubItems[7].Text), sFile);
            //    IniFile.G_IniWriteIntValue(sSection, "OverlaySearchStart", Convert.ToInt32(lvRecipe.Items[i].SubItems[8].Text), sFile);
            //    IniFile.G_IniWriteIntValue(sSection, "OverlaySearchLenght", Convert.ToInt32(lvRecipe.Items[i].SubItems[9].Text), sFile);
            //    IniFile.G_IniWriteIntValue(sSection, "OverlayThreshold", Convert.ToInt32(lvRecipe.Items[i].SubItems[10].Text), sFile);

            //}
        }

        private void InitVID()
        {
            int i = 0;
            for (i = 0; i < Enum.GetNames(typeof(eVID_NO)).Length; i++) {
                m_nVID[i] = -1;
            }
        }

        public void GetVIDList(ref string[] sVIDList, ref int[] nVIDNum)
        {
            string sFile = m_sVIDPath;
            FileInfo fi = new FileInfo(sFile);
            for (int i = 0; i < Enum.GetNames(typeof(eVID_NO)).Length; i++) {
                string sSection = String.Format("VID" + " " + Enum.GetName(typeof(eVID_NO), i));
                sVIDList[i] = Enum.GetName(typeof(eVID_NO), i).Replace("v_", "");
                nVIDNum[i] = IniFile.G_IniReadIntValue(sSection, "Num", sFile);
            }
        }

        public void GetCEIDList(ref string[] sCEIDList, ref int[] nCEIDNum)
        {
            string sFile = m_sCEIDPath;
            FileInfo fi = new FileInfo(sFile);
            for (int i = 0; i < Enum.GetNames(typeof(eCEID_NO)).Length; i++) {
                string sSection = String.Format("EVENT" + " " + Enum.GetName(typeof(eCEID_NO), i));
                sCEIDList[i] = Enum.GetName(typeof(eCEID_NO), i).Replace("e_", "");
                nCEIDNum[i] = IniFile.G_IniReadIntValue(sSection, "Num", sFile);
            }
        }

        public void GetECVList(ref string[] sECVList, ref int[] nECVNum)
        {
            string sFile = m_sECVPath;
            FileInfo fi = new FileInfo(sFile);
            for (int i = 0; i < Enum.GetNames(typeof(eECV_No)).Length; i++) {
                string sSection = String.Format("ECV" + " " + Enum.GetName(typeof(eECV_No), i));
                sECVList[i] = Enum.GetName(typeof(eECV_No), i).Replace("e_", "");
                nECVNum[i] = IniFile.G_IniReadIntValue(sSection, "Num", sFile);
            }
        }

        public long SetSVValue(eVID_NO eVID, string data)
        {
            long[] sv = new long[1];
            string[] value = new string[1];
            sv[0] = m_nVID[(int)eVID];
            value[0] = data;
            long rt = m_XGem.GEMSetVariable(1, sv, value);
            return rt;
        }

        public string GetSVValue(eVID_NO eVID)
        {
            long[] sv = new long[1];
            string[] value = new string[1];
            sv[0] = m_nVID[(int)eVID];
            long rt = m_XGem.GEMGetVariable(1, ref sv, ref value);
            return value[0];
        }

        public long SetECVValue(eVID_NO eVID, string data)
        {
            long[] sv = new long[1];
            string[] value = new string[1];
            sv[0] = m_nVID[(int)eVID];
            value[0] = data;
            long rt = m_XGem.GEMSetECVChanged(1, sv, value);
            return rt;
        }
        #endregion

        #region Public_Method
        public void Initialize()
        {
            long ret = m_XGem.Initialize(m_sCfgPath);
            WriteLog("XGem Module Initialized (Return Value = " + ret.ToString() + ")");
        }
        public void GemStart()
        {
            long ret = m_XGem.Start();
            WriteLog("Xgem Module Started (Return Value = " + ret.ToString() + ")");
        }
        public void GemStop()
        {
            long ret = m_XGem.Stop();
            long ret2 = m_XGem.Close();
            WriteLog("Xgem Module Closed (Return Value = " + ret + " " + ret2 + ")");
        }

        public void SetOffline()
        {
            switch (m_eSecsGemSite) {
                case eSECSGEMSITE.SAMSUNG_EM:
                    SetSVValue(eVID_NO.v_CRST, ((int)eCRST_SSEM.Offline).ToString());
                    SendEvent_SamSung_ELEC(eCEID_NO.e_OFFLINE);
                    m_XGem.GEMReqOffline();
                    WriteLog("Set Offline");
                    break;
                default:
                    m_XGem.GEMReqOffline();
                    break;
            }
        }

        public void SetOnlineLocal()
        {
            switch (m_eSecsGemSite) {
                case eSECSGEMSITE.SAMSUNG_EM:
                    SetSVValue(eVID_NO.v_CRST, ((int)eCRST_SSEM.Local).ToString());
                    SendEvent_SamSung_ELEC(eCEID_NO.e_ONLINELOCAL);
                    WriteLog("Set Local");
                    break;
                default:
                    m_XGem.GEMReqLocal();
                    break;
            }
        }
        public void SetOnlineRemote()
        {
            switch (m_eSecsGemSite) {
                case eSECSGEMSITE.SAMSUNG_EM:
                    SetSVValue(eVID_NO.v_CRST, ((int)eCRST_SSEM.Remote).ToString());
                    SendEvent_SamSung_ELEC(eCEID_NO.e_ONLINEREMOTE);
                    WriteLog("Set Online Remote");
                    break;
                default:
                    m_XGem.GEMReqRemote();
                    break;
            }
        }
        public void SetEQPState(eEQPState_SSEM eEQPState)
        {
            SetSVValue(eVID_NO.v_EQ_STATUS, ((long)eEQPState).ToString());
            SetSVValue(eVID_NO.v_OldEQState, ((long)m_eEQPSTATE).ToString());
            SetSVValue(eVID_NO.v_CurEQState, ((long)eEQPState).ToString());
            WriteLog("EQP State Change " + m_eEQPSTATE.ToString() + " -> " + eEQPState.ToString());
            m_eEQPSTATE = eEQPState;
            SetSVValue(eVID_NO.v_ALST, "0");
            SendEvent_SamSung_ELEC(eCEID_NO.e_EQPSTATUSCHANGE);
        }
        public enum eALList
        {
            Emergency_Error = 1000,
            CDA_Low_Error,
            GN2_Low_Error,
            Interlock_Key_Error,
            MC_Reset_Signal_Error,
            Door_Lock_Error,
            Motor_Stop_Error,
            Light_Error,
            TCP_Disconnect_Error,
            Ionizer_Error,
            Vision_Ready_Signal_Error,
            FDC_Temperature_Error,
            FDC_Presure_Error,
            FDC_Vacuum_Error,
            FDC_FFU_Error,
            FDC_Electrostatic_Error,
            FDC_Illumination_Error,
            FDC_EcoPower_Error,
            Barcode_Read_Error,
            TCP_Disconnect_Error2,
            WTR_Communication_Error,
            LoadPort_Communication_Error,
            Aligner_Communication_Error,
            WTR_No_Wafer_Before_Get_Error,
            WTR_No_Wafer_After_Get_Error,
            WTR_No_Wafer_During_Get_Error,
            WTR_No_Wafer_Before_Put_Error,
            WTR_No_Wafer_After_Put_Error,
            WTR_No_Wafer_During_Put_Error,
            LoadPort_Docking_Error,
            LoadPort_Door_Open_Error,
            LoadPort_Door_Close_Error,
            LoadPort_Door_Interlock_Error,
            Aligner_Alignment_Error,
            Aligner_No_Wafer_Error,

        }

        public void SetAlarm(bool bSet, bool bSerious, eALList nALID, string sALTX)
        {
            if (nALEnable[((int)nALID) - 1000] == 1) {
                long nObjectID = 0;
                m_XGem.MakeObject(ref nObjectID);
                m_XGem.SetListItem(nObjectID, 5);
                m_XGem.SetStringItem(nObjectID, m_EQPID);
                m_XGem.SetStringItem(nObjectID, (Convert.ToInt32(bSet).ToString()));
                m_XGem.SetStringItem(nObjectID, (Convert.ToInt32(bSerious) + 1).ToString());
                m_XGem.SetStringItem(nObjectID, ((int)nALID).ToString());
                m_XGem.SetStringItem(nObjectID, sALTX);//ALTX
                m_XGem.SendSECSMessage(nObjectID, 5, 1, 0);
                if (bSet) {
                    SetSVValue(eVID_NO.v_ALST, Convert.ToInt32(bSet).ToString());
                    SetSVValue(eVID_NO.v_ALCD, Convert.ToInt32(bSerious).ToString());
                    SetSVValue(eVID_NO.v_ALID, ((int)nALID).ToString());
                    SetSVValue(eVID_NO.v_ALTX, sALTX);
                    SetEQPState(eEQPState_SSEM.EQ_DOWN);
                }
                else {
                    SetSVValue(eVID_NO.v_ALST, " ");
                    SetSVValue(eVID_NO.v_ALCD, " ");
                    SetSVValue(eVID_NO.v_ALID, " ");
                    SetSVValue(eVID_NO.v_ALTX, " ");
                    SetEQPState(eEQPState_SSEM.EQ_IDLE);
                }
            }

        }

        public cGemSetting GetParameters()
        {
            string IPAddress = null;
            string Port = null;
            string Mode = null;
            string[] IPText = new string[4];
            m_XGem.GEMGetParam("IP", ref IPAddress);
            m_XGem.GEMGetParam("Port", ref Port);
            m_XGem.GEMGetParam("Active", ref Mode);
            m_GemSetting.IP = IPAddress;
            m_GemSetting.Port = Port;
            if (Mode == "true")
                m_GemSetting.Mode = "Active";
            else if (Mode == "false")
                m_GemSetting.Mode = "Passive";
            WriteLog("IP: " + IPAddress + " PORT: " + Port + " MODE: " + Mode);
            return m_GemSetting;
        }

        public void LoadRequest(int nLP)
        {
            switch (m_eSecsGemSite) {
                case eSECSGEMSITE.SAMSUNG_EM:
                    SendEvent_SamSung_ELEC(eCEID_NO.e_LP_LOADREQUEST, nLP);
                    WriteLog("LP Loadport Load Request Event Send : LP " + nLP);
                    break;

            }
        }

        public void UnLoadRequest(int nLP)
        {
            switch (m_eSecsGemSite) {
                case eSECSGEMSITE.SAMSUNG_EM:
                    SendEvent_SamSung_ELEC(eCEID_NO.e_LP_UNLOADREQUEST, nLP);
                    WriteLog("LP Loadport Unload Request Event Send : LP " + nLP);
                    break;
                default:
                    break;
            }
        }

        public void LoadComplete(int nLP)
        {
            switch (m_eSecsGemSite) {
                case eSECSGEMSITE.SAMSUNG_EM:
                    m_cCMS[m_nWorkLP].m_eLPTransferState = cCMS.eLPTransferState.ReadyToProcess;
                    SetSVValue(eVID_NO.v_LOTID, "");
                    SetSVValue(eVID_NO.v_CarrierID, "");
                    SetSVValue(eVID_NO.v_PortTransferState, ((int)(cCMS.eLPTransferState.ReadyToProcess)).ToString());
                    SendEvent_SamSung_ELEC(eCEID_NO.e_LP_LOADCOMPLETE, nLP);
                    WriteLog("LP Loadport Complete Event Send : LP " + nLP);
                    break;
                default:
                    break;
            }
        }

        public void UnLoadComplete(int nLP)
        {
            switch (m_eSecsGemSite) {
                case eSECSGEMSITE.SAMSUNG_EM:
                    SendEvent_SamSung_ELEC(eCEID_NO.e_LP_UNLOADCOMPLETE, nLP);
                    WriteLog("LP Loadport Complete Event Send : LP " + nLP);
                    break;
                default:
                    break;
            }
        }

        public void CarrierIDRead(int nLP, string CarrierID, string LotID)
        {
            switch (m_eSecsGemSite) {
                case eSECSGEMSITE.SAMSUNG_EM:
                    m_cCMS[nLP].CarrierID = CarrierID;
                    m_cCMS[nLP].LotID = LotID;
                    m_cCMS[nLP].m_eCarrierIDState = cCMS.eCarrierIDState.WAITING_FOR_HOST;
                    SetSVValue(eVID_NO.v_LOTID, LotID);
                    SetSVValue(eVID_NO.v_CarrierID, CarrierID);
                    SetSVValue(eVID_NO.v_PortTransferState, ((int)(cCMS.eLPTransferState.ReadyToProcess)).ToString());
                    SendEvent_SamSung_ELEC(eCEID_NO.e_LP_CARRIERIDREAD, nLP);
                    break;
                default:
                    break;
            }
        }

        public void SendSlotMapToHost(int nLp, string SlotMap)
        {
            switch (m_eSecsGemSite) {
                case eSECSGEMSITE.SAMSUNG_EM:
                    m_cCMS[nLp].m_eSlotMapState = cCMS.eCarrierSlotMapState.WAITING_FOR_HOST;
                    m_cPJ.m_SlotMap = SlotMap;
                    SendEvent_SamSung_ELEC(eCEID_NO.e_LP_SLOTMAP_WAITINGFORHOST, nLp);
                    break;
                default:
                    break;
            }
        }

        public void SendSlotMapVerificationOK(int nLp)
        {
            m_cCMS[nLp - 1].m_eSlotMapState = cCMS.eCarrierSlotMapState.SLOTMAP_VERIFICATION_OK;
            SendEvent_SamSung_ELEC(eCEID_NO.e_LP_SLOTMAP_VERIFICATIONOK, nLp);
        }


        //public void SetLPStateReadyToUnLoad(int nLP)
        //{
        //    string sLP = "BP" + nLP.ToString();
        //    long ret = m_XGem.CMSSetReadyToUnload(sLP);
        //    if (ret == 0)
        //        WriteLog("LP State Change OK (Ready To Unload");
        //    else
        //        WriteLog("LP State Change Error(Ready To Unload) Error Code : " + ret);
        //}

        //public void LPPlaceSensorDetected(int nLP)
        //{
        //    string sLP = "BP" + nLP.ToString();
        //    long ret = m_XGem.CMSSetPIOSignalState(sLP, 7, 1);
        //    if (ret == 0)
        //        WriteLog("LP Placement Sensor Detected OK");
        //    else
        //        WriteLog("LP Placement Sensor Detected Error, Error Code : " + ret);
        //}

        //public void OHTLoadingComplete(int nLP)
        //{
        //    string sLP = "BP" + nLP.ToString();
        //    long ret = m_XGem.CMSSetCarrierOnOff(sLP, 1);       // state = 1(정상적 Docking) 0 (비정상)
        //    if (ret == 0)
        //        WriteLog("OHT Loading Complete");
        //    else
        //        WriteLog("OHT Loading Complete Error, Error Code : " + ret);
        //}

        //public void CarrierIDRead(int nLP, string sCarrierID)
        //{
        //    string sLP = "BP" + nLP.ToString();
        //    m_cCMS[nLP - 1].CarrierID = sCarrierID;
        //    long ret = m_XGem.CMSSetCarrierID(sLP, sCarrierID, 0);     // nResult 0 : Succeeded 1: Failed
        //    if (ret == 0)
        //        WriteLog("Carrier ID Read OK ");
        //    else
        //        WriteLog("Carrier ID Read Fail, Error Code : " + ret);
        //}

        public void SetLPAccessMode(int nLP, cCMS.ePortAccessMode AccessMode)
        {
            m_cCMS[nLP - 1].m_eAccessMode = AccessMode;
            if (IsOnlineRemote()) {
                SendEvent_SamSung_ELEC(eCEID_NO.e_LP_ACCESSMODECHANGE, nLP - 1);
            }
        }

        public bool IsOnlineRemote()
        {
            bool ret = (eControlState)m_nContState == eControlState.CONT_ONLINEREMOTE;
            return ret;
        }
        //public void MapDateVerification(int nLP, string SlotMapData, string sCarrierID)      //입력 SlotMap Exist:1 Empty:0 Double:D Closs:C
        //{
        //    string sLP = "BP" + nLP.ToString();
        //    string SendMapData = null;
        //    long nResult = 0;
        //    for (int i = 0; i < SlotMapData.Length; i++) {
        //        switch (SlotMapData.Substring(i, 1)) {
        //            case "0":
        //                SendMapData += "1";
        //                break;
        //            case "1":
        //                SendMapData += "3";
        //                break;
        //            case "D":
        //                SendMapData += "4";
        //                nResult = 1;
        //                break;
        //            case "C":
        //                SendMapData += "5";
        //                nResult = 1;
        //                break;
        //            default:
        //                WriteLog("It includes a character that is not confirmed in the SlotMap Input (Slot Map Data : " + SlotMapData + " )");
        //                return;
        //        }
        //    }
        //    long ret = m_XGem.CMSSetSlotMap(sLP, SendMapData, sCarrierID, nResult);
        //    if (ret == 0)
        //        WriteLog("SlotMap Send OK, SlotMapData : " + SendMapData);
        //    else
        //        WriteLog("Slot Map Send Fail, SlotMap Data : " + SendMapData + ",  Error Code : " + ret);
        //}
        public eCommunicateState GetCommunicationState()
        {
            return (eCommunicateState)m_nCommState;
        }

        public eControlState GetContolState()
        {
            return (eControlState)m_nContState;
        }

        public eEQPState_SSEM GetEQPState()
        {
            return (eEQPState_SSEM)m_eEQPSTATE;
        }

        public cCMS GetCMS(int nLP)
        {
            if (nLP < 0 || nLP > 2)
                return null;
            return m_cCMS[nLP - 1];
        }
        public cPJ GetPJ()
        {
            return m_cPJ;
        }

        public cCJ GetCJ()
        {
            return m_cCJ;
        }

        public void MapDataInfoRequest(string WaferID)
        {
            #region Item_Desciption
            /*   Set MID  : Material ID 
                 Set IDTYP : IDType (0:WaferID,1:CassetteID,2:RingFrame) 
                 Set MAPFT : It must be 1
                 Set FNLOC : flat/notch location in degrees clockwise from bottom, ex) 0, 90, 180, 270
                 Set FFROT : film frame location in degrees clockwise from bottom, ex) 0, 90, 180, 270  
                 Set ORLOC :  Origin 좌료의 위치  ( 0: center die of wafer , 1: upper right, 2: upper left, 3: lower left, 4: lower right)       
                 PRAXI Example:
                 서버로 부터“123456789ABCDEF”ROWCT=3 COLCT=5 메세지를 받았을때
                 PRAXI=0:             PRAXI=1:
                1  2  3  4  5      5  4  3  2  1
                6  7  8  9  A      A  9  8  7  6
                B  C  D  E  F      F  E  D  C  B
                PRAXI=2:            PRAXI=3:
                B  C  D  E  F      F  E  D  C  B
                6  7  8  9  A      A  9  8  7  6
                1  2  3  4  5      5  4  3  2  1
                PRAXI=4:            PRAXI=5:
                1  4  7  A  D      3  6  9  C  F
                2  5  8  B  E      2  5  8  B  E
                3  6  9  C  F      1  4  7  A  D
                PRAXI=6:            PRAXI=7:
                D  A  7  4  1      F  C  9  6  3
                E  B  8  5  2      E  B  8  5  2
                F  C  9  6  3      D  A  7  4  1
                 BCEQU : Bincode
                 NULBC : NullBinCode 표시할 글자 칩이 아닌부분을 X 로 채울것인지 스페이스로 채울것인지 등등에 관한                                                                   
                */

            #endregion

            long nObjID = 0;
            string bincode = "123456789D";
            m_XGem.MakeObject(ref nObjID);
            m_XGem.SetListItem(nObjID, 9);

            m_XGem.SetStringItem(nObjID, WaferID);                       //Set MID  : 
            m_XGem.SetBinaryItem(nObjID, 0);  //Set IDTYP :
            m_XGem.SetBinaryItem(nObjID, Convert.ToByte(1));          // Set MAPFT :
            m_XGem.SetUint4Item(nObjID, (ushort)0);                        //Set FNLOC :
            m_XGem.SetUint4Item(nObjID, (ushort)0);                        //Set FFROT :
            m_XGem.SetBinaryItem(nObjID, Convert.ToByte(2));             //Set ORLOC :
            m_XGem.SetBinaryItem(nObjID, Convert.ToByte(0));            //Set PRAXI 
            m_XGem.SetStringItem(nObjID, bincode);                       //Set BCEQU
            m_XGem.SetStringItem(nObjID, " ");                           //Set NULBC                                         
            long StreamNum = 12;
            long FuntionNum = 3;
            m_XGem.SendSECSMessage(nObjID, StreamNum, FuntionNum, 0);
        }
        public void RecieveMapInfo(long nObjectID)
        {
            m_bMapReq = true;
        }

        bool m_bMapReq = false;
        public void MapDataDownLoad(string WaferID)
        {
            long nObjID = 0;

            m_XGem.MakeObject(ref nObjID);
            m_XGem.SetListItem(nObjID, 2);
            m_XGem.SetStringItem(nObjID, WaferID);                       //Set MID   : WaferID
            m_XGem.SetBinaryItem(nObjID, 0);  //Set IDTYP : IDType (0:WaferID,1:CassetteID,2:RingFrame) 

            long StramNum = 12;
            long FuntionNum = 15;
            m_XGem.SendSECSMessage(nObjID, StramNum, FuntionNum, 0);
        }

        public void RecieveMapData(long nObjectID)
        {
            string sWAFERID = "";
            string sBinLT = "";
            byte[] temp = new byte[1000];
            int[] ntemp = new int[1000];
            m_XGem.GetStringItem(nObjectID, ref sWAFERID);
            m_XGem.GetBinaryItem(nObjectID, ref temp);
            m_XGem.GetInt4Item(nObjectID, ref ntemp);
            m_XGem.GetStringItem(nObjectID, ref sBinLT);

            string sFile = m_sMapDataPath;
            FileInfo fi = new FileInfo(sFile);
            string sSection = String.Format("MapData");
            IniFile.G_IniWriteValue(sSection, sWAFERID, sBinLT, sFile);
            WriteLog("Map Data Down ");
//            m_bMapDown = true;
//            m_bWaferIDRead = true;
        }

//        bool m_bMapDown = false;


        public void Rework(int nLP)
        {
            SendEvent_SamSung_ELEC(eCEID_NO.e_Rework, nLP);
        }

        public void Scrap(int nLP)
        {
            SendEvent_SamSung_ELEC(eCEID_NO.e_Scrap, nLP);
        }

        public void WaferStart(cCMS.eProductType WaferType, string WaferID, int Slot)
        {
            m_cWaferInfo.m_WaferID = WaferID;
            m_cWaferInfo.m_nSlotNum = Slot;
            m_cWaferInfo.m_eProductType = WaferType;

            switch (WaferType) {
                case cCMS.eProductType.WAFER:
                    m_cCMS[0].m_eProducType = cCMS.eProductType.WAFER;
                    m_cCMS[1].m_eProducType = cCMS.eProductType.WAFER;
                    SendEvent_SamSung_ELEC(eCEID_NO.e_WaferStarted, 1);
                    break;
            }
        }

        public void WaferEnd(cCMS.eProductType WaferType)
        {
            switch (WaferType) {
                case cCMS.eProductType.WAFER:
                    SendEvent_SamSung_ELEC(eCEID_NO.e_WaferCompleted, 1);
                    break;
            }
        }

        public void PRJobStateChage(cPJ.ePRJObState eState)
        {
            //switch (eState) {
            //    case cPJ.ePRJObState.Queued:
            //        SendEvent_SamSung_ELEC(eCEID_NO.e_PRJob_Queued);
            //        break;
            //    case cPJ.ePRJObState.Processing:
            //        SendEvent_SamSung_ELEC(eCEID_NO.e_PRJob_Processing);
            //        break;
            //    case cPJ.ePRJObState.WaitingForStart:
            //        SendEvent_SamSung_ELEC(eCEID_NO.e_PRJob_WaitSTart);
            //        break;
            //    case cPJ.ePRJObState.ProcessingComplete:
            //        SendEvent_SamSung_ELEC(eCEID_NO.e_PRJob_Completed);
            //        break;
            //}
        }

        public void CTRLJobStateChage(cCJ.eCTRLJobState eState)
        {
            //switch (eState) {
            //    case cCJ.eCTRLJobState.Queued:
            //        SendEvent_SamSung_ELEC(eCEID_NO.e_CTRLJob_Queued);
            //        break;
            //    case cCJ.eCTRLJobState.Excuting:
            //        SendEvent_SamSung_ELEC(eCEID_NO.e_CTRLJob_Excuting);
            //        break;
            //    case cCJ.eCTRLJobState.WaitingForStart:
            //        SendEvent_SamSung_ELEC(eCEID_NO.e_CTRLJob_WaitSTart);
            //        break;
            //    case cCJ.eCTRLJobState.Completed:
            //        SendEvent_SamSung_ELEC(eCEID_NO.e_CTRLJob_Completed);
            //        break;
            //}
        }

        public void SetParameters()
        {

        }

        public void PPIDChanged(ePPIDChangeMode eMode, string PPID, string RedLight, string BlueLight, string GreenLight, string CoaxLight, string SideLight, string AlignStep, string AlignRange, string OverlaySearchStart, string OverlaySearchLenght, string OverlayThreshold)
        {
            long nObjectID = 0;

            m_XGem.MakeObject(ref nObjectID);
            m_XGem.SetListItem(nObjectID, 4);
            m_XGem.SetStringItem(nObjectID, ((int)eMode).ToString());
            m_XGem.SetStringItem(nObjectID, m_EQPID);
            m_XGem.SetStringItem(nObjectID, PPID);
            if (eMode == ePPIDChangeMode.Delete) {
                m_XGem.SetListItem(nObjectID, 0);
            }
            else {

                m_XGem.SetListItem(nObjectID, 1);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, "");
                m_XGem.SetListItem(nObjectID, 10);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, "1");
                m_XGem.SetStringItem(nObjectID, RedLight);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, "2");
                m_XGem.SetStringItem(nObjectID, BlueLight);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, "3");
                m_XGem.SetStringItem(nObjectID, GreenLight);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, "4");
                m_XGem.SetStringItem(nObjectID, CoaxLight);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, "5");
                m_XGem.SetStringItem(nObjectID, SideLight);

                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, "6");
                m_XGem.SetStringItem(nObjectID, AlignStep);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, "7");
                m_XGem.SetStringItem(nObjectID, AlignRange);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, "8");
                m_XGem.SetStringItem(nObjectID, OverlaySearchStart);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, "9");
                m_XGem.SetStringItem(nObjectID, OverlaySearchLenght);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, "10");
                m_XGem.SetStringItem(nObjectID, OverlayThreshold);

            }
            m_XGem.SendSECSMessage(nObjectID, 7, 107, 0);
        }

        #endregion

        #region GemEvent
        public void OnGEMReqOnline(long nMsgId, long nFromState, long nToState)
        {
            WriteLog("Recieve Online Request");
            m_XGem.GEMRspOnline(nMsgId, 0);
        }

        private void OnGEMReqOffline(long nMsgId, long nFromState, long nToState)
        {
            SetOffline();
            WriteLog("Recieve Offline Request");
            //m_XGem.GEMRsqOffline(nMsgId, 0);
        }

        private void OnGEMCommStateChanged(long nState)
        {
            m_nCommState = nState;
            if ((eCommunicateState)m_nCommState != eCommunicateState.COMM_COMMUNICATING)
                m_nTOTSMP = 0;
            WriteLog("Communication State Change : " + ((eCommunicateState)m_nCommState).ToString());
        }

        private void OnGEMControlStateChanged(long nState)
        {
            m_nContState = nState;
            WriteLog("Control State Change : " + ((eControlState)m_nContState).ToString());
            if (m_eSecsGemSite == eSECSGEMSITE.SAMSUNG_EM) {
                switch ((eControlState)m_nContState) {
                    case eControlState.CONT_HOSTOFFLINE:
                        SetOffline();
                        break;
                    case eControlState.CONT_ONLINEREMOTE:
                        SetOnlineRemote();
                        break;
                }
            }
        }

        private void OnXGEMStateEvent(long nState)
        {
            switch ((eXGemModuleState)nState) {
                case eXGemModuleState.INIT:
                    break;
                case eXGemModuleState.IDLE:
                    break;
                case eXGemModuleState.SETUP:
                    break;
                case eXGemModuleState.READY:
                    break;
                case eXGemModuleState.EXCUTE:
                    m_XGem.CMSSetLPInfo("BP1", 2, 0, 0, 0, "");
                    m_XGem.CMSSetLPInfo("BP2", 2, 0, 0, 0, "");
                    m_XGem.GEMSetEstablish(1);
                    GetParameters();
                    SetSoftwareVersion();
                    //m_XGem.GEMReqRemote();
                    break;
            }
        }

        private void OnGEMTerminalMessageRecieve(long nTid, string sMsg)
        {
            OnRecieveTerminalMsg(sMsg);
        }

        private void OnGEMTerminalMessageMultiRecieve(long nTid, long nCount, string[] sMsg)
        {
            OnRecieveTerminalMultiMsg(sMsg);
        }

        private void OnGEMReqDateTime(long nMsgld, string sSystemTime)
        {
            //KJWKJW 시간 설정하기
            long ret = 1;       // ret 1 = Error 상태  0 : 정상
            if (sSystemTime.Length == 14) {
                SYSTEMTIME Systime = new SYSTEMTIME();
                Systime.wYear = Convert.ToUInt16(sSystemTime.Substring(0, 4));
                Systime.wDayOfWeek = 4;
                Systime.wMonth = Convert.ToUInt16(sSystemTime.Substring(4, 2));
                Systime.wDay = Convert.ToUInt16(sSystemTime.Substring(6, 2));
                Systime.wHour = Convert.ToUInt16(sSystemTime.Substring(8, 2));
                Systime.wMinute = Convert.ToUInt16(sSystemTime.Substring(10, 2));
                Systime.wSecond = Convert.ToUInt16(sSystemTime.Substring(12, 2));
                int i = SetSystemTime(ref Systime);
                ret = 0;
            }
            m_XGem.GEMRspDateTime(nMsgld, ret);
        }

        private void OnGemReqGetDateTime(long nMsgld)
        {
            string time = System.DateTime.Now.ToString("yyyyMMddhhmmss");
            m_XGem.GEMRspGetDateTime(nMsgld, time);
        }

        private void OnSECSMessageReceived(long nObjectID, long nStream, long nFunction, long nSysbyte)
        {
            int nError = 0;
            WriteLog("On SecsMessage Receive S:" + nStream.ToString() + "F:" + nFunction.ToString());

            if (((eControlState)m_nContState == eControlState.CONT_HOSTOFFLINE) && !((nStream == 1 && nFunction == 13) || (nStream == 1 && nFunction == 17))) {
                long nObjectId = 0;
                m_XGem.MakeObject(ref nObjectId);
                m_XGem.SetListItem(nObjectId, 0);
                m_XGem.SendSECSMessage(nObjectId, nStream, 0, 0);
                return;
            }
            switch (nStream) {
                case 1:
                    switch (nFunction) {
                        case 3:
                            VID_Request(nObjectID, nSysbyte);
                            break;
                        case 5:
                            string SFCD = "";
                            m_XGem.GetStringItem(nObjectID, ref SFCD);
                            if (SFCD == "1")
                                SendEQPState_SSEM(nSysbyte);
                            //KJW SFCD == 1이 아닐때 처리해야됨
                            break;
                        case 11:
                            VIDInfo_Request(nObjectID, nSysbyte);
                            break;
                        case 15:
                            if ((eControlState)m_nContState == eControlState.CONT_HOSTOFFLINE) {
                                long nObjectId = 0;
                                m_XGem.MakeObject(ref nObjectId);
                                m_XGem.SetStringItem(nObjectId, "2");
                                m_XGem.SendSECSMessage(nObjectId, 1, nFunction + 1, nSysbyte);
                            }
                            SetHostOffline();
                            SendRspControlStateChange(15, nSysbyte);
                            break;
                        case 17:
                            if ((eControlState)m_nContState == eControlState.CONT_ONLINEREMOTE) {
                                long nObjectId = 0;
                                m_XGem.MakeObject(ref nObjectId);
                                m_XGem.SetStringItem(nObjectId, "2");
                                m_XGem.SendSECSMessage(nObjectId, 1, nFunction + 1, nSysbyte);
                            }
                            else {
                                SetHostOnlineRemote();
                                SendRspControlStateChange(17, nSysbyte);
                            }
                            break;
                        default:
                            nError = 1;
                            break;
                    }
                    break;
                case 2:
                    switch (nFunction) {
                        case 13:
                            ECID_Request(nObjectID, nSysbyte);
                            break;
                        case 15:
                            SendNack(nSysbyte);
                            break;
                        case 23:
                            RecieveTraceInit(nObjectID, nSysbyte);
                            break;
                        case 29:
                            ECID_ConstantRequest(nObjectID, nSysbyte);
                            break;
                        case 31:
                            ChangeTime(nObjectID, nSysbyte);
                            break;
                        case 41:
                            RecieveRemoteCommand(nObjectID, nSysbyte);
                            break;
                        default:
                            nError = 1;
                            break;
                    }
                    break;
                case 3:
                    switch (nFunction) {
                        case 17:
                            RecieveCarrierAction(nObjectID, nSysbyte);
                            break;
                        default:
                            nError = 1;
                            break;
                    }
                    break;
                case 5:
                    switch (nFunction) {
                        case 3:
                            RecieveAlramEnable(nObjectID, nSysbyte);
                            break;
                        case 5:
                            RecieveAlramReq(nObjectID, nSysbyte);
                            break;
                    }
                    break;
                case 6:
                    switch (nFunction) {
                        case 12:
                            string ACKC6 = "";
                            m_XGem.GetStringItem(nObjectID, ref ACKC6);
                            RecieveEvent_SAMSUNG_ELEC(ACKC6);
                            break;
                        case 15:
                            string nCEID = "";
                            int n = -1;
                            m_XGem.GetStringItem(nObjectID, ref nCEID);
                            for (int i = 0; i < Enum.GetNames(typeof(eCEID_NO)).Length; i++) {
                                if (m_nCEID[i] == Convert.ToInt32(nCEID)) {
                                    n = i;
                                    break;
                                }
                            }
                            if (n != -1)
                                SendEvent_SamSung_ELEC((eCEID_NO)n, m_nWorkLP, m_cCMS[m_nWorkLP].CarrierID, m_cCMS[m_nWorkLP].LotID, m_cPJ.m_SlotMap, nSysbyte);
                            else {
                                long nOBJ = 0;
                                m_XGem.MakeObject(ref nOBJ);
                                m_XGem.SetListItem(nOBJ, 0);
                                m_XGem.SendSECSMessage(nOBJ, 6, 16, nSysbyte);
                            }
                            break;
                        default:
                            nError = 1;
                            break;
                    }
                    break;
                case 7:
                    switch (nFunction) {
                        case 23:
                            RecieveRecipeChange(nObjectID, nSysbyte);
                            break;
                        case 25:
                            long temp = 0;
                            string sPPID = "";
                            m_XGem.GetListItem(nObjectID, ref temp);
                            m_XGem.GetStringItem(nObjectID, ref sPPID);
                            RecievePPFormatRequest(sPPID, nSysbyte);
                            break;
                        case 107:
                            break;
                        default:
                            nError = 1;
                            break;
                    }
                    break;
                case 12:
                    switch (nFunction) {
                        case 4:
                            RecieveMapInfo(nObjectID);
                            break;
                        case 16:
                            RecieveMapData(nObjectID);
                            break;
                    }
                    break;
                case 14:
                    switch (nFunction) {
                        case 9:
                            CTRLJobCreate(nObjectID, nSysbyte);
                            break;
                        default:
                            nError = 1;
                            break;

                    }
                    break;
                case 16:
                    switch (nFunction) {
                        case 5:
                            PRJobCommandRecived(nObjectID, nSysbyte);
                            break;
                        case 11:
                            //PRJOBCreate(nObjectID, nSysbyte);
                            break;
                        case 15:
                            PRJOBCreate(nObjectID, nSysbyte);
                            break;
                        case 17:
                            PRJobDequeue(nObjectID, nSysbyte);
                            break;
                        case 27:
                            CTRLJobCommandRecieve(nObjectID, nSysbyte);
                            break;
                        default:
                            nError = 1;
                            break;
                    }
                    break;
                default:
                    nError = 2;
                    break;
            }
            if (nError != 0)
                WriteLog("Not Define Message (S" + nStream.ToString() + "F" + nFunction.ToString() + ")");
        }


        int[] nALEnable = new int[35] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };


        private void RecieveAlramReq(long nObjectID, long nSysbyte)
        {
            long nAckObjectID = 0;
            long nItemIndex = 0;
            string sTemp = "";
            bool nNotDefine = false;

            m_XGem.GetListItem(nObjectID, ref nItemIndex);
            m_XGem.MakeObject(ref nAckObjectID);
            if (nItemIndex == 0) {
                m_XGem.SetListItem(nAckObjectID, 35);
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1000");
                m_XGem.SetStringItem(nAckObjectID, "Emergency_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1001");
                m_XGem.SetStringItem(nAckObjectID, "CDA_Low_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1002");
                m_XGem.SetStringItem(nAckObjectID, "GN2_Low_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1003");
                m_XGem.SetStringItem(nAckObjectID, "Interlock_Key_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1004");
                m_XGem.SetStringItem(nAckObjectID, "MC_Reset_Signal_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1005");
                m_XGem.SetStringItem(nAckObjectID, "Door_Lock_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1006");
                m_XGem.SetStringItem(nAckObjectID, "Motor_Stop_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1007");
                m_XGem.SetStringItem(nAckObjectID, "Light_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1008");
                m_XGem.SetStringItem(nAckObjectID, "TCP_Disconnect_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1009");
                m_XGem.SetStringItem(nAckObjectID, "Ionizer_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1010");
                m_XGem.SetStringItem(nAckObjectID, "Vision_Ready_Signal_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1011");
                m_XGem.SetStringItem(nAckObjectID, "FDC_Temperature_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "FDC_Presure_Error");
                m_XGem.SetStringItem(nAckObjectID, "GN2_Low_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1013");
                m_XGem.SetStringItem(nAckObjectID, "FDC_Vacuum_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1014");
                m_XGem.SetStringItem(nAckObjectID, "FDC_FFU_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1015");
                m_XGem.SetStringItem(nAckObjectID, "FDC_Electrostatic_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1016");
                m_XGem.SetStringItem(nAckObjectID, "FDC_Illumination_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1017");
                m_XGem.SetStringItem(nAckObjectID, "FDC_EcoPower_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1018");
                m_XGem.SetStringItem(nAckObjectID, "Barcode_Read_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1019");
                m_XGem.SetStringItem(nAckObjectID, "TCP_Disconnect_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1020");
                m_XGem.SetStringItem(nAckObjectID, "WTR_Communication_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1021");
                m_XGem.SetStringItem(nAckObjectID, "LoadPort_Communication_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1022");
                m_XGem.SetStringItem(nAckObjectID, "Aligner_Communication_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1023");
                m_XGem.SetStringItem(nAckObjectID, "WTR_No_Wafer_Before_Get_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1024");
                m_XGem.SetStringItem(nAckObjectID, "WTR_No_Wafer_After_Get_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1025");
                m_XGem.SetStringItem(nAckObjectID, "WTR_No_Wafer_During_Get_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1026");
                m_XGem.SetStringItem(nAckObjectID, "WTR_No_Wafer_Before_Put_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1027");
                m_XGem.SetStringItem(nAckObjectID, "WTR_No_Wafer_After_Put_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1028");
                m_XGem.SetStringItem(nAckObjectID, "WTR_No_Wafer_During_Put_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1029");
                m_XGem.SetStringItem(nAckObjectID, "LoadPort_Docking_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1030");
                m_XGem.SetStringItem(nAckObjectID, "LoadPort_Door_Open_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1031");
                m_XGem.SetStringItem(nAckObjectID, "LoadPort_Door_Close_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1032");
                m_XGem.SetStringItem(nAckObjectID, "LoadPort_Door_Interlock_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1033");
                m_XGem.SetStringItem(nAckObjectID, "Aligner_Alignment_Error");
                m_XGem.SetListItem(nAckObjectID, 3);
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetStringItem(nAckObjectID, "1034");
                m_XGem.SetStringItem(nAckObjectID, "Aligner_No_Wafer_Error");

            }
            else {
                m_XGem.SetListItem(nAckObjectID, nItemIndex);
                for (int i = 0; i < nItemIndex; i++) {
                    m_XGem.GetStringItem(nObjectID, ref sTemp);
                    switch (Convert.ToInt16(sTemp)) {
                        case 1000:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1000");
                            m_XGem.SetStringItem(nAckObjectID, "Emergency_Error");
                            break;
                        case 1001:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1001");
                            m_XGem.SetStringItem(nAckObjectID, "CDA_Low_Error");
                            break;
                        case 1002:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1002");
                            m_XGem.SetStringItem(nAckObjectID, "GN2_Low_Error");
                            break;
                        case 1003:
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1003");
                            m_XGem.SetStringItem(nAckObjectID, "Interlock_Key_Error");
                            m_XGem.SetListItem(nAckObjectID, 3);
                            break;
                        case 1004:
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1004");
                            m_XGem.SetStringItem(nAckObjectID, "MC_Reset_Signal_Error");
                            break;
                        case 1005:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1005");
                            m_XGem.SetStringItem(nAckObjectID, "Door_Lock_Error");
                            break;
                        case 1006:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1006");
                            m_XGem.SetStringItem(nAckObjectID, "Motor_Stop_Error");
                            break;
                        case 1007:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1007");
                            m_XGem.SetStringItem(nAckObjectID, "Light_Error");
                            break;
                        case 1008:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1008");
                            m_XGem.SetStringItem(nAckObjectID, "TCP_Disconnect_Error");
                            break;
                        case 1009:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1009");
                            m_XGem.SetStringItem(nAckObjectID, "Ionizer_Error");
                            break;
                        case 1010:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1010");
                            m_XGem.SetStringItem(nAckObjectID, "Vision_Ready_Signal_Error");
                            break;
                        case 1011:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1011");
                            m_XGem.SetStringItem(nAckObjectID, "FDC_Temperature_Error");
                            break;
                        case 1012:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "FDC_Presure_Error");
                            m_XGem.SetStringItem(nAckObjectID, "GN2_Low_Error");
                            break;
                        case 1013:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1013");
                            m_XGem.SetStringItem(nAckObjectID, "FDC_Vacuum_Error");
                            break;
                        case 1014:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1014");
                            m_XGem.SetStringItem(nAckObjectID, "FDC_FFU_Error");
                            break;
                        case 1015:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1015");
                            m_XGem.SetStringItem(nAckObjectID, "FDC_Electrostatic_Error");
                            break;
                        case 1016:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1016");
                            m_XGem.SetStringItem(nAckObjectID, "FDC_Illumination_Error");
                            break;
                        case 1017:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1017");
                            m_XGem.SetStringItem(nAckObjectID, "FDC_EcoPower_Error");
                            break;
                        case 1018:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1018");
                            m_XGem.SetStringItem(nAckObjectID, "Barcode_Read_Error"); break;
                        case 1019:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1019");
                            m_XGem.SetStringItem(nAckObjectID, "TCP_Disconnect_Error2"); break;
                        case 1020:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1020");
                            m_XGem.SetStringItem(nAckObjectID, "WTR_Communication_Error"); break;
                        case 1021:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1021");
                            m_XGem.SetStringItem(nAckObjectID, "LoadPort_Communication_Error"); break;
                        case 1022:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1022");
                            m_XGem.SetStringItem(nAckObjectID, "Aligner_Communication_Error"); break;
                        case 1023:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1023");
                            m_XGem.SetStringItem(nAckObjectID, "WTR_No_Wafer_Before_Get_Error"); break;
                        case 1024:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1024");
                            m_XGem.SetStringItem(nAckObjectID, "WTR_No_Wafer_After_Get_Error"); break;
                        case 1025:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1025");
                            m_XGem.SetStringItem(nAckObjectID, "WTR_No_Wafer_During_Get_Error"); break;
                        case 1026:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1026");
                            m_XGem.SetStringItem(nAckObjectID, "WTR_No_Wafer_Before_Put_Error"); break;
                        case 1027:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1027");
                            m_XGem.SetStringItem(nAckObjectID, "WTR_No_Wafer_After_Put_Error"); break;
                        case 1028:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1028");
                            m_XGem.SetStringItem(nAckObjectID, "WTR_No_Wafer_During_Put_Error"); break;
                        case 1029:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1029");
                            m_XGem.SetStringItem(nAckObjectID, "LoadPort_Docking_Error"); break;
                        case 1030:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1030");
                            m_XGem.SetStringItem(nAckObjectID, "LoadPort_Door_Open_Error");
                            break;
                        case 1031:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1031");
                            m_XGem.SetStringItem(nAckObjectID, "LoadPort_Door_Close_Error");
                            break;
                        case 1032:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1032");
                            m_XGem.SetStringItem(nAckObjectID, "LoadPort_Door_Interlock_Error");
                            break;
                        case 1033:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1033");
                            m_XGem.SetStringItem(nAckObjectID, "Aligner_Alignment_Error");
                            break;
                        case 1034:
                            m_XGem.SetListItem(nAckObjectID, 3);
                            m_XGem.SetStringItem(nAckObjectID, "1");
                            m_XGem.SetStringItem(nAckObjectID, "1034");
                            m_XGem.SetStringItem(nAckObjectID, "Aligner_No_Wafer_Error");
                            break;
                        default:
                            m_XGem.CloseObject(nAckObjectID);
                            m_XGem.MakeObject(ref nAckObjectID);
                            m_XGem.SetListItem(nAckObjectID, 0);
                            nNotDefine = true;
                            break;
                    }
                    if (nNotDefine)
                        break;
                }
            }
            m_XGem.SendSECSMessage(nAckObjectID, 5, 6, nSysbyte);
        }

        private void RecieveAlramEnable(long nObjectID, long nSysbyte)
        {
            string sID = "";
            string sEnable = "";
            long nTemp = 0;
            int nAck = 0;
            m_XGem.GetListItem(nObjectID, ref nTemp);
            m_XGem.GetStringItem(nObjectID, ref sEnable);
            m_XGem.GetStringItem(nObjectID, ref sID);

            if (sID == "") {
                for (int i = 0; i < 18; i++) {
                    nALEnable[i] = Convert.ToInt32(sEnable);
                }
            }
            else {
                if (Convert.ToInt32(sID) - 1000 < 0 || Convert.ToInt32(sID) - 1000 > 17) {
                    nAck = 1;
                }
                else {
                    nALEnable[Convert.ToInt32(sID) - 1000] = Convert.ToInt32(sEnable);
                }
            }
            long nAckObject = 0;
            m_XGem.MakeObject(ref nAckObject);
            m_XGem.SetStringItem(nAckObject, nAck.ToString());
            m_XGem.SendSECSMessage(nAckObject, 5, 4, nSysbyte);
        }

        private void VID_Request(long nObjecID, long nSysbyte)
        {
            long nItemCount = 0;
            long nAckObject = 0;
            string sTemp = "";
            long[] nVID;
            string[] sValue;
            m_XGem.MakeObject(ref nAckObject);

            m_XGem.GetListItem(nObjecID, ref nItemCount);
            if (nItemCount == 0) {
                m_XGem.SetListItem(nAckObject, Enum.GetNames(typeof(eVID_NO)).Length);
                for (int i = 0; i < Enum.GetNames(typeof(eVID_NO)).Length; i++) {
                    m_XGem.SetStringItem(nAckObject, GetSVValue((eVID_NO)i));
                }
            }
            else {
                m_XGem.SetListItem(nAckObject, nItemCount);
                nVID = new long[nItemCount];
                sValue = new string[nItemCount];
                for (int i = 0; i < nItemCount; i++) {
                    m_XGem.GetStringItem(nObjecID, ref sTemp);
                    nVID[i] = Convert.ToInt32(sTemp);
                }
                m_XGem.GEMGetVariable(nItemCount, ref nVID, ref sValue);
                if (sValue[0] == null) {
                    m_XGem.CloseObject(nAckObject);
                    m_XGem.MakeObject(ref nAckObject);
                    m_XGem.SetListItem(nAckObject, 0);
                }
                else {
                    for (int i = 0; i < nItemCount; i++) {
                        m_XGem.SetStringItem(nAckObject, sValue[i]);
                    }
                }
            }
            m_XGem.SendSECSMessage(nAckObject, 1, 4, nSysbyte);
        }
        private string GetVIDName(long VID)
        {
            int index = -1;
            for (int i = 0; i < Enum.GetNames(typeof(eVID_NO)).Length; i++) {
                if (VID == (int)m_nVID[i])
                    index = i;
            }
            if (index != -1)
                return Enum.GetName(typeof(eVID_NO), index).Replace("v_", "");
            else
                return "";
        }

        private void VIDInfo_Request(long nObjecID, long nSysbyte)
        {
            {
                long nItemCount = 0;
                long nAckObject = 0;
                string sTemp = "";
                long[] nVID;
                string[] sValue;
                m_XGem.MakeObject(ref nAckObject);
                bool nFail = false;

                m_XGem.GetListItem(nObjecID, ref nItemCount);
                if (nItemCount == 0) {
                    m_XGem.SetListItem(nAckObject, Enum.GetNames(typeof(eVID_NO)).Length);
                    nVID = new long[Enum.GetNames(typeof(eVID_NO)).Length];
                    sValue = new string[Enum.GetNames(typeof(eVID_NO)).Length];
                    for (int i = 0; i < Enum.GetNames(typeof(eVID_NO)).Length; i++) {
                        m_XGem.SetListItem(nAckObject, 3);
                        m_XGem.SetStringItem(nAckObject, m_nVID[i].ToString());
                        sTemp = GetVIDName(m_nVID[i]);
                        if (sTemp == "") {
                            nFail = true;
                        }
                        else {
                            m_XGem.SetStringItem(nAckObject, sTemp);
                        }
                        m_XGem.SetStringItem(nAckObject, "");
                    }
                }
                else {
                    m_XGem.SetListItem(nAckObject, nItemCount);
                    nVID = new long[nItemCount];
                    sValue = new string[nItemCount];
                    for (int i = 0; i < nItemCount; i++) {
                        m_XGem.SetListItem(nAckObject, 3);
                        m_XGem.GetStringItem(nObjecID, ref sTemp);
                        nVID[i] = Convert.ToInt32(sTemp);
                        m_XGem.SetStringItem(nAckObject, nVID[i].ToString());
                        sTemp = GetVIDName(nVID[i]);
                        if (sTemp == "") {
                            nFail = true;
                        }
                        else {
                            m_XGem.SetStringItem(nAckObject, sTemp);
                        }
                        m_XGem.SetStringItem(nAckObject, "");
                    }
                }
                if (nFail) {
                    m_XGem.CloseObject(nAckObject);
                    m_XGem.MakeObject(ref nAckObject);
                    m_XGem.SetListItem(nAckObject, 0);
                    m_XGem.SendSECSMessage(nAckObject, 1, 12, nSysbyte);
                }
                else
                    m_XGem.SendSECSMessage(nAckObject, 1, 12, nSysbyte);
            }

        }
        private void SetHostOffline()
        {
            SetSVValue(eVID_NO.v_CRST, ((int)eCRST_SSEM.Offline).ToString());
            SendEvent_SamSung_ELEC(eCEID_NO.e_OFFLINE);
            m_XGem.GEMReqHostOffline();
            WriteLog("Set Host Offline");
        }
        private void SendRspControlStateChange(long nFunction, long nSysbyte)
        {
            long nObjectId = 0;
            m_XGem.MakeObject(ref nObjectId);
            m_XGem.SetStringItem(nObjectId, "0");
            m_XGem.SendSECSMessage(nObjectId, 1, nFunction + 1, nSysbyte);
        }
        private void SetHostOnlineRemote()
        {
            SetSVValue(eVID_NO.v_CRST, ((int)eCRST_SSEM.Remote).ToString());
            SendEvent_SamSung_ELEC(eCEID_NO.e_ONLINEREMOTE);
            m_XGem.GEMReqRemote();
            WriteLog("Set Host OnlineRemote");
        }
        //private void RecieveRecipeChange(long nObjectId, long nSysbyte)
        //{
        //    long ntemp = 0;
        //        string stemp = "";
        //        long nItemIndex = 0;
        //        string sRecipeID = "";

        //        ePPIDChangeMode eMode = ePPIDChangeMode.Create;
        //        int Index = 0;
        //        string[] sParam = new string[3] { "0", "0", "0" };
        //        string nAck = "0";

        //        m_XGem.GetListItem(nObjectId, ref ntemp);
        //        m_XGem.GetStringItem(nObjectId, ref sRecipeID);
        //        m_XGem.GetStringItem(nObjectId, ref stemp);
        //        m_XGem.GetStringItem(nObjectId, ref stemp);
        //        m_XGem.GetListItem(nObjectId, ref ntemp);
        //        m_XGem.GetListItem(nObjectId, ref ntemp);
        //        m_XGem.GetStringItem(nObjectId, ref stemp);
        //        m_XGem.GetListItem(nObjectId, ref nItemIndex);
        //        if (sRecipeID.IndexOf(" ") >= 0)
        //            sRecipeID = sRecipeID.Substring(0, sRecipeID.IndexOf(""));
        //        string[] sName = new string[nItemIndex];
        //        string[] sValue = new string[nItemIndex];
        //        for (int i = 0; i < nItemIndex; i++) {
        //            m_XGem.GetListItem(nObjectId,ref ntemp);
        //            m_XGem.GetStringItem(nObjectId,ref sName[i]);
        //            if (sName[i] != "OCRUSE" && sName[i] != "BCRUSE" && sName[i] != "NotchPos")
        //                nAck = "7";
        //            m_XGem.GetStringItem(nObjectId,ref sValue[i]);
        //        }
        //        if(nAck == "0")
        //            OnRecipeChanged(sRecipeID, sName, sValue);

        //        long nObjectID2 = 0;
        //        m_XGem.MakeObject(ref nObjectID2);
        //        m_XGem.SetStringItem(nObjectID2, nAck);
        //        m_XGem.SendSECSMessage(nObjectID2, 7, 24, nSysbyte);

        //}
        private void RecieveRecipeChange(long nObjectId, long nSysbyte)
        {

            long ntemp = 0;
            string stemp = "";
            long nItemIndex = 0;
            string sRecipeID = "";
            string sParam_Name = "";
            string sParam_Value = "";
//            string sValue;
            ePPIDChangeMode eMode = ePPIDChangeMode.Create;
            int Index = 0;
            string[] sParam = new string[10] { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" };
            string nAck = "0";

            m_XGem.GetListItem(nObjectId, ref ntemp);
            m_XGem.GetStringItem(nObjectId, ref sRecipeID);
            m_XGem.GetStringItem(nObjectId, ref stemp);
            m_XGem.GetStringItem(nObjectId, ref stemp);
            m_XGem.GetListItem(nObjectId, ref ntemp);
            m_XGem.GetListItem(nObjectId, ref ntemp);
            m_XGem.GetStringItem(nObjectId, ref stemp);
            m_XGem.GetListItem(nObjectId, ref nItemIndex);
            if (sRecipeID.IndexOf(" ") >= 0)
                sRecipeID = sRecipeID.Substring(0, sRecipeID.IndexOf(""));

            for (int i = 0; i < m_cRecipeList.nRecipeNum; i++) {
                if (m_cRecipeList.sName[i] == sRecipeID) {
                    eMode = ePPIDChangeMode.Modify;
                    Index = i;
                }
            }

            for (int i = 0; i < nItemIndex; i++) {
                m_XGem.GetListItem(nObjectId, ref ntemp);
                m_XGem.GetStringItem(nObjectId, ref sParam_Name);
                if (sParam_Name.IndexOf(" ") >= 0)
                    sParam_Name = sParam_Name.Substring(0, sParam_Name.IndexOf(""));
                m_XGem.GetStringItem(nObjectId, ref sParam_Value);
                if (sParam_Value.IndexOf(" ") >= 0)
                    sParam_Value = sParam_Value.Substring(0, sParam_Value.IndexOf(""));

                switch (sParam_Name) {
                    case "1":
                        if (eMode == ePPIDChangeMode.Modify) {
                            OnChangeList(Index, 1, sParam_Value);
                            //lvRecipe.Items[Index].SubItems[1].Text = sParam_Value;
                        }
                        else {
                            sParam[0] = sParam_Value;
                        }
                        break;
                    case "2":
                        if (eMode == ePPIDChangeMode.Modify) {
                            OnChangeList(Index, 2, sParam_Value);
                            //lvRecipe.Items[Index].SubItems[2].Text = sParam_Value;
                        }
                        else {
                            sParam[1] = sParam_Value;
                        }
                        break;
                    case "3":
                        if (eMode == ePPIDChangeMode.Modify) {
                            OnChangeList(Index, 3, sParam_Value);
                            //lvRecipe.Items[Index].SubItems[3].Text = sParam_Value;
                        }
                        else {
                            sParam[2] = sParam_Value;
                        }
                        break;
                    case "4":
                        if (eMode == ePPIDChangeMode.Modify) {
                            OnChangeList(Index, 4, sParam_Value);
                            //lvRecipe.Items[Index].SubItems[4].Text = sParam_Value;
                        }
                        else {
                            sParam[3] = sParam_Value;
                        }
                        break;
                    case "5":
                        if (eMode == ePPIDChangeMode.Modify) {
                            OnChangeList(Index, 5, sParam_Value);
                            //lvRecipe.Items[Index].SubItems[5].Text = sParam_Value;
                        }
                        else {
                            sParam[4] = sParam_Value;
                        }
                        break;
                    case "6":
                        if (eMode == ePPIDChangeMode.Modify) {
                            OnChangeList(Index, 6, sParam_Value);
                            // lvRecipe.Items[Index].SubItems[6].Text = sParam_Value;
                        }
                        else {
                            sParam[5] = sParam_Value;
                        }
                        break;
                    case "7":
                        if (eMode == ePPIDChangeMode.Modify) {
                            OnChangeList(Index, 7, sParam_Value);
                            //lvRecipe.Items[Index].SubItems[7].Text = sParam_Value;
                        }
                        else {
                            sParam[6] = sParam_Value;
                        }
                        break;
                    case "8":
                        if (eMode == ePPIDChangeMode.Modify) {
                            OnChangeList(Index, 8, sParam_Value);
                            //lvRecipe.Items[Index].SubItems[8].Text = sParam_Value;
                        }
                        else {
                            sParam[7] = sParam_Value;
                        }
                        break;
                    case "9":
                        if (eMode == ePPIDChangeMode.Modify) {
                            OnChangeList(Index, 9, sParam_Value);
                            //lvRecipe.Items[Index].SubItems[9].Text = sParam_Value;
                        }
                        else {
                            sParam[8] = sParam_Value;
                        }
                        break;
                    case "10":
                        if (eMode == ePPIDChangeMode.Modify) {
                            OnChangeList(Index, 10, sParam_Value);
                            //lvRecipe.Items[Index].SubItems[10].Text = sParam_Value;
                        }
                        else {
                            sParam[9] = sParam_Value;
                        }
                        break;
                    default:
                        nAck = "1";
                        WriteLog("Parameter Error : " + sParam_Name);
                        break;
                }
            }
            if (eMode == ePPIDChangeMode.Create && nAck == "0") {
                string[] ItemList = new string[4];
                ItemList[0] = sRecipeID;
                ItemList[1] = sParam[0];
                ItemList[2] = sParam[1];
                ItemList[3] = sParam[2];
                ItemList[4] = sParam[3];
                ItemList[5] = sParam[4];
                ItemList[6] = sParam[5];
                ItemList[7] = sParam[6];
                ItemList[8] = sParam[7];
                ItemList[9] = sParam[8];
                ItemList[10] = sParam[9];
                ListViewItem item = new ListViewItem(ItemList);
                OnMakeList(item);

                //lvRecipe.Items.Add(item);
            }
            SaveRecipe();
            LoadRecipe();
            long nObjectID2 = 0;
            m_XGem.MakeObject(ref nObjectID2);
            m_XGem.SetStringItem(nObjectID2, nAck);
            m_XGem.SendSECSMessage(nObjectID2, 7, 24, nSysbyte);

        }
        #endregion

        private void WriteLog(String sLog)
        {
            //string LogFileName = "XGem300";
            //String sLogPathDate = DateTime.Now.ToShortDateString() + @"\";
            //String sTimeLog = DateTime.Now.ToString("HH:mm:ss") + ":" + DateTime.Now.Millisecond.ToString("000") + " - " + sLog;
            //string sLogFileName = LogFileName;

            //String FullPath = m_sLogPath + sLogPathDate;
            //DirectoryInfo di = new DirectoryInfo(FullPath);
            //if (di.Exists == false) {
            //    di.Create();
            //}

            //FullPath += sLogFileName;
            //StreamWriter sw = new StreamWriter(FullPath, true); // append

            //sw.WriteLine(sTimeLog);
            //OnRecieveLogEvent(sTimeLog);
            //sw.Close();
        }

        private void RecieveCarrierAction(long nObjectID, long nSysbyte)
        {
            long ItemCount = 0;
            int nAck = 0;

            switch (m_eSecsGemSite) {
                case eSECSGEMSITE.SAMSUNG_EM:
                    m_XGem.GetListItem(nObjectID, ref ItemCount);
                    string sDataID = "", sCarrierAction = "", sCarrierID = "", sLotID = "", sPTN = "";
                    m_XGem.GetStringItem(nObjectID, ref sDataID);
                    m_XGem.GetStringItem(nObjectID, ref sCarrierAction);
                    m_XGem.GetStringItem(nObjectID, ref sCarrierID);
                    m_XGem.GetStringItem(nObjectID, ref sLotID);
                    m_XGem.GetStringItem(nObjectID, ref sPTN);
                    if (sCarrierAction.IndexOf(" ") >= 0)
                        sCarrierAction = sCarrierAction.Substring(0, sCarrierAction.IndexOf(" "));
                    switch (sCarrierAction) {
                        case "CancelCarrierAtPort":
                            //if (m_clpstate[0].m_ecarrieridstate == clpstate.ecarrieridstate.waiting_for_host || m_clpstate[0].m_ecarrieridstate == clpstate.ecarrieridstate.id_verification_ok)
                            //{
                            //    m_clpstate[0].m_ecarrieridstate = clpstate.ecarrieridstate.id_verification_failed;
                            //}
                            //else if (m_clpstate[1].m_ecarrieridstate == clpstate.ecarrieridstate.waiting_for_host || m_clpstate[1].m_ecarrieridstate == clpstate.ecarrieridstate.id_verification_ok)
                            //{
                            //    m_clpstate[1].m_ecarrieridstate = clpstate.ecarrieridstate.id_verification_failed;
                            //}
                            //else if (m_clpstate[0].m_eslotmapstate == clpstate.ecarrierslotmapstate.waiting_for_host || m_clpstate[0].m_eslotmapstate == clpstate.ecarrierslotmapstate.slotmap_verification_ok)
                            //{
                            //    m_clpstate[0].m_eslotmapstate = clpstate.ecarrierslotmapstate.slotmap_verification_failed;
                            //}
                            //else if (m_clpstate[1].m_eslotmapstate == clpstate.ecarrierslotmapstate.waiting_for_host || m_clpstate[1].m_eslotmapstate == clpstate.ecarrierslotmapstate.slotmap_verification_ok)
                            //{
                            //    m_clpstate[1].m_eslotmapstate = clpstate.ecarrierslotmapstate.slotmap_verification_failed;
                            //}

                            break;
                        case "ProceedWithCarrierID":
                            //if (m_clpstate[0].m_ecarrieridstate == clpstate.ecarrieridstate.waiting_for_host)
                            //{
                            //    m_clpstate[0].m_ecarrieridstate = clpstate.ecarrieridstate.id_verification_ok;
                            //}
                            //else if (m_clpstate[1].m_ecarrieridstate == clpstate.ecarrieridstate.waiting_for_host)
                            //{
                            //    m_clpstate[1].m_ecarrieridstate = clpstate.ecarrieridstate.id_verification_ok;
                            //}
                            //else
                            //    nack = 2;
                            m_cCMS[0].m_eCarrierIDState = cCMS.eCarrierIDState.ID_VERIFICATION_OK;
                            break;
                        case "ProceedWithSlotmap":
                            //if (m_cLPState[0].m_eSlotMapState == cLPSTATE.eCarrierSlotMapState.WAITING_FOR_HOST)
                            //{
                            //    m_cLPState[0].m_eSlotMapState = cLPSTATE.eCarrierSlotMapState.SLOTMAP_VERIFICATION_OK;
                            //}
                            //else if (m_cLPState[1].m_eSlotMapState == cLPSTATE.eCarrierSlotMapState.WAITING_FOR_HOST)
                            //{
                            //    m_cLPState[1].m_eSlotMapState = cLPSTATE.eCarrierSlotMapState.SLOTMAP_VERIFICATION_OK;
                            //}
                            //else
                            //    nAck = 2;
                            m_cCMS[0].m_eSlotMapState = cCMS.eCarrierSlotMapState.SLOTMAP_VERIFICATION_OK;
                            break;
                        default:
                            nAck = 1;
                            break;
                    }
                    SendCarrierActionReq(sCarrierAction, nAck, nSysbyte);
                    break;
            }
        }

        private void SendCarrierActionReq(string sCarrierAction, int nAck, long nSysbyte)
        {
            long nObjectID = 0;
//            string CAACK = "0";
            eERRCODE eErrorCode;
            if (nAck == 0)
                eErrorCode = eERRCODE.NO_ERROR;
            else
                eErrorCode = eERRCODE.Verifacation_Error;
            m_XGem.MakeObject(ref nObjectID);
            m_XGem.SetListItem(nObjectID, 2);
            m_XGem.SetStringItem(nObjectID, nAck.ToString());
            if (eErrorCode == eERRCODE.NO_ERROR) {
                m_XGem.SetListItem(nObjectID, 0);
            }
            else {
                m_XGem.SetListItem(nObjectID, 1);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, ((int)eErrorCode).ToString());
                m_XGem.SetStringItem(nObjectID, sCarrierAction + " OK");
            }
            m_XGem.SendSECSMessage(nObjectID, 3, 18, nSysbyte);

        }

        public void SendDecallEvent()
        {
            SendEvent_SamSung_ELEC(eCEID_NO.e_Decall);
        }

        private void SendEQPState_SSEM(long nSysbyte)
        {
            #region Message_Structure
            /*            <L, 2 * Unit Status Event Info
            1. <A[1] $SFCD> * SFCD
            2. <L, 5
                1. <L, 4 * EQP Control State Set
                    1. <A[30] $EQPID> * EQPID
                    2. <A[1] $CRST> * Online Control State
 *                  3. <A[40] $PPID> Recipe ID
                    3. <A[1] $EQ_STATUS > * 설비의 동작 상태 정보 
                2. <L, n
                    1.<L, 4 * EQP Port State Set
                        1.<A[2] $PORT_NO> *설비 내 Port에 정의 된 No
                        2.<A[1] $PORTAVAILABLESTATE > * Port Available state        EMPTY = 0 , LOAD = 1
                        3.<A[1] $PORTACCESSMODE > * 의 동작 상태 정보               READYTOLOAD =1, READYTOProcess =2, Processing =3, ReadyToUnload =4
                        4.<A[1] $PORTTRANSFERSTATE> * Port 물류 상태 정보
                3.<L, 2 * RPTID 201Set
                    1.<A[5] $ '201' > * RPTID 201
                    2.<L, n * Durable Set
                        1. <L, 5  * Durable List
                            1. <A[40] $DURABLEID> * 자재에 부여 된 ID
                            2. <A[5] $DURABLEPORTID >  * 자재 PORT 위치 정보
                            3. <A[20] $DURABLETYPE>  * 자재 유형
                            4. <A[1] $DURABLEASSIGNPOSCODE >  * 자재 가용 상태 정보
                            5. <A[5] $DURABLEQTY > * 자재 수량
                4.<L, 2 * RPTID 202Set
                    1.<A[5] $ '202' > * RPTID 202
                    2.<L, n * Consume Set
                        1. <L, 5  * Consume List
                            1. <A[100] $CONSUMID> * 자재에 부여 된 ID
                            2. <A[5] $CONSUMPORTID >  * 자재 PORT 위치 정보
                            3. <A[20] $CONSUMTYPE>  * 자재 유형
                            4. <A[1] $CONSUMASSIGNPOSCODE>  * 자재 가용 상태 정보
                            5. <A[5] $CONSUMQTY> * 자재 수량
                5<L, m * Unit List
                    1.<L, 2 * Unit Set
                        1.<A[30] $UNITID> * EQP단위 설비의 UNIT별로 관리 하는 UNIT의 ID
                        2.<A[1] $UNITST> * UNIT의 가동정보 상태 정보 값 */
            #endregion
            long nObjectID = 0;
            m_XGem.MakeObject(ref nObjectID);

            m_XGem.SetListItem(nObjectID, 2);
            m_XGem.SetStringItem(nObjectID, "1");
            m_XGem.SetListItem(nObjectID, 5);
            m_XGem.SetListItem(nObjectID, 4);
            m_XGem.SetStringItem(nObjectID, m_EQPID);
            m_XGem.SetStringItem(nObjectID, GetSVValue(eVID_NO.v_CRST));
            m_XGem.SetStringItem(nObjectID, m_cPJ.m_sRecipeID);
            m_XGem.SetStringItem(nObjectID, GetSVValue(eVID_NO.v_EQ_STATUS));        // Down = 0 RUN =1 IDLE =2 PM =3

            m_XGem.SetListItem(nObjectID, 2);
            for (int i = 1; i < 3; i++) { //LP1, LP2 For loop                                       
                m_XGem.SetListItem(nObjectID, 5);
                m_XGem.SetStringItem(nObjectID, "BP" + (i).ToString("D2"));       //Port ID
                m_XGem.SetStringItem(nObjectID, "1");
                m_XGem.SetStringItem(nObjectID, ((int)m_cCMS[i - 1].m_eLPTransferState).ToString());
                m_XGem.SetStringItem(nObjectID, ((int)m_cCMS[i - 1].GetLPAvailableState()).ToString());
                m_XGem.SetStringItem(nObjectID, ((int)(m_cCMS[i - 1].m_eAccessMode)).ToString());

            }

            //Durable Consume = EQP에 장착된 소모품 List 일단 없는걸로 전송
            m_XGem.SetListItem(nObjectID, 0);
            //m_XGem.SetListItem(nObjectID, 5);
            //m_XGem.SetStringItem(nObjectID, m_cWaferInfo.m_WaferID);
            //m_XGem.SetStringItem(nObjectID, "BP"+m_cWaferInfo.m_nPortNum.ToString());
            //m_XGem.SetStringItem(nObjectID, "0");             //DURABLE TYPE 몰라
            //m_XGem.SetStringItem(nObjectID, "0");             //Durable Assign pos code 몰라
            //m_XGem.SetStringItem(nObjectID, "1000");             //QTY 몰라

            //Durable Consume = EQP에 장착된 소모품 List 일단 없는걸로 전송
            m_XGem.SetListItem(nObjectID, 0);
            //m_XGem.SetListItem(nObjectID, 5);
            //m_XGem.SetStringItem(nObjectID, m_cWaferInfo.m_WaferID);
            //m_XGem.SetStringItem(nObjectID, "BP" + m_cWaferInfo.m_nPortNum.ToString());
            //m_XGem.SetStringItem(nObjectID, "0");             //Consume TYPE 몰라
            //m_XGem.SetStringItem(nObjectID, "0");             //Consume Assign pos code 몰라
            //m_XGem.SetStringItem(nObjectID, "1000");             //QTY 몰라

            m_XGem.SetListItem(nObjectID, 2);
            m_XGem.SetListItem(nObjectID, 2);
            m_XGem.SetStringItem(nObjectID, "HUMT9884_010");
            m_XGem.SetStringItem(nObjectID, GetSVValue(eVID_NO.v_EQ_STATUS));
            m_XGem.SetListItem(nObjectID, 2);
            m_XGem.SetStringItem(nObjectID, "HUMT9884_020");
            m_XGem.SetStringItem(nObjectID, GetSVValue(eVID_NO.v_EQ_STATUS));

            WriteLog("Send EQP State Rsp S1F6");
            long ret = m_XGem.SendSECSMessage(nObjectID, 1, 6, nSysbyte);

        }

        private void SendEvent_SamSung_ELEC(eCEID_NO eCEID, int nLP = 0, string CarrierID = "", string LotID = "", string SlotMap = "", long nSysByte = 0)
        {
            long nObjectID = 0;
            WriteLog("SendEvent : " + eCEID.ToString());
            m_XGem.MakeObject(ref nObjectID);
            switch (eCEID) {
                case eCEID_NO.e_OFFLINE:
                    #region Offline
                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, " ");
                    m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID].ToString());
                    SetRPT(nObjectID, 100);

                    #endregion
                    break;
                case eCEID_NO.e_ONLINELOCAL:
                    #region LOCAL
                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, " ");
                    m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID].ToString());
                    SetRPT(nObjectID, 100);
                    m_XGem.GEMReqLocal();
                    #endregion
                    break;
                case eCEID_NO.e_ONLINEREMOTE:
                    #region REMOTE
                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, " ");
                    m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID].ToString());
                    SetRPT(nObjectID, 100);
                    m_XGem.GEMReqRemote();
                    #endregion
                    break;
                case eCEID_NO.e_EQPSTATUSCHANGE:
                    #region EQP_STATE_CHANGE
                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, " ");
                    m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID].ToString());
                    m_XGem.SetListItem(nObjectID, 3);
                    SetRPT(nObjectID, 100);
                    SetRPT(nObjectID, 101);
                    SetRPT(nObjectID, 104);
                    #endregion
                    break;
                case eCEID_NO.e_LP_LOADREQUEST:
                    #region LOAD_REQUEST
                    if (nLP == 0) {
                        SetSVValue(eVID_NO.v_LOTID, "");
                        SetSVValue(eVID_NO.v_CarrierID, "");
                        SetSVValue(eVID_NO.v_SlotMap, "");
                        SetSVValue(eVID_NO.v_PortTransferState, ((int)(cCMS.eLPTransferState.ReadyToLoad)).ToString());
                        m_cCMS[0].m_eLPTransferState = cCMS.eLPTransferState.ReadyToLoad;
                    }
                    else if (nLP == 1) {
                        SetSVValue(eVID_NO.v_LOTID, "");
                        SetSVValue(eVID_NO.v_CarrierID, "");
                        SetSVValue(eVID_NO.v_SlotMap, "");
                        SetSVValue(eVID_NO.v_PortTransferState, ((int)(cCMS.eLPTransferState.ReadyToLoad)).ToString());
                        m_cCMS[1].m_eLPTransferState = cCMS.eLPTransferState.ReadyToLoad;
                    }
                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, "    ");
                    m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID_NO.e_LP_LOADREQUEST].ToString());
                    m_XGem.SetListItem(nObjectID, 3);
                    SetRPT(nObjectID, 100, nLP);
                    SetRPT(nObjectID, 107, nLP);
                    SetRPT(nObjectID, 306, nLP);
                    #endregion
                    break;
                case eCEID_NO.e_LP_LOADCOMPLETE:
                    #region LoadComplete
                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, "    ");
                    m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID_NO.e_LP_LOADCOMPLETE].ToString());
                    m_XGem.SetListItem(nObjectID, 3);
                    SetRPT(nObjectID, 100, nLP);
                    SetRPT(nObjectID, 107, nLP);
                    SetRPT(nObjectID, 306, nLP);
                    #endregion
                    break;
                case eCEID_NO.e_LP_CARRIERIDREAD:
                    #region CarrierIDRead
                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, "    ");
                    m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID_NO.e_LP_CARRIERIDREAD].ToString());
                    m_XGem.SetListItem(nObjectID, 3);
                    SetRPT(nObjectID, 100, nLP);
                    SetRPT(nObjectID, 107, nLP);
                    SetRPT(nObjectID, 306, nLP);
                    #endregion
                    break;
                case eCEID_NO.e_LP_SLOTMAP_WAITINGFORHOST:
                    #region SlotMap_WaitingForHost
                    if (nLP == 1)
                        SetSVValue(eVID_NO.v_SlotMap, SlotMap);
                    else if (nLP == 2)
                        SetSVValue(eVID_NO.v_SlotMap, SlotMap);
                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, "    ");
                    m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID_NO.e_LP_SLOTMAP_WAITINGFORHOST].ToString());
                    m_XGem.SetListItem(nObjectID, 3);
                    SetRPT(nObjectID, 100, nLP);
                    SetRPT(nObjectID, 107, nLP);
                    SetRPT(nObjectID, 306, nLP);
                    #endregion
                    break;
                case eCEID_NO.e_LP_SLOTMAP_VERIFICATIONOK:
                    #region SlotMap_VerificationOK
                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, "    ");
                    m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID_NO.e_LP_SLOTMAP_VERIFICATIONOK].ToString());
                    m_XGem.SetListItem(nObjectID, 5);
                    SetRPT(nObjectID, 100, nLP);
                    SetRPT(nObjectID, 107, nLP);
                    SetRPT(nObjectID, 306, nLP);
                    SetRPT(nObjectID, 201, nLP);
                    SetRPT(nObjectID, 202, nLP);
                    #endregion
                    break;
                case eCEID_NO.e_LP_UNLOADREQUEST:
                    #region UnloadRequest

                    SetSVValue(eVID_NO.v_PortTransferState, ((int)(cCMS.eLPTransferState.ReadyToUnload)).ToString());
                    m_cCMS[m_nWorkLP].m_eLPTransferState = cCMS.eLPTransferState.ReadyToUnload;

                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, "    ");
                    m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID_NO.e_LP_UNLOADREQUEST].ToString());
                    m_XGem.SetListItem(nObjectID, 3);
                    SetRPT(nObjectID, 100, nLP);
                    SetRPT(nObjectID, 107, nLP);
                    SetRPT(nObjectID, 306, nLP);
                    #endregion
                    break;
                case eCEID_NO.e_LP_UNLOADCOMPLETE:
                    #region UnloadComplete
                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, "    ");
                    m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID_NO.e_LP_UNLOADCOMPLETE].ToString());
                    m_XGem.SetListItem(nObjectID, 3);
                    SetRPT(nObjectID, 100, nLP);
                    SetRPT(nObjectID, 107, nLP);
                    SetRPT(nObjectID, 306, nLP);
                    m_cCMS[m_nWorkLP].m_eLPTransferState = cCMS.eLPTransferState.ReadyToLoad;
                    #endregion
                    break;
                case eCEID_NO.e_LP_ACCESSMODECHANGE:
                    #region AccessModeChange
                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, "  ");
                    m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID_NO.e_LP_ACCESSMODECHANGE].ToString());
                    m_XGem.SetListItem(nObjectID, 2);
                    SetRPT(nObjectID, 100);
                    SetRPT(nObjectID, 306, nLP);
                    #endregion
                    break;
                case eCEID_NO.e_WorkStarted:
                    #region WorkStart
                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, "  ");
                    m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID_NO.e_WorkStarted].ToString());
                    m_XGem.SetListItem(nObjectID, 3);
                    SetRPT(nObjectID, 100);
                    SetRPT(nObjectID, 306, nLP);
                    SetRPT(nObjectID, 111, nLP);
                    m_cCMS[m_nWorkLP].m_eLPTransferState = cCMS.eLPTransferState.Processing;
                    #endregion
                    break;
                case eCEID_NO.e_WorkCancel:
                    #region WorkStart
                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, "  ");
                    m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID_NO.e_WorkStarted].ToString());
                    m_XGem.SetListItem(nObjectID, 3);
                    SetRPT(nObjectID, 100);
                    SetRPT(nObjectID, 306, nLP);
                    SetRPT(nObjectID, 111, nLP);
                    #endregion
                    break;
                case eCEID_NO.e_WorkAbort:
                    #region WorkStart
                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, "  ");
                    m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID_NO.e_WorkAbort].ToString());
                    m_XGem.SetListItem(nObjectID, 3);
                    SetRPT(nObjectID, 100);
                    SetRPT(nObjectID, 306, nLP);
                    SetRPT(nObjectID, 111, nLP);
                    #endregion
                    break;
                case eCEID_NO.e_WorkCompleted:
                    #region WorkStart
                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, "  ");
                    m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID_NO.e_WorkCompleted].ToString());
                    m_XGem.SetListItem(nObjectID, 3);
                    SetRPT(nObjectID, 100);
                    SetRPT(nObjectID, 306, nLP);
                    SetRPT(nObjectID, 111, nLP);
                    #endregion
                    break;
                case eCEID_NO.e_WorkPause:
                    #region WorkStart
                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, "  ");
                    m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID_NO.e_WorkPause].ToString());
                    m_XGem.SetListItem(nObjectID, 3);
                    SetRPT(nObjectID, 100);
                    SetRPT(nObjectID, 306, nLP);
                    SetRPT(nObjectID, 111, nLP);
                    #endregion
                    break;
                case eCEID_NO.e_WorkResume:
                    #region WorkStart
                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, "  ");
                    m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID_NO.e_WorkResume].ToString());
                    m_XGem.SetListItem(nObjectID, 3);
                    SetRPT(nObjectID, 100);
                    SetRPT(nObjectID, 306, nLP);
                    SetRPT(nObjectID, 111, nLP);
                    #endregion
                    break;
                case eCEID_NO.e_WaferStarted:
                case eCEID_NO.e_WaferCompleted:
                case eCEID_NO.e_ProcessStarted:
                case eCEID_NO.e_ProcessEnded:
                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, "");
                    m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID].ToString());
                    m_XGem.SetListItem(nObjectID, 3);
                    SetRPT(nObjectID, 100, nLP);
                    SetRPT(nObjectID, 303, nLP);
                    SetRPT(nObjectID, 307, nLP);
                    break;
                //case eCEID_NO.e_PRJob_Queued:
                //case eCEID_NO.e_PRJob_WaitSTart:
                //case eCEID_NO.e_PRJob_Processing:
                //case eCEID_NO.e_PRJob_Completed:
                //    m_XGem.SetListItem(nObjectID, 2);
                //    m_XGem.SetStringItem(nObjectID, "");
                //    m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID].ToString());
                //    //m_XGem.SetListItem(nObjectID, 3);
                //    //m_XGem.SetStringItem(nObjectID, "  ");
                //    //m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID].ToString());
                //    //m_XGem.SetListItem(nObjectID, 0);
                //    break;
                //case eCEID_NO.e_CTRLJob_Queued:
                //case eCEID_NO.e_CTRLJob_WaitSTart:
                //case eCEID_NO.e_CTRLJob_Excuting:
                //case eCEID_NO.e_CTRLJob_Completed:
                //    m_XGem.SetListItem(nObjectID, 2);
                //    m_XGem.SetStringItem(nObjectID, "");
                //    m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID].ToString());
                //    //m_XGem.SetListItem(nObjectID, 3);
                //    //m_XGem.SetStringItem(nObjectID, "  ");

                //    //m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID].ToString());
                //    //m_XGem.SetListItem(nObjectID, 0);
                //    break;
                case eCEID_NO.e_Rework:
                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, "");
                    m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID].ToString());
                    m_XGem.SetListItem(nObjectID, 2);
                    SetRPT(nObjectID, 100, nLP);
                    SetRPT(nObjectID, 901, nLP);
                    break;
                case eCEID_NO.e_Scrap:
                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, "");
                    m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID].ToString());
                    m_XGem.SetListItem(nObjectID, 2);
                    SetRPT(nObjectID, 100, nLP);
                    SetRPT(nObjectID, 900, nLP);
                    break;
                case eCEID_NO.e_Decall:
                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, "    ");
                    m_XGem.SetStringItem(nObjectID, m_nCEID[(int)eCEID].ToString());
                    m_XGem.SetListItem(nObjectID, 2);
                    SetRPT(nObjectID, 100);
                    SetRPT(nObjectID, 210);
                    break;
                default:
                    WriteLog("Send Massage Fail");
                    return;
            }

            if (nSysByte == 0)
                m_XGem.SendSECSMessage(nObjectID, 6, 11, 0);
            else
                m_XGem.SendSECSMessage(nObjectID, 6, 16, nSysByte);
        }

        private void RecieveEvent_SAMSUNG_ELEC(string sACK6)    //문제 생기면 SystemByte 보고 하도록 수정해야뎀
        {
            if (sACK6 == "0") {
                /*    switch (m_eLastEventMSG) {
                        case eCEID_NO.e_OFFLINE:
                            //m_XGem.GEMReqOffline();
                            break;
                        case eCEID_NO.e_ONLINELOCAL:
                            //m_XGem.GEMReqLocal();
                            break;
                        case eCEID_NO.e_ONLINEREMOTE:
                            //m_XGem.GEMReqRemote();
                            break;
                        case eCEID_NO.e_EQPSTATUSCHANGE:
                            break;
                        case eCEID_NO.e_LP_LOADREQUEST:
                            break;
                        case eCEID_NO.e_LP_LOADCOMPLETE:
                            break;
                        case eCEID_NO.e_LP_CARRIERIDREAD:
                            //if (m_cLPState[0].m_eCarrierIDState == cLPSTATE.eCarrierIDState.WAITING_FOR_HOST)
                            //    m_cLPState[0].m_eCarrierIDState = cLPSTATE.eCarrierIDState.ID_VERIFICATION_OK;
                            //if (m_cLPState[1].m_eCarrierIDState == cLPSTATE.eCarrierIDState.WAITING_FOR_HOST)
                            //    m_cLPState[1].m_eCarrierIDState = cLPSTATE.eCarrierIDState.ID_VERIFICATION_OK;
                            break;
                        case eCEID_NO.e_LP_SLOTMAP_WAITINGFORHOST:
                            break;
                        default:
                            WriteLog("Not Define Event In EQP : " + m_eLastEventMSG.ToString());
                            break;
                    }*/
            }
            else {
                WriteLog("Last Event Fail : ");
            }
        }

        private void SetRPT(long nObjectID, int nReportNum, int nLP = 0)
        {
            #region Report_Define
            switch (nReportNum) {
                case 100:
                    m_XGem.SetListItem(nObjectID, 2);
                    m_XGem.SetStringItem(nObjectID, "100");
                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, m_EQPID);
                    m_XGem.SetStringItem(nObjectID, GetSVValue(eVID_NO.v_CRST));
                    m_XGem.SetStringItem(nObjectID, GetSVValue(eVID_NO.v_EQ_STATUS));
                    break;
                case 101:
                    m_XGem.SetListItem(nObjectID, 2);
                    m_XGem.SetStringItem(nObjectID, "101");
                    m_XGem.SetListItem(nObjectID, 2);
                    m_XGem.SetStringItem(nObjectID, GetSVValue(eVID_NO.v_OldEQState));
                    m_XGem.SetStringItem(nObjectID, GetSVValue(eVID_NO.v_CurEQState));
                    break;
                case 104:
                    m_XGem.SetListItem(nObjectID, 2);
                    m_XGem.SetStringItem(nObjectID, "104");
                    if (GetSVValue(eVID_NO.v_CurEQState) == "0") {
                        m_XGem.SetListItem(nObjectID, 4);
                        m_XGem.SetStringItem(nObjectID, GetSVValue(eVID_NO.v_ALST));
                        m_XGem.SetStringItem(nObjectID, GetSVValue(eVID_NO.v_ALCD));
                        m_XGem.SetStringItem(nObjectID, GetSVValue(eVID_NO.v_ALID));
                        m_XGem.SetStringItem(nObjectID, GetSVValue(eVID_NO.v_ALTX));
                    }
                    else {
                        m_XGem.SetListItem(nObjectID, 0);
                    }
                    break;
                case 105:
                    m_XGem.SetListItem(nObjectID, 2);
                    m_XGem.SetStringItem(nObjectID, "105");
                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, m_EQPID);
                    m_XGem.SetStringItem(nObjectID, GetSVValue(eVID_NO.v_CRST));
                    m_XGem.SetStringItem(nObjectID, GetSVValue(eVID_NO.v_EQ_STATUS));
                    break;
                case 107:
                    m_XGem.SetListItem(nObjectID, 2);
                    m_XGem.SetStringItem(nObjectID, "107");
                    m_XGem.SetListItem(nObjectID, 5);
                    m_XGem.SetStringItem(nObjectID, m_cCMS[m_nWorkLP].LotID);
                    m_XGem.SetStringItem(nObjectID, m_cCMS[m_nWorkLP].CarrierID);
                    m_XGem.SetStringItem(nObjectID, m_cPJ.m_SlotMap);
                    m_XGem.SetStringItem(nObjectID, ((int)m_cCMS[m_nWorkLP].m_eCarrierType).ToString());
                    m_XGem.SetStringItem(nObjectID, m_cPJ.m_ePreRunFlag);
                    break;
                case 111:
                    m_XGem.SetListItem(nObjectID, 2);
                    m_XGem.SetStringItem(nObjectID, "111");
                    m_XGem.SetListItem(nObjectID, 1);
                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetStringItem(nObjectID, m_cCMS[m_nWorkLP].LotID);
                    m_XGem.SetStringItem(nObjectID, m_cPJ.m_sRecipeID);
                    m_XGem.SetListItem(nObjectID, 1);
                    m_XGem.SetListItem(nObjectID, 5);
                    m_XGem.SetStringItem(nObjectID, m_cCMS[m_nWorkLP].CarrierID);
                    m_XGem.SetStringItem(nObjectID, FindSlotMapNum().ToString());
                    m_XGem.SetStringItem(nObjectID, m_cPJ.m_SlotMap);
                    m_XGem.SetStringItem(nObjectID, m_cPJ.m_SlotMapInsp);

                    m_XGem.SetListItem(nObjectID, 3);
                    m_XGem.SetListItem(nObjectID, 2);
                    m_XGem.SetStringItem(nObjectID, "6KCD10.001");
                    m_XGem.SetStringItem(nObjectID, "2");
                    m_XGem.SetListItem(nObjectID, 2);
                    m_XGem.SetStringItem(nObjectID, "6KCD10.002");
                    m_XGem.SetStringItem(nObjectID, "3");
                    m_XGem.SetListItem(nObjectID, 2);
                    m_XGem.SetStringItem(nObjectID, "6KCD10.003");
                    m_XGem.SetStringItem(nObjectID, "4");
                    //m_XGem.SetListItem(nObjectID, 1);
                    //m_XGem.SetListItem(nObjectID,2);
                    //m_XGem.SetStringItem(nObjectID, m_cWaferInfo.m_WaferID);
                    //m_XGem.SetStringItem(nObjectID, m_cWaferInfo.m_nSlotNum.ToString());
                    break;
                case 201:
                    m_XGem.SetListItem(nObjectID, 2);
                    m_XGem.SetStringItem(nObjectID, "201");
                    m_XGem.SetListItem(nObjectID, 1);
                    m_XGem.SetListItem(nObjectID, 5);
                    m_XGem.SetStringItem(nObjectID, "");
                    m_XGem.SetStringItem(nObjectID, "");
                    m_XGem.SetStringItem(nObjectID, "");
                    m_XGem.SetStringItem(nObjectID, "");
                    m_XGem.SetStringItem(nObjectID, "");
                    break;
                case 202:
                    m_XGem.SetListItem(nObjectID, 2);
                    m_XGem.SetStringItem(nObjectID, "202");
                    m_XGem.SetListItem(nObjectID, 1);
                    m_XGem.SetListItem(nObjectID, 5);
                    m_XGem.SetStringItem(nObjectID, "");
                    m_XGem.SetStringItem(nObjectID, "");
                    m_XGem.SetStringItem(nObjectID, "");
                    m_XGem.SetStringItem(nObjectID, "");
                    m_XGem.SetStringItem(nObjectID, "");
                    break;
                case 210:
                    m_XGem.SetListItem(nObjectID, 2);
                    m_XGem.SetStringItem(nObjectID, "210");
                    m_XGem.SetListItem(nObjectID, 2);
                    m_XGem.SetListItem(nObjectID, 2);
                    m_XGem.SetStringItem(nObjectID, "Data_ITEM1");
                    m_XGem.SetStringItem(nObjectID, "Data_Value1");
                    m_XGem.SetListItem(nObjectID, 2);
                    m_XGem.SetStringItem(nObjectID, "Data_ITEM2");
                    m_XGem.SetStringItem(nObjectID, "Data_Value2");
                    break;
                case 303:
                    m_XGem.SetListItem(nObjectID, 2);
                    m_XGem.SetStringItem(nObjectID, "303");
                    m_XGem.SetListItem(nObjectID, 5);
                    m_XGem.SetStringItem(nObjectID, m_cCMS[m_nWorkLP].CarrierID);
                    m_XGem.SetStringItem(nObjectID, m_cPJ.m_sRecipeID);
                    m_XGem.SetStringItem(nObjectID, ((int)m_cCMS[m_nWorkLP].m_eProducType).ToString());
                    m_XGem.SetStringItem(nObjectID, m_sPRODID);
                    m_XGem.SetStringItem(nObjectID, m_OperatorID);
                    break;
                case 306:
                    m_XGem.SetListItem(nObjectID, 2);
                    m_XGem.SetStringItem(nObjectID, "306");
                    m_XGem.SetListItem(nObjectID, 5);
                    m_XGem.SetStringItem(nObjectID, "BP" + (nLP + 1).ToString("D2"));
                    m_XGem.SetStringItem(nObjectID, ((int)m_cCMS[m_nWorkLP].m_ePortType).ToString());
                    m_XGem.SetStringItem(nObjectID, ((int)m_cCMS[m_nWorkLP].m_eLPTransferState).ToString());
                    m_XGem.SetStringItem(nObjectID, ((int)m_cCMS[m_nWorkLP].GetLPAvailableState()).ToString());
                    m_XGem.SetStringItem(nObjectID, ((int)m_cCMS[m_nWorkLP].m_eAccessMode).ToString());
                    break;

                case 307:
                    m_XGem.SetListItem(nObjectID, 2);
                    m_XGem.SetStringItem(nObjectID, "307");
                    m_XGem.SetListItem(nObjectID, 1);
                    m_XGem.SetListItem(nObjectID, 10);
                    m_XGem.SetStringItem(nObjectID, m_cPJ.m_sPanelID[m_nWorkSlot - 1]);
                    m_XGem.SetStringItem(nObjectID, m_cPJ.m_sPanelID[m_nWorkSlot - 1]);     //KJW OCr 결과
                    m_XGem.SetStringItem(nObjectID, "");
                    m_XGem.SetStringItem(nObjectID, m_cCMS[nLP].CarrierID);
                    m_XGem.SetStringItem(nObjectID, m_cCMS[nLP].LotID);
                    m_XGem.SetStringItem(nObjectID, "");
                    m_XGem.SetStringItem(nObjectID, (m_nWorkSlot).ToString());
                    m_XGem.SetStringItem(nObjectID, " ");
                    m_XGem.SetStringItem(nObjectID, "");
                    m_XGem.SetStringItem(nObjectID, "");
                    break;
                case 900:
                    m_XGem.SetListItem(nObjectID, 2);
                    m_XGem.SetStringItem(nObjectID, "900");
                    m_XGem.SetListItem(nObjectID, 5);
                    m_XGem.SetStringItem(nObjectID, ((int)(m_cCMS[m_nWorkLP - 1].m_eProducType)).ToString());
                    m_XGem.SetStringItem(nObjectID, m_cCMS[nLP - 1].CarrierID);
                    m_XGem.SetStringItem(nObjectID, m_cWaferInfo.m_WaferID);
                    m_XGem.SetStringItem(nObjectID, m_cWaferInfo.m_WaferID);
                    m_XGem.SetStringItem(nObjectID, m_cWaferInfo.m_nSlotNum.ToString());
                    break;
                case 901:
                    m_XGem.SetListItem(nObjectID, 2);
                    m_XGem.SetStringItem(nObjectID, "901");
                    m_XGem.SetListItem(nObjectID, 6);
                    m_XGem.SetStringItem(nObjectID, ((int)(m_cCMS[nLP - 1].m_eProducType)).ToString());
                    m_XGem.SetStringItem(nObjectID, m_cCMS[nLP - 1].CarrierID);
                    m_XGem.SetStringItem(nObjectID, m_cWaferInfo.m_WaferID);
                    m_XGem.SetStringItem(nObjectID, m_cWaferInfo.m_WaferID);
                    m_XGem.SetStringItem(nObjectID, m_cCMS[nLP - 1].LotID);
                    m_XGem.SetStringItem(nObjectID, m_cWaferInfo.m_nSlotNum.ToString());
                    break;
                default:
                    break;
            }
            #endregion
        }

        string m_sStepID = "*";
        string m_sPRODID = "*";

        private void PRJOBCreate(long nObjectID, long nSysbyte)
        {
            string stemp = "";
            long nItemCount = 0;
            long nItemCount2 = 0;
            long temp = 0;
            long temp2 = 0;
            string sTemp2 = "";
            string sTemp3 = "";

            if (m_cPJ.m_sPRJobID != "") { // PRJob 생성되 있을시
                SendAckPRJobCreate(1, nSysbyte, m_cPJ.m_sPRJobID, eERRCODECJ.ObjectIdentifier_InUse, "Already PJ Exist PJID : ");
                return;
            }
            m_XGem.GetListItem(nObjectID, ref temp);
            m_XGem.GetStringItem(nObjectID, ref stemp);      //Data ID 무시
            m_XGem.GetListItem(nObjectID, ref temp);
            if (temp != 1) {
                SendAckPRJobCreate(1, nSysbyte, m_cPJ.m_sPRJobID, eERRCODECJ.InsufficientParameters, "Does Not Support MuliCreat");
                return;
            }
            m_XGem.GetListItem(nObjectID, ref temp);
            //m_XGem.GetStringItem(nObjectID, ref m_cPJ.m_sPRJobID);
            m_XGem.GetStringItem(nObjectID, ref stemp);
            if (stemp.IndexOf(" ") >= 0)
                m_cPJ.m_sPRJobID = stemp.Substring(0, stemp.IndexOf(" "));
            else
                m_cPJ.m_sPRJobID = stemp;

            m_XGem.GetStringItem(nObjectID, ref stemp);              // PRJOB ID
            m_XGem.GetStringItem(nObjectID, ref stemp);              // PrerunFlag 무시
            m_cPJ.m_ePreRunFlag = stemp;
            m_XGem.GetStringItem(nObjectID, ref stemp);              // Reallotflag 무시
            m_XGem.GetStringItem(nObjectID, ref stemp);              // ProductID
            m_sPRODID = stemp;
            m_XGem.GetStringItem(nObjectID, ref stemp);              // StepID\
            m_sStepID = stemp;
            m_XGem.GetListItem(nObjectID, ref nItemCount);
            for (int i = 0; i < nItemCount; i++) {
                if (i == 0) {
                    m_XGem.GetListItem(nObjectID, ref temp);
                    m_XGem.GetStringItem(nObjectID, ref m_cCMS[m_nWorkLP].CarrierID);            //Carrier ID
                    m_XGem.GetListItem(nObjectID, ref temp);
                    m_XGem.GetStringItem(nObjectID, ref m_cPJ.m_SlotMap);
                    m_XGem.GetStringItem(nObjectID, ref m_cPJ.m_SlotMapInsp);
                    m_XGem.GetListItem(nObjectID, ref nItemCount2);
                    if (FindSlotMapNum() != nItemCount2) {
                        SendAckPRJobCreate(1, nSysbyte, m_cPJ.m_sPRJobID, eERRCODECJ.InsufficientParameters, "Not Matched Slot Map");
                        m_cPJ = new cPJ();
                        return;
                    }
                    for (int n = 0; n < nItemCount2; n++) {
                        m_XGem.GetListItem(nObjectID, ref temp);
                        m_XGem.GetStringItem(nObjectID, ref sTemp2);
                        m_XGem.GetStringItem(nObjectID, ref sTemp3);
                        m_cPJ.m_sPanelID[Convert.ToInt32(sTemp3) - 1] = sTemp2;
                    }
                }
                else {      // 사용 안함
                    m_XGem.GetListItem(nObjectID, ref temp);
                    m_XGem.GetStringItem(nObjectID, ref stemp);
                    m_XGem.GetListItem(nObjectID, ref temp);
                    m_XGem.GetStringItem(nObjectID, ref stemp);
                    m_XGem.GetStringItem(nObjectID, ref stemp);
                    m_XGem.GetListItem(nObjectID, ref temp);
                    for (int n = 0; n < temp; n++) {
                        m_XGem.GetListItem(nObjectID, ref temp2);
                        m_XGem.GetStringItem(nObjectID, ref stemp);
                        m_XGem.GetStringItem(nObjectID, ref stemp);
                    }
                }
            }
            m_XGem.GetListItem(nObjectID, ref temp);
            m_XGem.GetStringItem(nObjectID, ref stemp);      //Reciep 선택 모드 0, 1 : Recipe 선택 2: Recipe 튜닝 2 지원 안함 
            if (stemp != "1") {
                SendAckPRJobCreate(1, nSysbyte, m_cPJ.m_sPRJobID, eERRCODECJ.InsufficientParameters, "PRRECIPEMETHOD Param Error : " + stemp);
                m_cPJ = new cPJ();
                return;
            }
            m_XGem.GetStringItem(nObjectID, ref stemp);      //Recipe ID
            if (stemp.IndexOf(" ") >= 0)
                stemp = stemp.Substring(0, stemp.IndexOf(" "));
            m_cPJ.m_sRecipeID = stemp;
            if (!FindRecipe(stemp)) {
                if (stemp != "1") {
                    SendAckPRJobCreate(1, nSysbyte, m_cPJ.m_sPRJobID, eERRCODECJ.InsufficientParameters, "Recipe Does Not Exist Recipe Name : " + stemp);
                    m_cPJ = new cPJ();
                    return;
                }
            }
            if (OnRecipeSelected != null)
                OnRecipeSelected(stemp);

            m_XGem.GetListItem(nObjectID, ref nItemCount);       //Recipe Spec 없음    
            for (int i = 0; i < nItemCount; i++) {
                m_XGem.GetListItem(nObjectID, ref temp);
                m_XGem.GetStringItem(nObjectID, ref stemp);
                m_XGem.GetStringItem(nObjectID, ref stemp);
            }
            m_XGem.GetStringItem(nObjectID, ref stemp);          //PRPRocessStart Flag
            m_cPJ.m_sAutoStart = stemp;
            m_XGem.GetListItem(nObjectID, ref nItemCount);
            for (int i = 0; i < nItemCount; i++) {
                m_XGem.GetStringItem(nObjectID, ref stemp);
            }
            SendAckPRJobCreate(0, nSysbyte, m_cPJ.m_sPRJobID);
        }
        private bool FindRecipe(string sRcpName)
        {
            for (int i = 0; i < m_cRecipeList.nRecipeNum; i++) {
                if (m_cRecipeList.sName[i] == sRcpName)
                    return true;
            }
            return false;

            //string sDirPath = m_sRecipePath + sRcpName;
            //DirectoryInfo di = new DirectoryInfo(sDirPath);
            //if (di.Exists) {
            //    WriteLog("Recipe Find OK , Path : " + sDirPath);
            //    return true;
            //}
            //else {
            //    WriteLog("Recipe Does Not Exist, Please Check Dir Path : " + sDirPath);
            //    return false;
            //}
        }


        private void SendNack(long nSysbyte)
        {
            long m_AckObject = 0;
            m_XGem.MakeObject(ref m_AckObject);
            m_XGem.SetStringItem(m_AckObject, "4");
            m_XGem.SendSECSMessage(m_AckObject, 2, 16, nSysbyte);
        }
        public enum eERRCODECJ
        {
            NO_ERROR = 0,
            Unkown_Object,
            Unkown_TargetObjuctType,
            Unkown_ObjectInstance,
            Unkown_AttributeName,
            ReadOnly_AccessDenied,
            Unkown_ObjectType,
            Invalid_AttibuteValue,
            Syntax_Error,
            Verifacation_Error,
            Validataion_Error,
            ObjectIdentifier_InUse,
            ParametersImproperlySpecified,
            InsufficientParameters,
            Unsupported_Option,
            Busy,
        }
        private int FindSlotMapNum()
        {
            int num = 0;
            for (int i = 0; i < m_cPJ.m_SlotMap.Length; i++) {
                if (m_cPJ.m_SlotMap.Substring(i, 1) == "1") {
                    num++;
                    if (num == 1)
                        m_nWorkSlot = i + 1;
                }
            }
            return num;
        }
        private void SendAckPRJobCreate(int nAck, long nSystemByte, string sPRJobID, eERRCODECJ eErrorCode = eERRCODECJ.NO_ERROR, string nErrorTXT = "")     // True False
        {
            long nObjectID = 0;
            m_XGem.MakeObject(ref nObjectID);
            m_XGem.SetListItem(nObjectID, 2);
            m_XGem.SetListItem(nObjectID, 1);
            m_XGem.SetStringItem(nObjectID, sPRJobID);
            m_XGem.SetListItem(nObjectID, 2);
            m_XGem.SetStringItem(nObjectID, nAck.ToString());
            if (eErrorCode == eERRCODECJ.NO_ERROR) {
                m_XGem.SetListItem(nObjectID, 0);
            }
            else {
                m_XGem.SetListItem(nObjectID, 1);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, ((int)eErrorCode).ToString());
                m_XGem.SetStringItem(nObjectID, nErrorTXT);
            }
            m_XGem.SendSECSMessage(nObjectID, 16, 16, nSystemByte);
        }

        private void PRJobDequeue(long nObjectID, long nSysByte)
        {
            long ItemCount = 0;
            string[] JobID = new string[5] { "", "", "", "", "" };
            int nIsExistJob = 0;
            m_XGem.GetListItem(nObjectID, ref ItemCount);
            for (int i = 0; i < ItemCount; i++) {
                m_XGem.GetStringItem(nObjectID, ref JobID[i]);
                if (JobID[i] == m_cPJ.m_sPRJobID)
                    nIsExistJob = 1;
            }
            if (nIsExistJob == 1) {
                SendAckPRJobDequeue(nSysByte, JobID, 0);
            }
            else {
                SendAckPRJobDequeue(nSysByte, JobID, 1, eERRCODECJ.InsufficientParameters, "Job ID Does Not Exist");
            }


        }

        private void SendAckPRJobDequeue(long nSysByte, string[] JobID, int nAck, eERRCODECJ eError = eERRCODECJ.NO_ERROR, string ErrorTXT = "")
        {
            long nObjectID = 0;
            int nIDNum = 0;
            m_XGem.MakeObject(ref nObjectID);
            m_XGem.SetListItem(nObjectID, 2);
            for (int i = 0; i < JobID.Length; i++) {
                if (JobID[i] == "") {
                    nIDNum = i;
                    break;
                }
            }
            m_XGem.SetListItem(nObjectID, nIDNum);
            for (int i = 0; i < nIDNum; i++) {
                m_XGem.SetStringItem(nObjectID, JobID[i]);
            }
            m_XGem.SetListItem(nObjectID, 2);
            m_XGem.SetStringItem(nObjectID, nAck.ToString());
            if (eError == eERRCODECJ.NO_ERROR) {
                m_XGem.SetListItem(nObjectID, 0);
                m_cPJ = new cPJ();
            }
            else {
                m_XGem.SetListItem(nObjectID, 1);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, ((int)eError).ToString());
                m_XGem.SetStringItem(nObjectID, ErrorTXT);
            }
            m_XGem.SendSECSMessage(nObjectID, 16, 18, nSysByte);
        }

        private void PRJobCommandRecived(long nObjectID, long nSysByte)
        {
            long ItemCount = 0;
            string sTemp = "";
            string PRJobID = "";
            string sPRJobCMD = "";
            long nTemp = 0;
            m_XGem.GetListItem(nObjectID, ref ItemCount);
            m_XGem.GetStringItem(nObjectID, ref sTemp);
            m_XGem.GetStringItem(nObjectID, ref PRJobID);
            m_XGem.GetStringItem(nObjectID, ref sPRJobCMD);
            m_XGem.GetListItem(nObjectID, ref ItemCount);
            for (int i = 0; i < ItemCount; i++) {
                m_XGem.GetListItem(nObjectID, ref nTemp);
                m_XGem.GetStringItem(nObjectID, ref sTemp);
                m_XGem.GetStringItem(nObjectID, ref sTemp);
            }
            if (sPRJobCMD.IndexOf(" ") >= 0)
                sPRJobCMD.Substring(0, sPRJobCMD.IndexOf(" "));
            switch (sPRJobCMD) {
                case "START":
                    //if (m_cPJ.m_sPRJobID != "")
                    //    SendEvent_SamSung_ELEC(eCEID_NO.e_PRJob_Processing);
                    m_cPJ.PRJobState = cPJ.ePRJObState.Processing;
                    break;
                case "STOP":
                    SetEQPState(eEQPState_SSEM.EQ_IDLE);
                    //SendEvent_SamSung_ELEC(eCEID_NO.e_PRJob_WaitSTart);
                    m_cPJ.PRJobState = cPJ.ePRJObState.Stopped;
                    break;
                case "PAUSE":
                    SetEQPState(eEQPState_SSEM.EQ_IDLE);
                    //SendEvent_SamSung_ELEC(eCEID_NO.e_PRJob_WaitSTart);
                    m_cPJ.PRJobState = cPJ.ePRJObState.Paused;
                    break;
                case "RESUME":
                    //if (m_cPJ.m_sPRJobID != "")
                    //    SendEvent_SamSung_ELEC(eCEID_NO.e_PRJob_Processing);
                    SetEQPState(eEQPState_SSEM.EQ_RUN);
                    m_cPJ.PRJobState = cPJ.ePRJObState.Processing;
                    break;
                case "ABORT":
                    m_cPJ = new cPJ();
                    m_cPJ.PRJobState = cPJ.ePRJObState.Aborted;
                    break;
                case "CANCLE":
                    m_cPJ = new cPJ();
                    m_cPJ.PRJobState = cPJ.ePRJObState.Stopped;
                    break;
            }
            SendAckPRJobCommand(nSysByte, m_cPJ.m_sPRJobID, 0, 0, "");
        }
        private void SendAckPRJobCommand(long nSysByte, string sPRJOBID, int nAck, eERRCODECJ nErrorCode = 0, string sErrorTXT = "")
        {
            long nObjectID = 0;
            m_XGem.MakeObject(ref nObjectID);
            m_XGem.SetListItem(nObjectID, 2);
            m_XGem.SetStringItem(nObjectID, sPRJOBID);
            m_XGem.SetListItem(nObjectID, 2);
            m_XGem.SetStringItem(nObjectID, nAck.ToString());
            if (nAck == 0) {
                m_XGem.SetListItem(nObjectID, 0);
            }
            else {
                m_XGem.SetListItem(nObjectID, 1);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, ((int)nErrorCode).ToString());
                m_XGem.SetStringItem(nObjectID, sErrorTXT);
            }
            m_XGem.SendSECSMessage(nObjectID, 16, 6, nSysByte);
        }

        private void CTRLJobCreate(long nObjectId, long nSysbyte)
        {
            eERRCODECJ eErrorCode = eERRCODECJ.NO_ERROR;
            string sErrorTxt = "";
            long nItemCount = 0;
            long nItemCount2 = 0;
            long nItemCount3 = 0;
            string sTemp = "";
            long nTemp = 0;
            long nAckObjectID = 0;
            string sCMD = "";
            m_XGem.MakeObject(ref nAckObjectID);

            m_XGem.GetListItem(nObjectId, ref nItemCount);
            m_XGem.SetListItem(nAckObjectID, nItemCount);

            m_XGem.GetStringItem(nObjectId, ref sTemp); //OBJSPEC
            m_XGem.SetStringItem(nAckObjectID, sTemp);

            m_XGem.GetStringItem(nObjectId, ref sTemp);      //OBJTYPE

            m_XGem.GetListItem(nObjectId, ref nItemCount);
            m_XGem.SetListItem(nAckObjectID, nItemCount);

            for (int i = 0; i < nItemCount; i++) {
                m_XGem.GetListItem(nObjectId, ref nTemp);
                m_XGem.SetListItem(nAckObjectID, nTemp);
                m_XGem.GetStringItem(nObjectId, ref sCMD);
                m_XGem.SetStringItem(nAckObjectID, sCMD);
                if (sCMD.IndexOf(" ") >= 0)
                    sCMD = sCMD.Substring(0, sCMD.IndexOf(" "));
                switch (sCMD) {
                    case "OBJID":
                        //if (m_cCJ.sCTRLJobID != "" && eErrorCode == eERRCODECJ.NO_ERROR)
                        //{

                        //    m_XGem.GetStringItem(nObjectId, ref sTemp);
                        //    m_XGem.SetStringItem(nAckObjectID, sTemp);
                        //    sErrorTxt = "Already Exist Control Job";
                        //    eErrorCode = eERRCODECJ.ObjectIdentifier_InUse;
                        //}
                        //else
                        //{5
                        m_XGem.GetStringItem(nObjectId, ref m_cCJ.sCTRLJobID);
                        m_XGem.SetStringItem(nAckObjectID, m_cCJ.sCTRLJobID);
                        //}
                        break;
                    case "CARRIERINPUTSPEC":
                        m_XGem.GetListItem(nObjectId, ref nTemp);
                        m_XGem.SetListItem(nAckObjectID, nTemp);

                        m_XGem.GetStringItem(nObjectId, ref m_cCJ.sCarrierID);
                        m_XGem.SetStringItem(nAckObjectID, m_cCJ.sCarrierID);
                        break;
                    case "PROCESSINGCTRLSPEC":
                        m_XGem.GetListItem(nObjectId, ref nTemp);
                        m_XGem.SetListItem(nAckObjectID, nTemp);

                        m_XGem.GetStringItem(nObjectId, ref sTemp);
                        m_XGem.SetStringItem(nAckObjectID, sTemp);
                        if (sTemp.IndexOf(" ") >= 0)
                            m_cCJ.sPRJOBID = sTemp.Substring(0, sTemp.IndexOf(" "));
                        else
                            m_cCJ.sPRJOBID = sTemp;
                        //if (m_cPJ.m_sPRJobID != m_cCJ.sPRJOBID && eErrorCode == eERRCODECJ.NO_ERROR)
                        //{
                        //    sErrorTxt = "Does Not Match PRJOB ID";
                        //    eErrorCode = eERRCODECJ.Invalid_AttibuteValue;
                        //}
                        break;
                    case "PRPROCESSSTART":
                        m_XGem.GetStringItem(nObjectId, ref m_cCJ.m_sAutoStart);
                        m_XGem.SetStringItem(nAckObjectID, m_cCJ.m_sAutoStart);
                        //if (m_cCJ.m_sAutoStart != "T" && m_cCJ.m_sAutoStart != "F" && eErrorCode == eERRCODECJ.NO_ERROR)
                        //{
                        //    sErrorTxt = "PRPROCESSSTART Param Error";
                        //    eErrorCode = eERRCODECJ.Invalid_AttibuteValue;
                        //}
                        break;
                    case "MTRLOUTSPEC":
                        m_XGem.GetListItem(nObjectId, ref nTemp);
                        m_XGem.SetListItem(nAckObjectID, nTemp);

                        m_XGem.GetListItem(nObjectId, ref nItemCount2);
                        m_XGem.SetListItem(nAckObjectID, nItemCount2);

                        for (int n = 0; n < nItemCount2; n++) {
                            m_XGem.GetListItem(nObjectId, ref nTemp);
                            m_XGem.SetListItem(nAckObjectID, nTemp - 1);

                            if (n == 0) {
                                m_XGem.GetStringItem(nObjectId, ref sTemp);
                                m_XGem.SetStringItem(nAckObjectID, sTemp);
                                //if (sTemp != m_cCJ.sCarrierID && eErrorCode == eERRCODECJ.NO_ERROR)
                                //{
                                //    eErrorCode = eERRCODECJ.Invalid_AttibuteValue;
                                //    sErrorTxt = "Carrier ID Not Matched";
                                //}
                                m_XGem.GetStringItem(nObjectId, ref sTemp);
                                //m_XGem.SetStringItem(nAckObjectID, sTemp);
                                m_XGem.GetListItem(nObjectId, ref nItemCount3);
                                m_XGem.SetListItem(nAckObjectID, nItemCount3);
                                for (int p = 0; p < nItemCount3; p++) {
                                    m_XGem.GetStringItem(nObjectId, ref sTemp);
                                    m_XGem.SetStringItem(nAckObjectID, sTemp);
                                }
                            }
                            else {
                                m_XGem.GetStringItem(nObjectId, ref sTemp);
                                m_XGem.SetStringItem(nAckObjectID, sTemp);
                                m_XGem.GetStringItem(nObjectId, ref sTemp);
                                //m_XGem.SetStringItem(nAckObjectID, sTemp);
                                m_XGem.GetListItem(nObjectId, ref nItemCount3);
                                m_XGem.SetListItem(nAckObjectID, nItemCount3);
                                for (int p = 0; p < nItemCount3; p++) {
                                    m_XGem.GetStringItem(nObjectId, ref sTemp);
                                    m_XGem.SetStringItem(nAckObjectID, sTemp);
                                }
                            }
                        }
                        break;
                    default:
                        WriteLog("CTRLJobCread(S14F9) Not Define MSG : " + sTemp);
                        break;
                }
            }
            m_XGem.SetListItem(nAckObjectID, 2);
            if (eErrorCode == eERRCODECJ.NO_ERROR) {
                m_XGem.SetStringItem(nAckObjectID, "0");
                m_XGem.SetListItem(nAckObjectID, 0);
            }
            else {
                m_XGem.SetStringItem(nAckObjectID, "1");
                m_XGem.SetListItem(nAckObjectID, 1);
                m_XGem.SetListItem(nAckObjectID, 2);
                m_XGem.SetStringItem(nAckObjectID, ((int)eErrorCode).ToString());
                m_XGem.SetStringItem(nAckObjectID, sErrorTxt);
                if (eErrorCode != eERRCODECJ.ObjectIdentifier_InUse)
                    m_cCJ = new cCJ();
            }
            m_XGem.SendSECSMessage(nAckObjectID, 14, 10, nSysbyte);
        }
        private void CTRLJobCommandRecieve(long nObjectID, long SysByte)
        {
            long ItemCount = 0;
            string sTemp = "";
            int nAck = 0;
            long nTemp = 0;
            string sCMD = "";
            m_XGem.GetListItem(nObjectID, ref nTemp);
            m_XGem.GetStringItem(nObjectID, ref sTemp);
            m_XGem.GetStringItem(nObjectID, ref sCMD);
            if (sCMD.IndexOf(" ") >= 0)
                sCMD = sCMD.Substring(0, sCMD.IndexOf(" "));
            switch (sCMD) {
                case "1"://"CJStart":

                    //if (m_cCJ.sCTRLJobID != "")
                    //    SendEvent_SamSung_ELEC(eCEID_NO.e_CTRLJob_Excuting);
                    SetEQPState(eEQPState_SSEM.EQ_RUN);
                    m_cCJ.CTRLJobState = cCJ.eCTRLJobState.Excuting;
                    break;
                case "2"://"CJPause":

                    SetEQPState(eEQPState_SSEM.EQ_IDLE);
                    //SendEvent_SamSung_ELEC(eCEID_NO.e_CTRLJob_WaitSTart);
                    m_cCJ.CTRLJobState = cCJ.eCTRLJobState.Paused;
                    break;
                case "3"://"CJResume":

                    //if (m_cCJ.sCTRLJobID != "")
                    //    SendEvent_SamSung_ELEC(eCEID_NO.e_CTRLJob_Excuting);
                    SetEQPState(eEQPState_SSEM.EQ_RUN);
                    m_cCJ.CTRLJobState = cCJ.eCTRLJobState.Excuting;
                    break;
                case "4"://"CJCancel":
                    m_cCJ = new cCJ();

                    SetEQPState(eEQPState_SSEM.EQ_IDLE);
                    m_cCJ.CTRLJobState = cCJ.eCTRLJobState.Paused;
                    break;
                case "5"://"CJDeselects":
                    m_cCJ = new cCJ();
                    SetEQPState(eEQPState_SSEM.EQ_IDLE);
                    m_cCJ.CTRLJobState = cCJ.eCTRLJobState.Paused;
                    break;
                case "6":// "CJStop":

                    SetEQPState(eEQPState_SSEM.EQ_IDLE);
                    //SendEvent_SamSung_ELEC(eCEID_NO.e_CTRLJob_WaitSTart);
                    m_cCJ.CTRLJobState = cCJ.eCTRLJobState.WaitingForStart;
                    break;
                case "7"://"CJAbort":
                    m_cCJ = new cCJ();

                    SetEQPState(eEQPState_SSEM.EQ_IDLE);
                    m_cCJ.CTRLJobState = cCJ.eCTRLJobState.Paused;
                    break;
                case "8"://"CJHOQ":
                    m_cCJ.CTRLJobState = cCJ.eCTRLJobState.Paused;
                    break;
                default:
                    break;
            }
            m_XGem.GetListItem(nObjectID, ref ItemCount);
            for (int i = 0; i < ItemCount; i++) {
                m_XGem.GetStringItem(nObjectID, ref sTemp);
            }
            SendAckCTRLJobCommand(nAck, SysByte);
        }

        private void SendAckCTRLJobCommand(int nAck, long nSystemByte, eERRCODECJ eErrorCode = eERRCODECJ.NO_ERROR, string nErrorTXT = "")     // True False
        {
            long nObjectID = 0;
            m_XGem.MakeObject(ref nObjectID);
            m_XGem.SetListItem(nObjectID, 2);
            m_XGem.SetStringItem(nObjectID, nAck.ToString());
            if (eErrorCode == eERRCODECJ.NO_ERROR) {
                m_XGem.SetListItem(nObjectID, 0);
            }
            else {
                m_XGem.SetListItem(nObjectID, 1);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, ((int)eErrorCode).ToString());
                m_XGem.SetStringItem(nObjectID, nErrorTXT);
            }
            m_XGem.SendSECSMessage(nObjectID, 16, 28, nSystemByte);
        }

        private void ECID_Request(long nObjectId, long Sysbyte)
        {
            long nItemCount = 0;
            string nTemp = "0";

            m_XGem.GetListItem(nObjectId, ref nItemCount);
            m_XGem.GetStringItem(nObjectId, ref nTemp);
            m_XGem.GetListItem(nObjectId, ref nItemCount);
            long[] nECID;
            if (nItemCount == 0) {          // 모든 ECID Report
                nECID = new long[Enum.GetNames(typeof(eECV_No)).Length];
                for (int i = 0; i < Enum.GetNames(typeof(eECV_No)).Length; i++) {
                    nECID[i] = (long)m_nECV[i];
                }
            }
            else {
                nECID = new long[nItemCount];
                for (int i = 0; i < nItemCount; i++) {
                    m_XGem.GetStringItem(nObjectId, ref nTemp);
                    nECID[i] = Convert.ToInt32(nTemp);
                }
            }
            SendECVDataToHost(Sysbyte, nECID);
        }
        private void SendECVDataToHost(long SysByte, long[] sECIDList)
        {
            long nObjectID = 0;
            string[] sNames = new string[sECIDList.Length];
            string[] sDefs = new string[sECIDList.Length];
            string[] sMins = new string[sECIDList.Length];
            string[] sMaxs = new string[sECIDList.Length];
            string[] sUnits = new string[sECIDList.Length];

            m_XGem.MakeObject(ref nObjectID);
            m_XGem.SetListItem(nObjectID, sECIDList.Length);
            long rt = m_XGem.GEMGetECVInfo((long)(sECIDList.Length), sECIDList, ref sNames, ref sDefs, ref sMins, ref sMaxs, ref sUnits);            //KJWKJW
            if (sDefs[0] == null) {
                m_XGem.CloseObject(nObjectID);
                m_XGem.MakeObject(ref nObjectID);
                m_XGem.SetListItem(nObjectID, 0);
            }
            else {
                for (int i = 0; i < sECIDList.Length; i++) {
                    m_XGem.SetStringItem(nObjectID, sDefs[i]);
                }
            }
            m_XGem.SendSECSMessage(nObjectID, 2, 14, SysByte);
        }
        private void ECID_ConstantRequest(long nObjectID, long Sysbyte)
        {
            long nItemCount = 0;
            string nTemp = "";

            m_XGem.GetListItem(nObjectID, ref nItemCount);
            m_XGem.GetStringItem(nObjectID, ref nTemp);
            m_XGem.GetListItem(nObjectID, ref nItemCount);
            long[] nECID;

            if (nItemCount == 0) {
                nECID = new long[Enum.GetNames(typeof(eECV_No)).Length];
                for (int i = 0; i < Enum.GetNames(typeof(eECV_No)).Length; i++) {
                    nECID[i] = m_nECV[i];
                }
            }
            else {
                nECID = new long[nItemCount];
                for (int i = 0; i < nItemCount; i++) {
                    m_XGem.GetStringItem(nObjectID, ref nTemp);
                    nECID[i] = Convert.ToInt32(nTemp);
                }
            }
            SendECVConstantDataToHost(Sysbyte, nECID);
        }
        private void SendECVConstantDataToHost(long SysByte, long[] sECIDList)
        {
            long nObjectID = 0;
            string[] sNames = new string[sECIDList.Length];
            string[] sDefs = new string[sECIDList.Length];
            string[] sMins = new string[sECIDList.Length];
            string[] sMaxs = new string[sECIDList.Length];
            string[] sUnits = new string[sECIDList.Length];

            long rt = m_XGem.GEMGetECVInfo((long)(sECIDList.Length), sECIDList, ref sNames, ref sDefs, ref sMins, ref sMaxs, ref sUnits);
            if (sNames[0] != null) {
                m_XGem.MakeObject(ref nObjectID);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, m_EQPID);
                m_XGem.SetListItem(nObjectID, sECIDList.Length);
                for (int i = 0; i < sECIDList.Length; i++) {
                    m_XGem.SetListItem(nObjectID, 6);
                    m_XGem.SetStringItem(nObjectID, sECIDList[i].ToString());
                    m_XGem.SetStringItem(nObjectID, sNames[i]);
                    m_XGem.SetStringItem(nObjectID, sMins[i]);
                    m_XGem.SetStringItem(nObjectID, sMaxs[i]);
                    m_XGem.SetStringItem(nObjectID, sDefs[i]);
                    m_XGem.SetStringItem(nObjectID, sUnits[i]);
                }
            }
            else {
                m_XGem.MakeObject(ref nObjectID);
                m_XGem.SetListItem(nObjectID, 0);
            }
            m_XGem.SendSECSMessage(nObjectID, 2, 30, SysByte);
        }

        private void RecieveRemoteCommand(long nObjectID, long nSysbyte)
        {
            long nTemp = 0;
            string sRCMD = "";
            string sTemp = "";
            m_XGem.GetListItem(nObjectID, ref nTemp);
            m_XGem.GetStringItem(nObjectID, ref sRCMD);

            switch ((eRCMD)(Convert.ToInt32(sRCMD))) {
                case eRCMD.JOB_START:
                    //m_eMainCycle = m_eLanstCycle;
                    //if (m_eLanstCycle == eMainCycle.Check_VisionInspDone) {
                    //    if (OnRecive_EQStart != null)
                    //        OnRecive_EQStart();
                    //}
                    //if (m_cPJ.m_sPRJobID != "")
                    //    SendEvent_SSEM(eCEID_NO.e_PRJob_Processing);
                    //m_cPJ.PRJobState = cPJ.ePRJObState.Processing;
                    SendRemoteCommandReq(sRCMD, 0, nSysbyte);
                    break;
                case eRCMD.JOB_STOP:
                    //m_eLanstCycle = m_eMainCycle;
                    //if (m_eLanstCycle == eMainCycle.Check_VisionInspDone) {
                    //    if (OnRecive_EQStop != null)
                    //        OnRecive_EQStop();
                    //}
                    //SetEQPState(eEQPState_SSEM.EQ_IDLE);
                    //SendEvent_SSEM(eCEID_NO.e_PRJob_WaitSTart);
                    //m_cPJ.PRJobState = cPJ.ePRJObState.Stopped;
                    SendRemoteCommandReq(sRCMD, 0, nSysbyte);
                    break;
                case eRCMD.JOB_ABORT:
                    //m_cCJ = new cCJ();
                    //m_eMainCycle = eMainCycle.WaitPRJOBCreate;
                    //SetEQPState(eEQPState_SSEM.EQ_IDLE);
                    //m_cPJ.PRJobState = cPJ.ePRJObState.Aborted;
                    SendRemoteCommandReq(sRCMD, 0, nSysbyte);
                    break;
                case eRCMD.JOB_CANCLE:
                    //m_cCJ = new cCJ();
                    //m_eMainCycle = eMainCycle.WaitPRJOBCreate;
                    //m_cPJ.PRJobState = cPJ.ePRJObState.Stopped;
                    SendRemoteCommandReq(sRCMD, 0, nSysbyte);
                    break;
                case eRCMD.JOB_PAUSE:
                    //m_eLanstCycle = m_eMainCycle;
                    //if (m_eLanstCycle == eMainCycle.Check_VisionInspDone) {
                    //    if (OnRecive_EQStop != null)
                    //        OnRecive_EQStop();
                    //}
                    //SetEQPState(eEQPState_SSEM.EQ_IDLE);
                    //SendEvent_SSEM(eCEID_NO.e_PRJob_WaitSTart);
                    //m_cPJ.PRJobState = cPJ.ePRJObState.Paused;
                    SendRemoteCommandReq(sRCMD, 0, nSysbyte);
                    break;
                case eRCMD.JOB_RESUME:
                    //m_eMainCycle = m_eLanstCycle;
                    //if (m_eLanstCycle == eMainCycle.Check_VisionInspDone) {
                    //    if (OnRecive_EQStart != null)
                    //        OnRecive_EQStart();
                    //}
                    //if (m_cPJ.m_sPRJobID != "")
                    //    SendEvent_SSEM(eCEID_NO.e_PRJob_Processing);
                    //SetEQPState(eEQPState_SSEM.EQ_RUN);
                    //m_cPJ.PRJobState = cPJ.ePRJObState.Processing;
                    SendRemoteCommandReq(sRCMD, 0, nSysbyte);
                    break;
                case eRCMD.PPSELECT:
                    string sRecipe = "";
                    bool nExistRecipe = false;
                    m_XGem.GetListItem(nObjectID, ref nTemp);
                    m_XGem.GetListItem(nObjectID, ref nTemp);
                    m_XGem.GetStringItem(nObjectID, ref sTemp);
                    m_XGem.GetStringItem(nObjectID, ref sRecipe);
                    if (sRecipe.IndexOf(" ") >= 0)
                        sRecipe = sRecipe.Substring(0, sRecipe.IndexOf(" "));

                    for (int i = 0; i < m_cRecipeList.nRecipeNum; i++) {
                        if (m_cRecipeList.sName[i] == sRecipe)
                            nExistRecipe = true;
                    }
                    if (nExistRecipe) {
                        m_cPJ.m_sRecipeID = sRecipe;
                        OnRecipeSelected(sRecipe);
                        SendRemoteCommandReq(sRCMD, 0, nSysbyte);
                        WriteLog("PPSELECTED : " + sRecipe);
                    }
                    else {
                        SendRemoteCommandReq(sRCMD, 3, nSysbyte);
                        WriteLog("Recipe Does not Exist");
                    }
                    break;
                case eRCMD.OPCALL:
                    string[] sOPCALL = new string[2] { "", "" };
                    m_XGem.GetListItem(nObjectID, ref nTemp);
                    m_XGem.GetListItem(nObjectID, ref nTemp);
                    m_XGem.GetStringItem(nObjectID, ref sTemp);
                    m_XGem.GetStringItem(nObjectID, ref sOPCALL[0]);
                    m_XGem.GetStringItem(nObjectID, ref sOPCALL[1]);
                    OnRecieveTerminalMultiMsg(sOPCALL);
                    SendRemoteCommandReq(sRCMD, 0, nSysbyte);
                    break;
                default:
                    SendRemoteCommandReq(sRCMD, 1, nSysbyte);
                    WriteLog("RCMD Does not Exist : " + sRCMD);
                    break;
            }

        }
        private void RecieveTraceInit(long nObjectId, long nSysbyte)
        {
            long nTemp = 0;
            string sTemp = "";
            string sVID = "";
//            long nItemIndex = 0;
            long ret = 0;


            for (int i = 0; i < Enum.GetNames(typeof(eVID_NO)).Length; i++)
                m_nTraceVID[i] = -1;


            m_XGem.GetListItem(nObjectId, ref nTemp);
            m_XGem.GetStringItem(nObjectId, ref sTemp);
            m_sTRID = sTemp;
            m_XGem.GetStringItem(nObjectId, ref sTemp);
            //m_nDSPER = Convert.ToInt32(sTemp);
            m_nDSPER = Convert.ToInt32(sTemp.Substring(2, 2)) * 60 + Convert.ToInt32(sTemp.Substring(4, 2));
            m_nDSPER = m_nDSPER * 100;
            m_XGem.GetStringItem(nObjectId, ref sTemp);
            m_nTOTSMP = Convert.ToInt32(sTemp);
            if (m_nTOTSMP == 0)
                m_nTOTSMP = -1; // 무한보고
            m_XGem.GetStringItem(nObjectId, ref sTemp);
            nTraceGroupNum = Convert.ToInt32(sTemp);
            m_XGem.GetListItem(nObjectId, ref nTemp);
            m_nTraceNum = (int)nTemp;
            if (nTemp == 0) {
                m_nTOTSMP = -2;
            }
            else {
                for (int i = 0; i < nTemp; i++) {
                    m_XGem.GetStringItem(nObjectId, ref sVID);
                    m_nTraceVID[i] = Convert.ToInt32(sVID);
                }
                string[] psval = new string[m_nTraceNum];
                ret = m_XGem.GEMGetVariable(m_nTraceNum, ref m_nTraceVID, ref psval);

            }
            m_swTrace.Start();
            long nAckObjectId = 0;
            m_XGem.MakeObject(ref nAckObjectId);
            if (ret == 0)
                m_XGem.SetStringItem(nAckObjectId, "0");
            else
                m_XGem.SetStringItem(nAckObjectId, "4");
            m_XGem.SendSECSMessage(nAckObjectId, 2, 24, nSysbyte);
            m_nSMPLN = 1;
            nSendTraceFlag = 1;
            //long nTemp = 0;
            //string sTemp = "";
            //string sVID = "";

            //for (int i = 0; i < Enum.GetNames(typeof(eVID_NO)).Length; i++)
            //    m_nTraceVID[i] = -1;


            //m_XGem.GetListItem(nObjectId, ref nTemp);
            //m_XGem.GetStringItem(nObjectId, ref sTemp);
            //m_sTRID = sTemp;
            //m_XGem.GetStringItem(nObjectId, ref sTemp);
            //m_nDSPER = Convert.ToInt32(sTemp);
            //m_XGem.GetStringItem(nObjectId, ref sTemp);
            //m_nTOTSMP = Convert.ToInt32(sTemp);
            //if (m_nTOTSMP == 0)
            //    m_nTOTSMP = -1; // 무한보고
            //m_XGem.GetStringItem(nObjectId, ref sTemp);
            //nTraceGroupNum = Convert.ToInt32(sTemp);
            //m_XGem.GetListItem(nObjectId, ref nTemp);
            //m_nTraceNum = (int)nTemp;
            //if (nTemp == 0) {
            //    m_nTOTSMP = -2;
            //}
            //else {
            //    for (int i = 0; i < nTemp; i++) {
            //        m_XGem.GetStringItem(nObjectId, ref sVID);
            //        m_nTraceVID[i] = Convert.ToInt32(sVID);
            //    }
            //}
            //m_swTrace.Start();
            //long nAckObjectId = 0;
            //m_XGem.MakeObject(ref nAckObjectId);
            //m_XGem.SetStringItem(nAckObjectId, "0");
            //m_XGem.SendSECSMessage(nAckObjectId, 2, 24, nSysbyte);
            //m_nSMPLN = 1;
            //nSendTraceFlag = 1;
        }


        private long nTraceObjectID = 0;
        private int nSendTraceFlag = 1;
        private int nTraceGroupNum = 0;
        private string[] sValue;
        private int m_nSMPLN = 0;
        public void SendTraceData()
        {
            sValue = new string[m_nTraceNum];

            if (m_nTOTSMP > 0 || m_nTOTSMP == -1) {
                if (m_swTrace.Check() > m_nDSPER * 10) {
                    m_swTrace.Start();
                    if (nSendTraceFlag == 1) {
                        m_XGem.MakeObject(ref nTraceObjectID);
                        m_XGem.SetListItem(nTraceObjectID, 4);
                        m_XGem.SetStringItem(nTraceObjectID, m_sTRID);
                        m_XGem.SetStringItem(nTraceObjectID, m_nSMPLN.ToString());
                        m_nSMPLN++;
                        long[] nTemp = new long[1] { 1 };
                        string[] sTime = new string[1] { "" };
                        m_XGem.GEMGetVariable(1, ref nTemp, ref sTime);
                        m_XGem.SetStringItem(nTraceObjectID, sTime[0]);
                        m_XGem.SetListItem(nTraceObjectID, m_nTraceNum * nTraceGroupNum);
                    }

                    m_XGem.GEMGetVariable(m_nTraceNum, ref m_nTraceVID, ref sValue);
                    for (int i = 0; i < m_nTraceNum; i++) {
                        m_XGem.SetListItem(nTraceObjectID, 2);
                        m_XGem.SetStringItem(nTraceObjectID, m_nTraceVID[i].ToString());
                        m_XGem.SetStringItem(nTraceObjectID, sValue[i]);
                    }

                    if (nSendTraceFlag == nTraceGroupNum) {
                        m_XGem.SendSECSMessage(nTraceObjectID, 6, 1, 0);
                        nSendTraceFlag = 1;
                        if (m_nTOTSMP > 0)
                            m_nTOTSMP--;
                    }
                    else
                        nSendTraceFlag++;
                }
            }
        }

        private void SendRemoteCommandReq(string sRCMD, int Ack, long nSysbyte)
        {
            long nObjectID = 0;
            m_XGem.MakeObject(ref nObjectID);
            m_XGem.SetListItem(nObjectID, 2);
            m_XGem.SetStringItem(nObjectID, sRCMD);
            m_XGem.SetStringItem(nObjectID, Ack.ToString());
            m_XGem.SendSECSMessage(nObjectID, 2, 42, nSysbyte);
            WriteLog("Send Ack RCMD : " + sRCMD + " ACK :" + Ack.ToString());
        }
        private void RecievePPFormatRequest(string sPPID, long SysByte)
        {
            long nObjectID = 0;
            int Sel = -1;

            m_XGem.MakeObject(ref nObjectID);
            m_XGem.SetListItem(nObjectID, 4);
            m_XGem.SetStringItem(nObjectID, sPPID);
            m_XGem.SetStringItem(nObjectID, m_sMLDN);
            m_XGem.SetStringItem(nObjectID, m_sSoftrev);
            if (sPPID.IndexOf(" ") >= 0)
                sPPID = sPPID.Substring(0, sPPID.IndexOf(" "));
            for (int i = 0; i < m_cRecipeList.nRecipeNum; i++) {
                if (m_cRecipeList.sName[i] == sPPID)
                    Sel = i;
            }
            if (Sel != -1) {
                m_XGem.SetListItem(nObjectID, 1);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, " ");
                m_XGem.SetListItem(nObjectID, 10);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, "1");
                m_XGem.SetStringItem(nObjectID, m_cRecipeList.sRedLight[Sel]);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, "2");
                m_XGem.SetStringItem(nObjectID, m_cRecipeList.sBlueLight[Sel]);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, "3");
                m_XGem.SetStringItem(nObjectID, m_cRecipeList.sGreenLight[Sel]);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, "4");
                m_XGem.SetStringItem(nObjectID, m_cRecipeList.sCoaxLight[Sel]);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, "5");
                m_XGem.SetStringItem(nObjectID, m_cRecipeList.sSideLight[Sel]);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, "6");
                m_XGem.SetStringItem(nObjectID, m_cRecipeList.sAlignStep[Sel]);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, "7");
                m_XGem.SetStringItem(nObjectID, m_cRecipeList.sAlignRange[Sel]);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, "8");
                m_XGem.SetStringItem(nObjectID, m_cRecipeList.sOverlaySearchStart[Sel]);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, "9");
                m_XGem.SetStringItem(nObjectID, m_cRecipeList.sOverlaySearchLength[Sel]);
                m_XGem.SetListItem(nObjectID, 2);
                m_XGem.SetStringItem(nObjectID, "10");
                m_XGem.SetStringItem(nObjectID, m_cRecipeList.sOverlayThreshold[Sel]);
                //m_XGem300.SetStringItem(nObjectID, "RedLight");
                //m_XGem300.SetStringItem(nObjectID, m_cRecipeList.sRedLight[Sel]);
                //m_XGem300.SetListItem(nObjectID, 2);
                //m_XGem300.SetStringItem(nObjectID, "BlueLight");
                //m_XGem300.SetStringItem(nObjectID,m_cRecipeList.sBlueLight[Sel]);
                //m_XGem300.SetListItem(nObjectID, 2);
                //m_XGem300.SetStringItem(nObjectID, "GreenLight");
                //m_XGem300.SetStringItem(nObjectID, m_cRecipeList.sGreenLight[Sel]);
                //m_XGem300.SetListItem(nObjectID, 2);
                //m_XGem300.SetStringItem(nObjectID, "CoaxLight");
                //m_XGem300.SetStringItem(nObjectID, m_cRecipeList.sCoaxLight[Sel]);
                //m_XGem300.SetListItem(nObjectID, 2);
                //m_XGem300.SetStringItem(nObjectID, "SideLight");
                //m_XGem300.SetStringItem(nObjectID, m_cRecipeList.sSideLight[Sel]);
                //m_XGem300.SetListItem(nObjectID, 2);
                //m_XGem300.SetStringItem(nObjectID, "AlignStep");
                //m_XGem300.SetStringItem(nObjectID, m_cRecipeList.sAlignStep[Sel]);
                //m_XGem300.SetListItem(nObjectID, 2);
                //m_XGem300.SetStringItem(nObjectID, "AlignRange");
                //m_XGem300.SetStringItem(nObjectID, m_cRecipeList.sAlignRange[Sel]);
                //m_XGem300.SetListItem(nObjectID, 2);
                //m_XGem300.SetStringItem(nObjectID, "OverlaySearchStart");
                //m_XGem300.SetStringItem(nObjectID,m_cRecipeList.sOverlaySearchStart[Sel]);
                //m_XGem300.SetListItem(nObjectID, 2);
                //m_XGem300.SetStringItem(nObjectID, "OverlaySearchLenght");
                //m_XGem300.SetStringItem(nObjectID, m_cRecipeList.sOverlaySearchLength[Sel]);
                //m_XGem300.SetListItem(nObjectID, 2);
                //m_XGem300.SetStringItem(nObjectID, "OverlayThreshold");
                //m_XGem300.SetStringItem(nObjectID, m_cRecipeList.sOverlayThreshold[Sel]);

            }
            else {
                m_XGem.SetListItem(nObjectID, 0);
            }
            m_XGem.SendSECSMessage(nObjectID, 7, 26, SysByte);
        }


    }
}
