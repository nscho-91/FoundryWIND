using System;
using System.Collections; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ezAxis;
using ezAutoMom;
using ezTools; 

namespace ezAuto_EFEM
{
    class HW_LoadPortMap
    {
        enum eError { ePusher, ePusherSafe, ePusherInterLock, eMapPusher, eMapInterLock, eAxis };

        public string m_id;
        public bool m_bHome = false;
        public bool m_bMapOK = false;
        Auto_Mom m_auto;
        Log m_log;
        Work_Mom m_work;
        Control_Mom m_control;
        XGem300_Mom m_xGem;
        int m_msOpen = 3000;
        int m_msMapping = 15000;
        int m_msHome = 30000;
        Info_Carrier m_infoCarrier;
        int m_iCarrier = 0;
        int m_zWaferGap = 0;
        int m_nThickRange = 50; 
        int m_zMapGap = 1000; 
        int[] m_doDoor = new int[2] { -1, -1 }; 
        int[] m_diDoor = new int[2] { -1, -1 };
        int m_diDoorOpenPos = -1; 
        int[] m_diDoorInterLock = new int[2] { -1, -1 };
        int m_diDoorSafe = -1;
        int[] m_doMapPusher = new int[2] { -1, -1 };
        int[] m_diMapPusher = new int[2] { -1, -1 };
        int[] m_diMapSafe = new int[(int)Wafer_Size.eSize.Empty];
        int[] m_diMapInterLock = new int[(int)Wafer_Size.eSize.Empty];
        int m_diMap = -1;
        int m_diProduct = -1;
        int m_vMap = 1000;
        MinMax[] m_zMap = new MinMax[(int)Wafer_Size.eSize.Empty];
        int[] m_zOpen = new int[2] { -1, -1 }; 
        Axis_Mom m_axis;
        HW_WTR_Mom m_wtr;
        Recipe_Mom m_recipe; 
        Queue m_queMap = new Queue();
        bool m_bRun = false;
        Thread m_thread;
        bool m_bRunProduct = false;
        Thread m_threadProduct;

        public HW_LoadPortMap()
        {
        }

        public void Init(string id, Auto_Mom auto, Info_Carrier infoCarrier, Log log)
        {
            int n;
            m_id = id; m_auto = auto; m_infoCarrier = infoCarrier; m_log = log;
            m_work = m_auto.ClassWork();
            m_control = m_auto.ClassControl();
            m_wtr = m_auto.ClassHandler().ClassWTR();
            m_xGem = m_auto.ClassXGem();
            m_recipe = m_auto.ClassRecipe();
            for (n = 0; n < (int)(Wafer_Size.eSize.Empty); n++)
            {
                m_diMapSafe[n] = -1;
                m_diMapInterLock[n] = -1; 
                m_zMap[n] = new MinMax(-1, -1, log);
            }
            InitString(); 
            m_thread = new Thread(new ThreadStart(RunThread)); m_thread.Start();
            m_threadProduct = new Thread(new ThreadStart(RunThreadProduct)); m_threadProduct.Start();
        }

        public void ThreadStop()
        {
            if (m_bRun) { m_bRun = false; m_thread.Join(); }
            if (m_bRunProduct) { m_bRunProduct = false; m_threadProduct.Join(); }
        }

        public void InitString()
        {
            InitString(eError.ePusher, "Door Pusher Timeout");
            InitString(eError.ePusherSafe, "Door Pusher Safe Sensor not Detected");
            InitString(eError.ePusherInterLock, "Door Pusher InterLock Sensor Detected");
            InitString(eError.eMapPusher, "Map Pusher Timeout");
            InitString(eError.eMapInterLock, "Map Pusher InterLock Sensor Detected");
            InitString(eError.eAxis, "LoadPort Axis Move Timeout");
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
            m_axis = control.AddAxis(rGrid, m_id, "Map", "Axis Mapping");
            control.AddDO(rGrid, ref m_doDoor[1], m_id, "Door_Open", "DO Door Open");
            control.AddDO(rGrid, ref m_doDoor[0], m_id, "Door_Close", "DO Door Close");
            control.AddDO(rGrid, ref m_doMapPusher[1], m_id, "Map_Pusher_On", "Map Pusher On");
            control.AddDO(rGrid, ref m_doMapPusher[0], m_id, "Map_Pusher_Off", "Map Pusher Off");
            control.AddDI(rGrid, ref m_diDoor[1], m_id, "Door_Open", "DI Door Open");
            control.AddDI(rGrid, ref m_diDoor[0], m_id, "Door_Close", "DI Door Close");
            control.AddDI(rGrid, ref m_diDoorOpenPos, m_id, "Door_OpenPos", "DI ODoor Open Position"); 
            control.AddDI(rGrid, ref m_diDoorSafe, m_id, "Door_Safe", "DI Pusher Safe");
            control.AddDI(rGrid, ref m_diDoorInterLock[0], m_id, "Door_InterLock0", "DI Pusher InterLock");
            control.AddDI(rGrid, ref m_diDoorInterLock[1], m_id, "Door_InterLock1", "DI Pusher InterLock");
            control.AddDI(rGrid, ref m_diMapPusher[1], m_id, "Map_Pusher_On", "DI Map InterLock");
            control.AddDI(rGrid, ref m_diMapPusher[0], m_id, "Map_Pusher_Off", "DI Map InterLock");
            for (int n = 0; n < (int)(Wafer_Size.eSize.Empty); n++) 
            {
                if (m_infoCarrier.m_wafer.m_bEnable[n])
                {
                    string strWafer = ((Wafer_Size.eSize)n).ToString(); 
                    control.AddDI(rGrid, ref m_diMapSafe[n], m_id, "Map_Safe_" + strWafer, "DI Carrier Sensor");
                    control.AddDI(rGrid, ref m_diMapInterLock[n], m_id, "Map_InterLock_" + strWafer, "DI Carrier Sensor");
                }
            } 
            control.AddDI(rGrid, ref m_diMap, m_id, "Map_Sensor", "DI Map Sensor");
            control.AddDI(rGrid, ref m_diProduct, m_id, "Product", "DI Product Sensor");
        }

        public void RunGrid(ezGrid rGrid, eGrid eMode)
        {
            rGrid.Set(ref m_msOpen, "Timeout", "Door", "Door Pusher Timeout (ms)");
            rGrid.Set(ref m_msHome, "Timeout", "Home", "Axis Home Timeout (ms)");
            rGrid.Set(ref m_msMapping, "Timeout", "Mapping", "Wafer Mapping Timeout (ms)");
            rGrid.Set(ref m_zOpen[0], "Door", "Close", "Axis Position (pulse)");
            rGrid.Set(ref m_zOpen[1], "Door", "Open", "Axis Position (pulse)");
            rGrid.Set(ref m_vMap, "Mapping", "Speed", "Axis Speed (pulse/Sec)");
            for (int n = 0; n < (int)(Wafer_Size.eSize.Empty); n++) 
            {
                bool bEnable = (m_infoCarrier.m_wafer.m_bEnable[n]) || (eMode != eGrid.eInit); 
                rGrid.Set(ref m_zMap[n], "Mapping", ((Wafer_Size.eSize)n).ToString(), "Axis Position (pulse)", false, bEnable); 
            }
            rGrid.Set(ref m_nThickRange, "Mapping", "Thickness", "Map Sensor Valid Check Thickness (%)");
            rGrid.Set(ref m_zMapGap, "Mapping", "MapGap", "Map Sensor Minimum Position Error (pulse)"); 
        }

        void RunThread()
        {
            bool bInterLock = false;
            m_bRun = true;
            Thread.Sleep(5000);
            while (m_bRun)
            {
                Thread.Sleep(1);
                if (!IsDoorInterLock()) bInterLock = false;
                else
                {
                    DoDoor(true);
                    if (!bInterLock) SetAlarm(eAlarm.Warning, eError.ePusherInterLock);
                    bInterLock = true;
                }
            }
        }

        void RunThreadProduct()
        {
            bool bPopup = false; 
            m_bRunProduct = true;
            Thread.Sleep(5000);
            while (m_bRunProduct)
            {
                Thread.Sleep(1);
                if (IsDoorAxisMove() && IsPruductCheck())
                {
                    m_axis.StopAxis(true); 
                    if (!bPopup)
                    {
                        m_log.Popup("Door Axis Stoped Cause Product Sensor Detect !!");
                        bPopup = true;
                    }
                }
                else
                {
                    bPopup = false; 
                }
            }
        }

        public bool RunDoor(bool bOpen)
        {
            bool bError; 
            ezStopWatch sw = new ezStopWatch();
            if (IsDoor(bOpen)) return false;
            if (!IsDoorSafe()) { SetAlarm(eAlarm.Warning, eError.ePusherSafe); return true; }
            if (!bOpen && IsMapPusher(true)) { m_log.Popup("Door Close Canceled cause Map Pusher On"); return false; }
            DoDoor(bOpen);
            while (!IsDoor(bOpen) && (sw.Check() <= m_msOpen)) Thread.Sleep(10);
            bError = (sw.Check() >= m_msOpen);
            if (bError) SetAlarm(eAlarm.Warning, eError.ePusher);
            if (bOpen) Thread.Sleep(500); 
            return bError;
        }

        void DoDoor(bool bOpen)
        {
            m_control.WriteOutputBit(m_doDoor[1], bOpen);
            m_control.WriteOutputBit(m_doDoor[0], !bOpen);
        }

        public bool IsDoor(bool bOpen)
        {
            if (m_control.GetInputBit(m_diDoor[1]) != bOpen) return false;
            if (m_control.GetInputBit(m_diDoor[0]) == bOpen) return false;
            return true;
        }

        bool IsDoorInterLock()
        {
            if (m_control.GetInputBit(m_diDoorInterLock[0])) return true;
            if (m_control.GetInputBit(m_diDoorInterLock[1])) return true;
            return false;
        }

        public bool IsDoorAxisMove()
        {
            if (m_axis == null) return false;
            return m_axis.IsAxisMove();
        }

        bool IsDoorSafe()
        {
            return m_control.GetInputBit(m_diDoorSafe);
        }

        public bool RunMapPusher(bool bOn)
        {
            bool bError;
            ezStopWatch sw = new ezStopWatch();
            if (IsMapPusher(bOn)) return false;
            if (bOn && IsDoor(false)) { m_log.Popup("Map Push On Canceled cause Door Closeed"); return false; }
            if (bOn && !IsMapSafe()) { SetAlarm(eAlarm.Warning, eError.eMapInterLock); return true; } 
            DoMapPusher(bOn);
            while (!IsMapPusher(bOn) && (sw.Check() <= m_msOpen)) Thread.Sleep(10);
            bError = (sw.Check() >= m_msOpen);
            if (bError) SetAlarm(eAlarm.Warning, eError.eMapPusher); 
            Thread.Sleep(1000);
            return bError; 
        }

        void DoMapPusher(bool bOn)
        {
            m_control.WriteOutputBit(m_doMapPusher[1], bOn);
            m_control.WriteOutputBit(m_doMapPusher[0], !bOn);
        }

        public bool IsMapPusher(bool bOn)
        {
            if (m_control.GetInputBit(m_diMapPusher[1]) != bOn) return false;
            if (m_control.GetInputBit(m_diMapPusher[0]) == bOn) return false;
            return true;
        }

        bool IsMapSafe()
        {
            if (m_diMapSafe[(int)m_infoCarrier.m_wafer.m_eSize] < 0) return true;
            return m_control.GetInputBit(m_diMapSafe[(int)m_infoCarrier.m_wafer.m_eSize]);
        }
        
        bool IsMapInterLock()
        {
            return m_control.GetInputBit(m_diMapInterLock[(int)m_infoCarrier.m_wafer.m_eSize]);
        }

        bool IsMapCheck()
        {
            return m_control.GetInputBit(m_diMap);
        }

        public bool IsPruductCheck()
        {
            return m_control.GetInputBit(m_diProduct);
        }

        public bool IsDoorOpenPos()
        {
            return m_control.GetInputBit(m_diDoorOpenPos); 
        }

        public eHWResult DoMapping()
        {
            ezStopWatch sw = new ezStopWatch();
            bool bMapOn; int nHigh, nLow;
            if (!IsMapPusher(false) && RunMapPusher(false))
            {
                m_log.Popup("Map Pusher Close Error");
                return eHWResult.Error;
            }
            m_iCarrier = (int)m_infoCarrier.m_wafer.m_eSize;
            m_zWaferGap = (m_zMap[m_iCarrier].Max - m_zMap[m_iCarrier].Min) / (m_infoCarrier.m_lWafer - 1);
            if (RunDoor(true)) return eHWResult.Error;
            m_axis.Move(m_zMap[m_iCarrier].Max + m_zWaferGap / 2);
            if (m_axis.WaitReady()) 
            { 
                SetAlarm(eAlarm.Warning, eError.eAxis); 
                return eHWResult.Error; 
            }
            if (RunMapPusher(true)) return eHWResult.Error;
            m_queMap.Clear();
            m_axis.MoveV(m_zMap[m_iCarrier].Min - m_zWaferGap / 2, m_vMap);
            sw.Start();
            bMapOn = m_control.GetInputBit(m_diMap);
            nHigh = (int)m_axis.GetPos(true);
            while ((m_axis.GetPos(true) > m_zMap[m_iCarrier].Min - m_zWaferGap / 2) && (sw.Check() <= m_msMapping))
            {
                Thread.Sleep(1); 
                if (bMapOn != m_control.GetInputBit(m_diMap))
                {
                    if (!bMapOn) nHigh = (int)m_axis.GetPos(true); 
                    else
                    {
                        nLow = (int)m_axis.GetPos(true);
                        MinMax minmax = new MinMax(nLow, nHigh, null); 
                        m_queMap.Enqueue(minmax); 
                    }
                    bMapOn = !bMapOn; 
                }
                if (IsMapInterLock())
                {
                    m_axis.StopAxis();
                    DoMapPusher(false);
                    SetAlarm(eAlarm.Warning, eError.eAxis);
                    m_log.Popup("Mapping Bar End Postion Check Sensor Detected");
                    return eHWResult.Error;
                }
            }
            if (sw.Check() >= m_msMapping) 
            {
                m_axis.StopAxis();
                DoMapPusher(false);
                SetAlarm(eAlarm.Warning, eError.eAxis); 
                return eHWResult.Error; 
            }
            bool bMapPushError = RunMapPusher(false);
            if (CalcMap() == eHWResult.Error) return eHWResult.Error;
            if (bMapPushError) return eHWResult.Error;
            return eHWResult.OK;
        }

        eHWResult CalcMap()
        {
            int iWafer, nThickSum = 0, nThickCount = 0, nThickAve = 0;
            eHWResult eResult = eHWResult.OK; 
            m_bMapOK = true;
            m_log.Add("Start Map");
            bool[] m_bExist = new bool[m_infoCarrier.m_lWafer];
            for (int n = 0; n < m_infoCarrier.m_lWafer; n++) m_bExist[n] = false;
            foreach (MinMax mm in m_queMap)
            {
                nThickSum += mm.Range();
                nThickCount++;
            }
            if (nThickCount > 1)
            {
                nThickAve = nThickSum / nThickCount;
            }
            foreach (MinMax mm in m_queMap)
            {
                iWafer = CalcMap(mm, nThickAve);
                if (iWafer >= 0)
                {
                    if (m_bExist[iWafer]) eResult = eHWResult.Error;
                    m_bExist[iWafer] = true;
                    nThickSum += mm.Range();
                    nThickCount++;
                }
                else m_bMapOK = false;
            }
            for (int n = 0; n < m_infoCarrier.m_lWafer; n++)
            {
                m_infoCarrier.m_infoWafer[n].SetExist(m_bExist[n]);
            }
            return eResult; 
        }

        int CalcMap(MinMax mm, int nThickAve)
        {
            int nThick = mm.Range();
            int nPos = mm.Center();
            int iWafer = (nPos - (m_zMap[m_iCarrier].Min - m_zWaferGap / 2)) / m_zWaferGap;
            int dGap = nPos - (iWafer * m_zWaferGap + m_zMap[m_iCarrier].Min);
            m_log.Add(iWafer.ToString("Index=00 ") + nThick.ToString("Width=00 Gap=") + dGap.ToString());
            if (Math.Abs(dGap) > m_zMapGap) 
            { 
                int nSlot = m_infoCarrier.m_infoWafer[iWafer].m_nSlot; 
                m_log.Popup(nSlot.ToString("Slot00") + " : Wafer Position Invalid !!"); 
                return -1; 
            }
            if (nThickAve > 0)
            {
                if (nThick < ((100 - m_nThickRange) * nThickAve / 100)) { m_log.Popup(" Error : Too Thin Wafer Detect !!"); return -1; }
                if (nThick > ((100 + m_nThickRange) * nThickAve / 100)) { m_log.Popup(" Error : Too Thick Wafer Detect !!"); return -1; }
            }
            return iWafer; 
        }

        public eHWResult DoDoorOpen()
        {
            if (RunDoor(true)) return eHWResult.Error;
            if (!IsMapPusher(false) && RunMapPusher(false))
            {
                m_log.Popup("Map Pusher Close Error");
                return eHWResult.Error;
            }
            m_axis.Move(m_zOpen[1]);
            if (m_axis.WaitReady()) 
            { 
                SetAlarm(eAlarm.Warning, eError.eAxis); 
                return eHWResult.Error; 
            }
            return eHWResult.OK;
        }

        public eHWResult DoDoorClose()
        {
            if (IsDoor(false)) return eHWResult.OK;
            
            if(m_control.GetInputBit(m_diProduct) == true)      //Interlock LP 2 Wafer 돌출 감지
            {
                m_log.Popup("Wafer Protrution Error. Please Check Wafer Placement In Cassete");
                return eHWResult.Error;
            }
            if (!IsMapPusher(false) && RunMapPusher(false))     // Mapping Bar Check
            {
                m_log.Popup("Map Pusher Close Error");
                return eHWResult.Error;
            }
            m_axis.Move(m_zOpen[0]);
            if (m_axis.WaitReady()) 
            { 
                SetAlarm(eAlarm.Warning, eError.eAxis); 
                return eHWResult.Error; 
            }
            if (RunDoor(false)) return eHWResult.Error;
            return eHWResult.OK;
        }

        public eHWResult RunHome()
        {
            ezStopWatch sw = new ezStopWatch();
            m_bHome = false;
            if (m_control.GetInputBit(m_diProduct) == true)      //Interlock LP 2 Wafer 돌출 감지
            {
                m_log.Popup("Wafer Protrution Error. Please Check Wafer Placement In Cassete");
                return eHWResult.Error;
            }
            if (!IsMapPusher(false) && RunMapPusher(false)) 
            { 
                m_log.Popup("Map Pusher Close Error"); 
                return eHWResult.Error; 
            }
            if (!IsDoor(true) && RunDoor(true)) 
            { 
                m_log.Popup("Pusher Open Error"); 
                return eHWResult.Error; 
            }
            m_axis.HomeSearch();
            while (!m_axis.IsReady() && sw.Check() <= m_msHome) 
            {  
                Thread.Sleep(10);
                if (m_control.GetInputBit(m_diProduct) == true)      //Interlock LP 2 Wafer 돌출 감지
                {
                    m_axis.StopAxis(true);
                    m_log.Popup("Wafer Protrution Error. Please Check Wafer Placement In Cassete");
                    return eHWResult.Error;
                }
            }
            if (!m_axis.IsReady()) 
            {
                m_log.Popup("Axis Home Time Out !");
                return eHWResult.Error; 
            }
            if (DoDoorClose() != eHWResult.OK)
            {
                m_log.Popup("DoorClose Time Out !");
                return eHWResult.Error;
            }
            m_log.Add("Find Home Finished"); 
            m_bHome = true; 
            return eHWResult.OK; 
        }

    }
}
