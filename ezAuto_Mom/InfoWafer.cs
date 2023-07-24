using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ezTools; 

namespace ezAutoMom
{
    public class InfoWafer
    {
        public enum eState
        {
            Empty,
            Exist,
            Select,
            Run,
            Done,
        };
        public eState m_eState = eState.Empty;

        public ArrayList m_aLocate = new ArrayList();
        public int m_nLocate = 0; 

        public string m_id;

        public string m_sWaferID = "";
        public string m_sFrameID = "";
        public string m_sLotID = "";
        public string m_sCarrierID = "CarrierID";

        public string m_sBoxID = "";
        public string m_sBoxState = "";

        public string m_sWTR = ""; 

        public int m_nID = -1;
        public int m_nSlot = -1;

        public bool m_bUseBCR = false;
        public double m_fAngleBCR = 0;
        public double m_fdRBCR = 0;
        public string m_sTypeBCR = "2D";
        public string m_sWaferIDType = "None";  

        public bool m_bUseOCR = false;
        public double m_fAngleOCR = 0;
        public double m_fdROCR = 0;
        public double m_fOCRReadScore = 170;    

        public bool m_bUseVision = true;
        public bool m_bUseBackSide = false;

        public bool m_bRunEdge = false;
        public bool m_bRunReview = false;

        public int m_nGood = 0;
        public int m_nTotal = 0;

        Log m_log;
        ezRegistry m_reg;

        public Wafer_Size m_wafer;

        public InfoWafer(string id, int nID, int nSlot, Log log, Wafer_Size.eSize eSize = Wafer_Size.eSize.Empty)
        {
            m_nID = nID;
            m_id = id + m_nID.ToString("_Wafer00"); 
            m_nSlot = nSlot; 
            m_log = log; 
            m_reg = m_log.m_reg; 
            m_eState = eState.Empty;
            m_wafer = new Wafer_Size(eSize,m_log); 
            ReadReg(true); 
        }

        public void ReadReg(bool bLocate, ezRegistry reg = null)
        {
            if (reg == null) reg = m_reg; 
            m_eState = (eState)reg.Read(m_id + "_State", (int)m_eState);
            reg.Read(m_id + "_BoxID", ref m_sBoxID);
            reg.Read(m_id + "_WaferID", ref m_sWaferID);
            reg.Read(m_id + "_FrameID", ref m_sFrameID);
            reg.Read(m_id + "_LotID", ref m_sLotID);
            reg.Read(m_id + "_nLocate", ref m_nLocate);
            if (bLocate)
            {
                int lLocate = 0;
                string str = "";
                m_aLocate.Clear();
                reg.Read(m_id + "_lLocate", ref lLocate);
                for (int n = 0; n < lLocate; n++)
                {
                    reg.Read(m_id + "_Locate" + n.ToString(), ref str);
                    m_aLocate.Add(str);
                }
            }
        }

        public void WriteReg(bool bLocate, ezRegistry reg = null)
        {
            if (reg == null) reg = m_reg; 
            reg.Write(m_id + "_State", (int)m_eState);
            reg.Write(m_id + "_BoxID", m_sBoxID);
            reg.Write(m_id + "_WaferID", m_sWaferID);
            reg.Write(m_id + "_FrameID", m_sFrameID);
            reg.Write(m_id + "_LotID", m_sLotID);
            reg.Write(m_id + "_nLocate", m_nLocate);
            if (bLocate)
            {
                reg.Write(m_id + "_lLocate", m_aLocate.Count);
                int n = 0;
                foreach (string str in m_aLocate)
                {
                    reg.Write(m_id + "_Locate" + n.ToString(), str);
                    n++;
                }
            }
        }

        public void SetExist(bool bExist)
        {
            if (m_eState == eState.Run) return;
            if (bExist)
            {
                m_eState = eState.Exist;
                WriteReg(false); 
            }
            else
            {
                m_eState = eState.Empty;
                m_nTotal = 0;
                SetUnknown(); 
                ClearLocate();
            }
        }

        public eState GetState()
        {
            return m_eState; 
        }

        public void SetState(eState nState)
        {
            m_log.Add(m_id + " State : " + m_eState.ToString() + " -> " + nState.ToString()); 
            m_eState = nState;
            WriteReg(false); 
        }

        public void SetNextLocate(string sLocNext)
        {
            if (m_aLocate.Count <= m_nLocate + 1)
            {
                m_log.Popup("InfoWafer SetNextLocate Error : Locate Count Over");
                Set1Locate();
                WriteReg(true);
                return; 
            }
            string[] sLocates = m_aLocate[m_nLocate + 1].ToString().Split(','); 
            if ((sLocates.Length <= 0) || (sLocNext != sLocates[0]))
            {
                m_log.Popup("InfoWafer SetNextLocate Error : Locate Miss Match");
                Set1Locate();
                WriteReg(true);
                return;
            }
            m_log.Add(m_id + " Locate : " + m_aLocate[m_nLocate].ToString() + " -> " + m_aLocate[m_nLocate + 1].ToString());
            m_nLocate++;
            WriteReg(false);
        }

        void Set1Locate()
        {
            if (m_aLocate.Count <= 0) return; 
            string sLocate1 = (string)m_aLocate[0];
            m_aLocate.Clear();
            m_aLocate.Add(sLocate1);
        }

        public bool IsFinish()
        {
            return m_nLocate == (m_aLocate.Count - 1);
        }

        public void SetWaferID(string sWaferID)
        {
            m_log.Add(m_id + " WaferID : " + m_sWaferID + " -> " + sWaferID);
            m_sWaferID = sWaferID;
            WriteReg(false); 
        }

        public void SetFrameID(string sFrameID)
        {
            m_log.Add(m_id + " FrameID : " + m_sFrameID + " -> " + sFrameID);
            m_sFrameID = sFrameID;
            WriteReg(false);
        }

        public void ClearLocate()
        {
            m_aLocate.Clear();
            m_nLocate = 0;
            WriteReg(true); 
        }

        public void AddLocate(string sLocate, bool bWrite = true)
        {
            m_aLocate.Add(sLocate);
            if (bWrite) WriteReg(true); 
        }

        public string GetLocate(int nNext)
        {
            if ((nNext + m_nLocate) >= m_aLocate.Count) return ""; 
            return (string)m_aLocate[nNext + m_nLocate]; 
        }

        public void SetAutoUnload() 
        {
            if (m_aLocate.Count <= 2) return; 
            m_nLocate = m_aLocate.Count - 2;
        }

        public bool HasChild(string sChild)
        {
            foreach (string str in m_aLocate)
            {
                string[] strs = str.Split(',');
                if (sChild == strs[0]) return true; 
            }
            return false; 
        }

        public bool IsUnknown()
        {
            if (m_sFrameID == "") return true;
//            if (m_sWaferID == "") return true; //forgetRS1000
            return false; 
        }

        public void SetUnknown()
        {
            m_sFrameID = "";
            m_sWaferID = "";
        }

    }
}
