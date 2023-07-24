using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ezAutoMom;
using ezTools;
using System.IO;
using SevenZip;  

namespace ezAuto_EFEM
{
    class EFEM_Handler : Handler_Mom, Control_Child
    {
        
        const int c_msHome = 90000;

        Auto_Mom m_auto;
        string[] m_strLoadPort = new string[3] { "ATI", "ATI", "ATI" };
        string[] m_strLoadPorts = new string[4] { "ATI", "RND", "RND4568", "TDK" };
        string[] m_strRFID = new string[3] { "CEYON", "BROOKS","Multi" };
        string[] m_strRFIDs = new string[2] { "BROOKS", "CEYON"};
        string m_sAligner = "Tazmo";
        string[] m_sAligners = new string[5] { "ATI", "Tazmo", "EBR", "EBRFix", "LineScan" };
        string m_sWTR = "Tazmo";
        string[] m_sWTRs = new string[2] { "Tazmo", "RND" };
        public string m_sBackSide = "No";
        string[] m_sBacksides = new string[2] { "No", "ATI" };
        public HW_LoadPort_Mom[] m_loadport;
        public HW_WTR_Mom m_wtr;
        public HW_VisionWorks_Mom m_visionWorks = new HW_VisionWorks();
        public HW_ImageVS_Mom m_imageVS = new HW_ImageVS();
        public HW_FDC_Mom m_FDC = new HW_FDC_Mom();
        public HW_RFID_Mom[] m_RFID;
        public HW_BackSide_Mom m_backSide = null;
        public HW_RemoteUI m_remoteUI = new HW_RemoteUI();
        public HW_PowerMeter m_powermeter = new HW_PowerMeter(); // ing 170626
        XGem300_Mom m_xGem;
        EFEM_HomeProgressBar m_progressHome;
        public HW_FFU_Mom m_ffu; // MarsFFU 
        public MarsLog m_marslog;
        public string m_strPathMarsLog = "C:\\Logs\\EventLog\\"; 
        bool m_bUseIonizer = false;
        int m_diIonizerLoadPort = -1;
        bool m_bIonizerLoadPort = true; 
        int m_diIonizerAlign = -1;
        bool m_bIonizerAlign = true; 
        int m_diIonizerVisionStage = -1;
        bool m_bIonizerVisionStage = true; 
        int m_diIonizerVisionLoad = -1;
        bool m_bIonizerVisionLoad = true;
        int m_doIonizer = -1;
        int m_doIonizerVision = -1;
        int m_diMCReset = -1;
        bool m_bMCReset = true; 
        int m_diKeyLock = -1;
        bool m_bKeyLock = true;
        int[] m_diFFU = new int[3] { -1, -1, -1 };
        bool m_bFFU = true;
        //bool m_bAlignerEdgeInsp = false;        //KDG 160912 Add Edge Inspection        //KDG 161006 Delete
        public bool m_bManualRecovery = false;
    
        bool m_bRunThread = false;
        Thread m_thread;

        public EFEM_Handler()
        {
        }

        public override int GetMCResetIONUM()
        {
            return m_diMCReset;
        }

        public override void Init(string id, object auto)
        {
            int n;
            m_auto = (Auto_Mom)auto;
            base.Init(id, auto); 
            m_control.Add(this);
            m_xGem = m_auto.ClassXGem();
            m_log.m_reg.Read("LoadPort", ref m_nLoadPort); 
            m_loadport = new HW_LoadPort_Mom[m_nLoadPort];
            m_RFID = new HW_RFID_Mom[m_nLoadPort];

            m_aligner = NewAligner(); 
            m_wtr = NewWTR();
            for (n = 0; n < m_nLoadPort; n++)
            {
                m_loadport[n] = NewLoadPort(n);
                m_loadport[n].Init(n, m_auto);
                m_RFID[n] = NewRFID(n);
                m_RFID[n].Init("RFID"+n.ToString(), m_auto);
            }
            m_auto.m_InitState.SetInitDone(InitState.eInitModule.Loadport);
            m_backSide = NewBackSide();

            m_aligner.Init("Aligner", m_auto);
            m_auto.m_InitState.SetInitDone(InitState.eInitModule.Aligner);
            m_backSide.Init("BackSide", m_auto);
            m_wtr.Init("WTR", m_auto);
            m_auto.m_InitState.SetInitDone(InitState.eInitModule.WTR);
            m_visionWorks.Init("VisionWorks", m_auto);
            m_auto.m_InitState.SetInitDone(InitState.eInitModule.Vision);
            m_imageVS.Init("ImageVS", m_auto);
            m_FDC.Init("FDC", m_auto);
            if (m_bFFUUse) // MarsFFU
            {
                m_ffu = new HW_FFU_Mars();
                if (m_nFFUModuleNum < 0) m_bFFUUse = false;
                else m_ffu.Init("FFU", m_auto, m_nFFUModuleNum);
            } 
            m_remoteUI.Init("RemoteUI", m_auto);
            m_powermeter.Init("PowerMeter", m_auto); // ing 170626
            ShowChild();
            InitString();
            m_thread = new Thread(new ThreadStart(RunThread)); m_thread.Start();
            m_progressHome = new EFEM_HomeProgressBar(m_auto);
            m_marslog = m_auto.ClassMarsLog(); 
        }

        HW_LoadPort_Mom NewLoadPort(int nIndex)
        {
            if (m_strLoadPort[nIndex] == "ATI") return new HW_LoadPort_ATI(); 
            if (m_strLoadPort[nIndex] == "RND") return new HW_LoadPort_RND();
            if (m_strLoadPort[nIndex] == "RND4568") return new HW_LoadPort_RND_4568();
            if (m_strLoadPort[nIndex] == "TDK") return new HW_LoadPort_TDK(); 
            return new HW_LoadPort_ATI(); 
        }

        HW_Aligner_Mom NewAligner()
        {
            if (m_sAligner == "ATI") return new HW_Aligner_ATI();
            if (m_sAligner == "Tazmo") return new HW_Aligner_Tazmo();
            if (m_sAligner == "EBR") return new HW_Aligner_EBR();
            if (m_sAligner == "EBRFix") return new HW_Aligner_EBRFix();
            if (m_sAligner == "LineScan") return new HW_Aligner_LineScan();
            return new HW_Aligner_Tazmo(); 
        }

        HW_BackSide_Mom NewBackSide()
        {
            if (m_sBackSide == "ATI") return new HW_BackSide_ATI();
            return new HW_BackSide_Nothing(); 
        }

        HW_WTR_Mom NewWTR()
        {
            if (m_sWTR == "RND") return new HW_WTR_RND();
            if (m_sWTR == "Tazmo") return new HW_WTR_Tazmo();
            return new HW_WTR_Tazmo(); 
        }

        HW_RFID_Mom NewRFID(int nIndex)
        {
            if (m_strRFID[nIndex] == "BROOKS") return new HW_RFID_Brooks();
            if (m_strRFID[nIndex] == "CEYON") return new HW_RFID_Ceyon();
            return new HW_RFID_Ceyon();
        }

        public void ThreadStop()
        {
            int n;
            RunIonizer(false); 
            if (m_bRunThread) { m_bRunThread = false; m_thread.Join(); }
            for (n = 0; n < m_nLoadPort; n++) m_loadport[n].ThreadStop();
            m_aligner.ThreadStop();
            m_wtr.ThreadStop();
            m_visionWorks.ThreadStop();
            m_imageVS.ThreadStop();
            m_backSide.ThreadStop();
            m_remoteUI.ThreadStop();
            m_powermeter.ThreadStop(); // ing 170626
            if (m_bFFUUse && m_ffu != null) m_ffu.ThreadStop(); // MarsFFU 
        }

        void InitString()
        {
            InitString(eError.Home, "설비 Initialize에 실패 하였습니다.");
            InitString(eError.HomeWTR, "WTR의 Initialize에 실패 하였습니다. WTR 연결을 확인하여 주세요");
            InitString(eError.HomeAligner, "얼라이너 Initialize에 실패 하였습니다. 축 동작 상태를 확인하여 주세요.");
            InitString(eError.HomeVision, "Search Home Fail (VisionWorks) Please Check TCP Connect!!");
            InitString(eError.HomeLoadport, "LoadPort 의 Initialize에 실패하였습니다. Loadport 연결을 확인하여 주세요.");
            InitString(eError.AlignerError, "Aligner State Error(Lifter State Check) !!");
            InitString(eError.eMCReset, "MC Reset 시그널이 꺼졌습니다. 장비의 MC Rest 버튼을 확인하여 주세요.");
            InitString(eError.eIonizer," 설비 내부의 Ionizer에 이상이 감지 되었습니다. Ionizer를 확인해 주세요.");
            InitString(eError.FFU, "설비의 FFU 동작에 이상이 감지되었습니다. FFU 를 확인해 주세요.");

            InitString(eError.FDCError_CDA, "FDC CDA Error!!");
            InitString(eError.FDCError_Vacuum, "FDC Vacuum Error!!");
            InitString(eError.FDCError_VisionPressure, "FDC VisionPressure Error!!"); 
            InitString(eError.FDCError_EFEMPressure, "FDC EFEMPressure Error!!"); 
            InitString(eError.FDCError_Temp, "FDC Temp Error!!"); 
            InitString(eError.FDCError_Elect, "FDC Elect Error!!"); 
            InitString(eError.FDCError_Vacuum1, "FDC Vacuum1 Error!!"); 
            InitString(eError.FDCError_Vacuum2, "FDC Vacuum2 Error!!"); 
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

        public override void ShowChild()
        {
            int n; 
            CPoint cpShow = new CPoint(10, 10);
            for (n = 0; n < m_nLoadPort; n++)
            {
                m_loadport[n].ShowDlg(this, ref cpShow);
            }
            m_aligner.ShowDlg(this, ref cpShow);
            m_wtr.ShowDlg(this, ref cpShow);
            m_visionWorks.ShowDlg(this, ref cpShow);
            m_imageVS.ShowDlg(this, ref cpShow);
            m_backSide.ShowDlg(this, ref cpShow); 
            m_FDC.ShowDlg(this, ref cpShow);
            if (m_ffu != null && m_bFFUUse) m_ffu.ShowDlg(this, ref cpShow); // MarsFFU 
            for (n = 0; n < m_nLoadPort; n++)
            {    
                 m_RFID[n].ShowDlg(this, ref cpShow);
            }
            m_remoteUI.ShowDlg(this, ref cpShow);
            m_powermeter.ShowDlg(this, ref cpShow); // ing 170626
        }

        protected override void RunTimer()
        {
            if (m_progressHome == null) return;
            if (m_progressHome.m_bShow) m_progressHome.Show();
            else m_progressHome.Hide();
        }

        void RunIonizer(bool bRun)
        {
            m_control.WriteOutputBit(m_doIonizer, bRun);
            m_control.WriteOutputBit(m_doIonizerVision, bRun);
        }

        void CheckDIO()
        {
            if (m_control.GetInputBit(m_diMCReset) != m_bMCReset)
            {
                SetAlarm(eAlarm.Error, eError.eMCReset);
            }
            if (m_diKeyLock > -1 && m_control.GetInputBit(m_diKeyLock) == m_bKeyLock)
            {
                SetXgemOffline();
            }
            if ((m_control.GetInputBit(m_diFFU[0]) != m_bFFU) && m_diFFU[0] > -1 || (m_control.GetInputBit(m_diFFU[1]) != m_bFFU && m_diFFU[1] > -1) || (m_control.GetInputBit(m_diFFU[2]) != m_bFFU && m_diFFU[2] > -1))
            {
                SetAlarm(eAlarm.Error, eError.FFU);
            }
            if (m_control.GetOutputBit(m_doIonizer) == false)
            {
                //SetAlarm(eAlarm.Error, eError.eIonizer);           
            }
            if (m_control.GetOutputBit(m_doIonizerVision) == false)
            {
                //SetAlarm(eAlarm.Error, eError.eIonizer);           
            }
            if (m_control.GetInputBit(m_diIonizerAlign) != m_bIonizerAlign && m_bUseIonizer) // ing 170206
            {
                SetAlarm(eAlarm.Error, eError.eIonizer);           
            }
            if (m_control.GetInputBit(m_diIonizerLoadPort) != m_bIonizerLoadPort && m_bUseIonizer) // ing 170206
            {
                SetAlarm(eAlarm.Error, eError.eIonizer);           
            }
            if (m_diIonizerVisionLoad != -1 && m_control.GetInputBit(m_diIonizerVisionLoad) != m_bIonizerVisionLoad && m_bUseIonizer) // ing 170206
            {
                SetAlarm(eAlarm.Error, eError.eIonizer);           
            }
            if (m_diIonizerVisionStage != -1 && m_control.GetInputBit(m_diIonizerVisionStage) != m_bIonizerVisionStage && m_bUseIonizer) // ing 170206
            {
                SetAlarm(eAlarm.Error, eError.eIonizer);           
            }
        }

        void SetXgemOffline()
        {
            for (int n = 0; n < m_nLoadPort; n++)
            {
                if (m_xGem.m_aXGem300Carrier[n].m_ePortAccess == XGem300Carrier.ePortAccess.Auto)
                {
                    m_log.Popup(string.Format("Interlock Key turned to MANUAL : {0} Access Mode go Manual", m_xGem.m_aXGem300Carrier[n].m_sLocID));
                    m_xGem.LPAccessModeChange(XGem300Carrier.ePortAccess.Manual, m_xGem.m_aXGem300Carrier[n].m_sLocID);
                }
            } 
            
            if (m_auto.m_bXgemUse && m_xGem.IsOnline())
            {
                m_log.Popup("Interlock Key turned to MANUAL : XGEM go Offline");
                m_xGem.XGemOffline();
            }
        }

        public void SetInfoWafer()
        {
            m_aligner.InfoWafer = FindInfoWafer(Info_Wafer.eLocate.Aligner, m_aligner.CheckWaferExist(true) == eHWResult.On);
            m_visionWorks.InfoWafer = FindInfoWafer(Info_Wafer.eLocate.Vision, m_visionWorks.IsWaferExist() == eHWResult.On);
            m_backSide.InfoWafer = FindInfoWafer(Info_Wafer.eLocate.BackSide, m_backSide.IsWaferExist() == eHWResult.On);
            m_wtr.SetInfoWafer(HW_WTR_Mom.eArm.Lower, FindInfoWafer(Info_Wafer.eLocate.WTR_Lower, m_wtr.CheckWaferExist(HW_WTR_Mom.eArm.Lower) == eHWResult.On));
            m_wtr.SetInfoWafer(HW_WTR_Mom.eArm.Upper, FindInfoWafer(Info_Wafer.eLocate.WTR_Upper, m_wtr.CheckWaferExist(HW_WTR_Mom.eArm.Upper) == eHWResult.On));
            m_bEnableStart = !IsWaferOut(); 
        }

        public override bool IsWaferOut()
        {
            if (m_aligner.InfoWafer != null) return true;
            if (m_visionWorks.InfoWafer != null) return true;
            if (m_backSide.InfoWafer != null) return true;
            if (m_wtr.GetInfoWafer(HW_WTR_Mom.eArm.Upper) != null) return true;
            if (m_wtr.GetInfoWafer(HW_WTR_Mom.eArm.Lower) != null) return true;
            if (m_aligner.CheckWaferExist() == eHWResult.On) return true;
            if (m_visionWorks.IsWaferExist() == eHWResult.On) return true;
            if (m_backSide.IsWaferExist() == eHWResult.On) return true;
            if (m_wtr.CheckWaferExist(HW_WTR_Mom.eArm.Upper) == eHWResult.On) return true;
            if (m_wtr.CheckWaferExist(HW_WTR_Mom.eArm.Lower) == eHWResult.On) return true; 
            return false;
        }

        Info_Wafer FindInfoWafer(Info_Wafer.eLocate eLocate, bool bExist)
        {
            int n, i;
            Info_Carrier infoCarrier;
            Info_Wafer infoWafer = null; 
            for (n = 0; n < m_nLoadPort; n++)
            {
                infoCarrier = ClassCarrier(n);
                for (i = 0; i < infoCarrier.m_lWafer; i++)
                {
                    if (eLocate == infoCarrier.m_infoWafer[i].Locate)
                    {
                            infoWafer = infoCarrier.m_infoWafer[i];
                        if (!bExist && infoWafer != null)
                        {
                            m_bManualRecovery = true;
                            if (m_auto.m_strWork == Auto_Mom.eWork.MSB.ToString() || m_auto.m_strWork == Auto_Mom.eWork.Sanken.ToString())
                            {
                                if (MessageBox.Show("On " + eLocate.ToString() + " The Wafer Is Not Detect !!\n" +
                                    "Please Check " + (infoWafer.m_nLoadPort + 1).ToString() + " Loadport : " + infoWafer.m_nSlot.ToString() + " Slot Wafer !!\n" +
                                    "Yes = Wafer Is Existed On " + eLocate.ToString() + "\n No = Wafer Is Not Existed On " + eLocate.ToString() + " (Should be Unloaded on LoadPort by your hands)",
                                    "Lost Wafer Infomation !!", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Yes)
                                {
                                    infoCarrier.m_infoWafer[i].Locate = eLocate;
                                    infoWafer = infoCarrier.m_infoWafer[i];
                                }
                                else
                                {
                                    infoCarrier.m_infoWafer[i].Locate = Info_Wafer.eLocate.LoadPort;
                                    infoWafer = null;
                                }
                            }
                            else
                            {
                                if (MessageBox.Show(eLocate.ToString() + "에서 웨이퍼가 감지되지 않습니다 !!\n" +
                                    (infoWafer.m_nLoadPort + 1).ToString() + " Loadport의 " + infoWafer.ToString() + "번 Slot Wafer 를 확인해주세요 !!\n" +
                                    "Yes = "+ eLocate.ToString() + "에 웨이퍼가 존재함\nNo = " + eLocate.ToString() + 
                                    "에 웨이퍼가 존재하지 않음 (이 경우 Wafer의 안착 상태를 확인해 주세요)",
                                    "Lost Wafer Infomation !!", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Yes)
                                {
                                    infoCarrier.m_infoWafer[i].Locate = eLocate;
                                    infoWafer = infoCarrier.m_infoWafer[i];
                                }
                                else
                                {
                                    infoCarrier.m_infoWafer[i].Locate = Info_Wafer.eLocate.LoadPort;
                                    infoWafer = null;
                                }

                            }
                        }
                    }
                }
            }
            if (infoWafer != null)
            {
                m_log.Popup(eLocate.ToString() + " <- LoadPort" + (infoWafer.m_nLoadPort + 1).ToString("0") + infoWafer.m_nSlot.ToString(" Slot00")); 
            }
            return infoWafer; 
        }

        public override void UnloadStrip()
        {
        }

        public override void RunEmergency()
        {
        }

        public override bool IsWaferExistInEQP()
        {
            if (m_aligner.CheckWaferExist() == eHWResult.On) return true;
            if (m_visionWorks.IsWaferExist() == eHWResult.On) return true;
            if (m_backSide.IsWaferExist() == eHWResult.On) return true;
            if (m_wtr.CheckWaferExist(HW_WTR_Mom.eArm.Lower) == eHWResult.On) return true;
            if (m_wtr.CheckWaferExist(HW_WTR_Mom.eArm.Upper) == eHWResult.On) return true;
            return false;
        }

        public void ModelGrid(ezGrid rGrid, eGrid eMode)
        {
            int n, nLoadPort = m_nLoadPort;
            rGrid.Set(ref m_nLoadPort, "LoadPort", "Count", "# of LoadPort");
            if (eMode == eGrid.eUpdate) m_log.m_reg.Write("LoadPort", m_nLoadPort);
            for (n = 0; n < m_nLoadPort; n++) rGrid.Set(ref m_strLoadPort[n], m_strLoadPorts, "LoadPort", n.ToString("LoadPort0"), "LoadPort Type");
            for (n = 0; n < m_nLoadPort; n++) rGrid.Set(ref m_strRFID[n], m_strRFIDs, "RFID", n.ToString("RFID0"), "RFID Type"); 
            rGrid.Set(ref m_sAligner, m_sAligners, "Aligner", "Model", "Aligner Type");
            rGrid.Set(ref m_sWTR, m_sWTRs, "WTR", "Model", "WTR Type");
            rGrid.Set(ref m_sBackSide, m_sBacksides, "BackSide", "Model", "BackSide Type");
            rGrid.Set(ref m_bUseIonizer, "Ionizer", "Use", "Use Ionizer"); // ing 170206
            rGrid.Set(ref m_bIonizerLoadPort, "Ionizer", "Loadport", "Ionizer Loadport");
            rGrid.Set(ref m_bIonizerAlign, "Ionizer", "Align", "Ionizer Align");
            rGrid.Set(ref m_bIonizerVisionStage, "Ionizer", "VisionStage", "Ionizer VisionStage");
            rGrid.Set(ref m_bIonizerVisionLoad, "Ionizer", "VisionLoad", "Ionizer VisionLoad");
            rGrid.Set(ref m_bFFU, "Ionizer", "FFU", "Ionizer FFU");
            rGrid.Set(ref m_bKeyLock, "Switch", "KeyLock", "Switch KeyLock");
            rGrid.Set(ref m_bMCReset, "Switch", "MCReset", "Switch MCReset");
            rGrid.Set(ref m_bFDCUse, "FDC", "FDC Use", "FDC Use CHeck");
            rGrid.Set(ref m_bFDCITV_Val, "FDC", "Value Interval", "FDC Value update Interval");
            rGrid.Set(ref m_nFDCModuleNum, "FDC", "FDC Module Num", "Number of FDC Module");
            rGrid.Set(ref m_bFFUUse, "FDC", "FFU Use", "Use FFU"); 
            rGrid.Set(ref m_nFFUModuleNum, "FDC", "FFU Module Num", "Number of FFU Module"); 
            m_imageVS.ModelGrid(rGrid, eMode);
            m_remoteUI.ModelGrid(rGrid, eMode); 
        }

        public void ControlGrid(Control_Mom control, ezGrid rGrid, eGrid eMode)
        {
            control.AddDO(rGrid, ref m_doIonizer, m_id, "Ionizer", "DO Ionizer");
            control.AddDO(rGrid, ref m_doIonizerVision, m_id, "Ionizer_Vision", "DO Ionizer Vision");
            control.AddDI(rGrid, ref m_diIonizerLoadPort, m_id, "Ionizer_Loadport", "DI Loadport Ionizer Alram");
            control.AddDI(rGrid, ref m_diIonizerAlign, m_id, "Ionizer_Align", "DI Aligner Ionizer Alarm");
            control.AddDI(rGrid, ref m_diIonizerVisionStage, m_id, "Ionizer_VisionStage", "DI Vision Stage Ionizer Alarm");
            control.AddDI(rGrid, ref m_diIonizerVisionLoad, m_id, "Ionizer_VisionLoad", "DI Vision Load Ionizer Alarm");
            control.AddDI(rGrid, ref m_diKeyLock, m_id, "KeyLock", "DI Number");
            control.AddDI(rGrid, ref m_diFFU[0], m_id, "FFU0", "FFU0");
            control.AddDI(rGrid, ref m_diFFU[1], m_id, "FFU1", "FFU1");
            control.AddDI(rGrid, ref m_diFFU[2], m_id, "FFU2", "FFU2");
            control.AddDI(rGrid, ref m_diMCReset, m_id, "MCReset", "MCReset");
            
        }

        public override bool CheckUnloadOK()
        {
            if (!CheckUnloadOK(m_aligner.InfoWafer)) return false; 
            if (!CheckUnloadOK(m_visionWorks.InfoWafer)) return false;
            if (!CheckUnloadOK(m_backSide.InfoWafer)) return false;
            if (!CheckUnloadOK(m_wtr.GetInfoWafer(HW_WTR_Mom.eArm.Lower))) return false;
            if (!CheckUnloadOK(m_wtr.GetInfoWafer(HW_WTR_Mom.eArm.Upper))) return false;
            return true; 
        }

        public bool CheckUnloadOK(Info_Wafer infoWafer)
        {
            if (infoWafer == null)
                return true;
            HW_LoadPort_Mom loadPort = ClassLoadPort(infoWafer.m_nLoadPort);
            if (loadPort == null)
            {
                m_log.Popup("LoadPort not Defined !!"); 
                return false;
            }
            if (loadPort.GetState() != HW_LoadPort_Mom.eState.LoadDone)
            {
                m_log.Popup("LoadPort State is not LoadDone !!"); 
                return false;
            }
            if (!loadPort.m_infoCarrier.m_wafer.Equals(infoWafer.m_wafer))
            {
                m_log.Popup("LoadPort WaferSize is not Same !!"); 
                return false;
            }
            return true; 
        }

        public override HW_VisionWorks_Mom ClassVisionWorks()
        {
            return m_visionWorks;
        }

        public override HW_ImageVS_Mom ClassImageVS()
        {
            return m_imageVS;
        }

        public override HW_BackSide_Mom ClassBackSide()
        {
            return m_backSide;
        }

        public override HW_WTR_Mom ClassWTR()
        {
            return m_wtr;
        }

        public override HW_Aligner_Mom ClassAligner()
        {            
            return m_aligner;
        }

        public override HW_PowerMeter ClassPowerMeter()
        {
            return m_powermeter;
        }


        public override HW_LoadPort_Mom ClassLoadPort(int nIndex)
        {
            if ((nIndex < 0) || (nIndex >= m_nLoadPort)) return null;
            return m_loadport[nIndex];
        }

        public override HW_RFID_Mom ClassRFID(int nIndex)
        {
            if ((nIndex < 0) || (nIndex >= m_nLoadPort)) return null;
            return m_RFID[nIndex];
        }

        public override Info_Carrier ClassCarrier(int nIndex)
        {
            if ((nIndex < 0) || (nIndex >= m_nLoadPort)) return null;
            return m_loadport[nIndex].m_infoCarrier; 
        }

        public override HW_FDC_Mom ClassFDC()
        {
            return m_FDC;
        }
        public override HW_FFU_Mom ClassFFU()
        {
            return m_ffu;
        } 
        void RunThread()
        {
            eWorkRun[] eStatRun = new eWorkRun[2];
            ezStopWatch swCheck = new ezStopWatch(); 
            m_bRunThread = true; 
            eStatRun[0] = eWorkRun.Init;
            Thread.Sleep(3000);
            if (m_bUseIonizer) RunIonizer(true); // ing 170206
            Thread.Sleep(2000);
            swCheck.Start();
            SetInfoWafer();
            while (m_bRunThread)
            {
                Thread.Sleep(10); 
                eStatRun[1] = m_work.GetState();
                switch (eStatRun[1])
                {
                    case eWorkRun.Home: RunHome(); break;
                    case eWorkRun.Reset: RunReset(); m_work.m_run.SetReady(); break;
                    case eWorkRun.AutoUnload: RunAutoUnload(); break;
                    case eWorkRun.Ready:
                        SetProcAutoUnload(eProcAutoUnload.GetLPInfo); // ing 170728
                        //m_procAutoUnload = eProcAutoUnload.GetLPInfo;
                        if (!IsWaferOut()) m_bEnableStart = true; //forget
                        break; 
                }
                eStatRun[0] = eStatRun[1];
                if (swCheck.Check() > 2000)
                {
                    CheckDIO();
                    if (m_marslog != null) CompressMarsLog(); //21130 nscho
                    swCheck.Start(); 
                }
                if (m_FDC.GetFDCError() != null)
                {
                    string str = m_FDC.GetFDCError();
                    string[] FDCError = str.Split(':');
                    switch (FDCError[0])
                    {
                        case "CDA ":
                            m_log.ChangeString((int)eError.FDCError_CDA, str);
                            SetAlarm(eAlarm.Error, eError.FDCError_CDA);
                            break;
                        case "Vaccum ":
                        case "Vacuum ":
                        case "VACUUM ":
                            m_log.ChangeString((int)eError.FDCError_Vacuum, str);
                            SetAlarm(eAlarm.Error, eError.FDCError_Vacuum);
                            break;
                        case "EQP_Pressure ":
                        case "Vision_Pressure ":
                        case "VisionPressure ":
                            m_log.ChangeString((int)eError.FDCError_VisionPressure, str);
                            SetAlarm(eAlarm.Error, eError.FDCError_VisionPressure);
                            break;
                        case "EQP_Pressure2 ":
                        case "EFEM_Pressure ":
                        case "EFEM Pressure ":
                            m_log.ChangeString((int)eError.FDCError_EFEMPressure, str);
                            SetAlarm(eAlarm.Error, eError.FDCError_EFEMPressure);
                            break;
                        case "Temp ":
                            m_log.ChangeString((int)eError.FDCError_Temp, str);
                            SetAlarm(eAlarm.Error, eError.FDCError_Temp);
                            break;
                        case "Elec ":
                        case "Elect ":
                        case "Electstatic ":  
                        case "Electrostatic ":
                            m_log.ChangeString((int)eError.FDCError_Elect, str);
                            SetAlarm(eAlarm.Error, eError.FDCError_Elect);
                            break;
                        case "Vaccum1 ": 
                        case "Vacuum1 ": 
                        case "Vaccum-1 ":
                        case "Vacuum-1 ":
                            m_log.ChangeString((int)eError.FDCError_Vacuum1, str);
                            SetAlarm(eAlarm.Error, eError.FDCError_Vacuum1);
                            break;
                        case "Vaccum2 ":  
                        case "Vacuum2 ":  
                        case "Vaccum-2 ":
                        case "Vacuum-2 ":
                            m_log.ChangeString((int)eError.FDCError_Vacuum2, str);
                            SetAlarm(eAlarm.Error, eError.FDCError_Vacuum2);
                            break;
                    } 
                }
            }
        }
        bool CompressMarsLog()
        {
            string strMarsLog = "EventLog";
            string strCompressMarsLog = "";
            DateTime date = DateTime.Now.AddHours(-1);
            SevenZipExtractor.SetLibraryPath("C:\\Program Files\\7-Zip\\7z.dll"); // 7-zip 필수 설치
            SevenZipCompressor comp = new SevenZipCompressor();
            comp.CompressionLevel = CompressionLevel.Normal;
            comp.ArchiveFormat = OutArchiveFormat.Zip;
            comp.CompressionMethod = CompressionMethod.Default;
            comp.FastCompression = true;
            comp.DirectoryStructure = false;
            strMarsLog += date.ToString("yyyMMddHH") + ".txt";
            if (date.ToString("mm") == "01" && !File.Exists(m_strPathMarsLog + strMarsLog + ".zip") && File.Exists(m_strPathMarsLog + strMarsLog))
            {
                strCompressMarsLog = m_strPathMarsLog + "\\" + strMarsLog + ".zip";
                DirectoryInfo di = new DirectoryInfo(m_strPathMarsLog);
                FileInfo[] filesInfo = di.GetFiles(strMarsLog);
                m_log.Add("Compress File Count : " + filesInfo.Length);
                try
                {
                    for (int i = 0; i < filesInfo.Length; i++)
                    {
                        comp.CompressFiles(strCompressMarsLog, filesInfo[i].FullName);
                    }
                }
                catch (Exception ex)
                {
                    m_log.Popup("Compression Fail !!");
                    m_log.Add(ex.Message);
                    return false;
                }
                try
                {
                    if (File.Exists(m_strPathMarsLog + strMarsLog))
                    {
                        File.Delete(m_strPathMarsLog + strMarsLog);
                    }
                } //210602 압축 후 파일삭제
                catch (Exception ex)
                {
                    m_log.Popup("File Delete Fail");
                    m_log.Add(ex.Message);
                    return false;
                } //210602 압축 후 파일삭제

            }
            return true;
        } 

        public override void RunStart() 
        {
            if (IsWaferExistInEQP())
            {
                bool IsDoorOpen = false;
                for (int i = 0; i < m_nLoadPort; i++)
                {
                    if (ClassLoadPort(i).IsDoorOpenPos())
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

        public override void RunStop()
        {
            m_work.m_run.Stop(); 
        }

        public override void RunRecover()
        {
            SetInfoWafer();
            int nPortNum = GetPortForRecovery();
            for (int i = 0; i < m_nLoadPort; i++)
            {
                ClassLoadPort(i).m_infoCarrier.m_bRNR = false;
            }

            EFEM_Recovery recover = new EFEM_Recovery();
            //m_log.HidePopup(true);
            recover.Init("Recover", m_auto, m_log, nPortNum);

            if (recover.GetLPNumForRecovery() != -1 && m_bPossibleAutoRecover && !m_bManualRecovery)
            {
                //if (m_handler.CheckUnloadOK() != true) return;
                SetRecoverPortNum(recover.GetLPNumForRecovery());
                SetProcAutoUnload(Handler_Mom.eProcAutoUnload.GetLPInfo);
                m_work.m_run.SetManual(eWorkRun.AutoUnload);
            }
            else
            {
                if (nPortNum == -1 || !ClassLoadPort(nPortNum).IsDoorOpenPos())
                {
                    if (m_auto.m_strWork != Auto_Mom.eWork.MSB.ToString() && m_auto.m_strWork != Auto_Mom.eWork.Sanken.ToString()) m_log.Popup("설비가 Wafer 정보를 잃었습니다. Loadport(" + nPortNum + ") Load를 먼저 진행하여 주십시오");
                    else m_log.Popup("Wafer Infomation Was Losted On Equipment. Please Load Loadport(" + nPortNum + "), Befor Push Recovery Button");
                    return;
                }
                ezStopWatch sw = new ezStopWatch();
                while (m_loadport[0].NeedWTRMapping() || m_loadport[1].NeedWTRMapping())
                {
                    Thread.Sleep(100);
                    if (sw.Check() > 15000) break;
                } 
                if (recover.ShowDialog() != DialogResult.OK) return;
                if (CheckUnloadOK() != true) return;
                m_work.m_run.SetManual(eWorkRun.AutoUnload); // ing 170823
                SetProcAutoUnload(Handler_Mom.eProcAutoUnload.VisionHome);
                //m_work.m_run.SetManual(eWorkRun.AutoUnload);
            }
            m_bManualRecovery = false;
        }

        private int GetPortForRecovery()
        {
            
            HW_VisionWorks_Mom Vision = ClassVisionWorks();
            HW_Aligner_Mom Aligner = ClassAligner();
            HW_WTR_Mom WTR = ClassWTR();

            if (Vision.InfoWafer != null && Vision.IsWaferExist() == eHWResult.On)
            {
                return Vision.InfoWafer.m_nLoadPort;
            }
            else if (Aligner.InfoWafer != null && Aligner.CheckWaferExist() == eHWResult.On)
            {
                return Aligner.InfoWafer.m_nLoadPort;
            }
            else if (WTR.GetInfoWafer(HW_WTR_Mom.eArm.Lower) != null && WTR.CheckWaferExist(HW_WTR_Mom.eArm.Lower) == eHWResult.On)
            {
                return WTR.GetInfoWafer(HW_WTR_Mom.eArm.Lower).m_nLoadPort;
            }
            else if (WTR.GetInfoWafer(HW_WTR_Mom.eArm.Upper) != null && WTR.CheckWaferExist(HW_WTR_Mom.eArm.Upper) == eHWResult.On)
            {
                return WTR.GetInfoWafer(HW_WTR_Mom.eArm.Upper).m_nLoadPort;
            }
            for (int n = 0; n < m_nLoadPort; n++)
            {
                if (ClassLoadPort(n).IsDoorOpenPos())
                    return n;
            }
            return -1;
        }

        void RunReset()
        {
            int n; 
            for (n = 0; n < m_nLoadPort; n++)
                m_loadport[n].Reset();
            m_xGem.ClearAlarm(); // ing 170807
        }

        void RunHome()
        {
            int n;
            //SetInfoWafer(); // ing 170402
            m_progressHome.Reset();
            m_progressHome.m_bShow = true;
            m_work.RunBuzzer(Work_Mom.eBuzzer.Home, true);
            m_log.WriteLog("Sequence", "[Home Start]"); //230721 nscho
            if (!(m_auto.ClassWork().m_bTestRun))
            {
                ezStopWatch sw = new ezStopWatch();
                m_wtr.SetHome();
                m_progressHome.AddWTR();
                while (m_wtr.GetState() == HW_WTR_Mom.eState.Home && sw.Check() < m_wtr.m_msHome)
                    Thread.Sleep(1);
                if (m_wtr.GetState() != HW_WTR_Mom.eState.Ready)
                {
                    SetAlarm(eAlarm.Warning, eError.HomeWTR);
                    m_work.RunBuzzer(Work_Mom.eBuzzer.Home, false);
                    return;
                }
                m_aligner.SetHome();
                m_progressHome.AddAligner();
                if (m_visionWorks.IsVisionEnable())
                {
                    m_visionWorks.SetHome();
                    m_progressHome.AddVision();
                }

                if (m_backSide.m_bEnable)   //KDG Add BackSide Enable // Changed by ing (IsBacksideEnalbe() -> m_bEable)
                    m_backSide.SetHome(); 

                for (n = 0; n < m_nLoadPort; n++)
                {
                    m_bDontStartLoading[n] = true; // ing 180220
                    m_loadport[n].SetHome();
                    m_progressHome.AddLoadPort(n);
                }

                sw.Start();
                while ((m_aligner.GetState() == HW_Aligner_Mom.eState.Home || m_aligner.GetState() == HW_Aligner_Mom.eState.RunReady) && sw.Check() < m_aligner.m_msHome) Thread.Sleep(1);
                if (m_aligner.GetState() != HW_Aligner_Mom.eState.Ready && m_aligner.GetState() != HW_Aligner_Mom.eState.Done) // ing 170402
                {
                    SetAlarm(eAlarm.Warning, eError.HomeAligner);
                    m_work.RunBuzzer(Work_Mom.eBuzzer.Home, false);
                    return;
                }
                if (m_backSide.m_bEnable)   //KDG Add BackSide Enable // Changed by ing (IsBacksideEnalbe() -> m_bEable)
                {
                    while (m_backSide.GetState() == HW_BackSide_Mom.eState.Home && sw.Check() < m_backSide.m_msHome) 
                        Thread.Sleep(1);
                }
                if (m_visionWorks.IsVisionEnable())
                {
                    if (!m_visionWorks.IsConnected())
                    {
                        SetAlarm(eAlarm.Warning, eError.HomeVision);
                        return;
                    }
                    while (m_visionWorks.GetState() == HW_VisionWorks_Mom.eState.Home && sw.Check() < m_visionWorks.m_msHome)
                        Thread.Sleep(1);
                    if (m_visionWorks.GetState() != HW_VisionWorks_Mom.eState.LoadWait)
                    {
                        SetAlarm(eAlarm.Warning, eError.HomeVision);
                        m_work.RunBuzzer(Work_Mom.eBuzzer.Home, false);
                        return;
                    }
                }
                for (n = 0; n < m_nLoadPort; n++)
                {
                    while (m_loadport[n].GetState() == HW_LoadPort_Mom.eState.Home && sw.Check() < m_loadport[n].m_msHome)
                        Thread.Sleep(1);
                    if (m_loadport[n].GetState() != HW_LoadPort_Mom.eState.Ready)
                    {
                        SetAlarm(eAlarm.Warning, eError.HomeLoadport);
                        m_work.RunBuzzer(Work_Mom.eBuzzer.Home, false);
                        return;
                    }
                }
            }
            SetInfoWafer(); 
            m_work.m_run.SetReady();
            m_work.RunBuzzer(Work_Mom.eBuzzer.Home, false);

            for (int i = 0; i < m_nLoadPort; i++)
            {
                if (ClassLoadPort(i).IsPlaced()) m_bDontStartLoading[i] = true;
                else m_bDontStartLoading[i] = false;
            }

            if (m_auto.m_strWork == Auto_Mom.eWork.SSEM.ToString()) 
            {
                if (m_work.GetSSEMGEM() != null) ((Gem_SSEM)(m_work.GetSSEMGEM())).SetMainCycle(Gem_SSEM.eMainCycle.IDLE, 0); // ing 161121
            }

            if (m_auto.m_bXgemUse && m_xGem.IsOnline()) {
                m_xGem.ResetXGem();
                m_xGem.SetProcessState(XGem300_Mom.eProcessState.STOP);

                Thread.Sleep(2000);
                m_xGem.XGemOffline();
                m_xGem.XGemOnlineRemote();
            }
            m_xGem.ClearAlarm(); // ing 170807
            m_progressHome.m_bShow = false;
            m_progressHome.Reset();
            m_log.Popup("Home Finished !!");
            m_log.WriteLog("Sequence", "[Home Done]"); //230721 nscho 
        }

        void RunAutoUnload()
        {
            if (m_auto.m_strWork == EFEM.eWork.Sanken.ToString()) // ing 171106
            {
                switch (m_procAutoUnload)
                {
                    case eProcAutoUnload.GetLPInfo:
                        if (m_nLPNumForRecovery != -1)
                        {
                            SetProcAutoUnload(Handler_Mom.eProcAutoUnload.LPLoadWait);
                            m_loadport[m_nLPNumForRecovery].SetState(HW_LoadPort_Mom.eState.RunLoad);
                        }
                        break;
                    case eProcAutoUnload.LPLoadWait:
                        if (m_loadport[m_nLPNumForRecovery].GetState() == HW_LoadPort_Mom.eState.LoadDone)
                            SetProcAutoUnload(Handler_Mom.eProcAutoUnload.CheckLPSlot);
                        break;
                    case eProcAutoUnload.CheckLPSlot:
                        if (m_aligner.InfoWafer != null)
                        {
                            if (m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_infoWafer[(25 - (m_aligner.InfoWafer.m_nSlot % m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_lWafer)) % m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_lWafer].State == Info_Wafer.eState.Exist)
                            {
                                m_bPossibleAutoRecover = false;
                                return;
                            }
                            else
                            {
                                m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_infoWafer[(25 - (m_aligner.InfoWafer.m_nSlot % m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_lWafer)) % m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_lWafer].State = Info_Wafer.eState.Run;
                            }
                        }
                        if (m_visionWorks.InfoWafer != null)
                        {
                            if (m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_infoWafer[(25 - (m_visionWorks.InfoWafer.m_nSlot % m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_lWafer)) % m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_lWafer].State == Info_Wafer.eState.Exist)
                            {
                                m_bPossibleAutoRecover = false;
                                return;
                            }
                            else
                            {
                                m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_infoWafer[(25 - (m_visionWorks.InfoWafer.m_nSlot % m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_lWafer)) % m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_lWafer].State = Info_Wafer.eState.Run;
                            }
                        }
                        if (m_wtr.GetInfoWafer(HW_WTR_Mom.eArm.Lower) != null)
                        {
                            if (m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_infoWafer[(25 - (m_wtr.GetInfoWafer(HW_WTR_Mom.eArm.Lower).m_nSlot % m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_lWafer)) % m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_lWafer].State == Info_Wafer.eState.Exist)
                            {
                                m_bPossibleAutoRecover = false;
                                return;
                            }
                            else
                            {
                                m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_infoWafer[(25 - (m_wtr.GetInfoWafer(HW_WTR_Mom.eArm.Lower).m_nSlot % m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_lWafer)) % m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_lWafer].State = Info_Wafer.eState.Run;
                            }
                        }
                        if (m_wtr.GetInfoWafer(HW_WTR_Mom.eArm.Upper) != null)
                        {
                            if (m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_infoWafer[(25 - (m_wtr.GetInfoWafer(HW_WTR_Mom.eArm.Upper).m_nSlot % m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_lWafer)) % m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_lWafer].State == Info_Wafer.eState.Exist)
                            {
                                m_bPossibleAutoRecover = false;
                                return;
                            }
                            else
                            {
                                m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_infoWafer[(25 - (m_wtr.GetInfoWafer(HW_WTR_Mom.eArm.Upper).m_nSlot % m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_lWafer)) % m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_lWafer].State = Info_Wafer.eState.Run;
                            }
                        }
                        SetProcAutoUnload(Handler_Mom.eProcAutoUnload.VisionHome);
                        break;



                    case eProcAutoUnload.VisionHome:
                        if (m_visionWorks.IsVisionEnable()) // ing 170328
                        {
                            m_visionWorks.SetHome();
                            while (m_visionWorks.GetState() == HW_VisionWorks_Mom.eState.Home)
                                Thread.Sleep(1);
                            if (m_visionWorks.GetState() != HW_VisionWorks_Mom.eState.LoadWait)
                            {
                                SetAlarm(eAlarm.Warning, eError.Home);
                                return;
                            }
                            //m_procAutoUnload = eProcAutoUnload.VisionUnload;
                        }
                        SetProcAutoUnload(Handler_Mom.eProcAutoUnload.AlignerReady);
                        break;

                    case eProcAutoUnload.AlignerReady:      //KDG 161025 Add
                        if (m_aligner.IsWaferExist() == eHWResult.On)
                        {
                            if (m_aligner.UnloadLift() == eHWResult.Error)
                            {
                                SetAlarm(eAlarm.Warning, eError.AlignerError);
                                return;
                            }
                            SetProcAutoUnload(Handler_Mom.eProcAutoUnload.VisionUnload);
                        }
                        else
                            SetProcAutoUnload(Handler_Mom.eProcAutoUnload.VisionUnload);
                        break;
                    case eProcAutoUnload.VisionUnload:
                        if (m_visionWorks.IsWaferExist() == eHWResult.On || m_visionWorks.m_bManualWaferExist)    //KDG 160929 Modify
                        {
                            m_visionWorks.RunUnload();
                            SetProcAutoUnload(Handler_Mom.eProcAutoUnload.VisionWaitDone);
                        }
                        else
                            SetProcAutoUnload(Handler_Mom.eProcAutoUnload.StartWTR);
                        break;
                    case eProcAutoUnload.VisionWaitDone:
                        if (m_visionWorks.GetState() == HW_VisionWorks_Mom.eState.LoadWait || m_visionWorks.GetState() == HW_VisionWorks_Mom.eState.Done)
                        {
                            SetProcAutoUnload(Handler_Mom.eProcAutoUnload.StartWTR);
                        }
                        break;
                    case eProcAutoUnload.StartWTR:
                        m_wtr.StartAutoUnload();
                        SetProcAutoUnload(Handler_Mom.eProcAutoUnload.Idle);
                        break;
                    default: break;
                }
            }
            else
            {
                switch (m_procAutoUnload)
                {
                    case eProcAutoUnload.GetLPInfo:
                        if (m_nLPNumForRecovery != -1)
                        {
                            SetProcAutoUnload(Handler_Mom.eProcAutoUnload.LPLoadWait);
                            m_loadport[m_nLPNumForRecovery].SetState(HW_LoadPort_Mom.eState.RunLoad);
                        }
                        break;
                    case eProcAutoUnload.LPLoadWait:
                        if (m_loadport[m_nLPNumForRecovery].GetState() == HW_LoadPort_Mom.eState.LoadDone)
                            SetProcAutoUnload(Handler_Mom.eProcAutoUnload.CheckLPSlot);
                        break;
                    case eProcAutoUnload.CheckLPSlot:
                        if (m_aligner.InfoWafer != null)
                        {
                            if (m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_infoWafer[(m_aligner.InfoWafer.m_nSlot - 1) % m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_lWafer].State == Info_Wafer.eState.Exist)
                            {
                                m_bPossibleAutoRecover = false;
                                return;
                            }
                            else
                            {
                                m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_infoWafer[(m_aligner.InfoWafer.m_nSlot - 1) % m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_lWafer].State = Info_Wafer.eState.Run;
                            }
                        }
                        if (m_visionWorks.InfoWafer != null)
                        {
                            if (m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_infoWafer[(m_visionWorks.InfoWafer.m_nSlot - 1) % m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_lWafer].State == Info_Wafer.eState.Exist)
                            {
                                m_bPossibleAutoRecover = false;
                                return;
                            }
                            else
                            {
                                m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_infoWafer[(m_visionWorks.InfoWafer.m_nSlot - 1) % m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_lWafer].State = Info_Wafer.eState.Run;
                            }
                        }
                        if (m_wtr.GetInfoWafer(HW_WTR_Mom.eArm.Lower) != null)
                        {
                            if (m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_infoWafer[(m_wtr.GetInfoWafer(HW_WTR_Mom.eArm.Lower).m_nSlot - 1) % m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_lWafer].State == Info_Wafer.eState.Exist)
                            {
                                m_bPossibleAutoRecover = false;
                                return;
                            }
                            else
                            {
                                m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_infoWafer[(m_wtr.GetInfoWafer(HW_WTR_Mom.eArm.Lower).m_nSlot - 1) % m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_lWafer].State = Info_Wafer.eState.Run;
                            }
                        }
                        if (m_wtr.GetInfoWafer(HW_WTR_Mom.eArm.Upper) != null)
                        {
                            if (m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_infoWafer[(m_wtr.GetInfoWafer(HW_WTR_Mom.eArm.Upper).m_nSlot - 1) % m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_lWafer].State == Info_Wafer.eState.Exist)
                            {
                                m_bPossibleAutoRecover = false;
                                return;
                            }
                            else
                            {
                                m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_infoWafer[(m_wtr.GetInfoWafer(HW_WTR_Mom.eArm.Upper).m_nSlot - 1) % m_loadport[m_nLPNumForRecovery].m_infoCarrier.m_lWafer].State = Info_Wafer.eState.Run;
                            }
                        }
                        SetProcAutoUnload(Handler_Mom.eProcAutoUnload.VisionHome);
                        break;



                    case eProcAutoUnload.VisionHome:
                        if (m_visionWorks.IsVisionEnable()) // ing 170328
                        {
                            m_visionWorks.SetHome();
                            while (m_visionWorks.GetState() == HW_VisionWorks_Mom.eState.Home)
                                Thread.Sleep(1);
                            if (m_visionWorks.GetState() != HW_VisionWorks_Mom.eState.LoadWait)
                            {
                                SetAlarm(eAlarm.Warning, eError.Home);
                                return;
                            }
                            //m_procAutoUnload = eProcAutoUnload.VisionUnload;
                        }
                        SetProcAutoUnload(Handler_Mom.eProcAutoUnload.AlignerReady);
                        break;

                    case eProcAutoUnload.AlignerReady:      //KDG 161025 Add
                        if (m_aligner.IsWaferExist() == eHWResult.On)
                        {
                            if (m_aligner.UnloadLift() == eHWResult.Error)
                            {
                                SetAlarm(eAlarm.Warning, eError.AlignerError);
                                return;
                            }
                            SetProcAutoUnload(Handler_Mom.eProcAutoUnload.VisionUnload);
                        }
                        else
                            SetProcAutoUnload(Handler_Mom.eProcAutoUnload.VisionUnload);
                        break;
                    case eProcAutoUnload.VisionUnload:
                        if (m_visionWorks.IsWaferExist() == eHWResult.On || m_visionWorks.m_bManualWaferExist)    //KDG 160929 Modify
                        {
                            m_visionWorks.RunUnload();
                            SetProcAutoUnload(Handler_Mom.eProcAutoUnload.VisionWaitDone);
                        }
                        else
                            SetProcAutoUnload(Handler_Mom.eProcAutoUnload.StartWTR);
                        break;
                    case eProcAutoUnload.VisionWaitDone:
                        if (m_visionWorks.GetState() == HW_VisionWorks_Mom.eState.LoadWait || m_visionWorks.GetState() == HW_VisionWorks_Mom.eState.Done)
                        {
                            SetProcAutoUnload(Handler_Mom.eProcAutoUnload.StartWTR);
                        }
                        break;
                    case eProcAutoUnload.StartWTR:
                        m_wtr.StartAutoUnload();
                        SetProcAutoUnload(Handler_Mom.eProcAutoUnload.Idle);
                        break;
                    default: break;
                }
            }
        }
        public override bool IsWaferInfoExistInEQP()
        {
            string msgbox = "자재는 없지만, 정보는 있습니다. 정보를 삭제하시겠습니까?";
            if (m_aligner.CheckWaferExist() == eHWResult.On && m_aligner.InfoWafer != null)
            {
                if (MessageBox.Show(msgbox, "ALIGNER", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Yes)
                {
                    m_aligner.InfoWafer = null;
                }
            }
            if (m_visionWorks.IsWaferExist() == eHWResult.On && m_visionWorks.InfoWafer != null)
            {
                if (MessageBox.Show(msgbox, "VISION", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Yes)
                {
                    m_visionWorks.InfoWafer = null;
                }
            }
            if (m_backSide.IsWaferExist() == eHWResult.On && m_backSide.InfoWafer != null)
            {
                if (MessageBox.Show(msgbox, "BACKSIDE", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Yes)
                {
                    m_backSide.InfoWafer = null;
                }
            }
            if (m_wtr.CheckWaferExist(HW_WTR_Mom.eArm.Lower) == eHWResult.On && m_wtr.GetInfoWafer(HW_WTR_Mom.eArm.Lower) != null)
            {
                if (MessageBox.Show(msgbox, "WTR_LOWER", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Yes)
                {
                    m_wtr.SetInfoWafer(HW_WTR_Mom.eArm.Lower, null);
                }
            }
            if (m_wtr.CheckWaferExist(HW_WTR_Mom.eArm.Upper) == eHWResult.On && m_wtr.GetInfoWafer(HW_WTR_Mom.eArm.Upper) != null)
            {
                if (MessageBox.Show(msgbox, "WTR_UPPER", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Yes)
                {
                    m_wtr.SetInfoWafer(HW_WTR_Mom.eArm.Upper, null);
                }
            }
            return false;
        }  
        void AllLoadPortOutofService()          //KJWAuto  추가됨
        {
            if (m_auto.m_bXgemUse && m_auto.ClassXGem().IsOnline())
            {
                for (int i = 0; i < m_nLoadPort; i++)
                {
                    ChangeLPInOutService(i, 0);
                }
            }
            else return;
        }

        void ChangeLPInOutService(int nLP, int nType)   //KJWAuto  추가됨
        {
            if (ClassLoadPort(nLP).GetState() == HW_LoadPort_Mom.eState.RunLoad) return;

            string sLocID = "LP" + (nLP + 1).ToString();
            m_auto.ClassXGem().SendChangeServiceStatus(sLocID, nType);
        }

        void ChangeAccess(int nMode, int nLP)   //KJWAuto  추가됨
        {
            string sLocID = "LP" + (nLP + 1).ToString();
            m_auto.ClassXGem().CMSReqChangeAccess(nMode, sLocID);
        }

        void SetProcAutoUnload(eProcAutoUnload proc)
        {
            string strProc;
            if (m_procAutoUnload == proc) return;
            strProc = "AutoUnload Process : " + m_procAutoUnload.ToString() + " -> ";
            m_procAutoUnload = proc;
            strProc += m_procAutoUnload.ToString();
            m_log.Add(strProc);
        }
    }
}

      