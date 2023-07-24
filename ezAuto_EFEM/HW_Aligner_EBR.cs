﻿using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;
using ezAxis;
using ezCam;
using ezAutoMom;
using ezTools;

namespace ezAuto_EFEM
{
    class HW_Aligner_EBR : HW_Aligner_Mom, Control_Child
    {
        const int c_nBGR = 2;
        enum eError
        {
            CheckWafer,
            Rotate,
            RotateTime,
            Shift, 
            InfoWafer,
            Lift,
            AlignError,
            Vac,
            MoveCam
        }

        enum eAlignMode
        {
            Align,
            Inspect,
            Grab, 
            GrabEdge, 
            GrabRGB,
            Grab3D,
            Grab3Dx10,
            Setup,
            CamSetup,
            LightR,
            LightG,
            LightB,
            EBR_RNR,
            EBR_RNRFile,
        }
        string[] m_strAlignModes = Enum.GetNames(typeof(eAlignMode));
        string m_strAlignMode = eAlignMode.Align.ToString();

        enum eLight
        {
            EBR,
            B,
            G,
            R,
            e3D,
            Off
        };
        int[] m_idLight = new int[Enum.GetNames(typeof(eLight)).Length - 1];
        Light_RGBW m_light = new Light_RGBW();

        int m_msAlign = 10000;
        int m_nAlignCount = 1;

        Axis_Mom m_axisRotate;
        int m_nPulseRotate = 360000;
        int m_dTrigger = 2; 
        int m_nPulseBack = 10000;

        Axis_Mom m_axisShift;
        Axis_Mom m_axisCamZ;
        int m_xReady = 0;
        int m_zReady = 0;

        int[] m_doLift = new int[2] { -1, -1 };
        int[] m_diLift = new int[2] { -1, -1 };
        //int m_doVac = -1;
        //int m_doBlow = -1;
        //int m_diVac = -1;

        //KDG 161229 Modify
        int[] m_doVac = new int[2] { -1, -1 };      //0 : Aligner Chuck, 1 : Aligner Lifter
        int[] m_doBlow = new int[2] { -1, -1 };     //0 : Aligner Chuck, 1 : Aligner Lifter
        int[] m_diVac = new int[2] { -1, -1 };      //0 : Aligner Chuck, 1 : Aligner Lifter

        int m_diWaferCheck = -1; 

        ezCam_CognexOCR m_camOCR = new ezCam_CognexOCR();

        ezCam_Dalsa m_camAlign = new ezCam_Dalsa();
        CPoint m_szCam = new CPoint(); 

        ezView m_viewAlign;
        ezView m_viewOCR;

        CPoint m_szImg = new CPoint(0, 0); 

        double m_degOCR = 90;
        double m_degVision = 0; 

        HW_Aligner_EBR_AOI m_AOI = new HW_Aligner_EBR_AOI(); 
        HW_Aligner_EBR_EBR m_EBR = new HW_Aligner_EBR_EBR();
        HW_Aligner_EBR_Edge m_Edge = new HW_Aligner_EBR_Edge();
        HW_Aligner_EBR_3D m_3D = new HW_Aligner_EBR_3D(); 

        RPoint m_rtSetup = new RPoint(100, 0);

        Button[] m_buttonGrab = new Button[2];

        public override void Init(string id, Auto_Mom auto)
        {
            m_strAlignModel = "EBR";
            m_light.Init("AlignLight", auto);
            base.Init(id, auto);
            m_control.Add(this);
            InitString();
            m_camAlign.Init("Cam_Align", m_auto.ClassLogView());
            m_camOCR.Init(m_id + "OCR", m_log, m_recipe.m_eOCR == Recipe_Mom.eType_OCR.CognexCam);
            m_viewAlign = new ezView("Align", 0, m_auto);
            m_viewOCR = new ezView("OCR", 0, m_auto);
            m_AOI.Init(m_id + "AOI", m_log); 
            m_EBR.Init(m_id + "EBR", m_auto, m_viewAlign, m_AOI);
            m_Edge.Init(m_id + "Edge", m_auto, m_viewAlign, m_AOI, m_EBR);
            m_3D.Init(m_id + "3D", m_auto, m_viewAlign, m_AOI); 
            RunGrid(eGrid.eRegRead);
            m_strAlignMode = eAlignMode.Align.ToString(); 
            RunGrid(eGrid.eInit);
            m_log.m_reg.Read("rtSetup", ref m_rtSetup);
            m_camOCR.Connect();
        }

        public override void ThreadStop()
        {
            base.ThreadStop();
            m_camAlign.ThreadStop();
            m_AOI.ThreadStop(); 
            m_EBR.ThreadStop();
            m_Edge.ThreadStop();
            m_3D.ThreadStop(); 
            m_camOCR.ThreadStop();
            m_viewAlign.ThreadStop();
            m_viewOCR.ThreadStop();
            m_light.ThreadStop();
        }

        public override IDockContent GetContentFromPersistString(string persistString)
        {
            IDockContent iDock;
            iDock= m_EBR.GetContentFromPersistString(persistString); 
            if (iDock != null) return iDock;
            iDock = m_Edge.GetContentFromPersistString(persistString);
            if (iDock != null) return iDock;
            iDock = m_3D.GetContentFromPersistString(persistString);
            if (iDock != null) return iDock;
            if (m_viewAlign.IsPersistString(persistString)) return m_viewAlign;
            if (m_viewOCR.IsPersistString(persistString)) return m_viewOCR;
            if (m_light.IsPersistString(persistString)) return m_light;
            return null;
        }

        public override void ShowAll(DockPanel dockPanel)
        {
            m_viewAlign.Show(dockPanel);
            m_EBR.ShowAll(dockPanel);
            m_Edge.ShowAll(dockPanel);
            m_3D.ShowAll(dockPanel); 
            m_viewOCR.Show(dockPanel);
            m_light.Show(dockPanel);
        }

        public override void ShowDlg(Form parent, ref CPoint cpShow)
        {
            base.ShowDlg(parent, ref cpShow);
            m_EBR.ShowDlg(parent, ref cpShow);
            m_Edge.ShowDlg(parent, ref cpShow);
            m_3D.ShowDlg(parent, ref cpShow); 
        }

        public void InitString()
        {
            InitString(eError.CheckWafer, "Wafer Check Error !!");
            InitString(eError.Rotate, "Rotate AxisMove Error !!");
            InitString(eError.RotateTime, "Rotate Timeout Error !!");
            InitString(eError.Shift, "Shift AxisMove Error !!");
            InitString(eError.InfoWafer, "Info Wafer not Exist !!");
            InitString(eError.Lift, "Lift Up/Down Error !!");
            InitString(eError.AlignError, "Aligner Align Fail !!");
            InitString(eError.Vac, "Aligner Vacuum Fail !!");
            InitString(eError.MoveCam, "Move Camera AxisMove Error !!");
        } 

        void InitString(eError eErr, string str)
        {
            m_log.AddString(str);
            if (m_xGem == null) return;
            m_xGem.AddALID(m_id, (int)eErr, str);
        } 

        void SetAlarm(eAlarm alarm, eError eErr)
        {
            m_work.SetError(alarm, m_log, (int)eErr);
            if (m_xGem == null) return;
            m_xGem.SetAlarm(m_id, (int)eErr);
        } 

        public void ControlGrid(Control_Mom control, ezGrid rGrid, eGrid eMode)
        {
            m_axisRotate = control.AddAxis(rGrid, m_id, "Rotate", "Axis Rotate");
            m_axisShift = control.AddAxis(rGrid, m_id, "Shift", "Axis Shift");
            m_axisCamZ = control.AddAxis(rGrid, m_id, "CamZ", "Axis CamZ");
            control.AddDO(rGrid, ref m_doLift[0], m_id, "Lift_Down", "DO Lift Down");
            control.AddDO(rGrid, ref m_doLift[1], m_id, "Lift_Up", "DO Lift Up");
            control.AddDO(rGrid, ref m_doVac[0], m_id, "Chuck_Vac", "DO Chuck Vac");
            control.AddDO(rGrid, ref m_doVac[1], m_id, "Lifter_Vac", "DO Lifter Vac");
            control.AddDO(rGrid, ref m_doBlow[0], m_id, "Chuck_Blow", "DO Chuck Blow");
            control.AddDO(rGrid, ref m_doBlow[1], m_id, "Lifter_Blow", "DO Lifter Blow");
            control.AddDI(rGrid, ref m_diLift[0], m_id, "Lift_Down", "DI Lift Down");
            control.AddDI(rGrid, ref m_diLift[1], m_id, "Lift_Up", "DI Lift Up");
            control.AddDI(rGrid, ref m_diVac[0], m_id, "Chuck_Vac", "DI Chuck Vac");
            control.AddDI(rGrid, ref m_diVac[1], m_id, "Lifter_Vac", "DI Lifter Vac");
            control.AddDI(rGrid, ref m_diWaferCheck, m_id, "WaferCheck", "DI WaferCheck");
        }

        protected override void RunGrid(eGrid eMode)
        {
            base.RunGrid(eMode);
            m_grid.Set(ref m_strAlignMode, m_strAlignModes, "Mode", "Align", "Align Mode");
            m_grid.Set(ref m_nPulseRotate, "Rotate", "PpR", "Pulse / 1R");
            m_grid.Set(ref m_dTrigger, "Rotate", "Trigger", "dTrigger (pulse)");
            m_grid.Set(ref m_nPulseBack, "Rotate", "Back", "Back Pulse cause Backlash (pulse)");
            m_grid.Set(ref m_degOCR, "Rotate", "OCR", "OCR Position (deg)");
            m_grid.Set(ref m_degVision, "Rotate", "Vision", "Rotate Offset (deg)");
            m_grid.Set(ref m_vRotateBack, "Rotate", "V_Back", "Back Rotate Speed (pulse/sec)");
            m_grid.Set(ref m_xReady, "Shift", "Ready", "Shift Axis Ready Pos (pulse)");
            m_grid.Set(ref m_zReady, "CamZ", "Ready", "CamZ Axis Ready Pos (pulse)");
            m_grid.Set(ref m_msHome, "Time", "Home", "Home Timeout (ms)");
            m_grid.Set(ref m_msAlign, "Time", "Align", "Align Timeout (ms)");
            for (int n = 0; n < m_idLight.Length; n++)
            {
                m_grid.Set(ref m_idLight[n], "Light", ((eLight)n).ToString(), "Align Light ID"); 
            }
            m_AOI.RunGrid(m_grid, eMode); 
            m_EBR.RunGrid(m_grid, eMode);
            m_Edge.RunGrid(m_grid, eMode);
            m_3D.RunGrid(m_grid, eMode); 
            m_camOCR.RunGrid(m_grid);
        }

        public void RecipeGrid(ezGrid rGrid, eGrid eMode, ezJob job)
        {
            rGrid.Set(ref m_bEnableAlign, "Align", "Use", "Use Align");
            rGrid.Set(ref m_nAlignCount, "Align", "Count", "Align Count");
            if (m_EBR.m_strEBRType == "Trim") m_AOI.RecipeGrid(rGrid, eMode, job, false, false, false);
            else m_AOI.RecipeGrid(rGrid, eMode, job);
            m_EBR.RecipeGrid(rGrid, eMode, job);
            m_Edge.RecipeGrid(rGrid, eMode, job);
            m_3D.RecipeGrid(rGrid, eMode, job); 
        }

        public override void Draw(Graphics dc, ezImgView imgView)
        {
            if (imgView.m_id == "Align")
            {
                m_AOI.Draw(dc, imgView);
                if (m_EBR.m_bUseSurface) m_EBR.m_concentric.DrawDefect(dc);
            }
            if (imgView.m_id == "EBR") m_EBR.Draw(dc, imgView);
            if (imgView.m_id == "Edge") m_Edge.Draw(dc, imgView);
        }

        public override void InvalidView()
        {
            m_viewAlign.InvalidView(false);
            m_EBR.InvalidView();
            m_Edge.InvalidView();
            m_3D.InvalidView(); 
            m_viewOCR.InvalidView(false);
        }

        protected override void SetProcess(eProcess run)
        {
            base.SetProcess(run);
        }

        protected override void ProcHome()
        {
            ezStopWatch sw = new ezStopWatch();
            //RunVac(true);
            m_control.WriteOutputBit(m_doVac[0], true);     //KDG 161230 Add
            m_control.WriteOutputBit(m_doVac[1], true);     //KDG 161230 Add
            RunLifterVac(true);

            Thread.Sleep(500);
            m_axisShift.HomeSearch();
            Thread.Sleep(500);
            m_axisRotate.HomeSearch();
            if (m_axisCamZ != null)
            {
                Thread.Sleep(500);
                m_axisCamZ.HomeSearch();
                while (!m_axisCamZ.IsReady() && (sw.Check() <= m_msHome)) Thread.Sleep(10);
            }
            eHWResult eEdgeHome = m_Edge.SearchHome(m_msHome, sw);
            eHWResult e3DHome = m_3D.SearchHome(m_msHome, sw); 
            while (!m_axisRotate.IsReady() && (sw.Check() <= m_msHome)) Thread.Sleep(10);
            while (!m_axisShift.IsReady() && (sw.Check() <= m_msHome)) Thread.Sleep(10);
            if ((sw.Check() >= m_msHome) || (eEdgeHome != eHWResult.OK))
            {
                m_log.Popup("Axis Home Time Out !!");
                SetState(eState.Init);
            }
            else
            {
                m_log.Add("Find Home Finished");
                SetState(eState.RunReady);
            }
            MoveAxis(0); 
            RunVac(false);
            RunLifterVac(false);
            DoLift(false);
        }

        double GetDeg(double fPulse)
        {
            return fPulse * 360 / m_nPulseRotate;
        }

        double m_dRotate = 0; //pt
        void SetRotatePos(double fPos)
        {
            double m_fDiff;
            Thread.Sleep(100);
            m_fDiff = m_axisRotate.GetPos(true) - m_axisRotate.GetPos(false);
            if (Math.Abs(m_fDiff) > 100)
            {
                SetAlarm(eAlarm.Warning, eError.Rotate);
                SetState(eState.Error); // ing 170222
                return;
            }
            m_dRotate += (m_axisRotate.GetPos(true) - fPos); 
            m_axisRotate.SetPos(true, fPos);
            m_axisRotate.SetPos(false, fPos);
            m_log.Add("SetRotatePos : " + fPos.ToString()); 
        }

        double m_vRotateBack = 180000; //pt
        void RotateBack()
        {
            double fPos = m_axisRotate.GetPos(true);
            m_axisRotate.SetPos(true, fPos + m_dRotate);
            m_axisRotate.SetPos(false, fPos + m_dRotate);
            m_dRotate = 0; 
            m_axisRotate.MoveV(fPos, m_vRotateBack); 
        }
        
        protected override void ProcRunReady()
        {
            RotateBack(); 
            SetRotatePos(-m_nPulseBack);
            if (RunLift(false) != eHWResult.OK)
            {
                SetAlarm(eAlarm.Warning, eError.Lift);
                SetState(eState.Error);
            }
            if (m_Edge.MoveCam(false)) // ing 170105
            {
                SetAlarm(eAlarm.Warning, eError.MoveCam);
                SetState(eState.Error);
                RunLight(eLight.Off);
                return;
            }
            if (m_3D.MoveCam(false)) 
            {
                SetAlarm(eAlarm.Warning, eError.MoveCam);
                SetState(eState.Error);
                RunLight(eLight.Off);
                return;
            }
            if (MoveAxis(0)) return;
            if (MoveCamZAxis()) return;
            SetState(eState.Ready);
        }

        protected override void ProcPreProcess()
        {
            if (m_infoWafer == null)
            {
                SetAlarm(eAlarm.Warning, eError.InfoWafer);
                SetState(eState.Error);
                return;
            }
            RunVac(true);
            if (CheckWaferExist() != eHWResult.On)
            {
                SetAlarm(eAlarm.Warning, eError.CheckWafer);
                SetState(eState.Error);
                return;
            }
            RunLight(eLight.EBR);
            SetProcess(eProcess.Align);
        }

        bool RotatePulse(double pulse)
        {
            Thread.Sleep(500); // ing170220
            double posNow = m_axisRotate.GetPos(true);
            while ((posNow - pulse) > (m_nPulseRotate / 2)) posNow -= m_nPulseRotate;
            while ((posNow - pulse) < (-m_nPulseRotate / 2)) posNow += m_nPulseRotate;
            SetRotatePos(posNow); 
            m_axisRotate.Move(pulse);
            if (m_axisRotate.WaitReady())
            {
                SetAlarm(eAlarm.Warning, eError.Rotate);
                SetState(eState.Error);
                return true;
            }
            return false; 
        }

        protected override void RunRotate(double fDeg)
        {
            RotatePulse(fDeg * m_nPulseRotate / 360);
        }

        bool MoveAxis(double pulse)
        {
            m_axisShift.Move(pulse + m_xReady);
            if (m_axisShift.WaitReady())
            {
                SetAlarm(eAlarm.Warning, eError.Shift);
                SetState(eState.Error);
                return true;
            }
            Thread.Sleep(2000); //forget
            return false;
        }

        bool MoveCamZAxis()
        {
            if (m_axisCamZ == null) return false;
            m_axisCamZ.Move(m_zReady);
            if (m_axisCamZ.WaitReady())
            {
                SetAlarm(eAlarm.Warning, eError.MoveCam);
                SetState(eState.Error);
                return true;
            }
            Thread.Sleep(2000); //forget
            return false;
        }

        bool GrabAlign()
        {
            ezStopWatch sw = new ezStopWatch();
            m_camAlign.GetszCam(ref m_szCam);
            m_axisRotate.SetTrigger(-m_dTrigger * m_szCam.y, m_nPulseRotate + m_nPulseBack, m_dTrigger, true, 1);
            if (m_camAlign.Grab(m_viewAlign.ClassImage(), ref m_szImg, m_msAlign, -1, 1)) return true; 
            Thread.Sleep(10);
            while (!m_camAlign.IsGrabDone() && (sw.Check() <= m_msAlign)) Thread.Sleep(1);
            if (sw.Check() >= m_msAlign) return true;
            return false; 
        }

        bool MoveToCenter(double pulseR, double pulseX)
        {
            ezStopWatch sw = new ezStopWatch();
            m_log.Add("MoveToCenter : " + pulseR.ToString() + ", " + pulseX.ToString()); 
            if (m_Edge.SearchHome(m_msHome, sw) != eHWResult.OK) return true;       //KDG 161229 Add Center 진행 전 Edge Cam축 이동 필요
            if (m_3D.MoveCam(false)) return true; 

            if (RotatePulse(pulseR)) return true;
            RunVac(false);
            if (RunLift(true) != eHWResult.OK) return true;
            RotateBack(); 
            if (MoveAxis(pulseX)) return true;
            if (m_axisRotate.WaitReady())
            {
                m_log.Popup("Rotate Back Error !!"); 
                SetAlarm(eAlarm.Warning, eError.RotateTime);
                SetState(eState.Error);
                return true;
            }
            if (RunLift(false) != eHWResult.OK) return true;
            RunVac(true);
            MoveAxis(0); 
            return false; 
        }

        const int c_rShift = 1000; 

        bool MoveToCenter()
        {
            return MoveToCenter((-m_AOI.m_rtFFT.y + m_rtSetup.y) * m_nPulseRotate / 360, -c_rShift * m_AOI.m_rtFFT.x / m_rtSetup.x); 
        }

        void RunAlignSetup()
        {
            //if (m_control.GetInputBit(m_diWaferCheck) == false) return;
            m_log.Add("Start RunAlignSetup"); 
            RunLight(eLight.EBR);
            RunVac(true);

            SetRotatePos(-m_nPulseBack);
            m_axisRotate.Move(m_nPulseRotate + m_nPulseBack);
            if (GrabAlign())
            {
                SetAlarm(eAlarm.Warning, eError.RotateTime);
                SetState(eState.Error);
                return;
            }

            m_AOI.Inspect(m_viewAlign.ClassImage());
            RPoint xyShift0 = m_AOI.m_xyFFT;
            if (m_axisRotate.WaitReady())
            {
                SetAlarm(eAlarm.Warning, eError.Rotate);
                SetState(eState.Error);
                return; 
            }

            if (MoveToCenter(0, c_rShift)) return;

            RotatePulse(-m_nPulseBack);
            m_axisRotate.Move(m_nPulseRotate + m_nPulseBack);
            if (GrabAlign())
            {
                SetAlarm(eAlarm.Warning, eError.RotateTime);
                SetState(eState.Error);
                return;
            }
            m_viewAlign.m_imgView.FileSave("D:\\ShiftImage.bmp");
            m_AOI.Inspect(m_viewAlign.ClassImage());
            RPoint xyShift1 = m_AOI.m_xyFFT;
            if (m_axisRotate.WaitReady())
            {
                SetAlarm(eAlarm.Warning, eError.Rotate);
                SetState(eState.Error);
                return;
            }

            RPoint xyDelta = xyShift1 - xyShift0;
            m_rtSetup.x = Math.Sqrt(xyDelta.x * xyDelta.x + xyDelta.y * xyDelta.y);
            m_rtSetup.y = 180 * Math.Atan2(xyDelta.y, xyDelta.x) / Math.PI;
            m_log.Add("rtSetup = " + m_rtSetup.ToString()); 
            m_log.m_reg.Write("rtSetup", m_rtSetup); 

            if (MoveToCenter()) return;

            RotatePulse(-m_nPulseBack);
            m_axisRotate.Move(m_nPulseRotate + m_nPulseBack);
            if (GrabAlign())
            {
                SetAlarm(eAlarm.Warning, eError.RotateTime);
                SetState(eState.Error);
                return;
            }

            RunLight(eLight.Off);
            m_AOI.Inspect(m_viewAlign.ClassImage());
            if (m_axisRotate.WaitReady())
            {
                SetAlarm(eAlarm.Warning, eError.Rotate);
                SetState(eState.Error);
                return; 
            }
            m_AOI.SetupNotchPattern(); // ing 170302
        }

        void SaveImg()
        {
            string strPath = "c:\\TestImg";
            Directory.CreateDirectory(strPath);
            strPath += "\\" + m_infoWafer.m_nLoadPort.ToString() + "_" + m_infoWafer.m_nSlot.ToString("00");
            Directory.CreateDirectory(strPath);
            m_viewAlign.ClassImage().FileSave(strPath + ".jpg"); 
        }

        protected override void TestRun()
        {
            ezStopWatch sw = new ezStopWatch();
            if (m_strAlignMode == eAlignMode.Align.ToString() && !m_work.IsRun())
            {
                if (RunAlign(sw)) return;

                RunEdge();
                m_log.Add("Edge Grab Done : " + sw.Check().ToString());

                Run3D();
                m_log.Add("3D Grab Done : " + sw.Check().ToString());
                SetProcess(eProcess.BCR);
                return;
            }
            if (m_strAlignMode == eAlignMode.Inspect.ToString() && !m_work.IsRun())
            {
                m_AOI.Inspect(m_viewAlign.m_imgView.m_pImg, m_EBR.m_EBRGraph.m_umX);
                m_auto.Invalidate(0);
                return;
            }

            if (m_strAlignMode == eAlignMode.Grab.ToString() && !m_work.IsRun())
            {
                RunLight(eLight.EBR);
                SetRotatePos(-m_nPulseBack);
                m_axisRotate.Move(m_nPulseRotate + m_nPulseBack);
                if (GrabAlign())
                {
                    SetAlarm(eAlarm.Warning, eError.RotateTime);
                    SetState(eState.Error);
                    RunLight(eLight.Off);
                    return;
                }
                if (m_axisRotate.WaitReady())
                {
                    SetAlarm(eAlarm.Warning, eError.Rotate);
                    SetState(eState.Error);
                }
                RunLight(eLight.Off);
                SetState(eState.RunReady);
                return;
            }

            if (m_strAlignMode == eAlignMode.GrabEdge.ToString() && !m_work.IsRun())
            {
                m_camAlign.GetszCam(ref m_szCam);
                SetRotatePos(-m_nPulseBack);
                RunLight(eLight.R);
                m_light.WriteRGB('r');
                if (m_Edge.MoveCam(true)) // ing 170105
                {
                    SetAlarm(eAlarm.Warning, eError.MoveCam);
                    SetState(eState.Error);
                    RunLight(eLight.Off);
                    return;
                }
                if (MoveAxis(m_Edge.m_posShift - m_xReady)) return;
                m_axisRotate.SetTrigger(-m_dTrigger * m_szCam.y, m_nPulseRotate + m_nPulseBack, m_dTrigger, true, 1);
                m_axisRotate.Move(m_nPulseRotate + m_nPulseBack);
                if (m_Edge.Grab(0, m_msAlign))
                {
                    SetAlarm(eAlarm.Warning, eError.RotateTime);
                    SetState(eState.Error);
                    RunLight(eLight.Off);
                    if (m_Edge.MoveCam(false)) // ing 170105
                    {
                        SetAlarm(eAlarm.Warning, eError.MoveCam);
                        SetState(eState.Error);
                        RunLight(eLight.Off);
                        return;
                    }
                    if (MoveAxis(0)) return;
                    return;
                }
                if (m_axisRotate.WaitReady())
                {
                    SetAlarm(eAlarm.Warning, eError.Rotate);
                    SetState(eState.Error);
                    if (m_Edge.MoveCam(false)) // ing 170105
                    {
                        SetAlarm(eAlarm.Warning, eError.MoveCam);
                        SetState(eState.Error);
                        RunLight(eLight.Off);
                        return;
                    }
                    if (MoveAxis(0)) return;
                    return;
                }
                if (m_Edge.MoveCam(false)) // ing 170105
                {
                    SetAlarm(eAlarm.Warning, eError.MoveCam);
                    SetState(eState.Error);
                    RunLight(eLight.Off);
                    return;
                }
                if (MoveAxis(0)) return;
                SetState(eState.RunReady);
                return;
            }

            if (m_strAlignMode == eAlignMode.GrabRGB.ToString() && !m_work.IsRun())
            {
                RunEdge();
                m_log.Add("Edge Grab Done : " + sw.Check().ToString());
                SetState(eState.RunReady);
                return;
            }

            if (m_strAlignMode == eAlignMode.Grab3D.ToString() && !m_work.IsRun())
            {
                RotatePulse(-m_nPulseBack);
                Run3D();
                RotatePulse(-m_nPulseBack);
                m_log.Add("3D Grab Done : " + sw.Check().ToString());
                SetState(eState.RunReady);
                return;
            }

            if (m_strAlignMode == eAlignMode.Grab3Dx10.ToString() && !m_work.IsRun())
            {
                double[,] fH = new double[10, m_3D.m_3DGraph.m_nInspect];
                double[,] fStep = new double[10, m_3D.m_3DGraph.m_nInspect];
                RotatePulse(-m_nPulseBack);
                for (int n = 0; n < 10; n++)
                {
                    Run3D();
                    RotatePulse(-m_nPulseBack);
                    for (int i = 0; i < m_3D.m_3DGraph.m_nInspect; i++) fH[n, i] = m_3D.m_3DGraph.m_lHeight[i];
                    m_log.Add("3D Grab Done : " + n.ToString() + "/10  " + sw.Check().ToString());
                }
                StreamWriter fs = new StreamWriter(new FileStream("c:\\TestImg\\3D\\H.txt", FileMode.Create));
                for (int i = 0; i < m_3D.m_3DGraph.m_nInspect; i++)
                {
                    for (int n = 0; n < 10; n++) fs.Write(fH[n, i].ToString(".00") + ", ");
                    for (int n = 0; n < 10; n++) fs.Write(fStep[n, i].ToString(".00") + ", ");
                    fs.WriteLine();
                }
                fs.Close();

                SetState(eState.RunReady);
                return;
            }

            if (m_strAlignMode == eAlignMode.Setup.ToString() && !m_work.IsRun())
            {
                RunAlignSetup();
                SetState(eState.RunReady);
                return;
            }

            if (m_strAlignMode == eAlignMode.CamSetup.ToString() && !m_work.IsRun())
            {
                m_camAlign.ShowDialog();
                SetState(eState.RunReady);
                return;
            }

            if (m_strAlignMode == eAlignMode.EBR_RNR.ToString() && !m_work.IsRun())
            {
                int nRound = 20;
                RunLight(eLight.EBR);
                RunVac(true);
                double[,] aEBR = new double[nRound, m_EBR.m_EBRGraph.m_nInspect];
                double[,] aBevel = new double[nRound, m_EBR.m_EBRGraph.m_nInspect];
                double[,] aDiffNotch = new double[3, nRound];
                for (int n = 0; n < nRound; n++)
                {
                    RotatePulse(-m_nPulseBack);
                    RunGrab(sw);
                    m_AOI.Inspect(m_viewAlign.ClassImage(), m_EBR.m_EBRGraph.m_umX);
                    m_EBR.SetInfoWafer(m_infoWafer); // ing 161221
                    m_EBR.Inspect();
                    m_log.Add("EBR Grab Done : " + sw.Check().ToString());
                    for (int m = 0; m < m_EBR.m_EBRGraph.m_nInspect; m++)
                    {
                        aEBR[n, m] = m_EBR.m_EBRGraph.m_lEBR[m];
                        aBevel[n, m] = m_EBR.m_EBRGraph.m_lBevel[m];
                    }
                    aDiffNotch[0, n] = m_AOI.m_dDiffNotch;
                    aDiffNotch[1, n] = m_AOI.m_cpNotch.y;
                    aDiffNotch[2, n] = m_AOI.m_cpTrimNotch.y;
                    m_viewAlign.ClassImage().FileSave("d:\\EBR_RNR" + n.ToString("00") + ".bmp");
                }
                StreamWriter fs = new StreamWriter(new FileStream("d:\\EBR_RNR.txt", FileMode.Create));
                for (int m = 0; m < m_EBR.m_EBRGraph.m_nInspect; m++)
                {
                    for (int n = 0; n < nRound - 1; n++) fs.Write(aEBR[n, m] + ", ");
                    fs.WriteLine(aEBR[nRound - 1, m]);
                }
                for (int m = 0; m < m_EBR.m_EBRGraph.m_nInspect; m++)
                {
                    for (int n = 0; n < nRound - 1; n++) fs.Write(aBevel[n, m] + ", ");
                    fs.WriteLine(aBevel[nRound - 1, m]);
                }
                if (m_AOI.m_bTrimNotch)
                {
                    for (int n = 0; n < nRound; n++)
                    {
                        fs.Write(aDiffNotch[0, n] + ", ");
                        fs.Write(aDiffNotch[1, n] + ", ");
                        fs.WriteLine(aDiffNotch[2, n] + ", ");
                    }
                }
                fs.Close();
                SetState(eState.RunReady);
                RunLight(eLight.Off);
                return;
            }

            if (m_strAlignMode == eAlignMode.EBR_RNRFile.ToString() && !m_work.IsRun())
            {
                int nRound = 10;
                double[,] aEBR = new double[nRound, m_EBR.m_EBRGraph.m_nInspect];
                double[,] aBevel = new double[nRound, m_EBR.m_EBRGraph.m_nInspect];
                StreamWriter fs = new StreamWriter(new FileStream("d:\\EBR_RNR.txt", FileMode.Create));
                for (int n = 0; n < nRound; n++)
                {
                    m_viewAlign.m_imgView.FileOpen("d:\\EBRRNR\\EBR_RNR" + n.ToString("00") + ".bmp");
                    m_AOI.Inspect(m_viewAlign.ClassImage(), m_EBR.m_EBRGraph.m_umX);
                    m_infoWafer.m_strWaferID = n.ToString();
                    m_EBR.SetInfoWafer(m_infoWafer); // ing 161221
                    m_EBR.Inspect();
                    m_EBR.m_EBRGraph.InvalidData(m_EBR.m_strEBRType == "Trim");
                    m_EBR.m_EBRGraph.chartEBR.Invalidate();
                    m_EBR.m_EBRGraph.listViewEBR.Invalidate();
                    m_EBR.m_EBRGraph.Invalidate();
                    m_EBR.SaveResult();
                    m_log.Add("EBR Grab Done : " + sw.Check().ToString());
                    for (int m = 0; m < m_EBR.m_EBRGraph.m_nInspect; m++)
                    {
                        aEBR[n, m] = m_EBR.m_EBRGraph.m_lEBR[m];
                        aBevel[n, m] = m_EBR.m_EBRGraph.m_lBevel[m];
                    }
                    fs.Write(m_AOI.m_cpNotch.y + ", ");
                }
                fs.WriteLine();
                fs.WriteLine();
                for (int m = 0; m < m_EBR.m_EBRGraph.m_nInspect; m++)
                {
                    for (int n = 0; n < nRound - 1; n++) fs.Write(aEBR[n, m] + ", ");
                    fs.WriteLine(aEBR[nRound - 1, m]);
                }
                fs.WriteLine();
                for (int m = 0; m < m_EBR.m_EBRGraph.m_nInspect; m++)
                {
                    for (int n = 0; n < nRound - 1; n++) fs.Write(aBevel[n, m] + ", ");
                    fs.WriteLine(aBevel[nRound - 1, m]);
                }
                fs.Close();
                return;
            }

            if (m_strAlignMode == eAlignMode.LightR.ToString() && !m_work.IsRun()) { m_light.WriteRGB('r'); RunLight(eLight.R); SetState(eState.RunReady); return; } // ing 170105
            if (m_strAlignMode == eAlignMode.LightG.ToString() && !m_work.IsRun()) { m_light.WriteRGB('g'); RunLight(eLight.G); SetState(eState.RunReady); return; }
            if (m_strAlignMode == eAlignMode.LightB.ToString() && !m_work.IsRun()) { m_light.WriteRGB('b'); RunLight(eLight.B); SetState(eState.RunReady); return; }

        }

        protected override void ProcAlign()
        {
            ezStopWatch sw = new ezStopWatch();
            if (m_infoWafer == null) return;

            if (m_AOI.m_bFindNotchNoAlign)
            {
                if (RunGrabAndRecovery(sw)) return;
                m_EBR.SetKlarf(ref m_infoWafer);
                //m_EBR.m_strFile = "LotEnd_" + m_auto.ClassRecipe().m_sRecipe + "_" + m_EBR.m_klarfChipping.m_strLotID + "_00-" + m_EBR.m_klarfChipping.m_nSlot.ToString("00");
                //m_EBR.m_klarfChipping.m_strTiffFileName = m_auto.ClassRecipe().m_sRecipe + "_" + m_EBR.m_klarfChipping.m_strLotID + "_00-" + m_EBR.m_klarfChipping.m_nSlot.ToString("00") + ".tif";
                m_EBR.m_strFile = "LotEnd_" + m_auto.ClassRecipe().m_sRecipe + "_" + m_EBR.m_klarfChipping.m_strCSTID + "_00-" + m_EBR.m_klarfChipping.m_nSlot.ToString("00");
                m_EBR.m_klarfChipping.m_strTiffFileName = m_auto.ClassRecipe().m_sRecipe + "_" + m_EBR.m_klarfChipping.m_strCSTID + "_00-" + m_EBR.m_klarfChipping.m_nSlot.ToString("00") + ".tif";
                string strDate = DateTime.Now.Date.Year.ToString() + DateTime.Now.Date.Month.ToString() + DateTime.Now.Date.Day.ToString();
                Directory.CreateDirectory(m_EBR.m_strPathBackupChipping);
                Directory.CreateDirectory(m_EBR.m_strPathBackupChipping + "\\" + strDate);
                //string strPath = m_EBR.m_strPathChipping + "\\" + m_auto.ClassRecipe().m_sRecipe + "_" + m_EBR.m_klarfChipping.m_strLotID + "_00-" + m_EBR.m_klarfChipping.m_nSlot.ToString("00");
                //string strPathBackup = m_EBR.m_strPathBackupChipping + "\\" + strDate + "\\" + m_auto.ClassRecipe().m_sRecipe + "_" + m_EBR.m_klarfChipping.m_strLotID + "_00-" + m_EBR.m_klarfChipping.m_nSlot.ToString("00");
                string strPath = m_EBR.m_strPathChipping + "\\" + m_auto.ClassRecipe().m_sRecipe + "_" + m_EBR.m_klarfChipping.m_strCSTID + "_00-" + m_EBR.m_klarfChipping.m_nSlot.ToString("00");
                string strPathBackup = m_EBR.m_strPathBackupChipping + "\\" + strDate + "\\" + m_auto.ClassRecipe().m_sRecipe + "_" + m_EBR.m_klarfChipping.m_strCSTID + "_00-" + m_EBR.m_klarfChipping.m_nSlot.ToString("00");
                m_EBR.m_klarfChipping.SaveKlarf(strPath, true, (m_AOI.GetNotchAngle() + (double)(m_nPulseBack * 360 / m_nPulseRotate)) % 360.0);
                m_EBR.m_klarfChipping.SaveKlarf(strPathBackup, true, (m_AOI.GetNotchAngle() + (double)(m_nPulseBack * 360 / m_nPulseRotate)) % 360.0);
                SetState(eState.Done);
                return;
            }

            if (RunAlign(sw)) return; 

            RunEdge();
            m_log.Add("Edge Grab Done : " + sw.Check().ToString());

            Run3D();
            m_log.Add("3D Grab Done : " + sw.Check().ToString());
            SetProcess(eProcess.BCR); 
        }

        bool RunAlign(ezStopWatch sw)
        {
            if (m_nAlignCount < 1)
            {
                m_log.Popup("Align Count Is Low !!");
                return true;
            }

            RunLight(eLight.EBR);
            RunVac(true);
            
            for (int n = 0; n < m_nAlignCount; n++)
            {
                SetRotatePos(-m_nPulseBack);
                if (RunGrab(sw)) return true;

                if (MoveToCenter())
                {
                    SetAlarm(eAlarm.Warning, eError.AlignError);
                    SetState(eState.Error);
                    RunLight(eLight.Off);
                    return true;
                }
                m_log.Add("Move To Center : " + sw.Check().ToString());
            }
            
            SetRotatePos(-m_nPulseBack);
            if (RunGrab(sw)) return true; 

            RunLight(eLight.Off);

            ezStopWatch swSave = new ezStopWatch();
            while (swSave.Check() < m_EBR.m_msSaveTimeout)
            {
                Thread.Sleep(10);
                if (!m_EBR.m_klarf.m_bBusy && !m_EBR.m_klarfChipping.m_bBusy) break;
            }
            if (m_EBR.m_klarf.m_bBusy || m_EBR.m_klarfChipping.m_bBusy)
            {
                m_log.Popup("Klarf File Save Fail, Can not Inspect EBR !!");
                return true;
            }

            m_AOI.Inspect(m_viewAlign.ClassImage(), m_EBR.m_EBRGraph.m_umX);
            m_EBR.SetInfoWafer(m_infoWafer); // ing 161221
            m_EBR.StartInspect();
            m_log.Add("EBR Grab Done : " + sw.Check().ToString());

            return false; 
        }

        bool RunGrabAndRecovery(ezStopWatch sw) // ing 170531
        {
            RunLight(eLight.EBR);
            RunVac(true);

            SetRotatePos(-m_nPulseBack);
            if (RunGrab(sw)) return true;

            m_axisRotate.Move(-m_nPulseBack);
            RunLight(eLight.Off);
            if (m_axisRotate.WaitReady())
            {
                SetAlarm(eAlarm.Warning, eError.Rotate);
                SetState(eState.Error);
                return true;
            }
            m_AOI.Inspect(m_viewAlign.ClassImage(), m_EBR.m_EBRGraph.m_umX);
            m_log.Add("Wafer Grab And Recovery : " + sw.Check().ToString());
            m_log.Add("NotchAngle : " + ((m_AOI.GetNotchAngle() + (double)(m_nPulseBack * 360 / m_nPulseRotate)) % 360.0).ToString("0.00"));
            return false;
        }

        bool RunGrab(ezStopWatch sw)
        {
            m_axisRotate.Move(m_nPulseRotate + m_nPulseBack);
            if (GrabAlign())
            {
                SetAlarm(eAlarm.Warning, eError.RotateTime);
                SetState(eState.Error);
                RunLight(eLight.Off);
                return true;
            }

            m_AOI.Inspect(m_viewAlign.ClassImage(), m_EBR.m_EBRGraph.m_umX);
            if (m_axisRotate.WaitReady())
            {
                SetAlarm(eAlarm.Warning, eError.Rotate);
                SetState(eState.Error);
                return true;
            }
            try
            {
                m_viewAlign.m_imgView.m_pImg.FileSave("D:\\Aligner\\" + m_infoWafer.m_nSlot.ToString("00") + ".bmp");
            }
            catch (Exception ex)
            {
                m_log.Add(ex.Message);
            }
            return false; 
        }

        bool RunEdge(int nIndex)
        {
            int nTrigger = m_nPulseRotate * (3 * nIndex + 1) / 2; 
            m_axisRotate.SetTrigger(nTrigger - m_dTrigger * m_szCam.y, nTrigger + m_nPulseRotate + m_nPulseBack, m_dTrigger, true, 1);
            if (m_Edge.Grab(nIndex, m_msAlign))
            {
                SetAlarm(eAlarm.Warning, eError.RotateTime);
                SetState(eState.Error);
                RunLight(eLight.Off);
                return true;
            }
            ezStopWatch swSave = new ezStopWatch(); // ing 170525
            while (swSave.Check() < m_EBR.m_msSaveTimeout)
            {
                Thread.Sleep(10);
                if (!m_Edge.m_klarf.m_bBusy) break;
            }
            if (m_Edge.m_klarf.m_bBusy)
            {
                m_log.Popup("Klarf File Save Fail, Can not Inspect Edge !!");
                return true;
            }
            m_Edge.StartInspect(nIndex, m_infoWafer);
            return false; 
        }

        void RunEdge()
        {
            if (m_Edge.m_bUse == false) return;

            if (m_Edge.MoveCam(true)) // ing 170105
            {
                SetAlarm(eAlarm.Warning, eError.MoveCam);
                SetState(eState.Error);
                RunLight(eLight.Off);
                return;
            }
            if (MoveAxis(m_Edge.m_posShift - m_xReady)) return;
            
            m_Edge.ReadyInspect(); 

            int nPulse2 = m_nPulseRotate / 2;
            double posNow = m_axisRotate.GetPos(true);
            while (posNow > (nPulse2 - m_nPulseBack)) posNow -= m_nPulseRotate;
            SetRotatePos(posNow);
            m_axisRotate.Move(9 * nPulse2 + m_nPulseBack);

            RunLight(eLight.B);
            m_light.WriteRGB('b');
            if (RunEdge(0)) return; 

            RunLight(eLight.G);
            m_light.WriteRGB('g');
            if (RunEdge(1)) return; 

            RunLight(eLight.R);
            m_light.WriteRGB('r');
            m_axisRotate.SetTrigger(7 * nPulse2, 9 * nPulse2, m_dTrigger, true, 1);
            if (RunEdge(2)) return;
            m_EBR.m_klarfChipping.m_img[1] = m_Edge.m_aImg[c_nBGR]; // ing 170419
            RunLight(eLight.Off);
            if (m_axisRotate.WaitReady())
            {
                SetAlarm(eAlarm.Warning, eError.Rotate);
                SetState(eState.Error);
                return; 
            }

            if (m_Edge.MoveCam(false)) // ing 170105
            {
                SetAlarm(eAlarm.Warning, eError.MoveCam);
                SetState(eState.Error);
                RunLight(eLight.Off);
                return;
            }
            if (MoveAxis(0)) return;

        }

        bool Run3D()
        {
            if (m_3D.m_bUse == false) return false;
            RunLight(eLight.e3D);

            RunVac(true);
            if (m_3D.MoveCam(true))
            {
                SetAlarm(eAlarm.Warning, eError.MoveCam);
                SetState(eState.Error);
                RunLight(eLight.Off);
                return true;
            }
            if (MoveAxis(m_Edge.m_posShift - m_xReady)) return true; //forget

            m_3D.ReadyInspect(m_nPulseRotate);
            int nPulse2 = m_nPulseRotate / 2;
            double posNow = m_axisRotate.GetPos(true);
            while (posNow > (nPulse2 - m_nPulseBack)) posNow -= m_nPulseRotate;
            SetRotatePos(posNow);
            m_axisRotate.SetTrigger(0, m_nPulseRotate + m_nPulseBack, m_3D.m_dTrigger, true, 4);
            m_axisRotate.Move(m_nPulseRotate + m_nPulseBack);
            if (m_3D.Grab()) return true;
            RunLight(eLight.Off);
            m_3D.RunInspect(m_infoWafer); 
            if (m_axisRotate.WaitReady())
            {
                SetAlarm(eAlarm.Warning, eError.Rotate);
                SetState(eState.Error);
                return true;
            }

            if (m_3D.MoveCam(false))
            {
                SetAlarm(eAlarm.Warning, eError.MoveCam);
                SetState(eState.Error);
                RunLight(eLight.Off);
                return true;
            }
            if (MoveAxis(0)) return true;
            
            m_log.Add("Run3D Done");
            return false; 
        }

        protected override void ProcBCR()
        {
            SetProcess(eProcess.OCR);
        }

        protected override void ProcOCR()
        {
            if (m_infoWafer != null)
            {
                if (m_infoWafer.m_bUseOCR)
                {
                    RunLight(eLight.Off);
                    RunRotate(m_infoWafer.m_fAngleOCR + m_degOCR + m_AOI.GetNotchAngle());
                    m_camOCR.ReadOCR(m_viewOCR.m_imgView.m_pImg);
                    if (m_camOCR.bOCRDone)  
                    {
                        m_infoWafer.m_strWaferID = m_camOCR.m_strOCR.Replace("\r\n", "");
                        if ((m_infoWafer.m_strWaferID.Length == 0) || (m_infoWafer.m_strWaferID.Substring(0, 1) == "*") || (m_camOCR.m_scoreOCR < m_infoWafer.m_fOCRReadScore))  //KDG 161028 OCR Score를 가져오는 부분 없어서 우선 원복
                        {
                            ChangeCode(m_viewOCR); 
                        }
                        SetProcess(eProcess.Edge);
                    }
                }
                else 
                {
                    m_infoWafer.m_strWaferID = m_infoWafer.m_nSlot.ToString();
                    SetProcess(eProcess.Edge);
                }
            }
        }

        protected override void ProcEdge()
        {
            SetProcess(eProcess.Rotate);
        }

        protected override void ProcRotate()
        {
            if (m_infoWafer == null) return;
            RunLight(eLight.Off);
            RunRotate(m_infoWafer.m_fAngleAlign + m_degVision + m_AOI.GetNotchAngle());
            m_log.Add("Rotate : " + m_AOI.GetNotchAngle().ToString());
            Thread.Sleep(500);
            RunVac(false);
            RunLift(false);
            SetState(eState.Done);
        }

        protected override void ProcError()
        {
        }

        protected override void RunTimer_200ms()
        {
        }

        public override bool RunVac(bool bVac)
        {
            m_control.WriteOutputBit(m_doVac[0], bVac);
            Thread.Sleep(100);
            if (m_control.WaitInputBit(m_diVac[0], bVac))
            {
                SetAlarm(eAlarm.Warning, eError.Vac);
                SetState(eState.Error);
                return true; 
            }
            return false; 
        }

        public override bool RunLifterVac(bool bVac)        //KDG 161229 Add Lifter Vacuum
        {
            m_control.WriteOutputBit(m_doVac[1], bVac);
            Thread.Sleep(100);
            return false;
        }

        public override eHWResult CheckWaferExist(bool bVacCheck = false)
        {
            bool bExist = m_control.GetInputBit(m_diWaferCheck);
            bool bVacOn = false;
            if (bVacCheck && m_infoWafer != null)
            {
                    bVacOn = m_control.GetOutputBit(m_doVac[0]);
                    m_control.WriteOutputBit(m_doVac[0], true);
                    Thread.Sleep(500);
                    bExist = m_control.GetInputBit(m_diVac[0]);
                    m_control.WriteOutputBit(m_doVac[0], bVacOn);
            }
            //if ((m_infoWafer != null) && (bExist == false)) bExist = CheckWaferExistCam(); 
            if (bExist) m_waferExist = eHWResult.On;
            else m_waferExist = eHWResult.Off;
            return m_waferExist; 
        }

        public override bool IsReady(Info_Wafer infoWafer)
        {
            if (!IsLiftDown()) return false;
            return (InfoWafer == null); 
        }

        public override void Grab()
        {
        }

        void RunLight(eLight light)
        {
            if (light == eLight.Off)
            {
                m_light.LightOn(-1, false);
                return;
            }
            m_light.LightOn(m_idLight[(int)light], true);
        }
        bool IsLiftUp()
        {
            if (!m_control.GetInputBit(m_diLift[0]) && m_control.GetInputBit(m_diLift[1])) return true;
            return false;
        }

        bool IsLiftDown()
        {
            if (!m_control.GetInputBit(m_diLift[1]) && m_control.GetInputBit(m_diLift[0])) return true;
            return false;
        }

        public override eHWResult RunLift(bool bUp)
        {
            DoLift(bUp);
            if (WaitLift(bUp) != eHWResult.OK)
            {
                DoLift(false);
                Thread.Sleep(3000);
                return eHWResult.Error;  
            }
            Thread.Sleep(1000);
            return eHWResult.OK;
        }

        void DoLift(bool bUp)
        {
            //KDG 161229 Add Lifter 동작전 Vacuum 하도록 추가
            RunLifterVac(bUp);
            Thread.Sleep(500); 

            m_control.WriteOutputBit(m_doLift[1], bUp);
            m_control.WriteOutputBit(m_doLift[0], !bUp);
        }

        eHWResult WaitLift(bool bUp)
        {
            if (m_control.WaitInputBit(m_diLift[1], bUp)) return eHWResult.Error;
            if (m_control.WaitInputBit(m_diLift[0], !bUp)) return eHWResult.Error;
            return eHWResult.OK; 
        }

        void ChangeCode(ezView view)
        {
            m_work.WorkerBuzzerOn();
            HW_Aligner_ATI_Code code = new HW_Aligner_ATI_Code(view.m_imgView);
            if (code.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            m_infoWafer.m_strWaferID = code.GetCode();
            m_work.WorkerBuzzerOff();
        }

        public override void LotStart()
        {
            m_EBR.LotStart();
            m_Edge.LotStart(); // ing 170426
            m_3D.LotStart(); 
        }

        public override void LotEnd()
        {
            m_EBR.LotEnd();
            m_Edge.LotEnd(); // ing 170426
            m_3D.LotEnd(); 
        }

        public override ezView ClassView()
        {
            return m_viewAlign;
        }
    }
}
