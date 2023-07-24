using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using ezAutoMom;
using ezTools;

namespace ezAuto_EFEM
{
    public partial class HW_Aligner_EBR_3D_Graph : DockContent
    {
        string m_id;
        Auto_Mom m_auto;
        Log m_log;

        bool m_bInvalid = false;
        int m_lInspect = 0;
        public int m_nInspect = 32;

        public double m_degOffset = 1;
        public double m_degEnd = 360; // ing 170531

        public double[] m_lHeight = null;
        public double[] m_lWidth = null;
        public double[] m_lStep = null;

        public HW_Aligner_EBR_3D_Graph()
        {
            InitializeComponent();
        }

        public void Init(string id, Auto_Mom auto, Log log)
        {
            m_id = id;
            this.Text = id;
            m_auto = auto;
            m_log = log;
            ReAllocate();
        }

        public bool IsPersistString(string str)
        {
            return GetPersistString() == str;
        }

        public void ReAllocate()
        {
            if (m_nInspect == m_lInspect) return;
            m_lInspect = m_nInspect;
            m_lHeight = new double[m_nInspect];
            m_lWidth = new double[m_nInspect];
            m_lStep = new double[m_nInspect];
        }

        public bool Invalid(int msWait)
        {
            ezStopWatch sw = new ezStopWatch();
            m_bInvalid = true;
            while (m_bInvalid && sw.Check() < msWait) Thread.Sleep(1);
            if (sw.Check() > msWait) return true;
            return false;
        }

        public void InvalidData()
        {
            Random rand = new Random();
            chartEBR.Series["Height"].Points.Clear();
            for (int i = 0; i < m_nInspect; i++)
            {
                double x = 360.0 * i / m_nInspect + m_degOffset;
                chartEBR.Series["Height"].Points.AddXY(x, m_lHeight[i]);
            }

            try
            {
                Size sz = listViewEBR.Size;
                sz.Height = 30 + 16 * m_nInspect;
                listViewEBR.Size = sz;
                listViewEBR.Items.Clear();
                for (int i = 0; i < m_nInspect; i++)
                {
                    ListViewItem item = new ListViewItem((i + 1).ToString());
                    item.SubItems.Add(m_lHeight[i].ToString(".00"));
                    item.SubItems.Add(m_lWidth[i].ToString(".0"));
                    item.SubItems.Add((360.0 * i / m_nInspect + m_degOffset).ToString(".00"));
                    listViewEBR.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            chartEBR.Invalidate();
            listViewEBR.Invalidate();
            Invalidate(); 
        }

        public void RunGrid(ezGrid rGrid, eGrid eMode)
        {
//            rGrid.Set(ref m_umX, "Camera", "um", "(um / pixel)");
        }

        public void RecipeGrid(ezGrid rGrid, eGrid eMode, ezJob job)
        {
            rGrid.Set(ref m_nInspect, "3D", "Count", "EBR Inspect Count");
            rGrid.Set(ref m_degOffset, "3D", "Offset", "Inspect Offset (deg)");
            rGrid.Set(ref m_degEnd, "3D", "EndPoint", "Inspect End Point (deg)");
        }

        public void SortEdge(int nIndex)
        {
            //forget Calc
//            m_lEBR[nIndex] = (m_xEdge[nIndex, 2] - m_xEdge[nIndex, 0]) * m_umX;
        }

        public CPoint GetSize()
        {
            CPoint cpSize = new CPoint();
            if (chartEBR.Size.Width > listViewEBR.Size.Width) cpSize.x = chartEBR.Size.Width;
            else cpSize.x = listViewEBR.Width;
            cpSize.y = chartEBR.Size.Height + listViewEBR.Height;
            return cpSize;
        }

        public bool AddImage(Bitmap bmp, int x, int y)
        {
            try
            {
                chartEBR.DrawToBitmap(bmp, new Rectangle(x, y, chartEBR.Size.Width, chartEBR.Size.Height));
                listViewEBR.DrawToBitmap(bmp, new Rectangle(x, y + chartEBR.Size.Height, listViewEBR.Width, listViewEBR.Height));
            }
            catch (Exception ex)
            {
                m_log.Add(ex.Message);
                return true;
            }
            return false;
        }

        public double GetAngle(int nIndex)
        {
            return m_degOffset + nIndex * m_degEnd / m_nInspect; 
        }

        int m_gapInsp = 10;
        public void RunInspect(int nIndex, double[] aBufH, int minH, double umCam, double umX)
        {
            int nEnd = 0;
            while ((nEnd < aBufH.Length) && (aBufH[nEnd] > 0)) nEnd++;
            double nSumL = 0;
            for (int n = 0; n < m_gapInsp; n++) nSumL += aBufH[n];
            double nSumR = 0;
            for (int n = m_gapInsp + 1; n <= m_gapInsp + 10; n++) nSumR += aBufH[n];
            int nEdge = 0;
            int vMax = 0;
            for (int n = m_gapInsp; n < nEnd - m_gapInsp; n++)
            {
                int nSub = (int)Math.Abs(nSumR - nSumL);
                if (vMax < nSub)
                {
                    vMax = nSub;
                    nEdge = n;
                }
                nSumL += aBufH[n] - aBufH[n - m_gapInsp];
                nSumR += aBufH[n + m_gapInsp + 1] - aBufH[n + 1];
            }
            m_lWidth[nIndex] = umX * (nEnd - nEdge);
            if ((vMax < minH) || (m_lWidth[nIndex] < 2 * m_gapInsp)) m_lHeight[nIndex] = 0;
            else
            {
                double x2b = 0;
                double xb = 0;
                double xyb = 0;
                double yb = 0;
                int N = nEdge - m_gapInsp;
                for (int x = 0; x < N; x++)
                {
                    double fX = x * umX; 
                    xb += fX;
                    x2b += (fX * fX);
                    yb += aBufH[x];
                    xyb += (fX * aBufH[x]);
                }
                xb /= N;
                x2b /= N;
                yb /= N;
                xyb /= N;
                double a = (xyb - xb * yb) / (x2b - xb * xb);
                double b = yb - a * xb;
                double x0 = umX * (nEdge + m_gapInsp + m_gapInsp / 2) / 2.0;
                double y0 = 0;
                for (int x = nEdge + m_gapInsp; x < nEdge + 2 * m_gapInsp; x++) y0 += aBufH[x];
                y0 /= m_gapInsp;
                m_lHeight[nIndex] = umCam * Math.Abs(a * x0 - y0 + b) / Math.Sqrt(a * a + 1);
                m_lStep[nIndex] = umCam * y0; 
            }
        }

    }
}
