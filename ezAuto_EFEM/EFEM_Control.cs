using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;
using ezAxis;
using ezAutoMom;
using ezTools; 

namespace ezAuto_EFEM
{
    class EFEM_Control : Control_Mom
    {
        int m_nDI = 144;
        int m_nDO = 144;
        int m_nDIO = 3;

        public Axiss_Ajin m_axiss = new Axiss_Ajin();
        public DIO_Ajin[] m_dio;

        public EFEM_Control()
        {
        }

        public override void Init(string id, int nDIO, LogView logView)
        {
            int n;
            m_nDIO = nDIO; 
            m_nDI = 48 * m_nDIO; 
            m_nDO = 48 * m_nDIO;
            base.Init(id, nDIO, logView);
            m_axiss.Init("Axis", logView);
            m_dio = new DIO_Ajin[nDIO];
            for (n = 0; n < m_nDIO; n++) m_dio[n] = new DIO_Ajin(n.ToString("DIO_0"), n, logView);

        }

        public void ThreadStop()
        {
            int n;
            m_axiss.ThreadStop();
            for (n = 0; n < m_nDIO; n++) m_dio[n].ThreadStop(); 
            m_axiss.CloseLib(); 
        }

        public IDockContent GetContentFromPersistString(string persistString)
        {
            int n; 
            if (persistString == typeof(EFEM_Control).ToString()) return this;
            else if (persistString == m_axiss.GetType().ToString()) return m_axiss;
            else
            {
                for (n = 0; n < m_nDIO; n++) if (m_dio[n].IsPersistString(persistString)) return m_dio[n]; 
            }
            return null;
        }

        public void ShowAll(DockPanel dockPanel)
        {
            int n; 
            this.Show(dockPanel); 
            m_axiss.Show(dockPanel);
            for (n = 0; n < m_nDIO; n++) m_dio[n].Show(dockPanel); 
        }

        public void RunLogIn(bool bEnable)
        {
            int n;
            this.Enabled = bEnable;
            m_axiss.Enabled = bEnable;
            for (n = 0; n < m_nDIO; n++) m_dio[n].Enabled = bEnable;
        }

        public override bool GetInputBit(int n)
        {
            if ((n < 0) || (n >= m_nDI)) return false;
            return m_dio[n / 48].GetInputBit(n);
        }

        public override bool GetOutputBit(int n)
        {
            if ((n < 0) || (n >= m_nDO)) return false;
            return m_dio[n / 48].GetOutputBit(n);
        }

        public override void WriteOutputBit(int n, bool bOn)
        {
            if ((n < 0) || (n >= m_nDO)) return;
            m_dio[n / 48].WriteOutputBit(n, bOn);
        }

        public override void RunEmergency()
        {
            m_axiss.RunEmergency();
        }

        public override void SetDICaption(int nDI, string str)
        {
            if ((nDI < 0) || (nDI >= m_nDI)) return;
            m_dio[nDI / 48].SetDICaption(nDI, str);
        }

        public override void SetDOCaption(int nDO, string str)
        {
            if ((nDO < 0) || (nDO >= m_nDO)) return;
            m_dio[nDO / 48].SetDOCaption(nDO, str);
        }

        public override void SetGantry(int nMaster, int nSlave)
        {
            base.SetGantry(nMaster, nSlave);
        }

        public override Axis_Mom GetAxis(ezAxis.Type type, string strID)
        {
            return m_axiss.GetAxis(type, strID);
        }

    }
}
