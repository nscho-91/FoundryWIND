using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ezAutoMom;
using GEM_XGem300Pro;
using ezTools;

namespace ezAuto_EFEM
{
    class XGem300_SEC : XGem300_Mom
    {
        enum eSV
        {
            Wafer_LoadPort,
            Wafer_Slot,
            Wafer_CarrierID,
            Wafer_Total,
            Wafer_Good,
            Wafer_Bad,
            Wafer_Code,
            Wafer_CodeBad,
            Wafer_ID,
            ALID,
            ALCD,
            ALTX,
            //CDA,
            //VAC,
            //VisionPress,
            //EFEMPress,
            //Temp,
            //ElecSensor, 
            Voltage,
            Current,
            WTRSpeed,
            Daily_GV,
        };
        string[] m_sSVs = Enum.GetNames(typeof(eSV));

        enum eECV
        {
        };
        string[] m_sECVs = Enum.GetNames(typeof(eECV));

        enum eCEID
        {
            LPCLamped,
            LPUnCLamped,
            LPDocking,
            LPUnDocking,
            LPOpen,
            LPClose,
            Wafer_Start,
            Wafer_End,
            WaferID_Read,
            Process_Start,
            Process_End,
        };
        string[] m_sCEIDs = Enum.GetNames(typeof(eCEID)); 

        enum eRecipeMode
        {
            Create = 1,
            Delete,
            Edit
        };

        EFEM_Handler m_handler;
        Recipe_Mom m_recipe; 

        public override void Init(string id, Auto_Mom auto)
        {
            foreach (string sID in m_sSVs)
            {
                XGem300Data xGemData = new XGem300Data(sID);
                m_aSV.Add(xGemData);
            }
            foreach (string sID in m_sECVs)
            {
                XGem300Data xGemData = new XGem300Data(sID);
                m_aECV.Add(xGemData);
            }
            foreach (string sID in m_sCEIDs)
            {
                XGem300Data xGemData = new XGem300Data(sID);
                m_aCEID.Add(xGemData);
            }
            base.Init(id, auto);
            m_handler = (EFEM_Handler)m_auto.ClassHandler();
            m_recipe = m_auto.ClassRecipe(); 
        }

        void SetSV(eSV nSV, object value, bool bLog = true)
        {
            if (m_aSV.Count <= (int)nSV) return;
            SetSV((XGem300Data)m_aSV[(int)nSV], value,bLog);
        }

        string GetSV(eSV nSV)
        {
            return GetSV((XGem300Data)m_aSV[(int)nSV]);
        }

        void SetECV(eECV nECV, object value)
        {
            SetECV((XGem300Data)m_aECV[(int)nECV], value);
        }

        void SetCEID(eCEID nCEID)
        {
            SetCEID((XGem300Data)m_aCEID[(int)nCEID]); 
        }

        protected override int ReqChangeECV(int nID, string sValue)
        {
            eECV nECV = (eECV)nID;
            switch (nECV)
            {
                default: break;
            }
            return 0;
        }

        public override void SetCEID_InspectDone(object obj)
        {
            Info_Wafer infoWafer = (Info_Wafer)obj; 
            SetSV(eSV.Wafer_LoadPort, infoWafer.m_nLoadPort);
            SetSV(eSV.Wafer_Slot, infoWafer.m_nSlot);
            SetSV(eSV.Wafer_CarrierID, infoWafer.m_strCarrierID);
            SetSV(eSV.Wafer_Total, infoWafer.m_nTotal);
            SetSV(eSV.Wafer_Good, infoWafer.m_nGood);
            SetSV(eSV.Wafer_Bad, infoWafer.m_nTotal - infoWafer.m_nGood); 
            //forget
            SetCEID(eCEID.Wafer_End); 
        }

        public override void SetFDCData()
        {
            if (m_auto != null &&  m_auto.ClassHandler().m_bFDCUse) 
            {
                string str = "0";
                //for (int i = 0; i < m_auto.ClassHandler().m_nFDCModuleNum; i++)
                //{
                //    if (m_auto.ClassHandler().ClassFDC().GetFDCModule(i) == null) continue;
                //    str = m_auto.ClassHandler().ClassFDC().GetFDCModule(i).GetFDCData().ToString();
                //    if (str != "0")
                //    {
                //        SetSV((eSV)(i + 11), str, false);
                //    }
                //}     
                if(m_auto.ClassHandler().ClassPowerMeter() != null && m_auto.ClassHandler().ClassPowerMeter().m_bUse){
                    str = m_auto.ClassHandler().ClassPowerMeter().GetVoltage();
                    SetSV(eSV.Voltage, str, false);
                    str = m_auto.ClassHandler().ClassPowerMeter().GetCurrent();
                    SetSV(eSV.Current, str,false);
                }
             }
        }

        protected override void RemoteCommand(long nMsgID, string sRcmd, long nCount, string[] sCpNames, string[] sCpVals, ref long[] nCpAcks)
        {
            string strRecipe = "Recipe";
            bool bVRS = false;
            bool bEdge = false; 
            
        }
        public override void WaferStart(Info_Wafer InfoWafer)
        {
            SetSV(eSV.Wafer_Slot, InfoWafer.m_nSlot);
            SetSV(eSV.Wafer_ID, InfoWafer.m_strWaferID);
        }

        public override void ClearWafer() {
            SetSV(eSV.Wafer_Slot, 0);
            SetSV(eSV.Wafer_ID, 0);
        }  
        public override void SetAlarmReport(XGem300ALID alid)
        {
            SetSV(eSV.ALID, alid.m_nALID);
            SetSV(eSV.ALCD, alid.m_nALCD);
            SetSV(eSV.ALTX, alid.m_id);
        }
        public override void SendDailyWaferFDC(double dDailyGV)
        {
            SetSV(eSV.Daily_GV, dDailyGV);
        } 
    }
}
