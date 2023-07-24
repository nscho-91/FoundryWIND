using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ezAutoMom;
using ezTools; 

namespace ezAuto_EFEM
{
    class HW_LoadPortCover
    {
        enum eError { eOpen, eLock };
        public enum eLoadPort_Cover { eClose, eOpen, eError };

        public string m_id;
        Auto_Mom m_auto;
        Log m_log;
        Work_Mom m_work;
        Control_Mom m_control;
        XGem300_Mom m_xGem;
        int m_msOpen = 3000, m_msLock = 2000, m_msClose = 1000;
        int[] m_doOpen = new int[2] { -1, -1 };
        int[,] m_diOpen = new int[2, 2] { { -1, -1 }, { -1, -1 } };
        int[] m_doLock = new int[2] { -1, -1 };
        int[] m_diLock = new int[2] { -1, -1 };
        int[] m_doSW = new int[2] { -1, -1 };
        int[] m_diSW = new int[2] { -1, -1 };

        public HW_LoadPortCover()
        {
        }

        public void Init(string id, Auto_Mom auto, Log log)
        {
            m_id = id; m_auto = auto; m_log = log;
            m_work = m_auto.ClassWork();
            m_control = m_auto.ClassControl();
            m_xGem = m_auto.ClassXGem();
            InitString(); 
        }

        void InitString()
        {
            InitString(eError.eOpen, "Cover Open/Close Timeout");
            InitString(eError.eLock, "Cover Lock/Unlock Timeout");
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
            control.AddDO(rGrid, ref m_doSW[1], m_id, "SW_Open", "DO Open Switch");
            control.AddDO(rGrid, ref m_doSW[0], m_id, "SW_Close", "DO Close Switch");
            control.AddDO(rGrid, ref m_doOpen[1], m_id, "Open", "DO Door Open");
            control.AddDO(rGrid, ref m_doOpen[0], m_id, "Close", "DO Door Close");
            control.AddDO(rGrid, ref m_doLock[1], m_id, "Lock", "DO Door Loack");
            control.AddDO(rGrid, ref m_doLock[0], m_id, "Unlock", "DO Door Unloack");
            control.AddDI(rGrid, ref m_diSW[1], m_id, "SW_Open", "DI Open Switch");
            control.AddDI(rGrid, ref m_diSW[0], m_id, "SW_Close", "DI Close Switch");
            control.AddDI(rGrid, ref m_diOpen[0, 1], m_id, "Open0", "DI Door Open0");
            control.AddDI(rGrid, ref m_diOpen[1, 1], m_id, "Open1", "DI Door Open1");
            control.AddDI(rGrid, ref m_diOpen[0, 0], m_id, "Close0", "DI Door Close0");
            control.AddDI(rGrid, ref m_diOpen[1, 0], m_id, "Close1", "DI Door Close1");
            control.AddDI(rGrid, ref m_diLock[1], m_id, "Lock", "DI Door Lock");
            control.AddDI(rGrid, ref m_diLock[0], m_id, "Unlock", "DI Door Unloack");
        }

        public void RunGrid(ezGrid rGrid)
        {
            rGrid.Set(ref m_msOpen, "Timeout", "Open", "Cover Open/Close Timeout (ms)");
            rGrid.Set(ref m_msLock, "Timeout", "Lock", "Cover Lock/Unlock Timeout (ms)");
            rGrid.Set(ref m_msClose, "Delay", "Close", "Cover Close Lock Delay (ms)");
        }

        public bool CoverOpen()
        {
            if (IsOpen(true) && IsLock(false)) return false;
            if (IsLock(true)) if (RunLock(false)) return true;
            if (RunOpen(true)) return true;
            return false;
        }

        public bool CoverClose()
        {
            if (IsOpen(false) && IsLock(true)) return false;
            if (IsLock(true)) if (RunLock(false)) return true;
            if (IsOpen(true)) if (RunOpen(false)) return true;
            if (RunLock(true)) return true;
            return false;
        }

        bool RunOpen(bool bOpen)
        {
            ezStopWatch sw = new ezStopWatch();
            m_control.WriteOutputBit(m_doOpen[1], bOpen);
            m_control.WriteOutputBit(m_doOpen[0], !bOpen);
            while (!IsOpen(bOpen) && (sw.Check() <= m_msOpen)) Thread.Sleep(10);
            if (sw.Check() <= m_msOpen)
            {
                if (!bOpen) Thread.Sleep(m_msClose); 
                return false;
            }
            SetAlarm(eAlarm.Warning, eError.eOpen); return true;
        }

        bool RunLock(bool bLock)
        {
            ezStopWatch sw = new ezStopWatch();
            m_control.WriteOutputBit(m_doLock[1], bLock);
            m_control.WriteOutputBit(m_doLock[0], !bLock);
            while (!IsLock(bLock) && (sw.Check() <= m_msLock)) Thread.Sleep(10);
            if (sw.Check() <= m_msLock) return false;
            SetAlarm(eAlarm.Warning, eError.eLock); return true;
        }

        public bool IsOpen(bool bOpen)
        {
            if (m_control.GetInputBit(m_diOpen[0, 1]) != bOpen) return false;
            if (m_control.GetInputBit(m_diOpen[1, 1]) != bOpen) return false;
            if (m_control.GetInputBit(m_diOpen[0, 0]) == bOpen) return false;
            if (m_control.GetInputBit(m_diOpen[1, 0]) == bOpen) return false;
            return true;
        }

        public bool IsLock(bool bLock)
        {
            if (m_control.GetInputBit(m_diLock[1]) != bLock) return false;
            if (m_control.GetInputBit(m_diLock[0]) == bLock) return false;
            return true;
        }

        public eLoadPort_Cover GetStat()
        {
            if (IsOpen(true) && IsLock(false)) return eLoadPort_Cover.eOpen;
            if (IsOpen(false) && IsLock(true)) return eLoadPort_Cover.eClose;
            return eLoadPort_Cover.eError;
        }

        public eHWResult CheckSW(bool bOpen)
        {
            int nDI;
            if (bOpen) nDI = 1; else nDI = 0;
            if (!m_control.GetInputBit(m_diSW[nDI])) return eHWResult.Error;
            Thread.Sleep(10); 
            if (m_control.GetInputBit(m_diSW[nDI])) return eHWResult.OK;
            return eHWResult.Error;
        }

        public void RunLED(bool bOpen, bool bOn)
        {
            int nIndex;
            if (bOpen) nIndex = 1; else nIndex = 0;
            m_control.WriteOutputBit(m_doSW[nIndex], bOn && IsOpen(!bOpen)); 
        }

    }
}
