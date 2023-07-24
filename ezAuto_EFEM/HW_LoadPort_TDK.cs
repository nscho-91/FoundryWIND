using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ezAutoMom;
using ezTools;
using System.Windows.Forms;

namespace ezAuto_EFEM
{
    class HW_LoadPort_TDK : HW_LoadPort_Mom, Control_Child
    {
        #region enum
        enum eError
        {
            Connect,
            Timeout,
            Protocol,
            Checksum,
            Combusy,
            Fouploading,
            Latchkey,
            Foupclamp,
            DockPlate,
            DoorOpenPos,
            WaferProtrusion,
            Mappingbar,
            DoorVac,
            NotInitial,
            Busy,
            PosZLimit,
            PosYLimit,
            MappingBarPos,
            MappingBarPosZ,
            MappingBarPosStop,
            FoupClampOpen,
            FoupClampClose,
            LatchkeyOpen,
            LatchkeyClose,
            VacOn,
            VacOff,
            MainAir,
            FoupOpenPos,
            FoupClosePos,
            Unknown,
            NAK
        }
        #endregion
        
        //command
        #region Define
        const string CMD_HOME = "MOV:ABORG";     // Home
        const string CMD_RESET_CPU = "SET:RESET";         // error시 명령어가 듣지 않음. 리셋.
        const string CMD_LOAD = "MOV:CLDMP";
        const string CMD_UNLOAD = "MOV:CULOD";
        const string CMD_STOP = "MOV_STOP_";
        const string CMD_GETMAPDATA = "GET:MAPDT";

        const char SPLITFLAG = ':';
        const char ENDFLAG = ';';
        const char PARAMFLAG = '/';

        //Error cmd
        const string ERR_CHECKSUM = "CKSUM";
        const string ERR_COMMAND = "CMDER";
        const string ERR_COMBUSY = "CBUSY";
        const string ERR_FOUPLOADING = "FPILG";
        const string ERR_LATCHKEY = "LATCH";
        const string ERR_FOUPCLAMP = "FPCLP";
        const string ERR_DOCKPLATEPOS = "YPOSI";
        const string ERR_DOOROPENCLOSEPOS = "DOCPO";
        const string ERR_DOOROPENCLOSEPOS2 = "DPOSI";
        const string ERR_WAFERPROTRUSION = "PROTS";
        const string ERR_MAPINGARTSTARTPOS = "MPARM";
        const string ERR_DOORVAC = "DVACM";
        const string ERR_NOTINITIAL = "ORGYT";
        const string ERR_LOADNOTCOMPLETE = "CLDDK";
        const string ERR_UNLOADNOTCIMPLETE = "CULDK";
        const string ERR_ZAXISLIMIT = "ZLMIT";
        const string ERR_YAXISLIMIT = "YLMIT";
        const string ERR_DOORAXISPOS = "DLMIT";
        const string ERR_MAPPINGARMPOS = "MPBAR";
        const string ERR_MAPPINGARMPOSZ = "MPZLM";
        const string ERR_MAPPINGBARSTOPPER = "MPSTP";
        const string ERR_MAPIINGBARPOSSTOP = "MPEDL";
        const string ERR_FOUPCLAMPOPEN = "CLOPS";
        const string ERR_FOUPCLAMCLOSE = "CLCLS";
        const string ERR_LATCHKEYOPEN = "DROPS";
        const string ERR_LATCHKEYCLOSE = "DRCLS";
        const string ERR_VACUUMON = "VACCS";
        const string ERR_VACUUMOFF = "VACOS";
        const string ERR_MAINAIR = "AIRSN";
        const string ERR_FOUPOPENPOS = "INTOP";
        const string ERR_FOUPCLOSEPOS = "INTCL";
        const string ERR_INTERLOCK = "INTER";
     
        const int BUF_SIZE = 4096;
        #endregion

        #region Declaration
        ezRS232 m_rs232;
        byte[] m_aBuf = new byte[BUF_SIZE];
        string m_sLastCmd = "";
        bool m_bSendCMD= false;
        int m_nLPNum = 0;

        int m_diPresent = -1;
        int m_diPlacement = -1;
        int m_diSwitch = -1;
        int m_diOpen = -1;
        int m_diUnloadSwitch = -1;

        new int m_msHome = 20000;
        int m_msMotion = 20000;
        int m_msRS232 = 20000;
        #endregion

        //BaudRate:9600 
        //DatatBit:8
        //StopBit:1
        //Parity & FlowControl: None
        #region Init
        public override void Init(int nID, Auto_Mom auto)
        {
            int nMode = 0, nType = 0;
            base.Init(nID, auto);
            m_nLPNum = nID;
            m_control.Add(this);
            m_log.m_reg.Read("Mode", ref nMode);
            m_log.m_reg.Read("TypeSlot", ref nType); // ing 171106
            m_infoCarrier.m_eSlotType = (Info_Carrier.eSlotType)nType;  // ing 171106
            m_infoCarrier.Init(m_id + "InfoCarrier", m_log, Wafer_Size.eSize.mm300);

            m_rs232 = new ezRS232(m_id + "_RS232", m_log);
            m_rs232.CallMsgRcv += m_rs232_CallMsgRcv;

            RunGrid(eGrid.eRegRead);
            RunGrid(eGrid.eInit);
            InitString();
            if (!m_rs232.Connect(true))
            {
                SetAlarm(eAlarm.Stop, eError.Connect);
            }
        }

        public override void ThreadStop()
        {
            base.ThreadStop();
            m_rs232.ThreadStop();
        }

        void InitString()
        {
            InitString(eError.Connect, "Not Connect !!");
            InitString(eError.Timeout, "Operation TimeOver !!");
            InitString(eError.Protocol, "RS232 Cmd Protocol Error !!");
            InitString(eError.Busy, "Loadport busy");
            InitString(eError.Checksum, "Checksum error");
            InitString(eError.Combusy, "communication busy. The command is not accepted");
            InitString(eError.DockPlate, "Doking plate position error");
            InitString(eError.DoorOpenPos, "Door open/close position error");
            InitString(eError.DoorVac, "Door - Being vacuumed");
            InitString(eError.Foupclamp, "FOUP CLAMP Error");
            InitString(eError.Fouploading, "FOUP loading/unloading error or no FOUP");
            InitString(eError.Latchkey, "Latchkey error. check latchey status");
            InitString(eError.Mappingbar, "Maping bar position error. please check mapping bar");
            InitString(eError.NotInitial, "Load port does not initailized");
            InitString(eError.WaferProtrusion, "Wafer protrusion detected");
            InitString(eError.PosZLimit,"Loadport Z-Axis Position Error");
            InitString(eError.PosYLimit,"Loadport Y-Axis Position Error");
            InitString(eError.MappingBarPos,"Mapping Bar Position Error");
            InitString(eError.MappingBarPosZ,"Mapping Bar Z-Axis Position Error");
            InitString(eError.MappingBarPosStop,"Mapping Bar Sopt Position Error");
            InitString(eError.FoupClampOpen,"Foup Clamp Open Position Error");
            InitString(eError.FoupClampClose,"Foup Clamp Close Position Error");
            InitString(eError.LatchkeyOpen,"Latchkey Open Error");
            InitString(eError.LatchkeyClose,"Latchkey Close Error");
            InitString(eError.VacOn,"The Front of the FOUP is not contacting the Z-Axis");
            InitString(eError.VacOff,"Vacuum Sensor Failure");
            InitString(eError.MainAir,"Loadport Main Air too Low");
            InitString(eError.FoupOpenPos,"Foup Open Position Error");
            InitString(eError.FoupClosePos,"Foup Close Position Error");
            InitString(eError.Unknown, "Can not Find Error MSG.");
            InitString(eError.NAK, "RS232 Return NAK !!");
        } 

        void InitString(eError eErr, string str)
        {
            m_log.AddString(str);
            if (m_xGem == null) return;
            m_xGem.AddALID(m_id, (int)eErr, str);
        } 

        void SetAlarm(eAlarm alarm, eError eErr)
        {
            SetState(eState.Error);
            m_work.SetError(alarm, m_log, (int)eErr);
            if (m_xGem == null) return;
            m_xGem.SetAlarm(m_id, (int)eErr);
        } 

        public void ControlGrid(Control_Mom control, ezGrid rGrid, eGrid eMode)
        {
            //forgetpublic void ControlGrid(Control_Mom control, ezGrid rGrid, eGrid eMode)
            control.AddDI(rGrid, ref m_diPlacement, m_id, "Present", "Present Check Sensor");
            control.AddDI(rGrid, ref m_diPresent, m_id, "Placment", "Placement Check Sensor");
            control.AddDI(rGrid, ref m_diOpen, m_id, "Open", "Open Check Sensor");
            control.AddDI(rGrid, ref m_diSwitch, m_id, "Switch", "Loadport Switch");
            control.AddDI(rGrid, ref m_diUnloadSwitch, m_id, "UnloadSwitch", "Unload Switch");
        }

        public override void RunGrid(ezTools.eGrid eMode)
        {
            m_grid.Update(eMode);
            m_rs232.RunGrid(m_grid, eMode);
            m_grid.Set(ref m_msHome, "Timeout", "Home", "Timeout (ms)");
            m_grid.Set(ref m_msMotion, "Timeout", "Motion", "Timeout (ms)");
            m_grid.Set(ref m_msRS232, "Timeout", "Home", "Timeout (ms)");
            m_grid.Set(ref m_bRFIDUse, "RFID", "RFID Use", "RFID Use");
            m_infoCarrier.RunGrid(m_grid, eMode);
            m_grid.Refresh();
            //forget
        }
        #endregion 
        #region RS232
        void m_rs232_CallMsgRcv()
        {
            int nRead;
            nRead = m_rs232.Read(m_aBuf, BUF_SIZE);
            m_aBuf[nRead] = (byte)0x00;
            m_aBuf[1] = (byte)0x01;
            string sMsg = Encoding.UTF8.GetString(m_aBuf);
            int nSplitFlag = sMsg.IndexOf(SPLITFLAG);
            int nEndFlag = sMsg.IndexOf(ENDFLAG);
            string sMSGType = sMsg.Substring(nSplitFlag - 3, 3);
            string sCMD = sMsg.Substring(nSplitFlag + 1, 5);
            string[] sParam = sMsg.Split(PARAMFLAG);

            m_log.Add(sMsg);
            m_log.Add("MSGTYPE = " + sMSGType + " CMD = " + sCMD +" PARM : "+ sParam);
            switch (sMSGType)
            {
                case "ACK":
                    m_log.Add("Receive ACK");
                    AnalyzeMSG(sCMD,sParam);
                    break;
                case "INF":
                    SendFINMSGToTDKLoadport();
                    break;
                case "RIF":
                    SendFINMSGToTDKLoadport();
                    break;
                case "RAS":
                    FindTDKErrorMSG(sParam[1]);
                    SendFINMSGToTDKLoadport();
                    break;
                case "NAK":
                    SendFINMSGToTDKLoadport();
                    break;
                default:
                    for (int i = 0; i < sParam.Length; i++)
                    {
                        FindTDKErrorMSG(sParam[i]);
                        m_log.Add("Parameter = "+sParam[i]);
                    }
                    SendFINMSGToTDKLoadport();
                    break;
            }
        }

        void AnalyzeMSG(string CMD, string[] sMSGs)
        {
            switch(CMD)
            {
                case "MAPDT":
                    if (SetMappingData(sMSGs[1]) == eHWResult.Error) 
                    { 
                        SetState(eState.Error);
                    }
                    SendFINMSGToTDKLoadport();
                    break;
            }
        }
        void FindTDKErrorMSG(string Error)
        {
            #region Error_List
            switch (Error.Substring(0, 5))
            {
                case ERR_CHECKSUM:
                    SetAlarm(eAlarm.Error, eError.Checksum);
                    break;
                case ERR_COMMAND:
                    SetAlarm(eAlarm.Error, eError.Protocol);
                    break;
                case ERR_COMBUSY:
                    SetAlarm(eAlarm.Error, eError.Busy);
                    break;
                case ERR_FOUPLOADING:
                    SetAlarm(eAlarm.Error, eError.Fouploading);
                    break;
                case ERR_LATCHKEY:
                    SetAlarm(eAlarm.Error, eError.Latchkey);
                    break;
                case ERR_FOUPCLAMP:
                    SetAlarm(eAlarm.Error, eError.Foupclamp);
                    break;
                case ERR_DOCKPLATEPOS:
                    SetAlarm(eAlarm.Error, eError.DockPlate);
                    break;
                case ERR_DOOROPENCLOSEPOS:
                    SetAlarm(eAlarm.Error, eError.DoorOpenPos);
                    break;
                case ERR_DOOROPENCLOSEPOS2:
                    SetAlarm(eAlarm.Error, eError.DoorOpenPos);
                    break;
                case ERR_WAFERPROTRUSION:
                    SetAlarm(eAlarm.Error, eError.WaferProtrusion);
                    break;
                case ERR_MAPINGARTSTARTPOS:
                    SetAlarm(eAlarm.Error, eError.Mappingbar);
                    break;
                case ERR_DOORVAC:
                    SetAlarm(eAlarm.Error, eError.DoorVac);
                    break;
                case ERR_NOTINITIAL:
                    SetAlarm(eAlarm.Error, eError.NotInitial);
                    break;
                case ERR_LOADNOTCOMPLETE:
                    SetAlarm(eAlarm.Error, eError.Fouploading);
                    break;
                case ERR_UNLOADNOTCIMPLETE:
                    SetAlarm(eAlarm.Error, eError.Fouploading);
                    break;
                case ERR_ZAXISLIMIT:
                    SetAlarm(eAlarm.Error, eError.PosZLimit);
                    break;
                case ERR_YAXISLIMIT:
                    SetAlarm(eAlarm.Error, eError.PosYLimit);
                    break;
                case ERR_DOORAXISPOS:
                    SetAlarm(eAlarm.Error, eError.DoorOpenPos);
                    break;
                case ERR_MAPPINGARMPOS:
                    SetAlarm(eAlarm.Error, eError.MappingBarPos);
                    break;
                case ERR_MAPPINGARMPOSZ:
                    SetAlarm(eAlarm.Error, eError.MappingBarPosZ);
                    break;
                case ERR_MAPPINGBARSTOPPER:
                    SetAlarm(eAlarm.Error, eError.MappingBarPosStop);
                    break;
                case ERR_MAPIINGBARPOSSTOP:
                    SetAlarm(eAlarm.Error, eError.MappingBarPosStop);
                    break;
                case ERR_FOUPCLAMPOPEN:
                    SetAlarm(eAlarm.Error, eError.FoupClampOpen);
                    break;
                case ERR_FOUPCLAMCLOSE:
                    SetAlarm(eAlarm.Error, eError.FoupClampClose);
                    break;
                case ERR_LATCHKEYOPEN:
                    SetAlarm(eAlarm.Error, eError.LatchkeyOpen);
                    break;
                case ERR_LATCHKEYCLOSE:
                    SetAlarm(eAlarm.Error, eError.LatchkeyClose);
                    break;
                case ERR_VACUUMON:
                    SetAlarm(eAlarm.Error, eError.VacOn);
                    break;
                case ERR_VACUUMOFF:
                    SetAlarm(eAlarm.Error, eError.VacOff);
                    break;
                case ERR_MAINAIR:
                    SetAlarm(eAlarm.Error, eError.MainAir);
                    break;
                case ERR_FOUPOPENPOS:
                    SetAlarm(eAlarm.Error, eError.FoupOpenPos);
                    break;
                case ERR_FOUPCLOSEPOS:
                    SetAlarm(eAlarm.Error, eError.FoupClosePos);
                    break;
                case ERR_INTERLOCK:
                    m_log.Add("InterLock Error");
                    break;
                default:
                    SetAlarm(eAlarm.Error, eError.Unknown);
                    break;

            }
            #endregion 
        }

        void WriteCmd(string CMD)
        {
            // TDK MSG Format = SOH + LEN + ADR + CMD + CheckSum + DEL;
            if (CMD.Substring(0, 3) == "FIN")
                m_bSendCMD = false;
            else
                m_bSendCMD = true;

            byte[] SOH = { (byte)0x01 };    //Start Of Communication
            byte[] DEL = { (byte)0x03 };    //End Of Commnunication;
            string ADR = "00";        // Destination Address
            int n = 0;
            byte[] LEN = new byte[2];         //Lengh ADR to CSI
            string CheckSum;
            byte[] SendByte = new byte[500];
            
            CMD += ";";

            LEN[1] = Convert.ToByte(ADR.Length + CMD.Length + 2);

            CheckSum = CalcCheckSum(LEN[1], ADR, CMD);

            Buffer.BlockCopy(SOH, 0, SendByte, n, 1);
            n += 1;
            Buffer.BlockCopy(LEN, 0, SendByte, n, 2);
            n += 2;
            Buffer.BlockCopy(Encoding.UTF8.GetBytes(ADR), 0, SendByte, n, Encoding.UTF8.GetBytes(ADR).Length);
            n += Encoding.UTF8.GetBytes(ADR).Length;
            Buffer.BlockCopy(Encoding.UTF8.GetBytes(CMD), 0, SendByte, n, Encoding.UTF8.GetBytes(CMD).Length);
            n += Encoding.UTF8.GetBytes(CMD).Length;
            Buffer.BlockCopy(Encoding.UTF8.GetBytes(CheckSum), 0, SendByte, n, Encoding.UTF8.GetBytes(CheckSum).Length);
            n += Encoding.UTF8.GetBytes(CheckSum).Length;
            Buffer.BlockCopy(DEL, 0, SendByte, n, DEL.Length);
            n += DEL.Length;

            m_sLastCmd = CMD;
            m_rs232.Write(SendByte, n, true);
            m_log.Add("CMD Send : " + m_sLastCmd);
        }

        string CalcCheckSum(byte Len, string ADR, string Cmd)
        {
            string ret = "";
            int n = Len;
            for (int i = 0; i < ADR.Length; i++)
            {
                n += Convert.ToChar(ADR.Substring(i, 1));
            }
            for (int i = 0; i < Cmd.Length; i++)
            {
                n += Convert.ToChar(Cmd.Substring(i, 1));
            }
            ret = Convert.ToSByte(n % (16 * 16)).ToString("X2");

            return ret;
        }
        eHWResult WaitReply(int msDelay, string sMsg)
        {
            int ms10 = 0;
            while (m_bSendCMD)
            {
                Thread.Sleep(10);
                ms10 += 10;
                if (ms10 > msDelay)
                {
                    SetAlarm(eAlarm.Warning, eError.Timeout);
                    m_log.Popup(m_sLastCmd + " Timeout !!");
                    m_bSendCMD = false;
                    return eHWResult.Error;
                }
            }
            if (GetState() == eState.Error)
                return eHWResult.Error;
            else
                return eHWResult.OK;
        }

        void SendFINMSGToTDKLoadport()
        {
            if (m_sLastCmd.Length < 9) return; 
            string sSend = "FIN:" + m_sLastCmd.Substring(4, 5);
            WriteCmd(sSend);
        }

        #endregion
        public bool GetLoadRequest()
        {
            if (m_control.GetInputBit(m_diSwitch) == true)
            {
                return true;
            }
            return false;
        }

        public override eHWResult SetMappingData(string MapData)
        {
            eHWResult ret = eHWResult.Error;
            for (int i = 0; i < m_infoCarrier.m_lWafer; i++)
            {
                switch (MapData.Substring(m_infoCarrier.m_lWafer - i -1, 1))
                {
                    case "1":
                        m_infoCarrier.m_infoWafer[i].SetExist(true);
                        ret = eHWResult.OK;
                        break;
                    case "0":
                        m_infoCarrier.m_infoWafer[i].SetExist(false);
                        ret = eHWResult.OK;
                        break;
                    case "2":
                        m_log.Popup("Double Wafer Detected Slot : " + i.ToString());
                        m_infoCarrier.m_eState = Info_Carrier.eState.Done;
                        SetState(eState.RunUnload); 
                        return ret;
                    case "3":
                        m_log.Popup("Cross Wafer Detected, Slot : " + i.ToString());
                        m_infoCarrier.m_eState = Info_Carrier.eState.Done;
                        SetState(eState.RunUnload); 
                        return ret;
                    default:
                        m_log.Popup("Can not be Analyzed MapData");
                        return ret;
                }
            }
            m_infoCarrier.m_eState = Info_Carrier.eState.MapDone;
            return ret;
        }
        protected override void ProcInit()
        {
        }

        protected override void ProcHome()
        {
            WriteCmd(CMD_RESET_CPU);
            if (WaitReply(m_msRS232, "Error Reset Command Error") == eHWResult.Error)
            {
                SetState(eState.Error);
                return;
            }
            WriteCmd(CMD_HOME);
            if (WaitReply(m_msHome, "Home Command Error") == eHWResult.Error)
            {
                SetState(eState.Error);
                return;
            }
            SetState(eState.Ready);
        }
        protected override void ProcReady()
        {
            if (!GetLoadRequest())
                return;
            if (m_bRFIDUse)
                SetState(eState.CSTIDRead);
            else
                SetState(eState.RunLoad);
        }

        protected override void ProcCSTIDRead()
        {
            HW_RFID_Mom RFID = m_auto.ClassHandler().ClassRFID(m_nLPNum);
            ezStopWatch sw = new ezStopWatch();
            bool bRFIDFail = false;
            RFID.ResetID();

            RFID.LOTIDRead();
            sw.Start();
            while (true)
            {
                if (RFID.GetLOTID() != "")
                {
                    m_infoCarrier.m_strLotID = RFID.GetLOTID();
                    m_log.Add("Lot ID = " + RFID.GetLOTID());
                    break;
                }
                else if (sw.Check() > 2000)
                {
                    RFIDReadFail RFIDFailUI = new RFIDReadFail(m_infoCarrier);
                    m_work.WorkerBuzzerOn();
                    if (RFIDFailUI.ShowModal() == DialogResult.OK)
                        bRFIDFail = true;
                    m_work.WorkerBuzzerOff();
                    break;
                }
            }
            RFID.CSTIDRead();
            sw.Start();
            while (true && !bRFIDFail)
            {
                if (RFID.GetCSTID() != "")
                {
                    m_infoCarrier.m_strCarrierID = RFID.GetCSTID();
                    m_log.Add("CST ID = " + RFID.GetCSTID());
                    break;
                }
                else if (sw.Check() > 2000)
                {
                    RFIDReadFail RFIDFailUI = new RFIDReadFail(m_infoCarrier);
                    m_work.WorkerBuzzerOn();
                    if (RFIDFailUI.ShowModal() == DialogResult.OK)
                        bRFIDFail = true;
                    m_work.WorkerBuzzerOff();
                    break;
                }
            }
            SetState(eState.RunLoad);
        }
        
        protected override void ProcLoad()
        {
            WriteCmd(CMD_LOAD);
            if (WaitReply(m_msMotion, "Load Command Error") == eHWResult.Error)
            {
                SetState(eState.Error);
                return;
            }
            WriteCmd(CMD_GETMAPDATA);
            if (WaitReply(m_msRS232, "Get Mapping Data Error") == eHWResult.Error)
            {
                SetState(eState.Error);
                return;
            }
            if (GetState() != eState.RunUnload)
            {
                SetState(eState.LoadDone);
                if (m_auto.ClassHandler().IsWaferExistInEQP() == false)
                {
                    m_auto.ClassHandler().ClassVisionWorks().SendLotStart(m_infoCarrier.m_strLotID);
                    m_auto.ClassHandler().ClassAligner().LotStart(); 
                }
            }
        }

        protected override void ProcDone()
        {
            if (m_auto.m_strWork == EFEM.eWork.MSB.ToString()) m_infoCarrier.m_strLotID = m_xGem.m_XGem300Process[m_nLPNum].m_sJobID; // ing 170329
            if (((m_infoCarrier.IsEnableUnload()) && !(m_auto.ClassHandler().IsWaferExistInEQP())) && (m_work.GetState() == eWorkRun.Ready))
            {
                SetState(eState.RunUnload);
            }
        }

        protected override void ProcUnload()
        {
            WriteCmd(CMD_UNLOAD);
            if (WaitReply(m_msMotion, "Unload Command Error") == eHWResult.Error)
            {
                SetState(eState.Error);
                return;
            }

            if (m_infoCarrier.m_bRNR)
            {
                SetState(eState.RunLoad);
      
            }
            else
                SetState(eState.Ready);
        }

        protected override eHWResult IsCoverOpen(bool bOpen)
        {
            return eHWResult.Off;
        }

        public override eHWResult RunLoad(bool bLoad)
        {
            if (bLoad)
            {
                WriteCmd(CMD_LOAD);
                if (WaitReply(m_msMotion, "Load Command Error") == eHWResult.Error)
                {
                    return eHWResult.Error;
                }
            }
            else
            {
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
            WriteCmd(CMD_GETMAPDATA);
            if (WaitReply(m_msRS232, "Mapping Data Error") == eHWResult.Error)
            {
                return eHWResult.Error;
            }
            return eHWResult.OK;
            //return base.RunMapping();
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
//            return m_map.IsDoorOpenPos();
            return m_control.GetInputBit(m_diOpen);
        }

        public override Wafer_Size.eSize CheckCarrier()
        {
            if (IsPlaced())
                return m_infoCarrier.m_wafer.m_eSize;
            else
                return Wafer_Size.eSize.Empty;
        }

        public override bool IsDoorOpen()
        {
            return false;
            
        }

        public override bool IsDoorAxisMove()
        {
            return !m_control.GetInputBit(m_diOpen);
        }

        public override bool IsCoverClose()
        {
            return true;
        }
        
        public override void Reset()
        {
            WriteCmd(CMD_RESET_CPU);
            if (WaitReply(m_msHome, "Home Command Error") == eHWResult.Error)
            {
                SetState(eState.Error);
                return;
            }
            SetState(eState.Init);
        }

        public override bool IsDocking()
        {
            return m_control.GetInputBit(m_diOpen);
        }

        public override bool IsPlaced()
        {
            return m_control.GetInputBit(m_diPlacement) && m_control.GetInputBit(m_diPresent);
        }

        protected override void RunTimer(bool bLED)
        {
            if (!IsPlaced()) m_infoCarrier.Clear(); 
        }

    }
}
