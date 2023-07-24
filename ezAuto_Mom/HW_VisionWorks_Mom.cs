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

namespace ezAutoMom
{
    public partial class HW_VisionWorks_Mom : Form
    {
        public enum eState
        {
            Init,
            Home,
            LoadWait,
            Load,
            Ready,
            LoadDone,
            Run,
            Done,
            UnLoadDone,
            Error,
        }      
       
        public string m_id;
        protected Auto_Mom m_auto;
        Size[] m_sz = new Size[2];
        protected Log m_log;
        protected ezGrid m_grid;
        protected Work_Mom m_work;
        protected Control_Mom m_control;
        protected Handler_Mom m_handler;
        protected XGem300_Mom m_xGem;
        protected eState m_eState = eState.Init;
        protected Info_Wafer m_infoWafer = null;      
        protected bool m_bRunThread = false;
        Thread m_thread;
        protected ezTCPSocket m_socket;

        protected Wafer_Size m_wafer = null;
        protected Info_Wafer m_infoWaferTest = null;
        protected Info_Carrier m_infoCarrierTest = null;
       
        public bool m_bWaferExist = false;
        public bool m_bACK = false;
        public bool m_bManualWaferExist = false;
        int m_fWPH = 0;
        //Queue m_qWPH = new Queue();
        int[] m_WPHTime = new int[Enum.GetNames(typeof(Wafer_Size.eSize)).Length];
        ezStopWatch m_swWPH = new ezStopWatch();
        protected bool m_bEnableVision = true; // ing 170327
        
        public int m_msHome = 30000;

        //add by kiwon 211213
        public string m_strRecipePath = "D:\\WisVision\\Recipe";

        public HW_VisionWorks_Mom()
        {
            InitializeComponent();
            m_sz[0] = m_sz[1] = this.Size;
            m_sz[0].Height = 26;
        }

        public virtual void Init(string id, Auto_Mom auto)
        {
            m_id = id;
            m_auto = auto;
            checkView.Text = m_id;
            m_log = new Log(m_id, m_auto.m_logView, "VisionWorks");
            m_grid = new ezGrid(m_id, grid, m_log, false);
            m_work = m_auto.ClassWork();
            m_control = m_auto.ClassControl();
            m_handler = m_auto.ClassHandler();
            m_xGem = m_auto.ClassXGem();
            m_socket = new ezTCPSocket(m_id, m_log, true);
            m_wafer = new Wafer_Size(m_log);
            m_infoWaferTest = new Info_Wafer(0, 0, 1, m_log);
            m_infoCarrierTest = new Info_Carrier(0);

            RunGrid(eGrid.eRegRead);
            RunGrid(eGrid.eInit);
            m_socket.Init();
            m_socket.CallMsgRcv += CallMsgRcv;

            m_thread = new Thread(new ThreadStart(RunThread)); 
            m_thread.Start();

            m_swWPH.Stop();
            LoadLastWPHTime();
        }

        protected virtual bool SendSocket(string str)
        {
            if (!m_bEnableVision) return true;
            if (m_socket.Send(str) == eTCPResult.OK) return true;
            else return false;
        }

        protected virtual void CallMsgRcv(byte[] byteMsg, int nSize)
        {
          //  throw new NotImplementedException();          
        }

        public void ThreadStop()
        {
            m_socket.ThreadStop();
            if (m_bRunThread) 
            { 
                m_bRunThread = false; 
                m_thread.Join(); 
            }
        }

        private void checkView_CheckedChanged(object sender, EventArgs e)
        {
            m_handler.ShowChild();
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            RunLoad(m_infoWaferTest);
        }

        private void buttonUnload_Click(object sender, EventArgs e)
        {
            RunUnload(); 
        }

        private void buttonGetState_Click(object sender, EventArgs e)
        {
            CheckStageState(); 
        }

        private void buttonHome_Click(object sender, EventArgs e)
        {
            SetState(eState.Home); 
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate); 
            RunGrid(eGrid.eRegWrite);
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

        protected virtual void RunGrid(ezTools.eGrid eMode)
        {

        }

        protected bool CheckStateChange(eState s)
        {
            bool bRst = false;
            switch (s)
            {
                case eState.Init:
                    bRst = true;
                    break;
                case eState.Home:
                    //if (m_eState != eState.Run)
                     //&& m_eState != eState.Error)
                    {
                        bRst = true;
                    }
                    break;
                case eState.LoadWait:
                    if (m_eState == eState.UnLoadDone || m_eState == eState.Home || m_eState == eState.Done || m_eState == eState.Init)
                    {
                        bRst = true;
                    }
                    break;
                case eState.Load:
                    if (m_eState == eState.LoadWait)
                    {
                        bRst = true;
                    }
                    break;
                case eState.Ready:
                    if (m_eState == eState.Load)
                    {
                        bRst = true;
                    }
                    break;
                case eState.LoadDone:
                    if (m_eState == eState.Ready)
                    {
                        bRst = true;
                    }
                    break;
                case eState.Run:
                    if (m_eState == eState.LoadDone)
                    {
                        bRst = true;
                    }
                    break;
                case eState.Done:
                   
                        bRst = true;
                    
                    break;
                case eState.UnLoadDone:
                    if (m_eState == eState.Done || m_eState == eState.LoadWait)
                    {
                        bRst = true;
                    }
                    break;
                case eState.Error:
                    bRst = true;
                    break;
            }
            return bRst;
        }

        protected eHWResult SetState(eState s)
        {
            if (CheckStateChange(s) == true)
            {
                m_log.Add("SetState : " + m_eState + " to " + s);
                m_eState = s;
                return eHWResult.OK;
            }
            else
            {
                m_log.Popup("SetState Fail " + m_eState + " to " + s);
                return eHWResult.Error;
            }
        }
        
        public eState GetState()
        {
            return m_eState;
        }

        public virtual string GetStageState()
        {
            return "";
        }

        public Info_Wafer InfoWafer
        {
            get { return m_infoWafer; }
            set
            {
                m_infoWafer = value;
                if (m_infoWafer == null) return;
                m_infoWafer.Locate = Info_Wafer.eLocate.Vision; 
            }
        }

        protected virtual eHWResult RunHome() { return eHWResult.Error; }
        public virtual bool IsConnected() { return m_socket.IsConnect();} 
        public virtual bool IsLifterUp() { return false; }
        public virtual bool IsWaferExistHome() { return false; }
        public virtual bool IsWaferExistLoad() { return false; }
        public virtual bool IsWaferVacOn() { return false; }
        public virtual eHWResult IsWFStgReady() { return eHWResult.Off; }       //XY Ready Position Check
        //public virtual eHWResult IsStgLiftUp() { return eHWResult.Off; }      //KDG 161007 Delete IsLifterUp()과 같은 함수여서 삭제
        public virtual eHWResult IsWaferExist() { return eHWResult.Off; }
        public virtual eHWResult CheckStageState() { return eHWResult.Error; }
        public virtual bool IsVSReadySignal() { return false; }
        protected virtual eHWResult RunLoad(Info_Wafer infoWafer) { return eHWResult.Error; }
        public virtual eHWResult RunUnload() { return eHWResult.Error; }
        protected virtual eHWResult RunStart() { return eHWResult.Error; }
        public virtual eHWResult RunStop() { return eHWResult.Error; }
        public virtual eHWResult SendLotStart(string strLotID) { return eHWResult.Error; }
        public virtual eHWResult SendLotEnd(Info_Carrier infoCarrier) { return eHWResult.Error; }
        public virtual eHWResult SendLoadDone(Info_Wafer infoWafer) { return eHWResult.Error; }
        public virtual eHWResult SendUnloadDone() { return eHWResult.Error; }
        public virtual void ezASLReady(bool bOn) { }
        public virtual eHWResult RunEmergencyStop() { return eHWResult.Error; }
        public virtual eHWResult RunInkChange() { return eHWResult.Error; }

        protected virtual void RunThread() { }

        public eHWResult SetHome() 
        {
            return SetState(eState.Home);  
        }

        public eHWResult SetLoad(Info_Wafer infoWafer) 
        {
            m_swWPH.Start();
            LoadLastWPHTime();
            m_infoWafer = infoWafer; 
            return SetState(eState.Load); 
        }

        public eHWResult SetLoadDone(Info_Wafer infoWafer, bool bMSG)  
        {
            InfoWafer = m_infoWafer;
            if (bMSG)
            {
                return SetState(eState.LoadDone);
            }
            else
                return eHWResult.OK;
        }

        public eHWResult SetUnloadDone()
        {
            SaveLastWPHTime(InfoWafer.m_wafer.m_eSize);
            InfoWafer.m_nProgressCurrent = 0;
            InfoWafer = null;
            m_bWaferExist = false;
            return SetState(eState.UnLoadDone); 
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            RunStart();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            RunStop();
        }

        private void buttonLotStart_Click(object sender, EventArgs e)
        {
            SendLotStart(m_infoCarrierTest.m_strLotID);
        }

        private void buttonLotEnd_Click(object sender, EventArgs e)
        {
            SendLotEnd(m_infoCarrierTest);
        }

        private void buttonLoadDone_Click(object sender, EventArgs e)
        {
            SendLoadDone(m_infoWaferTest);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendUnloadDone();
        }

        public int GetmsInspect()
        {
            if (InfoWafer == null) return 0;
            return (int)m_swWPH.Check();
        }

        public void SaveLastWPHTime(Wafer_Size.eSize waferSize)
        {
            m_WPHTime[(int)waferSize] = (int)m_swWPH.Check();
            m_log.m_reg.Write(waferSize.ToString(), m_WPHTime[(int)waferSize]); 
            m_swWPH.Stop();
            LoadLastWPHTime();
        }

        public void LoadLastWPHTime() 
        {
            m_WPHTime[0] = m_log.m_reg.Read(Wafer_Size.eSize.inch4.ToString(), 0);
            m_WPHTime[1] = m_log.m_reg.Read(Wafer_Size.eSize.inch5.ToString(), 0);
            m_WPHTime[2] = m_log.m_reg.Read(Wafer_Size.eSize.inch6.ToString(), 0);
            m_WPHTime[3] = m_log.m_reg.Read(Wafer_Size.eSize.mm200.ToString(), 0);
            m_WPHTime[4] = m_log.m_reg.Read(Wafer_Size.eSize.mm300.ToString(), 0);
            m_fWPH = m_WPHTime[3];
        }

        public int GetWPH()
        {
            if(InfoWafer != null) m_fWPH = m_WPHTime[(int)InfoWafer.m_wafer.m_eSize]; 
            return m_fWPH;
        }

        public virtual bool IsVisionEnable()
        {
            return false;
        }

        private void buttonInkChange_Click(object sender, EventArgs e)
        {
            RunInkChange();
        }        
    }
}
