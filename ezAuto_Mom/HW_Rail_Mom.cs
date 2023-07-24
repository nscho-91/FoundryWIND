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
using ezTools; 

namespace ezAutoMom
{
    public partial class HW_Rail_Mom : Form, If_InlineFlow
    {
        public enum eState
        {
            Init,
            Home,
            RunReady,
            Ready,
            Load,
            RunDone,
            Done,
            Send,
        };
        protected eState m_eState = eState.Init;

        public string m_id;
        protected int m_nID;

        protected string m_strReady = "";
        protected string m_strDone = "";
        protected Info_Strip m_infoStrip;
        protected Info_Strip m_infoStripLoad;

        protected If_InlineFlow m_nextFlow;

        protected Auto_Mom m_auto;
        protected Control_Mom m_control;
        protected Handler_Mom m_handler;
        protected Work_Mom m_work;
        protected XGem300_Mom m_xGem;
        protected Log m_log;
        protected ezGrid m_grid; 

        Size[] m_sz = new Size[2];
        
        bool m_bRun = false;
        Thread m_thread;

        ezStopWatch m_swTimeover = new ezStopWatch();

        public HW_Rail_Mom()
        {
            InitializeComponent();
            m_sz[0] = m_sz[1] = this.Size; m_sz[0].Height = 26;
        }

        private void checkView_CheckedChanged(object sender, EventArgs e)
        {
            m_handler.ShowChild(); 
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            Send(); 
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
            this.TopLevel = false; this.Parent = parent; this.Location = cpShow.ToPoint();
            if (checkView.Checked) nIndex = 1; else nIndex = 0;
            this.Size = m_sz[nIndex]; cpShow.y += m_sz[nIndex].Height;
            Show();
        }

        private void HW_Rail_Mom_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public virtual void Init(int nID, string id, Auto_Mom auto, If_InlineFlow nextFlow)
        {
            m_id = id;
            checkView.Text = m_id;
            m_nID = nID;
            m_auto = auto;
            m_sz[0] = m_sz[1] = this.Size; 
            m_sz[0].Height = 26;
            SetNextFlow(nextFlow); 
            m_control = m_auto.ClassControl();
            m_handler = ((Auto_Mom)auto).ClassHandler();
            m_work = m_auto.ClassWork();
            m_xGem = m_auto.ClassXGem();
            m_log = new Log(m_id, m_auto.ClassLogView(), "Rail");
            m_grid = new ezGrid(id, grid, m_log, false);
            m_thread = new Thread(new ThreadStart(RunThread)); m_thread.Start();
        }

        public void SetNextFlow(If_InlineFlow nextFlow)
        {
            m_nextFlow = nextFlow;
//            buttonSend.Enabled = (nextFlow != null);
        }

        public virtual void ThreadStop()
        {
            if (m_bRun)
            {
                m_bRun = false;
                m_thread.Join();
            }
            RunMove(false);
            RunStopper(false);
        }

        public void StartHome()
        {
            if (m_eState > eState.Ready) return; 
            m_eState = eState.Home; 
        }

        public virtual bool CheckInitOK()
        {
            if (m_eState > eState.Ready)
            {
                m_log.Popup("Rail State is not Ready to Home : " + m_eState.ToString()); 
                return false;
            }
            return true;
        }

        public virtual void RunGrid(eGrid eMode, ezJob job = null) { }
        public virtual bool StartLoad(Info_Strip infoStrip) { return false; }
        public virtual bool Send() { return false; }
        public virtual bool IsArrival() { return false; }
        public virtual bool IsStop() { return false; }
        public virtual bool IsCheck() { return false; }
        public virtual bool IsPass() { return false; }
        public virtual void RunInit() { }
        
        protected virtual void RunMove(bool bMove) { }
        protected virtual void RunStopper(bool bUp) { }

        protected virtual void StateHome() { }
        protected virtual void StateRunReady() { }
        protected virtual void StateLoad() { }
        protected virtual void StateRunDone() { }
        protected virtual void StateDone() { }
        protected virtual void StateSend() { }

        void RunThread()
        {
            m_bRun = true; Thread.Sleep(5000);
            while (m_bRun)
            {
                Thread.Sleep(50);
                switch (m_eState)
                {
                    case eState.Home: StateHome(); break;
                    case eState.RunReady: StateRunReady(); break; 
                    case eState.Load: StateLoad(); break;
                    case eState.RunDone: StateRunDone(); break; 
                    case eState.Done: StateDone(); break;
                    case eState.Send: StateSend(); break;
                    default: break;
                }
            }
        }

        public void Clear()
        {
            m_infoStrip = null;
            m_strReady = "";
        }

        public bool IsReady(string strReady)
        {
            if (!IsReady()) return false;
            if (m_strReady == "") m_strReady = strReady;
            return m_strReady == strReady;
        }

        public virtual bool IsReady()
        {
            return m_eState == eState.Ready;
        }

        public bool IsDone(string strDone)
        {
            m_strDone = strDone;
            return m_eState == eState.Done;
        }

        public eState GetState()
        {
            return m_eState;
        }

        public bool SetReady()
        {
            SetStrip(null); 
            m_eState = eState.Ready;
            m_strReady = ""; // ing
            return false;
        }

        public bool SetDone(Info_Strip infoStrip)
        {
            if (!IsCheck()) return true;
            SetStrip(infoStrip); 
            m_eState = eState.Done;
            return false;
        }

        public bool WaitReady(int msTimeover)
        {
            m_swTimeover.Start();
            while (!IsReady() && (m_swTimeover.Check() <= msTimeover)) Thread.Sleep(10);
            return m_swTimeover.Check() >= msTimeover;
        }

        public Info_Strip GetStrip()
        {
            return m_infoStrip;
        }

        public Info_Strip GetLoadStrip()
        {
            if (m_infoStripLoad != null) return m_infoStripLoad;
            return m_infoStrip;
        }

        protected void SetStrip(Info_Strip infoStrip)
        {
            m_infoStrip = infoStrip;
            if (m_infoStrip == null)
            {
                m_strDone = "";
                m_log.Add("SetStrip Null");
            }
            else
            {
                m_infoStrip.m_sPos = m_id;
                m_strReady = "";
                m_log.Add("SetStrip " + m_infoStrip.m_nStrip.ToString());
            }
        }

    }
}
