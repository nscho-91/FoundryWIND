using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;
using ezTools; 


namespace ezAutoMom
{
    public class Auto_Mom
    {
        public string m_id;
        public bool m_bOK = true;
        public int m_lvLogin = 0, m_nDocCount = 2;
        public int nInStartNum = 96;                                               //170105 SDH ADD OHT Input  시작 지점 설정.
        public int nOutStartNum = 96;                                              //170105 SDH ADD OHT Output 시작 지점 설정.
        public LogView m_logView;
        public Login m_login;
        public Log m_log; //230724 nscho 
        public UI_EFEM_Mom m_uiEFEM = null;
        public InitState m_InitState = new InitState();

        public enum eWork
        {
            Hynix,
            Hynix_RGBY,
            Sanken,
            SSEM,
            SEC,
            MSB
        };

        public enum eOHTType
        {
            AJIN,
            EIP,
            PIOTEST
        }

        public string m_strWork = eWork.Hynix.ToString();
        public string m_strOHTType = eOHTType.AJIN.ToString();
        
        protected DockPanel m_dockPanel;

        Model m_model;

        public virtual void Init(Model model, DockPanel dockPanel)
        {
            m_model = model; 
            m_id = m_model.m_strModel; 
            m_dockPanel = dockPanel; 
            m_logView = new LogView("LogView", m_id, m_dockPanel);
            m_log = new Log(m_id, m_logView);  //230724 nscho 
            model.Init(this); 
            m_login = new Login(m_logView);
        }

        public virtual void ThreadStop()
        {
            m_log.WriteLog("Sequence", "[P/G END]"); //230724 nscho 
            m_logView.ThreadStop();
        }

        public virtual void ShowAll()
        {
            m_logView.Show(m_dockPanel); 
        }
        public bool m_bXgemUse = false;
        public bool m_bUseFANAlarm = false;
        

        public int Login() 
        {
            if (m_login.m_lvLogin > 0)
            {
                m_lvLogin = 0;
                m_login.LogOut();
            }
            else
            {
                m_logView.HidePopup(true);
                m_login.ShowDialog();
                m_lvLogin = m_login.m_lvLogin;
                m_logView.HidePopup(false);
            }
            RunLogin(); 
            if (ClassWork() != null) ClassWork().m_bInvalid = true;
            return m_lvLogin; 
        }

        public virtual IDockContent GetContentFromPersistString(string persistString) { return null; }
        public virtual void JobOpen() { }
        public virtual bool JobOpen(string str) { return false; } //kns20160417
        public virtual void JobSave() { }
        public virtual void JobSaveLog() { }
        public virtual void ParamSave() { }
        public virtual void RunLogin() { }
        public virtual void RunEmergency() { }
        public virtual void ClearResult(int nID, Info_Strip infoStrip, int lUnit, bool bRun) { }
        public virtual void Clear() { }
        public virtual void Grab(int nID) { }
        public virtual void Invalidate(int nID) { }
        public virtual void UpdateLoadport(int nID) { }
        public virtual bool IsEdit(int nID) { return false; }
        public virtual void CheckLBD(int nID, CPoint cpImg, bool bCBD) { }
        public virtual void CheckRBD(int nID, CPoint cpImg, bool bCBD) { }
        public virtual void CheckMove(int nID, CPoint cpImg, bool bFinish) { }
        public virtual void Draw(Graphics dc, int nID, ezImgView imgView) { }
        public virtual void ModelGrid(ezGrid rGrid, eGrid eMode) { }
        public virtual void RunAuto(object obj) { }
        public Model ClassModel() { return m_model; }
        public LogView ClassLogView() { return m_logView; }
        public virtual Control_Mom ClassControl() { return null; }
        public virtual Count_Mom ClassCount() { return null; }
        public virtual Work_Mom ClassWork() { return null; }
        public virtual Handler_Mom ClassHandler() { return null; }
        public virtual Light_Mom ClassLight() { return null; }
        public virtual XGem300_Mom ClassXGem() { return null; }
        public virtual XGem300Pro_Mom ClassXGemPro() { return null; }
        public virtual Boat_Mom ClassBoat(int nBoat) { return null; }
        public virtual ezView_Mom ClassView(int nView) { return null; }
        public virtual ezImgView ClassImgView(int nID) { return null; }
        public virtual Reject_Mom ClassReject(int nID) { return null; }
        public virtual Recipe_Mom ClassRecipe() { return null; }
        public virtual Display_Mom ClassDisplay() { return null; }
        public virtual void ShowOHTUI() { return; }
        public virtual bool IsCSTLoadOK(int nLPNum) { return false; }
		public virtual void SetCSTLoadOK(int nLPNum, bool bSet) { }

        public DockPanel GetDockPanel() { return m_dockPanel; }
        public virtual UI_EFEM_Mom GetEFEMUI() { return null; }
        public virtual HW_RnR_Mom ClassRnR() { return null; }
        public virtual void Invalidate() { }
        public virtual void AddToolStrip(System.Windows.Forms.ToolStrip toolStrip) { }
        public virtual MarsLog ClassMarsLog() { return null; } 
    }
}
