using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ezAutoMom;
using ezTools; 

namespace ezAuto_EFEM
{
    public class HW_LoadPortCarrier
    {
        public string m_id;
        Auto_Mom m_auto;
        Log m_log;
        Work_Mom m_work;
        Control_Mom m_control;
        XGem300_Mom m_xGem;
        Info_Carrier m_infoCarrier;

        int[] m_doCarrier = new int[(int)Wafer_Size.eSize.Empty];
        int m_doPresent = -1;
        int[,] m_diCarrier = new int[(int)Wafer_Size.eSize.Empty, 2];
        bool m_bCarrier = true; 

        public HW_LoadPortCarrier()
        {
        }

        public void Init(string id, Auto_Mom auto, Info_Carrier infoCarrier, Log log) 
        {
            m_id = id; m_auto = auto; m_infoCarrier = infoCarrier; m_log = log;
            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++)
            {
                m_doCarrier[n] = -1;
                m_diCarrier[n, 0] = -1;
                m_diCarrier[n, 1] = -1; 
            }
            m_work = m_auto.ClassWork();
            m_control = m_auto.ClassControl();
            m_xGem = m_auto.ClassXGem();
        }

        public void ControlGrid(Control_Mom control, ezGrid rGrid, eGrid eMode)
        {
            int n;
            control.AddDO(rGrid, ref m_doPresent, m_id, "LED_Present", "DO LED Display");
            for (n = 0; n < (int)(Wafer_Size.eSize.Empty); n++) if (m_infoCarrier.m_wafer.m_bEnable[n]) 
            {
                control.AddDO(rGrid, ref m_doCarrier[n], m_id, "LED_" + ((Wafer_Size.eSize)n).ToString(), "DO LED Display"); 
            }
            for (n = 0; n < (int)(Wafer_Size.eSize.Empty); n++) if (m_infoCarrier.m_wafer.m_bEnable[n])
            {                
                control.AddDI(rGrid, ref m_diCarrier[n, 0], m_id, "Carrier_" + ((Wafer_Size.eSize)n).ToString() + "_0", "DI Carrier Sensor"); 
                control.AddDI(rGrid, ref m_diCarrier[n, 1], m_id, "Carrier_" + ((Wafer_Size.eSize)n).ToString() + "_1", "DI Carrier Sensor");
            }
        }

        public void RunGrid(ezGrid rGrid)
        {
            rGrid.Set(ref m_bCarrier, "Setup", "Carrier", "Carrier DI");
        }

        public Wafer_Size.eSize CheckCarrier()
        {
            int[] nStat = new int[2] { 0, 0 };
            int n, m = 1;
            for (n = 0; n < (int)(Wafer_Size.eSize.Empty); n++, m *= 2)
            {
                if (m_control.GetInputBit(m_diCarrier[n, 0]) == m_bCarrier) nStat[0] += m;
                if (m_control.GetInputBit(m_diCarrier[n, 1]) == m_bCarrier) nStat[1] += m;
            }
            if (nStat[0] != nStat[1]) return Wafer_Size.eSize.Error;
            switch (nStat[0])
            {
                case 0: return Wafer_Size.eSize.Empty;
                case 1: return Wafer_Size.eSize.inch4;
                case 2: return Wafer_Size.eSize.inch5;
                case 4: return Wafer_Size.eSize.inch6;
                case 8: return Wafer_Size.eSize.mm200;
                case 16: return Wafer_Size.eSize.mm300;
                default: return Wafer_Size.eSize.Error;
            }
        }

        public void RunLED(bool bOn)
        {
            int n; bool bPresent = false;
            m_infoCarrier.SetWaferSize(CheckCarrier());
            for (n = 0; n < (int)(Wafer_Size.eSize.Empty); n++)
            {
                m_control.WriteOutputBit(m_doCarrier[n], bOn && (n == (int)m_infoCarrier.m_wafer.m_eSize));
                if (n == (int)m_infoCarrier.m_wafer.m_eSize) bPresent = true;
            }
            m_control.WriteOutputBit(m_doPresent, bOn && bPresent);
        }

        public bool IsReady()
        {
            return m_infoCarrier.m_wafer.m_eSize <= Wafer_Size.eSize.Empty;
        }

    }
}
