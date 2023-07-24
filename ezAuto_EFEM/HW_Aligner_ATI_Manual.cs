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
using ezCam;
using ezAxis; 
using ezTools; 

namespace ezAuto_EFEM
{
    public partial class HW_Aligner_ATI_Manual : Form
    {
        string m_id;
        Auto_Mom m_auto;
        Axis_Mom m_axis;
        ezCam_Basler_Simple m_camAlign;
        Log m_log;
        ezImgView m_imgView;

        bool m_bJog = false;
        bool m_bKeyShift = false;
        bool m_bKeyControl = false;

        public double m_posAxis = 0;
        double[] m_posRange = new double[2] { -2000, 42000 }; 
        
        public HW_Aligner_ATI_Manual()
        {
            InitializeComponent();
        }

        public void Init(string id, Auto_Mom auto, Axis_Mom axis, ezCam_Basler_Simple camAlign, double posMin, double posMax)
        {
            m_id = id;
            m_auto = auto;
            m_axis = axis;
            m_camAlign = camAlign;
            m_posRange[0] = posMin - 5000;
            m_posRange[1] = posMax + 5000; 
            m_log = new Log(m_id, m_auto.ClassLogView(), "Aligner");
            m_imgView = new ezImgView(m_id, 1, m_auto.ClassLogView(), this.Font);
        }

        public void ThreadStop()
        {
        }

        private void buttonJogLeft_MouseDown(object sender, MouseEventArgs e)
        {
            if (m_bJog) { m_axis.StopAxis(); m_bJog = false; }
            else if (m_axis.GetPos(true) > m_posRange[0]) { m_axis.Jog(-GetJogV()); m_bJog = true; }
        }

        private void buttonJogRight_MouseDown(object sender, MouseEventArgs e)
        {
            if (m_bJog) { m_axis.StopAxis(); m_bJog = false; }
            else if (m_axis.GetPos(true) < m_posRange[1]) { m_axis.Jog(GetJogV()); m_bJog = true; }
        }

        double GetJogV()
        {
            double V = 1;
            if (m_bKeyShift) V /= 4;
            if (m_bKeyControl) V /= 2;
            return V; 
        }

        private void buttonJogLeft_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_bJog) { m_axis.StopAxis(); m_bJog = false; }
        }

        private void buttonJogRight_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_bJog) { m_axis.StopAxis(); m_bJog = false; }
        }

        private void buttonJogLeft_MouseLeave(object sender, EventArgs e)
        {
            if (m_bJog) { m_axis.StopAxis(); m_bJog = false; }
        }

        private void buttonJogRight_MouseLeave(object sender, EventArgs e)
        {
            if (m_bJog) { m_axis.StopAxis(); m_bJog = false; }
        }

        private void buttonAlign_Click(object sender, EventArgs e)
        {
            m_posAxis = m_axis.GetPos(true); 
            this.DialogResult = System.Windows.Forms.DialogResult.OK; 
            Close();
        }

        private void HW_Aligner_ATI_Manual_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Shift: m_bKeyShift = true; break;
                case Keys.Control: m_bKeyControl = true; break;
            }
        }

        private void HW_Aligner_ATI_Manual_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Shift: m_bKeyShift = false; break;
                case Keys.Control: m_bKeyControl = false; break;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            double pos = m_axis.GetPos(true);
            if ((pos < m_posRange[0]) || (pos > m_posRange[1])) 
            {
                if (m_bJog) m_axis.StopAxis(); m_bJog = false; 
            }
            m_camAlign.Grab(m_imgView.m_pImg, false); 
            Invalidate(false);
            m_log.Add("Timer"); 
        }

        private void HW_Aligner_ATI_Manual_Paint(object sender, PaintEventArgs e)
        {
            m_imgView.Draw(e.Graphics);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (m_imgView.HasImage()) return;
            base.OnPaintBackground(e);
        }

        private void HW_Aligner_ATI_Manual_Load(object sender, EventArgs e)
        {
            timer.Start(); 
        }

        private void HW_Aligner_ATI_Manual_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.Stop(); 
        }

    }
}
