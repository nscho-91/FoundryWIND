using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using ezAxis;
using ezAutoMom;
using ezTools;
using ezCam;

namespace ezAuto_EFEM
{
    public partial class HW_Aligner_EBR_EBR : Form
    {
        string m_id;
        Auto_Mom m_auto;
        Handler_Mom m_handler;
        Log m_log;
        ezGrid m_grid;

        bool m_bEnable = false; 
        public bool m_bUse = false;
        public bool m_bUseEdge = false; // ing 170426
        public bool m_bUseSurface = false; // ing 170612
        Size[] m_sz = new Size[2];

        ezView m_viewAlign;
        ezView m_viewEBR;

        ezImg m_imgAlign; 
        ezImg m_imgEBR;

        CPoint m_szAlign; 

        const int c_lEBR = 9000;
        const int c_R = 3500; 
        CPoint c_szEBR = new CPoint(c_lEBR, c_lEBR);
        CPoint c_cpCenter = new CPoint(c_lEBR / 2, c_lEBR / 2);

        int m_lCoSin = 0; 
        double[] m_cos;
        double[] m_sin;

        double m_xScale = 1;
        double m_dEdgeLength = 15; // ing 170424

        int m_xAve = 3;
        int m_yAve = 25;
        int m_xOffsetBevel = 10;
        int m_xOffsetEBR = 10;
        public int m_dNotch = 67500; // ing 170419

        const int c_nThread = 32;

        int[] m_aAve = null;
        int[] m_aAveAve = null;
        int[] m_aDiff = null;
        int[] m_aDiffDraw = null;
        double[] m_aDiffSum = null;
        int m_indexDraw = 0;
        int m_nDiff = 20;
        int[] m_nDiffEBR = new int[2] { 20, 20 };
        int m_nRagneMax = 30;
        int m_nGVBevel = 50;

        byte[] m_nRandGV = new byte[100];

        bool m_bRun = true;
        bool[] m_bRunImage = new bool[c_nThread];
        bool[] m_bRunInside = new bool[c_nThread];
        Thread[] m_thread = new Thread[c_nThread];
        bool m_bRunMain = false;
        bool m_bSave = false;
        bool m_bSaveChipping = false; // ing 170301
        bool m_bSaveSurface = false; // ing 170612
        Thread m_mainThread; 

        HW_Aligner_EBR_AOI m_AOI;
        public HW_Aligner_EBR_EBR_Graph m_EBRGraph = new HW_Aligner_EBR_EBR_Graph();
        public HW_Aligner_EBR_EBR_Concentric m_concentric = new HW_Aligner_EBR_EBR_Concentric(); // ing 170612

        public ArrayList m_aEBR = new ArrayList(); // ing 161221
        public HW_Klarf m_klarf = new HW_Klarf("EBR_Klarf");
        public HW_Klarf m_klarfChipping = new HW_Klarf("Chipping_Klarf"); // ing 170419
        Info_Wafer m_infoWafer;
        Info_Carrier m_infoCarrier; // BHJ 190410 add
        string m_strPath = "D:\\Result";
        string m_strPathBackup = "D:\\EBRBackup";
        public string m_strPathChipping = "D:\\Result"; // ing 170301
        public string m_strPathBackupChipping = "D:\\ChippingBackup"; // ing 170301
        public string m_strFile = "Test";
        public int m_msSaveTimeout = 120000;

        //For Crop EBR Image
        bool m_bUseCrop = false;
        int m_yCrop = 100;
        string m_strPathCropEBR = "D:\\EBRCrop";

        //For Save RawImg
        bool m_bUseSaveRaw = false;
        int m_xShifEdge = 200;
        CPoint m_cpRawROI = new CPoint(800, 400);
        string m_strSaveRawImgPath = "D://EBRRaw";
        string m_strProcess = "ProcessID";
        string m_strStepSEQ = "StepSEQ";
        string m_strDate = "YYYYMMDD_HHMMSS";

        //For Defect Data Writing function
        bool m_bWriteDefectDataFlag = true; // BHJ 190315 add
        public bool USE_WRITE_DEFECT_DATA
        {
            get { return m_bWriteDefectDataFlag; }
        }

        enum eEBRType
        {
            EBR,
            Trim
        }
        string[] m_strEBRTypes = Enum.GetNames(typeof(eEBRType));
        public string m_strEBRType = eEBRType.EBR.ToString();

        public HW_Aligner_EBR_EBR()
        {
            InitializeComponent();
            m_sz[0] = m_sz[1] = this.Size;
            m_sz[0].Height = 26;
        }

        public void Init(string id, Auto_Mom auto, ezView viewAlign, HW_Aligner_EBR_AOI AOI)
        {
            m_id = id;
            m_auto = auto;
            m_AOI = AOI; 
            m_handler = m_auto.ClassHandler();
            m_viewAlign = viewAlign;
            m_log = new Log(m_id, m_auto.ClassLogView(), "Aligner");
            m_grid = new ezGrid(m_id, grid, m_log, false);
            m_viewEBR = new ezView("EBR", 0, m_auto);
            m_imgEBR = m_viewEBR.m_imgView.m_pImg;
            m_imgEBR.ReAllocate(c_szEBR, 1);
            m_klarf.Init(m_auto, m_log); // ing 161221
            m_klarfChipping.Init(m_auto, m_log); // ing 170301
            m_EBRGraph.Init("EBR_Graph", m_auto, m_log, m_strEBRType == "Trim");
            m_concentric.Init("Concentric", m_auto, viewAlign, m_AOI, m_EBRGraph, m_log); 
            RunGrid(eGrid.eRegRead);
            RunGrid(eGrid.eInit);
            m_mainThread = new Thread(new ThreadStart(RunThread));
            m_mainThread.Start(); 
            for (int n = 0; n < c_nThread; n++)
            {
                m_bRunImage[n] = false;
                m_bRunInside[n] = false; 
                m_thread[n] = new Thread(new ParameterizedThreadStart(RunThread));
                m_thread[n].Start(n);
            }
            timerSave.Enabled = true;
        }

        public void ThreadStop()
        {
            m_viewEBR.ThreadStop();
            m_concentric.ThreadStop(); 
            m_bRun = false;
            foreach (Thread thread in m_thread) thread.Join();
        }

        void InitArray()
        {
            if (m_szAlign == m_imgAlign.m_szImg) return;
            m_szAlign = m_imgAlign.m_szImg; 
            m_aAve = new int[m_szAlign.x];
            m_aAveAve = new int[m_szAlign.x];
            m_aDiff = new int[m_szAlign.x];
            m_aDiffDraw = new int[m_szAlign.x];
            m_viewEBR.m_imgView.MaxZoomOut();
        }

        void InitCoSin()
        {
            if (m_lCoSin == m_imgAlign.m_szImg.y) return; 
            m_lCoSin = m_imgAlign.m_szImg.y;
            m_sin = new double[m_lCoSin];
            m_cos = new double[m_lCoSin];
            for (int y = 0; y < m_lCoSin; y++)
            {
                double t = 2 * Math.PI * y / m_lCoSin;
                m_sin[y] = Math.Sin(t);
                m_cos[y] = Math.Cos(t);
            }
        }

        public void Draw(Graphics dc, ezImgView imgView)
        {
            if (m_imgEBR == null) return;
            if (m_imgEBR.m_bNew) return;
            if (m_AOI.m_aEdge == null) return;
            if (m_sin == null) return; 
//            CPoint cp0 = GetPos(m_AOI.m_aEdge[0], 0); 
//            for (int y = 1; y < m_szAlign.y / 10; y++)
//            {
//                CPoint cp1 = GetPos(m_AOI.m_aEdge[y], 10 * y);
//                imgView.DrawLine(dc, Color.Red, cp0, cp1);
//                cp0 = cp1; 
//            }
            DrawCross(dc, imgView, Color.Yellow, GetPos(m_AOI.m_cpNotch), 16);
            int dR = (int)(10 / imgView.m_fZoom);
            CPoint dp = new CPoint(-dR , dR / 2);
            CPoint dp2 = new CPoint(-dR / 2, dR / 2);
            for (int n = 0; n < m_EBRGraph.m_nInspect; n++)
            {
                CPoint cp0 = new CPoint((int)m_EBRGraph.m_xEdge[n, 2], m_EBRGraph.m_yEBR[n]);
                CPoint cp1 = new CPoint((int)m_EBRGraph.m_xEdge[n, 0], m_EBRGraph.m_yEBR[n]);
                imgView.DrawLine(dc, Color.Green, GetPos(cp0), GetPos(cp1));
                cp1.x -= (int)(2.5 * dR);
                cp0 = GetPos(cp1) + dp;
                imgView.DrawString(dc, m_EBRGraph.m_lBevel[n].ToString("0."), cp0, Brushes.Yellow);
                cp1.x -= (int)(3.2 * dR);
                cp0 = GetPos(cp1) + dp;
                imgView.DrawString(dc, m_EBRGraph.m_lEBR[n].ToString("0."), cp0, Brushes.Green);
                cp1.x -= (int)(3.9 * dR);
                cp0 = GetPos(cp1) + dp2;
                imgView.DrawString(dc, (n + 1).ToString("00"), cp0, Brushes.Blue);
            }
            DrawEBR(dc);
        }

        void DrawCross(Graphics dc, ezImgView imgView, Color color, CPoint cpCenter, int nLength)
        {
            CPoint cp0 = cpCenter;
            CPoint cp1 = cpCenter;
            cp0.x -= nLength;
            cp1.x += nLength;
            imgView.DrawLine(dc, color, cp0, cp1);
            cp0 = cpCenter;
            cp1 = cpCenter;
            cp0.y -= nLength;
            cp1.y += nLength;
            imgView.DrawLine(dc, color, cp0, cp1);
        }

        void DrawEBR(Graphics dc)
        {
            if (m_indexDraw <= 0) return; 
            Pen pen = new Pen(Color.AliceBlue);
            dc.DrawLine(pen, new Point(10, 310), new Point(10 + m_szAlign.x, 310));
            dc.DrawLine(pen, new Point(10, 310), new Point(10, 10));

            DrawEBR(dc, new Pen(Color.Green), m_aAve);
            DrawEBR(dc, new Pen(Color.GreenYellow), m_aAveAve);
            DrawEBR(dc, new Pen(Color.Yellow), m_aDiffDraw);
            dc.DrawLine(new Pen(Color.Yellow), new Point(10, 310 - m_nDiff), new Point(10 + m_szAlign.x, 310 - m_nDiff)); 
            DrawEBR(dc, m_EBRGraph.m_xEdge[m_indexDraw - 1, 0]);
            DrawEBR(dc, m_EBRGraph.m_xEdge[m_indexDraw - 1, 1]);
            DrawEBR(dc, m_EBRGraph.m_xEdge[m_indexDraw - 1, 2]); 
        }

        void DrawEBR(Graphics dc, Pen pen, int[] aArray)
        {
            Point p0 = new Point(10, 310 - aArray[0]);
            for (int x = 1; x < m_szAlign.x; x++)
            {
                Point p1 = new Point(10 + x, 310 - aArray[x]);
                dc.DrawLine(pen, p0, p1);
                p0 = p1;
            }
        }

        void DrawEBR(Graphics dc, double fX)
        {
            Point p0 = new Point(10 + (int)fX, 10);
            Point p1 = new Point(10 + (int)fX, 310); 
            dc.DrawLine(new Pen(Color.Red), p0, p1); 
        }

        public IDockContent GetContentFromPersistString(string persistString)
        {
            if (m_viewEBR.IsPersistString(persistString)) return m_viewEBR;
            if (m_EBRGraph.IsPersistString(persistString)) return m_EBRGraph;
            return null;
        }

        public void ShowAll(DockPanel dockPanel)
        {
            if (!m_bUse) return; 
            m_viewEBR.Show(dockPanel);
            m_EBRGraph.Show(dockPanel); 
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
            m_viewEBR.InvalidView(false);
        }

        private void checkView_CheckedChanged(object sender, EventArgs e)
        {
            m_handler.ShowChild();
        }

        private void buttonInspect_Click(object sender, EventArgs e)
        {
            Inspect(); 
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate);
            RunGrid(eGrid.eRegWrite);
        }

        public void RunGrid(ezGrid rGrid, eGrid eMode)
        {
            rGrid.Set(ref m_bEnable, "EBR", "Enable", "Enable EBR");
        }

        protected void RunGrid(eGrid eMode)
        {
            m_grid.Update(eMode, null);
            m_grid.Set(ref m_xScale, "Scale", "Image", "Image X Scale");
            m_grid.Set(ref m_strEBRType, m_strEBRTypes, "Inspect", "Type", "Select EBR Type");
            m_grid.Set(ref m_yAve, "Inspect", "yAve", "Y Ave Size (pixel)");
            m_grid.Set(ref m_dEdgeLength, "Inspect", "EdgeLength", "Edge Length (pixel)");
            m_grid.Set(ref m_msSaveTimeout, "Inspect", "SaveTimeout", "Klarf File Save Timeout (ms)");
            m_grid.Set(ref m_strSaveRawImgPath, "Inspect", "PathRawImg", "Path for Saving Raw Image");
            m_grid.Set(ref m_indexDraw, "Draw", "Index", "Test Draw Index");
            if (eMode == eGrid.eRegRead) m_indexDraw = 0;
            m_grid.Set(ref m_strPath, m_klarf.m_id, "Path", "File Path");
            m_grid.Set(ref m_strPathBackup, m_klarf.m_id, "BackupPath", "Backup File Name");
            m_grid.Set(ref m_strPathChipping, m_klarfChipping.m_id, "Path", "File Path");
            m_grid.Set(ref m_strPathBackupChipping, m_klarfChipping.m_id, "BackupPath", "Backup File Name");
            m_EBRGraph.RunGrid(m_grid, eMode); 
            m_klarf.RunGrid(m_grid, eMode);
            m_klarfChipping.RunGrid(m_grid, eMode);
            m_concentric.RunGrid(m_grid, eMode);
            m_grid.Set(ref m_bUseCrop, "Crop", "Use", "Use Image Crop For EBR");
            m_grid.Set(ref m_strPathCropEBR, "Crop", "Path", "File Path For EBR Crop");
            m_grid.Set(ref m_yCrop, "Crop", "Height", "Image Height");
            m_grid.Refresh(); 
        }

        public void RecipeGrid(ezGrid rGrid, eGrid eMode, ezJob job)
        {
            if (!m_bEnable) return;
            if (eMode == eGrid.eJobOpen) ResetDefaultParam();
            if (m_strEBRType == eEBRType.Trim.ToString())
            {
                rGrid.Set(ref m_bUse, "Trim", "Use", "Use Measure Trim Width");
                rGrid.Set(ref m_AOI.m_bTrimNotch, "Trim", "NotchAlign", "Use Notch Align");
                rGrid.Set(ref m_AOI.m_nTrimGV, "Trim", "GV", "Trim GV");
                rGrid.Set(ref m_nDiff, "Trim", "Variation", "Variation Of GV Trim Edge");
                rGrid.Set(ref m_xAve, "Trim", "xAve", "X Ave Size (pixel)");
                m_EBRGraph.RecipeGrid(rGrid, eMode, job, "Trim");
                rGrid.Set(ref m_bUseSurface, "Trim", "Surface", "Use Surface Inspection");
            }
            else
            {
                rGrid.Set(ref m_bUse, "EBR", "Use", "Use EBR");
                rGrid.Set(ref m_bWriteDefectDataFlag, "EBR", "DCOL Enable", "Enable Defect Data Writing. default : true"); // BHJ 190315 add
                rGrid.Set(ref m_xAve, "EBR", "xAve", "X Ave Size (pixel)");
                rGrid.Set(ref m_nDiffEBR[0], "EBR", "DiffBevel", "Diff GV Threshold For Detect Bevel");
                rGrid.Set(ref m_nDiffEBR[1], "EBR", "DiffEBR", "Diff GV Threshold For Detect EBR");
                rGrid.Set(ref m_nGVBevel, "EBR", "BevelGV", "Limit of Bevel GV (0~255)");
                rGrid.Set(ref m_xOffsetBevel, "EBR", "XOffsetBevel", "X Offset From Bevel Line (pixel)");
                rGrid.Set(ref m_xOffsetEBR, "EBR", "XOffsetEBR", "X Offset From EBR Line (pixel)");
                rGrid.Set(ref m_nRagneMax, "EBR", "RangeMax", "Range For Find Diff Max (pixel)");
                m_EBRGraph.RecipeGrid(rGrid, eMode, job);
                rGrid.Set(ref m_bUseSurface, "EBR", "Surface", "Use Surface Inspection");
                rGrid.Set(ref m_bUseSaveRaw, "EBRRaw", "SaveRawImg", "Use Save EBR Raw Image");
                rGrid.Set(ref m_xShifEdge, "EBRRaw", "Shift_X", "ROI X Shift (pixel)");
                rGrid.Set(ref m_cpRawROI, "EBRRaw", "ROI", "ROI Size for Raw Image Save(pixel)");
                rGrid.Set(ref m_strProcess, "EBRRaw", "ProcessID", "ProcessID");
                rGrid.Set(ref m_strStepSEQ, "EBRRaw", "StepSEQ", "StepSEQ");
            }
        }

        void ResetDefaultParam()
        {
            m_bUse = false;
            m_AOI.m_bTrimNotch = false;
            m_AOI.m_nTrimGV = 20;
            m_nDiff = 20;
            m_xAve = 3;
            m_EBRGraph.ResetDefaultParam();
            m_bUseSurface = false;
            m_nDiffEBR[0] = m_nDiffEBR[1] = 20;
            m_nGVBevel = 50;
            m_xOffsetBevel = m_xOffsetEBR = 10;
            m_nRagneMax = 30;
            m_bUseSaveRaw = false;
            m_xShifEdge = 200;
            m_cpRawROI.x = 800;
            m_cpRawROI.y = 400;
            m_strProcess = "ProcessID";
            m_strStepSEQ = "StepSEQ";
        }
        
        void RunThread()
        {
            Thread.Sleep(5000);
            while (m_bRun)
            {
                Thread.Sleep(1);
                if (m_bRunMain)
                {
                    Inspect();
                    m_bRunMain = false;
                }
            }
        }

        void RunThread(object obj)
        {
            int nIndex = (int)obj;
            Thread.Sleep(5000);
            while (m_bRun)
            {
                Thread.Sleep(1);
                if (m_bRunImage[nIndex])
                {
                    MakeImage(nIndex); 
                    m_bRunImage[nIndex] = false;
                }
                if (m_bRunInside[nIndex])
                {
                   FillInside(nIndex);
                   m_bRunInside[nIndex] = false; 
                }
            }
        }

        public void StartInspect()
        {
            m_bRunMain = true; 
        }

        public void Inspect()
        {
            ezStopWatch sw = new ezStopWatch();
            DateTime dt = DateTime.Now;
            m_strDate = dt.Year.ToString("0000") + dt.Month.ToString("00") + dt.Day.ToString("00") + "_" + dt.Hour.ToString("00") + dt.Minute.ToString("00") + dt.Second.ToString("00");
            m_EBRGraph.ReAllocate(); 
            m_aEBR.Clear(); // ing 161221
            m_imgAlign = m_viewAlign.m_imgView.m_pImg;
            m_klarfChipping.m_img[1] = null;
            SetKlarf(ref m_infoWafer);
            if (m_imgAlign.m_szImg.x == 0)
            {
                m_log.Popup("Align Image is Empty !!");
                return;
            }
            InitArray();
            if (m_imgAlign.m_bNew) m_AOI.Inspect(m_imgAlign, m_EBRGraph.m_umX);
            else if (m_AOI.m_bInspChipping)
            {
                m_AOI.Inspect(m_imgAlign, m_EBRGraph.m_umX);
                m_bSaveChipping = true;
            }
            if (!m_bUse && !m_bUseSurface) return;
            for (int n = 0; n < m_EBRGraph.m_nInspect; n++)
            {
                if (m_strEBRType == eEBRType.Trim.ToString()) CalcTrim(n);
                else CalcEBR(n);
                if (m_bUseCrop) CropEBR(n);
                if (m_bUseSaveRaw) SaveRawImg(n);
            }
            if (m_indexDraw > 0) CalcEBR(m_indexDraw - 1);
            AddExtraValue();

            MakeImage();

            if (m_bUseSurface) InspectConcentric();

            m_log.Add("Align EBR Inspect : " + sw.Check());
            if (m_bUse) m_bSave = true;
        }

        public void InspectFile(int nCount)
        {
            ezStopWatch sw = new ezStopWatch();
            int nInspectBefore = m_EBRGraph.m_nInspect;
            m_EBRGraph.m_nInspect = nCount;
            m_EBRGraph.ReAllocate();
            m_aEBR.Clear(); // ing 161221
            m_klarfChipping.m_img[1] = null;
            SetKlarf(ref m_infoWafer);
            InitArray();
            for (int n = 0; n < m_EBRGraph.m_nInspect; n++)
            {
                if (m_strEBRType == eEBRType.Trim.ToString()) CalcTrim(n);
                else CalcEBR(n, true);
            }

            m_EBRGraph.InvalidData(m_strEBRType == "Trim");
            SaveResult();
            m_log.Add(m_infoWafer.m_nSlot.ToString() + " Slot EBR Klarf Save Finish.");
            // Recovery
            m_imgAlign = m_viewAlign.m_imgView.m_pImg;
            m_EBRGraph.m_nInspect = nInspectBefore;
        }

        void InspectConcentric()
        {
            int nIndex = 1;
            if (m_strEBRType == "Trim") nIndex = 0;
            MinMax mmBevel = new MinMax();
            mmBevel.Min = mmBevel.Max = (int)Math.Round(m_EBRGraph.m_xEdge[0, nIndex]);
            for (int n = 1; n < m_EBRGraph.m_nInspect; n++)
            {
                int nBevel = (int)Math.Round(m_EBRGraph.m_xEdge[n, nIndex]);
                if (mmBevel.Min > nBevel) mmBevel.Min = nBevel; 
                if (mmBevel.Max < nBevel) mmBevel.Max = nBevel;
            }
            m_concentric.Inspect(mmBevel, m_strEBRType == "Trim");
            m_bSaveSurface = true; // ing 170612
        }

        void CropEBR(int nIndex)
        {
            int nLP;
            nLP = m_infoWafer.m_nLoadPort;
            if (nLP < 0) nLP = 0;
            double fAngle;
            fAngle = m_EBRGraph.m_degOffset + (nIndex * (m_EBRGraph.m_degEnd - m_EBRGraph.m_degOffset + 1) / m_EBRGraph.m_nInspect);
            int y0 = (int)(m_AOI.m_cpNotch.y + m_szAlign.y / 360 * fAngle);
            Bitmap bmpROI = new Bitmap(m_imgAlign.m_szImg.x, m_yCrop, PixelFormat.Format8bppIndexed);
            ColorPalette palette = bmpROI.Palette;
            Color[] entries = palette.Entries;
            for (int n = 0; n < 256; n++) entries[n] = Color.FromArgb(n, n, n);
            bmpROI.Palette = palette;
            BitmapData btData = bmpROI.LockBits(new Rectangle(0, 0, bmpROI.Width, bmpROI.Height), ImageLockMode.ReadWrite, bmpROI.PixelFormat);
            unsafe
            {
                byte* pSrc = (byte*)btData.Scan0;
                for (int y = 0; y < bmpROI.Height; y++)
                {
                    byte* pOrg = (byte*)m_imgAlign.GetIntPtr((y0 + y) % m_imgAlign.m_szImg.y, 0);
                    for (int x = 0; x < bmpROI.Width; x++)
                    {
                        *pSrc = *pOrg; pSrc++; pOrg++;
                    }
                }
            }
            bmpROI.UnlockBits(btData);
            string strDate = DateTime.Now.Date.Year.ToString("0000") + DateTime.Now.Date.Month.ToString("00") + DateTime.Now.Date.Day.ToString("00");
            try
            {
                Directory.CreateDirectory(m_strPathCropEBR);
                if (!Directory.Exists(m_strPathCropEBR))
                {
                    m_log.Add("Can not Crop for EBR Cause " + m_strPathCropEBR + " Is not Exist");
                    return;
                }
                Directory.CreateDirectory(m_strPathCropEBR + "\\" + strDate);
                Directory.CreateDirectory(m_strPathCropEBR + "\\" + strDate + "\\" + m_auto.ClassHandler().ClassLoadPort(nLP).m_infoCarrier.m_strLotID + "@");
                Directory.CreateDirectory(m_strPathCropEBR + "\\" + strDate + "\\" + m_auto.ClassHandler().ClassLoadPort(nLP).m_infoCarrier.m_strLotID + "@" + "\\" + m_infoWafer.m_nSlot);
                bmpROI.Save(m_strPathCropEBR + "\\" + strDate + "\\" + m_auto.ClassHandler().ClassLoadPort(nLP).m_infoCarrier.m_strLotID + "@" + "\\" + m_infoWafer.m_nSlot + "\\" + nIndex.ToString() + ".bmp");
            }
            catch (Exception ex)
            {
                m_log.Add(ex.Message);
            }
        }

        void SaveRawImg(int nIndex)
        {
            int nLP;
            nLP = m_infoWafer.m_nLoadPort;
            if (nLP < 0) nLP = 0;
            Info_Carrier infoCarrier = m_auto.ClassHandler().ClassLoadPort(nLP).m_infoCarrier;  // BHJ 190410 add
            string strTimeLotStart = infoCarrier.m_timeLotStart.ToString("yyyyMMdd_hhmmss");    // BHJ 190410 add
            if (!Directory.Exists(m_strSaveRawImgPath))
            {
                Directory.CreateDirectory(m_strSaveRawImgPath);
            }
            Directory.CreateDirectory(string.Format("{0}\\{1}", m_strSaveRawImgPath, m_strProcess));
            Directory.CreateDirectory(string.Format("{0}\\{1}\\{2}", m_strSaveRawImgPath, m_strProcess, m_strStepSEQ));
            Directory.CreateDirectory(string.Format("{0}\\{1}\\{2}\\{3}", m_strSaveRawImgPath, m_strProcess, m_strStepSEQ, strTimeLotStart));
            Directory.CreateDirectory(string.Format("{0}\\{1}\\{2}\\{3}\\{4}_{5}", m_strSaveRawImgPath, m_strProcess, m_strStepSEQ, strTimeLotStart, infoCarrier.m_strLotID, infoCarrier.m_strCarrierID));
            Directory.CreateDirectory(string.Format("{0}\\{1}\\{2}\\{3}\\{4}_{5}\\{6:00}", m_strSaveRawImgPath, m_strProcess, m_strStepSEQ, strTimeLotStart, infoCarrier.m_strLotID, infoCarrier.m_strCarrierID, m_infoWafer.m_nSlot));
            string strPath = string.Format("{0}\\{1}\\{2}\\{3}\\{4}_{5}\\{6:00}", m_strSaveRawImgPath, m_strProcess, m_strStepSEQ, strTimeLotStart, infoCarrier.m_strLotID, infoCarrier.m_strCarrierID, m_infoWafer.m_nSlot); // BHJ 190405 add
            double fAngle;
            fAngle = m_EBRGraph.m_degOffset + (nIndex * (m_EBRGraph.m_degEnd - m_EBRGraph.m_degOffset + 1) / m_EBRGraph.m_nInspect);
            int y0 = (int)(m_AOI.m_cpNotch.y + m_szAlign.y / 360 * fAngle);
            Bitmap bmpROI;
            FileStream fs;
            TiffBitmapEncoder encoder;
            if (m_cpRawROI.x < m_imgAlign.m_szImg.x) bmpROI = new Bitmap(m_cpRawROI.x, m_cpRawROI.y, PixelFormat.Format8bppIndexed);
            else bmpROI = new Bitmap(m_imgAlign.m_szImg.x, m_cpRawROI.y, PixelFormat.Format8bppIndexed);
            ColorPalette palette = bmpROI.Palette;
            Color[] entries = palette.Entries;
            for (int n = 0; n < 256; n++) entries[n] = Color.FromArgb(n, n, n);
            bmpROI.Palette = palette;
            BitmapData btData = bmpROI.LockBits(new Rectangle(0, 0, bmpROI.Width, bmpROI.Height), ImageLockMode.ReadWrite, bmpROI.PixelFormat);
            int xStart = (int)(m_EBRGraph.m_xEdge[nIndex, 2] + m_xShifEdge - bmpROI.Width);
            while (xStart < 0)
            {
                xStart++;
            }
            while ((xStart + bmpROI.Width - 1) > m_imgAlign.m_szImg.x)
            {
                xStart--;
            }
            unsafe
            {
                byte* pSrc = (byte*)btData.Scan0;
                for (int y = 0; y < bmpROI.Height; y++)
                {
                    byte* pOrg = (byte*)m_imgAlign.GetIntPtr((y0 + y) % m_imgAlign.m_szImg.y, xStart);
                    for (int x = 0; x < bmpROI.Width; x++)
                    {
                        *pSrc = *pOrg; pSrc++; pOrg++;
                    }
                }
            }
            bmpROI.UnlockBits(btData);
            try
            {
                BitmapSource bmpSorce;
                fs = new FileStream(strPath + "\\" + m_auto.ClassHandler().ClassLoadPort(nLP).m_infoCarrier.m_strLotID + "_" + m_infoWafer.m_nSlot.ToString("00") + "_" + fAngle.ToString() + ".tif", FileMode.Create);
                encoder = new TiffBitmapEncoder();
                bmpSorce = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmpROI.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                encoder.Compression = TiffCompressOption.None;
                encoder.Frames.Add(BitmapFrame.Create(bmpSorce));
                encoder.Save(fs);
                fs.Close();
            }
            catch (Exception ex)
            {
                m_log.Add(ex.Message);
            }
        }

        void CalcEBR(int nIndex, bool bFileInspect = false)
        {
            HW_Aligner_EBR_EBR_Dat ebr = new HW_Aligner_EBR_EBR_Dat();
            ebr.m_fAngle = m_EBRGraph.m_degOffset + (nIndex * (m_EBRGraph.m_degEnd - m_EBRGraph.m_degOffset + 1) / m_EBRGraph.m_nInspect);
            Array.Clear(m_aAve, 0, m_szAlign.x);
            int y0 = (int)((m_AOI.m_cpNotch.y + m_szAlign.y / 360 * ebr.m_fAngle + m_AOI.GetOffsetAngle(ebr.m_fAngle)) % m_imgAlign.m_szImg.y);
            if (bFileInspect)
            {
                ebr.m_fAngle = nIndex;
                y0 = nIndex * m_nFileHeight + m_nFileHeight / 2;
            }
            m_EBRGraph.m_yEBR[nIndex] = y0;
            for (int y = (y0 - m_yAve); y <= (y0 + m_yAve); y++)
            {
                int iy = y % m_szAlign.y;
                for (int x = 0; x < m_szAlign.x; x++) m_aAve[x] += m_imgAlign.m_aBuf[iy, x];
            }
            for (int x = 0; x < m_szAlign.x; x++) m_aAve[x] /= (m_yAve * 2 + 1);
            CalcAveAve();
            m_EBRGraph.m_xEdge[nIndex, 0] = m_AOI.FindEdge(y0);
            if (m_EBRGraph.m_xEdge[nIndex, 0] - m_xOffsetBevel < 10) m_EBRGraph.m_xEdge[nIndex, 1] = FindEBR((int)Math.Round(m_EBRGraph.m_xEdge[nIndex, 0]), m_nDiffEBR[0]);
            else m_EBRGraph.m_xEdge[nIndex, 1] = FindEBR((int)Math.Round(m_EBRGraph.m_xEdge[nIndex, 0] - m_xOffsetBevel), m_nDiffEBR[0]);
            for (int n = (int)m_EBRGraph.m_xEdge[nIndex, 1]; n < m_szAlign.x; n++)
            {
                if (m_aAveAve[n] < m_nGVBevel || m_EBRGraph.m_xEdge[nIndex, 1] >= m_szAlign.x - 1) break;
                m_EBRGraph.m_xEdge[nIndex, 1]++;
            }
            if (m_EBRGraph.m_xEdge[nIndex, 1] - m_xOffsetEBR < 10) m_EBRGraph.m_xEdge[nIndex, 2] = FindEBR((int)Math.Round(m_EBRGraph.m_xEdge[nIndex, 1]), m_nDiffEBR[1], m_nRagneMax);
            else m_EBRGraph.m_xEdge[nIndex, 2] = FindEBR((int)Math.Round(m_EBRGraph.m_xEdge[nIndex, 1] - m_xOffsetEBR), m_nDiffEBR[1], m_nRagneMax);
            m_EBRGraph.SortEdge(nIndex);
            ebr.m_fEBR = m_EBRGraph.m_lEBR[nIndex];
            ebr.m_fBevel = m_EBRGraph.m_lBevel[nIndex];
            ebr.m_fResolution = m_EBRGraph.m_umX;
            ebr.m_defect.m_cp0.y = ebr.m_defect.m_cp1.y = y0;
            ebr.m_defect.m_cp0.x = (int)m_EBRGraph.m_xEdge[nIndex, 0];
            ebr.m_defect.m_cp1.x = (int)m_EBRGraph.m_xEdge[nIndex, 2];
            ebr.m_defect.m_cpOffset.x = (int)(Math.Cos((ebr.m_fAngle - 90) * Math.PI / 180) * (m_klarf.m_fDiePitchX / 2) + (m_klarf.m_fDiePitchX / 2));
            ebr.m_defect.m_cpOffset.y = (int)(Math.Sin((ebr.m_fAngle - 90) * Math.PI / 180) * (m_klarf.m_fDiePitchY / 2) + (m_klarf.m_fDiePitchY / 2));
            m_aEBR.Add(ebr);
            m_log.Add((nIndex + 1).ToString("00") + m_EBRGraph.m_lEBR[nIndex].ToString(" EBR = 0.") + m_EBRGraph.m_lBevel[nIndex].ToString(" Bevel = 0."));
            //m_log.Add(ebr.m_fAngle.ToString() + " Dgree Edge X Position : " + ebr.m_defect.m_cp1.x.ToString());
        }

        void CalcFileEBR(Bitmap bmp, string strFile)
        {
            string strEbrFile = "";
            HW_Aligner_EBR_EBR_Dat ebr = new HW_Aligner_EBR_EBR_Dat();
            ebr.m_fAngle = 0.0;
            Array.Clear(m_aAve, 0, m_szAlign.x);
            int y0 = bmp.Size.Height / 2;
            if (m_yAve - 1 > y0)
            {
                m_log.Popup("yAve Parameter Is Wrong !!");
                return;
            }
            BitmapData btData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            unsafe
            {
                byte* pSrc = (byte*)btData.Scan0;
                for (int y = (y0 - m_yAve); y <= (y0 + m_yAve); y++)
                {
                    pSrc = (byte*)btData.Scan0 + (bmp.Width * y);
                    //int iy = y % m_szAlign.y;
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        m_aAve[x] += *pSrc;
                        pSrc++;
                    }
                }
            }
            for (int x = 0; x < bmp.Width; x++) m_aAve[x] /= (m_yAve * 2 + 1);
            //CalcAveAve();

            for (int x = 0; x < bmp.Width; x++) // ing 170206
            {
                int x0 = x - m_xAve;
                if (x0 < 0) x0 = 0;
                int x1 = x + m_xAve;
                if (x1 >= bmp.Width) x1 = bmp.Width - 1;
                int nSum = 0;
                for (int ix = x0; ix <= x1; ix++) nSum += m_aAve[ix];
                m_aAveAve[x] = nSum / (x1 - x0 + 1);
            }
            Array.Clear(m_aDiff, 0, bmp.Width);
            for (int x = m_xAve; x < bmp.Width - m_xAve; x++)
            {
                m_aDiff[x] = Math.Abs(m_aAveAve[x + m_xAve] - m_aAveAve[x - m_xAve]);
            }

            for (int x = bmp.Width - 10; x > 10; x--)
            {
                if ((int)m_aAve[x] < m_AOI.m_nGV)
                {
                    m_EBRGraph.m_xEdge[0, 0] = x;
                    break;
                }
            }
            if (m_EBRGraph.m_xEdge[0, 0] - m_xOffsetBevel < 10) m_EBRGraph.m_xEdge[0, 1] = FindEBR((int)Math.Round(m_EBRGraph.m_xEdge[0, 0]), m_nDiffEBR[0]);
            else m_EBRGraph.m_xEdge[0, 1] = FindEBR((int)Math.Round(m_EBRGraph.m_xEdge[0, 0] - m_xOffsetBevel), m_nDiffEBR[0]);
            for (int n = (int)m_EBRGraph.m_xEdge[0, 1]; n < m_szAlign.x; n++)
            {
                if (m_aAveAve[n] < m_nGVBevel || m_EBRGraph.m_xEdge[0, 1] >= m_szAlign.x - 1) break;
                m_EBRGraph.m_xEdge[0, 1]++;
            }
            if (m_EBRGraph.m_xEdge[0, 1] - m_xOffsetEBR < 10) m_EBRGraph.m_xEdge[0, 2] = FindEBR((int)Math.Round(m_EBRGraph.m_xEdge[0, 1]), m_nDiffEBR[1], m_nRagneMax);
            else m_EBRGraph.m_xEdge[0, 2] = FindEBR((int)Math.Round(m_EBRGraph.m_xEdge[0, 1] - m_xOffsetEBR), m_nDiffEBR[1], m_nRagneMax);
            m_EBRGraph.SortEdge(0);
            ebr.m_fEBR = m_EBRGraph.m_lEBR[0];
            ebr.m_fBevel = m_EBRGraph.m_lBevel[0];
            ebr.m_fResolution = m_EBRGraph.m_umX;
            ebr.m_defect.m_cp0.y = ebr.m_defect.m_cp1.y = y0;
            ebr.m_defect.m_cp0.x = (int)m_EBRGraph.m_xEdge[0, 0];
            ebr.m_defect.m_cp1.x = (int)m_EBRGraph.m_xEdge[0, 2];
            ebr.m_defect.m_cpOffset.x = (int)(Math.Cos((ebr.m_fAngle - 90) * Math.PI / 180) * (m_klarf.m_fDiePitchX / 2) + (m_klarf.m_fDiePitchX / 2));
            ebr.m_defect.m_cpOffset.y = (int)(Math.Sin((ebr.m_fAngle - 90) * Math.PI / 180) * (m_klarf.m_fDiePitchY / 2) + (m_klarf.m_fDiePitchY / 2));

            Bitmap bmpColor;
            Graphics g;
            MemoryStream streamBitmap = new MemoryStream();
            bmpColor = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format24bppRgb);
            g = Graphics.FromImage(bmpColor);
            BitmapData btColorData = bmpColor.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmpColor.PixelFormat);
            unsafe
            {
                byte* pSrcGray = (byte*)btData.Scan0;
                byte* pSrc = (byte*)btColorData.Scan0;
                for (int n = 0; n < bmp.Width * bmp.Height; n++)
                {
                    *pSrc = *pSrcGray; pSrc++;
                    *pSrc = *pSrcGray; pSrc++;
                    *pSrc = *pSrcGray; pSrc++;
                    pSrcGray++;
                }
            }
            bmp.UnlockBits(btData);
            bmpColor.UnlockBits(btColorData);
            bmpColor.RotateFlip(RotateFlipType.RotateNoneFlipY); // ing 170215
            g.DrawLine(new Pen(Color.Green, 3), ebr.m_defect.m_cp0.x, y0, ebr.m_defect.m_cp1.x, y0);
            g.DrawLine(new Pen(Color.Yellow, 3), ebr.m_defect.m_cp1.x, y0, ebr.m_defect.m_cp1.x - (int)(ebr.m_fBevel / ebr.m_fResolution), y0);
            strEbrFile = strFile.Remove(strFile.IndexOf(".bmp"), 4);
            bmpColor.Save(strFile + "_EBR.bmp");
        }

        void CalcTrim(int nIndex)
        {
            HW_Aligner_EBR_Trim_Dat trim = new HW_Aligner_EBR_Trim_Dat();
            trim.m_fAngle = m_EBRGraph.m_degOffset + (nIndex * (m_EBRGraph.m_degEnd - m_EBRGraph.m_degOffset + 1) / m_EBRGraph.m_nInspect);
            Array.Clear(m_aAve, 0, m_szAlign.x);
            int y0 = (int)((m_AOI.m_cpNotch.y + m_szAlign.y / 360 * trim.m_fAngle) % m_imgAlign.m_szImg.y);
            m_EBRGraph.m_yEBR[nIndex] = y0;
            for (int y = (y0 - m_yAve); y <= (y0 + m_yAve); y++)
            {
                int iy = y % m_szAlign.y;
                for (int x = 0; x < m_szAlign.x; x++) m_aAve[x] += m_imgAlign.m_aBuf[iy, x];
            }
            for (int x = 0; x < m_szAlign.x; x++) m_aAve[x] /= (m_yAve * 2 + 1);
            CalcAveAve();
            m_EBRGraph.m_xEdge[nIndex, 0] = m_AOI.FindEdge(y0);
            for (int x = 0; x < m_szAlign.x; x++) m_aDiff[x] = -m_aDiff[x];
            m_EBRGraph.m_xEdge[nIndex, 1] = FindEBR((int)Math.Round(m_EBRGraph.m_xEdge[nIndex, 0]), m_nDiff);

            m_EBRGraph.SortEdgeTrim(nIndex);
            trim.m_fTrim = m_EBRGraph.m_lBevel[nIndex];
            trim.m_fResolution = m_EBRGraph.m_umX;
            trim.m_defect.m_cp0.y = trim.m_defect.m_cp1.y = y0;
            trim.m_defect.m_cp0.x = (int)m_EBRGraph.m_xEdge[nIndex, 0];
            trim.m_defect.m_cp1.x = (int)m_EBRGraph.m_xEdge[nIndex, 1];
            trim.m_defect.m_cpOffset.x = (int)(Math.Cos((trim.m_fAngle - 90) * Math.PI / 180) * (m_klarf.m_fDiePitchX / 2) + (m_klarf.m_fDiePitchX / 2));
            trim.m_defect.m_cpOffset.y = (int)(Math.Sin((trim.m_fAngle - 90) * Math.PI / 180) * (m_klarf.m_fDiePitchY / 2) + (m_klarf.m_fDiePitchY / 2));
            m_aEBR.Add(trim); // ing 161221
            m_log.Add((nIndex + 1).ToString("00") + m_EBRGraph.m_lBevel[nIndex].ToString(" Trim = 0."));
        }

        void AddExtraValue()
        {
            HW_Aligner_EBR_EBR_Dat ebrAve = new HW_Aligner_EBR_EBR_Dat();
            HW_Aligner_EBR_EBR_Dat ebrRange = new HW_Aligner_EBR_EBR_Dat();
            HW_Aligner_EBR_EBR_Dat ebrMedian = new HW_Aligner_EBR_EBR_Dat();
            double[] m_aEBRRank = new double[m_EBRGraph.m_lEBR.Length];
            Array.Copy(m_EBRGraph.m_lEBR, m_aEBRRank, m_EBRGraph.m_lEBR.Length);
            double[] m_aBevelRank = new double[m_EBRGraph.m_lBevel.Length];
            Array.Copy(m_EBRGraph.m_lBevel, m_aBevelRank, m_EBRGraph.m_lBevel.Length);
            Array.Sort(m_aEBRRank);
            Array.Sort(m_aBevelRank);
            for (int n = 0; n < m_EBRGraph.m_lEBR.Length; n++)
            {
                if (n == m_EBRGraph.m_lEBR.Length - 1)
                {
                    ebrRange.m_fEBR += m_aEBRRank[n];
                    ebrRange.m_fBevel += m_aBevelRank[n];
                }
                else if (n == 0)
                {
                    ebrRange.m_fEBR -= m_aEBRRank[n];
                    ebrRange.m_fBevel -= m_aBevelRank[n];
                }
                else
                {
                    ebrAve.m_fEBR += m_aEBRRank[n];
                    ebrAve.m_fBevel += m_aBevelRank[n];
                }
            }
            if (m_EBRGraph.m_lEBR.Length == 0)
            {
                ebrAve.m_fEBR = ebrAve.m_fBevel = 0;
            }
            else if (m_EBRGraph.m_lEBR.Length > 2)
            {
                ebrAve.m_fEBR /= (m_EBRGraph.m_lEBR.Length - 2);
                ebrAve.m_fBevel /= (m_EBRGraph.m_lEBR.Length - 2);
            }
            if (m_EBRGraph.m_lEBR.Length % 2 == 1)
            {
                ebrMedian.m_fEBR = m_aEBRRank[(int)Math.Floor((double)m_EBRGraph.m_lEBR.Length / 2)];
                ebrMedian.m_fBevel = m_aBevelRank[(int)Math.Floor((double)m_EBRGraph.m_lEBR.Length / 2)];
            }
            else
            {
                ebrMedian.m_fEBR = (m_aEBRRank[(int)Math.Floor((double)m_EBRGraph.m_lEBR.Length / 2)] + m_aEBRRank[(int)Math.Floor((double)m_EBRGraph.m_lEBR.Length / 2) - 1]) / 2;
                ebrMedian.m_fBevel = (m_aBevelRank[(int)Math.Floor((double)m_EBRGraph.m_lEBR.Length / 2)] + m_aBevelRank[(int)Math.Floor((double)m_EBRGraph.m_lEBR.Length / 2) - 1]) / 2;
            }
            ebrAve.m_bImg = ebrRange.m_bImg = ebrMedian.m_bImg = false;
            m_aEBR.Add(ebrAve);
            m_aEBR.Add(ebrMedian);
            m_aEBR.Add(ebrRange);
        }

        void CalcAveAve()
        {
            for (int x = 0; x < m_szAlign.x; x++) // ing 170206
            {
                int x0 = x - m_xAve;
                if (x0 < 0) x0 = 0;
                int x1 = x + m_xAve;
                if (x1 >= m_szAlign.x) x1 = m_szAlign.x - 1;
                int nSum = 0;
                for (int ix = x0; ix <= x1; ix++) nSum += m_aAve[ix];
                m_aAveAve[x] = nSum / (x1 - x0 + 1); 
            }
            Array.Clear(m_aDiff, 0, m_szAlign.x);
            for (int x = m_xAve; x < m_szAlign.x - m_xAve; x++)
            {
                m_aDiff[x] = Math.Abs(m_aAveAve[x + m_xAve] - m_aAveAve[x - m_xAve]);
                m_aDiffDraw[x] = m_aDiff[x]; 
            }
        }

        void SaveProfile(int nIndex)
        {
            string strFile = "d:\\EBR" + nIndex.ToString("00") + ".txt"; 
            StreamWriter sw = new StreamWriter(new FileStream(strFile, FileMode.Create));
            for (int x = 0; x < m_szAlign.x; x++)
            {
                sw.WriteLine(m_aAve[x].ToString() + ", " + m_aAveAve[x].ToString() + ", " + m_aDiff[x].ToString()); 
            }
            sw.Close(); 
        }

        double FindEBR(int xp, int nGV, int nRangeMax = 0)
        {
            int nCountMax = 0;
            int vMid = nGV;
            int xMax = xp; 
            int vMax = 0;
            while (m_aDiff[xp] < vMid && xp > 0) xp--;
            while ((m_aDiff[xp] >= vMid || nCountMax < nRangeMax) && xp > 0)
            {
                if (vMax < m_aDiff[xp])
                {
                    vMax = m_aDiff[xp];
                    xMax = xp;
                    nCountMax = 0;
                }
                nCountMax++;
                xp--;
            }
            return FindEdge(xMax); 
        }

        double FindEdge(int xMax)
        {
            if (xMax < m_xAve) return 0.0;
            if ((m_aDiffSum == null) || (m_aDiffSum.Length < 2 * m_xAve)) m_aDiffSum = new double[4 * m_xAve];
            for (int x = xMax - m_xAve, ix = 0; x <= xMax + m_xAve; x++, ix++) m_aDiffSum[ix] = FindEdgeSum(x);
            for (int x = xMax - m_xAve, ix = 0; x <= xMax + m_xAve; x++, ix++)
            {
                if (m_aDiffSum[ix] < 0 && ix > 0)
                {
                    if (m_aDiffSum[ix - 1] == m_aDiffSum[ix]) return xMax;
                    double dx = m_aDiffSum[ix - 1] / (m_aDiffSum[ix - 1] - m_aDiffSum[ix]);
                    double fxMax = x - 1 + dx;
                    xMax = (int)Math.Round(fxMax);
                    for (int xp = xMax - 2 * m_xAve; xp <= xMax + 2 * m_xAve; xp++)
                    {
                        if (xp < 0)
                            xp = 0;
                        else if (xp >= m_aDiff.Length)
                            continue;

                        m_aDiff[xp] = 0;
                        
                        
                        if (xp - 1 < 0) 
                            xp = 1;
                        else if (xp >= m_aDiff.Length)
                            continue;
                        m_aDiff[xp - 1] = 0;

                    }
                    return fxMax;
                }
            }
            return xMax; 
        }

        double FindEdgeSum(int xp)
        {
            double fSum = 0;
            if(xp - m_xAve < 0)
                return 0;
            for (int x = xp - m_xAve; x < xp; x++) fSum -= m_aDiff[x];
            for (int x = xp + 1; x <= xp + m_xAve; x++)
            {
                if (x < 0 || x >= 1376)
                    continue;
                fSum += m_aDiff[x];
            }
            return fSum; 
        }

        void MakeImage()
        {
            InitCoSin(); 
            m_imgEBR.Fill(245); 
            for (int n = 0; n < c_nThread; n++) m_bRunImage[n] = true;
            Thread.Sleep(10);
            int nRun = c_nThread;
            while (nRun > 0)
            {
                Thread.Sleep(1);
                nRun = 0;
                for (int n = 0; n < c_nThread; n++) if (m_bRunImage[n]) nRun++;
            }
            FillInside(); 
        }

        void MakeImage(int nIndex)
        {
            int xp = 0, yp = 0; 
            int y0 = nIndex * m_lCoSin / c_nThread;
            int y1 = y0 + m_lCoSin / c_nThread; 
            for (int y = y0; y < y1; y++)
            {
                for (int x = 0; x < m_szAlign.x; x++)
                {
                    GetPos(x, y, ref xp, ref yp);
                    m_imgEBR.m_aBuf[yp, xp] = m_imgAlign.m_aBuf[y, x];
                }
            }
        }

        void GetPos(int x, int y, ref int xp, ref int yp)
        {
            int R = (int)(c_R + m_xScale * (x - m_szAlign.x / 2));
            y -= m_AOI.m_cpNotch.y;
            if (y < 0) y += m_szAlign.y;
            if (y > m_lCoSin - 1) y -= m_lCoSin;
            xp = (int)(R * m_sin[y]) + c_cpCenter.x;
            yp = (int)(-R * m_cos[y]) + c_cpCenter.y;
        }

        CPoint GetPos(int x, int y)
        {
            int xp = 0, yp = 0;
            GetPos(x, y, ref xp, ref yp);
            return new CPoint(xp, yp); 
        }

        CPoint GetPos(CPoint cp)
        {
            return GetPos(cp.x, cp.y); 
        }

        void FillInside()
        {
            int nCount = 0;
            int nSum = 0; 
            for (int y = 0; y < m_szAlign.y; y += 100)
            {
                nSum += m_imgAlign.m_aBuf[y, 2];
                nCount++;
            }
            int nAve = nSum / nCount;
            Random rand = new Random();
            rand.NextBytes(m_nRandGV);
            for (int n = 0; n < 100; n++ )
            {
                m_nRandGV[n] = (byte)((m_nRandGV[n] % 6) + nAve); 
            }
            for (int n = 0; n < c_nThread; n++) m_bRunInside[n] = true;
            Thread.Sleep(10);
            int nRun = c_nThread;
            while (nRun > 0)
            {
                Thread.Sleep(1);
                nRun = 0;
                for (int n = 0; n < c_nThread; n++) if (m_bRunInside[n]) nRun++;
            }
        }

        void FillInside(int yIndex)
        {
            int nRand = 0; 
            int R = (int)(c_R - m_xScale * m_szAlign.x / 2) + 2;
            for (int y = yIndex; y < m_szAlign.y; y += c_nThread)
            {
                double dy = y - c_cpCenter.y; 
                double dx2 = R * R - dy * dy; 
                if (dx2 > 0)
                {
                    int dx = (int)Math.Sqrt(dx2);
                    for (int x = c_cpCenter.x - dx; x <= c_cpCenter.x + dx; x++)
                    {
                        m_imgEBR.m_aBuf[y, x] = m_nRandGV[nRand % 100];
                        nRand++; 
                    }
                }
            }
        }

        public void SetInfoWafer(Info_Wafer infoWafer)
        {
            m_infoWafer = infoWafer;
            if (m_infoWafer == null)
            {
                m_infoWafer = new Info_Wafer(-1, -1, -1, m_log, Wafer_Size.eSize.mm300);
            }
        }

        public void SetKlarf(ref Info_Wafer infoWafer)
        {
            Info_Carrier infoCarrier;
            if (infoWafer == null) infoWafer = new Info_Wafer(-1, -1, -1, m_log, Wafer_Size.eSize.mm300);
            infoCarrier = m_auto.ClassHandler().ClassCarrier(infoWafer.m_nLoadPort);
            if (infoCarrier == null)
            {
                infoCarrier = new Info_Carrier(-1);
                infoCarrier.m_wafer = new Wafer_Size(Wafer_Size.eSize.mm300, m_log);
            }
            m_klarf.m_img[0].ReAllocate(m_imgAlign);
            m_klarf.m_img[0].Copy(m_imgAlign, new CPoint(0, 0), new CPoint(0, 0), m_imgAlign.m_szImg);

            m_klarf.m_strLotID = infoCarrier.m_strLotID;
            m_klarf.m_strCSTID = infoCarrier.m_strCarrierID;
            int nTemp = m_klarf.m_strLotID.IndexOf("_"); // _hj Klarf 데이터 LotID 수정 170320
            if(nTemp > 0)
            {
                m_klarf.m_strLotID = m_klarf.m_strLotID.Substring(0, nTemp);          // _hj Klarf 데이터 LotID 수정 170320
                nTemp = 0;
            }
            

            m_klarf.m_strTimeResult = "";
            m_klarf.m_strWaferID = infoWafer.m_nSlot.ToString("00");
            m_klarf.m_strRecipe = infoCarrier.m_strRecipe;
            m_klarf.m_strTiffFileName = "";
            m_klarf.m_strSampleTestPlan = "";
            m_klarf.m_strAreaPerTest = "";

            m_klarf.m_nSampleSize = 300;
            m_klarf.m_nFileVer1 = 1;
            m_klarf.m_nFileVer2 = 1;
            m_klarf.m_nKlarfRow = 0;
            m_klarf.m_nKlarfCol = 0;
            m_klarf.m_nSlot = infoWafer.m_nSlot;
            m_klarf.m_nInspectionTest = 1;
            m_klarf.m_nSampleTestCnt = 1;
            m_klarf.m_nDefectDieCnt = 0;
            m_klarf.m_nCenterX = 0;
            m_klarf.m_nCenterY = 0;

            if (infoCarrier.m_wafer.m_eSize == Wafer_Size.eSize.mm300)
            {
                m_klarf.m_fDiePitchX = 300000;
                m_klarf.m_fDiePitchY = 300000;
            }
            else
            {
                m_klarf.m_fDiePitchX = 100000;
                m_klarf.m_fDiePitchY = 100000;
            }
            m_klarf.m_fDieOriginX = 0.000000e+000;
            m_klarf.m_fDieOriginY = 0.000000e+000;
            m_klarf.m_fSampleCenterLocationX = 1.500000e+005;
            m_klarf.m_fSampleCenterLocationY = 1.500000e+005;
            m_klarf.m_fResolution = m_EBRGraph.m_umX;

            // ing 170301 For Chipping
            m_klarfChipping.m_img[0] = m_imgAlign;
            m_klarfChipping.m_strLotID = infoCarrier.m_strLotID;
            m_klarfChipping.m_strCSTID = infoCarrier.m_strCarrierID;

            nTemp = m_klarfChipping.m_strLotID.IndexOf("_");// _hj Klarf 데이터 LotID 수정 170320
            if(nTemp > 0)
            {
                m_klarfChipping.m_strLotID = m_klarfChipping.m_strLotID.Substring(0, nTemp);          // _hj Klarf 데이터 LotID 수정 170320
                nTemp = 0;
            }

           
            m_klarfChipping.m_strTimeResult = "";
            m_klarfChipping.m_strWaferID = infoWafer.m_nSlot.ToString("00");
            m_klarf.m_strRecipe = infoCarrier.m_strRecipe;
            m_klarfChipping.m_strTiffFileName = "";
            m_klarfChipping.m_strSampleTestPlan = "";
            m_klarfChipping.m_strAreaPerTest = "";

            m_klarfChipping.m_nSampleSize = 300;
            m_klarfChipping.m_nFileVer1 = 1;
            m_klarfChipping.m_nFileVer2 = 1;
            m_klarfChipping.m_nKlarfRow = 0;
            m_klarfChipping.m_nKlarfCol = 0;
            m_klarfChipping.m_nSlot = infoWafer.m_nSlot;
            m_klarfChipping.m_nInspectionTest = 1;
            m_klarfChipping.m_nSampleTestCnt = 1;
            m_klarfChipping.m_nDefectDieCnt = 0;
            m_klarfChipping.m_nCenterX = 0;
            m_klarfChipping.m_nCenterY = 0;

            if (infoCarrier.m_wafer.m_eSize == Wafer_Size.eSize.mm300)
            {
                m_klarfChipping.m_fDiePitchX = 300000; // ing 170306
                m_klarfChipping.m_fDiePitchY = 300000; // ing 170306
            }
            else
            {
                m_klarfChipping.m_fDiePitchX = 100000; // ing 170306
                m_klarfChipping.m_fDiePitchY = 100000; // ing 170306
            }
            m_klarfChipping.m_fDieOriginX = 0.000000e+000;
            m_klarfChipping.m_fDieOriginY = 0.000000e+000;
            m_klarfChipping.m_fSampleCenterLocationX = 1.500000e+005;
            m_klarfChipping.m_fSampleCenterLocationY = 1.500000e+005;
            m_klarfChipping.m_fResolution = m_EBRGraph.m_umX;
            m_klarfChipping.m_strSetupID = m_auto.ClassXGem().GetStepID();
        }

        void PrintKlarf()
        {
            
        }

        public void SaveResult()
        {
            int x, y;
            CPoint imgSize = m_EBRGraph.GetSize();
            m_strFile = "LotEnd_" + m_auto.ClassRecipe().m_sRecipe + "_" + m_klarf.m_strCSTID + "_00-" + m_klarf.m_nSlot.ToString("00");
            m_klarf.m_strTiffFileName = m_auto.ClassRecipe().m_sRecipe + "_" + m_klarf.m_strCSTID + "_00-" + m_klarf.m_nSlot.ToString("00") + ".tif";
            string strDate = DateTime.Now.Date.Year.ToString("0000") + DateTime.Now.Date.Month.ToString("00") + DateTime.Now.Date.Day.ToString("00");
            Directory.CreateDirectory(m_strPathBackup);
            Directory.CreateDirectory(m_strPathBackup + "\\" + strDate);
            string strPath = m_strPath + "\\" + m_auto.ClassRecipe().m_sRecipe + "_" + m_klarf.m_strCSTID + "_00-" + m_klarf.m_nSlot.ToString("00");
            string strPathBackup = m_strPathBackup + "\\" + strDate + "\\" + m_auto.ClassRecipe().m_sRecipe + "_" + m_klarf.m_strCSTID + "_00-" + m_klarf.m_nSlot.ToString("00");
            x = imgSize.y + imgSize.x;
            y = imgSize.y;
            if (y < imgSize.y) y = imgSize.y;
            Bitmap resultBMP = new Bitmap(x, y);
            Graphics dc = Graphics.FromImage(resultBMP);
            dc.DrawImage(m_imgEBR.GetBitmap(), new Rectangle(0, 0, y, y), new Rectangle(0, 0, m_imgEBR.m_szImg.x, m_imgEBR.m_szImg.y), GraphicsUnit.Pixel);
            resultBMP.RotateFlip(RotateFlipType.RotateNoneFlipY);
            if (m_EBRGraph.AddImage((Bitmap)resultBMP, y, 0))
            {
                m_log.Popup("EBR Result File Save Fail !!");
                return;
            }
            resultBMP.Save(strPathBackup + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            m_klarf.SaveKlarf(strPath, true, this, resultBMP);
            m_klarf.SaveKlarf(strPathBackup, true, this, resultBMP);
        }

        public void SaveChippingResult(int nID)
        {
            m_strFile = "LotEnd_" + m_auto.ClassRecipe().m_sRecipe + "_" + m_klarfChipping.m_strCSTID + "_00-" + m_klarfChipping.m_nSlot.ToString("00");
            m_klarfChipping.m_strTiffFileName = m_auto.ClassRecipe().m_sRecipe + "_" + m_klarfChipping.m_strCSTID + "_00-" + m_klarfChipping.m_nSlot.ToString("00") + ".tif";
            string strDate = DateTime.Now.Date.Year.ToString() + DateTime.Now.Date.Month.ToString() + DateTime.Now.Date.Day.ToString();
            Directory.CreateDirectory(m_strPathBackupChipping);
            Directory.CreateDirectory(m_strPathBackupChipping + "\\" + strDate);
            string strPath = m_strPathChipping + "\\" + m_auto.ClassRecipe().m_sRecipe + "_" + m_klarfChipping.m_strCSTID + "_00-" + m_klarfChipping.m_nSlot.ToString("00");
            string strPathBackup = m_strPathBackupChipping + "\\" + strDate + "\\" + m_auto.ClassRecipe().m_sRecipe + "_" + m_klarfChipping.m_strCSTID + "_00-" + m_klarfChipping.m_nSlot.ToString("00");
            m_klarfChipping.SaveKlarf(strPath, true, m_AOI.m_arrChipping, nID, 0, m_dNotch);
            m_klarfChipping.SaveKlarf(strPathBackup, true, m_AOI.m_arrChipping, nID, 0, m_dNotch);
        }

        public void SaveSurfaceResult()
        {
            m_strFile = "LotEnd_" + m_auto.ClassRecipe().m_sRecipe + "_" + m_klarfChipping.m_strCSTID + "_00-" + m_klarfChipping.m_nSlot.ToString("00");
            m_klarfChipping.m_strTiffFileName = m_auto.ClassRecipe().m_sRecipe + "_" + m_klarfChipping.m_strCSTID + "_00-" + m_klarfChipping.m_nSlot.ToString("00") + ".tif";
            string strDate = DateTime.Now.Date.Year.ToString() + DateTime.Now.Date.Month.ToString() + DateTime.Now.Date.Day.ToString();
            Directory.CreateDirectory(m_strPathBackupChipping);
            Directory.CreateDirectory(m_strPathBackupChipping + "\\" + strDate);
            string strPath = m_strPathChipping + "\\" + m_auto.ClassRecipe().m_sRecipe + "_" + m_klarfChipping.m_strCSTID + "_00-" + m_klarfChipping.m_nSlot.ToString("00");
            string strPathBackup = m_strPathBackupChipping + "\\" + strDate + "\\" + m_auto.ClassRecipe().m_sRecipe + "_" + m_klarfChipping.m_strCSTID + "_00-" + m_klarfChipping.m_nSlot.ToString("00");
            m_klarfChipping.SaveKlarf(strPath, true, m_concentric.m_arrSurface, 0, 0, m_dNotch); // ing 170419
            m_klarfChipping.SaveKlarf(strPathBackup, true, m_concentric.m_arrSurface, 0, 0, m_dNotch); // ing 170419
        }

        public void LotStart()
        {
            if (m_bUse) m_klarf.LotStart();
            if (m_AOI.m_bInspChipping || m_bUseSurface || m_AOI.m_bFindNotchNoAlign) m_klarfChipping.LotStart(); // ing 170612
        }

        public void LotEnd()
        {
            Thread.Sleep(5000);
            ezStopWatch swSave = new ezStopWatch();
            while (swSave.Check() < m_msSaveTimeout)
            {
                Thread.Sleep(10);
                if (!m_klarf.m_bBusy) break;
            }
            if (m_klarf.m_bBusy)
            {
                m_log.Popup("Klarf File Save Fail, Can not Save Lotend File !!");
                return;
            }
            if (m_bUse)
            {
                StreamWriter sw;
                try
                {
                    sw = new StreamWriter(new FileStream(m_strPath + "\\" + m_strFile + ".trf", FileMode.Create));
                }
                catch (Exception ex)
                {
                    m_log.Popup(m_strPath + "\\" + m_strFile + ".trf Is Wrong Path !!");
                    m_log.Add(ex.Message);
                    return;
                }
                try
                {
                    sw.WriteLine("FileVersion " + m_klarf.m_nFileVer1.ToString() + " " + m_klarf.m_nFileVer2.ToString() + ";");
                    m_klarf.Put_FileTimestamp(sw);
                    sw.WriteLine("InspectionStationID " + Convert.ToChar(34) + m_klarf.m_strInspectionStationVender + Convert.ToChar(34) + " " + Convert.ToChar(34) + m_klarf.m_strInspectionStationModel + Convert.ToChar(34) + " " + Convert.ToChar(34) + m_klarf.m_strInspectionStationMachineID + Convert.ToChar(34) + ";");
                    m_klarf.Put_ResultTimestamp(sw);
                    sw.WriteLine("LotID " + Convert.ToChar(34) + m_klarf.m_strLotID + "-" + m_klarf.m_strCSTID + Convert.ToChar(34) + ";");
                    sw.WriteLine("EndOfLotInspection;");
                    sw.WriteLine("EndOfFile;");
                    sw.Close();
                    m_log.Add(m_strPath + "\\" + m_strFile + ".trf LotEnd File Saved.");
                }
                catch (Exception ex)
                {
                    m_log.Popup(m_strPath + "\\" + m_strFile + ".trf Create Error !!");
                    m_log.Add(ex.Message);
                    sw.Close();
                }

                try
                {
                    sw = new StreamWriter(new FileStream(m_strPathBackup + "\\" + m_strFile + ".trf", FileMode.Create));
                }
                catch (Exception ex)
                {
                    m_log.Popup(m_strPathBackup + "\\" + m_strFile + ".trf Is Wrong Path !!");
                    m_log.Add(ex.Message);
                    return;
                }
                try
                {
                    sw.WriteLine("FileVersion " + m_klarf.m_nFileVer1.ToString() + " " + m_klarf.m_nFileVer2.ToString() + ";");
                    m_klarf.Put_FileTimestamp(sw);
                    sw.WriteLine("InspectionStationID " + Convert.ToChar(34) + m_klarf.m_strInspectionStationVender + Convert.ToChar(34) + " " + Convert.ToChar(34) + m_klarf.m_strInspectionStationModel + Convert.ToChar(34) + " " + Convert.ToChar(34) + m_klarf.m_strInspectionStationMachineID + Convert.ToChar(34) + ";");
                    m_klarf.Put_ResultTimestamp(sw);
                    sw.WriteLine("LotID " + Convert.ToChar(34) + m_klarf.m_strLotID + "-" + m_klarf.m_strCSTID + Convert.ToChar(34) + ";");
                    sw.WriteLine("EndOfLotInspection;");
                    sw.WriteLine("EndOfFile;");
                    sw.Close();
                    m_log.Add(m_strPathBackup + "\\" + m_strFile + ".trf LotEnd File Saved.");
                }
                catch (Exception ex)
                {
                    m_log.Popup(m_strPathBackup + "\\" + m_strFile + ".trf Create Error !!");
                    m_log.Add(ex.Message);
                    sw.Close();
                }
            }

            if (m_AOI.m_bInspChipping || m_bUseSurface || m_AOI.m_bFindNotchNoAlign)
            {
                StreamWriter sw;
                try
                {
                    sw = new StreamWriter(new FileStream(m_strPathChipping + "\\" + m_strFile + ".trf", FileMode.Create));
                }
                catch (Exception ex)
                {
                    m_log.Popup(m_strPathChipping + "\\" + m_strFile + ".trf Is Wrong Path !!");
                    m_log.Add(ex.Message);
                    return;
                }
                try
                {
                    sw.WriteLine("FileVersion " + m_klarfChipping.m_nFileVer1.ToString() + " " + m_klarfChipping.m_nFileVer2.ToString() + ";");
                    m_klarfChipping.Put_FileTimestamp(sw);
                    sw.WriteLine("InspectionStationID " + Convert.ToChar(34) + m_klarfChipping.m_strInspectionStationVender + Convert.ToChar(34) + " " + Convert.ToChar(34) + m_klarfChipping.m_strInspectionStationModel + Convert.ToChar(34) + " " + m_klarfChipping.m_strInspectionStationMachineID + ";");
                    m_klarfChipping.Put_ResultTimestamp(sw);
                    sw.WriteLine("LotID " + Convert.ToChar(34) + m_klarfChipping.m_strLotID +"-" +m_klarfChipping.m_strLotID + Convert.ToChar(34) + ";");
                    sw.WriteLine("EndOfLotInspection;");
                    sw.WriteLine("EndOfFile;");
                    sw.Close();
                    m_log.Add(m_strPathChipping + "\\" + m_strFile + ".trf LotEnd File Saved.");
                }
                catch (Exception ex)
                {
                    m_log.Popup(m_strPathChipping + "\\" + m_strFile + ".trf Create Error !!");
                    m_log.Add(ex.Message);
                    sw.Close();
                }

                try
                {
                    sw = new StreamWriter(new FileStream(m_strPathBackupChipping + "\\" + m_strFile + ".trf", FileMode.Create));
                }
                catch (Exception ex)
                {
                    m_log.Popup(m_strPathBackupChipping + "\\" + m_strFile + ".trf Is Wrong Path !!");
                    m_log.Add(ex.Message);
                    return;
                }
                try
                {
                    sw.WriteLine("FileVersion " + m_klarfChipping.m_nFileVer1.ToString() + " " + m_klarfChipping.m_nFileVer2.ToString() + ";");
                    m_klarfChipping.Put_FileTimestamp(sw);
                    sw.WriteLine("InspectionStationID " + Convert.ToChar(34) + m_klarfChipping.m_strInspectionStationVender + Convert.ToChar(34) + " " + Convert.ToChar(34) + m_klarfChipping.m_strInspectionStationModel + Convert.ToChar(34) + " " + m_klarfChipping.m_strInspectionStationMachineID + ";");
                    m_klarfChipping.Put_ResultTimestamp(sw);
                    sw.WriteLine("LotID " + Convert.ToChar(34) + m_klarfChipping.m_strLotID + "-" + m_klarfChipping.m_strCSTID + Convert.ToChar(34) + ";");
                    sw.WriteLine("EndOfLotInspection;");
                    sw.WriteLine("EndOfFile;");
                    sw.Close();
                    m_log.Add(m_strPathBackupChipping + "\\" + m_strFile + ".trf LotEnd File Saved.");
                }
                catch (Exception ex)
                {
                    m_log.Popup(m_strPathBackupChipping + "\\" + m_strFile + ".trf Create Error !!");
                    m_log.Add(ex.Message);
                    sw.Close();
                }
            }

        }

        bool IsExistEBRFile()
        {
            return false;
        }

        private void timerSave_Tick(object sender, EventArgs e)
        {
            if (m_bSave)
            {
                m_EBRGraph.InvalidData(m_strEBRType == "Trim");
                SaveResult();
                m_log.Add(m_infoWafer.m_nSlot.ToString() + " Slot EBR Klarf Save Finish.");
                m_bSave = false;
            }

            if (m_bSaveChipping)
            {
                if (m_AOI.m_bSaveEdgeChipping && m_bUseEdge)
                {
                    if (m_klarfChipping.m_img[1] != null)
                    {
                        SaveChippingResult(1);
                        m_log.Add(m_infoWafer.m_nSlot.ToString() + " Slot Chipping Klarf Save Finish.");
                        m_bSaveChipping = false;
                    }
                }
                else
                {
                    SaveChippingResult(0);
                    m_log.Add(m_infoWafer.m_nSlot.ToString() + " Slot Chipping Klarf Save Finish.");
                    m_bSaveChipping = false;
                }
            }

            if (m_bSaveSurface)
            {
                SaveSurfaceResult();
                m_log.Add(m_infoWafer.m_nSlot.ToString() + " Slot Chipping Klarf Save Finish.");
                m_bSaveSurface = false;
            }
        }

        int m_nFileHeight;
        private void buttonFileInsp_Click(object sender, EventArgs e)
        {
            bool bInfoWaferIsNull = false;
            ezImg beforImg;
            if (m_infoWafer == null)
            {
                bInfoWaferIsNull = true;
                m_infoWafer = new Info_Wafer(-1, -1, -1, m_log);
            }
            string[] strFileNames;
            byte[,] aBuf;
            CPoint cpSize;
            Bitmap[] bmpFiles;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "BMP Gray (*.bmp)|*.bmp";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            strFileNames = dlg.FileNames;
            bmpFiles = new Bitmap[strFileNames.Length];
            for (int n = 0; n < strFileNames.Length; n++)
            {
                bmpFiles[n] = new Bitmap(strFileNames[n]);
            }
            if (strFileNames.Length == 1)
            {
                cpSize = new CPoint(bmpFiles[0].Width, bmpFiles[0].Height);
                m_nFileHeight = bmpFiles[0].Height;
            }
            else if (strFileNames.Length > 1)
            {
                for (int n = 1; n < bmpFiles.Length; n++)
                {
                    if (bmpFiles[n - 1].Width != bmpFiles[n].Width || bmpFiles[n - 1].Height != bmpFiles[n].Height)
                    {
                        m_log.Popup("Image Size Is Difference");
                        return;
                    }
                }
                cpSize = new CPoint(bmpFiles[0].Width, bmpFiles[0].Height * strFileNames.Length);
                m_nFileHeight = bmpFiles[0].Height;
            }
            else
            {
                return;
            }
            m_imgAlign = new ezImg("FileEBR", m_log);
            m_imgAlign.ReAllocate(cpSize, 1);
            m_AOI.m_szImg = cpSize;
            aBuf = new byte[bmpFiles[0].Height * strFileNames.Length, bmpFiles[0].Width];
            for (int n = 0; n < bmpFiles.Length; n++)
            {
                BitmapData btData = bmpFiles[n].LockBits(new Rectangle(0, 0, bmpFiles[n].Width, bmpFiles[n].Height), ImageLockMode.ReadWrite, bmpFiles[n].PixelFormat);
                unsafe
                {
                    fixed(byte* pSrc = &aBuf[bmpFiles[0].Height * n, 0])
                    {
                        cpp_memcpy(pSrc, (byte*)btData.Scan0, bmpFiles[0].Width * bmpFiles[0].Height);
                    }
                }
                bmpFiles[n].UnlockBits(btData);
            }
            m_imgAlign.m_aBuf = aBuf;
            m_imgAlign.FileSave("D:\\EBRMerge.bmp");
            beforImg = m_AOI.m_img;
            m_AOI.m_img = m_imgAlign;
            InspectFile(bmpFiles.Length);
            /*
            foreach (string strBmp in strFileNames)
            {
                Bitmap bmp = new Bitmap(strBmp);
                CalcFileEBR(bmp, strBmp);
            }
            */
            if (bInfoWaferIsNull) m_infoWafer = null;
            m_AOI.m_img = beforImg;
        }

        [DllImport("ezCpp.dll")]
        unsafe public static extern void cpp_memcpy(byte* pDst, byte* pSrc, int nLength);

    }

    public class HW_Aligner_EBR_EBR_Dat
    {
        public Defect m_defect;
        public double m_fEBR;
        public double m_fBevel;
        public double m_fAngle;
        public double m_fResolution;
        public bool m_bImg;
        public HW_Aligner_EBR_EBR_Dat()
        {
            m_fAngle = 0;
            m_fResolution = 3.5;
            m_defect = new Defect();
            m_bImg = true;
        }
    }

    public class HW_Aligner_EBR_Trim_Dat
    {
        public Defect m_defect;
        public double m_fTrim;
        public double m_fAngle;
        public double m_fResolution;
        public HW_Aligner_EBR_Trim_Dat()
        {
            m_fAngle = 0;
            m_fResolution = 3.5;
            m_defect = new Defect();
        }
    }
}
