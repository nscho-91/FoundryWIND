using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ezAuto_EFEM
{
    public partial class UserControlOverViewLP : UserControl
    {
        public UserControlOverViewLP()
        {
            InitializeComponent();
        }
        public void Set(String sName, bool bPlaced, bool bDocking, bool bOpen)
        {
            labelLP.Text = sName;
            IOPlaced.Set(bPlaced);
            IOOpen.Set(bOpen);
            IODocking.Set(bDocking);
        }
    }
}
