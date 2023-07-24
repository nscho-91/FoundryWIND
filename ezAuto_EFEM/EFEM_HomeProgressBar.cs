using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ezAutoMom;
using ezTools;

namespace ezAuto_EFEM
{
    public partial class EFEM_HomeProgressBar : Form
    {
        Auto_Mom m_auto;
        ezStopWatch m_swWTR = new ezStopWatch();
        ezStopWatch[] m_swLP = new ezStopWatch[3];
        ezStopWatch m_swAligner = new ezStopWatch();
        ezStopWatch m_swVision = new ezStopWatch();
        bool m_bEnableWTR = false;
        bool[] m_bEnableLP = new bool[3];
        bool m_bEnableAligner = false;
        bool m_bEnableVision = false;
        public bool m_bShow = false;
        bool m_bReset = false;
        public EFEM_HomeProgressBar(Auto_Mom auto)
        {
            InitializeComponent();
            m_auto = auto;
            for (int i = 0; i < m_auto.ClassHandler().m_nLoadPort; i++)
            {
                m_swLP[i] = new ezStopWatch();
            }
        }

        public void AddWTR()
        {
            m_bEnableWTR = true;
            m_swWTR.Start();
        }

        public void AddLoadPort(int nID)
        {
            m_bEnableLP[nID] = true;
            m_swLP[nID].Start();
        }

        public void AddAligner()
        {
            m_bEnableAligner = true;
            m_swAligner.Start();
        }

        public void AddVision()
        {
            m_bEnableVision = true;
            m_swVision.Start();
        }

        public void Reset()
        {
            m_bEnableWTR = m_bEnableAligner = m_bEnableVision = false;
            m_swWTR.Reset();
            m_swAligner.Reset();
            m_swVision.Reset();
            for (int i = 0; i < m_auto.ClassHandler().m_nLoadPort; i++)
            {
                m_bEnableLP[i] = false;
                m_swLP[i].Reset();
            }
        }

        public void Progress_WTR()
        {
            labelWTR.Enabled = m_bEnableWTR;
            progressBarWtr.Enabled = m_bEnableWTR;
            if (m_auto.ClassHandler().ClassWTR().GetState() == HW_WTR_Mom.eState.Home)
            {
                labelWTR.Text = "WTR Home Progress : 진행중";
                ChangeColor(progressBarWtr.Handle, eProgressBarColor.Green);
                progressBarWtr.Value = (int)(100 * Math.Min((double)((double)m_swWTR.Check() / m_auto.ClassHandler().ClassWTR().m_msHome), (double)1.0));
            }
            else if (m_auto.ClassHandler().ClassWTR().GetState() == HW_WTR_Mom.eState.Ready)
            {
                labelWTR.Text = "WTR Home Progress : 완료";
                progressBarWtr.Value = 100;
            }
            else if (m_auto.ClassHandler().ClassWTR().GetState() == HW_WTR_Mom.eState.Error)
            {
                labelWTR.Text = "WTR Home Progress : 실패";
                ChangeColor(progressBarWtr.Handle, eProgressBarColor.Red);
            }
            else
            {
                progressBarWtr.Value = 0;
            }
        }

        public void Progress_AL()
        {
            labelAligner.Enabled = m_bEnableAligner;
            progressBarAligner.Enabled = m_bEnableAligner;

            if (m_auto.ClassHandler().ClassAligner().GetState() == HW_Aligner_Mom.eState.Home)
            {
                labelAligner.Text = "Aligner Home Progress : 진행중";
                progressBarAligner.Value = (int)(100 * Math.Min((double)((double)m_swAligner.Check() / m_auto.ClassHandler().ClassAligner().m_msHome), 1));
            }
            else if (m_auto.ClassHandler().ClassAligner().GetState() == HW_Aligner_Mom.eState.Ready || m_auto.ClassHandler().ClassAligner().GetState() == HW_Aligner_Mom.eState.RunReady)
            {
                labelAligner.Text = "Aligner Home Progress : 완료";
                progressBarAligner.Value = 100;
            }
            else if (m_auto.ClassHandler().ClassAligner().GetState() == HW_Aligner_Mom.eState.Error || (m_auto.ClassHandler().ClassAligner().GetState() == HW_Aligner_Mom.eState.Init && m_auto.ClassWork().GetState() == eWorkRun.Error))
            {
                labelAligner.Text = "Aligner Home Progress : 실패";
                ChangeColor(progressBarAligner.Handle, eProgressBarColor.Red);
            }
            else
            {
                progressBarAligner.Value = 0;
            }
        }

        public void Progress_VS()
        {
            labelVision.Enabled = m_bEnableVision;
            progressBarVision.Enabled = m_bEnableVision;

            if (m_auto.ClassHandler().ClassVisionWorks().GetState() == HW_VisionWorks_Mom.eState.Home)
            {
                labelVision.Text = "Vision Home Progress : 진행중";
                progressBarVision.Value = (int)(100 * Math.Min((double)((double)m_swVision.Check() / m_auto.ClassHandler().ClassVisionWorks().m_msHome), 1));
            }
            else if (m_auto.ClassHandler().ClassVisionWorks().GetState() == HW_VisionWorks_Mom.eState.Ready || m_auto.ClassHandler().ClassVisionWorks().GetState() == HW_VisionWorks_Mom.eState.LoadWait)
            {
                labelVision.Text = "Vision Home Progress : 완료";
                progressBarVision.Value = 100;
            }
            else if (m_auto.ClassHandler().ClassVisionWorks().GetState() == HW_VisionWorks_Mom.eState.Error || (m_auto.ClassHandler().ClassVisionWorks().GetState() == HW_VisionWorks_Mom.eState.Init && m_auto.ClassWork().GetState() == eWorkRun.Error))
            {
                labelVision.Text = "Vision Home Progress : 실패";
                ChangeColor(progressBarVision.Handle, eProgressBarColor.Red);
            }
            else
            {
                progressBarVision.Value = 0;
            }
        }

        public void Progress_LP()
        {
            Label[] labelLP = new Label[3];
            ProgressBar[] progressBarLP = new ProgressBar[3];

            for (int i = 0; i < m_auto.ClassHandler().m_nLoadPort; i++)
            {
                labelLP[i] = new Label();
                progressBarLP[i] = new ProgressBar();
            }

            Label lbLP = new Label();
            ProgressBar PbLP = new ProgressBar();

            //------------------------------------------------------------------------------
            Dictionary<Label, Label> dic_lb = new Dictionary<Label, Label>()
                {
                    {labelLP[0],labelLP1},
	                {labelLP[1],labelLP2},
	                {labelLP[2],labelLP3},
                };
            Dictionary<ProgressBar, ProgressBar> dic_Pb = new Dictionary<ProgressBar, ProgressBar>()
                {
                    {progressBarLP[0],progressBarLP1},
	                {progressBarLP[1],progressBarLP2},
	                {progressBarLP[2],progressBarLP3},
                };
            //-------------------------------------------------------------------------------

            for (int i = 0; i < m_auto.ClassHandler().m_nLoadPort; i++)
            {

                dic_lb.TryGetValue(labelLP[i], out lbLP);
                dic_Pb.TryGetValue(progressBarLP[i], out PbLP);

                labelLP[i].Enabled = m_bEnableLP[i];
                progressBarLP[i].Enabled = m_bEnableLP[i];

                dic_lb.TryGetValue(labelLP[i], out lbLP);
                dic_Pb.TryGetValue(progressBarLP[i], out PbLP);
                if (m_auto.ClassHandler().ClassLoadPort(i).GetState() == HW_LoadPort_Mom.eState.Home)
                {
                    lbLP.Text = "Loadport" + (i + 1) + " Home Progress : 진행중";
                    ChangeColor(PbLP.Handle, eProgressBarColor.Green);
                    PbLP.Value = (int)(100 * Math.Min((double)((double)m_swLP[i].Check() / m_auto.ClassHandler().ClassLoadPort(i).m_msHome), 1));
                }
                else if (m_auto.ClassHandler().ClassLoadPort(i).GetState() == HW_LoadPort_Mom.eState.Ready)
                {
                    lbLP.Text = "Loadport" + (i + 1) + " Home Progress : 완료";
                    PbLP.Value = 100;
                }
                else if (m_auto.ClassHandler().ClassLoadPort(i).GetState() == HW_LoadPort_Mom.eState.Error || (m_auto.ClassHandler().ClassLoadPort(i).GetState() == HW_LoadPort_Mom.eState.Init && m_auto.ClassWork().GetState() == eWorkRun.Error))
                {
                    lbLP.Text = "Loadport" + (i + 1) + " Home Progress : 실패";
                    ChangeColor(PbLP.Handle, eProgressBarColor.Red);
                }
                else
                {
                    PbLP.Value = 0;
                }
            }
        }

        private void timerPrgress_Tick(object sender, EventArgs e)
        {
            if (m_bShow) Show();
            else return;
            Progress_WTR();
            Progress_LP();
            Progress_AL();
            Progress_VS();
        }

        private void EFEM_HomeProgressBar_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            m_bShow = false;
            this.Hide();
        }

        enum eProgressBarColor { Green = 1, Red, Yellow }
        void ChangeColor(IntPtr hWnd, eProgressBarColor eColor)
        {
            SendMessage(hWnd, 1040, (IntPtr)(int)eColor, IntPtr.Zero);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);

    }
}
