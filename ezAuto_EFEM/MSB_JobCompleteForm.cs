using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ezAutoMom;

namespace ezAuto_EFEM
{
    public partial class MSB_JobCompleteForm : Form
    {
        Auto_Mom m_auto = null;
        public MSB_JobCompleteForm(Auto_Mom auto, string A, string B)
        {
            InitializeComponent();
            m_auto = auto;
            label1.Text = A;
            label2.Text = B;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ChangeLabelColor(label1);
            ChangeLabelColor(label2);
        }

        private void ChangeLabelColor(Label label)
        {
            if (label.BackColor == Color.Salmon)
                label.BackColor = Color.PaleGreen;
            else if (label.BackColor == Color.PaleGreen)
                label.BackColor = Color.Salmon;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close(); 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            m_auto.ClassWork().WorkerBuzzerOff(); 
        }
    }
}
