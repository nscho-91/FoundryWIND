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
using WeifenLuo.WinFormsUI.Docking;
using ezTools; 


namespace ezAutoMom
{
    public partial class Display_Mom : DockContent 
    {
        public string m_id;
        public Log m_log;
        public Control_Mom m_control;
        public Work_Mom m_work;

        public Display_Mom()
        {
            InitializeComponent();
        }

        public virtual void Init(string id, object auto)
        {
            this.Text = m_id = id;
            m_log = new Log(id, ((Auto_Mom)auto).ClassLogView()); 
            m_control = ((Auto_Mom)auto).ClassControl();
            m_work = ((Auto_Mom)auto).ClassWork();
        }

        public virtual void ShowChild() { }
        public virtual void RunEmergency() { }
        public virtual void RunInit() { }
        protected virtual void RunTimer() { }
        
        public virtual HW_Picker_Mom ClassPicker(int nPicker) { return null; }

        private void timer_Tick(object sender, EventArgs e)
        {
            RunTimer(); 
        }

        private void Display_Mom_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
