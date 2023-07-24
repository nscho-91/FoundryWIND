using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ezAutoMom;
using ezTools;

namespace ezAutoMom
{
    public partial class HW_ImageVS_Mom : Form
    {

        public string m_id;
        public bool m_bUse = false;
        protected Auto_Mom m_auto;
        Size[] m_sz = new Size[2];
        protected Log m_log;
        protected ezGrid m_grid;
        protected Work_Mom m_work;
        protected Control_Mom m_control;
        protected Handler_Mom m_handler;
        protected XGem300_Mom m_xGem;
        protected Info_Wafer m_infoWafer = null;
        protected bool m_bRunThread = false;
        Thread m_thread;
        protected ezTCPSocket m_socket;

        public HW_ImageVS_Mom()
        {
            InitializeComponent();
            m_sz[0] = m_sz[1] = this.Size;
            m_sz[0].Height = 26;
        }

        public virtual void Init(string id, Auto_Mom auto)
        {
            m_id = id;
            m_auto = auto;
            checkView.Text = m_id;
            m_log = new Log(m_id, m_auto.m_logView, "ImageVS");
            m_grid = new ezGrid(m_id, grid, m_log, false);
            m_work = m_auto.ClassWork();
            m_control = m_auto.ClassControl();
            m_handler = m_auto.ClassHandler();
            m_xGem = m_auto.ClassXGem();
            m_socket = new ezTCPSocket(m_id, m_log, true);

            RunGrid(eGrid.eRegRead);
            RunGrid(eGrid.eInit);
            m_socket.Init();
            m_socket.CallMsgRcv += CallMsgRcv;

            m_thread = new Thread(new ThreadStart(RunThread));
            m_thread.Start();
        }

        protected virtual bool SendSocket(string str)
        {
            if (m_socket.Send(str) == eTCPResult.OK) return true;
            else return false;
        }

        protected virtual void CallMsgRcv(byte[] byteMsg, int nSize)
        {
            //  throw new NotImplementedException();          
        }

        public void ThreadStop()
        {
            m_socket.ThreadStop();
            if (m_bRunThread)
            {
                m_bRunThread = false;
                m_thread.Join();
            }
        }

        private void checkView_CheckedChanged(object sender, EventArgs e)
        {
            m_handler.ShowChild();
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate);
            RunGrid(eGrid.eRegWrite);
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

        public virtual void ModelGrid(ezGrid rGrid, eGrid eMode)
        {
        }

        protected virtual void RunGrid(ezTools.eGrid eMode)
        {
        }

        protected virtual void RunThread() { }
        public virtual bool IsConnected() { return m_socket.IsConnect(); } 
        public virtual bool IsImageVSEnable() { return false; }
    }
}
