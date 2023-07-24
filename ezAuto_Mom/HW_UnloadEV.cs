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
using ezAutoMom; 
using ezTools; 

namespace ezAutoMom
{
    public partial class HW_UnloadEV : Form, Control_Child
    {
        enum eError 
        { 
            Top,
            TopOff,
            Up,
            Down
        };

        enum eState
        { 
            Init, 
            Ready, 
            Unload, 
            Down, 
            Full
        };
        eState m_eState = eState.Init;

        enum eMove 
        { 
            Down, 
            Stop, 
            Up 
        };
        eMove m_eMove = eMove.Stop;

        string m_id;
        
        bool m_bLimit = false; 

        public int m_doUp = -1;
        public int m_doDown = -1;
        public int m_doBlow = -1;   //박상영20171223
        public int m_diUp = -1;
        public int m_diDown = -1;
        public int m_diCheck = -1;
        public int m_diTop = -1;

        Size[] m_sz = new Size[2];
        Log m_log;
        ezGrid m_grid;
        Auto_Mom m_auto;
        Control_Mom m_control;
        Work_Mom m_work;
        Handler_Mom m_handler;
        XGem300_Mom m_xGem;

        bool m_bRun = false;
        Thread m_thread;

        int m_msUnloadDown = 1000; 

        const int m_nThreadStartDelay = 7000;    //박상영20171222
        ezStopWatch m_swThreadStartDelay = new ezStopWatch();
        bool m_bThreadStartDelayEnd = false;
        bool m_bThreadStop = false;

        public bool m_bCheckview;   //박상영20180207

        public HW_UnloadEV()
        {
            InitializeComponent();
        }

        public void Init(string id, Auto_Mom auto, string sGroup = "Handler")
        {
            m_id = id; 
            checkView.Text = m_id;
            m_auto = auto;
            m_sz[0] = m_sz[1] = this.Size; 
            m_sz[0].Height = 26;
            m_log = new Log(m_id, m_auto.ClassLogView(), sGroup);
            m_grid = new ezGrid(m_id, grid, m_log, false);
            m_control = m_auto.ClassControl(); m_control.Add(this);
            m_work = m_auto.ClassWork();
            m_handler = m_auto.ClassHandler();
            m_xGem = m_auto.ClassXGem();
            RunGrid(eGrid.eRegRead);
            RunGrid(eGrid.eInit);
            InitString();
            m_thread = new Thread(new ThreadStart(RunThread)); 
            m_thread.Start();
        }

        public void ThreadStop()
        {
            m_bThreadStop = true;   //박상영20171222
            while (!m_bThreadStartDelayEnd) { Thread.Sleep(100); }
            if (m_bRun) { m_bRun = false; m_thread.Join(); }
        }

        void InitString()
        {
            InitString(eError.Top, "Top Sensor Check Time Over !!");
            InitString(eError.TopOff, "Top Sensor Off Time Over !!");
            InitString(eError.Up, "Up Sensor Check !!");
            InitString(eError.Down, "Down Sensor Check Time Over !!");
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

        public void ShowDlg(Form parent, ref CPoint cpShow)
        {
            int nIndex;
            this.TopLevel = false; this.Parent = parent; 
            this.Location = cpShow.ToPoint();
            if (checkView.Checked) nIndex = 1; 
            else nIndex = 0;
            this.Size = m_sz[nIndex]; 
            cpShow.y += m_sz[nIndex].Height;
            Show();

            m_bCheckview = checkView.Checked;   //박상영20180207
        }

        public void ControlGrid(Control_Mom control, ezGrid rGrid, eGrid eMode)
        {
            control.AddDO(rGrid, ref m_doUp, m_id, "Up", "DO Elevator Up");
            control.AddDO(rGrid, ref m_doDown, m_id, "down", "DO Elevator Down");
            control.AddDI(rGrid, ref m_diUp, m_id, "Up", "DI Elevator Up");
            control.AddDI(rGrid, ref m_diDown, m_id, "Down", "DI Elevator Down");
            control.AddDI(rGrid, ref m_diTop, m_id, "Top", "DI Elevator Top");
            control.AddDI(rGrid, ref m_diCheck, m_id, "Check", "DI Elevator Check");
        }

        public void RunGrid(eGrid eMode)
        {
            m_grid.Update(eMode, null);
            m_grid.Set(ref m_msUnloadDown, "Delay", "UnloadDown", "(ms)"); 
            m_grid.Refresh();
        }

        private void checkView_CheckedChanged(object sender, EventArgs e)
        {
            m_handler.ShowChild();
        }

        private void buttonUnload_Click(object sender, EventArgs e)
        {
            UnloadUp(); 
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            SetDown(); 
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate); 
            RunGrid(eGrid.eRegWrite);
        }

        void RunThread()
        {
            bool bUp, bDown;
            m_bRun = true; 
            m_bThreadStop = false;  //박상영20171222
            m_bThreadStartDelayEnd = false;
            m_swThreadStartDelay.Start();
            while (m_swThreadStartDelay.Check() < m_nThreadStartDelay)
            {
                if (m_bThreadStop) { m_bThreadStartDelayEnd = true; return; }
                Thread.Sleep(100);
            }
            m_bThreadStartDelayEnd = true;
            while (m_bRun)
            {
                Thread.Sleep(10);
                bUp = m_control.GetInputBit(m_diUp); 
                bDown = m_control.GetInputBit(m_diDown);
                if (m_control.GetInputBit(m_diDown)) m_eState = eState.Full; 
                if (m_bLimit) 
                { 
                    if (!bUp && !bDown) 
                    { 
                        m_bLimit = false; 
                        RunMove(eMove.Stop); 
                        m_eState = eState.Ready; 
                    } 
                }
                else
                {
                    if (bUp) 
                    { 
                        m_bLimit = true; 
                        m_log.Popup("Up Sensor Detected !!"); 
                        RunMove(eMove.Stop); 
                        Thread.Sleep(100); 
                        RunMove(eMove.Down); 
                    }
                    if (bDown)
                    {
                        m_bLimit = true; 
                        m_log.Popup("Down Sensor Detected !!");
                        RunMove(eMove.Stop); 
                        Thread.Sleep(100); 
                        RunMove(eMove.Up);
                    }
                }
                if (!m_bLimit)
                {
                    switch (m_eState)
                    {
                        case eState.Init: break;
                        case eState.Ready: RunReady(); break;
                        case eState.Unload: RunUnload(); break;
                        case eState.Down: RunDown(); break;
                        case eState.Full: break; 
                    }
                }
            }
        }

        void RunMove(eMove eM)
        {
            bool bUp = false;
            bool bDown = false;
            if (m_eMove == eM) return;
            if (m_control.GetInputBit(m_diDown)) m_eState = eState.Full; 
            if (Math.Abs(m_eMove - eM) == 2) RunMove(eMove.Stop);
            switch (eM)
            {
                case eMove.Down: bUp = false; bDown = true; break;
                case eMove.Stop: bUp = false; bDown = false; break;
                case eMove.Up: bUp = true; bDown = false; break;
            }
            m_control.WriteOutputBit(m_doUp, bUp); 
            m_control.WriteOutputBit(m_doDown, bDown);
            m_eMove = eM; 
            Thread.Sleep(100);
        }

        void RunReady()
        {
            if (m_control.GetInputBit(m_diTop))
            {
                RunMove(eMove.Down);
                if (m_control.WaitInputBit(m_diTop, false, 90000))
                {
                    SetAlarm(eAlarm.Warning, eError.TopOff);
                    return;
                }
            }
            RunMove(eMove.Stop); 
        }

        void RunUnload()
        {
            RunMove(eMove.Down);
            Thread.Sleep(m_msUnloadDown);
            RunMove(eMove.Stop);
            if (!m_control.GetInputBit(m_diTop))
            {
                if (RunUnloadUp()) return;
            }
            if (m_control.GetInputBit(m_diTop))
            {
                if (RunUnloadDown()) return; 
            } 
            RunMove(eMove.Stop); 
            if (m_control.GetInputBit(m_diDown)) m_eState = eState.Full; 
            else m_eState = eState.Ready;
        }

        bool RunUnloadUp()
        {
            ezStopWatch sw = new ezStopWatch(); 
            RunMove(eMove.Up);
            while (sw.Check() < 90000)
            {
                if (m_control.GetInputBit(m_diUp))
                {
                    SetAlarm(eAlarm.Warning, eError.Up);
                    m_eState = eState.Init; 
                    return true;
                }
                if (m_control.GetInputBit(m_diTop)) return false; 
                Thread.Sleep(10); 
            }
            SetAlarm(eAlarm.Warning, eError.Top);
            m_eState = eState.Init; 
            return true; 
        }

        bool RunUnloadDown()
        {
            ezStopWatch sw = new ezStopWatch();
            RunMove(eMove.Down);
            while (sw.Check() < 90000)
            {
                if (m_control.GetInputBit(m_diDown))
                {
                    SetAlarm(eAlarm.Warning, eError.Down);
                    m_eState = eState.Init; 
                    return true;
                }
                if (m_control.GetInputBit(m_diTop) == false) return false;
                Thread.Sleep(10);
            }
            SetAlarm(eAlarm.Warning, eError.TopOff);
            m_eState = eState.Init; 
            return true;
        }

        void RunDown()
        {
            RunMove(eMove.Down);
        }

        public void UnloadUp()
        {
            m_eState = eState.Unload; 
        }

        public void SetDown()
        {
            m_eState = eState.Down; 
        }

        public void Stop()
        {
            m_eState = eState.Ready; 
        }

        public bool IsCheck()
        {
            return m_control.GetInputBit(m_diCheck);
        }

        public bool IsReady()
        {
            return (m_eState == eState.Ready); 
        }

        public bool IsFull()
        {
            return (m_eState == eState.Full); 
        }

        public bool CheckInitOK()
        {
            RunMove(eMove.Down); 
            Thread.Sleep(500); 
            Stop(); 
            return true; 
        }

        public void RunInit()
        {
            UnloadUp();
        }

    }
}
