using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace spAuto
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string mtxName = "Metashower";
            Mutex mtx = new Mutex(true, mtxName); 
            TimeSpan tsWait = new TimeSpan(0, 0, 1);
            bool success = mtx.WaitOne(tsWait);
            if (!success)
            {
                mtx.ReleaseMutex();
                MessageBox.Show("Program is running !!.");
                return;
            }  
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new spAuto());
            
            mtx.ReleaseMutex();
        }
    }
}
