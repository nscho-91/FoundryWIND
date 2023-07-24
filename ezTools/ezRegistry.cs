using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32; 

namespace ezTools
{
    public class ezRegistry
    {
        bool m_bInit = false;
        RegistryKey m_reg;

        public ezRegistry(string strModel, string strGroup)
        {
            m_bInit = true; 
            m_reg = Registry.CurrentUser.CreateSubKey("Software").CreateSubKey(strModel).CreateSubKey(strGroup);
        }

        public void Read(string strSub, ref bool b)
        {
            object obj;
            if (!m_bInit) return; 
            obj = m_reg.GetValue(strSub, b.ToString());
            b = bool.Parse(obj.ToString());
        }

        public void Read(string strSub, ref uint n)
        {
            int i;
            i = (int)n;
            Read(strSub, ref i);
            n = (uint)i; 
        }

        public void Read(string strSub, ref int n)
        {
            object obj; 
            if (!m_bInit) return;
            obj = m_reg.GetValue(strSub, n);
            n = int.Parse(obj.ToString());
        }

        public int Read(string strSub, int n)
        {
            object obj;
            if (!m_bInit) return n;
            obj = m_reg.GetValue(strSub, n);
            return int.Parse(obj.ToString());
        }

        public void Read(string strSub, ref double f)
        {
            object obj;
            if (!m_bInit) return;
            obj = m_reg.GetValue(strSub, f);
            f = double.Parse(obj.ToString());
        }

        public void Read(string strSub, ref string str)
        {
            object obj;
            if (!m_bInit) return;
            obj = m_reg.GetValue(strSub, str);
            str = (string)obj;
        }

        public void Write(string strSub, object obj)
        {
            if (!m_bInit) return;
            m_reg.SetValue(strSub, obj.ToString());
        }

        public void Read(string strSub, ref CPoint cp)
        {
            if (!m_bInit) return;
            Read(strSub + ".x", ref cp.x);
            Read(strSub + ".y", ref cp.y);
        }

        public void Write(string strSub, CPoint cp)
        {
            if (!m_bInit) return;
            m_reg.SetValue(strSub + ".x", cp.x);
            m_reg.SetValue(strSub + ".y", cp.y);
        }

        public void Read(string strSub, ref RPoint rp)
        {
            if (!m_bInit) return;
            Read(strSub + ".x", ref rp.x);
            Read(strSub + ".y", ref rp.y);
        }

        public void Write(string strSub, RPoint rp)
        {
            if (!m_bInit) return;
            m_reg.SetValue(strSub + ".x", rp.x);
            m_reg.SetValue(strSub + ".y", rp.y);
        }
    }
}
