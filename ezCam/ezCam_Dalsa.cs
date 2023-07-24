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
using ezTools;
using DALSA.SaperaLT.SapClassBasic;
using System.Runtime.InteropServices;

namespace ezCam
{
    public enum eCam { eInit, eReady, eGrab, eDone };

    public partial class ezCam_Dalsa : Form, ezCam_Mom
    {
        public enum eRGB
        {
            B,
            G,
            R,
            Gray
        };
        eRGB m_eRGB = eRGB.Gray; 

        const int c_nBuf = 100;

        public IntPtr[] m_pSrc = new IntPtr[c_nBuf]; 
        
        string m_id;
        string m_strServer = "";
        string m_strFile = "";
        bool m_bRunThread = false;
        bool m_bInvY = false;
        eCam m_eCam = eCam.eInit;
        int m_nBuf = 10, m_msSnap;
        int m_nBlock = 135, m_iBlock = 0, m_iSnap = 0, m_nThres = -1;
        int m_nBlockStart = 0; 
        CPoint m_szCam = new CPoint(0, 0);
        CPoint m_szImg = new CPoint(0, 0); 
        ezStopWatch m_swSnap = new ezStopWatch(); 
        Log m_log;
        ezGrid m_grid;
        ezImg m_img; 
        Thread m_thread;
        SapAcquisition m_sapAcq = null;
        SapBuffer m_sapBuf = null;
        SapTransfer m_sapXfer = null;

        int m_yOffset = 0; 

        static void xfer_XferNotify(object sender, SapXferNotifyEventArgs args)
        {
            ezCam_Dalsa cam = args.Context as ezCam_Dalsa;
            cam.m_iSnap++; 
        }

        public ezCam_Dalsa()
        {
            InitializeComponent();
        }

        public void Init(string id, LogView logView)
        {
            m_id = id;
            m_log = new Log(m_id, logView);
            m_grid = new ezGrid(m_id, grid, m_log, false);
            RunGrid(eGrid.eRegRead); RunGrid(eGrid.eInit);
            InitCamera();
            m_thread = new Thread(new ThreadStart(RunThread)); m_thread.Start();
        }

        public void ThreadStop()
        {
            if (m_bRunThread) { m_bRunThread = false; m_thread.Join(); }
              DestroysObjects(null); 
        }

        void InitCamera()
        {
            if (m_strServer == "") return; 
            SapLocation loc = new SapLocation(m_strServer, 0);
            if (SapManager.GetResourceCount(m_strServer, SapManager.ResourceType.Acq) > 0)
            {
                m_sapAcq = new SapAcquisition(loc, m_strFile);
                m_sapBuf = new SapBuffer((int)m_nBuf, m_sapAcq, SapBuffer.MemoryType.ScatterGather);
                m_sapXfer = new SapAcqToBuf(m_sapAcq, m_sapBuf);
                if (!m_sapAcq.Create()) { loc.Dispose(); DestroysObjects("Error during SapAcquisition creation !!"); return; }
              //  m_sapAcq.EnableEvent(SapAcquisition.AcqEventType.StartOfFrame);

            }
            loc.Dispose();
            if (m_sapXfer == null) return; 
            m_sapXfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
            m_sapXfer.XferNotify += new SapXferNotifyHandler(xfer_XferNotify);
            m_sapXfer.XferNotifyContext = this;
            if (!m_sapBuf.Create()) { DestroysObjects("Error during SapBuffer creation !!"); return; }
            if (!m_sapXfer.Create()) { DestroysObjects("Error during SapTransfer creation !!"); return; }

        }

        void DestroysObjects(string strLog)
        {
            if (strLog != null) m_log.Popup(strLog);
            if (m_sapXfer != null) { m_sapXfer.Destroy(); m_sapXfer.Dispose(); }
            if (m_sapAcq != null) { m_sapAcq.Destroy(); m_sapAcq.Dispose(); }
            if (m_sapBuf != null) { m_sapBuf.Destroy(); m_sapBuf.Dispose(); }
        }

        public bool SetExposure(double ms)
        {
            return false; 
        }

        public bool IsBusy()
        {
            return m_eCam == eCam.eGrab;
        }

        public bool IsGrabDone()
        {
            if (m_eCam != eCam.eDone) return false;
            m_eCam = eCam.eReady; return true;
        }

        public bool SetROI(CPoint cp, CPoint sz)
        {
            return false;
        }

        public bool SetTrigger(bool bOn)
        {
            return false; 
        }

        public void GetszImage(ref CPoint szImg)
        {
            if (m_sapBuf == null) return;
            szImg.x = m_sapBuf.Width; szImg.y = m_sapBuf.Height * m_nBlock; 
        }

        public void GetszCam(ref CPoint szCam)
        {
            if (m_sapBuf == null) return;
            szCam.x = m_sapBuf.Width; szCam.y = m_sapBuf.Height; 
        }

        public bool Reset()
        {
            if (m_eCam == eCam.eGrab) { if (!m_sapXfer.Freeze()) return true; }
            m_eCam = eCam.eReady; return false; 
        }

        void RunGrid(eGrid eMode, ezJob job = null)
        {
            m_grid.Update(eMode, job);
            m_grid.Set(ref m_strServer, "Sapera", "Server", "Sapera Server Name");
            m_grid.Set(ref m_strFile, "Sapera", "File", "Sapera Config File Name");
            m_grid.Set(ref m_nBuf, "Sapera", "Buffer", "Sapera Buffer Count"); if (m_nBuf > c_nBuf) m_nBuf = c_nBuf;
            m_grid.Set(ref m_bInvY, "Image", "InvY", "Image Inverse Y");
            m_grid.Set(ref m_nBlock, "Image", "Block", "Camera Block Count");
            m_grid.Refresh();
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate); RunGrid(eGrid.eRegWrite); 
        }

        void RunThread()
        {
            m_bRunThread = true; Thread.Sleep(5000); 
            while (m_bRunThread)
            {
                Thread.Sleep(1);
                if (m_eCam == eCam.eGrab)
                {
                    if (m_eRGB != eRGB.Gray) ThreadSnapRGB(); 
                    else
                    {
                        if (m_nThres < 0) ThreadSnap();
                        else ThreadSnapThres();
                    }
                }
            }
        }

        void ThreadSnapRGB()
        {
            unsafe
            {
                byte* pBuf; byte* pSrc; byte* pDst; int y;
                m_swSnap.Start(); m_iBlock = -m_nBlockStart;
                if (m_sapBuf == null) { m_log.Popup("CamDalsa Buffer Error !!"); return; }
                pDst = (byte*)m_img.GetIntPtr(0, 0) + (int)m_eRGB;
                while (m_bRunThread && (m_eCam == eCam.eGrab) && (m_iBlock < m_nBlock) && (m_swSnap.Check() <= m_msSnap))
                {
                    if (WaitGrab()) return; 
                    pSrc = (byte*)m_pSrc[(m_iBlock + m_nBlockStart) % m_nBuf];
                    if (!m_bInvY)
                    {
                        pBuf = &pDst[3 * m_iBlock * m_szCam.y * m_szCam.x];
                        for (y = 0; y < m_szCam.y * m_szCam.x; y++)
                        {
                            *pBuf = *pSrc;
                            pBuf += 3; 
                            pSrc++;
                        }
                    }
                    else
                    {
                        pBuf = &pDst[3 * (m_nBlock - m_iBlock) * m_szCam.y * m_szCam.x]; pBuf -= 3 * m_szCam.x;
                        for (y = 0; y < m_szCam.y; y++)
                        {
                            for (int x = 0; x < m_szCam.x; x++)
                            {
                                *pBuf = *pSrc;
                                pBuf += 3;
                                pSrc++;
                            }
                            pBuf -= (6 * m_szCam.x);
                        }
                    }
                    m_iBlock++;
                    if (m_swSnap.Check() > m_msSnap) m_log.Add("Grab Time Over !! (" + m_iBlock.ToString() + "/" + m_iSnap.ToString() + ")");
                }
                m_iSnap = 0;
                m_eCam = eCam.eDone;
            }
            m_img.m_bNew = true;
        }

        void ThreadSnap()
        {
            int yp = 0; 
            unsafe
            {
                byte* pBuf; byte* pSrc; byte* pDst; int y;
                m_swSnap.Start(); m_iBlock = 0;
                if (m_sapBuf == null) { m_log.Popup("CamDalsa Buffer Error !!"); m_eCam = eCam.eInit; return; } // ing
                pDst = (byte*)m_img.GetIntPtr(0, 0);
                while (m_bRunThread && (m_eCam == eCam.eGrab) && (m_iBlock < m_nBlock) && (m_swSnap.Check() <= m_msSnap))
                {
                    if (WaitGrab()) return; 
                    pSrc = (byte*)m_pSrc[(m_iBlock + m_nBlockStart) % m_nBuf];
                    if (!m_bInvY) 
                    {
                        for (y = 0; y < m_szCam.y; y++)
                        {
                            yp = (m_iBlock * m_szCam.y + y + m_yOffset) % m_szImg.y; 
                            pBuf = &pDst[yp * m_szCam.x];
                            cpp_memcpy(pBuf, pSrc, m_szCam.x);
                            pSrc += m_szCam.x;
                        }
                    }
                    else
                    {
                        for (y = 0; y < m_szCam.y; y++)
                        {
                            yp = m_szImg.y - (m_iBlock * m_szCam.y + y + m_yOffset) % m_szImg.y - 1;
                            pBuf = &pDst[yp * m_szCam.x];
                            cpp_memcpy(pBuf, pSrc, m_szCam.x);
                            pSrc += m_szCam.x;
                        }
                    }
                    m_iBlock++;
                }
                m_iSnap = 0;
                m_eCam = eCam.eDone;
            }
            m_img.m_bNew = true;
        }

        void ThreadSnapThres()
        {
            unsafe
            {
                byte* pBuf; byte* pSrc; byte* pDst; int y;
                m_swSnap.Start(); m_iBlock = 0;
                if (m_sapBuf == null) { m_log.Popup("CamDalsa Buffer Error !!"); return; }
                pDst = (byte*)m_img.GetIntPtr(0, 0);
                while (m_bRunThread && (m_eCam == eCam.eGrab) && (m_iBlock < m_nBlock) && (m_swSnap.Check() <= m_msSnap))
                {
                    if (WaitGrab()) return; 
                    pSrc = (byte*)m_pSrc[(m_iBlock + m_nBlockStart) % m_nBuf];
                    if (!m_bInvY)
                    {
                        pBuf = &pDst[m_iBlock * m_szCam.y * m_szCam.x];
                        for (y = 0; y < m_szCam.y * m_szCam.x; y++)
                        {
                            if (*pSrc < m_nThres) *pBuf = 0;
                            else *pBuf = 255;
                            pBuf++; pSrc++;
                        }
                    }
                    else
                    {
                        pBuf = &pDst[(m_nBlock - m_iBlock) * m_szCam.y * m_szCam.x]; pBuf -= m_szCam.x;
                        for (y = 0; y < m_szCam.y; y++)
                        {
                            for (int x = 0; x < m_szCam.x; x++)
                            {
                                if (*pSrc < m_nThres) *pBuf = 0;
                                else *pBuf = 255;
                                pBuf++; pSrc++;
                            }
                            pBuf -= (2 * m_szCam.x);
                        }
                    }
                    m_iBlock++;
                    if (m_swSnap.Check() > m_msSnap) m_log.Add("Grab Time Over !! (" + m_iBlock.ToString() + "/" + m_iSnap.ToString() + ")");
                }
                m_iSnap = 0;
                m_eCam = eCam.eDone;
            }
            m_img.m_bNew = true;
        }

        bool WaitGrab()
        {
            while (m_bRunThread && (m_eCam == eCam.eGrab) && ((m_iBlock + m_nBlockStart) >= m_iSnap))
            {
                Thread.Sleep(1);
                if (m_swSnap.Check() > m_msSnap)
                {
                    m_log.Add("Grab Time Over !! (" + m_iBlock.ToString() + "/" + m_iSnap.ToString() + ")");
                    m_iSnap = 0;
                    m_eCam = eCam.eDone;
                    m_img.m_bNew = true;
                    return true;
                }
            }
            return false; 
        }

        public bool GrabShift(ezImg img, ref CPoint sz, int msSnap, int yOffset, int nBlockStart)
        {
            GetszImage(ref m_szImg);
            GetszCam(ref m_szCam);
            m_img = img;
            sz = m_szImg;
            m_nBlockStart = nBlockStart; 
            m_msSnap = msSnap;
            m_nThres = -1;
            m_yOffset = yOffset; 
            m_eRGB = eRGB.Gray;
            if (m_eCam == eCam.eGrab) { m_log.Popup("Camera Busy !!"); return true; }
            m_img.ReAllocate(m_szImg, 1); Snap();
            m_eCam = eCam.eGrab; return false;
        }

        public bool Grab(ezImg img, ref CPoint sz, int msSnap, int nThres, int nBlockStart)
        {
            GetszImage(ref m_szImg); 
            GetszCam(ref m_szCam);
            m_img = img; 
            sz = m_szImg;
            m_nBlockStart = nBlockStart; 
            m_msSnap = msSnap; 
            m_nThres = nThres;
            m_yOffset = 0; 
            m_eRGB = eRGB.Gray; 
            if (m_eCam == eCam.eGrab) { m_log.Popup("Camera Busy !!"); return true; }
            m_img.ReAllocate(m_szImg, 1); Snap();
            m_eCam = eCam.eGrab; return false;
        }

        public bool GrabRGB(ezImg img, ref CPoint sz, int msSnap, eRGB nRGB, int nBlockStart)
        {
            GetszImage(ref m_szImg); 
            GetszCam(ref m_szCam);
            m_img = img; 
            sz = m_szImg;
            m_nBlockStart = nBlockStart; 
            m_msSnap = msSnap;
            m_eRGB = nRGB;
            m_yOffset = 0; 
            if (m_eCam == eCam.eGrab) { m_log.Popup("Camera Busy !!"); return true; }
            m_img.ReAllocate(m_szImg, 3); 
            Snap();
            m_eCam = eCam.eGrab; 
            return false; 
        }

        void Snap()
        {
            if (m_sapBuf == null) { m_log.Add("Image buffer is not Ready"); return; }
            m_iBlock = -1;
            m_sapBuf.Index = (int)(m_nBuf - 1); m_sapXfer.Snap((int)(m_nBlock + m_nBlockStart));
            m_eCam = eCam.eGrab; GetBufAddress();
        }

        void GetBufAddress()
        {
            int n;
            for (n = 0; n < m_nBuf; n++) m_sapBuf.GetAddress(n, out m_pSrc[n]); 
        }

        public void ChangeMirror()  // ing for second scan 161010
        {
            m_bInvY = !m_bInvY;
        }

        //DllImport
        [DllImport("ezCpp.dll")]
        unsafe public static extern void cpp_memcpy(byte* pDst, byte* pSrc, int nLength);

        private void ezCam_Dalsa_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
