using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ezTools;

namespace ezCam
{
    public class ezCam_Keyence_RS232
    {
        string m_id;
        Log m_log;

        bool m_bUse = false;

        const char c_cSTX = (char)0x02;
        const char c_cETX = (char)0x03;

        ezRS232 m_rs232 = null;

        const int c_lBuf = 1024;
        char[] m_aBuf = new char[c_lBuf]; 
        
        public void Init(string id, Log log, bool bUse)
        {
            m_id = id;
            m_log = log;
            m_bUse = bUse;
            m_rs232 = new ezRS232(m_id + "_RS232", m_log);
            m_rs232.CallMsgRcv += m_rs232_CallMsgRcv;
        }

        public void ThreadStop()
        {
            if (!m_bUse) return;
            m_rs232.ThreadStop();
        }

        public void RunGrid(ezGrid rGrid, eGrid eMode)
        {
            if (!m_bUse) return;
            m_rs232.RunGrid(rGrid, eMode); 
        }

        string m_sCode = "";
        string m_sMGS = "";
        void m_rs232_CallMsgRcv()
        {
            int nRead;
            nRead = m_rs232.Read(m_aBuf, c_lBuf);
            m_aBuf[nRead] = (char)0x00;
            for (int n = 0; n < nRead; n++)
            {
                switch (m_aBuf[n])
                {
                    case c_cSTX: m_sMGS = ""; break;
                    case c_cETX: m_sCode = m_sMGS; break;
                    default: m_sMGS += m_aBuf[n]; break; 
                }
            }
            m_log.Add(m_sMGS + " <--"); 
        }

        public string ReadCode()
        {
            Thread.Sleep(10);
            m_sCode = "";
            string str = "\x02LON\x03";
            m_log.Add(" --> LON");
            m_rs232.Write(str, false);
            WaitReply(1000, "Read Code Error !!");
            SensorOff(); 
            return m_sCode; 
        }

        void SensorOff()
        {
            Thread.Sleep(10);
            string str = "\x02LOFF\x03";
            m_log.Add(" --> LOFF");
            m_rs232.Write(str, false);
        }

        eHWResult WaitReply(int msDelay, string sMsg)
        {
            ezStopWatch sw = new ezStopWatch(); 
            if (m_rs232.IsConnect() == false)
            {
                m_log.Popup("RS232 not Connect !!");
                return eHWResult.Error;
            }
            while (m_sCode == "")
            {
                Thread.Sleep(10);
                if (sw.Check() > msDelay)
                {
                    m_log.Popup(sMsg);
                    return eHWResult.Error;
                }
            }
            return eHWResult.OK;
        }

    }
}
