using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using ezAutoMom;
using ezTools;

namespace ezAuto_EFEM
{
    class HW_Aligner_ATI_AOI
    {
        public int m_nID;
        public string m_id;
        Log m_log;
        Work_Mom m_work;
        HW_Aligner_Mom m_aligner;

        HW_Aligner_ATI_AOI_Data m_data = null; 

        byte[,] m_aBuf;
        public ezImg m_img = null;
        public ezImg m_imgRotate = null; 
        public CPoint m_szImg = new CPoint(0, 0);

        int[] m_aEdge = null;
        int[] m_aCalc = null;
        int[] m_aCircle = null;
        List<CPoint> m_NotchList = new List<CPoint>(); // BHJ 190722 add

        public bool m_bFindNotch = false;

        int[] m_xROI = new int[2] { 200, 1200 }; 

        double m_nAlignSize = 0; // BHJ 190722 edit  int -> double
        public CPoint m_cpNotch = new CPoint();
        RPoint m_rpNotch = new RPoint();

        RPoint m_rpCenter;
        double m_dR = 0;
        public double m_degNotch = 0;

        double[,] m_XY = new double[4, 4] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
        double[,] m_a = new double[3, 3] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
        double[] m_b = new double[3] { 0, 0, 0 };

        public double m_posAxis = 0;

        string m_strFile;

        public ArrayList m_aChippingInfo = new ArrayList();

        ippTools m_ipp = new ippTools();

        public bool m_bInspect = false;
        bool m_bRun = false;
        Thread m_thread;

        public HW_Aligner_ATI_AOI()
        {
        }

        public void Init(string id, int nID, Work_Mom work, HW_Aligner_ATI_AOI_Data data, Log log, HW_Aligner_Mom aligner)
        {
            m_id = id;
            m_nID = nID;
            m_work = work;
            m_aligner = aligner;// ((EFEM)m_work.m_auto).ClassHandler().ClassAligner();
            m_data = data; 
            m_log = log;
            m_img = new ezImg(m_id, log);
            m_imgRotate = new ezImg(m_id + "Rotate", log); 
            m_thread = new Thread(new ThreadStart(RunThread));
            m_thread.Start();
        }

        public void ThreadStop()
        {
            if (m_bRun)
            {
                m_bRun = false;
                m_thread.Join();
            }
        }

        public void StartInspect(double posAxis, string strFile)
        {
            m_posAxis = posAxis;
            m_strFile = strFile;
            m_bInspect = true;
            m_aChippingInfo.Clear();
        }

        public bool WaitInspect(int msWait)
        {
            ezStopWatch sw = new ezStopWatch();
            while (m_bInspect && (sw.Check() <= msWait)) Thread.Sleep(1);
            return sw.Check() >= msWait;
        }
      
        public double CalcWaferShift() // BHJ 191128 add
        {
            int xMin = 0;
            int vMin = m_szImg.y + 1;
            for (int x = m_xROI[0]; x < m_xROI[1]; x++)  // BHJ 190226 add
            //for (int x = c_nMargin; x < m_szImg.x - c_nMargin; x++)    // BHJ 190226 del
            {
                if (vMin > m_aEdge[x])
                {
                    vMin = m_aEdge[x];
                    xMin = x;
                }
            }
            return (m_aEdge[xMin - 2] + m_aEdge[xMin - 1] + m_aEdge[xMin] + m_aEdge[xMin + 1] + m_aEdge[xMin + 2]) / 5.0;
        }

        void RunThread()
        {
            m_bRun = true;
            Thread.Sleep(1000);
            while (m_bRun)
            {
                Thread.Sleep(1);
                if (m_bInspect) RunInspect();
            }
        }

        public void Draw(Graphics dc, ezImgView imgView)
        {
            if (m_img == null) return;
            if (m_img.m_bNew) return;
            if (m_aCircle == null) return;
            imgView.DrawLine(dc, Color.Green, new CPoint(m_xROI[0], 0), new CPoint(m_xROI[0], m_szImg.y));
            imgView.DrawLine(dc, Color.Green, new CPoint(m_xROI[1], 0), new CPoint(m_xROI[1], m_szImg.y));
            imgView.DrawLine(dc, Color.Blue, new CPoint(m_xROI[0], 100), new CPoint(m_xROI[1], 100));
            DrawLines(dc, imgView, Color.Yellow, m_aCircle);
            if (m_data.m_eShape == HW_Aligner_ATI_AOI_Data.eShape.Notch)
            {
                DrawLines(dc, imgView, Color.Aqua, m_aCalc);
                if (m_bFindNotch) DrawCross(dc, imgView, Color.Red, m_cpNotch, 20);
            }
            DrawLines(dc, imgView, Color.Red, m_aEdge);
            foreach (Defect defect in m_aChippingInfo)
            {
                DrawCross(dc, imgView, Color.Red, defect.m_cp0, 10);
            }
        }

        void DrawLines(Graphics dc, ezImgView imgView, Color color, int[] aLine)
        {
            int x0 = m_xROI[0];
            int x1 = m_xROI[1];
            CPoint[] cp = new CPoint[2];
            cp[0] = new CPoint(x0, aLine[x0]);
            cp[1] = new CPoint(0, 0);
            for (int x = x0 + 1; x < x1; x++)
            {
                cp[1].x = x;
                cp[1].y = aLine[x];
                imgView.DrawLine(dc, color, cp[0], cp[1]);
                cp[0] = cp[1];
            }
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

        public double GetAlignSize() // BHJ 190722 edit  int -> double
        {
            return m_nAlignSize;
        }

        int GetNotchSize()
        {
            try
            {
                if (m_cpNotch.x < m_data.c_nNotchSize) return 0;
                if (m_cpNotch.x > (m_szImg.x - m_data.c_nNotchSize)) return 0;
                int nAns = 2 * m_aCalc[m_cpNotch.x];
                nAns -= m_aCalc[m_cpNotch.x - m_data.c_nNotchSize];
                nAns -= m_aCalc[m_cpNotch.x + m_data.c_nNotchSize];
                nAns -= 3 * Math.Abs(m_aCalc[m_cpNotch.x - m_data.c_nNotchSize] - m_aCalc[m_cpNotch.x + m_data.c_nNotchSize]);
                nAns -= (int)(Math.Abs(m_szImg.x / 2 - m_cpNotch.x) / 4);
                m_nAlignSize = nAns;
                return nAns;
            }
            catch (Exception ex)                        //190521 SDH ADD
            {
                m_log.Add(ex.Message);
                m_log.Add(ex.ToString());
                m_log.Popup("Image Insp Error !! 1");
                m_bInspect = false;
                return 0;
            }
        }

        void ImgBinary() // BHJ 190625 add
        {
            for (int i = 0; i < m_img.m_szImg.y; i++)
            {
                for (int j = 0; j < m_img.m_szImg.x; j++)
                {
                    bool bWafer = false;
                    if (m_img.m_aBuf[i, j] >= m_aligner.m_nBinaryGV)
                    {
                        if (j > 10 && j < m_img.m_szImg.x - 10)
                        {
                            if (m_img.m_aBuf[i, j + 10] <= m_aligner.m_nBinaryGV && m_img.m_aBuf[i, j - 10] <= m_aligner.m_nBinaryGV) bWafer = true;

                        }
                        if (i > 10 && i < m_img.m_szImg.y - 10)
                        {
                            if (m_img.m_aBuf[i + 10, j] <= m_aligner.m_nBinaryGV && m_img.m_aBuf[i - 10, j] <= m_aligner.m_nBinaryGV) bWafer = true;
                        }
                        if (bWafer) m_img.m_aBuf[i, j] = 0;
                        else m_img.m_aBuf[i, j] = 255;
                    }
                    else
                    {
                        m_img.m_aBuf[i, j] = 0;
                    }
                }
            }
        }

        void RunInspect()
        {
            try
            {
                ezStopWatch sw = new ezStopWatch();
                if (m_aligner.m_bUseImgBinary) ImgBinary(); // BHJ 190625 add
                m_bFindNotch = false;

                ReAllocate();
                if (m_szImg.x == 0)
                {
                    m_log.Popup("Grab Image Error !!");
                    m_bInspect = false;
                    return;
                }
                m_aBuf = (byte[,])m_img.m_aBuf.Clone();
                switch (m_data.m_eInspect)
                {
                    case HW_Aligner_ATI_AOI_Data.eInspect.WhiteWafer: FindEdgeWhiteWafer(); break;
                    case HW_Aligner_ATI_AOI_Data.eInspect.White300: FindEdgeWhite300(); break;
                    case HW_Aligner_ATI_AOI_Data.eInspect.BlackWafer: FindEdgeBlackWafer(); break;
                    case HW_Aligner_ATI_AOI_Data.eInspect.Black_Blob: FindEdgeBlob(false); break;
                    //                case eInspect.Black_Dicing: FindEdgeDicing(false); break;
                    default: FindEdgeBlob(false); break;
                }
                m_img.m_aBuf = m_aBuf; // ing 170816
                CalcEdge();
                NoiseFilterFor1D(m_aCalc, 6);   //lcs 190626

                switch (m_data.m_eShape)
                {
                    //case HW_Aligner_ATI_AOI_Data.eShape.Notch: RunInspectNotch(sw); break;
                    case HW_Aligner_ATI_AOI_Data.eShape.Notch: RunInspectNotch2(sw); break; // BHJ 190722 add
                    case HW_Aligner_ATI_AOI_Data.eShape.Flat: RunInspectFlat(sw); break;
                    default: RunInspectNotch(sw); break;
                }

                if (m_data.m_nChipping > 0) CalcChipping();

                if (m_strFile != null)
                {
                    m_img.FileSave(m_strFile);
                }

                m_img.m_bNew = false;
                m_bInspect = false;
            }
            catch (Exception ex)                        //190521 SDH ADD
            {
                m_log.Add(ex.Message);
                m_log.Add(ex.ToString());
                m_log.Popup("Image Insp Error !! 2");
                m_bInspect = false;
                return;
            }
             
        }

        void RunInspectNotch(ezStopWatch sw)
        {
            try
            {
                FindNotch();
                if (GetNotchSize() > 0)
                {
                    DetectEdge(m_cpNotch, m_data.c_nNotchSize, 2);
                    GaussEllimination();
                    for (int x = m_xROI[0]; x < m_xROI[1]; x++)
                    {
                        double dx = Math.Abs(x - m_rpCenter.x);
                        double dy = Math.Sqrt(m_dR * m_dR - dx * dx);
                        m_aCircle[x] = (int)(m_rpCenter.y - dy);
                        if (m_aCircle[x] < 0) m_aCircle[x] = 0;
                    }
                    m_bFindNotch = HasNotch();
                    m_log.Add(string.Format("#In RunInspectNotch() : m_bFindNotch = {0}", m_bFindNotch), false); // BHJ 190405 add #alignFailDebug
                    if (m_bFindNotch)
                    {
                        FindNotchAgain();
                        CalcAngle();
                        if (m_data.m_bNotch) HasNotchCC();
                        m_log.Add("Inspect Done " + m_nID.ToString("00") + " : " + sw.Check().ToString() + "ms, Size = " + m_nAlignSize.ToString() + ", Axis = " + m_posAxis.ToString() + ", Deg = " + m_degNotch.ToString("0.0000"));
                    }
                }
            }
            catch (Exception ex)                        //190521 SDH ADD
            {
                m_log.Add(ex.Message);
                m_log.Add(ex.ToString());
                m_log.Popup("Image Insp Error !! 3");
                m_bInspect = false;
                return;
            }
        }

        void RunInspectNotch2(ezStopWatch sw)   //lcs 190626 노치 검색 수정 // BHJ 190722 add
        {
            m_nAlignSize = 0;
            m_cpNotch = new CPoint(0, 0);
            FindNotchGroup();
            int nXSize = m_data.m_imgNotch.m_szImg.x * 2;
            int nYSize = m_data.m_imgNotch.m_szImg.y * 2;
            CPoint sz = new CPoint(nXSize, nYSize);
            for (int n = 0; n < m_NotchList.Count; n++)
            {
                int nXStartPoint = m_NotchList[n].x - m_data.m_imgNotch.m_szImg.x;
                int nYStartPoint = m_NotchList[n].y - m_data.m_imgNotch.m_szImg.y;
                if (nXStartPoint < 0) nXStartPoint = 0;
                if (nYStartPoint < 0) nYStartPoint = 0;
                CPoint cp0 = new CPoint(nXStartPoint, nYStartPoint);


                CPoint cpNewNotch = new CPoint(0, 0);
                double AlignSize = m_ipp.ippiCrossCorrSame_NormLevel(m_data.m_imgNotch, m_img, cp0, sz, ref cpNewNotch);
                if (AlignSize > m_nAlignSize)
                {
                    m_nAlignSize = AlignSize;
                    m_cpNotch = cpNewNotch;
                }
                m_log.Add(m_nID.ToString("00") + ", CC Calibration :" + m_NotchList[n].ToString() + "->" + cpNewNotch.ToString() + ", Score : " + AlignSize.ToString("0.0000"));
            }
            m_bFindNotch = (m_nAlignSize > 65);
            DetectEdge(m_cpNotch, m_data.c_nNotchSize, 2);
            GaussEllimination();
            for (int x = m_xROI[0]; x < m_xROI[1]; x++)
            {
                double dx = Math.Abs(x - m_rpCenter.x);
                double dy = Math.Sqrt(m_dR * m_dR - dx * dx);
                m_aCircle[x] = (int)(m_rpCenter.y - dy);
                if (m_aCircle[x] < 0) m_aCircle[x] = 0;
            }

            if (m_bFindNotch)
            {
                FindNotchAgain();
                CalcAngle();
                m_log.Add("Inspect CC Done " + m_nID.ToString("00") + " : " + "Score = " + m_nAlignSize.ToString("0.0000") + ", Axis = " + m_posAxis.ToString() + ", Deg = " + m_degNotch.ToString("0.0000"));
            }
        }

        bool HasNotch()
        {
            try
            {
                if (m_nAlignSize <= 0) return false;
                if (m_dR < 3000)
                {
                    m_nAlignSize = 0;
                    return false;
                }
                return HasNotchOld();
            }
            catch (Exception ex)                        //190521 SDH ADD
            {
                m_log.Add(ex.Message);
                m_log.Add(ex.ToString());
                m_log.Popup("Image Insp Error !! 4");
                m_bInspect = false;
                return false;
            }
        }

        bool HasNotchCC()
        {
            try
            {
                CPoint cp0 = m_cpNotch - m_data.c_szNotchROI2;
                CPoint sz = m_data.c_szNotchROI;
                CPoint cp = new CPoint(0, 0);
                CPoint szImg = m_img.m_szImg;
                Rectangle rect = new Rectangle(0, 0, szImg.x, szImg.y);
                m_imgRotate.ReAllocate(m_img);
                m_ipp.ippiRotate(m_imgRotate, szImg, rect, m_img, szImg, rect, -m_degNotch, m_cpNotch.x, m_cpNotch.y, 4);
                m_nAlignSize = (int)m_ipp.ippiCrossCorrSame_NormLevel(m_data.m_imgNotch, m_imgRotate, cp0, sz, ref cp);
                return true;
            }
            catch (Exception ex)                        //190521 SDH ADD
            {
                m_log.Add(ex.Message);
                m_log.Add(ex.ToString());
                m_log.Popup("Image Insp Error !! 5");
                m_bInspect = false;
                return false;
            }
        }

        bool HasNotchOld()
        {
            m_nAlignSize = 0;
            if (m_dR < m_data.m_minR) return false;
            int dy1 = 0;
            int dy0 = m_szImg.y;
            for (int x = m_cpNotch.x; x < m_cpNotch.x + m_data.c_nNotchSize; x++)
            {
                dy1 = m_aEdge[x] - m_aCircle[x];
                if (dy1 > 1.5 * m_data.c_nNotchSize)
                {
                    m_nAlignSize = 0;
                    return false;
                }
                if (dy1 > (dy0 + 10)) x = m_cpNotch.x + m_data.c_nNotchSize + 10;
                m_nAlignSize += dy1;
                dy0 = dy1;
            }
            dy0 = m_szImg.y;
            for (int x = m_cpNotch.x; x > m_cpNotch.x - m_data.c_nNotchSize; x--)
            {
                dy1 = m_aEdge[x] - m_aCircle[x];
                if (dy1 > 1.5 * m_data.c_nNotchSize)
                {
                    m_nAlignSize = 0;
                    return false;
                }
                if (dy1 > (dy0 + 10)) x = m_cpNotch.x - m_data.c_nNotchSize - 10;
                m_nAlignSize += dy1;
                dy0 = dy1;
            }
            m_nAlignSize -= (Math.Abs(m_cpNotch.x - m_szImg.x / 2) / 4);
            return true;
        }

        void ReAllocate()
        {
            if (m_szImg != m_img.m_szImg)
            {
                m_szImg = m_img.m_szImg;
                m_aEdge = new int[m_szImg.x];
                m_aCalc = new int[m_szImg.x];
                m_aCircle = new int[m_szImg.x];
            }
            m_xROI[0] = m_data.m_nMargin + m_data.m_dxROI;
            m_xROI[1] = m_szImg.x - m_data.m_nMargin + m_data.m_dxROI; 
        }

        void FindEdgeBlackWafer()
        {
            try
            {
                for (int x = m_xROI[0]; x < m_xROI[1]; x++)
                {
                    int y = 5;
                    while ((y < m_szImg.y) && (m_img.m_aBuf[y, x] <= (2 * m_data.m_nGV))) y++;
                    if (y < m_szImg.y - 1)
                    {
                        do
                        {
                            if (m_data.m_nErosion > 0) XErosion(x, y + 1, m_data.m_nErosion);
                            y++;
                        } while ((y < m_szImg.y - 1) && (m_img.m_aBuf[y, x] >= m_data.m_nGV));

                    }
                    if (y >= m_szImg.y) y = 0;
                    m_aEdge[x] = y;
                }
            }
            catch (Exception ex)                        //190521 SDH ADD
            {
                m_log.Add(ex.Message);
                m_log.Add(ex.ToString());
                m_log.Popup("Image Insp Error !! 6");
                m_bInspect = false;
                return;
            }
        }

        void FindEdgeWhiteWafer()
        {
            try
            {
                for (int x = m_xROI[0]; x < m_xROI[1]; x++)
                {
                    int y = 5;
                    while ((y < m_szImg.y) && ((m_img.m_aBuf[y, x] - m_img.m_aBuf[y - 5, x]) < m_data.m_nGV)) y++;
                    m_aEdge[x] = y;
                }
            }

            catch (Exception ex)                        //190521 SDH ADD
            {
                m_log.Add(ex.Message);
                m_log.Add(ex.ToString());
                m_log.Popup("Image Insp Error !! 7");
                m_bInspect = false;
                return;
            }
        }

        void FindEdgeWhite300()
        {
            for (int x = m_xROI[0]; x < m_xROI[1]; x++)
            {
                int y = 5;
                while ((y < m_szImg.y) && (m_img.m_aBuf[y, x] < m_data.m_nGV)) y++;
                m_aEdge[x] = y;
            }
        }

        enum eID
        {
            Off,
            On,
            Mark,
            Find
        }
        eID[,] m_aWafer = null; 

        void FindEdgeBlob(bool bWhite)
        {
            try
            {
                ReAllocateBlob();
                Array.Clear(m_aWafer, 0, m_szImg.x * m_szImg.y);
                for (int x = 0; x < m_szImg.x; x++) MakeBlob(x, bWhite);
                for (int x = 0; x < m_szImg.x; x++) FindEdge(x);
            }
            catch (Exception ex)                        //190521 SDH ADD
            {
                m_log.Add(ex.Message);
                m_log.Add(ex.ToString());
                m_log.Popup("Image Insp Error !! 8");
                m_bInspect = false;
                return;
            }
        }

        void FindEdgeDicing(bool bWhite)
        {
            ReAllocateBlob();
            Array.Clear(m_aWafer, 0, m_szImg.x * m_szImg.y);
            for (int x = 0; x < m_szImg.x; x++) MakeBlobDicing(x, bWhite);
            for (int x = 0; x < m_szImg.x; x++) FindEdgeDicing(x);
        }

        void ReAllocateBlob()
        {
            if ((m_aWafer != null) && (m_aWafer.Length == (m_szImg.x * m_szImg.y))) return;
            m_aWafer = new eID[m_szImg.y, m_szImg.x]; 
        }

        void MakeBlob(int x, bool bWhite)
        {
            for (int y = 0; y < m_szImg.y; y++)
            {
                if (bWhite ^ (m_img.m_aBuf[y, x] < m_data.m_nGV)) m_aWafer[y, x] = eID.On;
                else m_aWafer[y, x] = eID.Off;
            }
            for (int y = m_szImg.y - 1; y > 0; y--)
            {
                if (m_aWafer[y, x] != eID.On) return; 
                m_aWafer[y, x] = eID.Mark;
            }
        }

        void MakeBlobDicing(int x, bool bWhite)
        {
            for (int y = 0; y < m_szImg.y; y++)
            {
                if ((bWhite == false) ^ (m_img.m_aBuf[y, x] < m_data.m_nGV)) m_aWafer[y, x] = eID.On;
                else m_aWafer[y, x] = eID.Off;
            }
            for (int y = 0; y < m_szImg.y; y++)
            {
                if (m_aWafer[y, x] != eID.On) return;
                m_aWafer[y, x] = eID.Mark;
            }
        }

        void FindEdge(int x)
        {
            for (int y = 0; y < m_szImg.y; y++) CheckWafer(x, y);
            for (int y = 5; y < m_szImg.y - 5; y++)
            {
                if (m_aWafer[y, x] == eID.Mark)
                {
                    m_aEdge[x] = y; 
                    return;
                }
            }
        }

        void FindEdgeDicing(int x)
        {
            for (int y = 0; y < m_szImg.y; y++) CheckWafer(x, y);
            for (int y = m_szImg.y - 5; y > 5; y--)
            {
                if (m_aWafer[y, x] == eID.Mark)
                {
                    m_aEdge[x] = y;
                    return;
                }
            }
        }

        void CheckWafer(int x, int y)
        {
            if (m_aWafer[y, x] != eID.On) return;
            eID eid = FindWafer(x, y);
            FillWafer(x, y, eid);
        }

        Stack m_aStack = new Stack();

        eID FindWafer(int cpx, int cpy)
        {
            int x, y, x0, y0;
            int xl = m_xROI[0];
            int xr = m_xROI[1] - 1;
            int yl = 0;
            int yr = m_szImg.y - 1;
            m_aWafer[cpy, cpx] = eID.Find;
            m_aStack.Clear();
            while (true)
            {
                x0 = cpx;
                y0 = cpy; 
                if (cpx > xl)
                {
                    x = x0 - 1;
                    y = y0;
                    if (m_aWafer[y, x] == eID.Mark) return eID.Mark; 
                    if (m_aWafer[y, x] == eID.On)
                    {
                        m_aWafer[y, x] = eID.Find;
                        m_aStack.Push(new CPoint(x0, y0));
                        cpx--;
                        continue; 
                    }
                }
                if (cpy > yl)
                {
                    x = x0;
                    y = y0 - 1;
                    if (m_aWafer[y, x] == eID.Mark) return eID.Mark;
                    if (m_aWafer[y, x] == eID.On)
                    {
                        m_aWafer[y, x] = eID.Find;
                        m_aStack.Push(new CPoint(x0, y0));
                        cpy--;
                        continue;
                    }
                }
                if (cpy < yr)
                {
                    x = x0;
                    y = y0 + 1;
                    if (m_aWafer[y, x] == eID.Mark) return eID.Mark;
                    if (m_aWafer[y, x] == eID.On)
                    {
                        m_aWafer[y, x] = eID.Find;
                        m_aStack.Push(new CPoint(x0, y0));
                        cpy++;
                        continue;
                    }
                }
                if (cpx < xr)
                {
                    x = x0 + 1;
                    y = y0;
                    if (m_aWafer[y, x] == eID.Mark) return eID.Mark;
                    if (m_aWafer[y, x] == eID.On)
                    {
                        m_aWafer[y, x] = eID.Find;
                        m_aStack.Push(new CPoint(x0, y0));
                        cpx++;
                        continue;
                    }
                }
                if (m_aStack.Count == 0) break;
                CPoint cp0 = (CPoint)m_aStack.Pop();
                cpx = cp0.x;
                cpy = cp0.y;
            }
            return eID.Off;
        }

        void FillWafer(int cpx, int cpy, eID eid)
        {
            int x, y, x0, y0;
            int xl = m_xROI[0];
            int xr = m_xROI[1] - 1;
            int yl = 0;
            int yr = m_szImg.y - 1;
            m_aWafer[cpy, cpx] = eid;
            m_aStack.Clear();
            while (true)
            {
                x0 = cpx;
                y0 = cpy;
                if (cpx > xl)
                {
                    x = x0 - 1;
                    y = y0;
                    if (m_aWafer[y, x] == eID.Find)
                    {
                        m_aWafer[y, x] = eid;
                        m_aStack.Push(new CPoint(x0, y0));
                        cpx--;
                        continue;
                    }
                }
                if (cpy > yl)
                {
                    x = x0;
                    y = y0 - 1;
                    if (m_aWafer[y, x] == eID.Find)
                    {
                        m_aWafer[y, x] = eid;
                        m_aStack.Push(new CPoint(x0, y0));
                        cpy--;
                        continue;
                    }
                }
                if (cpy < yr)
                {
                    x = x0;
                    y = y0 + 1;
                    if (m_aWafer[y, x] == eID.Find)
                    {
                        m_aWafer[y, x] = eid;
                        m_aStack.Push(new CPoint(x0, y0));
                        cpy++;
                        continue;
                    }
                }
                if (cpx < xr)
                {
                    x = x0 + 1;
                    y = y0;
                    if (m_aWafer[y, x] == eID.Find)
                    {
                        m_aWafer[y, x] = eid;
                        m_aStack.Push(new CPoint(x0, y0));
                        cpx++;
                        continue;
                    }
                }
                if (m_aStack.Count == 0) break;
                CPoint cp0 = (CPoint)m_aStack.Pop();
                cpx = cp0.x;
                cpy = cp0.y;
            }
        }

        void CalcEdge()
        {
            try
            {
                for (int x = 0; x < m_szImg.x; x++) m_aCalc[x] = 0;
                for (int x = m_xROI[0] + m_data.c_nNotchSize; x < m_xROI[1] - m_data.c_nNotchSize; x++)
                {
                    m_aCalc[x] = 100 + 2 * m_aEdge[x] - m_aEdge[x - m_data.c_nNotchSize] - m_aEdge[x + m_data.c_nNotchSize];
                    if (m_aCalc[x] < 0) m_aCalc[x] = 0;
                }
            }
            catch (Exception ex)                        //190521 SDH ADD
            {
                m_log.Add(ex.Message);
                m_log.Add(ex.ToString());
                m_log.Popup("Image Insp Error !! 9");
                m_bInspect = false;
                return;
            }
        }

        //LCS 전체적인 응용을 위해서는 수정이 필요함 -> 98과 100 등
        void NoiseFilterFor1D(int[] LineData, int Num = 1)
        {
            for (int n = 0; n < Num; n++)
            {
                for (int x = 0; x < LineData.Length; x++)
                {
                    if (LineData[x] != 0)
                    {
                        if (LineData[x] > 98 && LineData[x] < 102) LineData[x] = 100;
                        else
                        {
                            LineData[x] = (LineData[x - 1] + LineData[x + 1]) / 2;
                        }
                    }
                }
            }
        }

        void FindNotch()
        {
            try
            {
                for (int x = 0; x < m_szImg.x; x++) m_aCircle[x] = 0;
                int nMax = 0;
                int xMax = m_szImg.x / 2;
                for (int x = m_xROI[0]; x < m_xROI[1]; x++)
                {
                    if (nMax < m_aCalc[x])
                    {
                        nMax = m_aCalc[x];
                        xMax = x;
                    }
                }

                int vb = 0;
                int xvb = 0;
                for (int x = (xMax - m_xROI[0]); x < (xMax + m_xROI[0]); x++)
                {
                    vb += m_aCalc[x];
                    xvb += (x * m_aCalc[x]);
                }

                int xNotch = xMax;
                if (vb > 0) xNotch = (int)Math.Round(1.0 * xvb / vb);
                m_cpNotch = new CPoint(xNotch, m_aEdge[xNotch]);
                if (m_cpNotch.x < m_data.c_nNotchSize) m_cpNotch.x = m_data.c_nNotchSize;
                if (m_cpNotch.x > (m_szImg.x - m_data.c_nNotchSize)) m_cpNotch.x = m_szImg.x - m_data.c_nNotchSize;
            }
            catch (Exception ex)                        //190521 SDH ADD
            {
                m_log.Add(ex.Message);
                m_log.Add(ex.ToString());
                m_log.Popup("Image Insp Error !! 10");
                m_bInspect = false;
                return;
            }
        }

        void FindNotchGroup() // BHJ 190722 add 
        {
            for (int x = 0; x < m_szImg.x; x++) m_aCircle[x] = 0;

            //SearchLevel에 맞게 노치 서치 및 추가
            m_NotchList.Clear();
            int StartX = 0;
            int EndX = 0;
            for (int x = m_xROI[0]; x < m_xROI[1]; x++)
            {
                if (m_aCalc[x] > 100 + m_data.m_nNotchThreadhold)
                {
                    if (m_aCalc[x] < m_aCalc[x + 1])
                    {
                        if (StartX == 0) StartX = x;
                    }
                    if (m_aCalc[x] > m_aCalc[x + 1])
                    {
                        if (StartX != 0 && EndX == 0) EndX = x;
                    }
                    if (StartX != 0 && EndX != 0)
                    {
                        int NotchX = (StartX + EndX) / 2;
                        m_cpNotch = new CPoint(NotchX, m_aEdge[NotchX]);
                        m_NotchList.Add(m_cpNotch);
                        m_log.Add(m_nID.ToString("00") + ", NotchGroup[" + m_NotchList.Count.ToString() + "]" + m_cpNotch.ToString());
                        StartX = 0;
                        EndX = 0;
                    }
                }
            }
        }

        bool DetectEdge(CPoint cp, double dR, int nSample)
        {
            try
            {
                int x, y;
                for (y = 0; y < 4; y++) for (x = 0; x < 4; x++) m_XY[x, y] = 0;
                for (y = 0; y < 3; y++)
                {
                    m_b[y] = 0;
                    for (x = 0; x < 3; x++) m_a[x, y] = 0;
                }
                for (x = m_xROI[0]; x < m_xROI[1]; x += nSample)
                {
                    y = m_aEdge[x];
                    if ((GetDist(cp, x, y) >= dR))
                    {
                        m_XY[0, 0]++;
                        m_XY[0, 1] += y;
                        m_XY[1, 0] += x;
                        m_XY[0, 2] += (y * y);
                        m_XY[1, 1] += (x * y);
                        m_XY[2, 0] += (x * x);
                        m_XY[0, 3] += (1.0 * y * y * y);
                        m_XY[1, 2] += (1.0 * x * y * y);
                        m_XY[2, 1] += (1.0 * x * x * y);
                        m_XY[3, 0] += (1.0 * x * x * x);
                    }
                }
                m_a[0, 0] = m_XY[2, 0];
                m_a[1, 1] = m_XY[0, 2];
                m_a[2, 2] = m_XY[0, 0];
                m_a[1, 0] = m_a[0, 1] = m_XY[1, 1];
                m_a[2, 0] = m_a[0, 2] = m_XY[1, 0];
                m_a[2, 1] = m_a[1, 2] = m_XY[0, 1];
                m_b[0] = -m_XY[3, 0] - m_XY[1, 2];
                m_b[1] = -m_XY[2, 1] - m_XY[0, 3];
                m_b[2] = -m_XY[2, 0] - m_XY[0, 2];
                return false;
            }
            catch (Exception ex)                        //190521 SDH ADD
            {
                m_log.Add(ex.Message);
                m_log.Add(ex.ToString());
                m_log.Popup("Image Insp Error !!11");
                m_bInspect = false;
                return false;
            }
        }

        bool GaussEllimination()
        {
            try
            {
                for (int k = 0; k < 3; k++)
                {
                    if (m_a[k, k] == 0)
                    {
                        for (int i = k + 1; i < 3; i++)
                        {
                            if (m_a[i, k] != 0) continue;
                            for (int j = 0; j < 3; j++) Swap(ref m_a[k, j], ref m_a[i, j]);
                            Swap(ref m_b[k], ref m_b[i]);
                            break;
                        }
                    }
                    if (m_a[k, k] == 0) return true;
                    for (int i = k + 1; i < 3; i++)
                    {
                        m_a[i, k] /= m_a[k, k];
                        for (int j = k + 1; j < 3; j++) m_a[i, j] -= (m_a[i, k] * m_a[k, j]);
                        m_b[i] -= (m_a[i, k] * m_b[k]);
                    }
                }

                double[] dResult = new double[3] { 0, 0, 0 };
                for (int i = 2; i >= 0; i--)
                {
                    double dSum = 0;
                    for (int j = i + 1; j < 3; j++) dSum += (m_a[i, j] * dResult[j]);
                    dResult[i] = (m_b[i] - dSum) / m_a[i, i];
                }
                dResult[0] = -0.5 * dResult[0];
                dResult[1] = -0.5 * dResult[1];
                dResult[2] = Math.Sqrt(dResult[0] * dResult[0] + dResult[1] * dResult[1] - dResult[2]);

                m_rpCenter = new RPoint(dResult[0], dResult[1]);
                m_dR = dResult[2];

                return false;
            }
            catch (Exception ex)                        //190521 SDH ADD
            {
                m_log.Add(ex.Message);
                m_log.Add(ex.ToString());
                m_log.Popup("Image Insp Error !!12");
                m_bInspect = false;
                return false ;
            }
        }

        void FindNotchAgain()
        {
            try
            {
                double fSum = 0;
                int x0 = m_cpNotch.x - 2 * m_data.c_nNotchSize;
                int x1 = m_cpNotch.x + 2 * m_data.c_nNotchSize;
                for (int x = x0; x <= x1; x++)
                {
                    fSum += (m_aEdge[x] - m_aCircle[x]);
                }
                fSum /= 2;
                double fSum0 = 0;
                double fSum1 = 0;
                for (int x = x0; x <= x1; x++)
                {
                    fSum0 = fSum1;
                    fSum1 += (m_aEdge[x] - m_aCircle[x]);
                    if (fSum1 > fSum)
                    {
                        m_rpNotch.y = m_aEdge[x];
                        m_rpNotch.x = x - 1 + (fSum - fSum0) / (fSum1 - fSum0);
                        m_cpNotch = m_rpNotch.ToCPoint();
                        return;
                    }
                }
                m_rpNotch = new RPoint(m_cpNotch);
            }
            catch (Exception ex)                        //190521 SDH ADD
            {
                m_log.Add(ex.Message);
                m_log.Add(ex.ToString());
                m_log.Popup("Image Insp Error !!13");
                m_bInspect = false;
                return;
            }
        }

        void CalcAngle()
        {
            try
            {
                double dx = m_rpCenter.x - m_rpNotch.x;
                double dy = m_rpCenter.y - m_rpNotch.y;
                m_degNotch = 90 - Math.Atan2(dy, dx) * 180 / Math.PI;
            }
            catch (Exception ex)                        //190521 SDH ADD
            {
                m_log.Add(ex.Message);
                m_log.Add(ex.ToString());
                m_log.Popup("Image Insp Error !!14");
                m_bInspect = false;
                return;
            }
        }

        void Swap(ref double a, ref double b)
        {
            double c = a;
            a = b;
            b = c;
        }

        double GetDist(CPoint cp, int x, int y)
        {
            x -= cp.x;
            y -= cp.y;
            return Math.Sqrt(x * x + y * y);
        }

        void RunInspectFlat(ezStopWatch sw)
        {
            m_nAlignSize = 0;
            m_degNotch = 0;
            int dx = 20;
            int x0 = m_xROI[0] + dx;
            int x1 = m_xROI[1] - dx;
            int xc = m_szImg.x / 2;
            for (int x = 0; x < m_szImg.x; x++) m_aCircle[x] = 0;
            for (int x = x0; x < x1; x++)
            {
                m_aCalc[x] = xc + m_aEdge[x + dx] - m_aEdge[x - dx];
                if (m_aCalc[x] > 0 && m_aCalc[x] < m_img.m_szImg.x)
                    m_aCircle[m_aCalc[x]]++;
            }
            int xMax = 0;
            int vMax = 0;
            for (int x = 0; x < m_szImg.x; x++)
            {
                if (vMax < m_aCircle[x])
                {
                    vMax = m_aCircle[x];
                    xMax = x;
                }
            }

            m_nAlignSize = m_aCircle[xMax - 1] + m_aCircle[xMax] + m_aCircle[xMax + 1] - 500;
            if (m_nAlignSize < 0) return;

            double x2b = 0;
            double xb = 0;
            double xyb = 0;
            double yb = 0;
            int N = 0;
            for (int x = x0; x < x1; x++)
            {
                if ((m_aCalc[x] >= xMax - 1) && (m_aCalc[x] <= xMax + 1))
                {
                    N++;
                    xb += x;
                    x2b += (x * x);
                    yb += m_aEdge[x];
                    xyb += (x * m_aEdge[x]);
                }
            }
            if (N == 0) return;
            xb /= N;
            x2b /= N;
            yb /= N;
            xyb /= N;
            double a = (xyb - xb * yb) / (x2b - xb * xb);
            m_degNotch = -Math.Atan(a) * 180 / Math.PI;

            m_log.Add("Inspect Done " + m_nID.ToString("00") + " : " + sw.Check().ToString() + "ms, Size = " + m_nAlignSize.ToString() + ", Axis = " + m_posAxis.ToString() + ", Deg = " + m_degNotch.ToString("0.0000"));
        }

        void XErosion(int x, int y, int nCount)
        {
            if (x < nCount || x > m_img.m_szImg.x - nCount) return;
            byte nMin = 255;
            unsafe
            {
                fixed (byte* pDst = &m_aBuf[y, x])
                {
                    byte* pSrc = (byte*)m_img.GetIntPtr(y, x);
                    for (int n = -nCount; n <= nCount; n++)
                    {
                        if (nMin > *(pDst + n))
                        {
                            nMin = *(pDst + n);
                        }
                    }
                    *pSrc = nMin;
                }
            }
        }

        void CalcChipping()
        {
            int nChipping = m_data.m_nChipping + 100;
            int x = m_xROI[0] + m_data.c_nNotchSize;
            int x1 = m_xROI[1] - m_data.c_nNotchSize;
            CalcChipping(x, x1, nChipping); 
        }

        void CalcChipping(int x, int x1, int nChipping)
        {
            int xMax = 0, vMax = 0, nStart, nEnd;
            while ((x < x1) && (m_aCalc[x] >= nChipping)) x++;
            while ((x < x1) && (m_aCalc[x] < nChipping)) x++;
            nStart = x;
            while ((x < x1) && (m_aCalc[x] >= nChipping))
            {
                if (vMax < m_aCalc[x])
                {
                    xMax = x;
                    vMax = m_aCalc[x];
                }
                x++; 
            }
            nEnd = x;
            if (x < x1)
            {
                if (vMax > 0)
                {
                    Defect defect = new Defect();
                    defect.m_cp0 = new CPoint(nStart, m_aEdge[nStart]);
                    defect.m_cp1 = new CPoint(nEnd, m_aCalc[nEnd]);
                    defect.m_nSize = m_aCalc[xMax] - 100;
                    m_aChippingInfo.Add(defect);
                    //CPoint cp = new CPoint(xMax, m_aEdge[xMax]);
                    //m_aChipping.Add(cp);
                }
                CalcChipping(x, x1, nChipping); 
            }
        }

    }
}
