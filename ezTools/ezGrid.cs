using System;
using System.Collections; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PropertyGridEx;

namespace ezTools
{
    public enum eGrid { eInit, eUpdate, eRegRead, eRegWrite, eJobOpen, eJobSave }

    public class ezGrid
    {
        const char c_sGroup = '>'; 
        public int m_nID = 0;
        bool m_bForeign = false;
        public string m_id; 
        string m_strGroup, m_strProp, m_strSub;
        object m_objOld, m_objNew;
        Queue m_queGroup = new Queue(); 
        Log m_log;
        public ezRegistry m_reg;
        ezJob m_job;
        public eGrid m_mode;
        public PropertyGridEx.PropertyGridEx m_grid;
        public bool m_bInitGrid = false; 

        public ezGrid(string id, PropertyGridEx.PropertyGridEx grid, Log log, bool bForeign)
        {
            m_id = id; m_grid = grid; m_log = log; m_reg = m_log.m_reg; m_bForeign = bForeign; 
        }

        public void Update(eGrid eMode, ezJob job = null)
        {
            m_mode = eMode; m_job = job;
            if (m_mode == eGrid.eInit)
            {
                m_grid.ShowCustomProperties = true;
                m_grid.Item.Clear();
                m_queGroup.Clear(); 
            }
        }

        public void PropertyChange(PropertyValueChangedEventArgs e)
        {
            GridItem gridItem; 
            if (e.ChangedItem.Parent.Parent.Parent == null)
            {
                m_strSub = ""; 
                gridItem = e.ChangedItem;
            }
            else
            {
                m_strSub = e.ChangedItem.Label;
                gridItem = e.ChangedItem.Parent;
            }
            string[] strGroup = gridItem.Parent.Label.Split(c_sGroup);
            m_strGroup = strGroup[1];
            m_strProp = gridItem.Label;
            m_objNew = gridItem.Value;
            m_objOld = e.OldValue;
        }

        string GetGroupName(string strGroup)
        {
            int n = 0; 
            foreach (string sGroup in m_queGroup)
            {
                if (strGroup == sGroup)
                {
                    return n.ToString("00>") + strGroup; 
                }
                n++; 
            }
            m_queGroup.Enqueue(strGroup);
            return n.ToString("00>") + strGroup; 
        }

        public void Set(ref bool b, string strGroup, string strProp, string strDesc, bool bReadOnly = false, bool bVisuable = true)
        {
            switch (m_mode)
            {
                case eGrid.eJobOpen: m_job.Read(m_id, strGroup + strProp, ref b); break;
                case eGrid.eJobSave: m_job.Write(m_id, strGroup + strProp, b); break;
                case eGrid.eRegRead: m_reg.Read(strGroup + strProp, ref b); break;
                case eGrid.eRegWrite: m_reg.Write(strGroup + strProp, b); break;
                case eGrid.eInit: strGroup = GetID(strGroup); strProp = GetID(strProp);
                    m_grid.Item.Add(strProp, b, bReadOnly, GetGroupName(strGroup), strDesc, bVisuable); break;
                case eGrid.eUpdate: strGroup = GetID(strGroup); strProp = GetID(strProp); 
                    if ((strGroup == m_strGroup) && (strProp == m_strProp))
                    {
                        m_log.Add(strGroup + strProp + m_strSub + " : " + b.ToString() + " -> " + m_objNew.ToString());
                        b = (bool)m_objNew;
                    } break;
            }
        }

        public void Set(ref uint l, string strGroup, string strProp, string strDesc, bool bReadOnly = false, bool bVisuable = true)
        {
            int i = (int)l;
            Set(ref i, strGroup, strProp, strDesc, bReadOnly, bVisuable);
            l = (uint)i; 
        }

        public int Set(int l, string strGroup, string strProp, string strDesc, bool bReadOnly = false, bool bVisuable = true)
        {
            Set(ref l, strGroup, strProp, strDesc, bReadOnly, bVisuable); 
            return l;
        }

        public void Set(ref int l, string strGroup, string strProp, string strDesc, bool bReadOnly = false, bool bVisuable = true)
        {
            switch (m_mode)
            {
                case eGrid.eJobOpen: m_job.Read(m_id, strGroup + strProp, ref l); break;
                case eGrid.eJobSave: m_job.Write(m_id, strGroup + strProp, l); break;
                case eGrid.eRegRead: m_reg.Read(strGroup + strProp, ref l); break;
                case eGrid.eRegWrite: m_reg.Write(strGroup + strProp, l); break;
                case eGrid.eInit: strGroup = GetID(strGroup); strProp = GetID(strProp);
                    m_grid.Item.Add(strProp, l, bReadOnly, GetGroupName(strGroup), strDesc, bVisuable); break;
                case eGrid.eUpdate: strGroup = GetID(strGroup); strProp = GetID(strProp); 
                    if ((strGroup == m_strGroup) && (strProp == m_strProp))
                    {
                        m_log.Add(strGroup + strProp + m_strSub + " : " + l.ToString() + " -> " + m_objNew.ToString());
                        l = (int)m_objNew;
                    } break;
            }
        }

        public void Set(ref double f, string strGroup, string strProp, string strDesc, bool bReadOnly = false, bool bVisuable = true)
        {
            switch (m_mode)
            {
                case eGrid.eJobOpen: m_job.Read(m_id, strGroup + strProp, ref f); break;
                case eGrid.eJobSave: m_job.Write(m_id, strGroup + strProp, f); break;
                case eGrid.eRegRead: m_reg.Read(strGroup + strProp, ref f); break;
                case eGrid.eRegWrite: m_reg.Write(strGroup + strProp, f); break;
                case eGrid.eInit: strGroup = GetID(strGroup); strProp = GetID(strProp);
                    m_grid.Item.Add(strProp, f, bReadOnly, GetGroupName(strGroup), strDesc, bVisuable); break;
                case eGrid.eUpdate: strGroup = GetID(strGroup); strProp = GetID(strProp); 
                    if ((strGroup == m_strGroup) && (strProp == m_strProp))
                    {
                        m_log.Add(strGroup + strProp + m_strSub + " : " + f.ToString() + " -> " + m_objNew.ToString());
                        f = (double)m_objNew;
                    } break;
            }
        }

        public void Set(ref string str, string[] strList, string strGroup, string strProp, string strDesc, bool bReadOnly = false, bool bVisuable = true)
        {
            Set(ref str, strGroup, strProp, strDesc, bReadOnly, bVisuable);
            if (m_mode == eGrid.eInit) m_grid.Item[m_grid.Item.Count - 1].Choices = new CustomChoices(strList, true);
        }

        public void Set(ref string str, string strGroup, string strProp, string strDesc, bool bReadOnly = false, bool bVisuable = true)
        {
            switch (m_mode)
            {
                case eGrid.eJobOpen: m_job.Read(m_id, strGroup + strProp, ref str); break;
                case eGrid.eJobSave: m_job.Write(m_id, strGroup + strProp, str); break;
                case eGrid.eRegRead: m_reg.Read(strGroup + strProp, ref str); break;
                case eGrid.eRegWrite: m_reg.Write(strGroup + strProp, str); break;
                case eGrid.eInit: strGroup = GetID(strGroup); strProp = GetID(strProp);
                    m_grid.Item.Add(strProp, str, bReadOnly, GetGroupName(strGroup), strDesc, bVisuable); break;
                case eGrid.eUpdate: strGroup = GetID(strGroup); strProp = GetID(strProp); 
                    if ((strGroup == m_strGroup) && (strProp == m_strProp))
                    {
                        m_log.Add(strGroup + strProp + m_strSub + " : " + str + " -> " + m_objNew.ToString());
                        str = (string)m_objNew;
                    } break;
            }
        }

        public void Set(ref CPoint cp, string strGroup, string strProp, string strDesc, bool bReadOnly = false, bool bVisuable = true)
        {
            CPoint cpOld = cp;
            switch (m_mode)
            {
                case eGrid.eJobOpen: m_job.Read(m_id, strGroup + strProp, ref cp); break;
                case eGrid.eJobSave: m_job.Write(m_id, strGroup + strProp, cp); break;
                case eGrid.eRegRead: m_reg.Read(strGroup + strProp, ref cp); break;
                case eGrid.eRegWrite: m_reg.Write(strGroup + strProp, cp); break;
                case eGrid.eInit: strGroup = GetID(strGroup); strProp = GetID(strProp);
                    m_grid.Item.Add(strProp, cp, bReadOnly, GetGroupName(strGroup), strDesc, bVisuable);
                    m_grid.Item[m_grid.Item.Count - 1].IsBrowsable = true;
                    m_grid.Item[m_grid.Item.Count - 1].BrowsableLabelStyle = BrowsableTypeConverter.LabelStyle.lsNormal; break;
                case eGrid.eUpdate: strGroup = GetID(strGroup); strProp = GetID(strProp); 
                    if ((strGroup == m_strGroup) && (strProp == m_strProp))
                    {
                        if (m_strSub == "X") cpOld.x = (int)m_objOld; else cpOld.y = (int)m_objOld;
                        m_log.Add(strGroup + strProp + m_strSub + " : " + cpOld.ToString() + " -> " + m_objNew.ToString());
                        cp = (CPoint)m_objNew; 
                    } break;
            }
        }

        public void Set(ref RPoint rp, string strGroup, string strProp, string strDesc, bool bReadOnly = false, bool bVisuable = true)
        {
            RPoint rpOld = rp;
            switch (m_mode)
            {
                case eGrid.eJobOpen: m_job.Read(m_id, strGroup + strProp, ref rp); break;
                case eGrid.eJobSave: m_job.Write(m_id, strGroup + strProp, rp); break;
                case eGrid.eRegRead: m_reg.Read(strGroup + strProp, ref rp); break;
                case eGrid.eRegWrite: m_reg.Write(strGroup + strProp, rp); break;
                case eGrid.eInit: strGroup = GetID(strGroup); strProp = GetID(strProp);
                    m_grid.Item.Add(strProp, rp, bReadOnly, GetGroupName(strGroup), strDesc, bVisuable);
                    m_grid.Item[m_grid.Item.Count - 1].IsBrowsable = true;
                    m_grid.Item[m_grid.Item.Count - 1].BrowsableLabelStyle = BrowsableTypeConverter.LabelStyle.lsNormal; break;
                case eGrid.eUpdate: strGroup = GetID(strGroup); strProp = GetID(strProp); 
                    if ((strGroup == m_strGroup) && (strProp == m_strProp))
                    {
                        if (m_strSub == "X") rpOld.x = (double)m_objOld; else rpOld.y = (double)m_objOld;
                        m_log.Add(strGroup + strProp + m_strSub + " : " + rpOld.ToString() + " -> " + m_objNew.ToString());
                        rp = (RPoint)m_objNew;
                    } break;
            }
        }

        ezGrid_Object Set(ezGrid_Object obj, string strGroup, string strProp, string strDesc, bool bReadOnly = false, bool bVisuable = true)
        {
            switch (m_mode)
            {
                case eGrid.eJobOpen: obj.JobOpen(m_job, m_id, strGroup + strProp); break;
                case eGrid.eJobSave: obj.JobSave(m_job, m_id, strGroup + strProp); break;
                case eGrid.eRegRead: obj.RegRead(m_reg, strGroup + strProp); break;
                case eGrid.eRegWrite: obj.RegWrite(m_reg, strGroup + strProp); break;
                case eGrid.eInit: strGroup = GetID(strGroup); strProp = GetID(strProp);
                    m_grid.Item.Add(strProp, obj, bReadOnly, GetGroupName(strGroup), strDesc, bVisuable);
                    m_grid.Item[m_grid.Item.Count - 1].IsBrowsable = true;
                    m_grid.Item[m_grid.Item.Count - 1].BrowsableLabelStyle = obj.GetBrowsableType(); break;
                case eGrid.eUpdate: strGroup = GetID(strGroup); strProp = GetID(strProp);
                    if ((strGroup == m_strGroup) && (strProp == m_strProp)) 
                        obj.Update(m_strGroup + "_" + m_strProp + "_" + m_strSub, m_objOld, m_objNew); break;
            }
            return obj;
        }

        public void Set(ref Light_Data light, string strGroup, string strProp, string strDesc, bool bReadOnly = false, bool bVisuable = true)
        {
            light = (Light_Data)Set(light, strGroup, strProp, strDesc, bReadOnly, bVisuable); 
        }

        public void Set(ref MinMax minmax, string strGroup, string strProp, string strDesc, bool bReadOnly = false, bool bVisuable = true)
        {
            minmax = (MinMax)Set(minmax, strGroup, strProp, strDesc, bReadOnly, bVisuable);
        }

        public void Set(ref LaserParam lpm, string strGroup, string strProp, string strDesc, bool bReadOnly = false, bool bVisuable = true)
        {
            lpm = (LaserParam)Set(lpm, strGroup, strProp, strDesc, bReadOnly, bVisuable);
        }

        public void Refresh()
        {
            if (m_mode == eGrid.eInit) m_grid.Refresh(); 
        }

        public string GetID(string str) 
        {
            string[] strLines; 
            if (!m_bForeign) return str;
            strLines = str.Split(new char[] { '.' });
            switch (strLines.Length)
            {
                case 1: if (m_mode == eGrid.eInit) m_log.AddString(str);  
                    m_reg.Read("ID_" + str, ref str); return str;
                case 2: if (m_mode == eGrid.eInit) m_log.AddString(strLines[1]); 
                    m_reg.Read("ID_" + strLines[1], ref strLines[1]);
                    return strLines[0] + "." + strLines[1];
                case 3: if (m_mode == eGrid.eInit) m_log.AddString(strLines[2]); 
                    m_reg.Read("ID_" + strLines[2], ref strLines[2]);
                    return strLines[0] + "." + strLines[1] + "." + strLines[2];
                default: return str; 
            }
        }

        public void WriteLogString()
        {
            ezRegistry reg = new ezRegistry(m_log.m_strModel, m_strGroup);
            reg.Write("ID_" + m_strProp, (string)m_objNew);
            reg.Write("ID_" + (string)m_objNew, m_strProp); 
        }
    }
}
