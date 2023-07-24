using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using WeifenLuo.WinFormsUI.Docking;
using EasyModbus;
using ezTools;

namespace ezAutoMom
{
    public partial class HW_FFU_Mars : HW_FFU_Mom
    {
        public enum eType { Serial, TCPIP };
        eType m_eType = eType.Serial;
        string m_strType = eType.Serial.ToString();
        string[] m_strTypes = Enum.GetNames(typeof(eType));

        //for Serial 
        //RS232 Setting 9600 8 1 None
        string m_strSerialPort = "COM50";
        int m_nBaud = 9600;
        StopBits m_eStopBits = StopBits.One;
        string m_strStopBits = StopBits.One.ToString();
        string[] m_strStopBitsArray = Enum.GetNames(typeof(StopBits));
        Parity m_eParity = Parity.None;
        string m_strParity = Parity.None.ToString();
        string[] m_strParitys = Enum.GetNames(typeof(Parity));

        //for TCPIP
        string m_strIP = "192.0.0.1";
        int m_nPortNumber = 7700;

        //bool m_bUse = false;
        bool m_bUseLog = false;
        bool m_bBusy = false;
        int m_nConnect = 3;
        int m_nInterval = 500;
        int m_nTimeout = 3000;
        bool m_bPause = false;

        ModbusClient m_client = new ModbusClient();
        HW_FFU_UI m_ui = new HW_FFU_UI();
        FFUModule_Mars m_currentModule;
        Thread m_thread;
        ezStopWatch m_swTimeout = new ezStopWatch();
        bool m_bRunThread = false;

        public override void Init(string id, Auto_Mom auto, int nModuleNum)
        {
            base.Init(id, auto, nModuleNum);
            m_module = new FFUModule_Mars[m_nModuleNumber];
            for (int n = 0; n < m_nModuleNumber; n++)
            {
                m_module[n] = new FFUModule_Mars(n);
            }
            RunGrid(eGrid.eRegRead);
            RunGrid(eGrid.eInit);
            m_client.SerialPort = m_strSerialPort;
            m_client.Baudrate = m_nBaud;
            m_client.StopBits = m_eStopBits;
            m_client.Parity = m_eParity;
            m_client.IPAddress = m_strIP;
            m_client.Port = m_nPortNumber;
            for (int n = 0; n < m_nConnect; n++)
            {
                try
                {
                    m_client.Connect();
                    m_client.NumberOfRetries = 0;
                }
                catch (Exception ex)
                {
                    m_log.Popup(m_id + " Config is Wrong !!");
                    m_log.Add(ex.Message);
                }
                if (m_client.Connected)
                {
                    m_client.SendDataChanged += new ModbusClient.SendDataChangedHandler(SendDataChanged);
                    m_client.ReceiveDataChanged += new ModbusClient.ReceiveDataChangedHandler(ReceiveDataChanged);
                    m_thread = new Thread(new ThreadStart(RunThread));
                    m_thread.Start();
                    break;
                }
                Thread.Sleep(100);
            }
            if (!m_client.Connected)
            {
                m_log.Popup(m_id + " : Connection Fail !!");
            }
            m_ui.Init(this, m_nModuleNumber);
            m_ui.SetTableLayoutPanel();
        }

        protected override void Connect()
        {
            if (!m_client.Connected)
            {
                try
                {
                    m_client.Connect();
                    m_client.NumberOfRetries = 0;
                }
                catch (Exception ex)
                {
                    m_log.Popup(m_id + " Config is Wrong !!");
                    m_log.Add(ex.Message);
                }
                if (m_client.Connected)
                {
                    m_client.SendDataChanged += new ModbusClient.SendDataChangedHandler(SendDataChanged);
                    m_client.ReceiveDataChanged += new ModbusClient.ReceiveDataChangedHandler(ReceiveDataChanged);
                    m_thread = new Thread(new ThreadStart(RunThread));
                    m_thread.Start();
                }
            }
        }

        protected override void Reset()
        {
            m_bPause = true;
            Thread.Sleep(1000);
            ezStopWatch swTimeout = new ezStopWatch();
            swTimeout.Start();
            while (m_bBusy && swTimeout.Check() < m_nTimeout)
            {
                Thread.Sleep(100);
            }
            if (m_bBusy)
            {
                m_log.Popup("Reset Fail !!");
                return;
            }
            ArrayList listFan = new ArrayList();
            bool bHeaterEFEM = false;
            bool bHeaterVision = false;
            for (int n = 0; n < m_module.Length; n++)
            {
                FFUModule_Mars module = (FFUModule_Mars)m_module[n];
                bool bExisted = false;
                foreach (int nAddress in listFan)
                {
                    if (nAddress == module.m_nSubAddress)
                    {
                        bExisted = true;
                        break;
                    }
                    else if (module.m_eDataType == FFUModule_Mars.eDataType.EFEMTemp)
                    {
                        bHeaterEFEM = true;
                    }
                    else if (module.m_eDataType == FFUModule_Mars.eDataType.VisionTemp)
                    {
                        bHeaterVision = true;
                    }
                }
                if (!bExisted) listFan.Add(module.m_nSubAddress);
            }
            foreach (int n in listFan)
            {
                m_bBusy = true;
                m_client.WriteSingleRegister(n + (int)FFUModule_Mars.eDataType.Reset, 1);
                if (WaitRecive()) m_bBusy = false;
            }
            if (bHeaterEFEM)
            {
                m_bBusy = true;
                m_client.WriteSingleRegister(209, 1);
                if (WaitRecive()) m_bBusy = false;
            }
            if (bHeaterVision)
            {
                m_bBusy = true;
                m_client.WriteSingleRegister(257, 1);
                if (WaitRecive()) m_bBusy = false;
            }
            m_bPause = false;
        }

        void ReceiveDataChanged(object sender)
        {
            string strHex;
            strHex = BitConverter.ToString(m_client.receiveData);
            if (m_bUseLog) m_log.Add(m_id + "<-- : " + strHex);
            if (m_client.receiveData.Length != 7) return;
            byte[] aData = new byte[2];
            aData[0] = m_client.receiveData[4];
            aData[1] = m_client.receiveData[3];
            BitArray aBit = new BitArray(aData);
            switch (m_currentModule.m_eDataType)
            {
                case FFUModule_Mars.eDataType.RPM:
                case FFUModule_Mars.eDataType.Pressure:
                case FFUModule_Mars.eDataType.EFEMTemp:
                case FFUModule_Mars.eDataType.VisionTemp:
                case FFUModule_Mars.eDataType.EFEMHumidity:
                case FFUModule_Mars.eDataType.VisionHumidity:
                    m_currentModule.m_fData = 0;
                    for (int n = 0; n < aBit.Length; n++)
                    {
                        if (aBit.Get(n))
                        {
                            m_currentModule.m_fData += (int)Math.Pow(2, n);
                        }
                    }
                    if (m_currentModule.m_nDecimalPoint != 0) m_currentModule.m_fData /= (10 * m_currentModule.m_nDecimalPoint);
                    m_currentModule.m_sValue = m_currentModule.m_fData.ToString();
                    break;
                case FFUModule_Mars.eDataType.Config:
                    // 미사용
                    break;
                case FFUModule_Mars.eDataType.State:
                    m_currentModule.m_sValue = "";
                    bool bAlarm = false;
                    for (int n = 0; n < m_currentModule.m_bState.Length; n++)
                    {
                        m_currentModule.m_bState[n] = aBit.Get(n);
                        if (n != 9 && n != 10) bAlarm = m_currentModule.m_bState[n] || bAlarm;
                    }
                    if (m_currentModule.m_bState[9]) { m_currentModule.m_sValue += "Fan Working"; }
                    else { m_currentModule.m_sValue += "Fan Stop"; bAlarm = true; }
                    if (m_currentModule.m_bState[7]) m_currentModule.m_sValue += ", Communicate Error";
                    if (m_currentModule.m_bState[6]) m_currentModule.m_sValue += ", Timeover Error";
                    if (m_currentModule.m_bState[5]) m_currentModule.m_sValue += ", Pressure Low Error";
                    if (m_currentModule.m_bState[4]) m_currentModule.m_sValue += ", Pressure High Error";
                    if (m_currentModule.m_bState[3]) m_currentModule.m_sValue += ", RPM Low Error";
                    if (m_currentModule.m_bState[2]) m_currentModule.m_sValue += ", RPM High Error";
                    if (m_currentModule.m_bState[1]) m_currentModule.m_sValue += ", Pressure Sensor Error";
                    if (m_currentModule.m_bState[0]) m_currentModule.m_sValue += ", Fan Operating Error";
                    m_currentModule.m_bStateAlarmMessage = bAlarm;
                    break;
                case FFUModule_Mars.eDataType.Reset:
                    // 미사용
                    break;
                case FFUModule_Mars.eDataType.EFEMTempAlarm:
                case FFUModule_Mars.eDataType.VisionTempAlam:
                    m_currentModule.m_sValue = "";
                    bool bAlarm1 = false;
                    for (int n = 0; n < m_currentModule.m_bTempAlarm.Length; n++)
                    {
                        m_currentModule.m_bTempAlarm[n] = aBit.Get(n);
                        bAlarm1 = m_currentModule.m_bTempAlarm[n] || bAlarm1;
                    }
                    if (m_currentModule.m_bTempAlarm[11]) m_currentModule.m_sValue += "Spare Temperature1 Low Alarm ";
                    if (m_currentModule.m_bTempAlarm[10]) m_currentModule.m_sValue += "Sapre Temperature1 Low Alarm ";
                    if (m_currentModule.m_bTempAlarm[9]) m_currentModule.m_sValue += "Heater2 Temperature Low Alarm ";
                    if (m_currentModule.m_bTempAlarm[8]) m_currentModule.m_sValue += "Heater2 Temperature High Alarm ";
                    if (m_currentModule.m_bTempAlarm[7]) m_currentModule.m_sValue += "Heater1 Temperature Low Alarm ";
                    if (m_currentModule.m_bTempAlarm[6]) m_currentModule.m_sValue += "Heater1 Temperature High Alarm ";
                    if (m_currentModule.m_bTempAlarm[5]) m_currentModule.m_sValue += "Moudule Temperature2 Low Alarm ";
                    if (m_currentModule.m_bTempAlarm[4]) m_currentModule.m_sValue += "Moudule Temperature2 High Alarm ";
                    if (m_currentModule.m_bTempAlarm[3]) m_currentModule.m_sValue += "Moudule Temperature1 Low Alarm ";
                    if (m_currentModule.m_bTempAlarm[2]) m_currentModule.m_sValue += "Moudule Temperature1 High Alarm ";
                    if (m_currentModule.m_bTempAlarm[1]) m_currentModule.m_sValue += "Moudule Wetness Low Alarm ";
                    if (m_currentModule.m_bTempAlarm[0]) m_currentModule.m_sValue += "Moudule Wetness High Alarm ";
                    m_currentModule.m_bStateAlarmMessage = bAlarm1;
                    break;
                case FFUModule_Mars.eDataType.EFEMTempWarning:
                case FFUModule_Mars.eDataType.VisionempWarning:
                    m_currentModule.m_sValue = "";
                    bool bAlarm2 = false;
                    for (int n = 0; n < m_currentModule.m_bTempWarning.Length; n++)
                    {
                        m_currentModule.m_bTempWarning[n] = aBit.Get(n);
                        bAlarm2 = m_currentModule.m_bTempWarning[n] || bAlarm2;
                    }
                    if (m_currentModule.m_bTempWarning[11]) m_currentModule.m_sValue += "Spare Temperature1 Low Warning ";
                    if (m_currentModule.m_bTempWarning[10]) m_currentModule.m_sValue += "Sapre Temperature1 Low Warning ";
                    if (m_currentModule.m_bTempWarning[9]) m_currentModule.m_sValue += "Heater2 Temperature Low Warning ";
                    if (m_currentModule.m_bTempWarning[8]) m_currentModule.m_sValue += "Heater2 Temperature High Warning ";
                    if (m_currentModule.m_bTempWarning[7]) m_currentModule.m_sValue += "Heater1 Temperature Low Warning ";
                    if (m_currentModule.m_bTempWarning[6]) m_currentModule.m_sValue += "Heater1 Temperature High Warning ";
                    if (m_currentModule.m_bTempWarning[5]) m_currentModule.m_sValue += "Moudule Temperature2 Low Warning ";
                    if (m_currentModule.m_bTempWarning[4]) m_currentModule.m_sValue += "Moudule Temperature2 High Warning ";
                    if (m_currentModule.m_bTempWarning[3]) m_currentModule.m_sValue += "Moudule Temperature1 Low Warning ";
                    if (m_currentModule.m_bTempWarning[2]) m_currentModule.m_sValue += "Moudule Temperature1 High Warning ";
                    if (m_currentModule.m_bTempWarning[1]) m_currentModule.m_sValue += "Moudule Wetness Low Warning ";
                    if (m_currentModule.m_bTempWarning[0]) m_currentModule.m_sValue += "Moudule Wetness High Warning ";
                    m_currentModule.m_bStateAlarmMessage = bAlarm2;
                    break;
                case FFUModule_Mars.eDataType.EFEMTempHMCData:
                case FFUModule_Mars.eDataType.VisionTempHMCData:
                    m_currentModule.m_sValue = "";
                    for (int n = 0; n < m_currentModule.m_bHCMData.Length; n++)
                    {
                        m_currentModule.m_bHCMData[n] = aBit.Get(n);
                        m_currentModule.m_bHCMDataAlarmMessage = m_currentModule.m_bHCMDataAlarmMessage || m_currentModule.m_bState[n];
                    }
                    if (m_currentModule.m_bHCMData[4]) m_currentModule.m_sValue += "Temperature/Wetness Sensor Comm Fail ";
                    if (m_currentModule.m_bHCMData[3]) m_currentModule.m_sValue += "4ch Temperature Conteroler Comm Fail ";
                    if (m_currentModule.m_bHCMData[2]) m_currentModule.m_sValue += "Temperature Controler Comm Fail ";
                    if (m_currentModule.m_bHCMData[1]) m_currentModule.m_sValue += "Wetness Controler Comm Fail ";
                    if (m_currentModule.m_bHCMData[0]) m_currentModule.m_sValue += "Controler Fail ";
                    break;

            }
            m_bBusy = false;
        }

        void SendDataChanged(object sender)
        {
            string strHex;
            strHex = BitConverter.ToString(m_client.sendData);
            if (m_bUseLog) m_log.Add(m_id + "--> : " + strHex);
        }

        void RunThread()
        {
            m_bRunThread = true;
            Thread.Sleep(5000);
            while (m_bRunThread)
            {
                Thread.Sleep(500);
                for (int n = 0; n < m_module.Length; n++)
                {
                    try
                    {
                        if (m_bPause) continue;
                        m_currentModule = (FFUModule_Mars)m_module[n];
                        if (m_currentModule.m_nAddress > 0 && m_currentModule.m_nAddress < 255) m_client.UnitIdentifier = Convert.ToByte(m_currentModule.m_nAddress);
                        m_bBusy = true;
                        int[] aResult = m_client.ReadHoldingRegisters(m_currentModule.m_nSubAddress + (int)m_currentModule.m_eDataType, 1);
                        /*
                        switch (m_currentModule.m_eDataType)
                        {
                            case FFUModule_Mars.eDataType.RPM:
                                int[] aRPM = m_client.ReadHoldingRegisters(m_currentModule.m_nSubAddress + (int)FFUModule_Mars.eDataType.RPM, 1);
                                break;
                            case FFUModule_Mars.eDataType.Config:
                                // 미사용
                                break;
                            case FFUModule_Mars.eDataType.State:
                                int[] aState = m_client.ReadHoldingRegisters(m_currentModule.m_nSubAddress + (int)FFUModule_Mars.eDataType.State, 1);
                                break;
                            case FFUModule_Mars.eDataType.Reset:
                                // 미사용
                                break;
                            case FFUModule_Mars.eDataType.Pressure:
                                int[] aPressure = m_client.ReadHoldingRegisters(m_currentModule.m_nSubAddress + (int)FFUModule_Mars.eDataType.Pressure, 1);
                                break;
                        }
                        */
                        if (WaitRecive()) m_bBusy = false;
                        if (!m_bRunThread) return;
                    }
                    catch (Exception ex)
                    {
                        m_bBusy = false;
                        m_log.Add(ex.Message);
                    }
                    Thread.Sleep(1 + m_nInterval);
                }
            }
        }

        bool WaitRecive()
        {
            m_swTimeout.Start();
            while (m_bBusy && m_swTimeout.Check() < m_nTimeout)
            {
                Thread.Sleep(10);
            }
            if (m_bBusy) return true;
            return false;
        }

        public override void ThreadStop()
        {
            base.ThreadStop();
            if (m_bRunThread)
            {
                m_bRunThread = false;
                m_thread.Join();
            }
            if (m_client.Connected)
            {
                m_client.Disconnect();
            }
        }

        protected override void RunGrid(eGrid eMode)
        {
            base.RunGrid(eMode);
            m_grid.Set(ref m_nInterval, m_id, "Interval", "Interval");
            m_grid.Set(ref m_nTimeout, m_id, "Timeout", "Recive Timeout");
            m_grid.Set(ref m_bUseLog, m_id, "UseLog", "Use Trace Communication Log");
            m_grid.Set(ref m_strType, m_strTypes, m_id, "Type", "Communication Type");
            m_grid.Set(ref m_nConnect, m_id, "TryConnection", "Number of Count To Connect");
            if (m_eType == eType.Serial)
            {
                m_grid.Set(ref m_strSerialPort, m_id, "Port", "Serial Port Number");
                m_grid.Set(ref m_nBaud, m_id, "Baud", "Serial Baud rate");
                m_grid.Set(ref m_strStopBits, m_strStopBitsArray, m_id, "StopBits", "Serial StopBits");
                m_grid.Set(ref m_strParity, m_strParitys, m_id, "Parity", "Serial Parity");
                m_eStopBits = (StopBits)Enum.Parse(typeof(StopBits), m_strStopBits);
                m_eParity = (Parity)Enum.Parse(typeof(Parity), m_strParity);
            }
            else
            {
                m_grid.Set(ref m_strIP, m_id, "IP", "TCP/IP Address");
                m_grid.Set(ref m_nPortNumber, m_id, "Port", "TCP/IP Port Number");
            }
            for (int n = 0; n < m_module.Length; n++)
            {
                m_module[n].RunGrid(m_grid, eMode);
            }
            m_grid.Refresh();
        }

        public void ReadRPM()
        {
            if (!m_client.Connected)
            {
                try
                {
                    m_client.Connect();
                }
                catch (Exception ex)
                {
                    m_log.Add(m_id + " : " + ex.Message);
                }
                return;
            }
            //if (fdcModule.m_nAddress < 0) return;
            if (m_bBusy) return;
            m_bBusy = true;
            try
            {

            }
            catch (Exception ex)
            {
                m_bBusy = false;
                m_log.Add(m_id + " : " + ex.Message);
            }
        }

        public override IDockContent GetContentFromPersistString(string persistString)
        {
            if (m_ui.IsPersistString(persistString)) return m_ui;
            return null;
        }

        public override void ShowAll(DockPanel dockPanel)
        {
            m_ui.ShowPanel(dockPanel);
        }

        public override FFUModule_Mom GetFFUModule(int nIndex)
        {
            return m_module[nIndex];
        }

        protected override void RunTimer()
        {
            base.RunTimer();
        }
    }

    public class FFUModule_Mars : FFUModule_Mom
    {
        public enum eDataType
        {
            RPM = 0,
            Config = 32,
            State = 64,
            Reset = 96,
            Pressure = 128,
            None = 160,
            EFEMTemp = 192, //0xC0
            EFEMHumidity = 193, // 0xC1
            EFEMTempAlarm = 201,
            EFEMTempWarning = 202,
            EFEMTempHMCData = 204,
            VisionTemp = 224, //0xD0
            VisionHumidity = 225, // 0xD1
            VisionTempAlam = 233,
            VisionempWarning = 234,
            VisionTempHMCData = 236,
        }

        public bool[] m_bState = new bool[16]
        {
            false, // 15
            false, // 14
            false, // 13
            false, // 12
            false, // 11
            true, // 10 동작 모드 false : RPM, PWR  true : PRE
            true, // 9 false : 정지, true : 운전
            false, // 8
            false, // 7 통신 false : 정상, true : 경보
            false, // 6 시간오버 false : 정상, true : 경보
            false, // 5 차압Low false : 정상, true : 경보
            false, // 4 차압High false : 정상, true : 경보
            false, // 3 RPM Low false : 정상, true : 경보
            false, // 2 RPM High false : 정상, true : 경보
            false, // 1 차압센서 false : 정상, true : 경보
            false, // 0 FAN 동작 false : 정상, true : 경보
        };

        public bool[] m_bTempAlarm = new bool[16]
        {
            false, // 15
            false, // 14
            false, // 13
            false, // 12
            false, // 11 Spaer온도L false : 정상, true : 경보
            false, // 10 Spare온도H false : 정상, true : 경보
            false, // 9 히터2온도L false : 정상, true : 경보
            false, // 8 히터2온도H false : 정상, true : 경보
            false, // 7 히터1온도L false : 정상, true : 경보
            false, // 6 히터1온도H false : 정상, true : 경보
            false, // 5 내부온도2L false : 정상, true : 경보
            false, // 4 내부온도2H false : 정상, true : 경보
            false, // 3 내부온도1L false : 정상, true : 경보
            false, // 2 내부온도1H false : 정상, true : 경보
            false, // 1 내부 습도L false : 정상, true : 경보
            false, // 0 내부 습도H false : 정상, true : 경보
        };

        public bool[] m_bTempWarning = new bool[16]
        {
            false, // 15
            false, // 14
            false, // 13
            false, // 12
            false, // 11 Spaer온도L false : 정상, true : 경보
            false, // 10 Spare온도H false : 정상, true : 경보
            false, // 9 히터2온도L false : 정상, true : 경보
            false, // 8 히터2온도H false : 정상, true : 경보
            false, // 7 히터1온도L false : 정상, true : 경보
            false, // 6 히터1온도H false : 정상, true : 경보
            false, // 5 내부온도2L false : 정상, true : 경보
            false, // 4 내부온도2H false : 정상, true : 경보
            false, // 3 내부온도1L false : 정상, true : 경보
            false, // 2 내부온도1H false : 정상, true : 경보
            false, // 1 내부 습도L false : 정상, true : 경보
            false, // 0 내부 습도H false : 정상, true : 경보
        };

        public bool[] m_bHCMData = new bool[16]
        {
            false, // 15
            false, // 14
            false, // 13
            false, // 12
            false, // 11
            false, // 10
            false, // 9
            false, // 8
            false, // 7
            false, // 6
            false, // 5
            false, // 4 온습도센서통신이상 false : 정상, true : 경보
            false, // 3 4Ch온도컨트롤러통신이상 false : 정상, true : 경보
            false, // 2 온도컨트롤러통신이상 false : 정상, true : 경보
            false, // 1 습도컨트롤러통신이상 false : 정상, true : 경보
            false, // 0 ControlerFail false : 정상, true : 경보
        };

        public int m_nID = -1;
        public eUnit m_eUnit = eUnit.None;
        public int m_nAddress = -1;
        public int m_fHighLimit = 0;
        public int m_fLowLimit = 0;
        public int m_nDecimalPoint = 0;
        public double m_fData = 0;
        public bool m_bStateAlarmMessage = false;
        public bool m_bTempAlarmMessage = false;
        public bool m_bTempWarningMessage = false;
        public bool m_bHCMDataAlarmMessage = false;

        public eDataType m_eDataType = eDataType.RPM;
        string[] m_strDataTypes = Enum.GetNames(typeof(eDataType));
        string m_strDataType = eDataType.RPM.ToString();
        public int m_nSubAddress = 0;

        public FFUModule_Mars(int nID)
        {
            m_nID = nID;
            m_nAddress = 1;
            m_nSubAddress = nID;
        }

        public override void RunGrid(ezGrid rGrid, eGrid eMode)
        {
            string[] strUnits = new string[Enum.GetNames(typeof(FFUModule_Mars.eUnit)).Length];
            for (int i = 0; i < Enum.GetNames(typeof(FFUModule_Mars.eUnit)).Length; i++)
            {
                strUnits[i] = Enum.GetName(typeof(FFUModule_Mars.eUnit), i);
            }
            string strUnit = m_eUnit.ToString();
            rGrid.Set(ref strUnit, strUnits, m_nID.ToString("FFU_0"), "Unit", "Unit");
            m_eUnit = (eUnit)Enum.Parse(typeof(eUnit), strUnit);
            rGrid.Set(ref m_nAddress, m_nID.ToString("FFU_0"), "ID", "ID Address");
            rGrid.Set(ref m_nSubAddress, m_nID.ToString("FFU_0"), "SubAdress", "SubsAddress");
            rGrid.Set(ref m_strDataType, m_strDataTypes, m_nID.ToString("FFU_0"), "DataType", "Data Type");
            m_eDataType = (eDataType)Enum.Parse(typeof(eDataType), m_strDataType);
            rGrid.Set(ref m_nDecimalPoint, m_nID.ToString("FFU_0"), "Dec Point", "소수점");
            rGrid.Set(ref m_sName, m_nID.ToString("FFU_0"), "Name", "Name");
            rGrid.Set(ref m_fHighLimit, m_nID.ToString("FFU_0"), "High Limit", "High Limit");
            rGrid.Set(ref m_fLowLimit, m_nID.ToString("FFU_0"), "Low Limit", "High Limit");
            rGrid.Set(ref m_nSVID, m_nID.ToString("FFU_0"), "SVID", "SVID");
        }

        public override string GetAlarmMessage()
        {
            string strMessage = "";
            if (m_sValue == null) return strMessage;
            try
            {
                switch (m_eDataType)
                {
                    case eDataType.RPM:
                    case eDataType.Pressure:
                    case eDataType.EFEMTemp:
                    case eDataType.VisionTemp:
                    case eDataType.EFEMHumidity:
                    case eDataType.VisionHumidity:
                        double dVal = Convert.ToDouble(m_sValue);
                        if (m_fHighLimit < dVal)
                        {
                            strMessage = m_sName + " Value Is Over The High Limit (Value : " + m_sValue + ", High Limit : " + m_fHighLimit.ToString();
                        }
                        else if (m_fLowLimit > dVal)
                        {
                            strMessage = m_sName + " Value Is Under The Low Limit(Value : " + m_sValue + ", Low Limit : " + m_fLowLimit.ToString();
                        }
                        break;
                    case eDataType.EFEMTempAlarm:
                    case eDataType.EFEMTempWarning:
                    case eDataType.EFEMTempHMCData:
                    case eDataType.VisionTempAlam:
                    case eDataType.VisionempWarning:
                    case eDataType.VisionTempHMCData:
                    case eDataType.State:
                        if (m_bStateAlarmMessage || m_bHCMDataAlarmMessage || m_bTempWarningMessage)
                        {
                            strMessage = m_sName+m_sValue;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
            }
            return strMessage;
        }
    }
}