using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using WeifenLuo.WinFormsUI.Docking;
using ezAxis;
using ezAutoMom;
using ezTools;
using ezCam;
using ezATI3D_CLR;
using System.Xml;
using System.Xml.Serialization;

namespace ezAuto_EFEM
{
    public partial class HW_Aligner_EBR_3D : Form, Control_Child
    {
        string m_id;
        Auto_Mom m_auto;
        Control_Mom m_control;
        Handler_Mom m_handler;
        Log m_log;
        ezGrid m_grid;

        HW_Aligner_EBR_AOI m_AOI;
        public HW_Aligner_EBR_3D_Graph m_3DGraph = new HW_Aligner_EBR_3D_Graph(); 

        ezView m_viewAlign;
        ezImg m_imgAlign;

        ezView m_view3D;
        ezImg m_img3D;

        public ezCam_Silicon m_cam3D = new ezCam_Silicon();
        ATI_3D m_3D = new ATI_3D(); 

        public int m_dTrigger = 36; 
        int m_lCoSin = 0;
        double[] m_cos;
        double[] m_sin;
        
        const int c_lEdge = 1550;
        const int c_R = 645;
        CPoint c_szEdge = new CPoint(c_lEdge, c_lEdge);
        CPoint c_cpCenter = new CPoint(c_lEdge / 2, c_lEdge / 2);

        Axis_Mom m_axisX;
        Axis_Mom m_axisZ;
        int m_xCam = 0;
        int m_zCam = 0; 

        public bool m_bEnable = false;
        public bool m_bUse = false;

        int m_msGrab = 60000;

        int m_nThread = 12; 
        bool m_bRun = true;
        bool[] m_bInspect = null; 
        Thread[] m_thread = null;

        ushort[] m_aBufH = new ushort[2];
        byte[] m_aBufB = null;

        double m_xScale = -0.05;

        Info_Wafer m_infoWafer = null; 
        
        Size[] m_sz = new Size[2];

        bool m_bSave = false;

        public int m_umWafer = 700;
        public int m_umWafer0 = 700; 

        public HW_Aligner_EBR_3D()
        {
            InitializeComponent();
            m_sz[0] = m_sz[1] = this.Size;
            m_sz[0].Height = 26;
        }

        unsafe public void Init(string id, Auto_Mom auto, ezView viewAlign, HW_Aligner_EBR_AOI AOI) // ing 170419
        {
            m_id = id;
            m_auto = auto;
            m_AOI = AOI;
            m_control = m_auto.ClassControl();
            m_control.Add(this);
            m_handler = m_auto.ClassHandler();
            m_viewAlign = viewAlign;
            m_log = new Log(m_id, m_auto.ClassLogView(), "Aligner");
            m_grid = new ezGrid(m_id, grid, m_log, false);
            m_cam3D.Init("Cam_3D", m_auto.ClassLogView());
            m_cam3D.m_dgGrabDone += GrabDone; 
            m_view3D = new ezView("3D", 0, m_auto);
            m_img3D = m_view3D.m_imgView.m_pImg;
            m_img3D.ReAllocate(c_szEdge, 1);
            Directory.CreateDirectory("c:\\TestImg"); 
            Directory.CreateDirectory("c:\\TestImg\\3D"); 
            m_3DGraph.Init("3D_Graph", m_auto, m_log); 
            RunGrid(eGrid.eRegRead);
            m_bSave = false;
            m_bInspect = new bool[m_nThread];
            m_thread = new Thread[m_nThread];
            m_3D.Init(m_nThread);
            RunGrid(eGrid.eInit);
            m_cam3D.InitCamera();
            for (int n = 0; n < m_nThread; n++)
            {
                m_bInspect[n] = false;
                m_thread[n] = new Thread(new ParameterizedThreadStart(RunThread));
                m_thread[n].Start(n);
            }
        }

        public void ThreadStop()
        {
            m_cam3D.ThreadStop();
            m_view3D.ThreadStop();
            m_3D.ThreadStop(); 
            m_bRun = false;
            foreach (Thread thread in m_thread) thread.Join();
        }

        unsafe void InitData(int nPulseRotate)
        {
            m_imgAlign = m_viewAlign.m_imgView.m_pImg;
            int lCoSin = nPulseRotate / m_dTrigger;
            if (m_lCoSin == lCoSin) return;
            m_lCoSin = lCoSin;
            m_sin = new double[m_lCoSin];
            m_cos = new double[m_lCoSin];
            for (int y = 0; y < m_lCoSin; y++)
            {
                double t = 2 * Math.PI * y / m_lCoSin;
                m_sin[y] = Math.Sin(t);
                m_cos[y] = Math.Cos(t);
            }
            int w = m_cam3D.m_szBuf.x; 
            m_aBufH = new ushort[m_lCoSin * w];
            m_aBufB = new byte[m_lCoSin * w];
            m_3D.SetBuffer(0, w, m_cam3D.m_szBuf.y, w, m_lCoSin);
            for (int n = 0; n < m_lCoSin; n++)
            {
                fixed (ushort* pBufH = &m_aBufH[n * w]) fixed (byte* pBufB = &m_aBufB[n * w])
                {
                    m_3D.SetBuffer(n, pBufH, pBufB);
                }
            }
        }

        public IDockContent GetContentFromPersistString(string persistString)
        {
            if (m_view3D.IsPersistString(persistString)) return m_view3D;
            if (m_3DGraph.IsPersistString(persistString)) return m_3DGraph;
            return null;
        }

        public void ShowAll(DockPanel dockPanel)
        {
            if (!m_bEnable) return;
//            m_view3D.Show(dockPanel);
            m_3DGraph.Show(dockPanel); 
        }

        public void ShowDlg(Form parent, ref CPoint cpShow)
        {
            if (m_bEnable == false)
            {
                Hide();
                return;
            }
            int nIndex;
            this.TopLevel = false;
            this.Parent = parent;
            this.Location = cpShow.ToPoint();

            if (checkView.Checked) nIndex = 1;
            else nIndex = 0;

            this.Size = m_sz[nIndex];
            cpShow.y += m_sz[nIndex].Height;
            Show();
        }

        public void InvalidView()
        {
            if (!m_bUse) return;
            m_view3D.InvalidView(false);
        }

        private void checkView_CheckedChanged(object sender, EventArgs e)
        {
            m_handler.ShowChild();
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate);
            RunGrid(eGrid.eRegWrite);
        }

        public void RunInspect(Info_Wafer infoWafer)
        {
            StartInspect(infoWafer);
            WaitRunThread();
            m_bInvalidGraph = true;
            m_img3D.Fill(128);
            StartThread(MakeImageH);
        }

        int m_msInspect = 10000; 
        private void buttonInspect_Click(object sender, EventArgs e)
        {
            if (m_bSave)
            {
                m_bSave = false; 
                m_AOI.m_cpNotch.y = 15200;
                m_AOI.m_szImg.y = 360000; 
                m_cam3D.m_szBuf = new CPoint(2000, 256);
                InitData(360000);
                XmlSerializer xmlSerializer;
                StreamReader xmlReader;
                xmlSerializer = new XmlSerializer(m_aBufH.GetType());
                FileInfo fi = new FileInfo("c:\\TestImg\\3D\\H.raw");
                if (fi.Exists)
                {
                    xmlReader = new StreamReader("c:\\TestImg\\3D\\H.raw");
                    m_aBufH = (ushort[])xmlSerializer.Deserialize(xmlReader);
                    xmlReader.Close();
                }
                m_imgSaveHB.FileOpen("c:\\TestImg\\3D\\B.bmp");
                for (int y = 0; y < m_lCoSin; y++)
                {
                    int h = y * m_cam3D.m_szBuf.x;
                    for (int x = 0; x < m_cam3D.m_szBuf.x; x++) m_aBufB[x + h] = m_imgSaveHB.m_aBuf[y, x];
                }
            }
            StartInspect(null);
            WaitRunThread();
            m_bInvalidGraph = true; 
        }

        ezImg m_imgSaveHB = new ezImg("SaveHB", null);
        private void buttonBufH_Click(object sender, EventArgs e)
        {
            m_img3D.Fill(128); 
            StartThread(MakeImageH);
            if (m_bSave == false) return;
            m_imgSaveHB.ReAllocate(new CPoint(m_cam3D.m_szBuf.x, m_lCoSin), 1);
            for (int y = 0; y < m_lCoSin; y++)
            {
                int h = y * m_cam3D.m_szBuf.x; 
                for (int x = 0; x < m_cam3D.m_szBuf.x; x++) m_imgSaveHB.m_aBuf[y, x] = (byte)(m_aBufH[x + h] >> 8); 
            }
            m_imgSaveHB.FileSave("c:\\TestImg\\3D\\H.bmp");

            XmlSerializer xmlSerializer;
            StreamWriter xmlWriter;
            xmlSerializer = new XmlSerializer(m_aBufH.GetType());
            xmlWriter = new StreamWriter("c:\\TestImg\\3D\\H.raw");
            xmlSerializer.Serialize(xmlWriter, m_aBufH);
            xmlWriter.Close();
            
            WaitRunThread(); 
        }

        private void buttonBufB_Click(object sender, EventArgs e)
        {
            m_img3D.Fill(128); 
            StartThread(MakeImageB);
            if (m_bSave == false) return;
            m_imgSaveHB.ReAllocate(new CPoint(m_cam3D.m_szBuf.x, m_lCoSin), 1);
            for (int y = 0; y < m_lCoSin; y++)
            {
                int h = y * m_cam3D.m_szBuf.x;
                for (int x = 0; x < m_cam3D.m_szBuf.x; x++) m_imgSaveHB.m_aBuf[y, x] = m_aBufB[x + h];
            }
            m_imgSaveHB.FileSave("c:\\TestImg\\3D\\B.bmp");
            WaitRunThread(); 
        }

        public void ControlGrid(Control_Mom control, ezGrid rGrid, eGrid eMode)
        {
            m_axisX = control.AddAxis(rGrid, m_id, "CamR", "Axis R 3D Cam");
            m_axisZ = control.AddAxis(rGrid, m_id, "CamZ", "Axis Z 3D Cam");
        }

        public void RunGrid(ezGrid rGrid, eGrid eMode)
        {
            rGrid.Set(ref m_bEnable, "3D", "Enable", "Enable 3D");
            rGrid.Set(ref m_umWafer, "3D", "Thickness", "Wafer Thickness (um)");
        }

        protected void RunGrid(eGrid eMode)
        {
            m_grid.Update(eMode, null);
            m_grid.Set(ref m_bSave, "Test", "Test", "Test Run"); 
            m_grid.Set(ref m_dTrigger, "Trigger", "Period", "Trigger Period (pulse)");
            m_grid.Set(ref m_msGrab, "Grab", "Timeout", "Grab Timeout (ms)");
            m_grid.Set(ref m_xScale, "Image", "Scale", "Image X Scale");
            m_grid.Set(ref m_yOffset, "Image", "Offset", "Image Rotate Offset (pixel)");
            m_grid.Set(ref m_xCam, "CamPos", "X", "Camera X Position (pulse)");
            m_grid.Set(ref m_zCam, "CamPos", "Z", "Camera Z Position (pulse)");
            m_grid.Set(ref m_msInspect, "Inspect", "Timeout", "Inspect Timeout (ms)");
            m_grid.Set(ref m_umCam, "Inspect", "Cam", "Camera Pixel Size (um)");
            m_grid.Set(ref m_umMin, "Inspect", "MinH", "Mininum Inspect Height (um)");
            m_grid.Set(ref m_yAve, "Inspect", "AveY", "Average Y Range (pixel)");
            m_grid.Set(ref m_fScale, "Inspect", "Scale", "Inspect Length Scale");
            m_grid.Set(ref m_nThread, "Inspect", "Thread", "# of Inspect Thread");
            m_grid.Set(ref m_bInv3D, "3D", "Inv", "Inverse Image");
            m_grid.Set(ref m_nGV3D, "3D", "GV", "3D Calc GV (0 ~ 255)"); 
            m_cam3D.RunGrid(m_grid, eMode); 
            m_3DGraph.RunGrid(m_grid, eMode); 
            m_grid.Refresh();
        }

        public void RecipeGrid(ezGrid rGrid, eGrid eMode, ezJob job)
        {
            if (!m_bEnable) return;
            rGrid.Set(ref m_bUse, "3D", "Use", "Use Edge");
            m_3DGraph.RecipeGrid(rGrid, eMode, job); 
        }

        public eHWResult SearchHome(int msHome, ezStopWatch sw)
        {
            m_axisX.HomeSearch();
            m_axisZ.HomeSearch();
            while (!m_axisX.IsReady() && (sw.Check() <= msHome)) Thread.Sleep(10);
            while (!m_axisZ.IsReady() && (sw.Check() <= msHome)) Thread.Sleep(10);
            if (sw.Check() <= msHome) return eHWResult.OK;
            else return eHWResult.Error;
        }

        delegate void dgInspect(int nIndex);
        dgInspect m_dgInspect = null;
        bool StartThread(dgInspect dgInspect)
        {
            if (IsRunThread()) return true;
            m_dgInspect = dgInspect;
            for (int n = 0; n < m_nThread; n++) m_bInspect[n] = true;
            return false; 
        }

        bool IsRunThread()
        {
            for (int n = 0; n < m_nThread; n++)
            {
                if (m_bInspect[n]) return true; 
            }
            return false;
        }

        public bool WaitRunThread()
        {
            ezStopWatch sw = new ezStopWatch();
            while ((IsRunThread() == true) && (sw.Check() < m_msInspect)) Thread.Sleep(1);
            if (sw.Check() >= m_msInspect)
            {
                for (int n = 0; n < m_nThread; n++) m_bInspect[n] = false;
                m_log.Popup("Inspect Timeout !!"); 
                return true; 
            }
            return false;
        }

        void RunThread(object obj)
        {
            int nIndex = (int)obj;
            Thread.Sleep(5000);
            while (m_bRun)
            {
                Thread.Sleep(1);
                if (m_bInspect[nIndex])
                {
                    m_dgInspect(nIndex);
                    m_bInspect[nIndex] = false; 
                }
            }
        }

        public void ReadyInspect(int nPulseRotate)
        {
            if (!m_bUse) return;
            InitData(nPulseRotate);
            if (m_imgAlign.m_bNew) m_AOI.Inspect(m_imgAlign);
        }

        public void StartInspect(Info_Wafer infoWafer)
        {
            if (infoWafer != null) m_infoWafer = infoWafer;
            if (m_bUse == false)
            {
                m_log.Popup("Use Edge Inspect is false");
                return;
            }
            StartThread(RunInsect);
        }

        public bool MoveCam(bool bForward)
        {
            if (m_bUse && bForward)
            {
                m_axisX.Move(m_xCam);
                m_axisZ.Move(m_zCam + m_umWafer0 - m_umWafer);
            }
            else
            {
                m_axisX.Move(0);
                m_axisZ.Move(0);
            }
            if (m_axisX.WaitReady()) return true;
            if (m_axisZ.WaitReady()) return true;
            return false;
        }

        int m_nGV3D = 20;
        bool m_bInv3D = true; 
        ezImg m_imgSaveCalc = new ezImg("SaveCalc", null);
        ezStopWatch m_swGrab = new ezStopWatch(); 
        public bool Grab()
        {
            if (m_bUse == false) return false;
            m_swGrab.Start(); 
            m_cam3D.StartGrab(m_lCoSin);
            m_3D.StartCalc_Ave(m_bInv3D, m_lCoSin, m_msGrab, 0, 0, m_nGV3D); 
            Thread.Sleep(10);
            while (!m_cam3D.IsGrabDone() && (m_swGrab.Check() <= m_msGrab)) Thread.Sleep(1);
            m_log.Add("Grab : " + m_cam3D.m_nGrab.ToString() + " / " + m_cam3D.m_lGrab.ToString() + ", FPS : " + (1000 * m_cam3D.m_nGrab / m_swGrab.Check()).ToString());
            while (!m_3D.IsDone() && (m_swGrab.Check() <= m_msGrab)) Thread.Sleep(1);
            if (m_bSave)
            {
                unsafe
                {
                    int n1 = Math.Min(m_nThread, m_lCoSin);
                    for (int n = 0; n < n1; n++)
                    {
                        m_imgSaveCalc.ReAllocate(m_cam3D.m_szBuf, 1);
                        m_imgSaveCalc.Copy(m_3D.GetBufCalc(n));
                        m_imgSaveCalc.FileSave("c:\\TestImg\\3D\\Calc" + n.ToString("00") + ".bmp");
                    }
                }
            }
            if (m_swGrab.Check() >= m_msGrab) return true;
            return false;
        }

        ezImg m_imgSaveRaw = new ezImg("SaveRaw", null);
        unsafe void GrabDone(int nGrab, byte* pBuf)
        {
            m_3D.GrabDone(nGrab, pBuf);
            if (nGrab == 0) m_swGrab.Start(); 
            if (m_bSave && ((nGrab % (m_lCoSin / 10)) == 0))
            {
                m_imgSaveRaw.ReAllocate(m_cam3D.m_szBuf, 1);
                m_imgSaveRaw.Copy(pBuf);
                m_imgSaveRaw.FileSave("c:\\TestImg\\3D\\Raw" + nGrab.ToString() + ".bmp");
            }
        }

        void MakeImageH(int nIndex)
        {
            int w = m_cam3D.m_szBuf.x;
            int xp = 0, yp = 0;
            int y0 = nIndex * m_lCoSin / m_nThread;
            int y1 = y0 + m_lCoSin / m_nThread;
            int dy = (int)Math.Round(1.0 * m_lCoSin * m_AOI.m_cpNotch.y / m_AOI.m_szImg.y + m_yOffset);
            for (int y = y0; y < y1; y++)
            {
                int n0 = y * w;
                int iy = y - dy; 
                for (int x = 0, n = n0; x < w; x++, n++)
                {
                    GetPos(x, iy, ref xp, ref yp);
                    m_img3D.m_aBuf[yp, xp] = (byte)(m_aBufH[n] >> 8);
                }
            }
        }

        void MakeImageB(int nIndex)
        {
            int w = m_cam3D.m_szBuf.x;
            int xp = 0, yp = 0;
            int y0 = nIndex * m_lCoSin / m_nThread;
            int y1 = y0 + m_lCoSin / m_nThread;
            int dy = (int)Math.Round(1.0 * m_lCoSin * m_AOI.m_cpNotch.y / m_AOI.m_szImg.y + m_yOffset); 
            for (int y = y0; y < y1; y++)
            {
                int n0 = y * w;
                int iy = y - dy; 
                for (int x = 0, n = n0; x < w; x++, n++)
                {
                    GetPos(x, iy, ref xp, ref yp);
                    m_img3D.m_aBuf[yp, xp] = m_aBufB[n]; 
                }
            }
        }

        int m_yOffset = 7320; 
        void GetPos(int x, int y, ref int xp, ref int yp)
        {
            int R = (int)(c_R - m_xScale * (x - m_cam3D.m_szBuf.x / 2));
            while (y < 0) y += m_lCoSin;
            y %= m_lCoSin; 
            xp = (int)(R * m_sin[y]) + c_cpCenter.x;
            yp = (int)(-R * m_cos[y]) + c_cpCenter.y;
        }

        double m_umCam = 6.1;
        double m_umMin = 50; 
        int m_yAve = 20; 
        int m_minH = 0;
        double m_fScale = 1;
        void RunInsect(int nIndex)
        {
            double umCam = m_umCam / Math.Sqrt(2);
            m_minH = (int)Math.Round(m_umMin / umCam);
            umCam *= m_fScale * m_cam3D.m_szBuf.y / 65536;
            int w = m_cam3D.m_szBuf.x;
            double[] aBufH = new double[w];
            int[] aCount = new int[w];
            int dy = (int)Math.Round(1.0 * m_lCoSin * m_AOI.m_cpNotch.y / m_AOI.m_szImg.y + m_yOffset); 
            for (int n = nIndex; n < m_3DGraph.m_nInspect; n += m_nThread) 
            {
                Array.Clear(aBufH, 0, w);
                Array.Clear(aCount, 0, w);
                int y0 = (int)Math.Round(m_3DGraph.GetAngle(n) * m_lCoSin / 360) + dy;
                while (y0 < 0) y0 += m_lCoSin;
                y0 %= m_lCoSin; 
                for (int y = y0 - m_yAve; y <= y0 + m_yAve; y++)
                {
                    int yi = y;
                    while (yi < 0) yi += m_lCoSin;
                    yi = yi % m_lCoSin;
                    int i0 = yi * w;
                    for (int x = 0, i = i0; x < w; x++, i++)
                    {
                        if (m_aBufH[i] > 0)
                        {
                            aBufH[x] += m_aBufH[i];
                            aCount[x]++;
                        }
                    }
                }
                int minCount = m_yAve;
                for (int x = 0; x < w; x++)
                {
                    if (aCount[x] < minCount) aBufH[x] = 0;
                    else aBufH[x] = (ushort)(aBufH[x] / aCount[x]); 
                }
                m_3DGraph.RunInspect(n, aBufH, m_minH, umCam, m_umCam);
                if (m_bSave)
                {
                    StreamWriter fs = new StreamWriter(new FileStream("c:\\TestImg\\3D\\Buf" + n.ToString("00") + ".txt", FileMode.Create));
                    for (int x = 0; x < w; x++) fs.WriteLine(aBufH[x]);
                    fs.Close();
                }
            }
        }

        bool m_bInvalidGraph = false; 
        private void timerSave_Tick(object sender, EventArgs e)
        {
            if (m_bInvalidGraph)
            {
                m_3DGraph.InvalidData();
                m_bInvalidGraph = false; 
            }
        }

        public void LotStart()
        {

        }

        public void LotEnd()
        {

        }

    }
}

