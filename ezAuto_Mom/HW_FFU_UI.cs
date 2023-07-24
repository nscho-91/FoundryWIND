using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;

namespace ezAutoMom
{
    public partial class HW_FFU_UI : DockContent
    {
        HW_FFU_Mom m_ffu;
        Button[] m_Button;
        System.Windows.Forms.Timer timerUI = new Timer();

        public HW_FFU_UI()
        {
            InitializeComponent();
        }

        public void Init(HW_FFU_Mom ffu, int nModuleNum)
        {
            m_ffu = ffu;
            m_Button = new Button[nModuleNum * 2 + 1];
            for (int i = 0; i < m_Button.Length; i++)
            {
                m_Button[i] = new Button();
                m_Button[i].Font = new Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                m_Button[i].ForeColor = SystemColors.ControlText;
                m_Button[i].Margin = new Padding(0, 0, 0, 0);
            }
            timerUI.Tick += new EventHandler(timer_Tick);
            timerUI.Enabled = true;
            timerUI.Interval = 1000;
            timerUI.Start();
        }

        public void ShowPanel(DockPanel dockpanel)
        {
            this.Show(dockpanel);
        }

        public void SetTableLayoutPanel()
        {
            for (int i = tableLayoutPanel1.RowCount - 1; i >= 0; --i)
            {
                tableLayoutPanel1.Controls.Remove(m_Button[i]);
            }

            this.tableLayoutPanel1.RowStyles.Clear();

            
            for (int i = 0; i < m_ffu.m_nModuleNumber; i++)
            {
                this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, (float)100 / m_ffu.m_nModuleNumber));
                m_Button[2 * i].Text = m_ffu.GetFFUModule(i).m_sName;
                m_Button[2 * i + 1].Text = m_ffu.GetFFUModule(i).m_sValue;
                m_Button[2 * i].Enabled = false;
                m_Button[2 * i + 1].Enabled = false;
                m_Button[2 * i].BackColor = Color.LemonChiffon;
                m_Button[2 * i + 1].BackColor = Color.PaleGreen;
                m_Button[2 * i].Dock = DockStyle.Fill;
                m_Button[2 * i + 1].Dock = DockStyle.Fill;
                this.tableLayoutPanel1.Controls.Add(m_Button[2 * i], 0, i);
                this.tableLayoutPanel1.Controls.Add(m_Button[2 * i + 1], 1, i);
            }
            this.Invalidate();
        }

        private void Ui_FFU_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < m_ffu.m_nModuleNumber; i++)
            {
                m_Button[2 * i + 1].Text = m_ffu.GetFFUModule(i).m_sValue;
                if (m_ffu.GetFFUModule(i).GetAlarmMessage() != "")
                {
                    m_Button[2 * i + 1].BackColor = Color.OrangeRed;
                }
                else
                {
                    m_Button[2 * i + 1].BackColor = Color.PaleGreen;
                }
            }
        }

        public bool IsPersistString(string str)
        {
            return GetPersistString() == str;
        }
    }
}
