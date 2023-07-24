using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ezTools; 

namespace ezAxis
{
    public class Type
    {
        public enum eType
        {
            UC, 
            EIP
        }
        public eType m_eType = eType.UC;
        string[] m_strTypes = Enum.GetNames(typeof(eType));

        public string m_id; 
        public int m_nAxis = -1; 

        public void RunGrid(ezGrid grid, string strGroup, string strProp, string strDesc)
        {
            grid.Set(ref m_nAxis, strGroup, strProp, strDesc);
            string strType = m_eType.ToString(); 
            grid.Set(ref strType, m_strTypes, strGroup, strProp + "_eType", strDesc);
                m_eType = GetType(strType);
            }

        public void RunGridIO(ezGrid grid, string strGroup, string strProp, string strDesc)
        {
            string strType = m_eType.ToString();
            grid.Set(ref strType, m_strTypes, strGroup, strProp, strDesc);
            m_eType = GetType(strType);
        }

        public bool IsUC()
        {
            return m_eType == eType.UC; 
        }

        eType GetType(string strType)
        {
            if (strType == "UC") return eType.UC;
            else return eType.EIP;
        }

    }
}
