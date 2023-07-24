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
using ezAuto_Mom;
using ezTools;

namespace ezAuto_EFEM
{
    public partial class HW_WTR_Mom : Form
    {
        public enum eState
        {
            Init,
            Home,
            Ready,
            Run,
            Error,
        }
        public enum eArm
        {
            Lower,
            Upper
        }

        public enum eTeaching
        {
            LoadPort0,
            LoadPort1,
            LoadPort2,
            Aligner,
            Vision,
            None
        }
        
        protected string m_id;
        protected string m_sError; 
        protected int m_nALID = -1;
        Size[] m_sz = new Size[2];
        protected Log m_log;
        protected ezGrid m_grid;
        protected Auto_Mom m_auto;
        protected Control_Mom m_control;
        protected XGem_Mom m_xGem;
        protected Work_Mom m_work;
        protected Handler_Mom m_handler;
        protected HW_Aligner_Mom m_aligner = null;
        protected HW_VisionWorks m_visionworks = null;
        public Info_Wafer[] m_InfoWafer = new Info_Wafer[2] { null, null };
        protected eState m_eState = eState.Init;

        protected int m_nLoadPort = 2; //forget

        protected bool m_bRunThread = false;
        Thread m_thread;                

        public HW_WTR_Mom()
        {
            InitializeComponent();
            m_sz[0] = m_sz[1] = this.Size;
            m_sz[0].Height = 26;
        }

        public virtual void Init(string id, Auto_Mom auto, HW_Aligner_Mom aligner, HW_VisionWorks visionworks)
        {
            m_id = id; 
            m_auto = auto; 
            m_aligner = aligner;
            m_visionworks = visionworks;
            m_sz[0] = m_sz[1] = this.Size; 
            m_sz[0].Height = 26;
            m_log = new Log(m_id, m_auto.ClassLogView());
            m_grid = new ezGrid(m_id, grid, m_log, false);
            m_control = m_auto.ClassControl();
            m_xGem = m_auto.ClassXGem();
            m_work = m_auto.ClassWork();
            m_handler = m_auto.ClassHandler();
            RunGrid(eGrid.eRegRead); 
            RunGrid(eGrid.eInit);
            m_thread = new Thread(new ThreadStart(RunThread)); m_thread.Start();
            InitComboBox();
        }

        protected virtual void InitComboBox()
        {
            comboArm.Items.Clear();
            comboArm.Items.Add(eArm.Lower.ToString());
            comboArm.Items.Add(eArm.Upper.ToString());
            comboArm.SelectedIndex = 0;
            
            comboPosition.Items.Clear();
            for (int i = 0; i < m_nLoadPort; i++)
                comboPosition.Items.Add(((eTeaching)i).ToString());
            comboPosition.Items.Add(eTeaching.Aligner.ToString());
            comboPosition.Items.Add(eTeaching.Vision.ToString());
            comboPosition.SelectedIndex = 0;
            
            for (int i = 1; i <= 25; i++)
                comboSlot.Items.Add(i.ToString());
            comboSlot.SelectedIndex = 0;

            for(int i =0; i < Enum.GetNames(typeof(eWaferSize)).Length-2; i++)
                comboWaferSize.Items.Add(((eWaferSize)i).ToString());
            comboWaferSize.SelectedIndex = 0;
            
        }

        public void ThreadStop()
        {
            if (m_bRunThread) { m_bRunThread = false; m_thread.Join(); }
        }

        public virtual void RunGrid(eGrid eMode)
        {
            m_grid.Update(eMode, null);
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

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate); RunGrid(eGrid.eRegWrite);
        }

        public eState GetState()
        {
            return m_eState;
        }

        public eHWResult SetHome()
        {
            if ((m_eState == eState.Run) || (m_eState == eState.Error))
            {
                m_log.Popup("Can't Start Home When State is " + m_eState.ToString());
                return eHWResult.Error;
            }
            SetState(eState.Home);
            return eHWResult.OK;
        }

        protected virtual void RunThread(){}
        protected virtual void SetState(eState state) { }

        protected virtual void RunVac(eArm nArm, bool bVac) { }
        protected virtual eHWResult RunHome() { return eHWResult.Error; }
        public virtual bool IsWaferExist(eArm nArm) { return false; }
        public virtual eHWResult isVacOn(eArm nArm) { return eHWResult.Off; }
        
        protected virtual void buttonHome_Click(object sender, EventArgs e)
        {
            if (m_work.RUNSTATE != eWorkRun.Teach) return; 
            
        }
        protected virtual void buttonVacOn_Click(object sender, EventArgs e)
        {
            if (m_work.RUNSTATE != eWorkRun.Teach) return;
            //forget
        }

        protected virtual void buttonVacOff_Click(object sender, EventArgs e)
        {
            if (m_work.RUNSTATE != eWorkRun.Teach) return; 
            
        }

        protected virtual void buttonReset_Click(object sender, EventArgs e)
        {
            if (m_work.RUNSTATE != eWorkRun.Teach) return; 
            
        }

        protected virtual void buttonGet_Click(object sender, EventArgs e)
        {
            if (m_work.RUNSTATE != eWorkRun.Teach) return; 
            
        }

        protected virtual void buttonPut_Click(object sender, EventArgs e)
        {
            if (m_work.RUNSTATE != eWorkRun.Teach) return; 
            
        }

    }
}
