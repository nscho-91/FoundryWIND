using System;
using System.Collections; 
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;
using ezAxis;
using ezAutoMom;
using ezTools;
using ezCam;

namespace ezAuto_EFEM
{
    public partial class HW_Aligner_EBR_Edge : Form, Control_Child
    {
        string m_id;
        Auto_Mom m_auto;
        Control_Mom m_control; 
        Handler_Mom m_handler;
        Log m_log;
        ezGrid m_grid;

        public bool m_bEnable = false; 
        public bool m_bUse = false;

        ezView m_viewAlign;
        ezView m_viewEdge;

        ezCam_Dalsa m_camEdge = new ezCam_Dalsa();
        ezImg m_imgAlign;
        ezImg m_imgEdge;

        CPoint m_szAlign;
        CPoint m_szImg = new CPoint(0, 0); 

        int m_lCoSin = 0;
        double[] m_cos;
        double[] m_sin;
        double m_xScale = 0.5;

        const int c_lEdge = 21600;
        const int c_R = 9000;
        CPoint c_szEdge = new CPoint(c_lEdge, c_lEdge);
        CPoint c_cpCenter = new CPoint(c_lEdge / 2, c_lEdge / 2);

        HW_Aligner_EBR_AOI m_AOI;
        HW_Aligner_EBR_EBR m_EBR; // ing 170419

        Axis_Mom m_axisCam;
        int m_posCam = 0;
        public int m_posShift = 0;

        int m_yOffset = 0; 

        public ezImg[] m_aImg = new ezImg[3]; 

        const int c_nThread = 33;
        bool m_bRun = true;
        bool[,] m_bRunImage = new bool[3, c_nThread];
//        bool[] m_bRunInside = new bool[c_nThread];

        public HW_Klarf m_klarf = new HW_Klarf("Bevel_Klarf"); // ing 170525
        double m_umX = 3.5; // ing 170426

        enum eInspect
        {
            AveY,
            AveSub,
            CalcIsland, 
            FindIsland, 
            SaveImage,
            Finish
        }
        eInspect m_eInspect = eInspect.Finish;

        const int c_nBGR = 2;
        int m_dNotch = 67500;

        bool[,] m_bRunInspects = new bool[6, c_nThread]; 
        Thread[] m_thread = new Thread[c_nThread];
        ezStopWatch m_swEdge = new ezStopWatch(); 

        CPoint m_szROI = new CPoint(); 

        Size[] m_sz = new Size[2];

        MinMax m_mmROI;
        int m_lAveY = 200;
        ezImg m_imgAveY;
        ezImg m_imgSub;
        ezImg m_imgSave;
        ezImg m_imgJPG; 
        int[] m_aSumY;
        int[] m_aIsland;
        int m_nGV = 35; 
        int m_lNotch = 1000;
        CPoint m_szInspect = new CPoint(8, 8);
        ArrayList m_listResult = new ArrayList();
        ArrayList m_aDefect = new ArrayList(); // ing 170426
        string m_strPath = "d:\\Edge_Image";
        string m_strPathBackup = "d:\\EdgeBackup"; // ing 170426
        string m_strLotEnd = "Test"; // ing 170426
        Info_Wafer m_infoWafer = null; 

        bool m_bTimerSave = false;
        bool m_bSaveInspect = false; 

        public HW_Aligner_EBR_Edge()
        {
            InitializeComponent();
            m_sz[0] = m_sz[1] = this.Size;
            m_sz[0].Height = 26;
        }

        public void Init(string id, Auto_Mom auto, ezView viewAlign, HW_Aligner_EBR_AOI AOI, HW_Aligner_EBR_EBR ebr) // ing 170419
        {
            m_id = id;
            m_auto = auto;
            m_AOI = AOI;
            m_EBR = ebr; // ing 170419
            m_control = m_auto.ClassControl(); 
            m_control.Add(this);
            m_handler = m_auto.ClassHandler();
            m_viewAlign = viewAlign;
            m_log = new Log(m_id, m_auto.ClassLogView(), "Aligner");
            m_grid = new ezGrid(m_id, grid, m_log, false);
            m_camEdge.Init("Edge_Cam", m_auto.ClassLogView());
            for (int n = 0; n < 3; n++) m_aImg[n] = new ezImg(m_id + n.ToString(), m_log); 
            m_viewEdge = new ezView("Edge", 0, m_auto);
            m_mmROI = new MinMax(300, 680, m_log);
            m_imgAveY = new ezImg("AveY", m_log);
            m_imgSub = new ezImg("Sub", m_log);
            m_imgSave = new ezImg("Save", m_log);
            m_imgJPG = new ezImg("JPG", m_log); 
            m_imgEdge = m_viewEdge.m_imgView.m_pImg;
            m_imgEdge.ReAllocate(c_szEdge, 3);
            RunGrid(eGrid.eRegRead);
            RunGrid(eGrid.eInit);
            for (int n = 0; n < c_nThread; n++)
            {
                for (int i = 0; i < 3; i++) m_bRunImage[i, n] = false;
//                m_bRunInside[n] = false;
                m_thread[n] = new Thread(new ParameterizedThreadStart(RunThread));
                m_thread[n].Start(n);
            }
            m_klarf.Init(m_auto, m_log); // ing 170426
        }

        public void ThreadStop()
        {
            m_camEdge.ThreadStop();
            m_viewEdge.ThreadStop();
            m_bRun = false;
            foreach (Thread thread in m_thread) thread.Join();
        }

        void InitArray()
        {
            m_imgAlign = m_viewAlign.m_imgView.m_pImg;
            if (m_szAlign == m_imgAlign.m_szImg) return;
            m_szAlign = m_imgAlign.m_szImg;
            m_viewEdge.m_imgView.MaxZoomOut();
        }

        void InitCoSin()
        {
            if (m_lCoSin == m_aImg[0].m_szImg.y) return;
            m_lCoSin = m_aImg[0].m_szImg.y;
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
            if (m_imgEdge == null) return;
            if (m_imgEdge.m_bNew) return;
            if (m_AOI.m_aEdge == null) return;
            if (m_sin == null) return; 
        }

        public IDockContent GetContentFromPersistString(string persistString)
        {
            if (m_viewEdge.IsPersistString(persistString)) return m_viewEdge;
            return null;
        }

        public void ShowAll(DockPanel dockPanel)
        {
            if (!m_bEnable) return;
            m_viewEdge.Show(dockPanel);
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
            m_viewEdge.InvalidView(false);
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

        private void buttonInspect_Click(object sender, EventArgs e)
        {
            bool bUse = m_bUse; 
            m_bUse = true;
            m_bSaveInspect = true; 
            ReadyInspect(); 
            StartInspect(0, null);
            StartInspect(1, null);
            StartInspect(2, null);
            m_bUse = bUse; 
        }

        string m_strImgPath = "";
        string m_strImgExt = "";
        
        private void buttonRGBRead_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "BMP Files (*.bmp)|*.bmp";
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            GetImgFileName(dlg.FileName); 
            m_aImg[0].FileOpen(m_strImgPath + ".B." + m_strImgExt, false);
            m_aImg[1].FileOpen(m_strImgPath + ".G." + m_strImgExt, false);
            m_aImg[2].FileOpen(m_strImgPath + ".R." + m_strImgExt, false);
            m_viewAlign.m_imgView.m_pImg.FileOpen(m_strImgPath + ".A." + m_strImgExt, false);
            m_EBR.m_klarfChipping.m_img[1] = m_aImg[2]; // ing 170419
            m_log.Popup("Image Open Done");
        }

        private void buttonRGBSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "BMP Files (*.bmp)|*.bmp";
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            GetImgFileName(dlg.FileName); 
            m_aImg[0].FileSave(m_strImgPath + ".B." + m_strImgExt);
            m_aImg[1].FileSave(m_strImgPath + ".G." + m_strImgExt);
            m_aImg[2].FileSave(m_strImgPath + ".R." + m_strImgExt);
            m_viewAlign.m_imgView.m_pImg.FileSave(m_strImgPath + ".A." + m_strImgExt);
            m_log.Popup("Image save Done");
        }

        private void buttonCamSetup_Click(object sender, EventArgs e)
        {
            m_camEdge.ShowDialog();
        }

        void GetImgFileName(string strFile)
        {
            string[] strFiles = strFile.Split('.');
            if (strFiles.Length < 2) return; 
            if (strFiles.Length == 2)
            {
                m_strImgPath = strFiles[0];
                m_strImgExt = strFiles[1]; 
            }
            if (strFiles.Length > 2)
            {
                m_strImgExt = strFiles[strFiles.Length - 1]; 
                int nPath = strFiles.Length - 1;
                if (IsRGB(strFiles[strFiles.Length - 2])) nPath--;
                m_strImgPath = strFiles[0];
                for (int n = 1; n < nPath; n++) m_strImgPath += "." + strFiles[n]; 
            }
        }

        bool IsRGB(string str)
        {
            if (str == "R") return true;
            if (str == "G") return true;
            if (str == "B") return true;
            if (str == "A") return true; 
            return false; 
        }

        public void ControlGrid(Control_Mom control, ezGrid rGrid, eGrid eMode)
        {
            m_axisCam = control.AddAxis(rGrid, m_id, "Cam", "Axis Edge Cam");
        }

        public void RunGrid(ezGrid rGrid, eGrid eMode)
        {
            rGrid.Set(ref m_bEnable, "Edge", "Enable", "Enable Edge");
            rGrid.Set(ref m_posCam, "Edge", "Cam_Ready", "Ready Position (pulse)"); // ing 170105
            rGrid.Set(ref m_posShift, "Edge", "Shift_Ready", "Ready Position (pulse)"); // ing 170105
        }

        protected void RunGrid(eGrid eMode)
        {
            m_grid.Update(eMode, null);
            m_grid.Set(ref m_xScale, "Image", "Scale", "Image X Scale");
            m_grid.Set(ref m_yOffset, "Image", "Offset", "Image Y Offset (pixel)");
            m_grid.Set(ref m_mmROI, "Inspect", "ROI", "Inspect ROI (pixel)");
            m_grid.Set(ref m_lAveY, "Inspect", "AveY", "AveY Range (pixel)");
            m_grid.Set(ref m_lNotch, "Inspect", "Notch", "Notch Size (1/2) (pixel)");
            m_grid.Set(ref m_dNotch, "Inspect", "dNotch", "Notch Size (1/2) (pixel)");
            m_grid.Set(ref m_nGV, "Inspect", "dGV", "Delta GV (0 ~ 255)");
            m_grid.Set(ref m_szInspect, "Inspect", "Size", "Min Inspect Size (pixel)");
            m_grid.Set(ref m_umX, "Inspect", "Resolution", "Cam Resolution");
            m_grid.Set(ref m_strPath, m_klarf.m_id, "Path", "File Path"); // ing 170426
            m_grid.Set(ref m_strPathBackup, m_klarf.m_id, "BackupPath", "Backup File Path"); // ing 170426
            m_klarf.RunGrid(m_grid, eMode); // ing 170426
            m_EBR.m_dNotch = m_dNotch; // ing 170419
            m_grid.Refresh();
        }

        public void RecipeGrid(ezGrid rGrid, eGrid eMode, ezJob job)
        {
            if (!m_bEnable) return;
            rGrid.Set(ref m_bUse, "Edge", "Use", "Use Edge");
            m_EBR.m_bUseEdge = m_bUse; // ing 170426
        }

        public eHWResult SearchHome(int msHome, ezStopWatch sw)
        {
            if (!m_bEnable) return eHWResult.OK;
            m_axisCam.HomeSearch();
            while (!m_axisCam.IsReady() && (sw.Check() <= msHome)) Thread.Sleep(10);
            if (sw.Check() <= msHome) return eHWResult.OK;
            else return eHWResult.Error; 
        }

        void RunThread(object obj)
        {
            int nIndex = (int)obj;
            Thread.Sleep(5000);
            while (m_bRun)
            {
                Thread.Sleep(1);
                if (m_eInspect != eInspect.Finish)
                {
                    if (m_bRunInspects[(int)m_eInspect, nIndex])
                    {
                        switch (m_eInspect)
                        {
                            case eInspect.AveY: MakeAveY(c_nBGR, nIndex); break;
                            case eInspect.AveSub: MakeSub(c_nBGR, nIndex); break;
                            case eInspect.CalcIsland: CalcIsland(nIndex); break; 
                            case eInspect.FindIsland: FindIsland(nIndex); break;
                            case eInspect.SaveImage: MakeSave(c_nBGR, nIndex); break;
                        }
                        m_bRunInspects[(int)m_eInspect, nIndex] = false;
                    }
                    if (nIndex == 0)
                    {
                        int nRun = 0; 
                        for (int n = 0; n< c_nThread; n++)
                        {
                            if (m_bRunInspects[(int)m_eInspect, n]) nRun++;
                        }
                        if (nRun == 0)
                        {
                            for (int n = 0; n < c_nThread; n++) m_bRunInspects[(int)(m_eInspect + 1), n] = true;
                            m_log.Add("Edge Inspect " + m_eInspect.ToString() + " Done : " + m_swEdge.Check().ToString());
                            m_swEdge.Start(); 
                            m_eInspect++;
                        }
                    }
                }
                else
                {
                    for (int nID = 0; nID < 3; nID++)
                    {
                        if (m_bRunImage[nID, nIndex])
                        {
                            MakeImage(nID, nIndex);
                            m_bRunImage[nID, nIndex] = false;
                            m_auto.Invalidate(0);
                        }
                    }
                }
            }
        }

        void MakeAveY(int nID, int nIndex)
        {
            int nDiv = 0;
            int xl = m_mmROI.Max - m_mmROI.Min;
            int x0 = m_mmROI.Min + nIndex * xl / c_nThread;
            int x1 = m_mmROI.Min + (nIndex + 1) * xl / c_nThread; 
            for (int y = -m_lAveY; y <= m_lAveY; y++)
            {
                MakeSumY(nID, x0, x1, y);
                nDiv++;
            }
            for (int y = 0; y < m_szAlign.y; y++)
            {
                for (int x = x0, ix = x0 - m_mmROI.Min; x < x1; x++, ix++)
                {
                    m_imgAveY.m_aBuf[y, ix] = (byte)(m_aSumY[ix] / nDiv);
                }
                MakeSubY(nID, x0, x1, y - m_lAveY);
                MakeSumY(nID, x0, x1, y + m_lAveY + 1);
            }
        }

        void MakeSumY(int nID, int x0, int x1, int y)
        {
            if (y < 0) y += m_szAlign.y;
            y = y % m_szAlign.y;
            for (int x = x0, ix = x0 - m_mmROI.Min; x < x1; x++, ix++) m_aSumY[ix] += m_aImg[nID].m_aBuf[y, x];
        }

        void MakeSubY(int nID, int x0, int x1, int y)
        {
            if (y < 0) y += m_szAlign.y;
            y = y % m_szAlign.y;
            for (int x = x0, ix = x0 - m_mmROI.Min; x < x1; x++, ix++) m_aSumY[ix] -= m_aImg[nID].m_aBuf[y, x];
        }

        void MakeSub(int nID, int nIndex)
        {
            int y0 = nIndex * m_szAlign.y / c_nThread;
            int y1 = (nIndex + 1) * m_szAlign.y / c_nThread; 
            for (int y = y0; y < y1; y++)
            {
                for (int x = m_mmROI.Min, ix = 0; x < m_mmROI.Max; x++, ix++)
                {
                    m_imgSub.m_aBuf[y, ix] = (byte)Math.Abs((int)m_aImg[nID].m_aBuf[y, x] - m_imgAveY.m_aBuf[y, ix]);
                }
            }
        }

        void CalcIsland(int nIndex)
        {
            int y0 = nIndex * m_szAlign.y / c_nThread;
            int y1 = (nIndex + 1) * m_szAlign.y / c_nThread;
            int nIsland = 0;
            for (int y = y0; y < y1; y++)
            {
                for (int x = m_mmROI.Min, ix = 0; x < m_mmROI.Max; x++, ix++)
                {
                    if (m_imgSub.m_aBuf[y, ix] <= m_nGV) nIsland = 0; 
                    else
                    {
                        nIsland++;
                        if (m_aIsland[y] < nIsland) m_aIsland[y] = nIsland; 
                    }
                }
            }
        }

        void FindIsland(int nIndex)
        {
            if (nIndex != 0) return;
            DeleteNotch(); 
            int y = 0;
            while (y < m_szAlign.y) FindIsland(ref y); 
        }

        void FindIsland(ref int y)
        {
            while ((y < m_szAlign.y) && (m_aIsland[y] < m_szInspect.x)) y++;
            int y0 = y;
            while ((y < m_szAlign.y) && (m_aIsland[y] >= m_szInspect.x)) y++;
            int y1 = y - 1;
            if ((y1 - y0) > m_szInspect.y)
            {
                Defect defect = new Defect(); // ing 170426
                defect.m_cp0.x = 0; // ing 170426
                defect.m_cp1.x = m_aImg[2].m_szImg.x; // ing 170426
                defect.m_cp0.y = y0; // ing 170426
                defect.m_cp1.y = y1; // ing 170426
                defect.m_nSize = y1 - y0; // ing 170426
                defect.m_fAngle = (((y0 + y1) / 2 - GetNotchY() + m_aImg[2].m_szImg.y) % m_aImg[2].m_szImg.y) * 360.0 / m_aImg[2].m_szImg.y; // ing 170426
                m_aDefect.Add(defect); // ing 170426
                m_listResult.Add((y0 + y1) / 2);
                m_log.Add("Result = " + ((y0 + y1) / 2).ToString()); 
            }
        }

        void DeleteNotch()
        {
            int yNotch = GetNotchY();
            for (int y = yNotch - m_lNotch; y < yNotch + m_lNotch; y++) m_aIsland[(y + m_szAlign.y) % m_szAlign.y] = 0;
        }

        void MakeSave(int nID, int nIndex)
        {
            if (nIndex != 0) return;
            m_imgJPG.ReAllocate(new CPoint(m_szROI.x, 200), 1);
            int yNotch = GetNotchY();
            SetKlarf(ref m_infoWafer); // ing 170426
            m_bTimerSave = true;
            if (m_bSaveInspect == false) return;
            m_imgSave.ReAllocate(new CPoint(3 * m_szROI.x, m_szROI.y), 1);
            for (int y = 0; y < m_szAlign.y; y++)
            {
                Array.Copy(m_imgSub.m_aBuf, y * m_szROI.x, m_imgSave.m_aBuf, 3 * y * m_szROI.x, m_szROI.x);
                Array.Copy(m_imgAveY.m_aBuf, y * m_szROI.x, m_imgSave.m_aBuf, (3 * y + 1) * m_szROI.x, m_szROI.x);
                Array.Copy(m_aImg[nID].m_aBuf, y * m_szAlign.x + m_mmROI.Min, m_imgSave.m_aBuf, (3 * y + 2) * m_szROI.x, m_szROI.x);
            }
            for (int x = 0; x < m_szROI.x; x++) m_imgSave.m_aBuf[yNotch, x] = 255;
        }

        int GetNotchY()
        {
            //            return 259468;  //forget
            return (m_AOI.m_cpNotch.y + m_dNotch) % m_szAlign.y;
        }

        void MakeImage(int nID, int nIndex)
        {
            int xp = 0, yp = 0; 
            int y0 = nIndex * m_lCoSin / c_nThread;
            int y1 = y0 + m_lCoSin / c_nThread;
            for (int y = y0; y < y1; y++)
            {
                for (int x = 0; x < m_szAlign.x; x++)
                {
                    GetPos(x, y, ref xp, ref yp);
                    m_imgEdge.m_aBuf[yp, 3 * xp + nID] = m_aImg[nID].m_aBuf[y, x];
                }
            }
        }

        void GetPos(int x, int y, ref int xp, ref int yp)
        {
            int R = (int)(c_R - m_xScale * (x - m_szAlign.x / 2));
            y -= m_AOI.m_cpNotch.y;
            while (y < 0) y += m_szAlign.y;
            xp = (int)(R * m_sin[y]) + c_cpCenter.x;
            yp = (int)(-R * m_cos[y]) + c_cpCenter.y;
        }

        public void ReadyInspect()
        {
            if (!m_bUse) return;
            InitArray();
            InitCoSin();
            Array.Clear(m_imgEdge.m_aBuf, 0, c_szEdge.x * c_szEdge.y * 3);
            if (m_imgAlign.m_bNew) m_AOI.Inspect(m_imgAlign, m_EBR.m_EBRGraph.m_umX);
        }

        public void StartInspect(int nID, Info_Wafer infoWafer)
        {
            m_infoWafer = infoWafer; 
            if (m_bUse == false)
            {
                m_log.Popup("Use Edge Inspect is false");
                return; 
            }
            if (nID == 2)
            {
                m_szROI = new CPoint(m_mmROI.Max - m_mmROI.Min, m_szAlign.y);
                m_imgAveY.ReAllocate(m_szROI, 1);
                m_imgSub.ReAllocate(m_szROI, 1);
                m_aSumY = new int[m_szROI.x];
                Array.Clear(m_aSumY, 0, m_szROI.x);
                m_aIsland = new int[m_szAlign.y];
                Array.Clear(m_aIsland, 0, m_szAlign.y);
                m_listResult.Clear();
                m_aDefect.Clear(); // ing 170426
                for (int n = 0; n < c_nThread; n++) m_bRunInspects[(int)eInspect.AveY, n] = true;
                m_eInspect = eInspect.AveY;
                m_swEdge.Start(); 
            }
            for (int n = 0; n < c_nThread; n++) m_bRunImage[nID, n] = true; 
        }

        public bool MoveCam(bool bForward)
        {
            if (!m_bEnable) return false;
            double fPos = 0;
            if (m_bUse && bForward) fPos = m_posCam; 
            m_axisCam.Move(fPos);
            if (m_axisCam.WaitReady()) return true;
            return false; 
        }

        public bool Grab(int nImg, int msGrab)
        {
            if (m_bUse == false) return false;
            ezStopWatch sw = new ezStopWatch();
            int yOffset = m_yOffset + (nImg % 2) * m_szAlign.y / 2; 
            if (m_camEdge.GrabShift(m_aImg[nImg], ref m_szImg, msGrab, yOffset, 1)) return true;
            //if (m_camEdge.Grab(m_viewAlign.ClassImage(), ref m_szImg, msGrab, -1, 1)) return true;
            //m_szImg = m_aImg[nImg].m_szImg; 
            Thread.Sleep(10);
            while (!m_camEdge.IsGrabDone() && (sw.Check() <= msGrab)) Thread.Sleep(1);
            if (sw.Check() >= msGrab) return true;
            return false;
        }

        void CutImage(CPoint cp, CPoint sz, ref ezImg img)
        {
            if ((cp.x < 0) || ((cp.x + sz.x) >= m_szImg.x)) return;
            if ((cp.y < 0) || ((cp.y + sz.y) >= m_szImg.y)) return; 
            img.ReAllocate(sz, 3);
            for (int iy = 0, y = cp.y; iy < sz.y; iy++, y++)
            {
                for (int ix = 0, x = cp.x; ix < sz.x; ix++)
                {
                    img.m_aBuf[iy, 3 * ix + 0] = m_aImg[0].m_aBuf[y, x];
                    img.m_aBuf[iy, 3 * ix + 1] = m_aImg[1].m_aBuf[y, x];
                    img.m_aBuf[iy, 3 * ix + 2] = m_aImg[2].m_aBuf[y, x]; 
                }
            }
        }

        private void timerSave_Tick(object sender, EventArgs e)
        {
            if (m_bTimerSave == false) return;
            m_bTimerSave = false;
            SaveInspect();
            SaveKlarf(); // ing 170426
        }

        void SaveInspect()
        {
            ezDateTime date = new ezDateTime();
            string strFile; 
            Directory.CreateDirectory(m_strPath); 
            if (m_infoWafer == null) strFile = m_strPath + "\\Edge"; 
            else
            {
                strFile = m_strPath + "\\" + date.GetDate();
                Directory.CreateDirectory(strFile);
                strFile = m_strPath + "\\" + date.GetDate() + "\\" + m_infoWafer.m_strWaferID;
                Directory.CreateDirectory(strFile);
                strFile = m_strPath + "\\" + date.GetDate() + "\\" + m_infoWafer.m_strWaferID + "\\Edge";
            }
            foreach (int y in m_listResult)
            {
                int yp = y; 
                if (yp < 0) yp = 100;
                if (yp >= (m_szAlign.y - 100)) yp = m_szAlign.y - 101;
                //for (int yd = 0, ys = yp - 100; yd < 200; yd++, ys++)
                for (int yd = 0, ys = yp - 100; yd < 200 && ys < m_szAlign.y; yd++, ys++) // ing 170511
                {
//                    Array.Copy(m_imgSub.m_aBuf, ys * m_szROI.x, m_imgJPG.m_aBuf, yd * m_szROI.x, m_szROI.x);
//                    m_imgJPG.FileSave(m_strImgPath + "." + yp.ToString() + ".jpg"); 
                    Array.Copy(m_aImg[c_nBGR].m_aBuf, ys * m_szAlign.x + m_mmROI.Min, m_imgJPG.m_aBuf, yd * m_szROI.x, m_szROI.x);
                }
                int nAngle = 360 * (yp - m_AOI.m_cpNotch.y - m_dNotch) / m_szAlign.y;
                nAngle = (nAngle + 720) % 360;
                m_imgJPG.FileSave(strFile + "." + nAngle.ToString("000") + ".jpg"); 
                
            }
            if (m_bSaveInspect == false) return;
            m_bSaveInspect = false;
            //            m_imgSave.FileSave(m_strImgPath + ".I." + m_strImgExt);
        }

        public void LotStart() // ing 170426
        {
            if (m_bUse) m_klarf.LotStart();
        }

        public void LotEnd() // ing 170426
        {
            Thread.Sleep(5000);
            ezStopWatch swSave = new ezStopWatch(); // ing 170525
            while (swSave.Check() < m_EBR.m_msSaveTimeout)
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
                    sw = new StreamWriter(new FileStream(m_strPath + "\\" + m_strLotEnd + ".trf", FileMode.Create));
                }
                catch (Exception ex)
                {
                    m_log.Popup(m_strPath + "\\" + m_strLotEnd + ".trf Is Wrong Path !!");
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
                    m_log.Add(m_strPath + "\\" + m_strLotEnd + ".trf LotEnd File Saved.");
                }
                catch (Exception ex)
                {
                    m_log.Popup(m_strPath + "\\" + m_strLotEnd + ".trf Create Error !!");
                    m_log.Add(ex.Message);
                    sw.Close();
                }

                try
                {
                    sw = new StreamWriter(new FileStream(m_strPathBackup + "\\" + m_strLotEnd + ".trf", FileMode.Create));
                }
                catch (Exception ex)
                {
                    m_log.Popup(m_strPathBackup + "\\" + m_strLotEnd + ".trf Is Wrong Path !!");
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
                    m_log.Add(m_strPathBackup + "\\" + m_strLotEnd + ".trf LotEnd File Saved.");
                }
                catch (Exception ex)
                {
                    m_log.Popup(m_strPathBackup + "\\" + m_strLotEnd + ".trf Create Error !!");
                    m_log.Add(ex.Message);
                    sw.Close();
                }
            }
        }

        void SetKlarf(ref Info_Wafer infoWafer) // ing 170426
        {
            Info_Carrier infoCarrier;
            if (infoWafer == null) infoWafer = new Info_Wafer(-1, -1, -1, m_log, Wafer_Size.eSize.mm300);
            infoCarrier = m_auto.ClassHandler().ClassCarrier(infoWafer.m_nLoadPort);
            if (infoCarrier == null)
            {
                infoCarrier = new Info_Carrier(-1);
                infoCarrier.m_wafer = new Wafer_Size(Wafer_Size.eSize.mm300, m_log);
            }
            m_klarf.m_img[0] = m_aImg[2];

            m_klarf.m_strLotID = infoCarrier.m_strLotID;
            m_klarf.m_strCSTID = infoCarrier.m_strCarrierID; // ingyu 180629
            int nTemp = m_klarf.m_strLotID.IndexOf("_"); // _hj Klarf 데이터 LotID 수정 170320
            if (nTemp > 0)
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
                m_klarf.m_fDiePitchX = 300000; // ing 170306
                m_klarf.m_fDiePitchY = 300000; // ing 170306
            }
            else
            {
                m_klarf.m_fDiePitchX = 100000; // ing 170306
                m_klarf.m_fDiePitchY = 100000; // ing 170306
            }
            m_klarf.m_fDieOriginX = 0.000000e+000;
            m_klarf.m_fDieOriginY = 0.000000e+000;
            m_klarf.m_fSampleCenterLocationX = 1.500000e+005;
            m_klarf.m_fSampleCenterLocationY = 1.500000e+005;
            m_klarf.m_fResolution = m_umX;
        }

        void SaveKlarf() // ing 170426
        {
            //m_strLotEnd = "LotEnd_" + m_auto.ClassRecipe().m_sRecipe + "_" + m_klarf.m_strLotID + "_00-" + m_klarf.m_nSlot.ToString("00");
            //m_klarf.m_strTiffFileName = m_auto.ClassRecipe().m_sRecipe + "_" + m_klarf.m_strLotID + "_00-" + m_klarf.m_nSlot.ToString("00") + ".tif";
            m_strLotEnd = "LotEnd_" + m_auto.ClassRecipe().m_sRecipe + "_" + m_klarf.m_strCSTID + "_00-" + m_klarf.m_nSlot.ToString("00");
            m_klarf.m_strTiffFileName = m_auto.ClassRecipe().m_sRecipe + "_" + m_klarf.m_strCSTID + "_00-" + m_klarf.m_nSlot.ToString("00") + ".tif";
            string strDate = DateTime.Now.Date.Year.ToString() + DateTime.Now.Date.Month.ToString() + DateTime.Now.Date.Day.ToString();
            Directory.CreateDirectory(m_strPathBackup);
            Directory.CreateDirectory(m_strPathBackup + "\\" + strDate);
            //string strPath = m_strPath + "\\" + m_auto.ClassRecipe().m_sRecipe + "_" + m_klarf.m_strLotID + "_00-" + m_klarf.m_nSlot.ToString("00");
            //string strPathBackup = m_strPathBackup + "\\" + strDate + "\\" + m_auto.ClassRecipe().m_sRecipe + "_" + m_klarf.m_strLotID + "_00-" + m_klarf.m_nSlot.ToString("00");
            string strPath = m_strPath + "\\" + m_auto.ClassRecipe().m_sRecipe + "_" + m_klarf.m_strCSTID + "_00-" + m_klarf.m_nSlot.ToString("00");
            string strPathBackup = m_strPathBackup + "\\" + strDate + "\\" + m_auto.ClassRecipe().m_sRecipe + "_" + m_klarf.m_strCSTID + "_00-" + m_klarf.m_nSlot.ToString("00");
            m_klarf.SaveKlarf(strPath, true, m_aDefect, 0, 0, m_dNotch);
            m_klarf.SaveKlarf(strPathBackup, true, m_aDefect, 0, 0, m_dNotch);
        }

    }
}
