using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;
using ezAutoMom;
using ezTools;


namespace ezAuto_EFEM
{
    public partial class OHT_EFEM : DockContent, Control_Child
    {
        const int nIONum = 24;                                                      //IO갯수 각각8개씩
        int[] nLP_VALID = { -1, -1, -1 };
        int[] nLP_CS_0 = { -1, -1, -1 };
        int[] nLP_CS_1 = { -1, -1, -1 };
        int[] nLP_TR_REQ = { -1, -1, -1 };
        int[] nLP_BUSY = { -1, -1, -1 };
        int[] nLP_COMPT = { -1, -1, -1 };                                           // OHT IO 공통
        int[] nLP_L_REQ = { -1, -1, -1 };
        int[] nLP_U_REQ = { -1, -1, -1 };
        int[] nLP_READY = { -1, -1, -1 };
        //-----------------------------------------------------------               //천안,온양
        int[] nLP_CS_2 = { -1, -1, -1 };
        int[] nLP_CS_3 = { -1, -1, -1 };
        int[] nLP_ABORT = { -1, -1, -1 };
        //-----------------------------------------------------------               //기흥
        int[] nLP_CONT = { -1, -1, -1 };
        int[] nLP_HO_AVBL = { -1, -1, -1 };
        int[] nLP_ES = { -1, -1, -1 };
        //-----------------------------------------------------------
        Auto_Mom m_auto;
        Log m_log;
        ezRegistry m_reg = new ezRegistry("EFEM", "Control");
        ezStopWatch[] sw = new ezStopWatch[3];
        Control_Mom mom_Control;
        HW_LoadPort_Mom[] m_Loadport = new HW_LoadPort_Mom[3];
        Button[] bLP1Input = new Button[nIONum];
        Button[] bLP1Output = new Button[nIONum];
        Button[] bLP2Input = new Button[nIONum];
        Button[] bLP2Output = new Button[nIONum];
        Button[] bLP3Input = new Button[nIONum];
        Button[] bLP3Output = new Button[nIONum];
        TextBox[] tbTP = new TextBox[7];
        Label[] lbTP = new Label[7];
        int[] nTP = new int[7];                                                     // TP1~6 
        string[] m_SECInput = { "Vaild", "CS_0", "CS_1", "N/C", "TR_REQ", "BUSY", "COMPT", "CONT" };
        string[] m_SSEMInput = { "Vaild", "CS_0", "CS_1", "CS_2", "CS_3", "TR_REQ", "BUSY", "COMPT" };
        string[] m_sInput = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
        bool[] m_bInput = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };           //Input 상태 
        string[] m_SECOutput = { "L_REQ", "U_REQ", "N/C", "READY", "N/C", "N/C", "HO_AVBL", "ES" };
        string[] m_SSEMOutput = { "L_REQ", "U_REQ", "ABORT", "READY", "N/C", "N/C", "N/C", "N/C" };
        string[] m_sOutput = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
        bool[] m_bOutput = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };        //Output 상태
        bool[] m_bInput_Test = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };           //PIO TEST Input IO
        bool[] m_bOutput_Test = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };          //PIO TEST Output IO 
        int[] nOHTcycle = { 1, 1, 1 };
        bool[] Loadstate = { false, false, false };
        bool[] bBusy = { false, false, false };
        int[] LPIOStart = { 0, 8, 16 };
        int[] LPIOEnd = { 7, 15, 23 };
        int[] LPInitStartNum_EIP = { 8, 16, 24 };                                   //EIP
        int[] LPInitStartNum_AJIN = { 0, 8, 16 };
        bool[] bLPLoadPro = { false, false, false };
        bool[] bLPUnLoadPro = { false, false, false };
        bool bAxisOpen = false;
        bool[] LPErr = { false, false, false };
        bool[] bLPPlaceCheck = { false, false, false };
        bool[] bLPCheck_Test = { false, false, false };
        bool[] bLPCompleteKey = { false, false, false };
        public string str_OHT_Model = "";
        public string str_OHT_IOType = "";
        public bool bOHTTest = false;
        int nPortNum = -1;
        public string strLog = "";
        bool[] bIsCassette = { false, false, false };
        public eLoadUnloadFlag[] m_eFlag = { eLoadUnloadFlag.None, eLoadUnloadFlag.None, eLoadUnloadFlag.None };
        int nTChat = 0;                                                             //  chattering time
        int nTSL = 0;                                                               //  Sensor Logic Time
        //----------------------------------------------------------
        int[] nLPPresent = { -1, -1, -1 };
        int[] nLPPlacement = { -1, -1, -1 };
        int[] nLPOpen = { -1, -1, -1 };
        //----------------------------------------------------------
        public enum eMainCycle
        {
            Init = 1,
            CASSETTE_SELECT = 2,
            NETWORK_VALID = 3,
            TR_REQ = 4,
            BUSY_START = 5,
            CSTStateChange = 6,
            BUSY_END = 7,
            COMPLETE = 8,
            CONTINUE = 9,
            LOAD_REQUEST = 10,
            UNLOAD_REQUEST = 11,
            READY = 12,
            HO_AVBL = 13,
            ES = 14,
            Error = 15,
            Manualstate = 16,
            Loadstate = 17,
            PROCESSEND = 18,

        }

        public enum eLoadUnloadFlag
        {
            None,
            load_Sequence,
            Unload_Sequence,
        }

        public enum eCSTState
        {
            On,
            Off,
            Not_Present,
            Not_Placement
        }

        public OHT_EFEM()
        {
            InitializeComponent();
        }
        //-----------------------------------------------------------------------------------------------------------------------
        public void LPIOUpdate()                                             // Loadport 상태 알수 있도록 표시
        {
            for (int i = 0; i < nPortNum; i++)
            {
                m_reg.Read("LoadPort" + Convert.ToString(i) + "DI_Present", ref nLPPresent[i]);
                m_reg.Read("LoadPort" + Convert.ToString(i) + "DI_Placment", ref nLPPlacement[i]);
                m_reg.Read("LoadPort" + Convert.ToString(i) + "DI_Open", ref nLPOpen[i]);
            }
        }

        public void Init(Auto_Mom auto)                                      //프로그램 종료 전 상태 끌어오기 및 IO Display
        {
            m_auto = auto;
            nPortNum = m_auto.ClassHandler().m_nLoadPort;
            m_log = new Log("OHT", m_auto.ClassLogView(), "OHT");
            mom_Control = m_auto.ClassControl();
            mom_Control.Add(this);
            bAxisOpen = mom_Control.IsAxisOpen();
            m_log.m_reg.Read("OHT_LP1_Status", ref m_auto.ClassWork().bLPCommStatus[0]);
            m_log.m_reg.Read("OHT_LP2_Status", ref m_auto.ClassWork().bLPCommStatus[1]);
            m_log.m_reg.Read("OHT_LP3_Status", ref m_auto.ClassWork().bLPCommStatus[2]);
            m_log.m_reg.Read("Chattering_Time", ref nTChat);
            m_log.m_reg.Read("Sensor_Logic_Time", ref nTSL);
            for (int i = 1; i < 7; i++)
            {
                m_log.m_reg.Read("TP" + Convert.ToString(i), ref nTP[i]);
            }
            for (int i = 0; i < nPortNum; i++)
            {
                m_Loadport[i] = m_auto.ClassHandler().ClassLoadPort(i);
                sw[i] = new ezStopWatch();
            }
            MakeIO();
            LPIOUpdate();
            this.Width = 660;
            btnLP1Reset.BackgroundImage = null;
            SetMode();
            SetLPUI(nPortNum);
        }
        public void SetLPUI(int nLPCount)
        {
            for (int n = 0; n < 6; n++)
            {
                tableLayoutPanel1.ColumnStyles[n].Width = 0;
            }
            for (int i = 0; i < nLPCount; i++)
            {
                tableLayoutPanel1.ColumnStyles[2 * i].Width = 100;
                tableLayoutPanel1.ColumnStyles[(2 * i) + 1].Width = 100;
            }
        }
        public void SetMode()
        {
            if (m_auto.m_strOHTType == "PIOTEST") bOHTTest = true;
        }
        public bool ISCSTLoadOK(int nLPNum)
        {
            if ((eMainCycle)nOHTcycle[nLPNum] == eMainCycle.PROCESSEND) return true;
            else return false;
        }

        bool[] IsCSTOK = new bool[3] { false, false, false };

        public bool IsCSTLoadOK2(int nLPNum)
        {
            return IsCSTOK[nLPNum];
        }

        public void ResetCSTLoadOK(int nLPNum)
        {
            IsCSTOK[nLPNum] = false;
        }


        public void SetCSTLoadOK(int nLPNum, bool bOK)
        {
            IsCSTOK[nLPNum] = bOK;
        }
        public void ModelGrid(ezGrid rGrid, eGrid eMode)
        {
            rGrid.Set(ref bOHTTest, "OHT", "Test Mode", "Use PIO Simulation");
        }

        public void ControlGrid(Control_Mom control, ezGrid rGrid, eGrid eMode)
        {
            if (str_OHT_Model == "SEC")                                          //기흥
            {
                //-------------------------------------------------InPut(LP1)------------------------------------------------------------
                control.AddDI(rGrid, ref nLP_VALID[0], "OHT", "LP1_VALID", "LP1_VALID");
                control.AddDI(rGrid, ref nLP_CS_0[0], "OHT", "LP1_CS_0", "LP1_CS_0");
                control.AddDI(rGrid, ref nLP_CS_1[0], "OHT", "LP1_CS_1", "LP1_CS_1");
                control.AddDI(rGrid, ref nLP_TR_REQ[0], "OHT", "LP1_TR_REQ", "LP1_TR_REQ");
                control.AddDI(rGrid, ref nLP_BUSY[0], "OHT", "LP1_BUSY", "LP1_BUSY");
                control.AddDI(rGrid, ref nLP_COMPT[0], "OHT", "LP1_COMPT", "LP1_COMPT");
                control.AddDI(rGrid, ref nLP_CONT[0], "OHT", "LP1_CONT", "LP1_CONT");
                //-------------------------------------------------InPut(LP2)------------------------------------------------------------
                control.AddDI(rGrid, ref nLP_VALID[1], "OHT", "LP2_VALID", "LP2_VALID");
                control.AddDI(rGrid, ref nLP_CS_0[1], "OHT", "LP2_CS_0", "LP2_CS_0");
                control.AddDI(rGrid, ref nLP_CS_1[1], "OHT", "LP2_CS_1", "LP2_CS_1");
                control.AddDI(rGrid, ref nLP_TR_REQ[1], "OHT", "LP2_TR_REQ", "LP2_TR_REQ");
                control.AddDI(rGrid, ref nLP_BUSY[1], "OHT", "LP2_BUSY", "LP2_BUSY");
                control.AddDI(rGrid, ref nLP_COMPT[1], "OHT", "LP2_COMPT", "LP2_COMPT");
                control.AddDI(rGrid, ref nLP_CONT[1], "OHT", "LP2_CONT", "LP2_CONT");
                //-------------------------------------------------InPut(LP3)------------------------------------------------------------
                control.AddDI(rGrid, ref nLP_VALID[2], "OHT", "LP3_VALID", "LP3_VALID");
                control.AddDI(rGrid, ref nLP_CS_0[2], "OHT", "LP3_CS_0", "LP3_CS_0");
                control.AddDI(rGrid, ref nLP_CS_1[2], "OHT", "LP3_CS_1", "LP3_CS_1");
                control.AddDI(rGrid, ref nLP_TR_REQ[2], "OHT", "LP3_TR_REQ", "LP3_TR_REQ");
                control.AddDI(rGrid, ref nLP_BUSY[2], "OHT", "LP3_BUSY", "LP3_BUSY");
                control.AddDI(rGrid, ref nLP_COMPT[2], "OHT", "LP3_COMPT", "LP3_COMPT");
                control.AddDI(rGrid, ref nLP_CONT[2], "OHT", "LP3_CONT", "LP3_CONT");
                //-------------------------------------------------OutPut(LP1)------------------------------------------------------------
                control.AddDO(rGrid, ref nLP_L_REQ[0], "OHT", "LP1_L_REQ", "LP1_L_REQ");
                control.AddDO(rGrid, ref nLP_U_REQ[0], "OHT", "LP1_U_REQ", "LP1_U_REQ");
                control.AddDO(rGrid, ref nLP_READY[0], "OHT", "LP1_READY", "LP1_READY");
                control.AddDO(rGrid, ref nLP_HO_AVBL[0], "OHT", "LP1_HO_AVBL", "LP1_HO_AVBL");
                control.AddDO(rGrid, ref nLP_ES[0], "OHT", "LP1_ES", "LP1_ES");
                //-------------------------------------------------OutPut(LP2)------------------------------------------------------------
                control.AddDO(rGrid, ref nLP_L_REQ[1], "OHT", "LP2_L_REQ", "LP2_L_REQ");
                control.AddDO(rGrid, ref nLP_U_REQ[1], "OHT", "LP2_U_REQ", "LP2_U_REQ");
                control.AddDO(rGrid, ref nLP_READY[1], "OHT", "LP2_READY", "LP2_READY");
                control.AddDO(rGrid, ref nLP_HO_AVBL[1], "OHT", "LP2_HO_AVBL", "LP2_HO_AVBL");
                control.AddDO(rGrid, ref nLP_ES[1], "OHT", "LP2_ES", "LP2_ES");
                //-------------------------------------------------OutPut(LP2)------------------------------------------------------------
                control.AddDO(rGrid, ref nLP_L_REQ[2], "OHT", "LP3_L_REQ", "LP3_L_REQ");
                control.AddDO(rGrid, ref nLP_U_REQ[2], "OHT", "LP3_U_REQ", "LP3_U_REQ");
                control.AddDO(rGrid, ref nLP_READY[2], "OHT", "LP3_READY", "LP3_READY");
                control.AddDO(rGrid, ref nLP_HO_AVBL[2], "OHT", "LP3_HO_AVBL", "LP3_HO_AVBL");
                control.AddDO(rGrid, ref nLP_ES[2], "OHT", "LP3_ES", "LP3_ES");
            }
            else if (str_OHT_Model == "SSEM")                             //천안 온양
            {
                //-------------------------------------------------InPut(LP1)------------------------------------------------------------
                control.AddDI(rGrid, ref nLP_VALID[0], "OHT", "LP1_VALID", "LP1_VALID");
                control.AddDI(rGrid, ref nLP_CS_0[0], "OHT", "LP1_CS_0", "LP1_CS_0");
                control.AddDI(rGrid, ref nLP_CS_1[0], "OHT", "LP1_CS_1", "LP1_CS_1");
                control.AddDI(rGrid, ref nLP_CS_2[0], "OHT", "LP1_CS_2", "LP1_CS_2");
                control.AddDI(rGrid, ref nLP_CS_3[0], "OHT", "LP1_CS_3", "LP1_CS_3");
                control.AddDI(rGrid, ref nLP_TR_REQ[0], "OHT", "LP1_TR_REQ", "LP1_TR_REQ");
                control.AddDI(rGrid, ref nLP_BUSY[0], "OHT", "LP1_BUSY", "LP1_BUSY");
                control.AddDI(rGrid, ref nLP_COMPT[0], "OHT", "LP1_COMPT", "LP1_COMPT");
                //-------------------------------------------------InPut(LP2)------------------------------------------------------------
                control.AddDI(rGrid, ref nLP_VALID[1], "OHT", "LP2_VALID", "LP2_VALID");
                control.AddDI(rGrid, ref nLP_CS_0[1], "OHT", "LP2_CS_0", "LP2_CS_0");
                control.AddDI(rGrid, ref nLP_CS_1[1], "OHT", "LP2_CS_1", "LP2_CS_1");
                control.AddDI(rGrid, ref nLP_CS_2[1], "OHT", "LP2_CS_2", "LP2_CS_2");
                control.AddDI(rGrid, ref nLP_CS_3[1], "OHT", "LP2_CS_3", "LP2_CS_3");
                control.AddDI(rGrid, ref nLP_TR_REQ[1], "OHT", "LP2_TR_REQ", "LP1_TR_REQ");
                control.AddDI(rGrid, ref nLP_BUSY[1], "OHT", "LP2_BUSY", "LP2_BUSY");
                control.AddDI(rGrid, ref nLP_COMPT[1], "OHT", "LP2_COMPT", "LP2_COMPT");
                //-------------------------------------------------OutPut(LP1)------------------------------------------------------------
                control.AddDO(rGrid, ref nLP_L_REQ[0], "OHT", "LP1_L_REQ", "LP1_L_REQ");
                control.AddDO(rGrid, ref nLP_U_REQ[0], "OHT", "LP1_U_REQ", "LP1_U_REQ");
                control.AddDI(rGrid, ref nLP_ABORT[0], "OHT", "LP1_ABORT", "LP1_ABORT");
                control.AddDO(rGrid, ref nLP_READY[0], "OHT", "LP1_READY", "LP1_READY");
                //-------------------------------------------------OutPut(LP2)------------------------------------------------------------
                control.AddDO(rGrid, ref nLP_L_REQ[1], "OHT", "LP2_L_REQ", "LP2_L_REQ");
                control.AddDO(rGrid, ref nLP_U_REQ[1], "OHT", "LP2_U_REQ", "LP2_U_REQ");
                control.AddDI(rGrid, ref nLP_ABORT[1], "OHT", "LP2_ABORT", "LP2_ABORT");
                control.AddDO(rGrid, ref nLP_READY[1], "OHT", "LP2_READY", "LP2_READY");
            }
        }
        public void MakeIO()                                                //IO 생성
        {
            for (int i = 0; i < nIONum; i++)
            {
                bLP1Input[i] = new Button();
                bLP1Output[i] = new Button();
                bLP2Input[i] = new Button();
                bLP2Output[i] = new Button();
                bLP3Input[i] = new Button();
                bLP3Output[i] = new Button();
                if (i < 8)
                {
                    UpdateOHTIO(i);
                    gbloadport1.Controls.Add(bLP1Input[i]);
                    gbloadport1.Controls.Add(bLP1Output[i]);
                    bLP1Input[i].Name = "bLP1" + m_sInput[i];
                    bLP1Output[i].Name = "bLP1" + m_sOutput[i];
                    bLP1Input[i].Text = m_sInput[i];
                    bLP1Output[i].Text = m_sOutput[i];
                    bLP1Input[i].Dock = DockStyle.Fill;
                    bLP1Output[i].Dock = DockStyle.Fill;
                    bLP1Input[i].BackColor = Color.LemonChiffon;
                    bLP1Output[i].BackColor = Color.LemonChiffon;
                    tableLayoutLP1.Controls.Add(bLP1Input[i], 0, i);
                    tableLayoutLP1.Controls.Add(bLP1Output[i], 1, i);
                    bLP1Input[i].Tag = i;
                    bLP1Input[i].Click += button1_Click;
                    bLP1Output[i].Tag = i;
                    bLP1Output[i].Click += button4_Click;
                }
                else if (i >= 8 && i < 16)
                {
                    gbloadport2.Controls.Add(bLP2Input[i]);
                    gbloadport2.Controls.Add(bLP2Output[i]);
                    bLP2Input[i].Name = "bLP2" + m_sInput[i - 8];
                    bLP2Output[i].Name = "bLP2" + m_sOutput[i - 8];
                    bLP2Input[i].Text = m_sInput[i - 8];
                    bLP2Output[i].Text = m_sOutput[i - 8];
                    bLP2Input[i].Dock = DockStyle.Fill;
                    bLP2Output[i].Dock = DockStyle.Fill;
                    bLP2Input[i].BackColor = Color.LemonChiffon;
                    bLP2Output[i].BackColor = Color.LemonChiffon;
                    tableLayoutLP2.Controls.Add(bLP2Input[i], 0, i - 8);
                    tableLayoutLP2.Controls.Add(bLP2Output[i], 1, i - 8);
                    bLP2Input[i].Tag = i;
                    bLP2Input[i].Click += button1_Click;
                    bLP2Output[i].Tag = i;
                    bLP2Output[i].Click += button4_Click;
                }
                else if (i >= 16)
                {
                    gbloadport3.Controls.Add(bLP3Input[i]);
                    gbloadport3.Controls.Add(bLP3Output[i]);
                    bLP3Input[i].Name = "bLP3" + m_sInput[i - 16];
                    bLP3Output[i].Name = "bLP3" + m_sOutput[i - 16];
                    bLP3Input[i].Text = m_sInput[i - 16];
                    bLP3Output[i].Text = m_sOutput[i - 16];
                    bLP3Input[i].Dock = DockStyle.Fill;
                    bLP3Output[i].Dock = DockStyle.Fill;
                    bLP3Input[i].BackColor = Color.LemonChiffon;
                    bLP3Output[i].BackColor = Color.LemonChiffon;
                    tableLayoutLP3.Controls.Add(bLP3Input[i], 0, i - 16);
                    tableLayoutLP3.Controls.Add(bLP3Output[i], 1, i - 16);
                    bLP3Input[i].Tag = i;
                    bLP3Input[i].Click += button1_Click;
                    bLP3Output[i].Tag = i;
                    bLP3Output[i].Click += button4_Click;
                }


            }
            if (str_OHT_Model == "SEC")
            {
                for (int j = 0; j < 6; j++)
                {
                    int k = j + 1;
                    tbTP[k] = new TextBox();
                    lbTP[j] = new Label();
                    this.Controls.Add(tbTP[k]);
                    this.Controls.Add(lbTP[j]);
                    tbTP[k].Name = "TP" + k;
                    lbTP[j].Name = "TP" + j;
                    lbTP[j].Text = "TP" + k;
                    tbTP[k].Text = Convert.ToString(nTP[k]);
                    tbTP[k].Location = new Point(715, 70 + (j * 30));
                    lbTP[j].Location = new Point(671, 73 + (j * 30));
                }
            }
            else if (str_OHT_Model == "SSEM")
            {
                for (int i = 1; i <= 6; i++)
                {
                    tbTP[i] = new TextBox();
                    lbTP[i] = new Label();
                    if (i == 1 || i == 3 || i == 6)
                    {
                        this.Controls.Add(tbTP[i]);
                        this.Controls.Add(lbTP[i]);
                        tbTP[i].Name = "TP" + i;
                        lbTP[i].Name = "TP" + i;
                        tbTP[i].Text = Convert.ToString(nTP[i]);
                        lbTP[i].Text = "t" + i;
                        tbTP[i].Location = new Point(715, 70 + (i * 15));
                        lbTP[i].Location = new Point(671, 73 + (i * 15));
                    }
                    else
                    {
                        tbTP[i].Text = Convert.ToString(nTP[i]);
                    }
                }
            }
        }
        public void DisplayIO(int nLPNum)                           //IO 상태 실시간 갱신
        {
            if (nLPNum == 0)
            {
                for (int i = 0; i < nIONum; i++)
                {
                    bool bIn = false;
                    bool bOut = false;

                    if (str_OHT_IOType == "AJIN")
                    {
                        bIn = m_auto.ClassControl().GetInputBit(m_auto.nInStartNum + i);          //170105 SDH ADD LP0 IO 시작지점 설정.
                        bOut = m_auto.ClassControl().GetOutputBit(m_auto.nOutStartNum + i);       //170105 SDH ADD LP0 IO 시작지점 설정.
                    }
                    else if (str_OHT_IOType == "EIP")
                    {
                        bIn = m_auto.ClassControl().GetInputBit(m_auto.nInStartNum + i);
                        bOut = m_auto.ClassControl().GetOutputBit(m_auto.nInStartNum + 8 + i);
                    }
                    else if (str_OHT_IOType == "PIOTEST")
                    {
                        bIn = m_bInput_Test[i];
                        bOut = m_bOutput_Test[i];
                    }
                    if (bIn)
                    {
                        bLP1Input[i].BackColor = Color.LightGreen;
                        m_bInput[i] = true;
                    }
                    else if (!bIn)
                    {
                        bLP1Input[i].BackColor = Color.LemonChiffon;
                        m_bInput[i] = false;
                    }
                    if (bOut)
                    {
                        bLP1Output[i].BackColor = Color.LightGreen;
                        m_bOutput[i] = true;
                    }
                    else if (!bOut)
                    {
                        bLP1Output[i].BackColor = Color.LemonChiffon;
                        m_bOutput[i] = false;
                    }
                }
            }
            if (nLPNum == 1)
            {
                for (int i = 8; i < nIONum; i++)
                {
                    bool bIn = false;
                    bool bOut = false;
                    if (str_OHT_IOType == "AJIN")
                    {
                        bIn = m_auto.ClassControl().GetInputBit(m_auto.nInStartNum + i);        //170105 SDH ADD LP0 IO 시작지점 설정.
                        bOut = m_auto.ClassControl().GetOutputBit(m_auto.nOutStartNum + i);     //170105 SDH ADD LP0 IO 시작지점 설정.
                    }
                    else if (str_OHT_IOType == "EIP")
                    {
                        bIn = m_auto.ClassControl().GetInputBit(m_auto.nInStartNum + 8 + i);
                        bOut = m_auto.ClassControl().GetOutputBit(m_auto.nInStartNum + 16 + i);
                    }
                    else if (str_OHT_IOType == "PIOTEST")
                    {
                        bIn = m_bInput_Test[i];
                        bOut = m_bOutput_Test[i];
                    }
                    if (bIn)
                    {
                        bLP2Input[i].BackColor = Color.LightGreen;
                        m_bInput[i] = true;
                    }

                    else if (!bIn)
                    {
                        bLP2Input[i].BackColor = Color.LemonChiffon;
                        m_bInput[i] = false;
                    }
                    if (bOut)
                    {
                        bLP2Output[i].BackColor = Color.LightGreen;
                        m_bOutput[i] = true;
                    }
                    else if (!bOut)
                    {
                        bLP2Output[i].BackColor = Color.LemonChiffon;
                        m_bOutput[i] = false;
                    }
                }
            }
            if (nLPNum == 2)
            {
                for (int i = 16; i < nIONum; i++)
                {
                    bool bIn = false;
                    bool bOut = false;
                    if (str_OHT_IOType == "AJIN")
                    {
                        bIn = m_auto.ClassControl().GetInputBit(m_auto.nInStartNum + i);        //170105 SDH ADD LP0 IO 시작지점 설정.
                        bOut = m_auto.ClassControl().GetOutputBit(m_auto.nOutStartNum + i);     //170105 SDH ADD LP0 IO 시작지점 설정.
                    }
                    else if (str_OHT_IOType == "EIP")
                    {
                        bIn = m_auto.ClassControl().GetInputBit(m_auto.nInStartNum + 16 + i);
                        bOut = m_auto.ClassControl().GetOutputBit(m_auto.nInStartNum + 24 + i);
                    }
                    else if (str_OHT_IOType == "PIOTEST")
                    {
                        bIn = m_bInput_Test[i];
                        bOut = m_bOutput_Test[i];
                    }
                    if (bIn)
                    {
                        bLP3Input[i].BackColor = Color.LightGreen;
                        m_bInput[i] = true;
                    }

                    else if (!bIn)
                    {
                        bLP3Input[i].BackColor = Color.LemonChiffon;
                        m_bInput[i] = false;
                    }
                    if (bOut)
                    {
                        bLP3Output[i].BackColor = Color.LightGreen;
                        m_bOutput[i] = true;
                    }
                    else if (!bOut)
                    {
                        bLP3Output[i].BackColor = Color.LemonChiffon;
                        m_bOutput[i] = false;
                    }
                }
            }
            if (str_OHT_Model == "SEC")
            {
                IsES(nLPNum);
                IsHo_AVBL(nLPNum);
            }
            else if (str_OHT_Model == "SSEM")
            {
                IsABORT(nLPNum);
            }
        }

        public bool IsAll_In_Off(int nPortNum)                       //프로세스 종료후 IO 모두 정상적으로 꺼졋는지 확인
        {
            int OnNum = 0;
            bool bResult = false;
            for (int i = LPIOStart[nPortNum]; i <= LPIOEnd[nPortNum]; i++)
            {
                if (m_bInput[i])
                {
                    OnNum++;
                }
            }
            if (OnNum == 0) bResult = true;
            else if (OnNum > 0) bResult = false;
            return bResult;
        }
        public bool IsAll_Out_Off(int nPortNum)                       //프로세스 종료후 IO 모두 정상적으로 꺼졋는지 확인
        {
            int OnNum = 0;
            bool bResult = false;
            for (int i = LPIOStart[nPortNum]; i <= LPIOEnd[nPortNum]; i++)
            {
                if (i <= LPIOEnd[nPortNum] - 2)                      //ES,Ho_AVBL 제외
                {
                    if (m_bOutput[i])
                    {
                        OnNum++;
                    }
                }
            }
            if (OnNum == 0) bResult = true;
            else if (OnNum > 0) bResult = false;
            return bResult;
        }

        public void StatusUpdate(int nPortNum)                          //LoadPort 상태 실시간으로 갱신
        {
            CheckForIllegalCrossThreadCalls = false;
            if (nPortNum == 0)
            {
                if (Out(nLP_L_REQ[nPortNum])) bLP1Load.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.LoadRun;

                else bLP1Load.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Load;

                if (Out(nLP_U_REQ[nPortNum])) bLP1UnLoad.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.UnloadRun;

                else bLP1UnLoad.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Unload;

                if (m_auto.ClassXGem().m_aXGem300Carrier != null)
                {
                    if (m_auto.ClassXGem().m_aXGem300Carrier[nPortNum].m_ePortAccess == XGem300Carrier.ePortAccess.Auto) bLP1Status.Text = "Auto";
                    else bLP1Status.Text = "Manual";
                }

                if (m_auto.m_strOHTType == "PIOTEST")
                {
                    if (bLPCheck_Test[nPortNum])
                    {
                        bLPPlaceCheck[nPortNum] = true;
                        bCkLP1.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Cassette;
                        bCkLP1.BackgroundImageLayout = ImageLayout.Zoom;
                    }
                    else
                    {
                        bLPPlaceCheck[nPortNum] = false;
                        bCkLP1.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Loadport;
                        bCkLP1.BackgroundImageLayout = ImageLayout.Zoom;
                    }
                }
                else
                {
                    if (IsLightCurtainErr())
                    {
                        bCkLP1.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.LightCurtain;
                        bCkLP2.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.LightCurtain;
                        bCkLP3.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.LightCurtain;
                    }
                    else if (IsCheckCST(nPortNum) == eCSTState.On)
                    {
                        bLPPlaceCheck[nPortNum] = true;
                        bCkLP1.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Cassette;
                        bCkLP1.BackgroundImageLayout = ImageLayout.Zoom;
                    }
                    else
                    {
                        bLPPlaceCheck[nPortNum] = false;
                        bCkLP1.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Loadport;
                        bCkLP1.BackgroundImageLayout = ImageLayout.Zoom;
                    }
                }
                lbStateLP1.Text = ((eMainCycle)nOHTcycle[0]).ToString();
                if (btnLP1Reset.Enabled)
                    btnLP1Reset.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Button;
                else
                    btnLP1Reset.BackgroundImage = null;
            }
            else if (nPortNum == 1)
            {
                if (Out(nLP_L_REQ[nPortNum])) bLP2Load.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.LoadRun;

                else bLP2Load.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Load;

                if (Out(nLP_U_REQ[nPortNum])) bLP2UnLoad.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.UnloadRun;

                else bLP2UnLoad.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Unload;

                if (m_auto.ClassXGem().m_aXGem300Carrier != null)
                {
                    if (m_auto.ClassXGem().m_aXGem300Carrier[nPortNum].m_ePortAccess == XGem300Carrier.ePortAccess.Auto) bLP2Status.Text = "Auto";
                    else bLP2Status.Text = "Manual";
                }
                if (m_auto.m_strOHTType == "PIOTEST")
                {
                    if (bLPCheck_Test[nPortNum])
                    {
                        bLPPlaceCheck[nPortNum] = true;
                        bCkLP2.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Cassette;
                        bCkLP2.BackgroundImageLayout = ImageLayout.Zoom;
                    }
                    else
                    {
                        bLPPlaceCheck[nPortNum] = false;
                        bCkLP2.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Loadport;
                        bCkLP2.BackgroundImageLayout = ImageLayout.Zoom;
                    }
                }
                else
                {
                    if (IsCheckCST(nPortNum) == eCSTState.On)
                    {
                        bLPPlaceCheck[nPortNum] = true;
                        bCkLP2.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Cassette;
                        bCkLP2.BackgroundImageLayout = ImageLayout.Zoom;

                    }
                    else
                    {
                        bLPPlaceCheck[nPortNum] = false;
                        bCkLP2.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Loadport;
                        bCkLP2.BackgroundImageLayout = ImageLayout.Zoom;
                    }
                }
                lbStateLP2.Text = ((eMainCycle)nOHTcycle[1]).ToString();
                if (btnLP2Reset.Enabled)
                    btnLP2Reset.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Button;
                else
                    btnLP2Reset.BackgroundImage = null;
            }
            else if (nPortNum == 2)
            {
                if (Out(nLP_L_REQ[nPortNum])) bLP3Load.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.LoadRun;

                else bLP3Load.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Load;

                if (Out(nLP_U_REQ[nPortNum])) bLP3UnLoad.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.UnloadRun;

                else bLP3UnLoad.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Unload;

                if (m_auto.ClassXGem().m_aXGem300Carrier != null)
                {
                    if (m_auto.ClassXGem().m_aXGem300Carrier[nPortNum].m_ePortAccess == XGem300Carrier.ePortAccess.Auto) bLP3Status.Text = "Auto";
                    else bLP3Status.Text = "Manual";
                }
                if (m_auto.m_strOHTType == "PIOTEST")
                {
                    if (bLPCheck_Test[nPortNum])
                    {
                        bLPPlaceCheck[nPortNum] = true;
                        bCkLP3.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Cassette;
                        bCkLP3.BackgroundImageLayout = ImageLayout.Zoom;
                    }
                    else
                    {
                        bLPPlaceCheck[nPortNum] = false;
                        bCkLP3.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Loadport;
                        bCkLP3.BackgroundImageLayout = ImageLayout.Zoom;
                    }
                }
                else
                {
                    if (IsCheckCST(nPortNum) == eCSTState.On)
                    {
                        bLPPlaceCheck[nPortNum] = true;
                        bCkLP3.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Cassette;
                        bCkLP3.BackgroundImageLayout = ImageLayout.Zoom;

                    }
                    else
                    {
                        bLPPlaceCheck[nPortNum] = false;
                        bCkLP3.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Loadport;
                        bCkLP3.BackgroundImageLayout = ImageLayout.Zoom;
                    }
                }
                lbStateLP3.Text = ((eMainCycle)nOHTcycle[2]).ToString();
                if (btnLP3Reset.Enabled)
                    btnLP3Reset.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Button;
                else
                    btnLP3Reset.BackgroundImage = null;
            }
        }
        public void On(int n)                                               //On
        {
            m_auto.ClassControl().WriteOutputBit(n, true);
        }
        public void Off(int n)                                              //Off
        {
            m_auto.ClassControl().WriteOutputBit(n, false);
        }
        public bool In(int n)                                               //InPut On됫는지 확인
        {
            bool bIn = false;
            if (m_auto.m_strOHTType == "PIOTEST")
            {
                if (n >= m_auto.nInStartNum) bIn = m_bInput_Test[n - m_auto.nInStartNum];
            }
            else
            {
                bIn = m_auto.ClassControl().GetInputBit(n);
            }
            return bIn;
        }
        public bool Out(int n)                                              //OutPut On됫는지 확인
        {
            bool bOut = false;
            if (m_auto.m_strOHTType == "PIOTEST")
            {
                if (n >= m_auto.nInStartNum) bOut = m_bOutput_Test[n - m_auto.nInStartNum];
            }
            else
            {
                bOut = m_auto.ClassControl().GetOutputBit(n);
            }
            return bOut;
        }
        public bool IsReadyLP(int nLP)                                      //LoadPort Ready 상태인지 확인
        {
            bool bResult = false;
            if (m_auto.m_strOHTType == "PIOTEST") bResult = true;
            else
            {
                if (m_Loadport[nLP].GetState() == HW_LoadPort_Mom.eState.Ready && !m_Loadport[nLP].IsDocking() && !m_Loadport[nLP].IsDoorOpenPos()) bResult = true;     //조건 추가
                else bResult = false;
            }
            return bResult;
        }
        public eMainCycle GetMainCycle(int nLPNum)
        {
            return (eMainCycle)nOHTcycle[nLPNum];
        }
        public void OHTProcess(int nPortNum)                                //OHT Process LoadPort 각각 별도로 진행
        {
            if (m_Loadport[nPortNum] == null) return;
            if ((m_auto.ClassWork().bLPCommStatus[nPortNum] && IsReadyLP(nPortNum) && !IsLightCurtainErr() && !m_Loadport[nPortNum].m_bRnRStart) || bOHTTest)
            {
                eMainCycle eState = (eMainCycle)nOHTcycle[nPortNum];
                switch ((eMainCycle)nOHTcycle[nPortNum])
                {
                    case eMainCycle.Init:
                        if (!LPErr[nPortNum])
                        {
                            nOHTcycle[nPortNum] = (int)eMainCycle.Loadstate;
                            bLPCompleteKey[nPortNum] = false;
                            bLPLoadPro[nPortNum] = false;
                        }
                        break;

                    case eMainCycle.Loadstate:
                        if ((In(nLP_CS_0[nPortNum]) || In(nLP_CS_1[nPortNum])))
                        {
                            if (IsReadyLP(nPortNum) && !LPErr[nPortNum] && m_auto.ClassWork().bLPCommStatus[nPortNum])
                            {
                                m_eFlag[nPortNum] = eLoadUnloadFlag.None;
                                sw[nPortNum].Start();
                                nOHTcycle[nPortNum] = (int)eMainCycle.CASSETTE_SELECT;
                                strLog = " Loadport LoadState";
                                AddOHTLog(nPortNum, Convert.ToString(eState), strLog);
                            }
                        }
                        break;
                    case eMainCycle.CASSETTE_SELECT:

                        if ((In(nLP_CS_0[nPortNum]) || In(nLP_CS_1[nPortNum])) && In(nLP_VALID[nPortNum]))
                        {
                            if (IsCheckCST(nPortNum) == eCSTState.Off)
                            {
                                sw[nPortNum].Start();
                                if (str_OHT_Model == "SEC")
                                {
                                    if (Out(nLP_ES[nPortNum]) && Out(nLP_HO_AVBL[nPortNum]))
                                    {
                                        nOHTcycle[nPortNum] = (int)eMainCycle.NETWORK_VALID;
                                        On(nLP_L_REQ[nPortNum]);
                                        bLPLoadPro[nPortNum] = true;
                                        bLPUnLoadPro[nPortNum] = false;
                                        strLog = " Carrier Handoff 할 Loadport 설정)";
                                        AddOHTLog(nPortNum, Convert.ToString(eState), strLog);
                                    }
                                }
                                else if (str_OHT_Model == "SSEM")
                                {
                                    if (!In(nLP_ABORT[nPortNum]))
                                    {
                                        nOHTcycle[nPortNum] = (int)eMainCycle.NETWORK_VALID;
                                        On(nLP_L_REQ[nPortNum]);
                                        bLPLoadPro[nPortNum] = true;
                                        bLPUnLoadPro[nPortNum] = false;
                                        strLog = " Carrier Handoff 할 Loadport 설정)";
                                        AddOHTLog(nPortNum, Convert.ToString(eState), strLog);
                                    }
                                }
                                IsCheckSensorLogic(nPortNum, "TP1", eState, sw[nPortNum], m_eFlag[nPortNum]);
                            }
                        }
                        if ((In(nLP_CS_0[nPortNum]) || In(nLP_CS_1[nPortNum])) && In(nLP_VALID[nPortNum]))
                        {
                            if (IsCheckCST(nPortNum) == eCSTState.On)
                            {
                                sw[nPortNum].Start();
                                if (str_OHT_Model == "SEC")
                                {
                                    if (Out(nLP_ES[nPortNum]) && Out(nLP_HO_AVBL[nPortNum]))
                                    {
                                        nOHTcycle[nPortNum] = (int)eMainCycle.NETWORK_VALID;
                                        On(nLP_U_REQ[nPortNum]);
                                        bLPLoadPro[nPortNum] = false;
                                        bLPUnLoadPro[nPortNum] = true;
                                        strLog = " Carrier Handoff 할 Loadport 설정)";
                                        AddOHTLog(nPortNum, Convert.ToString(eState), strLog);
                                    }
                                }
                                else if (str_OHT_Model == "SSEM")
                                {
                                    if (!In(nLP_ABORT[nPortNum]))
                                    {
                                        nOHTcycle[nPortNum] = (int)eMainCycle.NETWORK_VALID;
                                        On(nLP_U_REQ[nPortNum]);
                                        bLPLoadPro[nPortNum] = false;
                                        bLPUnLoadPro[nPortNum] = true;
                                        strLog = " Carrier Handoff 할 Loadport 설정)";
                                        AddOHTLog(nPortNum, Convert.ToString(eState), strLog);
                                    }
                                }
                            }
                        }
                        if (CheckChattering(nPortNum))
                        {
                            IsCheckSensorLogic(nPortNum, "TP1", eState, sw[nPortNum], m_eFlag[nPortNum]);

                            if (!(In(nLP_CS_0[nPortNum]) && !In(nLP_CS_1[nPortNum])) || In(nLP_VALID[nPortNum]))
                            {
                                nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                strLog = " TP1 Illegal Sequence(" + IO_Off_Check(nPortNum, eState) + " signal was turned OFF Improperly)";
                                AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                LPErr[nPortNum] = true;
                                ShowErrState(nPortNum, eState);

                            }
                            else if (Out(nLP_BUSY[nPortNum]) || In(nLP_COMPT[nPortNum]))
                            {
                                nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                strLog = " TP1 Illegal Sequence(" + IO_On_Check(nPortNum, eState) + " signal was turned On Improperly)";
                                AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                LPErr[nPortNum] = true;
                                ShowErrState(nPortNum, eState);
                            }
                        }
                        break;

                    case eMainCycle.NETWORK_VALID:
                        if ((In(nLP_CS_0[nPortNum]) || In(nLP_CS_1[nPortNum])) && In(nLP_VALID[nPortNum]) && Out(nLP_L_REQ[nPortNum]))
                        {
                            sw[nPortNum].Start();
                            nOHTcycle[nPortNum] = (int)eMainCycle.LOAD_REQUEST;
                            m_eFlag[nPortNum] = eLoadUnloadFlag.load_Sequence;
                            strLog = " 인터페이스 통신이 유효함을 나타냄";
                            AddOHTLog(nPortNum, Convert.ToString(eState), strLog);
                            sw[nPortNum].Start();
                        }
                        else if ((In(nLP_CS_0[nPortNum]) || In(nLP_CS_1[nPortNum])) && In(nLP_VALID[nPortNum]) && Out(nLP_U_REQ[nPortNum]))
                        {
                            sw[nPortNum].Start();
                            nOHTcycle[nPortNum] = (int)eMainCycle.UNLOAD_REQUEST;
                            m_eFlag[nPortNum] = eLoadUnloadFlag.Unload_Sequence;
                            strLog = " 인터페이스 통신이 유효함을 나타냄";
                            AddOHTLog(nPortNum, Convert.ToString(eState), strLog);
                            sw[nPortNum].Start();
                        }
                        if (CheckChattering(nPortNum))
                        {
                            IsCheckSensorLogic(nPortNum, "TP1", eState, sw[nPortNum], m_eFlag[nPortNum]);

                            if ((!In(nLP_CS_0[nPortNum]) && !In(nLP_CS_1[nPortNum])) || !In(nLP_VALID[nPortNum]))
                            {
                                nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                strLog = " TP1 Illegal Sequence(" + IO_Off_Check(nPortNum, eState) + " signal was turned OFF Improperly)";
                                AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                LPErr[nPortNum] = true;
                                ShowErrState(nPortNum, eState);
                            }
                            else if (In(nLP_BUSY[nPortNum]) || In(nLP_COMPT[nPortNum]))
                            {
                                nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                strLog = " TP1 Illegal Sequence(" + IO_On_Check(nPortNum, eState) + " signal was turned On Improperly)";
                                AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                LPErr[nPortNum] = true;
                                ShowErrState(nPortNum, eState);
                            }
                        }
                        break;
                    //------------------------------------------------------------loadport1 Load/Unload process--------------------------------------------------------------------------------------
                    case eMainCycle.LOAD_REQUEST:
                        if ((In(nLP_CS_0[nPortNum]) || In(nLP_CS_1[nPortNum])) && In(nLP_VALID[nPortNum]) && Out(nLP_L_REQ[nPortNum]) && In(nLP_TR_REQ[nPortNum]))
                        {
                            sw[nPortNum].Start();
                            nOHTcycle[nPortNum] = (int)eMainCycle.TR_REQ;
                            On(nLP_READY[nPortNum]);
                            strLog = " 해당Loadport가carrier를 Load할 준비가 되었음을 나타냄.";
                            AddOHTLog(nPortNum, Convert.ToString(eState), strLog);
                        }
                        if (CheckChattering(nPortNum))
                        {
                            IsCheckSensorLogic(nPortNum, "TP1", eState, sw[nPortNum], m_eFlag[nPortNum]);

                            if ((!In(nLP_CS_0[nPortNum]) && !In(nLP_CS_1[nPortNum])) || !In(nLP_VALID[nPortNum]))
                            {
                                nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                strLog = " TP1 Illegal Sequence(" + IO_Off_Check(nPortNum, eState) + " signal was turned OFF Improperly)";
                                AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                LPErr[nPortNum] = true;
                                ShowErrState(nPortNum, eState);
                            }
                            else if (In(nLP_BUSY[nPortNum]) || In(nLP_COMPT[nPortNum]))
                            {
                                nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                strLog = " TP1 Illegal Sequence(" + IO_On_Check(nPortNum, eState) + " signal was turned On Improperly)";
                                AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                LPErr[nPortNum] = true;
                                ShowErrState(nPortNum, eState);
                            }
                            else if (sw[nPortNum].Check() > nTP[1])                     //공통 사양
                            {
                                nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                strLog = "TP1 TimeOut Error: Trigger Request Input Signal is off";
                                AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                ShowErrState(nPortNum, eState);
                            }
                        }
                        break;
                    case eMainCycle.UNLOAD_REQUEST:
                        if ((In(nLP_CS_0[nPortNum]) || In(nLP_CS_1[nPortNum])) && In(nLP_VALID[nPortNum]) && Out(nLP_U_REQ[nPortNum]) && In(nLP_TR_REQ[nPortNum]))
                        {
                            sw[nPortNum].Start();
                            nOHTcycle[nPortNum] = (int)eMainCycle.TR_REQ;
                            On(nLP_READY[nPortNum]);
                            strLog = " Loadport1이 carrier를 UnLoad할 준비가 되었음을 나타냄.";
                            AddOHTLog(nPortNum, Convert.ToString(eState), strLog);
                        }
                        if (CheckChattering(nPortNum))
                        {
                            IsCheckSensorLogic(nPortNum, "TP1", eState, sw[nPortNum], m_eFlag[nPortNum]);

                            if ((!In(nLP_CS_0[nPortNum]) && !In(nLP_CS_1[nPortNum])) || !In(nLP_VALID[nPortNum]))
                            {
                                nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                strLog = " TP1 Illegal Sequence(" + IO_Off_Check(nPortNum, eState) + " signal was turned OFF Improperly)";
                                AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                LPErr[nPortNum] = true;
                                ShowErrState(nPortNum, eState);
                            }
                            else if (In(nLP_BUSY[nPortNum]) || In(nLP_COMPT[nPortNum]))
                            {
                                nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                strLog = " TP1 Illegal Sequence(" + IO_On_Check(nPortNum, eState) + " signal was turned On Improperly)";
                                AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                LPErr[nPortNum] = true;
                                ShowErrState(nPortNum, eState);
                            }
                            else if (sw[nPortNum].Check() > nTP[1])                     //공통 사양
                            {
                                nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                strLog = "TP1 Timeout (TR_REQ signal did not turn ON within specified time.)";
                                AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                ShowErrState(nPortNum, eState);
                            }
                        }
                        break;
                    case eMainCycle.TR_REQ:
                        if ((In(nLP_CS_0[nPortNum]) || In(nLP_CS_1[nPortNum])) && In(nLP_VALID[nPortNum]) && (Out(nLP_L_REQ[nPortNum]) || Out(nLP_U_REQ[nPortNum])) && In(nLP_TR_REQ[nPortNum]) && Out(nLP_READY[nPortNum]))
                        {
                            sw[nPortNum].Start();
                            nOHTcycle[nPortNum] = (int)eMainCycle.READY;
                            strLog = " OHT 에서 Handoff를 요청.";
                            AddOHTLog(nPortNum, Convert.ToString(eState), strLog);
                            sw[nPortNum].Start();
                        }
                        else if (CheckChattering(nPortNum))
                        {
                            IsCheckSensorLogic(nPortNum, "TP2", eState, sw[nPortNum], m_eFlag[nPortNum]);

                            if ((!In(nLP_CS_0[nPortNum]) && !In(nLP_CS_1[nPortNum])) || !In(nLP_VALID[nPortNum]) || !In(nLP_TR_REQ[nPortNum]))
                            {
                                nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                strLog = " TP2 Illegal Sequence(" + IO_Off_Check(nPortNum, eState) + " signal was turned OFF Improperly)";
                                AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                LPErr[nPortNum] = true;
                                ShowErrState(nPortNum, eState);
                            }
                            else if (In(nLP_COMPT[nPortNum]))
                            {
                                nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                strLog = " TP2 Illegal Sequence(" + IO_On_Check(nPortNum, eState) + " signal was turned On Improperly)";
                                AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                LPErr[nPortNum] = true;
                                ShowErrState(nPortNum, eState);
                            }
                        }
                        break;
                    case eMainCycle.READY:
                        if ((In(nLP_CS_0[nPortNum]) || In(nLP_CS_1[nPortNum])) && In(nLP_VALID[nPortNum]) && (Out(nLP_L_REQ[nPortNum]) || Out(nLP_U_REQ[nPortNum])) && In(nLP_TR_REQ[nPortNum]) && Out(nLP_READY[nPortNum]) && In(nLP_BUSY[nPortNum]))
                        {
                            sw[nPortNum].Start();
                            nOHTcycle[nPortNum] = (int)eMainCycle.BUSY_START;
                            strLog = " ATI 장비로 Host에서 전송요청 허가";
                            AddOHTLog(nPortNum, Convert.ToString(eState), strLog);
                            sw[nPortNum].Start();
                        }
                        if (CheckChattering(nPortNum))
                        {
                            IsCheckSensorLogic(nPortNum, "TP2", eState, sw[nPortNum], m_eFlag[nPortNum]);

                            if ((!In(nLP_CS_0[nPortNum]) && !In(nLP_CS_1[nPortNum])) || !In(nLP_VALID[nPortNum]) || !In(nLP_TR_REQ[nPortNum]))
                            {
                                nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                strLog = " TP2 Illegal Sequence(" + IO_Off_Check(nPortNum, eState) + " signal was turned OFF Improperly)";
                                AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                LPErr[nPortNum] = true;
                                ShowErrState(nPortNum, eState);
                            }
                            else if (In(nLP_COMPT[nPortNum]))
                            {
                                nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                strLog = " TP2 Illegal Sequence(" + IO_On_Check(nPortNum, eState) + " signal was turned On Improperly)";
                                AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                LPErr[nPortNum] = true;
                                ShowErrState(nPortNum, eState);
                            }
                            if (str_OHT_Model == "SEC")
                            {
                                if (sw[nPortNum].Check() > nTP[2])
                                {
                                    nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                    strLog = "TP2 Timeout (BUSY signal did not turn ON within specified time.)";
                                    AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                    ShowErrState(nPortNum, eState);
                                }
                            }
                            else if (str_OHT_Model == "SSEM")
                            {
                                if (sw[nPortNum].Check() > nTP[3])
                                {
                                    nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                    strLog = "TP3 TimeOut Error :Load or Unload Req Input Signal is off";
                                    AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                    ShowErrState(nPortNum, eState);
                                }
                            }
                        }
                        break;
                    case eMainCycle.BUSY_START:
                        if (m_eFlag[nPortNum] == eLoadUnloadFlag.load_Sequence && IsCheckCST(nPortNum) == eCSTState.On)
                        {
                            strLog = " Handoff가 Host로 인해진행되고있음.";
                            AddOHTLog(nPortNum, Convert.ToString(eState), strLog);
                            sw[nPortNum].Start();
                            bBusy[nPortNum] = true;
                            nOHTcycle[nPortNum] = (int)eMainCycle.CSTStateChange;
                        }
                        else if (m_eFlag[nPortNum] == eLoadUnloadFlag.Unload_Sequence && IsCheckCST(nPortNum) == eCSTState.Off)
                        {
                            strLog = " Handoff가 Host로 인해진행되고있음.";
                            AddOHTLog(nPortNum, Convert.ToString(eState), strLog);
                            sw[nPortNum].Start();
                            bBusy[nPortNum] = true;
                            nOHTcycle[nPortNum] = (int)eMainCycle.CSTStateChange;
                        }
                        if (CheckChattering(nPortNum))
                        {
                            IsCheckSensorLogic(nPortNum, "TP3", eState, sw[nPortNum], m_eFlag[nPortNum]);

                            if (str_OHT_Model == "SEC")
                            {
                                if (sw[nPortNum].Check() > nTP[3])
                                {
                                    nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                    if (m_eFlag[nPortNum] == eLoadUnloadFlag.load_Sequence)
                                    {
                                        strLog = "TP3 Timeout (Carrier was not detected within specified time.)";
                                        AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                        LPErr[nPortNum] = true;
                                    }
                                    else if (m_eFlag[nPortNum] == eLoadUnloadFlag.Unload_Sequence)
                                    {
                                        strLog = "TP3 Timeout (Carrier was not removed within specified time.)";
                                        AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                        LPErr[nPortNum] = true;
                                    }
                                    ShowErrState(nPortNum, eState);
                                }
                            }
                            if ((!In(nLP_CS_0[nPortNum]) && !In(nLP_CS_1[nPortNum])) || !In(nLP_VALID[nPortNum]) || !In(nLP_TR_REQ[nPortNum]) || !In(nLP_BUSY[nPortNum]))
                            {
                                nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                strLog = " TP3 Illegal Sequence(" + IO_Off_Check(nPortNum, eState) + " signal was turned OFF Improperly)";
                                AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                LPErr[nPortNum] = true;
                                ShowErrState(nPortNum, eState);
                            }
                            else if (In(nLP_COMPT[nPortNum]))
                            {
                                nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                strLog = " TP3 Illegal Sequence(" + IO_On_Check(nPortNum, eState) + " signal was turned On Improperly)";
                                AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                LPErr[nPortNum] = true;
                                ShowErrState(nPortNum, eState);
                            }

                        }
                        break;
                    case eMainCycle.CSTStateChange:
                        if (m_eFlag[nPortNum] == eLoadUnloadFlag.load_Sequence && IsCheckCST(nPortNum) == eCSTState.On)
                        {
                            Off(nLP_L_REQ[nPortNum]);
                            nOHTcycle[nPortNum] = (int)eMainCycle.BUSY_END;
                            sw[nPortNum].Start();
                        }
                        else if (m_eFlag[nPortNum] == eLoadUnloadFlag.Unload_Sequence && IsCheckCST(nPortNum) == eCSTState.Off)
                        {
                            Off(nLP_U_REQ[nPortNum]);
                            nOHTcycle[nPortNum] = (int)eMainCycle.BUSY_END;
                            sw[nPortNum].Start();
                        }
                        if (CheckChattering(nPortNum))
                        {
                            IsCheckSensorLogic(nPortNum, "TP3", eState, sw[nPortNum], m_eFlag[nPortNum]);

                            if ((!In(nLP_CS_0[nPortNum]) && !In(nLP_CS_1[nPortNum])) || !In(nLP_VALID[nPortNum]) || !In(nLP_TR_REQ[nPortNum]) || !In(nLP_BUSY[nPortNum]))
                            {
                                nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                strLog = " TP3 Illegal Sequence(" + IO_Off_Check(nPortNum, eState) + " signal was turned OFF Improperly)";
                                AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                LPErr[nPortNum] = true;
                                ShowErrState(nPortNum, eState);
                            }
                            else if (In(nLP_COMPT[nPortNum]))
                            {
                                nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                strLog = " TP3 Illegal Sequence(" + IO_On_Check(nPortNum, eState) + " signal was turned On Improperly)";
                                AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                LPErr[nPortNum] = true;
                                ShowErrState(nPortNum, eState);
                            }
                        }
                        break;
                    case eMainCycle.BUSY_END:
                        if (bBusy[nPortNum])
                        {
                            if ((In(nLP_CS_0[nPortNum]) || In(nLP_CS_1[nPortNum])) && In(nLP_VALID[nPortNum]) && Out(nLP_READY[nPortNum]) && In(nLP_COMPT[nPortNum]) && !In(nLP_BUSY[nPortNum]) && !In(nLP_TR_REQ[nPortNum]))
                            {
                                if (bLPLoadPro[nPortNum] && !bLPUnLoadPro[nPortNum] && IsCheckCST(nPortNum) == eCSTState.On)
                                {
                                    sw[nPortNum].Start();
                                    nOHTcycle[nPortNum] = (int)eMainCycle.COMPLETE;
                                    sw[nPortNum].Stop();
                                    strLog = " Handoff가 Host로 인해완료됨.";
                                    AddOHTLog(nPortNum, Convert.ToString(eState), strLog);
                                    bBusy[nPortNum] = false;
                                    bLPCompleteKey[nPortNum] = true;
                                }
                                else if (!bLPLoadPro[nPortNum] && bLPUnLoadPro[nPortNum] && IsCheckCST(nPortNum) == eCSTState.Off)
                                {
                                    sw[nPortNum].Start();
                                    nOHTcycle[nPortNum] = (int)eMainCycle.COMPLETE;
                                    sw[nPortNum].Stop();
                                    strLog = " Handoff가 Host로 인해완료됨.";
                                    AddOHTLog(nPortNum, Convert.ToString(eState), strLog);
                                    Off(nLP_READY[nPortNum]);
                                    bBusy[nPortNum] = false;
                                    bLPCompleteKey[nPortNum] = true;
                                }
                            }

                            IsCheckSensorLogic(nPortNum, "TP4", eState, sw[nPortNum], m_eFlag[nPortNum]);

                            if (CheckChattering(nPortNum))
                            {
                                if (str_OHT_Model == "SEC")
                                {
                                    if (Out(nLP_L_REQ[nPortNum]) || Out(nLP_U_REQ[nPortNum]))
                                    {
                                        if (sw[nPortNum].Check() > nTP[3])
                                        {
                                            nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                            strLog = "TP3 TimeOut Error: Load or UnLoad Req Input Signal is On";
                                            AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                            ShowErrState(nPortNum, eState);
                                            LPErr[nPortNum] = true;

                                        }
                                    }
                                    if (sw[nPortNum].Check() > nTP[4])
                                    {
                                        if (Out(nLP_BUSY[nPortNum]))
                                        {
                                            nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                            strLog = "TP4 TimeOut Error: BUSY Input Signal is On";
                                            AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                            ShowErrState(nPortNum, eState);
                                            LPErr[nPortNum] = true;
                                        }
                                        if (In(nLP_TR_REQ[nPortNum]))
                                        {
                                            nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                            strLog = "TP4 TimeOut Error: TR_REQ Input Signal is On";
                                            AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                            ShowErrState(nPortNum, eState);
                                            LPErr[nPortNum] = true;
                                        }
                                        if (!In(nLP_COMPT[nPortNum]))
                                        {
                                            nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                            strLog = "TP4 TimeOut Error: COMPT Input Signal is On";
                                            AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                            ShowErrState(nPortNum, eState);
                                            LPErr[nPortNum] = true;
                                        }
                                    }
                                }
                                if ((!In(nLP_CS_0[nPortNum]) && !In(nLP_CS_1[nPortNum])) || !In(nLP_VALID[nPortNum]))
                                {
                                    nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                    strLog = " TP4 Illegal Sequence(" + IO_On_Check(nPortNum, eState) + " signal was turned OFF Improperly)";
                                    AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                    LPErr[nPortNum] = true;
                                    ShowErrState(nPortNum, eState);
                                }
                            }
                        }

                        break;
                    case eMainCycle.COMPLETE:
                        if (!Out(nLP_BUSY[nPortNum]) && !In(nLP_TR_REQ[nPortNum]))
                        {
                            sw[nPortNum].Start();
                            Off(nLP_READY[nPortNum]);
                            nOHTcycle[nPortNum] = (int)eMainCycle.PROCESSEND;
                            strLog = " Host가 Handoff가 완료되었음을 알려줌.";
                            AddOHTLog(nPortNum, Convert.ToString(eState), strLog);
                            sw[nPortNum].Start();
                        }

                        IsCheckSensorLogic(nPortNum, "TP5", eState, sw[nPortNum], m_eFlag[nPortNum]);

                        if (CheckChattering(nPortNum))
                        {
                            if (str_OHT_Model == "SEC")
                            {
                                if (sw[nPortNum].Check() > nTP[5])
                                {

                                    if (In(nLP_COMPT[nPortNum]))
                                    {
                                        nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                        strLog = "TP5 Timeout (COMPT signal did not turn OFF within specified time.)";
                                        AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                        LPErr[nPortNum] = true;
                                        ShowErrState(nPortNum, eState);
                                    }
                                    else if (In(nLP_CS_0[nPortNum]) || In(nLP_CS_1[nPortNum]))
                                    {
                                        nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                        strLog = "TP5 Timeout (CS_0 signal did not turn OFF within specified time.)";
                                        AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                        LPErr[nPortNum] = true;
                                        ShowErrState(nPortNum, eState);
                                    }
                                    else if (In(nLP_VALID[nPortNum]))
                                    {
                                        nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                        strLog = "TP5 Timeout (VALID signal did not turn OFF within specified time.)";
                                        AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                        LPErr[nPortNum] = true;
                                        ShowErrState(nPortNum, eState);
                                    }
                                }
                            }

                            if (In(nLP_BUSY[nPortNum]) || In(nLP_TR_REQ[nPortNum]))
                            {
                                nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                strLog = " TP5 Illegal Sequence(" + IO_On_Check(nPortNum, eState) + " signal was turned On Improperly)";
                                AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                LPErr[nPortNum] = true;
                                ShowErrState(nPortNum, eState);
                            }
                        }
                        break;

                    case eMainCycle.PROCESSEND:
                        if (IsAll_In_Off(nPortNum) && IsAll_Out_Off(nPortNum))
                        {
                            //Thread.Sleep(5000);
                            nOHTcycle[nPortNum] = (int)eMainCycle.Init;
                            strLog = " 프로세스 정상적으로 종료.";
                            AddOHTLog(nPortNum, Convert.ToString(eState), strLog);
                            bLPLoadPro[nPortNum] = false;
                            bLPUnLoadPro[nPortNum] = false;
                            sw[nPortNum].Start();
                            if (m_auto.m_bXgemUse && m_eFlag[nPortNum] == eLoadUnloadFlag.Unload_Sequence)
                            {
                                m_auto.ClassXGem().SetLPCarrierOnOff(nPortNum, false);
                                m_auto.ClassXGem().SetLPPresentSensor(nPortNum, false);
                                m_auto.ClassXGem().SetCMSTransferState(XGem300Carrier.eLPTransfer.ReadyToLoad, nPortNum);
                                //  m_Loadport[nPortNum].m_infoCarrier.m_eState
                            }
                            else if (m_auto.m_bXgemUse && m_eFlag[nPortNum] == eLoadUnloadFlag.load_Sequence)
                            {
                                IsCSTOK[nPortNum] = true;
                            }


                        }
                        if (CheckChattering(nPortNum))
                        {
                            if (In(nLP_BUSY[nPortNum]) || In(nLP_TR_REQ[nPortNum]))
                            {
                                nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                strLog = " TP5 Illegal Sequence(" + IO_On_Check(nPortNum, eState) + " signal was turned On Improperly)";
                                AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                LPErr[nPortNum] = true;
                                ShowErrState(nPortNum, eState);
                            }

                            if (str_OHT_Model == "SEC")
                            {
                                if (sw[nPortNum].Check() > nTP[5])
                                {
                                    nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                    strLog = "TP5 TimeOut Error: All Input Signal is not Off";
                                    //strLog = "Process:" + Convert.ToString(eState) + "Input OFF Error: 시퀀스 동작중" + IO_Off_Check(nPortNum, eState) + "신호가 ON 되었습니다.";
                                    AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                    LPErr[nPortNum] = true;
                                    ShowErrState(nPortNum, eState);
                                }
                            }
                            else if (str_OHT_Model == "SSEM")
                            {
                                if (sw[nPortNum].Check() > nTP[6])
                                {
                                    nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                                    strLog = "TP6 TimeOut Error: All Input Signal is not Off";
                                    //strLog = "Process:" + Convert.ToString(eState) + "Input OFF Error: 시퀀스 동작중" + IO_Off_Check(nPortNum, eState) + "신호가 ON 되었습니다.";
                                    AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                                    LPErr[nPortNum] = true;
                                    ShowErrState(nPortNum, eState);
                                }
                            }
                        }
                        break;
                    case eMainCycle.Error:
                        LPReSetKeyEnable(nPortNum);
                        break;
                }
            }
        }

        public string IO_Off_Check(int nNum, eMainCycle eProcess)                        //필요없는 IO 켜졋는지 확인
        {
            string str = "";
            if (eProcess == eMainCycle.CASSETTE_SELECT || eProcess == eMainCycle.NETWORK_VALID || eProcess == eMainCycle.LOAD_REQUEST || eProcess == eMainCycle.UNLOAD_REQUEST)         //TP1 감시 구간
            {
                if (!In(nLP_CS_0[nNum])) str = str + " CS_0 ";
                if (!In(nLP_VALID[nNum])) str = str + " VALID ";
            }
            else if (eProcess == eMainCycle.TR_REQ || eProcess == eMainCycle.READY)                                                                                                     //TP2 감시 구간
            {
                if (!In(nLP_CS_0[nNum])) str = str + " CS_0";
                if (!In(nLP_VALID[nNum])) str = str + " VALID";
                if (!In(nLP_TR_REQ[nNum])) str = str + " TR_REQ";
            }
            else if (eProcess == eMainCycle.BUSY_START || eProcess == eMainCycle.CSTStateChange)                                                                                        //TP3 감시 구간
            {
                if (!In(nLP_CS_0[nNum])) str = str + " CS_0";
                if (!In(nLP_VALID[nNum])) str = str + " VALID";
                if (!In(nLP_TR_REQ[nNum])) str = str + " TR_REQ";
                if (!In(nLP_BUSY[nNum])) str = str + " BUSY";
            }
            else if (eProcess == eMainCycle.BUSY_END)                                                                                                                                   //TP4 감시 구간
            {
                if (!In(nLP_CS_0[nNum])) str = str + " CS_0";
                if (!In(nLP_VALID[nNum])) str = str + " VALID";
                if (!In(nLP_TR_REQ[nNum])) str = str + " TR_REQ";
                if (!Out(nLP_READY[nNum])) str = str + " READY";
            }
            return str;                                                                                                                                                                 //TP5 감시 구간 없음
        }


        public string IO_On_Check(int nNum, eMainCycle eProcess)                       //필요한 IO 꺼졋는지 확인
        {
            string str = "";
            if (eProcess == eMainCycle.CASSETTE_SELECT || eProcess == eMainCycle.NETWORK_VALID || eProcess == eMainCycle.LOAD_REQUEST || eProcess == eMainCycle.UNLOAD_REQUEST)         //TP1 감시 구간
            {
                if (In(nLP_COMPT[nNum])) str = str + " COMPT ";
                if (In(nLP_BUSY[nNum])) str = str + " BUSY ";
            }
            else if (eProcess == eMainCycle.TR_REQ || eProcess == eMainCycle.READY)                                                                                                     //TP2 감시 구간
            {
                if (In(nLP_COMPT[nNum])) str = str + " COMPT ";
            }
            else if (eProcess == eMainCycle.BUSY_START || eProcess == eMainCycle.CSTStateChange)                                                                                        //TP3 감시 구간
            {
                if (In(nLP_COMPT[nNum])) str = str + " COMPT ";
            }
            else if (eProcess == eMainCycle.BUSY_END)                                                                                                                                   //TP4 감시 구간 없음
            {

            }
            else if (eProcess == eMainCycle.COMPLETE || eProcess == eMainCycle.PROCESSEND)                                                                                              //TP5 감시 구간
            {
                if (In(nLP_TR_REQ[nNum])) str = str + " TR_REQ ";
                if (In(nLP_BUSY[nNum])) str = str + " BUSY ";
            }
            return str;
        }

        private void bLP1Status_Click(object sender, EventArgs e)                       //LoadPort1 상태 Manual<-->Auto 변경
        {
            if (bLP1Status.Text == "Manual")
            {
                bool bError = false;
                if (In(nLP_CS_0[0]))
                {
                    AddOHTLog(0, "", "CS_0 signal was turned ON improperly.", true);
                    bError = true;
                }
                if (In(nLP_VALID[0]))
                {
                    AddOHTLog(0, "", "VALID signal was turned ON improperly.", true);
                    bError = true;
                }
                if (In(nLP_TR_REQ[0]))
                {
                    AddOHTLog(0, "", "TR_REQ signal was turned ON improperly.", true);
                    bError = true;
                }
                if (In(nLP_BUSY[0]))
                {
                    AddOHTLog(0, "", "BUSY signal was turned ON improperly.", true);
                    bError = true;
                }
                if (In(nLP_COMPT[0]))
                {
                    AddOHTLog(0, "", "COMPT signal was turned ON improperly.", true);
                    bError = true;
                }

                if (bError)
                {
                    LPErr[0] = true;
                    ShowErrState(0);
                    nOHTcycle[0] = (int)eMainCycle.Error;
                }

                m_auto.ClassWork().bLPCommStatus[0] = true;
                m_log.m_reg.Write("OHT_LP1_Status", true);
                if (m_auto.m_strOHTType != "PIOTEST") m_auto.ClassXGem().LPAccessModeChange(XGem300Carrier.ePortAccess.Auto, "LP1");
                else bLP1Status.Text = "Auto";
            }
            else if (bLP1Status.Text == "Auto")
            {
                m_auto.ClassWork().bLPCommStatus[0] = false;
                m_log.m_reg.Write("OHT_LP1_Status", false);
                if (m_auto.m_strOHTType != "PIOTEST") m_auto.ClassXGem().LPAccessModeChange(XGem300Carrier.ePortAccess.Manual, "LP1");
                else bLP1Status.Text = "Manual";
            }
        }

        private void bLP2Status_Click(object sender, EventArgs e)                       //LoadPort2 상태 Manual<-->Auto 변경
        {
            if (bLP2Status.Text == "Manual")
            {
                bool bError = false;
                if (In(nLP_CS_0[1]))
                {
                    AddOHTLog(1, "", "CS_0 signal was turned ON improperly.", true);
                    bError = true;
                }
                if (In(nLP_VALID[1]))
                {
                    AddOHTLog(1, "", "VALID signal was turned ON improperly.", true);
                    bError = true;
                }
                if (In(nLP_TR_REQ[1]))
                {
                    AddOHTLog(1, "", "TR_REQ signal was turned ON improperly.", true);
                    bError = true;
                }
                if (In(nLP_BUSY[1]))
                {
                    AddOHTLog(1, "", "BUSY signal was turned ON improperly.", true);
                    bError = true;
                }
                if (In(nLP_COMPT[1]))
                {
                    AddOHTLog(1, "", "COMPT signal was turned ON improperly.", true);
                    bError = true;
                }

                if (bError)
                {
                    LPErr[1] = true;
                    ShowErrState(1);
                    nOHTcycle[1] = (int)eMainCycle.Error;
                }

                m_auto.ClassWork().bLPCommStatus[1] = true;
                m_log.m_reg.Write("OHT_LP2_Status", true);
                if (m_auto.m_strOHTType != "PIOTEST") m_auto.ClassXGem().LPAccessModeChange(XGem300Carrier.ePortAccess.Auto, "LP2");
                else bLP2Status.Text = "Auto";
            }
            else if (bLP2Status.Text == "Auto")
            {
                m_auto.ClassWork().bLPCommStatus[1] = false;
                m_log.m_reg.Write("OHT_LP2_Status", false);
                if (m_auto.m_strOHTType != "PIOTEST") m_auto.ClassXGem().LPAccessModeChange(XGem300Carrier.ePortAccess.Manual, "LP2");
                else bLP2Status.Text = "Manual";
            }
        }
        private void bLP3Status_Click(object sender, EventArgs e)
        {
            if (bLP3Status.Text == "Manual")
            {
                bool bError = false;
                if (In(nLP_CS_0[2]))
                {
                    AddOHTLog(2, "", "CS_0 signal was turned ON improperly.", true);
                    bError = true;
                }
                if (In(nLP_VALID[2]))
                {
                    AddOHTLog(2, "", "VALID signal was turned ON improperly.", true);
                    bError = true;
                }
                if (In(nLP_TR_REQ[2]))
                {
                    AddOHTLog(2, "", "TR_REQ signal was turned ON improperly.", true);
                    bError = true;
                }
                if (In(nLP_BUSY[2]))
                {
                    AddOHTLog(2, "", "BUSY signal was turned ON improperly.", true);
                    bError = true;
                }
                if (In(nLP_COMPT[2]))
                {
                    AddOHTLog(2, "", "COMPT signal was turned ON improperly.", true);
                    bError = true;
                }
                if (bError)
                {
                    LPErr[2] = true;
                    ShowErrState(2);
                    nOHTcycle[2] = (int)eMainCycle.Error;
                }

                m_auto.ClassWork().bLPCommStatus[2] = true;
                m_log.m_reg.Write("OHT_LP3_Status", true);
                if (m_auto.m_strOHTType == "PIOTEST") bLP3Status.Text = "Auto";
                else m_auto.ClassXGem().LPAccessModeChange(XGem300Carrier.ePortAccess.Auto, "LP3"); 
            }
            else if (bLP3Status.Text == "Auto")
            {
                m_auto.ClassWork().bLPCommStatus[2] = false;
                m_log.m_reg.Write("OHT_LP3_Status", false);
                if (m_auto.m_strOHTType == "PIOTEST") bLP3Status.Text = "Manual";
                else m_auto.ClassXGem().LPAccessModeChange(XGem300Carrier.ePortAccess.Manual, "LP3");
            }
        }
        public void UpdatePortState(int nPortNum)
        {
            if (m_auto.ClassXGem().m_aXGem300Carrier != null)
            {
                switch (m_auto.ClassXGem().m_aXGem300Carrier[nPortNum].m_ePortAccess)
                {
                    case XGem300Carrier.ePortAccess.Manual:
                        m_auto.ClassWork().bLPCommStatus[nPortNum] = false;
                        if (nPortNum == 0) bLP1Status.Text = "Manual";
                        else if (nPortNum == 1) bLP2Status.Text = "Manual";
                        else if (nPortNum == 2) bLP3Status.Text = "Manual";
                        break;
                    case XGem300Carrier.ePortAccess.Auto:
                        m_auto.ClassWork().bLPCommStatus[nPortNum] = true;
                        if (nPortNum == 0) bLP1Status.Text = "Auto";
                        else if (nPortNum == 1) bLP2Status.Text = "Auto";
                        else if (nPortNum == 2) bLP3Status.Text = "Auto";
                        break;
                }
            }
        }
        private void bTPDelay_Click(object sender, EventArgs e)                         //TimeOut Delay 값 입력
        {
            for (int i = 1; i < 7; i++)
            {
                try
                {
                    int nTP = Convert.ToInt32(tbTP[i].Text);
                    m_log.m_reg.Write("TP" + Convert.ToString(i), nTP);
                }
                catch
                {
                    MessageBox.Show("TP" + Convert.ToString(i) + "기입란에 알맞은 숫자를 넣어주세요");
                }
            }
            m_log.m_reg.Write("Chattering_Time", nTChat);
            m_log.m_reg.Write("Sensor_Logic_Time", nTSL);
        }
        public void ShowErrState(int nPortNum, eMainCycle eState = eMainCycle.Init)                     //TimeOut Error 출력
        {
            lbState.Text = "Error";
            lbdetail.Text = "Process:LoadPort" + Convert.ToString(nPortNum) + Convert.ToString(eState);
            this.BackColor = Color.Red;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < nPortNum; i++)
            {
                LPReSetKeyEnable(i);
            }
        }
        public void LPIOInit(int nNum)                                                  //해당 LoadPort IO 초기화
        {
            for (int i = LPIOStart[nNum]; i <= LPIOEnd[nNum]; i++)
            {
                if (str_OHT_IOType == "AJIN")
                {
                    Off(m_auto.nInStartNum + i);
                    Off(m_auto.nOutStartNum + i);
                }
                else if (str_OHT_IOType == "EIP")
                {
                    Off(m_auto.nInStartNum + LPInitStartNum_EIP[nNum] + i);
                }
                else if (str_OHT_IOType == "PIOTEST")
                {
                    m_bOutput_Test[i] = false;
                }
            }
        }

        private void btnLP1Reset_Click(object sender, EventArgs e)                      //Reset
        {
            if (IsAll_In_Off(0))
            {
                nOHTcycle[0] = (int)eMainCycle.Init;
                lbState.Text = "";
                lbdetail.Text = "";
                this.BackColor = Color.WhiteSmoke;
                LPIOInit(0);
                LPErr[0] = false;
                btnLP1Reset.Enabled = false;
                btnLP1Reset.BackgroundImage = null;
            }
            else
            {
                MessageBox.Show("InPut이 먼저 초기화가 되야합니다.");
            }
        }

        private void btnLP2Reset_Click(object sender, EventArgs e)                     //Reset
        {
            if (IsAll_In_Off(1))
            {
                nOHTcycle[1] = (int)eMainCycle.Init;
                lbState.Text = "";
                lbdetail.Text = "";
                this.BackColor = Color.WhiteSmoke;
                LPIOInit(1);
                LPErr[1] = false;
                btnLP2Reset.Enabled = false;
                btnLP2Reset.BackgroundImage = null;
            }
            else
            {
                MessageBox.Show("InPut이 먼저 초기화가 되야합니다.");
            }
        }
        private void btnLP3Reset_Click(object sender, EventArgs e)
        {
            if (IsAll_In_Off(2))
            {
                nOHTcycle[2] = (int)eMainCycle.Init;
                lbState.Text = "";
                lbdetail.Text = "";
                this.BackColor = Color.WhiteSmoke;
                LPIOInit(2);
                LPErr[2] = false;
                btnLP3Reset.Enabled = false;
                btnLP3Reset.BackgroundImage = null;
            }
            else
            {
                MessageBox.Show("InPut이 먼저 초기화가 되야합니다.");
            }
        }
        public void LPReSetKeyEnable(int nPortNum)                                  // Complete Key<-->Retry Key 상황에따라 변경
        {
            if (bLPCompleteKey[nPortNum])
            {
                if (nPortNum == 0)
                {
                    btnLP1Reset.Enabled = true;
                    btnLP1Reset.Text = "CompleteKey";
                }
                else if (nPortNum == 1)
                {
                    btnLP2Reset.Enabled = true;
                    btnLP2Reset.Text = "CompleteKey";
                }
                else if (nPortNum == 2)
                {
                    btnLP3Reset.Enabled = true;
                    btnLP3Reset.Text = "CompleteKey";
                }
            }
            else if (!bLPCompleteKey[nPortNum])
            {
                if (nPortNum == 0)
                {
                    btnLP1Reset.Enabled = true;
                    btnLP1Reset.Text = "RetryKey";
                }
                else if (nPortNum == 1)
                {
                    btnLP2Reset.Enabled = true;
                    btnLP2Reset.Text = "RetryKey";
                }
                else if (nPortNum == 2)
                {
                    btnLP3Reset.Enabled = true;
                    btnLP3Reset.Text = "RetryKey";
                }
            }
        }
        public void AddOHTLog(int nPortNum, string sPro, string str, bool bErrorList = false)          //ErrorList 에 Log 남김          
        {
            string strList = "";
            string strLog = "";

            strLog = "[LoadPort" + Convert.ToString(nPortNum) + "]" + " Process: " + sPro + " -> " + str;
            m_log.Add(strLog);
            if (bErrorList)
            {
                strList = DateTime.Now.ToString("yy/MM/dd HH:mm:ss") + " [LoadPort" + Convert.ToString(nPortNum + 1) + "]: " + str;
                lbErrorList.Items.Insert(0, strList);
            }
        }
        public void IsHo_AVBL(int nNum)                                                    // Ho_AVBL On/Off
        {
            if (m_auto.ClassWork().GetState() == eWorkRun.Run && !LPErr[nNum] && IsReadyLP(nNum) && m_auto.ClassWork().bLPCommStatus[nNum] && !IsLightCurtainErr())
            {
                m_auto.ClassControl().WriteOutputBit(nLP_HO_AVBL[nNum], true);
            }
            else
            {
                m_auto.ClassControl().WriteOutputBit(nLP_HO_AVBL[nNum], false);
            }
        }
        public void IsES(int nNum)                       //EMO 추가                       // ES On/Off
        {
            if (!IsLightCurtainErr() && !m_auto.ClassWork().IsEMGError())
            {
                m_auto.ClassControl().WriteOutputBit(nLP_ES[nNum], true);
            }
            else
            {
                m_auto.ClassControl().WriteOutputBit(nLP_ES[nNum], false);
            }
        }
        public bool IsLightCurtainErr()                                                     //LightCurtainSensor 감지확인
        {
            bool bResult = false;
            if (m_auto == null) return bResult;
            if (m_auto.ClassWork().bLightCurtainErr)
            {
                bResult = true;
                if (m_auto.ClassWork().bLPCommStatus[0] || m_auto.ClassWork().bLPCommStatus[1] || m_auto.ClassWork().bLPCommStatus[2])
                {

                    bCkLP1.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.LightCurtain;
                    bCkLP2.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.LightCurtain;
                    bCkLP3.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.LightCurtain;
                }
            }
            else
            {
                bResult = false;
            }
            return bResult;
        }
        public void IsABORT(int nNum)                                                       //ABORT On/Off
        {
            if (m_auto.ClassWork().GetState() == eWorkRun.Error || m_auto.ClassWork().bLightCurtainErr)
            {
                m_auto.ClassControl().WriteOutputBit(nLP_ABORT[nNum], true);
            }
            else
            {
                m_auto.ClassControl().WriteOutputBit(nLP_ABORT[nNum], false);
            }
        }
        public void CSErr_OnProcessing(int nPortNum)                                        //Process 진행중에 Cassette 감지 상태 작업자에 의해 변경되는지 확인(확인될시 Error발생)
        {
            bool bCheck = false;

            if (((nOHTcycle[nPortNum] == (int)eMainCycle.Init) || (nOHTcycle[nPortNum] == (int)eMainCycle.Loadstate)) && m_auto.ClassWork().bLPCommStatus[nPortNum])
            {
                if (m_auto.m_strOHTType == "PIOTEST") bCheck = bLPCheck_Test[nPortNum];
                else bCheck = bLPPlaceCheck[nPortNum];

                if (!bCheck)
                {
                    if (IsCheckCST(nPortNum) == eCSTState.On)
                    {
                        nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                        AddOHTLog(nPortNum, nOHTcycle[nPortNum].ToString(), "Cassette is detected On LoadProcessing", true);
                        LPErr[nPortNum] = true;
                    }
                }
                else if (bCheck)
                {
                    if (IsCheckCST(nPortNum) == eCSTState.Off)
                    {
                        nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                        AddOHTLog(nPortNum, nOHTcycle[nPortNum].ToString(), "Cassette is not detected On UnLoadProcessing", true);
                        LPErr[nPortNum] = true;
                    }
                }
            }
        }
        public void UpdateOHTIO(int IONum)                                                      //Site 에따른 IO Update 
        {
            if (str_OHT_Model == "SSEM")
            {
                m_sInput[IONum] = m_SSEMInput[IONum];
                m_sOutput[IONum] = m_SSEMOutput[IONum];
            }
            else if (str_OHT_Model == "SEC")
            {
                m_sInput[IONum] = m_SECInput[IONum];
                m_sOutput[IONum] = m_SECOutput[IONum];
            }

        }

        private void OHT_EFEM_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void MainCycleTimer_Tick_1(object sender, EventArgs e)
        {

            OHTProcess(0);
            OHTProcess(1);
            OHTProcess(2);
            for (int i = 0; i < nPortNum; i++)
            {
                DisplayIO(i);
                StatusUpdate(i);
                CSErr_OnProcessing(i);
                UpdatePortState(i);
            }
            IsLightCurtainErr();
        }

        bool m_bRun = false;

        public bool CheckChattering(int nPortNum)
        {
            bool bResult = false;
            if (sw[nPortNum].Check() > nTChat)
            {
                bResult = true;
            }
            else
            {
                bResult = false;
            }
            return bResult;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            sw[1].Start();
        }

        public void IsCheckSensorLogic(int nPortNum, string TPNum, eMainCycle eState, ezStopWatch eSW, eLoadUnloadFlag CheckState)
        {
            bool bError = false;
            if (eSW.Check() > nTSL)
            {
                if (CheckState == eLoadUnloadFlag.load_Sequence)
                {
                    if (eState <= eMainCycle.CSTStateChange)
                    {
                        if (IsCheckCST(nPortNum) == eCSTState.On) bError = true;
                    }
                    else
                    {
                        if (IsCheckCST(nPortNum) == eCSTState.Off) bError = true;
                    }

                    if (IsCheckCST(nPortNum) == eCSTState.Not_Placement) bError = true;
                    if (IsCheckCST(nPortNum) == eCSTState.Not_Present) bError = true;
                }
                else if (CheckState == eLoadUnloadFlag.Unload_Sequence)
                {
                    if (eState <= eMainCycle.CSTStateChange)
                    {
                        if (IsCheckCST(nPortNum) == eCSTState.Off) bError = true;
                    }
                    else
                    {
                        if (IsCheckCST(nPortNum) == eCSTState.On) bError = true;
                    }

                    if (IsCheckCST(nPortNum) == eCSTState.Not_Placement) bError = true;
                    if (IsCheckCST(nPortNum) == eCSTState.Not_Present) bError = true;
                }
                else if (CheckState == eLoadUnloadFlag.None)
                {
                    if (IsCheckCST(nPortNum) == eCSTState.Not_Placement) bError = true;
                    if (IsCheckCST(nPortNum) == eCSTState.Not_Present) bError = true;
                }
            }
            if (bError)
            {
                nOHTcycle[nPortNum] = (int)eMainCycle.Error;
                strLog = TPNum + " Sensor Logic (Carrier is placed incorrectly. Remove this or load stable.)";
                AddOHTLog(nPortNum, Convert.ToString(eState), strLog, true);
                LPErr[nPortNum] = true;
                ShowErrState(nPortNum, eState);
            }
        }

        public eCSTState IsCheckCST(int nPortNum)
        {
            eCSTState eRst = eCSTState.Off;
            if (m_auto.m_strOHTType == "PIOTEST")
            {
                if (bLPPlaceCheck[nPortNum]) eRst = eCSTState.On;
                else if (!bLPPlaceCheck[nPortNum]) eRst = eCSTState.Off;
            }
            else
            {
                if (In(nLPPresent[nPortNum]) && In(nLPPlacement[nPortNum])) eRst = eCSTState.On;
                else if (!In(nLPPresent[nPortNum]) && !In(nLPPlacement[nPortNum])) eRst = eCSTState.Off;
                else if (In(nLPPresent[nPortNum]) && !In(nLPPlacement[nPortNum])) eRst = eCSTState.Not_Placement;
                else if (!In(nLPPresent[nPortNum]) && In(nLPPlacement[nPortNum])) eRst = eCSTState.Not_Present;
            }
            return eRst;
        }
        #region Button Event
        private void bLP1Status_MouseDown(object sender, MouseEventArgs e)
        {
            this.bLP1Status.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Button_PUsh;
        }

        private void bLP1Status_MouseUp(object sender, MouseEventArgs e)
        {
            this.bLP1Status.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Button;
        }

        private void btnLP1Reset_MouseDown(object sender, MouseEventArgs e)
        {
            this.btnLP1Reset.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Button_PUsh;
        }

        private void btnLP1Reset_MouseUp(object sender, MouseEventArgs e)
        {
            this.btnLP1Reset.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Button;
        }

        private void btnLP2Reset_MouseDown(object sender, MouseEventArgs e)
        {
            this.btnLP2Reset.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Button_PUsh;
        }

        private void btnLP2Reset_MouseUp(object sender, MouseEventArgs e)
        {
            this.btnLP2Reset.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Button;
        }

        private void bLP2Status_MouseUp(object sender, MouseEventArgs e)
        {
            this.bLP2Status.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Button;
        }

        private void bLP2Status_MouseDown(object sender, MouseEventArgs e)
        {
            this.bLP2Status.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Button_PUsh;
        }

        private void btnLP3Reset_MouseDown(object sender, MouseEventArgs e)
        {
            this.btnLP3Reset.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Button_PUsh;
        }

        private void btnLP3Reset_MouseUp(object sender, MouseEventArgs e)
        {
            this.btnLP3Reset.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Button;
        }
        private void bLP3Status_MouseDown(object sender, MouseEventArgs e)
        {
            this.bLP3Status.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Button_PUsh;
        }
        private void bLP3Status_MouseUp(object sender, MouseEventArgs e)
        {
            this.bLP3Status.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Button;
        }
        private void OHT_EFEM_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            int i = Convert.ToInt32(btn.Tag);
            if (str_OHT_IOType == "PIOTEST")
            {
                if (m_bInput_Test[i]) m_bInput_Test[i] = false;
                else m_bInput_Test[i] = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            int i = Convert.ToInt32(btn.Tag);
            if (str_OHT_IOType == "PIOTEST")
            {
                if (m_bOutput_Test[i]) m_bOutput_Test[i] = false;
                else m_bOutput_Test[i] = true;
            }
        }
        private void bCkLP1_Click(object sender, EventArgs e)
        {
            if (m_auto.m_strOHTType == "PIOTEST")
            {
                if (bLPCheck_Test[0]) bLPCheck_Test[0] = false;
                else bLPCheck_Test[0] = true;
            }
        }
        private void bCkLP2_Click(object sender, EventArgs e)
        {
            if (m_auto.m_strOHTType == "PIOTEST")
            {
                if (bLPCheck_Test[1]) bLPCheck_Test[1] = false;
                else bLPCheck_Test[1] = true;
            }
        }
        private void bCkLP3_Click(object sender, EventArgs e)
        {
            if (m_auto.m_strOHTType == "PIOTEST")
            {
                if (bLPCheck_Test[2]) bLPCheck_Test[2] = false;
                else bLPCheck_Test[2] = true;
            }
        }
    }
        #endregion
}
