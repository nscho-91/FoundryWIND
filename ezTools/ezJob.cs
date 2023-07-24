using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ezTools
{
    class ezJob_Sub
    {
        public string m_strSub, m_strType, m_strVal; 

        public ezJob_Sub(string strSub, string strType, string strVal)
        {
            m_strSub = strSub; m_strType = strType; m_strVal = strVal; 
        }

    }
    
    class ezJob_Group
    {
        public string m_strGroup;
        ArrayList m_listSub = new ArrayList(); 

        public ezJob_Group(string strGroup)
        {
            m_strGroup = strGroup; 
        }

        public int Add(string strSub, string strType, string strVal)
        {
            ezJob_Sub sub = new ezJob_Sub(strSub, strType, strVal);
            if (Find(strSub) != null) return 1;
            m_listSub.Add(sub); return 0;
        }

        public ezJob_Sub Find(string strSub)
        {
            foreach (ezJob_Sub sub in m_listSub)
            {
                if (strSub == sub.m_strSub) return sub; 
            }
            return null;
        }

    }

    public class ezJob
    {
        bool m_bSave;
        public string m_strFile, m_strTitle; 
        StreamReader m_sr;
        StreamWriter m_sw; 
        ArrayList m_listGroup = new ArrayList(); 

        public ezJob(string strFile, bool bSave)
        {
            m_bSave = bSave; 
            m_strFile = strFile; 
            GetTitle(m_strFile);
            try
            {
                if (bSave) m_sw = new StreamWriter(new FileStream(strFile, FileMode.Create));
                else
                {
                    FileStream fs = new FileStream(strFile, FileMode.Open);
                    m_sr = new StreamReader(fs);
                    m_listGroup.Clear(); 
                    ReadFile();
                }
            }
            catch (Exception) 
            {
                m_sw = null;
                m_sr = null;
            }
        }

        public ezJob(string strFile, bool bSave, int nLine)
        {
            m_bSave = bSave; 
            m_strFile = strFile; 
            GetTitle(m_strFile);
            try
            {
                if (bSave) m_sw = new StreamWriter(new FileStream(strFile, FileMode.Create));
                else
                {
                    FileStream fs = new FileStream(strFile, FileMode.Open);
                    m_sr = new StreamReader(fs);
                    m_listGroup.Clear(); 
                    ReadRCPData(nLine);
                }
            }
            catch (Exception)
            {
                m_sw = null;
                m_sr = null;
            }
        }

        public void Close()
        {
            try
            {
                if (m_bSave)
                {
                    if (m_sw == null) return;
                    m_sw.Close();
                }
                else 
                {
                    if (m_sr == null) return;
                    m_sr.Close(); m_listGroup.Clear(); 
                }
            }
            catch (Exception)
            {
            }
        }

        void GetTitle(string strFile)
        {
            string[] strLines = strFile.Split(new char[] { '.' });
            m_strTitle = strLines[0];
        }

        void ReadFile()
        {
            string strLine; string[] strLines;
            strLine = m_sr.ReadLine(); 
            while (strLine != null)
            {
                strLines = strLine.Split(new char[] { ',' });
                Add(strLines[0], strLines[1], strLines[2], strLines[3]);
                strLine = m_sr.ReadLine(); 
            }
        }

        void ReadRCPData(int nLine)
        {
            int n = 0;
            string strLine; string[] strLines;
            strLine = m_sr.ReadLine();
            while (strLine != null)
            {
                if (n >= nLine) return;
                strLines = strLine.Split(new char[] { ',' });
                Add(strLines[0], strLines[1], strLines[2], strLines[3]);
                strLine = m_sr.ReadLine();
                n++;
            }
        }

        void Add(string strGroup, string strSub, string strType, string strVal)
        {
            ezJob_Group group = Find(strGroup);
            if (group == null) group = new ezJob_Group(strGroup);
            group.Add(strSub, strType, strVal);
            m_listGroup.Add(group); 
        }

        ezJob_Group Find(string strGroup)
        {
            foreach (ezJob_Group group in m_listGroup)
            {
                if (strGroup == group.m_strGroup) return group;
            }
            return null;
        }

        ezJob_Sub Find(string strGroup, string strSub)
        {
            ezJob_Group group = Find(strGroup);
            if (group == null) return null;
            return group.Find(strSub); 
        }

        public void Write(string strGroup, string strSub, bool b)
        {
            m_sw.Write(strGroup + ',' + strSub);
            m_sw.Write(",bool,");
            m_sw.WriteLine(b);
        }

        public void Read(string strGroup, string strSub, ref bool b)
        {
            ezJob_Sub sub = Find(strGroup, strSub);
            if (sub == null) return; 
            if (sub.m_strType != "bool") return;
            if (sub.m_strVal == "True") b = true; else b = false; 
        }

        public void Write(string strGroup, string strSub, uint n)
        {
            m_sw.Write(strGroup + ',' + strSub);
            m_sw.Write(",int,");
            m_sw.WriteLine(n);
        }

        public void Write(string strGroup, string strSub, int n)
        {
            m_sw.Write(strGroup + ',' + strSub);
            m_sw.Write(",int,");
            m_sw.WriteLine(n);
        }

        public void Read(string strGroup, string strSub, ref uint n)
        {
            int i;
            i = (int)n;
            Read(strGroup, strSub, ref i);
            n = (uint)i;
        }

        public void Read(string strGroup, string strSub, ref int n)
        {
            ezJob_Sub sub = Find(strGroup, strSub);
            if (sub == null) return; 
            if (sub.m_strType != "int") return;
            n = Convert.ToInt32(sub.m_strVal); 
        }

        public void Write(string strGroup, string strSub, double f)
        {
            m_sw.Write(strGroup + ',' + strSub);
            m_sw.Write(",double,");
            m_sw.WriteLine(f);
        }

        public void Read(string strGroup, string strSub, ref double f)
        {
            ezJob_Sub sub = Find(strGroup, strSub);
            if (sub == null) return; 
            if (sub.m_strType != "double") return;
            f = Convert.ToDouble(sub.m_strVal);
        }

        public void Write(string strGroup, string strSub, string str)
        {
            m_sw.Write(strGroup + ',' + strSub);
            m_sw.Write(",string,");
            m_sw.WriteLine(str);
        }

        public void Read(string strGroup, string strSub, ref string str)
        {
            ezJob_Sub sub = Find(strGroup, strSub);
            if (sub == null) return; 
            if (sub.m_strType != "string") return;
            str = sub.m_strVal;
        }

        public void Write(string strGroup, string strSub, CPoint cp)
        {
            Write(strGroup, strSub + ".x", cp.x);
            Write(strGroup, strSub + ".y", cp.y); 
        }

        public void Read(string strGroup, string strSub, ref CPoint cp)
        {
            Read(strGroup, strSub + ".x", ref cp.x);
            Read(strGroup, strSub + ".y", ref cp.y); 
        }

        public void Write(string strGroup, string strSub, RPoint rp)
        {
            Write(strGroup, strSub + ".x", rp.x);
            Write(strGroup, strSub + ".y", rp.y); 
        }

        public void Read(string strGroup, string strSub, ref RPoint rp)
        {
            Read(strGroup, strSub + ".x", ref rp.x);
            Read(strGroup, strSub + ".y", ref rp.y);
        }
    }
}
