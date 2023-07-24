using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ezAutoMom;
using GEM_XGem300Pro;
using ezTools;
using System.Threading;

namespace ezAuto_EFEM
{
    class XGem300_25D : XGem300_Mom
    {
        enum eSV
        {
            AlarmsEnabled,
            AlarmSet,
            EventsEnabled,
            SpoolCountActual,
            SpoolCountTotal,
            SpoolFullTime,
            SpoolStartTime,
            SpoolStatus,
            SpoolFull,
            MDLN,
            SOFTREV,
            ALCD,
            ChangedECID,
            EventLimit,
            LimitVariable,
            OperatorCommand,
            PPChangeName,
            PPChangeStatus,
            PPError,
            TransitionType,
            Clock,
            AlarmsSet_1,
            AlarmID,
            Clock_1,
            LotID,
            SlotID,
            PortID,
            CarrierType,
            Status,
            PPID,
            CSTID,
            WaferID,
            Total,
            GoodQTY,
            BadQTY,
            CarrierQTY,
            FNLOC,
            FFROT,
            Row_Count,
            Column_Count,
            BINLIST,
            OperatorID,
            CDA_Value,
            Vacuum_Value,
            EQP_Pressure_EFEM,
            EQP_Pressure_Vision,
            EQP_Temp,
            Static_Sensor_Value,
            Ionizer1_Value,
            Ionizer2_Value,
            Ionizer3_Value,
            Main_Voltage,
            Main_Current,
            WTR_Velocity,
            WTR_Alarm_State,
            Aligner_Theta_Axis_ErrPosition,
            Aligner_Theta_Axis_Alarm_State,
            Aligner_X_Axis_ErrPosition,
            Aligner_X_Alarm_State,
            ControlState,
            PreviousControlState,
            ProcessState,
            PreviousProcessState,
            CommState,
            PreviousCommState,
        }
        string[] m_sSVs = Enum.GetNames(typeof(eSV)); 

        enum eECV
        {   Equipment_Initiated_Connected,
            EstablishCommunicationsTimeout,
            InitControlState,
            OffLineSubState,
            OnLineFailState,
            OnLineSubState,
            MaxSpoolTransmit,
            OverWriteSpool,
            EnableSpooling,
            TimeFormat,
            Maker,
            T3Timeout,
            T5Timeout,
            T6Timeout,
            T7Timeout,
            T8Timeout,
            ActiveMode,
            DeviceID,
            IPAddress,
            PortNumber,
        }
        string[] m_sECVs = Enum.GetNames(typeof(eECV));

        enum eCEID
        {
            Offline,
            Local,
            Remote,
            OperatorCommand,
            ProcessingStarted,
            ProcessingCompleted,
            ProcessingStopped,
            EquipmentConstantChanged,
            ProcessProgramChanged,
            ProcessRecipeSeleted,
            MaterialReceived,
            MaterialRemoved,
            SpoolActivated,
            SpoolDeactivated,
            SpoolTransmitFailure,
            LoadComplete,
            UnloadRequest,
            UnloadComplete,
            RFID_READ,
            RFID_Reading_Failed,
            Work_Start,
            Work_End,
            LOT_INFO_REQ,
            SLOT_MAP_INFO_REQ,
            SCRAP,
            PORT_STATUS_CHANGE,
            OCRDataReport,
            Manual_Start,
            OHT_Unload_Request,
            Track_In_Cancel,
            AlarmSet,
            AlarmClear,
            EventLimit, 
            MaxSpoolMsg,
            LinkTestInterval,
            RetryLimit,
        }
        string[] m_sCEIDs = Enum.GetNames(typeof(eCEID));

        enum eRecipeMode
        {
            Create = 1,
            Delete,
            Edit
        };




        protected override void MessageReceived(long nObjectID, long nStream, long nFunction, long nSysbyte)
        {
            switch (nStream) {
                case 7:
                    break;
            }
        }

        public override void Init(string id, Auto_Mom auto)
        {
            foreach (string sID in m_sSVs)
            {
                XGem300Data sv = new XGem300Data(sID);
                m_aSV.Add(sv);
            }
            foreach (string sID in m_sECVs)
            {
                XGem300Data ecv = new XGem300Data(sID);
                m_aECV.Add(ecv);
            }

            foreach (string sID in m_sCEIDs) {
                XGem300Data ceid = new XGem300Data(sID);
                m_aCEID.Add(ceid);
            }

            base.Init(id, auto);
        }

        void SetSV(eSV nSV, object value)
        {
            SetSV((XGem300Data)m_aSV[(int)nSV], value); 
        }

        void SetEvent(eCEID ceid)
        {
            SetCEID((XGem300Data)m_aCEID[(int)ceid]);
        }

        string GetSV(eSV nSV)
        {
            return GetSV((XGem300Data)m_aSV[(int)nSV]); 
        }

        void SetECV(eECV nECV, object value)
        {
            SetECV((XGem300Data)m_aECV[(int)nECV], value); 
        }

        protected override int ReqChangeECV(int nID, string sValue)
        {
            eECV nECV = (eECV)nID; 
            switch (nECV)
            {
                default:
                    break;
            }
            return 0;
        }

        public override void Event_PortStateChange(XGem300_Mom.ePortState25D PortState, int nPort, string sLotID, string sCSTID, eCarrierType CarrierType)
        {
            SetSV(eSV.CSTID, sCSTID);
            SetSV(eSV.LotID, sLotID);
            SetSV(eSV.PortID,nPort.ToString());
            SetSV(eSV.CarrierType,Enum.GetName(typeof(eCarrierType),CarrierType));
            SetSV(eSV.Status, Enum.GetName(typeof(ePortState25D),PortState));
            SetEvent(eCEID.PORT_STATUS_CHANGE);
        }

        public override void Event_RFIDRead(string sCSTID, string sLOTID)
        {
            SetSV(eSV.CSTID, sCSTID);
            SetSV(eSV.LotID,sLOTID);
            SetEvent(eCEID.RFID_READ);
        }
        public override void Event_LotInfoReq(string sCSTID, string sLOTID) 
        {
            SetSV(eSV.CSTID, sCSTID);
            SetSV(eSV.LotID, sLOTID);
            SetEvent(eCEID.LOT_INFO_REQ);
        }
        public override void Evnet_SlotInfoReq(string sCSTID, string sLOTID) 
        {
            SetSV(eSV.CSTID, sCSTID);
            SetSV(eSV.LotID, sLOTID);
            SetEvent(eCEID.SLOT_MAP_INFO_REQ);
        }
        public override void Event_TKInReq(string sCSTID, string sLOTID) 
        {
            SetSV(eSV.CSTID, sCSTID);
            SetSV(eSV.LotID, sLOTID);
            SetEvent(eCEID.Work_Start);
        }

        public override void SaveRecipeFile(string sRecipeName)
        {
            string sFilePath = @"C:\AVIS\Recipe\";

            sFilePath += sRecipeName + ".prm";

            IniFile.G_IniWriteValue("Recipe Infomation", "Recipe Name", sRecipeName, sFilePath);

        }

        //public override void Event_TKOutReq(string sCSTID, string sLOTID, int GoodQTY, int CarrierQTY, eCarrierType CarrierType, int nPort)
        //{
        //    SetSV(eSV.CSTID, sCSTID);
        //    SetSV(eSV.LotID, sLOTID);
        //    SetSV(eSV.GoodQTY, GoodQTY.ToString());
        //    SetSV(eSV.CarrierQTY, CarrierQTY.ToString());
        //    SetSV(eSV.CarrierType, Enum.GetName(typeof(eCarrierType), CarrierType));
        //    SetSV(eSV.PortID, nPort.ToString());
        //    SetEvent(eCEID.Work_End);
        //}
    }
}
