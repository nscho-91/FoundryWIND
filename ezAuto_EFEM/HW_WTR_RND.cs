using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ezAutoMom;
using ezTools;

namespace ezAuto_EFEM
{
    class HW_WTR_RND : HW_WTR_Mom, Control_Child //forget
    {
        delegate void CallRS232(string[] sMsgs);

        #region Define
        const int BUF_SIZE = 4096;

        const char ACK = (char)0x06;
        const char NAK = (char)0x15;
        const char BUSY = (char)0x11;
        const char SPLITTER = ' ';
        const string True = "1";
        const string False = "0";
        public bool m_bFirstHome = true;
        public bool m_bNeedInit = false;

        //command
        const string CMD_READ_STATE = "PRAR VR1,VR2,VR3,VR4";     // State confirmation
        const string CMD_RESET_CPU = "DRT";         // error시 명령어가 듣지 않음. 리셋.
        const string CMD_INIT = "ORG";              // Initialize (얼라이너는 이걸로 홈)
        const string CMD_HOME = "HOME";              // 로봇 origin 위치로 이동
        const string CMD_VAC_ON_OFF = "VCP";            // Vaccuum on off
        const string CMD_GRIP_ON_OFF = "GRIP";            // Gripper on off
        const string CMD_MOVE_ARM = "TRDY";          // Arm moves to the specified position
        const string CMD_GET_WAFER = "GET";      // Pick up the wafer with the present finger
        const string CMD_PUT_WAFER = "PUT";       // Place the wafer with the present finger
        const string CMD_EXTEND_ARM = "EXTA";        // Extend the arm of the robot
        const string CMD_RETRACT_ARM = "RETA";       // Retract the arm of the robot
        const string CMD_READ_ERROR = "ERR";        // 발생한 에러를 읽음. History 0~1000
        const string CMD_ESTOP = "AES";             // WTR Emergency Stop
        const string CMD_WRITE_OUTPUT = "RIOT";     // WTR Write Output
        //Ring Frame CMD_CST 1번 Slot이 적은 곳에서 사용 CMD
        const string CMD_GET_EXTEND = "GAEXTA";      //WTR GET 세부동작1 팔 뻗고 Z축 Get 1단계 UP 동작 까지 수행                    
        const string CMD_GET_RETRACT = "GARETA";     //WTR GET 세부동작2 Grip On 이후 Get 2단계 Up 후 팔 접는 동작까지 수행
        const string CMD_PUT_EXTEND = "PAEXTA";      //WTR PUT 세부동작1 팔 뻗고 z축 PUT 1단계 Down 동작 까지 수행
        const string CMD_PUT_RETRACT = "PARETA";     //WTR PUT 세부동작2 Grip off 이후 Z축 PUT 2단계 Down 후 팔 접는 동작까지 수행
        const string CMD_GETREADY = "GRDY";
        const string CMD_PUTREADY = "PRDY";
        const string CMD_MAPPING = "MAP";
        const string CMD_MAPREAD = "MLD";
        #endregion

        #region enum
        enum eError
        {
            Connect = (int)eError_Mom.End,
            Timeout,
            Protocol,
            WaferCheckErrorDuringMove,
            NAK,
            ShiftLimit, // BHJ 190305 add
        }

        #endregion

        #region Declaration
        const int c_nERR = 194; 
        string[,] m_sErrorMsgs = new string[c_nERR, 2]
        {
            { "E01", "동작 중 실행 불가능 명령을 수신  명령 일람표 확인  "},
            { "E02", "에러 중에 구동 명령을 수신  DRT 명령 후 사용  "},
            { "E03", "이 상태에서는 명령은 수신할 수 없음  명령 일람표 확인"},
            { "E04", "정지 요구 중에 다시 정지 명령을 수신  정지 시 까지 대기"},
            { "E05", "데이터범위에러  데이터 설정 범위 확인"},
            { "E06", "포맷에러  미사용"},
            { "E07", "이동 범위가 소프트 리미트를 넘음  동작 영역 확인 및 소프트 리미트 확인"},
            { "E08", "POINT 넘버에러"},
            { "E09", "지정한 POINT 데이터 없음  지정 Point Data가 존재 하는지 확인"},
            { "E10", "이 명령은 실행할 수 없음  C-HOST 명령어 확인"},
            { "E11", "ＰＯＳ의 요청 범위가 너무 큼  수신 데이터 범위를 줄여서 사용"},
            { "E12", "지정한 명령어가 존재 하지 않음  명령어 확인"},
            { "E13", "매크로 명령어가 중복 정의되어 있음  매크로 명령어 확인"},
            { "E99", "통신 이상  통신 케이블 확인"},
            { "-786", "PROJECT_CUSTOM_ERROR   "},
            { "-1000", "Invalid Robot Number"},
            { "-1001", "Undefined Robot"},
            { "-1002", "Invalid axis number"},
            { "-1003", "Undefined axis"},
            { "-1004", "Invalid motor number"},
            { "-1005", "Undefined motor"},
            { "-1006", "Robot already attached"},
            { "-1007", "Robot not ready to be attached"},
            { "-1008", "Can’t detached a moving robot"},
            { "-1009", "Command is not completed"},
            { "-1010", "No robot selected"},
            { "-1011", "Illegal during special Cartesian mode"},
            { "-1012", "Joint out-of-range"},
            { "-1013", "Motor out-of-range"},
            { "-1014", "Time out during nulling"},
            { "-1015", "Invalid roll over spec"},
            { "-1016", "Torque control mode incorrect"},
            { "-1017", "Not in position control mode"},
            { "-1018", "Not in velocity control mode"},
            { "-1019", "Timeout sending servo setpoint"},
            { "-1020", "Timeout reading servo status"},
            { "-1021", "Robot not homed"},
            { "-1022", "Invalid homing parameter"},
            { "-1023", "Missed signal during homing"},
            { "-1024", "Encoder index disabled"},
            { "-1025", "Timeout enabling power"},
            { "-1026", "Timeout enabling amp"},
            { "-1027", "Timout starting commutation"},
            { "-1028", "Hard E-STOP"},
            { "-1029", "Asynchronous error"},
            { "-1030", "Fatal asynchronous error"},
            { "-1031", "Analog input value too small"},
            { "-1032", "Analog input value too big"},
            { "-1033", "Invalid Cartesian value"},
            { "-1034", "Negative Overtravel"},
            { "-1035", "Positive Overtravel"},
            { "-1036", "Kinematics not installed"},
            { "-1037", "Motors not commutated"},
            { "-1038", "Project generated robot error"},
            { "-1039", "Position too close"},
            { "-1040", "Position too far"},
            { "-1041", "Invalid Base transform"},
            { "-1042", "Can’t change robot config"},
            { "-1043", "Asynchronous soft error"},
            { "-1044", "Auto mode disabled"},
            { "-1045", "Soft E-STOP"},
            { "-1046", "Power not enabled"},
            { "-1047", "Virtual MCP in Jog mode"},
            { "-1048", "Hardware MCP in Jog mode"},
            { "-1049", "Timeout on homing DIN"},
            { "-1050", "Illegal during joint motion"},
            { "-1051", "Incorrect Cartesian trajectory mode"},
            { "-1052", "Beyond conveyor limits"},
            { "-1053", "Beyond conveyor limits while tracking"},
            { "-1054", "Can’t attach Encoder Only robot"},
            { "-1055", "Cartesian motion not configured"},
            { "-1522", "Can not save when power on"},
            { "-1600", "Power off requested"},
            { "-1601", "Software Reset using default settings"},
            { "-1602", "External E-STOP"},
            { "-1603", "Watchdog timer expired"},
            { "-1604", "Power light failure"},
            { "-1605", "Unknown power off request"},
            { "-1606", "E-STOP stuck off"},
            { "-1607", "Trajectory task overrun"},
            { "-1609", "E-STOP timer failed"},
            { "-1610", "Controller overheating"},
            { "-1611", "Auto/Manual switch set to Manual"},
            { "-1612", "Power supply relay stuck"},
            { "-1613", "Power supply shorted"},
            { "-1614", "Power supply overloaded"},
            { "-1615", "No 3-phase power"},
            { "-1616", "Shutdown due to overheating"},
            { "-1617", "CPU overheating"},
            { "-3000", "NULL pointer detected"},
            { "-3001", "Too many arguments"},
            { "-3002", "Too few arguments"},
            { "-3003", "Illegal value"},
            { "-3004", "Servo not initialized"},
            { "-3005", "Servo mode transition failed"},
            { "-3006", "Servo mode locked"},
            { "-3007", "Servo hash table not found"},
            { "-3008", "Servo hash entry collision"},
            { "-3009", "No hash entry found"},
            { "-3010", "Servo hash table full"},
            { "-3011", "Illegal parameter access"},
            { "-3012", "Servo high power failed"},
            { "-3013", "Servo task submission failed"},
            { "-3014", "Cal parameters not set correctly"},
            { "-3015", "Cal position not ready"},
            { "-3016", "Illegal cal seek command"},
            { "-3017", "No axis selected"},
            { "-3100", "Hard envelope error"},
            { "-3102", "Illegal zero index"},
            { "-3103", "Missing zero index"},
            { "-3104", "Motor duty cycle exceeded"},
            { "-3105", "Motor stalled"},
            { "-3106", "Axis over-speed"},
            { "-3107", "Amplifier over-current"},
            { "-3108", "Amplifier over-voltage"},
            { "-3109", "Amplifier under-voltage"},
            { "-3110", "Amplifier fault"},
            { "-3111", "Brake fault"},
            { "-3112", "Excessive dual encoder slippage"},
            { "-3113", "Motor commutation setup failed"},
            { "-3114", "Servo tasks overrun"},
            { "-3115", "Encoder quadrature error"},
            { "-3116", "Precise encoder index error"},
            { "-3117", "Amplifier RMS current exceeded"},
            { "-3118", "Dedicated DINs not config’ed for Hall"},
            { "-3119", "Illegal 6-step number"},
            { "-3120", "Illegal commutation angle"},
            { "-3121", "Encoder fault"},
            { "-3122", "Soft envelope error"},
            { "-3123", "Can’t switch serial encoder mode"},
            { "-3124", "Serial encoder busy"},
            { "-3125", "Illegal encoder command"},
            { "-3126", "Encoder operation error"},
            { "-3127", "Encoder battery low"},
            { "-3128", "Encoder battery down"},
            { "-3129", "Invalid encoder multi-turn data"},
            { "-3130", "Illegal encoder operation mode"},
            { "-3131", "Encoder not supported or mismatched"},
            { "-3132", "Trajectory extrapolation limit exceeded"},
            { "-3133", "Amplifier fault, DC bus stuck"},
            { "-3134", "Encoder data or accel/decel limit error"},
            { "-3135", "Phase offset too large"},
            { "-3136", "Cannot configure to adjust phase offset"},
            { "-3137", "Amplifier hardware failure or invalid"},
            { "-3138", "Encoder position not ready"},
            { "-3139", "Encoder not ready"},
            { "-3140", "Encoder communication error"},
            { "-3141", "Encoder overheated"},
            { "-3142", "Encoder hall sensor error"},
            { "-3143", "General serial bus encoder error"},
            { "-3144", "Amplifier overheating"},
            { "-3145", "Motor overheating"},
            { "2", "ERR_NOT_HOMED "},
            { "4", "ERR_EMERGENCY"},
            { "12", "ERR_MOTOR_ERROR"},
            { "194", "ERR_INTERLOCK"},
            { "202", "ERR_WAFER_BEFORE_GET "},
            { "203", "ERR_NO_WAFER_BEFORE_PUT"},
            { "204", "ERR_NO_WAFER_AFTER_GET"},
            { "205", "ERR_WAFER_AFTER_PUT"},
            { "206", "ERR_NO_WAFER_DURING_GET"},
            { "207", "ERR_WAFER_DURING_PUT"},
            { "208", "ERR_NOT_HOMED"},
            { "209", "ERR_NOT_SUPPORTED_FUNC "},
            { "251", "ERR_MAPPING_IS_NOT_PERFORMED"},
            { "252", "ERR_NO_MAPPING_DATA"},
            { "1001", "ERR_INVALID_COMMAND"},
            { "1011", "ERR_INVALID_DATA"},
            { "1012", "ERR_INVALID_STATION"},
            { "1013", "ERR_INVALID_HAND"},
            { "1014", "ERR_INVALID_SLOT"},
            { "1015", "ERR_INVALID_TEACHING_INDEX"},
            { "1016", "ERR_INVALID_PD_INDEX"},
            { "1017", "ERR_WAFER_DOUBLE_ERORR"},
            { "1018", "ERR_WAFER_NOEXIT_ERORR"},
            { "1021", "ERR_INVALID_COORDINATE_TYPE"},
            { "1031", "ERR_INVALID_ARGUMENT"},
            { "1033", "ERR_INVALID_FORMAT"},
            { "1034", "ERR_INVALID_LOCATION_FORMAT"},
            { "1035", "ERR_INVALID_PROFILE_FORMAT"},
            { "1041", "ERR_WRONG_PD_COMMAND"},
            { "1042", "ERR_WRONG_AWC_DATA"},
            { "1043", "ERR_NO_AWC_STATION"},
            { "1051", "ERR_NO_DATA"},
            { "1052", "ERR_NOT_HOME"},
            { "1053", "ERR_CANNOT_RETRACT_ARM"},
            { "1054", "ERR_VACUUM_DETECTING_ERORR"},
            { "1055", "ERR_NO_WAFER"},
            { "1056", "ERR_UPGRIP"},
            { "1057", "ERR_DOUBLEWAFERCHECH"},
            { "1060", "ERR_NOTSUPPLY_AIR"},
            { "1999", "USER_STOP_REQUEST"},
            { "2000", "ERR_RECEIVEBUF_FULL"},
            { "2001", "ERR_SENDBUF_FULL"},
        }; 

        //RS232 관련 변수
        ezRS232 m_rs232;            // RND Baudrate 57600, Data 8, Stop bit 1, Parity None
        CallRS232 m_cbRS232 = null;
        string m_sLastCmd = "";
        int m_nMappingLP = -1; 

        char[] m_aBuf = new char[BUF_SIZE];     //RS232 Recieve Data Buffer
        //InOutput
        int[] m_diCheckVac = new int[2] { -1, -1 };         //WTR Arm Wafer Check Sensor {Lower Arm, Upper Arm}
        int[] m_diCheckGrip = new int[2] { -1, -1 };
        int[] m_diArmClose = new int[2] { -1, -1 };      //WTR Arm Close Check Sensor {Lower Arm, Upper Arm}

        //Delay 및 설정 변수
        int m_msMotion = 20000;
        int m_msRS232 = 2000;
        int m_msArmClose = 3000;

        //상태 변수
        #endregion

        //IO 설정 변수
        bool m_bSwitch = false;
        string[] m_strArmVac = new string[2] { "1-0", "1-2" };
        string[] m_strArmGrip = new string[2] { "1-1", "1-3" };

        #region Init
        public override void Init(string id, Auto_Mom auto)
        {
            int nCount = 0;
            bool bConnect = false;
            base.Init(id, auto);
            m_control.Add(this);

            m_rs232 = new ezRS232(m_id + "_RS232", m_log);
            m_rs232.CallMsgRcv += m_rs232_CallMsgRcv;
            RunGrid(eGrid.eRegRead);
            RunGrid(eGrid.eInit);
            InitString();
            while (!bConnect && nCount < 5)
            {
                bConnect = m_rs232.Connect(true);
                Thread.Sleep(500);
                nCount++;
            }
            if (!bConnect)
            {
                SetAlarm(eAlarm.Stop, (int)eError.Connect); 
            }
        }

        public override void ThreadStop()
        {
            base.ThreadStop();
            m_rs232.ThreadStop();
        }

        void InitString()
        {
            InitString((int)eError.Connect, "WTR Not Connect !!");
            InitString((int)eError.Timeout, "WTR Operation TimeOver !!");
            InitString((int)eError.Protocol, "WTR RS232 Cmd Protocol Error !!");
            InitString((int)eError.WaferCheckErrorDuringMove, "Wafer 이송 도중 WTR의 Wafer감지에 이상이 감지되었습니다. 장비 내부의 Wafer 상태를 확인해주세요.");
            InitString((int)eError.NAK, "WTR RS232 Return NAK !!");
            InitString((int)eError.ShiftLimit, "WTR Shift Limit Overshoot Error !!");
            AddErrorMsg();
        } 

        void AddErrorMsg()
        {
            for (int n = 0; n < m_sErrorMsgs.GetLength(0); n++)
            {

                InitString(n + Enum.GetValues(typeof(eError)).Length + Enum.GetValues(typeof(eError_Mom)).Length, m_sErrorMsgs[n, 1]);
            }
        } 
         
        void InitString(int eErr, string str) 
        {
            m_log.AddString(str);
            if (m_xGem == null) return;
            m_xGem.AddALID(m_id, (int)eErr, str);
        }

        int GetErrorIndex(string sErrCode)
        {
            int n = 0;
            for (n = 0; n < c_nERR; n++)
            {
                if (sErrCode == m_sErrorMsgs[n, 0]) break;
            } 
            return n + Enum.GetValues(typeof(eError)).Length + (int)eError_Mom.End;
        } 

        void SetAlarm(eAlarm alarm, int eErr) 
        {
            SetState(eState.Error);
            m_work.SetError(alarm, m_log, (int)eErr);
            if (m_xGem == null) return;
            m_xGem.SetAlarm(m_id, (int)eErr);
        } 
         
        public void ControlGrid(Control_Mom control, ezGrid rGrid, eGrid eMode)
        {
            control.AddDI(rGrid, ref m_diCheckVac[(int)eArm.Lower], m_id, "LVacCheck", "Wafer Check On Lower Arm");
            control.AddDI(rGrid, ref m_diCheckVac[(int)eArm.Upper], m_id, "UVacCheck", "wafer Check On Upper Arm");
            control.AddDI(rGrid, ref m_diCheckGrip[(int)eArm.Lower], m_id, "LGripCheck", "RingFrame Check On Lower Arm");
            control.AddDI(rGrid, ref m_diCheckGrip[(int)eArm.Upper], m_id, "UGripCheck", "RingFrame Check On Upper Arm");
            control.AddDI(rGrid, ref m_diArmClose[(int)eArm.Lower], m_id, "LArmClose", "Lower Arm Close Signal");
            control.AddDI(rGrid, ref m_diArmClose[(int)eArm.Upper], m_id, "UArmClose", "Upper Arm Close Signal");
        }

        protected override void RunGrid(eGrid eMode)
        {
            int nLP, nSize;
            string strSize, strLP;
            string strDesc = "Teaching Point (index)";
            base.RunGrid(eMode);
            m_rs232.RunGrid(m_grid, eMode);
            m_grid.Set(ref m_msHome, "Delay", "Home", "Home Timeout (ms)");
            m_grid.Set(ref m_msMotion, "Delay", "Motion", "Get/Put Motion Timeout(ms)");
            m_grid.Set(ref m_msRS232, "Delay", "CommDelay", "RS232 Communication Delay(ms)");
            m_grid.Set(ref m_msWaitWTRShift, "Delay", "Wait WTR Shift", "Wait WTR Shift Timeout (ms)"); // BHJ 190304 add
            m_grid.Set(ref m_dShiftLimit, "WTR Shift", "Shift Limit", "WTR Offset Limit Setting. Default = 10mm");
            for (nSize = 0; nSize < (int)(Wafer_Size.eSize.Empty); nSize++)
            {
                strSize = ((Wafer_Size.eSize)nSize).ToString();
                for (nLP = 0; nLP < m_auto.ClassHandler().m_nLoadPort; nLP++)
                {
                    strLP = nLP.ToString("Teach_LoadPort0");
                    m_grid.Set(ref m_nTeaching[nSize, nLP], strLP, strSize, strDesc, false, m_infoCarrier[nLP].m_wafer.m_bEnable[nSize]);
                }
                m_grid.Set(ref m_nTeaching[nSize, (int)eTeaching.Aligner], "Teach_Aligner", strSize, strDesc, false, m_aligner.m_wafer.m_bEnable[nSize]);
                m_grid.Set(ref m_nTeaching[nSize, 
                    (int)eTeaching.Vision], "Teach_Vision", strSize, strDesc, false, m_aligner.m_wafer.m_bEnable[nSize]);
                m_grid.Set(ref m_nTeaching[nSize, (int)eTeaching.BackSide], "Teach_Backside", strSize, strDesc, false, m_backSide.m_wafer.m_bEnable[nSize]);
            }
            m_grid.Set(ref m_bSwitch, "IO", "Switch", "True : On(+), Off(-) , False : On(-), Off(+)");
            m_grid.Set(ref m_strArmVac[0], "IO", "LowerVac", "Lower Arm Vacuum Output");
            m_grid.Set(ref m_strArmGrip[0], "IO", "LowerGrip", "Lower Arm Gripper Output");
            m_grid.Set(ref m_strArmVac[1], "IO", "UpperVac", "Upper Arm Vacuum Output");
            m_grid.Set(ref m_strArmGrip[1], "IO", "UpperGrip", "Upper Arm Gripper Output");
        }
        #endregion

        #region RS232
        string sTempMGS = "";
        void m_rs232_CallMsgRcv()
        {
            Thread.Sleep(200);
            int nRead;
            nRead = m_rs232.Read(m_aBuf, BUF_SIZE);
            m_aBuf[nRead] = (char)0x00;
            string sMsg = new string(m_aBuf, 0, nRead);
            if (sMsg.IndexOf("\n") < 0) {
                sTempMGS = sMsg;
                return;
            }
            else if (sTempMGS != "") {
                sMsg = sTempMGS + sMsg;
                sTempMGS = "";
            }
            sMsg = sMsg.Replace("\r\n", "");
            string[] sMsgs = sMsg.Split(SPLITTER);
            if (sMsgs[0][0] == ACK) m_log.Add("Receive ACK");
            else if (sMsgs[0] == "ERR") m_log.Add(GetErrorString(sMsgs[1]));
            else m_log.Add("CMD Recieve : " + sMsg);

            if (m_cbRS232 != null)
            {
                m_cbRS232(sMsgs);
                return;
            }
        }

        void WriteCmd(string str)
        {
            m_sLastCmd = str.Split(SPLITTER)[0];
            if (str.IndexOf(" ") < 0) {
                CallDelegateFunction(str);
            }
            else {
                CallDelegateFunction(str.Substring(0, str.IndexOf(" ")));
            }
            str = str + (char)0x0D;
            char[] sChar = str.ToCharArray();
            m_rs232.Write(sChar, str.Length, true);
            m_log.Add("CMD Send : " + m_sLastCmd);
        }

        void WriteACK()
        {
            m_rs232.Write(ACK);
            m_log.Add("Send Ack");
        }

        void CallDelegateFunction(string sCMD)              // WriteCMD 시 m_cbRS232 인자로는 왼쪽과 같이 넣어준다.  * CheckACKAndCMD  (PC)CMD->(Module)ACK->동작->(Module)CMD->(PC)ACK 의 형태
        {                                                                      //                                                       * CheckCMD    (PC)CMD ->동작->(Module)CMD 의 형태      
            switch (sCMD)                                                      //                                                       * CheckACK (PC)CMD->동작->Module(ACK) 의 형태          
            {
                case CMD_MAPREAD:
                    m_cbRS232 = CheckMapping;
                    break;
                default:
                    m_cbRS232 = CheckCMD;
                    break;
            }
        }

        void CheckCMD(string[] sMsgs)
        {
            try {
                if (sMsgs.Length > 1) {
                    string str = GetErrorString(sMsgs[1]);
                    m_log.Popup(str);
                    SetState(eState.Error);
                    m_cbRS232 = null;
                    m_bNeedInit = true;
                    return;
                }
                else if (sMsgs[0] == m_sLastCmd && sMsgs.Length == 1)
                {
                    m_log.Add("Received Successfully : " + m_sLastCmd);
                    m_cbRS232 = null;
                }
                else
                {
                    m_log.Popup("Cannot Recieve Status Command : " + m_sLastCmd);
                    SetState(eState.Error);
                }
            }
            catch (Exception ex)
            {
                if (sMsgs[0].Length != m_sLastCmd.Length)
                {
                    m_log.Add("Received error(" + ex + ") : " + sMsgs[0]);   //SDH
                    m_cbRS232 = null;
                }
            }
        }

        void CheckMapping(string[] sMsgs)
        {
            try
            {
                if (sMsgs[0] != m_sLastCmd)
                {
                    m_log.Add("Command Missmatch : " + m_sLastCmd);
                    SetState(eState.Error);
                }
                else if (sMsgs.Length < 2)
                {
                    m_log.Popup("Mapping Data not Found");
                    SetState(eState.Error);
                }
                m_cbRS232 = null;
                m_loadPort[m_nMappingLP].SetMappingData(sMsgs[1]);
            }
            catch (Exception ex)
            {
                if (sMsgs[0].Length != m_sLastCmd.Length)
                {
                    m_log.Add("Received error(" + ex + ") : " + sMsgs[0]);   //SDH
                    m_cbRS232 = null;
                }
            }
        }

        eHWResult WaitReply(int msDelay, string sMsg)
        {
            string[] sMsgs = sMsg.Split(' '); 
            int ms10 = 0;
            while (m_cbRS232 != null)
            {
                Thread.Sleep(10);
                ms10 += 10;
                if (ms10 > msDelay)
                {
                    switch (m_sLastCmd)
                    {
                        case CMD_HOME:
                            m_xGem.SetMSBAlarm(XGem300_Mom.eMSBAlarm.WTR_Home_Error);
                            break;
                        case CMD_GET_EXTEND:
                        case CMD_GET_RETRACT:
                        case CMD_GET_WAFER:
                            m_xGem.SetMSBAlarm(XGem300_Mom.eMSBAlarm.WTR_GetWafer_Error);
                            break;
                        case CMD_PUT_EXTEND:
                        case CMD_PUT_RETRACT:
                        case CMD_PUT_WAFER:
                            m_xGem.SetMSBAlarm(XGem300_Mom.eMSBAlarm.WTR_PutWafer_Error);
                            break;
                        default:
                            m_xGem.SetMSBAlarm(XGem300_Mom.eMSBAlarm.WTR_Comm_Error);
                            break;
                    }
                    SetAlarm(eAlarm.Warning, (int)eError.Timeout); 
                    m_log.Popup(m_sLastCmd + " Timeout !!");
                    m_cbRS232 = null;
                    return eHWResult.Error;
                }
            }
            return eHWResult.OK;
        }
        #endregion

        
        #region Process
        protected override void ProcError()
        {
            WriteCmd(CMD_RESET_CPU);

            if (WaitReply(m_msHome, "Can not Recieve ACK From WTR") == eHWResult.Error)
            {
                SetState(eState.Init);
                return;
            }
            SetAlarm(eAlarm.Warning,(int) eError.Protocol); 
            SetState(eState.Init);
        }

        string GetErrorString(string sCode)
        {
            int n;
            for (n = 0; n < c_nERR; n++)
            {
                if (m_sErrorMsgs[n, 0] == sCode)
                {
                    SetAlarm(eAlarm.Warning, GetErrorIndex(sCode));
                    return m_sErrorMsgs[n, 1];
                }
            }
            return "Can't Find Error Massage !!";
        } 

        #endregion

        #region RunMethod

        protected override eHWResult RunHome()
        {
            m_log.WriteLog("Sequence", "[" + m_id + "Start]" + " Home");
            WriteCmd(CMD_RESET_CPU);
            if (WaitReply(m_msRS232, "Error Reset TimeOut") == eHWResult.Error) {
                WriteCmd(CMD_RESET_CPU);
                if (WaitReply(m_msRS232, "Error Reset TimeOut") == eHWResult.Error)
                {
                    m_log.WriteLog("Sequence", "[" + m_id + "Done]" + " Home FAIL");
                    return eHWResult.Error;
                }
            }

            //if (m_bFirstHome || m_bNeedInit)
            //{
                WriteCmd(CMD_INIT);
            //    m_bFirstHome = false;
            //    m_bNeedInit = false;
            //}
            //else if (!m_bFirstHome)
            //{
            //    WriteCmd(CMD_HOME);
           // }
            return WaitReply(m_msHome, "Home Motion Timeout");
        }

        protected override eHWResult AfterHome()
        {
            eHWResult eError;
            eError = RunGrip(eArm.Upper, false);
            eError = RunGrip(eArm.Lower, false);
            eError = RunGrip(eArm.Upper, true);
            eError = RunGrip(eArm.Lower, true);
            return eError;
        }

        protected override void RunVac(eArm nArm, bool bVac)
        {
            string sCmd, sArm, sParam;
            if (nArm == eArm.Lower)
                sArm = m_strArmVac[0];
            else
                sArm = m_strArmVac[1];
            if (bVac)
            {
                if (m_bSwitch) sParam = ":-";
                else sParam = ":+";
            }
            else
            {
                if (m_bSwitch) sParam = ":+";
                else sParam = ":-";
            }
            sCmd = CMD_WRITE_OUTPUT + SPLITTER + sArm + sParam;
            WriteCmd(sCmd);
            WaitReply(m_msRS232, "Vac On Action Time Out");
            }

        protected override eHWResult RunGrip(eArm nArm, bool bFlag)
        {
            string sCmd, sArm, sParam;
            if (nArm == eArm.Lower)
                sArm = m_strArmGrip[0];
            else
                sArm = m_strArmGrip[1];
            if (bFlag)
            {
                if (m_bSwitch) sParam = ":-";
                else sParam = ":+";
            }
            else
            {
                if (m_bSwitch) sParam = ":+";
                else sParam = ":-";
            }
            eHWResult eError;
            sCmd = CMD_WRITE_OUTPUT + SPLITTER + sArm + sParam;
            WriteCmd(sCmd);
            eError = WaitReply(m_msMotion, "Get Motion Time Out");
            if (eError == eHWResult.Error) return eHWResult.Error;
            return eHWResult.OK;
        }

        protected override eHWResult RunGetMotion(eArm nArm, int nPos, int nSlot, double mmShift = 0)
        {
            eHWResult eError;

            if (CheckWaferExist(nArm) != eHWResult.Off)
            {
                SetAlarm(eAlarm.Error, (int)HW_WTR_RND.eError.WaferCheckErrorDuringMove); 
                return eHWResult.Error;
            }
            if (m_dShiftLimit < Math.Abs(mmShift))
            {
                SetAlarm(eAlarm.Error, (int)HW_WTR_RND.eError.ShiftLimit); 
                return eHWResult.Error;
            }
            else
            {
                string sCmd;
                if (mmShift == 0)
                    sCmd = CMD_GET_WAFER + SPLITTER + nPos.ToString() + "," + (nSlot + 1).ToString() + "," + Convert.ToString((int)nArm + 1);
                else
                    sCmd = CMD_GET_WAFER + SPLITTER + nPos.ToString() + "," + (nSlot + 1).ToString() + "," + Convert.ToString((int)nArm + 1) + "," + mmShift.ToString("0.00");
                WriteCmd(sCmd);
                eError = WaitReply(m_msMotion, "Get Motion Time Out");
                if (CheckWaferExist(nArm) != eHWResult.On) { m_log.Popup("Wafer Check Error"); return eHWResult.Error; } //ing 161018
                if (eError == eHWResult.Error) return eHWResult.Error;

                return eHWResult.OK;
            }
        }

        protected override eHWResult RunPutMotion(eArm nArm, int nPos, int nSlot, double mmShift = 0)
        {
            eHWResult eError;

            if (CheckWaferExist(nArm) != eHWResult.On)
            {
                m_log.Popup("Wafer Does not Exist On the Arm While WTR Put Motion" + "Arm : " + nArm.ToString() + "Pos : " + nPos.ToString() + "Slot : " + nSlot + ToString());
                SetState(eState.Error);
                return eHWResult.Error;
            }
            if (m_dShiftLimit < Math.Abs(mmShift))
            {
                SetAlarm(eAlarm.Error, (int)HW_WTR_RND.eError.ShiftLimit); 
                return eHWResult.Error;
            }
            else
            {
                string sCmd;
                if (mmShift == 0)
                    sCmd = CMD_PUT_WAFER + SPLITTER + nPos.ToString() + "," + (nSlot + 1).ToString() + "," + Convert.ToString((int)nArm + 1);
                else
                    sCmd = CMD_PUT_WAFER + SPLITTER + nPos.ToString() + "," + (nSlot + 1).ToString() + "," + Convert.ToString((int)nArm + 1) + "," + mmShift.ToString("0.00");
                WriteCmd(sCmd);
                eError = WaitReply(m_msMotion, "Put Motion Time Out");
                if (CheckWaferExist(nArm) != eHWResult.Off) { m_log.Popup("Wafer Check Error"); return eHWResult.Error; } //ing 161018
                if (eError == eHWResult.Error) return eHWResult.Error;
                return eHWResult.OK;
            }
        }

        protected override eHWResult RunGetLPSLimSlot(HW_WTR_Mom.eArm nArm, int nPos, int nSlot)
        {
            //if (m_handler.ClassLoadPort(0).RunLiftUp() != eHWResult.OK)
            //{
            //    m_log.Popup("LP LIfT UP Error In SlimSlot" + "Arm : " + nArm.ToString() + "Pos : " + nPos.ToString() + "Slot : " + nSlot + ToString());
            //    SetState(eState.Error);
            //    return eHWResult.Error;
            //}
            if (RunGetReady(nArm, nPos, nSlot) != eHWResult.OK) {
                m_log.Popup("GET Ready Motion Error" + "Arm : " + nArm.ToString() + "Pos : " + nPos.ToString() + "Slot : " + nSlot + ToString());
                SetState(eState.Error);
                return eHWResult.Error;
            }
            if (RunGetExtendMotion(nArm, nPos, nSlot) != eHWResult.OK) {
                m_log.Popup("GET Extend Motion Error" + "Arm : " + nArm.ToString() + "Pos : " + nPos.ToString() + "Slot : " + nSlot + ToString());
                SetState(eState.Error);
                return eHWResult.Error;
            }
            if (m_handler.ClassLoadPort(0).RunLiftDown() != eHWResult.OK) {
                m_log.Popup("LP LIft Down Error In SlimSlot" + "Arm : " + nArm.ToString() + "Pos : " + nPos.ToString() + "Slot : " + nSlot + ToString());
                SetState(eState.Error);
                return eHWResult.Error;
            }
            if (RunGetRetractMotion(nArm, nPos, nSlot) != eHWResult.OK) {
                m_log.Popup("Get Retract Motion Error" + "Arm : " + nArm.ToString() + "Pos : " + nPos.ToString() + "Slot : " + nSlot + ToString());
                SetState(eState.Error);
                return eHWResult.Error;
            }
            return eHWResult.OK;
        }

        protected override eHWResult RunPutLPSLimSlot(HW_WTR_Mom.eArm nArm, int nPos, int nSlot)
        {
            //if (m_handler.ClassLoadPort(0).RunLiftDown() != eHWResult.OK)
            //{
            //    m_log.Popup("LP LIfT down Error In SlimSlot" + "Arm : " + nArm.ToString() + "Pos : " + nPos.ToString() + "Slot : " + nSlot + ToString());
            //    SetState(eState.Error);
            //    return eHWResult.Error;
            //}

            if (RunPutReady(nArm, nPos, nSlot) != eHWResult.OK) {
                m_log.Popup("PUT Extend Motion Error" + "Arm : " + nArm.ToString() + "Pos : " + nPos.ToString() + "Slot : " + nSlot + ToString());
                SetState(eState.Error);
                return eHWResult.Error;
            }
            if (RunPutExtendMotion(nArm, nPos, nSlot) != eHWResult.OK) {
                m_log.Popup("PUT Extend Motion Error" + "Arm : " + nArm.ToString() + "Pos : " + nPos.ToString() + "Slot : " + nSlot + ToString());
                SetState(eState.Error);
                return eHWResult.Error;
            }
            if (m_handler.ClassLoadPort(0).RunLiftUp() != eHWResult.OK) {
                m_log.Popup("LP LIft up Error In SlimSlot" + "Arm : " + nArm.ToString() + "Pos : " + nPos.ToString() + "Slot : " + nSlot + ToString());
                SetState(eState.Error);
                return eHWResult.Error;
            }
            if (RunPutRetractMotion(nArm, nPos, nSlot) != eHWResult.OK) {
                m_log.Popup("PUT Retract Motion Error" + "Arm : " + nArm.ToString() + "Pos : " + nPos.ToString() + "Slot : " + nSlot + ToString());
                SetState(eState.Error);
                return eHWResult.Error;
            }
            return eHWResult.OK;
        }

        protected override eHWResult RunGetReady(eArm nArm, int nPos, int nSlot)
        {
            eHWResult eError;

            if (CheckWaferExist(nArm) != eHWResult.Off) {
                m_log.Popup("Wafer Exist On the Arm While WTR Get Motion" + "Arm : " + nArm.ToString() + "Pos : " + nPos.ToString() + "Slot : " + nSlot + ToString());
                SetState(eState.Error);
                return eHWResult.Error;
            }
            else {
                string sCmd = CMD_GETREADY + SPLITTER + nPos.ToString() + "," + (nSlot + 1).ToString() + "," + Convert.ToString((int)nArm + 1);
                WriteCmd(sCmd);
                eError = WaitReply(m_msMotion, "Get Ready Time Out");
                if (CheckWaferExist(nArm) != eHWResult.Off) { m_log.Popup("Wafer Check Error"); return eHWResult.Error; } //ing 161018
                if (eError == eHWResult.Error) return eHWResult.Error;
                return eHWResult.OK;
            }
        }

        protected override eHWResult RunGetExtendMotion(eArm nArm, int nPos, int nSlot)
        {
            eHWResult eError;

            if (CheckWaferExist(nArm) != eHWResult.Off)
            {
                m_log.Popup("Wafer Exist On the Arm While WTR Get Motion" + "Arm : " + nArm.ToString() + "Pos : " + nPos.ToString() + "Slot : " + nSlot + ToString());
                SetState(eState.Error);
                return eHWResult.Error;
            }
            else
            {
                string sCmd = CMD_GET_EXTEND + SPLITTER + nPos.ToString() + "," + (nSlot + 1).ToString() + "," + Convert.ToString((int)nArm + 1);
                WriteCmd(sCmd);
                eError = WaitReply(m_msMotion, "Get Motion Time Out");
                if (CheckWaferExist(nArm) != eHWResult.Off) { m_log.Popup("Wafer Check Error"); return eHWResult.Error; } //ing 161018
                if (eError == eHWResult.Error) return eHWResult.Error;
                return eHWResult.OK;
            }
        }

        protected override eHWResult RunGetRetractMotion(eArm nArm, int nPos, int nSlot)
        {
            eHWResult eError;

            if (CheckWaferExist(nArm) != eHWResult.Off) {
                m_log.Popup("Wafer Exist On the Arm While WTR Get Motion" + "Arm : " + nArm.ToString() + "Pos : " + nPos.ToString() + "Slot : " + nSlot + ToString());
                SetState(eState.Error);
                return eHWResult.Error;
            }
            else {
                string sCmd = CMD_GET_RETRACT + SPLITTER + nPos.ToString() + "," + (nSlot + 1).ToString() + "," + Convert.ToString((int)nArm + 1);
                WriteCmd(sCmd);
                eError = WaitReply(m_msMotion, "Get Motion Time Out");
                if (CheckWaferExist(nArm) != eHWResult.Off) { m_log.Popup("Wafer Check Error"); return eHWResult.Error; } //ing 161018
                if (eError == eHWResult.Error) return eHWResult.Error;
                return eHWResult.OK;
            }
        }

        protected override eHWResult RunPutReady(eArm nArm, int nPos, int nSlot)
        {
            eHWResult eError;

            if (CheckWaferExist(nArm) != eHWResult.On) {
                m_log.Popup("Wafer Does not Exist On the Arm While WTR Put Motion" + "Arm : " + nArm.ToString() + "Pos : " + nPos.ToString() + "Slot : " + nSlot + ToString());
                SetState(eState.Error);
                return eHWResult.Error;
            }
            else {
                string sCmd = CMD_PUTREADY + SPLITTER + nPos.ToString() + "," + (nSlot + 1).ToString() + "," + Convert.ToString((int)nArm + 1);
                WriteCmd(sCmd);
                eError = WaitReply(m_msMotion, "Put Ready Pos Time Out");
                if (CheckWaferExist(nArm) != eHWResult.Off) { m_log.Popup("Wafer Check Error"); return eHWResult.Error; } //ing 161018
                if (eError == eHWResult.Error) return eHWResult.Error;
                return eHWResult.OK;
            }
        }
        
        protected override eHWResult RunPutExtendMotion(eArm nArm, int nPos, int nSlot)
        {
            eHWResult eError;

            if (CheckWaferExist(nArm) != eHWResult.On)
            {
                m_log.Popup("Wafer Does not Exist On the Arm While WTR Put Motion" + "Arm : " + nArm.ToString() + "Pos : " + nPos.ToString() + "Slot : " + nSlot + ToString());
                SetState(eState.Error);
                return eHWResult.Error;
            }
            else
            {
                string sCmd = CMD_PUT_EXTEND + SPLITTER + nPos.ToString() + "," + (nSlot + 1).ToString() + "," + Convert.ToString((int)nArm + 1);
                WriteCmd(sCmd);
                eError = WaitReply(m_msMotion, "Put Motion Time Out");
                if (CheckWaferExist(nArm) != eHWResult.Off) { m_log.Popup("Wafer Check Error"); return eHWResult.Error; } //ing 161018
                if (eError == eHWResult.Error) return eHWResult.Error;
                return eHWResult.OK;
            }
        }
        
        protected override eHWResult RunPutRetractMotion(eArm nArm, int nPos, int nSlot)
        {
            eHWResult eError;

            if (CheckWaferExist(nArm) != eHWResult.On)
            {
                m_log.Popup("Wafer Does not Exist On the Arm While WTR Put Motion" + "Arm : " + nArm.ToString() + "Pos : " + nPos.ToString() + "Slot : " + nSlot + ToString());
                SetState(eState.Error);
                return eHWResult.Error;
            }
            else
            {
                string sCmd = CMD_PUT_RETRACT + SPLITTER + nPos.ToString() + "," + (nSlot + 1).ToString() + "," + Convert.ToString((int)nArm + 1);
                WriteCmd(sCmd);
                eError = WaitReply(m_msMotion, "Put Motion Time Out");
                if (CheckWaferExist(nArm) != eHWResult.Off) { m_log.Popup("Wafer Check Error"); return eHWResult.Error; } //ing 161018
                if (eError == eHWResult.Error) return eHWResult.Error;
                return eHWResult.OK;
            }
        }

        protected override void ErrorReset()
        {
            WriteCmd(CMD_RESET_CPU);
            WaitReply(m_msRS232, "Communication Error Command : Error Reset");
        }

        public override eHWResult RunGoHome() // BHJ 190304 add
        {
            WriteCmd(CMD_RESET_CPU);
            if (WaitReply(m_msRS232, "Error Reset TimeOut") == eHWResult.Error)
            {
                WriteCmd(CMD_RESET_CPU);
                if (WaitReply(m_msRS232, "Error Reset TimeOut") == eHWResult.Error)
                    return eHWResult.Error;
            }

            WriteCmd(CMD_HOME);
            return WaitReply(m_msHome, "GoHome Motion Timeout");
        }
        #endregion

        
        //KDG 161007 아래 함수와 같은 내용이라 삭제
        /*
        public override eHWResult IsWaferExist(HW_WTR_Mom.eArm nArm)
        {
            bool bExist_Wafer = m_control.GetInputBit(m_diCheckVac[(int)nArm]);
            bool bExist_Ring = m_control.GetInputBit(m_diCheckGrip[(int)nArm]);
            if (bExist_Wafer || bExist_Ring) m_waferExist[(int)nArm] = eHWResult.On;
            else m_waferExist[(int)nArm] = eHWResult.Off;
            return m_waferExist[(int)nArm];
        }
        */ 

        public override eHWResult CheckWaferExist(eArm nArm)
        {
            bool bExist_Wafer = m_control.GetInputBit(m_diCheckVac[(int)nArm]);
            bool bExist_Ring = m_control.GetInputBit(m_diCheckGrip[(int)nArm]);
            if (bExist_Wafer || bExist_Ring) m_waferExist[(int)nArm] = eHWResult.On;
            else m_waferExist[(int)nArm] = eHWResult.Off;
            return m_waferExist[(int)nArm];
        }

        public override bool IsArmClose(eArm nArm, bool bWait) // ing 161013
        {
            if (!bWait) return m_control.GetInputBit(m_diArmClose[(int)nArm]); 
            else return !m_control.WaitInputBit(m_diArmClose[(int)nArm], true, m_msArmClose);
        }

        protected override eHWResult RunWTRMapping()
        {
            for (int n = 0; n < m_nLoadPort; n++)
            {
                if (m_loadPort[n].NeedWTRMapping())
                {
                    if (!m_loadPort[n].IsDoorOpenPos() || !m_loadPort[n].IsDocking()) return eHWResult.Error; // ing 170921
                    Info_Carrier infoCarrier = m_loadPort[n].m_infoCarrier;
                    int nPos = m_nTeaching[(int)infoCarrier.m_wafer.m_eSize, (int)eTeaching.LoadPort0 + n];
                    WriteCmd(CMD_MAPPING + " " + nPos.ToString());
                    WaitReply(m_msRS232, "Communication Error Command : Mapping");
                    m_nMappingLP = n; 
                    WriteCmd(CMD_MAPREAD);
                    WaitReply(m_msRS232, "Communication Error Command : Map Data Read");
                }
            }
            return eHWResult.OK;

        }

        protected override eHWResult RunWTRMapping(int nLP) // ing 20171029 WTR Mapping 시컨스 변경
        {
            if (m_loadPort[nLP].NeedWTRMapping())
            {
                if (!m_loadPort[nLP].IsDoorOpenPos() || !m_loadPort[nLP].IsDocking()) return eHWResult.Error; // ing 170921
                Info_Carrier infoCarrier = m_loadPort[nLP].m_infoCarrier;
                int nPos = m_nTeaching[(int)infoCarrier.m_wafer.m_eSize, (int)eTeaching.LoadPort0 + nLP];
                WriteCmd(CMD_MAPPING + " " + nPos.ToString());
                WaitReply(m_msRS232, "Communication Error Command : Mapping");
                m_nMappingLP = nLP;
                WriteCmd(CMD_MAPREAD);
                WaitReply(m_msRS232, "Communication Error Command : Map Data Read");
            }
            return eHWResult.OK;

        }
    }
}
