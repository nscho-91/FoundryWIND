using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using ezAutoMom;
using WeifenLuo.WinFormsUI.Docking;
using ezTools;


namespace ezAuto_EFEM
{

    public partial class UI_Gem300 : DockContent
    {
        Gem_SSEM m_Gem300 = null;
        delegate void SetListItemViewItem(string Text);
        delegate void SetListItemViewItems(string[] Texts);
        delegate void Delegate2Strings(string sPPID, string[] sName, string[] sValue);
        delegate void Delegate_2IntStrings(int a, int b, string s);
        delegate void Delegate_Int(int a);
        delegate void Delegate_Item(ListViewItem item);


        public enum eLvCMSItem
        {
            Port_Type = 0,
            Carrier_Type,
            Product_Type,
            AccessMode,
            Transfer_State,
            AVailable_State,
            CarrierIDState,
            SlotMapState,
            CarrierID,
            LotID,
        }

        public enum eLvPRJobItem
        {
            PRJOB_ID,
            RecipeID,
            PreRunFlag,
            SlotMap,
            InspSlotMap,
            PRJobState,
            WorkLP,
            WorkSlot,
            PanelID,
            CTRLJOB_ID,
            CTRLJOBState,
        }

        public enum eLvCTRLJobState
        {
            CTRLJOBState,
            CTRLJOB_ID,
            PRJobID,
        }
        public enum ePPIDChangeMode
        {
            Create = 1,
            Delete,
            Modify,
        }

        Auto_Mom m_Auto = null;
        public UI_Gem300(Auto_Mom auto, Gem_SSEM Gem300)
        {
            InitializeComponent();
            ProcessKill("XGem");

            m_Auto = auto;
            m_Gem300 = Gem300;
            m_Gem300.OnRecieveTerminalMsg += OnTerminalMsgRcv;
            m_Gem300.OnRecieveTerminalMultiMsg += OnTerminalMultiMsgRcv;
//            m_Gem300.OnRecieveLogEvent += OnRecieveLogEvent;
//            m_Gem300.OnRecipeDeleteEvent += OnRecipeDeleteEvent;
            m_Gem300.OnRecipeSelected += OnRecipeSelected;
//            m_Gem300.OnRecipeChanged += OnRecipeChanged;
            m_Gem300.OnChangeList += OnChangeList;
            m_Gem300.OnMakeList += OnMakeList;
            m_Gem300.OnDeleteItem += OnDeleteItem;

            pnCommState.Text = "Unkown";
            pnCommState.BackColor = Color.Firebrick;
            pnContState.Text = "Unkown";
            pnContState.BackColor = Color.Firebrick;
            InitLoadPortStateListView();
            InitVIDListView();
            InitCEIDListView();
            SetVIDListView();
            SetCEIDListView();
            InitTerminalMsgListView();
            InitPRJobListView();
            InitCTRLJobListView();
            InitRecipeListView();
            m_Gem300.Initialize();
            m_Gem300.GemStart();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_Gem300.GemStop();
        }

        private void InitLoadPortStateListView()
        {
            lvLPState.View = System.Windows.Forms.View.Details;
            lvLPState.GridLines = true;


            lvLPState.Columns.Add("State", 120, HorizontalAlignment.Center);
            lvLPState.Columns.Add("Loadport 1", 172, HorizontalAlignment.Center);
            lvLPState.Columns.Add("Loadport 2", 172, HorizontalAlignment.Center);

            string[] ItemList = new String[3];
            for (int i = 0; i < Enum.GetNames(typeof(eLvCMSItem)).Length; i++) {
                ItemList[0] = Enum.GetName((typeof(eLvCMSItem)), i);
                ItemList[1] = "";
                ItemList[2] = "";
                ListViewItem Item = new ListViewItem(ItemList);
                lvLPState.Items.Add(Item);

            }
            m_Gem300.LoadRecipe();
        }

        public void ThreadStop()
        {
            m_Gem300.ThreadStop();
            m_Gem300.GemStop();
        }
        private void InitVIDListView()
        {
            lvVIDList.View = System.Windows.Forms.View.Details;
            lvVIDList.Width = 231;
            lvVIDList.GridLines = true;


            lvVIDList.Columns.Add("Name", 171, HorizontalAlignment.Center);
            lvVIDList.Columns.Add("VID", 60, HorizontalAlignment.Center);

            string[] ItemList = new String[2];
            for (int i = 0; i < Enum.GetNames(typeof(Gem_SSEM.eVID_NO)).Length; i++) {
                ItemList[0] = "";
                ItemList[1] = "";
                ListViewItem Item = new ListViewItem(ItemList);
                lvVIDList.Items.Add(Item);
            }
        }

        private void InitTerminalMsgListView()
        {
            lvTerminalMSG.View = System.Windows.Forms.View.Details;
            lvVIDList.GridLines = true;

            lvTerminalMSG.Columns.Add("Time", 100, HorizontalAlignment.Left);
            lvTerminalMSG.Columns.Add("MSG", 370, HorizontalAlignment.Left);
        }

        private void InitPRJobListView()
        {
            lvPRJob.View = System.Windows.Forms.View.Details;
            lvPRJob.GridLines = true;


            lvPRJob.Columns.Add("List", 100, HorizontalAlignment.Center);
            lvPRJob.Columns.Add("Value", 131, HorizontalAlignment.Center);

            string[] ItemList = new String[2];
            for (int i = 0; i < Enum.GetNames(typeof(eLvPRJobItem)).Length; i++) {
                ItemList[0] = Enum.GetName((typeof(eLvPRJobItem)), i);
                ItemList[1] = "";
                ListViewItem Item = new ListViewItem(ItemList);
                lvPRJob.Items.Add(Item);

            }
        }

        private void InitCTRLJobListView()
        {
            lvCTRLJob.View = System.Windows.Forms.View.Details;
            lvCTRLJob.GridLines = true;


            lvCTRLJob.Columns.Add("List", 100, HorizontalAlignment.Center);
            lvCTRLJob.Columns.Add("Value", 131, HorizontalAlignment.Center);

            string[] ItemList = new String[2];
            for (int i = 0; i < Enum.GetNames(typeof(eLvCTRLJobState)).Length; i++) {
                ItemList[0] = Enum.GetName((typeof(eLvCTRLJobState)), i);
                ItemList[1] = "";
                ListViewItem Item = new ListViewItem(ItemList);
                lvCTRLJob.Items.Add(Item);

            }
        }

        private void OnRecipeDeleteEvent(string[] sPPID)
        {
            if (this.InvokeRequired) {
                SetListItemViewItems d = new SetListItemViewItems(OnRecipeDeleteEvent);
                this.Invoke(d, new object[] { sPPID });
            }
            else {

                for (int i = 0; i < sPPID.Length; i++) {
                    for (int j = 0; j < lvRecipe.Items.Count; j++) {
                        if (lvRecipe.Items[j].SubItems[0].Text == sPPID[i]) {
                            lvRecipe.Items[j].Remove();
                            lvRecipe.Update();
                        }
                    }
                }
                SaveRecipe();
                m_Gem300.LoadRecipe();
            }
        }
        private void OnRecipeChanged(string sPPID, string[] sName, string[] sValue)
        {
            if (this.InvokeRequired) {
                Delegate2Strings d = new Delegate2Strings(OnRecipeChanged);
                this.Invoke(d, new object[] { sPPID, sName, sValue });
            }
            else {
                ePPIDChangeMode eMode = ePPIDChangeMode.Create;
                int index = 0;
                for (int i = 0; i < lvRecipe.Items.Count; i++) {
                    if (lvRecipe.Items[i].SubItems[0].Text == sPPID) {
                        eMode = ePPIDChangeMode.Modify;
                        index = i;
                    }
                }
                string[] sParam = new string[3] { "0", "0", "0" };
                for (int i = 0; i < sName.Length; i++) {
                    switch (sName[i]) {
                        case "OCRUSE":
                            if (eMode == ePPIDChangeMode.Modify)
                                lvRecipe.Items[index].SubItems[1].Text = sValue[i];
                            else
                                sParam[0] = sValue[i];
                            break;
                        case "BCRUSE":
                            if (eMode == ePPIDChangeMode.Modify)
                                lvRecipe.Items[index].SubItems[2].Text = sValue[i];
                            else
                                sParam[1] = sValue[i];
                            break;
                        case "NotchPos":
                            if (eMode == ePPIDChangeMode.Modify)
                                lvRecipe.Items[index].SubItems[3].Text = sValue[i];
                            else
                                sParam[2] = sValue[i];
                            break;
                    }
                }
                if (eMode == ePPIDChangeMode.Create) {
                    string[] ItemList = new string[4];
                    ItemList[0] = sPPID;
                    ItemList[1] = sParam[0];
                    ItemList[2] = sParam[1];
                    ItemList[3] = sParam[2];
                    ListViewItem item = new ListViewItem(ItemList);
                    lvRecipe.Items.Add(item);
                }
                SaveRecipe();
                m_Gem300.LoadRecipe();
            }
        }

        private void OnRecipeSelected(string sRecipe)
        {
            if (this.InvokeRequired) {
                SetListItemViewItem d = new SetListItemViewItem(OnRecipeSelected);
                this.Invoke(d, new object[] { sRecipe });
            }
            else {
                for (int i = 0; i < lvRecipe.Items.Count; i++) {
                    if (lvRecipe.Items[i].SubItems[0].Text == sRecipe) {
                        lvRecipe.Items[i].Selected = true;
                    }
                }
            }
        }


        private void OnDeleteItem(int a)
        {
            if (this.InvokeRequired) {
                Delegate_Int d = new Delegate_Int(OnDeleteItem);
                this.Invoke(d, new object[] { a });
            }
            else {
                lvRecipe.Items[a].Remove();
                lvRecipe.Update();
                SaveRecipe();
                m_Gem300.LoadRecipe();
            }
        }

        private void InitRecipeListView()
        {
            lvRecipe.Clear();
            lvRecipe.View = System.Windows.Forms.View.Details;
            lvRecipe.GridLines = true;
            lvRecipe.HideSelection = false;
            lvRecipe.FullRowSelect = true;
            lvRecipe.Columns.Add("Name", 154, HorizontalAlignment.Center);
            lvRecipe.Columns.Add("RedLight", 54, HorizontalAlignment.Center);
            lvRecipe.Columns.Add("BlueLight", 54, HorizontalAlignment.Center);
            lvRecipe.Columns.Add("GreenLight", 54, HorizontalAlignment.Center);
            lvRecipe.Columns.Add("CoaxLight", 54, HorizontalAlignment.Center);
            lvRecipe.Columns.Add("Notch SideLight", 54, HorizontalAlignment.Center);
            lvRecipe.Columns.Add("AlignStep", 54, HorizontalAlignment.Center);
            lvRecipe.Columns.Add("AlignRange", 54, HorizontalAlignment.Center);
            lvRecipe.Columns.Add("OverlaySearchStart", 54, HorizontalAlignment.Center);
            lvRecipe.Columns.Add("OverlaySearchLenght", 54, HorizontalAlignment.Center);
            lvRecipe.Columns.Add("OverlayThreshold", 54, HorizontalAlignment.Center);
            string[] ItemList = new string[11];

            string sFile = m_Gem300.m_sRecipePath;
            FileInfo fi = new FileInfo(sFile);
            string sSection = String.Format("Recipe Num");
            int nNum = IniFile.G_IniReadIntValue(sSection, "Num", sFile);

            for (int i = 0; i < nNum; i++) {
                sSection = String.Format("Recipe" + i);
                ItemList[0] = IniFile.G_IniReadValue(sSection, "Name", sFile);
                ItemList[1] = IniFile.G_IniReadIntValue(sSection, "RedLight", sFile).ToString();
                ItemList[2] = IniFile.G_IniReadIntValue(sSection, "BlueLight", sFile).ToString();
                ItemList[3] = IniFile.G_IniReadIntValue(sSection, "GreenLight", sFile).ToString();
                ItemList[4] = IniFile.G_IniReadIntValue(sSection, "CoaxLight", sFile).ToString();
                ItemList[5] = IniFile.G_IniReadIntValue(sSection, "SideLight", sFile).ToString();
                ItemList[6] = IniFile.G_IniReadIntValue(sSection, "AlignStep", sFile).ToString();
                ItemList[7] = IniFile.G_IniReadIntValue(sSection, "AlignRange", sFile).ToString();
                ItemList[8] = IniFile.G_IniReadIntValue(sSection, "OverlaySearchStart", sFile).ToString();
                ItemList[9] = IniFile.G_IniReadIntValue(sSection, "OverlaySearchLenght", sFile).ToString();
                ItemList[10] = IniFile.G_IniReadIntValue(sSection, "OverlayThreshold", sFile).ToString();

                ListViewItem item = new ListViewItem(ItemList);
                lvRecipe.Items.Add(item);
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            string[] ItemList = new string[11];
            ItemList[0] = tbRecipeName.Text;
            ItemList[1] = textBox2.Text;
            ItemList[2] = textBox3.Text;
            ItemList[3] = textBox4.Text;
            ItemList[4] = textBox5.Text;
            ItemList[5] = textBox6.Text;
            ItemList[6] = textBox7.Text;
            ItemList[7] = textBox8.Text;
            ItemList[8] = textBox9.Text;
            ItemList[9] = textBox10.Text;
            ItemList[10] = textBox11.Text;
            ListViewItem item = new ListViewItem(ItemList);
            lvRecipe.Items.Add(item);
            SaveRecipe();

            m_Gem300.LoadRecipe();
            m_Gem300.PPIDChanged(Gem_SSEM.ePPIDChangeMode.Create, ItemList[0], ItemList[1], ItemList[2], ItemList[3], ItemList[4], ItemList[5], ItemList[6], ItemList[7], ItemList[8], ItemList[9], ItemList[10]);
        }

        private void SaveRecipe()
        {
            string sFile = m_Gem300.m_sRecipePath;
            FileInfo fi = new FileInfo(sFile);
            string sSection = String.Format("Recipe Num");
            IniFile.G_IniWriteIntValue(sSection, "Num", lvRecipe.Items.Count, sFile);
            for (int i = 0; i < lvRecipe.Items.Count; i++) {
                sSection = String.Format("Recipe" + i);
                IniFile.G_IniWriteValue(sSection, "Name", lvRecipe.Items[i].SubItems[0].Text, sFile);
                IniFile.G_IniWriteIntValue(sSection, "RedLight", Convert.ToInt32(lvRecipe.Items[i].SubItems[1].Text), sFile);
                IniFile.G_IniWriteIntValue(sSection, "BlueLight", Convert.ToInt32(lvRecipe.Items[i].SubItems[2].Text), sFile);
                IniFile.G_IniWriteIntValue(sSection, "GreenLight", Convert.ToInt32(lvRecipe.Items[i].SubItems[3].Text), sFile);
                IniFile.G_IniWriteIntValue(sSection, "CoaxLight", Convert.ToInt32(lvRecipe.Items[i].SubItems[4].Text), sFile);
                IniFile.G_IniWriteIntValue(sSection, "SideLight", Convert.ToInt32(lvRecipe.Items[i].SubItems[5].Text), sFile);
                IniFile.G_IniWriteIntValue(sSection, "AlignStep", Convert.ToInt32(lvRecipe.Items[i].SubItems[6].Text), sFile);
                IniFile.G_IniWriteIntValue(sSection, "AlignRange", Convert.ToInt32(lvRecipe.Items[i].SubItems[7].Text), sFile);
                IniFile.G_IniWriteIntValue(sSection, "OverlaySearchStart", Convert.ToInt32(lvRecipe.Items[i].SubItems[8].Text), sFile);
                IniFile.G_IniWriteIntValue(sSection, "OverlaySearchLenght", Convert.ToInt32(lvRecipe.Items[i].SubItems[9].Text), sFile);
                IniFile.G_IniWriteIntValue(sSection, "OverlayThreshold", Convert.ToInt32(lvRecipe.Items[i].SubItems[10].Text), sFile);
            }
            //string sFile = m_Gem300.m_sRecipePath;
            //FileInfo fi = new FileInfo(sFile);
            //string sSection = String.Format("Recipe Num");
            //IniFile.G_IniWriteIntValue(sSection, "Num", lvRecipe.Items.Count, sFile);
            //for (int i = 0; i < lvRecipe.Items.Count; i++) {
            //    sSection = String.Format("Recipe" + i);
            //    IniFile.G_IniWriteValue(sSection, "Name", lvRecipe.Items[i].SubItems[0].Text, sFile);
            //    IniFile.G_IniWriteIntValue(sSection, "USEOCR", Convert.ToInt32(lvRecipe.Items[i].SubItems[1].Text), sFile);
            //    IniFile.G_IniWriteIntValue(sSection, "USEBCR", Convert.ToInt32(lvRecipe.Items[i].SubItems[2].Text), sFile);
            //    IniFile.G_IniWriteIntValue(sSection, "NotchPos", Convert.ToInt32(lvRecipe.Items[i].SubItems[3].Text), sFile);
            //}
        }

        private void button23_Click(object sender, EventArgs e)
        {
            if (lvRecipe.SelectedItems.Count > 0) {
                string sPPID = lvRecipe.SelectedItems[0].SubItems[0].Text;
                string RedLight = lvRecipe.SelectedItems[0].SubItems[1].Text;
                string BlueLight = lvRecipe.SelectedItems[0].SubItems[2].Text;
                string GreenLight = lvRecipe.SelectedItems[0].SubItems[3].Text;
                string CoaxLight = lvRecipe.SelectedItems[0].SubItems[4].Text;
                string SideLight = lvRecipe.SelectedItems[0].SubItems[5].Text;
                string AlignStep = lvRecipe.SelectedItems[0].SubItems[6].Text;
                string AlignRange = lvRecipe.SelectedItems[0].SubItems[7].Text;
                string OverlaySearchStart = lvRecipe.SelectedItems[0].SubItems[8].Text;
                string OverlaySearchLenght = lvRecipe.SelectedItems[0].SubItems[9].Text;
                string OverlayThreshold = lvRecipe.SelectedItems[0].SubItems[10].Text;


                lvRecipe.SelectedItems[0].Remove();
                lvRecipe.Update();
                SaveRecipe();
                InitRecipeListView();
                m_Gem300.LoadRecipe();
                m_Gem300.PPIDChanged(Gem_SSEM.ePPIDChangeMode.Delete, sPPID, RedLight, BlueLight, GreenLight, CoaxLight, SideLight, AlignStep, AlignRange, OverlaySearchStart, OverlaySearchLenght, OverlayThreshold);
            }
        }

        private void InitCEIDListView()
        {
            lvCEIDList.View = System.Windows.Forms.View.Details;
            lvCEIDList.Width = 231;
            lvCEIDList.GridLines = true;


            lvCEIDList.Columns.Add("Name", 171, HorizontalAlignment.Center);
            lvCEIDList.Columns.Add("CEID", 60, HorizontalAlignment.Center);

            string[] ItemList = new String[2];
            for (int i = 0; i < Enum.GetNames(typeof(Gem_SSEM.eCEID_NO)).Length; i++) {
                ItemList[0] = "";
                ItemList[1] = "";
                ListViewItem Item = new ListViewItem(ItemList);
                lvCEIDList.Items.Add(Item);
            }
        }

        private void SetVIDListView()
        {
            string[] sVIDNameList = new string[Enum.GetNames(typeof(Gem_SSEM.eVID_NO)).Length];
            int[] nVIDValueList = new int[Enum.GetNames(typeof(Gem_SSEM.eVID_NO)).Length];
            m_Gem300.GetVIDList(ref sVIDNameList, ref nVIDValueList);

            for (int i = 0; i < Enum.GetNames(typeof(Gem_SSEM.eVID_NO)).Length; i++) {
                lvVIDList.Items[i].SubItems[0].Text = sVIDNameList[i];
                lvVIDList.Items[i].SubItems[1].Text = nVIDValueList[i].ToString();
            }
        }

        private void SetCEIDListView()
        {
            string[] sCEIDNameList = new string[Enum.GetNames(typeof(Gem_SSEM.eCEID_NO)).Length];
            int[] nCEIDValueList = new int[Enum.GetNames(typeof(Gem_SSEM.eCEID_NO)).Length];
            m_Gem300.GetCEIDList(ref sCEIDNameList, ref nCEIDValueList);

            for (int i = 0; i < Enum.GetNames(typeof(Gem_SSEM.eCEID_NO)).Length; i++) {
                lvCEIDList.Items[i].SubItems[0].Text = sCEIDNameList[i];
                lvCEIDList.Items[i].SubItems[1].Text = nCEIDValueList[i].ToString();
            }
        }

        public void ProcessKill(string Name)
        {
            Process[] ProcessList = Process.GetProcessesByName(Name);
            if (ProcessList.Length > 0) {
                ProcessList[0].Kill();
            }
        }

        private void btSetConfig_Click(object sender, EventArgs e)
        {
            m_Gem300.SetParameters();
        }

        private void btInit_Click(object sender, EventArgs e)
        {
            m_Gem300.Initialize();
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            m_Gem300.GemStart();
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            m_Gem300.GemStop();
        }

        private void Timer_ChangeInfo_Tick(object sender, EventArgs e)
        {
            #region GemSetting
            //string[] ip = new string[4];
            //if (m_Gem300.m_GemSetting.IP != null) {
            //    ip = m_Gem300.m_GemSetting.IP.Split('.');
            //    tbIP1.Text = ip[0];
            //    tbIP2.Text = ip[1];
            //    tbIP3.Text = ip[2];
            //    tbIP4.Text = ip[3];
            //}
            //tbPort.Text = m_Gem300.m_GemSetting.Port;
            //if (m_Gem300.m_GemSetting.Mode == "Active")
            //    cbMode.SelectedIndex = 1;
            //else
            //    cbMode.SelectedIndex = 0;
            #endregion
            #region GemState
            switch (m_Gem300.GetCommunicationState()) {
                case Gem_SSEM.eCommunicateState.COMM_DISABLE:
                    pnCommState.Text = "Diasble";
                    pnCommState.BackColor = Color.Firebrick;
                    break;
                case Gem_SSEM.eCommunicateState.COMM_WAITCR:
                    pnCommState.Text = "Wait CR";
                    pnCommState.BackColor = Color.PapayaWhip;
                    break;
                case Gem_SSEM.eCommunicateState.COMM_WAITDELAY:
                    pnCommState.Text = "Wait Delay";
                    pnCommState.BackColor = Color.PapayaWhip;
                    break;
                case Gem_SSEM.eCommunicateState.COMM_WAITCRA:
                    pnCommState.Text = "Wait CRA";
                    pnCommState.BackColor = Color.PapayaWhip;
                    break;
                case Gem_SSEM.eCommunicateState.COMM_COMMUNICATING:
                    pnCommState.Text = "Communicating";
                    pnCommState.BackColor = Color.PaleGreen;
                    break;
                default:
                    pnCommState.Text = "Unkown";
                    pnCommState.BackColor = Color.DimGray;
                    break;

            }
            switch (m_Gem300.GetContolState()) {
                case Gem_SSEM.eControlState.CONT_OFFLINE:
                    pnContState.Text = "OFFLine";
                    pnContState.BackColor = Color.Firebrick;
                    break;
                case Gem_SSEM.eControlState.CONT_ATTEMPTONLINE:
                    pnContState.Text = "WaitOnline";
                    pnContState.BackColor = Color.OrangeRed;
                    break;
                case Gem_SSEM.eControlState.CONT_HOSTOFFLINE:
                    pnContState.Text = "Host Offline";
                    pnContState.BackColor = Color.Firebrick;
                    break;
                case Gem_SSEM.eControlState.CONT_LOCAL:
                    pnContState.Text = "Online Local";
                    pnContState.BackColor = Color.PapayaWhip;
                    break;
                case Gem_SSEM.eControlState.CONT_ONLINEREMOTE:
                    pnContState.Text = "Online Remote";
                    pnContState.BackColor = Color.PaleGreen;
                    break;
                default:
                    pnContState.Text = "Unkown";
                    pnContState.BackColor = Color.DimGray;
                    break;
            }
            switch (m_Gem300.GetEQPState()) {
                case Gem_SSEM.eEQPState_SSEM.EQ_DOWN:
                    lbEQPSTate.Text = "DOWN";
                    lbEQPSTate.BackColor = Color.Firebrick;
                    break;
                case Gem_SSEM.eEQPState_SSEM.EQ_IDLE:
                    lbEQPSTate.Text = "IDLE";
                    lbEQPSTate.BackColor = Color.Gold;
                    break;
                case Gem_SSEM.eEQPState_SSEM.EQ_RUN:
                    lbEQPSTate.Text = "RUN";
                    lbEQPSTate.BackColor = Color.PaleGreen;
                    break;
                case Gem_SSEM.eEQPState_SSEM.EQ_PM:
                    lbEQPSTate.Text = "PM";
                    lbEQPSTate.BackColor = Color.RoyalBlue;
                    break;
            }
            #endregion
            #region LPSTATE

            lvLPState.Items[0].SubItems[1].Text = Enum.GetName(typeof(Gem_SSEM.cCMS.ePortType), (int)(m_Gem300.GetCMS(1).m_ePortType));
            lvLPState.Items[0].SubItems[2].Text = Enum.GetName(typeof(Gem_SSEM.cCMS.ePortType), (int)(m_Gem300.GetCMS(2).m_ePortType));
            lvLPState.Items[1].SubItems[1].Text = Enum.GetName(typeof(Gem_SSEM.cCMS.eCarrierType), (int)(m_Gem300.GetCMS(1).m_eCarrierType));
            lvLPState.Items[1].SubItems[2].Text = Enum.GetName(typeof(Gem_SSEM.cCMS.eCarrierType), (int)(m_Gem300.GetCMS(2).m_eCarrierType));
            lvLPState.Items[2].SubItems[1].Text = Enum.GetName(typeof(Gem_SSEM.cCMS.eProductType), (int)(m_Gem300.GetCMS(1).m_eProducType));
            lvLPState.Items[2].SubItems[2].Text = Enum.GetName(typeof(Gem_SSEM.cCMS.eProductType), (int)(m_Gem300.GetCMS(2).m_eProducType));
            lvLPState.Items[3].SubItems[1].Text = Enum.GetName(typeof(Gem_SSEM.cCMS.ePortAccessMode), (int)(m_Gem300.GetCMS(1).m_eAccessMode));
            lvLPState.Items[3].SubItems[2].Text = Enum.GetName(typeof(Gem_SSEM.cCMS.ePortAccessMode), (int)(m_Gem300.GetCMS(2).m_eAccessMode));
            lvLPState.Items[4].SubItems[1].Text = Enum.GetName(typeof(Gem_SSEM.cCMS.eLPTransferState), (int)(m_Gem300.GetCMS(1).m_eLPTransferState));
            lvLPState.Items[4].SubItems[2].Text = Enum.GetName(typeof(Gem_SSEM.cCMS.eLPTransferState), (int)(m_Gem300.GetCMS(2).m_eLPTransferState));
            lvLPState.Items[5].SubItems[1].Text = Enum.GetName(typeof(Gem_SSEM.cCMS.eLPAvailalbeState), (int)(m_Gem300.GetCMS(1).GetLPAvailableState()));
            lvLPState.Items[5].SubItems[2].Text = Enum.GetName(typeof(Gem_SSEM.cCMS.eLPAvailalbeState), (int)(m_Gem300.GetCMS(2).GetLPAvailableState()));
            lvLPState.Items[6].SubItems[1].Text = Enum.GetName(typeof(Gem_SSEM.cCMS.eCarrierIDState), (int)(m_Gem300.GetCMS(1).m_eCarrierIDState));
            lvLPState.Items[6].SubItems[2].Text = Enum.GetName(typeof(Gem_SSEM.cCMS.eCarrierIDState), (int)(m_Gem300.GetCMS(2).m_eCarrierIDState));
            lvLPState.Items[7].SubItems[1].Text = Enum.GetName(typeof(Gem_SSEM.cCMS.eCarrierSlotMapState), (int)(m_Gem300.GetCMS(1).m_eSlotMapState));
            lvLPState.Items[7].SubItems[2].Text = Enum.GetName(typeof(Gem_SSEM.cCMS.eCarrierSlotMapState), (int)(m_Gem300.GetCMS(2).m_eSlotMapState));
            lvLPState.Items[8].SubItems[1].Text = m_Gem300.GetCMS(1).CarrierID;
            lvLPState.Items[8].SubItems[2].Text = m_Gem300.GetCMS(2).CarrierID;
            lvLPState.Items[9].SubItems[1].Text = m_Gem300.GetCMS(1).LotID;
            lvLPState.Items[9].SubItems[2].Text = m_Gem300.GetCMS(2).LotID;


            lvPRJob.Items[0].SubItems[1].Text = m_Gem300.GetPJ().m_sPRJobID;
            lvPRJob.Items[1].SubItems[1].Text = m_Gem300.GetPJ().m_sRecipeID;
            lvPRJob.Items[2].SubItems[1].Text = m_Gem300.GetPJ().m_ePreRunFlag;
            lvPRJob.Items[3].SubItems[1].Text = m_Gem300.GetPJ().m_SlotMap;
            lvPRJob.Items[4].SubItems[1].Text = m_Gem300.GetPJ().m_SlotMapInsp;
            lvPRJob.Items[5].SubItems[1].Text = Enum.GetName(typeof(Gem_SSEM.cPJ.ePRJObState), (int)(m_Gem300.GetPJ().PRJobState));
            lvPRJob.Items[6].SubItems[1].Text = m_Gem300.m_nWorkLP.ToString();
            lvPRJob.Items[7].SubItems[1].Text = m_Gem300.m_nWorkSlot.ToString();
            lvPRJob.Items[8].SubItems[1].Text = m_Gem300.GetPJ().m_sPanelID[m_Gem300.m_nWorkSlot];



            lvCTRLJob.Items[0].SubItems[1].Text = Enum.GetName(typeof(Gem_SSEM.cCJ.eCTRLJobState), (int)(m_Gem300.GetCJ().CTRLJobState));
            lvCTRLJob.Items[1].SubItems[1].Text = m_Gem300.GetCJ().sCTRLJobID;
            lvCTRLJob.Items[2].SubItems[1].Text = m_Gem300.GetCJ().sPRJOBID;

            lbMainCycle.Text = Enum.GetName(typeof(Gem_SSEM.eMainCycle), m_Gem300.GetMainCycle());
            #endregion
        }

        private void btnOffline_Click(object sender, EventArgs e)
        {
            m_Gem300.SetOffline();
        }

        private void btnLocal_Click(object sender, EventArgs e)
        {
            m_Gem300.SetOnlineLocal();
        }

        private void btnRemote_Click(object sender, EventArgs e)
        {
            m_Gem300.SetOnlineRemote();
        }

        private void OnTerminalMsgRcv(string sMsg)
        {
            if (this.InvokeRequired) {
                SetListItemViewItem d = new SetListItemViewItem(OnTerminalMsgRcv);
                this.Invoke(d, new object[] { sMsg });
            }
            else {
                string[] ItemList = new String[2];
                string Time = DateTime.Now.ToString();

                ItemList[0] = Time;
                ItemList[1] = sMsg;
                ListViewItem Item = new ListViewItem(ItemList);
                lvTerminalMSG.Items.Add(Item);
            }
        }

        private void OnRecieveLogEvent(string sMsg)
        {
            if (this.InvokeRequired) {
                SetListItemViewItem d = new SetListItemViewItem(OnRecieveLogEvent);
                this.Invoke(d, new object[] { sMsg });
            }
            else {
                lbLogView.Items.Add(sMsg);
            }
        }

        private void OnChangeList(int a, int b, string s)
        {
            if (this.InvokeRequired) {
                Delegate_2IntStrings d = new Delegate_2IntStrings(OnChangeList);
                this.Invoke(d, new object[] { a, b, s });
            }
            else {
                lvRecipe.Items[a].SubItems[b].Text = s;
                SaveRecipe();
            }
        }

        private void OnMakeList(ListViewItem item)
        {
            if (this.InvokeRequired) {
                Delegate_Item d = new Delegate_Item(OnMakeList);
                this.Invoke(d, new object[] { item });
            }
            else {
                lvRecipe.Items.Add(item);
                SaveRecipe();
            }
        }

        private void OnTerminalMultiMsgRcv(string[] sMsgs)
        {
            if (this.InvokeRequired) {
                SetListItemViewItems d = new SetListItemViewItems(OnTerminalMultiMsgRcv);
                this.Invoke(d, new object[] { sMsgs });
            }
            else {
                string[] ItemList = new String[2];
                string Time = DateTime.Now.ToString();

                for (int i = 0; i < sMsgs.Length; i++) {
                    ItemList[0] = Time;
                    ItemList[1] = sMsgs[i];
                    ListViewItem Item = new ListViewItem(ItemList);
                    lvTerminalMSG.Items.Add(Item);
                }
            }
        }
        private void btnLoadRequestLP1_Click(object sender, EventArgs e)
        {
            m_Gem300.LoadRequest(1);

        }

        private void btnLoadRequestLP2_Click(object sender, EventArgs e)
        {
            m_Gem300.LoadRequest(2);

        }

        private void btnEQPDown_Click(object sender, EventArgs e)
        {
            m_Gem300.SetEQPState(Gem_SSEM.eEQPState_SSEM.EQ_DOWN);
        }

        private void btnEQPIDLE_Click(object sender, EventArgs e)
        {
            m_Gem300.SetEQPState(Gem_SSEM.eEQPState_SSEM.EQ_IDLE);
        }

        private void btnEQPRun_Click(object sender, EventArgs e)
        {
            m_Gem300.SetEQPState(Gem_SSEM.eEQPState_SSEM.EQ_RUN);
        }

        private void btnEQPPM_Click(object sender, EventArgs e)
        {
            m_Gem300.SetEQPState(Gem_SSEM.eEQPState_SSEM.EQ_PM);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            m_Gem300.LoadComplete(1);
            m_Gem300.SetMainCycle(Gem_SSEM.eMainCycle.Wait_CarrierIDRead, 1);
            m_Gem300.SetEQPState(Gem_SSEM.eEQPState_SSEM.EQ_RUN);

        }

        private void btnPlacementDetectedLP2_Click(object sender, EventArgs e)
        {
            m_Gem300.LoadComplete(2);
            m_Gem300.SetMainCycle(Gem_SSEM.eMainCycle.Wait_CarrierIDRead, 2);
            m_Gem300.SetEQPState(Gem_SSEM.eEQPState_SSEM.EQ_RUN);

        }

        private void btnAccessModeChangeLP1_Click(object sender, EventArgs e)
        {
            switch (m_Gem300.GetCMS(1).m_eAccessMode) {
                case Gem_SSEM.cCMS.ePortAccessMode.Auto:
                    m_Gem300.SetLPAccessMode(1, Gem_SSEM.cCMS.ePortAccessMode.Manual);
                    break;
                case Gem_SSEM.cCMS.ePortAccessMode.Manual:
                    m_Gem300.SetLPAccessMode(1, Gem_SSEM.cCMS.ePortAccessMode.Auto);
                    break;
            }
        }

        private void btnAccessModeChangeLP2_Click(object sender, EventArgs e)
        {
            switch (m_Gem300.GetCMS(2).m_eAccessMode) {
                case Gem_SSEM.cCMS.ePortAccessMode.Auto:
                    m_Gem300.SetLPAccessMode(2, Gem_SSEM.cCMS.ePortAccessMode.Manual);
                    break;
                case Gem_SSEM.cCMS.ePortAccessMode.Manual:
                    m_Gem300.SetLPAccessMode(2, Gem_SSEM.cCMS.ePortAccessMode.Auto);
                    break;
            }
        }

        private void btnDockingLP1_Click(object sender, EventArgs e)
        {
            m_Gem300.LoadComplete(1);
        }

        private void btnDockingLP2_Click(object sender, EventArgs e)
        {
            m_Gem300.LoadComplete(2);
        }

        private void btnCarrierIDReadLP1_Click(object sender, EventArgs e)
        {
            if (tbLP1LotID.Text == "" || tbP1CarrierID.Text == "") {
                MessageBox.Show("Insert Lot ID & Cassette ID First");
                return;
            }
            m_Gem300.CarrierIDRead(1, tbP1CarrierID.Text, tbLP1LotID.Text);
            if (m_Gem300.GetMainCycle() == Gem_SSEM.eMainCycle.Wait_CarrierIDRead)
                m_Gem300.SetMainCycle(Gem_SSEM.eMainCycle.WaitHost_CarrierIDPass, 1);
        }

        private void btnCarrierIDReadLP2_Click(object sender, EventArgs e)
        {
            if (tbLP2LotID.Text == "" || tbLP2CarrierId.Text == "") {
                MessageBox.Show("Insert Lot ID & Cassette ID Info First");
                return;
            }
            m_Gem300.CarrierIDRead(2, tbLP2CarrierId.Text, tbLP2LotID.Text);
            if (m_Gem300.GetMainCycle() == Gem_SSEM.eMainCycle.Wait_CarrierIDRead)
                m_Gem300.SetMainCycle(Gem_SSEM.eMainCycle.WaitHost_CarrierIDPass, 1);
        }

        private void btnSlotMapWait_Click(object sender, EventArgs e)
        {
            if (tbLP1SlotMap.Text == "" || tbLP1MaxSlot.Text == "") {
                MessageBox.Show("Insert SlotMap & Max Slot Info First");
                return;
            }
            if (Convert.ToInt32(tbLP1MaxSlot.Text) != tbLP1SlotMap.Text.Length) {
                MessageBox.Show("Different Size Slot Map (Check Max SlotNum & Slot Map lenght");
                return;
            }
            m_Gem300.SendSlotMapToHost(1, tbLP1SlotMap.Text);
            if (m_Gem300.GetMainCycle() == Gem_SSEM.eMainCycle.Wait_SlotMapForHost)
                m_Gem300.SetMainCycle(Gem_SSEM.eMainCycle.WaitHost_SlotMapPass, 1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (tbLP2SlotMap.Text == "" || tbLP2MaxSlot.Text == "") {
                MessageBox.Show("Insert SlotMap & Max Slot Info First");
                return;
            }
            if (Convert.ToInt32(tbLP2MaxSlot.Text) == tbLP2SlotMap.Text.Length) {
                MessageBox.Show("Different Size Slot Map (Check Max SlotNum & Slot Map lenght");
                return;
            }
            m_Gem300.SendSlotMapToHost(2, tbLP2SlotMap.Text);
            if (m_Gem300.GetMainCycle() == Gem_SSEM.eMainCycle.Wait_SlotMapForHost)
                m_Gem300.SetMainCycle(Gem_SSEM.eMainCycle.WaitHost_SlotMapPass, 1);
        }

        private void btnSlotMapOK_Click(object sender, EventArgs e)
        {
            m_Gem300.SendSlotMapVerificationOK(1);
            if (m_Gem300.GetMainCycle() == Gem_SSEM.eMainCycle.Wait_SlotMapVerificationOK)
                m_Gem300.SetMainCycle(Gem_SSEM.eMainCycle.WaitHost_PRJOBCreate, 1);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            m_Gem300.SendSlotMapVerificationOK(2);
            if (m_Gem300.GetMainCycle() == Gem_SSEM.eMainCycle.Wait_SlotMapVerificationOK)
                m_Gem300.SetMainCycle(Gem_SSEM.eMainCycle.WaitHost_PRJOBCreate, 2);
        }

        private void btnQueued_Click(object sender, EventArgs e)
        {
            m_Gem300.PRJobStateChage(Gem_SSEM.cPJ.ePRJObState.Queued);
            if (m_Gem300.GetMainCycle() == Gem_SSEM.eMainCycle.Wait_PRJobQueued)
                m_Gem300.SetMainCycle(Gem_SSEM.eMainCycle.WaitHost_CTRLJOBCreate, 1);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            m_Gem300.PRJobStateChage(Gem_SSEM.cPJ.ePRJObState.WaitingForStart);
            if (m_Gem300.GetMainCycle() == Gem_SSEM.eMainCycle.Wait_PRJobQueued)
                m_Gem300.SetMainCycle(Gem_SSEM.eMainCycle.WaitHost_CTRLJOBCreate, 2);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            m_Gem300.PRJobStateChage(Gem_SSEM.cPJ.ePRJObState.Processing);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            m_Gem300.PRJobStateChage(Gem_SSEM.cPJ.ePRJObState.ProcessingComplete);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            m_Gem300.CTRLJobStateChage(Gem_SSEM.cCJ.eCTRLJobState.Queued);

        }

        private void button16_Click(object sender, EventArgs e)
        {
            m_Gem300.CTRLJobStateChage(Gem_SSEM.cCJ.eCTRLJobState.WaitingForStart);

        }

        private void button15_Click(object sender, EventArgs e)
        {
            m_Gem300.CTRLJobStateChage(Gem_SSEM.cCJ.eCTRLJobState.Excuting);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            m_Gem300.CTRLJobStateChage(Gem_SSEM.cCJ.eCTRLJobState.Completed);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            m_Gem300.ProcessStart();
        }


        private void button18_Click(object sender, EventArgs e)
        {
            m_Gem300.ProcessEnd();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            switch ((Gem_SSEM.cCMS.eProductType)cbWaferType.SelectedIndex + 1) {
                case Gem_SSEM.cCMS.eProductType.QUAD:
                    m_Gem300.WaferStart(Gem_SSEM.cCMS.eProductType.QUAD, tbWaferIDVS.Text, Convert.ToInt32(tbSlotNumVS.Text));
                    break;
                case Gem_SSEM.cCMS.eProductType.PANEL:
                    m_Gem300.WaferStart(Gem_SSEM.cCMS.eProductType.PANEL, tbWaferIDVS.Text, Convert.ToInt32(tbSlotNumVS.Text));
                    break;
                case Gem_SSEM.cCMS.eProductType.WAFER:
                    m_Gem300.WaferStart(Gem_SSEM.cCMS.eProductType.WAFER, tbWaferIDVS.Text, Convert.ToInt32(tbSlotNumVS.Text));
                    break;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            switch ((Gem_SSEM.cCMS.eProductType)cbWaferType.SelectedIndex + 1) {
                case Gem_SSEM.cCMS.eProductType.QUAD:
                    m_Gem300.WaferEnd(Gem_SSEM.cCMS.eProductType.QUAD);
                    break;
                case Gem_SSEM.cCMS.eProductType.PANEL:
                    m_Gem300.WaferEnd(Gem_SSEM.cCMS.eProductType.PANEL);
                    break;
                case Gem_SSEM.cCMS.eProductType.WAFER:
                    m_Gem300.WaferEnd(Gem_SSEM.cCMS.eProductType.WAFER);
                    break;
            }

        }

        private void button19_Click(object sender, EventArgs e)
        {
            m_Gem300.Rework(1);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            m_Gem300.Scrap(1);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            /*switch (cbTest.SelectedIndex) {
                case 0:
                    //m_Gem300.Rework(1);//AlarmReport
                    //m_Gem300.SetAlarm(true, true, Gem300.eALList.TestAlram1, "TestAlram1");
                    //m_Gem300.AlarmReport(1000,false);
                    break;
                case 1:                 //MapReq
                    m_Gem300.MapDataInfoRequest("TestWafer");
                    break;
                case 2:                     //Map Down
                    m_Gem300.MapDataDownLoad("TestWafer");
                    break;
                case 3:
                    m_Gem300.SendDecallEvent();
                    break;
            }*/
            m_Gem300.SetAlarm(true, true, Gem_SSEM.eALList.Emergency_Error, "EmergencyError");
        }

        private void button24_Click(object sender, EventArgs e)
        {
            m_Gem300.WorkStart();

        }

        private void button6_Click(object sender, EventArgs e)
        {
            m_Gem300.UnLoadRequest(1);

        }

        private void button7_Click(object sender, EventArgs e)
        {
            m_Gem300.UnLoadRequest(2);

        }

        private void button8_Click(object sender, EventArgs e)
        {
            m_Gem300.UnLoadComplete(1);

        }

        private void button19_Click_1(object sender, EventArgs e)
        {
            m_Gem300.WorkComplete();
            m_Gem300.SetEQPState(Gem_SSEM.eEQPState_SSEM.EQ_IDLE);

        }

        private void button9_Click(object sender, EventArgs e)
        {
            m_Gem300.UnLoadComplete(1);

        }

        private void TraceTimer_Tick(object sender, EventArgs e)
        {
            m_Gem300.SendTraceData();
        }

        private void UI_Gem300_Load(object sender, EventArgs e)
        {

        }

        private void lvTerminalMSG_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lbMainCycle_Click(object sender, EventArgs e)
        {

        }

        private void button25_Click(object sender, EventArgs e)
        {
            int nIndex = -1;
            for (int i = 0; i < lvRecipe.Items.Count; i++) {
                if (lvRecipe.Items[i].SubItems[0].Text == tbRecipeName.Text)
                    nIndex = i;
            }
            if (nIndex != -1) {
                lvRecipe.Items[nIndex].SubItems[0].Text = tbRecipeName.Text;
                lvRecipe.Items[nIndex].SubItems[1].Text = textBox2.Text;
                lvRecipe.Items[nIndex].SubItems[2].Text = textBox3.Text;
                lvRecipe.Items[nIndex].SubItems[3].Text = textBox4.Text;
                lvRecipe.Items[nIndex].SubItems[4].Text = textBox5.Text;
                lvRecipe.Items[nIndex].SubItems[5].Text = textBox6.Text;
                lvRecipe.Items[nIndex].SubItems[6].Text = textBox7.Text;
                lvRecipe.Items[nIndex].SubItems[7].Text = textBox8.Text;
                lvRecipe.Items[nIndex].SubItems[8].Text = textBox9.Text;
                lvRecipe.Items[nIndex].SubItems[9].Text = textBox10.Text;
                lvRecipe.Items[nIndex].SubItems[10].Text = textBox11.Text;
                SaveRecipe();
                m_Gem300.LoadRecipe();
                m_Gem300.PPIDChanged(Gem_SSEM.ePPIDChangeMode.Modify, tbRecipeName.Text, textBox2.Text, textBox3.Text, textBox4.Text, textBox5.Text, textBox6.Text, textBox7.Text, textBox8.Text, textBox9.Text, textBox10.Text, textBox11.Text);
            }
        }


    }
}