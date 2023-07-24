using System;
using System.Collections; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading; 
using System.Threading.Tasks;
using System.Net; 
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace ezTools
{
    public enum eTCPResult
    {
        OK,
        Error,
        Busy,
        Timeout,
        Disconnect,
    }

    public class ezTCPSocket
    {
        public delegate void MsgRcv(byte[] byteMsg, int nSize);
        public event MsgRcv CallMsgRcv;

        public string m_id, m_strIP = "128.0.0.0";
        string m_strWrite;
        int m_nPort = 7700;
        bool m_bRun = false;
        public bool m_bConnecting = false;
        Log m_log = null;
        Socket m_socket = null;
        Socket m_WorkingSocket = null;
        byte[] m_aWriteBuff = new byte[4096]; 
        byte[] m_aReadBuff = new byte[4096];
        Thread m_thread;
        bool m_bServerMode = true;

        public ezTCPSocket(string id, Log log, bool bServerMode = true)
        {
            m_id = id; m_log = log;
            m_bServerMode = bServerMode;
        }  

        public void Init()
        {
            if (m_bServerMode)
            {
                InitServer();
            }
            else
            {
                InitClient();
            }
        }

        public void InitClient()
        {
            if (m_socket != null) return;            
            m_thread = new Thread(new ThreadStart(RunThread)); m_thread.Start();
        }

        public void InitServer()
        {
            if (m_socket != null) return; 
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var v_ka = new keepAlive();
            v_ka.onoff = 1;
            v_ka.keepAliveTime = 2000;
            v_ka.keepAliveInterval = 1000;
            m_socket.IOControl(IOControlCode.KeepAliveValues, v_ka.buffer, null);
            m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            try
            {
                m_socket.Bind(new IPEndPoint(IPAddress.Any, m_nPort));
                m_socket.Listen(5);
                m_socket.ReceiveTimeout = 1;
                m_socket.BeginAccept(new AsyncCallback(CallBack_Accept), m_socket);
            }
            catch (SocketException eX) { m_log.Popup("InitServer : " + eX.Message); }
        }

        public void ThreadStop()
        {
            if (m_bRun) { m_bRun = false; m_thread.Join();  }
            if (m_socket != null && m_socket.Connected)
            {
                m_socket.Disconnect(true);
                m_socket.Dispose();
            }
        }

        void RunThread()
        {
            m_bRun = true; Thread.Sleep(5000);
            while (m_bRun)
            {
                Thread.Sleep(10);
                m_log.Notify(m_id + " Try Connect !!", !m_bConnecting && !IsConnect(), true); 
                if (!m_bConnecting && !IsConnect())
                {
                    Connect(); 
                    Thread.Sleep(1000);
                }
            }
        }

        public virtual void RunGrid(ezGrid rGrid, ezJob job = null)
        {
            if (m_bServerMode == false)
            {
                rGrid.Set(ref m_strIP, m_id, "IP", "ServerIP");                
            }
            rGrid.Set(ref m_nPort, m_id, "Port", "PortNumber");       
        }

        void Connect()
        {
            try 
            {
                m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var v_ka = new keepAlive();
                v_ka.onoff = 1;
                v_ka.keepAliveTime = 2000;
                v_ka.keepAliveInterval = 1000;
                m_socket.IOControl(IOControlCode.KeepAliveValues, v_ka.buffer, null);
                m_socket.BeginConnect(m_strIP, m_nPort, new AsyncCallback(CallBack_Connect), m_socket);
                m_bConnecting = true;
            }
            catch (SocketException eX) 
            { 
                m_log.Popup("Connect :" + eX.Message); 
            }
        }

        public bool IsConnect()
        {
            if (m_WorkingSocket == null)
            {
                if (m_log.m_logView.GetLastLog().IndexOf("not Connected") < 0)
                    m_log.Add("TCP/IP not Connected !!"); 
                return false;
            }
            return m_WorkingSocket.Connected; 
        }

        void CallBack_Connect(IAsyncResult ar)
        {
            try
            {
                if (((Socket)ar.AsyncState).Connected == false)
                {
                    m_bConnecting = false;
                    return;
                } 
                m_socket.EndConnect(ar);
                m_WorkingSocket = m_socket;
                m_socket.BeginReceive(m_aReadBuff, 0, m_aReadBuff.Length, SocketFlags.None, new AsyncCallback(CallBack_Receive), m_socket);
                m_bConnecting = true; 
                m_log.Add(m_id + " is Connect !!"); 
            }
            catch (SocketException eX) { m_log.Popup("CallBack_Connect : " + eX.Message); }
        }

        void CallBack_Accept(IAsyncResult ar)
        {
            try
            {
                m_socket = (Socket)ar.AsyncState;
                m_WorkingSocket = m_socket.EndAccept(ar);
                m_socket.BeginAccept(new AsyncCallback(CallBack_Accept), m_socket);
                m_WorkingSocket.BeginReceive(m_aReadBuff, 0, m_aReadBuff.Length, SocketFlags.None, new AsyncCallback(CallBack_Receive), m_WorkingSocket);
                m_log.Add(m_id + " is Connect !!",true); 
            }
            catch (SocketException eX) 
            {
                m_log.Popup("CallBack_Accept : " + eX.Message); 
            }
        }

        void CallBack_Receive(IAsyncResult ar)
        {
            //byte[] aReadBuff = new byte[4096];
            try
            {
                int nReadLength = m_WorkingSocket.EndReceive(ar);
                if (nReadLength > 0)
                {
                    m_WorkingSocket.BeginReceive(m_aReadBuff, 0, m_aReadBuff.Length, SocketFlags.None, new AsyncCallback(CallBack_Receive), m_WorkingSocket);
                    CallMsgRcv(m_aReadBuff, nReadLength);
                }
                else
                {
                    m_WorkingSocket.Close();
                    m_log.Add("CallBack_Receive Close");
                }
            }
            catch (Exception eX) 
            {
                m_log.Popup(eX.Message);
            }
        }

        void CallBack_Send(IAsyncResult ar)
        {
            int nWrite;
            m_WorkingSocket = (Socket)ar.AsyncState;
            nWrite = m_WorkingSocket.EndSend(ar);
        }

        public eTCPResult Send(byte[] buf, int nSize)
        {
            if (!IsConnect()) 
            { 
                m_log.Popup(m_id + " is not Connected !!"); 
                return eTCPResult.Disconnect; 
            }
            try
            {
                if (nSize == 0) return eTCPResult.OK;
                m_WorkingSocket.BeginSend(buf, 0, nSize, SocketFlags.None, new AsyncCallback(CallBack_Send), m_WorkingSocket);
            }
            catch (Exception eX) { m_log.Popup("Send : " + eX.Message); return eTCPResult.Error; }
            return eTCPResult.OK; 
        }

        public eTCPResult Send(string str)
        {
            m_strWrite = str;
            if (m_aWriteBuff.Length < m_strWrite.Length)
            {
                m_log.Popup("Write Buffer Length Error : " + m_aWriteBuff.Length.ToString() + " < " + m_strWrite.Length.ToString());
            }
            for (int i = 0; i < m_strWrite.Length; i++)// Convert the string to byte
            {
                m_aWriteBuff[i] = Convert.ToByte(m_strWrite[i]);
            }
            return Send(m_aWriteBuff, m_strWrite.Length); 
        }

        
    }
    struct keepAlive
    {
        public int onoff;
        public int keepAliveTime;
        public int keepAliveInterval;

        public unsafe byte[] buffer
        {
            get
            {
                var buff = new byte[sizeof(keepAlive)];
                fixed (void* p = &this) Marshal.Copy(new IntPtr(p), buff, 0, buff.Length);
                return buff;
            }
        }
    }
}
