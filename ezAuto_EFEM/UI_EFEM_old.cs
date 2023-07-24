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
using ezAutoMom;
using ezTools;
using System.IO; 

namespace ezAuto_EFEM
{
    public partial class UI_EFEM_old : DockContent, UI_EFEM_Mom
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
        Info_Carrier[] m_infoCarrier = new Info_Carrier[3];
        int m_nLoadPort = 0;
        Info_Carrier.eState[] m_LastCarrierState = new Info_Carrier.eState[3];
        UserControlFDCModule[] m_ucFDC;
        HW_FDC_Mom m_FDC;

        // For Backside UI
        Panel m_panelBackside;
        Label m_labelBackside;
        UserControlWaferInfo m_waferInfoBackside;
        UserControlIOIndicator m_ioBacksideWaferExist;
        Thread m_thread; 
        Label m_labelImageVS;
        UserControlIOIndicator m_ioVSConnected;
        List<string> m_lstNew = new List<string>();
        public bool m_bRunThread = false;
        public UI_EFEM_old()
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
            if (((EFEM)m_auto).m_strWork == EFEM.eWork.SSEM.ToString())
                ChangeUIToSSEM();
            if (m_auto.m_bUseFANAlarm)
                listViewJobLog.Visible = false;
            else
                listViewJobLog.Visible = true;
            m_thread = new Thread(new ThreadStart((runthread))); m_thread.Start();
            
        }

        public void ChangeUIToSSEM()
        {
            Point LeftTop = new Point(10, 10);
            Size AllSize = new Size(1300, 1044);
            int nSideXGap = 10;
            int nModuleXGap = 10;
            int nModuleYGap = 10;
            int nCol1 = 232;
            int nCol2 = 292;
            int nCol3 = 292;
            int nCol4 = 292;
            int nCol5 = 225;
            int nRow1 = 121;
            int nRow2 = 121;
            int nRow3 = 121;
            int nRow4 = 266;
            int nRow5 = 450;

            layoutTime.SetBounds(LeftTop.X, LeftTop.Y, nCol1, nRow1 + 40);
            layoutTime.Invalidate();

            pnEQState.SetBounds(nSideXGap + nCol1 + nModuleXGap, LeftTop.Y, nCol2 + nCol3 + nCol4 + nCol5 + nModuleXGap * 5, nRow1 + nRow2 + nRow3 + nCol4 + +nModuleYGap * 5);

            lbEQPState.Parent = this;
            lbEQPState.BringToFront();
            lbEQPState.SetBounds(nSideXGap + nCol1 + nModuleXGap * 2, LeftTop.Y + nModuleYGap, nCol2, nRow1);

            panelAligner.Parent = this;
            panelAligner.BringToFront();
            panelAligner.SetBounds(nSideXGap + nCol1 + nModuleXGap * 2 + nCol2 + nModuleXGap, LeftTop.Y + nModuleYGap, nCol3, nRow1);

            userControlOverViewLP3.Parent = this;
            userControlOverViewLP3.BringToFront();
            userControlOverViewLP3.SetBounds(panelAligner.Left + panelAligner.Width + nModuleXGap, LeftTop.Y + nModuleYGap, nCol4, nRow1);

            // panel1.Parent = this;
            // panel1.BringToFront();
            //  panel1.SetBounds(userControlOverViewLP3.Left + userControlOverViewLP3.Width + nModuleXGap, LeftTop.Y + nModuleYGap, nCol5, 30);
            tableLayoutPanelFDC.Parent = this;
            tableLayoutPanelFDC.BringToFront();
            tableLayoutPanelFDC.RowStyles.Clear();
            tableLayoutPanelFDC.RowCount = 10;
            TableLayoutRowStyleCollection styles = this.tableLayoutPanelFDC.RowStyles;
            for (int i = 0; i < tableLayoutPanelFDC.RowCount; i++)
            {
                tableLayoutPanelFDC.RowStyles.Add(new RowStyle(SizeType.Percent, 100 / tableLayoutPanelFDC.RowCount));
            }
            tableLayoutPanelFDC.SetBounds(userControlOverViewLP3.Left + userControlOverViewLP3.Width + nModuleXGap, LeftTop.Y + nModuleYGap + 30, nCol5, nRow1 + nRow2 + nRow3 + nRow4 + nModuleYGap * 3 - 30);

            panel4.Parent = this;
            panel4.BringToFront();
            panel4.SetBounds(nSideXGap + nCol1 + nModuleXGap * 2, lbEQPState.Top + lbEQPState.Height + nModuleYGap, nCol2, nRow2 + nRow3 + nModuleYGap);

            panelUpperArm.Parent = this;
            panelUpperArm.BringToFront();
            panelUpperArm.SetBounds(panelAligner.Left, lbEQPState.Top + lbEQPState.Height + nModuleYGap, nCol2, nRow2);

            panelLowerArm.Parent = this;
            panelLowerArm.BringToFront();
            panelLowerArm.SetBounds(panelAligner.Left, lbEQPState.Top + lbEQPState.Height + nModuleYGap * 2 + nRow1, nCol2, nRow3);

            userControlOverViewLP2.Parent = this;
            userControlOverViewLP2.BringToFront();
            userControlOverViewLP2.SetBounds(panelAligner.Left + panelAligner.Width + nModuleXGap, LeftTop.Y + nRow2 + nModuleYGap * 2, nCol4, nRow2);

            userControlOverViewLP1.Parent = this;
            userControlOverViewLP1.BringToFront();
            userControlOverViewLP1.SetBounds(panelAligner.Left + panelAligner.Width + nModuleXGap, LeftTop.Y + nModuleYGap * 3 + nRow2 + nRow1, nCol4, nRow3);

            listViewJobLog.Parent = this;
            listViewJobLog.BringToFront();
            listViewJobLog.SetBounds(lbEQPState.Left + tableLayoutPanel1.Size.Width, panelLowerArm.Top + panelLowerArm.Height + nModuleYGap, nCol2 + nCol3 + nCol4 + nModuleXGap * 2 - tableLayoutPanel1.Size.Width, nRow4);

            tableLayoutPanelLP.SetBounds(LeftTop.X, pnEQState.Top + pnEQState.Height, nCol1 + nCol2 + nCol3 + nCol4 + nCol5 + nModuleXGap * 6, nRow5);

            panel12.SetBounds(LeftTop.X, layoutTime.Top + layoutTime.Height + nModuleYGap, nCol1, nRow2 + nRow3 + nRow4 + nModuleYGap * 2 - 40);

            int ButtonGap = 10;
            btnStart.SetBounds(ButtonGap, ButtonGap, (panel12.Width - 3 * ButtonGap) / 2, (panel12.Height - 5 * ButtonGap) / 4);
            btnHome.SetBounds(btnStart.Left + btnStart.Width + ButtonGap, ButtonGap, (panel12.Width - 3 * ButtonGap) / 2, (panel12.Height - 5 * ButtonGap) / 4);
            btnStop.SetBounds(btnStart.Left, btnStart.Top + btnStart.Height + ButtonGap, (panel12.Width - 3 * ButtonGap) / 2, (panel12.Height - 5 * ButtonGap) / 4);
            btnReset.SetBounds(btnHome.Left, btnStop.Top, (panel12.Width - 3 * ButtonGap) / 2, (panel12.Height - 5 * ButtonGap) / 4);
            btnCycleStop.SetBounds(btnStart.Left, btnStop.Top + btnStop.Height + ButtonGap, (panel12.Width - 3 * ButtonGap) / 2, (panel12.Height - 5 * ButtonGap) / 4);
            btnBuzzerOff.SetBounds(btnReset.Left, btnCycleStop.Top, (panel12.Width - 3 * ButtonGap) / 2, (panel12.Height - 5 * ButtonGap) / 4);
            btnRecovery.SetBounds(btnCycleStop.Left, btnCycleStop.Top + btnCycleStop.Height + ButtonGap, panel12.Width - ButtonGap * 2, (panel12.Height - 5 * ButtonGap) / 4);
        }

        public IDockContent GetIDockContent(string persistString)
        {
            return this;
        }

        public IDockContent GetContentFromPersistString(string persistString)
        {
            if (this.GetType().ToString() == persistString)
                return this;
            else
                return null;
        }

        public void ShowPanel(DockPanel dockPanel)
        {
            this.Show(dockPanel);
        }

        public void SetEnable(bool bEnable)
        {
            this.Enabled = bEnable;
        }

        public void SetLP()
        {
            tableLayoutPanelLP.ColumnCount = m_nLoadPort;
            m_ucLP = new UserControlLoadPort[m_nLoadPort];
            for (int n = 0; n < m_nLoadPort; n++)
            {
                this.tableLayoutPanelLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
                HW_LoadPort_Mom lp = m_handler.ClassLoadPort(n);
                m_ucLP[n] = new UserControlLoadPort(n, m_auto);
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
            if (m_auto.ClassHandler().m_bFDCUse)
            {
                int nFDCModuleNum = m_auto.ClassHandler().m_nFDCModuleNum;
                m_ucFDC = new UserControlFDCModule[nFDCModuleNum];
                this.tableLayoutPanelFDC.RowCount = nFDCModuleNum;
                for (int n = 0; n < nFDCModuleNum; n++)
                {
                    m_ucFDC[n] = new UserControlFDCModule(n, m_FDC.m_FDCModule[n].m_sName, m_log, m_auto);
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
            btnStart.Enabled = m_work.m_run.IsEnableSW() && (m_work.GetState() == eWorkRun.Ready) && m_handler.m_bEnableStart &&
                (m_handler.ClassLoadPort(0).GetState() != HW_LoadPort_Mom.eState.RunDocking && m_handler.ClassLoadPort(1).GetState() != HW_LoadPort_Mom.eState.RunDocking);
            btnStop.Enabled = m_work.m_run.IsEnableSW() && ((m_work.GetState() == eWorkRun.Run) || (m_work.GetState() == eWorkRun.AutoUnload));
            btnCycleStop.Enabled = !m_handler.ClassWTR().m_bCycleStop && m_work.m_run.IsEnableSW() && (m_work.GetState() == eWorkRun.Run) && m_handler.IsWaferOut();
            btnRecovery.Enabled = m_work.m_run.IsEnableSW() && bDoorOpen && (m_work.GetState() == eWorkRun.Ready) && !m_handler.m_bEnableStart;
            btnHome.Enabled = m_work.m_run.IsEnableSW() && !bCoverOpen && ((m_work.GetState() == eWorkRun.Init) || (m_work.GetState() == eWorkRun.Ready))
                && (m_handler.ClassLoadPort(0).GetState() != HW_LoadPort_Mom.eState.RunDocking && m_handler.ClassLoadPort(1).GetState() != HW_LoadPort_Mom.eState.RunDocking);
            btnReset.Enabled = m_work.m_run.IsEnableSW() && ((m_work.GetState() == eWorkRun.Error) || (m_work.GetState() == eWorkRun.Warning0) || (m_work.GetState() == eWorkRun.Warning1));
            btnBuzzerOff.Enabled = m_work.m_run.IsEnableSW() && m_work.IsBuzzerOn();
            ChangeEQPStatePanel();
            //KDG 161101
            if (m_auto.ClassHandler().m_bFDCUse)
            {
                for (int i = 0; i < m_handler.m_nFDCModuleNum; i++)
                    m_ucFDC[i].SetFDCData(m_FDC.m_FDCModule[i]);
            }
            if (m_auto.ClassXGem().m_eXGemModule == XGem300_Mom.eXGemModule.EXCUTE && m_auto.ClassXGem().m_eComm == XGem300_Mom.eCommunicate.COMMUNICATING)
            {
                switch (m_auto.ClassXGem().m_eControl)
                {
                    case XGem300_Mom.eControl.HOSTOFFLINE:
                    case XGem300_Mom.eControl.OFFLINE:
                    case XGem300_Mom.eControl.ATTEMPTONLINE:
                        btnOffline.BackColor = Color.Salmon;
                        btnLocal.BackColor = Color.Gray;
                        btnRemote.BackColor = Color.Gray;
                        break;
                    case XGem300_Mom.eControl.LOCAL:
                        btnOffline.BackColor = Color.Gray;
                        btnLocal.BackColor = Color.LemonChiffon;
                        btnRemote.BackColor = Color.Gray;
                        break;
                    case XGem300_Mom.eControl.ONLINEREMOTE:
                        btnOffline.BackColor = Color.Gray;
                        btnLocal.BackColor = Color.Gray;
                        btnRemote.BackColor = Color.LightGreen;
                        break;
                }
            }
            else
            {
                btnOffline.BackColor = Color.Gray;
                btnLocal.BackColor = Color.Gray;
                btnRemote.BackColor = Color.Gray;
            }
            //--> add by kiwon 211213
            //if( CheckRecipeList() == true )
            //    UpdateRecipList();
            //<-
        }

        private void AddJobLog(int nID)
        {
            listViewJobLog.BeginUpdate();
            Info_Carrier info = m_infoCarrier[nID];
            ListViewItem lvi = new ListViewItem((listViewJobLog.Items.Count + 1).ToString());

            //lvi.SubItems.Add((listViewJobLog.Items.Count+1).ToString());
            lvi.SubItems.Add((nID + 1).ToString());
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
            int nindex = listViewJobLog.Items.Count - 1;


            ColumnHeader colHeader = listViewJobLog.Columns[Header];
            int headerIndex = listViewJobLog.Columns.IndexOf(colHeader) + 1;

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
                    if (m_auto.ClassHandler().ClassLoadPort(0).GetState() > HW_LoadPort_Mom.eState.Placed || m_auto.ClassHandler().ClassLoadPort(1).GetState() > HW_LoadPort_Mom.eState.Placed || m_auto.ClassHandler().ClassLoadPort(2).GetState() > HW_LoadPort_Mom.eState.Placed)
                    {
                        lbText = "RUN";
                        lbColor = Color.SpringGreen;
                    }
                    else
                    {
                        lbText = "IDLE";
                        lbColor = Color.Khaki;
                    }
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

        private void DrawFanUI()
        {
            SetFanAlarmLabel(lbEFEMPCDoorFan, m_auto.ClassWork().m_diDoorFan[(int)Work_Mom.eFanAlarmPlace.EFEM_PC_Door_Fan]);
            SetFanAlarmLabel(lbEFEMPCFan, m_auto.ClassWork().m_diDoorFan[(int)Work_Mom.eFanAlarmPlace.EFEM_PC_Fan]);
            SetFanAlarmLabel(lbEFEM전장DoorFan, m_auto.ClassWork().m_diDoorFan[(int)Work_Mom.eFanAlarmPlace.EFEM_TOP_Door_Fan]);
            SetFanAlarmLabel(lbEFEM조명Fan, m_auto.ClassWork().m_diDoorFan[(int)Work_Mom.eFanAlarmPlace.EFEM_Illum_Fan]);
            SetFanAlarmLabel(lbVSPCDoorFan, m_auto.ClassWork().m_diDoorFan[(int)Work_Mom.eFanAlarmPlace.VS_PC_Door_Fan]);
            SetFanAlarmLabel(lbVSPCFan, m_auto.ClassWork().m_diDoorFan[(int)Work_Mom.eFanAlarmPlace.VS_PC_Fan]);
            SetFanAlarmLabel(lbVS전장DoorFAn, m_auto.ClassWork().m_diDoorFan[(int)Work_Mom.eFanAlarmPlace.VS_TOP_Door_Fan], m_auto.ClassWork().m_diDoorFan[(int)Work_Mom.eFanAlarmPlace.VS_BOT_Door_Fan]);
            SetFanAlarmLabel(lbVS조명FAN, m_auto.ClassWork().m_diDoorFan[(int)Work_Mom.eFanAlarmPlace.VS_Ilum1_Fan], m_auto.ClassWork().m_diDoorFan[(int)Work_Mom.eFanAlarmPlace.VS_Illum2_Fan]);
            SetFanAlarmLabel(lbWTRFan, m_auto.ClassWork().m_diDoorFan[(int)Work_Mom.eFanAlarmPlace.EFEM_WTR_Fan], m_auto.ClassWork().m_diDoorFan[(int)Work_Mom.eFanAlarmPlace.EFEM_WTR2_Fan]);
            SetFanAlarmLabel(lbAlignerDoorFan, m_auto.ClassWork().m_diDoorFan[(int)Work_Mom.eFanAlarmPlace.EFEM_Aligner_Door_Fan], m_auto.ClassWork().m_diDoorFan[(int)Work_Mom.eFanAlarmPlace.EFEM_Aligner_Door_Fan]);
        }

        private void SetFanAlarmLabel(Label label, int nInput)
        {
            if (m_auto.ClassControl().GetInputBit(nInput))
                label.BackColor = Color.LightCyan;
            else
                label.BackColor = Color.Crimson;
        }

        private void SetFanAlarmLabel(Label label, int nInput, int nInput2)
        {
            if (m_auto.ClassControl().GetInputBit(nInput) && m_auto.ClassControl().GetInputBit(nInput2))
                label.BackColor = Color.LightCyan;
            else
                label.BackColor = Color.Crimson;
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
            if (m_nLoadPort == 1)
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
            if (m_handler.ClassBackSide().m_bEnable)
            {
                HW_BackSide_Mom backside = m_handler.ClassBackSide();
                m_ioBacksideWaferExist.Set(backside.CheckWaferExist(false) == eHWResult.On);
                SetWaferInfo(m_waferInfoBackside, backside.InfoWafer);
            }

            if (m_handler.ClassImageVS() != null && m_handler.ClassImageVS().m_bUse)
            {
                m_ioVSConnected.Set(m_handler.ClassImageVS().IsConnected());
            }

            bool bDoorOut = m_auto.ClassWork().GetDoorLockOutState();
            bool bDoorIn = m_auto.ClassWork().GetDoorLockInState();

            if (bDoorIn && bDoorOut)
            {
                btnDoorLock.Text = "Door interlock On";
                btnDoorLock.BackColor = Color.LightGreen;
            }
            else if (bDoorOut && !bDoorIn)
            {
                btnDoorLock.Text = "Door Open 감지";
                btnDoorLock.BackColor = Color.Coral;
            }
            else
            {
                btnDoorLock.Text = "Door Interlock 해제";
                btnDoorLock.BackColor = Color.Red;
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
            DrawFanUI();
            for (int i = 0; i < m_nLoadPort; i++)
                UpdateLoadport(i);
        }
        /**
* @brief Start Btn 동작
* @param ojbect sendar : 전달할 정보
* @param EventArgs e : 이벤트
* @return 없음
* @note Patch-notes
* 날짜|작성자|설명|비고
* -|-|-|-
* 2023-07-24|조남수| Start 동작 시 로그 추가|-
* @warning 없음
*/
        private void btnStart_Click(object sender, EventArgs e)
        {
            m_log.WriteLog("Sequence", "[Button Click] Start"); //230724 nscho 
            if (m_handler.IsWaferExistInEQP())
            {
                bool IsDoorOpen = false;
                for (int i = 0; i < m_handler.m_nLoadPort; i++)
                {
                    if (m_handler.ClassLoadPort(i).IsDoorOpenPos())
                        IsDoorOpen = true;
                }
                if (IsDoorOpen == false)
                {
                    m_log.Popup("Loadport Load First For Recover Wafer");
                    return;
                }
            }

            m_work.m_bBuzzerOff = false;
            m_work.m_run.Run();
        }
        /**
* @brief Stop Btn 동작
* @param ojbect sendar : 전달할 정보
* @param EventArgs e : 이벤트
* @return 없음
* @note Patch-notes
* 날짜|작성자|설명|비고
* -|-|-|-
* 2023-07-24|조남수| Stop 동작 시 로그 추가|-
* @warning 없음
*/
        private void btnStop_Click(object sender, EventArgs e)
        {
            m_log.WriteLog("Sequence", "[Button Click] Stop"); //230724 nscho
            m_work.m_run.Stop();
            if (m_auto.m_bXgemUse && m_auto.ClassXGem().IsOnline())
                m_auto.ClassXGem().SetProcessState(XGem300_Mom.eProcessState.STOP);
        }

        private void btnCycleStop_Click(object sender, EventArgs e)
        {
            if (m_work.IsRun())
            {
                m_handler.ClassWTR().m_bCycleStop = true;
            }
        }
        /**
        * @brief Recovery Btn 동작
        * @param ojbect sendar : 전달할 정보
        * @param EventArgs e : 이벤트
        * @return 없음
        * @note Patch-notes
        * 날짜|작성자|설명|비고
        * -|-|-|-
        * 2023-07-24|조남수| Recovery 동작 시 로그 추가|-
        * @warning 없음
        */
        private void btnRecovery_Click(object sender, EventArgs e)
        {
            m_log.WriteLog("Sequence", "[Button Click] Recovery"); //230724 nscho 
            ((EFEM_Handler)m_handler).SetInfoWafer();
            int nPortNum = GetPortForRecovery();
            for (int i = 0; i < m_nLoadPort; i++)
            {
                m_handler.ClassLoadPort(i).m_infoCarrier.m_bRNR = false;
            }
            m_log.WriteLog("Sequence", "[RecoveryStart]"); //230724 nscho 
            EFEM_Recovery recover = new EFEM_Recovery();
            //m_log.HidePopup(true);
            recover.Init("Recover", m_auto, m_log, nPortNum);

            if (recover.GetLPNumForRecovery() != -1 && m_handler.m_bPossibleAutoRecover)
            {
                //if (m_handler.CheckUnloadOK() != true) return;
                m_handler.SetRecoverPortNum(recover.GetLPNumForRecovery());
                m_handler.m_procAutoUnload = Handler_Mom.eProcAutoUnload.GetLPInfo;
                m_work.m_run.SetManual(eWorkRun.AutoUnload);
                m_log.Add(string.Format("Auto Recovery : GetLPNumForRecovery = {0}, m_bPossibleAutoRecover = {1}", recover.GetLPNumForRecovery(), m_handler.m_bPossibleAutoRecover));
            }
            else
            {
                m_log.Add(string.Format("Manual Recovery : GetLPNumForRecovery = {0}, m_bPossibleAutoRecover = {1}", recover.GetLPNumForRecovery(), m_handler.m_bPossibleAutoRecover));
                if (nPortNum == -1 || !m_handler.ClassLoadPort(nPortNum).IsDoorOpenPos())
                {
                    m_handler.m_LostLPInfo = true;                            //190516 SDH ADD
                    m_log.WriteLog("Sequence", "[RecoveryDone] FAIL"); // 230724 nscho 
                    if (m_auto.m_strWork != Auto_Mom.eWork.MSB.ToString() && m_auto.m_strWork != Auto_Mom.eWork.Sanken.ToString()) m_log.Popup("설비가 Wafer 정보를 잃었습니다. Loadport(" + nPortNum + ") Load를 먼저 진행하여 주십시오");
                    else m_log.Popup("Wafer Infomation Was Losted On Equipment. Please Load Loadport(" + nPortNum + "), Befor Push Recovery Button");
                    return;
                }
                ezStopWatch sw = new ezStopWatch();
                while (m_handler.ClassLoadPort(0).NeedWTRMapping() || m_handler.ClassLoadPort(1).NeedWTRMapping())
                {
                    Thread.Sleep(100);
                    if (sw.Check() > 15000) break;
                }
                if (recover.ShowDialog() != DialogResult.OK) return;
                if (m_handler.CheckUnloadOK() != true) return;
                m_handler.m_procAutoUnload = Handler_Mom.eProcAutoUnload.VisionHome;
                m_work.m_run.SetManual(eWorkRun.AutoUnload);
            }
        }

        private int GetPortForRecovery()
        {
            HW_VisionWorks_Mom Vision = m_visionWorks;
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

            for (int n = 0; n < m_handler.m_nLoadPort; n++)
            {
                if (m_handler.ClassLoadPort(n).IsDoorOpenPos())
                    return n;
            }

            return -1;
        }
        /**
* @brief Home Btn 동작
* @param ojbect sendar : 전달할 정보
* @param EventArgs e : 이벤트
* @return 없음
* @note Patch-notes
* 날짜|작성자|설명|비고
* -|-|-|-
* 2023-07-24|조남수| Home 동작 시 로그 추가|-
* @warning 없음
*/
        private void btnHome_Click(object sender, EventArgs e)
        {
            m_log.WriteLog("Sequence", "[Button Click] Home"); //230724 nscho 
            m_work.m_run.Home();
        }
        /**
* @brief Reset Btn 동작
* @param ojbect sendar : 전달할 정보
* @param EventArgs e : 이벤트
* @return 없음
* @note Patch-notes
* 날짜|작성자|설명|비고
* -|-|-|-
* 2023-07-24|조남수| Reset 동작 시 로그 추가|-
* @warning 없음
*/
        private void btnReset_Click(object sender, EventArgs e)
        {
            m_log.WriteLog("Sequence", "[Button Click] Reset"); //230724 nscho 
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
            else
            {
                m_work.m_run.Reset();
            }
        }
        /**
* @brief Buzzer Off Btn 동작
* @param ojbect sendar : 전달할 정보
* @param EventArgs e : 이벤트
* @return 없음
* @note Patch-notes
* 날짜|작성자|설명|비고
* -|-|-|-
* 2023-07-24|조남수| BuzzerOff 동작 시 로그 추가|-
* @warning 없음
*/
        private void btnBuzzerOff_Click(object sender, EventArgs e)
        {
            m_log.WriteLog("Sequence", "[Button Click] BuzzerOff"); //230724 nscho 
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

        public void AddBacksideUI()
        {
            m_panelBackside = new Panel();
            m_labelBackside = new Label();
            m_waferInfoBackside = new UserControlWaferInfo();
            m_ioBacksideWaferExist = new UserControlIOIndicator();
            pnEQState.Controls.Add(m_panelBackside);
            // Panel
            m_panelBackside.Size = userControlOverViewLP1.Size;
            m_panelBackside.Location = new Point(userControlOverViewLP1.Location.X, panelLowerArm.Location.Y);
            m_panelBackside.BackColor = System.Drawing.Color.Khaki;
            m_panelBackside.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            m_panelBackside.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            m_panelBackside.Margin = new System.Windows.Forms.Padding(4);
            m_panelBackside.Controls.Add(m_labelBackside);
            m_panelBackside.Controls.Add(m_waferInfoBackside);
            m_panelBackside.Controls.Add(m_ioBacksideWaferExist);
            // Label
            m_labelBackside.AutoSize = true;
            m_labelBackside.Font = new System.Drawing.Font("Gulim", 16F, System.Drawing.FontStyle.Bold);
            m_labelBackside.Location = new System.Drawing.Point(1, 0);
            m_labelBackside.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            m_labelBackside.RightToLeft = System.Windows.Forms.RightToLeft.No;
            m_labelBackside.Size = new System.Drawing.Size(77, 22);
            m_labelBackside.Text = "Backside";
            m_labelBackside.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // WaferInfo
            m_waferInfoBackside.Location = new System.Drawing.Point(4, 37);
            m_waferInfoBackside.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            m_waferInfoBackside.Name = "WaferInfoAligner";
            m_waferInfoBackside.Size = new System.Drawing.Size(168, 69);
            // IO
            m_ioBacksideWaferExist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            m_ioBacksideWaferExist.IOName = "WaferExist";
            m_ioBacksideWaferExist.Location = new System.Drawing.Point(134, 0);
            m_ioBacksideWaferExist.Margin = new System.Windows.Forms.Padding(0);
            m_ioBacksideWaferExist.Size = new System.Drawing.Size(104, 34);
        }

        public void AddImageVSUI()
        {
            m_labelImageVS = new Label();
            m_ioVSConnected = new UserControlIOIndicator();
            this.panel4.Controls.Add(m_labelImageVS);
            this.panel4.Controls.Add(m_ioVSConnected);
            m_labelImageVS.AutoSize = true;
            m_labelImageVS.Font = new System.Drawing.Font("Gulim", 16F, System.Drawing.FontStyle.Bold);
            m_labelImageVS.Location = new System.Drawing.Point(1, 95);
            m_labelImageVS.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            m_labelImageVS.RightToLeft = System.Windows.Forms.RightToLeft.No;
            m_labelImageVS.Size = new System.Drawing.Size(77, 22);
            m_labelImageVS.Text = "ImageVS";
            m_labelImageVS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            m_ioVSConnected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            m_ioVSConnected.IOName = "Connected";
            m_ioVSConnected.Location = new System.Drawing.Point(144, 95);
            m_ioVSConnected.Margin = new System.Windows.Forms.Padding(0);
            m_ioVSConnected.Size = new System.Drawing.Size(104, 34);
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
        public void SetWTRMotion(eWTRSimnulMotion Motion)
        {

        }
        /**
* @brief Offline Btn 동작
* @param ojbect sendar : 전달할 정보
* @param EventArgs e : 이벤트
* @return 없음
* @note Patch-notes
* 날짜|작성자|설명|비고
* -|-|-|-
* 2023-07-24|조남수| Offline 동작 시 로그 추가|-
* @warning 없음
*/
        private void btnOffline_Click(object sender, EventArgs e)
        {
            m_log.WriteLog("Sequence", "[Button Click] Offline"); // 230724 nscho 
            m_auto.ClassXGem().XGemOffline();
            m_log.Add("Offline BTN Click");
        }
        /**
* @brief Local Btn 동작
* @param ojbect sendar : 전달할 정보
* @param EventArgs e : 이벤트
* @return 없음
* @note Patch-notes
* 날짜|작성자|설명|비고
* -|-|-|-
* 2023-07-24|조남수| Local 동작 시 로그 추가|-
* @warning 없음
*/
        private void btnLocal_Click(object sender, EventArgs e)
        {
            m_log.WriteLog("Sequence", "[Button Click] OnlineLocal"); //230724 nscho 
            m_auto.ClassXGem().XGemOnlineLocal();
            for (int i = 0; i < m_nLoadPort; i++)
            {
                if (!m_handler.ClassLoadPort(i).IsPlaced())
                {
                    m_auto.ClassXGem().SetLPPresentSensor(i, false);
                    m_auto.ClassXGem().SetLPCarrierOnOff(i, false);
                    m_auto.ClassXGem().SetCMSTransferState(XGem300Carrier.eLPTransfer.ReadyToLoad, i);
                    m_auto.ClassXGem().LPAccessModeChange(m_auto.ClassXGem().m_aXGem300Carrier[i].m_ePortAccess, m_auto.ClassXGem().m_aXGem300Carrier[i].m_sLocID);
                    m_auto.ClassXGem().SetLPInfo(i, XGem300Carrier.eLPTransfer.ReadyToLoad, m_auto.ClassXGem().m_aXGem300Carrier[i].m_ePortAccess, XGem300Carrier.ePortReservationState.NotReserved, XGem300Carrier.ePortAssocitionState.NotAssociated, "");
                }
                else
                {
                    m_auto.ClassXGem().SetLPPresentSensor(i, true);
                    m_auto.ClassXGem().SetLPCarrierOnOff(i, true);
                    m_auto.ClassXGem().LPAccessModeChange(m_auto.ClassXGem().m_aXGem300Carrier[i].m_ePortAccess, m_auto.ClassXGem().m_aXGem300Carrier[i].m_sLocID);
                    m_auto.ClassXGem().SetLPInfo(i, XGem300Carrier.eLPTransfer.TransferBlocked, m_auto.ClassXGem().m_aXGem300Carrier[i].m_ePortAccess, XGem300Carrier.ePortReservationState.NotReserved, XGem300Carrier.ePortAssocitionState.NotAssociated, "");
                }
            }
            m_log.Add("Local BTN Click");
        }
        /**
* @brief Remote Btn 동작
* @param ojbect sendar : 전달할 정보
* @param EventArgs e : 이벤트
* @return 없음
* @note Patch-notes
* 날짜|작성자|설명|비고
* -|-|-|-
* 2023-07-24|조남수| Remote 동작 시 로그 추가|-
* @warning 없음
*/
        private void btnRemote_Click(object sender, EventArgs e)
        {
            m_log.WriteLog("Sequence", "[Button Click] OnlineRemote");  //230724 nscho 
            m_auto.ClassXGem().XGemOnlineRemote();
            for (int i = 0; i < m_nLoadPort; i++)
            {
                if (!m_handler.ClassLoadPort(i).IsPlaced())
                {
                    m_auto.ClassXGem().SetLPPresentSensor(i, false);
                    m_auto.ClassXGem().SetLPCarrierOnOff(i, false);
                    m_auto.ClassXGem().SetCMSTransferState(XGem300Carrier.eLPTransfer.ReadyToLoad, i);
                    m_auto.ClassXGem().LPAccessModeChange(m_auto.ClassXGem().m_aXGem300Carrier[i].m_ePortAccess, m_auto.ClassXGem().m_aXGem300Carrier[i].m_sLocID);
                    m_auto.ClassXGem().SetLPInfo(i, XGem300Carrier.eLPTransfer.ReadyToLoad, m_auto.ClassXGem().m_aXGem300Carrier[i].m_ePortAccess, XGem300Carrier.ePortReservationState.NotReserved, XGem300Carrier.ePortAssocitionState.NotAssociated, "");
                }
                else
                {
                    m_auto.ClassXGem().SetLPPresentSensor(i, true);
                    m_auto.ClassXGem().SetLPCarrierOnOff(i, true);
                    m_auto.ClassXGem().LPAccessModeChange(m_auto.ClassXGem().m_aXGem300Carrier[i].m_ePortAccess, m_auto.ClassXGem().m_aXGem300Carrier[i].m_sLocID);
                    m_auto.ClassXGem().SetLPInfo(i, XGem300Carrier.eLPTransfer.TransferBlocked, m_auto.ClassXGem().m_aXGem300Carrier[i].m_ePortAccess, XGem300Carrier.ePortReservationState.NotReserved, XGem300Carrier.ePortAssocitionState.NotAssociated, "");
                }
            }
            m_log.Add("Online BTN Click");
        }

        public void AddRunData(Work_Mom.cRunData RunData)
        {

        }

        public void AddRunData2()
        {

        }

        public void AddStopData(Work_Mom.cStopData StopData)
        {

        }

        private void tableLayoutPanelLP_Paint(object sender, PaintEventArgs e)
        {

        }
        /**
* @brief DoorLock Btn 동작
* @param ojbect sendar : 전달할 정보
* @param EventArgs e : 이벤트
* @return 없음
* @note Patch-notes
* 날짜|작성자|설명|비고
* -|-|-|-
* 2023-07-24|조남수| Doorlock 동작 시 로그 추가|-
* @warning 없음
*/
        private void btnDoorLock_Click(object sender, EventArgs e)
        {
            m_log.WriteLog("Sequence", "[Button Click] DoorLock"); //230724 nscho 
            m_work.SetDoorLockOff();
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {
            if (listViewJobLog.Visible)
                listViewJobLog.Visible = false;
            else
                listViewJobLog.Visible = true;
        }
        public void runthread()
        {
            m_bRunThread = true; Thread.Sleep(5000);
            while (m_bRunThread)
            {
                while (CheckRecipeList() == true)
                {
                    UpdateRecipList();
                }
            }
        }
        DirectoryInfo[] filesVision;
        DirectoryInfo[] filesVisionRecipeName;
        //--> add by kiwon 211213
/**
* @brief EFEM 기준 Vision의 Recipe가 없는 경우 MainUI에서 표기 되는 기능
* @param 없음
* @return 없음 
* @note Patch-notes
* 날짜|작성자|설명|비고
* -|-|-|-
* 2023-01-11|조남수|EFEM 기준 Vision의 Recipe가 없는 경우 MainUI에서 표기 되는 기능|- 
* @warning 없음
*/
        public bool CheckRecipeList()
        {
            string strHandlerRecipePath = "D:\\Recipe";
            string strVisionRecipePath = m_handler.ClassVisionWorks().m_strRecipePath + "\\";
            DirectoryInfo drHandler = new DirectoryInfo(strHandlerRecipePath);
            Directory.CreateDirectory(strHandlerRecipePath);
            FileInfo[] filesHandler = drHandler.GetFiles("*.ezEFEM");

            DirectoryInfo drVision = new DirectoryInfo(strVisionRecipePath);
            Directory.CreateDirectory(strVisionRecipePath);


            m_lstNew.Clear();

            // Hander Recipe 목록과 Vision Recip 목록을 비교하여 상이한 목록은 별도로 저장.
            for (int i = 0; i < filesHandler.Length; i++)
            {
                if (filesHandler[i].Name != null)
                {
                    string strFileHead = filesHandler[i].Name.Split('.')[0];
                    string strFileFullName = "";
                    bool bSkip = false;
                    bool bFind = false;
                    string[] strFileName = filesHandler[i].Name.Split(new char[] { '.' });

                    for (int x = 0; x < strFileName.Length; x++)
                    {
                        if (strFileName[x] != "ezEFEM")
                        {
                            strFileFullName += strFileName[x];
                            if (x != strFileName.Length - 2)
                            {
                                strFileFullName += '.';
                            }
                        }
                    }
                    strVisionRecipePath += strFileHead;
                    try
                    {
                        filesVision = drVision.GetDirectories();
                    }
                    catch
                    {
                        bSkip = true;
                    }
                    string strVisionRName;
                    if (filesVision != null)
                    {
                        for (int j = 0; j < filesVision.Length; j++)
                        {
                            strVisionRName = filesVision[j].Name;
                            if (strFileHead.ToLower() == strVisionRName.ToLower())
                            {
                                DirectoryInfo drfileVisionName = new DirectoryInfo(drVision + "\\" + strVisionRName);
                                try
                                {
                                    filesVisionRecipeName = drfileVisionName.GetDirectories();
                                }
                                catch
                                {
                                    bSkip = true;
                                }
                                for (int y = 0; y < filesVisionRecipeName.Length; y++)
                                {
                                    if (strFileFullName.ToLower() == filesVisionRecipeName[y].Name.ToLower())
                                    {
                                        bFind = true;
                                        break;
                                    }
                                }

                            }
                        }
                    }
                }

            }
            //

            // 기존 리스트와 현재 신규로 생성된 리스트 갯수가 다를 경우 Item Clear
            // 상시로 Item을 Clear 하면 화면이 계속 깜빡거리기 때문에 목록이 상이할 경우에만 갱신 하기 위하여.
            if (listViewRecipe.Items.Count != m_lstNew.Count)
            {
                listViewRecipe.Items.Clear();
                return true;
            }

            // 기존 리스트와 신규 리스트 갯수가 같을 경우 하나씩 비교하여 Recipe name이 다른 항목이 있는지 확인.
            // 항목이 다른게 하나라도 있다면 Item Clear 후 재등록.
            for (int i = 0; i < m_lstNew.Count; i++)
            {
                bool bFind = false;
                for (int j = 0; j < listViewRecipe.Items.Count; j++)
                {
                    if (m_lstNew[i] == listViewRecipe.Items[j].Text)
                    {
                        bFind = true;
                        break;
                    }
                }

                if (!bFind)
                {
                    listViewRecipe.Items.Clear();
                    return true;
                }
            }
            //
            return false;
        }
        public void ThreadStop()
        {
            m_thread.Join();
            m_bRunThread = false;
        }  
        public void UpdateRecipList()
        {
            for (int i = 0; i < m_lstNew.Count; i++)
            {
                listViewRecipe.Items.Add(m_lstNew[i]);
            }
        }
        //<--
    }
}
