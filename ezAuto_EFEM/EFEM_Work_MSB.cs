﻿using System;
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
    public partial class EFEM_Work_MSB : Work_Mom, Control_Child
    {
        EFEM_Handler m_handler;

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
        bool m_bLogCurtain = false;
        Timer m_timerCurtain = new Timer();

        int m_diDoorLock = -1;
        int m_doDoorLock = -1;
        bool m_bDoorOpenUse = false;

        ezStopWatch m_swDoor = new ezStopWatch();
        string m_strDoorName = "";

        int m_doLight = -1;
        eWorkRun m_LastWorkRunState = eWorkRun.Error;


        public EFEM_Work_MSB()
        {
            InitializeComponent();
            m_timerCurtain.Enabled = true;
            m_timerCurtain.Interval = 2000;
            m_timerCurtain.Tick += new System.EventHandler(timerCurtain_Tick);
        }

        public override void Init(string id, object auto)
        {
            base.Init(id, auto);
            m_bJobOpen = true;
            m_control = ((Auto_Mom)m_auto).ClassControl();
            m_control.Add(this);
            m_handler = (EFEM_Handler)((Auto_Mom)m_auto).ClassHandler();
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
            control.AddDI(rGrid, ref m_diDoorLock, m_id, "DoorLock", "DoorLock");
            control.AddDI(rGrid, ref m_diLightCurtain, m_id, "LightCurtain", "LightCurtain");
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
            rGrid.Set(ref m_bDoorOpenUse, "Door", "Door Open Check", "Door Open Check");
            rGrid.Set(ref m_bTestRun, "Work", "bTest", "AirCheck Sensor State");
            rGrid.Set(ref m_bLightCurtain, "Work", "bLightCurtain", "LightCurtain Sensor State");
            if (eMode == eGrid.eRegRead) m_bTestRun = false; 
            m_bInvalid = true;
        }

        public override object GetManualJob()
        {
            return new UI_ManualJob_MSB();
        }

        public override object MakeUI()
        {
            return new UI_EFEM_old();
        }

        public override void JobOpen(ezJob job)
        {
            m_handler.m_backSide.JobOpen(job);
        }

        public override void JobSave(ezJob job)
        {
            m_handler.m_backSide.JobSave(job); 
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

        public override bool IsLightCurtainError()
        {
            if (m_bLightCurtain == m_control.GetInputBit(m_diLightCurtain)) return true;
            return false;
        }

        public override bool IsDoorOpen()
        {
            if (!m_bDoorOpenUse) return false;
            if (m_diDoorLock < 0) return false;
            return !m_control.GetInputBit(m_diDoorLock);
        }

        public override void RunLamp()
        {
            long msLamp = m_swLamp.Check() % 1000;
            bool bStartEnable = IsStartEnable(false);
            if (m_run.GetState() != m_LastWorkRunState)
            {
                m_bBuzzerOff = false;
                m_LastWorkRunState = m_run.GetState();
            }
            switch (m_run.GetState())
            {
                case eWorkRun.Init:
                    SettingLamp(eLampState.eInit);
                    break;
                case eWorkRun.Home:
                    SettingLamp(eLampState.eHome);
                    break;
                case eWorkRun.Ready:
                    SettingLamp(eLampState.eReady);
                    break;
                case eWorkRun.Run:
                    if (m_handler.ClassLoadPort(0).GetState() == HW_LoadPort_Mom.eState.Ready && m_handler.ClassLoadPort(1).GetState() == HW_LoadPort_Mom.eState.Ready) { SettingLamp(eLampState.eRunNone); }
                    else if (m_handler.ClassLoadPort(0).GetState() != HW_LoadPort_Mom.eState.Ready && m_handler.ClassLoadPort(1).GetState() != HW_LoadPort_Mom.eState.Ready) { SettingLamp(eLampState.eRunAll); }
                    else { SettingLamp(eLampState.eRunhalf); }
                    break;
                case eWorkRun.Warning0:
                    SettingLamp(eLampState.eAlarm1);
                    break;
                case eWorkRun.Warning1:
                    SettingLamp(eLampState.eAlarm2);
                    break;
                case eWorkRun.Error:
                    SettingLamp(eLampState.eAlarm3);
                    break;
                case eWorkRun.AutoLoad:
                case eWorkRun.AutoUnload:
                    SettingLamp(eLampState.eReacovery);
                    break;
                case eWorkRun.PickerSet:
                    break;
            }
            if (m_run.GetState() == eWorkRun.Run && IsDoorOpen() && m_swDoor.Check() > 10000)
            {
                m_log.Popup("Door Open Detected (" + m_strDoorName + ")");
                m_swDoor.Start();
            }
            m_control.WriteOutputBit(m_doLight, IsManual() || IsDoorOpen());
            m_control.WriteOutputBit(m_doDoorLock, m_run.GetState() == eWorkRun.Run);
            //if (m_run.GetState() == eWorkRun.Run && IsLightCurtainError() && (m_handler.ClassLoadPort(0).GetState() != HW_LoadPort_Mom.eState.Ready || m_handler.ClassLoadPort(1).GetState() != HW_LoadPort_Mom.eState.Ready))
            //{
            //    m_bLogCurtain = true;
            //    m_control.WriteOutputBit(m_doBuzzer0 + 1, true);
            //}
            //else
            //{
            //    m_bLogCurtain = false;
            //    m_control.WriteOutputBit(m_doBuzzer0 + 1, false);
            //}
        }

        public void SettingLamp(eLampState eState)                                  //170210 SDH ADD
        {
            long msLamp = m_swLamp.Check();

            bool bRed = false;
            bool bYellow = false;
            bool bGreen = false;
            bool bRFlash = false;
            bool bYFlash = false;
            bool bGFlash = false;
            bool bBuzzer1 = false;
            bool bBuzzer2 = false;
            bool bBuzzer3 = false;

            int nDelay = Convert.ToInt32(m_Lamp.SetValue[(int)eState, 4]);

            if (m_Lamp.SetValue[(int)eState, 0] == "ON") bRed = true;
            else if (m_Lamp.SetValue[(int)eState, 0] == "FLASH") bRFlash = true;

            if (m_Lamp.SetValue[(int)eState, 1] == "ON") bYellow = true;
            else if (m_Lamp.SetValue[(int)eState, 1] == "FLASH") bYFlash = true;

            if (m_Lamp.SetValue[(int)eState, 2] == "ON") bGreen = true;
            else if (m_Lamp.SetValue[(int)eState, 2] == "FLASH") bGFlash = true;

            if (m_Lamp.SetValue[(int)eState, 3] == "Buzzer1") bBuzzer1 = true;
            else if (m_Lamp.SetValue[(int)eState, 3] == "Buzzer2") bBuzzer2 = true;
            else if (m_Lamp.SetValue[(int)eState, 3] == "Buzzer3") bBuzzer3 = true;

            if (m_Lamp.SetValue[(int)eState, 5] == "Use")
            {
                if (nDelay == 0 || bRed) m_control.WriteOutputBit(m_doTowerRed, bRed || bRFlash);
                else if (bRFlash) m_control.WriteOutputBit(m_doTowerRed, bRFlash && (msLamp % (2 * nDelay) > nDelay));
                else m_control.WriteOutputBit(m_doTowerRed, bRed);

                if (nDelay == 0 || bYellow) m_control.WriteOutputBit(m_doTowerYellow, bYellow || bYFlash);
                else if (bYFlash) m_control.WriteOutputBit(m_doTowerYellow, bYFlash && (msLamp % (2 * nDelay) > nDelay));
                else m_control.WriteOutputBit(m_doTowerYellow, bYellow);

                if (nDelay == 0 || bGreen) m_control.WriteOutputBit(m_doTowerGreen, bGreen || bGFlash);
                else if (bGFlash) m_control.WriteOutputBit(m_doTowerGreen, bGFlash && (msLamp % (2 * nDelay) > nDelay));
                else m_control.WriteOutputBit(m_doTowerGreen, bGreen);

                if (m_bWorkerBuzzerOn)
                {
                    m_control.WriteOutputBit(m_doBuzzer0 + 3, true);
                }
                else if (m_doBuzzer0 > -1)
                {
                    m_control.WriteOutputBit(m_doBuzzer0 + 1, bBuzzer1 && !m_bBuzzerOff);
                    m_control.WriteOutputBit(m_doBuzzer0 + 2, bBuzzer2 && !m_bBuzzerOff);
                    m_control.WriteOutputBit(m_doBuzzer0 + 3, bBuzzer3 && !m_bBuzzerOff);
                }
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
            if (!bOn) m_bWorkerBuzzerOn = false; // ing 170717
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

        private void timerCurtain_Tick(object sender, EventArgs e)
        {
            if (m_bLogCurtain) m_log.Popup("Light Curtain Detected");
        }
    }
}
