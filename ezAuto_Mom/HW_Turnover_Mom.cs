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
    public partial class HW_Turnover_Mom : Form
    {
        public string m_id;
        public bool m_bRun, m_bTurn, m_bView;
        public double m_fSlowV;
        protected Info_Strip[] m_infoStrip = new Info_Strip[2] { null, null };

        Size[] m_sz = new Size[2];
        public Log m_log;
        public ezGrid m_grid;
        public ezStopWatch m_sw;
        public Control_Mom m_control;
        public Handler_Mom m_handler;
        public Work_Mom m_work;
        public XGem300_Mom m_xGem;
        public Thread m_thread;
        ezStopWatch m_swDown = new ezStopWatch();

        public bool m_bCheckview;   //박상영20180207

        public HW_Turnover_Mom()
        {
            InitializeComponent();
            m_sz[0] = m_sz[1] = this.Size; m_sz[0].Height = 26;
            m_bRun = m_bTurn = false;
        }
        public virtual void Init(string id, object auto)
        {
            m_id = id; checkView.Text = m_id;
            m_log = new Log(id, ((Auto_Mom)auto).m_logView);
            m_grid = new ezGrid(id, grid, m_log, false);
            m_sw = new ezStopWatch();
            m_control = ((Auto_Mom)auto).ClassControl();
            m_handler = ((Auto_Mom)auto).ClassHandler();
            m_work = ((Auto_Mom)auto).ClassWork();
            m_xGem = ((Auto_Mom)auto).ClassXGem();
            RunGrid(eGrid.eRegRead); RunGrid(eGrid.eInit);
        }

        private void checkView_CheckedChanged(object sender, EventArgs e)
        {
            m_handler.ShowChild();
        }

        private void buttonRunTurn_Click(object sender, EventArgs e)
        {
            RunTurn();
        }

        private void buttonSetReady_Click(object sender, EventArgs e)
        {
            RunInit();
        }

        private void buttonTurn0_Click(object sender, EventArgs e)
        {
            Rotate(0, false);
        }

        private void buttonTurn1_Click(object sender, EventArgs e)
        {
            Rotate(1, false);
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

            m_bCheckview = checkView.Checked;   //박상영20180207
        }

        public bool IsTurn() { return m_bTurn; }

        public virtual void SetLock(int nID, bool bLock) { }
        public virtual void RunGrid(eGrid eMode) { }
        public virtual bool RunTurn() { return false; }
        public virtual bool RunInit() { return false; }
        public virtual bool Rotate(int nIndex, bool bWait) { return false; }
        public virtual void RunVac(int nSide, bool bVac) { }
        public virtual void RunBlow(int nSide, bool bBlow) { }
        public virtual bool CheckInitHome() { return false; }
        public virtual bool IsLock() { return false; }
        public virtual bool IsCheck() { return false; }

        private void HW_Turnover_Mom_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public virtual void SetStrip(int nID, Info_Strip infoStrip)
        {
            m_infoStrip[nID] = infoStrip;
            if (infoStrip == null)
            {
                m_log.Add(m_id + nID.ToString() + " SetStrip Null");
            }
            else
            {
               m_infoStrip[nID].m_sPos = m_id;
                m_log.Add(m_id + nID.ToString() + " : SetStrip " + m_infoStrip[nID].m_nStrip.ToString());
            }
        }

        public virtual Info_Strip GetStrip(int nID)
        {
            return m_infoStrip[nID];
        }

    }
}
