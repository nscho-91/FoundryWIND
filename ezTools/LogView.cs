using System;
using System.Collections; 
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO; 
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ezTools
{
    public enum eAlarm
    {
        Popup,
        Stop,
        Warning,
        Error
    }

    public partial class LogView : DockContent
    {
        const int c_nLocCount = 200; 

        public string m_id;
        public string m_strModel;
        public string m_strPath = "c:\\Log";
        bool m_bHidePopup = false;
        DockPanel m_dockPanel = null; 

        int m_nGrid = 0;
        int m_msPopup = 0;
        public LogPopup m_logPopup;
        ImgPopup m_imgPopup;
        Log m_log;
        ezGrid m_grid;
        ArrayList m_listLog = new ArrayList();
        ArrayList m_listGroup = new ArrayList();

        static readonly object m_csLock = new object();

        public LogView(string id, string strModel, DockPanel dockPanel)
        {
            m_id = id; 
            m_strModel = strModel;
            m_dockPanel = dockPanel;
            InitializeComponent();
            tabControlLog.Location = new Point(10, 42);
            grid.Location = new Point(10, 42); grid.Hide(); 
            combo.SelectedIndex = 0;
            LogGroup logGroup = new LogGroup("Total", listBox_total);
            m_listGroup.Add(logGroup);
            AddLogTab("Error");
            logGroup = new LogGroup("Error", (ListBox)tabControlLog.TabPages["Error"].Controls[0]);
            m_listGroup.Add(logGroup);
            m_log = new Log(m_id, this); 
            m_grid = new ezGrid(m_id, grid, m_log, false);
            m_imgPopup = new ImgPopup("ImgPopup", m_strModel);
            m_logPopup = new LogPopup("LogPopup", m_strModel, dockPanel, m_imgPopup);
            m_nGrid = 1; RunGrid(eGrid.eRegRead); RunGrid(eGrid.eInit);
            m_nGrid = 2; RunGrid(eGrid.eRegRead); RunGrid(eGrid.eInit);
        }

        public void ThreadStop()
        {
            m_log.Add("Log Stop"); LogSave(); 
        }

        private void buttonPopup_Click(object sender, EventArgs e)
        {
            if (!m_bHidePopup) { m_logPopup.Show(m_dockPanel); m_imgPopup.ShowForm(); }
        }

        private void LogView_Resize(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            Size sz = control.Size; 
            if (control.Text == "LogView")
            {
                sz.Height -= 45; sz.Width -= 20;
                tabControlLog.Size = sz;
                grid.Size = sz; 
            }
        }

        public IDockContent GetContentFromPersistString(string persistString)
        {
            if (persistString == typeof(LogView).ToString()) return this;
            return null;
        }

        public void ShowAll(DockPanel dockPanel)
        {
            Show(dockPanel);
        }

        private void combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combo.SelectedIndex == 0)
            { 
                grid.Hide();
                tabControlLog.Show();
                RunGrid(eGrid.eRegWrite); 
            }
            else 
            {
                m_nGrid = combo.SelectedIndex; 
                grid.Show();
                tabControlLog.Hide();
                RunGrid(eGrid.eInit); 
            }
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e); 
            RunGrid(eGrid.eUpdate);
            switch (m_nGrid)
            {
                case 1: RunGrid(eGrid.eRegWrite); break;
                case 2: m_grid.WriteLogString(); break;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            m_msPopup += timer.Interval;
            if (m_msPopup > 7000) m_msPopup = 7000;
            LogSave();
        }

        void RunGrid(eGrid eMode, ezJob job = null)
        {
            if (m_grid == null) return;
            m_grid.Update(eMode, job);
            if (m_nGrid == 1) 
            {
                m_grid.Set(ref m_strPath, "Setting", "Path", "Save Log Path");
                m_logPopup.RunGrid(m_grid);
                m_imgPopup.RunGrid(m_grid);
            }
            if (m_nGrid == 2)
            {
                foreach (Log log in m_listLog) log.RunGrid(m_grid, eMode); 
            }
            m_grid.Refresh(); 
        }

        public int Add(Log log, string sGroup)
        {
            int nIndex = 0;
            m_listLog.Add(log);
            if (sGroup == null) return -1; 
            foreach (LogGroup logGroup in m_listGroup)
            {
                if (logGroup.m_sGroup == sGroup) return nIndex;
                nIndex++;
            }
            AddLogTab(sGroup);
            ListBox listBox = (ListBox)tabControlLog.TabPages[sGroup].Controls[0];
            LogGroup group = new LogGroup(sGroup, listBox);
            m_listGroup.Add(group);
            return nIndex;
        }

        public string GetLastLog()
        {
            if (((LogGroup)m_listGroup[0]).m_listBox.Items.Count > 0)
                return ((LogGroup)m_listGroup[0]).m_listBox.Items[((LogGroup)m_listGroup[0]).m_listBox.Items.Count-1].ToString();
            return "";
        }

        public string GetLastError(int nIndex)    //박상영20180109
        {
            if (((LogGroup)m_listGroup[1]).m_listBox.Items.Count <= 0) return "";
            int n = ((LogGroup)m_listGroup[1]).m_listBox.Items.Count - 1 - nIndex;
            if (n < 0) return "";
            return ((LogGroup)m_listGroup[1]).m_listBox.Items[n].ToString();
        }

        public string GetLastErrorLog(int nIndex)
        {
            string sLog = GetLastError(nIndex);
            int nIdx = sLog.IndexOf("]");
            if (nIdx < 0) return "";
            return sLog.Substring(nIdx + 1);
        }

        public string GetLastErrorTime(int nIndex, bool bRemoveUnderPoint = true)
        {
            string sLog = GetLastError(nIndex);
            int nIdx0 = sLog.IndexOf("[");
            int nIdx1 = sLog.IndexOf("]");                      
            if (nIdx0 < 0 || nIdx1 < 0) return "";
            sLog = sLog.Substring(nIdx0 + 1, nIdx1 - 1).Replace('-', ':');
            if (bRemoveUnderPoint)
            {
                int nIdx2 = sLog.IndexOf(".");
                sLog = sLog.Substring(0, nIdx2);
            }            
            return sLog;
        }

        public void ClearError()    //박상영20180110
        {
            ((LogGroup)m_listGroup[1]).m_listBox.Items.Clear();
        }

        public void Add(LogDat logDat)
        {
            lock (m_csLock)
            {
                ((LogGroup)m_listGroup[0]).m_que.Enqueue(logDat); 
                if (logDat.m_eLogLv > eLogLv.Add) ((LogGroup)m_listGroup[1]).m_que.Enqueue(logDat); 
                if (logDat.m_iGroup >= 0) ((LogGroup)m_listGroup[logDat.m_iGroup]).m_que.Enqueue(logDat); 
            }
        }

        public void AddLogTab(string Name)
        {
            var page = new TabPage(Name);
            var LogBox = new ListBox();

            page.Name = Name;
            page.Controls.Add(LogBox);
            page.Padding = tabControlLog.TabPages[0].Padding;

            LogBox.Name = "listBox_" + Name;
            LogBox.Dock = DockStyle.Fill;
            LogBox.HorizontalScrollbar = true;

            tabControlLog.TabPages.Add(page);
        }


        void LogSave()
        {
            ezDateTime date = new ezDateTime();
            Directory.CreateDirectory(m_strPath);
            string sPath = m_strPath + "\\" + date.GetDate();
            Directory.CreateDirectory(sPath);
            sPath += "\\" + date.GetDate(); 
            foreach (LogGroup logGroup in m_listGroup)
            {
                if (logGroup.m_que.Count > 0) LogSave(logGroup, sPath); 
            }
        }

        void LogSave(LogGroup logGroup, string sPath)
        {
            bool bPopup = logGroup.m_sGroup == "Error"; 
            LogDat logDat;
            string str; 
            string sFile = sPath + "_" + logGroup.m_sGroup + ".txt";
            StreamWriter sw = new StreamWriter(new FileStream(sFile, FileMode.Append));
            int l = logGroup.m_que.Count;
            for (int n = 0; n < l; n++)
            {
                logDat = (LogDat)logGroup.m_que.Dequeue();
                if (logDat != null)
                {
                    str = logDat.GetLog();
                    sw.WriteLine(str);
                    if (logDat.m_bShowList)
                    {
                        if (logGroup.m_listBox.Items.Count > c_nLocCount) logGroup.m_listBox.Items.RemoveAt(0);
                        logGroup.m_listBox.Items.Add(str);
                        logGroup.m_listBox.SelectedIndex = logGroup.m_listBox.Items.Count - 1;
                    }
                    if (!m_bHidePopup && bPopup && (m_msPopup > 1000))
                    {
                        m_logPopup.Popup(logDat);
                        m_imgPopup.Popup(logDat.m_id, logDat.m_str);
                        m_msPopup -= 1000;
                    }
                }
            }
            sw.Close(); 
        }

        public void HidePopup(bool bHide)
        {
            m_bHidePopup = bHide;
            if (m_bHidePopup) { m_logPopup.Hide(); m_imgPopup.Hide(); }
        }

        private void LogView_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

    }

    struct LogGroup
    {
        public string m_sGroup;
        public Queue m_que;
        public ListBox m_listBox;

        public LogGroup(string sGroup, ListBox listBox)
        {
            m_sGroup = sGroup; m_listBox = listBox;
            m_que = new Queue();
        }
    }

}
