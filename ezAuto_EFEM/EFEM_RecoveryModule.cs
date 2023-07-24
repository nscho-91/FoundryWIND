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

namespace ezAuto_EFEM
{
    public partial class EFEM_RecoveryModule : Form
    {
        public enum eState
        {
            Empty,
            Enable,
            Disable,
        }
        public eState m_eState = eState.Empty; 

        string m_id;
        public string m_strDisable; 
        Auto_Mom m_auto;
        Handler_Mom m_handler;
        Info_Carrier[] m_infoCarrier = new Info_Carrier[3]; 
        Info_Wafer m_infoWafer = null;
        public Info_Wafer m_infoCombo = null;
        private bool m_bWaferExist = false;

        public EFEM_RecoveryModule()
        {
            InitializeComponent();
        }

        public void Init(string id, Auto_Mom auto, Info_Wafer infoWafer, Log log, bool bWaferExist)
        {
            m_bWaferExist = bWaferExist;
            m_id = id;
            groupName.Text = m_id;
            m_auto = auto;
            m_handler = m_auto.ClassHandler(); 
            m_infoWafer = infoWafer;
            m_infoCombo = new Info_Wafer(-1, -1, -1, log); 
            if (m_infoWafer != null)
            {
                m_infoCombo.m_nLoadPort = m_infoWafer.m_nLoadPort;
                m_infoCombo.m_nID = m_infoWafer.m_nID;
                m_infoCombo.m_wafer = m_infoWafer.m_wafer; 
            }
            comboLoadPort.Enabled = false;
            comboSlot.Enabled = false;
            InitComboLoadPort();
            InitComboSlot();
            IOWaferExist.Set(bWaferExist);
            
        }

        public void ShowDlg(Form parent, ref CPoint cpShow)
        {
            this.TopLevel = false; 
            this.Parent = parent; 
            this.Location = cpShow.ToPoint();
            //cpShow.x += this.Size.Width; 
            Show();
        }

        private void checkEdit_CheckedChanged(object sender, EventArgs e)
        {
            comboLoadPort.Enabled = checkEdit.Checked;
            comboSlot.Enabled = checkEdit.Checked;
        }

        private void comboLoadPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckComboLoadPort(); 
            comboSlot.SelectedIndex = 0;
            CheckState(); 
        }

        private void comboSlot_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckState(); 
        }

        void InitComboLoadPort()
        {
            comboLoadPort.Items.Add(" ");
            for (int n = 0; n < 3; n++)
            {
                m_infoCarrier[n] = m_handler.ClassCarrier(n);
                if (m_infoCarrier[n] != null)
                {
                    comboLoadPort.Items.Add("LoadPort" + (n + 1).ToString());
                }
            }
            comboLoadPort.SelectedIndex = m_infoCombo.m_nLoadPort + 1; 
        }

        void CheckComboLoadPort()
        {
            m_infoCombo.m_nLoadPort = comboLoadPort.SelectedIndex - 1;
            InitComboSlot(); 
        }

        void InitComboSlot()
        {
            comboSlot.Items.Clear(); 
            comboSlot.Items.Add(" ");
            if (m_infoCombo.m_nLoadPort < 0) return; 
            for (int n = 0; n < m_infoCarrier[m_infoCombo.m_nLoadPort].m_lWafer; n++)
            {
                comboSlot.Items.Add("Slot" + m_infoCarrier[m_infoCombo.m_nLoadPort].m_infoWafer[n].m_nSlot.ToString("00"));
            }
            if (m_infoWafer != null)
            {
                comboLoadPort.SelectedIndex = m_infoCombo.m_nLoadPort + 1;
                comboSlot.SelectedIndex = m_infoWafer.m_nID + 1; 
            }
            else
            {
                comboLoadPort.SelectedIndex = m_infoCombo.m_nLoadPort + 1;
                comboSlot.SelectedIndex = 0;
            }
        }

        void CheckState() 
        {
            m_eState = CheckComboSlot();
            this.BackColor = GetColor(m_eState); 
        }

        eState CheckComboSlot() 
        {
            m_infoCombo.m_nID = comboSlot.SelectedIndex - 1;
            if(!m_bWaferExist)
                return eState.Empty;
            if (((m_infoCombo.m_nLoadPort < 0) || (m_infoCombo.m_nID < 0)) && m_bWaferExist)
            {
                m_strDisable = "Please Select"; // BHJ 190705 add
                return eState.Disable;
            }
            if ((m_infoWafer != null) && CheckSame(m_infoWafer))
            {
                if (m_infoCarrier[m_infoCombo.m_nLoadPort].m_eState == Info_Carrier.eState.Done
                    && m_infoCarrier[m_infoCombo.m_nLoadPort].m_eState != Info_Carrier.eState.MapDone)
                    return eState.Enable;
                else
                {
                    m_strDisable = "LoadPort : " + m_infoCarrier[m_infoCombo.m_nLoadPort].m_eState.ToString();
                    return eState.Disable;
                }
            }
            if (m_infoCarrier[m_infoCombo.m_nLoadPort].m_eState != Info_Carrier.eState.Done 
                && m_infoCarrier[m_infoCombo.m_nLoadPort].m_eState != Info_Carrier.eState.MapDone)
            {
                m_strDisable = "LP State : " + m_infoCarrier[m_infoCombo.m_nLoadPort].m_eState.ToString(); 
                return eState.Disable;
            }
            if (m_infoCarrier[m_infoCombo.m_nLoadPort].m_infoWafer[m_infoCombo.m_nID].IsEnableRecover() == false)
            {
                m_strDisable = "Slot State : " + m_infoCarrier[m_infoCombo.m_nLoadPort].m_infoWafer[m_infoCombo.m_nID].State.ToString(); 
                return eState.Disable;
            }
            return eState.Enable; 
        }

        public bool CheckSame(Info_Wafer infoWafer)
        {
            if (m_infoCombo.m_nLoadPort < 0) return false;
            if (m_infoCombo.m_nID < 0) return false;
            if (m_infoCombo.m_nLoadPort != infoWafer.m_nLoadPort) return false;
            if (m_infoCombo.m_nID != infoWafer.m_nID) return false;
            return true; 
        }

        public Color GetColor(eState state)
        {
            if (state != eState.Disable) labelError.Text = "";
            else labelError.Text = m_strDisable; 
            switch (state)
            {
                case eState.Empty: return Color.FromArgb(200, 200, 200);
                case eState.Enable: return Color.FromArgb(50, 200, 50); 
                case eState.Disable: return Color.FromArgb(200, 50, 50); 
            }
            return Color.FromArgb(200, 200, 200);
        }

        public Info_Wafer GetInfoWaferCombo()
        {
            if (m_infoCombo.m_nLoadPort < 0) return null; 
            if (m_infoCombo.m_nID < 0) return null;
            return m_infoCarrier[m_infoCombo.m_nLoadPort].m_infoWafer[m_infoCombo.m_nID]; 
        }

    }
}