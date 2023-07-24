using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using ezTools;

namespace ezAutoMom
{
    public partial class WTR_Mom_Wafers : DockContent
    {
        string m_id; 
        Log m_log;
        ezGrid m_grid;
        Size m_szGrid;
        ArrayList m_aWafer = null;

        Auto_Mom m_auto = null; 

        public WTR_Mom_Wafers()
        {
            InitializeComponent();
            m_szGrid = this.Size - grid.Size; 
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            RunGrid(eGrid.eInit);
        }

        private void WTR_Mom_Wafers_Resize(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            Size sz = control.Size;
            if (control.Text == m_id) grid.Size = sz - m_szGrid;
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
        }

        public bool IsPersistString(string str)
        {
            return GetPersistString() == str;
        }

        public void Init(string id, Auto_Mom auto, ArrayList aWafer)
        {
            this.Text = m_id = id;
            m_auto = auto;
            m_aWafer = aWafer; 
            m_log = new Log(m_id, m_auto.ClassLogView());
            m_grid = new ezGrid(m_id, grid, m_log, false);
        }

        public void RunGrid(eGrid eMode, ezJob job = null)
        {
            m_grid.Update(eMode, job);
            foreach (InfoWafer infoWafer in m_aWafer)
            {
                m_grid.Set(ref infoWafer.m_sWaferID, infoWafer.m_id, "WaferID", "Wafer ID", true);
                int nLeft = infoWafer.m_aLocate.Count - infoWafer.m_nLocate;
                m_grid.Set(ref nLeft, infoWafer.m_id, "Pos", "Number of Position to Move", true);
                if (infoWafer.m_sWTR != "") m_grid.Set(ref infoWafer.m_sWTR, infoWafer.m_id, "Skip", "Reason of WTR Skip", true); 
                for (int n = infoWafer.m_nLocate, i = 0; (n < infoWafer.m_aLocate.Count) && (i < 7); n++, i++)
                {
                    string sLocate = (string)infoWafer.m_aLocate[n];
                    m_grid.Set(ref sLocate, infoWafer.m_id, "Locate" + n.ToString(), "Locate", true); 
                }
            }
            m_grid.Refresh();
        }

    }
}
