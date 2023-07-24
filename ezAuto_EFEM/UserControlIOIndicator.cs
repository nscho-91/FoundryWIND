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
    public partial class UserControlIOIndicator : UserControl
    {
        public UserControlIOIndicator()
        {
            InitializeComponent();
            Set(false);
        }

        public string IOName
        {
            get { return labelName.Text; }
            set { labelName.Text = value; }
        }

        public void Set(bool b)
        {
            if(b == true)
            {
                this.panel1.BackgroundImage = global::ezAuto_EFEM.Properties.Resources._1460123042_circle_green;
            }
            else
            {
                this.panel1.BackgroundImage = global::ezAuto_EFEM.Properties.Resources._1460123046_circle_red;
            }            
        }
    }
}
