using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ezAutoMom;
using ezTools; 

namespace ezAuto_EFEM
{
    class EFEM_Work : Work_Mom, Control_Child
    {
        Control_Mom m_control;
        Handler_Mom m_handler;
        bool m_bEMG = false; 
        int m_diEMG = -1;
        bool m_bAirCheck = false;
        int m_diCDA = -1;
        int m_diVacLow = -1;
        int m_doTowerRed = -1;
        int m_doTowerYellow = -1;
        int m_doTowerGreen = -1;
        int m_doBuzzer0 = -1;
        int m_diLightCurtain = -1;
        bool m_bLightCurtain = false;
        int m_diDoorLock = -1;
        int[] m_diDoor = new int[5] { -1, -1, -1, -1, -1 };
        int[] m_diDoorVision = new int[5] { -1, -1, -1, -1, -1 };
        int[] m_diDoorFan = new int[6] { -1, -1, -1, -1, -1, -1 };
        int m_doLight = -1;
        int m_doDoorLock = -1;
        bool m_bDoorOpenUse = false;
        ezStopWatch m_swDoor = new ezStopWatch();
        string m_strDoorName = "";
        eWorkRun m_LastWorkRunState = eWorkRun.Error;
        
        string m_strManualJob = "Sanken";
        string[] m_strManualJobs = new string[2] { "Sanken", "Hynix" };

        string m_strUI = "Default";
        string[] m_strUIs = new string[1] { "Default" }; 

        public EFEM_Work()
        {
        }

        public void Init(string id, Auto_Mom auto)
        {
            base.Init(id, auto);
            m_bJobOpen = true; 
            m_control = auto.ClassControl();
            m_control.Add(this);
            m_handler = auto.ClassHandler();
            m_swDoor.Start();
            RunGrid(eGrid.eInit);
        }

        public void ControlGrid(Control_Mom control, ezGrid rGrid, eGrid eMode)
        {
            control.AddDO(rGrid, ref m_doTowerRed, m_id, "Tower_Red", "DO Number");
            control.AddDO(rGrid, ref m_doTowerYellow, m_id, "Tower_Yellow", "DO Number");
            control.AddDO(rGrid, ref m_doTowerGreen, m_id, "Tower_Green", "DO Number");
            control.AddDO(rGrid, ref m_doBuzzer0, m_id, "Buzzer0", "DO Number");
            if (m_doBuzzer0 >= 0)
            {
                m_control.SetDOCaption(m_doBuzzer0 + 1, "Buzzer1");
                m_control.SetDOCaption(m_doBuzzer0 + 2, "Buzzer2");
                m_control.SetDOCaption(m_doBuzzer0 + 3, "Buzzer3");
            }
            control.AddDO(rGrid, ref m_doLight, m_id, "Light", "DO Number");
            control.AddDO(rGrid, ref m_doDoorLock, m_id, "DoorLock", "DO Number");
            control.AddDI(rGrid, ref m_diEMG, m_id, "EMG", "DI Number");
            control.AddDI(rGrid, ref m_diCDA, m_id, "Vac_CDA", "CDA Low Limit");
            control.AddDI(rGrid, ref m_diVacLow, m_id, "Vac_Low", "Vacuum Low Limit");
            control.AddDI(rGrid, ref m_diDoor[0], m_id, "Door_Air", "Air Door Sensor");
            control.AddDI(rGrid, ref m_diDoor[1], m_id, "Door_PC", "PC Door Sensor");
            control.AddDI(rGrid, ref m_diDoor[2], m_id, "Door_Top", "Top Door Sensor");
            control.AddDI(rGrid, ref m_diDoor[3], m_id, "Door_Bot", "Bot Door Sensor");
            control.AddDI(rGrid, ref m_diDoor[4], m_id, "Door_WTR", "WTR Door Sensor");
            control.AddDI(rGrid, ref m_diDoorLock, m_id, "DoorLock", "DoorLock");
            control.AddDI(rGrid, ref m_diDoorVision[0], m_id, "VSDoor_UPS", "UPS Door Sensor");
            control.AddDI(rGrid, ref m_diDoorVision[1], m_id, "VSDoor_PC", "VisonPC Door Sensor");
            control.AddDI(rGrid, ref m_diDoorVision[2], m_id, "VSDoor_Top", "VIson Top Door Sensor");
            control.AddDI(rGrid, ref m_diDoorVision[3], m_id, "VSDoor_Bot", "Vision Bottom Door Sensor");
            control.AddDI(rGrid, ref m_diLightCurtain, m_id, "LightCurtain", "LightCurtain");
            control.AddDI(rGrid, ref m_diDoorFan[0], m_id, "EFEM PC Door Fan", "PC_Door_Fan");
            control.AddDI(rGrid, ref m_diDoorFan[1], m_id, "EFEM TOP Door Fan", "Top_Door_Fan");
            control.AddDI(rGrid, ref m_diDoorFan[2], m_id, "EFEM WTR Door Fan", "WTR_Door_Fan");
            control.AddDI(rGrid, ref m_diDoorFan[3], m_id, "VS PC Door Fan", "VS PC Door Fan");
            control.AddDI(rGrid, ref m_diDoorFan[4], m_id, "VS Top Door Fan", "VS Top Door Fan");
            control.AddDI(rGrid, ref m_diDoorFan[5], m_id, "VS Bot Door Fan", "VS Bot Door Fan");
        }

        public override void RunGrid(eGrid eMode, ezTools.ezJob job = null)
        {
            base.RunGrid(eMode, job);
            m_grid.Refresh();
        }
        

        public void ModelGrid(ezGrid rGrid, eGrid eMode)
        {
            rGrid.Set(ref m_bEMG, "Work", "bEMG", "Emergency Sensor State");
            rGrid.Set(ref m_bAirCheck, "Work", "bAirCheck", "AirCheck Sensor State");
            rGrid.Set(ref m_bMapDataUse,"MapData", "Use", "Input Map Data File Use");
            rGrid.Set(ref m_strFilePath, "MapData", "FilePath", "Input Map Data File Path");
            rGrid.Set(ref m_bDoorOpenUse, "Door", "Door Open Check", "Door Open Check");
            rGrid.Set(ref m_bTestRun, "Work", "bTest", "AirCheck Sensor State");
            rGrid.Set(ref m_strManualJob, m_strManualJobs, "Work", "ManualJob", "Type of ManualJob");
            rGrid.Set(ref m_strUI, m_strUIs, "Work", "UI", "Type of UI"); 
            rGrid.Set(ref m_bXgemUSe, "Gem", "XGEMUse", "XgemUSE");
            rGrid.Set(ref m_bLightCurtain, "Work", "bLightCurtain", "LightCurtain Sensor State");
            if (eMode == eGrid.eRegRead) m_bTestRun = false; 
            m_bInvalid = true;
        }

        public UI_ManualJob_Mom GetManualJob()
        {
            if (m_strManualJob == m_strManualJobs[0]) return new UI_ManualJob_Sanken();
            if (m_strManualJob == m_strManualJobs[1]) return new UI_ManualJob_Hynix(); 
            return null; 
        }

        public UI_EFEM_Mom GetUI()
        {
            if (m_strUI == m_strUIs[0]) return new UI_EFEM();
            return null; 
        }

        public override void JobOpen(ezJob job)
        {
        }

        public override void JobSave(ezJob job)
        {
        }

        public override void Clear()
        {
            base.Clear();
        }

        public override bool IsStartEnable(bool bPopup)
        {
            return true;
        }

        public override bool IsEMGError()
        {
            return m_bEMG == m_control.GetInputBit(m_diEMG); 
        }

        public override bool IsVacError()
        {
            if (m_bAirCheck == m_control.GetInputBit(m_diVacLow)) return true;
            return false;
        }

        public override bool IsCDAError()
        {
            if (m_bAirCheck == m_control.GetInputBit(m_diCDA)) return true;
            return false;
        }

        public bool IsLightCurtainError()                   // 160817 ADD SDH 
        {
            if (m_bLightCurtain == m_control.GetInputBit(m_diLightCurtain)) return true;
            return false;
        }

        public override bool IsDoorOpen()           ///kjw 수정 필요
        {
            if (!m_bDoorOpenUse)
                return false;
            for (int n = 0; n < 5; n++)
            {
                if (m_diDoor[n] < 0) return true;
                if (!m_control.GetInputBit(m_diDoor[n]))
                {
                    switch (n)
                    {
                        case 0:
                            m_strDoorName = "Main Air Door";
                            break;
                        case 1:
                            m_strDoorName = "EFEM PC Door";
                            break;
                        case 2:
                            m_strDoorName = "EFEM Top Door";
                            break;
                        case 3:
                            m_strDoorName = "EFEM Bottom Door";
                            break;
                        case 4:
                            m_strDoorName = "WTR Controller Door";
                            break;
                    }
                    return true;
                }
                if (!m_control.GetInputBit(m_diDoorVision[n]))
                {
                    switch (n)
                    {
                        case 0:
                            m_strDoorName = "Vision Module UPS Door";
                            break;
                        case 1:
                            m_strDoorName = "Vision Module PC Door";
                            break;
                        case 2:
                            m_strDoorName = "Vision Module Top Door";
                            break;
                        case 3:
                            m_strDoorName = "Vision Module Bottom Door";
                            break;
                        case 4:
                            m_strDoorName = "Unknown";
                            break;
                    }
                    return true;
                }
            }
            return false;
        }

        public override void RunLamp()
        {
            long msLamp = m_swLamp.Check() % 1000;
            bool bGreen = false, bYellow = false, bRed = false;
            bool bBuzzerWarning = false, bBuzzerError = false;
            bool bBuzzerRun = false;
            bool bStartEnable = IsStartEnable(false);
            if (m_run.GetState() != m_LastWorkRunState)
            {
                m_bBuzzerOff = false;
                m_LastWorkRunState = m_run.GetState();
            }
            switch (m_run.GetState())
            {
                case eWorkRun.Init:
                    break;
                case eWorkRun.Ready:
                    break;
                case eWorkRun.Run:
                    if (m_handler.ClassLoadPort(0).GetState() == HW_LoadPort_Mom.eState.Ready && m_handler.ClassLoadPort(1).GetState() == HW_LoadPort_Mom.eState.Ready)
                    { bYellow = true; bBuzzerRun = false; }
                    else
                    { bGreen = true; bBuzzerRun = false; }
                    //if (IsLoadPortNotReady()) { bGreen = true; }
                    //else { bYellow = true; bBuzzerRun = true; }  
                    break;
                case eWorkRun.Warning0:
                    bBuzzerWarning = true;
                    break;
                case eWorkRun.Warning1:
                    bRed = true;
                    bBuzzerWarning = true;
                    break;
                case eWorkRun.Error:
                    bRed = true;
                    bBuzzerError = true;
                    break;
                case eWorkRun.AutoLoad:
                case eWorkRun.AutoUnload:
                    break;
                case eWorkRun.PickerSet:
                    break;
            }
            m_control.WriteOutputBit(m_doTowerGreen, bGreen);
            m_control.WriteOutputBit(m_doTowerYellow, bYellow && (msLamp > 500));
            m_control.WriteOutputBit(m_doTowerRed, bRed && ((msLamp % 500) > 250));
            if (!m_bBuzzerOff && bBuzzerWarning) RunBuzzer(eBuzzer.Warning, true);
            else RunBuzzer(eBuzzer.Warning, false);
            if (!m_bBuzzerOff && bBuzzerError) RunBuzzer(eBuzzer.Error, true);
            else RunBuzzer(eBuzzer.Error, false);
            if (!m_bBuzzerOff && bBuzzerRun) RunBuzzer(eBuzzer.Buzzer, true);
            else RunBuzzer(eBuzzer.Buzzer, false);
            if (m_bWorkerBuzzerOn)
                RunBuzzer(eBuzzer.Warning, true);
            else
                RunBuzzer(eBuzzer.Warning, false);
            if (m_run.GetState() == eWorkRun.Run && IsDoorOpen() && m_swDoor.Check() > 10000)
            {
                m_log.Popup("Door Open Detected (" + m_strDoorName + ")");
                m_swDoor.Start();
            }
            m_control.WriteOutputBit(m_doLight, IsManual() || IsDoorOpen());
            m_control.WriteOutputBit(m_doDoorLock, m_run.GetState() == eWorkRun.Run);
            if (m_run.GetState() == eWorkRun.Run && IsLightCurtainError() && (m_handler.ClassLoadPort(0).GetState() != HW_LoadPort_Mom.eState.Ready || m_handler.ClassLoadPort(1).GetState() != HW_LoadPort_Mom.eState.Ready))
            {
                m_control.WriteOutputBit(m_doBuzzer0+1,true);
                m_log.Popup("Light Curtain Detected");
            }
            else { 
                m_control.WriteOutputBit(m_doBuzzer0 + 1, false);
            }
                
        }

        bool IsLoadPortNotReady()
        {
            HW_LoadPort_Mom loadPort = null; 
            for (int n=0; n<3; n++)
            {
                loadPort = m_handler.ClassLoadPort(n); 
                if ((loadPort != null) && (loadPort.GetState() != HW_LoadPort_Mom.eState.Ready)) return true;
            }
            return false; 
        }

        public override void RunBuzzer(eBuzzer eID, bool bOn)
        {
            if (m_doBuzzer0 < 0) return; 
            m_control.WriteOutputBit(m_doBuzzer0 + (int)eID, bOn);
        }

        public override bool IsBuzzerOn()
        {
            if (m_doBuzzer0 < 0) return false;
            for (int n = 0; n < 4; n++)
            {
                if (m_control.GetOutputBit(m_doBuzzer0 + n)) return true;
            }
            return false;
        }

    }
}
