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
using ezAutoMom;
using ezTools; 


namespace ezAuto_EFEM
{
    public partial class UI_STATE : DockContent
    {
        Auto_Mom m_auto = null;
        Handler_Mom m_handler = null;
        Work_Mom m_work = null;
        Log m_log = null;


        public UI_STATE()
        {
            InitializeComponent();
        }

        public void Init(Auto_Mom auto, Log log)
        {
            m_auto = auto;
            m_handler = m_auto.ClassHandler();
            m_work = m_auto.ClassWork();
            m_log = log;
        }

        public void ShowPanel(DockPanel dockpanel)
        {
            this.Show(dockpanel);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string lbText = "";
            Color lbColor = Color.Yellow;
            switch (m_auto.ClassWork().m_run.GetState()) {
                case eWorkRun.Init:
                    lbText = "INIT";
                    lbColor = Color.Wheat;
                    break;
                case eWorkRun.Ready:
                    lbText = "STOP";
                    lbColor = Color.LemonChiffon;
                    break;
                case eWorkRun.Run:
                    lbText = "RUN";
                    lbColor = Color.SpringGreen;
                    break;
                case eWorkRun.Home:
                    lbText = "HOME";
                    lbColor = Color.DeepSkyBlue;
                    break;
                case eWorkRun.Error:
                    lbText = "ERROR";
                    lbColor = Color.IndianRed;
                    break;
                case eWorkRun.Warning0:
                case eWorkRun.Warning1:
                    lbColor = Color.Tomato;
                    lbText = "WARNING";
                    break;
                case eWorkRun.AutoUnload:
                    lbText = "RECOVERY";
                    lbColor = Color.LightSkyBlue;
                    break;
            }
            label1.Text = lbText;
            label1.BackColor = lbColor;
        }

        private void UI_STATE_Resize(object sender, EventArgs e)
        {
            if (this.Width > 30 && this.Height > 30) {
                Font ft;
                Graphics gp;
                SizeF sz;
                Single Faktor, FaktorX, FaktorY;

                gp = label1.CreateGraphics();
                sz = gp.MeasureString(label1.Text, label1.Font);
                gp.Dispose();

                if (sz.Width == 0 || sz.Height == 0) return;

                FaktorX = (label1.Width) / sz.Width * (float)0.8;
                FaktorY = (label1.Height) / sz.Height * (float)0.8;

                if (FaktorX > FaktorY)
                    Faktor = FaktorY;
                else
                    Faktor = FaktorX;
                ft = label1.Font;
                label1.Font = new Font(ft.Name, ft.SizeInPoints * (Faktor));
            }
        }

        private void UI_STATE_Paint(object sender, PaintEventArgs e)
        {
            if (this.Width > 30 && this.Height > 30)
            {
                Font ft;
                Graphics gp;
                SizeF sz;
                Single Faktor, FaktorX, FaktorY;

                gp = label1.CreateGraphics();
                sz = gp.MeasureString(label1.Text, label1.Font);
                gp.Dispose();

                if (sz.Width == 0 || sz.Height == 0) return;

                FaktorX = (label1.Width) / sz.Width * (float)0.8;
                FaktorY = (label1.Height) / sz.Height * (float)0.8;

                if (FaktorX > FaktorY)
                    Faktor = FaktorY;
                else
                    Faktor = FaktorX;
                ft = label1.Font;
                label1.Font = new Font(ft.Name, ft.SizeInPoints * (Faktor));
            }
        }

    }
}
