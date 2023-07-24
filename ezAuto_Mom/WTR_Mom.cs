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
using WeifenLuo.WinFormsUI.Docking;
using ezAutoMom;
using ezTools;

namespace ezAutoMom
{
    public partial class WTR_Mom : Form
    {
        public enum eState
        {
            Init,
            Home,
            Ready,
            Run,
            AutoUnload,
            Error,
        }
        eState m_eState = eState.Init;

        public enum eArm
        {
            Lower,
            Upper,
        }

        protected enum eMode
        {
            Standard,
            RS_1000
        }
        protected eMode m_eMode = eMode.Standard; 
        protected string[] m_sModes = Enum.GetNames(typeof(eMode));

        protected string m_id;
        protected string m_sError;
        protected Log m_log;
        protected ezGrid m_grid;
        protected Auto_Mom m_auto;
        protected Control_Mom m_control;
        protected XGem300Pro_Mom m_xGem;
        protected Work_Mom m_work;
        protected Handler_Mom m_handler;
        protected Recipe_Mom m_recipe = null;

        protected InfoWafer[] m_InfoWafer = new InfoWafer[2] { null, null };
        protected Wafer_Size[] m_wsArm = new Wafer_Size[2];
        protected eHWResult[] m_waferExist = new eHWResult[2] { eHWResult.Off, eHWResult.Off };       //Wafer Check {Lower Arm, Upper Arm}

        protected ArrayList m_aChild = new ArrayList();
        public ArrayList m_aWafer = new ArrayList();
        WTR_Mom_Wafers m_formWafer = new WTR_Mom_Wafers();

        protected bool[] m_bEnableArm = new bool[2] { true, true };

        public bool m_bSingleArm = false;
        protected eArm m_eSingleArm = eArm.Lower; 

        protected bool m_bRun = false;
        Thread m_thread;

        Size[] m_sz = new Size[2];

        public WTR_Mom()
        {
            InitializeComponent();
            m_sz[0] = m_sz[1] = this.Size;
            m_sz[0].Height = 26;
        }

        private void checkView_CheckedChanged(object sender, EventArgs e)
        {
            m_handler.ShowChild();
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            RunClick(); 
        }

        private void buttonGet_Click(object sender, EventArgs e)
        {
            eArm nArm = (eArm)comboArm.SelectedIndex; 
            WTR_Child child = GetChild((string)comboLocate.SelectedItem);
            if (child == null) return;
            string sLocate = child.GetID() + ',' + tbID.Text; 
            InfoWafer infoWafer = child.GetInfoWafer(child.GetID(sLocate));
            if (infoWafer == null) return; 
            infoWafer.ClearLocate();
            infoWafer.AddLocate(sLocate);
            infoWafer.AddLocate("WTR");
            int nPos = 0, nID = 0;
            if (child.IsGetOK(sLocate, ref infoWafer, ref nPos, ref nID) == false) 
            {
                m_log.Popup("Wafer Get not OK");
                return; 
            }
            RunGet(child, nArm, infoWafer, nPos, nID); 
        }

        private void buttonPut_Click(object sender, EventArgs e)
        {
            eArm nArm = (eArm)comboArm.SelectedIndex;
            WTR_Child child = GetChild((string)comboLocate.SelectedItem);
            if (child == null) return; 
            InfoWafer infoWafer = GetInfoWafer(nArm);
            if (infoWafer == null) return; 
            infoWafer.ClearLocate();
            infoWafer.AddLocate("WTR");
            infoWafer.AddLocate(child.GetID() + "," + tbID.Text); 
            if (infoWafer == null)
            {
                m_log.Popup("InfoWafer is null");
                return;
            }
            int nPos = 0;
            int nID = 0;
            if (!child.IsPutOK(child.GetID() + "," + tbID.Text, infoWafer, ref nPos, ref nID))
            {
                m_log.Popup("Wafer Put not OK");
                return;
            }
            RunPut(child, nArm, nPos, nID); 
        }

        private void buttonHome_Click(object sender, EventArgs e)
        {
            RunHome();
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate);
            RunGrid(eGrid.eRegWrite);
            if (m_grid.m_bInitGrid) RunGrid(eGrid.eInit);
        }

        public virtual void Init(string id, Auto_Mom auto)
        {
            m_id = id;
            m_auto = auto;
            m_sz[0] = m_sz[1] = this.Size;
            m_sz[0].Height = 26;
            m_log = new Log(m_id, m_auto.ClassLogView(), m_id);
            m_grid = new ezGrid(m_id, grid, m_log, false);
            m_formWafer.Init(m_id + "Wafer", auto, m_aWafer); 
            m_control = m_auto.ClassControl();
            m_xGem = m_auto.ClassXGemPro();
            m_work = m_auto.ClassWork();
            m_handler = m_auto.ClassHandler();
            m_recipe = m_auto.ClassRecipe();
            m_wsArm[0] = new Wafer_Size(m_log);
            m_wsArm[1] = new Wafer_Size(m_log);
            m_thread = new Thread(new ThreadStart(RunThread)); m_thread.Start();
            InitComboBox();
        }

        void InitComboBox()
        {
            comboArm.Items.Clear();
            comboArm.Items.Add(eArm.Lower.ToString());
            comboArm.Items.Add(eArm.Upper.ToString());
            comboArm.SelectedIndex = 0;

            comboLocate.Items.Clear();
            foreach (WTR_Child child in m_aChild) comboLocate.Items.Add(child.GetID());
            if (m_aChild.Count > 0)
            {
                if (comboLocate.SelectedIndex < 0) comboLocate.SelectedIndex = 0;
                if (comboLocate.SelectedIndex >= m_aChild.Count) comboLocate.SelectedIndex = 0;
            }

            tbID.Text = "1";
            comboLocate.SelectedIndex = 0;
        }

        protected eArm GetComboArm()
        {
            return (eArm)comboArm.SelectedIndex; 
        }

        public virtual void ThreadStop()
        {
            if (m_bRun)
            {
                m_bRun = false;
                m_thread.Join();
            }
        }

        public IDockContent GetContentFromPersistString(string persistString)
        {
            if (m_formWafer.IsPersistString(persistString)) return m_formWafer;
            return null;
        }

        public void ShowAll(DockPanel dockPanel)
        {
            m_formWafer.Show(dockPanel);
        }

        public void ShowDlg(Form parent, ref CPoint cpShow)
        {
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

        protected virtual void RunGrid(eGrid eMode)
        {
            m_grid.Set(ref m_bEnableArm[0], "Arm_Mode", "Enale_Lower", "Set Enable Arm Use");
            m_grid.Set(ref m_bEnableArm[1], "Arm_Mode", "Enale_Upper", "Set Enable Arm Use");
            int nSingleArm = (int)m_eSingleArm;
            m_grid.Set(ref nSingleArm, "Arm_Mode", "Single", "0 = Lower, 1 = Upper");
            m_eSingleArm = (eArm)nSingleArm; 
            RunGridArmEnable(m_grid, eMode, eArm.Lower, "LowerArm");
            RunGridArmEnable(m_grid, eMode, eArm.Upper, "UpperArm");
            foreach (WTR_Child child in m_aChild)
            {
                child.RunChildGrid(m_grid, eMode);
            }
        }

        void RunGridArmEnable(ezGrid rGrid, eGrid eMode, eArm eArmMode, string strGroup)
        {
            for (int n = 0; n < (int)Wafer_Size.eSize.Empty; n++)
            {
                string strSize = ((Wafer_Size.eSize)n).ToString();
                rGrid.Set(ref m_wsArm[(int)eArmMode].m_bEnable[n], strGroup, strSize, "Enable Whole Wafer Size");
            }
        }

        public void Reset()
        {
            if (m_eState == eState.Error) m_eState = eState.Init; 
        }

        public eState GetState()
        {
            return m_eState;
        }

        public InfoWafer GetInfoWafer(eArm nArm)
        {
            return m_InfoWafer[(int)nArm];
        }

        public virtual void SetInfoWafer(eArm nArm, InfoWafer infoWafer)
        {
            m_InfoWafer[(int)nArm] = infoWafer;
            if (infoWafer == null) return;
        }

        protected virtual void RunClick() { }
        public virtual eHWResult IsWaferExist(eArm nArm) { return eHWResult.Off; }
        protected virtual eHWResult RunGrip(eArm nArm, bool bGrip) { return eHWResult.Error; }
        protected virtual eHWResult RunHome() { return eHWResult.Error; }
        protected virtual void ErrorReset() { }
        protected virtual void ProcError() { }

        protected virtual eHWResult RunGet(WTR_Child child, eArm nArm, InfoWafer infoWaferChild, int nPos, int nID) { return eHWResult.Error; }
        protected virtual eHWResult RunPut(WTR_Child child, eArm nArm, int nPos, int nID) { return eHWResult.Error; }

        public eHWResult StartHome()
        {
            if ((m_eState == eState.Run) || (m_eState == eState.Error))
            {
                m_log.Popup("Can't Start Home When State is " + m_eState.ToString());
                return eHWResult.Error;
            }
            SetState(eState.Home);
            return eHWResult.OK;
        }

        protected void SetState(eState state)
        {
            if (state == m_eState) return;
            m_log.Add(m_id + " State : " + m_eState.ToString() + " -> " + state.ToString());
            m_eState = state;
        }

        void RunThread()
        {
            m_bRun = true;
            Thread.Sleep(10000);
            while (m_bRun)
            {
                Thread.Sleep(10);
                switch (m_eState)
                {
                    case eState.Init:
                        break;
                    case eState.Home:
                        RunHome(); 
                        break;
                    case eState.Ready:
                        if (m_work.GetState() == eWorkRun.Run) SetState(eState.Run);
                        break;
                    case eState.Run:
                        switch (m_eMode)
                        {
                            case eMode.RS_1000: StateRun_RS1000(); break;
                            default: StateRun(); break;
                        }
                        break;
                    case eState.AutoUnload: 
                        switch (m_eMode)
                        {
                            case eMode.RS_1000: StateRun_RS1000(); break;
                            default: StateRun(); break;
                        }
                        break;
                    case eState.Error:
                        ProcError();
                        break;
                }
            }
        }

        ezStopWatch m_swState = new ezStopWatch(); 

        void StateRun_RS1000()
        {
            if (!m_work.IsRun())
            {
                SetState(eState.Ready);
                return;
            }

            if (GetInfoWafer(eArm.Upper) != null)
            {
                if (GetInfoWafer(eArm.Lower) != null)
                {
                    StateRun(eArm.Lower);
                    return;
                }
                else
                {
                    StateRun(eArm.Upper);
                    return;
                }
            }
            else
            {
                if (GetInfoWafer(eArm.Lower) != null)
                {
                    if (StateRun_RS1000_Get(eArm.Upper)) return; 
                    else
                    {
                        StateRun(eArm.Lower);
                        return;
                    }
                }
                else
                {
                    if (StateRun_RS1000_Get(eArm.Lower)) return;
                    if (StateRun_RS1000_Get(eArm.Upper)) return; 
                }
            }
            if (m_swState.Check() > 2000) SetState(eState.Ready); 
            Thread.Sleep(450); 
        }

        bool StateRun_RS1000_Get(eArm nArm)
        {
            if (m_bEnableArm[(int)nArm] == false) return false;
            if (m_bSingleArm && (m_eSingleArm != nArm)) return false;
            foreach (InfoWafer infoWafer in m_aWafer)
            {
                WTR_Child childGet = GetChild(infoWafer.GetLocate(0));
                if (childGet == null) infoWafer.m_sWTR = nArm.ToString() + " Get : " + infoWafer.GetLocate(0);
                else
                {
                    InfoWafer infoWaferGet = infoWafer;
                    int nPos = 0, nID = 0;
                    if (childGet.IsGetOK(infoWaferGet.GetLocate(0), ref infoWaferGet, ref nPos, ref nID))
                    {
                        if (IsGetOK(nArm, infoWaferGet))
                        {
                            RunGet(childGet, nArm, infoWaferGet, nPos, nID);
                            infoWafer.m_sWTR = ""; 
                            return true;
                        }
                    }
                }
            }
            m_swState.Start(); 
            return false; 
        }

        void StateRun()
        {
            if (!m_work.IsRun())
            {
                SetState(eState.Ready);
                return;
            }

            if (StateRun(eArm.Lower)) return;
            if (StateRun(eArm.Upper)) return;

            foreach (InfoWafer infoWafer in m_aWafer)
            {
                WTR_Child childGet = GetChild(infoWafer.GetLocate(0));
                if (childGet == null) infoWafer.m_sWTR = "Get : " + infoWafer.GetLocate(0);
                else
                {
                    InfoWafer infoWaferGet = infoWafer;
                    int nPos = 0, nID = 0;
                    if (childGet.IsGetOK(infoWaferGet.GetLocate(0), ref infoWaferGet, ref nPos, ref nID))
                    {
                        if (IsGetOK(eArm.Upper, infoWaferGet))
                        {
                            RunGet(childGet, eArm.Upper, infoWaferGet, nPos, nID);
                            infoWafer.m_sWTR = "";
                            return;
                        }
                        if (IsGetOK(eArm.Lower, infoWaferGet))
                        {
                            RunGet(childGet, eArm.Lower, infoWaferGet, nPos, nID);
                            infoWafer.m_sWTR = "";
                            return;
                        }
                    }
                }
            }
            m_swState.Start(); 
        }

        bool StateRun(eArm nArm) 
        {
            InfoWafer infoWaferPut = GetInfoWafer(nArm);
            if (infoWaferPut == null) return false;
            WTR_Child child = GetChild(infoWaferPut.GetLocate(1));
            if (child == null)
            {
                infoWaferPut.m_sWTR = nArm.ToString() + " Put : " + infoWaferPut.GetLocate(1);
                m_log.Popup(infoWaferPut.m_sWTR);
                m_work.m_run.Stop(); 
                return false;
            }
            InfoWafer infoWaferGet = null;
            int nPos = 0, nID = 0; 
            if (child.IsGetOK(infoWaferPut.GetLocate(1), ref infoWaferGet, ref nPos, ref nID))
            {
                eArm nArmGet = (eArm)(1 - (int)nArm);
                if (RunGet(child, nArmGet, infoWaferGet, nPos, nID) != eHWResult.OK)
                {
                    infoWaferPut.m_sWTR = nArmGet.ToString() + " Get Error";
                    m_log.Popup(infoWaferPut.m_sWTR);
                    return false;
                }
            }
            if (child.IsPutOK(infoWaferPut.GetLocate(1), infoWaferPut, ref nPos, ref nID) == false)
            {
                infoWaferPut.m_sWTR = nArm.ToString() + " Put " + infoWaferPut.GetLocate(1) + " not Enable";
                m_log.Popup(infoWaferPut.m_sWTR);
                m_work.m_run.Stop(); 
                return false;
            }
            infoWaferPut.m_sWTR = ""; 
            return (RunPut(child, nArm, nPos, nID) == eHWResult.OK);
        }

        protected WTR_Child GetChild(string sLocate)
        {
            string[] sLocates = sLocate.Split(',');
            if (sLocates.Length <= 0) return null; 
            foreach (WTR_Child child in m_aChild)
            {
                if (child.GetID() == sLocates[0]) return child;
            }
            return null;
        }

        bool IsGetOK(eArm nArm, InfoWafer infoWafer)
        {
            if (m_bEnableArm[(int)nArm] == false) return false;
            if (m_bSingleArm && (m_eSingleArm != nArm)) return false;
            int nArmGet = (int)nArm;
            if (m_wsArm[nArmGet].m_bEnable[(int)infoWafer.m_wafer.m_eSize] == false)
            {
                infoWafer.m_sWTR = nArm.ToString() + " not Enable size : " + infoWafer.m_wafer.m_eSize.ToString(); 
                return false;
            }
            WTR_Child childNext = GetChild(infoWafer.GetLocate(2));
            if (childNext == null)
            {
                infoWafer.m_sWTR = nArm.ToString() + " Next : " + infoWafer.GetLocate(2); 
                return false;
            }
            int nPos = 0, nID = 0;
            if (childNext.IsPutOK(infoWafer.GetLocate(2), infoWafer, ref nPos, ref nID)) return true;
            InfoWafer infoWaferGet = null;
            if (childNext.IsGetOK(infoWafer.GetLocate(2), ref infoWaferGet, ref nPos, ref nID) == false)
            {
                infoWafer.m_sWTR = nArm.ToString() + " Next Get not OK";
                return false;
            }
            if (infoWaferGet == null)
            {
                infoWafer.m_sWTR = nArm.ToString() + " InfoWafer is null"; 
                return false;
            }
            if (m_wsArm[1 - nArmGet].m_bEnable[(int)infoWaferGet.m_wafer.m_eSize] == false)
            {
                infoWafer.m_sWTR = nArm.ToString() + " Another Arm not Enable size"; 
                return false;
            }
            return true;
        }

        public void Add(WTR_Child child)
        {
            m_aChild.Add(child);
        }

        public void Add(InfoWafer infoWafer)
        {
            m_aWafer.Add(infoWafer);
        }

        public void Delete(InfoWafer infoWafer)
        {
            int n = 0; 
            foreach (InfoWafer info in m_aWafer)
            {
                if (info.m_id == infoWafer.m_id)
                {
                    m_aWafer.RemoveAt(n);
                    return; 
                }
                n++; 
            }
        }

        public void RunCancel()
        {
            for (int n = m_aWafer.Count - 1; n >= 0; n--)
            {
                InfoWafer infoWafer = (InfoWafer)m_aWafer[n];
                m_aWafer.RemoveAt(n); 
            }
        }

        public InfoWafer FindWafer(string sChild)
        {
            foreach (InfoWafer infoWafer in m_aWafer)
            {
                if (infoWafer.HasChild(sChild)) return infoWafer; 
            }
            return null;
        }

    }
}
