using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ezAutoMom
{
    public partial class PowerMeter_Mom : Form
    {
        public PowerMeter_Mom()
        {
            InitializeComponent();
        }

        private void checkSetup_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {

        }

        private void buttonZeroSet_Click(object sender, EventArgs e)
        {

        }

        private void buttonMeasure_Click(object sender, EventArgs e)
        {

        }

        private void buttonStop_Click(object sender, EventArgs e)
        {

        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {

        }

        private void PowerMeter_Mom_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }        
    }
}
