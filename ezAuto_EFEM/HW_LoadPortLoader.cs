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
    class HW_LoadPortLoader
    {
        enum eError { eLoad, eUnload };

        public string m_id;
        Auto_Mom m_auto;
        Log m_log;
        Work_Mom m_work;
        Control_Mom m_control;
        XGem300_Mom m_xGem;
        int m_msLoad = 3000;
        int[] m_doSW = new int[2] { -1, -1 };
        int[] m_diSW = new int[2] { -1, -1 };
        int[] m_doLED = new int[2] { -1, -1 };
        int[] m_doLoad = new int[2] { -1, -1 };
        int[] m_diLoad = new int[2] { -1, -1 };

        public HW_LoadPortLoader()
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
            InitString(eError.eLoad, "Load Carrier Timeout");
            InitString(eError.eUnload, "Unload Carrier Timeout");
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
            control.AddDO(rGrid, ref m_doLoad[1], m_id, "Load", "DO Load");
            control.AddDO(rGrid, ref m_doLoad[0], m_id, "Unload", "DO Unload");
            control.AddDO(rGrid, ref m_doSW[1], m_id, "SW_Load", "DO Load Switch");
            control.AddDO(rGrid, ref m_doSW[0], m_id, "SW_Unload", "DO Unoad Switch");
            control.AddDO(rGrid, ref m_doLED[1], m_id, "LED_Load", "DO Load LED");
            control.AddDO(rGrid, ref m_doLED[0], m_id, "LED_Unload", "DO Unoad LED");
            control.AddDI(rGrid, ref m_diLoad[1], m_id, "Load", "DI Load");
            control.AddDI(rGrid, ref m_diLoad[0], m_id, "Unload", "DI Unload");
            control.AddDI(rGrid, ref m_diSW[1], m_id, "SW_Load", "DI Load Switch");
            control.AddDI(rGrid, ref m_diSW[0], m_id, "SW_Unload", "DI Unoad Switch");         
        }

        public void RunGrid(ezGrid rGrid)
        {
            rGrid.Set(ref m_msLoad, "Timeout", "Load", "Carrier Load Timeout (ms)");
        }

        public eHWResult RunLoad(bool bLoad)
        {
            ezStopWatch sw = new ezStopWatch();
            if (IsLoad(bLoad)) return eHWResult.OK;
            RunLoadLED(bLoad, true);
            m_control.WriteOutputBit(m_doLoad[1], bLoad);
            m_control.WriteOutputBit(m_doLoad[0], !bLoad);
            while (!IsLoad(bLoad) && (sw.Check() <= m_msLoad)) Thread.Sleep(10);
            if (sw.Check() <= m_msLoad)
            {
                RunLoadLED(bLoad, false);
                return eHWResult.OK;
            }
            if (bLoad) SetAlarm(eAlarm.Warning, eError.eLoad);
            else SetAlarm(eAlarm.Warning, eError.eUnload);
            RunLoadLED(bLoad, false);
            return eHWResult.Error;
        }

        public bool IsLoad(bool bLoad)
        {
            if (m_control.GetInputBit(m_diLoad[1]) != bLoad) return false;
            if (m_control.GetInputBit(m_diLoad[0]) == bLoad) return false;
            return true;
        }

        public eHWResult CheckSW(bool bLoad)
        {
            int nDI;
            if (bLoad) nDI = 1; else nDI = 0;
            if (!m_control.GetInputBit(m_diSW[nDI])) return eHWResult.Error;
            Thread.Sleep(10);
            if (m_control.GetInputBit(m_diSW[nDI])) return eHWResult.OK;
            return eHWResult.Error;
        }

        void RunLoadLED(bool bLoad, bool bOn)
        {
            int n;
            if (bLoad) n = 1; else n = 0;
            m_control.WriteOutputBit(m_doLED[n], bOn);
        }

        public void RunLED(bool bLoad, bool bOn)
        {
            int nIndex;
            if (bLoad) nIndex = 1; else nIndex = 0;
            m_control.WriteOutputBit(m_doSW[nIndex], bOn && IsLoad(!bLoad));
        }

    }
}
