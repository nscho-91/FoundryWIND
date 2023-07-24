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
using ezAxis; 
using ezTools;

namespace ezAuto_EFEM
{
    public partial class HW_Aligner_ATI_Edge : Form, Control_Child
    {
        enum eError
        {
            TCPTimeout,
            Protocol,
            Inspect,
            TCPConnect //KDG 161007 Add TCP Connect Error
        };

        enum eCmd
        {
            Start = 20000,
            BoatGo,
            Done,
            Ack,
            None
        };
        eCmd m_eCmdSend = eCmd.None; 

        string m_id;
        Auto_Mom m_auto;
        public Axis_Mom m_axisRotate; 
        Log m_log;
        ezGrid m_grid;
        Size[] m_sz = new Size[2];

        XGem300_Mom m_xGem;
        Work_Mom m_work;
        Recipe_Mom m_recipe;
        Control_Mom m_control;
        Handler_Mom m_handler;
        ezTCPSocket m_socket;

        byte[] m_bufSend = new byte[4096]; 
        int c_msTCP = 3000;
        const int c_msEdgeInspect = 120000;     //KDG 161028 Add Delay

        Info_Wafer m_infoWafer = null; 

        public bool m_bUse; 

        double m_posStart = 0;
        double m_dpGrap = 40000;
        double m_dpAcc = 1000;
        double m_vGrab = 1000;
        double m_dpTrigger = 1; 
        bool m_bTrigger = false;

        string m_strLotStart = "0"; 
        
        bool m_bInspect = false;

        ezRS232 m_rs232 = null;
        byte[] m_aByteRead232 = new byte[4096];
        char[] m_aBufRead232 = new char[4096];
        char[] m_aBufWrite232 = new char[4096];
        string m_sBuf232 = "";

        int[] m_aPowerMax = new int[4] { 300, 1000, 1000, 1000 };
        public double[,] m_aPowerLED = new double[2, 4] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }}; 
        
        Axis_Mom m_axisX;
        int[] m_xEdge = new int[(int)Wafer_Size.eSize.Empty];

        Wafer_Size m_wafer; 
        
        public HW_Aligner_ATI_Edge()
        {
            InitializeComponent();
            m_sz[0] = m_sz[1] = this.Size;
            m_sz[0].Height = 26;
        }

        public virtual void Init(string id, Auto_Mom auto, Axis_Mom axisRotate, Wafer_Size wafer)
        {
            m_id = id;
            m_auto = auto;
            m_axisRotate = axisRotate;
            m_wafer = wafer; 
            checkView.Text = m_id;
            m_log = new Log(m_id, m_auto.ClassLogView(), "Aligner");
            m_grid = new ezGrid(m_id, grid, m_log, false);
            m_xGem = m_auto.ClassXGem();
            m_work = m_auto.ClassWork();
            m_recipe = m_auto.ClassRecipe();
            m_control = m_auto.ClassControl();
            m_control.Add(this);
            m_handler = m_auto.ClassHandler();
            m_infoWafer = new Info_Wafer(0, 0, 0, m_log); 
            m_socket = new ezTCPSocket(m_id, m_log, true);
            m_rs232 = new ezRS232("RS232", m_log, false);
            RunGrid(eGrid.eRegRead);
            RunGrid(eGrid.eInit);
            m_socket.Init();
            m_socket.CallMsgRcv += m_socket_CallMsgRcv;
            m_rs232.Connect(true);
            m_rs232.CallMsgRcv += m_rs232_CallMsgRcv;
            InitString(); 
        }

        public void ThreadStop()
        {
            m_socket.ThreadStop();
            m_rs232.ThreadStop();
        }

        void InitString()
        {
            InitString(eError.TCPTimeout, "TCPIP Communication Timeover !!, VisionWorks를 확인하여 주세요.");
            InitString(eError.Protocol, "TCPIP Invalid Protocol !!, VisionWorks를 확인하여 주세요.");
            InitString(eError.Inspect, "Inspect not Enable !!");
            InitString(eError.TCPConnect, "Edge Vision P/G not Connect !!");
        } 

        void InitString(eError eErr, string str)
        {
            m_log.AddString(str);
            if (m_xGem == null) return;
            m_xGem.AddALID(m_id, (int)eErr, str);
        } 

        void SetAlarm(eAlarm alarm, eError eErr)
        {
            m_work.SetError(alarm, m_log, (int)eErr);
            if (m_xGem == null) return;
            m_xGem.SetAlarm(m_id, (int)eErr);
        } 

        public void ControlGrid(Control_Mom control, ezGrid rGrid, eGrid eMode)
        {
            m_axisX = control.AddAxis(rGrid, m_id, "CamX", "Axis Camera X");
        }

        void RunGrid(eGrid eMode)
        {
            m_grid.Update(eMode, null);
            m_socket.RunGrid(m_grid);
            m_grid.Set(ref m_infoWafer.m_strWaferID, "Test", "WaferID", "Wafer ID");
            m_grid.Set(ref m_dpAcc, "Grab", "Acc", "Acc pos Margin (pulse)");
            m_grid.Set(ref m_posStart, "Grab", "Start", "Start Position (pulse)");
            m_grid.Set(ref m_dpGrap, "Grab", "Grap", "Grap Range (pulse)");
            m_grid.Set(ref m_vGrab, "Grab", "V", "Grab Speed (pulse/s)");
            m_grid.Set(ref m_dpTrigger, "Grab", "Period", "Trigger Period (pulse)");
            m_grid.Set(ref m_bTrigger, "Grab", "Trigger", "True = Actual, False = Commend");
            for (int nSize = 0; nSize < (int)(Wafer_Size.eSize.Empty); nSize++)
            {
                m_grid.Set(ref m_xEdge[nSize], ((Wafer_Size.eSize)nSize).ToString(), "Align", "Align Pos (pulse)", false, m_wafer.m_bEnable[nSize]);
            }
            m_grid.Set(ref c_msTCP, "Grab", "TCP Check Delay", "Delay(ms)");    //KDG 161028 Add TCP Delay
            m_rs232.RunGrid(m_grid, eMode);
            m_grid.Refresh(); 
        }

        public void RecipeGrid(bool bRAC, ezGrid rGrid, eGrid eMode, ezJob job)
        {
            //rGrid.Set(ref m_bUse, "Edge", "Use", "Use Edge Inspect"); 
            rGrid.Set(ref m_recipe.m_bRunEdge, "Edge", "Use", "Use Edge Inspect");
            for (int n = 0; n < 4; n++)
            {
                rGrid.Set(ref m_aPowerLED[0, n], "Edge", "Light_" + n.ToString(), "LED Power (0 ~ 100)");
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

        private void checkView_CheckedChanged(object sender, EventArgs e)
        {
            m_handler.ShowChild();
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            if (EdgeInspectionReady()) return;  //KDG 160927 Add
            SendStart(m_infoWafer, m_posStart, m_dpGrap); 
        }

        private void checkBoxLight_CheckedChanged(object sender, EventArgs e)
        {
            RunLED(checkBoxLight.Checked); 
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate);
            RunGrid(eGrid.eRegWrite);
        }

        public bool RunInspect()
        {
            if (m_axisRotate.WaitReady()) return true;
            if (m_axisX.WaitReady()) return true;
            m_axisRotate.SetTrigger(m_posStart, m_posStart + m_dpGrap + m_dpAcc, m_dpTrigger, m_bTrigger, 1);
            m_axisRotate.MoveV(m_posStart + m_dpGrap + m_dpAcc, m_vGrab);
            if (m_axisRotate.WaitReady()) return true;
            m_axisRotate.StopTrigger(); 
            SendDone();
            RunLED(false);
            m_bInspect = false;
            return false; 
        }

        public bool WaitDone()
        {
            ezStopWatch sw = new ezStopWatch();
            while (m_bInspect && (sw.Check() <= c_msEdgeInspect)) Thread.Sleep(10);
            m_bInspect = false;
            return (sw.Check() >= c_msEdgeInspect);
        }

        public void LotStart()
        {
            m_strLotStart = "1"; 
        }

        public eTCPResult SendStart(Info_Wafer infoWafer, double posStart, double dpGrap)
        {
            RunLED(true);
            //m_axisRotate.Move(m_posStart - m_dpAcc);
            //m_axisX.Move(m_xEdge[(int)m_wafer.m_eSize]); 
            m_bInspect = true;
            m_posStart = posStart;
            m_dpGrap = dpGrap;
            Info_Carrier infoCarrier = m_handler.ClassCarrier(m_infoWafer.m_nLoadPort); 
            string strSlot = infoWafer.m_nSlot.ToString();
            string strLotEnd = infoCarrier.GetEdgeLotEnd(); 
            string strLotID = infoCarrier.m_strLotID;
            if (m_recipe.m_sRecipe == "")
            {
                m_log.Popup("Recipe not Selected");
                return eTCPResult.Error; 
            }
            if (strLotID == "")
            {
                m_log.Popup("Recipe not Selected");
                return eTCPResult.Error;
            }
            string strMsg = infoWafer.m_strWaferID + "," + m_recipe.m_sRecipe + "," + strSlot + "," + strLotEnd + "," + strLotID + "," + m_strLotStart;
            eTCPResult eResult = SendCmd(eCmd.Start, strMsg);
            if (eResult != eTCPResult.OK) return eResult;
            return WaitReply(c_msTCP);
        }

        eTCPResult SendDone()
        {
            m_strLotStart = "0"; 
            eTCPResult eResult = SendCmd(eCmd.Done);
            if (eResult != eTCPResult.OK) return eResult;
            return WaitReply(c_msTCP);
//            return eTCPResult.OK; 
        }
        
        eTCPResult SendCmd(params object[] objs)
        {
            int lBuf = 4;
            string strLog = "==> ";
            if (m_eCmdSend != eCmd.None) return eTCPResult.Busy;
            if (objs.Length <= 0) return eTCPResult.Error;
            if (!m_socket.IsConnect()) return eTCPResult.Error;
            foreach (object obj in objs)
            {
                lBuf = SetBuf(obj, lBuf, ref strLog);
            }
            SetBuf(lBuf, 0, ref strLog);
            m_log.Add(strLog);
            m_eCmdSend = (eCmd)objs[0];
            return m_socket.Send(m_bufSend, lBuf);
        }

        int SetBuf(object obj, int lBuf, ref string strLog)
        {
            Union_UInt union = new Union_UInt();
            string strType = obj.GetType().ToString();
            switch (strType)
            {
                case "ezAuto_EFEM.HW_Aligner_ATI_Edge+eCmd":
                    union.m_uint = (uint)(eCmd)obj;
                    m_bufSend[lBuf++] = union.m_byte0;
                    m_bufSend[lBuf++] = union.m_byte1;
                    m_bufSend[lBuf++] = union.m_byte2;
                    m_bufSend[lBuf++] = union.m_byte3;                  
                    strLog += ((eCmd)obj).ToString();
                    return lBuf;
                case "System.Int32":
                    union.m_uint = (uint)(int)obj;
                    m_bufSend[lBuf++] = union.m_byte0;
                    m_bufSend[lBuf++] = union.m_byte1;
                    m_bufSend[lBuf++] = union.m_byte2;
                    m_bufSend[lBuf++] = union.m_byte3;
                    strLog += ", " + union.m_uint.ToString();
                    return lBuf;
                case "System.String":
                    string str = (string)obj;
                    lBuf = SetBuf(str.Length, lBuf, ref strLog);
                    foreach (char ch in str)
                    {
                        m_bufSend[lBuf++] = (byte)ch;
                    }
                    strLog += ", " + str;
                    return lBuf;
            }
            return lBuf;
        }

        eTCPResult WaitReply(int msWait)
        {
            int ms = 0;
            while (m_eCmdSend != eCmd.None)
            {
                Thread.Sleep(10);
                ms += 10;
                if (ms > msWait)
                {
                    m_log.Popup("Cmd : " + m_eCmdSend.ToString());
                    SetAlarm(eAlarm.Warning, eError.TCPTimeout);
                    m_eCmdSend = eCmd.None;
                    return eTCPResult.Timeout;
                }
            }
            m_eCmdSend = eCmd.None;
            return eTCPResult.OK;
        }

        void m_socket_CallMsgRcv(byte[] buf, int nSize)
        {
            byte[] buffer = new byte[nSize];
            string strLog = "<== ";
            string strHex = "<== ";
            for (int n = 0; n < nSize; n++)
            {
                strHex += Convert.ToString(buf[n], 16) + " ";
                buffer[n] = buf[n];
            }
            m_log.Add(strHex);
            int nIndex = 0;
            if (nSize < GetInt(buffer, ref nIndex, ref strLog))
            {
                SetAlarm(eAlarm.Stop, eError.Protocol);
                m_log.Add(strLog);
                return;
            }
            eCmd nCmd = GetCmd(buffer, ref nIndex, ref strLog);
            switch (nCmd)
            {
                case eCmd.BoatGo:
                    m_eCmdSend = eCmd.None; 
                    m_log.Add(strLog);
                    RunInspect(); 
                    break;
                case eCmd.Ack:
                    m_eCmdSend = eCmd.None;     //KDG 161028 Add State None 
                    break;
            }

        }

        eCmd GetCmd(byte[] buf, ref int nIndex, ref string strLog)
        {
            Union_UInt union = new Union_UInt();
            union.m_byte0 = buf[nIndex++];
            union.m_byte1 = buf[nIndex++];
            union.m_byte2 = buf[nIndex++];
            union.m_byte3 = buf[nIndex++];
            eCmd nCmd = (eCmd)union.m_uint;
            strLog += nCmd.ToString() + ", ";
            return nCmd;
        }

        byte GetByte(byte[] buf, ref int nIndex, ref string strLog)
        {
            strLog += buf[nIndex] + ", ";
            return buf[nIndex++];
        }

        int GetInt(byte[] buf, ref int nIndex, ref string strLog)
        {
            Union_UInt union = new Union_UInt();
            union.m_byte0 = buf[nIndex++];
            union.m_byte1 = buf[nIndex++];
            union.m_byte2 = buf[nIndex++];
            union.m_byte3 = buf[nIndex++];
            strLog += union.m_uint.ToString() + ", ";
            return (int)union.m_uint;
        }

        string GetString(byte[] buf, ref int nIndex, ref string strLog)
        {
            int nLength = GetInt(buf, ref nIndex, ref strLog);
            string str = Encoding.Default.GetString(buf, nIndex, nLength);
            strLog += str + ", ";
            nIndex += nLength;
            return str;
        }

        public bool RunLED(bool bOn)    //KDG 161028 public으로 변경
        {
            if (bOn)
            {
                for (int n = 0; n < 4; n++) SendRS232(n, m_aPowerLED[0, n]);    
            }
            else
            {
                for (int n = 0; n < 4; n++) SendRS232(n, 0);
            }
            return false; 
        }
        
        bool SendRS232(int nCh, double fPower)
        {
            if (nCh < 0) return true;
            if (nCh > 3) return true;

            if (fPower == m_aPowerLED[1, nCh]) return false;
            m_aPowerLED[1, nCh] = fPower;
            
            int nPower = (int)(fPower * m_aPowerMax[nCh] / 100);
            m_sBuf232 = "led " + nCh.ToString() + " " + nPower.ToString();

            string strCmd = m_sBuf232 + (char)0x0D + (char)0x0A;
            m_aBufWrite232 = strCmd.ToCharArray();
            Thread.Sleep(10); 
            m_rs232.Write(m_aBufWrite232, strCmd.Length, false);

            m_log.Add("-> RS232 -> " + m_sBuf232);
            Thread.Sleep(20); 
            return WaitReply(); 
        }

        bool WaitReply()
        {
            int ms = 0; 
            while (m_sBuf232 != "")
            {
                Thread.Sleep(10);
                ms += 10; 
                if (ms > 1000)
                {
                    m_log.Popup("RS232 Comm Timeover !!");
                    return true; 
                }
            }
            return false; 
        }

        void m_rs232_CallMsgRcv()
        {
            int nRead = m_rs232.Read(m_aBufRead232, 4096);
            for (int n = 0; n < nRead - 2; n++)
            {
                m_aByteRead232[n] = (byte)m_aBufRead232[n];
            }
            string sMsg = Encoding.Default.GetString(m_aByteRead232, 0, m_sBuf232.Length);
            m_log.Add("<< RS232 << " + sMsg); 
            if (m_sBuf232 == sMsg) m_sBuf232 = ""; 
        }

        public bool EdgeInspectionReady()   //KDG 160927 Add
        {
            m_axisRotate.Move(m_posStart - m_dpAcc);
            m_axisX.Move(m_xEdge[(int)m_wafer.m_eSize]);
            if (m_axisRotate.WaitReady()) return true;
            if (m_axisX.WaitReady()) return true;

            return false;
        }
        public bool IsConnected()     //KDG 161007 Add Edge Vision 연결상태
        {
            bool bResult = m_socket.IsConnect();

            if (!bResult)
                SetAlarm(eAlarm.Warning, eError.TCPConnect);

            return bResult;
        }
    }
}
