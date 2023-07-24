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
    public partial class UI_EFEM : DockContent, UI_EFEM_Mom
    {
        const string NONE = " - ";

        public delegate void ShowRecipeSetting();
     //   public event ShowRecipeSetting ShowRcpSetting;

        Handler_Mom m_handler;
        Work_Mom m_work; 
        UserControlLoadPort[] m_ucLP;
        Auto_Mom m_auto;
        HW_VisionWorks_Mom m_visionWorks;
        Log m_log = null;
        Info_Carrier [] m_infoCarrier = new Info_Carrier [3];
        int m_nLoadPort = 0;
        Info_Carrier.eState[] m_LastCarrierState = new Info_Carrier.eState[3];
        UserControlFDCModule[] m_ucFDC;
        HW_FDC_Mom m_FDC;

        public UI_EFEM()
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
            m_work = auto.ClassWork(); 
            m_nLoadPort = m_handler.m_nLoadPort;
            m_log = new Log("UI_EFEM", m_auto.ClassLogView(), "UI");
            m_infoCarrier[0] = m_handler.ClassCarrier(0);
            m_infoCarrier[1] = m_handler.ClassCarrier(1);
            m_infoCarrier[2] = m_handler.ClassCarrier(2);

            m_visionWorks = m_handler.ClassVisionWorks();
            m_FDC = m_handler.ClassFDC();

            SetLP();
            SetJobLogCol();
            SetFDCModule();
        }

        public IDockContent GetIDockContent()
        {
            return this;
        }

        public void ShowPanel(DockPanel dockPanel)
        {
            this.Show(dockPanel); 
        }

        public void SetEnable(bool bEnable)
        {
            this.Enabled = bEnable; 
        }

        private void SetLP()
        {
            tableLayoutPanelLP.ColumnCount = m_nLoadPort;
            m_ucLP = new UserControlLoadPort[m_nLoadPort];
            for (int n = 0; n < m_nLoadPort; n++)
            {
                this.tableLayoutPanelLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
                HW_LoadPort_Mom lp = m_handler.ClassLoadPort(n);
                m_ucLP[n] = new UserControlLoadPort(n, lp, m_auto);
                this.tableLayoutPanelLP.Controls.Add(m_ucLP[n], n, 0);
                m_ucLP[n].Dock = DockStyle.Fill;
            }
        }

        private void SetJobLogCol()
        {
            listViewJobLog.Columns[0].Name = "No";
            listViewJobLog.Columns[1].Name = "LoadPort";
            listViewJobLog.Columns[2].Name = "JobID";
            listViewJobLog.Columns[2].Name = "LotID";
            listViewJobLog.Columns[3].Name = "CarrierID";
            listViewJobLog.Columns[4].Name = "Recipe";
            listViewJobLog.Columns[5].Name = "SlotMap";
            listViewJobLog.Columns[6].Name = "StartTime";
            listViewJobLog.Columns[7].Name = "EndTime";
            listViewJobLog.Columns[8].Name = "State";
        }

        private void SetFDCModule()
        {
            if(m_auto.ClassHandler().m_bFDCUse){
                int nFDCModuleNum = m_auto.ClassHandler().m_nFDCModuleNum;
                m_ucFDC = new UserControlFDCModule[nFDCModuleNum];
                this.tableLayoutPanelFDC.RowCount = nFDCModuleNum;
                for(int n = 0; n < nFDCModuleNum; n++){
                    m_ucFDC[n] = new UserControlFDCModule(n, m_FDC.m_FDCModule[n].m_sName,m_log);
                    this.tableLayoutPanelFDC.Controls.Add(m_ucFDC[n], 0, n);
                    m_ucFDC[n].Dock = DockStyle.Fill;
                }
            }
        }

        private void UI_EFEM_Load(object sender, EventArgs e)
        {

        }

        private void UI_EFEM_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible == true)
            {
                this.Font = new Font(this.Font.Name, 12);
            }
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            bool bCoverOpen = false;
            bool bDoorOpen = false;
            for (int n = 0; n < m_nLoadPort; n++)
            {
                if (!m_handler.ClassLoadPort(n).IsCoverClose()) bCoverOpen = true;
                if (!m_handler.ClassLoadPort(n).IsDoorOpenPos()) bDoorOpen = true;
            }
            this.Invalidate();
            btnStart.Enabled = m_work.m_run.IsEnableSW() && (m_work.GetState() == eWorkRun.Ready) && m_handler.m_bEnableStart;
            btnStop.Enabled = m_work.m_run.IsEnableSW() && ((m_work.GetState() == eWorkRun.Run) || (m_work.GetState() == eWorkRun.AutoUnload));
            btnCycleStop.Enabled = !m_handler.ClassWTR().m_bCycleStop && m_work.m_run.IsEnableSW() && (m_work.GetState() == eWorkRun.Run) && m_handler.IsWaferOut();
            btnRecovery.Enabled = m_work.m_run.IsEnableSW() && bDoorOpen &&(m_work.GetState() == eWorkRun.Ready) && !m_handler.m_bEnableStart;
            btnHome.Enabled = m_work.m_run.IsEnableSW() && !bCoverOpen && ((m_work.GetState() == eWorkRun.Init) || (m_work.GetState() == eWorkRun.Ready));
            btnReset.Enabled = m_work.m_run.IsEnableSW() && ((m_work.GetState() == eWorkRun.Error) || (m_work.GetState() == eWorkRun.Warning0) || (m_work.GetState() == eWorkRun.Warning1));
            btnBuzzerOff.Enabled = m_work.m_run.IsEnableSW() && m_work.IsBuzzerOn();
            ChangeEQPStatePanel();
            for (int i = 0; i < m_handler.m_nFDCModuleNum; i++)
                m_ucFDC[i].SetFDCData(m_FDC.m_FDCModule[i]);
        }

        private void AddJobLog(int nID)
        {
            listViewJobLog.BeginUpdate();
            Info_Carrier info = m_infoCarrier[nID];
            ListViewItem lvi = new ListViewItem((listViewJobLog.Items.Count + 1).ToString());

            //lvi.SubItems.Add((listViewJobLog.Items.Count+1).ToString());
            lvi.SubItems.Add((nID+1).ToString());
            lvi.SubItems.Add(info.m_strJobID);
            lvi.SubItems.Add(info.m_strLotID);
            lvi.SubItems.Add(info.m_strCarrierID);
            lvi.SubItems.Add(info.m_strRecipe);
            lvi.SubItems.Add(info.GetSlotMap());
            lvi.SubItems.Add(info.m_timeLotStart.ToString("MM-dd HH:mm:ss"));
            //lvi.SubItems.Add(info.m_timeLotEnd.ToString("-"));
            lvi.SubItems.Add("");
            lvi.SubItems.Add(info.m_eJobState.ToString());
            listViewJobLog.Items.Add(lvi);
            listViewJobLog.Items[listViewJobLog.Items.Count - 1].Selected = true;
            listViewJobLog.EndUpdate();
        }

        private void ChangeJobLog(string carrierID, string Header, object content)
        {
            listViewJobLog.BeginUpdate();
            //var item = listViewJobLog.FindItemWithText(carrierID);
            //int nIndex = listViewJobLog.Items.IndexOf(item);
            int nindex = listViewJobLog.Items.Count-1;


            ColumnHeader colHeader = listViewJobLog.Columns[Header];
            int headerIndex = listViewJobLog.Columns.IndexOf(colHeader)+1;

            ListViewItem.ListViewSubItem subitem = new ListViewItem.ListViewSubItem();
            subitem.Text = content.ToString();
            //listViewJobLog.Items[0].SubItems.RemoveAt(headerIndex);
            listViewJobLog.Items[nindex].SubItems[headerIndex].Text = subitem.Text;
            //listViewJobLog.Items[0].SubItems.Insert(headerIndex, subitem);
            listViewJobLog.EndUpdate();
        }

        private void ChangeEQPStatePanel()
        {
            string lbText = "";
            Color lbColor = Color.Yellow;
            switch (m_auto.ClassWork().m_run.GetState())
            {
                case eWorkRun.Init:
                    lbText = "INIT";
                    lbColor = Color.Wheat;
                    break;
                case eWorkRun.Ready:
                    lbText = "STOP";
                    lbColor = Color.LemonChiffon;
                    break;
                case eWorkRun.Run:
                    lbText = "RUN";
                    lbColor = Color.SpringGreen;
                    break;
                case eWorkRun.Home:
                    lbText = "HOME";
                    lbColor = Color.DeepSkyBlue;
                    break;
                case eWorkRun.Error:
                    lbText = "ERROR";
                    lbColor = Color.Red;
                    break;
                case eWorkRun.Warning0:
                case eWorkRun.Warning1:
                    lbColor = Color.Tomato;
                    lbText = "WARNING";
                    break;
                case eWorkRun.AutoUnload:
                    lbText = "RECOVERY";
                    lbColor = Color.LightSkyBlue;
                    break;
            }
            lbEQPState.Text = lbText;
            lbEQPState.BackColor = lbColor;
        }

        private void DrawTime()
        {
            labelMonthDay.Text = DateTime.Now.ToString("MM-dd");
            labelTime.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void DrawInspTime()
        {
            TimeSpan time = TimeSpan.FromMilliseconds(m_visionWorks.GetmsInspect());
            labelInspTime.Text = string.Format("Insp Time : {0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
        }
        public void DrawWPHTimer()
        {
            TimeSpan time = TimeSpan.FromMilliseconds(m_visionWorks.GetWPH());
            labelWPH.Text = string.Format("Last Insp Time : {0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
        }

        private void DrawOverView()
        {
            HW_VisionWorks_Mom visionworks = m_handler.ClassVisionWorks();
            IOStageVisionConnected.Set(visionworks.IsConnected());
            IOStageLifterUp.Set(visionworks.IsLifterUp());
            IOStageWaferExist.Set(visionworks.m_bWaferExist);
            SetWaferInfo(WaferInfoStage, visionworks.InfoWafer);
            labelStateStage.Text = visionworks.GetStageState();

            HW_WTR_Mom wtr = m_handler.ClassWTR();
            IOLowerArmSafty.Set(wtr.IsArmClose(HW_WTR_Mom.eArm.Lower));
            IOLowerArmWaferExist.Set(wtr.CheckWaferExist(HW_WTR_Mom.eArm.Lower) == eHWResult.On);
            SetWaferInfo(WaferInfoLowerArm, wtr.GetInfoWafer(HW_WTR_Mom.eArm.Lower));

            IOUpperArmSafty.Set(wtr.IsArmClose(HW_WTR_Mom.eArm.Upper));
            IOUpperArmWaferExist.Set(wtr.CheckWaferExist(HW_WTR_Mom.eArm.Upper) == eHWResult.On);
            SetWaferInfo(WaferInfoUpperArm, wtr.GetInfoWafer(HW_WTR_Mom.eArm.Upper));

            HW_Aligner_Mom aligner = m_handler.ClassAligner();
            IOAlignerWaferExist.Set(aligner.CheckWaferExist() == eHWResult.On);                        
            SetWaferInfo(WaferInfoAligner, aligner.InfoWafer);

            HW_LoadPort_Mom loadport = null;
            if(m_nLoadPort == 1)
            {
               loadport = m_handler.ClassLoadPort(0);
               userControlOverViewLP3.Set("LP1", loadport.IsPlaced(), loadport.IsDocking(), loadport.IsDoorOpenPos());
               userControlOverViewLP3.Visible = true;
               userControlOverViewLP2.Visible = false;
               userControlOverViewLP1.Visible = false;
            }
            else if (m_nLoadPort == 2)
            {
                loadport = m_handler.ClassLoadPort(1);
                userControlOverViewLP3.Set("LP2", loadport.IsPlaced(), loadport.IsDocking(), loadport.IsDoorOpenPos());
                loadport = m_handler.ClassLoadPort(0);
                userControlOverViewLP2.Set("LP1", loadport.IsPlaced(), loadport.IsDocking(), loadport.IsDoorOpenPos());
                userControlOverViewLP3.Visible = true;
                userControlOverViewLP2.Visible = true;
                userControlOverViewLP1.Visible = false;
            }
            else if (m_nLoadPort == 3)
            {
                loadport = m_handler.ClassLoadPort(2);
                userControlOverViewLP3.Set("LP3", loadport.IsPlaced(), loadport.IsDocking(), loadport.IsDoorOpenPos());
                loadport = m_handler.ClassLoadPort(1);
                userControlOverViewLP2.Set("LP2", loadport.IsPlaced(), loadport.IsDocking(), loadport.IsDoorOpenPos());
                loadport = m_handler.ClassLoadPort(0);
                userControlOverViewLP1.Set("LP1", loadport.IsPlaced(), loadport.IsDocking(), loadport.IsDoorOpenPos());
                userControlOverViewLP3.Visible = true;
                userControlOverViewLP2.Visible = true;
                userControlOverViewLP1.Visible = true;
            }
        }

        private void SetWaferInfo(UserControlWaferInfo wi, Info_Wafer wafer)
        {
            if (wafer == null)
            {
                wi.Set(false);
            }
            else
            {
                wi.Set(true, wafer.m_nLoadPort, wafer.m_nSlot, GetWaferSizeString(wafer.m_wafer.m_eSize), wafer.WAFERID);
            }
        }

        private string GetWaferSizeString(Wafer_Size.eSize eSize)
        {
            switch (eSize)
            {
                case Wafer_Size.eSize.inch4: return "4inch";
                case Wafer_Size.eSize.inch5: return "5inch";
                case Wafer_Size.eSize.inch6: return "6inch";
                case Wafer_Size.eSize.mm200: return "200mm";
                case Wafer_Size.eSize.mm300: return "300mm";
                case Wafer_Size.eSize.mm300_RF: return "RF_300";
                case Wafer_Size.eSize.Empty: return "Empty";
                case Wafer_Size.eSize.Error: return "Error";
                default: return "Error";
            }
        }

        private void UI_EFEM_Paint(object sender, PaintEventArgs e)
        {
            DrawTime();
            DrawOverView();           
            DrawInspTime();
            DrawWPHTimer();
            for (int i = 0; i < m_nLoadPort;i++)
                UpdateLoadport(i);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (m_handler.IsWaferExistInEQP())
            {
                bool IsDoorOpen = false;
                for (int i = 0; i < m_handler.m_nLoadPort; i++){
                    if (m_handler.ClassLoadPort(i).IsDoorOpenPos())
                        IsDoorOpen = true;
                }
                if (IsDoorOpen == false){
                    m_log.Popup("Loadport Load First For Recover Wafer");
                    return;
                }
            }
            m_work.m_bBuzzerOff = false;
            m_work.m_run.Run();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            m_work.m_run.Stop(); 
        }

        private void btnCycleStop_Click(object sender, EventArgs e)
        {
            if (m_work.IsRun())
            {
                m_handler.ClassWTR().m_bCycleStop = true; 
            }
        }

        private void btnRecovery_Click(object sender, EventArgs e)
        {
            int nPortNum = GetPortForRecovery();

            if (nPortNum == -1 || !m_handler.ClassLoadPort(nPortNum).IsDoorOpenPos())
            {
                m_log.Popup("Loadport("+nPortNum+") Load First");
                return;
            }

            EFEM_Recovery recover = new EFEM_Recovery();
            //m_log.HidePopup(true);
            recover.Init("Recover", m_auto, m_log, nPortNum);
            if (recover.ShowDialog() != DialogResult.OK) return;
            if (m_handler.CheckUnloadOK() != true) return;
            m_work.m_run.SetManual(eWorkRun.AutoUnload);
        }

        private int GetPortForRecovery()
        {
            HW_VisionWorks_Mom Vision = m_visionWorks ;
            HW_Aligner_Mom Aligner = m_handler.ClassAligner();
            HW_WTR_Mom WTR = m_handler.ClassWTR();

            if (Vision.InfoWafer != null && Vision.IsWaferExist() == eHWResult.On)
                return Vision.InfoWafer.m_nLoadPort;
            else if (Aligner.InfoWafer != null && Aligner.CheckWaferExist() == eHWResult.On)
                return Aligner.InfoWafer.m_nLoadPort;
            else if (WTR.GetInfoWafer(HW_WTR_Mom.eArm.Lower) != null && WTR.CheckWaferExist(HW_WTR_Mom.eArm.Lower) == eHWResult.On)
                return WTR.GetInfoWafer(HW_WTR_Mom.eArm.Lower).m_nLoadPort;
            else if (WTR.GetInfoWafer(HW_WTR_Mom.eArm.Upper) != null && WTR.CheckWaferExist(HW_WTR_Mom.eArm.Upper) == eHWResult.On)
                return WTR.GetInfoWafer(HW_WTR_Mom.eArm.Upper).m_nLoadPort;

            for(int n=0; n < m_handler.m_nLoadPort;n++){
                if (m_handler.ClassLoadPort(n).IsDoorOpenPos())
                    return n;
            }

            return -1;
        }
        private void btnHome_Click(object sender, EventArgs e)
        {
            m_work.m_run.Home(); 
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (m_work.m_bTestRun)
            {
                m_auto.ClassControl().GetInputBit(81);
                m_auto.ClassControl().WriteOutputBit(80, true);
            //    byte[] m_aBuf = new byte[8];
            //    m_aBuf[0] = 0x01;
            //    m_aBuf[3] = 0x02;
            //    m_aBuf[4] = 0x3a;
            //    int ModuluNum = Convert.ToInt16(m_aBuf[0]);
            //    int nData = Convert.ToInt16(m_aBuf[3]) * 16 * 16 + Convert.ToInt16(m_aBuf[4]);
            }
            else { 
                m_work.m_run.Reset();
            }
        }

        private void btnBuzzerOff_Click(object sender, EventArgs e)
        {
            m_work.RunBuzzerOff(); 
        }

        public void UpdateLoadport(int nID)
        {
            if (m_infoCarrier[nID].m_eState == Info_Carrier.eState.Run && m_LastCarrierState[nID] != m_infoCarrier[nID].m_eState)
            {
                m_infoCarrier[nID].m_timeLotStart = DateTime.Now;
                AddJobLog(nID);
            }
            else if (m_infoCarrier[nID].m_eState == Info_Carrier.eState.Done && m_LastCarrierState[nID] != m_infoCarrier[nID].m_eState && !(m_infoCarrier[nID].IsWaferOut()))
            {
                if (listViewJobLog.Items.Count != 0)
                {
                    m_infoCarrier[nID].m_timeLotEnd = DateTime.Now;
                    ChangeJobLog(m_infoCarrier[nID].m_strCarrierID, "EndTime", m_infoCarrier[nID].m_timeLotEnd.ToString("MM-dd HH:mm:ss"));
                    ChangeJobLog(m_infoCarrier[nID].m_strCarrierID, "State", Info_Carrier.eJobState.Done);
                }
            }
            m_LastCarrierState[nID] = m_infoCarrier[nID].m_eState;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void WaferInfoStage_Load(object sender, EventArgs e)
        {

        }

    }
}
