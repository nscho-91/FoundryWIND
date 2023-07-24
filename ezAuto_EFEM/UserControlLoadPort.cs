using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ezAutoMom;
using ezTools;
using System.Reflection;
using System.Threading;

namespace ezAuto_EFEM
{

    public partial class UserControlLoadPort : UserControl
    {
        HW_LoadPort_Mom m_LoadPort = null;
        Info_Carrier m_carrier = null;
        Auto_Mom m_auto = null;
        Work_Mom m_work = null;
        XGem300_Mom m_xGem = null;
        //OHT_EFEM m_OHT = null;
        Log m_log = null;
        int m_nLP = 0;
        string m_strName;
        ezStopWatch m_swUI = new ezStopWatch();


        private eReloadProcess m_eReloadCycle = eReloadProcess.None;

        public UserControlLoadPort(int n, Auto_Mom auto)
        {
            InitializeComponent();
            m_nLP = n;
            m_auto = auto;
            m_work = m_auto.ClassWork();
            m_xGem = m_auto.ClassXGem();
            m_strName = "LDPort" + (n + 1).ToString();
            m_LoadPort = m_auto.ClassHandler().ClassLoadPort(n);
            m_carrier = m_LoadPort.m_infoCarrier;
            m_log = new Log("UI_Loadport" + n.ToString(), m_auto.ClassLogView(), "UI");
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            m_swUI.Start();

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

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {
            #region WaferInfo
            int nMargin = 3;
            int nH = splitContainerMain.Panel2.Height - nMargin;
            int nW = splitContainerMain.Panel2.Width - nMargin;
            int nHBox = nH / m_carrier.m_lWafer;

            using (BufferedGraphics bufferedgraphic = BufferedGraphicsManager.Current.Allocate(e.Graphics, this.ClientRectangle))
            {
                Graphics G = bufferedgraphic.Graphics;
                G.Clear(this.BackColor);
                G.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                G.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);

                Font FontWafer = null;
                if (nHBox * 0.6 > 15)
                    FontWafer = new Font(this.Font.Name, 10, FontStyle.Regular, GraphicsUnit.Pixel);
                else
                    FontWafer = new Font(this.Font.Name, (int)(nHBox * 0.6), FontStyle.Regular, GraphicsUnit.Pixel);
                int nWidthSlotNum = GetTxtWidth(G, String.Format("{0,3}", m_carrier.m_lWafer), FontWafer);
                int nWidthState = GetTxtWidth(G, Info_Wafer.eState.Select.ToString(), FontWafer);
                int nVisionTime = GetTxtWidth(G, "00000 Sec", FontWafer);
                int nWidthYield = GetTxtWidth(G, "100.00%", FontWafer);
                int nWidthWaferID = nW - nWidthYield - nWidthSlotNum - nWidthState - nVisionTime;


                if (m_auto.ClassHandler().ClassCarrier(m_nLP).m_eSlotType == Info_Carrier.eSlotType.ATI)
                {
                    for (int n = 0; n < m_carrier.m_lWafer; n++)
                    {
                        Info_Wafer wafer = m_carrier.m_infoWafer[n];

                        Rectangle rtSlot = new Rectangle(0, nH - (nHBox * (n + 1)), nWidthSlotNum, nHBox);
                        Rectangle rtState = new Rectangle(rtSlot.Right, rtSlot.Top, nWidthState, nHBox);
                        //Rectangle rtWaferID = new Rectangle(rtState.Right, rtState.Top, nWidthWaferID, nHBox);
                        Rectangle rtVsTime = new Rectangle(rtState.Right, rtState.Top, nVisionTime, nHBox);
                        Rectangle rtYield = new Rectangle(rtVsTime.Right, rtVsTime.Top, nWidthYield, nHBox);

                        string strSlotNum = String.Format("{0,3}", wafer.m_nSlot);
                        string strYield = "";
                        if (wafer.m_nTotal != 0)
                        {
                            if (wafer.m_nGood == -1)
                                strYield = "A/F";
                            else
                                strYield = ((double)wafer.m_nTotal - (double)wafer.m_nGood).ToString();
                        //    strYield = String.Format("{0:N2}%", (double)wafer.m_nGood * 100.0 / (double)wafer.m_nTotal);
                        }
                        else
                        {
                            strYield = "-";
                        }

                        if (wafer.Locate == Info_Wafer.eLocate.Vision)
                        {
                            if (wafer.m_nProgressCurrent != wafer.m_nProgressTotal)
                            {
                                SetProgressBar(wafer);
                            }
                            else if (wafer.m_nProgressCurrent != 0 && wafer.m_nProgressCurrent == wafer.m_nProgressTotal)
                            {
                                wafer.m_nProgressCurrent = 0;
                            }
                        }
                        else
                        {
                            wafer.m_nProgressCurrent = 0;
                            progressBar1.Value = 0;
                        }
                        DrawStringFillRect(G, strSlotNum, FontWafer, rtSlot, Brushes.FloralWhite);
                        DrawStringFillRect(G, wafer.State.ToString(), FontWafer, rtState, GetStateBrush(wafer.State));
                        //DrawStringFillRect(G, wafer.WAFERID, FontWafer, rtWaferID, Brushes.FloralWhite);
                        if (wafer.m_bVSAlignFail) DrawStringFillRect(G, "VS A/F", FontWafer, rtState, GetStateBrush(Info_Wafer.eState.Bad)); 
                        if (wafer.m_bPreAlignfail) DrawStringFillRect(G, "A/F", FontWafer, rtState, GetStateBrush(Info_Wafer.eState.Bad)); 
                        DrawStringFillRect(G, wafer.GetVSTime() + " Sec", FontWafer, rtVsTime, Brushes.LightGoldenrodYellow);
                        DrawStringFillRect(G, strYield, FontWafer, rtYield, Brushes.GhostWhite);
                    }
                }
                else if (m_auto.ClassHandler().ClassCarrier(m_nLP).m_eSlotType == Info_Carrier.eSlotType.Sanken)
                {
                    for (int n = m_carrier.m_lWafer - 1; n >= 0; n--)
                    {
                        Info_Wafer wafer = m_carrier.m_infoWafer[n];

                        Rectangle rtSlot = new Rectangle(0, nH - (nHBox * (n + 1)), nWidthSlotNum, nHBox);
                        Rectangle rtState = new Rectangle(rtSlot.Right, rtSlot.Top, nWidthState, nHBox);
                        Rectangle rtWaferID = new Rectangle(rtState.Right, rtState.Top, nWidthWaferID, nHBox);
                        Rectangle rtYield = new Rectangle(rtWaferID.Right, rtWaferID.Top, nWidthYield, nHBox);

                        string strSlotNum = String.Format("{0,3}", wafer.m_nSlot);
                        string strYield = "";
                        if (wafer.m_nTotal != 0)
                        {
                            if (wafer.m_nGood == -1)
                                strYield = "A/F";
                            else
                                strYield = String.Format("{0:N2}%", (double)wafer.m_nGood * 100.0 / (double)wafer.m_nTotal);
                        }
                        else
                        {
                            strYield = "-";
                        }

                        if (wafer.Locate == Info_Wafer.eLocate.Vision)
                        {
                            if (wafer.m_nProgressCurrent != wafer.m_nProgressTotal)
                            {
                                SetProgressBar(wafer);
                            }
                            else if (wafer.m_nProgressCurrent != 0 && wafer.m_nProgressCurrent == wafer.m_nProgressTotal)
                            {
                                wafer.m_nProgressCurrent = 0;
                            }
                        }
                        else
                        {
                            wafer.m_nProgressCurrent = 0;
                        }

                        DrawStringFillRect(G, strSlotNum, FontWafer, rtSlot, Brushes.FloralWhite);
                        DrawStringFillRect(G, wafer.State.ToString(), FontWafer, rtState, GetStateBrush(wafer.State));
                        if (wafer.m_bVSAlignFail) DrawStringFillRect(G, "VS A/F", FontWafer, rtState, GetStateBrush(Info_Wafer.eState.Bad)); 
                        if (wafer.m_bPreAlignfail) DrawStringFillRect(G, "A/F", FontWafer, rtState, GetStateBrush(Info_Wafer.eState.Bad)); 
                        DrawStringFillRect(G, wafer.WAFERID, FontWafer, rtWaferID, Brushes.FloralWhite);
                        DrawStringFillRect(G, strYield, FontWafer, rtYield, Brushes.GhostWhite);
                    }
                }
                bufferedgraphic.Render(e.Graphics);
            }
            #endregion
        }

        public void SetProgressBar(Info_Wafer infoWafer)
        {
            if (infoWafer == null) { progressBar1.Value = 0; return; }
            if (infoWafer.Locate != Info_Wafer.eLocate.Vision) { progressBar1.Value = 0; return; }
            if (infoWafer.m_nProgressTotal == 0) { progressBar1.Value = 0; return; }
            progressBar1.Value = (int)(100.0 * infoWafer.m_nProgressCurrent / infoWafer.m_nProgressTotal);
        }

        private void splitContainerMain_Panel1_Paint(object sender, PaintEventArgs e)
        {
            #region Info
            int nMargin = 3;
            int nH = splitContainerMain.Panel1.Height - nMargin;
            int nW = splitContainerMain.Panel1.Width - nMargin;
            int nFontSize = 20;

            using (BufferedGraphics bufferedgraphic = BufferedGraphicsManager.Current.Allocate(e.Graphics, this.ClientRectangle))
            {
                Graphics G = bufferedgraphic.Graphics;
                G.Clear(this.BackColor);
                G.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                G.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);

                Font FontInfo = new Font(this.Font.Name, nFontSize, FontStyle.Regular, GraphicsUnit.Pixel);
                int nWidthSubject = 100;
                int nWidthValue = nW - nWidthSubject;

                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                //if (checkBoxOHTAutoManual.Visible == false)
                //{
                //    checkBoxOHTAutoManual.Height = 0;
                //}
                //Rectangle rt0 = new Rectangle(0, checkBoxOHTAutoManual.Height, nWidthSubject + nWidthValue, nFontSize);
                Rectangle rt0 = new Rectangle(0, 0, nWidthSubject + nWidthValue, nFontSize);
                //Rectangle rt00 = new Rectangle(0, checkBoxOHTAutoManual.Height, nWidthSubject, nFontSize);
                Rectangle rt00 = new Rectangle(0, 0, nWidthSubject, nFontSize);
                Rectangle rt10 = new Rectangle(rt00.Left, rt00.Bottom, nWidthSubject, nFontSize);
                Rectangle rt20 = new Rectangle(rt10.Left, rt10.Bottom, nWidthSubject, nFontSize);
                Rectangle rt30 = new Rectangle(rt20.Left, rt20.Bottom, nWidthSubject, nFontSize);

                Rectangle rt01 = new Rectangle(rt00.Right, rt00.Top, nWidthValue, nFontSize);
                Rectangle rt11 = new Rectangle(rt01.Left, rt01.Bottom, nWidthValue, nFontSize);
                Rectangle rt21 = new Rectangle(rt11.Left, rt11.Bottom, nWidthValue, nFontSize);
                Rectangle rt31 = new Rectangle(rt21.Left, rt21.Bottom, nWidthValue, nFontSize);

                DrawStringFillRect(G, "Carrier ID", FontInfo, rt10, Brushes.FloralWhite);
                DrawStringFillRect(G, "Lot ID", FontInfo, rt20, Brushes.FloralWhite);
                DrawStringFillRect(G, "Recipe", FontInfo, rt30, Brushes.FloralWhite);

                DrawStringFillRect(G, m_strName, FontInfo, rt0, Brushes.DimGray);
                DrawStringFillRect(G, m_carrier.m_strCarrierID, FontInfo, rt11, Brushes.FloralWhite);
                DrawStringFillRect(G, m_carrier.m_strLotID, FontInfo, rt21, Brushes.FloralWhite);
                DrawStringFillRect(G, m_carrier.m_strRecipe, FontInfo, rt31, Brushes.FloralWhite);

                bufferedgraphic.Render(e.Graphics);
            }
            #endregion
        }

        private void DrawOHT()
        {
            //if (m_LoadPort.m_bUseOHT == false)
            //{
            //    checkBoxOHTAutoManual.Visible = false;
            //}
            //else
            //{
            //    checkBoxOHTAutoManual.Visible = true;
            //}

            //if (m_LoadPort.m_bOHTAuto == true)
            //{
            //    checkBoxOHTAutoManual.Text = "Auto";
            //    checkBoxOHTAutoManual.BackColor = Color.LawnGreen;
            //}
            //else
            //{
            //    checkBoxOHTAutoManual.Text = "Manual";
            //    checkBoxOHTAutoManual.BackColor = Color.PaleVioletRed;
            //}
        }

        private void DrawLoadButton()
        {
            bool bEnabled = false;
            string str = "Init";
            Color color = SystemColors.Control;
            switch (m_LoadPort.GetState())
            {
                case HW_LoadPort_Mom.eState.Init:
                    if (!m_LoadPort.IsCoverClose())
                    {
                        bEnabled = false;
                        str = "Door Open";
                        if (m_swUI.Check() % 1000 > 500)
                            color = SystemColors.Control;
                        else
                            color = Color.IndianRed;
                    }
                    else
                    {
                        bEnabled = false;
                        str = "Init";
                    }
                    break;
                case HW_LoadPort_Mom.eState.Home:
                    bEnabled = false;
                    str = "Home";
                    break;
                case HW_LoadPort_Mom.eState.Ready:
                    if (m_LoadPort.CheckCarrier() == Wafer_Size.eSize.Empty)
                    {
                        bEnabled = false;
                        str = "Ready";
                        if (m_auto.m_bXgemUse && m_xGem.IsOnline()) // ing 170227
                        {
                            if (m_xGem.m_aXGem300Carrier[m_nLP].m_eLPTransfer != XGem300Carrier.eLPTransfer.ReadyToLoad && m_xGem.m_aXGem300Carrier[m_nLP].m_eLPTransfer != XGem300Carrier.eLPTransfer.OutOfService)
                            {
                                m_auto.ClassXGem().SetLPCarrierOnOff(m_nLP, false);
                                m_auto.ClassXGem().SetLPPresentSensor(m_nLP, false);
                                m_auto.ClassXGem().SetCMSTransferState(XGem300Carrier.eLPTransfer.ReadyToLoad, m_nLP);
                            }
                        }
                    }
                    else if (!m_LoadPort.IsCoverClose())
                    {
                        bEnabled = false;
                        str = "Door Open";
                        if (m_swUI.Check() % 1000 > 500)
                            color = SystemColors.Control;
                        else
                            color = Color.IndianRed;
                    }
                    else if (m_auto.ClassWork().GetState() == eWorkRun.Home)
                    {

                    }
                    else
                    {
                        bEnabled = true;        //sdh
                        str = "Load";
                        color = Color.LightGreen;
                        if (!m_xGem.IsOnline() && CheckState_OtherLP(m_nLP, ">", HW_LoadPort_Mom.eState.Placed))
                        {
                            bEnabled = false;
                        }
                    }
                    break;
                case HW_LoadPort_Mom.eState.CSTIDRead: bEnabled = false;
                    str = "RFID READ";
                    break;
                case HW_LoadPort_Mom.eState.RunDocking: bEnabled = false;
                    str = "Docked";
                    break;
                case HW_LoadPort_Mom.eState.Placed:
                    bEnabled = true;
                    str = "Load";
                    color = Color.LightGreen;
                    if (!m_xGem.IsOnline() && CheckState_OtherLP(m_nLP, ">", HW_LoadPort_Mom.eState.Placed))
                    {
                        bEnabled = false;
                    }
                    break;
                case HW_LoadPort_Mom.eState.RunLoad:
                    bEnabled = false;
                    str = "Loading";
                    break;
                case HW_LoadPort_Mom.eState.LoadDone:
                    bEnabled = false;
                    str = "Start";
                    break;
                case HW_LoadPort_Mom.eState.RunUnload:
                    bEnabled = false;
                    str = "UnLoading";
                    break;
                case HW_LoadPort_Mom.eState.UnloadDone:
                    bEnabled = false;
                    str = "Done";
                    color = Color.RoyalBlue;
                    break;
                case HW_LoadPort_Mom.eState.WaitProcessEnd:
                    bEnabled = false;
                    str = "Wait Process";
                    break;
                case HW_LoadPort_Mom.eState.Error:
                    bEnabled = false;
                    str = "Error";
                    color = Color.Tomato;
                    break;
            }
            buttonLoad.Enabled = bEnabled;
            buttonLoad.Text = str;
            buttonLoad.BackColor = color;

            if (m_auto.m_bXgemUse)
            {
                if (m_auto.ClassXGem().m_aXGem300Carrier != null &&  m_xGem.m_aXGem300Carrier[m_nLP].m_ePortAccess == XGem300Carrier.ePortAccess.Auto)
                {
                    btnAccess.Text = "Auto Mode";
                    btnAccess.BackColor = Color.LightGreen;
                }
                else
                {
                    btnAccess.Text = "Manual Mode";
                    btnAccess.BackColor = Color.LemonChiffon;
                }
            }
            if (m_auto.m_bXgemUse)
            {
                if (m_auto.ClassXGem().m_aXGem300Carrier != null && m_xGem.m_aXGem300Carrier[m_nLP].m_eLPTransfer == XGem300Carrier.eLPTransfer.OutOfService)
                {
                    brnService.Text = "Out of Service";
                    brnService.BackColor = Color.OrangeRed;
                }
                else
                {
                    brnService.Text = "In Service";
                    brnService.BackColor = Color.LightGreen;
                }
            }

            if(m_auto.ClassHandler().ClassLoadPort(m_nLP).GetState() == HW_LoadPort_Mom.eState.RunLoad)
            {
                btnAccess.Enabled = false;
                btnReload.Enabled = false;
                btnUploadReq.Enabled = false;
                brnService.Enabled = false;
            }
            else
            {
                btnAccess.Enabled = true;
                btnUploadReq.Enabled = true;
                brnService.Enabled = true;

                if (CheckState_OtherLP( m_nLP, "==", HW_LoadPort_Mom.eState.RunLoad))
                {
                    btnReload.Enabled = false;
                }
                else
                {
                    btnReload.Enabled = true;
                }
            }

          

            if (m_auto.ClassHandler().ClassLoadPort(m_nLP).GetState() == HW_LoadPort_Mom.eState.WaitProcessEnd)
            {
                btnUploadReq.Text = "JobCancel && Unload Request";
            }
            else
            {
                btnUploadReq.Text = "Unload Request";
            }

        }

        private void DrawProgress()
        {

        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
            DrawOHT();
            DrawLoadButton();
            splitContainerMain.Panel2.Invalidate(true);
            splitContainerMain.Panel1.Invalidate(true);
            CheckShowManualJob();
            ReloadProcess();

        }
        public enum eReloadProcess
        {
            None = 0,
            SetTransferBlock,
            InitLPInfo,
            DelLPInfo,

        }

        private void ReloadProcess()
        {
            switch (m_eReloadCycle)
            {
                case eReloadProcess.SetTransferBlock:
                    m_xGem.SetCMSTransferState(XGem300Carrier.eLPTransfer.TransferBlocked, m_nLP);
                    m_log.Add("TransferState Change - TransferBlock");
                    m_eReloadCycle = eReloadProcess.InitLPInfo;
                    break;
                case eReloadProcess.InitLPInfo:
                    m_xGem.SetLPInfo(m_nLP, XGem300Carrier.eLPTransfer.TransferBlocked, m_xGem.m_aXGem300Carrier[m_nLP].m_ePortAccess, XGem300Carrier.ePortReservationState.NotReserved, XGem300Carrier.ePortAssocitionState.NotAssociated, "");
                    m_log.Add("LPInfo Init");
                    m_eReloadCycle = eReloadProcess.DelLPInfo;
                    break;
                case eReloadProcess.DelLPInfo:
                    m_xGem.DeleteAllLPInfo();
                    m_log.Add("Delete LP Info");
                    m_auto.ClassHandler().ClassLoadPort(m_nLP).SetAutoLoading();
                    m_LoadPort.m_bReload = true;
                    m_eReloadCycle = eReloadProcess.None;
                    break;
                default:
                    break;
            }
        }

        private void checkBoxOHTAutoManual_MouseUp(object sender, MouseEventArgs e)
        {
            string Msg;
            if (m_LoadPort.m_bOHTAuto == true)
            {
                Msg = "Change Manual Mode?";
            }
            else
            {
                Msg = "Change Auto Mode?";
            }
            if (System.Windows.Forms.MessageBox.Show(Msg, "Port Mode Change.", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }
            m_LoadPort.m_bOHTAuto = !m_LoadPort.m_bOHTAuto;

            DrawOHT();
        }

        public void ManualJob()
        {
            UI_ManualJob_Mom manualjob = (UI_ManualJob_Mom)(m_work.GetManualJob());
            if (manualjob == null) return;

            manualjob.Init("ManualJob", m_auto, m_nLP, m_work.m_bMapDataUse, m_work.m_strFilePath);

            if (m_carrier.m_bRNR == true && !(m_auto.ClassHandler().IsWaferExistInEQP()))
            {
                manualjob.SetRNRMode();
                m_carrier.m_eState = Info_Carrier.eState.Ready;
            }
            else if (m_auto.m_bXgemUse && m_xGem.IsOnline())
            {
                m_xGem.SetLPAssociation(m_nLP, XGem300Carrier.eCarrierAssociationState.IN_ACCESSED);
                manualjob.SetXGemMode(m_nLP);
                m_carrier.m_eState = Info_Carrier.eState.Ready;
            }
            else
            {
                if (!m_carrier.IsWaferOut() && !(m_auto.ClassHandler().IsWaferExistInEQP()))
                {
                    if (manualjob.ShowModal() == DialogResult.OK)
                    {
                        m_carrier.m_eState = Info_Carrier.eState.Ready; //job Start
                    }
                    else
                    {
                        m_carrier.m_eState = Info_Carrier.eState.Done;
                        m_LoadPort.SetUnload();
                    }
                    return;
                }
                m_carrier.m_eState = Info_Carrier.eState.Done;
            }

        }

        private void CheckShowManualJob()
        {
            if (m_carrier.m_eState == Info_Carrier.eState.MapDone)
            {
                if (m_auto.m_bXgemUse && m_xGem.IsOnline())
                {
                    if (m_xGem.GetCJSate(m_nLP) == XGem300Control.eState.Excuting && m_xGem.GetPJState(m_nLP) == XGem300Process.eJobState.Processing && m_auto.ClassHandler().ClassLoadPort(m_nLP).GetState() == HW_LoadPort_Mom.eState.LoadDone)
                    {
                        //_carrier.m_eState = (Info_Carrier.eState)m_xGem.SetSlotMap(m_carrier);
                        m_carrier.m_eState = Info_Carrier.eState.ManualJob;
                        ManualJob();
                    }
                    else if (m_auto.ClassHandler().m_LostLPInfo && m_auto.ClassHandler().ClassLoadPort(m_nLP).IsDoorOpenPos())
                    {
                        m_carrier.m_eState = Info_Carrier.eState.ManualJob;
                        ManualJob();
                    }
                    return;
                }
                else if (m_work.GetState() == eWorkRun.Run)
                {
                    m_carrier.m_eState = Info_Carrier.eState.ManualJob;
                    ManualJob();
                }
            }
            //if (m_auto.ClassXgem300().GetPJ().PRJobState == Gem300.cPJ.ePRJObState.Processing && m_auto.ClassXgem300().GetCJ().CTRLJobState == Gem300.cCJ.eCTRLJobState.Excuting) {
            //    m_Carrier.m_eState = Info_Carrier.eState.ManualJob;
            //    ManualJob();
            //}
        }
        protected bool CheckState_OtherLP(int m_nLPNum, string strCalc, HW_LoadPort_Mom.eState State)
        {
            bool bRst = false;
            int nCount = m_auto.ClassHandler().m_nLoadPort;
            bool[] bCheck = { false, false, false };
            for (int i = 0; i < nCount; i++)
            {
                if (i == m_nLPNum) bCheck[i] = false;
                else
                {
                    if (strCalc == "==") bCheck[i] = m_auto.ClassHandler().ClassLoadPort(i).GetState() == State;
                    else if (strCalc == "!=") bCheck[i] = m_auto.ClassHandler().ClassLoadPort(i).GetState() != State;
                    else if (strCalc == ">") bCheck[i] = m_auto.ClassHandler().ClassLoadPort(i).GetState() > State;
                    else if (strCalc == "<") bCheck[i] = m_auto.ClassHandler().ClassLoadPort(i).GetState() < State;
                    else bCheck[i] = false;

                    if (bCheck[i]) bRst = true;
                }
            }
            return bRst;

        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            if (!(m_LoadPort.IsCoverClose()))
            {
                m_log.Popup("Please Cover Close !!");
                return;
            }
            if (m_work.GetState() != eWorkRun.Run && !m_auto.ClassHandler().IsWaferExistInEQP() && m_auto.ClassHandler().m_bEnableStart)
            {
                m_log.Popup("Please Push Start Button");
                return;
            }

            if (CheckState_OtherLP(m_nLP, "==", HW_LoadPort_Mom.eState.RunLoad))
            {
                MessageBox.Show("다른 로드포트의 로딩이 완료될때 까지 기다린후 다시 시도해 주세요.");
                return;
            }

            if (m_xGem.IsOnline())
            {
                m_auto.ClassXGem().SetLPCarrierOnOff(m_nLP, true);
                m_auto.ClassXGem().SetLPPresentSensor(m_nLP, true);
                m_auto.ClassXGem().SetCMSTransferState(XGem300Carrier.eLPTransfer.TransferBlocked, m_nLP);
            }
            if (m_LoadPort.m_bRFIDUse || m_LoadPort.m_bUseBCR)
                m_LoadPort.SetState(HW_LoadPort_Mom.eState.CSTIDRead);
            else
                m_LoadPort.SetState(HW_LoadPort_Mom.eState.RunLoad);
        }

        private void btnAccess_Click(object sender, EventArgs e)
        {
            //if (m_xGem.IsOnline())
            //{
                switch (m_xGem.m_aXGem300Carrier[m_nLP].m_ePortAccess)
                {
                    case XGem300Carrier.ePortAccess.Manual:
                        if (System.Windows.Forms.MessageBox.Show("Do you Change Auto Mode?", "Program Exit", MessageBoxButtons.YesNo) == DialogResult.Yes)           //170711 SDH ADD
                        {
                            m_xGem.LPAccessModeChange(XGem300Carrier.ePortAccess.Auto, m_xGem.m_aXGem300Carrier[m_nLP].m_sLocID);
                        }
                        break;
                    case XGem300Carrier.ePortAccess.Auto:
                        if (System.Windows.Forms.MessageBox.Show("Do you Change Manual Mode?", "Program Exit", MessageBoxButtons.YesNo) == DialogResult.Yes)         //170711 SDH ADD
                        {
                            m_xGem.LPAccessModeChange(XGem300Carrier.ePortAccess.Manual, m_xGem.m_aXGem300Carrier[m_nLP].m_sLocID);
                        }
                        break;
                }
                m_log.Add("Access Mode Btn Click  " + m_nLP.ToString());
            //}
        }



        private void brnService_Click(object sender, EventArgs e) // ing170223
        {
            m_log.WriteLog("Sequence", "[Button Click] ServiceMode"); //230724 nscho 
            long nReturn, nState = 0;
            string sLocID;
            sLocID = "LP" + (m_nLP + 1).ToString(); // 0 index start
            m_log.Add(sLocID + " Service Btn Click  " + m_nLP.ToString());
             if (m_xGem.m_aXGem300Carrier[m_nLP].m_eLPTransfer == XGem300Carrier.eLPTransfer.OutOfService)
            {
                if (m_LoadPort.IsPlaced()) nState = 1;// TransferBlocked (In Service)
                else nState = 2; // ReadyToLoad (In Service)
                //else if (m_LoadPort.IsPlaced() && !m_LoadPort.IsOpen()) nState = 3; // ReadyToUnload (In Service)
            }
            else
            {
                nState = 0; // Out Of Service
            }
            nReturn = m_xGem.m_XGem300.CMSReqChangeServiceStatus(sLocID, nState);
            if (nReturn == 0)
            {
                m_log.Add("CMSReqChangeServiceStatus - [EQ ==> XGEM] Send Request Change Service Status successfully.");
            }
            else
            {
                m_log.Add("CMSReqChangeServiceStatus - [EQ ==> XGEM] Fail to Request Change Service Status.");
            }
            Thread.Sleep(500);
            if (m_xGem.m_aXGem300Carrier[m_nLP].m_eLPTransfer != XGem300Carrier.eLPTransfer.OutOfService)           //170711 SDH ADD
            {
                m_xGem.LPAccessModeChange(XGem300Carrier.ePortAccess.Manual, m_xGem.m_aXGem300Carrier[m_nLP].m_sLocID);  //170711 SDH ADD
            }
        }
        /**
* @brief UnloadRequest Btn 동작
* @param ojbect sendar : 전달할 정보
* @param EventArgs e : 이벤트
* @return 없음
* @note Patch-notes
* 날짜|작성자|설명|비고
* -|-|-|-
* 2023-07-24|조남수| UnloadRequest 동작 시 로그 추가|-
* @warning 없음
*/
        private void btnUploadReq_Click(object sender, EventArgs e)
        {
            m_log.WriteLog("Sequence", "[Button Click] Upload Request"); //230724 nscho 
            if (!m_xGem.IsAuto(m_nLP))
            {
                m_log.Add("Unload Req Btn Click  " + m_nLP.ToString());
                MessageBox.Show("해당 LoadPort상태가 Manual입니다. Auto상태로 바꾼 후 시도하세요");
                return;
            }
            if (System.Windows.Forms.MessageBox.Show("현재 FOUP에 대해 UnloadRequest를 보내겠습니까?", "Unloadreq", MessageBoxButtons.YesNo) == DialogResult.Yes)         //170711 SDH ADD
            {
                m_log.Popup("Try Unload Req");
                if (!m_LoadPort.IsPlaced()) {
                    m_log.Popup("Cst가 놓여있지 않습니다.");
                    return;
                }
                if (m_LoadPort.GetState() == HW_LoadPort_Mom.eState.RunDocking) {
                    m_log.Popup("LoadPort를 Undocking 합니다.");
                    if (m_LoadPort.RunUndocking() != eHWResult.OK) {
                        m_log.Popup("Undocking Fail !!");
                        return;
                    }
                    m_log.Popup("Undocking Complate");
                    m_LoadPort.SetState(HW_LoadPort_Mom.eState.Ready);
                    ((EFEM)m_auto).m_OHT.IsHo_AVBL(m_nLP);
                }
                else if (m_LoadPort.GetState() != HW_LoadPort_Mom.eState.Ready && m_LoadPort.GetState() != HW_LoadPort_Mom.eState.WaitProcessEnd)
                {
                    m_log.Popup("Loadport의 상태를 확인하여 주세요");
                    return;
                }

                m_LoadPort.m_eUploadCycle = HW_LoadPort_Mom.eUploadCycle.ReadCarrierID;
                m_log.Add("Unload Req Btn Click  " + m_nLP.ToString());
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("현재 FOUP을 Reload 하시겠습니까?", "Reload", MessageBoxButtons.YesNo) == DialogResult.Yes)         //170711 SDH ADD
            {
                if (m_eReloadCycle == 0)
                    m_eReloadCycle = eReloadProcess.SetTransferBlock;
                else
                    m_log.Popup("Reload가 진행중입니다.");
            }
        }
    }

    public class DoubleBufferSplitPanel : SplitContainer
    {
        public DoubleBufferSplitPanel()
        {
            MethodInfo mi = typeof(Control).GetMethod("SetStyle", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] args = new object[] { ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true };
            mi.Invoke(this.Panel1, args);
            mi.Invoke(this.Panel2, args);
        }
    }
}
