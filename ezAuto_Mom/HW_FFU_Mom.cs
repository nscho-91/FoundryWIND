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
using WeifenLuo.WinFormsUI.Docking;
using ezTools;

namespace ezAutoMom
{
    public partial class HW_FFU_Mom : Form
    {
        public enum eError
        {
            EFEM1_RPM,
            EFEM1_Pressure,
            EFEM2_RPM,
            EFEM2_Pressure,
            Vision1_RPM,
            Vision1_Pressure,
            Vision2_RPM,
            Vision2_Pressure
        }

        Auto_Mom m_auto;
        Handler_Mom m_handler;
        XGem300_Mom m_xGem; 
        public Work_Mom m_work; 
        Size[] m_sz = new Size[2];
        protected string m_id;
        protected Log m_log;
        protected ezGrid m_grid;
        public int m_nModuleNumber = -1;
        protected FFUModule_Mom[] m_module;
        protected int m_msAlarmInterval = 3000;

        public HW_FFU_Mom()
        {
            InitializeComponent();
            m_sz[0] = m_sz[1] = this.Size;
            m_sz[0].Height = 26;
        }

        public virtual void Init(string id, Auto_Mom auto, int nModuleNum)
        {
            m_id = id;
            m_auto = auto;
            checkView.Text = m_id;
            m_log = new Log(m_id, m_auto.m_logView, m_id);
            m_grid = new ezGrid(m_id, grid, m_log, false);
            m_work = ((Auto_Mom)auto).ClassWork(); 
            m_xGem = m_auto.ClassXGem(); 
            m_handler = m_auto.ClassHandler();
            InitString(); 
            m_nModuleNumber = nModuleNum;
            timer.Enabled = true;
        }
        void InitString()
        {
            InitString(eError.EFEM1_RPM,"FDC EFEM1 RPM Error!!");
            InitString(eError.EFEM1_Pressure, "FDC EFEM1 Pressure Error!!");
            InitString(eError.EFEM2_RPM, "FDC EFEM2 RPM Error!!");
            InitString(eError.EFEM2_Pressure, "FDC EFEM2 Pressure Error!!");
            InitString(eError.Vision1_RPM, "FDC Vision1 RPM Error!!");
            InitString(eError.Vision1_Pressure, "FDC Vision1 Pressure Error!!");
            InitString(eError.Vision2_RPM, "FDC Vision2 RPM Error!!");
            InitString(eError.Vision2_Pressure, "FDC Vision2, Pressure Error!!");
            
        } 
        void InitString(eError eErr, string str)
        {
            m_log.AddString(str);
            if (m_xGem == null) return;
            m_xGem.AddALID(m_id, (int)eErr, str);
        } 

        public void SetAlarm(eAlarm alarm, eError eErr)
        {
            m_work.SetError(alarm, m_log, (int)eErr);
            if (m_xGem == null) return;
            m_xGem.SetAlarm(m_id, (int)eErr);
        } 


        public virtual void ThreadStop()
        {

        }
        
        public void ShowDlg(Form parent, ref CPoint cpShow)
        {
            int nIndex;
            this.TopLevel = false;
            this.Parent = parent;
            this.Location = cpShow.ToPoint();
            if (checkView.Checked) nIndex = 1;
            else nIndex = 0;
            this.Size = m_sz[nIndex];
            cpShow.y += m_sz[nIndex].Height;
            Show();
        }

        protected virtual void RunGrid(eGrid eMode)
        {
            m_grid.Update(eMode, null);
            m_grid.Set(ref m_msAlarmInterval, "Setting", "AlarmInterval", "Alarm Interval Time (ms)");
            timer.Interval = m_msAlarmInterval;
        }


        private void checkView_CheckedChanged(object sender, EventArgs e)
        {
            m_handler.ShowChild();
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate);
            RunGrid(eGrid.eRegWrite);
        }

        public virtual IDockContent GetContentFromPersistString(string persistString)
        {
            return null;
        }

        public virtual void ShowAll(DockPanel dockPanel)
        {

        }

        public virtual FFUModule_Mom GetFFUModule(int nIndex)
        {
            return null;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (m_module != null)
            {
                string strAlarm;
                for (int n = 0; n < m_module.Length; n++)
                {
                    strAlarm = m_module[n].GetAlarmMessage();
                    if (strAlarm != "")
                    {
                        string[] FDCError = strAlarm.Split(' ');
                        switch (FDCError[0]) { 
                            case "EFEM1_RPM":
                                m_log.ChangeString((int)eError.EFEM1_RPM, strAlarm);
                                SetAlarm(eAlarm.Error, eError.EFEM1_RPM); 
                                break;
                            case "EFEM1_Pressure":
                                m_log.ChangeString((int)eError.EFEM1_Pressure, strAlarm);
                                SetAlarm(eAlarm.Error, eError.EFEM1_Pressure); 
                                break;
                            case "EFEM2_RPM":
                                m_log.ChangeString((int)eError.EFEM2_RPM, strAlarm);
                                SetAlarm(eAlarm.Error, eError.EFEM2_RPM);
                                break;
                            case "EFEM2_Pressure":
                                m_log.ChangeString((int)eError.EFEM2_Pressure, strAlarm);
                                SetAlarm(eAlarm.Error, eError.EFEM2_Pressure);
                                break;
                            case "Vision1_RPM":
                                m_log.ChangeString((int)eError.Vision1_RPM, strAlarm);
                                SetAlarm(eAlarm.Error, eError.Vision1_RPM);
                                break;
                            case "Vision1_Pressure":
                                m_log.ChangeString((int)eError.Vision1_Pressure, strAlarm);
                                SetAlarm(eAlarm.Error, eError.Vision1_Pressure);
                                break;
                            case "Vision2_RPM":
                                m_log.ChangeString((int)eError.Vision2_RPM, strAlarm);
                                SetAlarm(eAlarm.Error, eError.Vision2_RPM);
                                break;
                            case "Vision2_Pressure":
                                m_log.ChangeString((int)eError.Vision2_Pressure, strAlarm);
                                SetAlarm(eAlarm.Error, eError.Vision2_Pressure);
                                break;
                        } 
                    }
                }
                if (m_auto.m_bXgemUse)
                {
                    Dictionary<long, string> dicValue = new Dictionary<long, string>();
                    for (int n = 0; n < m_module.Length; n++)
                    {
                        if (m_module[n].m_nSVID > -1)
                        {
                            dicValue.Add((long)m_module[n].m_nSVID, m_module[n].m_sValue);
                        }
                    }
                    if (dicValue.Count > 0)
                    {
                        long[] lVids = dicValue.Keys.ToArray<long>();
                        string[] sValues = dicValue.Values.ToArray<string>();
                        m_auto.ClassXGem().m_XGem300.GEMSetVariable(dicValue.Count, lVids, sValues);
                    }
                }
            }
        }
        

        protected virtual void RunTimer()
        {

        }

        protected virtual void Connect()
        {

        }

        protected virtual void Reset()
        {

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            Connect();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }
    }

    public class FFUModule_Mom
    {
        public enum eUnit
        {
            None = 0,
            kPa,
            MPa,
            Temp,
            Voltage,
        }

        public string m_sValue;
        public string m_sName = "";
        public int m_nSVID = -1;

        public virtual void RunGrid(ezGrid rGrid, eGrid eMode)
        {
            
        }

        public string GetValue()
        {
            return m_sValue;
        }

        public virtual string GetAlarmMessage()
        {
            return "";
        }
    }
}
