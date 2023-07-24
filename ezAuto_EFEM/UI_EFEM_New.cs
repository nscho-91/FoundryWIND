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
    public partial class UI_EFEM_New : DockContent, UI_EFEM_Mom
    {
        Auto_Mom m_auto;
        Handler_Mom m_handler;
        HW_VisionWorks_Mom m_VisionWorks;
        UI_Button m_UI_Button = null;
        UI_Loadport[] m_UI_Loadport = null;
        UI_JobLog m_UI_JobLog = null;
        UI_Time m_UI_Time = null;
        UI_SystemTime m_UI_SystemTime = null;
        Ui_FDC m_UI_FDC = null;
        UI_Simulation m_UI_Simnulation = null;
        UI_STATE m_UI_STATE = null;
        UI_RunData m_UI_RunData = null;
        Log m_log = null;
        int i = 0;
        
        public UI_EFEM_New()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.CloseButtonVisible = false; 
        }
        public void Init(Auto_Mom auto) 
        {
            m_auto = auto;
            m_handler = m_auto.ClassHandler();
            m_VisionWorks = m_auto.ClassHandler().ClassVisionWorks();
            m_log = new Log("UI_EFEM", m_auto.ClassLogView(), "UI");
            m_UI_Button = new UI_Button();
            m_UI_Loadport = new UI_Loadport[m_auto.ClassHandler().m_nLoadPort];
            m_UI_JobLog = new UI_JobLog();
            m_UI_Time = new UI_Time();
            m_UI_SystemTime = new UI_SystemTime();
            m_UI_FDC = new Ui_FDC();
            //m_UI_Simnulation = new UI_Simulation();
            m_UI_RunData = new UI_RunData();
            m_UI_STATE = new UI_STATE();
            for (int i = 0; i < m_auto.ClassHandler().m_nLoadPort; i++)
            {
                m_UI_Loadport[i] = new UI_Loadport();
                m_UI_Loadport[i].Init(i, auto);
            }

            m_UI_Button.Init(auto, m_log);
            m_UI_JobLog.Init(auto);
            //m_UI_Simnulation.Init(auto);
            m_UI_FDC.Init(auto, m_auto.ClassHandler().ClassFDC(),m_log);
            m_UI_FDC.SetTableLayoutPanel();
            m_UI_STATE.Init(auto, m_log);
            m_UI_RunData.Init(auto);

        }
        public IDockContent GetIDockContent(string persistString) 
        {
            return null;  
        }
        

        public IDockContent GetContentFromPersistString(string persistString)
        {
            IDockContent dock = null;
            if (persistString == m_UI_Button.GetType().ToString()) dock = m_UI_Button;
            else if (persistString == m_UI_Loadport[0].GetType().ToString()) 
            {
                dock = m_UI_Loadport[i];
                i++;
            }
            else if (persistString == m_UI_JobLog.GetType().ToString()) dock = m_UI_JobLog;
            else if (persistString == m_UI_Time.GetType().ToString()) dock = m_UI_Time;
            else if (persistString == m_UI_SystemTime.GetType().ToString()) dock = m_UI_SystemTime;
            else if (persistString == m_UI_FDC.GetType().ToString()) dock = m_UI_FDC;
            //else if (persistString == m_UI_Simnulation.GetType().ToString()) dock = m_UI_Simnulation;
            else if (persistString == m_UI_STATE.GetType().ToString()) dock = m_UI_STATE;
            return dock;
        }

        public void ShowPanel(DockPanel dockPanel) 
        {
            m_UI_Button.ShowPanel(dockPanel);
            for (int i = 0; i < m_handler.m_nLoadPort; i++)
                m_UI_Loadport[i].ShowPanel(dockPanel);
            m_UI_JobLog.ShowPanel(dockPanel);
            m_UI_Time.ShowPanel(dockPanel);
            m_UI_SystemTime.ShowPanel(dockPanel);
            if(m_handler.m_bFDCUse)
                m_UI_FDC.ShowPanel(dockPanel);
           // m_UI_Simnulation.ShowPanel(dockPanel);
            m_UI_STATE.ShowPanel(dockPanel);
        }

        public void SetEnable(bool bEnable) { }
        public void UpdateLoadport(int nID) { }
        public void AddBacksideUI() { }
        public void AddImageVSUI() { }
        public void ThreadStop() { }  
        private void timer1_Tick(object sender, EventArgs e)
        {
            m_UI_Time.SetTime();
        }

        public void SetLP()
        {
            for (int i = 0; i < m_auto.ClassHandler().m_nLoadPort; i++)
            {
                m_UI_Loadport[i] = new UI_Loadport();
                m_UI_Loadport[i].Init(i, m_auto);
            }
        }

        public void SetWTRMotion(eWTRSimnulMotion Motion)
        {
         ///   m_UI_Simnulation.SetWTRMotion(Motion);
        }

        public void AddRunData(Work_Mom.cRunData RunData)
        {
            TimeSpan timeDiff = RunData.dOutTime - RunData.dInTime;

            RunData.sUPEH = (Convert.ToDouble(RunData.sLoadQty) / Convert.ToDouble(timeDiff.TotalSeconds) * 3600).ToString("0.00");
            RunData.dTotalTime = timeDiff.TotalSeconds.ToString();


            if (RunData.sLotID == "LotID")
                RunData.sLotID = "Empty";
            m_UI_RunData.AddRunData(RunData.sDate, RunData.sLotID, RunData.sPartNo, RunData.dInTime.ToString("HH:mm:ss"), RunData.dOutTime.ToString("HH:mm:ss"), RunData.dTotalTime.ToString(), RunData.dTotalTime.ToString(), "0", RunData.sUPEH, RunData.sLoadQty);
        }

        public void AddRunData2()
        {

            m_UI_RunData.AddRunData(DateTime.Now.ToString(), "LOT2", "PART", DateTime.Now.ToString(), DateTime.Now.ToString(), DateTime.Now.ToString(), DateTime.Now.ToString(), DateTime.Now.ToString(), DateTime.Now.ToString(), "10");

        }

        public void AddStopData(Work_Mom.cStopData StopData)
        {
            if (StopData.sLotID == "LotID")
                StopData.sLotID = "Empty";

            m_UI_RunData.AddStopData(StopData.sDate, StopData.sErrorCode, StopData.sLotID, StopData.sStartTime, StopData.sEndTime, StopData.sStopTime, StopData.sContents);
        }

    }
}
