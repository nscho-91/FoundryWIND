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
using System.IO;
using ezAxis;
using ezAutoMom;
using ezTools;

namespace ezAutoMom
{
    public partial class HW_Thick6 : Form
    {
        const int c_nThickData = 100;

        string[] m_strSteps = new string[4] { "Update", "Single", "Buffer", "Buffer3" };
        string m_id, m_strRun;
        bool[] m_bSave = new bool[2] { false, false };
	    bool m_bCommEco = false;
	    char[] m_aBuf = new char[1024];
	    int m_nAdd, m_msBuffer, m_nSmooth, m_nError, m_msWait;
        int[] m_nStrip = new int[2];
	    double[,] m_aLData = new double[7,c_nThickData];
        double[,] m_aLBoat = new double[7,c_nThickData];
        double[] m_aLGet = new double[c_nThickData];
	    double[,] m_aAxis = new double[2,3];
        double m_fV, m_fThick, m_dThick;
        public bool m_bEnable;
        Size[] m_sz = new Size[2];

	    Auto_Mom m_auto;
	    Work_Mom m_work;
        ezRS232 m_rs232;
	    ezStopWatch m_sw = new ezStopWatch();
	    Log m_log;
	    ezGrid m_grid;
	    Handler_Mom m_handler;

        public HW_Thick6()
        {
            InitializeComponent();
            m_bEnable = false; m_strRun = "Update"; m_msBuffer = m_msWait = 1000; m_nSmooth = 2; m_fV = 100000;
        }

        public void Init(string id, Auto_Mom auto)
        {
            m_id = id; m_auto = auto;
            m_sz[0] = m_sz[1] = this.Size; m_sz[0].Height = 26;
            m_work = m_auto.ClassWork();
            m_handler = m_auto.ClassHandler();
            m_log = new Log(m_id, m_auto.ClassLogView(), "Handler");
            m_rs232 = new ezRS232(m_id, m_log, true);
            m_grid = new ezGrid(m_id, grid, m_log, false);
            RunGrid(eGrid.eRegRead); RunGrid(eGrid.eInit);
            m_rs232.Connect(true);
        }

        public void ThreadStop()
        {
            m_rs232.ThreadStop();
        }

        public void RunGrid(eGrid eMode)
        {
            m_grid.Update(eMode, null);
            m_grid.Set(ref m_strRun, m_strSteps, "Run", "Cmd", "Run Command");
            m_grid.Set(ref m_nAdd, "Run", "Address", "Sensor Address");
            m_grid.Set(ref m_msBuffer, "Run", "Buffering", "Buffering Time (ms)");
            m_grid.Set(ref m_aAxis[0,0], "Boat0", "Start", "Start Position (pulse)");
            m_grid.Set(ref m_aAxis[0,2], "Boat0", "Trigger", "Trigger Position (pulse)");
            m_grid.Set(ref m_aAxis[0,1], "Boat0", "Length", "Inspect Length (pulse)");
            m_grid.Set(ref m_aAxis[1,0], "Boat1", "Start", "Start Position (pulse)");
            m_grid.Set(ref m_aAxis[1,2], "Boat1", "Trigger", "Trigger Position (pulse)");
            m_grid.Set(ref m_aAxis[1,1], "Boat1", "Length", "Inspect Length (pulse)");
            m_grid.Set(ref m_nError, "Inspect", "Error", "Error Count (Noise Ignore)");
            m_grid.Set(ref m_bEnable, "Setup", "Enable", "Enable Check Thickness");
            m_grid.Set(ref m_nSmooth, "Setup", "Smooth", "Buffering data Smoothing");
            m_grid.Set(ref m_fV, "Setup", "V", "Axis Velocity (pulse/sec)");
            m_grid.Set(ref m_msWait, "Setup", "Wait", "Wait for Vacuum (ms)");
            m_grid.Set(ref m_bCommEco, "Setup", "Eco", "Command Eco Mode");
            m_rs232.RunGrid(m_grid, eMode);
            m_grid.Refresh();
        }

        private void checkView_CheckedChanged(object sender, EventArgs e)
        {
            m_handler.ShowChild(); 
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            int n, nAdd; double fL = 0; string str; ezStopWatch sw = new ezStopWatch();
            RunGrid(eGrid.eUpdate);
            if (!m_bEnable) { m_log.Popup("Thick Sensor not Enable !!"); return; }
            if (m_strRun == "Single")
            {
                if (ReadSensor(m_nAdd, ref fL)) { m_log.Add("Read Sensor Error !!"); return; }
                str = m_nAdd.ToString("Read Sensor 0 : ") + fL.ToString(); m_log.Add(str);
            }
            if (m_strRun == "Buffer")
            {
                sw.Start();
                SetMode(m_nAdd, m_msBuffer);
                if (RunBuffer(m_nAdd, 1, -1)) return;
                m_log.Add("Buffer Read");

                for (n = 0; n < c_nThickData; n++) { str = m_aLData[m_nAdd,n].ToString(); m_log.Add(str, false); }
                str = sw.Check().ToString("Run Time : 00 ms"); m_log.Add(str);
            }
            if (m_strRun == "Buffer3")
            {
                if (m_nAdd < 4) nAdd = 1; else nAdd = 4;
                sw.Start();
                for (n = 0; n < 3; n++) SetMode(nAdd + n, m_msBuffer);
                if (RunBuffer(nAdd, 3, -1)) return;
                m_log.Add("Buffer3 Read");
                for (n = 0; n < c_nThickData; n++)
                {
                    str = m_aLData[nAdd, n].ToString() + " " + m_aLData[nAdd + 1, n].ToString() + " " + m_aLData[nAdd + 2, n].ToString(); m_log.Add(str, false); 
                }
                str = sw.Check().ToString("Run Time : 00 ms"); m_log.Add(str);
            }
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate); RunGrid(eGrid.eRegWrite);
        }

        public void ShowDlg(Form parent, ref CPoint cpShow)
        {
            int nIndex;
            this.TopLevel = false; this.Parent = parent; this.Location = cpShow.ToPoint();
            if (checkView.Checked) nIndex = 1; else nIndex = 0;
            this.Size = m_sz[nIndex]; cpShow.y += m_sz[nIndex].Height;
            Show();
        }
       
        public bool ReadSensor(int nAdd, ref double fL)
        {
            int n, ms, nEco; string str; char[] buf = new char[256];
            if (!m_bEnable) return false;
            m_sw.Start();
            if (m_bCommEco) nEco = 10; else nEco = 0;
            buf[0] = '%'; buf[1] = (char)(nAdd / 10 + '0'); buf[2] = (char)(nAdd % 10 + '0'); buf[3] = '#';
            buf[4] = 'R'; buf[5] = 'M'; buf[6] = 'D'; buf[7] = '*'; buf[8] = '*'; buf[9] = (char)0x0d;
            str = "%" + nAdd.ToString("00") + "#RMD**R";
            buf[9] = (char)0x0d;
            Thread.Sleep(10); m_rs232.Write(buf, 10, true); Thread.Sleep(50);
            if (ReadAns(m_aBuf, 18 + nEco, 100) < 0) { m_log.Add("Sensor Read String too Short !!"); return true; }
            ms = (int)m_sw.Check();
            m_aBuf[3+nEco] = '#';
            for (n = 0; n < 7; n++) if (buf[n] != m_aBuf[n + nEco]) { m_log.Add("Sensor Read Cmd Invalid !!"); return true; }
            fL = ReadL(m_aBuf, 7 + nEco);
            return false;
        }

        double ReadL(char[] buf, int nOffset = 0)
        {
            int n; double fL;
            fL = 0;
            for (n = 1 + nOffset; n < 8 + nOffset; n++) fL = 10 * fL + buf[n] - '0';
            if (buf[nOffset] == (char)0x2d) fL = -fL; // '-' to Ascii
            return fL / 10000;
        }

        int ReadAns(char[] buf, int nRead, int nTry, int nOffset = 0)
        {
            int nR;
            if (nTry <= 0) return -1; nTry--;
            Thread.Sleep(20); nR = m_rs232.Read(buf, nRead + 2, nOffset);
            if (nR < 0) return -1; // ing
            if (nR < nRead) return ReadAns(buf, nRead - nR, nTry, nOffset + nR);
            return 0;
        }

        public bool SetMode(int msRead)
        {
            int n;
            for (n = 1; n <= 6; n++) if (SetMode(n, msRead)) return true;
            return false;
        }

        bool SetMode(int nAdd, int msRead)
        {
            int nRate, nEco; char[] buf = new char[256];
            nRate = 5 * msRead / c_nThickData;
            if (m_bCommEco) nEco = 16; else nEco = 0;
            buf[0] = '%'; buf[1] = (char)(nAdd / 10 + '0'); buf[2] = (char)(nAdd % 10 + '0'); buf[3] = '#';
            buf[7] = '+'; buf[8] = '0'; buf[9] = '0'; buf[10] = '0'; buf[11] = '0'; buf[12] = '0'; buf[13] = '*'; buf[14] = '*'; buf[15] = (char)0x0d;
            buf[4] = 'W'; buf[5] = 'S'; buf[6] = 'P'; 
            Thread.Sleep(10); m_rs232.Write(buf, 16, true);
            if (ReadAns(m_aBuf, 10 + nEco, 100) < 0) { m_log.Add("Change Sampling Value Error !!"); return true; }
            buf[4] = 'W'; buf[5] = 'B'; buf[6] = 'D'; 
            Thread.Sleep(10); m_rs232.Write(buf, 16, true);
            if (ReadAns(m_aBuf, 10 + nEco, 100) < 0) { m_log.Add("Change Continuous Buffering Mode Error !!"); return true; }
            buf[4] = 'W'; buf[5] = 'B'; buf[6] = 'R'; buf[8] = (char)(nRate / 10000 + '0'); buf[9] = (char)((nRate / 1000) % 10 + '0'); 
            buf[10] = (char)((nRate / 100) % 10 + '0'); buf[11] = (char)((nRate / 10) % 10 + '0'); buf[12] = (char)(nRate % 10 + '0');
            Thread.Sleep(10); m_rs232.Write(buf, 16, true);
            if (ReadAns(m_aBuf, 10 + nEco, 100) < 0) { m_log.Add("Change Buffering Rate Error !!"); return true; }
            buf[4] = 'W'; buf[5] = 'B'; buf[6] = 'C'; buf[8] = '0'; buf[9] = '0'; buf[10] = '1'; buf[11] = '0'; buf[12] = '0';
            Thread.Sleep(10); m_rs232.Write(buf, 16, true);
            if (ReadAns(m_aBuf, 10 + nEco, 100) < 0) { m_log.Add("Change Buffering Count Error !!"); return true; } 
            return false;
        }

        public bool RunSensor(int nAdd, bool bRun)
        {
            char[] buf = new char[256]; int nEco;
            if (m_bCommEco) nEco = 16; else nEco = 0;
            buf[0] = '%'; buf[1] = (char)(nAdd / 10 + '0'); buf[2] = (char)(nAdd % 10 + '0'); buf[3] = '#'; buf[4] = 'W'; buf[5] = 'B'; buf[6] = 'S'; 
            buf[7] = '+'; buf[8] = '0'; buf[9] = '0'; buf[10] = '0'; buf[11] = '0'; buf[12] = '0'; buf[13] = '*'; buf[14] = '*'; buf[15] = (char)0x0d;
            if (bRun)  buf[12] = '1';
            Thread.Sleep(10); m_rs232.Write(buf, 16, true); if (ReadAns(m_aBuf, 10 + nEco, 100) < 0) { m_log.Add("Run Buffering Error !!"); return true; }
            return false;
        }

        public bool IsDone(int nAdd)
        {
            int n, nEco; char[] buf = new char[256];
            if (m_bCommEco) nEco = 10; else nEco = 0;
            buf[0] = '%'; buf[1] = (char)(nAdd / 10 + '0'); buf[2] = (char)(nAdd % 10 + '0'); buf[3] = '#'; buf[4] = 'R'; buf[5] = 'T'; buf[6] = 'S';
            buf[7] = '*'; buf[8] = '*'; buf[9] = (char)0x0d;
            Thread.Sleep(10); m_rs232.Write(buf, 10, true); if (ReadAns(m_aBuf, 16+nEco, 100) < 0) { m_log.Add("Buffering State Read Error !!"); return false; }
            m_aBuf[3 + nEco] = '#'; for (n = 0; n < 7; n++) if (buf[n] != m_aBuf[n + nEco]) return false;
            if (m_aBuf[12 + nEco] == '3') return true; else return false; // '3' to Ascii
        }
     
        bool ReadBuffer(int nAdd)
        {
            int n, nEco; char[] buf = new char[256];
            buf[0] = '%'; buf[1] = (char)(nAdd / 10 + '0'); buf[2] = (char)(nAdd % 10 + '0'); buf[3] = '#'; buf[4] = 'R'; buf[5] = 'L'; buf[6] = 'A';
            buf[7] = '0'; buf[8] = '0'; buf[9] = '0'; buf[10] = '0'; buf[11] = '1'; buf[12] = '0'; buf[13] = '0'; buf[14] = '1'; buf[15] = '0'; buf[16] = '0';
            buf[17] = '*';buf[18] = '*'; buf[19] = (char)0x0d;
            if (m_bCommEco) nEco = 20; else nEco = 0;
            Thread.Sleep(10); m_rs232.Write(buf, 20, true); if (ReadAns(m_aBuf, 8 * c_nThickData + 10 + nEco, 300) < 0) { m_log.Add("Buffering Data Read Error !!"); return true; }
            m_aBuf[3 + nEco] = '#'; for (n = 0; n < 7; n++) if (buf[n] != m_aBuf[n + nEco]) { m_log.Add("Buffering Data Read Cmd Invalid !!"); return true; }
            for (n = 0; n < c_nThickData; n++) m_aLGet[n] = ReadL(m_aBuf, 7 + 8 * n + nEco);
            Smoothing(nAdd); return false;
        }

        public bool RunBuffer(int nAdd, int nCount, int nStrip)
        {
            int n, m;
            for (n = 0; n < nCount; n++) if (RunSensor(nAdd + n, true)) { m_log.Add("Buffering Start Error !!"); return true; }
            Thread.Sleep(m_msBuffer);
            for (n = 0; n < nCount; n++) for (m = 0; m < c_nThickData; m++) { Thread.Sleep(10); if (IsDone(nAdd + n)) m = c_nThickData; }
            for (n = 0; n < nCount; n++) if (RunSensor(nAdd + n, false)) { m_log.Add("Buffering Stop Error !!"); return true; } Thread.Sleep(50);
            for (n = 0; n < nCount; n++) if (ReadBuffer(nAdd + n)) { Thread.Sleep(100); if (ReadBuffer(nAdd + n)) { m_log.Add("Buffering Read Data Error !!"); return true; } }
            return false;
        }

        void FileSave(int nID, int nAdd, int nStrip)
        {
            int n, m; double fT; string strPath, strFile;
            StreamWriter sw; 
            strPath = "d:\\Thick\\"; Directory.CreateDirectory(strPath, null);
            strPath += (m_work.m_strLot + "\\"); Directory.CreateDirectory(strPath, null);
            if (nStrip < 0) strFile = nID.ToString("Boat_00.csv"); else strFile = nStrip.ToString() + nID.ToString("_00.csv");
            sw = new StreamWriter(new FileStream(strPath + strFile, FileMode.Create)); 
            for (m = 0; m < c_nThickData; m++)
            {
                if (nStrip < 0) for (n = nAdd; n < nAdd + 3; n++) sw.Write(m_aLBoat[n, m].ToString("0.00 ,"));  
                if (nStrip >= 0) for (n = nAdd; n < nAdd + 3; n++)
                {
                    fT = m_aLBoat[n,m] - m_aLData[n,m] - m_work.m_fStripCZ[0]; if (fT < 0) fT = 0;
                    sw.Write(fT.ToString("0.00, ")); 
                }
                sw.WriteLine("");
            }
            sw.Close();
        }

        public void Copy2Boat(int nAdd)
        {
            int n;
            for (n = 0; n < c_nThickData; n++) m_aLBoat[nAdd,n] = m_aLData[nAdd,n];
        }

        void Smoothing(int nAdd)
        {
            int n, i; double fSum;
            int[] iB = new int[2];
	        for (n = 0; n<c_nThickData; n++)
	        {
		        fSum = 0;
		        iB[0] = n - m_nSmooth; if (iB[0]<0) iB[0] = 0;
		        iB[1] = n + m_nSmooth; if (iB[1] >= c_nThickData) iB[1] = c_nThickData - 1;
		        for (i = iB[0]; i <= iB[1]; i++) fSum += m_aLGet[i];
		        m_aLData[nAdd,n] = fSum / (iB[1] - iB[0] + 1);
	        }
        }

        object c_lockObject = new object();
        public bool Run(int nID, int nStrip, Axis_Mom axis, int doVac, int posDone)
        {
            bool bError; int n; double fL;
            if (m_work.m_fStripDZ == 0) return false;
            if (!m_bEnable) return false;
            fL = m_aAxis[nID,1]; m_msBuffer = (int)(1000 * fL / m_fV);
            Thread.Sleep(200); axis.WaitReady(); axis.Move(m_aAxis[nID, 0]); axis.WaitReady();
            m_auto.ClassControl().WriteOutputBit(doVac, false); Thread.Sleep(m_msWait);
            lock (c_lockObject)
            {
                if (nStrip < 0) for (n = 0; n < 3; n++) if (SetMode(3 * nID + 1 + n, m_msBuffer)) { m_log.Add("Buffering Setting Error !!"); return true; }
                axis.MoveV(posDone, m_fV);
                while (axis.GetPos(true) < m_aAxis[nID, 2]) Thread.Sleep(10);
                bError = RunBuffer(3 * nID + 1, 3, nStrip);
            }
            axis.WaitReady();
            if (nStrip < 0) { Copy2Boat(3 * nID + 1); Copy2Boat(3 * nID + 2); Copy2Boat(3 * nID + 3); }
            m_nStrip[nID] = nStrip; m_bSave[nID] = true; 
            if (bError) return true;
            if (nStrip < 0) return false;
            if (Inspect(nID) == false) return false;
            return true;
        }

        bool Inspect(int nID)
        {
            int n, m, nError; double fThick;
            nError = 0; m_fThick = m_work.m_fStripCZ[0]; m_dThick = m_work.m_fStripDZ;
            for (n = 3 * nID + 1; n <= 3 * nID + 3; n++) for (m = 0; m < c_nThickData; m++)
                {
                    fThick = m_aLBoat[n,m] - m_aLData[n,m];
                    if (fThick > (m_fThick + m_dThick)) nError++;
                }
            return (nError >= m_nError);
        }

        private void timerSave0_Tick(object sender, EventArgs e)
        {
            if (!m_bSave[0]) return;
            m_bSave[0] = false;
            FileSave(0, 1, m_nStrip[0]);
        }

        private void timerSave1_Tick(object sender, EventArgs e)
        {
            if (!m_bSave[1]) return;
            m_bSave[1] = false;
            FileSave(1, 4, m_nStrip[1]);
        }

        private void HW_Thick6_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

    }
}
