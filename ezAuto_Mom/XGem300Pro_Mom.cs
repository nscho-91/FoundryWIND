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
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WeifenLuo.WinFormsUI.Docking;
using GEM_XGem300Pro;
using ezTools;
using System.IO;

namespace ezAutoMom
{
    public partial class XGem300Pro_Mom : DockContent
    {
        public enum eCommunicate
        {
            DISABLE = 1,
            WAITCR = 2,
            WAITDELAY = 3,
            WAITCRA = 4,
            COMMUNICATING = 5,
        }
        public eCommunicate m_eComm = eCommunicate.DISABLE;
        bool m_bXGemEnable = false;

        public enum eControl
        {
            OFFLINE = 1,
            ATTEMPTONLINE = 2,
            HOSTOFFLINE = 3,
            LOCAL = 4,
            ONLINEREMOTE = 5,
        }
        public eControl m_eControl = eControl.OFFLINE;

        public enum eXGemModule
        {
            INIT = 0,
            IDLE,
            SETUP,
            READY,
            EXCUTE,
        }
        public eXGemModule m_eXGemModule = eXGemModule.INIT;

        protected string m_id = "XGem";
        protected Auto_Mom m_auto;
        protected Log m_log;
        protected ezGrid m_grid;

        protected XGem300ProNet m_XGem300;
        string m_sCfgPath = "C:\\AVIS\\INIT\\Gem300.cfg";

        long[] m_aID = new long[1] { 0 };
        string[] m_aValue = new string[1] { "Value" };

        protected ArrayList m_aSV = new ArrayList();
        protected ArrayList m_aECV = new ArrayList();
        protected ArrayList m_aCEID = new ArrayList();
        ArrayList m_aALID = new ArrayList();

        int m_nLoadPort = 0;
        public InfoCarrier[] m_infoCarrier = new InfoCarrier[5];

        public bool m_bSendMap = false;
        public bool m_bSendCarrierID = false;

        int c_nTimeout = 5000;

        Size m_szGrid;

        public enum eALIDAdd
        {
            WorkRun = 1,
            Handler = 11,
            Vision = 21, 
            LoadPort = 31,
            WTR = 41,
            Aligner = 51,
            FDC = 61
        }

        public XGem300Pro_Mom()
        {
            InitializeComponent();
            m_szGrid = this.Size - grid.Size; 
        }

        private void XGem300Pro_Mom_Resize(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            Size sz = control.Size;
            if (control.Text == m_id) grid.Size = sz - m_szGrid;
        }

        private void checkControl_Enable_CheckedChanged(object sender, EventArgs e)
        {
            XGemEnable(!m_bXGemEnable);
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate, comboView.Text);
            RunGrid(eGrid.eRegWrite, comboView.Text);
        }

        private void comboView_SelectedIndexChanged(object sender, EventArgs e)
        {
            RunGrid(eGrid.eInit, comboView.Text);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            radioControl_Offline.Checked = (m_eControl == eControl.OFFLINE);
            radioControl_Local.Checked = (m_eControl == eControl.LOCAL);
            radioControl_Remote.Checked = (m_eControl == eControl.ONLINEREMOTE);
            radioXGem_Init.Checked = (m_eXGemModule == eXGemModule.READY);
            radioXGem_Start.Checked = (m_eXGemModule == eXGemModule.EXCUTE);
            radioXGem_Stop.Checked = (m_eXGemModule == eXGemModule.READY);
            radioXGem_Close.Checked = false;
            checkControl_Enable.Checked = (m_eComm == eCommunicate.COMMUNICATING);
        }

        protected virtual void TestRun() { }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            TestRun(); 
        }

        public virtual void Init(string id, Auto_Mom auto)
        {
            this.Text = m_id = id;
            m_auto = auto;
            m_nLoadPort = m_auto.ClassHandler().m_nLoadPort;
            m_log = new Log(m_id, m_auto.m_logView, "XGem");
            m_grid = new ezGrid(m_id, grid, m_log, false);
            RunGrid(eGrid.eRegRead, "Setup");
            ReadReg();
            ProcessKill("XGem");
            m_XGem300 = new XGem300ProNet();
            XGemEventHandle();
            XGemInitialize();
            XGemStart();
        }

        public virtual void ThreadStop()
        {
            if (m_XGem300 == null) return;
            m_XGem300.Stop();
            m_XGem300.Close();
        }

        public void ReadReg()
        {
            RunGrid(eGrid.eRegRead, "SV");
            RunGrid(eGrid.eRegRead, "CEID");
            RunGrid(eGrid.eRegRead, "ALID");
            comboView.Text = "SV";
            RunGrid(eGrid.eInit, comboView.Text);
        }

        void ProcessKill(string id)
        {
            Process[] ProcessList = Process.GetProcessesByName(id);
            foreach (Process process in ProcessList) process.Kill();
        }

        public virtual void ProcessEnd(string sJobID)
        {
            if (m_XGem300 == null) return; 
            long nError = m_XGem300.PJSetState(sJobID, (long)XGem300Process.eJobState.ProcessingComplete);
            LogXGem(nError, "PJSetState", sJobID, XGem300Process.eJobState.ProcessingComplete); 
        }

        void RunGrid(eGrid eMode, string sMode)
        {
            m_grid.Update(eMode, null);
            switch (sMode)
            {
                case "SV":
                    foreach (XGem300Data sv in m_aSV) m_grid.Set(ref sv.m_nID, "SV", sv.m_id, "SV");
                    break;
                case "CEID":
                    foreach (XGem300Data ceid in m_aCEID) m_grid.Set(ref ceid.m_nID, "CEID", ceid.m_id, "SV");
                    break;
                case "ALID":
                    foreach (XGem300ALID alid in m_aALID) m_grid.Set(ref alid.m_nALID, alid.m_id, alid.m_str, "ALID");
                    break;
                case "Setup":
                    m_grid.Set(ref m_sCfgPath, "Config", "Path", "Config File Path");
                    break;
            }
            m_grid.Refresh();
        }

        public virtual void XGemOffline()
        {
            if (m_XGem300 == null) return;
            m_auto.SetCSTLoadOK(0, false);
            m_auto.SetCSTLoadOK(1, false);
            LogXGem(m_XGem300.GEMReqOffline(), "GEMReqOffline"); 
        }

        public virtual void XGemOnlineLocal()
        {
            if (m_XGem300 == null) return;
            m_auto.SetCSTLoadOK(0, false);
            m_auto.SetCSTLoadOK(1, false);
            LogXGem(m_XGem300.GEMReqLocal(), "GEMReqLocal");
        }

        public virtual void XGemOnlineRemote()
        {
            if (m_XGem300 == null) return;
            m_auto.SetCSTLoadOK(0, false);
            m_auto.SetCSTLoadOK(1, false);
            LogXGem(m_XGem300.GEMReqRemote(), "GEMReqRemote");
        }

        bool XGemEnable(bool bEnable)
        {
            if (m_XGem300 == null) return true;
            long nError = m_XGem300.GEMSetEstablish(Convert.ToInt32(bEnable));
            LogXGem(nError, "GEMSetEstablish", bEnable);
            return (nError != 0); 
        }

        protected bool XGemInitialize()
        {
            long nError = m_XGem300.Initialize(m_sCfgPath);
            LogXGem(nError, "Initialize", m_sCfgPath);
            return (nError != 0);
        }

        protected bool XGemStart()
        {
            long nError = m_XGem300.Start();
            LogXGem(nError, "Start");
            return (nError != 0);
        }

        protected bool XGemStop()
        {
            long nError = m_XGem300.Stop();
            LogXGem(nError, "Stop");
            return (nError != 0);
        }

        protected bool XGemClose()
        {
            long nError = m_XGem300.Close();
            LogXGem(nError, "Close");
            return (nError != 0);
        }

        protected bool SetSV(XGem300Data sv, object value)
        {
            if (sv.m_nID < 0)
            {
                m_log.Popup("SV not Defined : " + sv.m_id);
                return true;
            }
            m_aID[0] = sv.m_nID;
            m_aValue[0] = value.ToString();
            long nError = m_XGem300.GEMSetVariable(1, m_aID, m_aValue);
            LogXGem(nError, "GEMSetVariable", sv.m_id, value);
            return (nError != 0);
        }

        protected bool GetSV(XGem300Data sv, ref string sValue)
        {
            m_aID[0] = sv.m_nID;
            m_aValue[0] = "";
            long nError = m_XGem300.GEMGetVariable(1, ref m_aID, ref m_aValue);
            sValue = m_aValue[0];
            LogXGem(nError, "GEMGetVariable", sv.m_id, sValue);
            return (nError != 0);
        }

        protected bool SetCEID(XGem300Data ceid)
        {
            long nCEID = (long)ceid.m_nID;
            if (nCEID < 0)
            {
                m_log.Popup("CEID not Defined : " + ceid.m_id);
                return true;
            }
            long nError = m_XGem300.GEMSetEvent(nCEID);
            LogXGem(nError, "GEMSetEvent", ceid.m_nID);
            return (nError != 0);
        }

        public void AddALID(string id, int nID, string str)
        {
            XGem300ALID alid = new XGem300ALID(id, nID, str);
            m_aALID.Add(alid);
        }

        public bool IsSetALID(int nALID)
        {
            foreach (XGem300ALID alid in m_aALID)
            {
                if (alid.m_nID == nALID) return alid.m_bSet;
            }
            return false;
        }

        public virtual void SetAlarm(string id, int nID)
        {
            foreach (XGem300ALID alid in m_aALID)
            {
                if ((alid.m_id == id) && (alid.m_nID == nID) && !alid.m_bSet)
                {
                    SetAlarm(alid, true);
                }
            }
        }

        public virtual void ResetAlarm(int nALID)
        {
            foreach (XGem300ALID alid in m_aALID)
            {
                if ((alid.m_nID == nALID) && alid.m_bSet)
                {
                    SetAlarm(alid, false);
                }
            }
        }

        public virtual void ClearAlarm()
        {
            foreach (XGem300ALID alid in m_aALID)
            {
                if (alid.m_bSet)
                {
                    SetAlarm(alid, false);
                }
            }
        }

        void SetAlarm(XGem300ALID alid, bool bSet)
        {
            if (alid.m_nALID < 0) return;
            alid.m_bSet = bSet;
            SetAlarmReport(alid);
            long nError = m_XGem300.GEMSetAlarm(alid.m_nALID, Convert.ToInt32(bSet));
            LogXGem(nError, "GEMSetAlarm", alid.m_nALID, bSet);
        }

        public virtual void SetAlarmReport(XGem300ALID alid) { }
        public virtual void SetInitData() { }

        public bool Is0nlineRemote()
        {
            return (m_eControl == eControl.ONLINEREMOTE);
        }

        public bool IsOnlineLocal()
        {
            return (m_eControl == eControl.LOCAL);
        }

        public bool IsOnline()
        {
            return (m_eControl == eControl.ONLINEREMOTE || m_eControl == eControl.LOCAL);
        }

        protected void XGemEventHandle()
        {
            this.m_XGem300.OnGEMCommStateChanged += new OnGEMCommStateChanged(OnGEMCommStateChanged);
            this.m_XGem300.OnGEMControlStateChanged += new OnGEMControlStateChanged(OnGEMControlStateChanged);
            this.m_XGem300.OnXGEMStateEvent += new OnXGEMStateEvent(OnXGEMStateEvent);
            this.m_XGem300.OnGEMTerminalMessage += new OnGEMTerminalMessage(OnGEMTerminalMessageRecieve);
            this.m_XGem300.OnGEMTerminalMultiMessage += new OnGEMTerminalMultiMessage(OnGEMTerminalMessageMultiRecieve);
            this.m_XGem300.OnGEMReqDateTime += new OnGEMReqDateTime(OnGEMReqDateTime);
            this.m_XGem300.OnGEMReqGetDateTime += new OnGEMReqGetDateTime(OnGemReqGetDateTime);
            this.m_XGem300.OnSECSMessageReceived += new OnSECSMessageReceived(OnSECSMessageReceived);
            this.m_XGem300.OnGEMReqRemoteCommand += new OnGEMReqRemoteCommand(OnGemReqRemoteCommand);
            this.m_XGem300.OnCMSCarrierIDStatusChanged += new OnCMSCarrierIDStatusChanged(OnCMSCarrierIDStatusChanged);
            this.m_XGem300.OnCMSSlotMapStatusChanged += new OnCMSSlotMapStatusChanged(OnCMSSlotMapStatusChanged);
            this.m_XGem300.OnCMSCarrierVerifySucceeded += new OnCMSCarrierVerifySucceeded(OnCMSCarrierVerifySucceeded);
            this.m_XGem300.OnCMSTransferStateChanged += new OnCMSTransferStateChanged(OnCMSTransferStateChanged);
            this.m_XGem300.OnPJReqVerify += new OnPJReqVerify(OnPJReqVerify);
            this.m_XGem300.OnPJCreated += new OnPJCreated(OnPJCreated);
            this.m_XGem300.OnCJCreated += new OnCJCreated(OnCJCreated);
            this.m_XGem300.OnPJDeleted += new OnPJDeleted(OnPJDeleted);
            this.m_XGem300.OnCJDeleted += new OnCJDeleted(OnCJDeleted);
            this.m_XGem300.OnPJStateChanged += new OnPJStateChanged(OnPJStateChanged);
            this.m_XGem300.OnCJStateChanged += new OnCJStateChanged(OnCJStateChanged);
            this.m_XGem300.OnPJReqCommand += new OnPJReqCommand(OnPJReqCommand);
        }

        void OnGEMCommStateChanged(long nState)
        {
            m_eComm = (eCommunicate)nState;
            m_log.Add("<- OnGEMCommStateChanged, " + nState.ToString() + ", " + m_eComm.ToString());
        }

        void OnGEMControlStateChanged(long nState)
        {
            m_eControl = (eControl)nState;
            m_log.Add("<- OnGEMControlStateChanged, " + nState.ToString() + ", " + m_eControl.ToString());
            if (m_eControl == eControl.ONLINEREMOTE) XGemOnlineRemote();
        }

        void OnXGEMStateEvent(long nState)
        {
            m_eXGemModule = (eXGemModule)nState;
            m_log.Add("<- OnXGEMStateEvent, " + nState.ToString() + ", " + m_eXGemModule.ToString());
            if (m_eXGemModule == eXGemModule.EXCUTE)
            {
                XGemEnable(true);
                XGemOnlineRemote();
                SetInitData();
            }
        }

        public virtual void OnTerminalMessage(long nTid, string sMsg) 
        {
            m_log.Popup("<- OnGEMTerminalMessageRecieve : " + nTid.ToString() + ", " + sMsg);
        }

        void OnGEMTerminalMessageRecieve(long nTid, string sMsg)
        {
            OnTerminalMessage(nTid, sMsg); 
        }

        void OnGEMTerminalMessageMultiRecieve(long nTid, long nCount, string[] sMsg)
        {
            for (int n = 0; n < nCount; n++) OnTerminalMessage(nTid, sMsg[n]); 
        }

        void OnGEMReqDateTime(long nMsgld, string sSystemTime)
        {
            m_log.Popup("<- OnGEMReqDateTime, " + nMsgld.ToString() + ", " + sSystemTime);
            long nResult = 1;
            if (sSystemTime.Length == 14)
            {
                ezDateTime dt = new ezDateTime();
                dt.m_sTime.wYear = Convert.ToUInt16(sSystemTime.Substring(0, 4));
                dt.m_sTime.wMonth = Convert.ToUInt16(sSystemTime.Substring(4, 2));
                dt.m_sTime.wDay = Convert.ToUInt16(sSystemTime.Substring(6, 2));
                dt.m_sTime.wHour = Convert.ToUInt16(sSystemTime.Substring(8, 2));
                dt.m_sTime.wMinute = Convert.ToUInt16(sSystemTime.Substring(10, 2));
                dt.m_sTime.wSecond = Convert.ToUInt16(sSystemTime.Substring(12, 2));
                if (dt.SetSystemTime() != 0) nResult = 0;
            }
            long nError = m_XGem300.GEMRspDateTime(nMsgld, nResult);
            LogXGem(nError, "GEMRspDateTime", nMsgld, nResult);
        }

        void OnGemReqGetDateTime(long nMsgID)
        {
            m_log.Popup("<- OnGemReqGetDateTime, " + nMsgID.ToString());
            string sTime = System.DateTime.Now.ToString("yyyyMMddhhmmss");
            long nError = m_XGem300.GEMRspGetDateTime(nMsgID, sTime);
            LogXGem(nError, "GEMRspGetDateTime", nMsgID, sTime);
        }

        protected virtual void SECSMessageReceived(long nObjectID, long nStream, long nFunction, long nSysbyte) { }
        
        void OnSECSMessageReceived(long nObjectID, long nStream, long nFunction, long nSysbyte)
        {
            m_log.Popup("<- OnSECSMessageReceived, " + nObjectID.ToString() + ", " + nStream.ToString() + ", " + nFunction.ToString() + ", " + nSysbyte.ToString());
            SECSMessageReceived(nObjectID, nStream, nFunction, nSysbyte);
        }

        protected virtual void GemRemoteCommand(long nMsgID, string sRcmd, long nCount, string[] sCpNames, string[] sCpVals, ref long[] nCpAcks) { }
        
        void OnGemReqRemoteCommand(long nMsgID, string sRcmd, long nCount, string[] psCpName, string[] psCpVal)
        {
            if (psCpName.Length < nCount)
            {
                m_log.Popup("<- OnGemReqRemoteCommand, psCpName Count Invalid (" + psCpName.Length.ToString() + ", " + nCount.ToString() + ")");
                nCount = psCpName.Length;
            }
            if (psCpVal.Length < nCount)
            {
                m_log.Popup("<- OnGemReqRemoteCommand, psCpVal Count Invalid (" + psCpVal.Length.ToString() + ", " + nCount.ToString() + ")");
                nCount = psCpVal.Length;
            }
            long nHCAck = 0;
            long[] pnCpAck = new long[nCount];
            m_log.Add("<- OnGemReqRemoteCommand, " + nMsgID.ToString() + ", " + sRcmd);
            for (int n = 0; n < nCount; n++)
            {
                m_log.Add("            " + psCpName[n] + ", " + psCpVal[n]);
                pnCpAck[n] = 0;
            }
            GemRemoteCommand(nMsgID, sRcmd, nCount, psCpName, psCpVal, ref pnCpAck);
            long nError = m_XGem300.GEMRspRemoteCommand(nMsgID, sRcmd, nHCAck, nCount, psCpName, pnCpAck);
            LogXGem(nError, "GEMRspRemoteCommand", nMsgID, sRcmd, nHCAck);
        }

        private void OnCMSCarrierIDStatusChanged(string sLocID, long nState, string sCarrierID)
        {
            InfoCarrier.eXGemState eState = (InfoCarrier.eXGemState)nState;
            m_log.Add("<- OnCMSCarrierIDStatusChanged, " + sLocID + ", " + eState.ToString() + ", " + sCarrierID); 
            for (int n = 0; n < m_nLoadPort; n++)
            {
                if (m_infoCarrier[n].m_id == sLocID)
                {
                    m_infoCarrier[n].m_eXGemState = eState;
                    m_infoCarrier[n].m_sXGemCarrierID = sCarrierID;
                    m_log.Add("OnCMSCarrierIDStatusChanged OK, LocID = " + sLocID); 
                    return; 
                }
            }
            m_log.Popup("OnCMSCarrierIDStatusChanged Fail, sLocID = " + sLocID);
        }

        private void OnCMSSlotMapStatusChanged(string sLocID, long nState, string sCarrierID)
        {
            InfoCarrier.eXGemState eState = (InfoCarrier.eXGemState)nState;
            m_log.Add("<- OnCMSSlotMapStatusChanged, " + sLocID + ", " + eState.ToString() + ", " + sCarrierID);
            for (int n = 0; n < m_nLoadPort; n++)
            {
                if (m_infoCarrier[n].m_id == sLocID)
                {
                    m_infoCarrier[n].m_eXGemStateSlotMap = eState;
                    m_infoCarrier[n].m_sXGemCarrierID = sCarrierID;
                    m_log.Add("OnCMSSlotMapStatusChanged OK, LocID = " + sLocID); 
                }
            }
            m_log.Popup("OnCMSSlotMapStatusChanged Fail, sLocID = " + sLocID);
        }

        protected virtual void RunPJReq(InfoCarrier infoCarrier) { }

        private void OnPJReqVerify(long nMsgID, long nPJobCount, string[] psPJobID, long[] pnMtrlFormat, long[] pnAutoStart, long[] pnMtrlOrder, long[] pnMtrlCount, string[] psMtrlID, string[] psSlotInfo, long[] pnRcpMethod, string[] psRcpID, long[] pnRcpParCount, string[] psRcpParName, string[] psRcpParValue)
        {
            m_log.Add("<- OnPJReqVerify, " + nMsgID.ToString() + ", " + nPJobCount.ToString() + ", " + psPJobID[0] + ", " + pnMtrlFormat[0].ToString());
            if (nPJobCount != 1)
            {
                PJRspVerify(nMsgID, nPJobCount, psPJobID, 1, (long)(XGem300Process.eError.Unsupported_Option), "Can not Support Multi PJOB Create"); 
                return; 
            }
            InfoCarrier infoCarrier = GetInfoCarrier(psMtrlID[0]); 
            if (infoCarrier == null)
            {
                PJRspVerify(nMsgID, nPJobCount, psPJobID, 1, (long)(XGem300Process.eError.Invalid_AttibuteValue), "Matrial ID not Found");
                return; 
            }
            Recipe_Mom recipe = m_auto.ClassRecipe();
            if (recipe.IsRecipeExist(psRcpID[0]) == false)
            {
                PJRspVerify(nMsgID, nPJobCount, psPJobID, 1, (long)(XGem300Process.eError.Invalid_AttibuteValue), "Recipe Does Not Exist In EQ");
                return; 
            }
            infoCarrier.m_xGemProcess.m_eAutoStart = (XGem300Process.eFlag)pnAutoStart[0];
            infoCarrier.m_xGemProcess.m_sJobID = psPJobID[0];
            infoCarrier.m_xGemProcess.m_sRecipeID = psRcpID[0];
            string sSlotInfo = "";
            for (int i = 0; i < psSlotInfo.Length; i++) sSlotInfo += psSlotInfo[i];
            infoCarrier.m_sXGemSlotMap = sSlotInfo;
            PJRspVerify(nMsgID, nPJobCount, psPJobID, 0, (long)(XGem300Process.eError.NO_ERROR), "OnPJReqVerify Done");
            RunPJReq(infoCarrier); 
        }

        void PJRspVerify(long nMsgID, long nPJobCount, string[] psPJobID, long nAck, long nErrorCode, string sError)
        {
            long[] aErrorCode = new long[1] { nErrorCode };
            string[] asError = new string[1] { sError };
            long nError = m_XGem300.PJRspVerify(nMsgID, nPJobCount, psPJobID, nAck, 1, aErrorCode, asError);
            LogXGem(nError, "PJRspVerify", nMsgID, nPJobCount, nAck);
        }

        InfoCarrier GetInfoCarrier(string sLocID)
        {
            for (int n = 0; n < m_nLoadPort; n++)
            {
                if (m_infoCarrier[n].m_id == sLocID) return m_infoCarrier[n]; 
            }
            return null; 
        }

        private void OnPJCreated(string sPJobID, long nMtrlFormat, long nAutoStart, long nMtrlOrder, long nMtrlCount, string[] psMtrlID, string[] psSlotInfo, long nRcpMethod, string sRcpID, long nRcpParCount, string[] psRcpParName, string[] psRcpParValue)
        {
            m_log.Add("<- OnPJCreated, " + sPJobID + ", " + psMtrlID[0] + ", " + sRcpID);
        }

        private void OnCJCreated(string sCJobID, long nStartMethod, long nCountPRJob, string[] psPRJobID)
        {
            m_log.Add("<- OnCJCreated, " + sCJobID + ", " + psPRJobID[0]);
            for (int n = 0; n < m_nLoadPort; n++)
            {
                if (m_infoCarrier[n].m_xGemProcess.m_sJobID == psPRJobID[0])
                {
                    m_infoCarrier[n].m_xGemControl.m_sJobID = sCJobID;
                    m_log.Add("OnCJCreated, " + sCJobID + ", " + m_infoCarrier[n].m_id); 
                }
            }
        }

        private void OnPJStateChanged(string sPJobID, long nState)
        {
            XGem300Process.eJobState eState = (XGem300Process.eJobState)nState;
            m_log.Add("<- OnPJStateChanged, " + sPJobID + ", " + eState.ToString());
            InfoCarrier infoCarrier = GetInfoCarrierJob(sPJobID);
            if (infoCarrier == null)
            {
                m_log.Popup("OnPJStateChanged : Can't Find JobID = " + sPJobID); 
                return;
            }
            m_log.Add("OnPJStateChanged ( " + infoCarrier.m_xGemProcess.m_eJobState.ToString() + " -> " + eState.ToString() + " )");
            infoCarrier.m_xGemProcess.m_eJobState = eState; 
            if (infoCarrier.m_xGemProcess.m_eJobState == XGem300Process.eJobState.SettingUp)
            {
                long nError = m_XGem300.PJSettingUpCompt(infoCarrier.m_xGemProcess.m_sJobID);
                LogXGem(nError, "PJSettingUpCompt", infoCarrier.m_xGemProcess.m_sJobID);
            }
        }

        InfoCarrier GetInfoCarrierJob(string sJobID)
        {
            for (int n = 0; n < m_nLoadPort; n++)
            {
                if (m_infoCarrier[n].m_xGemProcess.m_sJobID == sJobID) return m_infoCarrier[n];
            }
            return null;
        }

        private void OnCJStateChanged(string sCJobID, long nState)
        {
            long nError = 0; 
            XGem300Control.eState eState = (XGem300Control.eState)nState;
            m_log.Add("<- OnCJStateChanged, " + sCJobID + ", " + eState.ToString());
            InfoCarrier infoCarrier = GetInfoCarrierControl(sCJobID); 
            if (infoCarrier == null)
            {
                m_log.Popup("OnCJStateChanged : Can't Find JobID = " + sCJobID);
                return;
            }
            m_log.Add("OnCJStateChanged [ " + infoCarrier.m_xGemControl.m_eState + "  ->  " + eState.ToString() + " ]");
            infoCarrier.m_xGemControl.m_eState = eState;
            switch (infoCarrier.m_xGemControl.m_eState)
            {
                case XGem300Control.eState.Queued:
                    nError = m_XGem300.CJReqSelect(sCJobID);
                    LogXGem(nError, "CJReqSelect", sCJobID);
                    break;
                case XGem300Control.eState.Excuting:
                    nError = m_XGem300.PJSettingUpStart(infoCarrier.m_xGemProcess.m_sJobID);
                    LogXGem(nError, "PJSettingUpStart", infoCarrier.m_xGemProcess.m_sJobID);
                    break;
            }
        }

        InfoCarrier GetInfoCarrierControl(string sJobID)
        {
            for (int n = 0; n < m_nLoadPort; n++)
            {
                if (m_infoCarrier[n].m_xGemControl.m_sJobID == sJobID) return m_infoCarrier[n];
            }
            return null;
        }

        private void OnPJDeleted(string sPJobID)
        {
            m_log.Add("<- OnPJDeleted, " + sPJobID);
        }

        private void OnCJDeleted(string sCJobID)
        {
            m_log.Add("<- OnCJDeleted, " + sCJobID);
            InfoCarrier infoCarrier = GetInfoCarrierControl(sCJobID);
            if (infoCarrier == null)
            {
                m_log.Popup("OnCJDeleted Error : " + sCJobID);
                return; 
            }
            long nError = m_XGem300.CMSSetCarrierAccessing(infoCarrier.m_id, (long)InfoCarrier.eXGemAccess.CarrierCompleted, infoCarrier.m_sXGemCarrierID);
            LogXGem(nError, "CMSSetCarrierAccessing", infoCarrier.m_id, InfoCarrier.eXGemAccess.CarrierCompleted, infoCarrier.m_sXGemCarrierID);
        }

        public void CMSSetReadyToLoad(InfoCarrier infoCarrier)
        {
            long nError = m_XGem300.CMSSetReadyToLoad(infoCarrier.m_id);
            LogXGem(nError, "CMSSetReadyToLoad", infoCarrier.m_id);
        }

        public void CMSSetReadyToUnload(InfoCarrier infoCarrier)
        {
            long nError = m_XGem300.CMSSetReadyToUnload(infoCarrier.m_id);
            LogXGem(nError, "CMSSetReadyToUnload", infoCarrier.m_id);
        }

        private void OnPJReqCommand(long nMsgId, string sPJobID, long nCommand) 
        {
            XGem300Process.eCommand eCommand = (XGem300Process.eCommand)nCommand; 
            m_log.Add("<- OnPJReqCommand, " + sPJobID + ", " + eCommand.ToString()); 
            InfoCarrier infoCarrier = GetInfoCarrierJob(sPJobID);
            if (infoCarrier == null)
            {
                m_log.Popup("OnPJReqCommand : Can't Find JobID = " + sPJobID);
                PJRspCommand(nMsgId, eCommand, sPJobID, 0, 1, 1, "PRJOB ID Match Fail"); 
                return;
            }
            XGem300Process.eJobState ePJStateChange = XGem300Process.eJobState.Queued;
            switch ((XGem300Process.eCommand)nCommand)
            {
                case XGem300Process.eCommand.START: ePJStateChange = XGem300Process.eJobState.Processing; break;
                case XGem300Process.eCommand.STOP: break;
                case XGem300Process.eCommand.RESUME: break;
                case XGem300Process.eCommand.PAUSE: break;
                case XGem300Process.eCommand.CANCEL: break;
                case XGem300Process.eCommand.ABORT: break;
            }
            PJRspCommand(nMsgId, eCommand, sPJobID, 1, 0, 0, ""); 
            if (ePJStateChange != XGem300Process.eJobState.Queued)
            {
                long nError = m_XGem300.PJSetState(infoCarrier.m_xGemProcess.m_sJobID, (long)ePJStateChange);
                LogXGem(nError, "PJSetState", infoCarrier.m_xGemProcess.m_sJobID, ePJStateChange);
            }
        }

        void PJRspCommand(long nMsgId, XGem300Process.eCommand eCommand, string sPJobID, long nAck, long nErrorCount, long nErrorCode, string sError)
        {
            long[] aErrorCode = new long[1] { nErrorCode };
            string[] asError = new string[1] { sError };
            long nError = m_XGem300.PJRspCommand(nMsgId, (long)eCommand, sPJobID, nAck, nErrorCount, aErrorCode, asError);
            LogXGem(nError, "PJRspCommand", nMsgId, eCommand, sPJobID, nAck, nErrorCount, aErrorCode[0], asError[0]);
        }

        public virtual bool SetSlotMap(WTR_Child child, InfoCarrier infoCarrier)
        {
            return CMSSetSlotMap(infoCarrier); 
        }

        protected virtual bool CMSSetSlotMap(InfoCarrier infoCarrier)
        {
            if (infoCarrier == null)
            {
                m_log.Popup("CMSSetSlotMap : InfoCarrier is null");
                return true;
            }
            string sLocID = infoCarrier.m_id;
            string sCarrierID = infoCarrier.m_sXGemCarrierID;

            string sSlotMap = "";
            for (int n = 0; n < 25; n++) sSlotMap += GetSlotMapChar(infoCarrier.m_infoWafer[n]);

            ezStopWatch sw = new ezStopWatch();
            m_bSendMap = true; 
            long nError = m_XGem300.CMSSetSlotMap(sLocID, sSlotMap, sCarrierID, 0);
            LogXGem(nError, "CMSSetSlotMap", sLocID, sSlotMap, sCarrierID, 0);
            while (m_bSendMap && (sw.Check() < c_nTimeout)) Thread.Sleep(10);
            if (m_bSendMap)
            {
                m_log.Popup("CMSSetSlotMap Responce Error !!");
                return true;
            }
            m_log.Add("CMSSetSlotMap Done"); 
            return false; 
        }

        char GetSlotMapChar(InfoWafer infoWafer)
        {
            if (infoWafer.GetState() == InfoWafer.eState.Exist) return '3';
            return '1'; 
        }

        public bool CMSSetCarrierID(InfoCarrier infoCarrier, string sCarrierID)
        {
            if (infoCarrier == null)
            {
                m_log.Popup("CMSSetCarrierID : InfoCarrier is null"); 
                return true;
            }
            long nError = m_XGem300.CMSSetCarrierIDStatus(sCarrierID, (long)XGem300Carrier.eCarrierState.WaitForHost);
            LogXGem(nError, "CMSSetCarrierIDStatus", sCarrierID, XGem300Carrier.eCarrierState.WaitForHost);
            infoCarrier.m_sXGemCarrierID = sCarrierID;
            nError = m_XGem300.CMSSetCarrierID(infoCarrier.m_id, sCarrierID, 0);
            LogXGem(nError, "CMSSetCarrierID", infoCarrier.m_id, sCarrierID, 0);
            m_bSendCarrierID = true;
            ezStopWatch sw = new ezStopWatch();
            while (m_bSendCarrierID && (sw.Check() < c_nTimeout)) Thread.Sleep(10);
            if (m_bSendCarrierID)
            {
                m_log.Popup("CMSSetCarrierID Responce Error !!");
                return true; 
            }
            nError = m_XGem300.CMSSetCarrierAccessing(infoCarrier.m_id, (long)InfoCarrier.eXGemAccess.InAccessed, sCarrierID);
            LogXGem(nError, "CMSSetCarrierAccessing", infoCarrier.m_id, InfoCarrier.eXGemAccess.InAccessed, sCarrierID);
            return false; 
        }

        private void OnCMSCarrierVerifySucceeded(long nVerifyType, string sLocID, string sCarrierID, string sSlotMap, long nCount, string[] psLotID, string[] psSubstrateID, string sUsage)
        {
            m_log.Add("<- OnCMSCarrierVerifySucceeded, " + nVerifyType.ToString() + ", " + sLocID + ", " + sCarrierID + ", " + sSlotMap); 
            if (nVerifyType == 1) // Slotmap Verification
            {
                InfoCarrier infoCarrier = GetInfoCarrier(sLocID); 
                if (infoCarrier == null)
                {
                    m_log.Popup("OnCMSCarrierVerifySucceeded : InfoCarrier is null");
                    return;
                }
                char[] aSlotMap = sSlotMap.ToCharArray();
                int nIndex = 0;
                for (int n = 0; n < 25; n++)
                {
                    if (aSlotMap[n] == '3')
                    {
                        if (nIndex == nCount) return;
                        infoCarrier.m_infoWafer[nIndex].m_sWaferID = psSubstrateID[nIndex];
                        nIndex++;
                    }
                }
                m_bSendMap = false;
                return; 
            }
            if (nVerifyType == 0) // CarrierID Verification
            {
                m_bSendCarrierID = false; 
                return; 
            }
        }

        public void SetCMSPresentSensor(InfoCarrier infoCarrier, bool bSensorOn)
        {
            if (infoCarrier == null)
            {
                m_log.Popup("SetLPPresentSensor : InfoCarrier is null");
                return;
            }
            long nError = m_XGem300.CMSSetPresenceSensor(infoCarrier.m_id, Convert.ToInt32(bSensorOn));
            LogXGem(nError, "CMSSetPresenceSensor", infoCarrier.m_id, bSensorOn);
        }

        public void CMSSetCarrierOnOff(InfoCarrier infoCarrier, bool bCarrierOn)
        {
            if (infoCarrier == null)
            {
                m_log.Popup("CMSSetCarrierOnOff : InfoCarrier is null");
                return;
            }
            long nError = m_XGem300.CMSSetCarrierOnOff(infoCarrier.m_id, Convert.ToInt32(bCarrierOn));
            LogXGem(nError, "CMSSetCarrierOnOff", infoCarrier.m_id, bCarrierOn);
        }

        private void OnCMSTransferStateChanged(string sLocID, long nState)
        {
            InfoCarrier.eXGemTransfer eState = (InfoCarrier.eXGemTransfer)nState;
            m_log.Add("<- OnCMSTransferStateChanged, " + sLocID + ", " + eState.ToString()); 
            for (int i = 0; i < m_nLoadPort; i++)
            {
                if (m_infoCarrier[i].m_id == sLocID)
                {
                    m_infoCarrier[i].m_eXTranfer = eState;
                    m_log.Popup("OnCMSTransferStateChanged < " + m_infoCarrier[i].m_eXTranfer.ToString() + " -> " + eState.ToString() + " >");
                    return; 
                }
            }
            m_log.Popup("OnCMSTransferStateChanged Fail, " + sLocID + ", " + eState.ToString());
        }

        void LogXGem(long nError, string sCmd, params object[] objs)
        {
            string sLog = " -> " + sCmd;
            foreach (object obj in objs) sLog += ", " + obj.ToString();
            m_log.Add(sLog);
            if (nError >= 0) return;
            m_log.Popup(sCmd + " (" + nError.ToString() + ") " + GetErrerString(nError));
        }


        string GetErrerString(long nError)
        {
            switch (nError)
            {
                case 0: return "No Error";
                case -10001: return "Already Started";              // Already initialized 이미 XGem300Pro 이 초기화되어 있는데 다시 불린 상태입니다.
                case -10002: return "Invalid Thread";               // Socket is not initialized.
                case -10003: return "Invalid DomDoc";               // Fail to load xml. XGem300Pro Process 로 부터 xml format이 아닌 message를 받았을 때 발생합니다.
                case -10004: return "Invalid Attribute";            // Fail to invalid attribute. XGem300Pro Process 로부터 받은 message의 attribute가 맞지 않을 경우 발생합니다.
                case -10005: return "Invalid Command";              // Fail to invalid command. XGem300Pro Process 로부터 invalid message를 받았을 경우 발생합니다.
                case -10006: return "Create Failed DomDoc";         // Fail to initiate DomDocument Dom 생성 실패 시 발생합니다.
                case -10007: return "Invalid Argument Value";       // Invalid argument value. 함수의 인자의 값이 유효하지 않은 값 입니다. 인자의 값을 확인하시기 바랍니다.
                case -10008: return "Not Ready XGem";               // Not ready XGem. XGem300Pro state 가 execute 가 아닌 상태에서 함수가 호출되었습니다.
                case -10009: return "Not Initialized";              // Not initialized yet. XGem300Pro 이 아직 초기화 되어 있지 않은 상태입니다. Initialize() 함수가 실패했거나 불리지 않은 상태입니다.
                case -10010: return "Not Started";                  // Not started yet. XGem300Pro 의 내부 프로세스가 아직 시작되지 않은 상태입니다. Start()가 호출되지 않았거나 실패한 상태입니다.
                case -10011: return "Not Connected";                // Not used.
                case -10012: return "Read Config File Error";       // Fail to read config file. cfg 파일을 읽기 실패하였습니다. cfg 파일의 경로와 item 을 확인하기 바랍니다.
                case -10013: return "Invalid Message Format";       // Invalid message format. Complex  type 의  message 를XGem300Pro Process 로 전송 시 발생하며 invalid message format 을 전송하려고 할 때 발생합니다.
                case -10014: return "Delete Complex List Error";    // Not used.
                case -10015: return "Item not Found";               // Not found item. data item 을 찾을 수 없습니다.
                case -10016: return "Item Type Mismatch";           // Mismatch item type 얻고자 하는 data type과 맞지 않습니다.
                case -10017: return "Item Count Mismatch";          // Mismatch item count. item 의 개수가 맞지 않습니다.
                case -10018: return "Invalid Message ID";           // Invalid message.
                case -10019: return "Argument Out of Range";        // Argument is out of range 인자 값의 범위를 벗어 납니다.
                case -10020: return "Invalid Parameter";            // Invalid Parameter.유효하지 않은 Parameter name
                case -10021: return "License Error";                // Not used.
                case -10022: return "Fail to Create Window";        // Fail to create window. window 생성 실패했습니다. 회사로 문의 바랍니다.
                case -10023: return "Invalid Receive Data";         // Not used.
                case -10024: return "Mismatch Message Name";        // Mismatch message name. 얻고자 하는 message name 과 XGem300Pro Process 에서 받은 message name 과 동일하지 않습니다.
                case -10026: return "VID does not use file memory"; // VID does not use file memory.
                case -10027: return "VID file memory is included";  // VID for using file memory is included.
                case -10028: return "Invalid VID";                  // invalid VID 유효하지 않은 VID.
                case -10029: return "Exceed Maximum Item Size";     // exceed maximum item size.
                case -10030: return "Fail to Create Mutex";         // Fail to create mutex.
                case -10031: return "Invalid SECS2 Message";        // invalid SECS2 message. SECS2 message format error.
                case -10032: return "Fail to Delete MSG List";      // Fail to delete msg list
                case -10033: return "Create Event Handle Error";    // Fail to create API’s event handle.
                case -10034: return "Fail to Start Process";        // Fail to start XGem300Pro process.
                case -30001: return "Fail to Read Rtartup Info";    // Fail to read startup information
                case -30002: return "Fail to Initialize XCom";      // Fail to initialize XCom
                case -30003: return "Fail to Initialize EQComm";    // Fail to initialize EQComm
                case -30004: return "Fail to Initialize EDAComm";   // Fail to initialize EDAComm
                case -30005: return "Fail to Start XCom";           // Fail to start XCom
                case -30006: return "Fail to Start EQComm";         // Fail to start EQComm
                case -30007: return "Fail to Start EDAComm";        // Fail to start EDAComm
                case -30008: return "Disconnected with Equipment";  // disconnected with Equipment
                case -30009: return "Disconnected with EDAComm";
                case -30010: return "Control Off Line";             // Can't send message because controloffline.
                case -30011: return "Control State not Chamged";    // Control state can't be changed to ONLINE_LOCAL in OFFLINE state.
                case -30012: return "File Create Error";            // cfg, sml file not created
                case -30013: return "Fail to Open Database";        // Fail to open database
                case -30014: return "Error in Execute SQL";         // Error in Execute SQL.
                case -30015: return "Error in Open SQL";            // Error in Open SQL.
                case -30016: return "CEID disabled";                // Ceid disabled
                case -30017: return "Report Buffer is Full";        // Report buffer is full.
                case -30018: return "System does not Use Spooling"; // This system does not use spooling now.
                case -30019: return "Spool Buffer is Full";         // Spool buffer is full.
                case -30020: return "Spool Undefined";              // Spool undefined
                case -30021: return "Spool is not Active";          // Spool is not active, so message cannot spooling.
                case -30022: return "Operating to 200mm spec";      // XGem300Pro is operating to 200mm spec, so message of 300mm spec can't send.
                case -30023: return "File does not Exist";          // File does not exist.
                case -30024: return "Shared Memory Start Error";    // Failed to open/start shared memory.
                case -30025: return "Shared Memory Stop Error";     // Failed to close/stop shared memory.
                case -30201: return "Fail Create DomDoc2";
                case -30202: return "Invalid DomDoc2";
                case -30203: return "Invalid Command2";
                case -30204: return "Invalid Attribute2";
                case -30205: return "Invalid Set Command";
                case -30251: return "VID does not Exist";           // VID does not exist.
                case -30252: return "ALID does not Exist";          // ALID does not exist.
                case -30253: return "CEID does not Exist";          // CEID does not exist.
                case -30254: return "RPTID does not Exist";         // RPTID does not exist.
                case -30255: return "Limit VID does not Exist";     // Limit VID does not exist.
                case -30256: return "Limit ID does not Exist";      // Limit ID does not exist.
                case -30257: return "Data Item does not Exist";     // Data Item does not exist.
                case -30258: return "RCmd does not Exist";          // RCmd does not exist.
                case -30259: return "Stream does not Exist";        // Stream does not exist.
                case -30260: return "Function does not Exist";      // Function does not exist.
                case -30261: return "SECS Param does not Exist";    // SECSParameters does not exist.
                case -30262: return "Invalid Error Code";
                case -30263: return "ECID does not Exist";          // ECID does not exist.
                case -30264: return "Format is Invalid";            // Format is invalid.
                case -30265: return "ConfigItem does not Exist";    // ConfigItem does not exist.
                case -30266: return "Invalid Structure";
                case -30267: return "PORTID does not Exist";        // PORTID does not exist.
                case -30268: return "Invalid EQ MSG ID";
                case -30269: return "Data Type is Invalid";         // Data Type is invalid.
                case -30270: return "PPID does not Exist";          // PPID does not exist.
                case -30271: return "Invalid State";                // State is invalid.
                case -30272: return "TRID does not Exist";          // TRID does not exist
                case -30273: return "Buffer does not Found";        // Buffer does not found.
                case -30274: return "Value is out of Range";        // Value is out of range.
                case -30275: return "SVID does not Exist";          // SVID does not exist.
                default: return "Unknown Error"; 
            }
        }

    }
}
