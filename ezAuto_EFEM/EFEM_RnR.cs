using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using ezAutoMom;
using ezTools; 

namespace ezAuto_EFEM
{
    public partial class EFEM_RnR : DockContent, HW_RnR_Mom, Control_Child
    {
        ezStopWatch m_swRnR = new ezStopWatch();
        Auto_Mom m_auto;
        Button m_Finish = new Button();
        Log m_Log;

        int m_nLP = 1;
        int nTest = 0;
        
        
        public EFEM_RnR()
        {
            m_auto = new Auto_Mom();
            InitializeComponent();
            m_swRnR.Stop();
        }

        public void Init(Auto_Mom auto)                                
        {
            m_auto = auto;
            m_Log = new Log("RnR", m_auto.ClassLogView(), "Loadport");
        }
        public void ControlGrid(Control_Mom control, ezGrid rGrid, eGrid eMode)
        {
        }
        public void DisplayTime()                                       //170213 SDH ADD 실시간으로 Run 중인 시간 Display
        {
            ezDateTime date = new ezDateTime();
            date.Check();
            string sSec = ((int)(m_swRnR.Check() / 1000) % 60).ToString().PadLeft(2, '0');
            string sMin = ((int)(m_swRnR.Check() / 60000) % 60).ToString().PadLeft(2, '0');
            string sHour = ((int)m_swRnR.Check() / 3600000).ToString().PadLeft(2, '0');
            lbRun.Text = sHour + "-" + sMin + "-" + sSec;
        }

        public void UpdateRnRState(int nLP)                             //170213 SDH ADD LoadPort 별 RNR 갱신 (횟수 다차면 RnR 종료)
        {
            m_Log.Add("Loadport0" + m_nLP + " Current RnR Count:" + tbCurrent.Text);
            tbCurrent.Text = (Convert.ToInt32(tbCurrent.Text)+1).ToString();
            if (tbLimit.Text == "") return;
            int nLimit = Convert.ToInt32(tbLimit.Text);
            int nCurrent = Convert.ToInt32(tbCurrent.Text);
            if(nLimit<=nCurrent)
            {
                RnREnd();
            }
            else
            {
                m_Log.Add("Loadport0" + m_nLP +" "+tbCurrent.Text + "RnR Test Start");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DisplayTime();
        }

        public void SetInit(int nLP)                                           //170213 SDH ADD RnR 시작시 처음 Setting
        {
            CheckForIllegalCrossThreadCalls = false;
            m_swRnR.Start();
            ezDateTime date = new ezDateTime();
            lbStart.Text = date.GetTime().ToString();
            lbEnd.Text = "00-00-00";
            tbCurrent.Text = "0";
            m_nLP = nLP;
            m_Log.Add("Loadport0" + m_nLP + " RnR Start");
            btnFinish.Enabled = true;
        }

        public void RnREnd()
        {
            ezDateTime date = new ezDateTime();
            lbEnd.Text = date.GetTime().ToString();
            m_swRnR.Stop();
            m_auto.ClassHandler().ClassLoadPort(m_nLP).m_infoCarrier.m_bRNR = false;
            m_Log.Add("Loadport0" + m_nLP + " RnR End");
            btnFinish.Enabled = false;
            m_auto.ClassHandler().ClassLoadPort(m_nLP).m_bRnRStart = false;

        }

        private void btnFinish_Click(object sender, EventArgs e)                //170213 SDH ADD Finish Button
        {
            if (MessageBox.Show("RnR Mode End?","", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                RnREnd();
            }

        }
        private void EFEM_RnR_FormClosing(object sender, FormClosingEventArgs e)
        {
            RnREnd();
            e.Cancel = true;
            this.Hide();
        }
    }
}
