﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ezAutoMom; 
using ezTools; 

namespace ezAutoMom
{
    public partial class  HW_LoadEV : Form, HW_LoadEV_Mom
    {
        enum eError { eEmpty, eIonizer }; 
        enum eStat { eInit, eReady, eDone, eLoad, eDown };
        enum eMove { eMoveDown, eMoveStop, eMoveUp };

        string m_id;
        bool m_bLimit, m_bStopFinish, m_bCheckPaper, m_bPaperFull, m_bPaperBox, m_bIonizerOn; //forget
        bool m_bCheckDI = true; 
        bool m_bPaper = false;
        bool m_bAC = true;
        bool[] m_bRun = new bool[2];
        int m_msDown, m_msEnd, m_diPaperFull, m_diPaperBox, m_diIonizer;
        public int m_doBlow, m_doIonizer;   //박상영20171223 public으로 변경
        public int m_doUp, m_doDown, m_diUp, m_diDown, m_diTop, m_diCheck, m_diPaper; 
        eStat m_eStat;
        eMove m_eMove;
        public bool m_bFinish, m_bLock, m_bManual;
        Size[] m_sz = new Size[2];
        Log m_log;
        ezGrid m_grid;
        Auto_Mom m_auto;
        Control_Mom m_control;
        Work_Mom m_work;
        Handler_Mom m_handler;
        XGem300_Mom m_xGem; 
        ezStopWatch m_swDown = new ezStopWatch();
        Thread m_thread;
        Thread m_threadCheck;
        const int m_nThreadStartDelay = 7000;    //박상영20171222
        ezStopWatch m_swThreadStartDelay = new ezStopWatch();
        bool m_bThreadStartDelayEnd = false;
        bool m_bThreadStop = false;

        public bool m_bCheckview;   //박상영20180207

        public HW_LoadEV()
        {
            InitializeComponent();
            m_bRun[0] = m_bRun[1] = m_bLimit = m_bPaper = m_bFinish = m_bCheckPaper = m_bLock = false;
            m_bCheckDI = m_bStopFinish = m_bPaperFull = m_bPaperBox = m_bManual = m_bIonizerOn = false;
            m_msDown = 1000; m_msEnd = 3000;
            m_diUp = m_diDown = m_diTop = m_diCheck = m_diPaper = m_diPaperFull = m_diPaperBox = m_diIonizer = -1;
            m_doUp = m_doDown = m_doBlow = m_doIonizer = -1;
            m_handler = null; m_eMove = eMove.eMoveStop; m_eStat = eStat.eInit;
        }

        public void Init(string id, Auto_Mom auto, string sGroup = "Handler")
        {
            m_id = id; m_auto = auto; checkView.Text = m_id;
            m_sz[0] = m_sz[1] = this.Size; m_sz[0].Height = 26;
            m_log = new Log(m_id, m_auto.ClassLogView(), sGroup);
            m_grid = new ezGrid(m_id, grid, m_log, false);
            m_control = m_auto.ClassControl(); m_control.Add(this);
            m_work = m_auto.ClassWork();
            m_handler = m_auto.ClassHandler();
            m_xGem = m_auto.ClassXGem();
            RunGrid(eGrid.eRegRead); RunGrid(eGrid.eInit);
            InitString(); 
            m_thread = new Thread(new ThreadStart(RunThread)); m_thread.Start();
            if (m_diIonizer > -1)
            {
                m_threadCheck = new Thread(new ThreadStart(RunThreadCheck));
                m_threadCheck.Start();
            }
            m_eMove = eMove.eMoveStop; 
        }

        public void ThreadStop()
        {
            m_bThreadStop = true;   //박상영20171222
            while (!m_bThreadStartDelayEnd) { Thread.Sleep(100); }
            RunMove(eMove.eMoveStop); 
            if (m_bRun[0]) { m_bRun[0] = false; m_thread.Join(); }
            if (m_bRun[1]) { m_bRun[1] = false; m_threadCheck.Join(); }
        }

        void InitString()
        {
            InitString(eError.eEmpty, "Strip Empty !! ");
            InitString(eError.eIonizer, "Ionizer Error !!");
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

        void RunThread()
        {
    	    bool bUp, bDown; 
            m_bRun[0] = true;
            m_bThreadStop = false;  //박상영20171222 프로그램 실행 직후 종료시 Sleep Delay 없이 종료되도록 시도했으나, 다른 Thread Delay 남아있음
            m_bThreadStartDelayEnd = false;
            m_swThreadStartDelay.Start();            
            while (m_swThreadStartDelay.Check() < m_nThreadStartDelay)
            {
                if (m_bThreadStop) { m_bThreadStartDelayEnd = true; return; }
                Thread.Sleep(100);
            }
            m_bThreadStartDelayEnd = true;
            while (m_bRun[0])
	        {
		        Thread.Sleep(10);
                bUp = m_control.GetInputBit(m_diUp); bDown = m_control.GetInputBit(m_diDown);
		        if (m_bLimit) { if (!bUp && !bDown) { m_bLimit = false; RunMove(eMove.eMoveStop); m_eStat = eStat.eReady; } }
		        else
		        {
                    if (bUp) { m_bLimit = true; m_log.Popup("Up Sensor Detected !!"); RunMove(eMove.eMoveStop); Thread.Sleep(100); RunMove(eMove.eMoveDown); }
			        if (bDown) 
			        {
                        m_bLimit = true; m_log.Popup("Down Sensor Detected !!"); RunMove(eMove.eMoveStop); Thread.Sleep(100); RunMove(eMove.eMoveUp);
				        if ((m_swDown.Check() < m_msDown)) Thread.Sleep(m_msDown - (int)m_swDown.Check()); 
			        }
		        }
	        	if (!m_bLimit) switch (m_eStat)
	        	{
	        	case eStat.eInit: RunMove(eMove.eMoveStop); break;
                case eStat.eReady: if (m_control.GetInputBit(m_diTop)) RunMove(eMove.eMoveDown); else RunMove(eMove.eMoveStop); break;
                case eStat.eLoad: RunLoad(); break;
                case eStat.eDone: RunMove(eMove.eMoveStop); if (!(m_work.IsRun() || m_work.IsManual() || m_bManual)) m_eStat = eStat.eReady; break;
                case eStat.eDown: RunDown(); break;
	        	}
	        }
        }

        void RunThreadCheck()
        {
            m_bRun[1] = true; Thread.Sleep(m_nThreadStartDelay); //박상영20171222
            while (m_bRun[1])
            {
                Thread.Sleep(10);
                if (m_bIonizerOn)
                {
                    Thread.Sleep(1000);
                    if (!CheckIonizer())
                    {
                        SetAlarm(eAlarm.Stop, eError.eIonizer);
                    }

                }
            }
        }

        void RunMove(eMove eM)
        {
	        bool bUp = false, bDown = false;
	        if (m_eMove == eM) return;
	        if (Math.Abs(m_eMove - eM) == 2) RunMove(eMove.eMoveStop);
	        switch (eM)
	        {
	        case eMove.eMoveDown: bUp = false; bDown = true; break;
            case eMove.eMoveStop: bUp = false; bDown = false; break;
            case eMove.eMoveUp: 
                if (m_bAC) 
                { 
                    bUp = true; 
                    bDown = false; 
                }
                else 
                { 
                    bUp = true; 
                    bDown = true; 
                }
                    break;
        	}           
        	m_control.WriteOutputBit(m_doUp, bUp); m_control.WriteOutputBit(m_doDown, bDown);
	        m_eMove = eM; Thread.Sleep(100);
        }

        void RunLoad()
        {
	        if (m_control.GetInputBit(m_diTop)) 
	        {
		        RunMove(eMove.eMoveStop);
                if (IsCheck())
                {
                    m_eStat = eStat.eDone;
                    if (RunCheck()) m_eStat = eStat.eLoad; // ing
                    m_work.RunBuzzer(Work_Mom.eBuzzer.Home, false);
                }
		        if (!IsCheck() && (!m_work.m_bDryRun))
		        {
		        	m_bFinish = true;
                    Down(m_msDown); 
                    if (m_bStopFinish) SetAlarm(eAlarm.Stop, eError.eEmpty);
                    else SetAlarm(eAlarm.Popup, eError.eEmpty); //m_log.Popup("Strip Empty !! ");   //박상영20180125
                    
                    m_work.RunBuzzer(Work_Mom.eBuzzer.Home, true);
                    m_eStat = eStat.eReady; // iglee 20171130
	        	}
	        }
	        else { m_eStat = eStat.eLoad; RunMove(eMove.eMoveUp); }
        }

        void RunDown()
        {
            m_bManual = false;
            RunMove(eMove.eMoveDown); 
        }

        public void LoadUp()
        {
            m_eStat = eStat.eLoad;
        }

        public void Down(int msDown)
        {
            m_eStat = eStat.eDown; 
            m_swDown.Start();  
            m_msDown = msDown; 
        }

        public void EndDown()
        {
            m_eStat = eStat.eDown; Thread.Sleep(m_msEnd); Stop();
        }

        public void Stop()
        {
            m_bManual = false;
            m_eStat = eStat.eReady; 
        }

        bool RunCheck()
        {
            if (m_bLock) return true;
            if (!IsDone()) return true;
            m_bCheckPaper = m_control.GetInputBit(m_diPaper); return false;
        }

        public bool IsPaper()
        {
            return m_bCheckPaper;
        }
        
        public bool IsCheck()
        {
	        if (m_bPaper && !m_work.IsUsePaper(1)) return true; 
	        return m_bCheckDI == m_control.GetInputBit(m_diCheck);
        }

        public bool IsTop()
        {
            return m_control.GetInputBit(m_diCheck);
        }

        public bool CheckInitOK()
        {
            m_bFinish = false; RunMove(eMove.eMoveDown); Thread.Sleep(500); Stop(); return true;
        }

        public void RunInit()
        {
            LoadUp(); Thread.Sleep(1000); m_eStat = eStat.eReady; m_bFinish = false; Blow(false);
            m_bManual = false;
        }

        public void Blow(bool bBlow)
        {
            if (m_doIonizer > -1)
            {
                m_control.WriteOutputBit(m_doIonizer, bBlow);
                m_bIonizerOn = bBlow;
            }
            m_control.WriteOutputBit(m_doBlow, bBlow);
        }

        public bool CheckIonizer()
        {
            if (m_diIonizer > -1) return m_control.GetInputBit(m_diIonizer);
            return true;
        }

        public bool IsDone()
        {
            if (m_work.m_bDryRun) return true;
            return m_eStat == eStat.eDone;
        }

        public bool IsReady() // ing 171130
        {
            return m_eStat == eStat.eReady;
        }

        public void ControlGrid(Control_Mom control, ezGrid rGrid, eGrid eMode)
        {
            control.AddDI(rGrid, ref m_diUp, m_id, "Up", "DI Elevator Up");
            control.AddDI(rGrid, ref m_diDown, m_id, "Down", "DI Elevator Down");
            control.AddDI(rGrid, ref m_diTop, m_id, "Top", "DI Elevator Top");
            control.AddDI(rGrid, ref m_diCheck, m_id, "Check", "DI Elevator Check");
            control.AddDI(rGrid, ref m_diPaper, m_id, "Paper", "DI Elevator Paper");
            control.AddDI(rGrid, ref m_diPaperFull, m_id, "PaperFull", "DI Elevator Paper Full");
            control.AddDI(rGrid, ref m_diPaperBox, m_id, "PaperBox", "DI Elevator Paper Box");
            control.AddDI(rGrid, ref m_diIonizer, m_id, "IonizerCheck", "DI Elevator Ionizer");                  
            control.AddDO(rGrid, ref m_doUp, m_id, "Up", "DO Elevator Up");
            control.AddDO(rGrid, ref m_doDown, m_id, "down", "DO Elevator Down");
            control.AddDO(rGrid, ref m_doBlow, m_id, "Blow", "DO Elevator Blow");
            control.AddDO(rGrid, ref m_doIonizer, m_id, "Ionizer", "DO Elevator Ionizer");                  
        }

        public void RunGrid(eGrid eMode)
        {
            m_grid.Update(eMode, null);
            m_grid.Set(ref m_bAC, "Set", "AC_Motor", "true = AC, false = DC"); 
            m_grid.Set(ref m_bCheckDI, "Set", "CheckSensor", "Check Sensor contact"); 
            m_grid.Set(ref m_bPaper, "Set", "PaperEV", "Paper Elevator"); 
            m_grid.Set(ref m_msDown, "Down", "msDown", "Down Delay (ms)");
            m_grid.Set(ref m_msEnd, "EndDown", "msEnd", "End Down Delay (ms)"); 
            m_grid.Set(ref m_bPaperFull, "Paper", "PaperFull", "Paper Full Sensor State");
            m_grid.Set(ref m_bPaperBox, "Paper", "PaperBox", "Paper Box Sensor State");
            m_grid.Refresh();
        }

        private void checkView_CheckedChanged(object sender, EventArgs e)
        {
            m_handler.ShowChild();
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            m_bManual = true;
            LoadUp();
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            Down(1000);
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void buttonBlow_Click(object sender, EventArgs e)
        {
            Blow(true); Thread.Sleep(2000); Blow(false); 
        }

        private void propertyGridEx1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate); RunGrid(eGrid.eRegWrite);
        }

        public void ShowDlg(Form parent, ref CPoint cpShow)
        {
            int nIndex;
            this.TopLevel = false; this.Parent = parent; this.Location = cpShow.ToPoint();
            if (checkView.Checked) nIndex = 1; else nIndex = 0;
            this.Size = m_sz[nIndex]; cpShow.y += m_sz[nIndex].Height;
            Show();

            m_bCheckview = checkView.Checked;   //박상영20180207
        }

        public bool IsPaperBoxFull()
        {
            return m_control.GetInputBit(m_diPaperFull) == m_bPaperFull; 
        }

        public bool IsPaperBoxCheck()
        {
            return m_control.GetInputBit(m_diPaperBox) == m_bPaperBox; 
        }

        private void HW_LoadEV_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

    }
}
