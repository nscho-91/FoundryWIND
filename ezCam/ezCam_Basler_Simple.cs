using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ezTools;
using PylonC.NET;
using System.Runtime.InteropServices;

namespace ezCam
{
    public partial class ezCam_Basler_Simple : ezCam_Mom
    {
        string m_id;
        bool m_bOK = false;
        public bool m_bDeviceOpenError = false; 
        string m_sPixelFormat = "Mono8";
//        string[] m_sPixelFormats = { "Undefined", "Mono8", "Mono16", "RGB8packed", "RGBA8packed", "RGB16packed", "RGB8planar", "RGB16planar", "Double" };
        Log m_log;

        PYLON_DEVICE_HANDLE m_hDev = null;
        PylonBuffer<Byte> m_pylonBuf = null;
        int m_nDeviceIndex = -1;
        string m_strName = "Alinger";
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();            //190521 SDH ADD
        string[] m_strAcqModes = new string[2] { "Continuous", "SingleFrame" };
        string m_strAcqMode = "Continuous";

        string[] m_strTriggers = new string[2] { "Off", "On" };
        string m_strTrigger = "Off";

        int m_msGrabTimeout = 1000; 

        public ezCam_Basler_Simple()
        {
            timer.Tick += new EventHandler(timer_Tick);                      //190521 SDH ADD
            timer.Enabled = true;
            timer.Interval = 10000;
            timer.Start();
        }

        public void Init(string id, LogView logView)
        {
            m_id = id;
            m_log = new Log(id, logView);
        }
        
        public void InitCamera(int nDeviceIndex = -1)
        {
            if (nDeviceIndex >= 0) m_nDeviceIndex = nDeviceIndex; 
            try
            {
#if DEBUG
                Environment.SetEnvironmentVariable("PYLON_GIGE_HEARTBEAT", "30000"); //ms
#endif
                Pylon.Initialize();
                //uint numDevices = Pylon.EnumerateDevices();
                PylonCreateDevice(); 
                if (m_hDev == null)
                {
                    m_log.Popup(m_nDeviceIndex.ToString("DeviceID 0 not Found !!"));
                    return;
                }
                m_bDeviceOpenError = true; 
                Pylon.DeviceOpen(m_hDev, Pylon.cPylonAccessModeControl | Pylon.cPylonAccessModeStream);
                m_bDeviceOpenError = false; 
                if (Pylon.DeviceFeatureIsAvailable(m_hDev, "EnumEntry_PixelFormat_" + m_sPixelFormat))
                {
                    Pylon.DeviceFeatureFromString(m_hDev, "PixelFormat", "Mono8");
                }
                else m_log.Popup("FixelFormat_" + m_sPixelFormat + " not Available");

                bool isAvail = Pylon.DeviceFeatureIsAvailable(m_hDev, "EnumEntry_TriggerSelector_AcquisitionStart");
                if (isAvail)
                {
                    Pylon.DeviceFeatureFromString(m_hDev, "TriggerSelector", "AcquisitionStart");
                    Pylon.DeviceFeatureFromString(m_hDev, "TriggerMode", m_strTrigger);
                }

                isAvail = Pylon.DeviceFeatureIsAvailable(m_hDev, "EnumEntry_TriggerSelector_FrameBurstStart");
                if (isAvail)
                {
                    Pylon.DeviceFeatureFromString(m_hDev, "TriggerSelector", "FrameBurstStart");
                    Pylon.DeviceFeatureFromString(m_hDev, "TriggerMode", "Off");
                }

                isAvail = Pylon.DeviceFeatureIsAvailable(m_hDev, "EnumEntry_TriggerSelector_FrameStart");
                if (isAvail)
                {
                    Pylon.DeviceFeatureFromString(m_hDev, "TriggerSelector", "FrameStart");
                    Pylon.DeviceFeatureFromString(m_hDev, "TriggerMode", "Off");
                }

                Pylon.DeviceFeatureFromString(m_hDev, "AcquisitionMode", m_strAcqMode);
                if (Pylon.DeviceFeatureIsWritable(m_hDev, "GevSCPSPacketSize"))
                {
                    Pylon.DeviceSetIntegerFeature(m_hDev, "GevSCPSPacketSize", 1500);
                }
                m_bOK = true; 
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
        }
        private void timer_Tick(object sender, EventArgs e)             //190521 SDH ADD
        {
            if (m_hDev != null)
            {
                try
                {
                    if (!Pylon.DeviceIsOpen(m_hDev))
                    {
                        m_log.Add(" Cam Close,Check Cam State");
                    }
                }
                catch (Exception ex) { m_log.Add(ex.ToString()); }
            }

        }
        void PylonCreateDevice()
        {
            //if (m_nDeviceID >= 0) m_hDevice = Pylon.CreateDeviceFromDirectShowID(); //forget
            uint numDevices = Pylon.EnumerateDevices();
            if (m_nDeviceIndex == -1 && m_strName != "")
            {
                for (int i = 0; i < numDevices; i++)
                {
                    PYLON_DEVICE_INFO_HANDLE hDi = Pylon.GetDeviceInfoHandle((uint)i);
                    string Name = Pylon.DeviceInfoGetPropertyValueByName(hDi, Pylon.cPylonDeviceInfoUserDefinedNameKey);
                    if (Name == m_strName)
                    {
                        m_hDev = Pylon.CreateDeviceByIndex((uint)i);
                    }
                }
            }
            else if (m_nDeviceIndex >= 0) m_hDev = Pylon.CreateDeviceByIndex((uint)m_nDeviceIndex);
            if (m_nDeviceIndex < 0) m_log.Popup("Device Index not Defined !!");
            if (m_hDev == null) m_log.Popup("Device Create Error !!"); 
        }

        void PylonSetTrigger(string strMode, bool bOn)
        {
            string strOn;
            if (bOn) strOn = "On"; else strOn = "Off"; 
            if (!Pylon.DeviceFeatureIsAvailable(m_hDev, "EnumEntry_TriggerSelector_" + strMode)) return; 
            Pylon.DeviceFeatureFromString(m_hDev, "TriggerSelector", strMode);
            Pylon.DeviceFeatureFromString(m_hDev, "TriggerMode", strOn);
        }

        public void ThreadStop()
        {
            if (m_pylonBuf != null) m_pylonBuf.Dispose();
            if ((m_bDeviceOpenError == false) && (m_hDev != null) && (m_nDeviceIndex >= 0))
            {
                if (Pylon.DeviceIsOpen(m_hDev)) Pylon.DeviceClose(m_hDev);
                Pylon.DestroyDevice(m_hDev);
            }
            if (m_bOK) Pylon.Terminate(); 
        }

        public void RunGrid(ezGrid rGrid, eGrid eMode)
        {
            rGrid.Set(ref m_nDeviceIndex, m_id + "Basler", "DeviceIndex", "Device Index");
            if (m_nDeviceIndex == -1) rGrid.Set(ref m_strName, m_id + "Basler", "DeviceName", "Device Name");
            rGrid.Set(ref m_strAcqMode, m_strAcqModes, m_id + "Basler", "AcquisitionMode", "AcquisitionMode");
            rGrid.Set(ref m_strTrigger, m_strTriggers, m_id + "Basler", "Trigger", "Trigger");
            rGrid.Set(ref m_msGrabTimeout, m_id + "Basler", "GrabTimeout", "GrabTimeout (ms)"); 
//            rGrid.Set(ref m_sPixelFormat, m_sPixelFormats, m_id, "Format", "Pixel Format");
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
            return false;
        }

        public bool SetROI(CPoint cp, CPoint sz) //ing need Test
        {
            bool bError;
            if (m_hDev == null) return true;
            try
            {
                if (cp.x < 1 || cp.y < 1) return true;
                bError = SetIntValue("Width", sz.x);
                bError = SetIntValue("Height", sz.y);
                bError = SetIntValue("OffsetX", cp.x);
                bError = SetIntValue("OffsetY", cp.y);
            }
            catch (Exception ex)
            {
                m_log.Popup("Set ROI Size Fail !!");
                m_log.Add(ex.Message);
                return true;
            }
            Thread.Sleep(100);
            return bError;
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

        public bool Grab(ezImg img, bool bLog = true)
        {
            if (!m_bOK) return true; 
            ezStopWatch sw = new ezStopWatch(); 
            PylonGrabResult_t grabResult;
            if (!Pylon.DeviceGrabSingleFrame(m_hDev, 0, ref m_pylonBuf, out grabResult, (uint)m_msGrabTimeout))
            {
                m_log.Popup("Grab timeout.");
                return true;
            }
            if (grabResult.Status == EPylonGrabStatus.Failed)
            {
                m_log.Popup("Grab Fail : " + grabResult.ErrorCode.ToString());
                return true; 
            }
            if (grabResult.Status != EPylonGrabStatus.Grabbed)
            {
                m_log.Popup("Grab Error : " + grabResult.Status.ToString());
                return true; 
            }
            img.ReAllocate(new CPoint(grabResult.SizeX, grabResult.SizeY), 1);
            unsafe
            {
                byte* pSrc = (byte*)m_pylonBuf.Pointer;
                for (int y = 0; y < grabResult.SizeY; y++)
                {
                    fixed (byte* pDst = &img.m_aBuf[grabResult.SizeY - y - 1, 0])
                    {
                        cpp_memcpy(pDst, pSrc, grabResult.SizeX);
                    }
                    pSrc += grabResult.SizeX; 
                }
            }
            img.m_bNew = true; 
            if (bLog) m_log.Add(sw.Check().ToString("Grab Done : 0 ms")); 
            return false; 
        }

        public bool SetIntValue(string strFeature, int nVal)
        {
            NODEMAP_HANDLE hNodeMap;
            NODE_HANDLE hNode;
            long nMin, nMax;
            bool bVal;
            EGenApiNodeType nodeType;

            hNodeMap = Pylon.DeviceGetNodeMap(m_hDev);
            hNode = GenApi.NodeMapGetNode(hNodeMap, strFeature);
            if (!hNode.IsValid)
            {
                m_log.Popup("There is no feature named '" + strFeature + "' !!");
            }
            nodeType = GenApi.NodeGetType(hNode);
            if (EGenApiNodeType.IntegerNode != nodeType)
            {
                m_log.Popup("'" + strFeature + "' is not an integer feature.");
            }
            bVal = GenApi.NodeIsReadable(hNode);
            if (bVal)
            {
                nMin = GenApi.IntegerGetMin(hNode);
                nMax = GenApi.IntegerGetMax(hNode);
                bVal = GenApi.NodeIsWritable(hNode);
                if (nVal < nMin || nVal > nMax)
                {
                    m_log.Popup("The Range of " + strFeature + " is " + nMin.ToString() + " ~ " + nMax.ToString());
                }
                if (bVal)
                {
                    GenApi.IntegerSetValue(hNode, nVal);
                }
                else
                    m_log.Popup("Cannot set value for feature '" + strFeature + "' - node not writable.");
            }
            return false;
        }

        [DllImport("ezCpp.dll")]
        unsafe public static extern void cpp_memcpy(byte* pDst, byte* pSrc, int nLength);
    }
}
