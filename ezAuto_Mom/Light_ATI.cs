using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CyUSB;
using ezAutoMom;
using ezTools; 

namespace ezAutoMom
{
    public class Light_ATI : Light_Mom
    {
        enum eError { eLED };

        const byte c_cCheckDevice = 0xff;

        Auto_Mom m_auto;
        Work_Mom m_work;
        XGem300_Mom m_xGem;
        USBDeviceList m_USBDevice;
        CyUSBDevice m_device;

        public Light_ATI()
        {
            
        }

        public void Init(string id, Auto_Mom auto)
        {
            m_auto = auto;
            base.Init(id, (object)auto); 
            m_work = m_auto.ClassWork();
            m_xGem = m_auto.ClassXGem();
            InitString(); OpenUSB();
        }

        public override void ThreadStop()
        {
            base.ThreadStop(); 
            if (m_USBDevice != null) m_USBDevice.Dispose();
        }

        void InitString()
        {
            InitString(eError.eLED, "USB LED Not Found"); 
        } 

        void InitString(eError eErr, string str)
        {
            m_log.AddString(str);
            if (m_xGem == null) return;
            m_xGem.AddALID(m_id, (int)eErr, str);
        } 

        void SetAlarm(eAlarm alarm, eError eErr)
        {
            m_work.SetError(alarm, m_log, (int)eErr);
            if (m_xGem == null) return;
            m_xGem.SetAlarm(m_id, (int)eErr);
        } 

        public override void ReConnect()
        {
	        m_device.Reset(); 
	        OpenUSB();
        }
        
        public override void LightOn(int nID, bool bLightOn)
        {
	        int n; 
	        if (nID < 0) for (n = 0; n < m_nCh; n++) LightOn(n, bLightOn);
	        base.LightOn(nID, bLightOn);
            try
            {
                ChangePower(nID, nID); // ing need test
            }
            catch
            {
                ChangePower(nID, 0); 
            }
        }

        public override void ChangePower(int nID, int nMode)
        {
	        int nCh; 
	        for (nCh = 0; nCh < m_nCh; nCh++) if (m_aSetup[nCh].m_bUse) 
	        {
	        	if (m_bLightOn) ChangeChannel(nCh, m_aPower[nMode,nCh]);
		        else ChangeChannel(nCh, 0); 
	        } 
        }       
        
        void OpenUSB() //kns
        {
            int n;
            for (n = 0; n < 100; n++)
            {
                try { m_USBDevice = new USBDeviceList(CyConst.DEVICES_CYUSB); }
                catch (Exception e)
                {
                    m_log.Popup("Usb Illi Device Not Found : " + e.ToString());
                    Thread.Sleep(50);
                }
                if (m_USBDevice != null) break;
            }
            if (m_USBDevice == null) { m_log.Popup("OpenLight - Device Not Found"); return; }
            m_device = m_USBDevice[0x0547, 0x1003] as CyUSBDevice;
            if (m_device == null) { m_log.Popup("OpenLight - Light Board Found"); return; }
            m_log.Add("OpenLight - OK");
	     /* int n, nFail = 0, PID; 
	        m_bPowerLED = bPowerLED; m_nUSB = m_device[0].DeviceCount(); m_nLED = 0; m_pDevice = NULL;
	        if (m_bPowerLED) PID = 0x1003; else PID = 0x1002;
	        for (n = 0; n<m_nUSB; n++) if (!m_device[n].Open((UCHAR)n))
	        {
	        	m_device[n].Reset(); m_device[n].ReConnect();
	        	if (!m_device[n].Open((UCHAR)n)) { nFail++; m_device[n].Close(); }
	        }
	        if (nFail) { m_log.Popup("USB LED Open Fail"); m_pDevice = NULL; return; }
	        else
	        {
		        for (n = 0; n<m_nUSB; n++) if ((m_device[n].VendorID == 0x0547) && (m_device[n].ProductID == PID)) { m_nLED++; m_pDevice = &m_device[n]; }
		        if (m_nLED == 0) { m_log.Popup("NotFound"); m_pDevice = NULL; SetAlarm(eAlarm.Stop, "NotFound"); return; }
		        if (m_nLED>1) { m_log.Popup("USB LED Found over 2"); m_pDevice = NULL; return; }
	        } */
        }
        
        byte ReadPort(ushort wIndex)  
        {
            CyControlEndPoint pEP; int nLength = 4;
            byte[] buf = new byte[4] { 0, 0, 0, 0 }; 
	        if (m_device == null) return 0; 
            pEP = m_device.ControlEndPt;
            pEP.Target = CyConst.TGT_DEVICE; 
            pEP.ReqType = CyConst.REQ_VENDOR; 
            pEP.Direction = CyConst.DIR_FROM_DEVICE;
	        pEP.ReqCode = 0x81; 
	        pEP.Value = 0; 
            pEP.Index = wIndex; 
            Thread.Sleep(1);       
	        if (pEP.XferData(ref buf, ref nLength)) return buf[0]; else return 0;
        }

        bool WritePort(ushort wIndex, ushort wValue)
        {
            CyControlEndPoint pEP; int nLength = 0;
            byte[] buf = new byte[4] { 0, 0, 0, 0 }; 
	        if (m_device == null) return false; 
            pEP = m_device.ControlEndPt;
            pEP.Target = CyConst.TGT_DEVICE; 
            pEP.ReqType = CyConst.REQ_VENDOR; 
            pEP.Direction = CyConst.DIR_TO_DEVICE;
	        pEP.ReqCode = 0x80; 
	        pEP.Value = wValue; 
            pEP.Index = wIndex;
            Thread.Sleep(1);
            try
            {
                return pEP.XferData(ref buf, ref nLength);
            }
            catch (Exception)
            {
                return false;
            }
        }

       bool ChangeChannel(int nCh, int nVal) 
        {
            if (m_device == null) return false;
            if (WritePort((ushort)nCh, (ushort)nVal))
            {
                //nRet = ReadPort((ushort)nCh); 
                //if (nRet != (byte)nVal) { str = "USB LED Run Fail :" + nRet.ToString(); m_log.Popup(str); return false; } 
            }
            return true;
        }

    }

}
