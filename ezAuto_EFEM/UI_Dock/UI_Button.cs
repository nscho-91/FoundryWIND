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
    public partial class UI_Button : DockContent
    {
        Auto_Mom m_auto = null;
        Handler_Mom m_handler = null;
        Work_Mom m_work = null;
        Log m_log = null;

        public UI_Button()
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

        private void UI_Button_SizeChanged(object sender, EventArgs e)
        {
        
        }

        private void UI_Button_Resize(object sender, EventArgs e)
        {
          
        }

        private void btnStart_Resize(object sender, EventArgs e)
        {
            if (this.Width != 0 && this.Height != 0) 
            {
                Font ft;
                Graphics gp;
                SizeF sz;
                Single Faktor, FaktorX, FaktorY;

                gp = btnStart.CreateGraphics();
                sz = gp.MeasureString(btnStart.Text, btnStart.Font);
                gp.Dispose();

                FaktorX = (btnStart.Width) / sz.Width * (float)0.3;
                FaktorY = (btnStart.Height) / sz.Height * (float)0.3;

                if (FaktorX > FaktorY)
                    Faktor = FaktorY;
                else
                    Faktor = FaktorX;
                ft = btnStart.Font;

                if (ft.SizeInPoints * (Faktor) > 9) 
                {
                    btnStart.Font = new Font(ft.Name, ft.SizeInPoints * (Faktor));
                    btnBuzzerOff.Font = new Font(ft.Name, ft.SizeInPoints * (Faktor));
                    btnCycleStop.Font = new Font(ft.Name, ft.SizeInPoints * (Faktor));
                    btnHome.Font = new Font(ft.Name, ft.SizeInPoints * (Faktor));
                    btnRecovery.Font = new Font(ft.Name, ft.SizeInPoints * (Faktor));
                    btnReset.Font = new Font(ft.Name, ft.SizeInPoints * (Faktor));
                    btnStop.Font = new Font(ft.Name, ft.SizeInPoints * (Faktor));
                }
                else 
                {
                    btnStart.Font = new Font(ft.Name, 9);
                    btnBuzzerOff.Font = new Font(ft.Name, 9);
                    btnCycleStop.Font = new Font(ft.Name, 9);
                    btnHome.Font = new Font(ft.Name, 9);
                    btnRecovery.Font = new Font(ft.Name, 9);
                    btnReset.Font = new Font(ft.Name, 9);
                    btnStop.Font = new Font(ft.Name, 9);
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            m_handler.RunStart(); 
        }

        private void btnRecovery_Click(object sender, EventArgs e)
        {
            m_handler.RunRecover(); 
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            m_handler.RunStop(); 
        }

        private void btnCycleStop_Click(object sender, EventArgs e)
        {
            if (m_work.IsRun()) 
            {
                m_handler.ClassWTR().m_bCycleStop = true;
            }
        }

        private void btnBuzzerOff_Click(object sender, EventArgs e)
        {
            m_work.RunBuzzerOff(); 
        }
         
        private void btnReset_Click(object sender, EventArgs e)
        {
            if (m_work.m_bTestRun == false) m_work.m_run.Reset();
            else {
                char[] buf = new char[4096];
                int nLength=1;

                nLength = 3;
                buf[0] = 'D';
                buf[1] = 'R';
                buf[2] = 'T';
                buf[3] = (char)0x0D;
                buf[4] = (char)0x0A;

                string str;
                str = nLength.ToString() + "," + new string(buf);
                m_log.Add("Read : " + str);
            }
            
            //switch (a) {
            //    case 0:
            //        m_auto.GetEFEMUI().SetWTRMotion(eWTRSimnulMotion.Home);
            //        a++;
            //        break;
            //    case 1:
            //        m_auto.GetEFEMUI().SetWTRMotion(eWTRSimnulMotion.LP1GetUArm);
            //        a++;
            //        break;
            //    case 2:
            //        m_auto.GetEFEMUI().SetWTRMotion(eWTRSimnulMotion.AlignerPutUArm);
            //        a++;
            //        break;
            //    case 3:
            //        m_auto.GetEFEMUI().SetWTRMotion(eWTRSimnulMotion.LP1GetUArm);
            //        a++;
            //        break;
            //    case 4:
            //        m_auto.GetEFEMUI().SetWTRMotion(eWTRSimnulMotion.AlignerGetLArm);
            //        a++;
            //        break;
            //    case 5:
            //        m_auto.GetEFEMUI().SetWTRMotion(eWTRSimnulMotion.AlignerPutUArm);
            //        a++;
            //        break;
            //    case 6:
            //        m_auto.GetEFEMUI().SetWTRMotion(eWTRSimnulMotion.VisionPutLArm);
            //        a++;
            //        break;
            //    case 7:
            //        m_auto.GetEFEMUI().SetWTRMotion(eWTRSimnulMotion.AlignerGetLArm);
            //        a++;
            //        break;
            //    case 8:
            //        m_auto.GetEFEMUI().SetWTRMotion(eWTRSimnulMotion.VisionGetUArm);
            //        a++;
            //        break;
            //    case 9:
            //        m_auto.GetEFEMUI().SetWTRMotion(eWTRSimnulMotion.VisionPutLArm);
            //        a++;
            //        break;
            //    case 10:
            //        m_auto.GetEFEMUI().SetWTRMotion(eWTRSimnulMotion.LP1PutUArm);
            //        a++;
            //        break;
            //    case 11:
            //        m_auto.GetEFEMUI().SetWTRMotion(eWTRSimnulMotion.VisionGetUArm);
            //        a++;
            //        break;
            //    case 12:
            //        m_auto.GetEFEMUI().SetWTRMotion(eWTRSimnulMotion.LP1PutUArm);
            //        a++;
            //        break;
            //    default:
            //        a = 0;
            //        break;
            //}
            
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            m_work.m_run.Home(); 
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            bool bCoverOpen = false;
            bool bDoorOpen = false;
            for (int n = 0; n < m_handler.m_nLoadPort; n++) 
            {
                if (!m_handler.ClassLoadPort(n).IsCoverClose()) bCoverOpen = true;
                if (!m_handler.ClassLoadPort(n).IsDoorOpenPos()) bDoorOpen = true;
            }
            this.Invalidate();
            btnStart.Enabled = m_work.m_run.IsEnableSW() && (m_work.GetState() == eWorkRun.Ready) && m_handler.m_bEnableStart;
            btnStop.Enabled = m_work.m_run.IsEnableSW() && ((m_work.GetState() == eWorkRun.Run) || (m_work.GetState() == eWorkRun.AutoUnload));
            btnCycleStop.Enabled = !m_handler.ClassWTR().m_bCycleStop && m_work.m_run.IsEnableSW() && (m_work.GetState() == eWorkRun.Run) && m_handler.IsWaferOut();
            btnRecovery.Enabled = m_work.m_run.IsEnableSW() && bDoorOpen && (m_work.GetState() == eWorkRun.Ready) && !m_handler.m_bEnableStart;
            btnHome.Enabled = m_work.m_run.IsEnableSW() && !bCoverOpen && ((m_work.GetState() == eWorkRun.Init) || (m_work.GetState() == eWorkRun.Ready));
            btnReset.Enabled = m_work.m_run.IsEnableSW() && ((m_work.GetState() == eWorkRun.Error) || (m_work.GetState() == eWorkRun.Warning0) || (m_work.GetState() == eWorkRun.Warning1));
            btnBuzzerOff.Enabled = m_work.m_run.IsEnableSW() && m_work.IsBuzzerOn();

            if (btnStart.Enabled)
                btnStart.BackColor = SystemColors.InactiveBorder;
            else
                btnStart.BackColor = SystemColors.Control;
            if (btnStop.Enabled)
                btnStop.BackColor = SystemColors.InactiveCaption;
            else
                btnStop.BackColor = SystemColors.Control;
            if (btnCycleStop.Enabled)
                btnCycleStop.BackColor = SystemColors.InactiveCaption;
            else
                btnCycleStop.BackColor = SystemColors.Control;
            if (btnRecovery.Enabled)
                btnRecovery.BackColor = SystemColors.InactiveCaption;
            else
                btnRecovery.BackColor = SystemColors.Control;
            if (btnHome.Enabled)
                btnHome.BackColor = SystemColors.InactiveCaption;
            else
                btnHome.BackColor = SystemColors.Control;
            if (btnReset.Enabled)
                btnReset.BackColor = SystemColors.InactiveCaption;
            else
                btnReset.BackColor = SystemColors.Control;
            if (btnBuzzerOff.Enabled)
                btnBuzzerOff.BackColor = SystemColors.InactiveCaption;
            else
                btnBuzzerOff.BackColor = SystemColors.Control;

        }

    }
}
