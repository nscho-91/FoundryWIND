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
    class XGem300_MICRON : XGem300_Mom
    {
        enum eSV
        {
            SoftRev,
            MDLN,
            PPID,
            WaferID,
            RecipeID,
            RecipeID2,
            WaferState,
            DefectCount,
            PM_RepeatCount,
            PM_Wafer_DefectCount,
            PM_Wafer_Resolution,
            PM_ATI_DefectCount,
            PM_ATI_Resolution,
            PM_NIST_Resolution,
            Prev_ProcessState,
            ProcessState,
            SlotNo,
            SlotMappingData,
            
        }
        string[] m_sSVs = Enum.GetNames(typeof(eSV)); 

        enum eECV
        {
            Empty
        }
        string[] m_sECVs = Enum.GetNames(typeof(eECV));

        enum eCEID
        {
            Send_WaferState,
            PM_Wafer_LLens,
            PM_Wafer_RLens,
            PM_ATI_LLens,
            PM_ATI_RLens,
            PM_NIST_LLens,
            PM_NIST_RLens,
            Send_ProcessStateChange,
        }
        string[] m_sCEIDs = Enum.GetNames(typeof(eCEID));

        enum eRecipeMode
        {
            Create = 1,
            Delete,
            Edit
        };

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
                case eECV.Empty: break;
            }
            return 0;
        }
       
        public override void WaferStart(Info_Wafer InfoWafer)
        {
            SetSV(eSV.SlotNo, InfoWafer.m_nID+1);
            SetSV(eSV.WaferID, InfoWafer.WAFERID);
            SetSV(eSV.WaferState,"READY");
            SetSV(eSV.DefectCount, "0");
            SetEvent(eCEID.Send_WaferState);
        }

        public override void WaferEnd(Info_Wafer InfoWafer)
        {
            SetSV(eSV.SlotNo, InfoWafer.m_nID + 1);
            SetSV(eSV.WaferID, InfoWafer.WAFERID);
            SetSV(eSV.WaferState, "PENDING");
            SetSV(eSV.DefectCount, InfoWafer.m_nDefectCount.ToString());
            SetEvent(eCEID.Send_WaferState);
        }

        public override void ReviewDone(Info_Wafer InfoWafer, int nDefectCount = 0)
        {
            SetSV(eSV.SlotNo, InfoWafer.m_nSlot);
            SetSV(eSV.WaferID, InfoWafer.WAFERID);
            SetSV(eSV.WaferState, "COMPLETE");
            SetSV(eSV.DefectCount, nDefectCount.ToString());
            SetEvent(eCEID.Send_WaferState);
        }

        public override bool SetSlotMap(object obj)
        {
            m_infoCarrierSlotMap = (Info_Carrier)obj;

            string sLocID = "LP" + (m_infoCarrierSlotMap.m_nLoadPort + 1).ToString();
            string sCarrierID = m_infoCarrierSlotMap.m_strCarrierID;

            string sSlotMap = "";
            for (int n = 0; n < 25; n++) sSlotMap += GetSlotMapChar(m_infoCarrierSlotMap.m_infoWafer[n]);

            SetSV(eSV.SlotMappingData, sSlotMap);
            ezStopWatch sw = new ezStopWatch();
            m_eSetSlotMap = eSetSlotMap.Send;

            long a = m_XGem300.CMSSetSlotMap(sLocID, sSlotMap, "LP"+(m_infoCarrierSlotMap.m_nLoadPort+1).ToString(), 0);
            while (m_eSetSlotMap == eSetSlotMap.Send && (sw.Check() < 5000)) Thread.Sleep(10);

            switch (m_eSetSlotMap) {
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

        public override void SendNISTTargetData(string sWaferID, string sRcpID, int nType, int nRepeatCount, double Resolution, int nDefectCnt)
        {

            SetSV(eSV.WaferID, sWaferID);
            SetSV(eSV.RecipeID, sRcpID);
            SetSV(eSV.RecipeID2, sRcpID);

            SetSV(eSV.PM_RepeatCount,nRepeatCount);
            switch(nType){
                case 0:     //Wafer Base Target Data_Left Lens
                    SetSV(eSV.PM_Wafer_Resolution,Resolution);
                    SetSV(eSV.PM_Wafer_DefectCount, nDefectCnt);
                    SetEvent(eCEID.PM_Wafer_LLens);
                    m_log.Add("EVENT : " + eCEID.PM_Wafer_LLens.ToString());
                    break;
                case 1:     //Wafer Base Target Data_Right Lens
                    SetSV(eSV.PM_Wafer_Resolution,Resolution);
                    SetSV(eSV.PM_Wafer_DefectCount, nDefectCnt);
                    SetEvent(eCEID.PM_Wafer_RLens);
                    m_log.Add("EVENT : " + eCEID.PM_Wafer_RLens.ToString());
                    break;
                case 2:      //ATI Target Data_Left Lens
                    SetSV(eSV.PM_ATI_Resolution,Resolution);
                    SetSV(eSV.PM_ATI_DefectCount, nDefectCnt);
                    SetEvent(eCEID.PM_ATI_LLens);
                    m_log.Add("EVENT : " + eCEID.PM_ATI_LLens.ToString());
                    break;
                case 3:     //ATI Target Data_Right Lens
                    SetSV(eSV.PM_ATI_Resolution,Resolution);
                    SetSV(eSV.PM_ATI_DefectCount, nDefectCnt);
                    SetEvent(eCEID.PM_ATI_RLens);
                    m_log.Add("EVENT : " + eCEID.PM_ATI_RLens.ToString());
                    break;
                case 4:      //NIST Target Data_Left Lens
                    SetSV(eSV.PM_NIST_Resolution,Resolution);
                    SetEvent(eCEID.PM_NIST_LLens);
                    m_log.Add("EVENT : " + eCEID.PM_NIST_LLens.ToString());
                     break;     
                case 5:      //NIST Target Data_Right Lens
                    SetSV(eSV.PM_NIST_Resolution,Resolution);
                    SetEvent(eCEID.PM_NIST_RLens);
                    m_log.Add("EVENT : " + eCEID.PM_NIST_RLens.ToString());
                    break;
            }
        }

        public override void SetInitData()
        {
            SetSV(eSV.MDLN,"WIND");
            SetSV(eSV.SoftRev,"V1.0");
        }

        public override void SetPPID(string sPPID)
        {
            SetSV(eSV.PPID, sPPID);
        }
        public override void SetProcessState(XGem300_Mom.eProcessState ProcessState)
        {  
            m_XGem300.GEMSetEvent(11400);
            int Current_Process = Convert.ToInt32(GetSV(eSV.ProcessState));
            if ((XGem300_Mom.eProcessState)Current_Process == ProcessState)
                return;
            else
            {
                SetSV(eSV.Prev_ProcessState, Current_Process);
                SetSV(eSV.ProcessState, (int)ProcessState);
                SetEvent(eCEID.Send_ProcessStateChange);
            }
        }

        public override void SetMSBAlarm(XGem300_Mom.eMSBAlarm MsbAlarm)
        {
            //if(Is0nlineRemote())
            //    m_XGem300.GEMSetAlarm((int)MsbAlarm, 1);
        }

        public override void ResetMSBAlarm()
        {
            if (IsOnlineRemote())
                for (int i = 0; i < Enum.GetNames(typeof(eMSBAlarm)).Length; i++)
                    m_XGem300.GEMSetAlarm(i, 0);
        }

        protected override void RemoteCommand(long nMsgID, string sRcmd, long nCount, string[] sCpNames, string[] sCpVals, ref long[] nCpAcks) 
        {
            switch (sRcmd)
            {
                case "GOREMOTE":
                    XGemOnlineRemote();
                    break;
                case "GOLOCAL":
                    XGemOnlineLocal();
                    break;
                case "START":
                    
                    break;
                case "STOP":
                    
                    break;
                case "HOME":
                   
                    break;
                default:
                    break;
            }
        }

        public override void ResetXGem()
        {
            //            m_XGem300.PJSetState(m_XGem300Process.m_sJobID, (long)XGem300Process.eJobState.JobCanceled);
            ClearJob(0);
            ClearJob(1);
            m_XGem300.CMSDelAllCarrierInfo();
            m_log.Add("--> CMSDelAllCarrierInfo");

            for (int i = 0; i < m_nLoadPort; i++)
            {
                if (m_auto.ClassHandler().ClassLoadPort(i).IsPlaced())
                {
                    SetLPInfo(i, XGem300Carrier.eLPTransfer.TransferBlocked, XGem300Carrier.ePortAccess.Manual, XGem300Carrier.ePortReservationState.NotReserved, XGem300Carrier.ePortAssocitionState.NotAssociated, "");
                    //CarrierOnEvent(i);
                    //SetCarrierID(i, "BBB");
                    //SetTestSlotMap(i); 
                }
                else
                {
                    SetLPInfo(i, XGem300Carrier.eLPTransfer.ReadyToLoad, XGem300Carrier.ePortAccess.Manual, XGem300Carrier.ePortReservationState.NotReserved, XGem300Carrier.ePortAssocitionState.NotAssociated, "");
                }
                    
            }
            m_XGem300.GEMSetEvent(11400);
            m_log.Add("--> GEMSetEvent, 11400");
            m_XGem300.GEMSetEvent(11401);
            m_log.Add("--> GEMSetEvent, 11401");

            
        }
    }
}
