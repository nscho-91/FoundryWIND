using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ezTools;
using SiSoFramegrabber;
using SisoSDKLib;
using PylonC.NET;

namespace ezCam
{
    public partial class ezCam_Silicon : ezCam_Mom
    {
        string m_id;
        Log m_log;

        public bool m_bDeviceOpenError = false;
        PYLON_DEVICE_HANDLE m_hDev = new PYLON_DEVICE_HANDLE();
        Framegrabber m_fgSISO = new Framegrabber();
        int m_nDeviceIndex = -1;
        string m_sMCF = "D:\\ATI\\optics.mcf"; 

        int m_lBufGrab = 10; 
        IntPtr m_pBufGrab = IntPtr.Zero;
        public CPoint m_szBuf = new CPoint(0, 0);

        public int m_nGrab = -1; 
        public int m_lGrab = 0; 

        unsafe public delegate void dgGrabDone(int nGrab, byte *pBuf);
        public event dgGrabDone m_dgGrabDone = null; 

        public ezCam_Silicon()
        {

        }

        public void Init(string id, LogView logView)
        {
            m_id = id;
            m_log = new Log(id, logView);
        }

        public void ThreadStop()
        {
            DeleteGrabberMem(); 
            if (m_fgSISO.FgFreeGrabber() != fgErrorType.fgeOK) m_log.Popup("FgFreeGrabber failed"); 
        }

        public void RunGrid(ezGrid rGrid, eGrid eMode)
        {
            rGrid.Set(ref m_nDeviceIndex, m_id, "DeviceIndex", "Device Index");
            rGrid.Set(ref m_sMCF, m_id, "ConfigFile", "Config File Name (mcf)");
            rGrid.Set(ref m_lBufGrab, m_id, "Buffer", "# of Grabber Buffer");
        }

        void AllocateGrabberMem(CPoint szBuf)
        {
            if (m_szBuf == szBuf) return;
            DeleteGrabberMem();
            ulong lSize = (ulong)(szBuf.x * szBuf.y * m_lBufGrab);
            m_pBufGrab = m_fgSISO.FgAllocMemEx(lSize, m_lBufGrab);
            m_szBuf = szBuf;
            if (m_pBufGrab == IntPtr.Zero) m_log.Popup("Allocate Grabber Mem Error"); 
        }

        fgErrorType DeleteGrabberMem()
        {
            if (m_pBufGrab != IntPtr.Zero)
            {
                fgErrorType fgError = m_fgSISO.FgFreeMemEx(m_pBufGrab);
                if (fgError != fgErrorType.fgeOK) m_log.Popup("Delete Grabber Mem Error : " + fgError.ToString()); 
            }
            m_pBufGrab = IntPtr.Zero;
            m_szBuf.x = m_szBuf.y = 0; 
            return fgErrorType.fgeOK; 
        }
        
        public void InitCamera()
        {
            if (m_nDeviceIndex < 0) return; 
            CPoint szBuf = new CPoint(0, 0);
#if DEBUG
            Environment.SetEnvironmentVariable("PYLON_GIGE_HEARTBEAT", "30000"); //ms
#endif
            try 
            { 
                Pylon.Initialize();
                uint numDevices = Pylon.EnumerateDevices();
                PylonCreateDevice();
                if (m_hDev == null)
                {
                    m_log.Popup(m_nDeviceIndex.ToString("DeviceID 0 not Found !!"));
                    return;
                }
                m_bDeviceOpenError = true;
                Pylon.DeviceOpen(m_hDev, Pylon.cPylonAccessModeControl | Pylon.cPylonAccessModeStream);
                m_bDeviceOpenError = false;
                Pylon.DeviceFeatureFromString(m_hDev, "UserSetSelector", "UserSet1");
                Pylon.DeviceExecuteCommandFeature(m_hDev, "UserSetLoad");

                NODEMAP_HANDLE hNodeMap = Pylon.DeviceGetNodeMap(m_hDev);
                NODE_HANDLE hNode = GenApi.NodeMapGetNode(hNodeMap, "Width");
                szBuf.x = (int)GenApi.IntegerGetValue(hNode);
                hNode = GenApi.NodeMapGetNode(hNodeMap, "Height");
                szBuf.y = (int)GenApi.IntegerGetValue(hNode); 
            }
            catch (Exception ex)
            {
                if (m_hDev != null)
                {
                    if (Pylon.DeviceIsOpen(m_hDev))
                    {
                        Pylon.DeviceClose(m_hDev);
                    }
                    Pylon.DestroyDevice(m_hDev);
                }
                m_log.Popup("Cam Init Fail !!");
                m_log.Add(ex.Message);
            }
            //forget File 있는지 확인
            fgErrorType ec = m_fgSISO.FgInitConfig(m_sMCF, (uint)m_nDeviceIndex);
            AllocateGrabberMem(szBuf); 
            //forget
            RegisterAPC(); 
        }

        void PylonCreateDevice()
        {
            if (m_nDeviceIndex >= 0) m_hDev = Pylon.CreateDeviceByIndex((uint)m_nDeviceIndex);
            if (m_nDeviceIndex < 0) m_log.Popup("Device Index not Defined !!");
            if (m_hDev == null) m_log.Popup("Device Create Error !!");
        }

        void RegisterAPC()
        {
            FgApcControlFlags flags = FgApcControlFlags.FG_APC_CONTROL_BASIC;
            FgApcControlCtrlFlags ctrlFlags = FgApcControlCtrlFlags.FG_APC_IGNORE_STOP | FgApcControlCtrlFlags.FG_APC_IGNORE_TIMEOUTS; 
            fgErrorType rc = m_fgSISO.FgRegisterAPCHandler(this, ApcEventHandler, (uint)m_nDeviceIndex, flags, ctrlFlags, m_pBufGrab);
        }

        unsafe public int ApcEventHandler(object sender, APCEvent ev)
        {
            m_nGrab = (int)ev.imageNo;
            IntPtr pBuf = m_fgSISO.FgGetImagePtrEx(ev.imageNo, (uint)m_nDeviceIndex, m_pBufGrab);
            m_dgGrabDone((int)ev.imageNo - 1, (byte*)pBuf.ToPointer()); //forget
            return 0;
        }

        public void StartGrab(long lGrab)
        {
            unsafe
            {
                m_nGrab = -1;
                m_lGrab = (int)lGrab; 
                int nFlag = (int)FgAcquisitionFlags.ACQ_STANDARD;
                fgErrorType rc = m_fgSISO.FgAcquireEx((uint)m_nDeviceIndex, lGrab, nFlag, m_pBufGrab);
                if (rc != fgErrorType.fgeOK) m_log.Popup("Start Grab Fail : " + rc.ToString()); 
            }
        }

        public void StopGrab()
        {
            fgErrorType rc = m_fgSISO.FgStopAcquireEx((uint)m_nDeviceIndex, m_pBufGrab, 0);
            if (rc != fgErrorType.fgeOK) m_log.Popup("Stop Grab Error : " + rc.ToString());
            m_nGrab = -1;
            m_lGrab = 0; 
        }
        
        public bool SetExposure(double ms)
        {
            return false;
        }

        public bool IsBusy()
        {
            return false;
        }

        public bool IsGrabDone()
        {
            return m_nGrab == m_lGrab;
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
        }

        public bool Reset()
        {
            return false;
        }

    }
}
