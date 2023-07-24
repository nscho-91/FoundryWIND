using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
    public partial class HW_Aligner_EBR_EBR_Graph : DockContent
    {
        string m_id;
        Auto_Mom m_auto;
        Log m_log;

        bool m_bInvalid = false;
        int m_lInspect = 0;
        public int m_nInspect = 32;

        public double m_umX = 3.5;
        public double m_degOffset = 1;
        public double m_degEnd = 360; // ing 170531

        public int[] m_yEBR = null;
        public double[,] m_xEdge = null;
        public double[] m_lEBR = null;
        public double[] m_lBevel = null;

        public HW_Aligner_EBR_EBR_Graph()
        {
            InitializeComponent();
        }

        private void HW_Aligner_EBR_EBR_Graph_Load(object sender, EventArgs e)
        {
        }

        public void Init(string id, Auto_Mom auto, Log log, bool bTrim = false)
        {
            m_id = id;
            this.Text = id; 
            m_auto = auto;
            m_log = log; 
            ReAllocate();
            if (bTrim)
            {
                listViewEBR.Columns.RemoveAt(1);
                listViewEBR.Columns[1].Text = "Trim";
                chartEBR.Series["Bevel"].LegendText = "Trim";
            }
        }

        public bool IsPersistString(string str)
        {
            return GetPersistString() == str;
        }

        public void ReAllocate()
        {
            if (m_nInspect == m_lInspect) return;
            m_lInspect = m_nInspect;
            m_yEBR = new int[m_nInspect];
            m_xEdge = new double[m_nInspect, 3];
            m_lEBR = new double[m_nInspect];
            m_lBevel = new double[m_nInspect];
        }

        public bool Invalid(int msWait)
        {
            ezStopWatch sw = new ezStopWatch();
            m_bInvalid = true;
            while (m_bInvalid && sw.Check() < msWait) Thread.Sleep(1);
            if (sw.Check() > msWait) return true;
            return false;
        }

        public void InvalidData(bool bTrim = false)
        {
            if (bTrim)
            {
                Random rand = new Random();
                chartEBR.Series["Bevel"].Points.Clear();
                for (int i = 0; i < m_nInspect; i++)
                {
                    double x = 360.0 * i / m_nInspect + m_degOffset;
                    chartEBR.Series["Bevel"].Points.AddXY(x, m_lBevel[i]);
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
                        item.SubItems.Add(m_lBevel[i].ToString(".00"));
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
            else
            {
                Random rand = new Random();
                chartEBR.Series["EBR"].Points.Clear();
                chartEBR.Series["Bevel"].Points.Clear();
                for (int i = 0; i < m_nInspect; i++)
                {
                    double x = 360.0 * i / m_nInspect + m_degOffset;
                    chartEBR.Series["EBR"].Points.AddXY(x, m_lEBR[i]);
                    chartEBR.Series["Bevel"].Points.AddXY(x, m_lBevel[i]);
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
                        item.SubItems.Add(m_lEBR[i].ToString(".00"));
                        item.SubItems.Add(m_lBevel[i].ToString(".00"));
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
        }

        public void RunGrid(ezGrid rGrid, eGrid eMode)
        {
            rGrid.Set(ref m_umX, "Camera", "um", "(um / pixel)");
        }

        public void RecipeGrid(ezGrid rGrid, eGrid eMode, ezJob job, string strGroup = "EBR")
        {
            rGrid.Set(ref m_nInspect, strGroup, "Count", strGroup + " Inspect Count");
            rGrid.Set(ref m_degOffset, strGroup, "Offset", "Inspect Offset (deg)");
            rGrid.Set(ref m_degEnd, strGroup, "EndPoint", "Inspect End Point (deg)");
        }

        public void ResetDefaultParam()
        {
            m_nInspect = 32;
            m_degOffset = 1;
            m_degEnd = 360;
        }

        public void SortEdge(int nIndex)
        {
            SortEdge(nIndex, 0, 1);
            SortEdge(nIndex, 1, 2);
            SortEdge(nIndex, 0, 1);
            m_lEBR[nIndex] = (m_xEdge[nIndex, 2] - m_xEdge[nIndex, 0]) * m_umX;
            m_lBevel[nIndex] = (m_xEdge[nIndex, 2] - m_xEdge[nIndex, 1]) * m_umX;
        }

        public void SortEdgeTrim(int nIndex)
        {
            SortEdge(nIndex, 0, 1);
            m_lBevel[nIndex] = (m_xEdge[nIndex, 1] - m_xEdge[nIndex, 0]) * m_umX;
        }


        void SortEdge(int nIndex, int i0, int i1)
        {
            if (m_xEdge[nIndex, i0] < m_xEdge[nIndex, i1]) return;
            double v = m_xEdge[nIndex, i0];
            m_xEdge[nIndex, i0] = m_xEdge[nIndex, i1];
            m_xEdge[nIndex, i1] = v;
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

        public int GetBevelEdge(int y, int lY)
        {
            int nIndex = 0;
            int yDistMin = GetDist(y - m_yEBR[0], lY);
            for (int n = 0; n < m_nInspect; n++)
            {
                int yDist = GetDist(y - m_yEBR[n], lY);
                if (yDistMin > yDist)
                {
                    nIndex = n;
                    yDistMin = yDist;
                }
            }
            return (int)m_xEdge[nIndex, 1];
        }

        public int GetTrimEdge(int y, int lY)
        {
            int nIndex = 0;
            int yDistMin = GetDist(y - m_yEBR[0], lY);
            for (int n = 0; n < m_nInspect; n++)
            {
                int yDist = GetDist(y - m_yEBR[n], lY);
                if (yDistMin > yDist)
                {
                    nIndex = n;
                    yDistMin = yDist;
                }
            }
            return (int)m_xEdge[nIndex, 0];
        }

        int GetDist(int dy, int lY)
        {
            dy = Math.Abs(dy);
            while (dy > lY) dy -= lY;
            return dy; 
        }

    }
}
