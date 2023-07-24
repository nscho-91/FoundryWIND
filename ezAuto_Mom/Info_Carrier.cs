using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ezTools; 

namespace ezAutoMom
{
    public class Info_Carrier
    {
        public enum eState
        {
            Init,
            MapDone,
            ManualJob,
            Ready,
            Run,
            Done
        };
        public eState m_eState = eState.Init;

        public enum eJobState
        {
            Reserved,
            Cancelled,
            Run,
            Done
        };
        public eJobState m_eJobState = eJobState.Run;

        public enum eSlotType
        {
            ATI,
            Sanken
        }
        public eSlotType m_eSlotType = eSlotType.ATI;

        public DateTime m_timeLotStart = new DateTime();
        public DateTime m_timeLotEnd = new DateTime();

        public string m_strJobID = "";
        public string m_strLotID = "";
        public string m_strCarrierID = "";
        public string m_strRecipe = "";
        public string m_strInkMark = ""; // for Denso

        public string m_id;
        public bool m_bRNR = false;
        public int m_nLoadPort = 0;
        public int m_lWafer = 25;
        public bool m_bUseVision = false;

        public bool m_bMappingDone = true; 

        public Info_Wafer[] m_infoWafer = new Info_Wafer[25];
        Log m_log;
        public Wafer_Size m_wafer; 

        public Info_Carrier(int nLoadPort)
        {
            m_nLoadPort = nLoadPort;
        }

        public void Init(string id, Log log, Wafer_Size.eSize eSize = Wafer_Size.eSize.Empty)
        {
            int n;
            m_id = id; m_log = log;
            m_wafer = new Wafer_Size(eSize, m_log);
            for (n = 0; n < 25; n++)
                m_infoWafer[n] = new Info_Wafer(m_nLoadPort, n, InitSlotNumber(n), m_log, eSize);
        }

        int InitSlotNumber(int nID)
        {
            switch (m_eSlotType)
            {
                case eSlotType.ATI: return nID + 1;
                case eSlotType.Sanken: return 25 - nID; 
            }
            return 0; 
        }

        public void Clear()
        {
            for (int n = 0; n < 25; n++) m_infoWafer[n].SetEmpty();
        }

        public Info_Wafer GetLoadWafer()
        {
            int iMin = -1, nMin = 1000; 
            if (m_eState == eState.Ready) m_eState = eState.Run;
            if (m_eState != eState.Run) return null;
            for (int n = 0; n < m_lWafer; n++)
            {
                if (m_infoWafer[n].State == Info_Wafer.eState.Select)
                {
                    if (nMin > m_infoWafer[n].m_nSlot)
                    {
                        nMin = m_infoWafer[n].m_nSlot;
                        iMin = n;
                    }
                }
            }
            if (iMin < 0) return null;
            return m_infoWafer[iMin]; 
        }

        public bool CheckDone(bool bCycleStop)
        {
            int n;
            for (n = 0; n < m_lWafer; n++)
            {
                if (!bCycleStop && m_infoWafer[n].State == Info_Wafer.eState.Select) return false;
                if (m_infoWafer[n].State == Info_Wafer.eState.Run) return false;
            }
            if (m_bRNR)
            {
                for (n = 0; n < m_lWafer; n++)
                {
                    if (m_infoWafer[n].State == Info_Wafer.eState.Done)
                        m_infoWafer[n].State = Info_Wafer.eState.Select;
                }
            }
            m_eState = eState.Done;
            return true;
        }

        public void RunGrid(ezGrid rGrid, eGrid eMode)
        {
            if (eMode != eGrid.eRegRead) rGrid.Set(ref m_bRNR, "Test", "RNR", "RNR Test Done -> Ready");
            m_eSlotType = (eSlotType)rGrid.Set((int)m_eSlotType, "Type", "Slot", "0 = ATI, 1 = Sanken");

            m_wafer.RunGrid(false,rGrid, eMode, "Wafer", "Size"); 
            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++)
            {
                string strSize = ((Wafer_Size.eSize)n).ToString(); 
                rGrid.Set(ref m_wafer.m_bEnable[n], "EnableSize", strSize, "Enable Whole Wafer Size");
                rGrid.Set(ref m_wafer.m_bLPMapping[n], "LoadportMapping", strSize, "Enable Loadport Mapping");
                rGrid.Set(ref m_wafer.m_lWafer[n], "WaferCount", strSize, "# of Wafer in Carrier");
            }
            rGrid.Set(ref m_lWafer, "Wafer", "Total Slot", "Total Slot Number");
        }

        public void SetWaferSize(Wafer_Size.eSize nSize)
        {
            m_wafer.m_eSize = nSize;
            for (int n = 0; n < m_lWafer; n++) m_infoWafer[n].m_wafer.m_eSize = nSize;
        }

        public Wafer_Size.eSize GetWaferSize()
        {
            return m_wafer.m_eSize;
        }
        public bool IsEnableUnload()
        {
            for (int n = 0; n < m_lWafer; n++)
            {
                if (m_infoWafer[n].Locate != Info_Wafer.eLocate.LoadPort) return false;
                if (m_infoWafer[n].State == Info_Wafer.eState.Select || m_infoWafer[n].State == Info_Wafer.eState.Run)
                    return false;
            }
            return true; 
        }
  
        public bool IsWaferOut()
        {
            for (int n = 0; n < m_lWafer; n++)
            {
                if (m_infoWafer[n].Locate != Info_Wafer.eLocate.LoadPort) 
                    return true;
                if (m_infoWafer[n].State == Info_Wafer.eState.Select || m_infoWafer[n].State == Info_Wafer.eState.Run)
                    return true;
                if (m_infoWafer[n].m_eAngleRF != Info_Wafer.eAngleRF.R0 && m_infoWafer[n].m_wafer.IsRingFrame())
                    return true;
            }
           return false;
        }

        public string GetSlotMap()
        { 
            string SlotMap = "";
            foreach (Info_Wafer info in m_infoWafer)
            { 
                if(info.State == Info_Wafer.eState.Empty)
                {
                    SlotMap += "0";
                }
                else
                {
                    SlotMap += "1";
                }
            }
            return SlotMap;
        }

        public string GetEdgeLotEnd()
        {
            int nMaxSlot = 0; 
            for (int n = 0; n < 25; n++)
            {
                if ((m_infoWafer[n].State == Info_Wafer.eState.Select) || (m_infoWafer[n].State == Info_Wafer.eState.Run))
                {
                    if (nMaxSlot < m_infoWafer[n].m_nSlot) nMaxSlot = m_infoWafer[n].m_nSlot;
                }
            }
            return nMaxSlot.ToString(); 
        }

        public void ResetWaferID()
        {
            for (int i = 0; i < m_lWafer; i++)
            {
                m_infoWafer[i].m_nTotal = 0;
                m_infoWafer[i].WAFERID = "";
            }
        }
    }
}
