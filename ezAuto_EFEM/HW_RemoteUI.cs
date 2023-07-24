using System;
using System.Collections;
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
    public partial class HW_RemoteUI : Form
    {
        enum eCmd
        {
            Stat_EFEM,
            Stat_Vision,
            Stat_Loadport,
            Stat_Aligner,
            Cmd_Home,
            Cmd_Start,
            Cmd_Stop,
            Cmd_CycleStop,
            Cmd_BuzzerOff,
            Cmd_Reset,
            Cmd_Recovery,
            Cmd_Recipe,
            Cmd_GemMode,
            Cmd_Load,
            Cmd_JobCencel,
            Cmd_JobCreate,
            Cmd_JobRunning,
            Cmd_JobStart,
            None
        }
        eCmd m_eCmd = eCmd.None;
        string m_strStat = "";

        string m_id;
        Auto_Mom m_auto;
        Work_Mom m_work; 
        Handler_Mom m_handler;
        HW_VisionWorks_Mom m_vision;
        HW_Aligner_Mom m_align;
        XGem300_Mom m_xGem; 
        Log m_log;
        ezGrid m_grid;

        ezTCPSocket m_socket;
        Queue m_qSend = new Queue();

        bool m_bRun = false;
        Thread m_thread;

        int m_msTimeout = 2000;
        int m_msPeriod = 10000; 

        bool m_bEnable = false;
        Size[] m_sz = new Size[2];

        bool[] m_bSendCreadJob = new bool[2]; // ing 170521

        public HW_RemoteUI()
        {
            InitializeComponent();
            m_swLoadport[0] = new ezStopWatch();
            m_swLoadport[1] = new ezStopWatch();
            m_sz[0] = m_sz[1] = this.Size;
            m_sz[0].Height = 26;
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

        public void Init(string id, Auto_Mom auto)
        {
            m_id = id;
            m_auto = auto;
            m_work = m_auto.ClassWork(); 
            m_handler = m_auto.ClassHandler();
            m_vision = m_handler.ClassVisionWorks();
            m_align = m_handler.ClassAligner();
            m_xGem = m_auto.ClassXGem(); 
            m_log = new Log(m_id, m_auto.ClassLogView(), m_id);
            m_grid = new ezGrid(m_id, grid, m_log, false);
            m_socket = new ezTCPSocket(m_id, m_log, true);
            RunGrid(eGrid.eRegRead);
            RunGrid(eGrid.eInit);
            if (!m_bEnable) return; // ing 170522
            m_socket.Init();
            m_socket.CallMsgRcv += CallMsgRcv;
            m_thread = new Thread(new ThreadStart(RunThread));
            m_thread.Start();
        }

        public void ThreadStop()
        {
            if (m_bRun) { m_bRun = false; m_thread.Join(); }
        }

        public void ShowDlg(Form parent, ref CPoint cpShow)
        {
            if (m_bEnable == false)
            {
                Hide();
                return;
            }
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

        public void ModelGrid(ezGrid rGrid, eGrid eMode)
        {
            rGrid.Set(ref m_bEnable, "RemoteUI", "Enable", "Enable RemoteUI");
        }

        protected void RunGrid(eGrid eMode)
        {
            m_grid.Update(eMode, null);
            m_socket.RunGrid(m_grid);
            m_grid.Set(ref m_msTimeout, "Time", "Timeout", "TCP/IP Timeput (ms)");
            m_grid.Set(ref m_msPeriod, "Time", "Period", "TCP/IP State Send Period (ms)"); 
            m_grid.Refresh();
        }

        void RunThread()
        {
            ezStopWatch sw = new ezStopWatch();
            m_bRun = true;
            Thread.Sleep(5000);
            while (m_bRun)
            {
                Thread.Sleep(1000); //forget
                if ((m_eCmd != eCmd.None) && (sw.Check() > m_msTimeout))
                {
                    //m_log.Popup("Send Cmd is Busy : " + m_eCmd.ToString());
                    m_eCmd = eCmd.None;
                }
                else if (m_eCmd == eCmd.None)
                {
                    sw.Start(); 
                    StatEFEM();
                    StatVision();
                    StatLoadport(0);
                    StatLoadport(1);
                    StatLoadport(2);
                    StatAligner();
                }
                if (m_qSend.Count > 0)
                {
                    string str = (string)m_qSend.Dequeue();
                    m_log.Add("--> " + str);
                    m_socket.Send(str);
                }

                for (int i = 0; i < m_auto.ClassHandler().m_nLoadPort;i++ )
                {
                    if (m_eLoadport[i] == HW_LoadPort_Mom.eState.LoadDone && m_xGem.m_eControl == XGem300_Mom.eControl.OFFLINE && !m_bSendCreadJob[i] && m_auto.ClassWork().GetState() == eWorkRun.Run) // ing 170521
                    {
                        Info_Carrier infoCarrier = m_handler.ClassLoadPort(i).m_infoCarrier;
                        AddCmd(eCmd.Cmd_JobCreate, i.ToString(), infoCarrier.m_strCarrierID, infoCarrier.m_strLotID, infoCarrier.GetSlotMap());
                        m_bSendCreadJob[i] = true;
                    }
                }
            }
        }

        ezStopWatch m_swEFEM = new ezStopWatch();
        eWorkRun m_eEFEM = eWorkRun.Init;
        XGem300_Mom.eControl m_eXgemMode = XGem300_Mom.eControl.OFFLINE; 

        void StatEFEM()
        {
            bool bChange = false;
            bool bJobRunning = m_handler.ClassLoadPort(0).m_infoCarrier.m_eState == Info_Carrier.eState.Run || m_handler.ClassLoadPort(1).m_infoCarrier.m_eState == Info_Carrier.eState.Run || m_handler.ClassLoadPort(2).m_infoCarrier.m_eState == Info_Carrier.eState.Run;
            bool bEnableRecovery = m_work.m_run.IsEnableSW() && (m_work.GetState() == eWorkRun.Ready) && !m_handler.m_bEnableStart;

            eWorkRun eEFEM = m_work.m_run.GetState();
            if (eEFEM != m_eEFEM) bChange = true;

            XGem300_Mom.eControl eXgemMode = m_xGem.m_eControl;
            if (eXgemMode != m_eXgemMode) bChange = true;

            if ((bChange == false) && (m_swEFEM.Check() < m_msPeriod)) return;
            m_eEFEM = eEFEM;
            m_eXgemMode = eXgemMode;
            AddCmd(eCmd.Stat_EFEM, m_eEFEM, m_eXgemMode, bJobRunning, bEnableRecovery);
            m_swEFEM.Start();
            m_strStat = eCmd.Stat_EFEM.ToString();
        }

        ezStopWatch m_swVision = new ezStopWatch();
        HW_VisionWorks_Mom.eState m_eVision = HW_VisionWorks_Mom.eState.Init;
        bool m_bVisionConnect = false;
        eHWResult m_bVisionWaferExist = eHWResult.Off;
        bool m_bVisionLiftUp = false;
        Info_Wafer m_iwVision = null; 

        void StatVision()
        {
            bool bChange = false;

            HW_VisionWorks_Mom.eState eVision = m_vision.GetState();
            if (eVision != m_eVision) bChange = true; 

            bool bVisionConnect = m_vision.IsConnected();
            if (bVisionConnect != m_bVisionConnect) bChange = true; 

            eHWResult bVisionWaferExist = m_vision.IsWaferExist();
            if (bVisionWaferExist != m_bVisionWaferExist) bChange = true; 

            bool bVisionLiftUp = m_vision.IsLifterUp();
            if (bVisionLiftUp != m_bVisionLiftUp) bChange = true; 

            Info_Wafer iwVision = m_vision.InfoWafer;
            if (iwVision != m_iwVision) bChange = true; 

            if ((bChange == false) && (m_swVision.Check() < m_msPeriod)) return;
            m_eVision = eVision;
            m_bVisionConnect = bVisionConnect;
            m_bVisionLiftUp = bVisionLiftUp;
            m_iwVision = iwVision; 
            AddCmd(eCmd.Stat_Vision, m_eVision, m_bVisionConnect, m_bVisionWaferExist, m_bVisionLiftUp, GetWaferString(m_iwVision)); 
            m_swVision.Start();
            m_strStat = eCmd.Stat_Vision.ToString();
        }

        ezStopWatch[] m_swLoadport = new ezStopWatch[3];
        HW_LoadPort_Mom.eState[] m_eLoadport = new HW_LoadPort_Mom.eState[3] { HW_LoadPort_Mom.eState.Init, HW_LoadPort_Mom.eState.Init, HW_LoadPort_Mom.eState.Init }; 
        
        void StatLoadport(int nID)
        {
            HW_LoadPort_Mom.eState eLoadport = m_handler.ClassLoadPort(nID).GetState();
            Info_Carrier infoCarrier = m_handler.ClassLoadPort(nID).m_infoCarrier;
            bool bPlaced = m_handler.ClassLoadPort(nID).IsPlaced();
            if ((m_eLoadport[nID] == eLoadport) && (m_swLoadport[nID].Check() < m_msPeriod)) return;
            m_eLoadport[nID] = eLoadport;
            if (m_eLoadport[nID] != HW_LoadPort_Mom.eState.LoadDone) m_bSendCreadJob[nID] = false; // ing 170521
            AddCmd(eCmd.Stat_Loadport, nID, m_eLoadport[nID], bPlaced, infoCarrier.m_strCarrierID, infoCarrier.m_strLotID, infoCarrier.m_strRecipe);
            m_swLoadport[nID].Start();
            m_strStat = eCmd.Stat_Loadport.ToString();
        }

        ezStopWatch m_swAligner = new ezStopWatch();
        eHWResult m_bAlignerWaferExist = eHWResult.Off;
        Info_Wafer m_iwAlign = null; 
        
        void StatAligner()
        {
            bool bChange = false;

            eHWResult bAlignerWaferExist = m_align.IsWaferExist();
            if (bAlignerWaferExist != m_bAlignerWaferExist) bChange = true; 

            Info_Wafer iwAlign = m_align.InfoWafer;
            if (iwAlign != m_iwAlign) bChange = true; 

            if ((bChange == false) && (m_swAligner.Check() < m_msPeriod)) return;
            m_bAlignerWaferExist = bAlignerWaferExist;
            m_iwAlign = iwAlign; 
            AddCmd(eCmd.Stat_Aligner, m_bAlignerWaferExist, GetWaferString(m_iwAlign)); 
            m_swAligner.Start();
            m_strStat = eCmd.Stat_Aligner.ToString();
        }

        void AddCmd(params object[] objs)
        {
            string str = "";
            if (objs.Length <= 0) return;
            if (!m_socket.IsConnect()) return;
            m_eCmd = (eCmd)objs[0];
            foreach (object obj in objs)
            {
                str += obj.ToString() + ",";
            }
            m_qSend.Enqueue(str);
        }

        string GetWaferString(Info_Wafer infoWafer)
        {
            if (infoWafer == null) return " , , , ";
            return infoWafer.ToString(); 
        }

        void CallMsgRcv(byte[] buf, int nSize)
        {
            string sMsg = Encoding.Default.GetString(buf, 0, nSize);
            string[] sMsgs = sMsg.Split(',');
            m_log.Add("<-- " + sMsg);
            if (sMsgs.Length <= 0) return;
            CallMsgRcv(sMsg, sMsgs);
        }

        void CallMsgRcv(string sMsg, string[] sMsgs)
        {
            eCmd nCmd = GetCmd(sMsgs[0]);
            switch (nCmd)
            {
                case eCmd.Stat_EFEM:
                    //if (m_strStat != sMsg) m_log.Popup("Stat_EQ Cmd MissMatch !!");
                    break;
                case eCmd.Stat_Vision:
                    //if (m_strStat != sMsg) m_log.Popup("Stat_Vision Cmd MissMatch !!");
                    break;
                case eCmd.Stat_Loadport:
                    //if (m_strStat != sMsg) m_log.Popup("Stat_Loadport Cmd MissMatch !!");
                    break;
                case eCmd.Stat_Aligner:
                    //if (m_strStat != sMsg) m_log.Popup("Stat_Aligner Cmd MissMatch !!");
                    break;
                case eCmd.Cmd_Home:
                    m_qSend.Enqueue(sMsg);
                    CmdHome();
                    break;
                case eCmd.Cmd_Start:
                    m_qSend.Enqueue(sMsg);
                    CmdStart();
                    break;
                case eCmd.Cmd_Stop:
                    m_qSend.Enqueue(sMsg);
                    CmdStop();
                    break;
                case eCmd.Cmd_CycleStop:
                    m_qSend.Enqueue(sMsg);
                    CmdCycleStop();
                    break;
                case eCmd.Cmd_BuzzerOff:
                    m_qSend.Enqueue(sMsg);
                    CmdBuzzerOff();
                    break;
                case eCmd.Cmd_Reset:
                    m_qSend.Enqueue(sMsg);
                    CmdReset();
                    break;
                case eCmd.Cmd_Recovery:
                    m_qSend.Enqueue(sMsg);
                    CmdRecovery();
                    break;
                case eCmd.Cmd_Recipe:
                    m_qSend.Enqueue(sMsg);
                    if (sMsgs.Length > 1) CmdRecipe(sMsgs[1]);
                    break;
                case eCmd.Cmd_GemMode:
                    m_qSend.Enqueue(sMsg);
                    if (sMsgs.Length > 1) CmdGemMode(sMsgs[1]);
                    break;
                case eCmd.Cmd_Load:
                    m_qSend.Enqueue(sMsg);
                    if (sMsgs.Length > 1) CmdLoad(sMsgs[1]);
                    break;
                case eCmd.Cmd_JobCencel:
                    m_qSend.Enqueue(sMsg);
                    if (sMsgs.Length > 1) CmdJobCencel(sMsgs[1]);
                    break;
                case eCmd.Cmd_JobCreate:
                    //m_qSend.Enqueue(sMsg);
                    break;
                case eCmd.Cmd_JobStart:
                    m_qSend.Enqueue(sMsg);
                    if (sMsgs.Length > 1) CmdJobStart(sMsgs[1], sMsgs[2], sMsgs[3], sMsgs[4], sMsgs[5]);
                    break;
            }
            if (m_eCmd == nCmd)
            {
                m_eCmd = eCmd.None;
                m_strStat = "";
            }
        }

        eCmd GetCmd(string strCmd)
        {
            for (int n = 0; n < (int)eCmd.None; n++)
            {
                if (strCmd == ((eCmd)n).ToString()) return (eCmd)n;
            }
            return eCmd.None;
        }

        void CmdHome()
        {
            m_work.m_run.Home(); 
        }

        void CmdStart()
        {
            m_handler.RunStart(); 
        }

        void CmdStop()
        {
            m_work.m_run.Stop(); 
        }

        void CmdCycleStop()
        {
            if (m_work.IsRun())
            {
                m_handler.ClassWTR().m_bCycleStop = true;
            }
        }

        void CmdBuzzerOff()
        {
            m_work.RunBuzzerOff(); 
        }

        void CmdReset()
        {
            if (m_work.m_bTestRun == false) m_work.m_run.Reset();
        }

        void CmdRecovery()
        {
            m_handler.RunRecover(); 
        }

        void CmdRecipe(string sMsg)
        {
            m_auto.ClassRecipe().JobOpen(sMsg); 
        }

        void CmdGemMode(string sMsg)
        {
            if (m_xGem == null) return; 
            switch (sMsg)
            {
                case "OffLine": m_xGem.XGemOffline(); break;
                case "Local": m_xGem.XGemOnlineLocal(); break;
                case "Remote": m_xGem.XGemOnlineRemote(); break;
            }
        }

        void CmdLoad(string sMsg) // ing 170521
        {
            int nID = Convert.ToInt32(sMsg);
            HW_LoadPort_Mom lp = m_handler.ClassLoadPort(nID);
            if (!(lp.IsCoverClose()))
            {
                m_log.Popup("Please Cover Close !!");
                return;
            }
            if (m_work.GetState() != eWorkRun.Run && !m_auto.ClassHandler().IsWaferExistInEQP() && !((EFEM_Handler)m_auto.ClassHandler()).m_bManualRecovery)
            {
                m_log.Popup("Please Push Start Button");
                return;
            }
            if (lp.m_bRFIDUse || lp.m_bUseBCR)
                lp.SetState(HW_LoadPort_Mom.eState.CSTIDRead);
            else
                lp.SetState(HW_LoadPort_Mom.eState.RunLoad);
            //m_handler.ClassLoadPort(nID).RunLoad(true); 
        }

        void CmdJobStart(string sMsg0, string sMsg1, string sMsg2, string sMsg3, string sMsg4)
        {
            Info_Carrier infoCarrier = m_handler.ClassLoadPort(0).m_infoCarrier;
            if (infoCarrier.m_eState != Info_Carrier.eState.ManualJob) return;
            int nID = Convert.ToInt32(sMsg0);
            infoCarrier.m_strCarrierID = sMsg1;
            infoCarrier.m_strLotID = sMsg2;
            infoCarrier.m_strRecipe = sMsg3;
            for (int n = 0; n < sMsg4.Length; n++)
            {
                if (sMsg4[n] == '1')
                {
                    Info_Wafer infoWafer = infoCarrier.m_infoWafer[n];// new Info_Wafer(nID, n, n + 1, m_log, m_handler.ClassLoadPort(nID).m_infoCarrier.m_wafer.m_eSize);
                    infoWafer.State = Info_Wafer.eState.Select;
                    m_auto.ClassRecipe().SetInfoWafer(infoWafer, null);
                }
            }
            infoCarrier.m_eState = Info_Carrier.eState.Ready; //job Start
        }

        void CmdJobCencel(string sMsg0)
        {
            Info_Carrier infoCarrier = m_handler.ClassLoadPort(0).m_infoCarrier;
            if (infoCarrier.m_eState != Info_Carrier.eState.ManualJob) return;
            int nID = Convert.ToInt32(sMsg0);
            m_handler.ClassLoadPort(nID).m_infoCarrier.m_eState = Info_Carrier.eState.Done; //job Cencel
            m_handler.ClassLoadPort(nID).SetState(HW_LoadPort_Mom.eState.RunUnload);
        }
    }

}
