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
    public partial class UI_Manual_BCR : Form
    {
        Info_Carrier m_infoCarrier = null;

        public UI_Manual_BCR(Info_Carrier infoCarrier)
        {
            InitializeComponent();
            m_infoCarrier = infoCarrier;
        }

        public void GetImage(Bitmap img)
        {
            img.RotateFlip(RotateFlipType.Rotate180FlipNone);
            pictureBox.BackgroundImage = img;
            pictureBox.Show();
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            if (m_infoCarrier != null) m_infoCarrier.m_strCarrierID = textBoxCode.Text;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void textBoxCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (m_infoCarrier != null) m_infoCarrier.m_strCarrierID = textBoxCode.Text;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }
    }
}
