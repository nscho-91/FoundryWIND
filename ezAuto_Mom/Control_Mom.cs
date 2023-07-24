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
using WeifenLuo.WinFormsUI.Docking;
using ezTools;
using ezAxis; 

namespace ezAutoMom
{
    public partial class Control_Mom : DockContent
    {
        string[] m_strTypes = new string[2] { "UC", "EIP" };

        public string m_id;

        ArrayList m_listAxis = new ArrayList(); 

        public Log m_log;
        ezGrid m_grid;
        ArrayList m_list = new ArrayList();
        Size m_szGrid; 

        public virtual bool GetInputBit(int n) { return false; }
        public virtual bool GetOutputBit(int n) { return false; }
        public virtual void WriteOutputBit(int n, bool bOn) { }
        public virtual bool IsAxisOpen() { return false; }
        public virtual void RunEmergency() { }
        public virtual void SetDICaption(int nDI, string str) { }
        public virtual void SetDOCaption(int nDI, string str) { }
        public virtual void SetGantry(int nMaster, int nSlave) { }
        public virtual Axis_Mom GetAxis(ezAxis.Type type, string strID) { return null; }

        public virtual void AddDI(ezGrid rGrid, ref int nDI, string strGroup, string strProp, string strDesc) 
        {
            rGrid.Set(ref nDI, strGroup, "DI_" + strProp, strDesc);
            SetDICaption(nDI, strGroup + strProp);
        }

        public virtual void AddDO(ezGrid rGrid, ref int nDO, string strGroup, string strProp, string strDesc) 
        {
            rGrid.Set(ref nDO, strGroup, "DO_" + strProp, strDesc);
            SetDOCaption(nDO, strGroup + strProp);
        }
        
        public virtual Axis_Mom AddAxis(ezGrid rGrid, string strGroup, string strProp, string strDesc) 
        {
            ezAxis.Type type = GetType(strGroup + strProp); 
            type.RunGrid(rGrid, strGroup, "Axis_" + strProp, strDesc); 
            return GetAxis(type, strGroup + strProp);
        }

        ezAxis.Type GetType(string str)
        {
            foreach (ezAxis.Type type in m_listAxis)
            {
                if (str == type.m_id) return type; 
            }
            ezAxis.Type newType = new ezAxis.Type(); 
            newType.m_id = str;
            m_listAxis.Add(newType);
            return newType; 
        }

        public Control_Mom()
        {
            InitializeComponent();
            m_szGrid = this.Size - grid.Size; 
        }

        public virtual void Init(string id, int nDIO, LogView logView)
        {
            this.Text = m_id = id;
            grid.Location = new Point(10, 42);
            m_log = new Log(m_id, logView);
            m_grid = new ezGrid(m_id, grid, m_log, false);
            RunGrid(eGrid.eRegRead); RunGrid(eGrid.eInit);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            RunGrid(eGrid.eRegWrite); 
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate); RunGrid(eGrid.eRegWrite);
        }

        public void RunGrid(eGrid eMode, ezJob job = null)
        {
            if (m_grid == null) return;
            m_grid.Update(eMode, job);
            foreach (Control_Child child in m_list) child.ControlGrid(this, m_grid, eMode);
            m_grid.Refresh();
        }

        public bool WaitInputBit(int nIn, bool bOn, int msWait = 3000)
        {
            ezStopWatch swWait = new ezStopWatch(); 
            if (nIn < 0) return false; 
            while ((GetInputBit(nIn) != bOn) && (swWait.Check() <= msWait)) Thread.Sleep(1); 
            return (swWait.Getms(true) >= msWait); 
        }

        public void Add(Control_Child child)
        {
            m_list.Add(child); 
        }

        private void Control_Mom_Resize(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            Size sz = control.Size;
            if (control.Text == m_id) grid.Size = sz - m_szGrid;
        }

        private void Control_Mom_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

    }
}
