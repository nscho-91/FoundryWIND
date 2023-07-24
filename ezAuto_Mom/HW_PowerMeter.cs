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

namespace ezAutoMom
{
    public partial class HW_PowerMeter : Form
    {
        const int BUF_SIZE = 4096;

        Auto_Mom m_auto;
        Handler_Mom m_handler;
        Size[] m_sz = new Size[2];
        protected string m_id;
        protected Log m_log;
        protected ezGrid m_grid;
        protected ezTCPSocket m_socket = null;
        //PortNumber = 502
        protected bool m_bRunThread = false;
        public bool m_bUse = false;
        Thread m_thread = null;

        protected int m_nTransID = 0;
        float m_fVolt = 0.0f;
        float m_fCurrent = 0.0f;


        public HW_PowerMeter()
        {
            InitializeComponent();
            m_sz[0] = m_sz[1] = this.Size;
            m_sz[0].Height = 26;
        }

        public void Init(string sID, Auto_Mom auto)
        {
            m_id = sID;
            m_auto = auto;
            checkView.Text = m_id;
            m_log = new Log(m_id, m_auto.m_logView, m_id);
            m_grid = new ezGrid(m_id, grid, m_log, false);
            m_socket = new ezTCPSocket(m_id, m_log, false);
            m_handler = m_auto.ClassHandler();
            RunGrid(eGrid.eRegRead);
            RunGrid(eGrid.eInit);
            if (!m_bUse) return;
            m_socket.Init();
            m_socket.CallMsgRcv += CallMsgRcv;
            m_thread = new Thread(new ThreadStart(RunThread)); m_thread.Start();
            
        }


        public void ThreadStop()
        {
            m_socket.ThreadStop();
            if (m_bRunThread) {
                m_bRunThread = false;
                m_thread.Join();
            }
        }

        public void ShowDlg(Form parent, ref CPoint cpShow)
        {
            int nIndex;
            this.TopLevel = false;
            this.Parent = parent;
            this.Location = cpShow.ToPoint();
            if (checkView.Checked) nIndex = 1;
            else nIndex = 0;
            this.Size = m_sz[nIndex];
            cpShow.y += m_sz[nIndex].Height;
            Show();
        }

        protected void RunGrid(eGrid eMode)
        {
            m_grid.Update(eMode, null);
            m_grid.Set(ref m_bUse, m_id, "Use", "Use");
            m_socket.RunGrid(m_grid);
            m_grid.Refresh();
        }

        private void checkView_CheckedChanged(object sender, EventArgs e)
        {
            m_handler.ShowChild();
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate);
            RunGrid(eGrid.eRegWrite);
        }

        public string GetVoltage()
        {
            return m_fVolt.ToString();
        }

        public string GetCurrent()
        {
            return m_fCurrent.ToString();
        }


        void RunThread()
        {
            m_bRunThread = true;
            Thread.Sleep(5000);
            while (m_bRunThread)
            {
                Thread.Sleep(1000);
                switch (m_nTransID % 3)
                {
                    case 0:
                        SendDataGetMSG(3, 11044 - 1, 1);
                        break;
                    case 1:
                        SendDataGetMSG(3, 11101 - 1, 56);
                        break;
                    case 2:
                        SendDataGetMSG(3, 11351 - 1, 60);
                        break;
                }
            }
        }


        protected void CallMsgRcv(byte[] byteMsg, int nSize)
        {
            string sMsg = Encoding.Default.GetString(byteMsg, 0, nSize);

            byte[] bTemp = new byte[4] { 0, 0, 0, 0 };
            switch (m_nTransID % 3)
            {
                case 0:

                    break;
                case 1:
                    bTemp[0] = byteMsg[40];
                    bTemp[1] = byteMsg[39];
                    bTemp[2] = byteMsg[38];
                    bTemp[3] = byteMsg[37];
                    m_fVolt = BitConverter.ToSingle(bTemp, 0);
                    break;
                case 2:
                    bTemp[0] = byteMsg[24];
                    bTemp[1] = byteMsg[23];
                    bTemp[2] = byteMsg[22];
                    bTemp[3] = byteMsg[21];
                    m_fCurrent = BitConverter.ToSingle(bTemp, 0);
                    break;
            }

            if (m_nTransID < 65000)
                m_nTransID++;
            else
                m_nTransID = 0;
            //  throw new NotImplementedException();      
        }

        
        protected void SendDataGetMSG(int nFuncCode, int nAddress, int nLength)
        {
            /*    TransactionID  + ProtocolID  + length + UnitID + FunctionCode + Address + Length 
             *       2byte            2byte       2byte     1byte     1byte       2byte      2byte
             *     
             */
            int nProtocolID = 0; // 0으로 고정
            int nMSGLenght = 6;
            int nUnitID = 1;

            byte[] SendByte = new byte[12];

            int n = 0;
            Buffer.BlockCopy(BitConverter.GetBytes(m_nTransID), 1, SendByte, n, 1);
            n += 1;
            Buffer.BlockCopy(BitConverter.GetBytes(m_nTransID), 0, SendByte, n, 1);
            n += 1;
            Buffer.BlockCopy(BitConverter.GetBytes(nProtocolID), 1, SendByte, n, 1);
            n += 1;
            Buffer.BlockCopy(BitConverter.GetBytes(nProtocolID), 0, SendByte, n, 1);
            n += 1;
            Buffer.BlockCopy(BitConverter.GetBytes(nMSGLenght), 1, SendByte, n, 1);
            n += 1;
            Buffer.BlockCopy(BitConverter.GetBytes(nMSGLenght), 0, SendByte, n, 1);
            n += 1;
            Buffer.BlockCopy(BitConverter.GetBytes(nUnitID), 0, SendByte, n, 1);
            n += 1;
            Buffer.BlockCopy(BitConverter.GetBytes(nFuncCode), 0, SendByte, n, 1);
            n += 1;
            Buffer.BlockCopy(BitConverter.GetBytes(nAddress), 1, SendByte, n, 1);
            n += 1;
            Buffer.BlockCopy(BitConverter.GetBytes(nAddress), 0, SendByte, n, 1);
            n += 1;
            Buffer.BlockCopy(BitConverter.GetBytes(nLength), 1, SendByte, n, 1);
            n += 1;
            Buffer.BlockCopy(BitConverter.GetBytes(nLength), 0, SendByte, n, 1);
            n += 1;

            m_socket.Send(SendByte, n);
        }


        private void button1_Click(object sender, EventArgs e)
        {

            float a = 218.01F;
            byte[] f = System.BitConverter.GetBytes(a);
            //SendDataGetMSG(3, 11043, 1);
        }
    }
}
//BYTE nReceiveBuf[300];    //KYS 140604 add
//   Socket->ReceiveBuf(nReceiveBuf, sizeof(BYTE)*300); //112

//   char line[256];
//   int nReceiveTrans;
//   int nReceiveProtocol;
//   int nReceiveMsgLength;
//   int nReceiveUnit;
//   int nReceiveFunct;
//   int nReceiveLenth;
//   int nExceptioncode;

//   nReceiveTrans=(((int)nReceiveBuf[0])*256)+((int)nReceiveBuf[1]);
//   nReceiveProtocol=(((int)nReceiveBuf[2])*256)+((int)nReceiveBuf[3]);
//   nReceiveMsgLength=(((int)nReceiveBuf[4])*256)+((int)nReceiveBuf[5]);
//   nReceiveUnit=(int)nReceiveBuf[6];
//   nReceiveFunct=(int)nReceiveBuf[7];
//   nReceiveLenth=(int)nReceiveBuf[8];
//   nExceptioncode=(int)nReceiveBuf[8];
//   int nCnt;


//   if(nReceiveFunct==3 || nReceiveFunct==6 || nReceiveFunct==16 || nReceiveFunct==106){
//      if(nReceiveLenth==112 && GetModbusTcpAddress()==(11101-2)){
//         for(int i=0;i<28;i++){
//            nCnt=i*4;
//            fModbusTcpValue[i]=ConvIEEE754(nReceiveBuf[nCnt+11], nReceiveBuf[nCnt+12], nReceiveBuf[nCnt+9], nReceiveBuf[nCnt+10]);
//            sprintf(line,"%s [%d]: %10.3f",sPTDataName[i],(i*2),fModbusTcpValue[i]);
//            mmModbusTcp->Lines->Add(line);
//         }
//      }
//      else if(nReceiveLenth==120 && GetModbusTcpAddress()==(11351-2)){
//         for(int i=0;i<30;i++){
//            nCnt=i*4;
//            fModbusTcpValue[i+28]=ConvIEEE754(nReceiveBuf[nCnt+11], nReceiveBuf[nCnt+12], nReceiveBuf[nCnt+9], nReceiveBuf[nCnt+10]);
//            sprintf(line,"%s [%d]: %10.3f",sPTDataName[i+28],i,fModbusTcpValue[i+28]);
//            mmModbusTcp->Lines->Add(line);
//         }
//      }
//      else if(nReceiveLenth==120 && GetModbusTcpAddress()==(11411-2)){
//         for(int i=0;i<30;i++){
//            nCnt=i*4;

//            if(i>=13 && i<=21){
//               fModbusTcpValue[i+58]=((int)nReceiveBuf[nCnt+11]*16777216)+((int)nReceiveBuf[nCnt+12]*65536)+((int)nReceiveBuf[nCnt+9]*256)+((int)nReceiveBuf[nCnt+10]);
//            }
//            else{
//               fModbusTcpValue[i+58]=ConvIEEE754(nReceiveBuf[nCnt+11], nReceiveBuf[nCnt+12], nReceiveBuf[nCnt+9], nReceiveBuf[nCnt+10]);
//            }
//            sprintf(line,"%s [%d]: %10.3f",sPTDataName[i+58],i+30,fModbusTcpValue[i+58]);
//            mmModbusTcp->Lines->Add(line);
//         }
//      }
//      else if(nReceiveLenth==24 && GetModbusTcpAddress()==(11471-2)){
//         for(int i=0;i<6;i++){
//            nCnt=i*4;
//            fModbusTcpValue[i+88]=ConvIEEE754(nReceiveBuf[nCnt+11], nReceiveBuf[nCnt+12], nReceiveBuf[nCnt+9], nReceiveBuf[nCnt+10]);
//            sprintf(line,"%s [%d]: %10.3f",sPTDataName[i+88],i+60,fModbusTcpValue[i+88]);
//            mmModbusTcp->Lines->Add(line);
//         }
//      }
//      else{
//         int nLoopCnt;

//         if(cbFloatMode->Checked){
//            nLoopCnt=nReceiveLenth/4;
//            for(int i=0;i<nLoopCnt;i++){
//               nCnt=i*4;
//               fModbusTcpValue[i]=ConvIEEE754(nReceiveBuf[nCnt+11], nReceiveBuf[nCnt+12], nReceiveBuf[nCnt+9], nReceiveBuf[nCnt+10]);
//               sprintf(line,"[%d]: %8.3f",i,fModbusTcpValue[i]);
//               mmModbusTcp->Lines->Add(line);
//            }
//         }
//         else{
//            nLoopCnt=nReceiveLenth/2;
//            for(int i=0;i<nLoopCnt;i++){
//               nCnt=i*2;
//               fModbusTcpValue[i]=(((int)nReceiveBuf[nCnt+9])*256)+((int)nReceiveBuf[nCnt+10]);
//               sprintf(line,"[%d]: %8.3f",i,fModbusTcpValue[i]);
//               mmModbusTcp->Lines->Add(line);
//            }
//         }
//      }
//   }
//   else if(nReceiveFunct==131 || nReceiveFunct==134 || nReceiveFunct==144 || nReceiveFunct==229){
//      if(nReceiveFunct==131){
//         if(nExceptioncode)      ShowMessage("Funct 131 ,Exceptioncode 2 : 지정한 레지스터 영역이 65536 범위를 벗어날 때.");
//         else if(nExceptioncode) ShowMessage("Funct 131 ,Exceptioncode 3 : 요청한 레지스터 개수가 0이거나 250보다 클 경우.");
//         else                    ShowMessage("Funct 131 ,Exceptioncode X");
//      }
//      else if(nReceiveFunct==134){
//         if(nExceptioncode)      ShowMessage("Funct 134 ,Exceptioncode 2");
//         else if(nExceptioncode) ShowMessage("Funct 134 ,Exceptioncode 3");
//         else                    ShowMessage("Funct 134 ,Exceptioncode X");
//      }
//      else if(nReceiveFunct==144){
//         if(nExceptioncode)      ShowMessage("Funct 144 ,Exceptioncode 2 : 지정한 레지스터 영역이 65536 범위를 벗어날 때.");
//         else if(nExceptioncode) ShowMessage("Funct 144 ,Exceptioncode 3 : 기록하려는 레지스터 개수가 0이거나 123보다 클 경우.");
//         else                    ShowMessage("Funct 144 ,Exceptioncode X");
//      }
//      else if(nReceiveFunct==229){
//         if(nExceptioncode)      ShowMessage("Funct 229 ,Exceptioncode 2 : 각 Block에서 지정한 레지스터 영역이 65536 범위를 벗어날 때.");
//         else if(nExceptioncode) ShowMessage("Funct 229 ,Exceptioncode 3 : Block 개수,word length, 0일 때, 너무 많은 수의 register를 요청하여 overflow발생");
//         else                    ShowMessage("Funct 229 ,Exceptioncode X");
//      }
//   }
//   else{

//   }





///루텍 장비는 무조건 502 번 사용.





// switch(m_nPowerReadIndex%6){    //KYS 140619 add
//      case 0:
//         ModbusTCPSend(3, 11044-1, 1);
//         break;
//      case 1:
//         ModbusTCPSend(3, 11101-2, 56);
//         break;
//      case 2:
//         ModbusTCPSend(3, 11351-2, 60);
//         break;
//      case 3:
//         ModbusTCPSend(3, 11411-2, 60);
//         break;
//      case 4:
//         ModbusTCPSend(3, 11471-2, 12);
//         break;
//      case 5:
//         {
//            for(int i=0;i<94;i++){
//               sPowerMeasuring.sprintf("%10.3f",fModbusTcpValue[i]);
//               impPowerMeasuring->Cells[4][i+1]=sPowerMeasuring;
//            }
//         }
//         break;
//   }

