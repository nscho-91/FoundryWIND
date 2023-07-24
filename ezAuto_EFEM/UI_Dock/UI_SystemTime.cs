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
    public partial class UI_SystemTime : DockContent
    {
        public UI_SystemTime()
        {
            InitializeComponent();
        }

        public void Init() 
        {

        }

        public void ShowPanel(DockPanel dockpanel)
        {
            this.Show(dockpanel);
        }

    }
}
