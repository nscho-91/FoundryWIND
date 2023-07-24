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
using WeifenLuo.WinFormsUI.Docking;
using ezTools; 

namespace ezAxis
{
    public partial class Axiss_Ajin : DockContent
    {
        public int[] m_lAxis = new int[3] { 0, 0, 0 };

        string m_id;
        bool m_bJog = false;
        Log m_log = null;
        ezGrid m_grid = null;
        Axis_Mom[] m_axis;
        Color[] m_color = new Color[2];
        bool[] m_bUseLib = new bool[2] { true, false };
        
        public Axiss_Ajin()
        {
            InitializeComponent();
        }

        public void Init(string id, LogView logView)
        {
            uint nError; int n, lAxis = -1;
            this.TabText = m_id = id;
            m_color[0] = Color.FromArgb(100, 100, 100);
            m_color[1] = Color.FromArgb(255, 0, 0);
            m_log = new Log(m_id, logView);
            m_grid = new ezGrid(m_id, grid, m_log, false); ResizeGrid();

            m_grid.Update(eGrid.eRegRead);
            m_grid.Set(ref m_lAxis[0], m_id, "Axis#", "% of Axis");
            m_grid.Set(ref m_lAxis[1], m_id, "EIP_Axis#", "% of Axis");
            m_lAxis[2] = m_lAxis[0] + m_lAxis[1];
            m_axis = new Axis_Mom[m_lAxis[2]];

            if (m_bUseLib[0])
            {
                nError = CAXL.AxlOpen(7);
                if (nError > 0) m_log.Popup("AXL Init Error : " + nError.ToString());
            }
            if (m_bUseLib[1])
            {
                nError = CANL.AnlOpen();
                if (nError > 0) m_log.Popup("ANL Init Error : " + nError.ToString());
            }

            for (n = 0; n < m_lAxis[0]; n++) m_axis[n] = new Axis_Ajin(n, m_log);
            for (n = 0; n < m_lAxis[1]; n++) m_axis[n + m_lAxis[0]] = new Axis_Ajin_EIP(n, m_log);

            RunGrid(eGrid.eRegRead); RunGrid(eGrid.eInit);
            for (n = 0; n < m_lAxis[2]; n++) m_axis[n].SaveAxis();

            if (m_bUseLib[0])
            {
                if (CAXM.AxmInfoGetAxisCount(ref lAxis) > 0) return;
                if (lAxis != m_lAxis[0]) m_log.Popup("Axis Count MissMatch : " + lAxis.ToString());
            }
            if (m_bUseLib[1])
            {
                if (CANM.AnmInfoGetAxisCount(ref lAxis) > 0) return;
                if (lAxis != m_lAxis[1]) m_log.Popup("Axis Count MissMatch : " + lAxis.ToString());
            }
        }

        public void InitComboBox()
        {
            int n;
            if (m_lAxis[2] > 0) for (n = 0; n < m_lAxis[2]; n++) comboAxis.Items.Add(m_axis[n].GetID());
        }

        public void ThreadStop()
        {
            int n;
            for (n = 0; n < m_lAxis[2]; n++)
            {
                m_axis[n].ServoOn(false);
                m_axis[n].ThreadStop();
            }
        }


        public void CloseLib()
        {
            if (m_bUseLib[0]) CAXL.AxlClose();
            if (m_bUseLib[1]) CANL.AnlClose(); 
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (comboAxis.SelectedIndex < 0) return;
            if (comboAxis.SelectedIndex >= m_lAxis[2]) return; 
            if (comboAxis.SelectedIndex < m_lAxis[0])
            {
                Axis_Ajin axis = (Axis_Ajin)m_axis[comboAxis.SelectedIndex];
                checkServoOn.Checked = axis.IsServoOn();
                labelHome.ForeColor = m_color[axis.m_mHome[1]];
                labelLimitM.ForeColor = m_color[axis.m_mLimitM[1]];
                labelLimitP.ForeColor = m_color[axis.m_mLimitP[1]];
                labelInPos.ForeColor = m_color[axis.m_mInPos[1]];
                labelAlram.ForeColor = m_color[axis.m_mAlarm[1]];
                labelEmg.ForeColor = m_color[axis.m_mEmergency[1]];
                labelCmd.Text = "Cmd : " + axis.GetPos(true).ToString();
                labelEnc.Text = "Enc  : " + axis.GetPos(false).ToString();
                return; 
            }
            if (comboAxis.SelectedIndex < m_lAxis[1])
            {
                Axis_Ajin_EIP axis = (Axis_Ajin_EIP)m_axis[comboAxis.SelectedIndex];
                checkServoOn.Checked = axis.IsServoOn();
                labelHome.ForeColor = m_color[axis.m_mHome[1]];
                labelLimitM.ForeColor = m_color[axis.m_mLimitM[1]];
                labelLimitP.ForeColor = m_color[axis.m_mLimitP[1]];
                labelInPos.ForeColor = m_color[axis.m_mInPos[1]];
                labelAlram.ForeColor = m_color[axis.m_mAlarm[1]];
                labelEmg.ForeColor = m_color[axis.m_mEmergency[1]];
                labelCmd.Text = "Cmd : " + axis.GetPos(true).ToString();
                labelEnc.Text = "Enc  : " + axis.GetPos(false).ToString();
                return;
            }
        }

        private void comboAxis_SelectedIndexChanged(object sender, EventArgs e)
        {
            RunGrid(eGrid.eInit); 
        }

        private void buttonLoadMOT_Click(object sender, EventArgs e)
        {
            int n; 
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.DefaultExt = "mot";
            fileDlg.Filter = "Mot Files (*.mot)|*.mot";
            fileDlg.ShowDialog();
            if (fileDlg.FileName.Length == 0) return;
            if (m_bUseLib[0])
            {
                if (CAXM.AxmMotLoadParaAll(fileDlg.FileName) != 0) m_log.Add("AXL Load Param Error !!");
            }
            if (m_bUseLib[1])
            {
                if (CANM.AnmMotLoadParaAll(fileDlg.FileName) != 0) m_log.Add("ANL Load Param Error !!");
            }
            for (n = 0; n < m_lAxis[2]; n++) m_axis[n].LoadAxis();
            RunGrid(eGrid.eInit); 
        }

        private void buttonSaveReg_Click(object sender, EventArgs e)
        {
            RunGrid(eGrid.eRegWrite); 
        }

        private void buttonPos0_Click(object sender, EventArgs e)
        {
            MoveAxis(comboAxis.SelectedIndex, 0); 
        }

        private void buttonPos1_Click(object sender, EventArgs e)
        {
            MoveAxis(comboAxis.SelectedIndex, 1); 
        }

        private void buttonPos2_Click(object sender, EventArgs e)
        {
            MoveAxis(comboAxis.SelectedIndex, 2); 
        }

        void MoveAxis(int nIndex, int nPos)
        {
            if (nIndex < 0) return;
            if (nIndex < m_lAxis[0])
            {
                m_axis[nIndex].Move(((Axis_Ajin)m_axis[nIndex]).m_nPos[nPos]);
                return; 
            }
            if (nIndex < m_lAxis[1])
            {
                m_axis[nIndex].Move(((Axis_Ajin_EIP)m_axis[nIndex]).m_nPos[nPos]);
                return;
            }
        }

        private void buttonRepeat_Click(object sender, EventArgs e)
        {
            if (comboAxis.SelectedIndex < 0) return;
            m_axis[comboAxis.SelectedIndex].StartRepeat(); 
        }

        private void buttonJogLeft_MouseDown(object sender, MouseEventArgs e)
        {
            if (comboAxis.SelectedIndex < 0) return;
            if (m_bJog) { m_axis[comboAxis.SelectedIndex].StopAxis(); m_bJog = false; }
            else { m_axis[comboAxis.SelectedIndex].Jog(-1); m_bJog = true; }
        }

        private void buttonJogRight_MouseDown(object sender, MouseEventArgs e)
        {
            if (comboAxis.SelectedIndex < 0) return;
            if (m_bJog) { m_axis[comboAxis.SelectedIndex].StopAxis(); m_bJog = false; }
            else { m_axis[comboAxis.SelectedIndex].Jog(1); m_bJog = true; }
        }

        private void buttonJogLeft_MouseLeave(object sender, EventArgs e)
        {
            if (comboAxis.SelectedIndex < 0) return;
            if (m_bJog) { m_axis[comboAxis.SelectedIndex].StopAxis(); m_bJog = false; }
        }

        private void buttonJogRight_MouseLeave(object sender, EventArgs e)
        {
            if (comboAxis.SelectedIndex < 0) return;
            if (m_bJog) { m_axis[comboAxis.SelectedIndex].StopAxis(); m_bJog = false; }
        }
        
        private void buttonJogLeft_MouseUp(object sender, MouseEventArgs e)
        {
            if (comboAxis.SelectedIndex < 0) return;
            if (m_bJog) { m_axis[comboAxis.SelectedIndex].StopAxis(); m_bJog = false; }
        }

        private void buttonJogRight_MouseUp(object sender, MouseEventArgs e)
        {
            if (comboAxis.SelectedIndex < 0) return;
            if (m_bJog) { m_axis[comboAxis.SelectedIndex].StopAxis(); m_bJog = false; }
        }

        private void checkServoOn_Click(object sender, EventArgs e)
        {
            if (comboAxis.SelectedIndex < 0) return;
            m_axis[comboAxis.SelectedIndex].ServoOn(!m_axis[comboAxis.SelectedIndex].IsServoOn()); 
        }

        private void buttonHome_Click(object sender, EventArgs e)
        {
            m_axis[comboAxis.SelectedIndex].Jog(0.01);
            Thread.Sleep(10);
            m_axis[comboAxis.SelectedIndex].StopAxis(); 
            m_axis[comboAxis.SelectedIndex].HomeSearch(); 
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            m_axis[comboAxis.SelectedIndex].ResetAlarm(); 
        }

        private void Axiss_Ajin_Resize(object sender, EventArgs e)
        {
            ResizeGrid(); 
        }

        void ResizeGrid()
        {
            Size sz = new Size(this.Size.Width, this.Size.Height);
            sz.Width -= (grid.Location.X + 12); sz.Height -= (grid.Location.Y + 12); grid.Size = sz; 
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate); RunGrid(eGrid.eRegWrite);
        }

        void RunGrid(eGrid eMode, ezJob job = null)
        {
            int n, lAxis; 
            m_grid.Update(eMode, job);
            lAxis = m_lAxis[0]; 
            m_grid.Set(ref m_lAxis[0], m_id, "Axis#", "% of Axis");
            if (lAxis != m_lAxis[0]) m_log.Popup("ReStart SW !!");
            lAxis = m_lAxis[1];
            m_grid.Set(ref m_lAxis[1], m_id, "EIP_Axis#", "% of Axis");
            if (lAxis != m_lAxis[1]) m_log.Popup("ReStart SW !!");
            if (eMode <= eGrid.eUpdate)
            {
                if (comboAxis.SelectedIndex < 0) return; 
                m_axis[comboAxis.SelectedIndex].RunGrid(m_grid, eMode);
            }
            else for (n = 0; n < m_lAxis[2]; n++) m_axis[n].RunGrid(m_grid, eMode);
            m_grid.Refresh();
        }

        public void ModelGrid(ezGrid rGrid, eGrid eMode) 
        {
            rGrid.Set(ref m_bUseLib[0], "AJinLib", "UC", "Use Ajin Library");
            rGrid.Set(ref m_bUseLib[1], "AJinLib", "EIP", "Use Ajin Library");
        }

        public Axis_Mom GetAxis(Type type, string strID)
        {
            if (!type.IsUC())
            {
                type.m_nAxis += m_lAxis[0];
                strID = "E." + strID; 
            }
            if ((type.m_nAxis < 0) || (type.m_nAxis >= m_lAxis[2])) return null;
            if (strID != null)
            {
                if (type.IsUC()) ((Axis_Ajin)m_axis[type.m_nAxis]).m_strID = strID;
                else ((Axis_Ajin_EIP)m_axis[type.m_nAxis]).m_strID = strID;
            }
            return m_axis[type.m_nAxis];
        }

        public void SetGantry(Type.eType type, int nMaster, int nSlave)
        {
            uint nOn = 0, nError = 0, nHome = 0;
            double fOffset = 0, fRange = 0;
            if (type == Type.eType.UC)
            {
                nError = CAXM.AxmLinkResetMode(0);
                nError = CAXM.AxmGantrySetDisable((int)nMaster, (int)nSlave);
                nError = CAXM.AxmGantrySetEnable((int)nMaster, (int)nSlave, 0, 0, 0);
                nError = CAXM.AxmGantryGetEnable((int)nMaster, ref nHome, ref fOffset, ref fRange, ref nOn);
            }
            if (type == Type.eType.EIP)
            {
                nError = CANM.AnmLinkResetMode(0);
                nError = CANM.AnmGantrySetDisable((int)nMaster, (int)nSlave);
                nError = CANM.AnmGantrySetEnable((int)nMaster, (int)nSlave, 0, 0, 0);
                nError = CANM.AnmGantryGetEnable((int)nMaster, ref nHome, ref fOffset, ref fRange, ref nOn);
            }
        }

        private void Axiss_Ajin_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public void RunEmergency()
        {
            for (int n = 0; n < m_lAxis[2]; n++) m_axis[n].ServoOn(false);
        }

        private void buttonJogRight_Click(object sender, EventArgs e)
        {

        }

    }
}
