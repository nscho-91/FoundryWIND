using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyGridEx;

namespace ezTools
{
    public struct Light_Data : ezGrid_Object
    {
        public string m_id;
        public bool m_bUse;
        Log m_log;

        public Light_Data(int nID, bool bUse, Log log)
        {
            m_bUse = bUse; m_log = log;
            m_id = "Ch_" + nID.ToString();
        }

        public string ID
        {
            get { return m_id; }
            set { m_id = value; }
        }

        public bool bUse
        {
            get { return m_bUse; }
            set { m_bUse = value; }
        }

        public override int GetHashCode()
        {
            return m_id.GetHashCode() ^ m_bUse.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (m_id != ((Light_Data)obj).m_id) return false;
            if (m_bUse != ((Light_Data)obj).m_bUse) return false;
            return true;
        }

        public override string ToString()
        {
            return m_id + ", " + m_bUse.ToString();
        }

        public void JobOpen(ezJob job, string strGroup, string strID)
        {
            job.Read(strGroup, strID + ".id", ref m_id);
            job.Read(strGroup, strID + ".bUse", ref m_bUse);
        }

        public void JobSave(ezJob job, string strGroup, string strID)
        {
            job.Write(strGroup, strID + ".id", m_id);
            job.Write(strGroup, strID + ".bUse", m_bUse);
        }

        public void RegRead(ezRegistry reg, string strID)
        {
            reg.Read(strID + ".id", ref m_id);
            reg.Read(strID + ".bUse", ref m_bUse);
        }

        public void RegWrite(ezRegistry reg, string strID)
        {
            reg.Write(strID + ".id", m_id);
            reg.Write(strID + ".bUse", m_bUse);
        }

        public void Update(string strID, object objOld, object objNew)
        {
            m_id = ((Light_Data)objNew).m_id;
            m_bUse = ((Light_Data)objNew).m_bUse;
            m_log.Add(strID + " : " + objOld.ToString() + " -> " + ((Light_Data)objNew).ToString());
        }

        public BrowsableTypeConverter.LabelStyle GetBrowsableType()
        {
            return BrowsableTypeConverter.LabelStyle.lsNormal;
        }

    }

}
