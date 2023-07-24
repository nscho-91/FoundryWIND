using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Drawing;
using Cognex.DataMan.SDK;
using Cognex.DataMan.SDK.Discovery;
using Cognex.DataMan.SDK.Utils;
using ezTools;

namespace ezCam
{
    public class ezCam_CognexBCR
    {
        bool m_bConnect = false;
        bool m_bReadSuccess = false;
        bool m_bTriggerOn = false;
        string m_id;
        string m_strPort = "COM3";
        string m_strIP = "192.0.0.0";
        string m_strDecode;
        int m_nType = 0; // 0 = None, 1 = USB or RS232, 2 = Ethernet
        Bitmap m_image;
        Log m_log;
        SerSystemDiscoverer m_serDiscoverer;
        SerSystemConnector m_serConnector;
        EthSystemDiscoverer m_ethDiscoverer;
        EthSystemConnector m_ethConnector;
//        SerSystemDiscoverer.SystemInfo m_serInfo;
//        EthSystemDiscoverer.SystemInfo m_ethInfo;
//        ResultCollector m_resultCollector;
        DataManSystem m_dmSystem;

        public ezCam_CognexBCR()
        {
        }

        public void Init(string id, Log log)
        {
            m_id = id;
            m_log = log;
            try
            {
                m_serDiscoverer = new SerSystemDiscoverer();
                m_serDiscoverer.SystemDiscovered += new SerSystemDiscoverer.SystemDiscoveredHandler(OnSerSystemDiscovered);
                m_ethDiscoverer = new EthSystemDiscoverer();
                m_ethDiscoverer.SystemDiscovered += new EthSystemDiscoverer.SystemDiscoveredHandler(OnEthSystemDiscovered);
                m_serDiscoverer.Discover();
                m_ethDiscoverer.Discover();
            }
            catch(Exception ex)
            {
                m_log.Add(ex.Message);
                return;
            }
        }

        public void ThreadStop()
        {
            m_serDiscoverer.Dispose();
        }

        bool Connect()
        {
            try
            {
                switch (m_nType)
                {
                    case 0:
                        break;
                    case 1:
                        m_serConnector = new SerSystemConnector(m_strPort);
                        m_dmSystem = new DataManSystem(m_serConnector);
                        m_dmSystem.DefaultTimeout = 5000;
                        m_dmSystem.SystemConnected += new SystemConnectedHandler(OnSystemConnected);
                        m_dmSystem.SystemDisconnected += new SystemDisconnectedHandler(OnSystemDisconnected);
                        m_dmSystem.XmlResultArrived += new XmlResultArrivedHandler(OnXmlResultArrived);
                        m_dmSystem.ImageArrived += new ImageArrivedHandler(OnImageArrived);
                        ResultTypes resultType1 = ResultTypes.ReadXml | ResultTypes.Image | ResultTypes.ImageGraphics;
                        //m_resultCollector = new ResultCollector(m_dmSystem, resultType1);
                        //m_resultCollector.ComplexResultArrived += new ComplexResultArrivedEventHandler(OnComplexResultArrived);
                        //m_resultCollector.PartialResultDropped += new PartialResultDroppedEventHandler(OnPartialResultDropped);
                        m_dmSystem.Connect();
                        m_dmSystem.SetResultTypes(resultType1);
                        break;
                    case 2:
                        m_ethConnector = new EthSystemConnector(System.Net.IPAddress.Parse(m_strIP));
                        m_dmSystem = new DataManSystem(m_ethConnector);
                        m_dmSystem.DefaultTimeout = 5000;
                        m_dmSystem.SystemConnected += new SystemConnectedHandler(OnSystemConnected);
                        m_dmSystem.SystemDisconnected += new SystemDisconnectedHandler(OnSystemDisconnected);
                        m_dmSystem.XmlResultArrived += new XmlResultArrivedHandler(OnXmlResultArrived);
                        m_dmSystem.ImageArrived += new ImageArrivedHandler(OnImageArrived);
                        ResultTypes resultType2 = ResultTypes.ReadXml | ResultTypes.Image | ResultTypes.ImageGraphics;
                        //m_resultCollector = new ResultCollector(m_dmSystem, resultType2);
                        //m_resultCollector.ComplexResultArrived += new ComplexResultArrivedEventHandler(OnComplexResultArrived);
                        //m_resultCollector.PartialResultDropped += new PartialResultDroppedEventHandler(OnPartialResultDropped);
                        m_dmSystem.Connect();
                        m_dmSystem.SetResultTypes(resultType2);
                        break;
                }
            }
            catch(Exception ex)
            {
                m_dmSystem.SystemConnected -= OnSystemConnected;
                m_dmSystem.SystemDisconnected -= OnSystemDisconnected;
                m_dmSystem.XmlResultArrived -= OnXmlResultArrived;
                m_dmSystem.ImageArrived -= OnImageArrived;
                m_log.Add(ex.Message);
                return false;
            }
            //m_serDiscoverer.SystemDiscovered -= OnSerSystemDiscovered;
            //m_ethDiscoverer.SystemDiscovered -= OnEthSystemDiscovered;
            return true;
        }

        void Disconnect()
        {

        }

        public void RunGrid(ezGrid rGrid, eGrid eMode)
        {
            rGrid.Set(ref m_nType, m_id, "Type", "0 = None, 1 = USB or RS232, 2 = Ethernet");
            rGrid.Set(ref m_strPort, m_id, "Port", "Port Number");
            rGrid.Set(ref m_strIP, m_id, "IP", "IP Number");
            rGrid.Set(ref m_bTriggerOn, m_id, "Trigger", "Trigger On");
            if (m_bTriggerOn && eMode == eGrid.eUpdate) Read();
        }

        void OnSerSystemDiscovered(SerSystemDiscoverer.SystemInfo systemInfo)
        {
            if (systemInfo.PortName == m_strPort && !m_bConnect)
            {
                m_bConnect = Connect();
            }
        }
        
        void OnEthSystemDiscovered(EthSystemDiscoverer.SystemInfo systemInfo)
        {
            if (systemInfo.IPAddress.ToString() == m_strIP && !m_bConnect)
            {
                m_bConnect = Connect();
            }
        }

        void OnSystemConnected(object sender, EventArgs args)
        {
            m_log.Add("Connected Successfully !!");
        }

        void OnSystemDisconnected(object sender, EventArgs args)
        {
            m_log.Add("Disconneted !!");
            try
            {
                switch (m_nType)
                {
                    case 0:
                        break;
                    case 1:
                        m_dmSystem.SystemConnected -= OnSystemConnected;
                        m_dmSystem.SystemDisconnected -= OnSystemDisconnected;
                        m_dmSystem.XmlResultArrived -= OnXmlResultArrived;
                        m_dmSystem.ImageArrived -= OnImageArrived;
                        //m_resultCollector.ComplexResultArrived -= OnComplexResultArrived;
                        //m_resultCollector.PartialResultDropped -= OnPartialResultDropped;
                        //m_resultCollector.Dispose();
                        m_dmSystem.Disconnect();
                        m_dmSystem.Dispose();
                        m_serConnector.Disconnect();
                        m_serConnector.Dispose();
                        break;
                    case 2:
                        m_dmSystem.SystemConnected -= OnSystemConnected;
                        m_dmSystem.SystemDisconnected -= OnSystemDisconnected;
                        m_dmSystem.XmlResultArrived -= OnXmlResultArrived;
                        m_dmSystem.ImageArrived -= OnImageArrived;
                        //m_resultCollector.ComplexResultArrived -= new ComplexResultArrivedEventHandler(OnComplexResultArrived);
                        //m_resultCollector.PartialResultDropped -= new PartialResultDroppedEventHandler(OnPartialResultDropped);
                        //m_resultCollector.Dispose();
                        m_dmSystem.Disconnect();
                        m_dmSystem.Dispose();
                        m_ethConnector.Disconnect();
                        m_ethConnector.Dispose();
                        break;
                }
            }
            catch (Exception ex)
            {
                m_log.Add(ex.Message);
            }
        }

        void OnXmlResultArrived(object sender, XmlResultArrivedEventArgs args)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(args.XmlResult);
                // ing 리딩 실패 확인 필요.
                XmlNode result = doc.SelectSingleNode("result/general/status");
                if (result == null || result.InnerText != "GOOD READ") return;
                XmlNode fullStringNode = doc.SelectSingleNode("result/general/full_string");
                if (fullStringNode == null) return;
                XmlAttribute encoding = fullStringNode.Attributes["encoding"];
                if (encoding != null && encoding.InnerText == "base64")
                {
                    byte[] code = Convert.FromBase64String(fullStringNode.InnerText);
                    m_strDecode = Encoding.ASCII.GetString(code);
                    m_bReadSuccess = true;
                }
            }
            catch(Exception ex)
            {
                m_log.Add(ex.Message);
                m_bReadSuccess = false;
            }
            
        }

        void OnImageArrived(object sender, ImageArrivedEventArgs args)
        {
            m_image = new Bitmap(args.Image);
            m_image.Save("D:\\BarcodeOrg.bmp");
        }

        //void OnComplexResultArrived(object sender, ResultInfo e)
        //{
            //m_strDecode = e.ReadString;
        //}

        //void OnPartialResultDropped(object sender, ResultInfo e)
        //{
            //m_strDecode = e.ReadString;
        //}

        public void Read()
        {
            m_image = null;
            m_bReadSuccess = false;
            try
            {
                m_dmSystem.SendCommand("TRIGGER ON");
            }
            catch
            {
                m_log.Add("Send Command Fail !!");
            }
        }

        public Bitmap GetImage()
        {
            if (m_image == null) m_image = new Bitmap(640, 480);
            return m_image;
        }

        public bool IsSuccessReading()
        {
            return m_bReadSuccess;
        }

        public string GetResult()
        {
            return m_strDecode;
        }
    }
}
