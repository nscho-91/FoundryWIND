using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using ezAutoMom;
using ezTools; 

namespace ezAuto_EFEM
{
    public partial class UI_JobLog : DockContent
    {

        public enum eJobLogContent
        {
            No,
            LoadPort,
            JobID,
            LotID,
            CarrierID,
            Recipe,
            SlotMap,
            StartTime,
            EndTime,
            State,
        }
        public ezRegistry m_reg;
        public UI_JobLog()
        {
            InitializeComponent();

        }
        
        public void ShowPanel(DockPanel dockpanel)
        {
            this.Show(dockpanel);
        }

        public void Init(Auto_Mom auto)
        {
            m_reg =  new ezRegistry(auto.ClassLogView().m_strModel, "UI_Logview");
            LoadListViewWidth();
            //listViewJobLog.Columns[0].Name = "No";
            //listViewJobLog.Columns[1].Name = "LoadPort";
            //listViewJobLog.Columns[2].Name = "JobID";
            //listViewJobLog.Columns[2].Name = "LotID";
            //listViewJobLog.Columns[3].Name = "CarrierID";
            //listViewJobLog.Columns[4].Name = "Recipe";
            //listViewJobLog.Columns[5].Name = "SlotMap";
            //listViewJobLog.Columns[6].Name = "StartTime";
            //listViewJobLog.Columns[7].Name = "EndTime";
            //listViewJobLog.Columns[8].Name = "State";
        }

        public void SaveListViewWidth()
        {
            for (int i = 0; i < Enum.GetNames(typeof(eJobLogContent)).Length; i++)
                m_reg.Write(Enum.GetName(typeof(eJobLogContent), i), listViewJobLog.Columns[i].Width);
            
        }

        public void LoadListViewWidth()
        {
            for (int i = 0; i < Enum.GetNames(typeof(eJobLogContent)).Length; i++){
                listViewJobLog.Columns[i].Width = m_reg.Read(Enum.GetName(typeof(eJobLogContent), i), listViewJobLog.Columns[i].Width);
            }
        }

        private void listViewJobLog_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            SaveListViewWidth();
        }


    }

    public class CustomFloatWindowFactory : DockPanelExtender.IFloatWindowFactory
    {
        public FloatWindow CreateFloatWindow(DockPanel dockPanel, DockPane pane, Rectangle bounds)
        {
            return new CustomFloatWindow(dockPanel, pane, bounds);
        }
        public FloatWindow CreateFloatWindow(DockPanel dockPanel, DockPane pane)
        {
            return new CustomFloatWindow(dockPanel, pane);
        }
    }

    public class CustomFloatWindow : FloatWindow
    {
        public CustomFloatWindow(DockPanel dockPanel, DockPane pane)
            : base(dockPanel, pane)
        {
            FormBorderStyle = FormBorderStyle.None;
        }
        public CustomFloatWindow(DockPanel dockPanel, DockPane pane, Rectangle bounds)
            : base(dockPanel, pane, bounds)
        {
            FormBorderStyle = FormBorderStyle.None;
        }
    }

}
