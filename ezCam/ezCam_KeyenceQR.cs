using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using ezTools;
using System.IO.Ports; 

namespace ezCam
{
    public class ezCam_KeyenceQR //forget
    {
        string m_id;
        Log m_log;

        SerialPort serialPortInstance = new SerialPort();

        const Byte STX = 0x02;
        const Byte ETX = 0x03;
        const Byte CR = 0x0d;

        const int RECV_DATA_MAX = 10240;
        string m_sBCRResult = "";
        bool m_bBCRRead = false;

        int m_nQRBaudRate = 115200;
        int m_nQRDataBits = 8;
        int m_nQRParity = 2;
        int m_nQRStopBits = 1;
        string m_strQRPortName = "COM1";

        public bool m_bUse = false;
        ezStopWatch m_swTimeout = new ezStopWatch();

        public void Init(string id, Log log, bool bUse)
        {
            m_id = id;
            m_log = log;
            m_bUse = bUse;
        }

        public void ThreadStop()
        {
            if (!m_bUse) return;
            if (serialPortInstance.IsOpen) serialPortInstance.Close();
        }

        public void RunGrid(ezGrid rGrid, eGrid eMode)
        {
            if (!m_bUse) return;

            rGrid.Set(ref m_strQRPortName, m_id, "Port", "Com Port Number");
            rGrid.Set(ref m_nQRBaudRate, m_id, "BaudRate", "Baud Rate");
            rGrid.Set(ref m_nQRDataBits, m_id, "DataBits", "Data Bits()");
            rGrid.Set(ref m_nQRParity, m_id, "Parity", "Parity (None = 0, Odd = 1, Even = 2, Mark = 3, Space = 4)");
            rGrid.Set(ref m_nQRStopBits, m_id, "StopBits", "Stop Bits (None = 0, One = 1, Two = 2, OnePointFive = 3)");
            rGrid.Set(ref m_msWait, m_id, "Wait", "Wait for Reply (ms)");
        }

        public bool BarcodeConnect()
        {
            this.serialPortInstance.BaudRate = m_nQRBaudRate;          // 9600, 19200, 38400, 57600 or 115200
            this.serialPortInstance.DataBits = m_nQRDataBits;               // 7 or 8
            this.serialPortInstance.Parity = (Parity)m_nQRParity;       // Even or Odd
            this.serialPortInstance.StopBits = (StopBits)m_nQRStopBits;    // One or Two
            this.serialPortInstance.PortName = m_strQRPortName;

            this.serialPortInstance.DataReceived += new SerialDataReceivedEventHandler(serialPortInstance_DataReceived);

            try
            {
                if (serialPortInstance.IsOpen)
                {
                    this.serialPortInstance.Close();
                }
                this.serialPortInstance.Open();
                this.serialPortInstance.ReadTimeout = 100;
            }
            catch (Exception ex)
            {
                m_log.Add(serialPortInstance.PortName + "\r\n" + ex.Message);
                return true;
            }
            return false;
        }

        void serialPortInstance_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Byte[] recvBytes = new Byte[RECV_DATA_MAX];
            int recvSize;

            if (this.serialPortInstance.IsOpen == false)
            {
                m_log.Add(serialPortInstance.PortName + " is disconnected.");
            }

            for (; ; )
            {
                try
                {
                    recvSize = readDataSub(recvBytes, this.serialPortInstance);
                }
                catch (IOException ex)
                {
                    m_log.Add(serialPortInstance.PortName + "\r\n" + ex.Message);    // disappeared
                    break;
                }
                if (recvSize == 0)
                {
                    m_log.Add(serialPortInstance.PortName + " has no data.");
                    break;
                }
                if (recvBytes[0] != STX)
                {
                    continue;
                }
                else
                {
                    recvSize--; 
                    char[] aResult = new char[recvSize];
                    for (int n = 0; n < recvSize; n++) aResult[n] = (char)recvBytes[n + 1];
                    aResult[recvSize - 1] = (char)0;
                    m_sBCRResult = new string(aResult);
                    m_bBCRRead = true;
                    break;
                }
            }
        }

        public void BarcodeRead()
        {
            if (serialPortInstance.IsOpen == false) BarcodeConnect(); 
            m_swTimeout.Start();
            m_bBCRRead = false;
            m_sBCRResult = "";
            string lon = "\x02LON\x03";   // <STX>LON<ETX>
            Byte[] sendBytes = ASCIIEncoding.ASCII.GetBytes(lon);

            if (this.serialPortInstance.IsOpen)
            {
                try
                {
                    this.serialPortInstance.Write(sendBytes, 0, sendBytes.Length);
                }
                catch (IOException ex)
                {
                    m_log.Add(serialPortInstance.PortName + "\r\n" + ex.Message);    // disappeared
                }
            }
            else
            {
                m_log.Add(serialPortInstance.PortName + " is disconnected.");
            }
        }

        public void BarcodeReadOff()
        {
            m_bBCRRead = false;
            m_sBCRResult = "";
            string lon = "\x02LOFF\x03";   // <STX>LON<ETX>
            Byte[] sendBytes = ASCIIEncoding.ASCII.GetBytes(lon);

            if (this.serialPortInstance.IsOpen)
            {
                try
                {
                    this.serialPortInstance.Write(sendBytes, 0, sendBytes.Length);
                }
                catch (IOException ex)
                {
                    m_log.Add(serialPortInstance.PortName + "\r\n" + ex.Message);    // disappeared
                }
            }
            else
            {
                m_log.Add(serialPortInstance.PortName + " is disconnected.");
            }
        }



        private int readDataSub(Byte[] recvBytes, SerialPort serialPortInstance)
        {
            int recvSize = 0;
            bool isCommandRes = false;
            Byte d;

            try         // Distinguish between command response and read data.
            {
                d = (Byte)serialPortInstance.ReadByte();
                recvBytes[recvSize++] = d;
                if (d == STX)
                {
                    isCommandRes = true;    // Distinguish between command response and read data.
                }
            }
            catch (TimeoutException)
            {
                return 0;   //  No data received.
            }

            for (; ; )      // Receive data until the terminator character.
            {
                try
                {
                    d = (Byte)serialPortInstance.ReadByte();
                    recvBytes[recvSize++] = d;

                    if (isCommandRes && (d == ETX))
                    {
                        break;  // Command response is received completely.
                    }
                }
                catch (TimeoutException ex)
                {
                    m_log.Add(ex.Message);        // No terminator is received.
                    return 0;
                }
            }

            return recvSize;
        }

        public bool SuccessReading()
        {
            while (!m_bBCRRead && m_swTimeout.Check() < 3000)
            {
                Thread.Sleep(100);
            }
            if (m_bBCRRead) return true;
            else return false;
        }

        public string GetResult()
        {
            if (m_bBCRRead) return m_sBCRResult;
            else return "";
        }

        int m_msWait = 500; 
        public string ReadCode()
        {
            BarcodeRead();
            Thread.Sleep(m_msWait);
            if (SuccessReading()) return GetResult();
            return ""; 
        }
    }
}
