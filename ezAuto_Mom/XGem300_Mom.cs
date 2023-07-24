﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WeifenLuo.WinFormsUI.Docking;
using GEM_XGem300Pro;
using ezTools;
using System.IO;

namespace ezAutoMom
{
    public partial class XGem300_Mom : DockContent
    {
        public enum eCommunicate
        {
            DISABLE = 1,
            WAITCR = 2,
            WAITDELAY = 3,
            WAITCRA = 4,
            COMMUNICATING = 5,
        }
        public eCommunicate m_eComm = eCommunicate.DISABLE;
        bool m_bXGemEnable = false;

        public enum eControl
        {
            OFFLINE = 1,
            ATTEMPTONLINE = 2,
            HOSTOFFLINE = 3,
            LOCAL = 4,
            ONLINEREMOTE = 5,
        }
        public eControl m_eControl = eControl.OFFLINE;

        public enum eXGemModule
        {
            INIT = 0,
            IDLE,
            SETUP,
            READY,
            EXCUTE,
        }
        public eXGemModule m_eXGemModule = eXGemModule.INIT;

        public enum eRecipeChangeMode
        {
            Create = 1,
            Edit,
            Delete,
        }

        public enum eXGemError
        {
            None = 0,
            ALREADY_STARTED = -10001,               // Already initialized 이미 XGem300Pro 이 초기화되어 있는데 다시 불린 상태입니다.
            INVALID_THREAD = -10002,                // Socket is not initialized.
            INVALID_DOMDOC = -10003,                // Fail to load xml. XGem300Pro Process 로 부터 xml format이 아닌 message를 받았을 때 발생합니다.
            ATTIBUTE_INVALID = -10004,              // Fail to invalid attribute. XGem300Pro Process 로부터 받은 message의 attribute가 맞지 않을 경우 발생합니다.
            INVALID_COMMAND = -10005,               // Fail to invalid command. XGem300Pro Process 로부터 invalid message를 받았을 경우 발생합니다.
            CREATE_FAILED_DOMDOC = -10006,          // Fail to initiate DomDocument Dom 생성 실패 시 발생합니다.
            ARGUMENT_VALUE_INVALID = -10007,        // Invalid argument value. 함수의 인자의 값이 유효하지 않은 값 입니다. 인자의 값을 확인하시기 바랍니다.
            NOW_NOTCONNECTED_WITH_XGEM = -10008,    // Not ready XGem. XGem300Pro state 가 execute 가 아닌 상태에서 함수가 호출되었습니다.
            NOT_INITIALIZED = -10009,               // Not initialized yet. XGem300Pro 이 아직 초기화 되어 있지 않은 상태입니다. Initialize() 함수가 실패했거나 불리지 않은 상태입니다.
            NOT_STARTED = -10010,                   // Not started yet. XGem300Pro 의 내부 프로세스가 아직 시작되지 않은 상태입니다. Start()가 호출되지 않았거나 실패한 상태입니다.
            NOT_CONNECTED = -10011,                 // Not used.
            READ_CONFIG = -10012,                   // Fail to read config file. cfg 파일을 읽기 실패하였습니다. cfg 파일의 경로와 item 을 확인하기 바랍니다.
            INVALID_XVALUE = -10013,                // Invalid message format. Complex  type 의  message 를XGem300Pro Process 로 전송 시 발생하며 invalid message format 을 전송하려고 할 때 발생합니다.
            FAIL_DELETE_COMPLEXLIST = -10014,       // Not used.
            ITEMS_NOT_FOUNDED = -10015,             // Not found item. data item 을 찾을 수 없습니다.
            ITEMTYPE_MISMATCH = -10016,             // Mismatch item type 얻고자 하는 data type과 맞지 않습니다.
            INVALID_ITEMCOUNT = -10017,             // Mismatch item count. item 의 개수가 맞지 않습니다.
            INVALID_MSGID = -10018,                 // Invalid message.
            ARGUMENT_OUT_OF_RANGE = -10019,         // Argument is out of range 인자 값의 범위를 벗어 납니다.
            INVALID_PARAMETER = -10020,             // Invalid Parameter.유효하지 않은 Parameter name
            LICENSE_ERROR = -10021,                 // Not used.
            FAIL_TO_CREATE_WINDOW = -10022,         // Fail to create window. window 생성 실패했습니다. 회사로 문의 바랍니다.
            INVALID_RECVDATA = -10023,              // Not used.
            ATTIBUTE_MSGNAME_MISMATCH = -10024,     // Mismatch message name. 얻고자 하는 message name 과 XGem300Pro Process 에서 받은 message name 과 동일하지 않습니다.
            NOT_FILE_MEMORY = -10026,               // VID does not use file memory.
            INCLUDE_FILE_MEMORY = -10027,           // VID for using file memory is included.
            INVALID_VID = -10028,                   // invalid VID 유효하지 않은 VID.
            EXCEED_MAX_ITEM_SIZE = -10029,          // exceed maximum item size.
            CREATE_MUTEX = -10030,                  // Fail to create mutex.
            INVALID_SECS2_MSG = -10031,             // invalid SECS2 message. SECS2 message format error.
            FAIL_DELETE_SECSMSGLIST = -10032,       // Fail to delete msg list
            CREATE_EVENT = -10033,                  // Fail to create API’s event handle.
            START_XGEM_PROCESS = -10034,            // Fail to start XGem300Pro process.
            READ_START_INFO = -30001,               // Fail to read startup information
            INITIALIZE_XCOM = -30002,               // Fail to initialize XCom
            INITIALIZE_EQCOMM = -30003,             // Fail to initialize EQComm
            INITIALIZE_EDACOMM = -30004,            // Fail to initialize EDAComm
            START_XCOM = -30005,                    // Fail to start XCom
            START_EQCOMM = -30006,                  // Fail to start EQComm
            START_EDACOMM = -30007,                 // Fail to start EDAComm
            DISCONNECTED_EQCOMM = -30008,           // disconnected with Equipment
            DISCONNECTED_EDACOMM = -30009,
            CONTROL_OFFLINE = -30010,               // Can't send message because controloffline.
            CONTROLSTATE_NOT_CHANGED = -30011,      // Control state can't be changed to ONLINE_LOCAL in OFFLINE state.
            FILE_NOT_CREATED = -30012,              // cfg, sml file not created
            OPEN_DATABASE = -30013,                 // Fail to open database
            EXECUTE_SQL = -30014,                   // Error in Execute SQL.
            OPEN_SQL = -30015,                      // Error in Open SQL.
            DISABLED_CEID = -30016,                 // Ceid disabled
            REPORT_FULL = -30017,                   // Report buffer is full.
            SPOOL_NOT_USE = -30018,                 // This system does not use spooling now.
            SPOOL_FULL = -30019,                    // Spool buffer is full.
            SPOOL_UNDEFINED = -30020,               // Spool undefined
            SPOOL_NOT_ACTIVE = -30021,              // Spool is not active, so message cannot spooling.
            OPERATION_XGEM200 = -30022,             // XGem300Pro is operating to 200mm spec, so message of 300mm spec can't send.
            FILE_NOT_EXIST = -30023,                // File does not exist.
            SHAREDMEM_START_POOLING = -30024,       // Failed to open/start shared memory.
            SHAREDMEM_STOP_POOLING = -30025,        // Failed to close/stop shared memory.
            CREATE_FAILED_DOMDOC2 = -30201,
            INVALID_DOMDOC2 = -30202,
            INVALID_COMMAND2 = -30203,
            ATTIBUTE_INVALID2 = -30204,
            INVALID_SET_COMAND = -30205,
            INVALID_VID2 = -30251,                  // VID does not exist.
            INVALID_ALID = -30252,                  // ALID does not exist.
            INVALID_CEID = -30253,                  // CEID does not exist.
            INVALID_RPTID = -30254,                 // RPTID does not exist.
            INVALID_LIMITV = -30255,                // Limit VID does not exist.
            INVALID_LIMITID = -30256,               // Limit ID does not exist.
            INVALID_DATAITEM = -30257,              // DataItem does not exist.
            INVALID_RCMD = -30258,                  // RCmd does not exist.
            INVALID_STREAM = -30259,                // Stream does not exist.
            INVALID_FUNCTION = -30260,              // Function does not exist.
            INVALID_SECSPARAMETERS = -30261,        // SECSParameters does not exist.
            INVALID_ERRORCODE = -30262,
            INVALID_ECID = -30263,                  // ECID does not exist.
            INVALID_FORMAT = -30264,                // Format is invalid.
            INVALID_CONFIGITEM = -30265,            // ConfigItem does not exist.
            INVALID_STRUCTURE = -30266,
            INVALID_PORTID = -30267,                // PORTID does not exist.
            INVALID_EQMSGID = -30268,
            INVALID_DATATYPE = -30269,              // Data Type is invalid.
            INVALID_PPID = -30270,                  // PPID does not exist.
            INVALID_STATE = -30271,                 // State is invalid.
            INVALID_TRID = -30272,                  // TRID does not exist
            NOT_FOUND_BUFFER = -30273,              // Buffer does not found.
            VALUE_OUT_OF_RANGE = -30274,            // Value is out of range.
            INVALID_SVID = -30275,                  // SVID does not exist.
        }
        eXGemError m_eXGemError = eXGemError.None;

        public enum eMSBAlarm
        {
            EQ_EmergencyStop_Error = 1,
            EQ_MainAirLow_Error,
            EQ_MainVacLow_Error,
            EQ_MCResetSignal_Error,
            EQ_DoorLockSignal_Error,
            EQ_FFUAlarm_Error,
            EQ_IonizerAlarm_Error,
            VS_LotStart_Error = 21,
            VS_LotEnd_Error,
            VS_TCPCmd_Error,
            LDP_Home_Erro = 31,
            LDP_Open_Error,
            LDP_Close_Error,
            LDP_Mapping_Error,
            LDP_Comm_Error,
            WTR_Home_Error = 41,
            WTR_GetWafer_Error,
            WTR_PutWafer_Error,
            WTR_InterLock_Error,
            WTR_Comm_Error,
            AL_Home_Error = 51,
            AL_WaferCheck_Error,
            AL_AlignFail_Error,
            FDC_CDA_Error = 61,
            FDC_VAC_Error,
            FDC_Press_Error,
            FDC_Temp_Error,
            FDC_ElecSensor_Error,
        }


        protected string m_id = "XGem";
        protected Auto_Mom m_auto;
        protected Log m_log;
        protected ezGrid m_grid;

        public XGem300ProNet m_XGem300;
        string m_sCfgPath = "C:\\AVIS\\INIT\\Gem300.cfg";

        long[] m_aID = new long[1] { 0 };
        string[] m_aValue = new string[1] { "Value" };
        string m_strStepID = ""; // ing 170306
        public bool[] bESReady = { false, false };
        public bool[] bHo_AVBLReady = { false, false };

        protected ArrayList m_aSV = new ArrayList();
        protected ArrayList m_aECV = new ArrayList();
        protected ArrayList m_aCEID = new ArrayList();
        ArrayList m_aALID = new ArrayList();

        protected int m_nLoadPort = 0;
        public XGem300Carrier[] m_aXGem300Carrier;
        public XGem300Process[] m_XGem300Process;
        public XGem300Control[] m_XGem300Control;

        protected virtual void MessageReceived(long nObjectID, long nStream, long nFunction, long nSysbyte) { }
        protected virtual void RemoteCommand(long nMsgID, string sRcmd, long nCount, string[] sCpNames, string[] sCpVals, ref long[] nCpAcks) { }
        protected virtual int ReqChangeECV(int nID, string sValue) { return 0; }

        public virtual void SetCEID_InspectDone(object obj) { }
        public virtual void SetCEID_LotStart(object obj) { }
        public virtual void SetCEID_LotEnd(object obj) { }

        public enum eSetSlotMap
        {
            Init,
            Send,
            Run,
            Cancel
        }
        public eSetSlotMap m_eSetSlotMap = eSetSlotMap.Init;

        public enum eSetCarrierID
        {
            Init,
            Send,
            Run,
            Cancel
        }
        public eSetCarrierID m_eSetCarrierIDState = eSetCarrierID.Init;

        protected Info_Carrier m_infoCarrierSlotMap;

        public virtual bool SetSlotMap(object obj)
        {
            m_infoCarrierSlotMap = (Info_Carrier)obj;

            string sLocID = "LP" + (m_infoCarrierSlotMap.m_nLoadPort + 1).ToString();
            string sCarrierID = m_infoCarrierSlotMap.m_strCarrierID;

            string sSlotMap = "";
            for (int n = 0; n < 25; n++) sSlotMap += GetSlotMapChar(m_infoCarrierSlotMap.m_infoWafer[n]);

            ezStopWatch sw = new ezStopWatch();
            m_eSetSlotMap = eSetSlotMap.Send;

            long a = m_XGem300.CMSSetSlotMap(sLocID, sSlotMap, sCarrierID, 0);
            m_log.Add("--> CMSSetSlotMap, " + sSlotMap + ", " + sCarrierID);
            while (m_eSetSlotMap == eSetSlotMap.Send && (sw.Check() < 5000)) Thread.Sleep(10);

            switch (m_eSetSlotMap)
            {
                case eSetSlotMap.Run: return true;
                case eSetSlotMap.Cancel:
                    m_log.Popup("CMSSetSlotMap Responce Error !!");
                    return false;
                case eSetSlotMap.Send:
                    m_log.Popup("CMSSetSlotMap has no Responce !!");
                    return false;
                default:
                    m_log.Popup("CMSSetSlotMap NotDefine Error !!");
                    return false;
            }
        }

        protected char GetSlotMapChar(Info_Wafer infoWafer)
        {
            switch (infoWafer.State)
            {
                case Info_Wafer.eState.Exist: return '3';
                default: return '1';
            }
        }

        public void SetLPInfo(int nLP, XGem300Carrier.eLPTransfer eTransferState, XGem300Carrier.ePortAccess eAccessMode, XGem300Carrier.ePortReservationState eReservationState, XGem300Carrier.ePortAssocitionState eAssociaionState, string sCarrierID)
        {
            if (m_aXGem300Carrier[nLP] != null)
            {
                XGem300Carrier CarrierInfo = m_aXGem300Carrier[nLP];
                CarrierInfo.m_eLPTransfer = eTransferState;
                CarrierInfo.m_ePortAccess = eAccessMode;
                CarrierInfo.m_eReservationState = eReservationState;
                CarrierInfo.m_eAssociationState = eAssociaionState;
                CarrierInfo.m_sCarrierID = sCarrierID;
                long nReturn = m_XGem300.CMSSetLPInfo(CarrierInfo.m_sLocID, (long)(CarrierInfo.m_eLPTransfer), (long)(CarrierInfo.m_ePortAccess), (long)(CarrierInfo.m_eReservationState), (long)(CarrierInfo.m_eAssociationState), CarrierInfo.m_sCarrierID);
                if (nReturn == 0)
                {
                    m_log.Add("--> CMSSetLPInfo - LOCID = " + CarrierInfo.m_sLocID + " TransferState = " + CarrierInfo.m_eLPTransfer.ToString() + " AccessMode = " + CarrierInfo.m_ePortAccess.ToString()
                        + " ReservationState = " + CarrierInfo.m_eReservationState.ToString() + " AssociationState = " + CarrierInfo.m_eAssociationState.ToString() + " CarrierID = " + CarrierInfo.m_sCarrierID);
                }
                else
                {
                    m_log.Add("--> CMSSetLPInfo Fail, ErrorCode = " + nReturn.ToString() + " - LOCID = " + CarrierInfo.m_sLocID + " TransferState = " + CarrierInfo.m_eLPTransfer.ToString() + " AccessMode = " + CarrierInfo.m_ePortAccess.ToString()
                        + " ReservationState = " + CarrierInfo.m_eReservationState.ToString() + " AssociationState = " + CarrierInfo.m_eAssociationState.ToString() + " CarrierID = " + CarrierInfo.m_sCarrierID);
                }
            }
            else
            {
                m_log.Add("CMSSetLPInfo Fail, CarrierInfo is Null");
            }
        }

        public void SetLPPresentSensor(int nLP, bool bSensorOn)
        {
            if (m_aXGem300Carrier[nLP] != null)
            {
                XGem300Carrier CarrierInfo = m_aXGem300Carrier[nLP];
                long nReturn = m_XGem300.CMSSetPresenceSensor(CarrierInfo.m_sLocID, Convert.ToInt32(bSensorOn));
                if (nReturn == 0)
                {
                    m_log.Add("--> CMSSetPresenceSensor - Loc ID = " + CarrierInfo.m_sLocID + " Presence Sensor  = " + bSensorOn.ToString());
                }
                else
                {
                    m_log.Add("--> CMSSetPresenceSensor Fail, ErrorCode = " + nReturn.ToString() + " - Loc ID = " + CarrierInfo.m_sLocID + " Presence Sensor  = " + bSensorOn.ToString());
                }
            }
            else
            {
                m_log.Add("CMSSetPresenceSensor Fail, CarrierInfo is Null");
            }
        }

        public void SetLPCarrierOnOff(int nLP, bool bCarrierOn)
        {
            if (m_aXGem300Carrier[nLP] != null)
            {
                XGem300Carrier CarrierInfo = m_aXGem300Carrier[nLP];
                long nReturn = m_XGem300.CMSSetCarrierOnOff(CarrierInfo.m_sLocID, Convert.ToInt32(bCarrierOn));
                if (nReturn == 0)
                {
                    m_log.Add("--> CMSSetCarrierOn - Loc ID = " + CarrierInfo.m_sLocID + " CarrierOn Sensor  = " + bCarrierOn.ToString());
                }
                else
                {
                    m_log.Add("--> CMSSetCarrierOn Fail, ErrorCode = " + nReturn.ToString() + " - Loc ID = " + CarrierInfo.m_sLocID + " CarrierOn Sensor  = " + bCarrierOn.ToString());
                }
            }
            else
            {
                m_log.Add("CMSSetCarrierOn Fail, CarrierInfo is Null");
            }
        }

        public bool SetCarrierID(int nLP, string sCarrierID)
        {
            if (m_aXGem300Carrier[nLP] != null)
            {
                m_XGem300.CMSSetCarrierIDStatus(sCarrierID, (long)XGem300Carrier.eCarrierState.WaitForHost);
                XGem300Carrier CarrierInfo = m_aXGem300Carrier[nLP];
                CarrierInfo.m_sCarrierID = sCarrierID;

                long nReturn = m_XGem300.CMSSetCarrierID(CarrierInfo.m_sLocID, CarrierInfo.m_sCarrierID, 0);
                if (nReturn == 0)
                {
                    m_log.Add("--> CMSSetCarrierID - LocID = " + CarrierInfo.m_sLocID + " CarrierID = " + CarrierInfo.m_sCarrierID);
                }
                else
                {
                    m_log.Add("--> CMSSetCarrierID Fail, ErrorCode = " + nReturn.ToString() + " - LocID = " + CarrierInfo.m_sLocID + " CarrierID = " + CarrierInfo.m_sCarrierID);
                }
                m_eSetCarrierIDState = eSetCarrierID.Send;


                ezStopWatch sw = new ezStopWatch();

                while (m_eSetCarrierIDState == eSetCarrierID.Send && (sw.Check() < 5000)) Thread.Sleep(10);


                switch (m_eSetCarrierIDState)
                {
                    case eSetCarrierID.Run:
                        //m_XGem300.CMSSetCarrierAccessing(CarrierInfo.m_sLocID, 1, CarrierInfo.m_sCarrierID);
                        return true;
                    case eSetCarrierID.Cancel:
                        m_log.Popup("CMSSetCarrierID Responce Error !!");
                        return false;
                    case eSetCarrierID.Send:
                        m_log.Popup("CMSSetCarrierID has no Responce !!");
                        return false;
                    default: return false;
                }
            }
            else
            {
                m_log.Add("CMSSetCarrier ID Fail, CarrierInfo is Null");
                return false;
            }
        }

        private void OnCMSCarrierVerifySucceeded(long nVerifyType, string sLocID, string sCarrierID, string sSlotMap, long nCount, string[] psLotID, string[] psSubstrateID, string sUsage)
        {
            m_log.Add("<== CMSCarrierVerifySucceeded, " + sLocID + ", " + sCarrierID + ", " + sSlotMap);
            if (nVerifyType == 0)
            {     // CarrierID Verification
                m_eSetCarrierIDState = eSetCarrierID.Run;
            }
            else if (nVerifyType == 1)
            {  // Slotmap Verification
                char[] aSlotMap = sSlotMap.ToCharArray();
                int nIndex = 0;
                for (int n = 0; n < 25; n++)
                {
                    if (aSlotMap[n] == '3')
                    {
                        if (nIndex == nCount) return;
                        m_infoCarrierSlotMap.m_infoWafer[nIndex].WAFERID = psSubstrateID[nIndex];
                        nIndex++;
                    }
                }
                m_eSetSlotMap = eSetSlotMap.Run;
                if (sLocID == "LP1") 
                    m_nWorkLP = 0;
                else if (sLocID == "LP2")
                    m_nWorkLP = 1;
                else if (sLocID == "LP3")
                    m_nWorkLP = 2;
            }
        }

        public XGem300_Mom()
        {
            InitializeComponent();
            cbPMType.SelectedIndex = 0;
        }

        private void radioXGem_Init_CheckedChanged(object sender, EventArgs e)
        {
            // XGemInitialize(); 
        }

        private void radioXGem_Start_CheckedChanged(object sender, EventArgs e)
        {
            // XGemStart(); 
        }

        private void radioXGem_Stop_CheckedChanged(object sender, EventArgs e)
        {
            //XGemStop(); 
        }

        private void radioXGem_Close_CheckedChanged(object sender, EventArgs e)
        {
            //XGemClose(); 
        }

        private void checkControl_Enable_CheckedChanged(object sender, EventArgs e)
        {
            XGemEnable(!m_bXGemEnable);
        }

        private void radioControl_Offline_CheckedChanged(object sender, EventArgs e)
        {
            //XGemOffline(); 
        }

        private void radioControl_Local_CheckedChanged(object sender, EventArgs e)
        {
            //XGemOnlineLocal(); 
        }

        private void radioControl_Remote_CheckedChanged(object sender, EventArgs e)
        {
            //XGemOnlineRemote(); 
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate, comboView.Text);
            RunGrid(eGrid.eRegWrite, comboView.Text);
        }

        private void comboView_SelectedIndexChanged(object sender, EventArgs e)
        {
            RunGrid(eGrid.eInit, comboView.Text);
        }

        ezStopWatch m_swFDC = new ezStopWatch();

        private void timer_Tick(object sender, EventArgs e)
        {
            radioControl_Offline.Checked = (m_eControl == eControl.OFFLINE);
            radioControl_Local.Checked = (m_eControl == eControl.LOCAL);
            radioControl_Remote.Checked = (m_eControl == eControl.ONLINEREMOTE);
            radioXGem_Init.Checked = (m_eXGemModule == eXGemModule.READY);
            radioXGem_Start.Checked = (m_eXGemModule == eXGemModule.EXCUTE);
            radioXGem_Stop.Checked = (m_eXGemModule == eXGemModule.READY);
            radioXGem_Close.Checked = false;
            checkControl_Enable.Checked = (m_eComm == eCommunicate.COMMUNICATING);
            if (m_swFDC.Check() > 10000)
            {
                SetFDCData();
                m_swFDC.Start();
            }
        }

        public virtual void SetFDCData()
        {
           
        }

        public virtual void Init(string id, Auto_Mom auto)
        {
            m_id = id;
            m_auto = auto;
            m_nLoadPort = m_auto.ClassHandler().m_nLoadPort;
            m_log = new Log(m_id, m_auto.m_logView, "XGem");
            m_grid = new ezGrid(m_id, grid, m_log, false);
            RunGrid(eGrid.eRegRead, "Setup");
            ReadReg();
            ProcessKill("XGem");
            m_XGem300 = new XGem300ProNet();
            m_aXGem300Carrier = new XGem300Carrier[m_nLoadPort];
            for (int i = 0; i < m_nLoadPort; i++)
                m_aXGem300Carrier[i] = new XGem300Carrier("LP" + (i + 1).ToString());

            m_XGem300Process = new XGem300Process[m_nLoadPort];
            for (int i = 0; i < m_nLoadPort; i++)
                m_XGem300Process[i] = new XGem300Process();

            m_XGem300Control = new XGem300Control[m_nLoadPort];
            for (int i = 0; i < m_nLoadPort; i++)
                m_XGem300Control[i] = new XGem300Control();

            XGemEventHandle();
            XGemInitialize();
            XGemStart();
            UISetting();
            m_swFDC.Start();
        }

        private void UISetting()
        {
            if (m_auto.m_strWork == Auto_Mom.eWork.MSB.ToString())
            {
                groupBox3.Hide();
            }
            else
            {
                groupBox11.Hide();
            }
        }

        public virtual void ThreadStop()
        {
            if (m_XGem300 == null) return;
            m_XGem300.Stop();
            m_XGem300.Close();
        }

        public void ReadReg()
        {
            RunGrid(eGrid.eRegRead, "SV");
            RunGrid(eGrid.eRegRead, "ECV");
            RunGrid(eGrid.eRegRead, "CEID");
            RunGrid(eGrid.eRegRead, "ALID");
            RunGrid(eGrid.eRegRead, "ALCD");
            comboView.Text = "SV";
            RunGrid(eGrid.eInit, comboView.Text);
        }

        void ProcessKill(string id)
        {
            Process[] ProcessList = Process.GetProcessesByName(id);
            foreach (Process process in ProcessList)
            {
                process.Kill();
            }
        }

        void RunGrid(eGrid eMode, string sMode)
        {
            m_grid.Update(eMode, null);
            switch (sMode)
            {
                case "SV":
                    foreach (XGem300Data sv in m_aSV)
                    {
                        m_grid.Set(ref sv.m_nID, "SV", sv.m_id, "SV");
                    }
                    break;
                case "ECV":
                    foreach (XGem300Data ecv in m_aECV)
                    {
                        m_grid.Set(ref ecv.m_nID, "ECV", ecv.m_id, "ECV");
                    }
                    break;
                case "CEID":
                    foreach (XGem300Data ceid in m_aCEID)
                    {
                        m_grid.Set(ref ceid.m_nID, "CEID", ceid.m_id, "SV");
                    }
                    break;
                case "ALID":
                    foreach (XGem300ALID alid in m_aALID)
                    {
                        m_grid.Set(ref alid.m_nALID, alid.m_id, alid.m_str, "ALID");
                    }
                    break;
                case "ALCD":
                    foreach (XGem300ALID alid in m_aALID)
                    {
                        m_grid.Set(ref alid.m_nALCD, alid.m_id, alid.m_str, "ALCD");
                    }
                    break;

                case "Setup":
                    m_grid.Set(ref m_sCfgPath, "Config", "Path", "Config File Path");
                    break;
            }
            m_grid.Refresh();
        }

        public virtual void XGemOffline()
        {
            if (m_XGem300 == null) return;
            m_XGem300.GEMReqOffline();
            m_log.Add("--> GEMReqOffline");
            if (m_eXGemError == eXGemError.None) return;
            m_log.Popup("XGemOffline Error : " + m_eXGemError.ToString());
        }

        public virtual void XGemOnlineLocal()
        {
            if (m_XGem300 == null) return;
            m_XGem300.GEMReqLocal();
            m_log.Add("--> GEMReqLocal");
            if (m_eXGemError == eXGemError.None) return;
            m_log.Popup("XGemOnlineLocal Error : " + m_eXGemError.ToString());
        }

        public virtual void XGemOnlineRemote()
        {
            if (m_XGem300 == null) return;
            m_eXGemError = (eXGemError)m_XGem300.GEMReqRemote();
            m_log.Add("--> GEMReqRemote");
            if (m_eXGemError == eXGemError.None) return;
            m_log.Popup("XGemOnlineRemote Error : " + m_eXGemError.ToString());
        }

        bool XGemEnable(bool bEnable)
        {
            long nEnable = 0;
            if (bEnable) nEnable = 1;
            m_eXGemError = (eXGemError)m_XGem300.GEMSetEstablish(nEnable);
            m_log.Add("--> GEMSetEstablish, " + nEnable.ToString()); 
            if (m_eXGemError == eXGemError.None)
            {
                m_log.Popup("XGemEnable : " + bEnable.ToString());
                return false;
            }
            else
            {
                m_log.Popup("XGemEnable Error : " + bEnable.ToString() + m_eXGemError.ToString());
                return false;
            }
        }

        protected bool XGemInitialize()
        {
            m_eXGemError = (eXGemError)m_XGem300.Initialize(m_sCfgPath);
            m_log.Add("--> Initialize, " + m_sCfgPath); 
            if (m_eXGemError == eXGemError.None) return false;
            m_log.Popup("XGemInitialize Error : " + m_eXGemError.ToString());
            return true;
        }

        protected bool XGemStart()
        {
            m_eXGemError = (eXGemError)m_XGem300.Start();
            m_log.Add("--> Start"); 
            if (m_eXGemError == eXGemError.None) return false;
            m_log.Popup("XGemStart Error : " + m_eXGemError.ToString());
            return true;
        }

        protected bool XGemStop()
        {
            m_eXGemError = (eXGemError)m_XGem300.Stop();
            m_log.Add("--> Stop"); 
            if (m_eXGemError == eXGemError.None) return false;
            m_log.Popup("XGemStop Error : " + m_eXGemError.ToString());
            return true;
        }

        protected bool XGemClose()
        {
            m_eXGemError = (eXGemError)m_XGem300.Close();
            m_log.Add("--> Close"); 
            if (m_eXGemError == eXGemError.None) return false;
            m_log.Popup("XGemClose Error : " + m_eXGemError.ToString());
            return true;
        }

        protected bool SetSV(XGem300Data sv, object value,bool bLog = true)
        {
            if (sv.m_nID < 0)
            {
                m_log.Popup("SV not Defined : " + sv.m_id);
                return true;
            }
            m_aID[0] = sv.m_nID;
            m_aValue[0] = value.ToString();
            m_eXGemError = (eXGemError)m_XGem300.GEMSetVariable(1, m_aID, m_aValue);
            if(bLog)
                m_log.Add("--> GEMSetVariable, " + m_aID[0] + ", " + m_aValue[0]); 
            if (m_eXGemError == eXGemError.None) return false;
            m_log.Popup("SetSV Error : " + m_eXGemError.ToString());
            return true;
        }

        protected string GetSV(XGem300Data sv)
        {
            m_aID[0] = sv.m_nID;
            m_aValue[0] = "";
            m_eXGemError = (eXGemError)m_XGem300.GEMGetVariable(1, ref m_aID, ref m_aValue);
            if (m_eXGemError != eXGemError.None) m_log.Popup("GetSV Error : " + m_eXGemError.ToString());
            return m_aValue[0];
        }

        protected bool SetECV(XGem300Data ecv, object value)
        {
            if (ecv.m_nID < 0)
            {
                m_log.Popup("SV not Defined : " + ecv.m_id);
                return true;
            }
            m_aID[0] = ecv.m_nID;
            m_aValue[0] = value.ToString();
            m_eXGemError = (eXGemError)m_XGem300.GEMSetECVChanged(1, m_aID, m_aValue);
            if (m_eXGemError == eXGemError.None) return false;
            m_log.Popup("SetECV Error : " + m_eXGemError.ToString());
            return true;
        }

        protected bool SetCEID(XGem300Data ceid)
        {
            long nCEID = (long)ceid.m_nID;
            if (nCEID < 0)
            {
                m_log.Popup("CEID not Defined : " + ceid.m_id);
                return true;
            }
            m_eXGemError = (eXGemError)m_XGem300.GEMSetEvent(nCEID);
            if (m_eXGemError == eXGemError.None) return false;
            m_log.Popup("SetCEID Error : " + m_eXGemError.ToString());
            return true;
        }

        public virtual void RecipeChanged(eRecipeChangeMode eMode, string sRecipeID)
        {
        }

        public void AddALID(string id, int nID, string str)
        {
            XGem300ALID alid = new XGem300ALID(id, nID, str);
            m_aALID.Add(alid);
        }

        public bool IsSetALID(int nALID)
        {
            foreach (XGem300ALID alid in m_aALID)
            {
                if (alid.m_nID == nALID) return alid.m_bSet;
            }
            return false;
        }

        public virtual void SetAlarm(string id, int nID)
        {
            foreach (XGem300ALID alid in m_aALID)
            {
                if ((alid.m_id == id) && (alid.m_nID == nID) && !alid.m_bSet)
                {
                    SetAlarm(alid, 1); 
                }
            }
        }

        public virtual void ResetAlarm(int nALID)
        {
            foreach (XGem300ALID alid in m_aALID)
            {
                if ((alid.m_nID == nALID) && alid.m_bSet)
                {
                    SetAlarm(alid, 0); 
                }
            }
        }

        public virtual void ClearAlarm()
        {
            foreach (XGem300ALID alid in m_aALID)
            {
                if (alid.m_bSet)
                {
                    SetAlarm(alid, 0); 
                }
            }
        }

        void SetAlarm(XGem300ALID alid, int nSet)
        {
            if (alid.m_nALID < 0) return;
            alid.m_bSet = (nSet == 1);
            SetAlarmReport(alid);
            if (m_XGem300 == null) return; 
            m_eXGemError = (eXGemError)m_XGem300.GEMSetAlarm(alid.m_nALID, nSet);
            if (m_eXGemError == eXGemError.None) return;
            m_log.Popup("ResetAlarm Error : " + m_eXGemError.ToString());
        }

        public void SetAlarm(int nNum, bool bSet)
        {
            if (nNum > 0)
            {
                int nSet = 0;
                if (bSet)
                    nSet = 1;
                m_eXGemError = (eXGemError)m_XGem300.GEMSetAlarm(nNum, nSet);
                if (m_eXGemError == eXGemError.None) return;
                m_log.Popup("setAlarm Error : " + m_eXGemError.ToString());
            }
        }

        public virtual void SetAlarmReport(XGem300ALID alid) { }

        public virtual void SetInitData()
        {
            m_auto.ClassXGem().m_XGem300.CMSReqChangeServiceStatus(m_auto.ClassXGem().m_aXGem300Carrier[0].m_sLocID, 0);
            m_auto.ClassXGem().m_XGem300.CMSReqChangeServiceStatus(m_auto.ClassXGem().m_aXGem300Carrier[1].m_sLocID, 0);
            m_auto.ClassXGem().LPAccessModeChange(XGem300Carrier.ePortAccess.Manual, m_auto.ClassXGem().m_aXGem300Carrier[0].m_sLocID);
            m_auto.ClassXGem().LPAccessModeChange(XGem300Carrier.ePortAccess.Manual, m_auto.ClassXGem().m_aXGem300Carrier[1].m_sLocID);
        }

        protected void XGemEventHandle()
        {
            this.m_XGem300.OnGEMCommStateChanged += new OnGEMCommStateChanged(OnGEMCommStateChanged);
            this.m_XGem300.OnGEMControlStateChanged += new OnGEMControlStateChanged(OnGEMControlStateChanged);
            this.m_XGem300.OnXGEMStateEvent += new OnXGEMStateEvent(OnXGEMStateEvent);
            this.m_XGem300.OnGEMTerminalMessage += new OnGEMTerminalMessage(OnGEMTerminalMessageRecieve);
            this.m_XGem300.OnGEMTerminalMultiMessage += new OnGEMTerminalMultiMessage(OnGEMTerminalMessageMultiRecieve);
            this.m_XGem300.OnGEMReqDateTime += new OnGEMReqDateTime(OnGEMReqDateTime);
            this.m_XGem300.OnGEMReqGetDateTime += new OnGEMReqGetDateTime(OnGemReqGetDateTime);
            this.m_XGem300.OnSECSMessageReceived += new OnSECSMessageReceived(OnSECSMessageReceived);
            this.m_XGem300.OnGEMReqRemoteCommand += new OnGEMReqRemoteCommand(OnGemReqRemoteCommand);
            this.m_XGem300.OnGEMReqChangeECV += new OnGEMReqChangeECV(OnGEMReqChangeECV);
            this.m_XGem300.OnCMSTransferStateChanged += new OnCMSTransferStateChanged(OnCMSTransferStateChanged);
            this.m_XGem300.OnCMSCarrierIDStatusChanged += new OnCMSCarrierIDStatusChanged(OnCMSCarrierIDStatusChanged);
            this.m_XGem300.OnCMSSlotMapStatusChanged += new OnCMSSlotMapStatusChanged(OnCMSSlotMapStatusChanged);
            this.m_XGem300.OnPJReqVerify += new OnPJReqVerify(OnPJReqVerify);
            this.m_XGem300.OnCJReqVerify += new OnCJReqVerify(OnCJReqVerify);
            this.m_XGem300.OnCJRspSelect += new OnCJRspSelect(OnCJRspSelect);
            this.m_XGem300.OnPJSettingUpStart += new OnPJSettingUpStart(OnPJSettingUpStart);
            this.m_XGem300.OnPJCreated += new OnPJCreated(OnPJCreated);
            this.m_XGem300.OnCJCreated += new OnCJCreated(OnCJCreated);
            this.m_XGem300.OnPJStateChanged += new OnPJStateChanged(OnPJStateChanged);
            this.m_XGem300.OnCJStateChanged += new OnCJStateChanged(OnCJStateChanged);
            this.m_XGem300.OnPJReqCommand += new OnPJReqCommand(OnPJReqCommand);
            this.m_XGem300.OnCMSCarrierVerifySucceeded += new OnCMSCarrierVerifySucceeded(OnCMSCarrierVerifySucceeded);
            this.m_XGem300.OnPJDeleted += new OnPJDeleted(OnPJDeleted);
            this.m_XGem300.OnCJDeleted += new OnCJDeleted(OnCJDeleted);
            this.m_XGem300.OnGEMReqPPList += new OnGEMReqPPList(OnGEMReqPPList);
            this.m_XGem300.OnCMSAccessModeStateChanged += new OnCMSAccessModeStateChanged(OnCMSAccessModeStateChanged);
            this.m_XGem300.OnCMSReqChangeServiceStatus += new OnCMSReqChangeServiceStatus(OnCMSReqChangeServiceStatus);
            //            this.m_XGem300.OnGEMReqPPFmtSend += new OnGEMReqPPFmtSend(OnGEMReqPPFmtSend);
        }

        void OnGEMCommStateChanged(long nState)
        {
            m_eComm = (eCommunicate)nState;
            m_log.Add("<== OnGEMCommStateChanged, " + nState.ToString() + ", " + m_eComm.ToString());
        }

        private void OnGEMReqPPList(long nMsgld)
        {
            string[] RecipeNames = m_auto.ClassRecipe().m_sRecipes;

            if (RecipeNames == null) return;

            if (RecipeNames.Length > 0)
                m_XGem300.GEMRspPPList(nMsgld, RecipeNames.Length, RecipeNames);
            else
                m_XGem300.GEMRspPPList(nMsgld, 0, RecipeNames);
        }

        private void OnCMSAccessModeStateChanged(string sLocID, long nState)
        {
            int nPort = 0;
            if (sLocID == "LP1")
            {
                nPort = 0;
            }
            else if (sLocID == "LP2")
            {
                nPort = 1;
            }
            else if (sLocID == "LP3")
            {
                nPort = 2;
            }
            switch (nState)
            {
                case (long)XGem300Carrier.ePortAccess.Manual:
                    m_aXGem300Carrier[nPort].m_ePortAccess = XGem300Carrier.ePortAccess.Manual;
                    break;
                case (long)XGem300Carrier.ePortAccess.Auto:
                    m_aXGem300Carrier[nPort].m_ePortAccess = XGem300Carrier.ePortAccess.Auto;
                    break;
            }
        }

        void OnCMSReqChangeServiceStatus(long nMsgId, string sLocID, long nState)
        {
            long nReturn = 0;
            long nOutState = 0;

            m_log.Add("<- OnCMSReqChangeServiceStatus : Loc ID - " + sLocID + ", State - " + nState.ToString());

            /*
            if (nState == 0)            //outofservice
                nOutState = 0;
            else if (nState == 1)       //inservice
                nOutState = 2;
            */

            nState = nOutState;

            //nState 0 : Out Of Service
            //nState 1 : TransferBlocked (In Service)
            //nState 2 : Ready To Load (In Service)
            //nState 3 : Ready To Unload(In Service)

            long nResult = 0;
            long nErrCount = 2;
            long[] naErrCode = new long[2];
            string[] saErrText = new string[2];

            naErrCode[0] = 10; saErrText[0] = "this is error 1";
            naErrCode[1] = 20; saErrText[1] = "this is error 2";

            //Send Response Message : S3F26
            nReturn = m_XGem300.CMSRspChangeServiceStatus(nMsgId, sLocID, nOutState, nResult, nErrCount, naErrCode, saErrText);
            if (nReturn == 0)
            {
                m_log.Add("<- OnCMSReqChangeServiceStatus : Send Response Change Service Status successfully");
            }
            else
            {
                m_log.Add("<- OnCMSReqChangeServiceStatus : Fail to Response Change Service Status");
            }
        }

        public void LPAccessModeChange(XGem300Carrier.ePortAccess AccessMode, string sLocID)
        {
            //if(IsOnline())
                m_XGem300.CMSReqChangeAccess((long)AccessMode, sLocID);
        }

        public bool IsOnlineRemote()
        {
            return (m_eControl == eControl.ONLINEREMOTE);
        }

        public bool IsOnlineLocal()
        {
            return (m_eControl == eControl.LOCAL);
        }

        public bool IsOnline()
        {
            return (m_eControl == eControl.ONLINEREMOTE || m_eControl == eControl.LOCAL);
        }

        public bool IsAuto(int nLPNum)
        {
            if (m_aXGem300Carrier == null) return false;
            return (m_aXGem300Carrier[nLPNum].m_ePortAccess == XGem300Carrier.ePortAccess.Auto);
        }

        void OnGEMControlStateChanged(long nState)
        {
            m_eControl = (eControl)nState;
            m_log.Add("<== OnGEMControlStateChanged, " + nState.ToString() + ", " + m_eControl.ToString());
            if (m_eControl == eControl.ONLINEREMOTE) XGemOnlineRemote();
        }

        void OnXGEMStateEvent(long nState)
        {
            m_eXGemModule = (eXGemModule)nState;
            m_log.Add("<== OnXGEMStateEvent, " + nState.ToString() + ", " + m_eXGemModule.ToString());
            if (m_eXGemModule == eXGemModule.EXCUTE)
            {
                m_XGem300.GEMSetEstablish(1);
                m_log.Add("GEMSetEstablish, 1");
                m_XGem300.GEMReqRemote();
                m_log.Add("GEMReqRemote");
                SetInitData();
            }
        }

        void OnGEMTerminalMessageRecieve(long nTid, string sMsg)
        {
            int nSearch;
            m_log.Popup("<== Terminal), " + nTid.ToString() + ", " + sMsg);
            nSearch = sMsg.IndexOf("STEPID="); // ing 170306
            if (nSearch > 0) // ing 170306
            {
                m_strStepID = sMsg;
                m_strStepID = m_strStepID.Remove(0, nSearch + 7);
                nSearch = m_strStepID.IndexOf(' ');
                m_strStepID = m_strStepID.Remove(nSearch, m_strStepID.Length - nSearch);
            }
        }

        void OnGEMTerminalMessageMultiRecieve(long nTid, long nCount, string[] sMsg)
        {
            int n;
            int nSearch; // ing 170306
            for (n = 0; n < nCount; n++)
            {
                m_log.Popup("<== Terminal), " + nTid.ToString() + ", " + sMsg[n]);
                nSearch = sMsg[n].IndexOf("STEPID="); // ing 170306
                if (nSearch > 0) // ing 170306
                {
                    m_strStepID = sMsg[n];
                    m_strStepID = m_strStepID.Remove(0, nSearch + 7);
                    nSearch = m_strStepID.IndexOf(' ');
                    m_strStepID = m_strStepID.Remove(nSearch, m_strStepID.Length - nSearch);
                }
            }
        }

        void OnGEMReqDateTime(long nMsgld, string sSystemTime)
        {
            m_log.Popup("<== OnGEMReqDateTime, " + nMsgld.ToString() + ", " + sSystemTime);
            long nResult = 1;
            if (sSystemTime.Length == 14)
            {
                ezDateTime dt = new ezDateTime();
                dt.m_sTime.wYear = Convert.ToUInt16(sSystemTime.Substring(0, 4));
                dt.m_sTime.wMonth = Convert.ToUInt16(sSystemTime.Substring(4, 2));
                dt.m_sTime.wDay = Convert.ToUInt16(sSystemTime.Substring(6, 2));
                dt.m_sTime.wHour = Convert.ToUInt16(sSystemTime.Substring(8, 2));
                dt.m_sTime.wMinute = Convert.ToUInt16(sSystemTime.Substring(10, 2));
                dt.m_sTime.wSecond = Convert.ToUInt16(sSystemTime.Substring(12, 2));
                if (dt.SetSystemTime() != 0) nResult = 0;
            }
            m_XGem300.GEMRspDateTime(nMsgld, nResult);
            m_log.Add("GEMRspDateTime, " + nMsgld.ToString() + ", " + nResult.ToString());
        }

        void OnGemReqGetDateTime(long nMsgld)
        {
            m_log.Popup("<== OnGemReqGetDateTime, " + nMsgld.ToString());
            string sTime = System.DateTime.Now.ToString("yyyyMMddhhmmss");
            m_XGem300.GEMRspGetDateTime(nMsgld, sTime);
            m_log.Add("GEMRspGetDateTime, " + nMsgld.ToString() + ", " + sTime);
        }

        void OnSECSMessageReceived(long nObjectID, long nStream, long nFunction, long nSysbyte)
        {
            m_log.Popup("<== OnSECSMessageReceived, " + nObjectID.ToString() + ", " + nStream.ToString() + ", " + nFunction.ToString() + ", " + nSysbyte.ToString());
            MessageReceived(nObjectID, nStream, nFunction, nSysbyte);
        }

        void OnGemReqRemoteCommand(long nMsgID, string sRcmd, long nCount, string[] psCpName, string[] psCpVal)
        {
            if (psCpName.Length < nCount)
            {
                m_log.Popup("OnGemReqRemoteCommand, psCpName Count Invalid (" + psCpName.Length.ToString() + ", " + nCount.ToString() + ")");
                nCount = psCpName.Length;
            }
            if (psCpVal.Length < nCount)
            {
                m_log.Popup("OnGemReqRemoteCommand, psCpVal Count Invalid (" + psCpVal.Length.ToString() + ", " + nCount.ToString() + ")");
                nCount = psCpVal.Length;
            }
            long nHCAck = 0;
            long[] pnCpAck = new long[nCount];
            m_log.Add("<== OnGemReqRemoteCommand, " + nMsgID.ToString() + ", " + sRcmd);
            for (int n = 0; n < nCount; n++)
            {
                m_log.Add("            " + psCpName[n] + ", " + psCpVal[n]);
                pnCpAck[n] = 0;
            }
            RemoteCommand(nMsgID, sRcmd, nCount, psCpName, psCpVal, ref pnCpAck);
            m_XGem300.GEMRspRemoteCommand(nMsgID, sRcmd, nHCAck, nCount, psCpName, pnCpAck);
        }

        void OnGEMReqChangeECV(long nMsgID, long nCount, long[] pnECIDs, string[] psValues)
        {
            if (pnECIDs.Length < nCount)
            {
                m_log.Popup("OnGEMReqChangeECV, pnECIDs Count Invalid (" + pnECIDs.Length.ToString() + ", " + nCount.ToString() + ")");
                nCount = pnECIDs.Length;
            }
            if (psValues.Length < nCount)
            {
                m_log.Popup("OnGEMReqChangeECV, psValues Count Invalid (" + psValues.Length.ToString() + ", " + nCount.ToString() + ")");
                nCount = psValues.Length;
            }
            int nResult = 0;
            for (int n = 0; n < nCount; n++)
            {
                m_log.Add("<== OnGEMReqChangeECV, " + nMsgID.ToString() + ", " + pnECIDs[n].ToString() + ", " + psValues[n]);
                nResult -= ReqChangeECV(nMsgID, pnECIDs[n], psValues[n]);
            }
            m_XGem300.GEMRspChangeECV(nMsgID, nResult);
            m_log.Add("--> GEMRspChangeECV, " + nMsgID.ToString() + ", " + nResult.ToString());
        }

        int ReqChangeECV(long nMsgID, long nECID, string sValue)
        {
            int n = 0;
            foreach (XGem300Data ecv in m_aECV)
            {
                if (ecv.m_nID == nECID) return ReqChangeECV(n, sValue);
                n++;
            }
            m_log.Popup("ReqChangeECV Error " + nECID.ToString());
            return -1;
        }

        private void OnCMSTransferStateChanged(string sLocID, long nState)
        {
            bool nLocIDMatch = false;
            m_log.Add("<== OnCMSTransferStateChanged, " + sLocID + ", " + nState.ToString());
            for (int i = 0; i < m_nLoadPort; i++)
            {
                if (m_aXGem300Carrier[i].m_sLocID == sLocID)
                {
                    m_aXGem300Carrier[i].m_eLPTransfer = (XGem300Carrier.eLPTransfer)nState;
                    nLocIDMatch = true;
                }
            }
            if (!nLocIDMatch) m_log.Popup("OnCMSTransferStateChanged Fail sLocID : " + sLocID);
        }

        private void OnCMSCarrierIDStatusChanged(string sLocID, long nState, string sCarrierID)
        {
            bool nLocIDMatch = false;
            m_log.Add("<== OnCMSCarrierIDStatusChanged, " + sLocID + ", " + nState.ToString() + ", " + sCarrierID);
            for (int i = 0; i < m_nLoadPort; i++)
            {
                if (m_aXGem300Carrier[i].m_sLocID == sLocID)
                {
                    m_aXGem300Carrier[i].m_eCarrierIDState = (XGem300Carrier.eCarrierState)nState;
                    nLocIDMatch = true;
                }
            }
            if (!nLocIDMatch) m_log.Popup("OnCMSCarrierIDStatusChanged Fail sLocID : " + sLocID);
        }

        private void OnCMSSlotMapStatusChanged(string sLocID, long nState, string sCarrierID)
        {
            bool nLocIDMatch = false;
            m_log.Add("<== OnCMSSlotMapStatusChanged, " + sLocID + ", " + nState.ToString() + ", " + sCarrierID);
            for (int i = 0; i < m_nLoadPort; i++)
            {
                if (m_aXGem300Carrier[i].m_sLocID == sLocID)
                {
                    m_aXGem300Carrier[i].m_eCarrierSlotMapState = (XGem300Carrier.eCarrierState)nState;
                    nLocIDMatch = true;
                }
            }
            if (!nLocIDMatch) m_log.Popup("OnCMSSlotMapStatusChanged Fail sLocID : " + sLocID);
        }

        public virtual void SetPPID(string sPPID) { }

        public void OnCJReqVerify(long nMsgId, string sCJobID, long nCarrierCount, string[] psCarrId, long nCountPRJob, string[] psPRJobID, long nProcessOrderMgmt, long nStartMethod)
        {
            m_log.Add("<== OnCJReqVerify, " + sCJobID); 
            long[] nErrorCode = new long[1] { 0 };
            string[] sErrTxt = new string[1] { "" };
            m_XGem300.CJRspVerify(nMsgId, sCJobID, 0, 0, nErrorCode, sErrTxt);
            m_log.Add("--> CJRspVerify, " + sCJobID); 
        }

        public void OnCJRspSelect(string sCJobID, long nResult)
        {
            m_log.Add("<== OnCJRspSelect"); 
        }

        public void OnPJSettingUpStart(string sPJobID)
        {
            m_log.Add("<== OnPJSettingUpStart, " + sPJobID); 
            m_XGem300.PJSettingUpCompt(sPJobID);
            m_log.Add("--> PJSettingUpCompt, " + sPJobID); 
        }

        int m_nWorkLP = 0;

        private void OnPJReqVerify(long nMsgID, long nPJobCount, string[] psPJobID, long[] pnMtrlFormat, long[] pnAutoStart, long[] pnMtrlOrder, long[] pnMtrlCount, string[] psMtrlID, string[] psSlotInfo, long[] pnRcpMethod, string[] psRcpID, long[] pnRcpParCount, string[] psRcpParName, string[] psRcpParValue)
        {
            m_log.Add("<== OnPJReqVerify"); 
            long nAck = 0;
            long[] nErrorCode = new long[1] { 0 };
            string[] sErrTxt = new string[1] { "" };
            string sSlotInfo = "";
            Recipe_Mom recipe = m_auto.ClassRecipe();

//            string PPID = psRcpID[0].Replace(".", "_");
            string PPID = psRcpID[0];

            if (nPJobCount == 1)
            {
                m_XGem300Process[m_nWorkLP].m_eAutoStart = (XGem300Process.eFlag)pnAutoStart[0];
                if (recipe.IsRecipeExist(PPID))
                {
                    m_XGem300Process[m_nWorkLP].m_sRecipeID = PPID;
                    SetPPID(PPID);
                }
                else
                {
                    nAck = 1;
                    nErrorCode[0] = (long)(XGem300Process.eError.Invalid_AttibuteValue);
                    sErrTxt[0] = "Recipe Does Not Exist In EQ";
                }
                for (int i = 0; i < psSlotInfo.Length; i++)
                {
                    sSlotInfo += psSlotInfo[i];
                }
                m_XGem300Process[m_nWorkLP].m_sJobID = psPJobID[0];
                m_XGem300Process[m_nWorkLP].m_sSlotMapInsp = sSlotInfo;
            }
            else
            {
                nAck = 1;
                nErrorCode[0] = (long)(XGem300Process.eError.Unsupported_Option);
                sErrTxt[0] = "Can not Support Multi PJOB Create";
            }
            m_XGem300.PJRspVerify(nMsgID, nPJobCount, psPJobID, nAck, 1, nErrorCode, sErrTxt);
            m_log.Add("--> PJRspVerify");
            m_log.Add("Send PRJOB Create Response , nAck : " + nAck.ToString() + " ErrCode : " + nErrorCode[0].ToString() + " ErrTxt : " + sErrTxt);
        }

        private void OnPJCreated(string sPJobID, long nMtrlFormat, long nAutoStart, long nMtrlOrder, long nMtrlCount, string[] psMtrlID, string[] psSlotInfo, long nRcpMethod, string sRcpID, long nRcpParCount, string[] psRcpParName, string[] psRcpParValue)
        {
            m_log.Add("<== PJReqCreate");
        }

        private void OnCJCreated(string sCJobID, long nStartMethod, long nCountPRJob, string[] psPRJobID)
        {
            m_log.Add("<== CJCreate");
            m_XGem300Control[m_nWorkLP] = new XGem300Control();
            m_XGem300Control[m_nWorkLP].m_sJobID = sCJobID;
        }

        private void OnPJStateChanged(string sPJobID, long nState)
        {
            m_log.Add("<== OnPJStateChanged, (" + m_XGem300Process[m_nWorkLP].m_eJobState + " ->  " + ((XGem300Process.eJobState)nState).ToString() + "), PJobID : " + sPJobID);
            if (m_XGem300Process[m_nWorkLP].m_sJobID == sPJobID)
            {
                m_XGem300Process[m_nWorkLP].m_eJobState = (XGem300Process.eJobState)nState;
                switch (m_XGem300Process[m_nWorkLP].m_eJobState)
                {
                    case XGem300Process.eJobState.Queued: break;
                    case XGem300Process.eJobState.SettingUp:
                        m_XGem300.PJSettingUpCompt(m_XGem300Process[m_nWorkLP].m_sJobID);
                        m_log.Add("--> Send PJSettingUpCompt, " + m_XGem300Process[m_nWorkLP].m_sJobID);
                        m_XGem300.PJReqCommand(1, m_XGem300Process[m_nWorkLP].m_sJobID);
                        m_log.Add("--> Send PJReqCommand, " + m_XGem300Process[m_nWorkLP].m_sJobID);
                        m_XGem300.PJSetState(m_XGem300Process[m_nWorkLP].m_sJobID, (long)3);
                        m_log.Add("--> Send PJSetState, " + m_XGem300Process[m_nWorkLP].m_sJobID); 
                        break;
                    case XGem300Process.eJobState.Processing:
                        break;
                }
            }
        }

        private void OnCJStateChanged(string sCJobID, long nState)
        {
            m_log.Add("<== OnCJStateChanged (" + m_XGem300Control[m_nWorkLP].m_eState + "  ->  " + ((XGem300Control.eState)nState).ToString() + ")");
            if (m_XGem300Control[m_nWorkLP].m_sJobID == sCJobID)
            {
                m_XGem300Control[m_nWorkLP].m_eState = (XGem300Control.eState)nState;

                switch (m_XGem300Control[m_nWorkLP].m_eState)
                {
                    case XGem300Control.eState.Queued:
                        m_XGem300.CJReqSelect(sCJobID);
                        //SetLPAssociation(m_nWorkLP, XGem300Carrier.eCarrierAssociationState.IN_ACCESSED);
                        m_log.Add("--> CJReqSelect, " + sCJobID);
                        break;
                    case XGem300Control.eState.Excuting:
                        m_XGem300.PJSettingUpStart(m_XGem300Process[m_nWorkLP].m_sJobID);
                        m_log.Add("--> PJSettingUpStart, " + m_XGem300Process[m_nWorkLP].m_sJobID);
                        break;
                }
            }
        }

        private void OnPJDeleted(string sPJJobID)
        {
            m_log.Add("<== OnPJDeleted, " + sPJJobID); 
        }

        private void OnCJDeleted(string sCJJobID)
        {
            m_log.Add("<== OnCJDeleted, " + sCJJobID); ;
            //XGem300Carrier CarrierInfo = m_aXGem300Carrier[0];     //KJW 
            //m_XGem300.CMSSetCarrierAccessing(CarrierInfo.m_sLocID, 2, CarrierInfo.m_sCarrierID);
            //m_log.Add("--> CMSSetCarrierAccessing, " + CarrierInfo.m_sLocID + ", " + CarrierInfo.m_sCarrierID); 
        }

        public void SetPJState(int nLP, XGem300Process.eJobState eState)
        {
            m_log.Add("Set PJ State ( " + eState.ToString() + " )");
            m_eXGemError = (eXGemError)m_XGem300.PJSetState(m_XGem300Process[nLP].m_sJobID, (long)eState);
            m_log.Add("PJSetState(" + m_XGem300Process[nLP].m_sJobID + ", " + eState.ToString() + ")");
            if (m_eXGemError != eXGemError.None) m_log.Popup("PJSetState Error : " + m_eXGemError.ToString());
        }
        public void SetCMSTransferState(XGem300Carrier.eLPTransfer eState, int nLP)
        {
            XGem300Carrier CarrierInfo = m_aXGem300Carrier[nLP];
            switch (eState)
            {
                case XGem300Carrier.eLPTransfer.OutOfService:
                    break;
                case XGem300Carrier.eLPTransfer.ReadyToLoad:
                    m_XGem300.CMSSetReadyToLoad(CarrierInfo.m_sLocID);
                    m_log.Add("--> CMSSetReadyToLoad, " + CarrierInfo.m_sLocID); 
                    break;
                case XGem300Carrier.eLPTransfer.ReadyToUnload:
                    m_XGem300.CMSSetReadyToUnload(CarrierInfo.m_sLocID);
                    m_log.Add("--> CMSSetReadyToUnload, " + CarrierInfo.m_sLocID); 
                    break;
                case XGem300Carrier.eLPTransfer.TransferBlocked:
                    break;
            }
        }


        private void OnPJReqCommand(long nMsgId, string sPJobID, long nCommand)
        {
            m_log.Add("<== OnPJReqCommand, " + sPJobID + ", " + nCommand.ToString()); 
            bool nAck = true;
            long nErrCount = 0;
            long[] pnErrCode = new long[5] { 0, 0, 0, 0, 0 };
            string[] psErrText = new string[5] { "", "", "", "", "" };
            XGem300Process.eJobState ePJStateChange = XGem300Process.eJobState.Queued;
            if (sPJobID == m_XGem300Process[m_nWorkLP].m_sJobID)
            {
                switch ((XGem300Process.eCommand)nCommand)
                {
                    case XGem300Process.eCommand.START:
                        ePJStateChange = XGem300Process.eJobState.Processing;
                        break;
                    case XGem300Process.eCommand.STOP: break;
                    case XGem300Process.eCommand.RESUME: break;
                    case XGem300Process.eCommand.PAUSE: break;
                    case XGem300Process.eCommand.CANCEL: break;
                    case XGem300Process.eCommand.ABORT: break;
                }
            }
            else
            {
                nAck = false;
                pnErrCode[nErrCount] = 1;
                psErrText[nErrCount] = "PRJOB ID Match Fail";
                nErrCount++;
            }

            m_XGem300.PJRspCommand(nMsgId, nCommand, m_XGem300Process[m_nWorkLP].m_sJobID, Convert.ToInt64(nAck), nErrCount, pnErrCode, psErrText);
            if (ePJStateChange != XGem300Process.eJobState.Queued)
            {
                m_XGem300.PJSetState(m_XGem300Process[m_nWorkLP].m_sJobID, (long)ePJStateChange);
                m_log.Add("--> PJSetState, " + ePJStateChange.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetLPPresentSensor(cbLPNum.SelectedIndex, true);
            SetLPCarrierOnOff(cbLPNum.SelectedIndex, true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (m_XGem300 != null)
            {
                m_XGem300.CJDelAllJobInfo();
                m_log.Add("--> CJDelAllJobInfo"); 
                m_XGem300.PJDelAllJobInfo();
                m_log.Add("--> PJDelAllJobInfo"); 
                m_XGem300.CMSDelAllCarrierInfo();
                m_log.Add("--> CMSDelAllCarrierInfo"); 

                for (int i = 0; i < m_nLoadPort; i++)
                {
                    SetLPInfo(i, XGem300Carrier.eLPTransfer.ReadyToLoad, XGem300Carrier.ePortAccess.Manual, XGem300Carrier.ePortReservationState.NotReserved, XGem300Carrier.ePortAssocitionState.NotAssociated, "");
                }
            }
        }

        public void DeleteCarrierInfo(string sCarrierID)
        {
            m_XGem300.CMSDelCarrierInfo(sCarrierID);
        }

        public void DeleteAllLPInfo()
        {
            m_XGem300.PJDelAllJobInfo();
            m_XGem300.CJDelAllJobInfo();
            m_XGem300.CMSDelAllCarrierInfo();
            m_XGem300Process[0].m_eJobState = XGem300Process.eJobState.Stopped;
            m_XGem300Control[0].m_eState = XGem300Control.eState.Paused;
            m_XGem300Process[1].m_eJobState = XGem300Process.eJobState.Stopped;
            m_XGem300Control[1].m_eState = XGem300Control.eState.Paused;
            m_XGem300Process[2].m_eJobState = XGem300Process.eJobState.Stopped;
            m_XGem300Control[2].m_eState = XGem300Control.eState.Paused;
            m_log.Add("DeleteAllLPInfo");
        }

        public void DellLPInfo(int nLP)
        {
            int nOther = 0;
            if (nLP == 0) nOther = 1;
            else nOther = 0;
            m_XGem300.CMSDelCarrierInfo(m_auto.ClassHandler().ClassLoadPort(nLP).m_infoCarrier.m_strCarrierID);

            if(m_auto.ClassHandler().ClassLoadPort(nLP).CheckState_OtherLP(nLP,"<=",HW_LoadPort_Mom.eState.CSTIDRead)||m_auto.ClassHandler().ClassLoadPort(nLP).CheckState_OtherLP(nLP,"==",HW_LoadPort_Mom.eState.UnloadDone))
            {
                DeleteAllLPInfo();
            }
        }

        public void LoadPortInit()
        {
            for (int i = 0; i < m_nLoadPort; i++)
            {
                SetLPInfo(i, XGem300Carrier.eLPTransfer.OutOfService, XGem300Carrier.ePortAccess.Manual, XGem300Carrier.ePortReservationState.NotReserved, XGem300Carrier.ePortAssocitionState.NotAssociated, "");
            }
        }

        public void SetLPAssociation(int nLP, XGem300Carrier.eCarrierAssociationState eState)
        {
            XGem300Carrier InfoCarrier = m_aXGem300Carrier[nLP];
            m_XGem300.CMSSetCarrierAccessing(InfoCarrier.m_sLocID, (long)eState, InfoCarrier.m_sCarrierID);
        }

        public string GetLOTID(int nLP)
        {
            if (m_XGem300Process[nLP].m_sJobID.IndexOf("_") >= 0)
            {
                return m_XGem300Process[nLP].m_sJobID.Substring(0, m_XGem300Process[nLP].m_sJobID.IndexOf("_"));
            }
            else
                return m_XGem300Process[nLP].m_sJobID;
        }

        public string GetRecipeID(int nLP)
        {
            return m_XGem300Process[nLP].m_sRecipeID;
        }

        public string GetCSTID(int nLP)
        {
            return m_aXGem300Carrier[nLP].m_sCarrierID;
        }
        public XGem300Process.eJobState GetPJState(int nLP)
        {
            return m_XGem300Process[nLP].m_eJobState;
        }
        public XGem300Control.eState GetCJSate(int nLP)
        {
            return m_XGem300Control[nLP].m_eState;
        }

        public void JOBEnd(int nLP)
        {
            //   m_XGem300.CMSSetCarrierAccessing("LP1",2,"MAO00059");
            m_XGem300.PJSetState(m_XGem300Process[nLP].m_sJobID, (long)(XGem300Process.eJobState.ProcessingComplete));
            m_log.Add("PJSetState - PJID = " + m_XGem300Process[nLP].m_sJobID + "  State = " + XGem300Process.eJobState.JobComplete.ToString());
        }

        public void SendComplete(int nlp, string sCarrierID)
        {
            long nCount = 1;
            long[] pnVid = new long[1] { 0 };
            string[] psValue = new string[1] { "0" };

            pnVid[0] = 33;
            psValue[0] = (nlp + 1).ToString();
            m_XGem300.GEMSetVariable(nCount, pnVid, psValue);
            pnVid[0] = 5006;
            psValue[0] = sCarrierID;
            m_XGem300.GEMSetVariable(nCount, pnVid, psValue);
            m_XGem300.GEMSetEvent(5013);
        }

        public void SetCarrierInfo(int nLP, string sCarrierID)
        {
            long nCount = 1;
            long[] pnVid = new long[1] { 0 };
            string[] psValue = new string[1] { "0" };

            pnVid[0] = 33;
            psValue[0] = (nLP + 1).ToString();
            m_XGem300.GEMSetVariable(nCount, pnVid, psValue);
            pnVid[0] = 5006;
            psValue[0] = sCarrierID;
            m_XGem300.GEMSetVariable(nCount, pnVid, psValue);
        }

        public void SendReadyToUnload(int nLP, string sCarrierID)
        {
            long nCount = 1;
            long[] pnVid = new long[1] { 0 };
            string[] psValue = new string[1] { "0" };

            pnVid[0] = 33;
            psValue[0] = (nLP + 1).ToString();
            m_XGem300.GEMSetVariable(nCount, pnVid, psValue);
            pnVid[0] = 5006;
            psValue[0] = sCarrierID;
            m_XGem300.GEMSetVariable(nCount, pnVid, psValue);
            m_XGem300.GEMSetEvent(5031);
        }

        public bool GetSlotMap(int nLP, int n)
        {
            bool bResult = false;

            string sSlotMap = m_XGem300Process[nLP].m_sSlotMapInsp;
            int nInfo = Convert.ToInt32(sSlotMap.Substring(n, 1));

            if (nInfo == 3) bResult = true;
            else bResult = false;

            return bResult;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            m_XGem300.CMSSetCarrierIDStatus(tbCarrierID.Text, (long)XGem300Carrier.eCarrierState.WaitForHost);
            m_log.Add("--> CMSSetCarrierIDStatus, " + tbCarrierID.Text + ", " + XGem300Carrier.eCarrierState.WaitForHost.ToString()); 
            SetCarrierID(cbLPNum.SelectedIndex, tbCarrierID.Text);
        }


        public void LPDocking(int nLP)
        {
            m_XGem300.GEMSetEvent(27);
            m_log.Add("--> GEMSetEvent, 27");
        }

        public void LPLoadComplete(int nLP)
        {
            m_XGem300.GEMSetEvent(23);
            m_log.Add("--> GEMSetEvent, 23");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LPDocking(cbLPNum.SelectedIndex);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            LPLoadComplete(cbLPNum.SelectedIndex);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Info_Carrier TestCarrier = new Info_Carrier(cbLPNum.SelectedIndex);
            if (cbLPNum.SelectedIndex == 0)
                TestCarrier.Init("LP1", m_log, Wafer_Size.eSize.mm300_RF);
            else if (cbLPNum.SelectedIndex == 1)
                TestCarrier.Init("LP1", m_log, Wafer_Size.eSize.mm300);
            TestCarrier.m_strCarrierID = tbCarrierID.Text;
            for (int i = 0; i < 25; i++)
            {
                if (tbSlotMap.Text.Substring(i, 1) == "1")
                {
                    TestCarrier.m_infoWafer[i].State = Info_Wafer.eState.Exist;
                }
                else
                {
                    TestCarrier.m_infoWafer[i].State = Info_Wafer.eState.Empty;
                }
            }

            SetSlotMap(TestCarrier);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            m_XGem300.GEMSetEvent(37);
            m_log.Add("--> GEMSetEvent, 37");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            long[] nId = new long[1] { -1 };
            string[] Value = new string[1] { "" };
            Value[0] = tbWaferID.Text;
            m_XGem300.GEMSetVariable(1, nId, Value);
            m_log.Add("--> GEMSetVariable, " + nId[0] + ", " + Value[0]);
            m_XGem300.GEMSetEvent(45);
            m_log.Add("--> GEMSetEvent, 45");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            long[] nId = new long[1] { -1 };
            string[] Value = new string[1] { "" };

            nId[0] = 50;
            Value[0] = tbLotID.Text;
            m_XGem300.GEMSetVariable(1, nId, Value);
            long n = nId[0];
            m_log.Add("--> GEMSetVariable, " + nId[0] + ", " + Value[0]);
            nId[0] = 51;
            Value[0] = (cbSlotNo.SelectedIndex + 1).ToString();
            m_XGem300.GEMSetVariable(1, nId, Value);
            m_log.Add("--> GEMSetVariable, " + nId[0] + ", " + Value[0]);
            nId[0] = 52;
            Value[0] = "1";
            m_XGem300.GEMSetVariable(1, nId, Value);
            m_log.Add("--> GEMSetVariable, " + nId[0] + ", " + Value[0]);
            nId[0] = 53;
            Value[0] = cbNoInkMap.SelectedIndex.ToString();
            m_XGem300.GEMSetVariable(1, nId, Value);
            m_log.Add("--> GEMSetVariable, " + nId[0] + ", " + Value[0]);
            nId[0] = 54;
            Value[0] = cbStackMap.SelectedIndex.ToString();
            m_XGem300.GEMSetVariable(1, nId, Value);
            m_log.Add("--> GEMSetVariable, " + nId[0] + ", " + Value[0]);
            m_XGem300.GEMSetEvent(152);
            m_log.Add("--> GEMSetEvent, 152");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            long[] nId = new long[1] { -1 };
            string[] Value = new string[1] { "" };
            nId[0] = 40;
            Value[0] = tbWaferID.Text;
            m_XGem300.GEMSetVariable(1, nId, Value);
            m_log.Add("--> GEMSetVariable, " + nId[0] + ", " + Value[0]);
            m_XGem300.GEMSetEvent(47);
            m_log.Add("--> GEMSetEvent, 47");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            long[] nId = new long[1] { -1 };
            string[] Value = new string[1] { "" };

            nId[0] = 50;
            Value[0] = tbLotID.Text;
            m_XGem300.GEMSetVariable(1, nId, Value);
            m_log.Add("--> GEMSetVariable, " + nId[0] + ", " + Value[0]);
            nId[0] = 51;
            Value[0] = (cbSlotNo.SelectedIndex + 1).ToString();
            m_XGem300.GEMSetVariable(1, nId, Value);
            m_log.Add("--> GEMSetVariable, " + nId[0] + ", " + Value[0]);
            nId[0] = 52;
            Value[0] = "0";
            m_XGem300.GEMSetVariable(1, nId, Value);
            m_log.Add("--> GEMSetVariable, " + nId[0] + ", " + Value[0]);
            nId[0] = 53;
            Value[0] = cbNoInkMap.SelectedIndex.ToString();
            m_XGem300.GEMSetVariable(1, nId, Value);
            m_log.Add("--> GEMSetVariable, " + nId[0] + ", " + Value[0]);
            nId[0] = 54;
            Value[0] = cbStackMap.SelectedIndex.ToString();
            m_XGem300.GEMSetVariable(1, nId, Value);
            m_log.Add("--> GEMSetVariable, " + nId[0] + ", " + Value[0]);
            m_XGem300.GEMSetEvent(152);
            m_log.Add("--> GEMSetEvent, 152");
        }

        private void button12_Click(object sender, EventArgs e)
        {
            m_XGem300.GEMSetEvent(38);
            m_log.Add("--> GEMSetEvent, 38");
            m_XGem300.PJSetState(m_XGem300Process[m_nWorkLP].m_sJobID, (long)XGem300Process.eJobState.ProcessingComplete);
            m_log.Add("--> PJSetState, " + m_XGem300Process[m_nWorkLP].m_sJobID + ", " + XGem300Process.eJobState.ProcessingComplete.ToString());

        }

        private void button13_Click(object sender, EventArgs e)
        {
            m_XGem300.GEMSetEvent(25);
            m_log.Add("--> GEMSetEvent, 25");
        }

        private void button14_Click(object sender, EventArgs e)
        {
            m_XGem300.GEMSetEvent(29);
            m_log.Add("--> GEMSetEvent, 29");
        }

        private void button15_Click(object sender, EventArgs e)
        {
            m_XGem300.CMSSetReadyToLoad("LP" + (cbLPNum.SelectedIndex + 1).ToString());
            m_log.Add("--> CMSSetReadyToLoad, " + "LP" + (cbLPNum.SelectedIndex + 1).ToString());
            SetLPPresentSensor(cbLPNum.SelectedIndex, false);
            SetLPCarrierOnOff(cbLPNum.SelectedIndex, false);
        }

        public virtual void WaferStart(Info_Wafer InfoWafer) { }
        public virtual void WaferEnd(Info_Wafer InfoWafer) { }
        public virtual void ClearWafer() { }  
        public virtual void ReviewDone(Info_Wafer InfoWafer, int nDefectCount = 0) { }
        private void button23_Click(object sender, EventArgs e)
        {
            Info_Wafer TestInfoWafer = new Info_Wafer(cbLPNum.SelectedIndex, cbSlotNo.SelectedIndex, cbSlotNo.SelectedIndex, m_log, Wafer_Size.eSize.mm300_RF);
            TestInfoWafer.WAFERID = textBox2.Text;
            WaferStart(TestInfoWafer);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            Info_Wafer TestInfoWafer = new Info_Wafer(cbLPNum.SelectedIndex, cbSlotNo.SelectedIndex, cbSlotNo.SelectedIndex, m_log, Wafer_Size.eSize.mm300_RF);
            TestInfoWafer.WAFERID = textBox2.Text;
            WaferEnd(TestInfoWafer);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            Info_Wafer TestInfoWafer = new Info_Wafer(cbLPNum.SelectedIndex, cbSlotNo.SelectedIndex, cbSlotNo.SelectedIndex, m_log, Wafer_Size.eSize.mm300_RF);
            TestInfoWafer.WAFERID = textBox2.Text;
            ReviewDone(TestInfoWafer);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            long[] nId = new long[1] { -1 };
            string[] Value = new string[1] { "" };

            nId[0] = 1005;
            Value[0] = textBox2.Text;
            m_XGem300.GEMSetVariable(1, nId, Value);
            m_log.Add("--> GEMSetVariable, " + nId[0] + ", " + Value[0]);
            nId[0] = 2001;
            Value[0] = "READY";
            m_XGem300.GEMSetVariable(1, nId, Value);
            m_log.Add("--> GEMSetVariable, " + nId[0] + ", " + Value[0]);
            nId[0] = 2002;
            Value[0] = "0";
            m_XGem300.GEMSetVariable(1, nId, Value);
            m_log.Add("--> GEMSetVariable, " + nId[0] + ", " + Value[0]);
            m_XGem300.GEMSetEvent(2001);
            m_log.Add("--> GEMSetEvent, 2001");
        }

        public void ClearJob(int nLP)
        {
            long nResult;
       //     nResult = m_XGem300.GEMSetEvent(320);
            //if (nResult == 0) m_log.Add("--> GEMSetEvent, 320");
            //else m_log.Add("GEMSetEvent Fail, ErrCode : " + nResult.ToString());

            m_XGem300Process[nLP].m_eJobState = XGem300Process.eJobState.Stopped;
            m_XGem300Control[nLP].m_eState = XGem300Control.eState.Completed;

            nResult = m_XGem300.CJDelJobInfo(m_XGem300Control[nLP].m_sJobID);
            if (nResult == 0)
                m_log.Add("--> CJDELJobInfo");
            else
                m_log.Add("-->CJDelJOb Info Fail, Error Code : " + nResult.ToString());

            nResult = m_XGem300.PJDelJobInfo(m_XGem300Process[nLP].m_sJobID);
            if (nResult == 0)
                m_log.Add("--> PJDELJobInfo");
            else
                m_log.Add("-->PJDelJOb Info Fail, Error Code : " + nResult.ToString());
        }

        private void button19_Click(object sender, EventArgs e)
        {
            long[] nId = new long[1] { -1 };
            string[] Value = new string[1] { "" };

            nId[0] = 1005;
            Value[0] = textBox2.Text;
            m_XGem300.GEMSetVariable(1, nId, Value);
            m_log.Add("--> GEMSetVariable, " + nId[0] + ", " + Value[0]);
            nId[0] = 2001;
            Value[0] = "COMPLETE";
            m_XGem300.GEMSetVariable(1, nId, Value);
            m_log.Add("--> GEMSetVariable, " + nId[0] + ", " + Value[0]);
            nId[0] = 2002;
            Value[0] = "0";
            m_XGem300.GEMSetVariable(1, nId, Value);
            m_log.Add("--> GEMSetVariable, " + nId[0] + ", " + Value[0]);
            m_XGem300.GEMSetEvent(2001);
            m_log.Add("--> GEMSetEvent, 2001");
        }

        private void button22_Click(object sender, EventArgs e)
        {
            long[] nId = new long[1] { -1 };
            string[] Value = new string[1] { "" };

            nId[0] = 2000;
            Value[0] = "";
            m_XGem300.GEMSetVariable(1, nId, Value);
            m_log.Add("--> GEMSetVariable, " + nId[0] + ", " + Value[0]);
            m_XGem300.GEMSetEvent(2000);
            m_log.Add("--> GEMSetEvent, 2000");
        }

        public virtual void ResetXGem()
        {
            for (int i = 0; i < m_nLoadPort; i++)
            {
                m_XGem300Process[i].m_eJobState = XGem300Process.eJobState.Stopped;
                m_XGem300Control[i].m_eState = XGem300Control.eState.Completed;
            }
            m_XGem300.CJDelAllJobInfo();
            m_XGem300.PJDelAllJobInfo();
            m_XGem300.CMSDelAllCarrierInfo();
            m_log.Add("--> CMSDelAllCarrierInfo");

            for (int i = 0; i < m_nLoadPort; i++)
            {

                if (m_auto.ClassHandler().ClassLoadPort(i).IsPlaced())
                    SetLPInfo(i, XGem300Carrier.eLPTransfer.TransferBlocked, XGem300Carrier.ePortAccess.Manual, XGem300Carrier.ePortReservationState.NotReserved, XGem300Carrier.ePortAssocitionState.NotAssociated, "");
                else
                    SetLPInfo(i, XGem300Carrier.eLPTransfer.ReadyToLoad, XGem300Carrier.ePortAccess.Manual, XGem300Carrier.ePortReservationState.NotReserved, XGem300Carrier.ePortAssocitionState.NotAssociated, "");
            }
        }

        private void button29_Click(object sender, EventArgs e)
        {
            //m_XGem300.GEMSetEvent(11401);
            ResetXGem();
        }

        public void CarrierOnEvent(int nLP)
        {
            SetLPPresentSensor(nLP, true);
            SetLPCarrierOnOff(nLP, true);
        }

        public void CarrierOffEvent(int nLP)
        {
            m_log.Add("Send CarrierOffEvent"); 
            m_XGem300.CMSSetReadyToLoad("LP" + (nLP + 1).ToString());
            m_log.Add("--> CMSSetReadyToLoad, " + "LP" + (nLP + 1).ToString());
            SetLPPresentSensor(nLP, false);
            SetLPCarrierOnOff(nLP, false);
        }

        private void button30_Click(object sender, EventArgs e)
        {
            CarrierOnEvent(comboBox4.SelectedIndex);
        }


        private void button28_Click(object sender, EventArgs e)
        {
            SetCarrierID(comboBox4.SelectedIndex, textBox4.Text);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            Info_Carrier TestCarrier = new Info_Carrier(comboBox4.SelectedIndex);
            if (comboBox4.SelectedIndex == 0)
                TestCarrier.Init("LP1", m_log, Wafer_Size.eSize.mm300_RF);
            else if (comboBox4.SelectedIndex == 1)
                TestCarrier.Init("LP2", m_log, Wafer_Size.eSize.mm300_RF);
            TestCarrier.m_strCarrierID = textBox4.Text;
            for (int i = 0; i < 25; i++)
            {
                if (textBox3.Text.Substring(i, 1) == "1")
                {
                    TestCarrier.m_infoWafer[i].State = Info_Wafer.eState.Exist;
                }
                else
                {
                    TestCarrier.m_infoWafer[i].State = Info_Wafer.eState.Empty;
                }
            }
            m_XGem300.CMSSetSlotMapStatus(TestCarrier.m_strCarrierID, 1);

            SetSlotMap(TestCarrier);
        }

        protected void SetTestSlotMap(int nLP)
        {
            Info_Carrier TestCarrier = new Info_Carrier(nLP);
            TestCarrier.Init("LP" + (nLP + 1).ToString(), m_log, Wafer_Size.eSize.mm300_RF);
            TestCarrier.m_strCarrierID = textBox4.Text;
            for (int i = 0; i < 25; i++)
            {
                if (textBox3.Text.Substring(i, 1) == "1")
                {
                    TestCarrier.m_infoWafer[i].State = Info_Wafer.eState.Exist;
                }
                else
                {
                    TestCarrier.m_infoWafer[i].State = Info_Wafer.eState.Empty;
                }
            }
            m_XGem300.CMSSetSlotMapStatus(TestCarrier.m_strCarrierID, 1);

            SetSlotMap(TestCarrier);
        }

        public virtual void ProcessEnd(int nLP)
        {
            long nResult = m_XGem300.PJSetState(m_XGem300Process[nLP].m_sJobID, (long)XGem300Process.eJobState.ProcessingComplete);
            if (nResult == 0) m_log.Add("--> PJSetState, " + m_XGem300Process[nLP].m_sJobID + ", " + XGem300Process.eJobState.ProcessingComplete.ToString());
            else m_log.Add("PJSetState Fail, ErrCode : " + nResult.ToString());
        }

        private void button18_Click(object sender, EventArgs e)
        {
            ProcessEnd(cbLPNum.SelectedIndex);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            CarrierOffEvent(cbLPNum.SelectedIndex);
        }

        public virtual void SendNISTTargetData(string sWaferID, string sRcpID, int nType, int nRepeatCount, double Resolution, int nDefectCnt) { return; }

        private void button24_Click(object sender, EventArgs e)
        {
            try
            {
                SendNISTTargetData(textBox2.Text, m_auto.ClassRecipe().m_sRecipe, cbPMType.SelectedIndex, Convert.ToInt32(tbRepeat.Text), Convert.ToDouble(tbResolution.Text), Convert.ToInt32(tbDefect.Text));
            }
            catch (Exception ex)
            {
                m_log.Add("Send Fail : " + ex.Message);
            }
        }

        public enum eProcessState
        {
            IDLE = 0,
            RUN = 1,
            STOP = 2,
        }


        public void SendChangeServiceStatus(string sLocID, int nState)
        {
            m_XGem300.CMSReqChangeServiceStatus(sLocID, nState);
            m_log.Add("--> CMSReqChangeServiceStatus, " + sLocID + ", " + nState.ToString());
        }

        public void CMSReqChangeAccess(int nMode, string sLocID)
        {
            m_XGem300.CMSReqChangeAccess(nMode, sLocID);
            m_log.Add("--> CMSReqChangeAccess, " + nMode.ToString() + ", " + sLocID);
        }

        public string GetStepID()
        {
            return m_strStepID;
        }

        public void SetFDCValue(int VID, string Value)
        {
            long nCount = 1;
            long[] pnVid = new long[1] { 0 };
            string[] psValue = new string[1] { "0" };
            pnVid[0] = VID;
            psValue[0] = Value;
            if (m_XGem300 == null) return;
            m_XGem300.GEMSetVariable(nCount, pnVid, psValue);
        }

        /// <summary>
        /// 천안 2.5D 관련 사용함수.
        /// </summary>
        public enum ePortState25D
        {
            LOAD,
            UNLOAD,
        }

        public enum eCarrierType
        {
            FOUP, FOSB, MAGAZINE, MAC, TRAY, CASSETTE,
        }
        public virtual void Event_PortStateChange(XGem300_Mom.ePortState25D PortState, int nPort, string sLotID, string sCSTID, eCarrierType CarrierType) { }
        public virtual void Event_RFIDRead(string sCSTID, string sLOTID) { }
        public virtual void Event_LotInfoReq(string sCSTID, string sLOTID) { }
        public virtual void Evnet_SlotInfoReq(string sCSTID, string sLOTID) { }
        public virtual void Event_TKInReq(string sCSTID, string sLOTID) { }
        public virtual void Event_TKOutReq(string sCSTID, string sLOTID) { }

        private void button26_Click(object sender, EventArgs e)
        {
            SendDCOLMSG("KCK0064-09");
            SendDCOLMSG("TCE0065-15");
        }

        public enum DColData_2DBump
        {
            Zone_Num,
            Min_DATA,
            MAX_DATA,
            Avg_DATA,
            Fail_Bump_CNT,
            Fail_Portion,
            UpperAVG_DATA,
            Stdev,
            PassFail,
        }

        string sDColFileSplitter = ",";
        public void SendDCOLMSG(string sWAFERID)
        {
            try
            {
                string spath = @"C:\AVIS\" + sWAFERID + "-NoProcess_24Zone.csv";

                string[] aFileData;
                int nStartIndex = 0;
                aFileData = File.ReadAllLines(spath);

                for (int i = 0; i < aFileData.Length; i++)
                {
                    if (aFileData[i].IndexOf("Zone Num") >= 0)
                    {
                        nStartIndex = i + 1;
                    }
                }

                //메세지 생성//
                int nZoneNum = aFileData.Length - nStartIndex;
                long nObjID = 0;
                m_XGem300.MakeObject(ref nObjID);
                m_XGem300.SetListItem(nObjID, 4);
                m_XGem300.SetBoolItem(nObjID, true);
                m_XGem300.SetUint4Item(nObjID, 0);
                m_XGem300.SetUint4Item(nObjID, 120);
                m_XGem300.SetListItem(nObjID, Enum.GetNames(typeof(DColData_2DBump)).Length + 4);


                m_XGem300.SetListItem(nObjID, 2);
                m_XGem300.SetStringItem(nObjID, "LotID");
                m_XGem300.SetListItem(nObjID, 1);
                m_XGem300.SetStringItem(nObjID, sWAFERID.Substring(0, 7));

                m_XGem300.SetListItem(nObjID, 2);
                m_XGem300.SetStringItem(nObjID, "WaferID");
                m_XGem300.SetListItem(nObjID, 1);
                m_XGem300.SetStringItem(nObjID, sWAFERID);

                m_XGem300.SetListItem(nObjID, 2);
                m_XGem300.SetStringItem(nObjID, "SlotID");
                m_XGem300.SetListItem(nObjID, 1);
                m_XGem300.SetStringItem(nObjID, sWAFERID.Substring(8, sWAFERID.Length - 8));

                m_XGem300.SetListItem(nObjID, 2);
                m_XGem300.SetStringItem(nObjID, "Zone CNT");
                m_XGem300.SetListItem(nObjID, 1);
                m_XGem300.SetStringItem(nObjID, nZoneNum.ToString());


                string[,] aData = new string[nZoneNum, Enum.GetNames(typeof(DColData_2DBump)).Length];
                //////////////
                for (int i = nStartIndex; i < aFileData.Length; i++)
                {
                    int nTemp = 0;
                    string sData = aFileData[i];

                    if (Int32.TryParse(sData.Substring(0, sData.IndexOf(sDColFileSplitter)), out nTemp))
                    {
                        int nNum = 0;
                        aData[i - nStartIndex, nNum] = sData.Substring(0, sData.IndexOf(sDColFileSplitter));
                        nNum++;
                        sData = sData.Substring(sData.IndexOf(sDColFileSplitter) + 1);
                        sData = sData.Substring(sData.IndexOf(sDColFileSplitter) + 1);      //ZoneX SKIP
                        sData = sData.Substring(sData.IndexOf(sDColFileSplitter) + 1);      //ZoneY SKIP

                        while (sData.IndexOf(sDColFileSplitter) >= 0)
                        {
                            aData[i - nStartIndex, nNum] = sData.Substring(0, sData.IndexOf(sDColFileSplitter));
                            nNum++;
                            sData = sData.Substring(sData.IndexOf(sDColFileSplitter) + 1);
                        }
                        aData[i - nStartIndex, nNum] = sData;

                    }
                }

                for (int i = 0; i < Enum.GetNames(typeof(DColData_2DBump)).Length; i++)
                {
                    m_XGem300.SetListItem(nObjID, 2);
                    m_XGem300.SetStringItem(nObjID, Enum.GetName(typeof(DColData_2DBump), i));
                    m_XGem300.SetListItem(nObjID, nZoneNum);
                    for (int Zone = 0; Zone < nZoneNum; Zone++)
                    {
                        m_XGem300.SetStringItem(nObjID, aData[Zone, i]);
                    }
                }
                m_XGem300.SendSECSMessage(nObjID, 6, 9, 0);
            }
            catch (Exception ex)
            {
                m_log.Add(ex.ToString());
                return;
            }
        }

        public virtual void SaveRecipeFile(string sRecipeName) { }
        public virtual void SetMSBAlarm(XGem300_Mom.eMSBAlarm MSBAlarm) { }
        public virtual void ResetMSBAlarm() { }
        public virtual void SetProcessState(XGem300_Mom.eProcessState ProcessState) { }
        public virtual void SendDailyWaferFDC(Double dDaliyGV) { }
    }

}
