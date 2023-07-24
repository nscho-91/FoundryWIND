using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using PropertyGridEx;
using ezTools;

namespace ezTools
{
    public class LaserParam : ezGrid_Object
    {
        protected string m_id;
        protected uint m_nScanID;
        protected Log m_log = null; 
        public uint[] m_nPulse = new uint[2] { 80, 100 };
        public uint[] m_nDelay = new uint[4] { 250, 100, 50, 100 };
        public int m_nDelayLaserOn = 50; 
	    public double m_fPower = 70;
        double[] m_fSpeed = new double[2] { 500, 100 };
 
        public LaserParam()
        {
        }

        public void Init(string id, uint nScanID, Log log) 
        {
            m_id = id; m_nScanID = nScanID; m_log = log; 
        }

        public void Set(LaserParam lpm)
        {
            int n;
            for (n = 0; n < 2; n++)
            {
                m_fSpeed[n] = lpm.m_fSpeed[n];
                m_nPulse[n] = lpm.m_nPulse[n];
            }
            for (n = 0; n < 4; n++)
            {
                m_nDelay[n] = lpm.m_nDelay[n];
            }
            m_nDelayLaserOn = lpm.m_nDelayLaserOn; 
        }

        public uint Pulse_Freq
        {
            get { return m_nPulse[0]; }
            set { m_nPulse[0] = value; }
        }

        public double Pulse_Power
        {
            get { return m_fPower; }
            set { m_fPower = value; }
        }

        public uint Pulse_FPS
        {
            get { return m_nPulse[1]; }
            set { m_nPulse[1] = value; }
        }

        public double V_Jump
        {
            get { return m_fSpeed[0]; }
            set { m_fSpeed[0] = value; }
        }

        public double V_Mark
        {
            get { return m_fSpeed[1]; }
            set { m_fSpeed[1] = value; }
        }

        public uint Delay_Jump
        {
            get { return m_nDelay[0]; }
            set { m_nDelay[0] = value; }
        }

        public uint Delay_Mark
        {
            get { return m_nDelay[1]; }
            set { m_nDelay[1] = value; }
        }

        public uint Delay_Poligon
        {
            get { return m_nDelay[2]; }
            set { m_nDelay[2] = value; }
        }

        public int Delay_LaserOn
        {
            get { return m_nDelayLaserOn; }
            set { m_nDelayLaserOn = value; }
        }

        public uint Delay_LaserOff
        {
            get { return m_nDelay[3]; }
            set { m_nDelay[3] = value; }
        }

        public override int GetHashCode()
        {
            return m_nPulse[0].GetHashCode()
                ^ m_nPulse[1].GetHashCode()
                ^ m_fPower.GetHashCode()
                ^ m_fSpeed[0].GetHashCode()
                ^ m_fSpeed[1].GetHashCode()
                ^ m_nDelay[0].GetHashCode()
                ^ m_nDelay[1].GetHashCode()
                ^ m_nDelay[2].GetHashCode()
                ^ m_nDelay[3].GetHashCode()
                ^ m_nDelayLaserOn.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (m_nPulse[0] != ((LaserParam)obj).m_nPulse[0]) return false;
            if (m_nPulse[1] != ((LaserParam)obj).m_nPulse[1]) return false;
            if (m_fPower != ((LaserParam)obj).m_fPower) return false;
            if (m_fSpeed[0] != ((LaserParam)obj).m_fSpeed[0]) return false;
            if (m_fSpeed[1] != ((LaserParam)obj).m_fSpeed[1]) return false;
            if (m_nDelay[0] != ((LaserParam)obj).m_nDelay[0]) return false;
            if (m_nDelay[1] != ((LaserParam)obj).m_nDelay[1]) return false;
            if (m_nDelay[2] != ((LaserParam)obj).m_nDelay[2]) return false;
            if (m_nDelay[3] != ((LaserParam)obj).m_nDelay[3]) return false;
            if (m_nDelayLaserOn != ((LaserParam)obj).m_nDelayLaserOn) return false;
            return true;
        }

        public override string ToString()
        {
            return m_nPulse[0].ToString() + ","
                + m_fPower.ToString("0.00") + ","
                + m_nPulse[1].ToString() + ","
                + m_fSpeed[0].ToString("0.00") + ","
                + m_fSpeed[1].ToString("0.00") + ","
                + m_nDelay[0].ToString() + ","
                + m_nDelay[1].ToString() + ","
                + m_nDelay[2].ToString() + ","
                + m_nDelayLaserOn.ToString() + ","
                + m_nDelay[3].ToString();
        }

        public void JobOpen(ezJob job, string strGroup, string strID)
        {
            job.Read(strGroup, strID + ".Pulse_Freq", ref m_nPulse[0]);
            job.Read(strGroup, strID + ".Pulse_Power", ref m_fPower);
            job.Read(strGroup, strID + ".Pulse_FPS", ref m_nPulse[1]);
            job.Read(strGroup, strID + ".V_Jump", ref m_fSpeed[0]);
            job.Read(strGroup, strID + ".V_Mark", ref m_fSpeed[1]);
            job.Read(strGroup, strID + ".Delay_Jump", ref m_nDelay[0]);
            job.Read(strGroup, strID + ".Delay_Mark", ref m_nDelay[1]);
            job.Read(strGroup, strID + ".Delay_Poligon", ref m_nDelay[2]);
            job.Read(strGroup, strID + ".Delay_LaserOn", ref m_nDelayLaserOn);
            job.Read(strGroup, strID + ".Delay_LaserOff", ref m_nDelay[3]);
        }

        public void JobSave(ezJob job, string strGroup, string strID)
        {
            job.Write(strGroup, strID + ".Pulse_Freq", m_nPulse[0]);
            job.Write(strGroup, strID + ".Pulse_Power", m_fPower);
            job.Write(strGroup, strID + ".Pulse_FPS", m_nPulse[1]);
            job.Write(strGroup, strID + ".V_Jump", m_fSpeed[0]);
            job.Write(strGroup, strID + ".V_Mark", m_fSpeed[1]);
            job.Write(strGroup, strID + ".Delay_Jump", m_nDelay[0]);
            job.Write(strGroup, strID + ".Delay_Mark", m_nDelay[1]);
            job.Write(strGroup, strID + ".Delay_Poligon", m_nDelay[2]);
            job.Write(strGroup, strID + ".Delay_LaserOn", m_nDelayLaserOn);
            job.Write(strGroup, strID + ".Delay_LaserOff", m_nDelay[3]);
        }

        public void RegRead(ezRegistry reg, string strID)
        {
            reg.Read(strID + ".Pulse_Freq", ref m_nPulse[0]);
            reg.Read(strID + ".Pulse_Power", ref m_fPower);
            reg.Read(strID + ".Pulse_FPS", ref m_nPulse[1]);
            reg.Read(strID + ".V_Jump", ref m_fSpeed[0]);
            reg.Read(strID + ".V_Mark", ref m_fSpeed[1]);
            reg.Read(strID + ".Delay_Jump", ref m_nDelay[0]);
            reg.Read(strID + ".Delay_Mark", ref m_nDelay[1]);
            reg.Read(strID + ".Delay_Poligon", ref m_nDelay[2]);
            reg.Read(strID + ".Delay_LaserOn", ref m_nDelayLaserOn);
            reg.Read(strID + ".Delay_LaserOff", ref m_nDelay[3]);
        }

        public void RegWrite(ezRegistry reg, string strID)
        {
            reg.Write(strID + ".Pulse_Freq", m_nPulse[0]);
            reg.Write(strID + ".Pulse_Power", m_fPower);
            reg.Write(strID + ".Pulse_FPS", m_nPulse[1]);
            reg.Write(strID + ".V_Jump", m_fSpeed[0]);
            reg.Write(strID + ".V_Mark", m_fSpeed[1]);
            reg.Write(strID + ".Delay_Jump", m_nDelay[0]);
            reg.Write(strID + ".Delay_Mark", m_nDelay[1]);
            reg.Write(strID + ".Delay_Poligon", m_nDelay[2]);
            reg.Write(strID + ".Delay_LaserOn", m_nDelayLaserOn);
            reg.Write(strID + ".Delay_LaserOff", m_nDelay[3]);
        }

        public void Update(string strID, object objOld, object objNew)
        {
            m_nPulse[0] = ((LaserParam)objNew).m_nPulse[0];
            if (m_nPulse[0] < 10)
            {
                m_nPulse[0] = 10;
                m_log.Popup("Frequency range over !!");
            }
            if (m_nPulse[0] > 200)
            {
                m_nPulse[0] = 200;
                m_log.Popup("Frequency range over !!");
            }
            m_fPower = ((LaserParam)objNew).m_fPower;
            if (m_fPower < 0)
            {
                m_fPower = 0;
                m_log.Popup("Power range over !!");
            }
            if (m_fPower > 100)
            {
                m_fPower = 100;
                m_log.Popup("Power range over !!");
            }
            m_nPulse[1] = ((LaserParam)objNew).m_nPulse[1];
            m_fSpeed[0] = ((LaserParam)objNew).m_fSpeed[0];
            m_fSpeed[1] = ((LaserParam)objNew).m_fSpeed[1];
            m_nDelay[0] = ((LaserParam)objNew).m_nDelay[0];
            m_nDelay[1] = ((LaserParam)objNew).m_nDelay[1];
            m_nDelay[2] = ((LaserParam)objNew).m_nDelay[2];
            m_nDelayLaserOn = ((LaserParam)objNew).m_nDelayLaserOn;
            m_nDelay[3] = ((LaserParam)objNew).m_nDelay[3];
            if (m_nDelayLaserOn > m_nDelay[3])
            {
                m_nDelayLaserOn = (int)m_nDelay[3];
                m_log.Popup("LaserOn delay shuld be low than off delay !!");
            }
            m_log.Add(strID + " : " + objOld.ToString() + " -> " + ((LaserParam)objNew).ToString());
        }

        public BrowsableTypeConverter.LabelStyle GetBrowsableType()
        {
            return BrowsableTypeConverter.LabelStyle.lsTypeName;
        }

    }
}
