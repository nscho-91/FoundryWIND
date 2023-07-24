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
    public partial class UI_Time : DockContent
    {
        public UI_Time()
        {
            InitializeComponent();
        }

        public void ShowPanel(DockPanel dockpanel)
        {
            this.Show(dockpanel);
        }

        public void SetTime()
        {
            btnDay.Text = DateTime.Now.ToString("MM월 dd일");
            btnTime.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void btnDay_Resize(object sender, EventArgs e)
        {
            if (this.Width != 0 && this.Height != 0) {
                Font ft;
                Graphics gp;
                SizeF sz;
                Single Faktor, FaktorX, FaktorY;

                gp = btnDay.CreateGraphics();
                sz = gp.MeasureString(btnDay.Text, btnDay.Font);
                gp.Dispose();

                FaktorX = (btnDay.Width) / sz.Width * (float)0.8;
                FaktorY = (btnDay.Height) / sz.Height * (float)0.8;

                if (FaktorX > FaktorY)
                    Faktor = FaktorY;
                else
                    Faktor = FaktorX;
                ft = btnDay.Font;

                if (ft.SizeInPoints * (Faktor) > 9) {
                    btnDay.Font = new Font(ft.Name, ft.SizeInPoints * (Faktor) * (float)0.9);
                    btnTime.Font = new Font(ft.Name, ft.SizeInPoints * (Faktor));
                }
                else {
                    btnDay.Font = new Font(ft.Name, 9);
                    btnTime.Font = new Font(ft.Name, 9);
                }
            }
        }

    }
}
