using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace ezTools
{
    public class ezRS232
    {
        public delegate void MsgRcv();
        public event MsgRcv CallMsgRcv;
 
        const int c_nRS232_Buf = 4096;
        const int c_msRS232_Timeout = 2000;

        public string m_id, m_strPort = "COM20", m_strParity = "None", m_strStop = "1";
        string[] m_strParityGroup = { "Even", "Odd", "None", "Mark", "Space" };
        string[] m_strStopGroup = { "None", "1", "1.5", "2" };
        int m_nBaud = 57600, m_nData = 8;
        bool m_bAsynchrone = false;
        byte[] m_buf = new byte[c_nRS232_Buf];
        SerialPort m_sp = new SerialPort();
        public Log m_log = null;

        public string [] m_readBuff;

        public ezRS232(string id, Log log, bool bAsynchrone = false)
        {
            m_id = id; 
            m_log = log; 
            m_bAsynchrone = bAsynchrone;
        }

        public void ThreadStop()
        {
            if (m_sp.IsOpen) m_sp.Close();
        }

        void m_sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            CallMsgRcv();
        }

        public void RunGrid(ezGrid rGrid, eGrid eMode, ezJob job = null)
        {
            rGrid.Set(ref m_strPort, m_id, "Port", "Port Number");
            rGrid.Set(ref m_nBaud, m_id, "Baud", "Baud rate");
            rGrid.Set(ref m_nData, m_id, "Data", "Data bit");
            rGrid.Set(ref m_strStop, m_strStopGroup, m_id, "Stop", "Stop bit");
            rGrid.Set(ref m_strParity, m_strParityGroup, m_id, "Parity", "Parity");
        }

        public bool Connect(bool bConnect)
        {
            if (!bConnect) m_sp.Close();
            else
            {
                Thread.Sleep(500);
                Parity parity = new Parity();
                StopBits stop = new StopBits();
                switch (m_strStop)
                {
                    case "None": stop = StopBits.None; break;
                    case "1": stop = StopBits.One; break;
                    case "1.5": stop = StopBits.OnePointFive; break;
                    case "2": stop = StopBits.Two; break;
                }
                switch (m_strParity)
                {
                    case "Even": parity = Parity.Even; break;
                    case "Odd": parity = Parity.Odd; break;
                    case "None": parity = Parity.None; break;
                    case "Mark": parity = Parity.Mark; break;
                    case "Space": parity = Parity.Space; break;
                }
                m_sp = new SerialPort(m_strPort, m_nBaud, parity, m_nData, stop);
                m_sp.ReadTimeout = c_msRS232_Timeout; 
                m_sp.WriteTimeout = c_msRS232_Timeout;
                Thread.Sleep(500);
                //m_sw.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceive);
                try { m_sp.Open(); }
                catch (Exception ex) { if (m_log != null) m_log.Popup(ex.Message); return false; }
            }
            if (!m_bAsynchrone) m_sp.DataReceived += m_sp_DataReceived;
            return m_sp.IsOpen;
        }

        public bool Connect(bool bConnect, string strPort, int nBaud, Parity parity, int nData, StopBits stop)
        {
            if (!bConnect) m_sp.Close();
            else
            {
                m_sp = new SerialPort(strPort, nBaud, parity, nData, stop);
                m_sp.ReadTimeout = c_msRS232_Timeout;
                m_sp.WriteTimeout = c_msRS232_Timeout;
                try { m_sp.Open(); }
                catch (Exception ex) { if (m_log != null) m_log.Popup(ex.Message); }
            }
            if (!m_bAsynchrone) m_sp.DataReceived += m_sp_DataReceived;
            return m_sp.IsOpen;
        }

        public bool IsConnect()
        {
            return m_sp.IsOpen;
        }

        public string ReadLine()
        {
            string str, strChar;
            if (!IsConnect()) Connect(true);
            if (!IsConnect()) return "";

            try 
            {
                strChar = m_sp.ReadLine(); 
            }
            catch 
            {
                return ""; 
            }

            str = strChar.Length.ToString() + ", " + strChar;
            if (m_log != null) m_log.Add("Read : " + str, false);
            return strChar;
        }

        public int Read(byte[] buf, int nRead, int nOffset = 0, bool bLog = true) 
        {
            int nLength; string str, strChar;
            if (!IsConnect()) Connect(true);
            if (!IsConnect()) return -1;
            char[] aResult = new char[nRead];
            try { nLength = m_sp.Read(buf, nOffset, nRead); }
            catch { return 0; }
            if (nLength < 0) return nLength;
            Encoding.Unicode.GetChars(buf, nOffset, nRead, aResult, 0);
            strChar = new string(aResult);
            str = nLength.ToString() + ", " + strChar.Substring(0, nLength);
            if (bLog && (m_log != null)) m_log.Add("Read : " + str, false);
            return nLength;
        }

        public int Read(char[] buf, int nRead, int nOffset = 0)
        {
            int nLength; string str;
            if (!IsConnect()) Connect(true);
            if (!IsConnect()) return -1;
            try { nLength = m_sp.Read(buf, nOffset, nRead); }
            catch { return 0; }
            if (nLength < 0) return nLength;
            str = nLength.ToString() + ", " + new string(buf);
            if (m_log != null) m_log.Add("Read : " + str.Substring(0, nLength), false);
            return nLength;
        }

        public void Write(byte[] buf, int nWrite, bool bLog, int nOffset = 0)
        {
            if (!IsConnect()) return;
            m_sp.ReadExisting();
            char[] aResult = new char[nWrite];
            if (bLog && (m_log != null)) m_log.Add("Write : " + new string(aResult), false);
            m_sp.Write(buf, nOffset, nWrite);
        }

        public void Write(char[] buf, int nWrite, bool bLog, int nOffset = 0)
        {
            if (!IsConnect()) return;
            m_sp.ReadExisting();
            char[] aResult = buf;
            if (bLog && (m_log != null)) m_log.Add("Write : " + new string(buf), false);
            m_sp.Write(buf, nOffset, nWrite);
        }

        public void Write(string str, bool bLog)
        {
            if (m_sp.IsOpen == false)
            {
                Connect(true);
                if (m_sp.IsOpen == false) return;
            }
            m_sp.Write(str);
        }

        public void WriteLine(string str, bool bLog)
        {
            try
            {
               m_sp.WriteLine(str);
               if (bLog && (m_log != null)) m_log.Add("Write : " + str, false);
            }
            catch (Exception e)
            {
                if (m_log != null) m_log.Popup(e.Message); 
            }
    
        }

        public void Write(char ch)
        {
            char[] cr = new char[1];
            cr[0] = ch;
            m_sp.Write(cr, 0, 1);
        }

    }
}
