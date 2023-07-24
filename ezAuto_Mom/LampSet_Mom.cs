using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ezTools;
using ezAutoMom;

namespace ezAutoMom
{
    public partial class LampSet_Mom : Form
    {

        string[] sState = { "Init", "Ready", "Run(LP1,LP2)", "Run(LP1)", "Run(None)", "Alarm1", "Alarm2", "Alarm3", "Recovery", "Home", "State11", "State12", "State13", "State14", "State15", "State16", "State17", "State18", "State19", "State20" };
        string[] sFunction = { "Red", "Yellow", "Green", "Buzzer", "Interval(ms)", "Use/UnUse" };
        int nOn = 0;
        int nOff = 1;
        int nFlash = 2;
        int nUse = 0;
        int nUnUse = 1;
        int nBuzzerUnUse = 0;
        int nBuzzer1 = 1;
        int nBuzzer2 = 2;
        int nBuzzer3 = 3;
        ezRegistry m_reg = new ezRegistry("spAuto", "LampSetting");
        
        DataGridViewComboBoxCell cLampCell = new DataGridViewComboBoxCell();
        DataGridViewComboBoxCell cIsUseCell = new DataGridViewComboBoxCell();
        DataGridViewComboBoxCell cBuzzerCell = new DataGridViewComboBoxCell();
        public string[,] SetValue = new string[20, 6]       // Red,Yellow,Green,Use/Unuse,Flash Term 순으로.....
        { 

        { "OFF", "OFF", "OFF", "UnUse", "0","UnUse" },              //1.State: Init
        { "OFF", "OFF", "OFF", "UnUse", "0","UnUse" },              //2.State: Ready 
        { "OFF", "OFF", "OFF", "UnUse", "0","UnUse" },              //3.State: Run(LP1:Run,LP2:Run) 
        { "OFF", "OFF", "OFF", "UnUse", "0","UnUse" },              //4.State: Run(LP1:Run,LP2:Ready  or  LP1:Ready,LP2:Run) 
        { "OFF", "OFF", "OFF", "UnUse", "0","UnUse" },              //5.State: Run(LP1:Ready,LP2:Ready) 
        { "OFF", "OFF", "OFF", "UnUse", "0","UnUse" },              //6.State: Alarm1 
        { "OFF", "OFF", "OFF", "UnUse", "0","UnUse" },              //7.State: Alarm2
        { "OFF", "OFF", "OFF", "UnUse", "0","UnUse" },              //8.State: Alarm3 
        { "OFF", "OFF", "OFF", "UnUse", "0","UnUse" },              //9.State: Recovery
        { "OFF", "OFF", "OFF", "UnUse", "0","UnUse" },              //10.State: Null
        { "OFF", "OFF", "OFF", "UnUse", "0","UnUse" },              //11.State: Null
        { "OFF", "OFF", "OFF", "UnUse", "0","UnUse" },              //12.State: Null
        { "OFF", "OFF", "OFF", "UnUse", "0","UnUse" },              //13.State: Null
        { "OFF", "OFF", "OFF", "UnUse", "0","UnUse" },              //14.State: Null
        { "OFF", "OFF", "OFF", "UnUse", "0","UnUse" },              //15.State: Null
        { "OFF", "OFF", "OFF", "UnUse", "0","UnUse" },              //16.State: Null
        { "OFF", "OFF", "OFF", "UnUse", "0","UnUse" },              //17.State: Null
        { "OFF", "OFF", "OFF", "UnUse", "0","UnUse" },              //18.State: Null
        { "OFF", "OFF", "OFF", "UnUse", "0","UnUse" },              //19.State: Null
        { "OFF", "OFF", "OFF", "UnUse", "0","UnUse" }               //20.State: Null

        };
        public LampSet_Mom()
        {
            InitializeComponent();
          
            LampSetupView.RowCount = 20;
            for (int i = 0; i < LampSetupView.RowCount; i++)
            {
                LampSetupView.Rows[i].Cells[0].Value = sState[i];
                LampSetupView.Rows[i].Resizable = DataGridViewTriState.False;
            }
            for (int i = 0; i < LampSetupView.ColumnCount; i++)
            {
                LampSetupView.Columns[i].Resizable = DataGridViewTriState.False;
            }
            cLampCell.Items.Add("ON");
            cLampCell.Items.Add("OFF");
            cLampCell.Items.Add("FLASH");
            cIsUseCell.Items.Add("Use");
            cIsUseCell.Items.Add("UnUse");
            cBuzzerCell.Items.Add("UnUse");
            cBuzzerCell.Items.Add("Buzzer1");
            cBuzzerCell.Items.Add("Buzzer2");
            cBuzzerCell.Items.Add("Buzzer3");
            LampSetupView.AllowUserToAddRows = false;
            SetLoad();
        }

        public void SetLoad()
        {
            for (int i = 0; i <19; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    m_reg.Read(sState[i] + "_" + sFunction[j], ref SetValue[i, j]);

                    if (SetValue[i, j] == "ON")
                    {
                        LampSetupView.Rows[i].Cells[j+1].Value = cLampCell.Items[nOn];
                    }
                    else if (SetValue[i, j] == "OFF")
                    {
                        LampSetupView.Rows[i].Cells[j+1].Value = cLampCell.Items[nOff];
                    }
                    else if (SetValue[i, j] == "FLASH")
                    {
                        LampSetupView.Rows[i].Cells[j+1].Value = cLampCell.Items[nFlash];
                    }

                }
                for (int n = 3; n < 4; n++)
                {
                    m_reg.Read(sState[i] + "_" + sFunction[n], ref SetValue[i, n]);
                    if (SetValue[i, n] == "UnUse")
                    {
                        LampSetupView.Rows[i].Cells[n + 1].Value = cBuzzerCell.Items[nBuzzerUnUse];
                    }
                    else if (SetValue[i, n] == "Buzzer1")
                    {
                        LampSetupView.Rows[i].Cells[n + 1].Value = cBuzzerCell.Items[nBuzzer1];
                    }
                    else if (SetValue[i, n] == "Buzzer2")
                    {
                        LampSetupView.Rows[i].Cells[n + 1].Value = cBuzzerCell.Items[nBuzzer2];
                    }
                    else if (SetValue[i, n] == "Buzzer3")
                    {
                        LampSetupView.Rows[i].Cells[n + 1].Value = cBuzzerCell.Items[nBuzzer3];
                    }
                }
                for (int k = 5; k < 6; k++)
                {
                    m_reg.Read(sState[i] + "_" + sFunction[k], ref SetValue[i, k]);
                    if (SetValue[i, k] == "Use")
                    {
                        LampSetupView.Rows[i].Cells[k+1].Value = cIsUseCell.Items[nUse];
                    }
                    else
                    {
                        LampSetupView.Rows[i].Cells[k+1].Value = cIsUseCell.Items[nUnUse];
                    }
                }
                for (int l = 4; l < 5; l++)
                {
                    m_reg.Read(sState[i] + "_" + sFunction[l], ref SetValue[i, l]);
                    {
                        LampSetupView.Rows[i].Cells[l + 1].Value = SetValue[i, l];
                    }
                }
                for (int k = 5; k < 6; k++)
                {
                    m_reg.Read(sState[i] + "_" + sFunction[k], ref SetValue[i, k]);
                    if (SetValue[i, k] == "Use")
                    {
                        LampSetupView.Rows[i].Cells[k + 1].Value = cIsUseCell.Items[nUse];
                    }
                    else
                    {
                        LampSetupView.Rows[i].Cells[k + 1].Value = cIsUseCell.Items[nUnUse];
                    }
                }
            }
        }
        public void SetSave()
        {
            for (int i = 0; i < 19; i++)
            {
                try
                {
                    for (int j = 0; j < 3; j++)
                    {
                        SetValue[i, j] = LampSetupView.Rows[i].Cells[j + 1].Value.ToString();
                        m_reg.Write(sState[i] + "_" + sFunction[j], SetValue[i, j]);

                    }
                    for (int k = 3; k < 4; k++)
                    {
                        SetValue[i, k] = LampSetupView.Rows[i].Cells[k + 1].Value.ToString();
                        m_reg.Write(sState[i] + "_" + sFunction[k], SetValue[i, k]);

                    }
                    for (int l = 4; l < 5; l++)
                    {
                        SetValue[i, l] = LampSetupView.Rows[i].Cells[l + 1].Value.ToString();
                        m_reg.Write(sState[i] + "_" + sFunction[l], SetValue[i, l]);
                    }
                    for (int n = 5; n < 6; n++)
                    {
                        SetValue[i, n] = LampSetupView.Rows[i].Cells[n + 1].Value.ToString();
                        m_reg.Write(sState[i] + "_" + sFunction[n], SetValue[i, n]);
                    }
                }
                catch(Exception)
                {
                    MessageBox.Show("올바른 값을 넣어주세요.");
                    return;
                }

            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SetSave();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            SetLoad();
        }
        private void LampSetupView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            LampSetupView.EditingControl.KeyPress += new KeyPressEventHandler(textBox1_KeyPress);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) & (Keys)e.KeyChar != Keys.Back & e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }
    }

}
