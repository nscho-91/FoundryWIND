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
    public partial class UI_Loadport : DockContent
    {
        UserControlLoadPort m_ucLoadport = null;

        public UI_Loadport()
        {
            InitializeComponent();
        }

        public void Init(int nLP, Auto_Mom auto)
        {
            m_ucLoadport = new UserControlLoadPort(nLP, auto);
            tableLayoutLP.Controls.Add(m_ucLoadport, 0, 0);
        }

        public void ShowPanel(DockPanel dockpanel)
        {
            this.Show(dockpanel);
        }
    }
}
