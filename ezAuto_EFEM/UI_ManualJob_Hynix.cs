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
using System.IO;

namespace ezAuto_EFEM
{
    public partial class UI_ManualJob_Hynix : Form, UI_ManualJob_Mom
    {
        public string m_id;
        int m_nLoadPort = 0;
        bool m_bMapDataUse=false;
        string m_strPath; 
        protected Log m_log;
        protected Auto_Mom m_auto;
        Handler_Mom m_handler;
        Info_Carrier m_infoCarrier;
        Recipe_Mom m_recipe;
        WaferSelect[] m_waferselect;
        

        public class WaferSelect
        {
            public CheckBox chkBox = new CheckBox();
            public TextBox tbWaferID = new TextBox();
            public WaferSelect(Panel obj)
            {
                chkBox.Appearance = Appearance.Button;
                chkBox.CheckedChanged += chkBox_CheckedChanged;
                chkBox.TextAlign = ContentAlignment.MiddleCenter;
                chkBox.BackgroundImageLayout = ImageLayout.Zoom;
                obj.Controls.Add(chkBox);
                obj.Controls.Add(tbWaferID);
            }

            void chkBox_CheckedChanged(object sender, EventArgs e)
            {
                if (chkBox.Checked == true)
                {
                    chkBox.BackgroundImage = global::ezAuto_EFEM.Properties.Resources._1460027369_tick_16;
                }
                else
                {
                    chkBox.BackgroundImage = null;
                }
            }

            public void SetEnable(bool b)
            {
                chkBox.Enabled = b;
                tbWaferID.Enabled = b;
            }

        }

        public UI_ManualJob_Hynix()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        public void Init(string id, Auto_Mom auto, int nLP,bool MapdataUse, string strPath)
        {
            m_id = id;
            m_auto = auto;
            m_nLoadPort = nLP;
            m_bMapDataUse = MapdataUse;
            m_strPath = strPath; 
            m_log = new Log(m_id, m_auto.m_logView);
            m_handler = m_auto.ClassHandler();
            m_recipe = m_auto.ClassRecipe();
            m_infoCarrier = m_auto.ClassHandler().ClassCarrier(nLP);
            m_waferselect = new WaferSelect[25];
            for (int n = 0; n < m_waferselect.Length; n++)
            {
                Info_Wafer wafer = m_infoCarrier.m_infoWafer[n];
                bool bExist = wafer.State == Info_Wafer.eState.Exist;
                bool bSelect = wafer.State == Info_Wafer.eState.Select;
                
                m_waferselect[n] = new WaferSelect(panelWaferInfo);
                m_waferselect[n].SetEnable(bExist || bSelect);
                m_waferselect[n].chkBox.Checked = bSelect;
            }
            tbCSTID.Text = m_infoCarrier.m_strCarrierID;
            tbLotID.Text = m_infoCarrier.m_strLotID;
            
        }

        private void UI_ManualJob_Hynix_Shown(object sender, EventArgs e)
        {

        }
        
        private void UI_ManualJob_Paint(object sender, PaintEventArgs e)
        {
        }

        private void UI_ManualJob_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible) return;
            RecipeFileSearch();
        }

        private Brush GetStateBrush(Info_Wafer.eState s)
        {
            Color ColorState = Color.Black;
            switch (s)
            {
                case Info_Wafer.eState.Empty: ColorState = Color.White; break;
                case Info_Wafer.eState.Exist: ColorState = Color.Silver; break;
                case Info_Wafer.eState.Select: ColorState = Color.Tan; break;
                case Info_Wafer.eState.Run: ColorState = Color.SkyBlue; break;
                case Info_Wafer.eState.Done: ColorState = Color.SpringGreen; break;
                case Info_Wafer.eState.Bad: ColorState = Color.Red; break;
            }
            return new SolidBrush(ColorState);
        }

        private int GetTxtWidth(Graphics G, string s, Font f)
        {
            SizeF AdjustedSizeNew = G.MeasureString(s, f);
            return (int)(AdjustedSizeNew.Width * 1.2);
        }

        private void DrawStringFillRect(Graphics G, string str, Font font, Rectangle rt, Brush fill = null)
        {
            Pen pen = new Pen(Color.LightGray);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            G.DrawRectangle(pen, rt);
            if (fill != null)
            {
                G.FillRectangle(fill, rt);
            }

            G.DrawString(str, font, Brushes.Black, rt, stringFormat);
        }

        private void DrawControl(Control ctrl, Rectangle rt)
        {
            ctrl.Location = new Point(rt.Left, rt.Top);
            ctrl.Size = new Size(rt.Width, rt.Height);
        }

        private void panelWaferInfo_Paint(object sender, PaintEventArgs e)
        {
            #region WaferInfo
            int nMargin = panelWaferInfo.Margin.Left + panelWaferInfo.Margin.Right;
            int nH = panelWaferInfo.Height - nMargin;
            int nW = panelWaferInfo.Width - nMargin;
            int nHBox = nH / m_infoCarrier.m_lWafer;

            using (BufferedGraphics bufferedgraphic = BufferedGraphicsManager.Current.Allocate(e.Graphics, this.ClientRectangle))
            {
                Graphics G = bufferedgraphic.Graphics;
                G.Clear(this.BackColor);
                G.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                G.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);

                Font FontWafer = new Font(this.Font.Name, (int)(nHBox * 0.6), FontStyle.Regular, GraphicsUnit.Pixel);
                int nWidthSelect = 70;
                int nWidthSlotNum = GetTxtWidth(G, String.Format("{0,3}", m_infoCarrier.m_lWafer), FontWafer);
                int nWidthState = GetTxtWidth(G, Info_Wafer.eState.Select.ToString(), FontWafer);
                int nWidthWaferID = nW - nWidthSlotNum - nWidthState - nWidthSelect;

                for (int n = 0; n < m_infoCarrier.m_lWafer; n++)
                {
                    Info_Wafer wafer = m_infoCarrier.m_infoWafer[n];
                    Rectangle rtSelect = new Rectangle(0, nH - (nHBox * (n + 1)), nWidthSelect, nHBox);
                    Rectangle rtSlot = new Rectangle(rtSelect.Right, rtSelect.Top, nWidthSlotNum, nHBox);
                    Rectangle rtState = new Rectangle(rtSlot.Right, rtSlot.Top, nWidthState, nHBox);
                    Rectangle rtWaferID = new Rectangle(rtState.Right, rtState.Top, nWidthWaferID, nHBox);

                    string strSlotNum = String.Format("{0,3}", wafer.m_nSlot);

                    DrawControl(m_waferselect[n].chkBox, rtSelect);
                    DrawStringFillRect(G, strSlotNum, FontWafer, rtSlot, Brushes.FloralWhite);
                    DrawStringFillRect(G, wafer.State.ToString(), FontWafer, rtState, GetStateBrush(wafer.State));
                    m_waferselect[n].tbWaferID.Font = FontWafer;
                    DrawControl(m_waferselect[n].tbWaferID, rtWaferID);

                }
                
                bufferedgraphic.Render(e.Graphics);
            }
            #endregion
        }

        private void buttonSelectAll_Paint(object sender, PaintEventArgs e)
        {
            panelWaferInfo.Invalidate();
        }

        private void buttonSelectAll_Click(object sender, EventArgs e)
        {
            for (int n = 0; n < m_waferselect.Length; n++)
            {
                if (m_waferselect[n].chkBox.Enabled)
                {
                    m_waferselect[n].chkBox.Checked = true;
                }
            }
            Invalidate();
        }

        private void buttonUnSelectAll_Click(object sender, EventArgs e)
        {
            for (int n = 0; n < m_waferselect.Length; n++)
            {
                m_waferselect[n].chkBox.Checked = false;
            }
            Invalidate();
        }

        private void buttonAdd1_Click(object sender, EventArgs e)
        {
            for (int n = 0; n < m_waferselect.Length; n++)
            {
                if (m_waferselect[n].chkBox.Enabled && (m_waferselect[n].chkBox.Checked == false))
                {
                    m_waferselect[n].chkBox.Checked = true;
                    return;
                }
            }
        }
        private void buttonCheckRecipe_Click(object sender, EventArgs e)
        {

        }

        private void cbRecipeID_EnabledChanged(object sender, EventArgs e)
        {

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            bool bRst = false;
           
            for (int n = 0; n < m_waferselect.Length; n++)
            {
                Info_Wafer wafer = m_infoCarrier.m_infoWafer[n];
                if (m_waferselect[n].chkBox.Checked == true)
                {
                    wafer.State = Info_Wafer.eState.Select;
                    wafer.WAFERID = m_waferselect[n].tbWaferID.Text;
                    bRst = true;
                    m_recipe.SetInfoWafer(wafer, m_infoCarrier); 
                }
                else
                {
                    if (wafer.State == Info_Wafer.eState.Select)
                    {
                        wafer.State = Info_Wafer.eState.Exist;
                    }
                    wafer.WAFERID = "";
                }
                    
            }
            m_infoCarrier.m_strRecipe = tbRecipe.Text;
            m_infoCarrier.m_strLotID = tbLotID.Text;
            m_infoCarrier.m_strCarrierID = tbCSTID.Text;        //160608 merge tbQRCode -> tbLotID
            m_auto.ClassWork().m_cRunData.sLotID = m_infoCarrier.m_strLotID;
            m_auto.ClassWork().m_cRunData.sPartNo = "*";
            m_auto.ClassWork().m_cRunData.sLoadQty = "0";

            if (bRst == true)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            else
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            }
            //m_handler.ClassVisionWorks().SendLotStart(m_infoCarrier);         //160608
            this.Close();
        }

        private void textBoxNameFilter_TextChanged(object sender, EventArgs e)
        {
            Invalidate();
            RecipeFileSearch();
        }

        public void RecipeFileSearch()
        {
            int n = 0;
            try
            {
                listViewRecipe.Items.Clear();
                if (m_recipe == null) return;

                DirectoryInfo dir = new DirectoryInfo(m_recipe.m_strPath);
                if (dir.Exists == false) return;       
                FileInfo[] files = dir.GetFiles("*.ezEFEM");
                string[] m_strModels = new string[files.Length];

                foreach (FileInfo file in files)
                {
                    m_recipe.JobOpen(file.FullName);

                    if (m_recipe.m_wafer.m_eSize == m_infoCarrier.m_wafer.m_eSize) 
                    {
                        m_strModels[n] = m_recipe.m_sRecipe;
                        n++;

                        if (textBoxNameFilter.Text == "" || m_recipe.m_sRecipe.Contains(textBoxNameFilter.Text) == true)
                        {
                            ListViewItem item = new ListViewItem(m_recipe.m_sRecipe);
                            item.SubItems.Add(m_recipe.m_wafer.m_eSize.ToString());    //size
                            double angle = m_recipe.m_fAngleAlign; 
                            item.SubItems.Add(angle.ToString());    //angle
                            item.SubItems.Add("");    //Mark
                            item.SubItems.Add(m_recipe.m_strModifyTime);    //Datetime

                            listViewRecipe.Items.Add(item);

                        }
                    }
                }
            }
            catch (Exception ext)
            {
                MessageBox.Show(ext.ToString());
            }
            if (m_recipe.m_strFile != "")
            {
                m_recipe.JobOpen(m_recipe.m_strFile);
                m_recipe.RunGrid(eGrid.eInit);
            }
        }

        private void listViewRecipe_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (!e.IsSelected) return;
            tbRecipe.Text = e.Item.Text;
            m_infoCarrier.m_strRecipe = e.Item.Text;
            m_recipe.m_strFile = m_recipe.m_strPath + "\\" + e.Item.Text + ".ezEFEM";
            m_recipe.m_log.m_reg.Write("Recipe_Job", m_recipe.m_strFile);
            m_recipe.JobOpen(m_recipe.m_strFile);
            m_recipe.RunGrid(eGrid.eInit); 
            if (m_infoCarrier.m_wafer.m_eSize.ToString() == e.Item.SubItems[1].Text)
            {
                tbWaferSize.Text = e.Item.SubItems[1].Text; 
            }
            else
            {
                tbWaferSize.Text = "MissMatch";
            }
        }

        public void SetRNRMode()
        {
            for (int n = 0; n < m_waferselect.Length; n++)
            {
                if (m_handler.ClassCarrier(m_nLoadPort).m_infoWafer[n].IsExist() == true)
                {
                    m_waferselect[n].chkBox.Checked = true;
                }

            }
            string strTime;
            strTime = System.DateTime.Now.ToString("MMddHHmm");
            for (int n = 0; n < m_waferselect.Length; n++)
            {
                Info_Wafer wafer = m_infoCarrier.m_infoWafer[n];
                if (m_waferselect[n].chkBox.Checked == true)
                {
                    wafer.State = Info_Wafer.eState.Select;
                    wafer.WAFERID = strTime + "_" + n.ToString();
                    m_recipe.SetInfoWafer(wafer, m_infoCarrier);
                }
            }
            m_infoCarrier.m_strRecipe = m_recipe.m_sRecipe;
        }

        public void SetXGemMode(int nLP)
        {
            for (int n = 0; n < m_waferselect.Length; n++) 
            {
                if (m_handler.ClassCarrier(m_nLoadPort).m_infoWafer[n].IsExist() == true && m_auto.ClassXGem().GetSlotMap(nLP, n))
                {
                    m_waferselect[n].chkBox.Checked = true;
                }
            }
            string strTime;
            strTime = System.DateTime.Now.ToString("MMddHHmm");

            m_recipe.JobOpen(m_recipe.GetFileName(m_auto.ClassXGem().m_XGem300Process[m_nLoadPort].m_sRecipeID));
            m_recipe.RunGrid(eGrid.eInit); 

            for (int n = 0; n < m_waferselect.Length; n++)
            {
                Info_Wafer wafer = m_infoCarrier.m_infoWafer[n];
                if (m_waferselect[n].chkBox.Checked == true)
                {
                    wafer.State = Info_Wafer.eState.Select;
                    wafer.WAFERID = (n+1).ToString();
                    m_recipe.SetInfoWafer(wafer, m_infoCarrier);
                    m_handler.ClassCarrier(m_nLoadPort).m_infoWafer[n].m_strCarrierID = m_auto.ClassXGem().GetCSTID(nLP);
                }
            }
            m_infoCarrier.m_strRecipe = m_auto.ClassXGem().m_XGem300Process[m_nLoadPort].m_sRecipeID;
            m_infoCarrier.m_strCarrierID = m_auto.ClassXGem().GetCSTID(nLP);
            m_infoCarrier.m_strLotID = m_auto.ClassXGem().GetLOTID(nLP);
            m_auto.ClassWork().m_cRunData.sLotID = m_infoCarrier.m_strLotID;
            m_auto.ClassWork().m_cRunData.sPartNo = "*";
            m_auto.ClassWork().m_cRunData.sLoadQty = "0";
            m_log.Add("Recipe Open : " + m_auto.ClassXGem().m_XGem300Process[m_nLoadPort].m_sRecipeID);
        
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            if (pnManualJob.BackColor == Color.SkyBlue)
            {
                pnManualJob.BackColor = Color.DeepSkyBlue;
            }
            else if (pnManualJob.BackColor == Color.DeepSkyBlue)
            {
                pnManualJob.BackColor = Color.AliceBlue;
            }
            else if (pnManualJob.BackColor == Color.AliceBlue)
            {
                pnManualJob.BackColor = Color.SkyBlue;
            }
            if (m_infoCarrier.m_eState != Info_Carrier.eState.ManualJob) // ing 170521
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void tbLotID_TextChanged(object sender, EventArgs e)
        {
            for (int n = 0; n < 3; n++)
            {
                Info_Carrier infoCarrier = m_auto.ClassHandler().ClassCarrier(n);
                if ((infoCarrier != null) && (n != m_nLoadPort))
                {
                }
            }
            ChangeWaferLotID();
        }

        void ChangeWaferLotID()
        {
            for (int n = 0; n < m_infoCarrier.m_lWafer; n++)
            {
                int nSlot = m_infoCarrier.m_infoWafer[n].m_nSlot;
                m_waferselect[n].tbWaferID.Text = tbLotID.Text + "_" + nSlot.ToString();
            }
        }

        private void btnCheckMapFile_Click(object sender, EventArgs e)
        {
            if (tbWaferSize.Text == "MissMatch" || tbWaferSize.Text == "")
            {
                m_log.Popup("Please Select Recipe");
                return;
            }
            else
            {
                if (buttonRun.Enabled == true)
                {
                    btnCheckMapFile.Text = "ReSetting";
                    for (int n = 0; n < m_infoCarrier.m_lWafer; n++)
                        m_waferselect[n].SetEnable(false);
                    buttonRun.Enabled = false;
                }
                else
                {
                    btnCheckMapFile.Text = "Check Recipe";
                    buttonRun.Enabled = true;
                    for (int n = 0; n < m_infoCarrier.m_lWafer; n++)
                        if (m_infoCarrier.m_infoWafer[n].State == Info_Wafer.eState.Exist || m_infoCarrier.m_infoWafer[n].State == Info_Wafer.eState.Select)
                            m_waferselect[n].SetEnable(true);
                }

            }
        }

        public DialogResult ShowModal()
        {
            return ShowDialog(); 
        }

    }
}
