using System;
using System.Collections;
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

namespace ezAutoMom
{
    public partial class Recipe_Mom : DockContent
    {
        public enum eType_BCR
        {
            Disable,
            CognexLib
        }



        ////////////////////////////InspMode 추가 170401 만우절
        public enum eInspMode
        {
            FullSlot,
            SelectedSlot,
            CountinuousSlot,
        }
        public eInspMode m_eInspMode = eInspMode.FullSlot;
        public string[] m_sInspMode = Enum.GetNames(typeof(eInspMode));

        public enum eInspDir
        {
            Bottom,
            Top,
        }
        public eInspDir m_eInspDIr = eInspDir.Bottom;
        public string[] m_sInspDir = Enum.GetNames(typeof(eInspDir));
        public int m_nInspCount = 1;
        public bool[] m_aInspSelect = new bool[13] { false, false, false, false, false, false, false, false, false, false, false, false, false };
        /// //////////////////////////////////


        public eType_BCR m_eBCR = eType_BCR.Disable;
        public bool m_bUseBCR = false;
        public bool m_bDirectionNotch = false;
        public double m_fAngleBCR = 0;
        public double m_fAngleChar = 5;
        public double m_fdRBCR = 0;
        public string m_sTypeBCR = "2D";
        protected string[] m_sTypeBCRs = new string[2] { "1D", "2D" };

        public enum eType_OCR
        {
            Disable,
            CognexCam
        }
        public eType_OCR m_eOCR = eType_OCR.Disable;
        public bool m_bUseOCR = false;
        public double m_fAngleOCR = 0;
        public double m_fdROCR = 0;
        public double m_fOCRReadScore = 170;


        public eType_OCR m_eOCRBottom = eType_OCR.Disable;
        public eInspDir m_eOCRDir = eInspDir.Top;
        public string[] m_sOCRDir = Enum.GetNames(typeof(eInspDir));

        public bool m_bUseVision = true;
        public bool m_bUseBackSide = false;
        public bool m_bRunEdge = false;
        public bool m_bOnlyInkMark = false;

        // for MSB Backside EFEM
        public string[] m_sWaferIDTypes = { "None", "OCR", "BCR", "PM" };
        public string m_sWaferIDType = "None";
        public string m_sPMWaferID = "PM_Micron";
        public string m_strBinPath = "D:\\BinCode\\BinCode.txt";
        public bool[] m_bUseCode = new bool[5];

        protected string m_strDefaultRCP = "Default";

        public enum eMarkType
        {
            Notch,
            FlatZone
        }
        public string m_strRACPath = "D:\\RecipeRAC";            //181227 SDH ADD
        public bool m_bUseRAC = false;
        public string m_id;
        public string m_strPath = "D:\\Recipe";
        public string m_sRecipe = "";
        public string[] m_sRecipes;
        public string m_strFile = "";
        public Wafer_Size m_wafer;
        public int m_nWaferThick = 100;
        public double m_fAngleAlign = 0;
        public string m_strModifyTime;
        protected Handler_Mom m_handler;
        protected Work_Mom m_work;
        protected HW_Aligner_Mom m_align = null;
        protected HW_BackSide_Mom m_backSide = null;
        public Log m_log;
        protected ezGrid m_grid;
        protected Auto_Mom m_auto;

        ArrayList m_aList = new ArrayList();

        public Recipe_Mom()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.CloseButtonVisible = false;
        }

        public virtual void Init(string id, Auto_Mom auto)
        {
            m_id = id; this.Text = m_id;
            m_auto = auto;
            m_handler = auto.ClassHandler();
            m_work = auto.ClassWork();
            m_align = m_handler.ClassAligner();
            m_backSide = m_handler.ClassBackSide();
            m_log = new Log(m_id, m_auto.m_logView);
            m_grid = new ezGrid(m_id, grid_Recipe, m_log, false);
            m_wafer = new Wafer_Size(m_log);

            m_log.m_reg.Read("Recipe_Job", ref m_strFile);
            ReadRecipeDate();
            timerEnable.Enabled = true;
            InitEditForm(); // ing 170623
        }

        public virtual void SetInfoWafer(Info_Wafer infoWafer, Info_Carrier infoCarrier)
        {
            infoWafer.m_sRecipe = m_sRecipe; 
            infoWafer.m_fAngleAlign = m_fAngleAlign;
            infoWafer.m_bUseBCR = m_bUseBCR;
            infoWafer.m_fAngleBCR = m_fAngleBCR;
            infoWafer.m_fdRBCR = m_fdRBCR;
            infoWafer.m_sTypeBCR = m_sTypeBCR;
            infoWafer.m_bUseOCR = m_bUseOCR;
            infoWafer.m_fAngleOCR = m_fAngleOCR;
            infoWafer.m_fdROCR = m_fdROCR;
            infoWafer.m_bUseVision = m_bUseVision;
            if(infoCarrier != null)
                infoCarrier.m_bUseVision = m_bUseVision;
            if (m_wafer.IsRingFrame()) // ing 171017
            {
                infoWafer.m_bUseBackSide = m_bUseBackSide;
                infoWafer.m_bRotate180 = m_auto.ClassHandler().ClassAligner().m_bRotate180;
            }
            infoWafer.m_bRunEdge = m_bRunEdge;
            infoWafer.m_fOCRReadScore = m_fOCRReadScore;    //KDG 161025 Add OCR Score
            infoWafer.m_bOnlyInkMark = m_bOnlyInkMark;
            infoWafer.m_sWaferIDType = m_sWaferIDType;
            if (m_sWaferIDType == "PM") infoWafer.m_strWaferID = m_sPMWaferID; // ing 170410
            infoWafer.m_bUseAligner = m_auto.ClassHandler().ClassAligner().m_bEnableAlign; // ing 170531
        }

        public virtual void RunGrid(eGrid eMode, ezJob job = null)
        {
            m_grid.Update(eMode, job);
        }

        public virtual void RunRAC(string strName, eGrid eMode, ezJob job = null)
        {
            //m_grid.Update(eMode, job);
        }

        private void Recipe_Paint(object sender, PaintEventArgs e)
        {
            if (m_work.IsRun()) return;
            //FileSearch();
        }

        public string GetFileName(string strModel)
        {
            return m_strPath + "\\" + strModel + ".ezEFEM";
        }

        public virtual void JobOpen(string strFile, bool bReadAll = true) // ing 161208
        {
            ezJob job = new ezJob(strFile, false);
            if (job == null) return;
            RunGrid(eGrid.eJobOpen, job);
            if (bReadAll) m_work.JobOpen(job); // ing 161013
            job.Read(m_id, "DateTime", ref m_strModifyTime);
            job.Close();
            m_log.Add("<" + strFile + "> Job Open Finish.");
        }

        public virtual void JobSave() // ing 161013
        {
            m_strFile = GetFileName(m_sRecipe);
            ezJob job = new ezJob(m_strFile, true);
            RunGrid(eGrid.eJobSave, job);
            m_work.JobSave(job);
            job.Write(m_id, "DateTime", DateTime.Now.ToString("yy/MM/dd HH:mm:ss"));
            job.Close();
            this.Invalidate();
            m_log.m_reg.Write("Recipe_Job", m_strFile);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            m_strFile = GetFileName(m_sRecipe);
            ezJob job = new ezJob(m_strFile, true);
            RunGrid(eGrid.eJobSave, job);
            m_work.JobSave(job); // ing 161013
            job.Write(m_id, "DateTime", DateTime.Now.ToString("yy/MM/dd HH:mm:ss"));
            job.Close();
            this.Invalidate();
            m_log.m_reg.Write("Recipe_Job", m_strFile);
            JobSave();
            ReadRecipeDate();
        }

        private void listViewRecipe_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            //ListView lvr = (ListView)sender;
            //if (!m_auto.ClassWork().IsRun())
            //{
            //    if (!e.IsSelected) return;
            //    if (m_auto.ClassWork().IsRun()) return;
            //    if (lvr.SelectedIndices.Count > 1) return;
            //    m_sRecipe = e.Item.Text;
            //    m_strFile = GetFileName(m_sRecipe);
            //    JobOpen(m_strFile);
            //    RunGrid(eGrid.eInit);
            //    m_log.m_reg.Write("Recipe_Job", m_strFile);
            //}
        }

        private void grid_Recipe_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate);
            RunGrid(eGrid.eInit);
        }

        public void ModelGrid(ezGrid rGrid, eGrid eMode)
        {
            int nType;
            rGrid.Set(ref m_strPath, "Recipe", "Path", "Default Recipe Path");
            rGrid.Set(ref m_strBinPath, "Recipe", "BinCodePath", "Bin Code File Path"); // ing 170412
            nType = (int)m_eBCR;
            rGrid.Set(ref nType, "BCR", "Type", "0=Disable, 1=CognexLib");
            m_eBCR = (eType_BCR)nType;
            nType = (int)m_eOCR;
            rGrid.Set(ref nType, "OCR", "Type", "0=Disable, 1=CognexCam");
            m_eOCR = (eType_OCR)nType;
            rGrid.Set(ref nType, "OCR_Bottom", "Type", "0=Disable, 1=CognexCam");
            m_eOCRBottom = (eType_OCR)nType;
            rGrid.Set(ref m_strDefaultRCP, "Recipe", "DefaultRecipe", "Default Recipe Name For Creating From Vision");
            rGrid.Set(ref m_bUseRAC, "RAC", "Use", "RAC(Recipe Auto Create) Check Use");                //181226 SDH ADD
            rGrid.Set(ref m_strRACPath, "RAC", "Path", "RAC(Recipe Auto Create) Check Path");           //181226 SDH ADD
        }

        public virtual bool IsRecipeExist(string strRecipe)
        {
            string[] strFiles = Directory.GetFiles(m_strPath, strRecipe + ".ezEFEM", SearchOption.TopDirectoryOnly);
            return (strFiles.Length > 0);
        }

        protected void ReadRecipeDate()
        {
            m_aList.Clear();
            try {
                DirectoryInfo dir = new DirectoryInfo(m_strPath);
                if (dir.Exists == false) {
                    if (dir.Root.Exists == false) {
                        MessageBox.Show(dir.Root + " Drive is not Exist!! Change RecipePath C:\\Recipe");
                        m_strPath = "C:\\Recipe";
                        dir = new DirectoryInfo(m_strPath);
                    }
                    dir.Create();
                }

                FileInfo[] files = dir.GetFiles("*.ezEFEM");
                foreach (FileInfo file in files) {
                    JobOpen(file.FullName, false);
                    string strBinCode = "";
                    RecipeData dat = new RecipeData();
                    dat.m_strModel = m_sRecipe;
                    dat.m_strSize = m_wafer.m_eSize.ToString();
                    dat.m_strAngle = m_fAngleAlign.ToString();
                    dat.m_strMark = "";
                    dat.m_strDateTime = m_strModifyTime;
                    for (int n = 0; n < m_bUseCode.Length; n++) // ing 170412 for MSB
                    {
                        if (m_bUseCode[n]) {
                            strBinCode += (n + 1).ToString() + " ,";
                        }
                    }
                    if (strBinCode.Length > 2) strBinCode = strBinCode.Remove(strBinCode.Length - 2, 2);
                    dat.m_strBinCode = strBinCode;
                    m_aList.Add(dat);
                }

                if (m_strFile != "") {
                    JobOpen(m_strFile);
                    RunGrid(eGrid.eInit);
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }

            FileSearch();
            if (m_auto.m_strWork == "MSB") {
                WriteBinCode();
            }
        }

        public void FileSearch()
        {
            try 
            {
                listViewRecipe.Items.Clear();
                int n = 0;
                DirectoryInfo dir = new DirectoryInfo(m_strPath);
                if (dir.Exists == false)
                {
                    if (dir.Root.Exists == false)
                    {
                        MessageBox.Show(dir.Root + " Drive is not Exist!! Change RecipePath C:\\Recipe");
                        m_strPath = "C:\\Recipe";
                        dir = new DirectoryInfo(m_strPath);
                    }
                    dir.Create();
                }
                FileInfo[] files = dir.GetFiles("*.ezEFEM");
                m_sRecipes = new string[files.Length];
                foreach (FileInfo file in files)
                {
                    JobOpen(file.FullName, false);

                    m_sRecipes[n] = m_sRecipe;
                    n++;
                }

                foreach (RecipeData dat in m_aList) 
                {
                    if (textBoxNameFilter.Text == "" || dat.m_strModel.ToLower().Contains(textBoxNameFilter.Text.ToLower()) == true) 
                    {
                        ListViewItem item = new ListViewItem(dat.m_strModel);
                        item.SubItems.Add(dat.m_strSize);    //size
                        item.SubItems.Add(dat.m_strAngle);    //angle
                        item.SubItems.Add(dat.m_strMark);    //Mark
                        item.SubItems.Add(dat.m_strDateTime);    //Datetime
                        listViewRecipe.Items.Add(item);
                    }
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            string s = "mi00104_11111";
            s.Substring(0, s.IndexOf("_"));
            IsRecipeExist("DAYLY_DAYLY");
            ReadRecipeDate();
            this.Invalidate();
        }

        private void btnDelete_Click(object sender, EventArgs e) // BHJ 190826 add quick fix
        {
            if (listViewRecipe.SelectedIndices.Count > 1) return;
            if (listViewRecipe.SelectedIndices.Count > 0) m_sRecipe = listViewRecipe.FocusedItem.Text;
            else
            {
                MessageBox.Show("No Recipe Selected", m_id,MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                return;
            }
            if (DialogResult.Yes == MessageBox.Show(string.Format("Delete this Recipe?\n\n{0}", m_sRecipe), m_id, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                File.Delete(GetFileName(m_sRecipe));
                ReadRecipeDate();
                this.Invalidate();
            }
        }

        private void textBoxNameFilter_TextChanged(object sender, EventArgs e)
        {
            FileSearch();
            this.Invalidate();
        }

        private void listViewRecipe_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (listViewRecipe.Sorting == SortOrder.Ascending) {
                listViewRecipe.Sorting = SortOrder.Descending;
            }
            else {
                listViewRecipe.Sorting = SortOrder.Ascending;
            }
        }

        private void Recipe_Mom_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        public void CheckRsp_Server()                           //181226 SDH ADD
        {
            if (m_bUseRAC)
            {
                DirectoryInfo di_Server = null;
                DirectoryInfo di_EQP = null;

                Directory.CreateDirectory(m_strRACPath);
                if (Directory.Exists(m_strRACPath))
                {
                    di_Server = new DirectoryInfo(m_strRACPath);
                }

                Directory.CreateDirectory(m_strPath);
                if (Directory.Exists(m_strPath))
                {
                    di_EQP = new DirectoryInfo(m_strPath);
                }

                foreach (var item_server in di_Server.GetFiles())
                {
                    bool bRspCheck = false;
                    var Rsp_Server = item_server.ToString().Split('.');
                    foreach (var item_eqp in di_EQP.GetFiles())
                    {
                        var Rsp_EQP = item_eqp.ToString().Split('.');

                        if (Rsp_Server[0] == Rsp_EQP[0])
                        {
                            bRspCheck = true;
                            break;
                        }
                        else
                        {
                            bRspCheck = false;
                        }
                    }
                    if (!bRspCheck)
                    {
                        MakeRsp_Auto(Rsp_Server[0]);
                        FileSearch();
                        this.Invalidate();
                    }
                }

            }
        }

        public void MakeRsp_Auto(string strRsp)
        {
            var strName = strRsp.Split('.');
            m_strFile = GetFileName(strName[0]);
            ezJob job = new ezJob(m_strFile, true);
            RunRAC(strName[0], eGrid.eJobSave, job);
            m_work.JobSave(job);
            job.Write(m_id, "DateTime", DateTime.Now.ToString("yy/MM/dd HH:mm:ss"));
            job.Close();
            this.Invalidate();
            m_log.m_reg.Write("Recipe_Job", strRsp);
        }

        public string GetRACVal_str(string Rsp, string sVal)
        {
            string str = "";
            string sline = "";
            string sPath = m_strRACPath + "\\" + Rsp + ".txt";
            StreamReader sReader = new StreamReader(sPath);
            while ((sline = sReader.ReadLine()) != null)
            {
                var sVal_RAC = sline.Split(',');
                if (sVal == sVal_RAC[0])
                {
                    str = sVal_RAC[1];
                    break;
                }
            }
            return str;
        }
        public double GetRACVal_Num(string Rsp, string sVal)
        {
            double dRst = 0.0;
            string sline = "";
            string sPath = m_strRACPath + "\\" + Rsp + ".txt";
            StreamReader sReader = new StreamReader(sPath);
            while ((sline = sReader.ReadLine()) != null)
            {
                var sVal_RAC = sline.Split(',');
                if (sVal == sVal_RAC[0])
                {
                    dRst = Convert.ToDouble(sVal_RAC[1]);
                    break;
                }
            }
            return dRst;
        }
        private void timerEnable_Tick(object sender, EventArgs e)
        {
            CheckRsp_Server();
            if (m_auto.ClassWork().IsRun()) {
                
                //listViewRecipe.Enabled = false;
                grid_Recipe.Enabled = false;
            }
            else {
                //listViewRecipe.Enabled = true;
                grid_Recipe.Enabled = true;
            }
        }

        public string GetSelectedRecipeName()
        {
            if (listViewRecipe.Items.Count > 0) {
                ListView.SelectedListViewItemCollection selectedItems = listViewRecipe.SelectedItems;
                if (selectedItems.Count > 0) {
                    ListViewItem item = selectedItems[0];

                    string recipeName = item.Text;

                    return recipeName;
                }

                return null;

            }

            return null;
        }

        public bool WriteBinCode()
        {
            try {

                DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(m_strBinPath));
                if (dir.Exists == false) {
                    if (dir.Root.Exists == false) {
                        MessageBox.Show(dir.Root + " Drive is not Exist!! Change binCode Path");
                        return true;
                    }
                    dir.Create();
                }

                StreamWriter sw;
                sw = new StreamWriter(new FileStream(m_strBinPath, FileMode.Create));
                if (sw == null) return true;
                sw.WriteLine("[BINCODE]");
                foreach (RecipeData data in m_aList) {
                    sw.WriteLine(data.m_strModel + "=" + data.m_strBinCode);
                }
                sw.Close();
            }
            catch {
                m_log.Popup("BinCode Text File Error !!");
                return true;
            }

            return false;
        }

        public virtual void CreateDefaltRCP(string strNewRcp) { }

        MenuItem m_itemEdit = new MenuItem("Edit Select Recipe");
        ListBox m_listRecipeVal = new ListBox();
        Form m_formEdit = new Form();
        TextBox m_textBox = new TextBox();
        Button m_buttonSave = new Button();
        bool[] m_bSelected;

        private void InitEditForm()
        {
            m_formEdit.Controls.Clear();
            m_listRecipeVal.Items.Clear();
            m_formEdit.Text = m_formEdit.Name = "Edit Recipe Window";
            m_textBox.Text = "";
            m_buttonSave.Text = "Recipe Save";
            m_formEdit.Size = new Size(512, 512);
            m_listRecipeVal.Size = new Size(200, 480);
            m_textBox.Size = new Size(100, 20);
            m_buttonSave.Size = new Size(100, 20);
            m_listRecipeVal.Location = new Point(0, 0);
            m_textBox.Location = new Point(215, 10);
            m_buttonSave.Location = new Point(215, 40);
            m_formEdit.Controls.Add(m_listRecipeVal);
            m_formEdit.Controls.Add(m_textBox);
            m_formEdit.Controls.Add(m_buttonSave);
            for (int n = 0; n < grid_Recipe.Item.Count; n++)
            {
                m_listRecipeVal.Items.Add(grid_Recipe.Item[n].Category + "_" + grid_Recipe.Item[n].Name);
            }
            m_buttonSave.Click += new System.EventHandler(SaveValue);
            m_itemEdit.Click += new System.EventHandler(ShowEditForm);
            m_formEdit.FormClosing += new System.Windows.Forms.FormClosingEventHandler(HideEditForm);
        }

        private void listViewRecipe_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listViewRecipe.SelectedItems.Count == 0) return;
                m_bSelected = new bool[listViewRecipe.Items.Count];
                for (int n = 0; n < listViewRecipe.Items.Count; n++)
                {
                    m_bSelected[n] = listViewRecipe.Items[n].Selected;
                }
                ContextMenu menu = new ContextMenu();
                menu.MenuItems.Add(m_itemEdit);
                menu.Show(listViewRecipe, new Point(e.X, e.Y));
            }
        }
        private void HideEditForm(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            m_formEdit.Hide(); 
        }
        private void ShowEditForm(object sender, EventArgs e)
        {
            m_formEdit.ShowDialog();
        }

        private void SaveValue(object sender, EventArgs e)
        {
            if (m_textBox.Text == "") return;
            int nStart, nEnd, nEndLine, nLines;
            string strFile, strAllLines, strTemp, strInsert, strType;
            for (int n = 0; n < listViewRecipe.Items.Count; n++)
            {
                if (m_bSelected[n])
                {
                    strFile = GetFileName(listViewRecipe.Items[n].Text);
                    try
                    {
                        StreamReader sr = new StreamReader(strFile);
                        strAllLines = sr.ReadToEnd();
                        sr.Close();
                        try
                        {
                            if (grid_Recipe.Item[m_listRecipeVal.SelectedIndex].Type == typeof(int))
                            {
                                strType = "int";
                                Convert.ToInt32(m_textBox.Text);
                            }
                            else if (grid_Recipe.Item[m_listRecipeVal.SelectedIndex].Type == typeof(double))
                            {
                                strType = "double";
                                Convert.ToDouble(m_textBox.Text);                            
                            }
                            else if (grid_Recipe.Item[m_listRecipeVal.SelectedIndex].Type == typeof(bool))
                            {
                                strType = "bool";
                                Convert.ToBoolean(m_textBox.Text);
                            }
                            else strType = "string";
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("값의 형태가 맞지 않습니다. 다시 입력해주세요 : " + ex.Message);
                            return;
                        }
                        nStart = strAllLines.IndexOf(grid_Recipe.Item[m_listRecipeVal.SelectedIndex].Category.Substring(3) + grid_Recipe.Item[m_listRecipeVal.SelectedIndex].Name + ",");
                        nEnd = strAllLines.IndexOf(',', nStart + (grid_Recipe.Item[m_listRecipeVal.SelectedIndex].Category.Substring(3) + grid_Recipe.Item[m_listRecipeVal.SelectedIndex].Name + ",").Length);
                        nEndLine = strAllLines.IndexOf('\r', nEnd);
                        if (nStart < 0)
                        {
                            nLines = 0;
                            for (int nReciveval = 0; nReciveval < m_listRecipeVal.SelectedIndex; nReciveval++)
                            {
                                try
                                {
                                    nLines = strAllLines.IndexOf('\n', nLines + 1);
                                }
                                catch
                                {
                                    strInsert = strAllLines + m_id + ',' + grid_Recipe.Item[m_listRecipeVal.SelectedIndex].Category.Substring(3) + grid_Recipe.Item[m_listRecipeVal.SelectedIndex].Name + "," + strType + "," + m_textBox.Text + "\r\n";
                                    StreamWriter swThrow = new StreamWriter(strFile);
                                    swThrow.Write(strInsert);
                                    swThrow.Close();
                                    return;
                                }
                            }
                            if (nLines < 0)
                            {
                                strInsert = strAllLines + m_id + ',' + grid_Recipe.Item[m_listRecipeVal.SelectedIndex].Category.Substring(3) + grid_Recipe.Item[m_listRecipeVal.SelectedIndex].Name + "," + strType + "," + m_textBox.Text + "\r\n";
                            }
                            else strInsert = strAllLines.Insert(nLines + 1, m_id + ',' + grid_Recipe.Item[m_listRecipeVal.SelectedIndex].Category.Substring(3) + grid_Recipe.Item[m_listRecipeVal.SelectedIndex].Name + "," + strType + "," + m_textBox.Text + "\r\n");
                        }
                        else
                        {
                            strTemp = strAllLines.Remove(nEnd + 1, nEndLine - nEnd - 1);
                            strInsert = strTemp.Insert(nEnd + 1, m_textBox.Text);
                        }
                        StreamWriter sw = new StreamWriter(strFile);
                        sw.Write(strInsert);
                        sw.Close();
                        Invalidate();
                        MessageBox.Show(strFile + " : Job Save Success");
                    }
                    catch
                    {
                        MessageBox.Show(strFile + " : Job Save Faill");
                        return;                        
                    }                    
                }
            }
        }

        private void listViewRecipe_DoubleClick(object sender, EventArgs e) // BHJ 190319 add
        {
            ListView lvr = (ListView)sender;
            if (!m_auto.ClassWork().IsRun())
            {
                if (lvr.FocusedItem == null) return;
                if (m_auto.ClassWork().IsRun()) return;
                m_sRecipe = lvr.FocusedItem.Text;
                m_strFile = GetFileName(m_sRecipe);
                JobOpen(m_strFile);
                RunGrid(eGrid.eInit);
                m_log.m_reg.Write("Recipe_Job", m_strFile);
                m_grid.m_grid.Show();
            }
        }

        private void listViewRecipe_KeyDown(object sender, KeyEventArgs e) // BHJ 190319 add
        {
            ListView lvr = (ListView)sender;
            if (e.KeyCode == Keys.A && e.Control)
            {
                foreach (ListViewItem lvItem in lvr.Items)
                    lvItem.Selected = true;
            }
        }

        private void listViewRecipe_Click(object sender, EventArgs e) // BHJ 190319 add
        {
            ListView lvr = (ListView)sender;
            if (!m_auto.ClassWork().IsRun())
            {
                if (lvr.FocusedItem == null) return;
                if (m_auto.ClassWork().IsRun()) return;
                m_grid.m_grid.Hide();
            }
        }
    }

    class RecipeData
    {
        public string m_strModel;
        public string m_strSize;
        public string m_strAngle;
        public string m_strMark;
        public string m_strDateTime;
        public string m_strBinCode;
    }
}