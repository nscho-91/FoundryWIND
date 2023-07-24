using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ezTools; 

namespace ezAutoMom
{
    public class XGem300Data
    {
        public string m_id; 
        public int m_nID = -1;

        public XGem300Data(string id, int nID = -1)
        {
            m_id = id;
            m_nID = nID; 
        }

    }

    public class XGem300ALID
    {
        public string m_id; 
        public int m_nID = -1;
        public int m_nALID = -1;
        public int m_nALCD = -1; 
        public bool m_bSet = false;
        public string m_str;

        public XGem300ALID(string id, int nID, string str)
        {
            m_id = id;
            m_str = str; 
            m_nID = nID;
            m_nALID = nID; 
            m_bSet = false;
        }
    }

    public class XGem300Carrier
    {
        public enum ePortType
        {
            InOut = 1,
            Input,
            Output
        }
        public ePortType m_ePortType = ePortType.InOut; 

        public enum eCarrierType
        {
            P_Foup = 1,
            Cassete,
            Magazine,
            Tray,
            FOSB
        }
        public eCarrierType m_eCarrierType = eCarrierType.Cassete; 

        public enum eLPAvailable
        {
            Empty = 0,
            Full = 1
        }

        public enum eLPTransfer
        {
            OutOfService = 0,
            TransferBlocked,
            ReadyToLoad,
            ReadyToUnload
        }
        public eLPTransfer m_eLPTransfer = eLPTransfer.OutOfService; 

        public enum ePortAccess
        {
            Manual = 0,
            Auto
        }
        public ePortAccess m_ePortAccess = ePortAccess.Manual;

        public enum ePortReservationState
        {
            NotReserved = 0,
            Reserved,
        }
        public ePortReservationState m_eReservationState = ePortReservationState.NotReserved;

        public enum ePortAssocitionState
        {
            NotAssociated = 0,
            Associated,
        }
        public ePortAssocitionState m_eAssociationState = ePortAssocitionState.NotAssociated;
        
        public enum eCarrierAssociationState
        {
            NOT_ACCESSED = 0,
            IN_ACCESSED,
            CARRIER_COMPLETED,
            CARRIER_STOPPED,
        }
        public eCarrierAssociationState m_eCarrierAssociationState = eCarrierAssociationState.NOT_ACCESSED;

        public enum eCarrierState
        {
            NotRead,
            WaitForHost,
            VerrificationOK,
            VerrificationFailed
        }
        public eCarrierState m_eCarrierIDState = eCarrierState.NotRead;
        public eCarrierState m_eCarrierSlotMapState = eCarrierState.NotRead; 

        public enum eProduct
        {
            Quad = 1,
            Panel,
            Wafer
        }
        public eProduct m_eProduct = eProduct.Panel;

        public string m_sLocID = "";
        public string m_sCarrierID = "";
        public string m_sLotID = "";
        public string m_sSlotMap = ""; 

        public XGem300Carrier(string sLocID)
        {
            m_sLocID = sLocID; 
        }

        public eLPAvailable GetLPAvailableState()
        {
            switch (m_eLPTransfer)
            {
                case eLPTransfer.ReadyToLoad: return eLPAvailable.Empty;
                case eLPTransfer.OutOfService:
                case eLPTransfer.TransferBlocked:
                case eLPTransfer.ReadyToUnload:
                default: return eLPAvailable.Full;
            }
        }

    }

    public class XGem300Process
    {
        public enum eFlag
        {
            N = 0,
            Y
        }
        public eFlag m_ePreRunFlag = eFlag.N; 
        public eFlag m_eAutoStart = eFlag.N; 

        public enum eJobState
        {
            Queued = 0,
            SettingUp,
            WaitingForStart,
            Processing,
            ProcessingComplete,
            Reserved,
            Pausing,
            Paused,
            Stopping,
            Aborting,
            Stopped,
            Aborted,
            JobCanceled,
            JobComplete,
        }
        public eJobState m_eJobState = eJobState.Stopped;

        public enum eError
        {
            NO_ERROR = 0,
            Unkown_Object,
            Unkown_TargetObjuctType,
            Unkown_ObjectInstance,
            Unkown_AttributeName,
            ReadOnly_AccessDenied,
            Unkown_ObjectType,
            Invalid_AttibuteValue,
            Syntax_Error,
            Verifacation_Error,
            Validataion_Error,
            ObjectIdentifier_InUse,
            ParametersImproperlySpecified,
            InsufficientParameters,
            Unsupported_Option,
            Busy,
        }

        public enum eCommand
        {
            START = 1,
            PAUSE,
            RESUME,
            STOP,
            ABORT,
            CANCEL,
        }

        public string m_sJobID = "";
        public string m_sRecipeID = "";
        public string m_sSlotMap = "";
        public string m_sSlotMapInsp = "";
        public string[] m_sPanelID = new string[25];
        public string[] m_sSlotNo = new string[25];

        public XGem300Process()
        {
            for (int n = 0; n < 25; n++)
            {
                m_sPanelID[n] = "";
                m_sSlotNo[n] = "";
            }
        }

    }

    public class XGem300Control
    {
        public enum eState
        {
            Queued = 0,
            Selected,
            WaitingForStart,
            Excuting,
            Paused,
            Completed,
        }
        public eState m_eState = eState.Queued;

        public string m_sJobID = ""; 
    }

}
