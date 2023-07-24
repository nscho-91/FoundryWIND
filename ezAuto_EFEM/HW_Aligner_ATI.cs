using System;
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
    class HW_Aligner_ATI : HW_Aligner_Mom, Control_Child
    {
        enum eError
        {
            CheckWafer,
            Rotate,
            RotateTime, 
            InfoWafer,
            MoveCam,
            Lift,
            AlignError,
            EdgeInspError
        }

        enum eAlignMode
        {
            Align,
            Inspect,
            InspectAll,
//            CheckWaferCam,
//            ReadWaferCam,
//            WriteWaferCam,
//            InspectWaferCam,
            SaveNotch,
			Setup,
        }
        string[] m_strAlignModes = Enum.GetNames(typeof(eAlignMode));
        string m_strAlignMode = eAlignMode.Align.ToString();

        public bool m_bAlignResult = false; 
        
        int m_msAlign = 10000;

        public bool m_bEnableAlignerEdge = false;
        
        Axis_Mom m_axisRotate;
        int m_nPulseRotate = 40000;
        int m_nPulseBack = 500;
        int m_degReady = 0; 
        double m_fSlowRotate = 0.5;
        double m_fPosAlign = 0;

        Axis_Mom m_axisCamX;
        int[] m_xAlign = new int[(int)Wafer_Size.eSize.Empty];
        int[] m_xBCR = new int[(int)Wafer_Size.eSize.Empty];
        double[] m_fUMpPulse = new double[2] { 1, 1 }; 

        Axis_Mom m_axisCamZ;
        int[] m_zCam = new int[(int)Wafer_Size.eSize.Empty];

        Axis_Mom m_axisOCR;
        int[] m_xOCR = new int[(int)Wafer_Size.eSize.Empty];
        int[] m_xOCRBtm = new int[(int)Wafer_Size.eSize.Empty];

        ezCam_Basler_Simple m_camAlign = new ezCam_Basler_Simple();
        int[] m_nGrab = new int[(int)Wafer_Size.eSize.Empty];
        CPoint m_szCam = new CPoint(1400, 512);
        CPoint m_cpImgOffset = new CPoint(0, 0);
        private int m_nCamResolution = 30; // BHJ 191128 add

        ezCam_CognexOCR m_camOCR = new ezCam_CognexOCR();
        ezCam_CognexOCR m_camOCRBottom = new ezCam_CognexOCR();

        Cognex_BCR m_azBCR = new Cognex_BCR(); 
        Cognex_BCR2D m_azBCR2D = new Cognex_BCR2D();
        Cognex_OCR m_azOCR; // ing 161210 for MSB RingFrame

        bool[] m_bLightAlign = new bool[3] { false, false, false }; 
        int[] m_doLightAlign = new int[3] { -1, -1, -1 }; // WW RF

        int[] m_doLift = new int[4] { -1, -1, -1, -1 }; // Down Up  //KDG 161006 Modify 2->4
        int[] m_diLift = new int[4] { -1, -1, -1, -1 };             //KDG 161006 Modify 2->4
        int[] m_doVac = new int[3] { -1, -1, -1 }; // WW RF Edge    KDG 160921 Add
        int[] m_doBlow = new int[3] { -1, -1, -1 }; // WW RF Edge   KDG 160921 Add

        int m_diWaferExist = -1;
        int[] m_diWaferCheck = new int[2] { -1, -1 }; // WW RF

        ezView m_viewAlign;
        ezView m_viewBCR;
        ezView m_viewOCR;

        bool m_bUseDefuser = false;
        double m_offsetLive = 0;
        double m_degVision = 0;
        double m_degNotch = 0;
        double m_degCam = 270;
        double m_degOCR = 90; 

        const int c_nMaxImg = 100;

        HW_Aligner_ATI_AOI_Data m_data = new HW_Aligner_ATI_AOI_Data();
        HW_Aligner_ATI_AOI[] m_aAOI = new HW_Aligner_ATI_AOI[c_nMaxImg];
        HW_Aligner_ATI_AOI m_aoi = new HW_Aligner_ATI_AOI();

        HW_Aligner_ATI_Edge m_aoiEdge = new HW_Aligner_ATI_Edge();

		HW_WTR_Mom m_wtr;
        bool m_bChipping = false; 
        HW_Klarf m_klarf = new HW_Klarf("Klarf");
        int m_msSaveTimeout = 30000;
        string m_strKlarfPath = "D:\\Klarf";
        string m_strKlarfBackupPath = "D:\\KlarfBackup";
        string m_strKlarfName = "KlarfName";

        //HW_Aligner_ATI_Manual m_manual = new HW_Aligner_ATI_Manual(); 

        int m_indexNotch;
        int m_nMinBCRLength = 7;
        CPoint m_cpNotch = new CPoint();
		RPoint m_rtSetup = new RPoint(0, 100);

        string m_strAlignPath = "c:\\TestImg";

        public override void Init(string id, Auto_Mom auto)
        {
            m_strAlignModel = "ATI";
            m_wtr = auto.ClassHandler().ClassWTR(); // BHJ 191128 add
            base.Init(id, auto);
            m_control.Add(this);
            InitString();
            m_camAlign.Init(m_id, m_auto.ClassLogView());
            m_camOCR.Init(m_id + "OCR", m_log, m_recipe.m_eOCR == Recipe_Mom.eType_OCR.CognexCam);
            m_camOCRBottom.Init(m_id + "OCR_Bottom", m_log, m_recipe.m_eOCRBottom == Recipe_Mom.eType_OCR.CognexCam);
            m_azBCR.Init(m_id + "BCR", m_log); 
            m_azBCR2D.Init(m_id + "BCR2D", m_log);
            m_azOCR = new Cognex_OCR("OCRLib", m_log);
            m_viewAlign = new ezView("Align", 0, m_auto);
            m_viewBCR = new ezView("BCR", 0, m_auto);
            m_viewOCR = new ezView("OCR", 0, m_auto);
            m_data.Init(m_id + "Data", m_log); 
            m_aoi.Init(m_id, 0, m_work, m_data, m_log,this);
            m_klarf.Init(m_auto, m_log);
            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++) m_nGrab[n] = c_nMaxImg;
            RunGrid(eGrid.eRegRead);
            m_strAlignMode = eAlignMode.Align.ToString();
            RunGrid(eGrid.eInit);
            m_aoiEdge.Init(m_id + "Edge", m_auto, m_axisRotate, m_wafer); 
            m_camAlign.InitCamera();
//            m_camAlign.SetROI(m_cpImgOffset, m_szCam);
            if (m_camAlign.m_bDeviceOpenError) m_auto.m_bOK = false;
            m_camAlign.Grab(m_viewAlign.m_imgView.m_pImg);
            m_viewBCR.m_imgView.m_pImg.ReAllocate(m_viewAlign.m_imgView.m_pImg); 
            m_szCam = m_viewAlign.m_imgView.m_pImg.m_szImg; 
            m_aoi.m_img.ReAllocate(m_viewAlign.m_imgView.m_pImg); 
            for (int n = 0; n < c_nMaxImg; n++)
            {
                m_aAOI[n] = new HW_Aligner_ATI_AOI();
                m_aAOI[n].Init(m_id + "AOI", n, m_work, m_data, m_log, this);
                m_aAOI[n].m_img.ReAllocate(m_viewAlign.m_imgView.m_pImg); 
            }
            m_camOCR.Connect();
            m_camOCRBottom.Connect();
            m_log.m_reg.Read("rtSetup", ref m_rtSetup); // BHJ 191128 add
        }

        public override void ThreadStop()
        {
            foreach (HW_Aligner_ATI_AOI aoi in m_aAOI) aoi.ThreadStop();
            m_aoi.ThreadStop(); 
            base.ThreadStop(); 
            m_camAlign.ThreadStop();
            m_camOCR.ThreadStop();
            m_camOCRBottom.ThreadStop(); 
            m_viewAlign.ThreadStop();
            m_viewBCR.ThreadStop(); 
            m_viewOCR.ThreadStop();
            m_aoiEdge.ThreadStop(); 
        }

        public override IDockContent GetContentFromPersistString(string persistString)
        {
            if (m_viewAlign.IsPersistString(persistString)) return m_viewAlign;
            if (m_viewBCR.IsPersistString(persistString)) return m_viewBCR;
            if (m_viewOCR.IsPersistString(persistString)) return m_viewOCR;
            return null; 
        }

        public override void ShowAll(DockPanel dockPanel)
        {
            m_viewAlign.Show(dockPanel);
            m_viewBCR.Show(dockPanel);
            m_viewOCR.Show(dockPanel);
        }

        public override void ShowDlg(Form parent, ref CPoint cpShow)
        {
            base.ShowDlg(parent, ref cpShow);
            if (m_bEnableAlignerEdge) m_aoiEdge.ShowDlg(parent, ref cpShow);
            else m_aoiEdge.Hide(); 
        }

        public void InitString()
        {
            
            InitString(eError.CheckWafer, "Aligner wafer 감지에 이상이 발견되었습니다.");
            InitString(eError.Rotate, "Aligner Theta축에 이상이 감지되었습니다.");
            InitString(eError.RotateTime, "Rotate Timeout Error !!");
            InitString(eError.InfoWafer, "얼라이너의 Wafer 정보가 정상적으로 전달되지 않았습니다.");
            InitString(eError.MoveCam, "Move Camera AxisMove Error !!"); 
            InitString(eError.Lift, "리프터가 정상적으로 동작하지 않았습니다. 리프터 up down상태를 확인하여 주세요.");
            InitString(eError.AlignError, "얼라이너에서 얼라인 동작에 실패하였습니다. 레시피를 확인하여 주세요.");
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
            m_aoiEdge.m_axisRotate = m_axisRotate; 
            m_axisCamX = control.AddAxis(rGrid, m_id, "CamX", "Axis Camera X");
            m_axisCamZ = control.AddAxis(rGrid, m_id, "CamZ", "Axis Camera Z");
            m_axisOCR = control.AddAxis(rGrid, m_id, "OCR", "Axis OCR X");
            control.AddDO(rGrid, ref m_doLightAlign[0], m_id, "Light_Align_WW", "DO Light");
            control.AddDO(rGrid, ref m_doLightAlign[1], m_id, "Light_Align_RF", "DO Light");
            control.AddDO(rGrid, ref m_doLightAlign[2], m_id, "Light_BCR", "DO Light");
            control.AddDO(rGrid, ref m_doLift[0], m_id, "Lift_Down", "DO Lift");
            control.AddDO(rGrid, ref m_doLift[1], m_id, "Lift_Up", "DO Lift");
            if(m_bEnableAlignerEdge)
            {
                control.AddDO(rGrid, ref m_doLift[2], m_id, "300WF_Lift_Down", "DO Lift");  //KDG 161006 Add
                control.AddDO(rGrid, ref m_doLift[3], m_id, "300WF_Lift_Up", "DO Lift");    //KDG 161006 Add
            }
            control.AddDO(rGrid, ref m_doVac[0], m_id, "Vac_WW", "DO Vacuum");
            control.AddDO(rGrid, ref m_doVac[1], m_id, "Vac_RF", "DO Vacuum");
            control.AddDO(rGrid, ref m_doVac[2], m_id, "Vac_Edge", "DO Vacuum");      //KDG 160921 Add
            control.AddDO(rGrid, ref m_doBlow[0], m_id, "Blow_WW", "DO Blow");
            control.AddDO(rGrid, ref m_doBlow[1], m_id, "Blow_RF", "DO Blow");
            control.AddDO(rGrid, ref m_doBlow[2], m_id, "Blow_Edge", "DO Blow");      //KDG 160921 Add
            control.AddDI(rGrid, ref m_diLift[0], m_id, "Lift_Down", "DI Lift");
            control.AddDI(rGrid, ref m_diLift[1], m_id, "Lift_Up", "DI Lift");
            if (m_bEnableAlignerEdge)
            {
                control.AddDI(rGrid, ref m_diLift[2], m_id, "300WF_Lifter_Down", "DI Lift");  //KDG 161006 Add
            }
            control.AddDI(rGrid, ref m_diWaferExist, m_id, "WaferExist", "DI Wafer Exist");
            control.AddDI(rGrid, ref m_diWaferCheck[0], m_id, "WaferCheck_WW", "DI Wafer Check");
            control.AddDI(rGrid, ref m_diWaferCheck[1], m_id, "WaferCheck_RF", "DI Wafer Check");
        }

        protected override void RunGrid(eGrid eMode)
        {
            base.RunGrid(eMode);
            m_grid.Set(ref m_strAlignMode, m_strAlignModes, "Mode", "Align", "Align Mode");
            m_grid.Set(ref m_strAlignPath, "Path", "Align", "Align image save path");
            m_camAlign.RunGrid(m_grid, eMode);
            m_camOCR.RunGrid(m_grid);
            m_camOCRBottom.RunGrid(m_grid);
            m_data.RunGrid(m_grid, eMode);
            m_grid.Set(ref m_nPulseRotate, "Rotate", "PpR", "Pulse / 1R");
            m_grid.Set(ref m_nPulseBack, "Rotate", "Back", "Back Pulse cause Backlash (pulse)");
            m_grid.Set(ref m_fSlowRotate, "Rotate", "Slow", "Slow Rotate Speed (Rate 0.1 ~ 1.0)");
            if (m_fSlowRotate > 1) m_fSlowRotate = 1;
            if (m_fSlowRotate < 0.1) m_fSlowRotate = 0.1;
            m_grid.Set(ref m_degReady, "Rotate", "Ready", "Ready Position (deg)"); 
            m_grid.Set(ref m_degVision, "Rotate", "Vision", "Rotate Offset (deg)");
            m_grid.Set(ref m_degCam, "Rotate", "Cam", "Camera Position (deg)");
            m_grid.Set(ref m_degOCR, "Rotate", "OCR", "OCR Position (deg)"); 
            m_grid.Set(ref m_offsetLive, "Rotate", "LiveOffset", "Pulse offset on Live Grab");
            m_grid.Set(ref m_msHome, "Time", "Home", "Home Timeout (ms)"); 
            m_grid.Set(ref m_msAlign, "Time", "Align", "Align Timeout (ms)"); 
            for (int nSize = 0; nSize < (int)(Wafer_Size.eSize.Empty); nSize++)
            {
                m_grid.Set(ref m_zCam[nSize], ((Wafer_Size.eSize)nSize).ToString(), "CamZ", "Camera Pos (pulse)", false, m_wafer.m_bEnable[nSize]);
                m_grid.Set(ref m_xAlign[nSize], ((Wafer_Size.eSize)nSize).ToString(), "Align", "Align Pos (pulse)", false, m_wafer.m_bEnable[nSize]);
                m_grid.Set(ref m_xBCR[nSize], ((Wafer_Size.eSize)nSize).ToString(), "BCR", "BCR Pos (pulse)", false, m_wafer.m_bEnable[nSize]);
                m_grid.Set(ref m_xOCR[nSize], ((Wafer_Size.eSize)nSize).ToString(), "OCR", "OCR Pos (pulse)", false, m_wafer.m_bEnable[nSize]);
                m_grid.Set(ref m_xOCRBtm[nSize], ((Wafer_Size.eSize)nSize).ToString(), "OCRBtm", "OCRBottom Pos (pulse)", false, m_wafer.m_bEnable[nSize]);
                m_grid.Set(ref m_nGrab[nSize], ((Wafer_Size.eSize)nSize).ToString(), "GrabCount", "# of Grab Count (<36)", false, m_wafer.m_bEnable[nSize]);
                if (m_nGrab[nSize] > c_nMaxImg) m_nGrab[nSize] = c_nMaxImg;
            }
            m_grid.Set(ref m_fUMpPulse[0], "AxisScale", "Align", "Axis Scale (um/pulse)"); 
            m_grid.Set(ref m_fUMpPulse[1], "AxisScale", "OCR", "Axis Scale (um/pulse)");
            m_grid.Set(ref m_szCam, "AlignCam", "Size", "Align Camera Image Size");
            m_grid.Set(ref m_cpImgOffset, "AlignCam", "Offset", "Align Camera Image Offset");
            if (eMode == eGrid.eRegWrite)
            {
                m_camAlign.SetROI(m_cpImgOffset, m_szCam);
            }
            m_grid.Set(ref m_bEnableAlignerEdge, "Edge", "Enable", "Enable Edge Inspect");
            m_azOCR.RunGridTeach("OCRParam", m_grid, eMode);
            m_grid.Set(ref m_nMinBCRLength, "BCR", "Length", "Minimum Barcode Length");
            m_grid.Set(ref m_bChipping, "Chipping", "Use", "Use Chipping Inspection");
            if (m_bChipping)
            {
                m_klarf.RunGrid(m_grid, eMode);
                m_grid.Set(ref m_msSaveTimeout, m_klarf.m_id, "SaveTimeout", "Klarf File Save Timeout (ms)");
                m_grid.Set(ref m_strKlarfPath, m_klarf.m_id, "Path", "File Path");
                m_grid.Set(ref m_strKlarfBackupPath, m_klarf.m_id, "BackupPath", "Backup File Path");
                m_grid.Set(ref m_klarf.m_fResolution, m_klarf.m_id, "Resolution", "Resolution (um)");
            }
            m_grid.Set(ref m_bUseImgBinary, "Img Binary", "Use", "Use Image Binary Algorithm (default = false)"); // BHJ 190625 add
            m_grid.Set(ref m_nBinaryGV, "Img Binary", "GV Threshold", "Threshold value of GV for Bynary Algorithm (default= 125)"); // BHJ 190625 add
            m_grid.Set(ref m_bWTRShiftUse, "WTR Shift", "Use", "Enable WTR Shift Centering function"); // BHJ 191128 add
            m_grid.Set(ref m_nCamResolution, "WTR Shift", "Resolution", "Align Camera Resolution"); // BHJ 191128 add
        }

        int m_umChipping = 300;
        int m_deltaChipping = 300;
        int m_nMerge = 50;

        public void RecipeGrid(bool bRAC, ezGrid rGrid, eGrid eMode, ezJob job)
        {
            if (eMode == eGrid.eJobOpen)
            {
                if (m_recipe.m_wafer.IsRingFrame())
                {
                    m_data.m_eInspect = HW_Aligner_ATI_AOI_Data.eInspect.BlackWafer;
                    m_data.m_nGV = 15;
                    m_bLightAlign[0] = false;
                    m_bLightAlign[1] = true;
                    m_bLightAlign[2] = false; 
                }
                else
                {
                    m_data.m_eInspect = HW_Aligner_ATI_AOI_Data.eInspect.White300;
                    m_data.m_nGV = 50;
                    m_bLightAlign[0] = true;
                    m_bLightAlign[1] = false;
                    m_bLightAlign[2] = false;
                }
                if (bRAC)
                {
                    m_offsetLive = 0;
                    var File = job.m_strTitle.Split('\\');
                    m_bEnableAlign = Convert.ToBoolean(m_recipe.GetRACVal_str(File[2], "AlignUse"));
                    m_bRotate180 = Convert.ToBoolean(m_recipe.GetRACVal_str(File[2], "AlignRoate"));
                    m_bLightAlign[0] = Convert.ToBoolean(m_recipe.GetRACVal_str(File[2], "AlignLight_Coaxial"));
                    m_bLightAlign[1] = Convert.ToBoolean(m_recipe.GetRACVal_str(File[2], "AlignLight_Side"));
                    m_bLightAlign[2] = Convert.ToBoolean(m_recipe.GetRACVal_str(File[2], "AlignLight_Side"));
                }

                rGrid.Set(ref m_bEnableAlign, "Align", "Use", "Use Align");
                rGrid.Set(ref m_bRotate180, "Align", "Roate 180", "Ring Frame Roate 180");  //KDG 161025 Add Ring Wafer 180 Rotate
                rGrid.Set(ref m_offsetLive, "Align", "FlatPos", "Flat Position Offset");

                m_data.RecipeGrid(bRAC, rGrid, eMode, job, m_recipe);

                rGrid.Set(ref m_bLightAlign[0], "Align", "Light_Coaxial", "Light Option for Align");
                rGrid.Set(ref m_bLightAlign[1], "Align", "Light_Back", "Light Option for Align");
                rGrid.Set(ref m_bLightAlign[2], "Align", "Light_Side", "Light Option for Align");

                if (m_bEnableAlignerEdge) m_aoiEdge.RecipeGrid(bRAC, rGrid, eMode, job);
            }

            rGrid.Set(ref m_bEnableAlign, "Align", "Use", "Use Align");
            rGrid.Set(ref m_bRotate180, "Align", "Rotate 180", "Ring Frame Roate 180");  //KDG 161025 Add Ring Wafer 180 Rotate

            m_data.RecipeGrid(bRAC, rGrid, eMode, job, m_recipe); 

            rGrid.Set(ref m_bLightAlign[0], "Align", "Light_Coaxial", "Light Option for Align");
            rGrid.Set(ref m_bLightAlign[1], "Align", "Light_Back", "Light Option for Align");
            rGrid.Set(ref m_bLightAlign[2], "Align", "Light_Side", "Light Option for Align");
            rGrid.Set(ref m_bUseDefuser, "Align", "UseDefuser", "Use Defuser for Image Snap");
            if (m_bEnableAlignerEdge) m_aoiEdge.RecipeGrid(bRAC, rGrid, eMode, job);
            if (m_bChipping)
            {
                rGrid.Set(ref m_umChipping, "Chipping", "Size", "Chipping Inspection Size (um)");
                rGrid.Set(ref m_deltaChipping, "Chipping", "Delta", "Delta posiotn between AOI (pixel)");
                m_data.m_nChipping = (int)Math.Round(m_umChipping * m_deltaChipping / Math.PI / 3000 + 0.5);
                rGrid.Set(ref m_nMerge, "Chipping", "Merge", "Merge Size (pixel)");
            }
            else m_data.m_nChipping = 0; 
            if (m_recipe.m_wafer.m_eSize != Wafer_Size.eSize.mm300_RF) m_bRotate180 = false;
        }

        public override void Draw(Graphics dc, ezImgView imgView)
        {
            if (imgView.m_id == "Align")
            {
                m_aoi.Draw(dc, imgView); 
            }
        }

        public override void InvalidView()
        {
            m_viewAlign.InvalidView(false);
            m_viewBCR.InvalidView(false); 
            m_viewOCR.InvalidView(false); 
        }

        protected override void SetProcess(eProcess run)
        {
            base.SetProcess(run); 
        }

        #region Home Process
        int GetCurrentDeg90()
        {
            int nPos = (int)m_axisRotate.GetPos(true);
            while (nPos < 0) nPos += m_nPulseRotate;
            while (nPos >= m_nPulseRotate) nPos -= m_nPulseRotate; 
            int nDeg = 360 * nPos / m_nPulseRotate - m_degReady;
            return ((nDeg + 45) / 90) * 90;
        }

        protected override void ProcHome()
        {
            if (m_logMars != null) m_logMars.AddFunctionLog("Aligner", "InitialIze", MarsLog.eStatus.Start, "$", MarsLog.eMateral.Wafer, null); 
            Home();
            if (m_logMars != null) m_logMars.AddFunctionLog("Aligner", "InitialIze", MarsLog.eStatus.End, "$", MarsLog.eMateral.Wafer, null); 
        }
        #endregion
        /**
* @brief Aligner home동작
* @param 없음
* @return 없음
* @note Patch-notes
* 날짜|작성자|설명|비고
* -|-|-|-
* 2023-07-24|조남수|Aligner Home동작에 대한 Log 추가|-
* @warning 없음
*/
        void Home()
        {
            m_log.WriteLog("Sequence", "[" + m_id + "Start]" + " Home"); //230721 nscho
            int nDeg = GetCurrentDeg90();
            if (EdgeInsepctionCheck())      //KDG 161007 Add Edge Vision 연결상태
            {
                if (m_bEnableAlignerEdge)   //KDG 161028 Add LED Off
                    m_aoiEdge.RunLED(false);

                m_control.WriteOutputBit(m_doVac[0], true);
                m_control.WriteOutputBit(m_doVac[1], true);
                ezStopWatch sw = new ezStopWatch();
                if (m_axisOCR != null)
                {
                    m_axisOCR.HomeSearch(); // ing 171018 OCR Bottom Camera 간섭으로 인한 홈 순서 설정
                    while (!m_axisOCR.IsReady() && (sw.Check() <= m_msHome))
                        Thread.Sleep(10);
                }
                if (m_axisRotate != null) m_axisRotate.HomeSearch();
                if (m_axisCamX != null) m_axisCamX.HomeSearch();
                if (m_axisCamZ != null) m_axisCamZ.HomeSearch();
                while (!m_axisRotate.IsReady() && (sw.Check() <= m_msHome))
                    Thread.Sleep(10);
                while (!m_axisCamX.IsReady() && (sw.Check() <= m_msHome))
                    Thread.Sleep(10);
                while (!m_axisCamZ.IsReady() && (sw.Check() <= m_msHome))
                    Thread.Sleep(10);

                if (sw.Check() >= m_msHome)
                {
                    m_log.Popup("Axis Home Time Out !!");
                    m_log.WriteLog("Sequence", "[" + m_id + "Done]" + " Home FAIL"); //230721 nscho
                    m_xGem.SetMSBAlarm(XGem300_Mom.eMSBAlarm.AL_Home_Error);
                    SetState(eState.Init);
                }
                else
                {
                    m_log.Add("Find Home Finished");
                    m_log.WriteLog("Sequence", "[" + m_id + "Done]" + " Home PASS"); //230721 nscho
                    if (m_infoWafer == null || !m_infoWafer.m_wafer.IsRingFrame() || !m_infoWafer.m_bRotate180) SetState(eState.RunReady);
                    else
                    {
                        int nDegWafer = (int)m_infoWafer.m_eAngleRF - nDeg / 90;
                        while (nDegWafer < 0) nDegWafer += 4;
                        m_infoWafer.m_eAngleRF = (Info_Wafer.eAngleRF)nDegWafer;
                        if (m_infoWafer.m_eAngleRF == Info_Wafer.eAngleRF.R0) SetState(eState.RunReady);
                        else ProcRotateRF(Info_Wafer.eAngleRF.R0);
                    }
                }
                RunVac(false);
            }
            else
            {
                m_xGem.SetMSBAlarm(XGem300_Mom.eMSBAlarm.AL_Home_Error);
                m_log.WriteLog("Sequence", "[" + m_id + "Done]" + " Home FAIL"); //230721 nscho
                SetState(eState.Error);
            }
        
        }
        protected override void ProcRunReady()
        {
            if (RotateDeg(0)) return;
            if (m_imgWaferEmpty == null) m_imgWaferEmpty = new ezImg("WaferEmpty", m_log);
            //CheckWaferGrab(m_imgWaferEmpty); 
            SetState(eState.Ready);
        }

        bool RotateAxis(double fPosAxis)
        {
            if (m_logMars != null) m_logMars.AddFunctionLog("Aligner", "MoveAxisRotate", MarsLog.eStatus.Start, (m_infoWafer != null) ? m_infoWafer.m_idMaterial : "$", MarsLog.eMateral.Wafer, null);
            try
            {
                m_axisRotate.Move(fPosAxis);
                if (m_axisRotate.WaitReady())
                {
                    SetAlarm(eAlarm.Warning, eError.Rotate);
                    SetState(eState.Error);
                    return true;
                }
                return false;
            }
            finally
            {
                if (m_logMars != null) m_logMars.AddFunctionLog("Aligner", "MoveAxisRotate", MarsLog.eStatus.End, (m_infoWafer != null) ? m_infoWafer.m_idMaterial : "$", MarsLog.eMateral.Wafer, null);
            }
        }

        bool RotateAxis(double fPosAxis, bool bBack)
        {
            if (fPosAxis == m_axisRotate.GetPos(true)) return false;
            if (bBack && fPosAxis < m_axisRotate.GetPos(true))
            {
                if (RotateAxis(fPosAxis - m_nPulseBack)) return true; 
            }
            if (RotateAxis(fPosAxis)) return true;
            return false;
        }
        
        bool RotateDeg(double fDeg)
        {
            double fPosAxis = (fDeg + m_degReady) * m_nPulseRotate / 360;
            return RotateAxis(fPosAxis, true); 
        }

        double GetDeg(double fPulse)
        {
            return fPulse * 360 / m_nPulseRotate - m_degReady; 
        }

        protected override void RunRotate(double fDeg) 
        {
            fDeg = fDeg + m_degNotch - m_degCam;
            //while (fDeg > 0) fDeg -= 360; 
            //while (fDeg < -15) fDeg += 360; 
            RotateDeg(fDeg); 
        }

        #region Run Process
        protected override void ProcPreProcess()
        {
            if (m_infoWafer == null)
            {
                SetAlarm(eAlarm.Warning, eError.InfoWafer);
                SetState(eState.Error);
                return; 
            }
            RunVac(true);                               //170213 Warp 심한 웨이퍼 인식이 안되는 문제 관련=> 리프터 올라가기전에 실행되게하면됨.
            if (m_bEnableAlignerEdge)   //KDG 160923 Add Edge Motion
            {
                if (RunLift(false) == eHWResult.Error)
                {
                    SetAlarm(eAlarm.Warning, eError.Lift);
                    return;
                }
                Thread.Sleep(1000);
            }
            if (MoveCamPos(eProcess.Align))
            {
                SetAlarm(eAlarm.Warning, eError.MoveCam);
                SetState(eState.Error);
                return; 
            }
            //RunVac(true);                               //170213 Warp 심한 웨이퍼 인식이 안되는 문제 관련=> 리프터 올라가기전에 실행되게하면됨.
            if (CheckWaferExist(true) != eHWResult.On)
            {
                SetAlarm(eAlarm.Warning, eError.CheckWafer);
                SetState(eState.Error);
                return; 
            }
            RunLight(eProcess.Align);
            SetProcess(eProcess.Align);
        }

        bool MoveCamPos(eProcess ePro)
        {
            if (m_work.m_bTestRun) return false;
            if (m_logMars != null) m_logMars.AddFunctionLog("Aligner", "MoveCam", MarsLog.eStatus.Start, (m_infoWafer != null) ? m_infoWafer.m_idMaterial : "$", MarsLog.eMateral.Wafer, null); 
            try
            {
                int nWaferSize = (int)m_infoWafer.m_wafer.m_eSize;
                m_axisCamZ.Move(m_zCam[nWaferSize]);
                switch (ePro)
                {
                    case eProcess.Align: // ing 171018 OCR Bottom Camera 간섭으로 인한 축 이동 순서 및 대기 시컨스 수정
                        RotateDeg(0);
                        if (m_axisRotate.WaitReady()) return true;
                        m_axisCamX.Move(m_xAlign[nWaferSize]);
                        break;
                    case eProcess.BCR:
                        m_axisCamX.Move(m_xBCR[(int)m_infoWafer.m_wafer.m_eSize] - 1000 * m_infoWafer.m_fdRBCR / m_fUMpPulse[0]);
                        if (m_infoWafer.m_bDirectionNotch) RunRotate(m_infoWafer.m_fAngleBCR + m_degCam);
                        else RunRotate(m_infoWafer.m_fAngleBCR + m_degCam - m_degNotch); // RotateDeg(m_infoWafer.m_fAngleBCR + m_degCam); 
                        break;
                    case eProcess.OCR: // ing 171018 OCR Bottom Camera 간섭으로 인한 축 이동 순서 및 대기 시컨스 수정
                        if (m_axisOCR != null)
                        {
                            RunRotate(m_infoWafer.m_fAngleOCR + m_degOCR);
                            if (m_axisRotate.WaitReady()) return true;
                            if ((Recipe_Mom.eInspDir)m_infoWafer.m_nOCRDir == Recipe_Mom.eInspDir.Bottom) m_axisOCR.Move(m_xOCRBtm[nWaferSize] - 1000 * m_infoWafer.m_fdROCR / m_fUMpPulse[1]); 
                            else m_axisOCR.Move(m_xOCR[nWaferSize] - 1000 * m_infoWafer.m_fdROCR / m_fUMpPulse[1]);
                        }
                        break;
                }
                if (m_axisCamX != null) if (m_axisCamX.WaitReady()) return true;
                if (m_axisCamZ != null) if (m_axisCamZ.WaitReady()) return true;
                if (m_axisOCR != null) if (m_axisOCR.WaitReady()) return true;
                return false;
            }
            finally
            {
                if (m_logMars != null) m_logMars.AddFunctionLog("Aligner", "MoveCam", MarsLog.eStatus.End, (m_infoWafer != null) ? m_infoWafer.m_idMaterial : "$", MarsLog.eMateral.Wafer, null);
            }
        }

        bool ReadWaferID()
        {
            int x, y;
            CPoint cp0, cp1;
            cp0 = new CPoint(0, 0);
            cp1 = new CPoint(m_viewOCR.m_imgView.m_pImg.m_szImg.x - 1, m_viewOCR.m_imgView.m_pImg.m_szImg.y - 1);
            string strWaferID = "";
            int nWaferSize = (int)m_infoWafer.m_wafer.m_eSize;
            if (m_infoWafer == null) return false;
            if (m_infoWafer.m_sWaferIDType == "None") return false;
            RunLight(eProcess.BCR);
            m_axisCamX.Move((m_xAlign[nWaferSize] + m_xBCR[nWaferSize]) / 2);
            RotateDeg(0);
            if (m_axisCamX.WaitReady()) return true;
            if (m_axisCamZ.WaitReady()) return true;
            if (m_axisOCR.WaitReady()) return true;

            if (m_infoWafer.m_sWaferIDType == "OCR")
            {
                m_camAlign.Grab(m_viewOCR.m_imgView.m_pImg, false);
                unsafe // Find ROI
                {
                    // Find cp0.x
                    byte* pSrc = (byte*)m_viewOCR.m_imgView.m_pImg.GetIntPtr(m_viewOCR.m_imgView.m_pImg.m_szImg.y / 2, 0);
                    for (x = 0; x < m_viewOCR.m_imgView.m_pImg.m_szImg.x; x++)
                    {
                        if (*pSrc > m_azOCR.GetThreshold())
                        {
                            cp0.x = x;
                            break;
                        }
                        pSrc++;
                    }

                    // Find cp1.x
                    pSrc = (byte*)m_viewOCR.m_imgView.m_pImg.GetIntPtr(m_viewOCR.m_imgView.m_pImg.m_szImg.y / 2, m_viewOCR.m_imgView.m_pImg.m_szImg.x - 1);
                    for (x = m_viewOCR.m_imgView.m_pImg.m_szImg.x - 1; x >= 0; x--)
                    {
                        if (*pSrc > m_azOCR.GetThreshold())
                        {
                            cp1.x = x;
                            break;
                        }
                        pSrc--;
                    }

                    // Find cp0.y
                    pSrc = (byte*)m_viewOCR.m_imgView.m_pImg.GetIntPtr(0, m_viewOCR.m_imgView.m_pImg.m_szImg.x / 2);
                    for (y = 0; y < m_viewOCR.m_imgView.m_pImg.m_szImg.y - 1; y++)
                    {
                        if (*pSrc > m_azOCR.GetThreshold())
                        {
                            cp0.y = y;
                            break;
                        }
                        pSrc += m_viewOCR.m_imgView.m_pImg.m_szImg.x;
                    }

                    // Find cp1.y
                    pSrc = (byte*)m_viewOCR.m_imgView.m_pImg.GetIntPtr(m_viewOCR.m_imgView.m_pImg.m_szImg.y - 1, m_viewOCR.m_imgView.m_pImg.m_szImg.x / 2);
                    for (y = m_viewOCR.m_imgView.m_pImg.m_szImg.y - 1; y >= 0; y--)
                    {
                        if (*pSrc > m_azOCR.GetThreshold())
                        {
                            cp1.y = y;
                            break;
                        }
                        pSrc -= m_viewOCR.m_imgView.m_pImg.m_szImg.x;
                    }
                }

                cp0 += new CPoint(20, 20);
                cp1 -= new CPoint(20, 20);

                if (m_azOCR.Run(m_viewOCR.m_imgView.m_pImg, cp0 , cp1 - cp0, ref strWaferID))
                {
                    m_viewOCR.m_imgView.m_pImg.FileSave("c:\\TestImg\\OCRFont\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".bmp");
                    Thread.Sleep(500);
                    m_viewOCR.m_imgView.m_pImg.Copy(m_viewOCR.m_imgView.m_pImg, new CPoint(0, 0), new CPoint(0, 0), m_szCam);
                    ChangeCode(m_viewOCR);
                }
                else
                {
                    m_infoWafer.m_strWaferID = strWaferID;
                }
            }
            else if (m_infoWafer.m_sWaferIDType == "BCR")
            {
                m_camAlign.Grab(m_viewBCR.m_imgView.m_pImg, false);
                m_infoWafer.m_strWaferID = m_azBCR.Run(m_viewBCR.m_imgView.m_pImg, new CPoint(0, 0), m_szCam);
                if (m_infoWafer.m_strWaferID == "Unknown")
                {
                    m_viewBCR.m_imgView.m_pImg.FileSave("C:\\Temp\\1DFail.bmp");
                    Thread.Sleep(500);
                    m_viewBCR.m_imgView.m_pImg.Copy(m_viewBCR.m_imgView.m_pImg, new CPoint(0, 0), new CPoint(0, 0), m_szCam);
                    ChangeCode(m_viewBCR);
                }
            }
            m_log.Add("Read RingWafer ID : " + m_infoWafer.m_strWaferID);
            return false;
        }

        protected override void TestRun()
        {
            if (m_work.IsRun()) return; 
            else if (m_strAlignMode == eAlignMode.Align.ToString())
            {
                ProcAlign();
            }
            else if (m_strAlignMode == eAlignMode.Inspect.ToString())
            {
                TestInspect(); 
            }
            else if (m_strAlignMode == eAlignMode.InspectAll.ToString())
            {
                m_data.SetRecipe(m_infoWafer.m_sRecipe); 
                for (int n = 0; n < 100; n++)
                {
                    m_aAOI[n].m_img.FileOpen("c:\\TestImg\\Align" + n.ToString("00") + ".bmp");
                    m_aAOI[n].StartInspect(0, null);
                    m_aAOI[n].WaitInspect(1000);
                }

                if (FindNotchIndex() < 0) return;
                MergeChipping();
                SaveKlarf();
                m_auto.Invalidate(0);
            }
            else if (m_strAlignMode == eAlignMode.SaveNotch.ToString())
            {
                TestInspect();
                if (m_aoi.m_img.m_aBuf == null) { m_log.Popup("Notch Image Save Error"); return; }
                if (m_aoi.m_bFindNotch == false) { m_log.Popup("Notch Find Error"); return; }
                ezImg imgNotch = new ezImg("Notch", m_log);
                imgNotch.ReAllocate(m_data.c_szNotchImg, 1);
                imgNotch.Copy(m_aoi.m_img, m_aoi.m_cpNotch - m_data.c_szNotchImg2, new CPoint(0, 0), m_data.c_szNotchImg); 
                Directory.CreateDirectory("c:\\TestImg\\Notch");
                imgNotch.FileSave("c:\\TestImg\\Notch\\" + m_infoWafer.m_sRecipe + ".bmp");
                m_data.m_sRecipe = "";
                m_log.Popup("Notch Image Saved");
            }
            else if (m_strAlignMode == eAlignMode.Setup.ToString())
            {
                RunAlignSetup();
            }
        }

        // BHJ 190226 add SetRotatePos, RotatePulse, MoveToCenter, RunAlignSetup
        double m_dRotate = 0; //pt
        void SetRotatePos(double fPos)
        {
            double m_fDiff;
            Thread.Sleep(100);
            m_fDiff = m_axisRotate.GetPos(true) - m_axisRotate.GetPos(false);
            if (Math.Abs(m_fDiff) > 100)
            {
                SetAlarm(eAlarm.Warning, eError.Rotate);
                SetState(eState.Error);
                return;
            }
            m_dRotate += (m_axisRotate.GetPos(true) - fPos);
            m_axisRotate.SetPos(true, fPos);
            m_axisRotate.SetPos(false, fPos);
            m_log.Add("SetRotatePos : " + fPos.ToString());
        }

        bool RotatePulse(double pulse)
        {
            Thread.Sleep(500);
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

        bool MoveToCenter(double pulseR, double pulseX)
        {
            ezStopWatch sw = new ezStopWatch();
            m_log.Add("MoveToCenter : " + pulseR.ToString() + ", " + pulseX.ToString());

            if (pulseX < 0) //forget190219
            {
                pulseX = -pulseX;
                pulseR += m_nPulseRotate / 2;
            }
            if (RotatePulse(pulseR)) return true;
            if (RunVac(false)) return true;
            m_wtr.RunWTRShift(pulseX / 1000); // forget190219
            if (RunVac(true)) return true;

            //else   // BHJ 190305 aligner x축 사용시 알고리즘
            //{
            //    if (RotatePulse(pulseR)) return true;
            //    RunVac(false);
            //    if (RunLift(true) != eHWResult.OK) return true;
            //    RotateBack();
            //    if (MoveAxis(pulseX)) return true;
            //    if (m_axisRotate.WaitReady())
            //    {
            //        m_log.Popup("Rotate Back Error !!");
            //        SetAlarm(eAlarm.Warning, eError.RotateTime);
            //        SetState(eState.Error);
            //        return true;
            //    }
            //    if (RunLift(false) != eHWResult.OK) return true;
            //    RunVac(true);
            //    MoveAxis(0);
            //}

            return false;
        }

        int c_rShift = 1000;
        bool MoveToCenter()
        {
            return MoveToCenter((-m_infoWafer.m_rtWaferShift.y + m_rtSetup.y) * m_nPulseRotate / 360, -c_rShift * m_infoWafer.m_rtWaferShift.x / m_rtSetup.x);
        }

        void RunAlignSetup()
        {
            // 1차 얼라인 프로세스 구동
            RunGrabAlign();
            // Shift 위치 계산
            ClearWaferShift();
            CalcWaferShift(0);

            RPoint xyShift0 = m_infoWafer.m_dpWaferShift;

            // 임의의 위치로 보정 - x축 1mm
            if (MoveToCenter(0, c_rShift)) return;

            // 2차 얼라인 프로세스 구동
            RunGrabAlign();

            // Shift 위치 계산
            ClearWaferShift();
            CalcWaferShift(0);

            RPoint xyShift1 = m_infoWafer.m_dpWaferShift;

            // 보정용 계산식
            RPoint xyDelta = xyShift1 - xyShift0;
            m_rtSetup.x = Math.Sqrt(xyDelta.x * xyDelta.x + xyDelta.y * xyDelta.y); // radius
            m_rtSetup.y = 180 * Math.Atan2(xyDelta.y, xyDelta.x) / Math.PI; // angle
            m_log.Add("rtSetup = " + m_rtSetup.ToString());
            m_log.m_reg.Write("rtSetup", m_rtSetup);

            // 센터 보정 작업
            //if (MoveToCenter()) return;

            ProcAlign();

            RunLight(eProcess.Rotate);
            SetState(eState.RunReady);
        }

        void TestInspect()
        {
            m_data.SetRecipe(m_infoWafer.m_sRecipe);
            ezImg imgView = m_viewAlign.m_imgView.m_pImg;
            m_aoi.m_img.ReAllocate(imgView.m_szImg, 1);
            m_aoi.m_img.Copy(imgView, new CPoint(0, 0), new CPoint(0, 0), imgView.m_szImg);
            m_aoi.StartInspect(0, null);
            m_aoi.WaitInspect(1000);
            imgView.Copy(m_aoi.m_img, new CPoint(0, 0), new CPoint(0, 0), imgView.m_szImg);
            //RunManualAlign(); 
            m_auto.Invalidate(0);
        }

        bool RunGrabAlign()
        {
            if (m_infoWafer == null) return true;
            if (MoveCamPos(eProcess.Align)) return true;

            string strPath = m_strAlignPath;
            Directory.CreateDirectory(strPath);
            strPath += "\\" + m_infoWafer.m_nLoadPort.ToString() + "_" + m_infoWafer.m_nSlot.ToString("00");
            Directory.CreateDirectory(strPath);
            DirectoryInfo dir = new DirectoryInfo(strPath);
            FileInfo[] files = dir.GetFiles("*.bmp");
            foreach (FileInfo file in files)
            {
                File.Delete(file.FullName);
            }

            RunLight(eProcess.Align);
            RunVac(true);
            m_data.SetRecipe(m_infoWafer.m_sRecipe);
            m_axisRotate.Move(m_nPulseRotate + 100 + m_degReady * m_nPulseRotate / 360, m_fSlowRotate);

            double posTrigger = m_axisRotate.GetPos(true);
            int iTrigger = 0;
            bool bRingFrame = m_infoWafer.m_wafer.IsRingFrame();
            string strFile;
            ezStopWatch sw = new ezStopWatch();
            if (m_logMars != null) m_logMars.AddFunctionLog("Aligner", "Snap", MarsLog.eStatus.Start, this.m_infoWafer.m_idMaterial, MarsLog.eMateral.Wafer, null); 
            while ((iTrigger < m_nGrab[(int)m_infoWafer.m_wafer.m_eSize]) && (sw.Check() < m_msAlign))
            {
                if (m_axisRotate.GetPos(true) >= posTrigger)
                {
                    m_camAlign.Grab(m_aAOI[iTrigger].m_img, false);
                    strFile = strPath + "\\Align" + iTrigger.ToString("00") + ".bmp";
                    m_aAOI[iTrigger].StartInspect(m_axisRotate.GetPos(true) + m_offsetLive, strFile);
                    posTrigger += (m_nPulseRotate / m_nGrab[(int)m_infoWafer.m_wafer.m_eSize]);
                    iTrigger++;
                }
            }
            if (m_logMars != null) m_logMars.AddFunctionLog("Aligner", "Snap", MarsLog.eStatus.End, this.m_infoWafer.m_idMaterial, MarsLog.eMateral.Wafer, null); 
            if (sw.Check() > m_msAlign)
            {
                SetAlarm(eAlarm.Warning, eError.RotateTime);
                SetState(eState.Error);
                return true;
            }
            if (m_axisRotate.WaitReady())
            {
                SetAlarm(eAlarm.Warning, eError.Rotate);
                SetState(eState.Error);
                return true;
            }
            return false;
        }
    
        protected override void ProcAlign()
        {
            MarsLog.Datas datas = new MarsLog.Datas();
            datas.Add("TotalSubEvents", "ea", 2);
            string sLotID = m_auto.ClassHandler().ClassLoadPort(m_infoWafer.m_nLoadPort).m_infoCarrier.m_strLotID;
            if (m_logMars != null) m_logMars.AddProcessLog("Aligner", MarsLog.eProcess.Process, MarsLog.eStatus.Start, m_infoWafer.m_idMaterial, MarsLog.eMateral.Wafer, 1, sLotID, m_infoWafer.m_sRecipe, null, datas); 
            Align();
            if (m_logMars != null) m_logMars.AddProcessLog("Aligner", MarsLog.eProcess.Process, MarsLog.eStatus.End, m_infoWafer.m_idMaterial, MarsLog.eMateral.Wafer, 1, sLotID, m_infoWafer.m_sRecipe, null, null); 
        }  
        void Align() {
            if (!m_infoWafer.m_bUseAligner) // ing 171018
            {
                SetProcess(eProcess.BCR);
                return;
            }
            SetKlarf(ref m_infoWafer);

            if (m_bEnableAlignerEdge)   //KDG 160923 Add Edge Motion
            {
                if (RunLift(false) == eHWResult.Error)
                {
                    m_xGem.SetMSBAlarm(XGem300_Mom.eMSBAlarm.AL_AlignFail_Error);
                    SetAlarm(eAlarm.Warning, eError.Lift);
                    return;
                }
                Thread.Sleep(1000);
            }
            else if (m_bUseDefuser)
            {
                m_axisRotate.Move(0);
                if (RunLift(true) == eHWResult.Error)
                {
                    RunLift(false);
                    SetAlarm(eAlarm.Warning, eError.Lift);
                    SetState(eState.Error);
                    return;
                }
            }

            m_degNotch = 0; // ing 170402
            m_bAlignResult = false;

            string sLotID = m_auto.ClassHandler().ClassLoadPort(m_infoWafer.m_nLoadPort).m_infoCarrier.m_strLotID;  
            MarsLog.StepProcess step1 = new MarsLog.StepProcess(1, 1, "Grab");  
            MarsLog.StepProcess Centerning = new MarsLog.StepProcess(2, 2, "Centerning");  
            MarsLog.StepProcess step2 = new MarsLog.StepProcess(1, 2, "Find Notch");  
            int m_nSeq = 0;  

            if (m_bWTRShiftUse)
            {
                if (m_logMars != null) m_logMars.AddProcessLog("Aligner", MarsLog.eProcess.StepProcess, MarsLog.eStatus.Start, m_infoWafer.m_idMaterial, MarsLog.eMateral.Wafer, 1, sLotID, m_infoWafer.m_sRecipe, step1, null);  
                if (RunGrabAlign()) return;
                if (m_logMars != null) m_logMars.AddProcessLog("Aligner", MarsLog.eProcess.StepProcess, MarsLog.eStatus.End, m_infoWafer.m_idMaterial, MarsLog.eMateral.Wafer, 1, sLotID, m_infoWafer.m_sRecipe, step1, null);  

                ClearWaferShift();
                CalcWaferShift(0);

                m_nSeq++;
                Centerning.m_nSeq = m_nSeq;  
                if (m_logMars != null) m_logMars.AddProcessLog("Aligner", MarsLog.eProcess.StepProcess, MarsLog.eStatus.Start, m_infoWafer.m_idMaterial, MarsLog.eMateral.Wafer, 1, sLotID, m_infoWafer.m_sRecipe, Centerning, null);  

                if (MoveToCenter())
                {
                    SetAlarm(eAlarm.Warning, eError.AlignError);
                    SetState(eState.Error);
                    return;
                }
                if (m_logMars != null) m_logMars.AddProcessLog("Aligner", MarsLog.eProcess.StepProcess, MarsLog.eStatus.End, m_infoWafer.m_idMaterial, MarsLog.eMateral.Wafer, 1, sLotID, m_infoWafer.m_sRecipe, Centerning, null);  
            }
            m_nSeq++;
            step1.m_nSeq = m_nSeq; 
            if (m_logMars != null) m_logMars.AddProcessLog("Aligner", MarsLog.eProcess.StepProcess, MarsLog.eStatus.Start, m_infoWafer.m_idMaterial, MarsLog.eMateral.Wafer, 1, sLotID, m_infoWafer.m_sRecipe, step1, null);
            if (RunGrabAlign()) return;
            if (m_logMars != null) m_logMars.AddProcessLog("Aligner", MarsLog.eProcess.StepProcess, MarsLog.eStatus.End, m_infoWafer.m_idMaterial, MarsLog.eMateral.Wafer, 1, sLotID, m_infoWafer.m_sRecipe, step1, null);

            m_nSeq++;
            step2.m_nSeq = m_nSeq;
            if (m_logMars != null) m_logMars.AddProcessLog("Aligner", MarsLog.eProcess.StepProcess, MarsLog.eStatus.Start, m_infoWafer.m_idMaterial, MarsLog.eMateral.Wafer, 1, sLotID, m_infoWafer.m_sRecipe, step2, null);
             try
            {
                if (FindNotchIndex() < 0)
                {
                    if (m_infoWafer.m_wafer.IsRingFrame())
                    {
                        m_bAlignResult = true;
                        m_infoWafer.m_fAngleAlign = 0;
                        if (RotateDeg(0)) return;
                        if (m_bUseDefuser)
                        {
                            if (RunLift(false) == eHWResult.Error)
                            {
                                SetAlarm(eAlarm.Warning, eError.Lift);
                                SetState(eState.Error);
                                return;
                            }
                        }
                        SetProcess(eProcess.BCR);
                        return;
                    }
                    m_infoWafer.m_bPreAlignfail = true; 
                    SetAlarm(eAlarm.Warning, eError.AlignError);
                    SetState(eState.Error);
                    return;
                }
            }
             finally
             {
                 if (m_logMars != null) m_logMars.AddProcessLog("Aligner", MarsLog.eProcess.StepProcess, MarsLog.eStatus.End, m_infoWafer.m_idMaterial, MarsLog.eMateral.Wafer, 1, sLotID, m_infoWafer.m_sRecipe, step2, null);
             }
            HW_Aligner_ATI_AOI aoiMax = m_aAOI[m_indexNotch];

            MergeChipping();

            string strPath = m_strAlignPath;
            strPath += "\\" + m_infoWafer.m_nLoadPort.ToString() + "_" + m_infoWafer.m_nSlot.ToString("00");
            string strFile = string.Empty;
            int nTry = 0;
            while ((nTry == 0) || (Math.Abs(aoiMax.m_degNotch) > m_data.m_degAlign) && (nTry < 5))
            {
                m_fPosAlign = aoiMax.m_posAxis - aoiMax.m_degNotch * m_nPulseRotate / 360;
                if (RotateAxis(m_fPosAlign)) return;
                m_camAlign.Grab(m_aoi.m_img, false);
                m_log.Add("Image Size Check. X:" + m_aoi.m_img.m_szImg.x.ToString() + "Y" + m_aoi.m_img.m_szImg.x.ToString());
                strFile = strPath + "\\AlignNotch" + nTry.ToString() + ".bmp";
                m_aoi.StartInspect(m_axisRotate.GetPos(true), strFile);
                m_aoi.WaitInspect(1000);
                aoiMax = m_aoi;
                nTry++;
            }

            m_viewAlign.m_imgView.m_pImg.Copy(m_aoi.m_img, new CPoint(0, 0), new CPoint(0, 0), m_szCam);

            m_degNotch = GetDeg(aoiMax.m_posAxis) - aoiMax.m_degNotch;

            if (m_infoWafer.m_wafer.IsRingFrame())
            {
                m_infoWafer.m_fAngleAlign = m_degNotch - m_degCam + m_degVision;
            }

            aoiMax.m_img.FileSave(strPath + "\\AlignNotch.bmp");

            SaveKlarf();

            m_bAlignResult = true;

            if (m_bUseDefuser && !m_infoWafer.m_wafer.IsRingFrame()) // ing 171017 디뷰져 사용시 축 이동전 리프터 내리기 -> 시점이동 (Cognex OCR Bottom Camera 간섭)
            {
                if (RotateDeg(0)) return;
                if (RunLift(false) == eHWResult.Error)
                {
                    SetAlarm(eAlarm.Warning, eError.Lift);
                    SetState(eState.Error);
                    return;
                }
            }

            SetProcess(eProcess.BCR);
        
        }
        int FindNotchIndex()
        {
            double nMax = 0;  // BHJ 190722 edit    int -> double
            HW_Aligner_ATI_AOI aoiMax = null;
            m_indexNotch = -1; 
            foreach (HW_Aligner_ATI_AOI aoi in m_aAOI)
            {
                aoi.WaitInspect(100);
                m_log.Add(string.Format("#IN FindNotchIndex() : aoi.GetAlignSize() = {0}", aoi.GetAlignSize()), false); // BHJ 190405 add #alignFailDebug
                if (nMax < aoi.GetAlignSize())
                {
                    nMax = aoi.GetAlignSize();
                    aoiMax = aoi;
                }
            }

            if (aoiMax == null)
            {
                m_log.Add("Max Index not Found !!");
                SetProcess(eProcess.Rotate);
                return m_indexNotch;
            }

            m_indexNotch = aoiMax.m_nID;
            m_cpNotch = aoiMax.m_cpNotch;
            m_log.Add("Max Index is " + aoiMax.m_nID.ToString());
            return m_indexNotch;
        }

        void ClearWaferShift()  // BHJ 190226 add
        {
            m_infoWafer.m_dpWaferShift.x = 0.0;
            m_infoWafer.m_dpWaferShift.y = 0.0;
            m_log.Add("BHJ test log : Wafer Shift Data Clear");
        }

        void CalcWaferShift(int indexNotch)  // BHJ 191128 add
        {
            if (m_infoWafer == null) return;
            if (m_bWTRShiftUse)
            {
                if ((m_infoWafer.m_wafer.m_eSize == Wafer_Size.eSize.mm300_RF)) // BHJ 191128 add
                {
                    // BHJ 현재 링프레임은 스펙상 고려 하지 않음
                    //if (m_nRingNotchAngle >= 90 && m_nRingNotchAngle <= 270)
                    //{
                    //    m_infoWafer.m_dpWaferShift.x = (m_aAOI[(indexNotch + 75) % 100].CalcWaferShift() - m_aAOI[(indexNotch + 25) % 100].CalcWaferShift()) / 2.0;
                    //    m_infoWafer.m_dpWaferShift.y = (m_aAOI[(indexNotch + 50) % 100].CalcWaferShift() - m_aAOI[indexNotch].CalcWaferShift()) / 2.0;
                    //}
                    //else
                    //{
                    //    m_infoWafer.m_dpWaferShift.x = ((m_aAOI[(indexNotch + 75) % 100].CalcWaferShift() - m_aAOI[(indexNotch + 25) % 100].CalcWaferShift()) / 2.0) * -1;
                    //    m_infoWafer.m_dpWaferShift.y = ((m_aAOI[(indexNotch + 50) % 100].CalcWaferShift() - m_aAOI[indexNotch].CalcWaferShift()) / 2.0) * -1;
                    //}

                }
                else
                {
                    m_infoWafer.m_dpWaferShift.x = (m_aAOI[(indexNotch + 75) % 100].CalcWaferShift() - m_aAOI[(indexNotch + 25) % 100].CalcWaferShift()) / 2.0;
                    m_infoWafer.m_dpWaferShift.y = (m_aAOI[(indexNotch + 50) % 100].CalcWaferShift() - m_aAOI[indexNotch].CalcWaferShift()) / 2.0;
                    m_infoWafer.m_rtWaferShift.x = m_infoWafer.m_dpWaferShift.GetL(new RPoint(0, 0));
                    m_infoWafer.m_rtWaferShift.y = 180 * Math.Atan2(m_infoWafer.m_dpWaferShift.y, m_infoWafer.m_dpWaferShift.x) / Math.PI;
                }
            }
            else
            {
                m_infoWafer.m_dpWaferShift.x = 0.0;
                m_infoWafer.m_dpWaferShift.y = 0.0;
            }
            m_infoWafer.m_dpWaferShift *= m_nCamResolution;
            m_infoWafer.m_rtWaferShift.x *= m_nCamResolution;
            m_log.Add("Wafer Shift => X : " + m_infoWafer.m_dpWaferShift.x.ToString() + ", Y : " + m_infoWafer.m_dpWaferShift.y.ToString());
        }

        void RunManualAlign()
        {
         //   DialogResult dlgResult = m_manual.ShowDialog();
        }

        protected override void ProcBCR()
        {
            ReadBCR(); // ing 170401
            SetProcess(eProcess.OCR);
        }

        protected override void ReadBCR()
        {
            if (ReadWaferID()) // ing 161210 for MSB EFEM RingFrame
            {
                m_log.Popup("Reading Fail (RingWafer ID) !!");
                SetState(eState.Error);
                return;
            }
            if (m_infoWafer != null)
            {
                if (m_infoWafer.m_bUseBCR)
                {
                    RunLight(eProcess.BCR);
                    if (MoveCamPos(eProcess.BCR)) return;
                    m_camAlign.Grab(m_viewBCR.m_imgView.m_pImg, false);
                    if (m_infoWafer.m_sTypeBCR == "1D")
                    {
                        m_infoWafer.m_strWaferID = m_azBCR.Run(m_viewBCR.m_imgView.m_pImg, new CPoint(0, 0), m_szCam);
                        double nOrgAngleBCR = m_infoWafer.m_fAngleBCR;
                        for (int n = -m_nTryBCR / 2; n < m_nTryBCR / 2; n++)
                        {
                            m_infoWafer.m_fAngleBCR = nOrgAngleBCR + (n * m_dPeriod);
                            if (MoveCamPos(eProcess.BCR)) return;
                            m_camAlign.Grab(m_viewBCR.m_imgView.m_pImg, false);
                            Thread.Sleep(50);
                            m_infoWafer.m_strWaferID = m_azBCR.Run(m_viewBCR.m_imgView.m_pImg, new CPoint(0, 0), m_szCam);
                            if (m_infoWafer.m_strWaferID == "Unknown" || m_infoWafer.m_strWaferID.Length < m_nMinBCRLength) m_viewBCR.m_imgView.m_pImg.FileSave("C:\\Temp\\1DFail_" + (n + m_nTryBCR / 2).ToString() + ".bmp");
                            else break;
                        }
                        if (m_infoWafer.m_strWaferID == "Unknown" || m_infoWafer.m_strWaferID.Length < m_nMinBCRLength)
                        {
                            RunRotate(m_infoWafer.m_fAngleBCR + m_degCam - m_degNotch + m_infoWafer.m_fAngleChar);
                            m_camAlign.Grab(m_viewBCR.m_imgView.m_pImg, false);
                            Thread.Sleep(500);
                            m_viewBCR.m_imgView.m_pImg.Copy(m_viewBCR.m_imgView.m_pImg, new CPoint(0, 0), new CPoint(0, 0), m_szCam); // ing
                            ChangeCode(m_viewBCR);
                        }
                    }
                    if (m_infoWafer.m_sTypeBCR == "2D")
                    {
                        m_infoWafer.m_strWaferID = m_azBCR2D.Run(m_viewBCR.m_imgView.m_pImg, new CPoint(0, 0), m_szCam);
                        if ((m_infoWafer.m_strWaferID == "Unknown") || (m_infoWafer.m_strWaferID == "UnTrained"))
                        {
                            ChangeCode(m_viewBCR);
                        }
                    }
                    m_log.Add("Read BCR : " + m_infoWafer.m_strWaferID);
                }
            }
        }

        protected override void ProcOCR() 
        {
            if (m_infoWafer != null) ReadOCR(); // ing 170401
            SetProcess(eProcess.Edge);
        }

        protected override void ReadOCR()
        {
            if (m_infoWafer.m_bUseOCR)
            {
                RunLight(eProcess.OCR);
                if (MoveCamPos(eProcess.OCR)) return;
                if ((Recipe_Mom.eInspDir)m_infoWafer.m_nOCRDir == Recipe_Mom.eInspDir.Top) // ing 171010
                {
                    m_camOCR.ReadOCR(m_viewOCR.m_imgView.m_pImg);
                    /*
                    m_infoWafer.m_strWaferID = m_camOCR.m_strOCR.Replace("\r\n", "");
                    if ((m_infoWafer.m_strWaferID.Length == 0) || (m_infoWafer.m_strWaferID.Substring(0, 1) == "*"))
                    {
                        ChangeCode(m_viewOCR);
                    }
                     * */
                    if (m_camOCR.bOCRDone)      //20160928 SDH ADD OCR Process 순서 대로 진행되도록 Bool변수 추가
                    {
                        m_infoWafer.m_strWaferID = m_camOCR.m_strOCR.Replace("\r\n", "");
                        if (((m_infoWafer.m_strWaferID.Length == 0) || (m_infoWafer.m_strWaferID.Substring(0, 1) == "*")) || (m_camOCR.m_scoreOCR < m_infoWafer.m_fOCRReadScore))     //KDG 161025 Add OCR Score
                        //if (((m_infoWafer.m_strWaferID.Length == 0) || (m_infoWafer.m_strWaferID.Substring(0, 1) == "*")))  //KDG 161028 OCR Score를 가져오는 부분 없어서 우선 원복
                        {
                            ChangeCode(m_viewOCR);
                        }
                    }
                    else ChangeCode(m_viewOCR); // ing 170401
                }
                else
                {
                    m_camOCRBottom.ReadOCR(m_viewOCR.m_imgView.m_pImg);
                    if (m_camOCRBottom.bOCRDone)
                    {
                        m_infoWafer.m_strWaferID = m_camOCRBottom.m_strOCR.Replace("\r\n", "");
                        if (((m_infoWafer.m_strWaferID.Length == 0) || (m_infoWafer.m_strWaferID.Substring(0, 1) == "*")) || (m_camOCRBottom.m_scoreOCR < m_infoWafer.m_fOCRReadScore))
                        {
                            ChangeCode(m_viewOCR);
                        }
                    }
                    else ChangeCode(m_viewOCR);
                }
            }
            if (m_axisOCR != null) m_axisOCR.Move(0); // ing 171017 간섭 발생해서 추가함
        }

        protected override void ProcEdge()
        {
            if ((m_infoWafer != null) && m_aoiEdge.m_bUse)
            {
                if (EdgeInsepctionCheck())  //KDG 161028 Add Edge Vision Connect
                {
                    if (m_aoiEdge.EdgeInspectionReady()) return;    //KDG 160927 Add
                    m_aoiEdge.SendStart(m_infoWafer, m_fPosAlign, m_nPulseRotate);
                    //if (m_aoiEdge.WaitDone()) return;
                    if (m_aoiEdge.WaitDone())       //KDG 161028 Modify
                    {
                        SetAlarm(eAlarm.Warning, eError.EdgeInspError);
                        SetState(eState.Error);
                    }
                }
                else
                    SetState(eState.Error);
            }
            SetProcess(eProcess.Rotate);
        }

        protected override void ProcRotate()
        {
            if (m_infoWafer == null) return;
            if (m_logMars != null) m_logMars.AddFunctionLog("Aligner", "Align", MarsLog.eStatus.Start, this.m_infoWafer.m_idMaterial, MarsLog.eMateral.Wafer, null); 
            try 
            {
                RunLight(eProcess.Rotate);
                //RunVac(false);        //KDG 160923 Theta회전을 위해서 Delete

                //KDG 160923 Add
                if (m_infoWafer.m_wafer.IsRingFrame() || (m_bAlignResult == false))
                {
                    if (m_infoWafer.m_bRotate180) // ing 170531
                    {
                        if ((m_infoWafer.m_fAngleAlign < -45) || (m_infoWafer.m_fAngleAlign > 135))
                        {
                            RotateDeg(180);
                            m_infoWafer.m_fAngleAlign -= 180;
                            m_infoWafer.m_eAngleRF = Info_Wafer.eAngleRF.R180;
                            //m_infoWafer.m_queProc.Enqueue(Info_Wafer.eProc.Align); // ing 170331
                            m_log.Add(m_infoWafer.m_strWaferID + " : Rotate 180 degree");
                        }
                    }
                    else
                    {
                        RotateDeg(0);
                    }
                }
                else
                {
                    RunRotate(m_infoWafer.m_fAngleAlign + m_degVision);
                }

                RunVac(false);
                if (m_bAlignResult == false && m_infoWafer.m_bUseAligner) // ing 171018
                {
                    SetAlarm(eAlarm.Warning, eError.AlignError);
                    SetState(eState.Init);
                    return;
                }
                //m_auto.ClassXgem300().SetWaferIDForMapDown(m_infoWafer.m_strWaferID,m_infoWafer.m_nSlot);

                if (m_bEnableAlignerEdge)   //KDG 160923 Add Edge Motion
                {
                    if (RunLift(true) == eHWResult.Error)
                    {
                        SetAlarm(eAlarm.Warning, eError.Lift);
                        return;
                    }
                    Thread.Sleep(1000);
                }
                SetWaferIDForMapDown(m_infoWafer.m_strWaferID, m_infoWafer.m_nSlot);
                SetState(eState.Done);
            }
            finally
            {
                if (m_logMars != null) m_logMars.AddFunctionLog("Aligner", "Align", MarsLog.eStatus.End, this.m_infoWafer.m_idMaterial, MarsLog.eMateral.Wafer, null); 
            }
        }

        protected void SetWaferIDForMapDown(string sWaferID, int nSlot)
        {
            if (m_work.GetSSEMGEM() == null) return; // ing 161121
            if (((Gem_SSEM)m_work.GetSSEMGEM()).IsOnlineRemote())
                ((Gem_SSEM)m_work.GetSSEMGEM()).SetWaferIDForMapDown(sWaferID, nSlot);
        }

        protected override void ProcRotateRF(Info_Wafer.eAngleRF eAngle)
        {
            if (!m_infoWafer.m_bRotate180 || !m_infoWafer.m_wafer.IsRingFrame()) // ing 170531
            {
                if (RotateDeg(0))
                {
                    SetAlarm(eAlarm.Warning, eError.Rotate);
                    SetState(eState.Error);
                }
                SetState(eState.Done);
                return;
            }
            int nAngle = (int)eAngle - (int)m_infoWafer.m_eAngleRF;
            if (nAngle < 0) nAngle += 4;
            if (nAngle > 3) nAngle -= 4; 
            if (RotateDeg(90 * nAngle))
            {
                SetAlarm(eAlarm.Warning, eError.Rotate);
                SetState(eState.Error);
            }
            m_infoWafer.m_eAngleRF = (Info_Wafer.eAngleRF)(nAngle + m_infoWafer.m_eAngleRF);
            if ((int)m_infoWafer.m_eAngleRF < 0) m_infoWafer.m_eAngleRF += 4;
            if ((int)m_infoWafer.m_eAngleRF > 3) m_infoWafer.m_eAngleRF -= 4;
            SetState(eState.Done);
        }

        #endregion

        #region Error Process
        protected override void ProcError()
        {
        }
        #endregion

        protected override void RunTimer_200ms()
        {
            if (m_data.m_bLiveGrab)
            {
                Grab();
                m_viewAlign.InvalidView(false); 
            }
        }
        
        public override bool RunVac(bool bVac)
        {
            string strFuction = bVac ? "SuctionOn" : "SuctionOff";
            if (m_logMars != null) m_logMars.AddFunctionLog("Aligner", strFuction, MarsLog.eStatus.Start, (m_infoWafer != null) ? m_infoWafer.m_idMaterial : "$", MarsLog.eMateral.Wafer, null); 
            try
            {
                if (bVac)
                {
                    if (m_infoWafer == null) return false;
                    m_control.WriteOutputBit(m_doVac[GetRingFrame()], true);
                    if (m_bEnableAlignerEdge) m_control.WriteOutputBit(m_doVac[2], true);   //KDG 160923 Add Edge Vacuum
                }
                else
                {
                    m_control.WriteOutputBit(m_doVac[0], false);
                    m_control.WriteOutputBit(m_doVac[1], false);
                    if (m_bEnableAlignerEdge) m_control.WriteOutputBit(m_doVac[2], false);  //KDG 160923 Add Edge Vacuum
                }
                return false;
            }
            finally
            {
                if (m_logMars != null) m_logMars.AddFunctionLog("Aligner", strFuction, MarsLog.eStatus.End, (m_infoWafer != null) ? m_infoWafer.m_idMaterial : "$", MarsLog.eMateral.Wafer, null); 
            }
        }

        public override eHWResult UnloadLift()
        {
            if (!m_bEnableAlignerEdge)    //일반 Aligner : Lifter 1ea
            {
                //if (m_wafer.IsRingFrame())
                if (InfoWafer.m_wafer.m_eSize == Wafer_Size.eSize.mm300_RF) //1611216 KDG Mdofiy m_wafer시 정보 잘못 가져오는 경우 있어서 Infowafer로 변경
                {
                    if (RunLift(true) == eHWResult.Error)     //Ring Frame이면 Up
                    {
                        return eHWResult.Error; 
                    }
                }
                //else if (m_wafer.Is300mmWafer())              //Normal Wafer이면 Down
                else if (InfoWafer.m_wafer.m_eSize == Wafer_Size.eSize.mm300)   //KDG 1612116 Modify
                {
                    if (RunLift(false) == eHWResult.Error)
                    {
                        return eHWResult.Error;
                    }
                }
                else   //KDG 161216 Add
                {
                    return eHWResult.Error;
                }
            }
            else    //Stage Aligner : Lifter 2ea
            {
                if (RunLift(true) == eHWResult.Error) //RunLift함수에서 제품별로 처리
                {
                    return eHWResult.Error;
                }
            }
            return eHWResult.OK; 
        }

        public override bool IsReady(Info_Wafer infoWafer)
        {
            if (InfoWafer != null) return false;

            if (!m_bEnableAlignerEdge)
            {
                if (RunLift(infoWafer.m_wafer.IsRingFrame()) == eHWResult.Error)
                {
                    SetAlarm(eAlarm.Warning, eError.Lift);
                    return false;
                }
                return true; 
            }
            else
            {
                if (RunLift(true) == eHWResult.Error)
                {
                    SetAlarm(eAlarm.Warning, eError.Lift);
                    return false;
                }
                return true; 
            }
        }

        public override void Grab()
        {
            m_camAlign.Grab(m_viewAlign.m_imgView.m_pImg, false); 
        }

        int GetRingFrame()
        {
            if (m_infoWafer.m_wafer.IsRingFrame()) return 1;
            else return 0; 
        }

        void RunLight(eProcess ePro)
        {
            m_control.WriteOutputBit(m_doLightAlign[0], ((ePro == eProcess.Align) && m_bLightAlign[0]) || (ePro == eProcess.BCR));
            m_control.WriteOutputBit(m_doLightAlign[1], ((ePro == eProcess.Align) && m_bLightAlign[1]));
            m_control.WriteOutputBit(m_doLightAlign[2], ((ePro == eProcess.Align) && m_bLightAlign[2]) || (ePro == eProcess.BCR)); 
        }

        public override eHWResult RunLift(bool bUp)
        {
            //if (m_wafer.IsRingFrame() && m_auto.m_strWork == EFEM.eWork.MSB.ToString()) // ing 170402
            //{
            //    m_control.WriteOutputBit(m_doLift[1], bUp);     //Ring Frame Lifter Up
            //    m_control.WriteOutputBit(m_doLift[0], !bUp);    //Ring Frame Lifter Down
            //    m_control.WriteOutputBit(m_doLift[2], true);     //300mm Wafer Lifter Down
            //    m_control.WriteOutputBit(m_doLift[3], false);    //300mm Wafer Lifter Up
            //    Thread.Sleep(2000);
            //    return eHWResult.OK;
            //}
            //KDG 161006 Modify
            string strFuction = bUp ? "LiftPinUp" : "LiftPinDown";
            if (m_logMars != null) m_logMars.AddFunctionLog("Aligner", strFuction, MarsLog.eStatus.Start, (m_infoWafer != null) ? m_infoWafer.m_idMaterial : "$", MarsLog.eMateral.Wafer, null); 
            try
            {
                if (!m_bEnableAlignerEdge)      //Aligner Lifter 1ea
                {
                    m_control.WriteOutputBit(m_doLift[1], bUp);
                    m_control.WriteOutputBit(m_doLift[0], !bUp);
                    if (m_control.WaitInputBit(m_diLift[1], bUp)) return eHWResult.Error;
                    if (m_control.WaitInputBit(m_diLift[0], !bUp)) return eHWResult.Error;
                }
                else                            //Aligner Lifter 2ea(Ring Frame / 300mm Wafer)
                {
                    if (m_wafer.IsRingFrame())
                    {
                        m_control.WriteOutputBit(m_doLift[1], bUp);     //Ring Frame Lifter Up
                        m_control.WriteOutputBit(m_doLift[0], !bUp);    //Ring Frame Lifter Down
                        m_control.WriteOutputBit(m_doLift[2], true);     //300mm Wafer Lifter Down
                        m_control.WriteOutputBit(m_doLift[3], false);    //300mm Wafer Lifter Up
                        if (m_control.WaitInputBit(m_diLift[1], bUp)) return eHWResult.Error;   //Ring Frame Lifter Up
                        if (m_control.WaitInputBit(m_diLift[0], !bUp)) return eHWResult.Error;  //Ring Frame Lifter Down
                    }
                    else if (m_wafer.Is300mmWafer())
                    {
                        m_control.WriteOutputBit(m_doLift[0], true);     //Ring Frame Lifter Down
                        m_control.WriteOutputBit(m_doLift[1], false);    //Ring Frame Lifter Up
                        m_control.WriteOutputBit(m_doLift[3], bUp);     //300mm Wafer Lifter Up
                        m_control.WriteOutputBit(m_doLift[2], !bUp);    //300mm Wafer Lifter Down

                        if (m_control.WaitInputBit(m_diLift[0], true)) return eHWResult.Error;   //Ring Frame Lifter Down
                        if (m_control.WaitInputBit(m_diLift[1], false)) return eHWResult.Error;  //Ring Frame Lifter Up
                        if (m_control.WaitInputBit(m_diLift[2], bUp)) return eHWResult.Error;   //300mm Wafer Lifter Up
                    }
                }
                return eHWResult.OK;
            }
            finally
            {
                if (m_logMars != null) m_logMars.AddFunctionLog("Aligner", strFuction, MarsLog.eStatus.End, (m_infoWafer != null) ? m_infoWafer.m_idMaterial : "$", MarsLog.eMateral.Wafer, null); 
            }

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
            m_aoiEdge.LotStart();
            m_klarf.LotStart();
        }

        public override void LotEnd()
        {
            if (m_bChipping)
            {
                Thread.Sleep(5000);
                ezStopWatch swSave = new ezStopWatch();
                while (swSave.Check() < m_msSaveTimeout)
                {
                    Thread.Sleep(10);
                    if (!m_klarf.m_bBusy) break;
                }
                if (m_klarf.m_bBusy)
                {
                    m_log.Popup("Klarf File Save Fail, Can not Save Lotend File !!");
                    return;
                }
                    StreamWriter sw;
                    try
                    {
                        sw = new StreamWriter(new FileStream(m_strKlarfPath + "\\" + m_strKlarfName + ".trf", FileMode.Create));
                    }
                    catch (Exception ex)
                    {
                        m_log.Popup(m_strKlarfPath + "\\" + m_strKlarfName + ".trf Is Wrong Path !!");
                        m_log.Add(ex.Message);
                        return;
                    }
                    try
                    {
                        sw.WriteLine("FileVersion " + m_klarf.m_nFileVer1.ToString() + " " + m_klarf.m_nFileVer2.ToString() + ";");
                        m_klarf.Put_FileTimestamp(sw);
                        sw.WriteLine("InspectionStationID " + Convert.ToChar(34) + m_klarf.m_strInspectionStationVender + Convert.ToChar(34) + " " + Convert.ToChar(34) + m_klarf.m_strInspectionStationModel + Convert.ToChar(34) + " " + Convert.ToChar(34) + m_klarf.m_strInspectionStationMachineID + Convert.ToChar(34) + ";");
                        m_klarf.Put_ResultTimestamp(sw);
                        //m_klarf.m_strLotID = m_klarf.m_strLotID.Substring(0, m_klarf.m_strLotID.IndexOf("_"));          //말도안되
                        sw.WriteLine("LotID " + Convert.ToChar(34) + m_klarf.m_strLotID + "-" + m_klarf.m_strCSTID + Convert.ToChar(34) + ";");
                        sw.WriteLine("EndOfLotInspection;");
                        sw.WriteLine("EndOfFile;");
                        sw.Close();
                        m_log.Add(m_strKlarfPath + "\\" + m_strKlarfName + ".trf LotEnd File Saved.");
                    }
                    catch (Exception ex)
                    {
                        m_log.Popup(m_strKlarfPath + "\\" + m_strKlarfName + ".trf Create Error !!");
                        m_log.Add(ex.Message);
                        sw.Close();
                    }

                    // Backup Lotend
                    // ing 170222
                    try
                    {
                        sw = new StreamWriter(new FileStream(m_strKlarfBackupPath + "\\" + m_strKlarfName + ".trf", FileMode.Create));
                    }
                    catch (Exception ex)
                    {
                        m_log.Popup(m_strKlarfBackupPath + "\\" + m_strKlarfName + ".trf Is Wrong Path !!");
                        m_log.Add(ex.Message);
                        return;
                    }
                    try
                    {
                        sw.WriteLine("FileVersion " + m_klarf.m_nFileVer1.ToString() + " " + m_klarf.m_nFileVer2.ToString() + ";");
                        m_klarf.Put_FileTimestamp(sw);
                        sw.WriteLine("InspectionStationID " + Convert.ToChar(34) + m_klarf.m_strInspectionStationVender + Convert.ToChar(34) + " " + Convert.ToChar(34) + m_klarf.m_strInspectionStationModel + Convert.ToChar(34) + " " + Convert.ToChar(34) + m_klarf.m_strInspectionStationMachineID + Convert.ToChar(34) + ";");
                        m_klarf.Put_ResultTimestamp(sw);
                        //m_klarf.m_strLotID = m_klarf.m_strLotID.Substring(0, m_klarf.m_strLotID.IndexOf("_"));          //말도안되
                        sw.WriteLine("LotID " + Convert.ToChar(34) + m_klarf.m_strLotID + "-" + m_klarf.m_strCSTID + Convert.ToChar(34) + ";");
                        sw.WriteLine("EndOfLotInspection;");
                        sw.WriteLine("EndOfFile;");
                        sw.Close();
                        m_log.Add(m_strKlarfBackupPath + "\\" + m_strKlarfName + ".trf LotEnd File Saved.");
                    }
                    catch (Exception ex)
                    {
                        m_log.Popup(m_strKlarfBackupPath + "\\" + m_strKlarfName + ".trf Create Error !!");
                        m_log.Add(ex.Message);
                        sw.Close();
                    }
            }
        }

        bool EdgeInsepctionCheck()  //KDG 161007 Add Edge Vision 연결상태
        {
            if (m_bEnableAlignerEdge)
            {
                if (!m_aoiEdge.IsConnected()) return false;
                else return true;
            }
            else return true;
        }

        void MergeChipping()
        {
            if (!m_bChipping) return;
            int szImgX2 = m_aAOI[0].m_szImg.x / 2;
            for (int n = 0; n < 100; n++)
            {
                for (int x = 0; x < m_aAOI[n].m_aChippingInfo.Count - 1; x++)
                {
                    Defect defect0, defect1;
                    defect0 = (Defect)m_aAOI[n].m_aChippingInfo[x];
                    defect1 = (Defect)m_aAOI[n].m_aChippingInfo[x + 1];
                    if (Math.Abs(defect0.m_cp1.x - defect1.m_cp0.x) < m_nMerge)
                    {
                        if (defect0.m_nSize > defect1.m_nSize)
                        {
                            defect0.m_cp1.x = defect1.m_cp1.x;
                            if (defect0.m_cp0.y < defect1.m_cp0.y) defect0.m_cp0.y = defect0.m_cp1.y = defect1.m_cp0.y;
                            m_aAOI[n].m_aChippingInfo.RemoveAt(x + 1);
                        }
                        else
                        {
                            defect1.m_cp0.x = defect0.m_cp0.x;
                            if (defect0.m_cp0.y < defect1.m_cp0.y) defect0.m_cp0.y = defect0.m_cp1.y = defect1.m_cp0.y;
                            m_aAOI[n].m_aChippingInfo.RemoveAt(x);
                        }
                        x--;
                    }
                }
            }
            for (int n = 0; n < 100; n++)
            {
                for (int x = m_aAOI[n].m_aChippingInfo.Count - 1; x >= 0; x--)
                {
                    int dx = (int)Math.Abs((((Defect)(m_aAOI[n].m_aChippingInfo[x])).m_cp0.x + ((Defect)(m_aAOI[n].m_aChippingInfo[x])).m_cp1.x) / 2 - szImgX2);
                    if (dx > m_deltaChipping) m_aAOI[n].m_aChippingInfo.RemoveAt(x);
                }
            }
            for (int n = 0; n < 100; n++)
            {
                MergeChipping(m_aAOI[n].m_aChippingInfo, m_aAOI[(n + 1) % 100].m_aChippingInfo, m_deltaChipping);
                MergeChipping(m_aAOI[n].m_aChippingInfo, m_aAOI[(n + 99) % 100].m_aChippingInfo, -m_deltaChipping);
            }
            for (int n = m_aAOI[m_indexNotch].m_aChippingInfo.Count - 1; n >= 0; n--)
            {
                int dx = (int)Math.Abs((((Defect)(m_aAOI[m_indexNotch].m_aChippingInfo[n])).m_cp0.x + ((Defect)(m_aAOI[m_indexNotch].m_aChippingInfo[n])).m_cp1.x) / 2 - m_cpNotch.x);
                if (dx < m_deltaChipping / 5) 
                {
                    m_aAOI[m_indexNotch].m_aChippingInfo.RemoveAt(n);
                    n--; 
                }
            }
            for (int n = 0; n < 100; n++)
            {
                for (int nChipping = 0; nChipping < m_aAOI[n].m_aChippingInfo.Count; nChipping++)
                {
                    Defect def = (Defect)m_aAOI[n].m_aChippingInfo[nChipping];
                    CPoint cp = (def.m_cp1 + def.m_cp0) / 2;
                    double fAngle = GetChippingAngle(n, cp);
                    if (fAngle < 0.2 || fAngle > 359.8)
                    {
                        m_aAOI[n].m_aChippingInfo.RemoveAt(nChipping);
                        nChipping--;
                        continue;
                    }
                    def.m_fAngle = fAngle;
                    m_log.Add("Chipping" + n.ToString("00 = (") + cp.x.ToString() + ", " + def.m_cp0.y.ToString() + "), " + fAngle.ToString());

                }
            }
        }

        void MergeChipping(ArrayList aChipping0, ArrayList aChipping1, int dX)
        {
            int deltaChipping = m_deltaChipping / 5; 
            int szImgX2 = m_aAOI[0].m_szImg.x / 2; 
            for (int n0 = aChipping0.Count - 1; n0 >= 0; n0--)
            {
                int x0 = (((Defect)aChipping0[n0]).m_cp0.x + ((Defect)aChipping0[n0]).m_cp1.x) / 2; 
                for (int n1 = aChipping1.Count - 1; n1 >= 0; n1--)
                {
                    int x1 = (((Defect)aChipping1[n1]).m_cp0.x + ((Defect)aChipping1[n1]).m_cp1.x) / 2 + dX;
                    int deltaX = (int)Math.Abs(x0 - x1); 
                    if (deltaX < deltaChipping)
                    {
                        int dx0 = (int)Math.Abs((((Defect)aChipping0[n0]).m_cp0.x + ((Defect)aChipping0[n0]).m_cp1.x) / 2 - szImgX2);
                        int dx1 = (int)Math.Abs((((Defect)aChipping0[n0]).m_cp0.x + ((Defect)aChipping0[n0]).m_cp1.x) / 2 - szImgX2);
                        if (dx0 < dx1)
                        {
                            aChipping1.RemoveAt(n1);
                        }
                        else
                        {
                            aChipping0.RemoveAt(n0);
                            n1 = -1;
                        }
                    }
                }
            }
        }

        double GetChippingAngle(int nIndex, CPoint cp)
        {
            double fAngle = 3.6 * (nIndex - m_indexNotch + 1.0 * (cp.x - m_cpNotch.x) / m_deltaChipping);
            while (fAngle < 0) fAngle += 360; 
            return fAngle; 
        }

        void SetKlarf(ref Info_Wafer infoWafer)
        {
            Info_Carrier infoCarrier;
            if (infoWafer == null) infoWafer = new Info_Wafer(-1, -1, -1, m_log, Wafer_Size.eSize.mm300);
            infoCarrier = m_auto.ClassHandler().ClassCarrier(infoWafer.m_nLoadPort);
            if (infoCarrier == null)
            {
                infoCarrier = new Info_Carrier(-1);
                infoCarrier.m_wafer = new Wafer_Size(Wafer_Size.eSize.mm300, m_log);
            }

            m_klarf.m_strTimeResult = "";
            m_klarf.m_strWaferID = infoWafer.m_nSlot.ToString("00");
            m_klarf.m_strRecipe = infoCarrier.m_strRecipe;
            m_klarf.m_strTiffFileName = "";
            m_klarf.m_strSampleTestPlan = "";
            m_klarf.m_strAreaPerTest = "";

            m_klarf.m_nSampleSize = 300;
            m_klarf.m_nFileVer1 = 1;
            m_klarf.m_nFileVer2 = 1;
            m_klarf.m_nKlarfRow = 0;
            m_klarf.m_nKlarfCol = 0;
            m_klarf.m_nSlot = infoWafer.m_nSlot;
            m_klarf.m_nInspectionTest = 1;
            m_klarf.m_nSampleTestCnt = 1;
            m_klarf.m_nDefectDieCnt = 0;
            m_klarf.m_nCenterX = 0;
            m_klarf.m_nCenterY = 0;
            if (infoCarrier.m_wafer.m_eSize == Wafer_Size.eSize.mm300)
            {
                m_klarf.m_fDiePitchX = 300000;
                m_klarf.m_fDiePitchY = 300000;
                m_klarf.m_fSampleCenterLocationX = 1.500000e+005;
                m_klarf.m_fSampleCenterLocationY = 1.500000e+005;
            }
            m_klarf.m_fDieOriginX = 0.000000e+000;
            m_klarf.m_fDieOriginY = 0.000000e+000;

        }

        void SaveKlarf()
        {
            if (!m_bChipping) return;
            //m_strKlarfName = "LotEnd_" + m_auto.ClassRecipe().m_sRecipe + "_" + m_klarf.m_strLotID + "_00-" + m_klarf.m_nSlot.ToString("00");
            //m_klarf.m_strTiffFileName = m_auto.ClassRecipe().m_sRecipe + "_" + m_klarf.m_strLotID + "_00-" + m_klarf.m_nSlot.ToString("00") + ".tif";
            m_strKlarfName = "LotEnd_" + m_auto.ClassRecipe().m_sRecipe + "_" + m_klarf.m_strCSTID + "_00-" + m_klarf.m_nSlot.ToString("00");
            m_klarf.m_strTiffFileName = m_auto.ClassRecipe().m_sRecipe + "_" + m_klarf.m_strCSTID + "_00-" + m_klarf.m_nSlot.ToString("00") + ".tif";
            string strDate = DateTime.Now.Date.Year.ToString() + DateTime.Now.Date.Month.ToString() + DateTime.Now.Date.Day.ToString();
            Directory.CreateDirectory(m_strKlarfBackupPath);
            Directory.CreateDirectory(m_strKlarfBackupPath + "\\" + strDate);
            //string strPath = m_strKlarfPath + "\\" + m_auto.ClassRecipe().m_sRecipe + "_" + m_klarf.m_strLotID + "_00-" + m_klarf.m_nSlot.ToString("00");
            //string strPathBackup = m_strKlarfBackupPath + "\\" + strDate + "\\" + m_auto.ClassRecipe().m_sRecipe + "_" + m_klarf.m_strLotID + "_00-" + m_klarf.m_nSlot.ToString("00");
            string strPath = m_strKlarfPath + "\\" + m_auto.ClassRecipe().m_sRecipe + "_" + m_klarf.m_strCSTID + "_00-" + m_klarf.m_nSlot.ToString("00");
            string strPathBackup = m_strKlarfBackupPath + "\\" + strDate + "\\" + m_auto.ClassRecipe().m_sRecipe + "_" + m_klarf.m_strCSTID + "_00-" + m_klarf.m_nSlot.ToString("00");
            m_klarf.SaveKlarf(strPath, true, m_aAOI);
            m_klarf.SaveKlarf(strPathBackup, true, m_aAOI);
        }

        ezImg m_imgWaferEmpty = null;
        ezImg m_imgWaferCheck = null;

        public override eHWResult CheckWaferExist(bool bVacCheck = false)
        {
            bool bExist = m_control.GetInputBit(m_diWaferExist);
            bool bVacOn = false;
            if (bVacCheck && m_infoWafer != null)
            {
                if (m_infoWafer.m_wafer.IsRingFrame())
                {
                    bVacOn = m_control.GetOutputBit(m_doVac[1]);
                    m_control.WriteOutputBit(m_doVac[1], true);
                    Thread.Sleep(500);
                    bExist = m_control.GetInputBit(m_diWaferCheck[1]);
                    m_control.WriteOutputBit(m_doVac[1], bVacOn);
                }
                else
                {
                    bVacOn = m_control.GetOutputBit(m_doVac[0]);
                    m_control.WriteOutputBit(m_doVac[0], true);
                    Thread.Sleep(500);
                    bExist = m_control.GetInputBit(m_diWaferCheck[0]);
                    m_control.WriteOutputBit(m_doVac[0], bVacOn);
                }
            }
//          if ((m_infoWafer != null) && (bExist == false)) bExist = CheckWaferExistCam(); 
            if (bExist) m_waferExist = eHWResult.On;
            else m_waferExist = eHWResult.Off;
            return m_waferExist; 
        }

        bool CheckWaferExistCam()
        {
            if (m_imgWaferCheck == null) m_imgWaferCheck = new ezImg("WaferCheck", m_log);
            CheckWaferGrab(m_imgWaferCheck);
            bool bExist = InspectWaferExist();
            if (bExist == false)
            {
                m_infoWafer = null;
                m_log.Popup("Aligner InfoWafer deleted !!");
            }
            return bExist; 
        }

        eHWResult CheckWaferGrab(ezImg imgGrab)
        {
            if (MoveCamPos(eProcess.Align)) return eHWResult.Error;
            RunLight(eProcess.Align);
            m_camAlign.Grab(m_imgWaferEmpty, false);
            RunLight(eProcess.Rotate);
            if (MoveCamPos(eProcess.Rotate)) return eHWResult.Error;
            return eHWResult.OK; 
        }

        bool InspectWaferExist()
        {
            //forget m_imgWaferCheck 비교 m_diWaferExist
            return false; 
        }

        public override ezView ClassView()
        {
            return m_viewAlign;
        }

    }
}
