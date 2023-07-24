
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;
using ezTools;
using ezAutoMom;
using System.Diagnostics;
using System.IO;

namespace ezAuto_EFEM
{
    public class EFEM : Auto_Mom
    {
        Log m_log;
        EFEM_Control m_control = new EFEM_Control();
        EFEM_Handler m_handler = new EFEM_Handler();
        Work_Mom m_work = null; 
        new UI_EFEM_Mom m_uiEFEM = null;
        Recipe_Mom m_recipe = new EFEM_Recipe();
        public OHT_EFEM m_OHT = new OHT_EFEM();
        public EFEM_RnR m_RnR = new EFEM_RnR();
        XGem300_Mom m_xGem = null;
        ImageEdit m_imageEdit;
        HW_ExcuteForm m_excuteForm = new HW_ExcuteForm("KVM", 3);
        
        string[] m_strWorks = Enum.GetNames(typeof(eWork));
        string[] m_strOHTTypes = Enum.GetNames(typeof(eOHTType));
        bool[] m_bOHTRunning = new bool[2] { false, false };
        bool m_bUseMarsLog = false; 
        MarsLog m_logMars; 
        enum eXGem
        {
            None,
            SSEM,
            SEC,
            Hynix,
            MICRON,
        }; 
        string m_strXGem = eXGem.None.ToString();
        string[] m_strXGems = Enum.GetNames(typeof(eXGem));
        
        public override bool IsCSTLoadOK(int nLPNum)
        {
            return m_OHT.IsCSTLoadOK2(nLPNum);
        }

        public override void Init(Model model, DockPanel dockPanel)
        {
            
            string strInit = @"C:\AVIS\INIT\InitModule.exe";


            if (File.Exists(strInit)) {
                m_InitState.Init();
                System.Diagnostics.Process ps = new System.Diagnostics.Process();
                ps.StartInfo.FileName = strInit;
                ps.Start();
            }


            m_strWork = eWork.Hynix.ToString();
            base.Init(model, dockPanel);
            m_log = new Log("EFEM", ClassLogView());
            if (m_bUseMarsLog) m_logMars = new MarsLog("Mars", m_log); 
            m_control.Init("Control", 3, m_logView);
            m_InitState.SetInitDone(InitState.eInitModule.MotorIO);
            m_handler.Init("Handler", this);
            m_work.Init("Work", this);
            m_OHT.Init(this);
            m_InitState.SetInitDone(InitState.eInitModule.OHT);
            m_RnR.Init(this);
            m_log.Add("Work = " + m_strWork); 

            m_control.RunGrid(eGrid.eRegRead); 
            m_control.RunGrid(eGrid.eInit);
            m_control.m_axiss.InitComboBox();

            m_uiEFEM = (UI_EFEM_Mom)(m_work.MakeUI());
            m_uiEFEM.Init(this);
            if (m_handler.m_backSide.m_bEnable) m_uiEFEM.AddBacksideUI();
            if (m_handler.m_imageVS.m_bUse) m_uiEFEM.AddImageVSUI();

            m_recipe.Init("Recipe", this);
            m_lvLogin = m_login.m_lvLogin; RunLogin();
            if(m_bXgemUse) m_xGem.Init("XGem",this);
            m_InitState.SetInitDone(InitState.eInitModule.SECSGEM);
            if (m_strWork == eWork.MSB.ToString())
            {
                m_imageEdit = new ImageEdit();
                m_imageEdit.Init("ImageEdit", this);
            }
            if (m_excuteForm.m_bUse) m_excuteForm.Init(this, m_log);

             
            ProcessKill("InitModule");
        }

        void ProcessKill(string id)
        {
            Process[] ProcessList = Process.GetProcessesByName(id);
            foreach (Process process in ProcessList)
            {
                process.Kill();
            }
        }

        public override void ThreadStop()
        {
            m_work.ThreadStop(); 
            m_handler.ThreadStop();
            m_control.ThreadStop();
            m_xGem.ThreadStop();
            m_excuteForm.ThreadStop();
            m_uiEFEM.ThreadStop();
            if (m_bUseMarsLog && m_logMars != null) m_logMars.ThreadStop(); 
            base.ThreadStop();
        }

        public override IDockContent GetContentFromPersistString(string persistString)
        {
            IDockContent iDock = null;
            iDock = m_control.GetContentFromPersistString(persistString);
            if (iDock != null) return iDock;
            iDock = m_handler.m_aligner.GetContentFromPersistString(persistString);
            if (iDock != null) return iDock;
            iDock = m_handler.m_backSide.GetContentFromPersistString(persistString);
            if (iDock != null) return iDock;
            if (m_excuteForm.m_bUse) iDock = m_excuteForm.GetContentFromPersistString(persistString);
            if (iDock != null) return iDock;
            iDock = m_uiEFEM.GetContentFromPersistString(persistString);
            if (iDock != null) return iDock;
            if (m_handler.m_bFFUUse && m_handler.ClassFFU() != null)
            {
                iDock = m_handler.ClassFFU().GetContentFromPersistString(persistString);
                if (iDock != null) return iDock;
            } 
            if (m_bUseMarsLog && m_logMars != null) iDock = m_logMars.GetContentFromPersistString(persistString); 
            if (persistString == m_logView.GetType().ToString()) return m_logView;
            else if (persistString == m_logView.m_logPopup.GetType().ToString()) return m_logView.m_logPopup;
            else if (persistString == m_handler.GetType().ToString()) return m_handler;
            //else if (persistString == m_uiEFEM.GetType().ToString()) return m_uiEFEM.GetIDockContent(persistString);
            else if (persistString == m_recipe.GetType().ToString()) return m_recipe;
            else if (persistString == m_xGem.GetType().ToString()) return m_xGem;
            else if (persistString == m_OHT.GetType().ToString()) return m_OHT;
            else if (m_imageEdit != null && persistString == m_imageEdit.GetType().ToString()) return m_imageEdit;
            return null;
        }

        public override void ShowAll()
        {
            base.ShowAll();
            m_control.ShowAll(m_dockPanel);
            m_handler.m_aligner.ShowAll(m_dockPanel);
            m_handler.m_backSide.ShowAll(m_dockPanel); // ing 160912
            if (m_handler.m_ffu != null && m_handler.m_bFFUUse) m_handler.m_ffu.ShowAll(m_dockPanel); // MarsFFU 
            m_handler.Show(m_dockPanel);
            
            m_recipe.Show(m_dockPanel);
            if (m_strXGem != eXGem.None.ToString()) m_xGem.Show(m_dockPanel);
            //m_xGem200.Show(m_dockPanel);
            m_OHT.Show(m_dockPanel);
            m_RnR.Show(m_dockPanel);
            if (m_imageEdit != null) m_imageEdit.Show(m_dockPanel);
            if (m_excuteForm.m_bUse) m_excuteForm.Show(m_dockPanel);
            m_uiEFEM.ShowPanel(m_dockPanel);
            if (m_bUseMarsLog && m_logMars != null) m_logMars.Show(m_dockPanel); 
           
        }

        public override void Invalidate()
        {
            this.Invalidate();
        }

        public override void RunLogin()
        {
            int lvLogin = 0;
            //m_uiEFEM.SetEnable(m_lvLogin > 0); 
            m_bOHTRunning[1] = false;
            if (m_OHT.GetMainCycle(0) != OHT_EFEM.eMainCycle.PROCESSEND && m_OHT.GetMainCycle(0) != OHT_EFEM.eMainCycle.Init && m_OHT.GetMainCycle(0) != OHT_EFEM.eMainCycle.Loadstate && m_OHT.GetMainCycle(0) != OHT_EFEM.eMainCycle.Error) m_bOHTRunning[1] = true;
            if (m_OHT.GetMainCycle(1) != OHT_EFEM.eMainCycle.PROCESSEND && m_OHT.GetMainCycle(1) != OHT_EFEM.eMainCycle.Init && m_OHT.GetMainCycle(1) != OHT_EFEM.eMainCycle.Loadstate && m_OHT.GetMainCycle(1) != OHT_EFEM.eMainCycle.Error) m_bOHTRunning[1] = true;
            if (!m_bOHTRunning[0] && m_bOHTRunning[1])
            {
                m_log.Popup("OHT가 동작중입니다. UI Lock을 설정합니다.");
                m_bOHTRunning[0] = m_bOHTRunning[1];
            }
            else if (m_bOHTRunning[0] && !m_bOHTRunning[1])
            {
                m_log.Popup("OHT가 동작이 끝났습니다. UI Lock을 해제합니다.");
                m_bOHTRunning[0] = m_bOHTRunning[1];
            }
            m_uiEFEM.SetEnable(!m_bOHTRunning[1]); 
            if (!m_work.IsRun()) lvLogin = m_lvLogin;
            m_logView.Enabled = (m_lvLogin > 0);
            m_control.RunLogIn(lvLogin >= 3);
            m_handler.Enabled = (m_lvLogin > 1);
            if (m_imageEdit != null) m_imageEdit.Enabled = (lvLogin > 0);
        }

        public override void RunEmergency()
        {
            m_control.RunEmergency();
        }

        public override void ModelGrid(ezGrid rGrid, eGrid eMode)
        {
            rGrid.Update(eMode, null);
            rGrid.Set(ref m_bUseMarsLog, "Setting", "UseMarsLog", "Use Mars Log For SEC"); 
            rGrid.Set(ref m_bXgemUse,"XGem","Use","XGemUse");
            rGrid.Set(ref m_strXGem,  m_strXGems, "XGem", "Type", "XGem Type ");
            rGrid.Set(ref m_bUseFANAlarm, "UseFANUI", "UseFANUI", "USE FAN Alarm UI");
            
            if (m_xGem == null) 
            {
                if (m_strXGem == eXGem.SEC.ToString())
                    m_xGem = new XGem300_SEC();
                else if (m_strXGem == eXGem.Hynix.ToString())
                    m_xGem = new XGem300_Mom();
                else if (m_strXGem == eXGem.None.ToString())
                    m_xGem = new XGem300_Mom();
                else if (m_strXGem == eXGem.MICRON.ToString())
                    m_xGem = new XGem300_MICRON();
                else
                    m_xGem = new XGem300_Mom();
            }


            rGrid.Set(ref m_strWork, m_strWorks, "Model", "Work", "Work Type"); 
            if (m_work == null)
            {
                if (m_strWork == eWork.Hynix.ToString()) m_work = new EFEM_Work_Hynix();
                if (m_strWork == eWork.Hynix_RGBY.ToString()) m_work = new EFEM_Work_Hynix_RGBY();
                if (m_strWork == eWork.Sanken.ToString()) m_work = new EFEM_Work_Sanken();
                if (m_strWork == eWork.SSEM.ToString()) m_work = new EFEM_Work_SSEM();
                if (m_strWork == eWork.SEC.ToString()) m_work = new EFEM_Work_SEC();
                if (m_strWork == eWork.MSB.ToString()) m_work = new EFEM_Work_MSB();
                if (m_work == null)
                {
                    m_work = new EFEM_Work_Hynix();
                    m_log.Popup("Work Model Select Error !!"); 
                }
            }
            m_work.ModelGrid(rGrid, eMode);

            m_control.m_axiss.ModelGrid(rGrid, eMode); 
            m_handler.ModelGrid(rGrid, eMode);
            m_recipe.ModelGrid(rGrid, eMode);
            rGrid.Set(ref m_strOHTType, m_strOHTTypes, "OHT", "OHTType", "OHT IO Board Type");                  //190421 SDH ADD
            rGrid.Set(ref nInStartNum, "OHT", "InStartNum", "OHT Input Signal Start Number");                   //190421 SDH ADD
            rGrid.Set(ref nOutStartNum, "OHT", "OutStartNum", "OHT Output Signal Start Number");                //190421 SDH ADD
            m_OHT.str_OHT_Model = m_strWork;
            m_OHT.str_OHT_IOType = m_strOHTType;
            m_excuteForm.ModelGrid(rGrid, eMode);
        }

        public override void Grab(int nID)
        {
            if (nID == 0) m_handler.m_aligner.Grab();
            if (nID == 1) m_handler.m_backSide.Grab(0); // ing for backside grab
            if (nID == 2) m_handler.m_backSide.Grab(1); // ing for backside grab
        }

        public override bool IsEdit(int nID)
        {
            if (m_imageEdit == null) return false;
            if (m_imageEdit.IsEdit(nID)) return true;
            return false;
        }

        public override void CheckLBD(int nID, CPoint cpImg, bool bCBD)
        {
            if (m_imageEdit == null) return;
            if (m_imageEdit.CheckLBD(nID, cpImg, bCBD)) return;
        }

        public override void CheckRBD(int nID, CPoint cpImg, bool bCBD)
        {
        }

        public override void CheckMove(int nID, CPoint cpImg, bool bFinish)
        {
            if (m_imageEdit == null) return;
            m_imageEdit.CheckMove(nID, cpImg, bFinish);
        }

        public override void UpdateLoadport(int nID)
        {
            m_uiEFEM.UpdateLoadport(nID); 
        }

        public override void Draw(Graphics dc, int nID, ezImgView imgView)
        {
            if (nID == 0) m_handler.m_aligner.Draw(dc, imgView);
            if (nID == 1 || nID == 2) m_handler.m_backSide.Draw(dc, imgView);
            if (m_imageEdit == null) return;
            if (m_imageEdit != null) m_imageEdit.Draw(dc, nID, imgView);
        }

        public override void Invalidate(int nID)
        {
            if (nID == 0) m_handler.m_aligner.InvalidView();
            if (nID == 1 || nID == 2) m_handler.m_backSide.InvalidView();
            if (m_handler.ClassBackSide().GetView(nID) == null) return;
            m_handler.ClassBackSide().GetView(nID).InvalidView(false);
        }

        public override ezView_Mom ClassView(int nView) 
        {
            if (nView == 0) return m_handler.m_aligner.ClassView();
            return m_handler.ClassBackSide().GetView(nView); 
        }

        public override Control_Mom ClassControl() { return m_control; }
        public override Handler_Mom ClassHandler() { return m_handler; }
        public override Work_Mom ClassWork() { return m_work; }
        public override Recipe_Mom ClassRecipe() { return m_recipe; }
        public override XGem300_Mom ClassXGem() { return m_xGem; }
        public override HW_RnR_Mom ClassRnR() { return m_RnR; }
        public override void ShowOHTUI() { if (m_OHT != null) m_OHT.Show(); }
        public override UI_EFEM_Mom GetEFEMUI()
        {
            return m_uiEFEM;
        }
		public override void SetCSTLoadOK(int nLPNum, bool bSet)
        {
            m_OHT.SetCSTLoadOK(nLPNum, bSet);
        }
        public override MarsLog ClassMarsLog()
        {
            return m_logMars;
        }  

    }
}
