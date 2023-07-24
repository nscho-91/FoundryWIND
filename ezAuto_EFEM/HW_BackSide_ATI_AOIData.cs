using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading; 
using System.Threading.Tasks;
using ezAutoMom;
using ezTools;

namespace ezAuto_EFEM
{
    public class HW_BackSide_ATI_AOIData
    {
        public string[] m_id = new string[2];
        Log m_log;

        public bool m_bSaveKlarf = false;
        public int m_nDown = 16;
        public string m_strPathKlarf = "D:\\Result";
        ezImg m_img; 
        public ezImg m_imgDown;
        CPoint m_szImg = new CPoint();
        CPoint m_szImgDown = new CPoint();
        public MinMax[] m_aEdge = null;
        public MinMax[] m_aEdgeDown = null;
        public int[] m_aGV = null; 

        // for Mapping
        public double m_fTheta = 0;
        public HW_DieMap m_dieMap = new HW_DieMap();
        public bool m_bReadMap = false;
        public double m_fResolution = 26;
        public double m_fScaleBar = 100;

        public bool m_bWhiteWafer = true; 
        public double m_dR = 0;
        public double m_dRFrame = 0; 
        public CPoint m_cpCenter = new CPoint();
        public CPoint m_cpCenterFrame = new CPoint();
        public CPoint m_cpCenterDown = new CPoint();
        public CPoint m_cpNotch = new CPoint();
        public CPoint m_cpDiffNotchAndCenter = new CPoint();

        public bool m_bSetup = false;
        public CPoint m_cpManualCenter = new CPoint();
        public int m_nManualR = 7500;
        public int m_nRange = 300;
        public double m_fAnglePeriod = 0.1;
        public double m_fAngleLimit = 1.0;

        public double m_gvAve = 0;
        public double m_gvSigma = 0; 

        public int c_nThread = 48;
        bool m_bRun = false;
        bool[] m_bCalcThread = null;
        Thread[] m_thread = null;

        public delegate void CalcThread(int nIndex, int y);
        CalcThread m_dgCalcThread = null; 

        public HW_BackSide_ATI_AOIData()
        {
        }

        public void Init(string id, Log log)
        {
            m_id[0] = id + "0";
            m_id[1] = id + "1";
            m_log = log;
            m_thread = new Thread[c_nThread];
            m_bCalcThread = new bool[c_nThread];
            m_imgDown = new ezImg("ImgDown", m_log); 

            m_bRun = true;
            for (int n = 0; n < c_nThread; n++)
            {
                m_bCalcThread[n] = false;
                m_thread[n] = new Thread(new ParameterizedThreadStart(RunThread));
                m_thread[n].Start(n);
            }
            m_dieMap.Init(new CPoint(10, 10), new RPoint(10, 10), m_log);
        }

        public void ThreadStop()
        {
            m_bRun = false;
            foreach (Thread thread in m_thread) thread.Join();
        }

        public void RunGrid(ezGrid grid, int nID, eGrid eMode, ezJob job = null)
        {
            if (nID == 0) grid.Set(ref m_bSaveKlarf, m_id[nID], "SaveKlarf", "Save Klarf and Image Files");
            if (nID != 0) return; 
            grid.Set(ref m_nDown, m_id[nID], "DownSize", "DownSize (1 ~ )");
            grid.Set(ref m_dieMap.m_rpPitch, m_id[nID], "DiePitch", "Size Of Die");
            if (eMode == eGrid.eJobSave) m_dieMap.SaveJob(job.m_strFile.Remove(job.m_strFile.Length - 7, 7));
            if (eMode == eGrid.eJobOpen)
            {
                m_bReadMap = m_dieMap.OpenJob(job.m_strFile.Remove(job.m_strFile.Length - 7, 7));
                RunGrid(grid, nID, eGrid.eUpdate, job);
            }
        }

        public int GetThreadCount()
        {
            return c_nThread; 
        }

        public void RunCalcThread(CalcThread dgCalcThread)
        {
            m_dgCalcThread = dgCalcThread;
            for (int n = 0; n < c_nThread; n++) m_bCalcThread[n] = true;
            Thread.Sleep(10);
            int nRun = c_nThread;
            while (nRun > 0)
            {
                Thread.Sleep(1);
                nRun = 0;
                for (int n = 0; n < c_nThread; n++) if (m_bCalcThread[n]) nRun++;
            }
        }

        public bool MakeDownImage(ezImg img)
        {
            ezStopWatch sw = new ezStopWatch();
            m_img = img;
            if (!m_img.HasImage()) return true;
            m_log.Add("AOIData - Make Down Start" + sw.Check().ToString());
            ReAllocate(img.m_szImg);

            RunCalcThread(CalcDown);

            m_log.Add("AOIData - Make Down Image : " + sw.Check().ToString());
            return false; 
        }

        void ReAllocate(CPoint szImg)
        {
            if (m_szImg == szImg) return;
            m_szImg = szImg;
            m_szImgDown = szImg / m_nDown;
            m_szImgDown.x = (m_szImgDown.x / 4) * 4; // ing
            m_aEdge = new MinMax[m_szImg.y];
            m_aEdgeDown = new MinMax[m_szImgDown.y];
            m_imgDown.ReAllocate(m_szImgDown, 1);
        }

        void RunThread(object obj)
        {
            int nIndex = (int)obj;
            Thread.Sleep(5000);
            while (m_bRun)
            {
                Thread.Sleep(10);
                if (m_bCalcThread[nIndex])
                {
                    if (m_dgCalcThread != null)
                    {
                        for (int y = nIndex; y < m_szImgDown.y; y += c_nThread) m_dgCalcThread(nIndex, y);
                    }
                    else
                    {
                        if (nIndex == 0) m_log.Popup("dgCalcThread is null !!"); 
                    }
                    m_bCalcThread[nIndex] = false;
                }
            }
        }

        void CalcDown(int nIndex, int y)
        {
            int y0 = y * m_nDown;
            for (int x = 0, x0 = 0; x < m_szImgDown.x; x++, x0 += m_nDown)
            {
                int nSum = 0;
                for (int iy = 0; iy < m_nDown; iy++)
                {
                    for (int ix = 0; ix < m_nDown; ix++)
                    {
                        if (y0 + iy > m_szImg.y)
                            break;
                        nSum += m_img.m_aBuf[y0 + iy, x0 + ix];
                    }
                }
                m_imgDown.m_aBuf[y, x] = (byte)(nSum / m_nDown / m_nDown);
            }
        }

        public void CalcEdge(double dR)
        {
            m_cpCenterDown = m_cpCenter / m_nDown;
            CalcEdge(m_cpCenter, m_aEdge, m_szImg, dR);
            CalcEdge(m_cpCenterDown, m_aEdgeDown, m_szImgDown, dR / m_nDown); 
        }

        void CalcEdge(CPoint cpCenter, MinMax[] aEdge, CPoint szImg, double dR)
        {
            double dR2 = dR * dR;
            for (int y = 0; y < szImg.y; y++)
            {
                aEdge[y].Min = 0;
                aEdge[y].Max = -1;
                double dY = y - cpCenter.y;
                double dX2 = dR2 - dY * dY;
                if (dX2 > 1)
                {
                    int dX = (int)Math.Sqrt(dX2);
                    aEdge[y].Min = cpCenter.x - dX;
                    aEdge[y].Max = cpCenter.x + dX;
                }
            }
        }
    }

    public class Die
    {
        public bool m_bEnable = true;
        public CPoint m_cpLT;
        public CPoint m_cpRT;
        public CPoint m_cpLB;
        public CPoint m_cpRB;
        public CPoint m_cpIndex;
        public ArrayList m_aDefect = new ArrayList();
        public eDefType m_defType = eDefType.None;

        public Die()
        {

        }

        public Die(int x, int y)
        {
            m_cpIndex = new CPoint(x, y);
        }

        public Die CloneDie()
        {
            Die newDie = new Die();
            newDie.m_bEnable = this.m_bEnable;
            newDie.m_cpLT = this.m_cpLT;
            newDie.m_cpRT = this.m_cpRT;
            newDie.m_cpLB = this.m_cpLB;
            newDie.m_cpRB = this.m_cpRB;
            newDie.m_cpIndex = this.m_cpIndex;
            newDie.m_aDefect = (ArrayList)this.m_aDefect.Clone();
            return newDie;
        }

        public CPoint GetDieCenter()
        {
            return (m_cpLT + m_cpRT + m_cpLB + m_cpRB) / 4;
        }

        public void SetStainMark()
        {
        }

        public void AddDieDefect(eDefType eType, string strCode = "000")
        {
            if (m_defType == eType) return;
            m_defType = eType;
            Defect defect = new Defect();
            int xMin, yMin, xMax, yMax;
            xMin = xMax = m_cpLB.x;
            yMin = yMax = m_cpLB.y;
            if (xMin > m_cpLT.x) xMin = m_cpLT.x;
            if (xMin > m_cpRB.x) xMin = m_cpRB.x;
            if (xMin > m_cpRT.x) xMin = m_cpRT.x;
            if (yMin > m_cpLT.y) yMin = m_cpLT.x;
            if (yMin > m_cpRB.y) yMin = m_cpRB.x;
            if (yMin > m_cpRT.y) yMin = m_cpRT.y;

            if (xMax < m_cpLT.x) xMax = m_cpLT.x;
            if (xMax < m_cpRB.x) xMax = m_cpRB.x;
            if (xMax < m_cpRT.x) xMax = m_cpRT.x;
            if (yMax < m_cpLT.y) yMax = m_cpLT.x;
            if (yMax < m_cpRB.y) yMax = m_cpRB.x;
            if (yMax < m_cpRT.y) yMax = m_cpRT.y;

            defect.m_cp0 = new CPoint(xMax, yMax);
            defect.m_cp1 = new CPoint(xMin, yMin);
            defect.m_cpDieIndex = m_cpIndex;
            defect.m_eDefType = eType;
            defect.m_strCode = strCode;
            defect.m_nSize = Math.Abs(defect.m_cp0.x - defect.m_cp1.x) * Math.Abs(defect.m_cp0.y - defect.m_cp1.y);
            m_aDefect.Add(defect);
        }

        public void SetCoaseGrindMark()
        {
        }
    }

    public enum eDefType { None, StainMark, RingMark, CoarseGrindMark, Edge };

    public class Defect
    {
        public int m_nDefIndex, m_nSize = 0; // m_nSize : Pixel
        public int m_nRotateMode = 0;
		public double m_fAngle = 0.0;
        public CPoint m_cp0, m_cp1, m_cpOffset, m_cpDieIndex; // m_cp0 > m_cp1
        public string m_strCode = "000";
        public eDefType m_eDefType = eDefType.None;
        public Die m_dieChipping;

        public Defect()
        {
            m_nSize = 0;
        }

        public CPoint GetCenter()
        {
            return (m_cp0 + m_cp1) / 2;
        }
    }
}
