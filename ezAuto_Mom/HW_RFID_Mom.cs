using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ezAutoMom;
using ezTools;

namespace ezAutoMom
{
    public partial class HW_RFID_Mom : Form
    {

        protected string m_id;
        protected Log m_log;
        protected ezGrid m_grid;
        protected Auto_Mom m_auto;
        protected Control_Mom m_control;
        protected Info_Carrier m_infoCarrier = null;
        protected string m_sCSTIDResult = "";
        protected string m_sLOTIDResult = "";
        Size[] m_sz = new Size[2];
        
        
        public HW_RFID_Mom()
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
            m_log = new Log(m_id, m_auto.ClassLogView(), "RFID");
            m_grid = new ezGrid(m_id, grid, m_log, false);
            
            m_control = m_auto.ClassControl();
        }
        protected virtual void RunGrid(eGrid eMode)
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

        private void btnRead_Click(object sender, EventArgs e)
        {
            CSTIDRead();
        }

        public virtual void CSTIDRead()
        {
            return ;
        }
        public virtual void LOTIDRead()
        {
            return;
        }

        private void checkView_CheckedChanged_1(object sender, EventArgs e)
        {
            m_auto.ClassHandler().ShowChild();
        }

        private void grid_PropertyValueChanged_1(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate);
            RunGrid(eGrid.eRegWrite);
        }

        private void grid_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            LOTIDRead();
        }

        public virtual void ResetID()
        {
            m_sCSTIDResult = "";
            m_sLOTIDResult = "";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tbLOTID.Text = m_sLOTIDResult;
            tbCSTID.Text = m_sCSTIDResult;
        }

        public string GetLOTID()
        {
            return m_sLOTIDResult;
        }

        public string GetCSTID()
        {
            return m_sCSTIDResult;
        }
    }
}
