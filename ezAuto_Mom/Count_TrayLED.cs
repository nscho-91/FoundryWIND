using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ezTools;

namespace ezAutoMom
{
    public class Count_TrayLED
    {
        ushort [] c_sCRC = new ushort[256]
          {	0x0000, 0xC0C1, 0xC181, 0x0140, 0xC301, 0x03C0, 0x0280, 0xC241,
	        0xC601, 0x06C0, 0x0780, 0xC741, 0x0500, 0xC5C1, 0xC481, 0x0440,
	        0xCC01, 0x0CC0, 0x0D80, 0xCD41, 0x0F00, 0xCFC1, 0xCE81, 0x0E40,
	        0x0A00, 0xCAC1, 0xCB81, 0x0B40, 0xC901, 0x09C0, 0x0880, 0xC841,
	        0xD801, 0x18C0, 0x1980, 0xD941, 0x1B00, 0xDBC1, 0xDA81, 0x1A40,
	        0x1E00, 0xDEC1, 0xDF81, 0x1F40, 0xDD01, 0x1DC0, 0x1C80, 0xDC41,
	        0x1400, 0xD4C1, 0xD581, 0x1540, 0xD701, 0x17C0, 0x1680, 0xD641,
	        0xD201, 0x12C0, 0x1380, 0xD341, 0x1100, 0xD1C1, 0xD081, 0x1040,
	        0xF001, 0x30C0, 0x3180, 0xF141, 0x3300, 0xF3C1, 0xF281, 0x3240,
	        0x3600, 0xF6C1, 0xF781, 0x3740, 0xF501, 0x35C0, 0x3480, 0xF441,
	        0x3C00, 0xFCC1, 0xFD81, 0x3D40, 0xFF01, 0x3FC0, 0x3E80, 0xFE41,
	        0xFA01, 0x3AC0, 0x3B80, 0xFB41, 0x3900, 0xF9C1, 0xF881, 0x3840,
	        0x2800, 0xE8C1, 0xE981, 0x2940, 0xEB01, 0x2BC0, 0x2A80, 0xEA41,
	        0xEE01, 0x2EC0, 0x2F80, 0xEF41, 0x2D00, 0xEDC1, 0xEC81, 0x2C40,
	        0xE401, 0x24C0, 0x2580, 0xE541, 0x2700, 0xE7C1, 0xE681, 0x2640,
	        0x2200, 0xE2C1, 0xE381, 0x2340, 0xE101, 0x21C0, 0x2080, 0xE041,
	        0xA001, 0x60C0, 0x6180, 0xA141, 0x6300, 0xA3C1, 0xA281, 0x6240,
	        0x6600, 0xA6C1, 0xA781, 0x6740, 0xA501, 0x65C0, 0x6480, 0xA441,
	        0x6C00, 0xACC1, 0xAD81, 0x6D40, 0xAF01, 0x6FC0, 0x6E80, 0xAE41,
	        0xAA01, 0x6AC0, 0x6B80, 0xAB41, 0x6900, 0xA9C1, 0xA881, 0x6840,
	        0x7800, 0xB8C1, 0xB981, 0x7940, 0xBB01, 0x7BC0, 0x7A80, 0xBA41,
	        0xBE01, 0x7EC0, 0x7F80, 0xBF41, 0x7D00, 0xBDC1, 0xBC81, 0x7C40,
	        0xB401, 0x74C0, 0x7580, 0xB541, 0x7700, 0xB7C1, 0xB681, 0x7640,
	        0x7200, 0xB2C1, 0xB381, 0x7340, 0xB101, 0x71C0, 0x7080, 0xB041,
	        0x5000, 0x90C1, 0x9181, 0x5140, 0x9301, 0x53C0, 0x5280, 0x9241,
	        0x9601, 0x56C0, 0x5780, 0x9741, 0x5500, 0x95C1, 0x9481, 0x5440,
	        0x9C01, 0x5CC0, 0x5D80, 0x9D41, 0x5F00, 0x9FC1, 0x9E81, 0x5E40,
	        0x5A00, 0x9AC1, 0x9B81, 0x5B40, 0x9901, 0x59C0, 0x5880, 0x9841,
	        0x8801, 0x48C0, 0x4980, 0x8941, 0x4B00, 0x8BC1, 0x8A81, 0x4A40,
	        0x4E00, 0x8EC1, 0x8F81, 0x4F40, 0x8D01, 0x4DC0, 0x4C80, 0x8C41,
	        0x4400, 0x84C1, 0x8581, 0x4540, 0x8701, 0x47C0, 0x4680, 0x8641,
	        0x8201, 0x42C0, 0x4380, 0x8341, 0x4100, 0x81C1, 0x8081, 0x4040 };

        string m_id;
        int m_msDelay = 50;
        Auto_Mom m_auto;
        Log m_log;
        ezRS232[] m_rs232 = new ezRS232[16];
        public int m_nRS232;

        public Count_TrayLED()
        {

        }

        public void Init(string id, Auto_Mom auto)
        {
            int n; string str;
            m_id = id; m_auto = auto;
            m_log = new Log(m_id, m_auto.ClassLogView());
            m_log.m_reg.Read("RS232", ref m_nRS232);
            for (n = 0; n < 16; n++) 
            { 
                str = n.ToString("00");
                m_rs232[n] = new ezRS232(m_id + str, m_log); 
            }
        }

        public void ThreadStop()
        {
            int n;
            for (n = 0; n < 16; n++) m_rs232[n].ThreadStop();
            m_log.m_reg.Write("RS232", m_nRS232);
        }

        public void RunGrid(ezGrid rGrid, eGrid eMode)
        {
            int n;
            rGrid.Set(ref m_nRS232, m_id, "RS232", "# of RS232");
            rGrid.Set(ref m_msDelay, m_id, "Delay", "Communication Delay");
            for (n = 0; n < m_nRS232; n++) m_rs232[n].RunGrid(rGrid, eMode);
        }

        public void Connect()
        {
            int n;
            for (n = 0; n < m_nRS232; n++) m_rs232[n].Connect(true);
        }

        public bool Write(int nRS232, int nAdd, string str) 
        {
            if (!m_rs232[nRS232].IsConnect()) return true;
            int n, l; byte nTemp; ushort nCRC = 0xffff;
            byte[] asciiByte = Encoding.ASCII.GetBytes(str); 
            byte[] aCode = new byte[20];
	        aCode[0] = (byte)nAdd; aCode[1] = 0x10; aCode[2] = 0; aCode[3] = 1; aCode[4] = 0; aCode[5] = 2; aCode[6] = 4;
	        for (n = 0; n<4; n++) aCode[7 + n] = 0x3f;
            for (n = 0; n < str.Length; n++) aCode[7 + n] = GetRS232Code(asciiByte[n]);
	        l = 11; aCode[l] = aCode[l + 2] = 0; n = l - 1;
	        for (n = 0; n<l; n++) { nTemp =(byte)(aCode[n] ^ (byte)nCRC); nCRC >>= 8; nCRC ^= c_sCRC[nTemp]; }
	        aCode[l] = (byte)(nCRC % 256); aCode[l + 1] = (byte)(nCRC / 256);
            try
            {
                m_rs232[nRS232].Write(aCode, l + 2, false); Thread.Sleep(m_msDelay);
                l = m_rs232[nRS232].Read(aCode, 20); Thread.Sleep(5);
            }
            catch(Exception ex)
            {
                m_log.Add(m_id + "_" + ex.Message);
                try
                {
                    m_rs232[nRS232].Write(aCode, l + 2, false); Thread.Sleep(m_msDelay);
                    l = m_rs232[nRS232].Read(aCode, 20); Thread.Sleep(5);
                }
                catch (Exception ex2)
                {
                    m_log.Add(m_id + "_" + ex2.Message);
                }
            }
            if (l == 0) { Reconnect(nRS232); m_log.Add(m_rs232[nRS232].m_strPort + "_" + m_rs232[nRS232].m_id + "Reconnect"); return true; }
	        else return false;
        }



        byte GetRS232Code(byte ch)
        {
            if ((ch >= '0') && (ch <= '9')) return (byte)(ch - '0');
            if ((ch >= 'A') && (ch <= 'Z')) return (byte)(ch - 'A' + 10);
            if (ch == '?') return (byte)(0x2C);
            if (ch == '#') return (byte)(0x32);
            return 0x3f;
        }

        public void Reconnect(int n)
        {
            Thread.Sleep(5); m_rs232[n].Connect(false);
            Thread.Sleep(5); m_rs232[n].Connect(true); Thread.Sleep(5);
        }
    }
}
