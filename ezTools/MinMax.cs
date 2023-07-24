using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyGridEx;

namespace ezTools
{
    public struct MinMax : ezGrid_Object
    {
        public int m_nMin, m_nMax;
        Log m_log; 

        public MinMax(int nMin, int nMax, Log log)
        {
            m_nMin = nMin; m_nMax = nMax; m_log = log;
        }

        public int Min
        {
            get { return m_nMin; }
            set { m_nMin = value; }
        }

        public int Max
        {
            get { return m_nMax; }
            set { m_nMax = value; }
        }

        public override int GetHashCode()
        {
            return m_nMin.GetHashCode() ^ m_nMax.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (m_nMin != ((MinMax)obj).m_nMin) return false;
            if (m_nMax != ((MinMax)obj).m_nMax) return false;
            return true;
        }

        public override string ToString()
        {
            return "(" + m_nMin.ToString() + ", " + m_nMax.ToString() + ")";
        }

        public int Range()
        {
            return m_nMax - m_nMin; 
        }

        public int Center()
        {
            return (m_nMin + m_nMax) / 2;
        }

        public void JobOpen(ezJob job, string strGroup, string strID)
        {
            job.Read(strGroup, strID + ".nMin", ref m_nMin);
            job.Read(strGroup, strID + ".nMax", ref m_nMax); 
        }

        public void JobSave(ezJob job, string strGroup, string strID)
        {
            job.Write(strGroup, strID + ".nMin", m_nMin);
            job.Write(strGroup, strID + ".nMax", m_nMax); 
        }

        public void RegRead(ezRegistry reg, string strID)
        {
            reg.Read(strID + ".nMin", ref m_nMin);
            reg.Read(strID + ".nMax", ref m_nMax);
        }

        public void RegWrite(ezRegistry reg, string strID)
        {
            reg.Write(strID + ".nMin", m_nMin);
            reg.Write(strID + ".nMax", m_nMax);
        }

        public void Update(string strID, object objOld, object objNew)
        {
            m_nMin = ((MinMax)objNew).m_nMin;
            m_nMax = ((MinMax)objNew).m_nMax;
            if (m_log == null) return; 
            m_log.Add(strID + " : " + objOld.ToString() + " -> " + ((MinMax)objNew).ToString());
        }

        public BrowsableTypeConverter.LabelStyle GetBrowsableType()
        {
            return BrowsableTypeConverter.LabelStyle.lsNormal; 
        }

    }
}
