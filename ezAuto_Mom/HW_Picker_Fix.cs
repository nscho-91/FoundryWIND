using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ezTools; 

namespace ezAutoMom
{
    public class HW_Picker_Fix : HW_Picker_Mom, Control_Child
    {
        enum eError { eVac };
        bool m_bRun, m_bVacOn;
        bool m_bWaitVacStop = true; 
        Thread m_thread;

        public HW_Picker_Fix()
        {
        }

        public void Init(string id, Auto_Mom auto)
        {
            base.Init(id, auto); m_control.Add(this); 
            m_bRun = m_bLoad = false; m_bVacOn = true;
            InitString();
            m_thread = new Thread(new ThreadStart(RunThread)); m_thread.Start();
        }

        public void ThreadStop()
        {
            if (m_bRun) { m_bRun = false; m_thread.Join(); }
        }

        void InitString()
        {
            InitString(eError.eVac, "Vacuum Fail !!");
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
            control.AddDI(rGrid, ref m_diVac[1], m_id, "Vac", "DI Picker Vac");
            control.AddDO(rGrid, ref m_doBlow, m_id, "Blow", "DO Picker Blow");
            control.AddDO(rGrid, ref m_doVac[0], m_id, "VacOff", "DO Picker VacOff");
            control.AddDO(rGrid, ref m_doVac[1], m_id, "VacOn", "DO Picker VacOn");
        }

        public override void RunGrid(eGrid eMode)
        {
            m_grid.Update(eMode, null);
            m_grid.Set(ref m_msVac, "Load", "Vac", "Vac on Delay (ms)");
            m_grid.Set(ref m_msEV, "Load", "EV", "EV Down Delay (ms)");
            m_grid.Set(ref m_nShake[0], "Load", "Shake", "# of Shake");
            m_grid.Set(ref m_msBlow, "UnLoad", "Blow", "Blow on Delay (ms)");
            m_grid.Set(ref m_nShake[1], "Unload", "Shake", "# of Shake");
            m_grid.Set(ref m_bVacOn, "Type", "diVac", "Vacuum Sensor State");
            m_grid.Refresh();
        }

        public override void RunReset()
        {
            if (IsVac() && (m_infoStrip != null)) return;
            RunInit();
        }

        public override void RunInit()
        {
            m_bLoad = false; m_infoStrip = null;
            if (!IsVac()) RunVac(false); 
        }

        public override bool LoadDown(int iVac)
        {
            //RunVac(true); //kns160726
            return false;
        }

        public override bool LoadUp(Info_Strip infoStrip, bool bUp = true)
        {
            return LoadEVUp(infoStrip, null); 
        }

        public override bool LoadEVUp(Info_Strip infoStrip, HW_LoadEV_Mom loadEV, bool bPaper = false)
        {
            bool bWaitVacStop = m_bWaitVacStop;
            if (loadEV != null) bWaitVacStop = false;
            RunVac(true); if (WaitVac(bWaitVacStop)) return true;
            if (loadEV != null) loadEV.Down(m_msEV);
            if (WaitVac(bWaitVacStop)) { if (loadEV != null) loadEV.LoadUp(); return true; }
            m_infoStrip = infoStrip; m_bLoad = true; m_bPaper = bPaper;
            if (loadEV != null) loadEV.LoadUp();
            return false;
        }

        public override bool UnloadDown(bool bShake)
        {
            m_bLoad = false;
            RunVac(false); return false;
        }

        public override bool UnloadUp(bool bUp = true)
        {
            return false;
        }

        bool WaitVac(bool bStop)
        {
            if (m_work.m_bDryRun) return false;
            if (!m_control.WaitInputBit(m_diVac[1], m_bVacOn)) { Thread.Sleep(m_msVac); return false; }
            if (bStop) SetAlarm(eAlarm.Stop, eError.eVac); 
            m_log.Popup("Wait Vacuum Error"); 
            return true;
        }

        public override void RunVac(bool bVac)
        {
            m_control.WriteOutputBit(m_doVac[1], bVac);
            m_control.WriteOutputBit(m_doVac[0], !bVac);
            if (!bVac) { RunBlow(true); Thread.Sleep(m_msBlow); RunBlow(false); }
        }

        void RunBlow(bool bBlow)
        {
            m_control.WriteOutputBit(m_doBlow, bBlow);
        }

        public override bool IsVac()
        {
            if (m_work.m_bDryRun) return true;
            return m_control.GetInputBit(m_diVac[1]) == m_bVacOn;
        }

        public override bool IsDown()
        {
            return false;
        }

        void RunThread()
        {
            m_bRun = true; Thread.Sleep(5000);
            while (m_bRun)
            {
                Thread.Sleep(10);
                if (m_bLoad && (IsVac() == false))
                {
                    m_bLoad = false;
                    SetAlarm(eAlarm.Stop, eError.eVac);
                }
            }
        }

    }
}
