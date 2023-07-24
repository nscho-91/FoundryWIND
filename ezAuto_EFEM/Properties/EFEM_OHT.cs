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
using System.Threading;
using ezAutoMom;
using ezTools;
using System.IO;

namespace ezAuto_EFEM
{
    public partial class EFEM_OHT : DockContent
    {
        Control_Mom m_control;
        Auto_Mom m_auto;
        const int nIO_Num = 8;
        const int EIPStartNum=96;
        public EFEM_OHT()
        {
            InitializeComponent();
            MakeIO();
            m_control = m_auto.ClassControl();
        }
        public void MakeIO()
        {
            for (int i = 0; i < nIO_Num; i++)
            {
                Button bInput = new Button();
                Button bOutput = new Button();
                this.Controls.Add(bInput);
                bInput.Name = bInput.Text = "InPut" + Convert.ToString(i);
                bInput.Location = new Point(80, 300 + (i * 40));
                this.Controls.Add(bOutput);
                bOutput.Name = bOutput.Text = "Output" + Convert.ToString(i);
                bOutput.Location = new Point(170, 300 + (i * 40));

            }
        }
        public void IODisplay()
        {
            for(int i=0;i<nIO_Num;i++)
            {
                bool bInput = false;
                bool bOutput = false;
                bInput = m_control.GetInputBit(EIPStartNum + i);
                bOutput = m_control.GetOutputBit(EIPStartNum + i);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            IODisplay(); 
        }
    }
}
