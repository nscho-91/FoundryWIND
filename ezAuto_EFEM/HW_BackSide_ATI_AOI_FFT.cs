using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ezTools; 

namespace ezAuto_EFEM
{
    public class HW_BackSide_ATI_AOI_FFT
    {
        public string m_id;
        CPoint m_sz = new CPoint(0, 0);
        CPoint m_cpCenter = new CPoint(0, 0);
        double m_dR = 0;
        double m_fSum = 0.0;
        public string m_strCode = "000";

        bool m_bFFTError = false;
        bool m_bDetect = false;
        bool m_bUse = true;

        Log m_log;
        ezImg m_imgDown;

        const int c_nFFT = 512;
        const int c_nPhase = 200;

        double[] m_aData = new double[c_nFFT];
        double[] m_aPhase = new double[c_nPhase];

        double[] m_aCos = new double[c_nFFT];
        double[] m_aSin = new double[c_nFFT];

        double m_fSigma = 5;

        public ArrayList m_aList; 


        HW_BackSide_ATI_AOIData m_data; 

        public HW_BackSide_ATI_AOI_FFT()
        {
        }

        public void Init(string id, HW_BackSide_ATI_AOIData data, Log log)
        {
            m_id = id;
            m_data = data; 
            m_log = log;
            for (int n = 0; n < c_nFFT; n++)
            {
                m_aCos[n] = Math.Cos(2 * Math.PI * n / c_nFFT);
                m_aSin[n] = Math.Sin(2 * Math.PI * n / c_nFFT);
            }
            m_aList = new ArrayList();
        }

        public void DrawPhase(Graphics dc, ezImgView view, bool bDraw)
        {
            if (bDraw == false) return;
            if (m_imgDown == null) return;
            int xGap = 40;
            CPoint cp1 = new CPoint(10 * xGap, (int)(m_aPhase[10] / 2));
            CPoint cp0 = new CPoint(cp1);
            double fPhaseMax = 0;
            for (int n = 11; n < c_nPhase; n++)
            {
                if (fPhaseMax < m_aPhase[n]) fPhaseMax = m_aPhase[n];
                cp1 = new CPoint(n * xGap, (int)(m_aPhase[n] * 4));
                view.DrawLine(dc, Color.Red, cp0, cp1);
                cp0 = cp1;
            }

            Pen pen = new Pen(Color.FromArgb(255, 255, 0), 100);
            if (m_bDetect)
            {
                view.DrawLine(dc, Color.Red, new CPoint(m_cpCenter.x * m_data.m_nDown, m_cpCenter.y * m_data.m_nDown), new CPoint((int)(m_cpCenter.x + m_dR) * m_data.m_nDown, m_cpCenter.y * m_data.m_nDown));
                view.DrawString(dc, "Grind:" + Math.Round(m_fSum, 2).ToString(), new CPoint(m_cpCenter.x * m_data.m_nDown, m_cpCenter.y * m_data.m_nDown));
            }
            else
            {
                view.DrawLine(dc, Color.Blue, new CPoint(m_cpCenter.x * m_data.m_nDown, m_cpCenter.y * m_data.m_nDown), new CPoint((int)(m_cpCenter.x + m_dR) * m_data.m_nDown, m_cpCenter.y * m_data.m_nDown));
                view.DrawString(dc, "Grind:" + Math.Round(m_fSum, 2).ToString(), new CPoint(m_cpCenter.x * m_data.m_nDown, m_cpCenter.y * m_data.m_nDown));
            }
        }

        public void RunGrid(ezGrid grid, eGrid eMode, ezJob job = null)
        {
            grid.Set(ref m_bUse, m_id, "Use", "Use"); 
            grid.Set(ref m_fSigma, m_id, "Sigma", "Small Value more Sensitive (3 ~ 8)");
            grid.Set(ref m_strCode, m_id, "Code", "Defect Code");
        }

        public bool Inspect(double dR)
        {
            m_fSum = 0.0;
            ezStopWatch sw = new ezStopWatch();
            m_imgDown = m_data.m_imgDown;
            m_cpCenter = m_data.m_cpCenterDown;
            m_dR = dR / m_data.m_nDown;
            m_data.CalcEdge(m_dR); 
            m_bFFTError = false;
            m_aList.Clear();
            if (!m_bUse) return false;
            if (!m_imgDown.HasImage()) return true;
            m_log.Add(m_id + " - Start FFT Inspection.");

            for (double R = 0.1; R < 0.95; R += 0.1) InspectR(R);
            m_log.Add(m_id + " - AOI Wheel Mark : " + sw.Check().ToString());
            for (int i = 0; i < m_aList.Count; i++)
            {
                m_fSum += (double)m_aList[i];
            }
            m_fSum /= m_aList.Count;
            if (m_fSum > m_fSigma) m_bDetect = true;
            else m_bDetect = false;
            return m_bFFTError;
        }

        void InspectR(double R)
        {
            int nCount = 0;
            double fSum = 0;
            double fSum2 = 0;
            double fMax = 0;
            SetData(R * m_dR);
            for (int i = 10; i < c_nPhase; i++)
            {
                double reSum = 0;
                double imSum = 0;
                for (int x = 0; x < c_nFFT; x++)
                {
                    int n = (i * x) % c_nFFT;
                    reSum += m_aData[x] * m_aCos[n];
                    imSum += -m_aData[x] * m_aSin[n];
                }
                m_aPhase[i] = Math.Sqrt(reSum * reSum + imSum * imSum);
                nCount++;
                fSum += m_aPhase[i];
                fSum2 += (m_aPhase[i] * m_aPhase[i]);
                if (fMax < m_aPhase[i]) fMax = m_aPhase[i];
            }
            double fAve = fSum / nCount;
            double fAve2 = fSum2 / nCount;
            double fSigma = Math.Sqrt(fAve2 - fAve * fAve);
            double fScore = (fMax - fAve) / fSigma;
            if (fScore > m_fSigma) m_bFFTError = true;
            m_aList.Add(fScore);
            m_log.Add("FFT " + R.ToString("0.0") + "R " + fScore.ToString("0.00"));

        }

        void SetData(double R)
        {
            for (int n = 0; n < c_nFFT; n++)
            {
                int x = m_cpCenter.x + (int)(R * m_aCos[n]);
                int y = m_cpCenter.y + (int)(R * m_aSin[n]);
                m_aData[n] = m_imgDown.m_aBuf[y, x];
/*                m_aData[n] = (m_imgDown.m_aBuf[y - 1, x - 1] + m_imgDown.m_aBuf[y - 1, x] + m_imgDown.m_aBuf[y - 1, x + 1] +
                    m_imgDown.m_aBuf[y, x - 1] + m_imgDown.m_aBuf[y, x] + m_imgDown.m_aBuf[y, x + 1] +
                    m_imgDown.m_aBuf[y + 1, x - 1] + m_imgDown.m_aBuf[y + 1, x] + m_imgDown.m_aBuf[y + 1, x + 1]) / 9; */
            }
        }

        public bool IsDefect()
        {
            return m_bDetect;
        }

        public void ClearDefect()
        {
            m_fSum = 0;
            m_bDetect = false;
        }
    }
}
