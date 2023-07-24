using System;
using System.Collections; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using ezTools; 

namespace ezAutoMom
{
    public class Info_Wafer
    {
        public enum eState 
        { 
            Empty, 
            Exist, 
            Select, 
            Run, 
            Done,
            Bad
        };
        eState m_eState = eState.Empty;

        public enum eLocate 
        { 
            LoadPort,
            WTR_Lower,
            WTR_Upper, 
            Aligner,
            Vision,
            BackSide,
            LoaderModule,
        };
        eLocate m_eLocate = eLocate.LoadPort;
        eLocate m_eLocLast = eLocate.LoadPort;

        public enum eAngleRF
        {
            R0,
            R90,
            R180,
            R270,
        }
        public eAngleRF m_eAngleRF = eAngleRF.R0; 

        public enum eProc
        {
            Rotate0,
            Rotate90,
            Rotate180,
            Rotate270,
            BCR, // ing 170626
            Align,
            Vision,
            BackSide,
            LoadPort
        }
        public eProc m_eProcCurrent = eProc.LoadPort;
        public eProc m_eProcNext = eProc.LoadPort;
        public Queue m_queProc = new Queue();

        public string m_sRecipe = ""; 
        public string m_strWaferID = "";
        public string m_strFrameID = "";
        public string m_strLotID = "";
        public string m_idMaterial = "";  
        public string m_strCarrierID = "CarrierID"; 
        public int m_nLoadPort = -1;
        public int m_nID = -1;
        public int m_nSlot = -1; 
        public double m_fAngleAlign = 0;

        public bool m_bUseBCR = false;
        public double m_fAngleBCR = 0;
        public double m_fdRBCR = 0;
        public string m_sTypeBCR = "2D";
        public string m_sWaferIDType = "None";  // ing 161214 For Reading RingFrame WaferID in MSB

        public bool m_bUseOCR = false;
        public double m_fAngleOCR = 0;
        public double m_fdROCR = 0;
        public double m_fOCRReadScore = 170;    //KDG 161025 Add OCR Score

        public double m_fAngleQR = 0;
        public double m_fdRQR = 0;
        public double m_fQRReadScore = 170;    //KDG 161025 Add OCR Score

        public bool m_bUseAligner = false; // ing 170531
        public bool m_bRotate180 = false; // ing 170531
        public bool m_bUseVision = true;
        public bool m_bUseBackSide = false;

        public bool m_bRunEdge = false;
        public bool m_bRunReview = false;
        public bool m_bOnlyInkMark = false;

        public bool m_bPrealignSucc = false;
        public int m_nGood = 0;
        public int m_nTotal = 0;
        public int m_nProgressCurrent = 0;
        public int m_nProgressTotal = 0;
        public int m_nDefectCount = 0;
        public bool m_bSendLotstartsigaal = false;
        public string m_sLotStartID = "";

        public bool m_bVSAlignFail = false; 
        public bool m_bPreAlignfail = false; 

        public int m_nOCRDir = 1;
        public bool m_bDirectionNotch = false;
        public double m_fAngleChar = 5;
        public bool m_bRunWait = false;

        public RPoint m_dpWaferShift = new RPoint(); // BHJ 191128 add
        public RPoint m_rtWaferShift = new RPoint(); // BHJ 191128 add

        Log m_log;
        ezRegistry m_reg;
        Stopwatch m_swVision = new Stopwatch();

        public Wafer_Size m_wafer; 

        public TimeSpan m_timeInsp = new TimeSpan(00, 00, 00);

        public Info_Wafer(int nLoadPort, int nID, int nSlot, Log log, Wafer_Size.eSize eSize = Wafer_Size.eSize.Empty)
        {
            m_nLoadPort = nLoadPort; 
            m_nID = nID; 
            m_nSlot = nSlot; 
            m_log = log; 
            m_reg = m_log.m_reg; 
            m_eState = eState.Empty;
            m_wafer = new Wafer_Size(eSize,m_log);
            m_bRunWait = false;
            ReadReg(); 
        }

        public string m_id
        {
            get
            {
                return m_nLoadPort.ToString("LoadPort0_") + m_nID.ToString("Wafer00");
            }
            set
            {
            }
        }

        public void SetEmpty()
        {
            if (m_eLocate != eLocate.LoadPort) return; 
            State = eState.Empty;
            m_eLocate = eLocate.LoadPort;
            m_eLocLast = eLocate.LoadPort;
            m_eAngleRF = eAngleRF.R0;
            m_bVSAlignFail = false; 
            m_bPreAlignfail = false; 
            m_swVision.Reset();
        }

        public bool IsExist()
        {
            return m_eState == eState.Exist;
        }

        public void SetExist(bool bExist)
        {
            if (!bExist)
            {
                SetEmpty();
            }
            else if (State == eState.Empty || State == eState.Done || State == eState.Run || State == eState.Select) //KJW
            {
                m_bRunWait = false;
                State = eState.Exist;
            }
            m_swVision.Reset();
            m_bVSAlignFail = false; 
            m_bPreAlignfail = false; 
        }
        
        void ReadReg()
        {
            m_eState = (eState)m_reg.Read(m_id + "_State", (int)m_eState);
            m_eLocate = (eLocate)m_reg.Read(m_id + "_Locate", (int)m_eLocate);
            m_reg.Read(m_id + "_WaferID", ref m_strWaferID);
            m_eAngleRF = (eAngleRF)m_reg.Read(m_id + "_eRF180", (int)m_eAngleRF); 
        }

        void WriteReg()
        {
            m_reg.Write(m_id + "_State", (int)m_eState);
            m_reg.Write(m_id + "_Locate", (int)m_eLocate);
            m_reg.Write(m_id + "_WaferID", m_strWaferID);
            m_reg.Write(m_id + "_bAngleRF180", (int)m_eAngleRF);
        }

        public eState State
        {
            get 
            { 
                return m_eState; 
            }
            set 
            { 
                m_eState = value;
                WriteReg(); 
            }
        }

        public eLocate Locate
        {
            get { return m_eLocate; }
            set
            {
                m_eLocLast = m_eLocate;
                m_eLocate = value;
                if ((m_eLocate != eLocate.WTR_Lower) && (m_eLocate != eLocate.WTR_Upper) && m_eLocate != m_eLocLast) // ing 170401 같은 모듈내에서 중복 Deque제거
                {
                    if (m_queProc.Count > 0)
                    {
                        m_eProcCurrent = m_eProcNext;
                        m_eProcNext = (eProc)m_queProc.Dequeue();
                    }
                    else
                    {
                        m_eProcNext = eProc.LoadPort;
                    }
                    m_log.Add(m_strWaferID + " : ProcCurrent -> " + m_eProcCurrent.ToString()); 
                    m_log.Add(m_strWaferID + " : ProcNext -> " + m_eProcNext.ToString()); 
                }
                WriteReg();
                if (m_eLocate == eLocate.Vision) m_swVision.Restart();
                else m_swVision.Stop();
            }
        }

        public eLocate LocLast
        {
            get { return m_eLocLast; }
            set { }
        }

        public string WAFERID
        {
            get { return m_strWaferID; }
            set
            {
                m_strWaferID = value;
                WriteReg();
            }
        }

        public string GetVSTime()
        {
            int nSec = (int)(m_swVision.ElapsedMilliseconds / 1000);
            return nSec.ToString();
        }

        public bool IsEnableRecover()
        {
            return (State == eState.Empty) || (State == eState.Run); 
        }

        public override string ToString()
        {
            return m_nLoadPort.ToString() + "," + m_wafer.ToString() + "," + m_nSlot.ToString() + "," + m_strWaferID;
        }
        
    }
}
