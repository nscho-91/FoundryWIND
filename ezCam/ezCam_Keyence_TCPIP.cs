using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using ezTools;

namespace ezCam
{
    public class ezCam_Keyence_TCPIP
    {
        string m_id;
        Log m_log;

        bool m_bUse = false;

        ezTCPSocket m_socket;

        public void Init(string id, Log log, bool bUse)
        {
            m_id = id;
            m_log = log;
            m_bUse = bUse;
            m_socket = new ezTCPSocket(m_id, m_log, false);
            m_socket.Init();
            m_socket.CallMsgRcv += CallMsgRcv;
        }

        public void ThreadStop()
        {
            if (!m_bUse) return;
            m_socket.ThreadStop();
        }

        public void RunGrid(ezGrid rGrid, eGrid eMode)
        {
            if (!m_bUse) return;
            m_socket.RunGrid(rGrid);
        }

        public bool IsConnect()
        {
            bool bConnect = m_socket.IsConnect();
            if (bConnect == false) m_log.Popup("TCP/IP not Connect !!");
            return bConnect;
        }

        const byte c_cSTX = (byte)0x02;
        const byte c_cETX = (byte)0x03;

        string m_sCode = "";
        void CallMsgRcv(byte[] buf, int nSize)
        {
            string sMsg = Encoding.Default.GetString(buf, 0, nSize - 1);
            if ((sMsg != "OK,LON") && (sMsg != "OK,LOFF")) m_sCode = sMsg; 
            m_log.Add("<== " + sMsg);
        }

        public string ReadCode()
        {
            Thread.Sleep(10);
            m_log.Add("==> LON");
            m_sCode = "";
            m_socket.Send("LON\r"); 
            WaitReply(1000, "Read Code Error !!");
            SensorOff();
            return m_sCode;
        }

        void SensorOff()
        {
            if (m_sCode != "") return; 
            Thread.Sleep(10);
            m_socket.Send("LOFF\r");
        }

        eHWResult WaitReply(int msDelay, string sMsg)
        {
            ezStopWatch sw = new ezStopWatch(); 
            if (m_socket.IsConnect() == false)
            {
                m_log.Popup("TCPIP not Connect !!");
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

        public eTCPResult ReadImage(ezImg img, string sFTP, string sFile)
        {
            try
            {
                WebClient ftp = new WebClient();
                ftp.Credentials = new NetworkCredential("admin", "admin");
                ftp.DownloadFile("ftp://" + m_socket.m_strIP + sFTP, sFile);
                Thread.Sleep(100);
                img.FileOpen(sFile);
            }
            catch (Exception e)
            {
                m_log.Popup("Read Image Error !!");
                m_log.Add(e.ToString());
            }
            return eTCPResult.OK;
        }
    }
}
