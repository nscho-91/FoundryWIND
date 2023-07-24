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

namespace ezAuto_EFEM
{
    public partial class UI_Simulation : DockContent
    {
        enum eWTRSubMotion
        {
            Init,
            Move,
            Turn,
            Extend,
            WaferOnOff,
            Retract,
        }
        
        class cPosition
        {
            public int X = 0;
            public int Y = 0;
            public int Width = 0;
            public int Height = 0;

            public cPosition(int x, int y, int width, int height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }

            public bool IsSamePos(cPosition pos)
            {
                if (this.X != pos.X) return false;
                if (this.Y != pos.Y) return false;
                return true;
            }
        }
        Auto_Mom m_auto = null;
        
        private eWTRSimnulMotion m_eWTRState = eWTRSimnulMotion.None;
        private eWTRSubMotion m_eWTRSubState = eWTRSubMotion.Init;

        private cPosition VisionHomePos_Boat = new cPosition(5, 17, 16, 16);
        private cPosition VisionHomePos_ScanY = new cPosition(1, 20, 40, 10);
        private cPosition VisionReadyPos_Boat = new cPosition(13, 62, 16, 16);
        private cPosition VisionReadyPos_ScanY = new cPosition(1, 65, 40, 10);
        private cPosition VisionInspPos_Boat = new cPosition(10, 42, 16, 16);
        private cPosition VisionInspPos_ScanY = new cPosition(1, 45, 40, 10);

        private cPosition WTR_HomePos = new cPosition(52, 67, 15, 30);
        private cPosition WTR_LP1Pos = new cPosition(52, 63, 15, 30);
        private cPosition WTR_LP2Pos = new cPosition(52, 29, 15, 30);
        private cPosition WTR_AlignerPos = new cPosition(52, 30, 15, 30);
        private cPosition WTR_VisionPos = new cPosition(52, 61, 15, 30);
        private RotateFlipType Cur_WTR_Rotate = RotateFlipType.RotateNoneFlipNone;

        private cPosition Cur_WTRPos = new cPosition(0, 0, 0, 0);
        private cPosition Cur_Vision_Boat = new cPosition(0, 0, 0, 0);
        private cPosition Cur_Vision_ScanY = new cPosition(0, 0, 0, 0);

        private cPosition WTRWafer_RotateNone_Pos = new cPosition(3, 0, 11, 15);
        private cPosition WTRWafer_Rotate90_Pos = new cPosition(8, 5, 11, 15);
        private cPosition WTRWafer_Rotate270_Pos = new cPosition(2 ,3 , 11 , 15);
        private cPosition WTRWafer_RotateNone_ExtendPos = new cPosition(1, 2, 14, 14);
        private cPosition WTRWafer_RotateNone_ExtendPos2 = new cPosition(1, 27, 14, 14);
        private cPosition WTRWafer_Rotate90_ExtendPos = new cPosition(18, 4, 15, 15);
        private cPosition WTRWafer_Rotate270_ExtendPos = new cPosition(4,4,15,15);

        public UI_Simulation()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }
         
        public void Init(Auto_Mom auto)
        {
            m_auto = auto;

            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, RotateFlipType.RotateNoneFlipNone);
            SetImageOnPanel(PanelBoat, global::ezAuto_EFEM.Properties.Resources.Boat);
            SetImageOnPanel(PanelScanY, global::ezAuto_EFEM.Properties.Resources.ScanY1);
            SetImageOnPanel(panelWaferInVS, global::ezAuto_EFEM.Properties.Resources.wafer);
            SetImageOnPanel(panelWaferInUArm, global::ezAuto_EFEM.Properties.Resources.wafer);
            SetImageOnPanel(PanelWaferInLArm, global::ezAuto_EFEM.Properties.Resources.wafer);
            SetImageOnPanel(panelWaferInAligner, global::ezAuto_EFEM.Properties.Resources.wafer);

            Cur_WTRPos = WTR_HomePos;

            Cur_Vision_Boat = VisionHomePos_Boat;
            Cur_Vision_ScanY = VisionHomePos_ScanY;
        }

        private void SetImageOnPanel(Panel panel, Bitmap bitmap, RotateFlipType rotate = RotateFlipType.RotateNoneFlipNone)
        {
            Bitmap bit = new Bitmap(bitmap);
            if (rotate != RotateFlipType.RotateNoneFlipNone) {
                bit.RotateFlip(rotate);
                
            }
            if (panel == panelWTR)
                Cur_WTR_Rotate = rotate;
            bit.MakeTransparent(Color.White);
            panel.BackgroundImage = bit;
        }

        public void ShowPanel(DockPanel dockpanel)
        {
            this.Show(dockpanel);
        }

        private void UI_Simulation_Resize(object sender, EventArgs e)
        {
            if (this.Width != 0 && this.Height != 0) {
                SetPanelPosition(panelLP1, 76, 33, 16, 21); // ing 170913 포트 1, 2번 위치가 바뀌어서 수정
                SetPanelPosition(panelLP2, 76, 65, 16, 21);
                SetPanelPosition(panelWTR, Cur_WTRPos);
                SetPanelPosition(PanelScanY, Cur_Vision_ScanY);
                SetPanelPosition(PanelBoat, Cur_Vision_Boat);
                SetPanelPosition(panelWaferInVS, 2, 2, Cur_Vision_Boat.Width - 4, Cur_Vision_Boat.Height - 4);
                SetPanelPosition(panelWaferInAligner, 54, 8, 12, 12);
                SetLabelPosition(lbVisionConnect, 0, 0, 42, 11);
            }
        }

        private void SetPanelPosition(Panel panel, int Percent_Position_X, int Percent_Position_Y, int Percent_Width, int Percent_Height) //각 Float 변수는 0~1 사이값
        {
            Point point = new Point();

            // Panel 위치 조정
            point.X = Convert.ToInt32(this.Width * (double)Percent_Position_X/100.0);
            point.Y = Convert.ToInt32(this.Height * (double)Percent_Position_Y/100.0);
            panel.Location = point;

            //Panel 크기 조정
            

            if (panel == panelWTR) {
                if (Cur_WTR_Rotate != RotateFlipType.RotateNoneFlipNone) {
                    panel.Height = Convert.ToInt32(this.Width * (double)Percent_Width / 100.0);
                    panel.Width = Convert.ToInt32(this.Height * (double)Percent_Height / 100.0);
                }
                else { 
                    panel.Width = Convert.ToInt32(this.Width * (double)Percent_Width / 100.0);
                    panel.Height = Convert.ToInt32(this.Height * (double)Percent_Height / 100.0);
                }
                cPosition temp = new cPosition(Percent_Position_X, Percent_Position_Y, Percent_Width, Percent_Height);
                Cur_WTRPos = temp;
            }
            else {
                panel.Width = Convert.ToInt32(this.Width * (double)Percent_Width / 100.0);
                panel.Height = Convert.ToInt32(this.Height * (double)Percent_Height / 100.0);
            }
        }

        private void SetLabelPosition(Label label, int Percent_Position_X, int Percent_Position_Y, int Percent_Width, int Percent_Height) //각 Float 변수는 0~1 사이값
        {
            Point point = new Point();

            // Panel 위치 조정
            point.X = Convert.ToInt32(this.Width * (double)Percent_Position_X / 100.0);
            point.Y = Convert.ToInt32(this.Height * (double)Percent_Position_Y / 100.0);
            label.Location = point;

            //Panel 크기 조정
            label.Width = Convert.ToInt32(this.Width * (double)Percent_Width / 100.0);
            label.Height = Convert.ToInt32(this.Height * (double)Percent_Height / 100.0);

            Font ft;
            Graphics gp;
            SizeF sz;
            Single Faktor, FaktorX, FaktorY;

            gp = label.CreateGraphics();
            sz = gp.MeasureString(label.Text, label.Font);
            gp.Dispose();
            if (sz.Height != 0 && sz.Width != 0) 
            {
                FaktorX = (label.Width) / sz.Width * (float)0.8;
                FaktorY = (label.Height) / sz.Height * (float)0.8;
                if (FaktorY <= 0 || FaktorX <= 0) return;
                if (FaktorX > FaktorY)
                    Faktor = FaktorY;
                else
                    Faktor = FaktorX;
                if (Faktor > 20) Faktor = 20; 
                ft = label.Font;
                if (Faktor >= 100) Faktor = 100; 
                label.Font = new Font(ft.Name, ft.SizeInPoints * (Faktor));
            }
        }

        private void SetPanelPosition(Panel panel, cPosition Pos) //각 Float 변수는 0~1 사이값
        {
            Point point = new Point();

            // Panel 위치 조정
            point.X = Convert.ToInt32(this.Width * (double)Pos.X/100.0);
            point.Y = Convert.ToInt32(this.Height * (double)Pos.Y / 100.0);
            panel.Location = point;

            //Panel 크기 조정
            if (panel == panelWTR) {
                if (Cur_WTR_Rotate != RotateFlipType.RotateNoneFlipNone) {
                    panel.Height = Convert.ToInt32(this.Width * (double)Pos.Width / 100.0);
                    panel.Width = Convert.ToInt32(this.Height * (double)Pos.Height / 100.0);
                }
                else {
                    panel.Width = Convert.ToInt32(this.Width * (double)Pos.Width / 100.0);
                    panel.Height = Convert.ToInt32(this.Height * (double)Pos.Height / 100.0);
                }
                cPosition temp = new cPosition(Pos.X, Pos.Y, Pos.Width, Pos.Height);
                Cur_WTRPos = temp;
            }
            else {
                panel.Width = Convert.ToInt32(this.Width * (double)Pos.Width / 100.0);
                panel.Height = Convert.ToInt32(this.Height * (double)Pos.Height / 100.0);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_auto.ClassHandler().ClassLoadPort(0) != null && !m_auto.ClassHandler().ClassLoadPort(0).IsPlaced())
                panelLP1.Visible = true;
            else
                panelLP1.Visible = false;

            if (m_auto.ClassHandler().ClassLoadPort(1) != null && !m_auto.ClassHandler().ClassLoadPort(1).IsPlaced())
                panelLP2.Visible = true;
            else
                panelLP2.Visible = false;

            if(m_auto.ClassHandler().ClassVisionWorks().IsWFStgReady() == eHWResult.OK && Cur_Vision_Boat != VisionReadyPos_Boat){
                SetPanelPosition(PanelBoat, VisionReadyPos_Boat);
                SetPanelPosition(PanelScanY, VisionReadyPos_ScanY);
                Cur_Vision_Boat = VisionReadyPos_Boat;
                Cur_Vision_ScanY = VisionReadyPos_ScanY;
            }

            WTRMotionProcess();

            if (m_auto.ClassHandler().ClassVisionWorks().IsConnected()) {
                lbVisionConnect.Text = "Vision : Connected";
                lbVisionConnect.BackColor = Color.PaleGreen;
            }
            else {
                
                lbVisionConnect.Text = "Vision : NotConnected";
                lbVisionConnect.BackColor = Color.Gray;
            }
        }

        private eHWResult WTRImageRotate(RotateFlipType Rotate)
        {
            if (Rotate == Cur_WTR_Rotate)
                return eHWResult.OK;
            SizeF tempSize = new SizeF();
            tempSize = panelWTR.Size;
            //panelWTR.Width = (int)tempSize.Height;
            //panelWTR.Height = (int)tempSize.Width;
            switch (Cur_WTR_Rotate) {
                case RotateFlipType.Rotate270FlipNone:
                    SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, RotateFlipType.RotateNoneFlipNone);
                    break;
                case RotateFlipType.Rotate90FlipNone:
                    SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, RotateFlipType.RotateNoneFlipNone);
                    break;
                case RotateFlipType.RotateNoneFlipNone:
                    SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Rotate);
                    break;
            }
            return eHWResult.Off;
        }

        private eHWResult WTRImageMove(cPosition pos)
        {
            int MoveStep = 1;
            if (pos.IsSamePos(Cur_WTRPos))
                return eHWResult.OK;
            if(pos.Y > Cur_WTRPos.Y)
                SetPanelPosition(panelWTR, Cur_WTRPos.X, Cur_WTRPos.Y + MoveStep,Cur_WTRPos.Width,Cur_WTRPos.Height);
            else
                SetPanelPosition(panelWTR, Cur_WTRPos.X, Cur_WTRPos.Y - MoveStep, Cur_WTRPos.Width, Cur_WTRPos.Height);
            return eHWResult.Off;
        }

        public void SetWTRMotion(eWTRSimnulMotion Motion)
        {
            m_eWTRSubState = eWTRSubMotion.Init;
            m_eWTRState = Motion;
        }
        
        private void WTRMotionProcess()
        {
            switch(m_eWTRState)
            {
                case eWTRSimnulMotion.None:
                    break;
                case eWTRSimnulMotion.Home:
                    #region Home
                    switch (m_eWTRSubState) {
                        case eWTRSubMotion.Init:
                            if (panelWTR.BackgroundImage != global::ezAuto_EFEM.Properties.Resources.wtr1) {
                                SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                            }
                            m_eWTRSubState = eWTRSubMotion.Turn;
                            break;
                        case eWTRSubMotion.Turn:
                            if(WTRImageRotate(RotateFlipType.RotateNoneFlipNone) == eHWResult.OK){
                                m_eWTRSubState = eWTRSubMotion.Move;
                            }
                            break;
                        case eWTRSubMotion.Move:
                            if (WTRImageMove(WTR_HomePos) == eHWResult.OK) {
                                m_eWTRSubState = eWTRSubMotion.WaferOnOff;
                            }
                            break;
                        case eWTRSubMotion.WaferOnOff:
                            if (m_auto.ClassWork().GetState() == eWorkRun.Ready) {
                                if (m_auto.ClassHandler().ClassWTR().CheckWaferExist(HW_WTR_Mom.eArm.Upper) == eHWResult.On) {
                                    SetPanelPosition(panelWaferInUArm, 3, 0, 11, 15);
                                    panelWaferInUArm.Visible = true;
                                }
                                else
                                    panelWaferInUArm.Visible = false;

                                if (m_auto.ClassHandler().ClassWTR().CheckWaferExist(HW_WTR_Mom.eArm.Lower) == eHWResult.On) {
                                    SetPanelPosition(PanelWaferInLArm, 3, 0, 11, 15);
                                    PanelWaferInLArm.Visible = true;
                                }
                                else
                                    PanelWaferInLArm.Visible = false;

                                if (m_auto.ClassHandler().ClassAligner().IsWaferExist()== eHWResult.On) {
                                    panelWaferInAligner.Visible = true;
                                }
                                else
                                    panelWaferInAligner.Visible = false;

                                if (m_auto.ClassHandler().ClassVisionWorks().IsWaferExist() == eHWResult.On) {
                                    panelWaferInVS.Visible = true;
                                }
                                else
                                    panelWaferInVS.Visible = false;
                                
                                m_eWTRState = eWTRSimnulMotion.None;
                                m_eWTRSubState = eWTRSubMotion.Init;
                            }
                            break;
                    }
                    #endregion
                    break;
                case eWTRSimnulMotion.LP1GetUArm:
                    #region LP1GetUArm
                    switch (m_eWTRSubState) {
                        case eWTRSubMotion.Init:
                            if (panelWTR.BackgroundImage != global::ezAuto_EFEM.Properties.Resources.wtr1) {
                                SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                                SetPanelPosition(panelWTR, Cur_WTRPos);
                            }
                            m_eWTRSubState = eWTRSubMotion.Turn;
                            break;
                        case eWTRSubMotion.Turn:
                            if(WTRImageRotate(RotateFlipType.Rotate90FlipNone) == eHWResult.OK){
                                m_eWTRSubState = eWTRSubMotion.Move;
                            }
                            break;
                        case eWTRSubMotion.Move:
                            if (WTRImageMove(WTR_LP1Pos) == eHWResult.OK) {
                                m_eWTRSubState = eWTRSubMotion.Extend;
                            }
                            break;
                        case eWTRSubMotion.Extend:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtrUpper1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, Cur_WTRPos.X, Cur_WTRPos.Y, 15, 50);
                            m_eWTRSubState = eWTRSubMotion.WaferOnOff;
                            break;
                        case eWTRSubMotion.WaferOnOff:
                            SetPanelPosition(panelWaferInUArm, WTRWafer_Rotate90_ExtendPos);
                            panelWaferInUArm.Visible = true;
                            m_eWTRSubState = eWTRSubMotion.Retract;
                            break;
                        case eWTRSubMotion.Retract:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, WTR_LP1Pos);
                            SetPanelPosition(panelWaferInUArm, WTRWafer_Rotate90_Pos);
                            m_eWTRSubState = eWTRSubMotion.Init;
                            m_eWTRState = eWTRSimnulMotion.None;
                            break;
                    }
                    #endregion
                    break;
                case eWTRSimnulMotion.LP1GetLArm:
                    #region LP1GetLArm
                    switch (m_eWTRSubState) {
                        case eWTRSubMotion.Init:
                            if (panelWTR.BackgroundImage != global::ezAuto_EFEM.Properties.Resources.wtr1) {
                                SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                                SetPanelPosition(panelWTR, Cur_WTRPos);
                            }
                            m_eWTRSubState = eWTRSubMotion.Turn;
                            break;
                        case eWTRSubMotion.Turn:
                            if (WTRImageRotate(RotateFlipType.Rotate90FlipNone) == eHWResult.OK) {
                                m_eWTRSubState = eWTRSubMotion.Move;
                            }
                            break;
                        case eWTRSubMotion.Move:
                            if (WTRImageMove(WTR_LP1Pos) == eHWResult.OK) {
                                m_eWTRSubState = eWTRSubMotion.Extend;
                            }
                            break;
                        case eWTRSubMotion.Extend:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtrLower1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, Cur_WTRPos.X, Cur_WTRPos.Y, 15, 50);
                            m_eWTRSubState = eWTRSubMotion.WaferOnOff;
                            break;
                        case eWTRSubMotion.WaferOnOff:
                            SetPanelPosition(PanelWaferInLArm, WTRWafer_Rotate90_ExtendPos);
                            PanelWaferInLArm.Visible = true;
                            m_eWTRSubState = eWTRSubMotion.Retract;
                            break;
                        case eWTRSubMotion.Retract:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, WTR_LP1Pos);
                            SetPanelPosition(PanelWaferInLArm, WTRWafer_Rotate90_Pos);
                            m_eWTRSubState = eWTRSubMotion.Init;
                            m_eWTRState = eWTRSimnulMotion.None;
                            break;
                    }
                    #endregion
                    break;
                case eWTRSimnulMotion.LP1PutUArm:
                    #region LP1PUTUArm
                    switch (m_eWTRSubState) {
                        case eWTRSubMotion.Init:
                            if (panelWTR.BackgroundImage != global::ezAuto_EFEM.Properties.Resources.wtr1) {
                                SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                                SetPanelPosition(panelWTR, Cur_WTRPos);
                            }
                            m_eWTRSubState = eWTRSubMotion.Turn;
                            break;
                        case eWTRSubMotion.Turn:
                            if (WTRImageRotate(RotateFlipType.Rotate90FlipNone) == eHWResult.OK) {
                                if (PanelWaferInLArm.Visible == true)
                                    SetPanelPosition(PanelWaferInLArm, WTRWafer_Rotate90_Pos);
                                if (panelWaferInUArm.Visible == true)
                                    SetPanelPosition(panelWaferInUArm, WTRWafer_Rotate90_Pos);
                                m_eWTRSubState = eWTRSubMotion.Move;
                            }
                            break;
                        case eWTRSubMotion.Move:
                            if (WTRImageMove(WTR_LP1Pos) == eHWResult.OK) {
                                m_eWTRSubState = eWTRSubMotion.Extend;
                            }
                            break;
                        case eWTRSubMotion.Extend:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtrUpper1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, Cur_WTRPos.X, Cur_WTRPos.Y, 15, 50);
                            SetPanelPosition(panelWaferInUArm, WTRWafer_Rotate90_ExtendPos);
                            m_eWTRSubState = eWTRSubMotion.WaferOnOff;
                            break;
                        case eWTRSubMotion.WaferOnOff:
                            panelWaferInUArm.Visible = false;
                            m_eWTRSubState = eWTRSubMotion.Retract;
                            break;
                        case eWTRSubMotion.Retract:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, WTR_LP1Pos);
                            SetPanelPosition(panelWaferInUArm, WTRWafer_Rotate90_Pos);
                            m_eWTRSubState = eWTRSubMotion.Init;
                            m_eWTRState = eWTRSimnulMotion.None;
                            break;
                    }
                    #endregion
                    break;
                case eWTRSimnulMotion.LP1PutLArm:
                    #region LP1PUTLArm
                    switch (m_eWTRSubState) {
                        case eWTRSubMotion.Init:
                            if (panelWTR.BackgroundImage != global::ezAuto_EFEM.Properties.Resources.wtr1) {
                                SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                                SetPanelPosition(panelWTR, Cur_WTRPos);
                            }
                            m_eWTRSubState = eWTRSubMotion.Turn;
                            break;
                        case eWTRSubMotion.Turn:
                            if (WTRImageRotate(RotateFlipType.Rotate90FlipNone) == eHWResult.OK) {
                                if (PanelWaferInLArm.Visible == true)
                                    SetPanelPosition(PanelWaferInLArm, WTRWafer_Rotate90_Pos);
                                if (panelWaferInUArm.Visible == true)
                                    SetPanelPosition(panelWaferInUArm, WTRWafer_Rotate90_Pos);
                                m_eWTRSubState = eWTRSubMotion.Move;
                            }
                            break;
                        case eWTRSubMotion.Move:
                            if (WTRImageMove(WTR_LP1Pos) == eHWResult.OK) {
                                m_eWTRSubState = eWTRSubMotion.Extend;
                            }
                            break;
                        case eWTRSubMotion.Extend:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtrUpper1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, Cur_WTRPos.X, Cur_WTRPos.Y, 15, 50);
                            SetPanelPosition(PanelWaferInLArm, WTRWafer_Rotate90_ExtendPos);
                            m_eWTRSubState = eWTRSubMotion.WaferOnOff;
                            break;
                        case eWTRSubMotion.WaferOnOff:
                            PanelWaferInLArm.Visible = false;
                            m_eWTRSubState = eWTRSubMotion.Retract;
                            break;
                        case eWTRSubMotion.Retract:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, WTR_LP1Pos);
                            SetPanelPosition(PanelWaferInLArm, WTRWafer_Rotate90_Pos);
                            m_eWTRSubState = eWTRSubMotion.Init;
                            m_eWTRState = eWTRSimnulMotion.None;
                            break;
                    }
                    #endregion
                    break;
                case eWTRSimnulMotion.LP2GetUArm:
                    #region LP2GetUArm
                    switch (m_eWTRSubState) {
                        case eWTRSubMotion.Init:
                            if (panelWTR.BackgroundImage != global::ezAuto_EFEM.Properties.Resources.wtr1) {
                                SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                                SetPanelPosition(panelWTR, Cur_WTRPos);
                            }
                            m_eWTRSubState = eWTRSubMotion.Turn;
                            break;
                        case eWTRSubMotion.Turn:
                            if (WTRImageRotate(RotateFlipType.Rotate90FlipNone) == eHWResult.OK) {
                                if (PanelWaferInLArm.Visible == true)
                                    SetPanelPosition(PanelWaferInLArm, WTRWafer_Rotate90_Pos);
                                if (panelWaferInUArm.Visible == true)
                                    SetPanelPosition(panelWaferInUArm, WTRWafer_Rotate90_Pos);
                                m_eWTRSubState = eWTRSubMotion.Move;
                            }
                            break;
                        case eWTRSubMotion.Move:
                            if (WTRImageMove(WTR_LP2Pos) == eHWResult.OK) {
                                m_eWTRSubState = eWTRSubMotion.Extend;
                            }
                            break;
                        case eWTRSubMotion.Extend:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtrUpper1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, Cur_WTRPos.X, Cur_WTRPos.Y, 15, 50);
                            m_eWTRSubState = eWTRSubMotion.WaferOnOff;
                            break;
                        case eWTRSubMotion.WaferOnOff:
                            SetPanelPosition(panelWaferInUArm, WTRWafer_Rotate90_ExtendPos);
                            panelWaferInUArm.Visible = true;
                            m_eWTRSubState = eWTRSubMotion.Retract;
                            break;
                        case eWTRSubMotion.Retract:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, WTR_LP2Pos);
                            SetPanelPosition(panelWaferInUArm, WTRWafer_Rotate90_Pos);
                            m_eWTRSubState = eWTRSubMotion.Init;
                            m_eWTRState = eWTRSimnulMotion.None;
                            break;
                    }
                    #endregion
                    break;
                case eWTRSimnulMotion.LP2GetLArm:
                    #region LP2GetLArm
                    switch (m_eWTRSubState) {
                        case eWTRSubMotion.Init:
                            if (panelWTR.BackgroundImage != global::ezAuto_EFEM.Properties.Resources.wtr1) {
                                SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                                SetPanelPosition(panelWTR, Cur_WTRPos);
                            }
                            m_eWTRSubState = eWTRSubMotion.Turn;
                            break;
                        case eWTRSubMotion.Turn:
                            if (WTRImageRotate(RotateFlipType.Rotate90FlipNone) == eHWResult.OK) {
                                if (PanelWaferInLArm.Visible == true)
                                    SetPanelPosition(PanelWaferInLArm, WTRWafer_Rotate90_Pos);
                                if (panelWaferInUArm.Visible == true)
                                    SetPanelPosition(panelWaferInUArm, WTRWafer_Rotate90_Pos);
                                m_eWTRSubState = eWTRSubMotion.Move;
                            }
                            break;
                        case eWTRSubMotion.Move:
                            if (WTRImageMove(WTR_LP2Pos) == eHWResult.OK) {
                                m_eWTRSubState = eWTRSubMotion.Extend;
                            }
                            break;
                        case eWTRSubMotion.Extend:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtrUpper1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, Cur_WTRPos.X, Cur_WTRPos.Y, 15, 50);
                            m_eWTRSubState = eWTRSubMotion.WaferOnOff;
                            break;
                        case eWTRSubMotion.WaferOnOff:
                            SetPanelPosition(PanelWaferInLArm, WTRWafer_Rotate90_ExtendPos);
                            PanelWaferInLArm.Visible = true;
                            m_eWTRSubState = eWTRSubMotion.Retract;
                            break;
                        case eWTRSubMotion.Retract:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, WTR_LP2Pos);
                            SetPanelPosition(PanelWaferInLArm, WTRWafer_Rotate90_Pos);
                            m_eWTRSubState = eWTRSubMotion.Init;
                            m_eWTRState = eWTRSimnulMotion.None;
                            break;
                    }
                    #endregion
                    break;
                case eWTRSimnulMotion.LP2PutUArm:
                    #region LP2PUTUArm
                    switch (m_eWTRSubState) {
                        case eWTRSubMotion.Init:
                            if (panelWTR.BackgroundImage != global::ezAuto_EFEM.Properties.Resources.wtr1) {
                                SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                                SetPanelPosition(panelWTR, Cur_WTRPos);
                            }
                            m_eWTRSubState = eWTRSubMotion.Turn;
                            break;
                        case eWTRSubMotion.Turn:
                            if(WTRImageRotate(RotateFlipType.Rotate90FlipNone) == eHWResult.OK){
                                if (PanelWaferInLArm.Visible == true)
                                    SetPanelPosition(PanelWaferInLArm, WTRWafer_Rotate90_Pos);
                                if (panelWaferInUArm.Visible == true)
                                    SetPanelPosition(panelWaferInUArm, WTRWafer_Rotate90_Pos);
                                m_eWTRSubState = eWTRSubMotion.Move;
                            }
                            break;
                        case eWTRSubMotion.Move:
                            if (WTRImageMove(WTR_LP2Pos) == eHWResult.OK) {
                                m_eWTRSubState = eWTRSubMotion.Extend;
                            }
                            break;
                        case eWTRSubMotion.Extend:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtrUpper1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, Cur_WTRPos.X, Cur_WTRPos.Y, 15, 50);
                            SetPanelPosition(panelWaferInUArm, WTRWafer_Rotate90_ExtendPos);
                            m_eWTRSubState = eWTRSubMotion.WaferOnOff;
                            break;
                        case eWTRSubMotion.WaferOnOff:
                            panelWaferInUArm.Visible = false;
                            m_eWTRSubState = eWTRSubMotion.Retract;
                            break;
                        case eWTRSubMotion.Retract:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, WTR_LP2Pos);
                            SetPanelPosition(panelWaferInUArm, WTRWafer_Rotate90_Pos);
                            m_eWTRSubState = eWTRSubMotion.Init;
                            m_eWTRState = eWTRSimnulMotion.None;
                            break;
                    }
                    #endregion
                    break;
                case eWTRSimnulMotion.LP2PutLArm:
                    #region LP2PUTLArm
                    switch (m_eWTRSubState) {
                        case eWTRSubMotion.Init:
                            if (panelWTR.BackgroundImage != global::ezAuto_EFEM.Properties.Resources.wtr1) {
                                SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                                SetPanelPosition(panelWTR, Cur_WTRPos);
                            }
                            m_eWTRSubState = eWTRSubMotion.Turn;
                            break;
                        case eWTRSubMotion.Turn:
                            if (WTRImageRotate(RotateFlipType.Rotate90FlipNone) == eHWResult.OK) {
                                if (PanelWaferInLArm.Visible == true)
                                    SetPanelPosition(PanelWaferInLArm, WTRWafer_Rotate90_Pos);
                                if (panelWaferInUArm.Visible == true)
                                    SetPanelPosition(panelWaferInUArm, WTRWafer_Rotate90_Pos);
                                m_eWTRSubState = eWTRSubMotion.Move;
                            }
                            break;
                        case eWTRSubMotion.Move:
                            if (WTRImageMove(WTR_LP2Pos) == eHWResult.OK) {
                                m_eWTRSubState = eWTRSubMotion.Extend;
                            }
                            break;
                        case eWTRSubMotion.Extend:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtrLower1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, Cur_WTRPos.X, Cur_WTRPos.Y, 15, 50);
                            SetPanelPosition(PanelWaferInLArm, 18, 4, 15, 15);
                            m_eWTRSubState = eWTRSubMotion.WaferOnOff;
                            break;
                        case eWTRSubMotion.WaferOnOff:
                            PanelWaferInLArm.Visible = false;
                            m_eWTRSubState = eWTRSubMotion.Retract;
                            break;
                        case eWTRSubMotion.Retract:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, WTR_LP2Pos);
                            SetPanelPosition(PanelWaferInLArm, 8, 5, 11, 15);
                            m_eWTRSubState = eWTRSubMotion.Init;
                            m_eWTRState = eWTRSimnulMotion.None;
                            break;
                    }
                    #endregion
                    break;
                case eWTRSimnulMotion.AlignerGetUArm:
                    #region AlignerGetUArm
                    switch (m_eWTRSubState) {
                        case eWTRSubMotion.Init:
                            if (panelWTR.BackgroundImage != global::ezAuto_EFEM.Properties.Resources.wtr1) {
                                SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                                SetPanelPosition(panelWTR, Cur_WTRPos);
                            }
                            m_eWTRSubState = eWTRSubMotion.Turn;
                            break;
                        case eWTRSubMotion.Turn:
                            if (WTRImageRotate(RotateFlipType.RotateNoneFlipNone) == eHWResult.OK) {
                                if (PanelWaferInLArm.Visible == true)
                                    SetPanelPosition(PanelWaferInLArm, WTRWafer_RotateNone_Pos);
                                if (panelWaferInUArm.Visible == true)
                                    SetPanelPosition(panelWaferInUArm, WTRWafer_RotateNone_Pos);
                                m_eWTRSubState = eWTRSubMotion.Move;
                            }
                            break;
                        case eWTRSubMotion.Move:
                            if (WTRImageMove(WTR_AlignerPos) == eHWResult.OK) {
                                m_eWTRSubState = eWTRSubMotion.Extend;
                            }
                            break;
                        case eWTRSubMotion.Extend:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtrUpper1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, Cur_WTRPos.X, Cur_WTRPos.Y-25, 15, 50);
                            m_eWTRSubState = eWTRSubMotion.WaferOnOff;
                            break;
                        case eWTRSubMotion.WaferOnOff:
                            SetPanelPosition(panelWaferInUArm, WTRWafer_RotateNone_ExtendPos);
                            panelWaferInAligner.Visible = false;
                            panelWaferInUArm.Visible = true;
                            m_eWTRSubState = eWTRSubMotion.Retract;
                            break;
                        case eWTRSubMotion.Retract:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, WTR_AlignerPos);
                            SetPanelPosition(panelWaferInUArm, WTRWafer_RotateNone_Pos);
                            m_eWTRSubState = eWTRSubMotion.Init;
                            m_eWTRState = eWTRSimnulMotion.None;
                            break;
                    }
                    #endregion
                    break;
                case eWTRSimnulMotion.AlignerPutUArm:
                    #region AlignerPutUArm
                    switch (m_eWTRSubState) {
                        case eWTRSubMotion.Init:
                            if (panelWTR.BackgroundImage != global::ezAuto_EFEM.Properties.Resources.wtr1) {
                                SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                                SetPanelPosition(panelWTR, Cur_WTRPos);
                            }
                            m_eWTRSubState = eWTRSubMotion.Turn;
                            break;
                        case eWTRSubMotion.Turn:
                            if (WTRImageRotate(RotateFlipType.RotateNoneFlipNone) == eHWResult.OK) {
                                if (PanelWaferInLArm.Visible == true)
                                    SetPanelPosition(PanelWaferInLArm, WTRWafer_RotateNone_Pos);
                                if (panelWaferInUArm.Visible == true)
                                    SetPanelPosition(panelWaferInUArm, WTRWafer_RotateNone_Pos);
                                m_eWTRSubState = eWTRSubMotion.Move;
                            }
                            break;
                        case eWTRSubMotion.Move:
                            if (WTRImageMove(WTR_AlignerPos) == eHWResult.OK) {
                                m_eWTRSubState = eWTRSubMotion.Extend;
                            }
                            break;
                        case eWTRSubMotion.Extend:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtrUpper1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, Cur_WTRPos.X, Cur_WTRPos.Y-25, 15, 50);
                            m_eWTRSubState = eWTRSubMotion.WaferOnOff;
                            break;
                        case eWTRSubMotion.WaferOnOff:
                            SetPanelPosition(panelWaferInUArm, WTRWafer_RotateNone_ExtendPos);
                            panelWaferInAligner.Visible = true;
                            panelWaferInUArm.Visible = false;
                            m_eWTRSubState = eWTRSubMotion.Retract;
                            break;
                        case eWTRSubMotion.Retract:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, WTR_AlignerPos);
                            SetPanelPosition(panelWaferInUArm, WTRWafer_RotateNone_Pos);
                            m_eWTRSubState = eWTRSubMotion.Init;
                            m_eWTRState = eWTRSimnulMotion.None;
                            break;
                    }
                    #endregion
                    break;
                case eWTRSimnulMotion.AlignerGetLArm:
                    #region AlignerGetLArm
                    switch (m_eWTRSubState) {
                        case eWTRSubMotion.Init:
                            if (panelWTR.BackgroundImage != global::ezAuto_EFEM.Properties.Resources.wtr1) {
                                SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                                SetPanelPosition(panelWTR, Cur_WTRPos);
                            }
                            m_eWTRSubState = eWTRSubMotion.Turn;
                            break;
                        case eWTRSubMotion.Turn:
                            if (WTRImageRotate(RotateFlipType.RotateNoneFlipNone) == eHWResult.OK) {
                                if (PanelWaferInLArm.Visible == true)
                                    SetPanelPosition(PanelWaferInLArm, WTRWafer_RotateNone_Pos);
                                if (panelWaferInUArm.Visible == true)
                                    SetPanelPosition(panelWaferInUArm, WTRWafer_RotateNone_Pos);
                                m_eWTRSubState = eWTRSubMotion.Move;
                            }
                            break;
                        case eWTRSubMotion.Move:
                            if (WTRImageMove(WTR_AlignerPos) == eHWResult.OK) {
                                m_eWTRSubState = eWTRSubMotion.Extend;
                            }
                            break;
                        case eWTRSubMotion.Extend:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtrLower1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, Cur_WTRPos.X, Cur_WTRPos.Y - 25, 15, 50);
                            if (panelWaferInUArm.Visible == true)
                                SetPanelPosition(panelWaferInUArm, WTRWafer_RotateNone_ExtendPos2);
                            m_eWTRSubState = eWTRSubMotion.WaferOnOff;
                            break;
                        case eWTRSubMotion.WaferOnOff:
                            SetPanelPosition(PanelWaferInLArm, WTRWafer_RotateNone_ExtendPos);
                            panelWaferInAligner.Visible = false;
                            PanelWaferInLArm.Visible = true;
                            m_eWTRSubState = eWTRSubMotion.Retract;
                            break;
                        case eWTRSubMotion.Retract:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, WTR_AlignerPos);
                            SetPanelPosition(PanelWaferInLArm, WTRWafer_RotateNone_Pos);
                            m_eWTRSubState = eWTRSubMotion.Init;
                            m_eWTRState = eWTRSimnulMotion.None;
                            break;
                    }
                    #endregion
                    break;
                case eWTRSimnulMotion.AlignerPutLArm:
                    #region AlignerPutLArm
                    switch (m_eWTRSubState) {
                        case eWTRSubMotion.Init:
                            if (panelWTR.BackgroundImage != global::ezAuto_EFEM.Properties.Resources.wtr1) {
                                SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                                SetPanelPosition(panelWTR, Cur_WTRPos);
                            }
                            m_eWTRSubState = eWTRSubMotion.Turn;
                            break;
                        case eWTRSubMotion.Turn:
                            if (WTRImageRotate(RotateFlipType.RotateNoneFlipNone) == eHWResult.OK) {
                                if (PanelWaferInLArm.Visible == true)
                                    SetPanelPosition(PanelWaferInLArm, WTRWafer_RotateNone_Pos);
                                if (panelWaferInUArm.Visible == true)
                                    SetPanelPosition(panelWaferInUArm, WTRWafer_RotateNone_Pos);
                                m_eWTRSubState = eWTRSubMotion.Move;
                            }
                            break;
                        case eWTRSubMotion.Move:
                            if (WTRImageMove(WTR_AlignerPos) == eHWResult.OK) {
                                m_eWTRSubState = eWTRSubMotion.Extend;
                            }
                            break;
                        case eWTRSubMotion.Extend:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtrLower1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, Cur_WTRPos.X, Cur_WTRPos.Y - 25, 15, 50);
                            m_eWTRSubState = eWTRSubMotion.WaferOnOff;
                            break;
                        case eWTRSubMotion.WaferOnOff:
                            SetPanelPosition(PanelWaferInLArm, WTRWafer_RotateNone_ExtendPos);
                            panelWaferInAligner.Visible = true;
                            PanelWaferInLArm.Visible = false;
                            m_eWTRSubState = eWTRSubMotion.Retract;
                            break;
                        case eWTRSubMotion.Retract:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, WTR_AlignerPos);
                            SetPanelPosition(PanelWaferInLArm, WTRWafer_RotateNone_Pos);
                            m_eWTRSubState = eWTRSubMotion.Init;
                            m_eWTRState = eWTRSimnulMotion.None;
                            break;
                    }
                    #endregion
                    break;
                case eWTRSimnulMotion.VisionGetUArm:
                    #region VisionGetUArm
                    switch (m_eWTRSubState) {
                        case eWTRSubMotion.Init:
                            if (panelWTR.BackgroundImage != global::ezAuto_EFEM.Properties.Resources.wtr1) 
                            {
                                SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                                SetPanelPosition(panelWTR, Cur_WTRPos);
                            }
                            m_eWTRSubState = eWTRSubMotion.Turn;
                            break;
                        case eWTRSubMotion.Turn:
                            if (WTRImageRotate(RotateFlipType.Rotate270FlipNone) == eHWResult.OK) 
                            {
                                if (PanelWaferInLArm.Visible == true)
                                    SetPanelPosition(PanelWaferInLArm, WTRWafer_Rotate270_Pos);
                                if (panelWaferInUArm.Visible == true)
                                    SetPanelPosition(panelWaferInUArm, WTRWafer_Rotate270_Pos);
                                m_eWTRSubState = eWTRSubMotion.Move;
                            }
                            break;
                        case eWTRSubMotion.Move:
                            if (WTRImageMove(WTR_VisionPos) == eHWResult.OK) 
                            {
                                m_eWTRSubState = eWTRSubMotion.Extend;
                            }
                            break;
                        case eWTRSubMotion.Extend:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtrUpper1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, Cur_WTRPos.X-25, Cur_WTRPos.Y, 15, 50);
                            m_eWTRSubState = eWTRSubMotion.WaferOnOff;
                            break;
                        case eWTRSubMotion.WaferOnOff:
                            SetPanelPosition(panelWaferInUArm, WTRWafer_Rotate270_ExtendPos);
                            panelWaferInVS.Visible = false;
                            panelWaferInUArm.Visible = true;
                            m_eWTRSubState = eWTRSubMotion.Retract;
                            break;
                        case eWTRSubMotion.Retract:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, WTR_VisionPos);
                            SetPanelPosition(panelWaferInUArm, WTRWafer_Rotate270_Pos);
                            m_eWTRSubState = eWTRSubMotion.Init;
                            m_eWTRState = eWTRSimnulMotion.None;
                            break;
                    }
                    #endregion
                    break;
                case eWTRSimnulMotion.VisionPutUArm:
                    #region VisionPutUArm
                    switch (m_eWTRSubState) {
                        case eWTRSubMotion.Init:
                            if (panelWTR.BackgroundImage != global::ezAuto_EFEM.Properties.Resources.wtr1) 
                            {
                                SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                                SetPanelPosition(panelWTR, Cur_WTRPos);
                            }
                            m_eWTRSubState = eWTRSubMotion.Turn;
                            break;
                        case eWTRSubMotion.Turn:
                            if (WTRImageRotate(RotateFlipType.Rotate270FlipNone) == eHWResult.OK) 
                            {
                                if (PanelWaferInLArm.Visible == true)
                                    SetPanelPosition(PanelWaferInLArm, WTRWafer_Rotate270_Pos);
                                if (panelWaferInUArm.Visible == true)
                                    SetPanelPosition(panelWaferInUArm, WTRWafer_Rotate270_Pos);
                                m_eWTRSubState = eWTRSubMotion.Move;
                            }
                            break;
                        case eWTRSubMotion.Move:
                            if (WTRImageMove(WTR_VisionPos) == eHWResult.OK) 
                            {
                                m_eWTRSubState = eWTRSubMotion.Extend;
                            }
                            break;
                        case eWTRSubMotion.Extend:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtrUpper1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, Cur_WTRPos.X-25, Cur_WTRPos.Y, 15, 50);
                            m_eWTRSubState = eWTRSubMotion.WaferOnOff;
                            break;
                        case eWTRSubMotion.WaferOnOff:
                            SetPanelPosition(panelWaferInUArm, WTRWafer_Rotate270_ExtendPos);
                            panelWaferInVS.Visible = true;
                            panelWaferInUArm.Visible = false;
                            m_eWTRSubState = eWTRSubMotion.Retract;
                            break;
                        case eWTRSubMotion.Retract:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, WTR_VisionPos);
                            SetPanelPosition(panelWaferInUArm, WTRWafer_Rotate270_Pos);
                            m_eWTRSubState = eWTRSubMotion.Init;
                            m_eWTRState = eWTRSimnulMotion.None;
                            break;
                    }
                    #endregion
                    break;
                case eWTRSimnulMotion.VisionGetLArm:
                    #region VisionGetLArm
                    switch (m_eWTRSubState) {
                        case eWTRSubMotion.Init:
                            if (panelWTR.BackgroundImage != global::ezAuto_EFEM.Properties.Resources.wtr1) {
                                SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                                SetPanelPosition(panelWTR, Cur_WTRPos);
                            }
                            m_eWTRSubState = eWTRSubMotion.Turn;
                            break;
                        case eWTRSubMotion.Turn:
                            if (WTRImageRotate(RotateFlipType.Rotate270FlipNone) == eHWResult.OK) {
                                if (PanelWaferInLArm.Visible == true)
                                    SetPanelPosition(PanelWaferInLArm, WTRWafer_Rotate270_Pos);
                                if (panelWaferInUArm.Visible == true)
                                    SetPanelPosition(panelWaferInUArm, WTRWafer_Rotate270_Pos);
                                m_eWTRSubState = eWTRSubMotion.Move;
                            }
                            break;
                        case eWTRSubMotion.Move:
                            if (WTRImageMove(WTR_VisionPos) == eHWResult.OK) {
                                m_eWTRSubState = eWTRSubMotion.Extend;
                            }
                            break;
                        case eWTRSubMotion.Extend:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtrLower1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, Cur_WTRPos.X - 25, Cur_WTRPos.Y, 15, 50);
                            m_eWTRSubState = eWTRSubMotion.WaferOnOff;
                            break;
                        case eWTRSubMotion.WaferOnOff:
                            SetPanelPosition(PanelWaferInLArm, WTRWafer_Rotate270_ExtendPos);
                            panelWaferInVS.Visible = false;
                            PanelWaferInLArm.Visible = true;
                            m_eWTRSubState = eWTRSubMotion.Retract;
                            break;
                        case eWTRSubMotion.Retract:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, WTR_VisionPos);
                            SetPanelPosition(PanelWaferInLArm, WTRWafer_Rotate270_Pos);
                            m_eWTRSubState = eWTRSubMotion.Init;
                            m_eWTRState = eWTRSimnulMotion.None;
                            break;
                    }
                    #endregion
                    break;
                case eWTRSimnulMotion.VisionPutLArm:
                    #region VisionPutUArm
                    switch (m_eWTRSubState) {
                        case eWTRSubMotion.Init:
                            if (panelWTR.BackgroundImage != global::ezAuto_EFEM.Properties.Resources.wtr1) {
                                SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                                SetPanelPosition(panelWTR, Cur_WTRPos);
                            }
                            m_eWTRSubState = eWTRSubMotion.Turn;
                            break;
                        case eWTRSubMotion.Turn:
                            if (WTRImageRotate(RotateFlipType.Rotate270FlipNone) == eHWResult.OK) {
                                if (PanelWaferInLArm.Visible == true)
                                    SetPanelPosition(PanelWaferInLArm, WTRWafer_Rotate270_Pos);
                                if (panelWaferInUArm.Visible == true)
                                    SetPanelPosition(panelWaferInUArm, WTRWafer_Rotate270_Pos); 
                                m_eWTRSubState = eWTRSubMotion.Move;
                            }
                            break;
                        case eWTRSubMotion.Move:
                            if (WTRImageMove(WTR_VisionPos) == eHWResult.OK) {
                                m_eWTRSubState = eWTRSubMotion.Extend;
                            }
                            break;
                        case eWTRSubMotion.Extend:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtrUpper1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, Cur_WTRPos.X - 25, Cur_WTRPos.Y, 15, 50);
                            m_eWTRSubState = eWTRSubMotion.WaferOnOff;
                            break;
                        case eWTRSubMotion.WaferOnOff:
                            SetPanelPosition(PanelWaferInLArm, WTRWafer_Rotate270_ExtendPos);
                            panelWaferInVS.Visible = true;
                            PanelWaferInLArm.Visible = false;
                            m_eWTRSubState = eWTRSubMotion.Retract;
                            break;
                        case eWTRSubMotion.Retract:
                            SetImageOnPanel(panelWTR, global::ezAuto_EFEM.Properties.Resources.wtr1, Cur_WTR_Rotate);
                            SetPanelPosition(panelWTR, WTR_VisionPos);
                            SetPanelPosition(PanelWaferInLArm, WTRWafer_Rotate270_Pos);
                            m_eWTRSubState = eWTRSubMotion.Init;
                            m_eWTRState = eWTRSimnulMotion.None;
                            break;
                    }
                    #endregion
                    break;
            }
        }
    }

    public class DoubleBufferPanel : Panel
    {
        public DoubleBufferPanel()
        {
            // Set the value of the double-buffering style bits to true.
            this.SetStyle(ControlStyles.DoubleBuffer |
              ControlStyles.UserPaint |
              ControlStyles.AllPaintingInWmPaint,
              true);
            this.UpdateStyles();
        }
    }

}
