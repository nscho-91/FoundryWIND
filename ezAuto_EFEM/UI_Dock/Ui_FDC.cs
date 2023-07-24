using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using ezAutoMom;
using ezTools; 


namespace ezAuto_EFEM
{
    public partial class Ui_FDC : DockContent
    {
        HW_FDC_Mom m_fdc = null;
        Auto_Mom m_auto = null;
        Button[] m_Button = new Button[22];
        Handler_Mom m_handler = null;
        Log m_log = null;
        public Ui_FDC()
        {
            InitializeComponent();
        }

        public void Init(Auto_Mom auto, HW_FDC_Mom fdc, Log log)
        {
            m_auto = auto;
            m_fdc = fdc;
            m_handler = m_auto.ClassHandler();
            m_log = log;
            for (int i = 0; i < 22; i++) {
                m_Button[i] = new Button();
                m_Button[i].Font = new Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                m_Button[i].ForeColor = SystemColors.ControlText;
            }
        }

        public void ShowPanel(DockPanel dockpanel)
        {
            this.Show(dockpanel);
        }

        public void SetTableLayoutPanel()
        {   
            for (int i = tableLayoutPanel1.RowCount - 1; i >= 0; --i) {
                tableLayoutPanel1.Controls.Remove(m_Button[i]);
            }

            this.tableLayoutPanel1.RowStyles.Clear();

            for (int i = 0; i < m_handler.m_nFDCModuleNum; i++) {
                this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, (float)100 / m_handler.m_nFDCModuleNum));
                m_Button[2 * i].Text = m_fdc.GetFDCModule(i).m_sName;
                m_Button[2 * i + 1].Text = m_fdc.GetFDCModule(i).GetFDCData().ToString();
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_auto.ClassHandler().m_bFDCUse) 
            {
                for (int i = 0; i < m_auto.ClassHandler().m_nFDCModuleNum; i++) 
                {
                    if (m_auto.ClassHandler().ClassFDC().GetFDCModule(i) == null) continue;
                    m_Button[2 * i + 1].Text = m_auto.ClassHandler().ClassFDC().GetFDCModule(i).GetFDCData().ToString();
                    if (m_auto.ClassHandler().ClassFDC().GetFDCModule(i).GetFDCData() > Convert.ToDouble(m_auto.ClassHandler().ClassFDC().GetFDCModule(i).m_fHighLimit))
                        m_log.Popup(" FDC Error (High Limit)  : " + m_auto.ClassHandler().ClassFDC().GetFDCModule(i).m_sName);
                    else if (m_auto.ClassHandler().ClassFDC().GetFDCModule(i).GetFDCData() < Convert.ToDouble(m_auto.ClassHandler().ClassFDC().GetFDCModule(i).m_fLowLimit))
                        m_log.Popup(" FDC Error (Low Limit)  : " + m_auto.ClassHandler().ClassFDC().GetFDCModule(i).m_sName);
                }    
            }
        }
    }
}
