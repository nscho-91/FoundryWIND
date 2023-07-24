using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using ezAutoMom;
using ezTools;
using System.IO; 

namespace ezAuto_EFEM
{
    public partial class UI_RunData : DockContent
    {
        Auto_Mom m_auto = null;
        private string sLogPath = @"C:\Log\RunData";

        public UI_RunData()
        {
            InitializeComponent();

        }
        
        public void ShowPanel(DockPanel dockpanel)
        {
            this.Show(dockpanel);
        }

        public void Init(Auto_Mom auto)
        {
            m_auto = auto;

            DirectoryInfo di = new DirectoryInfo(sLogPath);

            if (di.Exists == false) {
                di.Create();
                MakeRunDataFile(sLogPath);      //Run Data
                MakeStopDataFile(sLogPath);     //Stop Data
            }
        }


        public void MakeRunDataFile(string sLogPath)
        {
            string sRunPath = sLogPath + @"\WIND_RunData.csv";
            // create one file gridview.csv in writing mode using streamwriter
            StreamWriter sw = new StreamWriter(sRunPath, true);
            // now add the gridview header in csv file suffix with "," delimeter except last one

            for (int i = 0; i < RunGridView.Columns.Count; i++)     //Run Data 내용이 정리되는 Grid View에 List 내용 입력 하기
            {
                sw.Write(RunGridView.Columns[i].HeaderText);
                if (i != RunGridView.Columns.Count) {
                    sw.Write(",");
                }
            }

            // add new line
            sw.Write(sw.NewLine);
            // flush from the buffers.
            sw.Flush();
            // closes the file
            sw.Close();
        }

        public void MakeStopDataFile(string sLogPath)
        {
            string sStopPath = sLogPath + @"\WIND_StopData.csv";
            // create one file gridview.csv in writing mode using streamwriter
            StreamWriter sw = new StreamWriter(sStopPath, true);
            // now add the gridview header in csv file suffix with "," delimeter except last one

            for (int i = 0; i < JamGridView.Columns.Count; i++)     //Stop Data 내용이 정리되는 Grid View에 List 내용 입력 하기
            {
                sw.Write(JamGridView.Columns[i].HeaderText);
                if (i != JamGridView.Columns.Count) {
                    sw.Write(",");
                }
            }
            // add new line
            sw.Write(sw.NewLine);
            // flush from the buffers.
            sw.Flush();
            // closes the file
            sw.Close();
        }

        delegate void DelegateSetStringItem(string sDate, string sLotID, string sPartNo, string sInTime, string sOutTime, string sTotalTime, string sRunTime, string sStopTime, string sUPEH, string sLoadQty);

        public void AddRunData(string sDate, string sLotID, string sPartNo, string sInTime, string sOutTime, string sTotalTime, string sRunTime, string sStopTime, string sUPEH, string sLoadQty)       //Run Data Log 남기기
        {
            if (this.InvokeRequired)
            {
                DelegateSetStringItem d = new DelegateSetStringItem(AddRunData);
                this.Invoke(d, new object[] {  sDate,  sLotID,  sPartNo,  sInTime,  sOutTime,  sTotalTime,  sRunTime,  sStopTime,  sUPEH,  sLoadQty});
            }
            else
            {
                bool bFileCheck = false;
                sLogPath = @"C:\Log\RunData\" + DateTime.Today.ToString("yyyyMMdd");
                DirectoryInfo di = new DirectoryInfo(sLogPath);

                if (di.Exists == false)     //폴더 없는 경우 만들기
                {
                    di.Create();
                    MakeRunDataFile(sLogPath);
                }

                //RunGridView.Rows.Clear();       //이전에 입력된 Grid View 내용 지우고 다시쓰기위한 작업

                string[] row = new string[] { sDate, sLotID, sPartNo, sInTime, sOutTime, sTotalTime, sRunTime, sStopTime, sUPEH, sLoadQty };
                //RunGridView.Rows.Add(row);      //Grid View에 내용 넣어주기

                string sRunPath = sLogPath + @"\WIND_RunData.csv";

                bFileCheck = File.Exists(sRunPath); // 파일유무 확인

                if (!bFileCheck)        //파일 없으면 다시 Grid View List 내용 넣어서 만들기
                    MakeRunDataFile(sLogPath);

                // create one file gridview.csv in writing mode using streamwriter
                StreamWriter sw = new StreamWriter(sRunPath, true);
                // now add the gridview header in csv file suffix with "," delimeter except last one

                for (int i = 0; i < row.Length; i++)      //Grid View에 입력된 내용 가져오기
                {
                    sw.Write(row[i]);
                    if (i != row.Length)
                        sw.Write(",");
                }
                sw.Write(sw.NewLine);
                // flush from the buffers.
                sw.Flush();
                // closes the file
                sw.Close();
            }
        }
        public void AddStopData(string sDate, string ErrorCode, string sLotID, string sStartTime, string sEndTime, string sStopTime, string sContents)
        {
            bool bFileCheck = false;
            sLogPath = @"C:\Log\RunData\" + DateTime.Today.ToString("yyyyMMdd");

            DirectoryInfo di = new DirectoryInfo(sLogPath);

            if (di.Exists == false) {
                di.Create();
                MakeStopDataFile(sLogPath);
            }

            JamGridView.Rows.Clear();

            string[] row = new string[] { sDate, ErrorCode, sLotID, sStartTime, sEndTime, sStopTime, sContents };
            JamGridView.Rows.Add(row);

            string sStopPath = sLogPath + @"\WIND_StopData.csv";

            bFileCheck = File.Exists(sStopPath); // 파일유무 확인

            if (!bFileCheck)
                MakeStopDataFile(sLogPath);

            // create one file gridview.csv in writing mode using streamwriter
            StreamWriter sw = new StreamWriter(sStopPath, true);

            for (int i = 0; i < JamGridView.RowCount; i++) {
                for (int j = 0; j < JamGridView.Columns.Count; j++) {
                    sw.Write(JamGridView.Rows[i].Cells[j].Value);
                    if (j != JamGridView.Columns.Count) {
                        sw.Write(",");
                    }
                }
                if (i == 0) {
                    // add new line
                    sw.Write(sw.NewLine);
                    // flush from the buffers.
                    sw.Flush();
                    // closes the file
                    sw.Close();
                    break;
                }
            }
        }
    }
}
