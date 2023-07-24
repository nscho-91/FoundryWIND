using System;
using System.Collections; 
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading; 
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ezTools
{
    public partial class ezFileSave : Form
    {
        Queue m_que = new Queue();
        string m_strFile = "";

        public ezFileSave()
        {
            InitializeComponent();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            WriteFile();
        }

        public void Init(string strFile)
        {
            if (!IsReady()) WriteFile();
            m_strFile = strFile;
            CreateFolder();
        }

        public void End()
        {
            while (!IsReady()) Thread.Sleep(10);
        }

        void CreateFolder()
        {
            string[] strFiles = m_strFile.Split('\\'); 
            string strPath = strFiles[0]; 
            for (int n = 1; n < strFiles.Length - 1; n++)
            {
                strPath += strFiles[1];
                Directory.CreateDirectory(strPath); 
            }
        }

        public void Write(string strWrite)
        {
            m_que.Enqueue(strWrite); 
        }

        void WriteFile()
        {
            StreamWriter sw = new StreamWriter(new FileStream(m_strFile, FileMode.Append));
            int l = m_que.Count;
            for (int n = 0; n < l; n++)
            {
                string strWrite = (string)m_que.Dequeue();
                sw.WriteLine(strWrite);
            }
            sw.Close();
            sw.Dispose(); 
        }

        public bool IsReady()
        {
            return (m_que.Count == 0);
        }

    }
}
