using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using WeifenLuo.WinFormsUI.Docking;
using ezTools;

namespace ezAutoMom
{
    public partial class HW_ExcuteForm : DockContent
    {
        const int WM_SYSCOMMAND = 274;
        const int SC_MAXIMIZE = 61488;

        string[] m_sPaths;
        string[] m_sProcesses;
        Process[] m_aProcess;
        bool[] m_aExcuted;
        int[] m_aProcessID;
        bool m_bRun = false;
        //bool m_bUpdate = false;
        public int m_nProcess = 0;
        public string m_id;
        public Log m_log;
        public bool m_bUse = false;
        Auto_Mom m_auto;
        Thread m_thread;

        public HW_ExcuteForm(string id, int nProcess)
        {

            InitializeComponent();
            m_id = Text = id;
            m_nProcess = nProcess;
            int n;
            m_aProcess = new Process[m_nProcess];
            m_sPaths = new string[m_nProcess];
            m_sProcesses = new string[m_nProcess];
            m_aExcuted = new bool[m_nProcess];
            m_aProcessID = new int[m_nProcess];
            for (n = 0; n < m_aProcess.Length; n++)
            {
                m_aProcess[n] = new Process();
                m_sPaths[n] = "";
                m_sProcesses[n] = "";
            }

        }

        public void Init(Auto_Mom auto, Log log)
        {
            
            m_auto = auto;
            m_log = log;
            m_thread = new Thread(new ThreadStart(RunThread));
            m_thread.Start();
        }

        public void ThreadStop()
        {
            int n;
            if (m_bRun)
            {
                m_bRun = false;
                m_thread.Join();
            }
            for (n = 0; n < m_aProcess.Length; n++)
            {
                try
                {
                    if (m_aExcuted[n])
                        if (!m_aProcess[n].HasExited) 
                            m_aProcess[n].Dispose();
                }
                catch
                {
                    continue;
                }
            }
        }

        public IDockContent GetContentFromPersistString(string persistString)
        {
            if (persistString == typeof(HW_ExcuteForm).ToString()) return this;
            return null;
        }

        public void ModelGrid(ezGrid grid, eGrid Mode)
        {
            int n;
            grid.Set(ref m_bUse, m_id, "Use", "Use Multi-Display");
            if (m_aProcess == null) return;
            for (n = 0; n < m_aProcess.Length; n++)
            {
                m_aExcuted[n] = false;
                grid.Set(ref m_sPaths[n], m_id, "Path" + n.ToString(), "Program Path" + n.ToString());
                grid.Set(ref m_sProcesses[n], m_id, "Process" + n.ToString(), "Config Process Name" + n.ToString());
            }
        }

        void RunThread()
        {
            int n;
            Thread.Sleep(5000);
            m_bRun = true;
            while (m_bRun)
            {
                Thread.Sleep(500);
                SearchProcces();
                for (n = 0; n < m_aProcess.Length; n++)
                {
                    if (!m_aExcuted[n])
                    {
                        if (File.Exists(m_sPaths[n]))
                        {
                            StartProcess(ref m_aProcess[n], m_sPaths[n]);
                            break;
                        }
                    }
                }
            }
        }

        public bool StartProcess(ref Process process, string strPath)
        {
            try
            {
                process = new Process();
                ProcessStartInfo infoProcess = new ProcessStartInfo(strPath);
                process.StartInfo = infoProcess;
                process.Start();
            }
            catch (Exception ex)
            {
                m_log.Popup(m_id + " : " + ex.Message);
                return true;
            }
            return false;
        }

        public bool AddWindow(Process process)
        {
            try
            {
                IntPtr hPro;
                hPro = process.MainWindowHandle;
                SetParent(hPro, this.Handle);
                SendMessage(hPro, WM_SYSCOMMAND, SC_MAXIMIZE, 0);
            }
            catch (Exception ex)
            {
                m_log.Popup(m_id + " : " + ex.Message);
            }
            return false;
        }

        public bool AddWindow(string strProcessName)
        {
            bool bFind = false;
            Process process = new Process();
            IntPtr hPro;
            Process[] aCurrentProcess = Process.GetProcesses();
            foreach(Process pro in aCurrentProcess)
            {
                if (pro.ProcessName == strProcessName)
                {
                    bFind = true;
                    process = pro;
                }
            }
            if (!bFind) return true;
            hPro = process.MainWindowHandle;
            SetParent(hPro, this.Handle);
            SendMessage(hPro, WM_SYSCOMMAND, SC_MAXIMIZE, 0);
            Show();
            return false;
        }

        public void SearchProcces()
        {
            int n;
            Process[] aCurrentProcess = Process.GetProcesses();
            for (n = 0; n < m_aProcess.Length; n++)
            {
                m_aExcuted[n] = false;
            }
            foreach (Process pro in aCurrentProcess)
            {
                for (n = 0; n < m_aProcess.Length; n++)
                {
                    if (pro.ProcessName == m_sProcesses[n])
                    {
                        m_aExcuted[n] = true;
                        m_aProcess[n] = pro;
                        if (pro.Id != m_aProcessID[n])
                        {
                            m_aProcessID[n] = pro.Id;
                            //m_bUpdate = true;
                        }
                    }
                }
            }
        }

        private void HW_ExcuteForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            int n;
            for (n = m_aProcess.Length - 1; n >= 0; n--)
            {
                if (m_aExcuted[n])
                {
                    AddWindow(m_aProcess[n]);
                    /*if (m_bUpdate)
                    {
                        AddWindow(m_aProcess[n]);
                        m_bUpdate = false;
                    }*/
                    return;
                }
            }
        }

        [DllImport("user32.dll")]
        public static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    }

}
