using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using ezTools;

namespace ezAuto_EFEM
{
    public class HW_DieMap
    {
        Log m_log;
        public CPoint m_cpMap = new CPoint(1, 1);
        public RPoint m_rpPitch = new RPoint(100, 100);
        public Die[,] m_aDie;

        public HW_DieMap()
        {

        }

        public HW_DieMap(HW_DieMap dieMap)
        {
            m_cpMap = dieMap.m_cpMap;
            m_rpPitch = dieMap.m_rpPitch;
            m_aDie = (Die[,])dieMap.m_aDie.Clone();
        }

        public void Init(CPoint cpMap, RPoint rpPitch, Log log)
        {
            Init(cpMap.x, cpMap.y, rpPitch.x, rpPitch.y, log);
        }

        public void Init(int x, int y, double xPitch, double yPitch, Log log)
        {
            Allocate(x, y);
            m_rpPitch.x = xPitch;
            m_rpPitch.y = yPitch;
            m_aDie = new Die[m_cpMap.y, m_cpMap.x];
            for (int n = 0; n < y; n++)
            {
                for (int m = 0; m < x; m++)
                {
                    m_aDie[n, m] = new Die();
                }
            }
            m_log = log;
        }

        public bool SaveJob(string strFile)
        {
            string strLine = "";
            try
            {
                StreamWriter sw = new StreamWriter(strFile);
                if (sw == null)
                {
                    m_log.Popup("<" + strFile + "> Die Map File Is not Saved !!");
                    return true;
                }
                sw.WriteLine("<?xml version=\"1.0\"?>");
                //sw.WriteLine("<Map FormatRevision=\"SEMI G85-0703\" SubstrateId=\"0E815B6\" SubstrateType=\"Wafer\" xmlns:=\"http://www.semi.org\">");
                sw.WriteLine("<Device BinType=\"Decimal\" Columns=\"" + m_cpMap.x.ToString() + "\" DeviceId=\"S15CA02ANA\" DeviceSizeX=\"" + m_rpPitch.x.ToString() 
                    + "\" DeviceSizeY=\"" + m_rpPitch.y.ToString() + "\" FabricationFacility=\"FAB2\" FrameId=\"0E815B6\" LayoutId=\"S15CA02AAA-12-002\"" 
                    + " LotId=\"9878862.002\" NullBin=\"000\" Orientation=\"0\" OriginLocation=\"2\" ProbeLotId=\"9878862.002\" ProductId=\"S15C\" Rows=\"" 
                    + m_cpMap.y.ToString() + "\" Status=\"PROD\" SubstrateNumber=\"6\" SupplierName=\"FAB2\" WaferSize=\"300\">");
                for (int y = 0; y < m_cpMap.y; y++)
                {
                    strLine = "";
                    strLine += "<Row>";
                    for (int x = 0; x < m_cpMap.x; x++)
                    {
                        if (m_aDie[y, x].m_bEnable)
                        {
                            strLine += "001 ";
                        }
                        else
                        {
                            strLine += "000 ";
                        }
                    }
                    strLine = strLine.Remove(strLine.Length - 1, 1);
                    strLine += "</Row>";
                    sw.WriteLine(strLine);
                }
                sw.Close();
            }
            catch
            {
                m_log.Popup("<" + strFile + "> Die Map File Is not Saved !!");
            }
            m_log.Add("<" + strFile + "> Die Map File Is Saved.");
            return false;
        }

        public bool OpenJob(string strFile)
        {
            int nIndex;
            string strName;
            try
            {
                nIndex = strFile.IndexOf('.', strFile.Length - 4);
                if (nIndex > -1)
                {
                    strName = strFile.Remove(0, nIndex);
                    if (strName.ToLower() == ".asc") return ReadASC(strFile);
                    else if (strName.ToLower() == ".xml") return ReadG85XML(strFile); // ing
                    else
                    {
                        m_log.Popup("<" + strFile + "> Die Map File Is not Opened !!");
                        return true;
                    }
                }
                else return ReadG85(strFile);
            }
            catch
            {
                m_log.Popup("<" + strFile + "> Die Map File Is not Opened !!");

            }
            m_log.Add("<" + strFile + "> Die Map File Is Opened.");
            return false;
        }

        public bool Allocate(int x, int y)
        {
            if (m_aDie == null)
            {
                if (x > 0 && y > 0)
                {
                    m_cpMap.x = x;
                    m_cpMap.y = y;
                    m_aDie = new Die[y, x];
                    for (int n = 0; n < y; n++)
                    {
                        for (int m = 0; m < x; m++)
                        {
                            m_aDie[n, m] = new Die();
                        }
                    }
                    return false;
                }
                else return true;
            }
            else
            {
                if (x == m_aDie.GetLength(1) && y == m_aDie.GetLength(0)) return false; // ing need test
                else if (x > 0 && y > 0)
                {
                    m_cpMap.x = x;
                    m_cpMap.y = y;
                    m_aDie = new Die[y, x];
                    for (int n = 0; n < y; n++)
                    {
                        for (int m = 0; m < x; m++)
                        {
                            m_aDie[n, m] = new Die();
                        }
                    }
                    return false;
                }
                else
                {
                    Array.Clear(m_aDie, 0, m_aDie.Length);
                    m_aDie = null;
                    return true;
                }
            }
        }
        
        public bool ReadMapFile()
        {
            int nIndex;
            string strPath, strName;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "G85 Files (*.*)|*.*|ASC Files (*.asc)|*.asc";
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return true;
            strPath = dlg.FileName;
            nIndex = strPath.IndexOf('.', strPath.Length - 4);
            if (nIndex > -1)
            {
                strName = strPath.Remove(0, nIndex);
                if (strName.ToLower() == ".asc") return ReadASC(strPath);
                else if (strName.ToLower() == ".xml") return ReadG85XML(strPath); // ing
                else
                {
                    m_log.Popup("File Format Is Wrong !!");
                    return true;
                }
            }
            else return ReadG85(strPath);
        }

        public bool ReadG85(string strPath)
        {
            int nIndex = 0;
            string strLine, strRow = "";
            string[] strCodes;
            bool bColumn = false, bRow = false, bX = false, bY = false;
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(strPath);
                if (sr == null)
                {
                    m_log.Popup("G85 File Open Fail !!");
                    return true;
                }
                do
                {
                    strLine = sr.ReadLine();
                    if (!GetXMLValue(strLine, "Columns", ref m_cpMap.x)) { bColumn = true; }
                    if (!GetXMLValue(strLine, "Rows", ref m_cpMap.y)) { bRow = true;}
                    if (!GetXMLValue(strLine, "DeviceSizeX", ref m_rpPitch.x)) { bX = true; }
                    if (!GetXMLValue(strLine, "DeviceSizeY", ref m_rpPitch.y)) { bY = true; }
                    if (bColumn && bRow && bX && bY) break;
                } while (strLine != null);
            }
            catch (Exception ex)
            {
                m_log.Popup("Create Die Map Fail !!");
                m_log.Add(ex.Message);
                if (sr != null) sr.Close();
                return true;
            }

            Allocate(m_cpMap.x, m_cpMap.y);
            try
            {
                do
                {
                    strLine = sr.ReadLine();
                    if (!GetXMLRows(strLine, ref strRow))
                    {
                        strCodes = strRow.Split(' ');
                        for (int n = 0; n < strCodes.Length; n++)
                        {
                            if (strCodes[n] == "000") m_aDie[nIndex, n].m_bEnable = false;
                            else m_aDie[nIndex, n].m_bEnable = true;
                        }
                        nIndex++;
                    }
                } while (strLine != null && nIndex < m_aDie.GetLength(0));
            }
            catch (Exception ex)
            {
                m_log.Popup("Create Die Map Fail !!");
                m_log.Add(ex.Message);
                if (sr != null) sr.Close();
                return true;
            }
            m_log.Add("<" +strPath + "> Die Map Created.");
            sr.Close();
            return false;
        }

        public bool GetXMLValue(string strLine, string strItem, ref double fVal)
        {
            int nIndex;
            string strTemp;
            strItem = strItem + "=\"";
            nIndex = strLine.IndexOf(strItem);
            if (nIndex > -1)
            {
                strTemp = strLine.Remove(0, nIndex + strItem.Length);
                nIndex = strTemp.IndexOf("\"");
                if (nIndex > -1)
                {
                    strTemp = strTemp.Remove(nIndex, strTemp.Length - nIndex);
                    try
                    {
                        fVal = Convert.ToDouble(strTemp);
                    }
                    catch
                    {
                        m_log.Add("Can not Change Value : " + strTemp);
                        return true;
                    }
                    return false;
                }
            }
            return true;
        }

        public bool GetXMLValue(string strLine, string strItem, ref int nVal)
        {
            int nIndex;
            string strTemp;
            strItem = strItem + "=\"";
            nIndex = strLine.IndexOf(strItem);
            if (nIndex > -1)
            {
                strTemp = strLine.Remove(0, nIndex + strItem.Length);
                nIndex = strTemp.IndexOf("\"");
                if (nIndex > -1)
                {
                    strTemp = strTemp.Remove(nIndex, strTemp.Length - nIndex);
                    try
                    {
                        nVal = Convert.ToInt32(strTemp);
                    }
                    catch
                    {
                        m_log.Add("Can not Change Value : " + strTemp);
                        return true;
                    }
                    return false;
                }
            }
            return true;
        }

        public bool GetXMLRows(string strLine, ref string strRows)
        {
            int nIndex;
            nIndex = strLine.IndexOf("<Row>");
            if (nIndex > -1)
            {
                strRows = strLine.Remove(0, nIndex + 5);
                nIndex = strRows.IndexOf("</Row>");
                if (nIndex > -1)
                {
                    strRows = strRows.Remove(nIndex, strRows.Length - nIndex);
                    return false;
                }
            }
            return true;
        }

        public bool ReadG85XML(string strPath) // ing
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.LoadXml(strPath);
            }
            catch (Exception ex)
            {
                m_log.Popup("G85 File Open Fail !!");
                m_log.Add(ex.Message);
                return true;
            }
            return false;
        }

        public bool ReadASC(string strPath)
        {
            string strLine = "";
            bool bColumn = false, bRow = false;
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(strPath);
                if (sr == null)
                {
                    m_log.Popup("G85 File Open Fail !!");
                    return true;
                }
                do
                {
                    strLine = sr.ReadLine();
                    if (!GetValue(strLine, "X", ref m_cpMap.x)) { bColumn = true; continue; }
                    if (!GetValue(strLine, "Y", ref m_cpMap.y)) { bRow = true; continue; }
                    if (bColumn && bRow) break;
                } while (strLine != null);
            }
            catch (Exception ex)
            {
                m_log.Popup("Create Die Map Fail !!");
                m_log.Add(ex.Message);
                if (sr == null) sr.Close();
                return true;
            }

            Allocate(m_cpMap.x, m_cpMap.y);
            try
            {
                do
                {
                    strLine = sr.ReadLine();
                    if (strLine == null) break;
                    if (strLine.IndexOf("REFDIE:") > -1)
                    {
                        for (int y = 0; y < m_cpMap.y; y++)
                        {
                            strLine = sr.ReadLine();
                            if (strLine == "") strLine = sr.ReadLine();
                            for (int x = 0; x < m_cpMap.x; x++)
                            {
                                if (strLine[x] == '1') m_aDie[y, x].m_bEnable = true;
                                else m_aDie[y, x].m_bEnable = false;
                            }
                        }
                    }
                } while (strLine != null);
            }
            catch (Exception ex)
            {
                m_log.Popup("Create Die Map Fail !!");
                m_log.Add(ex.Message);
                if (sr == null) sr.Close();
                return true;
            }
            m_log.Add("<" + strPath + "> Die Map Created.");
            return false;
        }

        bool GetValue(string strLine, string strItem, ref int nVal)
        {
            int nIndex;
            string strTemp;
            strItem = strItem + ":";
            nIndex = strLine.IndexOf(strItem);
            if (nIndex > -1)
            {
                strTemp = strLine.Remove(0, nIndex + strItem.Length);
                try
                {
                    nVal = Convert.ToInt32(strTemp);
                    return false;
                }
                catch
                {
                    m_log.Add("Can not Change Value : " + strTemp);
                    return true;
                }
            }
            return true;
        }

    }
}
