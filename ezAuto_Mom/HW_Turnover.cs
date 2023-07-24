using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ezTools;
using ezAxis;

namespace ezAutoMom
{
    public class HW_Turnover : HW_Turnover_Mom, Control_Child
    {
        enum eError { eRotate0, eRotate1, eUp, eCheck};
        const int c_msHome = 30000;

        public bool m_bHome = false; 
        bool m_bInv, m_bCheckHome;
        bool[] m_bLock = new bool[2];
        int[] m_doVac = new int[2] { -1, -1 };
        int[] m_doBlow = new int[2] { -1, -1 };
        int[] m_posTurn = new int[2];
        int[] m_nAxisPos = new int[2];
        int m_diUp = -1, m_diDown = -1, m_diCheck = -1, m_doUp = -1, m_msBlow = 50, m_doCenBlow = -1;     
        bool m_bCheckUse, m_bCheckSensor;
        double[] m_fWidth = new double[2];
        Axis_Mom[] m_axis = new Axis_Mom[2];

        public HW_Turnover()
        {
            
        }

        public void Init(string id, Auto_Mom auto)
        {
            base.Init(id, auto); m_control.Add(this); 
            InitString(); RunGrid(eGrid.eRegRead); RunGrid(eGrid.eInit); 
            m_thread = new Thread(new ThreadStart(RunThread)); m_thread.Start();
        }

        public void ThreadStop()
        {
            if (m_bRun) { m_bRun = false; m_thread.Join(); }
        }

        void InitString()
        {
            InitString(eError.eRotate0, "Rotate0 Error !!");
            InitString(eError.eRotate1, "Rotate1 Error !!");
            InitString(eError.eUp, "Up Error !!");
            InitString(eError.eCheck, "Strip Check Error !!");
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
            m_axis[0] = control.AddAxis(rGrid, m_id, "Turn", "Axis ID");
            m_axis[1] = control.AddAxis(rGrid, m_id, "TurnRail", "Axis ID");
            control.AddDI(rGrid, ref m_diUp, m_id, "Up", "DI Turnover Up");
            control.AddDI(rGrid, ref m_diDown, m_id, "Down", "DI Turnover Up");
            control.AddDI(rGrid, ref m_diCheck, m_id, "Check", "ID Turnover Rail Check"); 
            control.AddDO(rGrid, ref m_doUp, m_id, "Up", "DO Turnover Up");
            control.AddDO(rGrid, ref m_doVac[0], m_id, "Vac0", "DO Turnover Vac0");
            control.AddDO(rGrid, ref m_doVac[1], m_id, "Vac1", "DO Turnover Vac1");
            control.AddDO(rGrid, ref m_doBlow[0], m_id, "Blow0", "DO Turnover Blow0");
            control.AddDO(rGrid, ref m_doBlow[1], m_id, "Blow1", "DO Turnover Blow1");
            control.AddDO(rGrid, ref m_doCenBlow, m_id, "CenterBlow", "DO Turnover CenterBlow");                  
        }

        public override void RunGrid(eGrid eMode)
        {
            m_grid.Update(eMode, null);
            m_grid.Set(ref m_bInv, m_id, "Inverse", "Turn Inverse Direction");
            m_grid.Set(ref m_posTurn[0], m_id, "Turn0", "Rotate Position (pulse)");
            m_grid.Set(ref m_posTurn[1], m_id, "Turn1", "Rotate Position (pulse)");
            m_grid.Set(ref m_fSlowV, m_id, "SlowV", "Door Open Speed (pulse/sec)");
            m_grid.Set(ref m_msBlow, m_id, "Blow0", "Blow Time (ms)");
            m_grid.Set(ref m_bCheckHome, m_id, "Home", "Check Home Sensor");

            m_grid.Set(ref m_nAxisPos[0], "Width0", "Axis", "Axis Position (pulse)");
            m_grid.Set(ref m_fWidth[0], "Width0", "Width", "Rail Width (mm)");
            m_grid.Set(ref m_nAxisPos[1], "Width1", "Axis", "Axis Position (pulse)");
            m_grid.Set(ref m_fWidth[1], "Width1", "Width", "Rail Width (mm)");
            m_grid.Set(ref m_bCheckUse, "Setup", "Check", "Use Check Sensor");
            m_grid.Set(ref m_bCheckSensor, "Setup", "Sensor", "CStrip Check Sensor");
            m_grid.Refresh();
        }

        public void RunThread()
        {
            m_bRun = true; Thread.Sleep(5000);
            while (m_bRun)
            {
                Thread.Sleep(1);
                if (m_work.IsRun() || m_work.IsManual()) TurnThread(); 
            }
        }

        public override bool RunTurn()
        {
            bool bTurn;
            if (!m_bInv)
            {
                m_infoStrip[0] = new Info_Strip();
                m_infoStrip[1] = null;
            }
            else
            {
                m_infoStrip[0] = null;
                m_infoStrip[1] = new Info_Strip();
            }
            bTurn = TurnThread();
            return bTurn; 
        }

        public bool RunReset()
        {
            if (IsCheck() || (m_infoStrip[1] == null)) return false;
            RunUp(false); 
            RunVac(1, false); 
            RunBlow(1, false); 
            m_infoStrip[1] = null; 
            return false;
        }

        public override bool RunInit()
        {
            if (!m_work.m_bDryRun && IsCheck())
            {
                SetAlarm(eAlarm.Stop, eError.eCheck);
                return base.RunInit();
            }
            ChangeWidth(); 
            if (m_axis[0] == null) return true;
            m_axis[0].Move(m_posTurn[0]); 
            RunUp(false); 
            RunVac(0, false); 
            RunVac(1, false); 
            RunBlow(0, false); 
            RunBlow(1, false);
            m_bTurn = false; 
            m_infoStrip[0] = null; 
            m_infoStrip[1] = null;
            if (m_axis[1] != null) m_axis[1].Move(m_posTurn[0]); 
            return false;
        }

        void ChangeWidth()
        {
            double fWidth, fAxisPos;
            fWidth = m_work.m_fStripCX[0];
            fAxisPos = m_nAxisPos[0] + (fWidth - m_fWidth[0]) * (m_nAxisPos[1] - m_nAxisPos[0]) / (m_fWidth[1] - m_fWidth[0]);
            if (m_axis[1] != null) m_axis[1].Move(fAxisPos);
        }

        bool TurnThread()
        {
            int nSide;
            if (m_work.m_eBoat == eBoatType.eSingle) return false; 
            m_sw.Start(); if (m_bInv) nSide = 1; else nSide = 0; 
            if (IsLock()) return false;
            if ((m_infoStrip[nSide] == null) || (m_infoStrip[1 - nSide] != null)) return true; 
            m_bTurn = true;
            RunUp(false); 
            RunVac(nSide, true); 
            RunVac(1 - nSide, false);
            if (m_work.IsSlowMove()) m_axis[0].MoveV(m_posTurn[1], m_fSlowV); 
            else m_axis[0].Move(m_posTurn[1]);  
            if (m_axis[0].WaitReady()) { SetAlarm(eAlarm.Error, eError.eRotate0); return true; }
            RunUp(true); 
            if (m_control.WaitInputBit(m_diUp, true)) { SetAlarm(eAlarm.Error, eError.eUp); return true; }
            RunVac(nSide, false); 
            RunVac(1 - nSide, true);
            RunBlow(nSide, true); 
            Thread.Sleep(m_msBlow);
            if (m_work.IsSlowMove()) m_axis[0].MoveV(m_posTurn[0], m_fSlowV); 
            else m_axis[0].Move(m_posTurn[0]);  
            if (m_axis[0].WaitReady()) { SetAlarm(eAlarm.Error, eError.eRotate1); return true; }
            RunUp(false); 
            RunBlow(nSide, false);
            m_log.Add(m_id + " : " + m_sw.Getms(true).ToString() + " ms");
            m_infoStrip[1 - nSide] = m_infoStrip[nSide];
            m_infoStrip[nSide] = null; 
            m_bTurn = false; 
            return false; 
        }

        public void RunUp(bool bUp)
        {
            m_control.WriteOutputBit(m_doUp, bUp);
        }

        public override void SetLock(int nID, bool bLock) 
        {
            m_bLock[nID] = bLock;
        }

        public override bool Rotate(int nIndex, bool bWait) 
        {
            m_axis[0].Move(m_posTurn[nIndex % 2]);
            if (bWait) return m_axis[0].WaitReady(); else return false;
        }

        public override void RunVac(int nSide, bool bVac) 
        {
            m_control.WriteOutputBit(m_doVac[nSide], bVac);
        }

        public override void RunBlow(int nSide, bool bBlow)
        {
            m_control.WriteOutputBit(m_doBlow[nSide], bBlow);
            if (m_bInv) m_control.WriteOutputBit(m_doCenBlow, bBlow);
        }

        public override bool CheckInitHome() 
        {
            m_axis[0].ServoOn(false);
            if (m_axis[1] != null) m_axis[1].ServoOn(false); 
            Thread.Sleep(1000);
            if (IsCheck()) { m_log.Popup("Check Strip Turnover_Rail"); return true; }
            if (m_axis[0].IsHome() == m_bCheckHome) return false;
            m_log.Popup("Check Turnover"); return true;
        }

        public override bool IsLock() 
        {
            return (m_bLock[0] || m_bLock[1]);
        }

        public override bool IsCheck() 
        {
            if (!m_bCheckUse) return false;
            return m_bCheckSensor == m_control.GetInputBit(m_diCheck);
        }
        
        public bool IsReadyPos()
        {
	        if ((m_axis[0].GetPos(true) < m_posTurn[0] + 100)) return true;
	        return false; 
        }

        public void RunThreadHome()
        {
            ezStopWatch sw = new ezStopWatch();
            m_bHome = false;
            m_axis[0].HomeSearch();
            while (!m_axis[0].IsReady() && sw.Check() <= c_msHome) Thread.Sleep(10);
            if (m_axis[0].IsReady()) { m_log.Add("Find Home Finished"); m_bHome = true; }
            else m_log.Popup("Axis Home Time Out !");
        }
    }
}
