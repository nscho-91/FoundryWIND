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
using WeifenLuo.WinFormsUI.Docking;
using ezAutoMom; 
using ezTools;

namespace ezAutoMom
{
    public partial class ImageEdit : DockContent
    {
        bool m_bLBD, m_bFixSize, m_bGray;
	    CPoint m_cpLBD, m_cpLBU, m_cpMove, m_cpRT, m_cpLB, m_cpTextPos, m_cpOrg;
        CPoint[] m_cpLimit = new CPoint[2];
        CPoint[] m_cpROI = new CPoint[2];
	    int m_nByte, m_nID, m_nDrawSize, m_nGV, m_nBinaThres, m_nLength, m_nSize;
	    int m_nWindow, m_nRatio, m_nMor, m_nThres, m_nBlobMin, m_nSegment, m_nEffortSet, m_nShift;
        int[] m_nCheckGV = new int[2];
        int[] m_nPoGV = new int[2];
        int[] m_nOverlayPos = new int[2];
        int[] m_nGVBlob = new int[2];
        int[] m_nBand = new int[2];
        float m_gvMax = 0; 
        float[] m_gvCount = new float[256];
	    double m_dScale, m_dAmount, m_gvAve, m_nAngle; 
        double[] m_dmmPix = new double[2];
        string m_id, m_strTool, m_strDraw, m_strBina, m_strMouse, m_sShiftDir, m_sNoise, m_sAlign, m_sMor; 
        string m_sOCRRot = "0",  m_sRotate = "0", m_strDecode;
        string[] m_strMouseList = new string[2] {"Yes", "No"};
        string[] m_strDrawList = new string[3] {"Point", "Circle", "Rectangular"};
        string[] m_sRotateList = new string[4] {"0", "90", "180", "270"};
        string[] m_sAlignList = new string[2] {"None", "Center"};
        string[] m_strBinaList = new string[6] {"Manual", "Otsu", "Interactive", "Adaptive", "KMean", "Band"};
        string[] m_sShiftDirList = new string[4] {"ShiftUp", "ShiftRight", "ShiftDown", "ShiftLeft"};
        string[] m_sMorList = new string[6] {"Erosion", "Dilation", "Openning", "Closing", "ErosionGray", "DilationGray"};
        string[] m_sNoiseList = new string[2] {"Gaussian", "Salt&Pepper"};
        string[] m_sOCRRotList = new string[4] {"0", "90", "180", "270"};
	    CPoint m_szImg, m_szROI, m_szFixROI; //kns
        Rectangle m_rectText;

	    azBlob m_blob;
	    ezGrid m_grid;
	    Log m_log;	    
	    ezImgView m_imgView;
	    ezImg m_ImgMask, m_ImgWorking, m_ImgBackup;
	    Auto_Mom m_auto;
        OCRTool m_OCRTool = new OCRTool();
        Cognex_OCR m_cogOCR;
        Cognex_BCR m_cogBCR = new Cognex_BCR();
        Cognex_BCR2D m_cogBCR2D = new Cognex_BCR2D();
        //ezBarcode2D m_bcd2D;
	    //VerifyView m_verifyView;
	    //COCRBinary m_OCRBinary;
	    //CTeaching m_teach;
	    //CMachineLearning m_ML;
	    //SeparaOCR m_OCR;	    

        public ImageEdit()
        {
            InitializeComponent();
        }

        public void Init(string id, Auto_Mom auto)
        {
            m_id = id; m_auto = auto;
            m_log = new Log(m_id, m_auto.ClassLogView());
            m_grid = new ezGrid(m_id, grid, m_log, false);
            m_blob = new azBlob(id, m_log);
            m_ImgWorking = new ezImg("ImgWorking", m_log);
            m_ImgMask = new ezImg("ImgMask", m_log); 
            m_ImgBackup = new ezImg("ImgBackUp", m_log);
            m_strTool = "None"; m_bLBD = false; m_nID = -1; //m_verifyView = null;
            m_strDraw = "Point"; m_nGV = 100; m_nDrawSize = 5;
            m_nAngle = 0.0; m_nShift = 0; m_dScale = 0.9; m_nEffortSet = 1;
            m_nBinaThres = 100; m_strBina = "Manual"; m_strMouse = "No"; m_sShiftDir = "ShiftUp"; m_sNoise = "Gaussian";
            m_dAmount = 10.0; m_dmmPix[0] = m_dmmPix[1] = 0.01; m_nCheckGV[0] = 100; m_nCheckGV[1] = 255;
            m_nWindow = 8; m_nRatio = 10;
            m_cpLimit[0] = m_cpLimit[1] = new CPoint(0, 0); m_szImg = new CPoint(-1, -1);
            m_sAlign = "None";
            m_nOverlayPos[0] = m_nOverlayPos[1] = 200;
            m_szFixROI = new CPoint(100, 100); m_bGray = true; m_bFixSize = false; 
            //m_OCRBinary = null; m_ML = null; m_teach = null;
            m_sRotate = "0";
            m_sMor = "Erosion"; m_nMor = 1;
            m_nGVBlob[0] = 100; m_nGVBlob[1] = 255; m_nBlobMin = 500;
            m_cpLB = new CPoint(0, 0); m_cpRT = new CPoint(0, 0); m_nThres = 75;
            for (int n = 0; n < 256; n++) m_gvCount[n] = 0;
            comboAdvanced.SelectedIndex = 0;
            m_nBand[0] = 100; m_nBand[1] = 255; m_nSegment = 2;
            m_nPoGV[0] = 0; m_nPoGV[1] = 150;
            m_cogOCR = new Cognex_OCR(m_id, m_log);
            m_cogBCR.Init(m_id, m_log);
            m_cogBCR2D.Init(m_id, m_log);
            m_OCRTool.Init("CogOCR", m_log); m_OCRTool.TopLevel = false;
            this.Controls.Add(m_OCRTool); m_OCRTool.Location = new Point(300, 20);           
            //m_pVerifyView = ((Auto_Mom*)m_pAuto)->GetClassVerify();
            //m_1DBarcode.Init(); 
            //CreateDirectory("C:\\Temp", null);
            RunGrid(eGrid.eRegRead); RunGrid(eGrid.eInit);
        }

        public void RunGrid(eGrid eMode, ezJob job = null)
        {
            m_grid.Update(eMode, job);
            m_grid.Set(ref m_strMouse, m_strMouseList, "Drawing", "Disp", "Display and Save image"); 
            m_grid.Set(ref m_strDraw, m_strDrawList, "Drawing", "Shape", "Size (Pixel)"); 
            m_grid.Set(ref m_nDrawSize, "Drawing", "Size", "Size (Pixel)");
            m_grid.Set(ref m_nGV, "Drawing", "Intensity", "GV (0~255)");
            m_grid.Set(ref m_sRotate, m_sRotateList, "SaveROI", "Rotate", "Rotate Angle"); 
            m_grid.Set(ref m_bGray, "SaveROI", "GrayType", "Yes: Gray; No: Binary");
            m_grid.Set(ref m_sAlign, m_sAlignList, "SaveROI", "Align", "Align Image"); 
            m_grid.Set(ref m_bFixSize, "SaveROI", "FixSize", "Fix ROI Size");
            m_grid.Set(ref m_nOverlayPos[0], "Overlay", "PosX", "Overlay position X");
            m_grid.Set(ref m_nOverlayPos[1], "Overlay", "PosY", "Overlay position Y");
            m_grid.Set(ref m_dmmPix[0], "CheckGV", "mmPix1", "Real pixel size view 1");
            m_grid.Set(ref m_dmmPix[1], "CheckGV", "mmPix2", "Real pixel size view 2");
            m_grid.Set(ref m_nCheckGV[0], "CheckGV", "GVMin", "GV (0~255)");
            m_grid.Set(ref m_nCheckGV[1], "CheckGV", "GVMax", "GV (0~255)");
            m_grid.Set(ref m_strBina, m_strBinaList, "Binarization","Method", "Binarization method"); 
            m_grid.Set(ref m_nBinaThres, "Binarization","Threshold", "GV (0~255)");
            m_grid.Set(ref m_nWindow, "Binarization","Adapt_Window", "Window Size");
            m_grid.Set(ref m_nRatio, "Binarization","Adapt_Ratio", "Ratio");
            m_grid.Set(ref m_nBand[0], "Binarization","Band_Min", "Minimum Band");
            m_grid.Set(ref m_nBand[1], "Binarization","Band_Max", "Maximum Band");
            m_grid.Set(ref m_nSegment, "Binarization","KMean_N", "Number of regions");
            m_grid.Set(ref m_nAngle, "Rotation", "Angle", "digree 0~360");
            m_grid.Set(ref m_nShift, "Shifting", "NPixels", "Shifting N pixels");
            m_grid.Set(ref m_sShiftDir, m_sShiftDirList, "Shifting", "Dir", "Shifting direction"); 
            m_grid.Set(ref m_dScale, "Resizing", "Scale", "Resizing scale");
            m_grid.Set(ref m_sMor, m_sMorList, "Morphology", "Method", "Mophology Method"); 
            m_grid.Set(ref m_nMor, "Morphology", "NTimes", "Number of Operations");
            m_grid.Set(ref m_sNoise, m_sNoiseList, "AddNoise", "Type", "Noise type"); 
            m_grid.Set(ref m_dAmount, "AddNoise", "Amount", "Amount percent");
            m_grid.Set(ref m_nGVBlob[0], "CountBlob", "GVMin", "GV Min");
            m_grid.Set(ref m_nGVBlob[1], "CountBlob", "GVMax", "GV Max");
            m_grid.Set(ref m_nBlobMin, "CountBlob", "MinSize", "Min Size");
            m_grid.Set(ref m_cpRT.x, "CountUnit", "xRightTop", "x value of right-top point of ROI");
            m_grid.Set(ref m_cpRT.y, "CountUnit", "yRightTop", "y value of right-top point of ROI");
            m_grid.Set(ref m_cpLB.x, "CountUnit", "xLeftBottom", "x value of left-bottom point of ROI");
            m_grid.Set(ref m_cpLB.y, "CountUnit", "yLeftBottom", "y value of left-bottom point of ROI");
            m_grid.Set(ref m_nThres, "CountUnit", "Thres", "Threshold of NCC (0-100)");
            m_grid.Set(ref m_nEffortSet, "MatrixCode", "Effort", "Effort : 0~2");
            m_grid.Set(ref m_nPoGV[0], "Polution_Length", "LowGV", "Low GV");
            m_grid.Set(ref m_nPoGV[1], "Polution_Length", "HiGV", "High GV");
            m_cogOCR.RunGridTeach("OCR", m_grid, eMode, job);
            m_cogBCR2D.RunGridTeach("DataMatrix", m_grid, eMode, job);
            m_grid.Refresh();
        }
              
        public bool CheckLBD(int nID, CPoint cpImg, bool bCBD)
        {
            m_imgView = m_auto.ClassView(nID).ClassImageView();
            if (m_imgView == null) return true;
            m_nByte = m_imgView.m_pImg.m_nByte;
	        m_cpLBD = m_cpOrg = cpImg; m_bLBD = true; m_nID = nID;
	        m_szImg = m_imgView.m_pImg.m_szImg;
	        return false;
        }

        public void InitOCRTool()
        {
	        /*if (m_teach.GetSafeHwnd() == null)
	        {
		        m_teach = new CTeaching(this);
		        m_teach.Create(CTeaching::IDD, this);
		        m_teach.SetWindowPos(null, 240, 5, 1000, 1100, SWP_FRAMECHANGED | SWP_NOZORDER);
		        m_teach.Init();
		        m_teach.ShowWindow(SW_HIDE);
	        }
	        if (m_OCRTool.GetSafeHwnd() == null)
	        {
		        m_OCRTool = new COCRTool(this);
		        m_OCRTool.Create(COCRTool::IDD, this);
		        m_OCRTool.SetWindowPos(null, 240, 5, 580, 1100, SWP_FRAMECHANGED | SWP_NOZORDER);
		        m_OCRTool.Init();
		        m_OCRTool.ShowWindow(SW_HIDE);
	        }
	        if (m_OCRBinaryGetSafeHwnd() == null)
	        {
		        m_OCRBinary = new COCRBinary(this);
		        m_OCRBinary.Create(COCRBinary::IDD, this);
		        m_OCRBinary.SetWindowPos(null, 240, 5, 580, 1100, SWP_FRAMECHANGED | SWP_NOZORDER);
		        m_OCRBinary.Init();
		        m_OCRBinary.ShowWindow(SW_HIDE);
	        }
	        if (m_ML.GetSafeHwnd() == null)
	        {
		        m_pML = new CMachineLearning(this);
		        m_ML.Create(CMachineLearning::IDD, this);
		        m_ML.SetWindowPos(null, 240, 5, 580, 1100, SWP_FRAMECHANGED | SWP_NOZORDER);
		        m_ML.Init();
		        m_ML.ShowWindow(SW_HIDE);
	        } */
        }

        public bool IsEdit(int nID)
        {
	        if (m_strTool == "Drawing" && m_strMouse == "Yes") return true;
	        if (!m_bLBD) return false;
	        if (nID != m_nID) return false;
	        return true;
        }

        public bool CheckMove(int nID, CPoint cpImg, bool bFinish)
        {
	        m_cpLBU = m_cpMove = cpImg;
            if(m_cpLBU.x - m_cpOrg.x < 0)
            {
                m_cpLBU.x = m_cpOrg.x;
                m_cpLBD.x = cpImg.x;
            }
            else
            {
                m_cpLBD.x = m_cpOrg.x;
            }
            if(m_cpOrg.y - m_cpLBU.y < 0)
            {
                m_cpLBU.y = m_cpOrg.y;
                m_cpLBD.y = cpImg.y;
            }
            else
            {
                m_cpLBD.y = m_cpOrg.y;
            }
            if (!IsEdit(nID)) return false;
	        if (m_strTool == "Drawing" && m_bLBD)
	        {
		        if (m_strDraw == "Point")        PaintPoint(cpImg);
		        if (m_strDraw == "Circle")       PaintCircle(cpImg);
		        if (m_strDraw == "Rectangular")  PaintRectangle(cpImg);
	        }
	        if (bFinish) m_bLBD = false;
	        if (m_strTool == "CheckGV" && bFinish) CheckGV();
	        return false;
        }

        public void Draw(Graphics dc, int nID, ezImgView imgView)
        {
	        CPoint[] cp = new CPoint[2]; 
            Pen pen = new Pen(Color.FromArgb(0, 0, 0), 5); 
	        if (imgView == null) return;
	        if (m_strTool == "Drawing" && m_strMouse == "Yes")
	        {
                cp[0] = imgView.GetWinPos(m_cpMove - new CPoint(m_nDrawSize, m_nDrawSize));
		        cp[1] = imgView.GetWinPos(m_cpMove + new CPoint(m_nDrawSize, m_nDrawSize));
		        if (m_strDraw == "Circle"){ dc.DrawEllipse(pen, cp[0].x, cp[0].y, cp[1].x, cp[1].y); }
		        if (m_strDraw == "Rectangular")
                {
                    dc.DrawRectangle(pen, cp[0].x, cp[0].y, Math.Abs(cp[1].x - cp[0].x), Math.Abs(cp[1].y - cp[0].y));			       
		        }
	        }
            if (m_cpLBD.x == m_cpLBU.x || m_cpLBD.y == m_cpLBU.y) return;
	        if (m_strTool != "Drawing"&& m_strTool != "None")
	        {
                pen = new Pen(Color.FromArgb(0, 255, 0), 1); 
                //color = Color.FromArgb(0, 255, 0); pen.CreatePen(PS_SOLID, 1, color); pPen = dc.SelectObject(pen);
		        cp[0] = imgView.GetWinPos(m_cpLBD); cp[1] = imgView.GetWinPos(m_cpLBU);
                dc.DrawRectangle(pen, cp[0].x, cp[0].y, Math.Abs(cp[1].x - cp[0].x), Math.Abs(cp[1].y - cp[0].y));		
		        m_cpROI[0] = m_cpLBD; m_cpROI[1] = m_cpLBU;

		        //Register ROI
		        m_szROI.x = Math.Abs(m_cpROI[1].x - m_cpROI[0].x); m_szROI.y = Math.Abs(m_cpROI[1].y - m_cpROI[0].y);
		        m_szROI.x = m_szROI.x - (m_szROI.x % 4); m_szROI.y = m_szROI.y - (m_szROI.y % 4);
		        if (m_cpROI[0].x < m_cpROI[1].x)  m_cpLimit[0].x = m_cpROI[0].x; else m_cpLimit[0].x = m_cpROI[1].x;
		        if (m_cpROI[0].y < m_cpROI[1].y)  m_cpLimit[0].y = m_cpROI[0].y; else m_cpLimit[0].y = m_cpROI[1].y;
		        m_cpLimit[1].x = m_cpLimit[0].x + m_szROI.x; m_cpLimit[1].y = m_cpLimit[0].y + m_szROI.y;
		        if (!m_bFixSize) m_szFixROI = m_szROI;
	        }
	        if (m_strTool == "CheckGV") DrawHist(dc, nID, imgView);
            //m_auto.Invalidate(m_nID); //kns160905
        }

        public void DrawHist(Graphics dc, int nID, ezImgView imgView)
        {
	        int n, x0, y0, y1; double lx = 0, ly = 0; string str; 
	        CPoint cpLBD, cpLBU;
            Pen pen = new Pen(Color.FromArgb(255, 0, 0), 1);
	        cpLBD = imgView.GetWinPos(m_cpLBD); cpLBU = imgView.GetWinPos(m_cpLBU);
            Font fontStr = new Font("Arial", 15, FontStyle.Bold);
            str = m_gvAve.ToString("GV=0.0"); dc.DrawString(str, fontStr, Brushes.Yellow, cpLBD.x + 5, cpLBD.y - 60);
            str = m_nLength.ToString("L=0"); dc.DrawString(str, fontStr, Brushes.Yellow, cpLBD.x + 5, cpLBD.y - 40);
            str = m_nSize.ToString("S=0"); dc.DrawString(str, fontStr, Brushes.Yellow, cpLBD.x + 5, cpLBD.y - 20);
	        if (nID == 0)
	        {
		        lx = m_cpLBU.x - m_cpLBD.x; if (lx<0) lx = -lx; lx *= m_dmmPix[0];
		        ly = m_cpLBU.y - m_cpLBD.y; if (ly<0) ly = -ly; ly *= m_dmmPix[0];
	        }
	        else if (nID == 1)
	        {
		        lx = m_cpLBU.x - m_cpLBD.x; if (lx<0) lx = -lx; lx *= m_dmmPix[1];
		        ly = m_cpLBU.y - m_cpLBD.y; if (ly<0) ly = -ly; ly *= m_dmmPix[1];
	        }
	        str= "(" + lx.ToString("0.00 ") + ly.ToString("0.00") + ")" + Math.Sqrt(lx*lx + ly*ly).ToString(" 0.00") + " mm";
            dc.DrawString(str, fontStr, Brushes.Yellow, cpLBD.x + 5, cpLBU.y + 2);
            x0 = 5; y0 = 5; y1 = y0 + 100;
            dc.DrawLine(pen, new PointF(x0, (y0 - 1)), new PointF((x0 + 256), (y0 - 1)));
            dc.DrawLine(pen, new PointF(x0, (y1 + 1)), new PointF((x0 + 256), (y1 + 1)));
            for (n = 1; n < 256; n++) dc.DrawLine(pen, new PointF((x0+n-1), (y1 - m_gvCount[n-1])), new PointF((x0 + n), (y1 - m_gvCount[n])));
            pen.Dispose(); 
        }

        public void PaintPoint(CPoint cpImg)
        {
            unsafe
            {
                byte* pDst; ezImg pImg;
                pImg = m_imgView.m_pImg;
                pDst = (byte*)m_imgView.m_pImg.GetIntPtr(cpImg.y, cpImg.x); *pDst = (byte)m_nGV;
            }
        }

        public void PaintRectangle(CPoint cpImg)
        {
	        int x, y; ezImg img;
            img = m_imgView.m_pImg;
	        for (y = cpImg.y - m_nDrawSize; y <= cpImg.y + m_nDrawSize; y++)
	        {
                unsafe
                {
                    byte *pDst;
                    pDst = (byte*)img.GetIntPtr(y, cpImg.x - m_nDrawSize);
                    for (x = -m_nDrawSize; x <= m_nDrawSize; x++, pDst++) *pDst = (byte)m_nGV; 
                }
	        } 
        }

        public void PaintCircle(CPoint cpImg)
        {
	        int x, y; ezImg img; double dR;
	        img = m_imgView.m_pImg;
	        for (y = cpImg.y - m_nDrawSize; y <= cpImg.y + m_nDrawSize; y++)
	        {
                if (y < 0) y = 0; if (y > img.m_szImg.y) return;
                unsafe
                {
                    byte* pDst;
                    pDst = (byte*)img.GetIntPtr(y, cpImg.x - m_nDrawSize);
                    for (x = -m_nDrawSize; x <= m_nDrawSize; x++, pDst++)
                    {
                        dR = (y - cpImg.y) * (y - cpImg.y) + x * x; dR = Math.Sqrt(dR);
                        if (dR <= m_nDrawSize) *pDst = (byte)m_nGV;
                    }
                }
	        }
        }

        public void ExtractImage(CPoint cp, CPoint szImg)
        {
            int w, y; ezImg img;
            w = szImg.x; img = m_imgView.m_pImg;
	        if (cp.x<0 || cp.y<0 || (cp.x + szImg.x) > img.m_szImg.x || (cp.y + szImg.y)>img.m_szImg.y) return;
            m_ImgMask.ReAllocate(szImg, m_nByte); m_ImgWorking.ReAllocate(szImg, m_nByte); m_ImgBackup.ReAllocate(szImg, m_nByte);
            unsafe
            {
                for (y = cp.y; y < cp.y + szImg.y; y++) 
                {

                    byte* pSrc, pMask, pData, pBackup;
                    pSrc = (byte*)img.GetIntPtr(y, cp.x); pMask = (byte*)m_ImgMask.GetIntPtr(y - cp.y, 0);
                    pData = (byte*)m_ImgWorking.GetIntPtr(y - cp.y, 0); pBackup = (byte*)m_ImgBackup.GetIntPtr(y - cp.y, 0);
                    cpp_memcpy(pData, pSrc, w); cpp_memcpy(pBackup, pSrc, w);
                    /*for (n = 0; n < w; n++)
                    {
                        *pMask = (byte)0; // memset(pMask, 0, w);
                        *pData = *pSrc; // memcpy(pData, pSrc, w);
                        *pBackup = *pSrc; // memcpy(pBackup, pSrc, w); // ing
                        pMask++; pData++; pSrc++; pBackup++;
                    }*/
                }
            }
        }

        public unsafe void AlignCenterImage(CPoint cp, CPoint szImg)
        {
            int w, y, n; byte* pSrc, pData; ezImg img; 
            w = szImg.x; img = m_imgView.m_pImg;
	        if (cp.x<0 || cp.y<0 || (cp.x + szImg.x)>img.m_szImg.x || (cp.y + szImg.y)>img.m_szImg.y) return;
            m_ImgMask.ReAllocate(szImg, m_nByte); m_ImgWorking.ReAllocate(szImg, m_nByte);
	        for (y = cp.y; y < cp.y + szImg.y; y++)
	        {
                pSrc = (byte*)img.GetIntPtr(y, cp.x); pData = (byte*)m_ImgWorking.GetIntPtr(y - cp.y, 0);
                cpp_memcpy(pData, pSrc, w);
                /*for (int i = 0; i < w; i++)//memcpy(pData, pSrc, w); // ing
                {
                    *pData = *pSrc;
                    pData++; pSrc++;
                }*/
	        }
	        if (m_sAlign == "Center")
	        {
		        m_blob.DoBlob(eBlob.eArea, m_ImgWorking, new CPoint(0, 0), szImg, m_nBinaThres, 256);
		        n = m_blob.GetMaxIndex(eBlob.eArea); if (n<1) return;
		        m_rectText = m_blob.GetBound(n);
		        m_cpTextPos.x = (m_rectText.Left + m_rectText.Right) / 2; m_cpTextPos.y = (m_rectText.Top + m_rectText.Bottom) / 2;
		        m_cpTextPos.x = cp.x + m_cpTextPos.x - szImg.x / 2; m_cpTextPos.y = cp.y + m_cpTextPos.y - szImg.y / 2;
                m_ImgWorking.ReAllocate(szImg, m_nByte);
		        for (y = m_cpTextPos.y; y<m_cpTextPos.y + szImg.y; y++)
		        {
                    pSrc = (byte*)img.GetIntPtr(y, m_cpTextPos.x); pData = (byte*)m_ImgWorking.GetIntPtr(y - m_cpTextPos.y, 0);
                    cpp_memcpy(pData, pSrc, szImg.x);
                    /*for (int i = 0; i < szImg.x; i++)//memcpy(pData, pSrc, szImg.x); // ing
                    {
                        *pData = *pSrc;
                        pData++; pSrc++;
                    }*/
		        }
		        if (!m_bGray) m_ImgWorking.Binarization(new CPoint(0, 0), szImg, m_nBinaThres);
	        }
	        m_ImgWorking.RotateImage(m_ImgWorking, m_ImgMask, m_sOCRRot); 
        }

        public void CheckGV()
        {
	        int n, nCount, x, y, x0, x1, y0, y1; ezImg pImg;  
	        nCount = 0; m_gvAve = 0; m_gvMax = 0; for (n = 0; n<256; n++) m_gvCount[n] = 0;
	        pImg = m_imgView.m_pImg;
	        x0 = m_cpLBD.x; y0 = m_cpLBD.y; x1 = m_cpLBU.x; y1 = m_cpLBU.y;
	        if (x0>x1) { x = x0; x0 = x1; x1 = x; } if (y0>y1) { y = y0; y0 = y1; y1 = y; }
	        if (x0<0) x0 = 0; if (x1 >= m_szImg.x) x1 = m_szImg.x - 1;
	        if (y0<0) y0 = 0; if (y1 >= m_szImg.y) y1 = m_szImg.y - 1;
            unsafe
            {
                byte* pBuf;
                for (y = y0; y <= y1; y++)
                {
                    pBuf = (byte*)pImg.GetIntPtr(y, x0);
                    for (x = x0; x <= x1; x++) { m_gvAve += *pBuf; m_gvCount[*pBuf]+=1; pBuf++; nCount++; }
                }
            }
            
	        for (n = 0; n<256; n++) if (m_gvCount[n]>m_gvMax) m_gvMax = m_gvCount[n];
	        if (m_gvMax>0) for (n = 0; n<256; n++) m_gvCount[n] = (100 * m_gvCount[n] / m_gvMax);
	        if (nCount > 0) m_gvAve /= nCount;
            if ((m_cpLBU.x - m_cpLBD.x) <= 0 || (m_cpLBD.y - m_cpLBU.y) <= 0) return;
            m_blob.DoBlob(eBlob.eL, pImg, new CPoint(m_cpLBD.x, m_cpLBU.y), new CPoint(m_cpLBU.x - m_cpLBD.x, m_cpLBD.y - m_cpLBU.y), m_nCheckGV[0], m_nCheckGV[1]);
            m_nLength = m_blob.GetSize(eBlob.eL, m_blob.GetMaxIndex(eBlob.eL));
            m_nSize = m_blob.GetSize(eBlob.eArea, m_blob.GetMaxIndex(eBlob.eArea));
	        Invalidate(); 
        }

        public void OverlayImage(ezImg imgOverlay, CPoint cp)
        {
	        int w, x, y; ezImg img; 
            CPoint szImg = imgOverlay.m_szImg; w = szImg.x;
	        img = m_imgView.m_pImg;
	        if (cp.x<0 || cp.y<0 || (cp.x + szImg.x)>img.m_szImg.x || (cp.y + szImg.y)>img.m_szImg.y) return;
            unsafe
            {
                byte* pSrc; byte* pOverlay;
                for (y = cp.y; y < cp.y + szImg.y; y++)
                {
                    pSrc = (byte*)img.GetIntPtr(y, cp.x);
                    pOverlay = (byte*)imgOverlay.GetIntPtr(y - cp.y, 0);
                    for (x = cp.x; x < cp.x + szImg.x; x++, pSrc++, pOverlay++) *pSrc = *pOverlay;
                }
            }
            m_auto.Invalidate(m_nID);
        }
        
        string Inspect2D()
        {
            /*int nStep, nTry, nRand; CPoint cp0, cpRand; CPoint sz2D; string str, strDecode;
            srand((unsigned)time(NULL));
            m_bcd2D.m_MatrixCode.m_2DMatPos.Clear(); m_bcd2D.m_nEffort = m_nEffortSet;
            strDecode = m_bcd2D.InspectAndVerify(&m_ImgWorking, new CPoint(0, 0), m_szROI);
            if ((strDecode != "Unknown") && (strDecode != ""))
            {
                str = "Inspect 2D Step 0, Try 0 : "; m_log.Add(str + strDecode);
                return strDecode;
            }
            else
            {
                sz2D = new CPoint(2 * m_szROI.y, 2 * m_szROI.y); nRand = m_szROI.y / 4; cp0 = sz2D;
                ExtractImage(m_cpLimit[0] - sz2D, m_szROI + sz2D + sz2D);
                for (nStep = 1; nStep <= 3; nStep++)
                {
                    sz2D = new CPoint(2 * nStep * m_szROI.y / 3, 2 * nStep * m_szROI.y / 3);
                    for (nTry = 0; nTry < 2; nTry++)
                    {
                        cpRand.x = (long)(nRand * ((2.0 * rand() / RAND_MAX) - 1));
                        cpRand.y = (long)(nRand * ((2.0 * rand() / RAND_MAX) - 1));
                        //m_bcd2D.m_MatrixCode.m_2DMatPos.Clear(); m_bcd2D.m_nEffort = 2;
                        //strDecode = m_bcd2D.InspectAndVerify(m_ImgWorking, cp0 - sz2D + cpRand, m_szROI + sz2D + sz2D);
                        if ((strDecode != "Unknown") && (strDecode != ""))
                        {
                            str = nStep.ToString("Inspect 2D Step 00") + nTry.ToString("Try 00 : "); m_log.Add(str + strDecode);
                            return strDecode;
                        }
                    }
                }
            } */
            m_log.Add("Inspect 2D Fail !!"); return "Unknown"; 
        }

        public void OnBnClickedImageEditRedo()
        {
	        if (!CheckInput()) return;
	        OverlayImage(m_ImgBackup, m_cpLimit[0]);
	        m_auto.Invalidate(m_nID);
        }

        public bool CheckInput()
        {
	        if (m_szROI.x < 0 || m_szROI.y < 0) return false;
	        if (m_szImg.x < 0 || m_szImg.y < 0) return false;
	        if (m_nByte>1) return false;
	        if (m_cpLimit[0].x < 0 || m_cpLimit[0].x + m_szROI.x >= m_szImg.x || m_cpLimit[0].y < 0 || m_cpLimit[0].y + m_szROI.y >= m_szImg.y) return false;
	        return true;
        }

        public void SaveROI()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "BMP Files (*.bmp)|*.bmp|JPG Files (*.jpg)|*.jpg";
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
	        if (m_bFixSize) m_szROI = m_szFixROI;
	        AlignCenterImage(m_cpLimit[0], m_szROI);
	        m_ImgMask.RotateImage(m_ImgMask, m_ImgWorking, m_sRotate);
	        m_ImgWorking.FileSave(dlg.FileName);
	        //UpdateData(FALSE);
	        double d = m_ImgWorking.GetBlackPercent(new CPoint(0, 0), m_ImgWorking.m_szImg, 30);
        }

        public void OverlayMultipleImage()   //Modify cuong 2411
        {
	        /*char[] aChar = new char[1000000]; int nBmpFiles = 0; Size sz;
	        Array.Clear(aChar, 0, 1000000);
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "BMP Files (*.bmp)|*.bmp|JPG Files (*.jpg)|*.jpg";
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

	        CFileDialog BMPInputDialog(TRUE, NULL, NULL, OFN_HIDEREADONLY | OFN_OVERWRITEPROMPT, "BITMAP FILE (*.BMP)|*.BMP||");
	        BMPInputDialog.m_ofn.Flags |= OFN_ALLOWMULTISELECT;
	        BMPInputDialog.m_ofn.nMaxFile = 1000000; BMPInputDialog.m_ofn.lpstrFile = pChar;
	        if (BMPInputDialog.DoModal() != IDOK) { delete[] pChar; return; }
	        POSITION pos;
	        pos = BMPInputDialog.GetStartPosition();
	        if (pos == NULL)
	        {
		        ::MessageBox.Show(null, "CANNOT LOAD BMP FILES..", "ERROR", MB_ICONERROR);
		        delete[] pChar;
		        return;
	        }
	        int nSizeX = 0; int nSizeY = 0;	int nStart = 0;
	        string	BmpFileName[50];
	        do
	        {
		        BmpFileName[nBmpFiles] = BMPInputDialog.GetNextPathName(pos);
		        m_ImgWorking.FileOpen(BmpFileName[nBmpFiles]);
		        nSizeX += m_ImgWorking.m_szImg.cx;
		        if (m_ImgWorking.m_szImg.cy > nSizeY)	nSizeY = m_ImgWorking.m_szImg.cy;
		        nBmpFiles++;
	        } while (pos != NULL);
	        if (nBmpFiles == 0) return;

	        m_ImgMask.ReAllocate(CSize(nSizeX, nSizeY), 1); m_ImgMask.ClearImage(0);

	        for (int i = 0; i<nBmpFiles; i++)
	        {
		        m_ImgWorking.FileOpen(BmpFileName[i]);
		        m_ImgMask.SetROI(&m_ImgWorking, CPoint(nStart, 0));
		        nStart += m_ImgWorking.m_szImg.cx;
	        }
	        CFileDialog dlg(FALSE, "bmp", NULL, OFN_EXPLORER | OFN_OVERWRITEPROMPT, "Image File (*.bmp,*jpg)|*.bmp;*.jpg|");
	        if (dlg.DoModal() != IDOK) return;
	        m_ImgMask.FileSave(dlg.GetPathName());
	        OverlayImage(&m_ImgMask, CPoint(m_nOverlayPos[0], m_nOverlayPos[1]));
	        delete pChar; */
        }

        public double CaclAngle(CPoint cp1, CPoint cp2)
        {
	        double dAngle = 0;
	        double PI = 3.1415926535897932384626433832795;
	        if (cp1.x == cp2.x || cp1.y == cp2.y) dAngle = 0; //PI / 2.0;
	        else dAngle = Math.Atan2(Math.Abs(cp2.x - cp1.x), Math.Abs(cp2.y - cp1.y)) * 180 / PI;
	        if (dAngle < 0) dAngle = dAngle + 2 * PI;
	        return dAngle;
        }

    /*  public void MatchingOpenCV(Mat &src, Mat temp, int &numOfMatch)
        {
	        Point minLoc, maxLoc, matchLoc;
	        double minVal, maxVal; Mat result;

	        // Create the result matrix
	        int result_cols = src.cols - temp.cols + 1;	int result_rows = src.rows - temp.rows + 1;
	        result.create(result_cols, result_rows, CV_32FC1);

	        /// Do the Matching and Normalize
	        matchTemplate(src, temp, result, 3);
	        normalize(result, result, 0, 1, NORM_MINMAX, -1, Mat());
	        minMaxLoc(result, &minVal, &maxVal, &minLoc, &maxLoc, Mat());
	        while (maxVal > (double)m_nThres / (double)100)
	        {
		        ellipse(result, maxLoc, Size(temp.cols * 9 / 10, temp.rows * 9 / 10), 0, 0, 360, Scalar::all(0), CV_FILLED);
		        numOfMatch += 1;
		        rectangle(src, maxLoc, Point(maxLoc.x + temp.cols, maxLoc.y + temp.rows), Scalar(0, 255, 255), 2);
		        minMaxLoc(result, &minVal, &maxVal, &minLoc, &maxLoc, Mat());
	        }
        } */

        public void CountUnit()
        {
	        /* Mat matHor, matVer, templ;    //add cuong 1209 for counting
	        ezImg *srcImg; srcImg = m_pImgView->GetImg(0);
	        if (m_cpRT.x > srcImg->m_szImg.cx) { AfxMessageBox("Set Right-Top point!"); return; }
	        if (m_cpRT.x == 0 || m_cpLB.y == 0) { AfxMessageBox("rSet Left-Bottom point & Right-Top point !"); return; }
	        AlignCenterImage(m_cpLimit[0], m_szROI);
	        m_ImgMask.FileSave("C:\\Temp\\ROI.jpg");
	        CPoint cpHor, cpVer; CSize szHor, szVer;
	        int dMove = m_szROI.cy / 10;
	        cpHor.x = m_cpLimit[0].x - m_szROI.cx / 2;
	        if (m_cpLimit[0].y < m_cpRT.y - m_szROI.cy)	cpHor.y = m_cpLimit[0].y - dMove;
	        else  cpHor.y = m_cpRT.y - m_szROI.cy - dMove;
	        szHor = CSize(m_cpRT.x - cpHor.x + m_szROI.cx / 2, m_szROI.cy + 2 * dMove + abs(m_cpLimit[0].y - m_cpRT.y + m_szROI.cy));
	        szHor.cx = szHor.cx - szHor.cx % 4; szHor.cy = szHor.cy - szHor.cy % 4;
	        dMove = m_szROI.cx / 7;

	        cpVer.y = m_cpLB.y - m_szROI.cy / 2;
	        if (m_cpLimit[0].x < m_cpLB.x) cpVer.x = m_cpLimit[0].x - dMove;
	        else  cpVer.x = m_cpLB.x - dMove;
	        szVer = CSize(m_szROI.cx + 2 * dMove + abs(m_cpLimit[0].x - m_cpLB.x), m_cpLimit[0].y + m_szROI.cy - cpVer.y + m_szROI.cy / 2);
	        szVer.cx = szVer.cx - szVer.cx % 4; szVer.cy = szVer.cy - szVer.cy % 4;

	        ezImg imgHor; imgHor.Init(""); imgHor.ReAllocate(szHor, 1);
	        imgHor.Copy(*srcImg, cpHor, CPoint(0, 0), imgHor.m_szImg);
	        imgHor.FileSave("C:\\Temp\\HorROI.jpg"); imgHor.Close();

	        ezImg imgVer; imgVer.Init(""); imgVer.ReAllocate(szVer, 1);
	        imgVer.Copy(*srcImg, cpVer, CPoint(0, 0), imgVer.m_szImg);
	        imgVer.FileSave("C:\\Temp\\VerROI.jpg"); imgVer.Close();

	        if (!PathFileExists("C:\\Temp\\HorROI.jpg")) return; matHor = imread("C:\\Temp\\HorROI.jpg", 1); 
	        if (!PathFileExists("C:\\Temp\\VerROI.jpg")) return; matVer = imread("C:\\Temp\\VerROI.jpg", 1);
	        if (!PathFileExists("C:\\Temp\\ROI.jpg")) return; templ = imread("C:\\Temp\\ROI.jpg", 1);

	        int nHorMatch = 0, nVerMatch = 0;

	        MatchingOpenCV(matHor, templ, nHorMatch);
	        MatchingOpenCV(matVer, templ, nVerMatch);

	        imwrite("C:\\Temp\\HozResult.jpg", matHor); 
	        imwrite("C:\\Temp\\VerResult.jpg", matVer);

	        m_strDecode.Format("Horizontal Block: %d \n Vertical Block: %d", nHorMatch, nVerMatch); UpdateData(FALSE); */
        }


        private void comboEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_strTool = comboEdit.Text;
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            if (!CheckInput()) return;
            Cursor.Current = Cursors.WaitCursor;
            ExtractImage(m_cpLimit[0], m_szROI);
            if (m_strTool == "SaveROI") SaveROI();
            if (m_strTool == "Overlay") OverlayMultipleImage();
            if (m_strTool == "Measurement") 
            {
                m_strDecode = CaclAngle(m_cpLBD, m_cpLBU).ToString();
            }
            if (m_strTool == "CountBlob")
            {
                int nBlob, n, count = 0; CPoint szBlob;
                m_blob.DoBlob(eBlob.eArea, m_ImgWorking, new CPoint(0, 0), m_ImgWorking.m_szImg, m_nGVBlob[0], m_nGVBlob[1]);
                nBlob = m_blob.GetNumLand();
                for (n = 1; n <= nBlob; n++)
                {
                    szBlob.x = m_blob.GetIsland(n).GetSize(eBlob.eX); szBlob.y = m_blob.GetIsland(n).GetSize(eBlob.eY);
                    if (szBlob.x * szBlob.y > m_nBlobMin) count++;
                }
                m_strDecode = count.ToString("Number of blobs: 00"); 
            }
            if (m_strTool == "CountUnit") CountUnit(); 

            if (m_strTool == "Binarization")
            {
                if (m_strBina == "Manual") m_ImgWorking.Binarization(new CPoint(0, 0), m_szROI, m_nBinaThres);
                if (m_strBina == "Otsu") m_ImgWorking.OtsuThreshold(new CPoint(0, 0), m_szROI);
                if (m_strBina == "Interactive") m_ImgWorking.InteractiveThreshold(new CPoint(0, 0), m_szROI);
                if (m_strBina == "Adaptive") m_ImgWorking.AdaptiveThrehold(new CPoint(0, 0), m_szROI, m_nWindow, m_nRatio);
                //if (m_strBina == "Band") m_ImgWorking.BandThreshold(new CPoint(0, 0), m_szROI, m_nBand[0], m_nBand[1]); // ing
                //if (m_strBina == "KMean") m_ImgWorking.KMeanThreshold(new CPoint(0, 0), m_szROI, m_nSegment);
                OverlayImage(m_ImgWorking, m_cpLimit[0]);
            } 
            if (m_strTool == "Rotation")
            {
                m_ImgWorking.RotateBilinear(m_nAngle);
                OverlayImage(m_ImgWorking, m_cpLimit[0]);
            }
            if (m_strTool == "Shifting")
            {
                m_ImgWorking.Shifting(m_ImgWorking, m_ImgMask, m_nShift, m_sShiftDir);
                OverlayImage(m_ImgMask, m_cpLimit[0]);
            }
            if (m_strTool == "Mophology")//Add 0605
            {
                if (m_sMor == "Erosion")
                {
                    for (int i = 0; i < m_nMor; i++) m_ImgWorking.Erosion(new CPoint(0, 0), m_szROI, 255);
                    OverlayImage(m_ImgWorking, m_cpLimit[0]);
                }
                if (m_sMor == "ErosionGray")
                {
                    for (int i = 0; i < m_nMor; i++) m_ImgWorking.ErosionGray();
                    OverlayImage(m_ImgWorking, m_cpLimit[0]);
                }
                if (m_sMor == "Dilation")
                {
                    for (int i = 0; i < m_nMor; i++) m_ImgWorking.Dilation(new CPoint(0, 0), m_szROI, 255);
                    OverlayImage(m_ImgWorking, m_cpLimit[0]);
                }
                if (m_sMor == "DilationGray")
                {
                    for (int i = 0; i < m_nMor; i++) m_ImgWorking.DilationGray();
                    OverlayImage(m_ImgWorking, m_cpLimit[0]);
                }
                if (m_sMor == "Openning")
                {
                    for (int i = 0; i < m_nMor; i++) m_ImgWorking.Erosion(new CPoint(0, 0), m_szROI, 255);
                    for (int i = 0; i < m_nMor; i++) m_ImgWorking.Dilation(new CPoint(0, 0), m_szROI, 255);
                    OverlayImage(m_ImgWorking, m_cpLimit[0]);
                }
                if (m_sMor == "Closing")
                {
                    for (int i = 0; i < m_nMor; i++) m_ImgWorking.Erosion(new CPoint(0, 0), m_szROI, 255);
                    for (int i = 0; i < m_nMor; i++) m_ImgWorking.Dilation(new CPoint(0, 0), m_szROI, 255);
                    OverlayImage(m_ImgWorking, m_cpLimit[0]);
                }
            }
            if (m_strTool == "Resize")
            {
                m_ImgWorking.Resize(m_ImgMask, m_dScale);
                OverlayImage(m_ImgMask, m_cpLimit[0]);
            }
            if (m_strTool == "AddNoise")
            {
                m_ImgWorking.AddNoise(m_sNoise, m_dAmount);
                OverlayImage(m_ImgWorking, m_cpLimit[0]);
            }
            if (m_strTool == "MatrixCode")
            {
                m_strDecode = m_cogBCR2D.Run(m_ImgWorking, new CPoint(0, 0), m_ImgWorking.m_szImg);
                //m_strDecode = Inspect2D();
                //if (m_pVerifyView != null) m_pVerifyView.InvalidData(m_bcd2D.m_MatrixCode.m_typeAIM, m_bcd2D.m_MatrixCode.m_typeSemiT10);
            }
            if (m_strTool == "OCR")
            {
                m_strDecode = "";
                //m_CogOCR.Run(m_ImgWorking, m_cpLBD, m_cpLBU - m_cpLBD, ref result);
                m_cogOCR.Run(m_ImgWorking, new CPoint(0, 0), m_ImgWorking.m_szImg, ref m_strDecode);

                /*m_ImgWorking.RotateImage(m_ImgWorking, m_ImgMask, m_sOCRRot);
                m_ImgMask.FileSave("C:\\Temp\\OCR.bmp");
                m_OCR.m_sMode[c_iFont] = m_sOCRLib;
                m_OCR.m_sMode[c_iEffort] = "High";
                m_OCR.m_nValue[c_iAcceptLevel] = m_nOCRAccept;
                m_OCR.LoadFont(m_sOCRLib);
                m_OCR.SetParams();
                m_strDecode = "OCR:";
                m_strDecode += m_OCR.ExcecuteOCR(m_ImgMask.m_szImg, m_ImgMask.m_aBuf);//Modify 0417 ThaiDN*/
            }
            if (m_strTool == "1DBarcode")
            {
                m_ImgWorking.FileSave("C:\\Temp\\1DBarcode.bmp");
                m_strDecode = "Code:"; m_strDecode += m_cogBCR.Run(m_ImgWorking, new CPoint(0, 0), m_ImgWorking.m_szImg); // m_1DBarcode.Excecute1DBarcode(0, m_ImgWorking.m_szImg, m_ImgWorking.m_aBuf);
                //m_strDecode += "\nType:"; m_strDecode += m_1DBarcode.m_strCodeType;
                //UpdateData(false);
            }
            /*if (m_strTool == "Polution Length") //Add 1127
            {
                azBlob blob; int nBlob, n, count = 0, nIndex = -1; CPoint szBlob;
                long nMaxSz = 0;
                blob.DoBlobInvert(eBlob.eArea, m_ImgWorking, new CPoint(0, 0), m_ImgWorking.m_szImg, m_nPoGV[0], m_nPoGV[1]);
                nBlob = blob.GetNumLand();
                for (n = 1; n <= nBlob; n++)
                {
                    if (nMaxSz <= blob.GetIsland(n).GetSize(eBlob.eArea))
                    {
                        nMaxSz = blob.GetIsland(n).GetSize(eBlob.eArea);
                        nIndex = n;
                    }
                }
                if (nIndex > 0)
                {
                    szBlob.x = blob.GetIsland(nIndex).GetSize(eBlob.eX);
                    szBlob.y = blob.GetIsland(nIndex).GetSize(eBlob.eY);
                    m_strDecode = "Size of max polution: " + szBlob.x.ToString() + "x" + szBlob.y.ToString();
                }
                else m_strDecode = "Size of max polution: 0 x 0"; 
            }*/
            labelResult.Text = m_strDecode;
            Cursor.Current = Cursors.Default;
        }

        private void buttonRedo_Click(object sender, EventArgs e)
        {
            if (!CheckInput()) return;
            OverlayImage(m_ImgBackup, m_cpLimit[0]);
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate); RunGrid(eGrid.eRegWrite);
        }

        private void comboAdvanced_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void buttonApply2_Click(object sender, EventArgs e)
        {
            if (comboAdvanced.SelectedItem.ToString() == "None")
            {
                m_OCRTool.Hide();
                //m_OCRBinary.ShowWindow(SW_HIDE);
                //m_ML.ShowWindow(SW_HIDE);
                //m_teach->ShowWindow(SW_HIDE);
            }
            if (comboAdvanced.SelectedItem.ToString() == "OCRTool")
            {
                m_OCRTool.Show();
                //m_OCRTool->ShowWindow(SW_SHOW);
                //m_OCRBinary.ShowWindow(SW_HIDE);
                //m_ML.ShowWindow(SW_HIDE);
                //m_teach->ShowWindow(SW_HIDE);
            }
            /*
            if (m_strAdvancedTool == "OCRGray")
            {
                m_OCRTool->ShowWindow(SW_SHOW);
                m_OCRBinary.ShowWindow(SW_HIDE);
                m_ML.ShowWindow(SW_HIDE);
                m_teach->ShowWindow(SW_HIDE);
            }
            if (m_strAdvancedTool == "OCRBinary")
            {
                m_OCRBinary.ShowWindow(SW_SHOW);
                m_OCRTool->ShowWindow(SW_HIDE);
                m_ML.ShowWindow(SW_HIDE);
                m_teach->ShowWindow(SW_HIDE);
            }
            if (m_strAdvancedTool == "MachineLearning")
            {
                m_OCRBinary.ShowWindow(SW_HIDE);
                m_OCRTool->ShowWindow(SW_HIDE);
                m_ML.ShowWindow(SW_SHOW);
                m_teach->ShowWindow(SW_HIDE);
            }
            if (m_strAdvancedTool == "Teaching")
            {
                m_OCRBinary.ShowWindow(SW_HIDE);
                m_OCRTool->ShowWindow(SW_HIDE);
                m_ML.ShowWindow(SW_HIDE);
                m_teach->ShowWindow(SW_SHOW);
            } */
        }

        private void ImageEdit_Resize(object sender, EventArgs e)
        {
            Size szGrid = new Size();
            szGrid.Width = grid.Size.Width;
            szGrid.Height = this.Size.Height - 200;
            grid.Size = szGrid;
            groupAdvanced.Location = new Point(grid.Location.X, this.Size.Height - 105);
            groupOutput.Location = new Point(grid.Location.X, this.Size.Height - 155);
        }
     
        [DllImport("ezCpp.dll")]
        unsafe public static extern void cpp_memcpy(byte* pDst, byte* pSrc, int nLength);

        private void ImageEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }   
    }
}
