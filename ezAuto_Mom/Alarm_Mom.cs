using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ezTools;
using ezAutoMom;
using System.Threading;

namespace ezAutoMom
{
    public partial class Alarm_Mom : Form
    {

        ezRegistry m_reg = new ezRegistry("spAuto", "Alarm");
        string sInfo = "";
        int m_Alnum = 0;
        Auto_Mom m_auto = null;
        int m_Num = 0;
        bool m_bReset = false;
        Thread m_thread;
        protected bool m_bRunThread = false;
        int m_nAlarmNum = 0;

        public Alarm_Mom()
        {
            InitializeComponent();
            
        }

        public void SetAlarmInfo(Auto_Mom auto, string sModule, string sAlramName, string sDetail, int num, int AlarmNum)
        {
            m_bReset = false;
            m_reg.Read(sAlramName, ref sInfo);
            tbModule.Text = sModule;
            tbAlarmCode.Text = sAlramName;
            tbDetail.Text = sDetail;
            m_Num = num;
            m_auto = auto;
            m_nAlarmNum = AlarmNum;
        }



        private void Alarm_Mom_FormClosing_1(object sender, FormClosingEventArgs e)
        {
          //  if (m_nAlarmNum > 0)
          //      m_auto.ClassXGem().SetAlarm(m_nAlarmNum, false);
            e.Cancel = true;
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
