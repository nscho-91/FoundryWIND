using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyGridEx;

namespace ezTools
{
    public class Wafer_Size
    {

        public enum eSize
        {
            inch4,
            inch5,
            inch6,
            mm200,
            mm300,
            mm300_RF,
            inch8,
            Error,
            Empty
        };
        public eSize m_eSize;
        public string size_RAC = "";
        string[] m_strSizes = new string[(int)eSize.Empty]; 

        public bool[] m_bEnable = new bool[(int)eSize.Empty];
        public bool[] m_bLPMapping = new bool[(int)eSize.Empty];
        public int[] m_lWafer = new int[(int)eSize.Empty];
        int[] m_nTeach = new int[(int)eSize.Empty]; 

        Log m_log;

        public Wafer_Size(Log log)
        {
            m_log = log;
            Init(); 
        }

        public Wafer_Size(eSize size, Log log)
        {
            m_eSize = size;
            m_log = log;
            Init(); 
        }

        void Init()
        {
            for (int n = 0; n< (int)eSize.Empty; n++)
            {
                m_bEnable[n] = false;
                m_bLPMapping[n] = true;
                m_lWafer[n] = 25; 
                m_strSizes[n] = ((eSize)n).ToString(); 
            }
        }

        public override int GetHashCode()
        {
            return m_eSize.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (m_eSize != ((Wafer_Size)obj).m_eSize) return false;
            return true;
        }

        public override string ToString()
        {
            return m_eSize.ToString();
        }

        public void RunGrid(bool bRAC, ezGrid grid, eGrid mode, string strGroup, string strProp, ezJob job = null)
        {

            string strSize = "";

            if (bRAC) strSize = size_RAC;
            else strSize = m_eSize.ToString();

            grid.Set(ref strSize, m_strSizes, strGroup, strProp, "Wafer Size");
            for (int n = 0; n < (int)eSize.Empty; n++)
            {
                if (strSize == ((eSize)n).ToString())
                    m_eSize = (eSize)n;
            }
        }

        public void AddEnable(Wafer_Size type)
        {
            if (type == null) return;
            for (int n = 0; n < (int)(eSize.Empty); n++)
            {
                m_bEnable[n] |= type.m_bEnable[n];
            }
        }

        public void InitString()
        {
            int nCount = 0;
            for (int n = 0; n < (int)eSize.Empty; n++) if (m_bEnable[n]) nCount++;
            m_strSizes = new string[nCount];
            nCount = 0;
            bool bOK = false;
            for (int n = 0; n < (int)eSize.Empty; n++) if (m_bEnable[n])
            {
                m_strSizes[nCount] = ((eSize)n).ToString();
                if (n == (int)m_eSize) bOK = true; 
                nCount++; 
            }
            if (bOK) return; 
            for (int n = 0; n < (int)eSize.Empty; n++) if (m_bEnable[n])
            {
                m_eSize = (eSize)n;
                return; 
            }
        }

        public bool IsRingFrame()
        {
            if (m_eSize == eSize.mm300_RF) return true;
            return false;
        }

        public bool Is300mmWafer()  //KDG 161006 Add
        {
            if (m_eSize == eSize.mm300) return true;
            return false;
        }

        public bool Is6InchWafer()
        {
            if (m_eSize == eSize.inch6) return true;
            return false;
        }

        public bool Is8InchWafer()
        {
            if (m_eSize == eSize.inch8) return true;
            return false;
        }

        public bool IsLPMapping()
        {
            return m_bLPMapping[(int)m_eSize]; 
        }

        public int GetWaferCount()
        {
            if (m_eSize >= eSize.Empty) return 25; 
            return m_lWafer[(int)m_eSize]; 
        }

        public int GetTeachID()
        {
            return m_nTeach[(int)m_eSize]; 
        }

        public void RunChildGrid(ezGrid rGrid, eGrid eMode, string id)
        {
            for (int n = 0; n < (int)eSize.Empty; n++)
            {
                if (m_bEnable[n]) rGrid.Set(ref m_nTeach[n], id + "Teach", ((eSize)n).ToString(), "WTR Teach ID");
            }
        }
    }
}
