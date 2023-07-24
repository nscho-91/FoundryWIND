using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ezTools;

namespace ezAutoMom
{
    public class InfoCarrier
    {
        public enum eSlotType
        {
            Up,
            Down
        }
        public eSlotType m_eSlotType = eSlotType.Up;

        public DateTime m_timeLotStart = new DateTime();
        public DateTime m_timeLotEnd = new DateTime();

        public string m_id;
        public int m_nID;
        Log m_log;

        const int c_lWafer = 25;
        public int m_lWafer = 25;
        public InfoWafer[] m_infoWafer = new InfoWafer[c_lWafer];

        public Wafer_Size m_wafer; 

        public enum eXGemTransfer
        {
            OutOfService = 0,
            TransferBlocked,
            ReadyToLoad,
            ReadyToUnload
        }
        public eXGemTransfer m_eXTranfer = eXGemTransfer.OutOfService;

        public string m_sXGemCarrierID = "";
        public string m_sXGemSlotMap = ""; 

        public enum eXGemState
        {
            NotRead,
            WaitForHost,
            VerrificationOK,
            VerrificationFailed
        }
        public eXGemState m_eXGemState = eXGemState.NotRead;
        public eXGemState m_eXGemStateSlotMap = eXGemState.NotRead;

        public enum eXGemAccess
        {
            NotAccessed,
            InAccessed,
            CarrierCompleted,
            CarrierStoped
        }

        public XGem300Process m_xGemProcess = new XGem300Process();
        public XGem300Control m_xGemControl = new XGem300Control(); 

        public InfoCarrier(int nID)
        {
            m_nID = nID;
        }

        public void Init(string id, Log log, Wafer_Size.eSize eSize = Wafer_Size.eSize.Empty)
        {
            m_id = id;
            m_log = log;
            m_wafer = new Wafer_Size(eSize, m_log);
            for (int n = 0; n < c_lWafer; n++) m_infoWafer[n] = null;
        }

        int GetSlotNumber(int nID)
        {
            switch (m_eSlotType)
            {
                case eSlotType.Up: return nID + 1;
                case eSlotType.Down: return m_lWafer - nID;
            }
            return 0;
        }

        public void Clear()
        {
            for (int n = 0; n < c_lWafer; n++) m_infoWafer[n] = null; 
        }

        public void RunGrid(ezGrid rGrid, eGrid eMode)
        {
            m_eSlotType = (eSlotType)rGrid.Set((int)m_eSlotType, "Type", "Slot", "0 = Up, 1 = Down");
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

        public void SetExist(int nIndex, bool bExist)
        {
            if ((nIndex < 0) || (nIndex >= m_lWafer)) return;
            if (bExist == false)
            {
                m_infoWafer[nIndex] = null;
                return; 
            }
            else
            {
                m_infoWafer[nIndex] = new InfoWafer(m_id + (nIndex + 1).ToString("00"), nIndex, nIndex, m_log, m_wafer.m_eSize);
                m_infoWafer[nIndex].SetExist(bExist);
            }
        }

        public void XGemLoad(XGem300Pro_Mom xGem)
        {
            if (xGem == null) return;
            if (xGem.Is0nlineRemote() == false) return;
            xGem.SetCMSPresentSensor(this, true);
            xGem.CMSSetCarrierOnOff(this, true);
        }

        public void XGemUnload(XGem300Pro_Mom xGem)
        {
            if (xGem == null) return;
            if (xGem.Is0nlineRemote() == false) return;
            xGem.ProcessEnd(m_xGemProcess.m_sJobID);
            xGem.CMSSetReadyToLoad(this);
            xGem.SetCMSPresentSensor(this, false);
            xGem.CMSSetCarrierOnOff(this, false);
            m_xGemProcess.m_eJobState = XGem300Process.eJobState.Stopped;
            m_xGemControl.m_eState = XGem300Control.eState.Completed; 
        }

    }
}
