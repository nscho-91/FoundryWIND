using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ezAutoMom;
using ezTools;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using ezCam;

namespace ezAuto_EFEM
{
    public class HW_LoadPort_RND : HW_LoadPort_Mom, Control_Child 
    {
        delegate void CallRS232(string[] sMsgs);

        #region enum

        enum eError
        {
            Error,
            Connect,
            Timeout,
            Protocol,
            SlotMapDouble,
            SlotMapCross
        } 
        #endregion

        int m_msMotion = 20000;
        int m_msRS232 = 20000;
        bool m_bFirstHome = true;
        bool m_bNeedInit = false;
        
        const char ACK = (char)0x06;
        const char NAK = (char)0x15;
        const char BUSY = (char)0x11;
        const char SPLITTER = ' ';
        const string True = "1";
        const string False = "0";
        bool m_bWaferCheck = false; 
        //command

        const string CMD_HOME = "ORG";                                  // Home
        const string CMD_RESET_CPU = "DRT";                             // error시 명령어가 듣지 않음. 리셋.
        const string CMD_LOAD = "RLMP";
        const string CMD_DOCKING = "RLFD";
        const string CMD_LOADINDOCK = "RLDN";
        const string CMD_UNLOAD = "RUNP";
        const string CMD_ANY_UNLOAD = "RUFI";
        const string CMD_LOAD_MAC = "RLMPMAC";
        const string CMD_UNLOAD_MAC = "RUNPMAC";
        const string CMD_CHANGE_RINGSTATE = "COMAC";
        const string CMD_CHANGE_300STATE = "COFOUP";
        const string CMD_UNDOCKING = "CLUD";
        const string CMD_STOP = "";
        const string CMD_GETMAPDATA = "MLD 1";
        const string CMD_GETMAPDATA_MAC = "MLD 2";
        const string READ_ERROR = "ERR";        // 발생한 에러를 읽음. History 0~1000
        const string CMD_LIFTUP = "CPDN";
        const string CMD_LIFTDOWN = "CPUP";
        const string CMD_LEDON_AUTO = "IOT 2-3:+";
        const string CMD_LEDON_MANUAL = "IOT 2-7:+";
        const string CMD_LEDOFF_AUTO = "IOT 2-3:-";
        const string CMD_LEDOFF_MANUAL = "IOT 2-7:-";
        const string CMD_LED_LOAD_ON = "IOT 2-1:+";
        const string CMD_LED_LOAD_OFF = "IOT 2-1:-";
        const string CMD_LED_UNLOAD_ON = "IOT 2-2:+";
        const string CMD_LED_UNLOAD_OFF = "IOT 2-2:-";
        const string CMD_DOOR_OPEN = "DROP"; 
        const string CMD_DOOR_CLOSE = "DRCL"; 
        const string CMD_CHECKWAFER = "CKWF"; 
        const string CMD_DOOR_UP = "DRUP";
        const string CMD_DOOR_DOWN = "DRDN";
        const char SPLITFLAG = ':';
        const char ENDFLAG = ';';
        const char PARAMFLAG = '/';

        const int c_nERR = 50;
        string[,] m_sErrorMsgs = new string[c_nERR, 2]
        {
            {"E02", "Loadport Busy Error"},
            {"201", "FOUP Clamp Up 동작 Error, Sensor 확인 Timeout"},
            {"202", "FOUP Clamp Down동작 Error, Sensor 확인 Timeout"},
            {"203", "FOUP Clamp LOCK 동작 Error, Sensor 확인 Timeout"},
            {"204", "FOUP Clamp Forward 동작 Error, Sensor 확인 Timeout"},
            {"205", "FOUP Clamp Backward 동작 Error, Sensor 확인 Timeout" },
            {"206", "FOUP Docking 동작 Error, Sensor 확인 Timeout"},
            {"207", "FOUP Undocking 동작 Error, Sensor 확인 Timeout"},
            {"208", "Door Latch 동작 Error, Sensor 확인 Timeout"},
            {"209", "Door Unlatch 동작 Error, Sensor 확인 Timeout"},
            {"210", "Door Suction On 동작 Error, Sensor 확인 Timeout"},
            {"211", "Door Suction Off 동작 Error, Sensor 확인 Timeout"},
            {"212", "Door Open 동작 Error, Sensor 확인 Timeout"},
            {"213", "Door Close 동작 Error, Sensor 확인 Timeout"},
            {"214", "Mapping Arm HOME 위치 Error, Sensor 확인 Timeout"},
            {"215", "Mapping Arm MAP 위치 Error, Sensor 확인 Timeout"},
            {"216", "INTER LOCK – Wafer가  FOUP으로부터 돌출 되었습니다."},
            {"217", "INTER LOCK – FOUP의 DOOR가 Loadport의 DOOR에 부착되어 있어야 합니다."},
            {"221", "INTER LOCK - FOUP Clamper가 UP 상태이어야 합니다."},
            {"222", "INTER LOCK - FOUP Clamper가 DOWN 상태이어야 합니다."},
            {"223", "INTER LOCK - FOUP Clamper가 LOCK 상태이어야 합니다."},
            {"224", "INTER LOCK - FOUP Clamper가 Forward 상태이어야 합니다."},
            {"225", "INTER LOCK - FOUP Clamper가 Backward 상태이어야 합니다."},
            {"226", "INTER LOCK – FOUP이 Docking 상태이어야 합니다."},
            {"227", "INTER LOCK – FOUP이 Undocking 상태이어야 합니다."},
            {"228", "INTER LOCK – Door Latch key가 Latch 상태이어야 합니다."},
            {"229", "INTER LOCK - Door Latch key가 Unlatch 상태이어야 합니다."},
            {"230", "INTER LOCK - Door Suction이 ON 상태이어야 합니다."},
            {"231", "INTER LOCK - Door Suction이 OFF 상태이어야 합니다."},
            {"232", "INTER LOCK - Door가 Open 상태이어야 합니다."},
            {"233", "INTER LOCK – Door가 Close 상태이어야 합니다."},
            {"234", "INTER LOCK - 매핑 암이 HOME 위치 상태이어야 합니다."},
            {"235", "INTER LOCK - 매핑 암이 MAP 위치 상태이어야 합니다."},
            {"236", "INTER LOCK – DOOR가 UP 상태이어야 합니다."},
            {"237", "INTER LOCK – DOOR가 DOWN 상태이어야 합니다."},
            {"238", "INTER LOCK – 매핑 시작 위치 이동 상태이어야 합니다."},
            {"239", "INTER LOCK – 매핑 끝위치 이동 상태이어야 합니다."},
            {"240", "INTER LOCK - 웨이퍼 돌출감지 error 입니다."},
            {"241", "INTER LOCK - MAIN AIR가 error 상태 입니다."},
            {"242", "INTER LOCK – FOUP이 정상적으로 놓여지지 않은 상태 입니다."},
            {"243", "INTER LOCK – FOUP이 없는 상태 입니다."},
            {"244", "INTER LOCK - OBSTACLE DETECTION error 상태 입니다."},
            {"245", "INTER LOCK - 매핑 위치 상태이어야 합니다."},
            {"246", "LOADING 조건 error 상태입니다."},
            {"247", "UNLOADING 조건 error 상태입니다."},
            {"248", "로딩중에 Safety Bar가 감지되었습니다"},
            {"251", "카세트의 웨이퍼 얼라인 센서 미 감지 error"},
            {"252", "MAC 의 ring frame 얼라인 센서 미 감지 error"},
            {"253", "Main door open 센서 미 감지 error"},
            {"254", "Main door close 센서 미 감지 error"},
        }; 

        //Error cmd
        //const string ERR_FOUPCLAMPUP = "201";
        //const string ERR_FOUPCLAMPDOWN = "202";
        //const string ERR_FOUPCLAMPLOCK = "203";
        //const string ERR_FOUPCLAMPFORE = "204";
        //const string ERR_FOUPCLAMPBACK = "205";
        //const string ERR_FOUPDOCK = "206";
        //const string ERR_FOUPUNDOCK = "207";
        //const string ERR_DOORLATCH = "208";
        //const string ERR_DOORUNLATCH = "209";
        //const string ERR_DOORSUCTIONON = "210";
        //const string ERR_DOORSUCTIONOFF = "211";
        //const string ERR_DOOROPEN = "212";
        //const string ERR_DOORCLOSE = "213";
        //const string ERR_MAPARMHOME = "214";
        //const string ERR_MAPARMMAPPOS = "215";
        //const string ERR_WAFERPROTRUTION = "240";
        //const string ERR_MAINAIR = "241";
        //const string ERR_NOPRODUCT = "243";
        //const string ERR_LODING = "246";
        //const string ERR_UNLODING = "247";
        //const string ERR_SAFTYBAR = "248";
        //const string ERR_COVEROPEN = "249";
        //const string ERR_CSTALIGN = "251";
        //const string ERR_MACALIGN = "252";
        //const string ERR_MAINDOOROPEN = "253";
        //const string ERR_MAINDOORCLOSE = "254";
        //const string ERR_LPBUSY = "E02";
		
        #region Define
        const int BUF_SIZE = 4096;
        #endregion


        ezRS232 m_rs232;
        CallRS232 m_cbRS232 = null;
        char[] m_aBuf = new char[BUF_SIZE];     //RS232 Recieve Data Buffer
        string m_sLastCmd = "";

        ezCam_CognexBCR m_cogBCR = new ezCam_CognexBCR(); // ing for BCR
        HW_RnR_Mom m_RnR;

        int m_diPresent = -1;
        int m_diPlacement = -1;
        int m_diSwitch = -1;
        int m_diOpen = -1;
        int m_diUnloadSwitch = -1;
        int m_nLPNum = 0;
        int m_diDocking = -1;   //KDG 161216 KDG Add Docking

        bool m_bAutoLoadMode = false;
        bool m_bEnableLotEnd = false;
        bool m_bSlotMapFail = false;
        
        DateTime m_cstIncomingTime; // BHJ 190723 add

        //BaudRate 56000
        //Data 8
        //Stop 1
        //Parity&flow None
        public override void Init(int nID, Auto_Mom auto)
        {
            string strSize="mm300";
            int nMode = 0, nType = 0;
            m_nLPNum = nID;
            base.Init(nID, auto);
            m_control.Add(this);
            m_log.m_reg.Read("Mode", ref nMode);
            m_log.m_reg.Read("WaferSize", ref strSize);
            m_log.m_reg.Read("TypeSlot", ref nType); // ing 171106
            m_infoCarrier.m_eSlotType = (Info_Carrier.eSlotType)nType;  // ing 171106
            Wafer_Size.eSize eSize = (Wafer_Size.eSize)Enum.Parse(typeof(Wafer_Size.eSize),strSize);
            m_infoCarrier.Init(m_id + "InfoCarrier", m_log, eSize);
            //m_cogBCR.Init("BCR", m_log);
            m_RnR = m_auto.ClassRnR();
            m_rs232 = new ezRS232(m_id + "_RS232", m_log);
            m_rs232.CallMsgRcv += m_rs232_CallMsgRcv;

            RunGrid(eGrid.eRegRead);
            RunGrid(eGrid.eInit);
            InitString();
            bool bConnect = false;
            for (int n = 0; n < 5; n++)
            {
                bConnect = m_rs232.Connect(true);
                if (bConnect) break;
                Thread.Sleep(100);
            }
            if (!bConnect) SetAlarm(eAlarm.Stop, (int)eError.Connect); 
        }

        public override void ThreadStop()
        {
            base.ThreadStop();
            m_rs232.ThreadStop();
        }

        void InitString()
        {
            InitString((int)eError.Error, "Error");
            InitString((int)eError.Connect, "로드포트 연결상태를 확인하여 주세요.");
            InitString((int)eError.Timeout, "Operation TimeOver !!");
            InitString((int)eError.Protocol, "RS232 Cmd Protocol Error !!");
            InitString((int)eError.SlotMapDouble, "Double Wafer Detected. Look at Logpopup for detail"); // BHJ 190605 add
            InitString((int)eError.SlotMapCross, "Cross Wafer Detected. Look at Logpopup for detail"); // BHJ 190605 add
            AddErrorMsg();
        } 

        void InitString(int eErr, string str)
        {
            m_log.AddString(str);
            if (m_xGem == null) return;
            m_xGem.AddALID(m_id, eErr, str);
        } 

        void AddErrorMsg()
        {
            
            for (int n = 0; n < m_sErrorMsgs.GetLength(0); n++)
            {
                InitString(n + Enum.GetValues(typeof(eError)).Length, m_sErrorMsgs[n, 1]);
            }
            
        } 

        int GetErrorIndex(string sErrCode)
        {
            int n = 0;
            for (n = 0; n < c_nERR; n++)
            {
                if (sErrCode == m_sErrorMsgs[n, 0]) break;
            }
            return n + Enum.GetValues(typeof(eError)).Length;
        } 

        void SetAlarm(eAlarm alarm, int eErr) 
        {
            SetState(eState.Error);
            m_work.SetError(alarm, m_log, (int)eErr);
            if (alarm != eAlarm.Popup) m_infoCarrier.m_eState = Info_Carrier.eState.Init; // ing 170408
            if (m_xGem == null) return;
            m_xGem.SetAlarm(m_id, (int)eErr);
        }

        public void ControlGrid(Control_Mom control, ezGrid rGrid, eGrid eMode)
        {
            //forgetpublic void ControlGrid(Control_Mom control, ezGrid rGrid, eGrid eMode)
            control.AddDI(rGrid, ref m_diPlacement, m_id, "Placement", "Placement Check Sensor");
            control.AddDI(rGrid, ref m_diPresent, m_id, "Present", "Present Check Sensor");
            control.AddDI(rGrid, ref m_diOpen, m_id, "Open", "Open Check Sensor");
            control.AddDI(rGrid, ref m_diSwitch, m_id, "Switch", "Loadport Switch");
            control.AddDI(rGrid, ref m_diUnloadSwitch, m_id, "UnloadSwitch", "Unload Switch");
            control.AddDI(rGrid, ref m_diDocking, m_id, "Docking", "Load Port Docking Check Sensor");
        }

        public override void RunGrid(ezTools.eGrid eMode)
        {
            m_grid.Update(eMode);
            m_rs232.RunGrid(m_grid, eMode);
            m_grid.Set(ref m_msHome, "Timeout", "Home", "Timeout (ms)");
            m_grid.Set(ref m_msMotion, "Timeout", "Motion", "Timeout (ms)");
            m_grid.Set(ref m_msRS232, "Timeout", "Home", "Timeout (ms)");
            m_infoCarrier.RunGrid(m_grid, eMode);
            m_grid.Set(ref m_bUseBCR, "BCR", "Use", "Use DM100X"); // ing for BCR
            m_grid.Set(ref m_bMSB, "BCR", "MSBMode", "Always Use CST ID = AAA");
            m_grid.Set(ref m_bUseSlimSlot, "SlimSlot", "Use SlimSlot", "Use SlimSlot");
            m_grid.Set(ref m_nSlimSlot, "SlimSlot", "TeachingPoint", "SlimSlot Teaching Point");
            m_grid.Set(ref m_bUseMultiPort, "MultiPort", "Use", "Use MultiPort");
            m_grid.Set(ref m_diRingPlacment, "MultiPort", "Ring Placement", "RingFrame Placement Sensor Num");
            m_grid.Set(ref m_bAutoLoadMode, "LoadMode", "AutoLoad", "Auto Load Mode at Manual");
            m_grid.Set(ref m_bRFIDUse, "RFID", "RFID Use", "RFID Use");
            m_cogBCR.RunGrid(m_grid, eMode);
            m_grid.Set(ref m_bCheckParticle, "Particle Check Mode", "Mode", "Check Particle Mode(Door Open/Close)"); //211210 nscho 
            m_grid.Refresh();
            //forget
        }
        #region RS232
        void m_rs232_CallMsgRcv()
        {
            int nRead;
            nRead = m_rs232.Read(m_aBuf, BUF_SIZE);
            m_aBuf[nRead] = (char)0x00;
            m_aBuf[nRead + 1] = (char)0x00;
            string sMsg = new string(m_aBuf,0, nRead);
            sMsg = sMsg.Replace("\r\n", "");
            string[] sMsgs = sMsg.Split(SPLITTER);
            for (int i = 0; i < sMsgs.Length; i++)
            {
                if (sMsgs[i] == "E12")
                {
                    m_cbRS232 = null;
                    WriteCmd(CMD_RESET_CPU);
                    return;
                }
            }
            if (sMsgs[0][0] == ACK) m_log.Add("Receive ACK");
            else m_log.Add("CMD Recieve : " + sMsg);
            if (m_cbRS232 != null)
            {
                m_cbRS232(sMsgs);
                return;
            }
        }

        void CallDelegateFunction(string sCMD)              // WriteCMD 시 m_cbRS232 인자로는 왼쪽과 같이 넣어준다.  * CheckACKAndCMD  (PC)CMD->(Module)ACK->동작->(Module)CMD->(PC)ACK 의 형태
        {                                                                      //                                                       * CheckCMD    (PC)CMD ->동작->(Module)CMD 의 형태      
            switch (sCMD)                                                      //                                                       * CheckACK (PC)CMD->동작->Module(ACK) 의 형태          
            {
                case CMD_HOME:
                case CMD_RESET_CPU:
                case CMD_LOAD:
                case CMD_DOCKING:
                case CMD_LOADINDOCK:
                case CMD_UNLOAD:
                case CMD_ANY_UNLOAD:
                case CMD_LOAD_MAC:
                case CMD_UNLOAD_MAC:
                case CMD_CHANGE_300STATE:
                case CMD_CHANGE_RINGSTATE:
                case CMD_STOP:
                case CMD_GETMAPDATA:
                case CMD_GETMAPDATA_MAC:
                case READ_ERROR:
                case CMD_LIFTUP:
                case CMD_LIFTDOWN:
                case CMD_CHECKWAFER: //211210 nscho 돌출감지
                case CMD_DOOR_OPEN:
                case CMD_DOOR_CLOSE:
                case CMD_DOOR_UP:
                case CMD_DOOR_DOWN:
                    m_cbRS232 = CheckCMD;
                    break;
                case CMD_LEDON_AUTO:
                case CMD_LEDOFF_AUTO:
                case CMD_LEDOFF_MANUAL:
                case CMD_LEDON_MANUAL:
                    m_cbRS232 = null;
                    break;
            }
        }

        void WriteCmd(string str)
        {
            if (str == CMD_GETMAPDATA || str == CMD_GETMAPDATA_MAC)
                m_sLastCmd = str.Substring(0, 3);
            else
                m_sLastCmd = str;
            CallDelegateFunction(str);
            str = str + (char)0x0D + (char)0x0A;
            char[] sChar = str.ToCharArray();
            m_rs232.Write(sChar, str.Length, true);
            m_log.Add("CMD Send : " + m_sLastCmd);
        }
        string m_sMap = null; 
        void CheckCMD(string[] sMsgs)
        {
            if (sMsgs.Length > 1 && m_sLastCmd != CMD_GETMAPDATA.Substring(0,3))
            {
                string str = ConvertErrorMSG(sMsgs[1]);
                if (str != null)
                {
                    m_log.Popup(str);
                    SetState(eState.Error);
                    m_cbRS232 = null;
                    return;
                } 
                if (sMsgs[1] == "240") m_bWaferCheck = true; //211210 nscho 돌출감지
            }
            else if (sMsgs[0].Substring(0,m_sLastCmd.Length) == m_sLastCmd)
            {
                m_log.Add("Received Successfully : " + m_sLastCmd);
                if (sMsgs[0] == "MLD")
                {
                    if (SetMappingData(sMsgs[1]) == eHWResult.Error)
                    {
                        SetState(eState.Error);
                    }
                    m_sMap = sMsgs[1];  
                }
                m_cbRS232 = null;
            }
            else
            {
                m_log.Popup("Commnuication Error5");
                SetState(eState.Error);
            }
        }

        //string ConvertErrorMSG(string sErr)
        //{
        //    string sErrMSG = "RND Error MSG : ";
        //    SetState(eState.Error);   //20160909 SDH ADD
        //    #region ErrorMSG
        //    if (sErr.IndexOf(",") >= 0)
        //        return null;
        //    switch (sErr)
        //    {
        //        case ERR_DOORCLOSE:
        //            sErrMSG += "Door Close Error";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_DOORLATCH:
        //            sErrMSG += "Door Latch Error";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_DOOROPEN:
        //            sErrMSG += "Door OPen Error";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_DOORSUCTIONOFF:
        //            sErrMSG += "Door Suction On/Off Error";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_DOORSUCTIONON:
        //            sErrMSG += "Door Suction On Error";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_DOORUNLATCH:
        //            sErrMSG += "Door Unlatch Error";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_FOUPCLAMPBACK:
        //            sErrMSG += "Foup Clamp Back Error";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_FOUPCLAMPDOWN:
        //            sErrMSG += "Foup Clamp Down Error";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_FOUPCLAMPFORE:
        //            sErrMSG += "Foup Clamp Foreward Error";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_FOUPCLAMPLOCK:
        //            sErrMSG += "FoupClam Lock Error";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_FOUPCLAMPUP:
        //            sErrMSG += "Foup Clamp UP Error";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_FOUPDOCK:
        //            sErrMSG += "Foup Docking Error";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_FOUPUNDOCK:
        //            sErrMSG += "Foup Undocking Error";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_LODING:
        //            sErrMSG += "Cassette Loading Error";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_MAINAIR:
        //            sErrMSG += "Main Air Error";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_MAPARMHOME:
        //            sErrMSG += "Mapping Arm Home Error";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_MAPARMMAPPOS:
        //            sErrMSG += "Mapping Arm Position Error";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_NOPRODUCT:
        //            sErrMSG += "No Product on Loadport Error";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_SAFTYBAR:
        //            sErrMSG += "Safty bar Detected Error";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_COVEROPEN:
        //            sErrMSG += "Cover Open Error";
        //            m_bNeedInit = false;
        //            break;
        //        case ERR_UNLODING:
        //            sErrMSG += "Cassette Unloading Error";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_WAFERPROTRUTION:
        //            sErrMSG += "Wafer Protrution Detected Error";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_LPBUSY:
        //            sErrMSG += "Loadport Busy Error";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_CSTALIGN:
        //            sErrMSG += "CST Align Sensor Not Detect";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_MACALIGN:
        //            sErrMSG += "MAC Align Sensor Not Detect";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_MAINDOOROPEN:
        //            sErrMSG += "Main Door Open Sensor Not Detect";
        //            m_bNeedInit = true;
        //            break;
        //        case ERR_MAINDOORCLOSE:
        //            sErrMSG += "Main Door Close Sensor Not Detect";
        //            m_bNeedInit = true;
        //            break;
        //        default:
        //            sErrMSG += "Can not Find Error MSG";
        //            m_bNeedInit = true;
        //            break;
        //    }
        //    #endregion
        //    return sErrMSG;
        //}
        string ConvertErrorMSG(string sErr)
        {
            string sErrMSG = "RND Error MSG : Can't Find Error Message";
            SetState(eState.Error);   //20160909 SDH ADD
            #region ErrorMSG
            if (sErr.IndexOf(",") >= 0)
                return null;
            for (int n = 0; n < c_nERR; n++)
            {
                
                if (m_sErrorMsgs[n, 0] == sErr)
                {
                    SetAlarm(eAlarm.Warning, GetErrorIndex(sErr));
                    return m_sErrorMsgs[n, 1];
                }
                
            } 
            #endregion
            SetAlarm(eAlarm.Warning, (int)eError.Protocol);
            return sErrMSG;
        }

        eHWResult WaitReply(int msDelay, string sMsg)
        {
            int ms10 = 0;
            while (m_cbRS232 != null)
            {
                Thread.Sleep(10);
                ms10 += 10;
                if (ms10 > msDelay)
                {
                    SetAlarm(eAlarm.Warning, (int)eError.Timeout); 
                    m_log.Popup(m_sLastCmd + " Timeout !!");
                    return eHWResult.Error;
                }
            }
            if (GetState() == eState.Error)
                return eHWResult.Error;
            else
                return eHWResult.OK;
        }

     
        #endregion
        public bool GetLoadRequest()
        {
            if (m_control.GetInputBit(m_diSwitch) == true)
            {
                m_auto.SetCSTLoadOK(m_nLPNum, false);
                return true;
            }
            else if (m_auto.IsCSTLoadOK(m_nLPNum) && IsPlaced() && m_work.GetState() == eWorkRun.Run && CheckState_OtherLP(m_nLPNum, "!=", eState.RunLoad))
            {
                m_auto.SetCSTLoadOK(m_nLPNum, false);
                if (m_xGem.IsOnline())
                {
                    m_auto.ClassXGem().SetLPCarrierOnOff(m_nLPNum, true);
                    m_auto.ClassXGem().SetLPPresentSensor(m_nLPNum, true);
                    m_auto.ClassXGem().SetCMSTransferState(XGem300Carrier.eLPTransfer.TransferBlocked, m_nLPNum);
                }
                return true;
            }
            else if (m_auto.m_bXgemUse && m_xGem.m_aXGem300Carrier!= null && 
                m_xGem.m_aXGem300Carrier[m_nLPNum].m_ePortAccess == XGem300Carrier.ePortAccess.Manual 
                && IsPlaced() && m_eAutoLoading == eAutoLoading.eLoading && !(m_auto.ClassHandler().m_bDontStartLoading[m_nLPNum]))
            {
                m_auto.SetCSTLoadOK(m_nLPNum, false);
                return true;
            }
            else if (IsPlaced() && m_eAutoLoading == eAutoLoading.eLoading && !(m_auto.ClassHandler().m_bDontStartLoading[m_nLPNum]) && m_bReload)
            {
                m_bReload = false;
                m_auto.SetCSTLoadOK(m_nLPNum, false);
                return true;
            }
            return false;
        }

        public enum eAutoLoading
        {
            eCSTDetect,
            eWaitCST,
            eLoading,
            eUnloading
        }
        public eAutoLoading m_eAutoLoading = eAutoLoading.eCSTDetect;

        private ezStopWatch m_swAutoLoad = new ezStopWatch();

        private void AutoloadingProcess()
        {
            switch (m_eAutoLoading)
            {
                case eAutoLoading.eCSTDetect:
                    if (IsPlaced() && m_xGem.m_aXGem300Carrier[m_nLPNum].m_ePortAccess == XGem300Carrier.ePortAccess.Manual && m_work.GetState() == eWorkRun.Run)
                    {
                        m_swAutoLoad.Start();
                        m_eAutoLoading = eAutoLoading.eWaitCST;
                    }
                    break;
                case eAutoLoading.eWaitCST:  //5초 카운트 해서 eloading
                    if (!IsPlaced())
                        m_eAutoLoading = eAutoLoading.eCSTDetect;
                    else if (m_swAutoLoad.Check() > 5000)
                    {
                        m_eAutoLoading = eAutoLoading.eLoading;
                    }
                    break;
                case eAutoLoading.eLoading:  //  loading 이면 시작하고 eunloding 으로
                    break;
                case eAutoLoading.eUnloading:  // unloding 에서 place꺼지면    ewafercst;
                    if (!IsPlaced())
                        m_eAutoLoading = eAutoLoading.eCSTDetect;
                    break;
            }
        }

        public override eHWResult SetMappingData(string MapData)
        {
            bool bD = false, bC = false, bE = false;
            string sMapData = "";
            for (int i = 0; i < m_infoCarrier.m_lWafer; i++)
            {
                sMapData += MapData.Substring(i, 1);
                switch (MapData.Substring(i, 1))
                {
                    case "1":
                        if (m_infoCarrier.m_bRNR && (m_infoCarrier.m_infoWafer[i].State == Info_Wafer.eState.Done || m_infoCarrier.m_infoWafer[i].State == Info_Wafer.eState.Select))
                        {
                            m_infoCarrier.m_infoWafer[i].State = Info_Wafer.eState.Select;
                            break;
                        }
                        m_infoCarrier.m_infoWafer[i].SetExist(true);
                        break;
                    case "0":
                        m_infoCarrier.m_infoWafer[i].SetExist(false);
                        break;
                    case "D":
                        m_log.Popup("Double Wafer Detected Slot : " + (i + 1).ToString());
                        bD = true;
                        break;
                    case "C":
                        m_log.Popup("Cross Wafer Detected, Slot : " + (i + 1).ToString());
                        bC = true;
                        break;
                    default:
                        m_log.Popup("Can not be Analyzed MapData");
                        bE = true;
                        break;
                }
            }
            if ((bD || bC || bE) && !m_auto.ClassXGem().IsOnline())
            {
                m_infoCarrier.m_eState = Info_Carrier.eState.Done;
                SetState(eState.RunUnload);
                if (bD) // BHJ 190605 add
                {
                    SetAlarm(0, (int)eError.SlotMapDouble);
                }
                if (bC) // BHJ 190605 add
                {
                    SetAlarm(0, (int)eError.SlotMapCross);
                }
                return eHWResult.Error;
            }
            m_infoCarrier.m_eState = Info_Carrier.eState.MapDone;
            if (m_auto.m_strWork == Auto_Mom.eWork.SSEM.ToString())
            {
                if (((Gem_SSEM)m_work.GetSSEMGEM()).IsOnlineRemote())
                    ((Gem_SSEM)m_work.GetSSEMGEM()).SetMappingDone(sMapData);
            }
            else if (m_auto.m_bXgemUse && m_auto.ClassXGem().IsOnline() && m_work.GetState() == eWorkRun.Run && GetState() != eState.WaitProcessEnd)
            {
                if (!m_auto.ClassXGem().SetSlotMap(m_infoCarrier))
                {
                    m_bSlotMapFail = true;
                    SetState(eState.RunUnload);
                }
            }
            else if (m_auto.m_bXgemUse && m_auto.ClassXGem().IsOnline() && m_work.GetState() == eWorkRun.Run && GetState() == eState.WaitProcessEnd) // 대기중이 풉 진행하는 경우
            {
                m_xGem.m_XGem300Process[m_nLPNum].m_eJobState = XGem300Process.eJobState.Processing;
                m_xGem.SetPJState(m_nLPNum, m_xGem.m_XGem300Process[m_nLPNum].m_eJobState);
            }
            return eHWResult.OK;
        }

        void InvalidAlarm_Tick(object sender, EventArgs e, Form form, Label label)
        {
            if (form.BackColor == Color.Yellow)
            {
                form.BackColor = Color.Red;
                label.ForeColor = Color.Yellow;
            }
            else
            {
                form.BackColor = Color.Yellow;
                label.ForeColor = Color.Red;
            }
        }

        protected override void ProcInit()
        {
        }

        protected override void ProcHome()
        {
            m_log.WriteLog("Sequence", "[" + m_id + "Start]" + " Home"); //230721 nscho 
            m_bChangePlace = true;
            WriteCmd(CMD_RESET_CPU);
            if (WaitReply(m_msRS232, "Error Reset TimeOut") == eHWResult.Error)
            {
                m_xGem.SetMSBAlarm(XGem300_Mom.eMSBAlarm.LDP_Home_Erro);
                m_log.WriteLog("Sequence", "[" + m_id + "Done]" + " Home FAIL"); //230721 nscho
                SetState(eState.Error);
                return;
            }
            if (m_wtr.IsArmClose(HW_WTR_Mom.eArm.Upper) && m_wtr.IsArmClose(HW_WTR_Mom.eArm.Lower))
            {
                try
                {
                    if (m_logMars != null) m_logMars.AddFunctionLog("LP" + (m_nLPNum + 1).ToString(), "Origin", MarsLog.eStatus.Start, "$", MarsLog.eMateral.Carrier, null); 
                    WriteCmd(CMD_HOME);
                    if (WaitReply(m_msRS232, "Error Reset TimeOut") == eHWResult.Error)
                    {
                        m_xGem.SetMSBAlarm(XGem300_Mom.eMSBAlarm.LDP_Home_Erro);
                        m_log.WriteLog("Sequence", "[" + m_id + "Done]" + " Home FAIL"); //230721 nscho
                        SetState(eState.Error);
                        return;
                    }
                }
                finally
                {
                    if (m_logMars != null) m_logMars.AddFunctionLog("LP" + (m_nLPNum + 1).ToString(), "Origin", MarsLog.eStatus.End, "$", MarsLog.eMateral.Carrier, null); 
                }
                m_log.WriteLog("Sequence", "[" + m_id + "Done]" + " Home PASS"); //230721 nscho
                m_bFirstHome = false;
                m_bNeedInit = false;
                SetState(eState.Ready);
            }
            if (WaitReply(m_msHome, "Home Command Error") == eHWResult.Error)
            {
                m_xGem.SetMSBAlarm(XGem300_Mom.eMSBAlarm.LDP_Home_Erro);
                m_log.WriteLog("Sequence", "[" + m_id + "Done]" + " Home FAIL"); //230721 nscho
                SetState(eState.Error);
                return;
            }
            m_log.WriteLog("Sequence", "[" + m_id + "Done]" + " Home PASS"); //230721 nscho
            m_eAutoLoading = eAutoLoading.eCSTDetect;
            m_bEnableLotEnd = false;
            m_eUploadCycle = eUploadCycle.None;
        }

        protected override void ProcReady()
        {
            if (IsPlaced() != m_bPrevPlace)
            {
                m_bChangePlace = true;
                m_bPrevPlace = IsPlaced();
            }
            if (!IsPlaced())
                m_auto.ClassHandler().m_bDontStartLoading[m_nLPNum] = false;
            if (!GetLoadRequest())
                return;
            m_bReload = false;
            if (m_auto.m_bXgemUse && m_auto.ClassXGem().IsOnline() && m_bChangePlace)
            {
                m_auto.ClassXGem().SetLPCarrierOnOff(m_nLPNum, IsPlaced());
                m_auto.ClassXGem().SetLPPresentSensor(m_nLPNum, IsPlaced());
                m_bChangePlace = false;
            }
            if (m_bRFIDUse || m_bUseBCR)
                SetState(eState.CSTIDRead);
            else
                SetState(eState.RunLoad);
        }

        protected override void ProcCSTIDRead()
        {
            if (m_bRFIDUse)
            {
                HW_RFID_Mom RFID = m_auto.ClassHandler().ClassRFID(m_nLPNum);
                RFID.ResetID();
                ezStopWatch sw = new ezStopWatch();
                bool bRFIDFail = false;
                //RFID.LOTIDRead();                 //190516 SDH Del
                // sw.Start();                      //190516 SDH Del
                try
                {
                    if (m_logMars != null) m_logMars.AddFunctionLog("LP" + (m_nLPNum + 1).ToString(), "CarrierIDRead", MarsLog.eStatus.Start, "$", MarsLog.eMateral.Carrier, null); 
                    RFID.CSTIDRead();
                    sw.Start();
                    while (!bRFIDFail)
                    {
                        if (RFID.GetCSTID() != "")
                        {
                            m_infoCarrier.m_strCarrierID = RFID.GetCSTID();
                            m_log.Add("CST ID = " + RFID.GetCSTID());
                            m_infoCarrier.m_strCarrierID = m_infoCarrier.m_strCarrierID.Trim();
                            if (m_eUploadCycle == eUploadCycle.ReadCarrierID || m_eUploadCycle == eUploadCycle.SetCarrierComplete)
                            {
                                SetState(eState.Ready);
                                return;
                            }

                            break;
                        }
                        else if (sw.Check() > 2000)
                        {
                            RFIDReadFail RFIDFailUI = new RFIDReadFail(m_infoCarrier);
                            if (RFIDFailUI.ShowModal() == DialogResult.OK)
                            {
                                bRFIDFail = true;
                                if (m_eUploadCycle == eUploadCycle.ReadCarrierID || m_eUploadCycle == eUploadCycle.SetCarrierComplete)
                                {
                                    SetState(eState.Ready);
                                    return;
                                }
                            }
                            else // ing 170817
                            {
                                m_log.Popup("Job Cancel !!");
                                SetState(eState.Ready);
                                return;
                            }
                            break;
                        }
                    }
                }
                finally
                {
                    if (m_logMars != null) m_logMars.AddFunctionLog("LP" + (m_nLPNum + 1).ToString(), "CarrierIDRead", MarsLog.eStatus.End, m_infoCarrier.m_strCarrierID, MarsLog.eMateral.Carrier, null); 
                }
                m_cstIncomingTime = DateTime.Now; // BHJ 190723
                if (m_auto.m_bXgemUse && m_auto.ClassXGem().IsOnline() && m_work.GetState() == eWorkRun.Run && m_eUploadCycle == eUploadCycle.None)
                {
                    if (!m_auto.ClassXGem().SetCarrierID(m_nLPNum, m_infoCarrier.m_strCarrierID))
                    {
                        SetState(eState.Ready);
                        m_auto.ClassHandler().m_bDontStartLoading[m_nLPNum] = true;
                        m_auto.ClassXGem().DeleteCarrierInfo(m_infoCarrier.m_strCarrierID);
                        MessageBox.Show("RFID 인증이 완료 되지 않았습니다. TK_In Cancel 후 다시 Load버튼을 눌러 Load 하여 주십시오.");
                        return;
                    }
                }
                if (CheckState_OtherLP(m_nLPNum, ">", eState.Placed))
                {
                    SetState(eState.RunDocking);
                    return;
                }
            }
            SetState(eState.RunLoad);
        }

        public override bool CheckState_OtherLP(int m_nLPNum, string strCalc, eState State)
        {
            bool bRst = false;
            int nCount = m_auto.ClassHandler().m_nLoadPort;
            bool[] bCheck = { false, false, false };
            for (int i = 0; i < nCount; i++)
            {
                if (i == m_nLPNum) bCheck[i] = false;
                else
                {
                    if (strCalc == "==")        bCheck[i] = m_auto.ClassHandler().ClassLoadPort(i).GetState() == State;
                    else if (strCalc == "!=")   bCheck[i] = m_auto.ClassHandler().ClassLoadPort(i).GetState() != State;
                    else if (strCalc == ">")    bCheck[i] = m_auto.ClassHandler().ClassLoadPort(i).GetState() > State;
                    else if (strCalc == "<")    bCheck[i] = m_auto.ClassHandler().ClassLoadPort(i).GetState() < State;
                    else if (strCalc == "<=")   bCheck[i] = m_auto.ClassHandler().ClassLoadPort(i).GetState() <= State;
                    else bCheck[i] = false;

                    if (bCheck[i]) bRst = true;
                }
            }
            return bRst;

        }

        private int GetOtherWaitProcessLPIndex()
        {
            int waitProcessLoadportNum = -1;
            for (int i = 0; i < m_auto.ClassHandler().m_nLoadPort; i++)
            {
                if (i != m_nID)
                {
                    if (m_auto.ClassHandler().ClassLoadPort(i).GetState() == eState.WaitProcessEnd)
                    {
                        waitProcessLoadportNum = i;
                    }
                }
            }
            return waitProcessLoadportNum;
        }

        private bool IsOtherPortFirst(int nOtherLPIndex)
        {
            HW_LoadPort_RND otherLP = (HW_LoadPort_RND)m_auto.ClassHandler().ClassLoadPort(nOtherLPIndex);
            if (otherLP.m_cstIncomingTime == null)
            {
                return false;
            }
            // 값 설명 0보다 작음  이 인스턴스는 value보다 이전입니다.
            //        Zero      이 인스턴스는 value과 같습니다.
            //        0보다 큼   이 인스턴스는 value보다 이후입니다.
            int timeDiff = this.m_cstIncomingTime.CompareTo(otherLP.m_cstIncomingTime); 
            return timeDiff > 0;
        }

        protected override void ProcDocking()
        {
            //if (CheckState_OtherLP(m_nLPNum, "==", eState.RunUnload)) return; // BHJ 191007 add
            //if (CheckState_OtherLP(eState.CSTIDRead, eState.LoadDone)) return; // BHJ 191007 add

            //m_wtr.SetPause();  // BHJ 191121 comment
            //Thread.Sleep(5000);// BHJ 191121 comment

            m_bSlotMapFail = false;
            m_infoCarrier.ResetWaferID();
            if (m_infoCarrier.m_bRNR && !m_bRnRStart)           //170213 SDH ADD RnR Check
            {
                m_RnR.SetInit(m_nLPNum);
                m_bRnRStart = true;
            }
            try
            {
                if (m_logMars != null) m_logMars.AddFunctionLog("LP" + (m_nLPNum + 1).ToString(), "CarrierLoad", MarsLog.eStatus.Start, m_infoCarrier.m_strCarrierID, MarsLog.eMateral.Carrier, null); 
                WriteCmd(CMD_LOAD);

                if (WaitReply(m_msMotion, "Load Command Error") == eHWResult.Error)
                {
                    SetState(eState.Error);
                    return;
                }
            }
            finally
            {
                if (m_logMars != null) m_logMars.AddFunctionLog("LP" + (m_nLPNum + 1).ToString(), "CarrierLoad", MarsLog.eStatus.End, m_infoCarrier.m_strCarrierID, MarsLog.eMateral.Carrier, null);
            }
            try
            {
                if (m_logMars != null) m_logMars.AddFunctionLog("LP" + (m_nLPNum + 1).ToString(), "ReadSlotMap", MarsLog.eStatus.Start, m_infoCarrier.m_strCarrierID, MarsLog.eMateral.Carrier, null);
                WriteCmd(CMD_GETMAPDATA);
                if (WaitReply(m_msRS232, "Get Mapping Data Error") == eHWResult.Error)
                {
                    SetState(eState.Error);
                    return;
                }
            }
            finally
            {
                if (m_logMars != null)
                {
                    MarsLog.Datas datas = new MarsLog.Datas();
                    datas.Add("MapID", "", m_sMap.Substring(0, Math.Min(m_sMap.Length, m_infoCarrier.m_lWafer))); //MarsLog 2.FNC MapData
                    m_logMars.AddFunctionLog("LP" + (m_nLPNum + 1).ToString(), "ReadSlotMap", MarsLog.eStatus.End, m_infoCarrier.m_strCarrierID, MarsLog.eMateral.Carrier, datas);
                }
            }
            try
            {
                if (m_logMars != null) m_logMars.AddFunctionLog("LP" + (m_nLPNum + 1).ToString(), "CarrierUnload", MarsLog.eStatus.Start, m_infoCarrier.m_strCarrierID, MarsLog.eMateral.Carrier, null); 
                WriteCmd(CMD_UNLOAD);
                if (WaitReply(m_msMotion, "Load Command Error") == eHWResult.Error) 
                {
                    SetState(eState.Error);
                    return;
                }
            }
            finally
            {
                if (m_logMars != null) m_logMars.AddFunctionLog("LP" + (m_nLPNum + 1).ToString(), "CarrierUnload", MarsLog.eStatus.End, m_infoCarrier.m_strCarrierID, MarsLog.eMateral.Carrier, null); 
            }
            //m_wtr.SetContinue(); // BHJ 191121 comment

            SetState(eState.WaitProcessEnd);
        }

        private bool CheckState_OtherLP(eState state1, eState state2)
        {
            bool bRst = false;
            int nCount = m_auto.ClassHandler().m_nLoadPort;
            bool[] bCheck = { false, false, false };
            for (int i = 0; i < nCount; i++)
            {
                if (i == m_nLPNum) bCheck[i] = false;
                else
                {
                    bCheck[i] = m_auto.ClassHandler().ClassLoadPort(i).GetState() > state1;
                    bCheck[i] = bCheck[i] && m_auto.ClassHandler().ClassLoadPort(i).GetState() < state2;

                    if (bCheck[i]) bRst = true;
                }
            }
            return bRst;
        }

        protected override void ProcWaitProcessEnd()
        {
            //if (CheckState_OtherLP(m_nLPNum, ">", eState.Placed) && CheckState_OtherLP(m_nLPNum, "<", eState.WaitProcessEnd))
            if (CheckState_OtherLP(eState.Placed, eState.WaitProcessEnd))
                return;
            else if (CheckState_OtherLP(m_nLPNum, "==", eState.WaitProcessEnd))
            {
                if (IsOtherPortFirst(GetOtherWaitProcessLPIndex()))
                    return;
            }
            try
            {
                if (m_logMars != null) m_logMars.AddFunctionLog("LP" + (m_nLPNum + 1).ToString(), "CarrierLoad", MarsLog.eStatus.Start, m_infoCarrier.m_strCarrierID, MarsLog.eMateral.Carrier, null); 
                WriteCmd(CMD_LOAD);

                if (WaitReply(m_msMotion, "Load Command Error") == eHWResult.Error)
                {
                    SetState(eState.Error);
                    return;
                }
            }
            finally
            {
                if (m_logMars != null) m_logMars.AddFunctionLog("LP" + (m_nLPNum + 1).ToString(), "CarrierLoad", MarsLog.eStatus.End, m_infoCarrier.m_strCarrierID, MarsLog.eMateral.Carrier, null); 
            }

            try
            {
                if (m_logMars != null) m_logMars.AddFunctionLog("LP" + (m_nLPNum + 1).ToString(), "ReadSlotMap", MarsLog.eStatus.Start, m_infoCarrier.m_strCarrierID, MarsLog.eMateral.Carrier, null); 
                WriteCmd(CMD_GETMAPDATA);
                if (WaitReply(m_msMotion, "Load Command Error") == eHWResult.Error)
                {
                    SetState(eState.Error);
                    return;
                }
            }
            finally
            {
                if (m_logMars != null)
                {
                    MarsLog.Datas datas = new MarsLog.Datas();
                    datas.Add("MapID", "", m_sMap.Substring(0, Math.Min(m_sMap.Length, m_infoCarrier.m_lWafer))); //MarsLog 2.FNC MapData
                    m_logMars.AddFunctionLog("LP" + (m_nLPNum + 1).ToString(), "ReadSlotMap", MarsLog.eStatus.End, m_infoCarrier.m_strCarrierID, MarsLog.eMateral.Carrier, datas);
                }
            } 

            if (m_auto.ClassHandler().IsWaferExistInEQP() == false) {
                m_auto.ClassHandler().ClassVisionWorks().SendLotStart(m_infoCarrier.m_strLotID);
                m_auto.ClassHandler().ClassAligner().LotStart();
                m_bEnableLotEnd = true;
            }
            if (m_work.GetState() == eWorkRun.Run)
            {
                m_work.m_cRunData.dInTime = DateTime.Now;
            }

            SetState(eState.LoadDone);
            m_log.WriteLog("Sequence", "[Lot Start]" + " Portnum : " + (m_infoCarrier.m_nLoadPort + 1).ToString()); // 230721 nscho
        }

        protected override void ProcLoad()
        {
            m_bSlotMapFail = false;
            m_infoCarrier.ResetWaferID();
            if (m_infoCarrier.m_bRNR && !m_bRnRStart)           //170213 SDH ADD RnR Check
            {
                m_RnR.SetInit(m_nLPNum);
                m_bRnRStart = true;
            }

            if (m_auto.m_bXgemUse && m_auto.ClassXGem().IsOnline() && m_work.GetState() == eWorkRun.Run){
                m_auto.ClassXGem().SetProcessState(XGem300_Mom.eProcessState.RUN);
                m_auto.ClassXGem().m_XGem300Control[m_nLPNum].m_eState = XGem300Control.eState.Queued;
                m_auto.ClassXGem().m_XGem300Process[m_nLPNum].m_eJobState = XGem300Process.eJobState.Queued;
            }
            try
            {
                if (m_logMars != null) m_logMars.AddFunctionLog("LP" + (m_nLPNum + 1).ToString(), "CarrierLoad", MarsLog.eStatus.Start, m_infoCarrier.m_strCarrierID, MarsLog.eMateral.Carrier, null); 
                if (m_bUseMultiPort)
                {
                    if (CheckCarrier() == Wafer_Size.eSize.mm300_RF)
                        WriteCmd(CMD_LOAD_MAC);
                    else if (CheckCarrier() == Wafer_Size.eSize.mm300)
                        WriteCmd(CMD_LOAD);
                }
                else
                    WriteCmd(CMD_LOAD);

                if (WaitReply(m_msMotion, "Load Command Error") == eHWResult.Error)
                {
                    m_xGem.SetMSBAlarm(XGem300_Mom.eMSBAlarm.LDP_Open_Error);
                    SetState(eState.Error);
                    return;
                }
            }
            finally
            {
                if (m_logMars != null) m_logMars.AddFunctionLog("LP" + (m_nLPNum + 1).ToString(), "CarrierLoad", MarsLog.eStatus.End, m_infoCarrier.m_strCarrierID, MarsLog.eMateral.Carrier, null); 
            }
            try
            {
                if (m_logMars != null) m_logMars.AddFunctionLog("LP" + (m_nLPNum + 1).ToString(), "ReadSlotMap", MarsLog.eStatus.Start, m_infoCarrier.m_strCarrierID, MarsLog.eMateral.Carrier, null); 
                if (m_bUseMultiPort)
                {
                    if (CheckCarrier() == Wafer_Size.eSize.mm300_RF)
                        WriteCmd(CMD_GETMAPDATA_MAC);
                    else if (CheckCarrier() == Wafer_Size.eSize.mm300)
                        WriteCmd(CMD_GETMAPDATA);
                }
                else
                    WriteCmd(CMD_GETMAPDATA);
                if (WaitReply(m_msRS232, "Get Mapping Data Error") == eHWResult.Error)
                {
                    m_xGem.SetMSBAlarm(XGem300_Mom.eMSBAlarm.LDP_Mapping_Error);
                    SetState(eState.Error);
                    return;
                }
            }
            finally
            {
                if (m_logMars != null)
                {
                    MarsLog.Datas datas = new MarsLog.Datas();
                    datas.Add("MapID", "", m_sMap.Substring(0, Math.Min(m_sMap.Length, m_infoCarrier.m_lWafer))); //MarsLog 2.FNC MapData
                    m_logMars.AddFunctionLog("LP" + (m_nLPNum + 1).ToString(), "ReadSlotMap", MarsLog.eStatus.End, m_infoCarrier.m_strCarrierID, MarsLog.eMateral.Carrier, datas); 
                }
            }
            
            if (m_work.GetState() == eWorkRun.Run) {
                m_work.m_cRunData.dInTime = DateTime.Now;
            }

            if (GetState() != eState.RunUnload)
            {
                SetState(eState.LoadDone);
                if (m_auto.m_strWork == EFEM.eWork.MSB.ToString())
                {
                    if (m_bMSB && m_xGem.IsOnlineRemote())
                        m_infoCarrier.m_strCarrierID = "AAA";//m_xGem.m_XGem300Process.m_sJobID; // ing 170408
                    if (m_xGem.IsOnlineRemote()) m_infoCarrier.m_strLotID = m_xGem.m_XGem300Process[m_nLPNum].m_sJobID; // ing 170329
                }
                if (m_auto.ClassHandler().IsWaferExistInEQP() == false)
                {
                    m_auto.ClassHandler().ClassVisionWorks().SendLotStart(m_infoCarrier.m_strLotID);
                    m_auto.ClassHandler().ClassAligner().LotStart();
                    m_bEnableLotEnd = true;
                    m_log.WriteLog("Sequence", "[Lot Start]" + " Portnum : " + (m_infoCarrier.m_nLoadPort + 1).ToString()); // 230721 nscho
                }   
            }
        }

        protected override void ProcDone()
        {
            if (((m_infoCarrier.IsEnableUnload()) && !(m_auto.ClassHandler().IsWaferExistInEQP())) && (m_work.GetState() == eWorkRun.Ready))
            {
                SetState(eState.RunUnload);
            }
        }

        public override eHWResult RunLiftUp()
        {
            WriteCmd(CMD_LIFTUP);
            if (WaitReply(m_msRS232, "LP Lift Up Timeout") == eHWResult.Error) {
                return eHWResult.Error;
            }
            return eHWResult.OK;
        }

        public override eHWResult RunLiftDown()
        {
            WriteCmd(CMD_LIFTDOWN);
            if (WaitReply(m_msRS232, "LP Lift Down Timeout") == eHWResult.Error) {
                return eHWResult.Error;
            }
            return eHWResult.OK;
        }

        public override eHWResult RunChange300Pos()
        {
            WriteCmd(CMD_CHANGE_300STATE);
            if (WaitReply(m_msRS232, "LP Change 300mm Position Error") == eHWResult.Error)
            {
                return eHWResult.Error;
            }
            return eHWResult.OK;
        }

        public override eHWResult RunChangeRingPos()
        {
            WriteCmd(CMD_CHANGE_RINGSTATE);
            if (WaitReply(m_msRS232, "LP Change RingCassette Position Error") == eHWResult.Error)
            {
                return eHWResult.Error;
            }
            return eHWResult.OK;
        }

        public override eHWResult RunLEDAuto()
        {
            WriteCmd(CMD_LEDOFF_MANUAL);
            WriteCmd(CMD_LEDON_AUTO);
            return eHWResult.OK;
        }

        public override eHWResult RunLEDManual()
        {
            WriteCmd(CMD_LEDOFF_AUTO);
            WriteCmd(CMD_LEDON_MANUAL);
            return eHWResult.OK;
        }

        public override eHWResult RunLEDLoad()
        {
            WriteCmd(CMD_LED_LOAD_ON);
            WriteCmd(CMD_LED_UNLOAD_OFF);
            return eHWResult.OK;

        }
        public override eHWResult RunLEDUnload()
        {
            WriteCmd(CMD_LED_LOAD_OFF);
            WriteCmd(CMD_LED_UNLOAD_ON);
            return eHWResult.OK;
        }
        
        public override eHWResult RunLoad(bool bLoad)
        {
            if (bLoad)
            {
                if (m_bUseMultiPort) {
                    if (CheckCarrier() == Wafer_Size.eSize.mm300_RF)
                        WriteCmd(CMD_LOAD_MAC);
                    else if (CheckCarrier() == Wafer_Size.eSize.mm300)
                        WriteCmd(CMD_LOAD);
                }
                else
                    WriteCmd(CMD_LOAD);
                if (WaitReply(m_msMotion, "Load Command Error") == eHWResult.Error)
                {
                    return eHWResult.Error;
                }
            }
            else
            {
                if (m_bUseMultiPort) {
                    if (CheckCarrier() == Wafer_Size.eSize.mm300_RF)
                        WriteCmd(CMD_UNLOAD_MAC);
                    else if (CheckCarrier() == Wafer_Size.eSize.mm300)
                        WriteCmd(CMD_UNLOAD);
                }
                else
                    WriteCmd(CMD_UNLOAD);
                if (WaitReply(m_msMotion, "Load Command Error") == eHWResult.Error)
                {
                    return eHWResult.Error;
                }
            }
            return eHWResult.OK;
        }

        protected override eHWResult RunMapping()
        {
            if (m_bUseMultiPort) {
                if (CheckCarrier() == Wafer_Size.eSize.mm300_RF)
                    WriteCmd(CMD_GETMAPDATA_MAC);
                else if (CheckCarrier() == Wafer_Size.eSize.mm300)
                    WriteCmd(CMD_GETMAPDATA);
            }
            else
                WriteCmd(CMD_GETMAPDATA);
            if(WaitReply(m_msRS232,"Mapping Data Error") == eHWResult.Error){
                return eHWResult.Error;
            }
            return eHWResult.OK;
        }

        public override eHWResult RunUndocking()
        {
            WriteCmd(CMD_UNDOCKING);
            if (WaitReply(m_msRS232, "Undocking Error") == eHWResult.Error)
            {
                return eHWResult.Error;
            }
            return eHWResult.OK;
        }

        protected override void ProcUnload()
        {
            if (m_eAutoLoading == eAutoLoading.eLoading)
                m_eAutoLoading = eAutoLoading.eUnloading;

            if (m_bSlotMapFail)
            {
                try
                {
                    if (m_logMars != null) m_logMars.AddFunctionLog("LP" + (m_nLPNum + 1).ToString(), "CarrierUnload", MarsLog.eStatus.Start, m_infoCarrier.m_strCarrierID, MarsLog.eMateral.Carrier, null); 
                    WriteCmd(CMD_UNLOAD);
                    if (WaitReply(m_msMotion, "Unload Command Error") == eHWResult.Error)
                    {
                        SetState(eState.Error);
                        return;
                    }
                }
                finally
                {
                    if (m_logMars != null) m_logMars.AddFunctionLog("LP" + (m_nLPNum + 1).ToString(), "CarrierUnload", MarsLog.eStatus.End, m_infoCarrier.m_strCarrierID, MarsLog.eMateral.Carrier, null); 
                }
                SetState(eState.Ready);
                if (m_auto.m_bXgemUse && m_auto.ClassXGem().IsOnline() && (m_work.GetState() == eWorkRun.Run ||  CheckState_OtherLP(m_nLPNum, "==", eState.RunDocking)))
                {
                    m_xGem.SetLPAssociation(m_nLPNum, XGem300Carrier.eCarrierAssociationState.CARRIER_COMPLETED);
                    m_xGem.SetCMSTransferState(XGem300Carrier.eLPTransfer.ReadyToUnload, m_nLPNum);
                }
                m_bSlotMapFail = false;
                return;
            }
            if (m_bEnableLotEnd)
            {
                m_auto.ClassHandler().ClassAligner().LotEnd();
                m_bEnableLotEnd = false;
            }
            if (m_auto.m_bXgemUse && m_auto.ClassXGem().IsOnline() && (m_work.GetState() == eWorkRun.Run || CheckState_OtherLP(m_nLPNum, "==", eState.RunDocking)))
            {
                m_auto.ClassXGem().JOBEnd(m_nLPNum);
            }
            if (m_auto.m_bXgemUse && (m_xGem.IsOnlineRemote() || m_xGem.IsOnlineLocal()))
            {
                m_auto.ClassXGem().ClearWafer();  
            }
            try
            {
                if (m_logMars != null) m_logMars.AddFunctionLog("LP" + (m_nLPNum + 1).ToString(), "CarrierUnload", MarsLog.eStatus.Start, m_infoCarrier.m_strCarrierID, MarsLog.eMateral.Carrier, null); 
                if (m_bUseMultiPort)
                {
                    if (CheckCarrier() == Wafer_Size.eSize.mm300_RF)
                        WriteCmd(CMD_UNLOAD_MAC);
                    else if (CheckCarrier() == Wafer_Size.eSize.mm300)
                        WriteCmd(CMD_UNLOAD);
                }
                else
                    WriteCmd(CMD_UNLOAD);

                m_log.WriteLog("Sequence", "[Lot End]" +
                    " Portnum : " + (m_infoCarrier.m_nLoadPort + 1).ToString() +
                    ", CarrierID : " + m_infoCarrier.m_strCarrierID +
                    ", LotID : " + m_infoCarrier.m_strLotID +
                    ", Recipe : " + m_infoCarrier.m_strRecipe
                    ); //230721 nscho 
                if (m_auto.m_bXgemUse && m_xGem.IsOnlineRemote())
                    m_xGem.ProcessEnd(m_nLPNum);

                if (WaitReply(m_msMotion, "Unload Command Error") == eHWResult.Error)
                {
                    m_xGem.SetMSBAlarm(XGem300_Mom.eMSBAlarm.LDP_Close_Error);
                    SetState(eState.Error);
                    return;
                }
            }
            finally
            {
                if (m_logMars != null) m_logMars.AddFunctionLog("LP" + (m_nLPNum + 1).ToString(), "CarrierUnload", MarsLog.eStatus.End, m_infoCarrier.m_strCarrierID, MarsLog.eMateral.Carrier, null); 
            }
            m_wtr.ClearIndexCarrier();
            m_auto.ClassHandler().IsWaferInfoExistInEQP();
            if (m_infoCarrier.m_bRNR && m_bRnRStart)
            {
                SetState(eState.RunLoad);
                m_RnR.UpdateRnRState(m_nLPNum);     //170213 SDH ADD LoadPort 별 RNR 갱신 (횟수 다차면 RnR 종료)
              //  m_bRnRStart = false;
            }
            else 
            {
                if ((m_work.GetState() == eWorkRun.Run || CheckState_OtherLP(m_nLPNum, "==", eState.RunDocking)))
                {
                    m_auto.ClassWork().m_cRunData.dOutTime = DateTime.Now;
                    m_auto.GetEFEMUI().AddRunData(m_work.m_cRunData);
                }

                SetState(eState.Ready);
            }
            if (m_auto.m_bXgemUse && m_auto.ClassXGem().IsOnline() && (m_work.GetState() == eWorkRun.Run || CheckState_OtherLP(m_nLPNum, "==", eState.RunDocking)))
            {
                m_xGem.SetLPAssociation(m_nLPNum, XGem300Carrier.eCarrierAssociationState.CARRIER_COMPLETED);
                m_xGem.SetCMSTransferState(XGem300Carrier.eLPTransfer.ReadyToUnload, m_nLPNum);
            }
            if (m_auto.m_bXgemUse && m_auto.ClassXGem().IsOnline())
            {
                m_xGem.m_XGem300Process[m_nLPNum].m_eJobState = XGem300Process.eJobState.Stopped;
                m_xGem.m_XGem300Control[m_nLPNum].m_eState = XGem300Control.eState.Paused;
            }

            m_infoCarrier.m_eState = Info_Carrier.eState.Done; // ing 170408
        }

        public override void SetAutoLoading()
        {
            if (((EFEM)m_auto).m_OHT.GetMainCycle(m_nLPNum) == OHT_EFEM.eMainCycle.Loadstate || ((EFEM)m_auto).m_OHT.GetMainCycle(m_nLPNum) == OHT_EFEM.eMainCycle.Init || ((EFEM)m_auto).m_OHT.GetMainCycle(m_nLPNum) == OHT_EFEM.eMainCycle.PROCESSEND)
            {
                m_auto.ClassHandler().m_bDontStartLoading[m_nID] = false;
                m_eAutoLoading = eAutoLoading.eLoading;
            }
            else
            {
                m_log.Popup("LP Autoloading Fail, Cause OHT Is Already Communiting");
            }
        }

        public override void Reset()
        {
            WriteCmd(CMD_RESET_CPU);
            if (WaitReply(m_msHome, "Home Command Error") == eHWResult.Error)
            {
                m_xGem.SetMSBAlarm(XGem300_Mom.eMSBAlarm.LDP_Comm_Error);
                SetState(eState.Error);
                return;
            }
            SetState(eState.Init);
        }
        public override eHWResult DoorOpen()
        {
            m_bWaferCheck = false;
            WriteCmd(CMD_DOOR_OPEN);
            if (WaitReply(m_msMotion, "Wafer Check") == eHWResult.Error)
            {
                SetState(eState.Error);
                return eHWResult.Error;
            }
            WriteCmd(CMD_DOOR_DOWN);
            if (WaitReply(m_msMotion, "Wafer Check") == eHWResult.Error)
            {
                SetState(eState.Error);
                return eHWResult.Error;
            }
            WriteCmd(CMD_CHECKWAFER);
            if (WaitReply(m_msMotion, "Wafer Check") == eHWResult.Error)
            {
                SetState(eState.Error);
                return eHWResult.Error;
            }
            if (m_bWaferCheck) return eHWResult.Error;
            else return eHWResult.OK;
        } 

        public override eHWResult DoorClose()
        {
            WriteCmd(CMD_DOOR_UP);
            if (WaitReply(m_msMotion, "Wafer Check") == eHWResult.Error)
            {
                SetState(eState.Error);
                return eHWResult.Error;
            }
            WriteCmd(CMD_DOOR_CLOSE);
            if (WaitReply(m_msMotion, "Wafer Check") == eHWResult.Error)
            {
                SetState(eState.Error);
                return eHWResult.Error;
            }
            return eHWResult.OK;
        } 
        protected override eHWResult IsCoverOpen(bool bOpen)
        {
            return eHWResult.Off;
        }

        protected override eHWResult IsLoad(bool bLoad)
        {
            if (m_control.GetInputBit(m_diOpen) == bLoad)
                return eHWResult.OK;
            else
                return eHWResult.Error;
        }

        protected override eHWResult IsCarrierReady()
        {
            return eHWResult.OK;
        }

        public override bool IsDoorOpenPos()
        {
            return m_control.GetInputBit(m_diOpen);
        }

        public override Wafer_Size.eSize CheckCarrier()
        {
            if (m_bUseMultiPort) {
                if (IsPlaced()) {
                    if (m_control.GetInputBit(m_diRingPlacment) && !m_control.GetInputBit(m_diPlacement))
                        return Wafer_Size.eSize.mm300_RF;
                    else if (m_control.GetInputBit(m_diPlacement) && !m_control.GetInputBit(m_diRingPlacment))
                        return Wafer_Size.eSize.mm300;
                    else
                        return Wafer_Size.eSize.Empty;
                }
                else
                    return Wafer_Size.eSize.Empty;
            }
            else {
                if (IsPlaced())
                    return m_infoCarrier.m_wafer.m_eSize;
                else
                    return Wafer_Size.eSize.Empty;
            }
        }

        public override bool IsDoorAxisMove()
        {
            return false;
        }

        public override bool IsCoverClose()
        {
            return true;
        }
        public override bool IsDocking()
        {
            //return m_control.GetInputBit(m_diOpen);       //161216 KDG 이전코드

            if (m_diDocking < 0)                            //161216 KDG Modify Docking센서 사용 유/무
                return m_control.GetInputBit(m_diOpen); 
            else
                return m_control.GetInputBit(m_diDocking);
        }
        public override bool IsPlaced()
        {
            if (UseMultiPort()) {
                if (m_control.GetInputBit(m_diPresent) && m_control.GetInputBit(m_diPlacement) && !m_control.GetInputBit(m_diRingPlacment))
                {
                    if (m_auto.ClassHandler().ClassLoadPort(m_nLPNum).m_infoCarrier.GetWaferSize() != Wafer_Size.eSize.mm300 && m_auto.ClassHandler() != null) {
                        m_auto.ClassHandler().ClassLoadPort(m_nLPNum).m_infoCarrier.m_lWafer = 25;
                        m_auto.ClassHandler().ClassLoadPort(m_nLPNum).m_infoCarrier.SetWaferSize(Wafer_Size.eSize.mm300);
                        m_auto.ClassHandler().ClassLoadPort(m_nLPNum).RunGrid(eGrid.eRegWrite);
                        m_auto.ClassHandler().ClassLoadPort(m_nLPNum).RunGrid(eGrid.eInit);

                    }
                    return true;
                }
                else if (m_control.GetInputBit(m_diPresent) && m_control.GetInputBit(m_diRingPlacment) && !m_control.GetInputBit(m_diPlacement))
                {
                    if (m_infoCarrier.GetWaferSize() != Wafer_Size.eSize.mm300_RF && m_auto.ClassHandler() != null) {
                        if (m_auto.ClassHandler().ClassLoadPort(m_nLPNum).m_infoCarrier.GetWaferSize() != Wafer_Size.eSize.mm300_RF && m_auto.ClassHandler() != null) {
                            m_auto.ClassHandler().ClassLoadPort(m_nLPNum).m_infoCarrier.SetWaferSize(Wafer_Size.eSize.mm300_RF);
                            m_auto.ClassHandler().ClassLoadPort(m_nLPNum).m_infoCarrier.m_lWafer = 13;
                            m_auto.ClassHandler().ClassLoadPort(m_nLPNum).RunGrid(eGrid.eRegWrite);
                            m_auto.ClassHandler().ClassLoadPort(m_nLPNum).RunGrid(eGrid.eInit);
                        }
                    }   
                    return true;
                }
                else
                    return false;
            }
            else
                return m_control.GetInputBit(m_diPresent) && (m_control.GetInputBit(m_diPlacement)||m_control.GetInputBit(m_diOpen)) ;
        }

        int m_bAuto = 0;  //0: Not Init 1:Manual 2: Auto
        ezStopWatch swUnload = new ezStopWatch();

        protected override void RunTimer(bool bLED)
        {
            if (m_xGem != null && m_xGem.m_aXGem300Carrier != null)
            {
                if (m_xGem.m_aXGem300Carrier[m_nLPNum].m_ePortAccess == XGem300Carrier.ePortAccess.Auto && m_bAuto != 2)
                {
                    RunLEDAuto();
                    m_bAuto = 2;
                }
                else if (m_xGem.m_aXGem300Carrier[m_nLPNum].m_ePortAccess == XGem300Carrier.ePortAccess.Manual && m_bAuto != 1)
                {
                    RunLEDManual();
                    m_bAuto = 1;
                }
            }

            if (!IsPlaced()) 
                m_infoCarrier.Clear();

            if (m_bAutoLoadMode && m_xGem.IsOnline()) AutoloadingProcess();

            switch (m_eUploadCycle)
            {
                case eUploadCycle.None:
                    break;
                case eUploadCycle.ReadCarrierID:
                    swUnload.Start();
                    SetState(eState.CSTIDRead);
                    m_eUploadCycle = eUploadCycle.SetCarrierComplete;
                    break;
                case eUploadCycle.SetCarrierComplete:
                    if (m_infoCarrier.m_strCarrierID != "" && swUnload.Check() > 1000)
                    {
                        m_xGem.DellLPInfo(m_nID);
                        //m_xGem.DeleteAllLPInfo();
                        swUnload.Start();
                        m_xGem.SendComplete(m_nLPNum, m_infoCarrier.m_strCarrierID);
                        m_eUploadCycle = eUploadCycle.DelCarrierInfo;
                    }
                    break;
                case eUploadCycle.DelCarrierInfo:
                    if (swUnload.Check() > 1000)
                    {
                        m_xGem.SetCarrierInfo(m_nLPNum, m_infoCarrier.m_strCarrierID);
                        swUnload.Start();
                        //m_xGem.SetLPInfo(m_nLPNum, XGem300Carrier.eLPTransfer.ReadyToUnload, XGem300Carrier.ePortAccess.Auto, XGem300Carrier.ePortReservationState.NotReserved, XGem300Carrier.ePortAssocitionState.NotAssociated, m_infoCarrier.m_strCarrierID);
                        m_eUploadCycle = eUploadCycle.SetLPReadyToUnload;
                    }
                    break;
                case eUploadCycle.SetLPReadyToUnload:
                    if (swUnload.Check() > 1000)
                    {
                        m_xGem.SendReadyToUnload(m_nLPNum, m_infoCarrier.m_strCarrierID);
                        m_eUploadCycle = eUploadCycle.None;
                    }
                    break;
            }
        }
    }
}
