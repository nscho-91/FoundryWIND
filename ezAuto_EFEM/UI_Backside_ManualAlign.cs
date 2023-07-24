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
    public partial class UI_Backside_ManualAlign : Form
    {
        ezImgView m_imgView;
        HW_BackSide_ATI_AOIData m_data;
        CPoint m_cpLBD, m_cpMove, m_cpNotch;

        public UI_Backside_ManualAlign()
        {
            InitializeComponent();
        }

        public void Init(ezImgView imgView, HW_BackSide_ATI_AOIData data)
        {
            m_imgView = imgView;
            m_data = data;
        }

        private void buttonAlign_Click(object sender, EventArgs e)
        {

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {

        }

        private void UI_Backside_ManualAlign_MouseDown(object sender, MouseEventArgs e)
        {

        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            m_imgView.Zoom(e.Delta, new CPoint(e.Location));
            SetTabText(m_cpMove); Invalidate();
        }

        void SetTabText(CPoint cpWin)
        {
            int nGV;
            CPoint cpImg = m_imgView.GetImgPos(cpWin);
            nGV = m_imgView.m_pImg.GetGV(cpImg);
        }

    }
}
