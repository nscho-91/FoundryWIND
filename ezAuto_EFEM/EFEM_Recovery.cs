using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ezAutoMom;
using ezTools; 

namespace ezAuto_EFEM
{
    public partial class EFEM_Recovery : Form
    {
        string m_id;
        int m_nLPNum, m_nModule = 4;
        Auto_Mom m_auto;
        Handler_Mom m_handler;
        EFEM_RecoveryModule[] m_module = new EFEM_RecoveryModule[5]; // ing 161024 for backside
        EFEM_RecoveryModule.eState m_eState = EFEM_RecoveryModule.eState.Disable; 

        public EFEM_Recovery()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        public void Init(string id, Auto_Mom auto, Log log, int nPort)
        {
            m_id = id;
            m_auto = auto;
            m_handler = m_auto.ClassHandler();
            m_nLPNum = nPort;
            for (int n=0; n<5; n++)
            {
                m_module[n] = new EFEM_RecoveryModule(); 
            }
            m_module[1].Init("Aligner", m_auto, m_handler.ClassAligner().InfoWafer, log, m_handler.ClassAligner().CheckWaferExist() == eHWResult.On);
            m_module[0].Init("Vision", m_auto, m_handler.ClassVisionWorks().InfoWafer, log, m_handler.ClassVisionWorks().IsWaferExist() == eHWResult.On);
            m_module[2].Init("WTR Upper", m_auto, m_handler.ClassWTR().GetInfoWafer(HW_WTR_Mom.eArm.Upper), log, m_handler.ClassWTR().CheckWaferExist(HW_WTR_Mom.eArm.Upper) == eHWResult.On);
            m_module[3].Init("WTR Lower", m_auto, m_handler.ClassWTR().GetInfoWafer(HW_WTR_Mom.eArm.Lower), log, m_handler.ClassWTR().CheckWaferExist(HW_WTR_Mom.eArm.Lower) == eHWResult.On);
            if (m_handler.ClassBackSide().m_bEnable) // ing 161024 for backside
            {
                m_nModule = 5;
                m_module[4].Init("Backside", m_auto, m_handler.ClassBackSide().InfoWafer, log, m_handler.ClassBackSide().CheckWaferExist() == eHWResult.On);
            } 
            //Module Position Set
            CPoint cpOrg = new CPoint(10, 10);
            CPoint cpShow = new CPoint(0,0);
            for (int n = 0; n < 4; n++)
            {
                cpShow.x = cpOrg.x + m_module[n].Size.Width * (int)(n/2);
                cpShow.y = cpOrg.y + m_module[n].Size.Height * (int)(n % 2);
                m_module[n].ShowDlg(this, ref cpShow);
            }
            if (m_handler.ClassBackSide().m_bEnable) // ing 161024 for backside
            {
                cpShow.x = cpOrg.x;
                cpShow.y = cpOrg.y + (m_module[0].Size.Height * 2);
                m_module[4].ShowDlg(this, ref cpShow);
            }
            //Wafer Info Position Set            
            cpShow.x = m_module[0].Size.Width * 2 +cpOrg.x; cpShow.y = cpOrg.y;
            pnWaferInfo.Location = cpShow.ToPoint();
            pnWaferInfo.Height = m_module[0].Size.Height * 2;
            if (m_handler.ClassBackSide().m_bEnable) pnWaferInfo.Height += m_module[4].Height; // ing 161024 for backside
            //Button Position Set
            Size sz = new Size(cpOrg.x + m_module[0].Size.Width * 2 + pnWaferInfo.Size.Width + 25, cpOrg.y + pnWaferInfo.Height + buttonRecover.Size.Height + 50);
            this.Size = sz;
            buttonRecover.Location = new Point(sz.Width / 4, sz.Height - buttonRecover.Size.Height - 45);
            buttonCancel.Location = new Point(sz.Width / 2, sz.Height - buttonCancel.Size.Height - 45);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            EFEM_RecoveryModule.eState eState = EFEM_RecoveryModule.eState.Enable;
            for (int n = 0; n < m_nModule; n++) // ing 161024 for backside
            {
                if (m_module[n].m_eState == EFEM_RecoveryModule.eState.Disable)
                {
                    eState = EFEM_RecoveryModule.eState.Disable;
                }
                for (int m = n + 1; m < m_nModule; m++) // ing 161024 for backside
                {
                    if (m_module[n].CheckSame(m_module[m].m_infoCombo))
                    {
                        eState = EFEM_RecoveryModule.eState.Disable;
                        m_module[n].m_strDisable = "Wafer Overlap"; 
                    }
                }
            }
            m_eState = eState;
            this.BackColor = m_module[0].GetColor(m_eState);
            buttonRecover.Enabled = (m_eState == EFEM_RecoveryModule.eState.Enable);
        }

        private void buttonRecover_Click(object sender, EventArgs e)
        {
            m_handler.ClassVisionWorks().InfoWafer = m_module[0].GetInfoWaferCombo();
            if (m_handler.ClassVisionWorks().InfoWafer != null) m_handler.ClassVisionWorks().m_bManualWaferExist = true;// ing 170905
            else m_handler.ClassVisionWorks().m_bManualWaferExist = false;
            m_handler.ClassAligner().InfoWafer = m_module[1].GetInfoWaferCombo();
            m_handler.ClassWTR().SetInfoWafer(HW_WTR_Mom.eArm.Upper, m_module[2].GetInfoWaferCombo());
            m_handler.ClassWTR().SetInfoWafer(HW_WTR_Mom.eArm.Lower, m_module[3].GetInfoWaferCombo());
            if (m_handler.ClassBackSide().m_bEnable) m_handler.ClassBackSide().InfoWafer = m_module[4].GetInfoWaferCombo(); // ing 161024 for backside
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close(); 
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close(); 
        }

        private void pnWaferInfo_Paint(object sender, PaintEventArgs e)
        {
            #region WaferInfo
            int nMargin = 3;
            int nH = pnWaferInfo.Height - nMargin;
            int nW = pnWaferInfo.Width - nMargin;
            Info_Carrier Carrier = m_auto.ClassHandler().ClassCarrier(m_nLPNum);
            int nHBox = nH / Carrier.m_lWafer;
            using (BufferedGraphics bufferedgraphic = BufferedGraphicsManager.Current.Allocate(e.Graphics, this.ClientRectangle))
            {
                Graphics G = bufferedgraphic.Graphics;
                G.Clear(this.BackColor);
                G.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                G.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);

                Font FontWafer = new Font(this.Font.Name, (int)(nHBox * 0.6), FontStyle.Regular, GraphicsUnit.Pixel);
                int nWidthSlotNum = GetTxtWidth(G, String.Format("{0,3}", Carrier.m_lWafer), FontWafer);
                int nWidthState = GetTxtWidth(G, Info_Wafer.eState.Select.ToString(), FontWafer);
                int nWidthWaferID = nW - nWidthSlotNum - nWidthState;


                if (m_auto.ClassHandler().ClassCarrier(m_nLPNum).m_eSlotType == Info_Carrier.eSlotType.ATI)
                {
                    for (int n = 0; n < Carrier.m_lWafer; n++)
                    {
                        Info_Wafer wafer = Carrier.m_infoWafer[n];

                        Rectangle rtSlot = new Rectangle(0, nH - (nHBox * (n + 1)), nWidthSlotNum, nHBox);
                        Rectangle rtState = new Rectangle(rtSlot.Right, rtSlot.Top, nWidthState, nHBox);
                        Rectangle rtWaferID = new Rectangle(rtState.Right, rtState.Top, nWidthWaferID, nHBox);
                    
                        string strSlotNum = String.Format("{0,3}", wafer.m_nSlot);
                   
                       
                        DrawStringFillRect(G, strSlotNum, FontWafer, rtSlot, Brushes.FloralWhite);
                        DrawStringFillRect(G, wafer.State.ToString(), FontWafer, rtState, GetStateBrush(wafer.State));
                        DrawStringFillRect(G, wafer.WAFERID, FontWafer, rtWaferID, Brushes.FloralWhite);
                    }
                }
                else if (m_auto.ClassHandler().ClassCarrier(m_nLPNum).m_eSlotType == Info_Carrier.eSlotType.Sanken)
                {
                    for (int n = Carrier.m_lWafer - 1; n >= 0; n--)
                    {
                        Info_Wafer wafer = Carrier.m_infoWafer[n];

                        Rectangle rtSlot = new Rectangle(0, nH - (nHBox * (n + 1)), nWidthSlotNum, nHBox);
                        Rectangle rtState = new Rectangle(rtSlot.Right, rtSlot.Top, nWidthState, nHBox);
                        Rectangle rtWaferID = new Rectangle(rtState.Right, rtState.Top, nWidthWaferID, nHBox);
                     
                        string strSlotNum = String.Format("{0,3}", wafer.m_nSlot);
                       

                        DrawStringFillRect(G, strSlotNum, FontWafer, rtSlot, Brushes.FloralWhite);
                        DrawStringFillRect(G, wafer.State.ToString(), FontWafer, rtState, GetStateBrush(wafer.State));
                        DrawStringFillRect(G, wafer.WAFERID, FontWafer, rtWaferID, Brushes.FloralWhite);
                        
                    }
                }
                bufferedgraphic.Render(e.Graphics);
            }
            #endregion
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

        public int GetLPNumForRecovery()
        {
            int ret = -1;
            bool IsAllSame = true;
            int[] nPortNums;
            if (m_handler.ClassBackSide().m_bEnable) nPortNums = new int[5] { -1, -1, -1, -1, -1 };
            else nPortNums = new int[4] { -1, -1, -1, -1 };

            if (m_handler.ClassAligner().IsWaferExist() == eHWResult.On && m_handler.ClassAligner().InfoWafer != null)
                nPortNums[0] = m_handler.ClassAligner().InfoWafer.m_nLoadPort;
            if (m_handler.ClassVisionWorks().IsWaferExist() == eHWResult.On && m_handler.ClassVisionWorks().InfoWafer != null)
                nPortNums[1] = m_handler.ClassVisionWorks().InfoWafer.m_nLoadPort;
            if (m_handler.ClassWTR().CheckWaferExist(HW_WTR_Mom.eArm.Lower) == eHWResult.On && m_handler.ClassWTR().GetInfoWafer(HW_WTR_Mom.eArm.Lower) != null)
                nPortNums[2] = m_handler.ClassWTR().GetInfoWafer(HW_WTR_Mom.eArm.Lower).m_nLoadPort;
            if (m_handler.ClassWTR().CheckWaferExist(HW_WTR_Mom.eArm.Upper) == eHWResult.On && m_handler.ClassWTR().GetInfoWafer(HW_WTR_Mom.eArm.Upper) != null)
                nPortNums[3] = m_handler.ClassWTR().GetInfoWafer(HW_WTR_Mom.eArm.Upper).m_nLoadPort;
            if (m_handler.ClassBackSide().m_bEnable)
            {
                if (m_handler.ClassBackSide().CheckWaferExist() == eHWResult.On && m_handler.ClassBackSide().InfoWafer != null)
                    nPortNums[4] = m_handler.ClassBackSide().InfoWafer.m_nLoadPort;
            }

            for (int i = 0; i < nPortNums.Length; i++)
            {
                for (int n = 0; n < nPortNums.Length; n++)
                {
                    if (nPortNums[i] != -1 && nPortNums[n] != -1)
                    {
                        ret = nPortNums[i];
                        if (nPortNums[i] != nPortNums[n])
                            IsAllSame = false;
                    }
                }
            }
            if (IsAllSame)
                return ret;
            else
                return -1;
        }
    }
}
