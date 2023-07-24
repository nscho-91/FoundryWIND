using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ezAutoMom;
using ezTools; 

namespace ezAuto_EFEM
{
    class EFEM_Work_Hynix_RGBY : Work_Mom, Control_Child
    {
        Handler_Mom m_handler;

        bool m_bEMG = false; 
        int m_diEMG = -1;

        bool m_bAirCheck = false;
        int m_diCDA = -1;
        int m_diVacLow = -1;

        int m_doTowerRed = -1;
        int m_doTowerYellow = -1;
        int m_doTowerGreen = -1;
        int m_doTowerBlue = -1;

        int m_doBuzzer0 = -1;
        int m_diLightCurtain = -1;
        bool m_bLightCurtain = false;

        int m_diDoorLock = -1;
        int[] m_diDoor = new int[5] { -1, -1, -1, -1, -1 };
        int[] m_diDoorVision = new int[5] { -1, -1, -1, -1, -1 };
        int[] m_diDoorFan = new int[7] { -1, -1, -1, -1, -1, -1, -1 };
        int m_doDoorLock = -1;
        bool m_bDoorOpenUse = false;
        bool m_bDoorFanAlarmUse = false;
        ezStopWatch m_swDoor = new ezStopWatch();
        ezStopWatch m_swDoorFan = new ezStopWatch();
        string m_strDoorName = "";
        string m_strDoorFanName = "";

        int m_doLight = -1;
        eWorkRun m_LastWorkRunState = eWorkRun.Error;

        XGem300_Mom m_xGem = null; 

        public EFEM_Work_Hynix_RGBY()
        {
        }

        public override void Init(string id, object auto)
        {
            base.Init(id, auto);
            m_bJobOpen = true;
            m_control = ((Auto_Mom)m_auto).ClassControl();
            m_control.Add(this);
            m_handler = ((Auto_Mom)m_auto).ClassHandler();
            m_xGem = ((Auto_Mom)m_auto).ClassXGem();
            m_swDoor.Start();
            m_swDoorFan.Start();
            RunGrid(eGrid.eInit);
        }

        public void ControlGrid(Control_Mom control, ezGrid rGrid, eGrid eMode)
        {
            control.AddDO(rGrid, ref m_doTowerRed, m_id, "Tower_Red", "DO Number");
            control.AddDO(rGrid, ref m_doTowerYellow, m_id, "Tower_Yellow", "DO Number");
            control.AddDO(rGrid, ref m_doTowerGreen, m_id, "Tower_Green", "DO Number");
            control.AddDO(rGrid, ref m_doTowerBlue, m_id, "Tower_Blue", "DO Number");
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
            control.AddDI(rGrid, ref m_diDoorVision[0], m_id, "VSDoor_UPS", "Vision UPS Door Sensor");
            control.AddDI(rGrid, ref m_diDoorVision[1], m_id, "VSDoor_PC", "VIsion PC Door Sensor");
            control.AddDI(rGrid, ref m_diDoorVision[2], m_id, "VSDoor_Top", "VIsion Top Door Sensor");
            control.AddDI(rGrid, ref m_diDoorVision[3], m_id, "VSDoor_Bot", "VIsion Bottom Door Sensor");
            control.AddDI(rGrid, ref m_diLightCurtain, m_id, "LightCurtain", "LightCurtain");
            control.AddDI(rGrid, ref m_diDoorFan[6], m_id, "EFEM Air Door Fan", "Air_Door_Fan");
            control.AddDI(rGrid, ref m_diDoorFan[0], m_id, "EFEM PC Door Fan", "PC_Door_Fan");
            control.AddDI(rGrid, ref m_diDoorFan[1], m_id, "EFEM TOP Door Fan", "Top_Door_Fan");
            control.AddDI(rGrid, ref m_diDoorFan[2], m_id, "EFEM WTR Door Fan", "WTR_Door_Fan");
            control.AddDI(rGrid, ref m_diDoorFan[3], m_id, "VS PC Door Fan", "Vision_PC_Door_Fan");
            control.AddDI(rGrid, ref m_diDoorFan[4], m_id, "VS Top Door Fan", "Vision_Top_Door_Fan");
            control.AddDI(rGrid, ref m_diDoorFan[5], m_id, "VS Bot Door Fan", "Vision_Bottom_Door_Fan");
        }

        public override void RunGrid(eGrid eMode, ezTools.ezJob job = null)
        {
            base.RunGrid(eMode, job);
            m_grid.Refresh();
        }

        public override void ModelGrid(ezGrid rGrid, eGrid eMode)
        {
            rGrid.Set(ref m_bEMG, "Work", "bEMG", "Emergency Sensor State");
            rGrid.Set(ref m_bAirCheck, "Work", "bAirCheck", "AirCheck Sensor State");
            rGrid.Set(ref m_bMapDataUse,"MapData", "Use", "Input Map Data File Use");
            rGrid.Set(ref m_strFilePath, "MapData", "FilePath", "Input Map Data File Path");
            rGrid.Set(ref m_bDoorFanAlarmUse, "Door", "Door Fan Check", "Door Fan Check");
            rGrid.Set(ref m_bDoorOpenUse, "Door", "Door Open Check", "Door Open Check");
            rGrid.Set(ref m_bTestRun, "Work", "bTest", "AirCheck Sensor State");
            rGrid.Set(ref m_bLightCurtain, "Work", "bLightCurtain", "LightCurtain Sensor State");
            if (eMode == eGrid.eRegRead) m_bTestRun = false; 
            m_bInvalid = true;
        }

        public override object GetManualJob()
        {
            return new UI_ManualJob_Hynix();
        }

        public override object MakeUI()
        {
            return new UI_EFEM_New();
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

        public override bool IsLightCurtainError()                   // 160817 ADD SDH 
        {
            if (m_bLightCurtain == m_control.GetInputBit(m_diLightCurtain)) return true;
            return false;
        }

        public override bool IsDoorOpen()           ///kjw 수정 필요
        {
            if (!m_bDoorOpenUse) return false;
            for (int n = 0; n < 5; n++)
            {
                if (!m_control.GetInputBit(m_diDoor[n]))
                {
                    if (n > 0 || m_diDoor[n] < 0) continue;
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
                    if (n > 0 || m_diDoorVision[n] < 0) continue;
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

        public override bool IsFanAlarm()        ///kjw 수정 필요
        {
            if (!m_bDoorFanAlarmUse)
                return false;
            for (int n = 0; n < 6; n++)
            {
                //if (m_diDoor[n] < 0) return true;

                if (m_control.GetInputBit(m_diDoorFan[n]) && m_diDoorFan[n] != -1)
                {
                    switch (n)
                    {
                        case 0:
                            m_strDoorFanName = "EFEM PC Door Fan";
                            break;
                        case 1:
                            m_strDoorFanName = "EFEM TOP Door Fan";
                            break;
                        case 2:
                            m_strDoorFanName = "EFEM WTR Door Fan";
                            break;
                        case 3:
                            m_strDoorFanName = "VS PC Door Fan";
                            break;
                        case 4:
                            m_strDoorFanName = "VS Top Door Fan";
                            break;
                        case 5:
                            m_strDoorFanName = "VS Bot Door Fan";
                            break;
                        case 6:
                            m_strDoorFanName = "EFEM Air Door Fan";
                            break;
                    }
                    return true;
                }
            }
            return false;
        }

        public override void RunLamp()
        {
            bool bLamp = ((m_swLamp.Check() / 1000) % 2) != 0;
            bool bStartEnable = IsStartEnable(false);
            if (m_run.GetState() != m_LastWorkRunState)
            {
                m_bBuzzerOff = false;
                m_LastWorkRunState = m_run.GetState();
            }
            if (m_doBuzzer0 > -1)
            {
                m_control.WriteOutputBit(m_doBuzzer0 + 1, m_run.GetState() == eWorkRun.Warning0);
                m_control.WriteOutputBit(m_doBuzzer0 + 2, m_run.GetState() == eWorkRun.Warning1);
                //m_control.WriteOutputBit(m_doBuzzer0 + 3, m_run.GetState() == eWorkRun.Error);
            }

            bool bRed = (m_run.GetState() == eWorkRun.Warning0) || (m_run.GetState() == eWorkRun.Warning1) || (m_run.GetState() == eWorkRun.Error);
            if (IsDoorOpen()) m_control.WriteOutputBit(m_doTowerRed, true); // Maintenance Mode
            else m_control.WriteOutputBit(m_doTowerRed, bRed && bLamp);
            m_control.WriteOutputBit(m_doTowerYellow, IsLoadPortNotReady());
            m_control.WriteOutputBit(m_doTowerGreen, m_run.GetState() == eWorkRun.Run);

            bool bBlue = false;
            if (m_xGem.IsOnlineRemote())
            {
                if (m_xGem.m_aXGem300Carrier[0].m_ePortAccess == XGem300Carrier.ePortAccess.Auto
                    || m_xGem.m_aXGem300Carrier[1].m_ePortAccess == XGem300Carrier.ePortAccess.Auto) // OHT Mode
                    bBlue = true;
                else bBlue = bLamp;
            }
            m_control.WriteOutputBit(m_doTowerBlue, bBlue); 

            if (m_run.GetState() == eWorkRun.Run && IsDoorOpen() && m_swDoor.Check() > 10000)
            {
                m_log.Popup("Door Open Detected (" + m_strDoorName + ")");
                m_swDoor.Start();
            }
            if (IsFanAlarm() && m_swDoorFan.Check() > 10000)
            {
                m_log.Popup("Door Fan Alarm Detected (" + m_strDoorFanName + ")");
                m_swDoorFan.Start();
            }
            m_control.WriteOutputBit(m_doLight, IsManual() || IsDoorOpen());
            m_control.WriteOutputBit(m_doDoorLock, m_run.GetState() == eWorkRun.Run);
            if (m_run.GetState() == eWorkRun.Run && IsLightCurtainError() && (m_handler.ClassLoadPort(0).GetState() != HW_LoadPort_Mom.eState.Ready || m_handler.ClassLoadPort(1).GetState() != HW_LoadPort_Mom.eState.Ready))
            {
                m_control.WriteOutputBit(m_doBuzzer0 + 1, true);
                m_log.Popup("Light Curtain Detected");
            }
            else m_control.WriteOutputBit(m_doBuzzer0 + 1, false);
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
