using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PropertyGridEx; 

namespace ezTools
{
    public partial class Login : Form 
    {
        const int c_nLogin = 8;

        public int m_lvLogin = 4;

        bool m_bUsePW = false;
        int m_nStartLevel = 0;
        string m_id = "Login";
        string m_strID = ""; 
        ezGrid m_grid;
        LoginData[] m_aLoginData = new LoginData[c_nLogin]; 
        Log m_log;
        CPoint m_szForm = new CPoint(140, 80); 

        public Login(LogView logView)
        {
            int n; 
            InitializeComponent();
            m_log = new Log(m_id, logView);
            for (n = 0; n < c_nLogin; n++) m_aLoginData[n] = new LoginData(m_log, n);
            m_log.m_reg.Read("UsePW", ref m_bUsePW);
            m_grid = new ezGrid(m_id, grid, m_log, false);
            RunGrid(eGrid.eRegRead);
            if (m_bUsePW) m_szForm.y = 120;
            RunGrid(eGrid.eInit);
            m_lvLogin = m_nStartLevel; 
            if (m_nStartLevel < 1) m_strID = "Log out";
        }

        private void textID_TextChanged(object sender, EventArgs e)
        {
            if (Check()) { 
                if (!IsResized()) 
                    Close(); 
            }
        }

        private void textPassword_TextChanged(object sender, EventArgs e)
        {
            if (Check()) { 
                if (!IsResized()) 
                    Close(); 
            }
        }

        bool IsResized()
        {
            if (this.Size == (Size)m_szForm.ToPoint()) return false;
            RunGrid(eGrid.eInit); return true; 
        }

        bool Check()
        {
            int n;
            for (n = 0; n < c_nLogin; n++)
            {
                if (m_aLoginData[n].IsMatch(textID.Text, textPassword.Text, m_bUsePW))
                {
                    m_lvLogin = m_aLoginData[n].GetLevel(m_log);
                    m_strID = m_aLoginData[n].m_id; return true; 
                }
            }
            return false;
        }

        void RunGrid(eGrid eMode, ezJob job = null)
        {
            int n;
            m_grid.Update(eMode, job);
            m_grid.Set(ref m_szForm, "Form", "Size", "Form Size (y=80, 120)");
            m_grid.Set(ref m_bUsePW, "Form", "UsePW", "Use Password");
            for (n = 1; n < c_nLogin; n++) m_aLoginData[n].RunGrid(m_grid, m_lvLogin, m_bUsePW);
            if (m_lvLogin >= 4) m_grid.Set(ref m_nStartLevel, "Form", "StartLevel", "StartUp Login Level");
            if (eMode == eGrid.eInit) grid.CollapseAllGridItems(); 
            m_grid.Refresh(); 
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate); RunGrid(eGrid.eRegWrite); 
        }

        private void Login_Load(object sender, EventArgs e)
        {
            textID.Text = ""; textPassword.Text = ""; textID.Select(); 
            RunGrid(eGrid.eInit);
            this.Size = (Size)m_szForm.ToPoint(); 
        }

        public void LogOut()
        {
            m_lvLogin = 0; m_strID = "Log out"; 
            m_log.Add(m_strID); 
        }

        public string GetButtonText()
        {
            if (m_lvLogin >= 4) return m_lvLogin.ToString() + " : " + "ATI"; 
            else return m_lvLogin.ToString() + " : " + m_strID; 
        }
    }

    class LoginData
    {
        public int m_nLevel;
        public string m_id, m_pw, m_strIndex;
        int m_nIndex;
        Log m_log = null; 
 
        public LoginData(Log log, int nIndex)
        {
            m_log = log; m_nIndex = nIndex; m_strIndex = nIndex.ToString();
            m_id = "ID" + m_strIndex; m_pw = "PW" + m_strIndex; m_nLevel = 0;
            if (nIndex == 0) { m_id = "TryThinkFight"; m_pw = "TryThinkFight"; m_nLevel = 4; }
            if (nIndex == 1) { m_id = "ati"; m_pw = "ati"; m_nLevel = 3; }
        }

        public void RunGrid(ezGrid grid, int lvLogin, bool bUsePW)
        {
            if ((m_nLevel > lvLogin) || (lvLogin == 0)) return; 
            grid.Set(ref m_id, m_strIndex, "ID", "ID string");
            if (bUsePW) grid.Set(ref m_pw, m_strIndex, "PW", "Password string");
            grid.Set(ref m_nLevel, m_strIndex, "Level", "Access Level (0 ~ 3)");
            if (m_nLevel < 0) m_nLevel = 0;
            if (m_nLevel > lvLogin) m_nLevel = lvLogin;
        }

        public bool IsMatch(string strID, string strPW, bool bUsePW)
        {
            if (bUsePW && (strPW != m_pw)) return false;
            if (strID != m_id) return false;
            return true; 
        }

        public int GetLevel(Log log)
        {
            log.Add("Log in : " + m_id + ", Lv : "+ m_nLevel.ToString());
            return m_nLevel; 
        }
    }
}
