using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using ezAutoMom;
using ezTools;

namespace ezAuto_EFEM
{
    public class HW_VisionWorks : HW_VisionWorks_Mom, Control_Child
    {
        #region SocketMsg
        enum eCmd
        {
            LOTSTART,       //H->V //CMD,Ack/Nak,LPNumber,CarrierID,LotID,RecipeName
            LOTEND,         //H->V //CMD,Ack/Nak
            LOAD,           //H->V //CMD,Ack/Nak,WaferID,SlotNumber,Prealign
            UNLOADVISION,   //H->V //CMD,Ack/Nak,WaferID,SlotNumber
            UNLOADHANDLER,  //V->H //CMD,Ack/Nak
            HOME,           //H->V //CMD,Ack/Nak
            START,          //H->V //CMD,Ack/Nak
            STOP,           //H->V //CMD,Ack/Nak
            PROGRESS,       //V->H //CMD,Ack/Nak,Current,Total
            RESULTYIELD,    //V->H //CMD,Ack/Nak,Current,Total
            ALARM,          //V->H //CMD,Ack/Nak,Code,String
            STATE,          //V->H //CMD,Ack/Nak,StageState
            GETSTATE,       //H->V //CMD,Ack/Nak
            LOADDONE,       //H->V //CMD,Ack/Nak
            UNLOADDONE,     //H->V //CMD,Ack/Nak
            RECIPEOPEN,
            NIST_PM,        //V->H //CMD,Ack/Nak,Type,Repeat,Resolution,Defect
			INKCHANGE,      //H->V //CMD,Ack/Nak
            RCPCREATE,      //V->H //CMD,Ack/Nak,RecipeName
            DAILY_WAFER,         //V->H //CMD,Ack/Nak,GV
            SEQUENCE,
            None
        }
        eCmd m_eCmdSend = eCmd.None; 

        //Data(','로 구분, 송신측 Ack 필드는 의미 없음, 데이터 앞에 이름 붙이고':'로구분)
        enum eData
        {
            CMD,
            ACK,
            PORTNUM,
            CARRIERID,
            LOTID,
            RECIPE,
            WAFERID,
            SLOTNUM,
            PREALIGN,
            PREALIGNSUCC,
            CURRENT,
            TOTAL,
            CODE,
            STRING,
            STATE,
            PRODUCTTYPE,
            DYNAMIC,
            STATIC,
            AGING,
            MANUAL,
            WAFEREXIST,
            EDGERUN,
            REVIEWRUN,
            //For PM List
            TYPE,
            REPEAT,
            RES,
            DEFECT,
            //End PM List
			ONLYINKMARK,
            NEWRECIPE,
            DAILY_GV,
            None
        }

        int m_nCurrunt = 0;
        int m_nTotal = 0;
        string m_strCode = "";
        string m_strString = "";

        byte[] m_bufSend = new byte[4096];
        int m_lBufSend = 0; 

        //For PM List // ing 161111
        int m_nType;
        int m_nRepeat;
        double m_nResolution;
        int m_nDefect = 0;
        double m_dDailyGV = 0; 
        //Stage State	
        enum eStageState
        {
            Init = 0,
            Idle,
            RcpSetup,
            Ready,
            Load,
            Run,
            Unload,
            Stop
        }

        #endregion

        eStageState m_StageState = eStageState.Init;
        int m_diWFStgReady = -1;
        //int m_diPiezoAlarm = -1;                          //KDG 160912 Delete
        int[] m_diPiezoAlarm = new int[2] { -1, -1 };       //KDG 160912 Modify
        int m_diStgLiftUpCheck = -1;
        int[] m_diStgVacCheck = new int[3] { -1, -1, -1 };  //KDG 160912 Modify 2->3
        int[] m_diStgWFCheck = new int[3] { -1, -1, -1 };   //KDG 160912 Modify 2->3
        int[] m_diStgReady = new int[2] { -1, -1 };
        int[] m_doVisionStg = new int[3] { -1, -1, -1 };
		int m_doezASLReady = -1;
        string m_strNewRCP = "";
        bool m_bAlarm = false;
        bool m_bCommunicating = false;

        enum eError
        {
            Connect,
            Timeout,
            Protocol,
            RecipeNotExist,
            VRSAlignFail,
            VisionUnload
        } 

        //Delay 및 설정 변수
        int m_msLoad = 30000;
        int m_msUnLoad = 30000;
        int m_msStart = 30000;
        int m_msStop = 30000;
        int m_msState = 3000;
        int m_msLotEnd = 30000;
        int m_msLotStart = 30000;
        int m_msInkChange = 30000;
        Recipe_Mom m_recipe;

        bool m_bUseWaferCheckSensor = true; // ing 170327

        public override void Init(string id, Auto_Mom auto)
        {
            base.Init(id, auto);
            m_recipe = m_auto.ClassRecipe();
            InitString(); // ing 161212
            m_control.Add(this);
            RunGrid(eGrid.eRegRead);
            RunGrid(eGrid.eInit);
        }

        public void InitString()
        {
            InitString(eError.Connect, "Not Connect !!");
            InitString(eError.Timeout, "TCPIP Cmd Response Timeout !!");
            InitString(eError.Protocol, "TCPIP Cmd Protocol Error !!");
            InitString(eError.RecipeNotExist, "검사 프로그램에 레시피가 존재하지 않습니다.");
            InitString(eError.VRSAlignFail, "VRS Align Fail(Check SMI Mark)");
            InitString(eError.VisionUnload, "Vision Unload Error !!"); 
        } 

        void InitString(eError eErr, string str)
        {
            m_log.AddString(str);
            if (m_xGem == null) return;
            m_xGem.AddALID(m_id, (int)eErr, str);
        } 

        void SetAlarm(eAlarm alarm, eError eErr)
        {
            m_work.SetError(alarm, m_log, (int)eErr);
            if (m_xGem == null) return;
            m_xGem.SetAlarm(m_id, (int)eErr);
        } 

        public void ControlGrid(Control_Mom control, ezGrid rGrid, eGrid eMode)
        {
			control.AddDO(rGrid, ref m_doezASLReady, m_id, "ezASL_Ready", "ezASL Ready Signal");
            control.AddDO(rGrid, ref m_doVisionStg[0], m_id, "Stg_Ionizer_Cont", "Stage Ionizer on/off");
            control.AddDO(rGrid, ref m_doVisionStg[1], m_id, "Stg_Clean_Vac", "Stage Clean on/off");
            control.AddDO(rGrid, ref m_doVisionStg[2], m_id, "Stg_Blow_Vac", "Stage Vacuum on/off");
            control.AddDI(rGrid, ref m_diWFStgReady, m_id, "WF_Stg_Ready", "Wafer Stage Ready Signal");
            control.AddDI(rGrid, ref m_diStgReady[0], m_id, "Stg_X_Ready", "Stage X Ready");
            control.AddDI(rGrid, ref m_diStgReady[1], m_id, "Stg_Y_Ready", "Stage Y Ready");

            //KDG 160912 Delete
            //control.AddDI(rGrid, ref m_diStgWFCheck[0], m_id, "Stg_Vac_Home", "Stage Vacuum Check Home");
            //control.AddDI(rGrid, ref m_diStgWFCheck[1], m_id, "Stg_Vac_Load", "Stage Vacuum Check Load");

            //KDG 160912 Modify
            control.AddDI(rGrid, ref m_diStgWFCheck[0], m_id, "Stg_WF_Check(Home)", "Stage Wafer Check Sensor(Home Position)");
            control.AddDI(rGrid, ref m_diStgWFCheck[1], m_id, "Stg_WF_Check(Load)", "Stage Wafer Check Sensor(Load Position)");
            control.AddDI(rGrid, ref m_diStgWFCheck[2], m_id, "Stg_RF_Check", "Stage Ring Frame Check Sensor");

            //KDG 160912 Delete
            //control.AddDI(rGrid, ref m_diStgVacCheck[0], m_id, "Stg_Vac_Check1", "Stage Vacuum Check1");
            //control.AddDI(rGrid, ref m_diStgVacCheck[1], m_id, "Stg_Vac_Check2", "Stage Vacuum Check2");

            //KDG 160912 Modify
            control.AddDI(rGrid, ref m_diStgVacCheck[0], m_id, "Stg_Vac_Check1", "Stage Vacuum Check1");
            control.AddDI(rGrid, ref m_diStgVacCheck[1], m_id, "Stg_Vac_Check2", "Stage Vacuum Check2");
            control.AddDI(rGrid, ref m_diStgVacCheck[2], m_id, "Stg_Vac_Check3", "Stage Vacuum Check3");

            control.AddDI(rGrid, ref m_diStgLiftUpCheck, m_id, "Stg_Lifter_Up", "Stage Lifter Up Check");

            //KDG 160912 Delete
            //control.AddDI(rGrid, ref m_diPiezoAlarm, m_id, "Piezo Alram", "Piezo Controller Alram");

            //KDG 160912 Modify
            control.AddDI(rGrid, ref m_diPiezoAlarm[0], m_id, "Piezo1_Alram", "Piezo Controller1 Alram");
            control.AddDI(rGrid, ref m_diPiezoAlarm[1], m_id, "Piezo2_Alram", "Piezo Controller2 Alram");
        }

        protected override void RunGrid(eGrid eMode)
        {
            m_grid.Update(eMode);
            m_socket.RunGrid(m_grid);

            m_grid.Set(ref m_bEnableVision, m_id, "Use", "Use Vision"); // ing 170327
            m_grid.Set(ref m_bUseWaferCheckSensor, m_id, "CheckSensor", "Use Check Sensor"); // ing 170327
            m_grid.Set(ref m_msHome, m_id, "HomeWait", "msec");
            m_grid.Set(ref m_msLoad, m_id, "LoadWait", "msec");
            m_grid.Set(ref m_msUnLoad, m_id, "UnloadWait", "msec");
            m_grid.Set(ref m_msStart, m_id, "StartWait", "msec");
            m_grid.Set(ref m_msStop, m_id, "StopWait", "msec");
            m_grid.Set(ref m_msState, m_id, "StateWait", "msec");
            m_grid.Set(ref m_msLotEnd, m_id, "LotEndWait", "msec");
            m_grid.Set(ref m_msLotStart, m_id, "LotStartWait", "msec");
			m_grid.Set(ref m_msInkChange, m_id, "InkChangeWait", "msec");
            m_grid.Set(ref m_strRecipePath, m_id, "RecipePath", "Path"); //add by kiwon 211213           

            string sGroupCommTest = "Test";
            m_grid.Set(ref m_infoWaferTest.m_nLoadPort, sGroupCommTest, "LoadPort", "LoadPort");
            m_grid.Set(ref m_infoWaferTest.m_nSlot, sGroupCommTest, "SlotNum", "SlotNum");
            m_grid.Set(ref m_infoWaferTest.m_fAngleAlign, sGroupCommTest, "PreAlign", "PreAlign");
            m_grid.Set(ref m_infoWaferTest.m_bPrealignSucc, sGroupCommTest, "PrealignSucc", "PrealignSucc");
            m_infoWaferTest.m_wafer.RunGrid(false,m_grid, eMode, sGroupCommTest, "WaferSize"); 
            m_grid.Set(ref m_infoCarrierTest.m_strCarrierID, sGroupCommTest, "CarrierID", "CarrierID");
            m_grid.Set(ref m_infoCarrierTest.m_strLotID, sGroupCommTest, "LotID", "LotID");
            m_grid.Set(ref m_infoCarrierTest.m_strRecipe, sGroupCommTest, "Recipe", "Reicpe");
            

            m_grid.Refresh();
        }

        protected override void RunThread()
        {
            m_bRunThread = true;
            Thread.Sleep(5000);

            while (m_bRunThread)
            {
                Thread.Sleep(10); 
                switch (m_eState)
                {
                    case eState.Init: break;
                    case eState.Home:
                        if (m_doVisionStg[1] > -1) m_control.WriteOutputBit(m_doVisionStg[1], false);
                        if (m_doVisionStg[2] > -1) m_control.WriteOutputBit(m_doVisionStg[2], false);
                        if (RunHome() == eHWResult.OK)
                        {
                            m_log.WriteLog("Sequence", "[" + m_id + "Done]" + " Home PASS"); //230724 nscho
                            SetState(eState.LoadWait);
                        }
                        else
                        {
                            SetState(eState.Init);
                        }
                        break;
                    case eState.LoadWait:
                        break;
                    case eState.Load:
                        if (m_doVisionStg[1] > -1) m_control.WriteOutputBit(m_doVisionStg[1], true);
                        if (m_doVisionStg[2] > -1) m_control.WriteOutputBit(m_doVisionStg[2], true);
                        if (RunLoad(m_infoWafer) == eHWResult.OK)
                        {
                            SetState(eState.Ready);
                        }    
                        else
                        {
                            SetState(eState.Error);
                        }
                        break;
                    case eState.Ready:                       
                        break;
                    case eState.LoadDone:
                        if (m_doVisionStg[1] > -1) m_control.WriteOutputBit(m_doVisionStg[1], false);
                        if (m_doVisionStg[2] > -1) m_control.WriteOutputBit(m_doVisionStg[2], false);
                        if (SendLoadDone(m_infoWafer) == eHWResult.OK)
                        {
                            SetState(eState.Run);
                        }
                        else
                        {
                            SetState(eState.Error);
                        }
                        break;
                    case eState.Run:// 비전웍스가 검사 할때. 1min마다 체크해서 프로그램 다운 감지 하도록 추가.
                        Thread.Sleep(10000);
                        if (CheckStageState() != eHWResult.OK)
                        {
                            m_log.Popup("Vision Check Stage State Error !!", true);
                            SetState(eState.Error);
                        }                           
                        break;
                    case eState.Done:// 검사 끝나고 리프터 올리면 던 UNLOADVISION오면 Done으로 
                        break;
                    case eState.UnLoadDone:
                        if (SendUnloadDone() == eHWResult.OK)
                        {
                            SetState(eState.LoadWait);
                        }
                        else
                        {
                            SetAlarm(eAlarm.Warning, eError.VisionUnload); 
                        }
                        break;
                    case eState.Error:
                        SetAlarm(eAlarm.Warning, eError.Protocol);
                        SetState(eState.Init);
                        break;
                }
            }
        }

        public override bool IsLifterUp()
        {
            if (m_diStgLiftUpCheck < 0) return true;
            return m_control.GetInputBit(m_diStgLiftUpCheck);
        }

        public override bool IsWaferExistHome()
        {
            if (!m_bUseWaferCheckSensor) return m_bWaferExist; // ing 170327
            return m_control.GetInputBit(m_diStgWFCheck[0]);
        }

        public override bool IsWaferExistLoad()
        {
            if (!m_bUseWaferCheckSensor) return m_bWaferExist; // ing 170327
            return m_control.GetInputBit(m_diStgWFCheck[1]);
        }

        public override bool IsWaferVacOn()
        {
            return m_control.GetInputBit(m_diStgVacCheck[0]);
        }

        public override eHWResult IsWaferExist()
        {
            if (m_bWaferExist)
                return eHWResult.On;
            else
                return eHWResult.Off;
        }

        ezStopWatch m_swVSConnect = new ezStopWatch();
        

        public override bool IsConnected()
        {
            if (IsVisionEnable())
            {
                if (m_socket.IsConnect()) return true;
                else
                {
                    if (m_swVSConnect.Check() >= 10000) {
                        
                        SetAlarm(eAlarm.Warning, eError.Connect); // ing 161208
                        m_swVSConnect.Start();
                    }
                    return false;
                }
                //return m_socket.IsConnect();
            }
            else
                return false;
        }

        eHWResult WaitReply(int msWait)
        {
            int ms = 0;
            while (m_eCmdSend != eCmd.None)
            {
                Thread.Sleep(10);
                ms += 10;
                lock (m_csLock)
                {
                    if (m_bAlarm)
                    {
                        m_bAlarm = false;
                        return eHWResult.OK;
                    }
                }
                if (ms > msWait)
                {
                    m_log.Popup("Cmd : " + m_eCmdSend.ToString() + ", Wait : " + msWait.ToString());
                    if (m_eCmdSend != eCmd.LOTSTART && m_eCmdSend != eCmd.LOTEND)
                        m_xGem.SetMSBAlarm(XGem300_Mom.eMSBAlarm.VS_TCPCmd_Error);
                    SetAlarm(eAlarm.Warning, eError.Timeout);
                    m_eCmdSend = eCmd.None;
                    lock (m_csLock) m_bCommunicating = false;
                    return eHWResult.Error;
                }
            }
            m_eCmdSend = eCmd.None;
            lock (m_csLock) m_bCommunicating = false;
            return eHWResult.OK;
        }

        protected override eHWResult RunHome()
        {
            m_log.WriteLog("Sequence", "[" + m_id + "Start]" + " Home"); //230724 nscho
            if (SetCmd(eCmd.HOME))
            {
                m_log.WriteLog("Sequence", "[" + m_id + "Done]" + "Home FAIL"); //230724 nscho
                return eHWResult.Error;
            }
            eTCPResult eResult = SendCmd();
            if (eResult != eTCPResult.OK)
            {
                m_log.WriteLog("Sequence", "[" + m_id + "Done]" + "Home FAIL"); //230724 nscho
                return eHWResult.Error;
            }
            return WaitReply(m_msHome);
        }

        public override eHWResult CheckStageState()
        {
            if (SetCmd(eCmd.GETSTATE)) return eHWResult.Error; 
            eTCPResult eResult = SendCmd();
            if (eResult != eTCPResult.OK) return eHWResult.Error;
            return WaitReply(m_msState);
        }

        protected override eHWResult RunLoad(Info_Wafer infoWafer)
        {
            if (infoWafer == null)
            {
                m_log.Popup("SendLoad : Info Wafer is null");
                return eHWResult.Error;
            }
            Info_Carrier infoCarrier = m_handler.ClassCarrier(infoWafer.m_nLoadPort);
            if (infoCarrier == null)
            {
                m_log.Popup("SendLoad : InfoCarrier is null");
                return eHWResult.Error;
            }

            if (SetCmd(eCmd.LOAD)) return eHWResult.Error;
            AddData(eData.PORTNUM, infoWafer.m_nLoadPort);
            AddData(eData.CARRIERID, infoCarrier.m_strCarrierID);
            AddData(eData.LOTID, infoCarrier.m_strLotID);
            AddData(eData.RECIPE, infoCarrier.m_strRecipe);
            AddData(eData.WAFERID, infoWafer.WAFERID);
            AddData(eData.SLOTNUM, infoWafer.m_nSlot);
            AddData(eData.PREALIGN, 0);
            AddData(eData.PREALIGNSUCC, GetBoolString(infoWafer.m_bPrealignSucc));
            AddData(eData.PRODUCTTYPE, GetVisionProductType(infoWafer.m_wafer.m_eSize));
            AddData(eData.EDGERUN, GetBoolString(infoWafer.m_bRunEdge));
            AddData(eData.REVIEWRUN, GetBoolString(infoWafer.m_bUseVision)); 
			AddData(eData.ONLYINKMARK, GetBoolString(infoWafer.m_bOnlyInkMark)); 
            eTCPResult eResult = SendCmd();

            if (eResult != eTCPResult.OK) return eHWResult.Error;
            return WaitReply(m_msLoad);
        }

        string GetBoolString(bool b)
        {
            if (b) return "TRUE";
            else return "FALSE"; 
        }

        public override eHWResult RunUnload()
        {
            if (SetCmd(eCmd.UNLOADHANDLER)) return eHWResult.Error; 
            eTCPResult eResult = SendCmd();
            if (eResult != eTCPResult.OK) return eHWResult.Error;
            return WaitReply(m_msUnLoad);
        }

        public override eHWResult RunStop()
        {
            if (SetCmd(eCmd.STOP)) return eHWResult.Error; 
            eTCPResult eResult = SendCmd();
            if (eResult != eTCPResult.OK) return eHWResult.Error;
            return WaitReply(m_msStop);
        }

        protected override eHWResult RunStart()
        {
            if (SetCmd(eCmd.START)) return eHWResult.Error; 
            eTCPResult eResult = SendCmd();
            if (eResult != eTCPResult.OK) return eHWResult.Error;
            return WaitReply(m_msStart);
        }

        public override eHWResult SendLotStart(string strLotID)
        {
            if (SetCmd(eCmd.LOTSTART))
            {
                m_xGem.SetMSBAlarm(XGem300_Mom.eMSBAlarm.VS_LotStart_Error);
                return eHWResult.Error;
            } 
            AddData(eData.LOTID, strLotID); 
            eTCPResult eResult = SendCmd();
//            m_xGem.SetCEID_LotStart(infoCarrier);
            if (eResult != eTCPResult.OK)
            {
                m_xGem.SetMSBAlarm(XGem300_Mom.eMSBAlarm.VS_LotStart_Error);
                return eHWResult.Error;
            }
            return WaitReply(m_msLotStart);
        }

        public override eHWResult SendLotEnd(Info_Carrier infoCarrier)
        {
            if (SetCmd(eCmd.LOTEND))
            {
                m_xGem.SetMSBAlarm(XGem300_Mom.eMSBAlarm.VS_LotEnd_Error);
                return eHWResult.Error;
            }
            AddData(eData.LOTID, infoCarrier.m_strLotID);
            eTCPResult eResult = SendCmd();
            m_xGem.SetCEID_LotEnd(infoCarrier);
            if (eResult != eTCPResult.OK)
            {
                m_xGem.SetMSBAlarm(XGem300_Mom.eMSBAlarm.VS_LotEnd_Error);
                return eHWResult.Error;
            }
            return WaitReply(m_msLotEnd);
        }

        public override eHWResult SendLoadDone(Info_Wafer infoWafer) 
        {
            if (infoWafer == null)
            {
                m_log.Popup("SendLoadDone : Info Wafer is null"); 
                return eHWResult.Error;
            }
            HW_LoadPort_Mom loadport = m_handler.ClassLoadPort(infoWafer.m_nLoadPort); 
            if (loadport == null)
            {
                m_log.Popup("SendLoadDone : LoadPort is null");
                return eHWResult.Error;
            }

            if (SetCmd(eCmd.LOADDONE)) return eHWResult.Error;
            AddData(eData.DYNAMIC, loadport.m_nDynamic);
            AddData(eData.STATIC, loadport.m_nStatic);
            AddData(eData.AGING, GetBoolString(loadport.m_bAging));
            AddData(eData.MANUAL, GetBoolString(loadport.m_bManual));
            eTCPResult eResult = SendCmd();
            if (eResult != eTCPResult.OK) return eHWResult.Error;
            return WaitReply(m_msLotEnd);
        }

        public override eHWResult SendUnloadDone() 
        {
            if (SetCmd(eCmd.UNLOADDONE)) return eHWResult.Error;
            eTCPResult eResult = SendCmd();
            if (eResult != eTCPResult.OK) return eHWResult.Error;
            m_eCmdSend = eCmd.None;
            return eHWResult.OK;
            //return WaitReply(m_msLotEnd);
        }

        eHWResult SendUnloadVision(Info_Wafer infoWafer)
        {
            if (infoWafer == null)
            {
                m_log.Popup("SendLoaddone : Info Wafer is null");
                return eHWResult.Error;
            }
            if (SetCmd(eCmd.UNLOADVISION)) return eHWResult.Error;
            AddData(eData.WAFERID, infoWafer.WAFERID);
            AddData(eData.SLOTNUM, infoWafer.m_nSlot);
            eTCPResult eResult = SendCmd();
            if (eResult != eTCPResult.OK) return eHWResult.Error;
            return eHWResult.OK; 
        }

        eHWResult RCPCreate()
        {
            if (m_strNewRCP == "")
            {
                m_log.Popup("Can Not Create Recipe Cause New Rcipe Name Is Null");
                return eHWResult.Error;
            }
            if (!File.Exists(m_recipe.GetFileName(m_strNewRCP)))
            {
                m_recipe.CreateDefaltRCP(m_strNewRCP);
            }
            if (SetCmd(eCmd.RCPCREATE)) return eHWResult.Error;
            eTCPResult eResult = SendCmd();
            if (eResult != eTCPResult.OK) return eHWResult.Error;
            return eHWResult.OK;
        }

        static readonly object m_csLock = new object();

        bool SetCmd(eCmd cmd)
        {
            lock (m_csLock)
            {
                m_lBufSend = 0;
                ezStopWatch sw = new ezStopWatch();
                while ((m_eCmdSend != eCmd.None) && (sw.Check() < 2000)) Thread.Sleep(10);
                if (m_eCmdSend != eCmd.None) m_log.Popup("Send Cmd is Busy : " + m_eCmdSend.ToString());
                if (!m_socket.IsConnect()) return true;
                m_eCmdSend = cmd;
                string strCmd = "CMD:" + cmd.ToString() + ",ACK:TRUE,";
                foreach (char ch in strCmd) m_bufSend[m_lBufSend++] = (byte)ch;
                return false; 
            }
        }

        bool AddData(eData data, object obj)
        {
            lock (m_csLock)
            {
                string strData = data.ToString() + ":" + obj.ToString();
                foreach (char ch in strData) m_bufSend[m_lBufSend++] = (byte)ch;
                m_bufSend[m_lBufSend++] = (byte)(',');
                return false;
            }
        }

        eTCPResult SendCmd()
        {
            if (!m_bEnableVision)
            {
                m_log.Add("Can not Use VisionWorks. Please Check a Option.");
                return eTCPResult.Error;
            }
            m_log.Add("--> " + Encoding.Default.GetString(m_bufSend, 0, m_lBufSend));
            lock (m_csLock) m_bCommunicating = true;
            return m_socket.Send(m_bufSend, m_lBufSend);
        }

        public override string GetStageState()
        {
            return m_StageState.ToString();
        }

        public override eHWResult IsWFStgReady() //pnj
        {
            if (!m_control.GetInputBit(m_diStgReady[0]) || !m_control.GetInputBit(m_diStgReady[1]))
            {
                return eHWResult.Error;
            }
            else return eHWResult.OK;
        }

        public eHWResult IsPiezoControl() //pnj
        {
            //if (m_control.GetInputBit(m_diPiezoAlarm))  //KDG 160912 Delete
            if (m_control.GetInputBit(m_diPiezoAlarm[0]) || m_control.GetInputBit(m_diPiezoAlarm[1]))    //KDG 160912 Modify
            {
                m_log.Add("Piezo Controller Alram!!!");
                return eHWResult.Error;
            }
            return eHWResult.OK;
        }

        //KDG 161007 Delete IsLifterUp()과 같은 함수여서 삭제
        /*
        public override eHWResult IsStgLiftUp() //pnj
        {
            if (m_control.GetInputBit(m_diStgLiftUpCheck)) return eHWResult.Off;
            else return eHWResult.On;
        }
        */

        public override bool IsVSReadySignal()
        {
            return m_control.GetInputBit(m_diWFStgReady);
        }
        
        eCmd GetCmd(string[] sCmds)
        {
            if ((sCmds.Length != 2) || (sCmds[0] != "CMD"))
            {
                m_log.Popup("Invalid Cmd Protocol");
                return eCmd.None;
            }
            for (int n = 0; n < (int)eCmd.None; n++)
            {
                if (sCmds[1] == ((eCmd)n).ToString()) return (eCmd)n;
            }
            return eCmd.None;
        }

        bool IsACK(string[] sACKs)
        {
            if ((sACKs.Length != 2) || (sACKs[0] != "ACK") || (sACKs[1] != "TRUE"))
            {
                m_log.Popup("Invalid ACK Protocol");
                return false;
            }
            return true; 
        }

        eData GetData(string sData)
        {
            for (int n = 0; n < (int)eData.None; n++)
            {
                if (sData == ((eData)n).ToString())   //KJW 160718 eCMD-> eData
                    return (eData)n;
            }
            return eData.None; 
        }

        bool GetData(eCmd nCmd, string[] sDatas)
        {
            if (sDatas.Length != 2)
            {
                m_log.Popup("Invalid Protocol");
                return true;
            }
            eData data = GetData(sDatas[0]);
            switch (data)
            {
                case eData.CMD:
                    return true; 
                case eData.ACK:
                    m_bACK = (sDatas[1] == "TRUE");
                    break;
                case eData.None:
                    m_log.Popup("Invalid Data Protocol");
                    return false;
                case eData.WAFEREXIST:
                    m_bWaferExist = (sDatas[1] == "TRUE"); 
                    break;
                case eData.CURRENT:
                    m_nCurrunt = Convert.ToInt32(sDatas[1]); 
                    break;
                case eData.TOTAL:
                    m_nTotal = Convert.ToInt32(sDatas[1]); 
                    break;
                case eData.CODE:
                    m_strCode = sDatas[1];
                    break; 
                case eData.STRING:
                    m_strString = sDatas[1]; 
                    break;
                case eData.STATE:
                    m_StageState = (eStageState)Convert.ToInt32(sDatas[1]);
                    break;
                case eData.TYPE:
                    m_nType = Convert.ToInt32(sDatas[1]);
                    break;
                case eData.REPEAT:
                    m_nRepeat = Convert.ToInt32(sDatas[1]);
                    break;
                case eData.RES:
                    m_nResolution = Convert.ToDouble(sDatas[1]);
                    break;
                case eData.DEFECT:
                    m_infoWafer.m_nDefectCount = m_nDefect = Convert.ToInt32(sDatas[1]);
                    break;
                case eData.NEWRECIPE:
                    m_strNewRCP = sDatas[1];
                    break;
                case eData.DAILY_GV:
                    m_dDailyGV = Convert.ToDouble(sDatas[1]);
                    break; 
            }
            return false; 
        }

        eCmd ReadCmd(string[] sMsgs, ref int nIndex)
        {
            eCmd nCmd = GetCmd(sMsgs[nIndex++].Split(':'));
            bool bACK = IsACK(sMsgs[nIndex++].Split(':'));
            if (!bACK) return eCmd.None;
            while (nIndex < sMsgs.Length - 1)
            {
                if (GetData(nCmd, sMsgs[nIndex].Split(':'))) return nCmd;
                nIndex++; 
            }
            return nCmd; 
        }
        
        protected override void CallMsgRcv(byte[] byteMsg, int nSize)
        {
            base.CallMsgRcv(byteMsg, nSize);
            string sMsg = Encoding.Default.GetString(byteMsg, 0, nSize);
            m_log.Add("<< " + sMsg);

            string[] sMsgs = sMsg.Split(',');
            if (sMsgs.Length < 3)
            {
                m_log.Popup("Too Short Massage : " + sMsg);
                return; 
            }

            int nIndex = 0;
            while (nIndex < sMsgs.Length - 1)
            {
                eCmd nCmd = ReadCmd(sMsgs, ref nIndex);
                switch (nCmd)
                {
                    case eCmd.LOTSTART:
                        if(GetState() != eState.LoadWait)
                            SetState(eState.LoadWait);
                        break;
                    case eCmd.LOAD:
                        m_infoWafer.m_nProgressCurrent = 0;
                        /*if (!m_bACK)
                            SetAlarm(eAlarm.Warning, eError.RecipeNotExist);*/
                        break;
                    case eCmd.UNLOADVISION:
                        SetState(eState.Done); //done으로 바꾸며 ㄴwtr이 집으러감
                        SendUnloadVision(m_infoWafer);
                        break;
                    case eCmd.UNLOADHANDLER:
                        SetState(eState.Done);
                        break;
                    case eCmd.PROGRESS:
                        m_infoWafer.m_nProgressCurrent = m_nCurrunt;
                        m_infoWafer.m_nProgressTotal = m_nTotal;
                        break;
                    case eCmd.RESULTYIELD:
                        m_infoWafer.m_nGood = m_nCurrunt;
                        m_infoWafer.m_nTotal = m_nTotal;
                        m_infoWafer.m_nProgressCurrent = m_infoWafer.m_nProgressTotal;
                        m_xGem.SetCEID_InspectDone(m_infoWafer);
                        m_infoWafer.m_bVSAlignFail = false; 
                        break;
                    case eCmd.ALARM:
                        lock (m_csLock)
                        {
                            if (m_bCommunicating) m_bAlarm = true;
                        }
                        m_log.Popup("ALARM - " + m_strCode + " : " + m_strString, true);
                        if (m_strCode.Substring(0, 4) == "1206")
                        {
                            m_infoWafer.m_nGood = -1;
                            m_infoWafer.m_nTotal = 1;
                            m_work.RunBuzzer(Work_Mom.eBuzzer.Buzzer, true);
                            m_auto.ClassXGem().SetAlarm(1202, true);
                        }
                        else if (m_strCode.Substring(0, 4) == "1201")
                        {
                            SetAlarm(0, eError.RecipeNotExist);
                            m_log.Popup("Recipe가 존재하지 않습니다. - " + m_strString);
                        }
                        else if (m_strCode.Substring(0,4) == "1202")
                        {
                            m_infoWafer.m_bVSAlignFail = true;  
                        } 
                        else if (m_strCode.Substring(0,4) == "1221")
                        {
                            m_handler.RunStop();
                            RunStop();
                            SetAlarm(eAlarm.Popup, eError.VRSAlignFail);
                        } 
                        break;
                    case eCmd.STATE:
                        if (m_eCmdSend == eCmd.GETSTATE) m_eCmdSend = eCmd.None;
                        break;
                    case eCmd.NIST_PM:
                        m_log.Add("PM LIST -> TYPE : " + m_nType.ToString() + ", REPEAT : " + m_nRepeat.ToString() + ", RESOLUTION : " + m_nResolution.ToString() + ", DEFECT : " + m_nDefect.ToString());
                        //For Need Secs/Gem
                        m_auto.ClassXGem().SendNISTTargetData(m_recipe.m_sPMWaferID, m_recipe.m_sRecipe, m_nType, m_nRepeat, m_nResolution, m_nDefect);
                        break;
                    case eCmd.RCPCREATE:
                        RCPCreate();
                        break;
                    case eCmd.DAILY_WAFER:
                        m_log.Add("Send DailyGV Infomation");
                        m_xGem.SendDailyWaferFDC(m_dDailyGV);
                        break; 
                    case eCmd.SEQUENCE: 
                        m_log.Popup("Seqence - " + m_strCode + " : " + m_strString, true); 
                        m_log.WriteLog("Sequence", m_strString); //230724 nscho 
                        break;
                }

                if (m_eCmdSend == nCmd) 
                    m_eCmdSend = eCmd.None;
            }

        }

        int GetVisionProductType(Wafer_Size.eSize s)
        {
            switch (s)
            {
                case Wafer_Size.eSize.inch4: return 1;
                case Wafer_Size.eSize.inch5: return 10;
                case Wafer_Size.eSize.inch6: return 2;
                case Wafer_Size.eSize.mm200: return 3;
                case Wafer_Size.eSize.mm300: return 4;
                case Wafer_Size.eSize.mm300_RF: return 5;
                case Wafer_Size.eSize.inch8: return 3;
                default: return 10;
            }
        }

        public override bool IsVisionEnable()
        {
            return m_bEnableVision; // ing 170327
            //return m_recipe.m_bUseVision;
        }
/*	
    ProductType_Inch_2	0
    ProductType_Inch_4	1
    ProductType_Inch_6	2
    ProductType_Inch_8	3
    ProductType_Inch_12	4
    ProductType_RingFrame	5
    ProductType_Reticle	6
    ProductType_Tray	7
    ProductType_Strip	8
    ProductType_MetalTray	9
    ProductType_END	10
*/

		public override void ezASLReady(bool bOn)
        {
            m_control.WriteOutputBit(m_doezASLReady, bOn);
        }

        public override eHWResult RunInkChange()
        {
            if (SetCmd(eCmd.INKCHANGE)) return eHWResult.Error;
            eTCPResult eResult = SendCmd();
            if (eResult != eTCPResult.OK) return eHWResult.Error;
            return WaitReply(m_msInkChange);
        }
    }
}
