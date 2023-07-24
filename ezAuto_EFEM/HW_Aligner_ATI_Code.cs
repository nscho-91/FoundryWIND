using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ezTools; 

namespace ezAuto_EFEM
{
    public partial class HW_Aligner_ATI_Code : Form
    {
        ezImgView m_imgView = null; 

        public HW_Aligner_ATI_Code(ezImgView imgView)
        {
            InitializeComponent();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            m_imgView = imgView;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close(); 
        }

        public string GetCode()
        {
            return tbCode.Text; 
        }

        private void HW_Aligner_ATI_Code_Paint(object sender, PaintEventArgs e)
        {
            Graphics dc = e.Graphics;
            Size szBorder = this.Size - this.ClientSize;
            Size szImg = (Size)(m_imgView.m_pImg.m_szImg.ToPoint());
            if (szImg.Width < 150) szImg.Width = 250;
            if (szImg.Height < 48) szImg.Height = 48; 
            this.Size = szBorder + szImg;
            m_imgView.m_fZoom = 1; 
            m_imgView.Draw(dc); 
        }

    }
}
