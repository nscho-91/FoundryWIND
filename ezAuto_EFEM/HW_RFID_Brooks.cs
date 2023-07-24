using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ezAutoMom;
using ezTools;


namespace ezAuto_EFEM
{
    class HW_RFID_Brooks : HW_RFID_Mom, Control_Child
    {
        const int BUF_SIZE = 4096; 

        delegate void CallRS232(string[] sMsgs);
        ezRS232 m_rs232;
        CallRS232 m_cbRS232 = null;
        string[] m_sMsgs;
        char[] m_aBuf = new char[BUF_SIZE];
        string m_sCSTIDAddress = "";
        string m_sLOTIDAddress = "";
        int m_nCSTIDLen = 0;
        int m_nLOTIDLen = 0;
        int m_nLen = 0;
        string m_sAddr = "";
        string sResultMSG = "";     //KDG 160929 Add
        //19200 8 even 1 none x53 CR

        public override void Init(string id, Auto_Mom auto)
        {
            base.Init(id, auto);
            m_control.Add(this);
            m_rs232 = new ezRS232(m_id, m_log);
            m_rs232.CallMsgRcv += m_rs232_CallMsgRcv;
            RunGrid(eGrid.eRegRead);
            RunGrid(eGrid.eInit);
            m_rs232.Connect(true);
        }

        void SetAlarm()
        {
        }

        public void ControlGrid(Control_Mom control, ezGrid rGrid, eGrid eMode)
        {
            
        }
        protected override void RunGrid(eGrid eMode)
        {
            base.RunGrid(eMode);
            m_rs232.RunGrid(m_grid, eMode);
            m_grid.Set(ref m_sCSTIDAddress, "Read Opt", "CSTID Address", "CST ID Start Address( 0 ~ E )");
            m_grid.Set(ref m_sLOTIDAddress, "Read Opt", "LOTID Address", "LOT ID Start Address( 0 ~ E )");
            m_grid.Set(ref m_nCSTIDLen, "Read Opt", "CSTID Len", "CST ID Lenght");
            m_grid.Set(ref m_nLOTIDLen, "Read Opt", "LOTID Len", "LOT ID Lenght");
        }
        void m_rs232_CallMsgRcv()
        {
            Thread.Sleep(500);
            int nRead = m_rs232.Read(m_aBuf, BUF_SIZE);
            string sMsg = new string(m_aBuf);
            m_log.Add("RFID Message : "+sMsg);
            
            
            switch (sMsg.Substring(0, 1))
            {
                case "S":
                    switch (sMsg.Substring(3, 1))
                    {
                        case "x":
                            m_nLen = Convert.ToInt32(sMsg.Substring(1, 2));
                            m_sAddr = sMsg.Substring(6, 1);
                            sResultMSG = sMsg.Substring(7, 1);  //KDG 160929 Add
                            if (m_sAddr == m_sCSTIDAddress)
                            {
                                m_sCSTIDResult = RFIDResult(sMsg.Substring(7, nRead - 7));      //20160930 SDH ADD // ing 170918 6->7
                            }
                            else if (m_sAddr == m_sLOTIDAddress)
                            {
                                m_sLOTIDResult = RFIDResult(sMsg.Substring(7, nRead - 7));      //20160930 SDH ADD // ing 170918 6->7
                            }
                            break;
                        case "e":
                            #region ErrorList
                            switch (sMsg.Substring(4, 2))
                            {
                                case "00":
                                    break;
                                case "01":
                                    break;
                                case "02":
                                    break;
                                case "03":
                                    break;
                                case "04":
                                    break;
                                case "05":
                                    break;
                                case "06":
                                    break;
                                case "07":
                                    break;
                                case "08":
                                    break;
                                case "09":
                                    break;
                                case "0A":
                                    break;
                                case "0:":
                                    break;
                                case "0;":
                                    break;
                                case "0B":
                                    break;
                            }
                            #endregion
                            break;
                        default:
                           break;
                    }
                    break;
                default:
                   
                    break;
            }

            if (m_cbRS232 == null) return;
            m_cbRS232(m_sMsgs);
        }

        void WriteCmd(string cmd)
        {
            for (int n = 0; n < 50; n++)
                m_aBuf[n] = (char)0x00;
            string sStartString = "S";
            char aEnd = (char)0x0D;
            m_sMsgs = null;
            string sCMD ="";

            sCMD += sStartString;
            sCMD += cmd;
            sCMD += aEnd;
            int XOR = 0x00;
            int And = 0;
            byte[] aCMD = Encoding.Default.GetBytes(sCMD);
            for (int n = 0; n < sCMD.Length; n++)
            {
                XOR ^= aCMD[n]; 
            }
            for (int n = 0; n < sCMD.Length; n++)
            {
                And += aCMD[n];
            }
            XOR = XOR % (16 * 16);
            sCMD += (XOR / 16).ToString("X");
            sCMD += (XOR % 16).ToString("X");
            And = And % (16*16);
            sCMD += (And / 16).ToString("X");
            sCMD += (And % 16).ToString("X");
           
            char[] sChar = sCMD.ToCharArray();
            m_rs232.Write(sChar, sCMD.Length, true);
            m_log.Add("CMD Send : ");
        }

        public override void CSTIDRead()
        {
            WriteCmd("04X00"+m_sCSTIDAddress);
        }

        public override void LOTIDRead()
        {
            WriteCmd("04X00" + m_sLOTIDAddress);
        }

        public override void ResetID()
        {
            base.ResetID();
        }

        public string RFIDResult(string sMsg)    //20160930 SDH ADD
        {
            bool bDone = false;
            int nRFIDLen = 0;
            string sResult = "";
            int nLength = sMsg.Length;
            for (int i = 0; i < nLength; i++)
            {
                string str = sMsg.Substring(i, 1);
                if (str == "\r")
                {
                    bDone = true;
                    nRFIDLen = i;
                }
            }
            if (!bDone) return "Error(not find \r)";
            if (nRFIDLen % 2 != 0) return "Error(Incomplete RFID)";
            for (int j = 0; j < nRFIDLen; j += 2)
            {
                string str = sMsg.Substring(j, 2);

                char decValue = (char)int.Parse(str, System.Globalization.NumberStyles.HexNumber);
                sResult += Convert.ToString(decValue);
            }

            return sResult;
        }
    }
}
