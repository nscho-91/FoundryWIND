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

namespace ezAutoMom
{
    public partial class TrayMonitor : Form
    {
        Log m_log;
        Count_Mom m_count;
        Screen m_screen = null;
        Rectangle m_rtWin;
        Font m_font;

        public TrayMonitor()
        {
            InitializeComponent();
        }
        
        public void Init(Count_Mom count, int indexMonitor, Log log)
        {
            int index = 1;
            m_log = log;
            m_count = count;
            foreach(Screen screen in Screen.AllScreens)
            {
                if(index == indexMonitor) m_screen = screen;
                index++;
            }
            if (m_screen == null)
            {
                log.Popup("TrayMonitor Index Error !!"); Dispose(); return;
            }
            m_rtWin = m_screen.WorkingArea;
            Location = new Point(m_rtWin.X, m_rtWin.Y);
            Size = new Size(m_rtWin.Width, m_rtWin.Height);
            this.BackColor = Color.FromArgb(255, 255, 255);
            Show();
        }

        private void TrayMonitor_Paint(object sender, PaintEventArgs e)
        {
            int x, y;
            Location = new Point(m_rtWin.X, m_rtWin.Y);
            CPoint cp = new CPoint(10, (Size.Height * 9 / 10) - (Size.Height * 9 / 10) / (m_count.GetSize().y));
            m_font = new Font(Font.FontFamily, m_rtWin.Height / m_count.GetSize().y / 4, FontStyle.Bold);
            for (y = 0; y < m_count.GetSize().y; y++)
            {
                cp.x = 10;
                for(x = 0; x < m_count.GetSize().x; x++)
                {
                    m_count.GetTray(y * m_count.GetSize().x + x).DrawMonitor(e.Graphics, cp, m_font);
                    cp.x += Size.Width / m_count.GetSize().x;
                }
                //cp.y += (Size.Height * 9 / 10) / m_count.GetSize().y;
                cp.y -= (Size.Height * 9 / 10) / m_count.GetSize().y;
            }
            buttonClear.Size = buttonPrint.Size = new Size(Size.Width / 10, Size.Height * 1 / 12);
            buttonPrint.Location = new Point(Size.Width / 2 - buttonPrint.Size.Width - 20, Size.Height * 9 / 10 - 10);
            buttonClear.Location = new Point(Size.Width / 2 + buttonClear.Size.Width - 20, Size.Height * 9 / 10 - 10);
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            m_count.Print();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            m_count.ClearTray();
        }

        private void TrayMonitor_Resize(object sender, EventArgs e)
        {
            /*Control control = (Control)sender;

            if (control.Size.Height != control.Size.Width)
            {
                control.Size = new Size(control.Size.Width, control.Size.Width);
            }*/
            Invalidate();
        }

        private void TrayMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
