using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using WeifenLuo.WinFormsUI.Docking;
using ezTools;
using ezAutoMom;
using ezAuto_EFEM;


namespace spAuto
{
    public partial class spAuto : Form 
    {
        Model m_model = new Model();
        LampSet_Mom m_Lamp = new LampSet_Mom(); 
        Auto_Mom m_auto;
        private DeserializeDockContent m_deserializeDockContent;
        
        public spAuto()
        {
            InitializeComponent();
            AddModel(); 
            m_deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);
            dockPanel.AllowEndUserDocking = false;
        }

        private void AddModel()
        {
         
            EFEM m_efem = new EFEM();
            m_auto = m_efem; 
            m_auto.Init(m_model, dockPanel);
            toolLogIn.Text = m_auto.m_login.GetButtonText();
            ShowMenu();     
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            return m_auto.GetContentFromPersistString(persistString); 
        }

        private void spAuto_Load(object sender, EventArgs e)
        {
            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
            if (File.Exists(configFile))
                dockPanel.LoadFromXml(configFile, m_deserializeDockContent);
            if (!m_auto.m_bOK) 
                Close();
            Assembly assemobj = Assembly.GetExecutingAssembly();
            Version v = assemobj.GetName().Version;
            this.Text += string.Format("_spAuto_FOUNDRY_ASL{0}.{1:00}.{2:000}", v.Major, v.Minor, v.Build);
            string sSeq = string.Format("[P/G Start] {0}", Text);
            m_auto.m_log.WriteLog("Sequence", sSeq);
            //if (m_auto.m_login.m_lvLogin < 3) Unlock(false);
        }

        private void spAuto_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_auto.ClassLogView().HidePopup(true);
            #if (!DEBUG)
            if (System.Windows.Forms.MessageBox.Show("Do you want to exit the program?", "Program Exit", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                e.Cancel = true;
            }            
            else
            #endif
            {
                string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
                m_auto.ThreadStop();
                dockPanel.SaveAsXml(configFile);
            }
            m_auto.ClassLogView().HidePopup(false);            
        }

        private void toolLogIn_Click(object sender, EventArgs e)
        {
            m_auto.Login(); 
            ShowMenu(); 
        }

        private void toolJobOpen_Click(object sender, EventArgs e)
        {
            m_auto.JobOpen();
            ShowMenu();
        }

        private void menuSaveJob_Click(object sender, EventArgs e)
        {
            m_auto.JobSave();
            ShowMenu(); 
        }

        private void menuSaveParam_Click(object sender, EventArgs e)
        {
            m_auto.ParamSave();
        }

        private void toolUnlock_Click(object sender, EventArgs e)
        {
            if (m_auto.m_login.m_lvLogin < 3) Unlock(false); 
            else 
            Unlock(!dockPanel.AllowEndUserDocking); 
        }

        void Unlock(bool bUnlock)
        {
            dockPanel.AllowEndUserDocking = bUnlock;
            if (bUnlock) 
            {
                toolUnlock.Text = "Unlock"; dockPanel.DocumentStyle = DocumentStyle.DockingWindow;
                dockPanel.Invalidate();
                m_auto.ShowAll();
            }
            else 
            { 
                toolUnlock.Text = "Lock"; 
                dockPanel.DocumentStyle = DocumentStyle.DockingSdi; 
                dockPanel.Invalidate();
                m_auto.ShowAll();
            }
        }

        private void toolShow_Click(object sender, EventArgs e)
        {
            m_auto.ShowAll(); 
        }

        private void toolModel_Click(object sender, EventArgs e)
        {
            m_model.ShowDialog();
            if (m_model.m_bChange) 
                Close();
        }

        void ShowMenu()
        {
            toolShow.Visible = true;
            toolLogIn.Text = m_auto.m_login.GetButtonText();
            toolModel.Visible = (m_auto.m_login.m_lvLogin >= 4);
            toolUnlock.Visible = (m_auto.m_login.m_lvLogin >= 3);
            toolLampSet.Visible = (m_auto.m_login.m_lvLogin >= 3) && ((m_model.m_strModel == "EFEM") || (m_model.m_strModel == "EFEM_C"));
            if (m_model.m_strModel == "AVIS_3800")
            {
                menuSaveJob.Text = "다른이름으로 저장";
            //    menuSaveParam.Text = "저장 : " + ((AVIS_3800)m_auto).m_strJob;
            }
        }

        private void toolLampSet_Click(object sender, EventArgs e)
        {
            m_Lamp.ShowDialog();
           // if (m_Lamp.m_bChange) Close();
        }
    }
}

