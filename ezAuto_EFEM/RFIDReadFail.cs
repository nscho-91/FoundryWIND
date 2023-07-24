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
    public partial class RFIDReadFail : Form
    {
        Info_Carrier m_InfoCarrier = null;
        public RFIDReadFail(Info_Carrier infoCarrier)
        {
            InitializeComponent();
            m_InfoCarrier = infoCarrier;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (tbCSTID.Text != "" && tbLotID.Text != "") {
                m_InfoCarrier.m_strCarrierID = tbCSTID.Text;
                m_InfoCarrier.m_strLotID = tbLotID.Text;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }
        public DialogResult ShowModal()
        {
            return ShowDialog();
        }
    }
}
