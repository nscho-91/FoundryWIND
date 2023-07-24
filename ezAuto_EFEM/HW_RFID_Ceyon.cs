using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ezAutoMom;
using ezTools;

namespace ezAuto_EFEM
{
    class HW_RFID_Ceyon : HW_RFID_Mom, Control_Child
    {
        const int BUF_SIZE = 4096; 

        delegate void CallRS232(string[] sMsgs);
        ezRS232 m_rs232;
        CallRS232 m_cbRS232 = null;
        string[] m_sMsgs = null;
        char[] m_aBuf = new char[BUF_SIZE];
        string m_sCSTIDAddress = "";
        string m_sLOTIDAddress = "";
        int m_nCSTIDLen = 0;
        int m_nLOTIDLen = 0;
        //9600 8 1 x03
        //56000 8 1 none

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
            m_grid.Set(ref m_sCSTIDAddress, "Read Opt", "CSTID Address", "CST ID Start Address( 00 ~ EE )");
            m_grid.Set(ref m_sLOTIDAddress, "Read Opt", "LOTID Address", "LOT ID Start Address( 00 ~ EE )");
            m_grid.Set(ref m_nCSTIDLen, "Read Opt", "CSTID Len", "CST ID Lenght");
            m_grid.Set(ref m_nLOTIDLen, "Read Opt", "LOTID Len", "LOT ID Lenght");
        }
        void m_rs232_CallMsgRcv()
        {
            int nRead = m_rs232.Read(m_aBuf, BUF_SIZE);
            string sMsg = new string(m_aBuf);
            
            
            
            if (m_cbRS232 == null) return;
            m_cbRS232(m_sMsgs);

        }

        void WriteCmd(string cmd)
        {
            string strCMD = ((char)0x05).ToString();
            strCMD += cmd;


            char[] sChar = strCMD.ToCharArray();
            int CheckSum = 0;
            string sCheckSum = "";

            for (int i = 0; i < strCMD.Length; i++) {
                CheckSum += sChar[i];
            }
            sCheckSum = CheckSum.ToString("X").Substring(CheckSum.ToString("X").Length - 2, 2);     // 뒤 두자리 저장
            strCMD += sCheckSum;

            char[] sCMDChar = strCMD.ToCharArray();

            m_rs232.Write(sCMDChar, sCMDChar.Length, true);
            m_log.Add("CMD Send : ");
        }


        public override void CSTIDRead()
        {

            /*  Read 명령 :  ENQ(0x05) + MachineID(2) + CMD(2) +Address(2) + Length(2) */
             
            string str = "01"; //MachineID 1고정
            str += "80";        //Channel1 Read CMD
            m_sCSTIDAddress = m_sCSTIDAddress.ToUpper();
            if (m_sCSTIDAddress.Length == 1) {
                m_sCSTIDAddress = "0" + m_sCSTIDAddress;
            }
            str += m_sCSTIDAddress;

            str += m_nCSTIDLen.ToString("00");
            
            WriteCmd(str);
        }
    }
}
