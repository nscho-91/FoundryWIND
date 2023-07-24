using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Media.Imaging;
using ezAutoMom;
using ezTools;

namespace ezAuto_EFEM
{
    public partial class HW_BackSide_ATI_AOI : DockContent
    {
        string m_id;
        Auto_Mom m_auto;
        Log m_log;

        ezGrid[] m_grid = new ezGrid[2];
        Recipe_Mom m_recipe;
        public ezView[] m_view = new ezView[2];
        public ezImg[] m_inspImg = new ezImg[2];
        public ezImg m_imgDown;
        public bool m_bDraw = false;
        bool m_bRingMark = false;
        bool m_bFindEdge = false;

        public HW_BackSide_ATI_AOIData m_aoiData = new HW_BackSide_ATI_AOIData(); //wafer 데이터, 중심점, 반지름, 다이정보
        HW_BackSide_ATI_AOI_Edge m_aoiEdge = new HW_BackSide_ATI_AOI_Edge();//에지검출
        HW_BackSide_ATI_AOI_WhiteBalance[] m_aoiWhite = new HW_BackSide_ATI_AOI_WhiteBalance[2];
        HW_BackSide_ATI_AOI_Ring[] m_aoiRing = new HW_BackSide_ATI_AOI_Ring[2];
        HW_BackSide_ATI_AOI_Surface[,] m_aoiSurface = new HW_BackSide_ATI_AOI_Surface[3, 2];
        HW_BackSide_ATI_AOI_Sobel[,] m_aoiSobel = new HW_BackSide_ATI_AOI_Sobel[3, 2];
        HW_BackSide_ATI_AOI_FFT[] m_aoiFFT = new HW_BackSide_ATI_AOI_FFT[2];
        HW_BackSide_ATI_AOI_StainMark m_aoiStain;
        HW_BackSide_ATI_AOI_Chipping m_aoiChipping;
        ArrayList m_aIsland = new ArrayList();//검사하고 머지된 불량 정보
        public HW_Klarf[] m_klarf = new HW_Klarf[2];
        public HW_Backside_InfoWafer[] m_bsInfoWafer = new HW_Backside_InfoWafer[2];

        // For PM
        public string m_strPMRecipe = "GOLDEN_RECIPE";
        public CPoint[] m_cpScaleBarRange = new CPoint[2];

        //For ADC
        public string m_strADCBmp = "D://ADCBmp";
        int m_nADCImgSize = 500;
        
        public HW_BackSide_ATI_AOI()
        {
            m_klarf[0] = new HW_Klarf("Klarf");
            m_klarf[1] = new HW_Klarf("KlarfBuffer");
            InitializeComponent();
        }

        public void Init(string id, Auto_Mom auto, Log log)
        {
            m_id = id;
            m_auto = auto;
            m_log = log;
            m_grid[0] = new ezGrid(m_id, grid0, m_log, false);
            m_grid[1] = new ezGrid(m_id, grid1, m_log, false);
            m_view[0] = new ezView("View_0", 1, auto);
            m_view[1] = new ezView("View_1", 2, auto);
            m_inspImg[0] = new ezImg("InspImage_0", m_log);
            m_inspImg[1] = new ezImg("InspImage_1", m_log);
            m_imgDown = new ezImg("DownImg", m_log);
            m_recipe = m_auto.ClassRecipe();
            m_klarf[0].Init(m_auto, m_log); 
            m_klarf[1].Init(m_auto, m_log);
            m_bsInfoWafer[0] = new HW_Backside_InfoWafer(m_log);
            m_bsInfoWafer[1] = new HW_Backside_InfoWafer(m_log);
            m_aoiEdge = new HW_BackSide_ATI_AOI_Edge();
            m_aoiStain = new HW_BackSide_ATI_AOI_StainMark();
            m_aoiStain.Init("AOI_Stain1", m_aoiData, m_log);
            m_aoiChipping = new HW_BackSide_ATI_AOI_Chipping();
            m_aoiChipping.Init("AOI_Chipping", m_aoiData, m_log);
            // for ADC
            HW_BackSide_ATI_ADC.Instance.Init("ADC");
            for (int n = 0; n < 2; n++)
            {
                for (int m = 0; m < 3; m++ )
                {
                    m_aoiSurface[m, n] = new HW_BackSide_ATI_AOI_Surface();
                    m_aoiSobel[m, n] = new HW_BackSide_ATI_AOI_Sobel();
                    m_aoiSurface[m, n].Init("AOI_Blob" + n.ToString() + "#" + m.ToString(), m_aoiData, m_log);
                    m_aoiSobel[m, n].Init("AOI_Sobel" + n.ToString() + "#" + m.ToString(), m_aoiData, m_log);
                }
                m_aoiWhite[n] = new HW_BackSide_ATI_AOI_WhiteBalance();
                m_aoiRing[n] = new HW_BackSide_ATI_AOI_Ring();
                m_aoiFFT[n] = new HW_BackSide_ATI_AOI_FFT();
                if (n == 0) m_aoiData.Init("AOI", m_log);
                if (n == 0) m_aoiEdge.Init("AOI_Edge" + n.ToString(), m_aoiData, m_log);
                m_aoiWhite[n].Init("AOI_WhiteBalance" + n.ToString(), m_aoiData, m_log);
                m_aoiFFT[n].Init("AOI_FFT" + n.ToString(), m_aoiData, m_log);
                m_aoiRing[n].Init("AOI_Ring" + n.ToString(), m_aoiData, m_log);
            }
            RunGrid(0, eGrid.eInit);
            RunGrid(1, eGrid.eInit);
        }

        public virtual void ThreadStop()
        {
            int n;
            for (n = 0; n < 2; n++)
            {
                for (int m = 0; m < 3; m++)
                {
                    m_aoiSobel[m, n].ThreadStop();
                    m_aoiSurface[m, n].ThreadStop();
                }
                m_view[n].ThreadStop();
                m_aoiWhite[n].ThreadStop();
            }
            m_aoiStain.ThreadStop();
            m_aoiChipping.ThreadStop(); 
            m_aoiData.ThreadStop();
        }

        protected override string GetPersistString()
        {
            return GetType().ToString() + "," + m_id;
        }

        public bool IsPersistString(string str)
        {
            return GetPersistString() == str;
        }

        public void RunHandlerGrid(ezGrid rGrid, eGrid eMode, ezJob job = null)
        {
            HW_BackSide_ATI_ADC.Instance.RunGrid(rGrid, eMode, job);
            rGrid.Set(ref m_nADCImgSize, HW_BackSide_ATI_ADC.Instance.m_id, "ImgSize", "ADC Image Size");
        }

        public void RunGrid(int nID, eGrid eMode, ezJob job = null)
        {
            m_grid[nID].Update(eMode, job);
            m_aoiData.RunGrid(m_grid[nID], nID, eMode, job);
            if (nID == 0) m_aoiEdge.RunGrid(m_grid[nID], eMode, job);
            m_aoiWhite[nID].RunGrid(m_grid[nID], eMode, job);
            for (int m = 0; m < 3; m++) m_aoiSurface[m, nID].RunGrid(m_grid[nID], eMode, job);
            for (int m = 0; m < 3; m++) m_aoiSobel[m, nID].RunGrid(m_grid[nID], eMode, job);
            m_aoiFFT[nID].RunGrid(m_grid[nID], eMode, job);
            m_aoiRing[nID].RunGrid(m_grid[nID], eMode, job);
            if (nID == 1) m_aoiStain.RunGrid(m_grid[nID], eMode, job);
            if (nID == 0) m_aoiChipping.RunGrid(m_grid[nID], eMode, job);
            if (nID == 1) HW_BackSide_ATI_ADC.Instance.RunRecipeGrid(m_grid[nID], eMode, job);
            m_grid[nID].Refresh();
        }

        public void JobOpen(ezJob job)
        {
            RunGrid(0, eGrid.eJobOpen, job);
            RunGrid(1, eGrid.eJobOpen, job);
            RunGrid(0, eGrid.eInit, null);
            RunGrid(1, eGrid.eInit, null);
            if (m_recipe.m_bUseBackSide) m_aoiChipping.ReadNotchArr(m_auto.ClassRecipe().m_strPath + "\\" + m_auto.ClassRecipe().m_sRecipe + ".notch");
        }

        public void JobSave(ezJob job)
        {
            RunGrid(0, eGrid.eJobSave, job);
            RunGrid(1, eGrid.eJobSave, job);
        }

        private void grid0_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid[0].PropertyChange(e);
            RunGrid(0, eGrid.eUpdate);
        }

        private void grid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid[1].PropertyChange(e);
            RunGrid(1, eGrid.eUpdate);
        }

        private void buttonParamSave_Click(object sender, EventArgs e)
        {
            m_recipe.JobSave();
        }

        private void buttonInspection_Click(object sender, EventArgs e)
        {
            if (m_auto.ClassWork().IsRun()) return;
            HW_Klarf klarf;
            Info_Wafer infoWafer = new Info_Wafer(-1, -1, -1, m_log);
            infoWafer.m_strWaferID = System.DateTime.Now.TimeOfDay.Hours.ToString() + System.DateTime.Now.TimeOfDay.Minutes.ToString() + System.DateTime.Now.TimeOfDay.Seconds.ToString();
            Info_Carrier infoCarrier = new Info_Carrier(-1);
            ClearDefect();
            Inspect(infoWafer, infoCarrier, 0);
            Inspect(infoWafer, infoCarrier, 1);
            if (!m_aoiData.m_bSaveKlarf) return;
            klarf = m_klarf[0];
            if (klarf.m_bBusy) klarf = m_klarf[1];
            Thread threadKlarf = new Thread(delegate()
            {
                AfterInspect(klarf, infoWafer, infoCarrier, 0);
            });
            threadKlarf.Start();
        }

        private void buttonInspection90_Click(object sender, EventArgs e)
        {
            if (m_auto.ClassWork().IsRun()) return;
            HW_Klarf klarf;
            Info_Wafer infoWafer = new Info_Wafer(-1, -1, -1, m_log);
            infoWafer.m_strWaferID = System.DateTime.Now.TimeOfDay.Hours.ToString() + System.DateTime.Now.TimeOfDay.Minutes.ToString() + System.DateTime.Now.TimeOfDay.Seconds.ToString();
            Info_Carrier infoCarrier = new Info_Carrier(-1);
            infoWafer.m_eAngleRF = Info_Wafer.eAngleRF.R90;
            ClearDefect();
            Inspect(infoWafer, infoCarrier, 0);
            if (!m_aoiData.m_bSaveKlarf) return;
            klarf = m_klarf[0];
            if (klarf.m_bBusy) klarf = m_klarf[1];
            Thread threadKlarf = new Thread(delegate()
            {
                AfterInspect(klarf, infoWafer, infoCarrier, 1);
            });
            threadKlarf.Start();
        }

        private void buttonInspMerge_Click(object sender, EventArgs e)
        {
            //if (m_auto.ClassWork().IsRun()) return;
            HW_Klarf klarf;
            Info_Wafer infoWafer = new Info_Wafer(-1, -1, -1, m_log);
            infoWafer.m_strWaferID = System.DateTime.Now.TimeOfDay.Hours.ToString() + System.DateTime.Now.TimeOfDay.Minutes.ToString() + System.DateTime.Now.TimeOfDay.Seconds.ToString();
            Info_Carrier infoCarrier = new Info_Carrier(-1);
            if (!m_aoiData.m_bSaveKlarf) return;
            klarf = m_klarf[0];
            if (klarf.m_bBusy) klarf = m_klarf[1];
            Thread threadKlarf = new Thread(delegate()
            {
                AfterInspect(klarf, infoWafer, infoCarrier, 2);
            });
            threadKlarf.Start();
        }

        private void BackSide_Inspection_Resize(object sender, EventArgs e)
        {
            System.Windows.Forms.Control control = (System.Windows.Forms.Control)sender;
            Size sz = control.Size;
            grid0.Size = new Size(control.Size.Width - buttonParamSave.Size.Width, control.Size.Height / 2) - new Size(20, 20);
            grid1.Location = new Point(grid0.Location.X, grid0.Location.Y + grid0.Size.Height + 10);
            grid1.Size = grid0.Size;
            buttonParamSave.Location = new Point(grid0.Location.X + grid0.Width + 5, grid0.Location.Y);
            buttonInspection0.Location = new Point(buttonParamSave.Location.X, buttonParamSave.Location.Y + buttonParamSave.Size.Height + 5);
            buttonInspection90.Location = new Point(buttonParamSave.Location.X, buttonInspection0.Location.Y + buttonInspection0.Size.Height + 5);
            buttonInspMerge.Location = new Point(buttonParamSave.Location.X, buttonInspection90.Location.Y + buttonInspection90.Size.Height + 5);
            buttonReadMap.Location = new Point(buttonParamSave.Location.X, buttonInspMerge.Location.Y + buttonInspMerge.Size.Height + 5);
            buttonInspView0.Location = new Point(buttonParamSave.Location.X, buttonReadMap.Location.Y + buttonReadMap.Size.Height + 5);
            buttonInspView1.Location = new Point(buttonParamSave.Location.X, buttonInspView0.Location.Y + buttonInspView0.Size.Height + 5);
            buttonClear.Location = new Point(buttonParamSave.Location.X, buttonInspView1.Location.Y + buttonInspView1.Size.Height + 5);
            buttonSaveNotch.Location = new Point(buttonParamSave.Location.X, buttonClear.Location.Y + buttonClear.Size.Height + 5);
        }

        public void ClearDefect()
        {
            m_bDraw = false;
            m_aoiChipping.Clear(); 
            m_aoiStain.m_aMerge.Clear();
            m_aoiStain.m_aIsland.Clear(); // ing 170707
            m_aoiStain.m_strList.Clear();
            for (int n = 0; n < 2; n++)
            {
                for (int m = 0; m < 3; m++)
                {
                    m_aoiSurface[m, n].m_aIsland.Clear();
                    m_aoiSobel[m, n].m_aIsland.Clear();
                }
                m_aoiRing[n].ClearDefect();
                m_aoiFFT[n].ClearDefect();
            }
            m_bsInfoWafer[0].m_bInspDone0 = false;
            m_bsInfoWafer[0].m_bInspDone90 = false;
            m_bsInfoWafer[1].m_bInspDone0 = false;
            m_bsInfoWafer[1].m_bInspDone90 = false;
        }
        public void Inspect(Info_Wafer infoWafer, Info_Carrier infoCarrier, int nID)
        {
            try
            {
                if (nID == 1 && m_recipe.m_strFile.ToLower().IndexOf(m_strPMRecipe.ToLower()) > -1) return;
                if (infoCarrier == null) infoCarrier = new Info_Carrier(-1);
                HW_Backside_InfoWafer bsInfoWafer = null;
                int nMerge, nRotateMode;
                m_bDraw = false;
                m_bRingMark = false;
                m_bFindEdge = true;

                if (infoWafer.m_eAngleRF == Info_Wafer.eAngleRF.R0 || infoWafer.m_eAngleRF == Info_Wafer.eAngleRF.R180) nRotateMode = 0;
                else nRotateMode = 1;

                if (infoWafer.m_eAngleRF == Info_Wafer.eAngleRF.R90)
                {
                    m_view[nID].m_imgView.m_pImg = m_view[nID].m_imgView.m_pImg.Rotate("90");
                    //m_view[nID].m_imgView.m_pImg.Shift(m_cpRotateOffset);
                }
                //else if (infoWafer.m_eAngleRF == Info_Wafer.eAngleRF.R180) m_view[nID].m_imgView.m_pImg = m_view[nID].m_imgView.m_pImg.Rotate("180");

                m_inspImg[nID].ReAllocate(m_view[nID].m_imgView.m_pImg); //원본 이미지 복사
                m_inspImg[nID].Copy(m_view[nID].m_imgView.m_pImg, new CPoint(0, 0), new CPoint(0, 0), m_view[nID].m_imgView.m_pImg.m_szImg);
                ezImg img = m_inspImg[nID];
                if (img.GetBitmap() == null) return;

                for (int n = 0; n < 2; n++)
                {
                    if (m_bsInfoWafer[n].m_infoWafer != null)
                    {
                        if (infoWafer.m_nSlot == m_bsInfoWafer[n].m_infoWafer.m_nSlot)
                        {
                            bsInfoWafer = m_bsInfoWafer[n];
                            m_log.Add("BacksideInfoWafer Index : " + n.ToString());
                            break;
                        }
                    }
                }
                if (bsInfoWafer == null)
                {
                    for (int n = 0; n < 2; n++)
                    {
                        if (m_bsInfoWafer[n].IsReady())
                        {
                            bsInfoWafer = m_bsInfoWafer[n];
                            m_log.Add("BacksideInfoWafer Change Index : " + n.ToString());
                            break;
                        }
                    }
                    if (bsInfoWafer == null)
                    {
                        m_log.Popup("Backside Inspection Is not Finished !!");
                        return;
                    }
                }
                if (nRotateMode == 0) bsInfoWafer.m_bInspDone0 = false;
                else bsInfoWafer.m_bInspDone90 = false;

                if ((nID == 0) && m_aoiEdge.Inspect(img, nRotateMode)) m_bFindEdge = false;
                if (!m_bFindEdge)
                {
                    m_log.Popup("Edge Find Fail !!");
                    return;
                }
                if (nID == 0 && nRotateMode == 0)
                {
                    bsInfoWafer.m_cpOffCenter = m_aoiEdge.m_cpShift;
                    bsInfoWafer.m_bOffCenter = m_aoiEdge.m_bOffCenter;
                    bsInfoWafer.m_eOrientation = m_aoiEdge.m_eOrientation;
                    if (m_aoiChipping.Inspect(img)) return;
                    bsInfoWafer.m_strEdgeChipping = m_aoiChipping.m_strCode;
                    bsInfoWafer.m_aEdgeChipping = (ArrayList)m_aoiChipping.m_arrDefect.Clone();
                }
                
                bsInfoWafer.m_dieMap = new HW_DieMap(m_aoiData.m_dieMap);
                bsInfoWafer.m_infoCarrier = infoCarrier;
                bsInfoWafer.m_infoWafer = infoWafer;
                bsInfoWafer.m_fResolution = m_aoiData.m_fResolution;
                bsInfoWafer.m_dieMap = m_aoiData.m_dieMap;
                bsInfoWafer.m_cpCenter[nRotateMode] = m_aoiData.m_cpCenter;
                bsInfoWafer.m_dR[nRotateMode] = m_aoiData.m_dR;
                bsInfoWafer.m_fTheta[nRotateMode] = m_aoiData.m_fTheta;
                bsInfoWafer.m_nDown = m_aoiData.m_nDown;
                bsInfoWafer.m_strPath = m_aoiData.m_strPathKlarf;
                bsInfoWafer.m_strModel = infoCarrier.m_strRecipe;

                if (nRotateMode == 1)
                {
                    m_view[nID].m_imgView.m_pImg.Shift(bsInfoWafer.m_cpCenter[0] - bsInfoWafer.m_cpCenter[1]);
                    m_inspImg[nID].Shift(bsInfoWafer.m_cpCenter[0] - bsInfoWafer.m_cpCenter[1]);
                    m_aoiEdge.Inspect(img, nRotateMode);
                }

                bsInfoWafer.m_imgInspect[nID, nRotateMode].ReAllocate(m_view[nID].m_imgView.m_pImg);
                bsInfoWafer.m_imgInspect[nID, nRotateMode].Copy(m_view[nID].m_imgView.m_pImg, new CPoint(0, 0), new CPoint(0, 0), img.m_szImg);
                m_aoiData.MakeDownImage(img);
                bsInfoWafer.m_imgDown[nID, nRotateMode].ReAllocate(m_aoiData.m_imgDown);
                bsInfoWafer.m_imgDown[nID, nRotateMode].Copy(m_aoiData.m_imgDown, new CPoint(0, 0), new CPoint(0, 0), m_aoiData.m_imgDown.m_szImg);

                m_aoiEdge.CheckWrongNotchOrientation(nRotateMode);
                m_aoiWhite[nID].Inspect(img, m_aoiData.m_dR - 65);

                m_aoiData.MakeDownImage(img);
                m_imgDown.ReAllocate(m_aoiData.m_imgDown);
                m_imgDown.Copy(m_aoiData.m_imgDown, new CPoint(0, 0), new CPoint(0, 0), m_aoiData.m_imgDown.m_szImg);

                if (m_recipe.m_strFile.ToLower().IndexOf(m_strPMRecipe.ToLower()) > -1)
                {
                    m_log.Add("PM Inspection Start");
                    m_aoiSurface[0, 0].InspectPM(img, m_aoiData.m_dR - 70, m_aoiData.m_gvSigma, m_cpScaleBarRange);
                    m_bDraw = true;
                    m_auto.ClassHandler().ClassBackSide().SavePM();
                    return;
                }

                //CoarseGrindMark
                m_aoiFFT[nID].Inspect(m_aoiData.m_dR);
                bsInfoWafer.m_bCorseGrindMark = (m_aoiFFT[0].IsDefect() || m_aoiFFT[1].IsDefect());
                bsInfoWafer.m_strCoarseGrindCode = m_aoiFFT[nID].m_strCode;
                if (bsInfoWafer.m_bCorseGrindMark)
                {
                    if (nRotateMode == 0) bsInfoWafer.m_bInspDone0 = true;
                    else bsInfoWafer.m_bInspDone90 = true;
                    m_bDraw = true;
                    return;
                }

                //RingMark
                m_aoiRing[nID].Inspect(img);
                m_bRingMark = bsInfoWafer.m_bRingMark = (m_aoiRing[0].IsDefect() || m_aoiRing[1].IsDefect());
                bsInfoWafer.m_nSearchRing = (m_aoiRing[0].m_nSearchPosition + m_aoiRing[1].m_nSearchPosition) / 2;
                bsInfoWafer.m_nRingWidth = (m_aoiRing[0].m_nSearchWidth + m_aoiRing[1].m_nSearchWidth) / 2;
                bsInfoWafer.m_strRingCode = m_aoiRing[0].m_strCode;
                //MappingRingMark(nID);

                //Blob
                for (int m = 0; m < 3; m++) m_aoiSurface[m, nID].Inspect(img, m_aoiData.m_dR - 70, m_aoiData.m_gvSigma, nRotateMode);

                //Sobel
                for (int m = 0; m < 3; m++) m_aoiSobel[m, nID].Inspect(m_aoiData.m_dR - 120, m_aoiEdge.GetNotchPos(), nRotateMode);

                if (nID == 1 && nRotateMode == 0) m_aoiStain.Inspect(img, m_aoiEdge.GetEdge());

                #region
                //주석
                /*nMerge = m_aoiSurface[0, nID].m_nMerge; // Merge Surface0 and Surface1
                if (nMerge > m_aoiSurface[1, nID].m_nMerge) nMerge = m_aoiSurface[1, nID].m_nMerge;
                MergeIsland(m_aoiSurface[0, nID].m_aIsland, m_aoiSurface[1, nID].m_aIsland, nMerge);

                nMerge = m_aoiSurface[1, nID].m_nMerge; // Merge Surface1 and Surface2
                if (nMerge > m_aoiSurface[2, nID].m_nMerge) nMerge = m_aoiSurface[2, nID].m_nMerge;
                MergeIsland(m_aoiSurface[1, nID].m_aIsland, m_aoiSurface[2, nID].m_aIsland, nMerge);

                nMerge = m_aoiSurface[0, nID].m_nMerge; // Merge Surface0 and Surface2
                if (nMerge > m_aoiSurface[2, nID].m_nMerge) nMerge = m_aoiSurface[2, nID].m_nMerge;
                MergeIsland(m_aoiSurface[0, nID].m_aIsland, m_aoiSurface[2, nID].m_aIsland, nMerge);
                
                nMerge = m_aoiSobel[0, nID].m_nBlobMerge; // Merge m_aoiSobel0 and m_aoiSobel1
                if (nMerge > m_aoiSobel[1, nID].m_nBlobMerge) nMerge = m_aoiSobel[1, nID].m_nBlobMerge;
                MergeIsland(m_aoiSobel[0, nID].m_aIsland, m_aoiSobel[1, nID].m_aIsland, nMerge);

                nMerge = m_aoiSobel[1, nID].m_nBlobMerge; // Merge Surface1 and Surface2
                if (nMerge > m_aoiSobel[2, nID].m_nBlobMerge) nMerge = m_aoiSobel[2, nID].m_nBlobMerge;
                MergeIsland(m_aoiSobel[1, nID].m_aIsland, m_aoiSobel[2, nID].m_aIsland, nMerge);

                nMerge = m_aoiSobel[0, nID].m_nBlobMerge; // Merge Surface0 and Surface2
                if (nMerge > m_aoiSobel[2, nID].m_nBlobMerge) nMerge = m_aoiSobel[2, nID].m_nBlobMerge;
                MergeIsland(m_aoiSobel[0, nID].m_aIsland, m_aoiSobel[2, nID].m_aIsland, nMerge);*/
                #endregion

                for (int m = 0; m < 3; m++)
                {
                    for (int n = 1; n < 3; n++)
                    {
                        if (m == n) continue;
                        // Merge Surface
                        nMerge = m_aoiSurface[m, nID].m_nMerge; 
                        if (nMerge > m_aoiSurface[n, nID].m_nMerge) nMerge = m_aoiSurface[n, nID].m_nMerge;
                        MergeIsland(m_aoiSurface[m, nID].m_aIsland, m_aoiSurface[n, nID].m_aIsland, nMerge);
                        // Merge Sobel
                        nMerge = m_aoiSobel[n, nID].m_nBlobMerge;
                        if (nMerge > m_aoiSobel[m, nID].m_nBlobMerge) nMerge = m_aoiSobel[m, nID].m_nBlobMerge;
                        MergeIsland(m_aoiSobel[m, nID].m_aIsland, m_aoiSobel[n, nID].m_aIsland, nMerge);
                    }
                }
                
                for (int m = 0; m < 3; m++)
                {
                    for (int n = 0; n < 3; n++)
                    {
                        nMerge = m_aoiSurface[m, nID].m_nMerge;
                        if (nMerge > m_aoiSobel[n, nID].m_nBlobMerge) nMerge = m_aoiSobel[n, nID].m_nBlobMerge;
                        MergeIsland(m_aoiSobel[n, nID].m_aIsland, m_aoiSurface[m, nID].m_aIsland, nMerge);
                    }
                }

                for (int m = 0; m < 3; m++)
                {
                    m_aoiSobel[m, nID].ClearInvalidIsland();
                    m_aoiSurface[m, nID].ClearInvalidIsland();
                    bsInfoWafer.m_aIslandSobel[nID, nRotateMode, m] = (ArrayList)m_aoiSobel[m, nID].m_aIsland.Clone();
                    bsInfoWafer.m_aIslandBlob[nID, nRotateMode, m] = (ArrayList)m_aoiSurface[m, nID].m_aIsland.Clone();
                }
                if (nID == 1 && nRotateMode == 0) bsInfoWafer.m_aIslandStain[nRotateMode] = (ArrayList)m_aoiStain.m_aIsland.Clone();

                for (int m = 0; m < 3; m++) while (MergeIsland(m, nID)) ;

                for (int m = 0; m < 3; m++)
                {
                    Classify(bsInfoWafer.m_imgInspect[nID, nRotateMode], m_aoiSobel[m, nID].m_aIsland, infoWafer);
                    Classify(bsInfoWafer.m_imgInspect[nID, nRotateMode], m_aoiSurface[m, nID].m_aIsland, infoWafer);
                }

                if (nRotateMode == 0) bsInfoWafer.m_bInspDone0 = true;
                else bsInfoWafer.m_bInspDone90 = true;
                m_bDraw = true;
            }
            catch (System.Exception ex)
            {
                m_log.Popup(infoWafer.m_strWaferID + " Insepction Fail !!");
                m_log.Add(ex.Message);
            }
        }

        
        public void AfterInspect(HW_Klarf klarf, Info_Wafer infoWafer, Info_Carrier infoCarrier, int nRotate)
        {
            if (m_bsInfoWafer[0].m_infoWafer != null && infoWafer.m_nSlot == m_bsInfoWafer[0].m_infoWafer.m_nSlot) m_bsInfoWafer[0].AfterInspect(klarf, infoWafer, infoCarrier, nRotate);
            if (m_bsInfoWafer[1].m_infoWafer != null && infoWafer.m_nSlot == m_bsInfoWafer[1].m_infoWafer.m_nSlot) m_bsInfoWafer[1].AfterInspect(klarf, infoWafer, infoCarrier, nRotate);
        }

        void MergeIsland(ArrayList aIsland0, ArrayList aIsland1, int nMerge)
        {
            foreach (azBlob.Island island0 in aIsland0)
            {
                foreach (azBlob.Island island1 in aIsland1)
                {
                    MergeIsland(island0, island1, nMerge);
                }
            }
        }

        void MergeIsland(azBlob.Island island0, azBlob.Island island1, int nMerge)
        {
            if (island0.m_bValid == false) return;
            if (island1.m_bValid == false) return;
            if (island0.IsSeperate(island1, nMerge)) return;
            if (island0.m_nSize > island1.m_nSize) island0.Merge(island1, nMerge);
            else island1.Merge(island0, nMerge);
        }

        bool MergeIsland(int nCount, int nID)
        {
            bool bMerge = false;
            for (int n = 0; n < m_aIsland.Count; n++)
            {
                if (((azBlob.Island)m_aIsland[n]).m_bValid)
                {
                    azBlob.Island island = (azBlob.Island)m_aIsland[n];
                    for (int m = n + 1; m < m_aIsland.Count; m++)
                    {
                        if (((azBlob.Island)m_aIsland[m]).m_bValid)
                        {
                            if (island.Merge((azBlob.Island)m_aIsland[m], m_aoiSurface[nCount, nID].m_nMerge)) bMerge = true;
                        }
                        if (((azBlob.Island)m_aIsland[n]).m_cp0.x > ((azBlob.Island)m_aIsland[m]).m_cp0.x)
                        {
                            // ing
                        }
                    }
                }
            }
            ClearInvalidIsland();
            return bMerge;
        }

        public void ClearInvalidIsland()
        {
            ArrayList aIsland = new ArrayList();
            foreach (azBlob.Island island in m_aIsland)
            {
                if (island.m_bValid) aIsland.Add(island);
            }
            m_aIsland = aIsland;
        }

        void Classify(ezImg img, ArrayList arrIsland, Info_Wafer infoWafer)
        {
            Directory.CreateDirectory(m_strADCBmp);
            Directory.CreateDirectory(m_strADCBmp + "\\" + m_auto.ClassRecipe().m_sRecipe);
            Directory.CreateDirectory(m_strADCBmp + "\\" + m_auto.ClassRecipe().m_sRecipe + "\\" + infoWafer.m_strWaferID + "_" + infoWafer.m_strLotID + "_" + infoWafer.m_nSlot.ToString());
            string strBMP;
            string strName = string.Empty;
            double dScore = 0.0f;
            Bitmap corpped = null;
            double dDisFromEdge = 0.0f;
            double x1, y1, x2, y2;
            int nIndex = 0;
            foreach (azBlob.Island island in arrIsland)
            {
                CPoint cpSize = island.m_cp1 - island.m_cp0;
                CPoint cpCenter = (island.m_cp1 + island.m_cp0) / 2;
                CPoint cp0, cp1;
                if (cpSize.x < m_nADCImgSize && cpSize.y < m_nADCImgSize)
                {
                    while (!IsInside(cpCenter + new CPoint(m_nADCImgSize / 2, m_nADCImgSize / 2)) || !IsInside(cpCenter + new CPoint(-m_nADCImgSize / 2, m_nADCImgSize / 2)) 
                        || !IsInside(cpCenter + new CPoint(m_nADCImgSize / 2, -m_nADCImgSize / 2)) || !IsInside(cpCenter + new CPoint(-m_nADCImgSize / 2, -m_nADCImgSize / 2)))
                    {
                        dDisFromEdge = m_aoiData.m_dR - m_aoiData.m_cpCenter.GetL(cpCenter);
                        x1 = cpCenter.x - m_aoiData.m_cpCenter.x;
                        y1 = cpCenter.y - m_aoiData.m_cpCenter.y;
                        x2 = y2 = 0.5;
                        if (x1 < 0) x2 = -0.5;
                        if (y1 < 0) y2 = -0.5;
                        cpCenter = cpCenter - new CPoint((int)Math.Round(x2 + 0.1 * x1 * (m_nADCImgSize / 2 * Math.Sqrt(2) - dDisFromEdge) / m_aoiData.m_dR), (int)(y2 + 0.1 * y1 * Math.Round(m_nADCImgSize / 2 * Math.Sqrt(2) - dDisFromEdge) / m_aoiData.m_dR));
                    }
                    corpped = img.GetBitmapROI(cpCenter + new CPoint(m_nADCImgSize / 2, m_nADCImgSize / 2), cpCenter - new CPoint(m_nADCImgSize / 2, m_nADCImgSize / 2));
                    if (HW_BackSide_ATI_ADC.Instance.UseAdc == true)
                    {
                        ADC.Instance.Classify(HW_BackSide_ATI_ADC.Instance.Recipe, corpped, out strName, out dScore, m_log);
                        if (dScore > HW_BackSide_ATI_ADC.Instance.Score)
                        {
                            island.m_strCode = HW_BackSide_ATI_ADC.Instance.GetRoughbin(strName);
                        }
                    }
                    strBMP = m_strADCBmp + "\\" + m_auto.ClassRecipe().m_sRecipe + "\\" + infoWafer.m_strWaferID + "_" + infoWafer.m_strLotID + "_" + infoWafer.m_nSlot.ToString() + "\\" + nIndex.ToString() + "_" + strName;
                    corpped.Save(strBMP + cpCenter.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                }
                else
                {
                    int[] aClassifiedDefect = new int[HW_BackSide_ATI_ADC.Instance.sDefectName.Length];
                    ArrayList aListCode = new ArrayList();
                    cp0 = cp1 = (CPoint)island.m_aPosition[0];
                    for (int n = 1; n < island.m_aPosition.Count - 1; n++)
                    {
                        cp1 = (CPoint)island.m_aPosition[n];
                        cpSize = cp1 - cp0;
                        if (cpSize.x >= m_nADCImgSize || cpSize.y >= m_nADCImgSize)
                        {
                            cpCenter = (cp1 + cp0) / 2;
                            dDisFromEdge = m_aoiData.m_dR - m_aoiData.m_cpCenter.GetL(cpCenter);
                            if (dDisFromEdge < m_nADCImgSize / 2 * Math.Sqrt(2))
                            {
                                x1 = cpCenter.x - m_aoiData.m_cpCenter.x;
                                y1 = cpCenter.y - m_aoiData.m_cpCenter.y;
                                cpCenter = cpCenter - new CPoint((int)(x1 * (m_nADCImgSize / 2 * Math.Sqrt(2) - dDisFromEdge) / m_aoiData.m_dR), (int)(y1 * (m_nADCImgSize / 2 * Math.Sqrt(2) - dDisFromEdge) / m_aoiData.m_dR));
                            }
                            corpped = img.GetBitmapROI(cpCenter + new CPoint(m_nADCImgSize / 2, m_nADCImgSize / 2), cpCenter - new CPoint(m_nADCImgSize / 2, m_nADCImgSize / 2));
                            cp0 = cp1;
                            if (HW_BackSide_ATI_ADC.Instance.UseAdc == true)
                            {
                                ADC.Instance.Classify(HW_BackSide_ATI_ADC.Instance.Recipe, corpped, out strName, out dScore, m_log);
                                if (dScore > HW_BackSide_ATI_ADC.Instance.Score)
                                {
                                    for (int nADC = 0; nADC < aClassifiedDefect.Length; nADC++)
                                    {
                                        if (HW_BackSide_ATI_ADC.Instance.GetRoughbin(strName) == HW_BackSide_ATI_ADC.Instance.sDefectName[nADC])
                                        {
                                            aClassifiedDefect[nADC]++;
                                        }
                                    }
                                }
                            }
                            strBMP = m_strADCBmp + "\\" + m_auto.ClassRecipe().m_sRecipe + "\\" + infoWafer.m_strWaferID + "_" + infoWafer.m_strLotID + "_" + infoWafer.m_nSlot.ToString() + "\\" + nIndex.ToString() + "_" + strName;
                            corpped.Save(strBMP + cpCenter.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                        }
                    }
                    cpCenter = (cp1 + cp0) / 2;
                    dDisFromEdge = m_aoiData.m_dR - m_aoiData.m_cpCenter.GetL(cpCenter);
                    if (dDisFromEdge < m_nADCImgSize / 2 * Math.Sqrt(2))
                    {
                        x1 = cpCenter.x - m_aoiData.m_cpCenter.x;
                        y1 = cpCenter.y - m_aoiData.m_cpCenter.y;
                        cpCenter = cpCenter - new CPoint((int)(x1 * (m_nADCImgSize / 2 * Math.Sqrt(2) - dDisFromEdge) / m_aoiData.m_dR), (int)(y1 * (m_nADCImgSize / 2 * Math.Sqrt(2) - dDisFromEdge) / m_aoiData.m_dR));
                    }
                    corpped = img.GetBitmapROI(cpCenter + new CPoint(m_nADCImgSize / 2, m_nADCImgSize / 2), cpCenter - new CPoint(m_nADCImgSize / 2, m_nADCImgSize / 2));
                    //corpped = bsInfoWafer.m_imgInspect[nID, nRotateMode].GetBitmapROI(cpCenter + new CPoint(m_nADCImgSize / 2, m_nADCImgSize / 2), cpCenter - new CPoint(m_nADCImgSize / 2, m_nADCImgSize / 2));
                    if (HW_BackSide_ATI_ADC.Instance.UseAdc == true)
                    {
                        ADC.Instance.Classify(HW_BackSide_ATI_ADC.Instance.Recipe, corpped, out strName, out dScore, m_log);
                        if (dScore > HW_BackSide_ATI_ADC.Instance.Score)
                        {
                            for (int nADC = 0; nADC < aClassifiedDefect.Length; nADC++)
                            {
                                if (HW_BackSide_ATI_ADC.Instance.GetRoughbin(strName) == HW_BackSide_ATI_ADC.Instance.sDefectName[nADC])
                                {
                                    aClassifiedDefect[nADC]++;
                                }
                            }
                        }
                        int nMax = 0;
                        int nMaxIndex = -1;
                        for (int nADC = 0; nADC < aClassifiedDefect.Length; nADC++)
                        {
                            if (nMax < aClassifiedDefect[nADC])
                            {
                                nMax = aClassifiedDefect[nADC];
                                nMaxIndex = nADC;
                            }
                        }
                        if (nMaxIndex > -1)
                        {
                            island.m_strCode = HW_BackSide_ATI_ADC.Instance.GetRoughbin(HW_BackSide_ATI_ADC.Instance.sDefectName[nMaxIndex]);
                        }
                    }
                    strBMP = m_strADCBmp + "\\" + m_auto.ClassRecipe().m_sRecipe + "\\" + infoWafer.m_strWaferID + "_" + infoWafer.m_strLotID + "_" + infoWafer.m_nSlot.ToString() + "\\" + nIndex.ToString() + "_" + strName;
                    corpped.Save(strBMP + cpCenter.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                }
                nIndex++;
            }
        }

        bool IsInside(CPoint cp)
        {
            if (cp.GetL(m_aoiData.m_cpCenter) > m_aoiData.m_dR) return false;
            return true;
        }

        public void Draw(Graphics dc, ezImgView imgView)
        {
            int nID = Convert.ToInt32(imgView.m_id[imgView.m_id.Length - 1]) - '0';
            m_aoiEdge.DrawEdge(dc, imgView, m_bDraw);
            m_aoiEdge.DrawDie(dc, imgView, Color.Blue, m_bDraw);
            m_aoiRing[nID].DrawRing(dc, imgView, m_bDraw);
            for (int m = 0; m < 3; m++) m_aoiSurface[m, nID].Draw(dc, imgView, m_bDraw);
            for (int m = 0; m < 3; m++) m_aoiSobel[m, nID].Draw(dc, imgView, m_bDraw);
            m_aoiFFT[nID].DrawPhase(dc, imgView, m_bDraw);
            if (nID == 0) m_aoiChipping.DrawChipping(dc, imgView, m_bDraw); 
            if (nID == 1) m_aoiStain.DrawStain(dc, imgView, m_bDraw);
            if (m_bDraw) return;
            foreach (azBlob.Island island in m_aIsland)
            {
                island.DrawRect(dc, imgView, Color.Red, m_bDraw);
            }

        }

        private void HW_BackSide_ATI_AOI_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void buttonReadMap_Click(object sender, EventArgs e)
        {
            if (m_auto.ClassWork().IsRun()) return;
            m_aoiData.m_bReadMap = !m_aoiData.m_dieMap.ReadMapFile();
            RunGrid(0, eGrid.eUpdate, null);
            RunGrid(0, eGrid.eInit, null);
        }

        private void buttonInspView0_Click(object sender, EventArgs e)
        {
            if (m_auto.ClassWork().IsRun()) return;
            Info_Wafer infoWafer = new Info_Wafer(-1, -1, -1, m_log);
            infoWafer.m_strWaferID = System.DateTime.Now.TimeOfDay.Hours.ToString() + System.DateTime.Now.TimeOfDay.Minutes.ToString() + System.DateTime.Now.TimeOfDay.Seconds.ToString();
            Info_Carrier infoCarrier = new Info_Carrier(-1);
            ClearDefect();
            Inspect(infoWafer, infoCarrier, 0);
        }

        private void buttonInspView1_Click(object sender, EventArgs e)
        {
            if (m_auto.ClassWork().IsRun()) return;
            Info_Wafer infoWafer = new Info_Wafer(-1, -1, -1, m_log);
            infoWafer.m_strWaferID = System.DateTime.Now.TimeOfDay.Hours.ToString() + System.DateTime.Now.TimeOfDay.Minutes.ToString() + System.DateTime.Now.TimeOfDay.Seconds.ToString();
            Info_Carrier infoCarrier = new Info_Carrier(-1);
            ClearDefect();
            Inspect(infoWafer, infoCarrier, 1);
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            if (m_auto.ClassWork().IsRun()) return;
            ClearDefect();
            m_bsInfoWafer[0].Clear();
            m_bsInfoWafer[1].Clear();
        }

        private void buttonSaveNotch_Click(object sender, EventArgs e)
        {
            m_aoiChipping.SaveNotchArr(m_auto.ClassRecipe().m_strPath + "\\" + m_auto.ClassRecipe().m_sRecipe + ".notch");
        }
    }

}
