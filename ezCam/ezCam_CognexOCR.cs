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
    public class ezCam_CognexOCR
    {
        enum eCmd
        {

        }

        string m_id;
        Log m_log; 

        ezTCPSocket m_socket;
        bool m_bSendCmd = false;
        byte[] m_bufSend = new byte[4096];

        public string m_strOCR = "";
        public double m_scoreOCR = 0.0;
        public bool bOCRDone = false;
        public bool m_bUse = false;

        public ezCam_CognexOCR()
        {
        }

        public void Init(string id, Log log, bool bUse)
        {
            m_id = id;
            m_log = log;
            m_bUse = bUse;
            if (m_bUse)
            {
                m_socket = new ezTCPSocket(m_id, m_log, false);
            }
        }

        public void Connect()
        {
            if (m_bUse)
            {
                m_socket.Init();
                m_socket.CallMsgRcv += CallMsgRcv;
            }
        }

        public void ThreadStop()
        {
            if (m_socket != null) m_socket.ThreadStop(); 
        }

        public void RunGrid(ezGrid rGrid)
        {
            if(m_bUse)
                m_socket.RunGrid(rGrid); 
        }

        public bool CheckInitOK()
        {
            if (!m_socket.IsConnect())
            {
                m_log.Popup("TCPIP not Connected !!");
                return false;
            }
            return true;
        }

        public eTCPResult ReadOCR(ezImg img)
        {
            m_strOCR = "";  //KDG 161028 Add OCR Data Init
            File.Delete("c:\\TestImg\\OCR.bmp"); 
            eTCPResult eResult = SendCmd("READ(-1)");
            if (eResult != eTCPResult.OK) return eResult;
            eResult = WaitReply(1000);
            if (eResult != eTCPResult.OK) return eResult;
            if (img == null) return eTCPResult.OK;
            Thread.Sleep(200);
            return ReadImage(img); 
        }

        eTCPResult ReadImage(ezImg img)
        {
            try
            {
                WebClient ftp = new WebClient();
                ftp.Credentials = new NetworkCredential("admin", "");
                //Uri URL = new Uri("ftp://" + m_socket.m_strIP + "/image.bmp");
                ftp.DownloadFile("ftp://" + m_socket.m_strIP + "/image.bmp", "c:\\TestImg\\OCR.bmp"); 
                Thread.Sleep(100);
                img.FileOpen("c:\\TestImg\\OCR.bmp");
            }
            catch (Exception e)
            {
                m_log.Popup("Read Image Error !!");
                m_log.Add(e.ToString()); 
            }
            return eTCPResult.OK; 
        }

        eTCPResult SendCmd(string strCmd)
        {
            int lBuf = 0;
            ezStopWatch sw = new ezStopWatch();
            while (m_bSendCmd && (sw.Check() < 2000)) Thread.Sleep(10);
            if (!m_socket.IsConnect()) return eTCPResult.Error;
            foreach (char ch in strCmd)
            {
                m_bufSend[lBuf++] = (byte)ch;
            }
            m_bufSend[lBuf++] = (byte)0x0d; 
            m_bufSend[lBuf++] = (byte)0x0a; 
            m_log.Add("--> " + strCmd);
            return m_socket.Send(m_bufSend, lBuf);
        }

        eTCPResult WaitReply(int msWait)
        {
            int ms = 0;
            while (m_bSendCmd)
            {
                Thread.Sleep(10);
                ms += 10;
                if (ms > msWait)
                {
                    m_log.Popup("Cognex OCR Camera TCP/IP Response Timeout !!");
                    m_bSendCmd = false;
                    return eTCPResult.Timeout;
                }
            }
            m_bSendCmd = false;
            return eTCPResult.OK;
        }

        void CallMsgRcv(byte[] buf, int nSize)
        {
            bOCRDone = false;
            string sMsg = Encoding.Default.GetString(buf, 0, nSize);
            sMsg = sMsg.Replace("\r\n", "");
            m_log.Add("<-- " + sMsg);
            string[] sMsgs = sMsg.Split(','); 
            m_strOCR = sMsgs[0];
            bOCRDone = true;
            if (sMsgs.Length <= 1) return;
            m_scoreOCR = Convert.ToDouble(sMsgs[1]); 
        }

    }
}
