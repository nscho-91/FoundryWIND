using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Drawing;
using System.IO;
using ezTools;
using ezAutoMom;

namespace ezAuto_EFEM
{
    public class HW_Backside_InfoWafer
    {
        const int m_nMergeOffset = 100;
        Log m_log;
        public Info_Wafer m_infoWafer;
        public Info_Carrier m_infoCarrier;
        // 0 : high angle illumination, 1 : low angle illumination
        public int m_nDown;
        public int m_nSearchRing, m_nRingWidth;
        public bool m_bInspDone0, m_bInspDone90, m_bRingMark, m_bCorseGrindMark;
        //public bool m_bReady = true;
        public bool[] m_bInspectDone = new bool[2];
        public double m_fResolution = 26;
        public double[] m_dR = new double[2]; // Rotate Mode
        public double[] m_fTheta = new double[2]; // Rotate Mode
        public string m_strPath, m_strModel;
        public string m_strRingCode, m_strCoarseGrindCode, m_strEdgeChipping;
        public ezImg[,] m_imgDown = new ezImg[2, 2]; // illumination Mode , Rotate Mode
        public ezImg[,] m_imgInspect = new ezImg[2, 2]; // illumination Mode , Rotate Mode
        public ezImg m_imgMerge;
        public ArrayList[, ,] m_aIslandSobel = new ArrayList[2, 2, 3]; // illumination Mode , Rotate Mode, Sobel Count
        public ArrayList[, ,] m_aIslandBlob = new ArrayList[2, 2, 3]; // illumination Mode , Rotate Mode, Blob Count
        public ArrayList[] m_aIslandStain = new ArrayList[2]; // Rotate Mode
        public ArrayList[] m_aIslandRing = new ArrayList[2]; // Rotate Mode
        public ArrayList m_aEdgeChipping;
        public CPoint[] m_cpCenter = new CPoint[2]; // illumination Mode
        public Color[,] m_colorSoble = new Color[2, 3]; // illumination Mode, Sobel Count
        public Color[,] m_colorBlob = new Color[2, 3]; // illumination Mode, Sobel Count
        public HW_DieMap m_dieMap;
        public CPoint m_cpOffCenter = new CPoint(0, 0);
        public bool m_bOffCenter = false;
        public eOrientation m_eOrientation = eOrientation.R0;
        ippTools ipp = new ippTools();
 
        public HW_Backside_InfoWafer(Log log)
        {
            m_log = log;
            int nMode, nRotate, nCount;
            m_aEdgeChipping = new ArrayList();
            for (nMode = 0; nMode < 2; nMode++)
            {
                m_aIslandStain[nMode] = new ArrayList();
                m_imgMerge = new ezImg(nMode.ToString("MergeImg"), m_log);
                for (nRotate = 0; nRotate < 2; nRotate++)
                {
                    m_imgDown[nMode, nRotate] = new ezImg(nMode.ToString("Down_0") + "_" + nRotate.ToString(), m_log);
                    m_imgInspect[nMode, nRotate] = new ezImg(nMode.ToString("Ins_0") + "_" + nRotate.ToString(), m_log);
                    for (nCount = 0; nCount < 3; nCount++)
                    {
                        m_aIslandSobel[nMode, nRotate, nCount] = new ArrayList();
                        m_aIslandBlob[nMode, nRotate, nCount] = new ArrayList();
                    }
                }
            }
        }

        public void Clear()
        {
            int nMode, nRotate, nCount;
            m_bInspDone0 = m_bInspDone90 = m_bRingMark = m_bCorseGrindMark = false;
            m_aEdgeChipping.Clear();
            for (nMode = 0; nMode < 2; nMode++)
            {
                m_cpCenter[nMode] = new CPoint(0, 0);
                m_bInspectDone[nMode] = false;
                m_aIslandStain[nMode].Clear();
                for (nRotate = 0; nRotate < 2; nRotate++)
                {
                    if (m_imgDown[nMode, nRotate].m_aBuf != null) m_imgDown[nMode, nRotate].Clear();
                    for (nCount = 0; nCount < 3; nCount++)
                    {
                        m_aIslandSobel[nMode, nRotate, nCount].Clear();
                        m_aIslandBlob[nMode, nRotate, nCount].Clear();
                    }
                }
            }
        }

        public bool MergeImage(int nMode)
        {
            int dR;
            dR = (int)((m_dR[0] + m_dR[1]) / 2 + 100) / m_nDown;
            m_imgMerge.ReAllocate(new CPoint(dR * 2, dR * 2), 1);
            m_imgDown[nMode, 0].FileSave("D:\\DownIlum_" + nMode.ToString() + "Rotate0.bmp");
            m_imgDown[nMode, 1].FileSave("D:\\DownIlum_" + nMode.ToString() + "Rotate90.bmp");
            if (m_imgDown[nMode, 0].m_aBuf == null || m_imgDown[nMode, 1].m_aBuf == null) return true;
            unsafe
            {
                for (int y = 0; y < m_imgMerge.m_szImg.y; y++)
                {
                    byte* pDst = (byte*)m_imgMerge.GetIntPtr(y, 0);
                    byte* pSrc0 = (byte*)m_imgDown[nMode, 0].GetIntPtr(m_cpCenter[0].y / m_nDown - dR + y, m_cpCenter[0].x / m_nDown - dR);
                    byte* pSrc1 = (byte*)m_imgDown[nMode, 1].GetIntPtr(m_cpCenter[1].y / m_nDown - dR + y, m_cpCenter[1].x / m_nDown - dR);
                    for (int x = 0; x < m_imgMerge.m_szImg.x; x++ )
                    {
                        *pDst = (byte)((*pSrc0 / 2) + (*pSrc1 / 2));
                        pDst++; pSrc0++; pSrc1++;
                    }
                }
            }
            m_imgMerge.FileSave("D:\\MergeImage.bmp");
            return false;
        }

        void Relocate()
        {
            CPoint cpDiff = m_cpCenter[0] - m_cpCenter[1];
            int n, nCount;
            for (int nMode = 0; nMode < 2; nMode++)
            {
                for (nCount = 0; nCount < 3; nCount++)
                {
                    for (n = 0; n < m_aIslandSobel[nMode, 1, nCount].Count; n++)
                    {
                        ((azBlob.Island)m_aIslandSobel[nMode, 1, nCount][n]).m_cp0 += cpDiff;
                        ((azBlob.Island)m_aIslandSobel[nMode, 1, nCount][n]).m_cp1 += cpDiff;
                        //((azBlob.Island)m_aIslandSobel[nMode, 1, nCount][n]).Shift(cpDiff);
                    }
                    for (n = 0; n < m_aIslandBlob[nMode, 1, nCount].Count; n++)
                    {
                        ((azBlob.Island)m_aIslandBlob[nMode, 1, nCount][n]).m_cp0 += cpDiff;
                        ((azBlob.Island)m_aIslandBlob[nMode, 1, nCount][n]).m_cp1 += cpDiff;
                        //((azBlob.Island)m_aIslandBlob[nMode, 1, nCount][n]).Shift(cpDiff);
                    }
                }

            }
        }

        void MergeIsland(azBlob.Island island0, azBlob.Island island1, int nMerge)
        {
            if (island0.m_bValid == false) return;
            if (island1.m_bValid == false) return;
            if (island0.IsSeperate(island1, nMerge)) return;
            if (island0.m_nSize > island1.m_nSize) island0.Merge(island1, nMerge);
            else island1.Merge(island0, nMerge);
        }

        public void AbsorbIsland(ArrayList aListA, ArrayList aListB, int nOffset)
        {
            int n, m;
            for (n = 0; n < aListA.Count; n++)
            {
                for (m = 0; m < aListB.Count; m++)
                {
                    if (((azBlob.Island)aListA[n]).IsInside((azBlob.Island)aListB[m], nOffset))
                    {
                        aListB.RemoveAt(m);
                        m--;
                    }
                    else if (((azBlob.Island)aListB[m]).IsInside((azBlob.Island)aListA[n], nOffset))
                    {
                        aListA.RemoveAt(n);
                        n--;
                        m = aListB.Count;
                        break;
                    }
                }
            }
            for (n = 0; n < aListB.Count; n++)
            {
                for (m = 0; m < aListA.Count; m++)
                {
                    if (((azBlob.Island)aListB[n]).IsInside((azBlob.Island)aListA[m], nOffset))
                    {
                        aListA.RemoveAt(m);
                        m--;
                    }
                }
            }
        }

        void MappingDefect()
        {
            int nMode, nRotate, nCount;
            MappingChipping(0);
            for (nMode = 0; nMode < 2; nMode++)
            {
                for (nRotate = 0; nRotate < 2; nRotate++)
                {
                    for (nCount = 0; nCount < 3; nCount++)
                    {
                        MappingDefect(nMode, m_aIslandSobel[nMode, nRotate, nCount]);
                        MappingDefect(nMode, m_aIslandBlob[nMode, nRotate, nCount]);
                    }
                }
            }
        }

        void MappingDefect(int nMode, ArrayList aIsland)
        {
            int n, m;
            double dX, dY, fL;
            CPoint cp;
            azBlob.Island island;
            ezStopWatch sw = new ezStopWatch();
            sw.Start();
            for (m = 0; m < aIsland.Count; m++)
            {
                island = (azBlob.Island)aIsland[m];
                for (n = 0; n < island.m_aPosition.Count; n++)
                {
                    if (n > island.m_aPosition.Count - 1) break;
                    cp = (CPoint)island.m_aPosition[n] + island.m_cpShift; // ing 170408
                    fL = Math.Sqrt(Math.Pow(m_dieMap.m_aDie[0, 0].m_cpLT.x - cp.x, 2) + Math.Pow(m_dieMap.m_aDie[0, 0].m_cpLT.y - cp.y, 2));
                    CPoint dp = cp - m_dieMap.m_aDie[0, 0].m_cpLT;
                    double fTheta = Math.Atan2(dp.y, dp.x) - (m_fTheta[nMode] * Math.PI / 180);
                    dX = Math.Abs(Math.Cos(fTheta) * fL);
                    dY = Math.Abs(Math.Sin(fTheta) * fL);
                    if (AddDefect(dX, dY, m, island.m_strCode, island.m_nRoateMode))
                    {
                        m_log.Popup("Defect Mapping Is fail !!");
                        continue;
                    }
                }
            }
            m_log.Add("AOI Mapping : " + sw.Check().ToString());
        }

        void MappingChipping(int nMode)
        {
            int n, m;
            double dX, dY, fL;
            CPoint cp;
            azBlob.Island island;
            ezStopWatch sw = new ezStopWatch();
            sw.Start();
            for (m = 0; m < m_aEdgeChipping.Count; m++)
            {
                island = (azBlob.Island)m_aEdgeChipping[m];
                Die chippingDie = new Die();
                chippingDie.m_cpLB = island.m_cp0 + new CPoint(-300, -300);
                chippingDie.m_cpLT = island.m_cp0 + new CPoint(-300, 300);
                chippingDie.m_cpRB = island.m_cp0 + new CPoint(300, -300);
                chippingDie.m_cpRT = island.m_cp0 + new CPoint(300, 300);
                for (n = 0; n < island.m_aPosition.Count; n++)
                {
                    //if (n > island.m_aPosition.Count - 1) break;
                    cp = (CPoint)island.m_aPosition[n] + island.m_cpShift;
                    fL = Math.Sqrt(Math.Pow(m_dieMap.m_aDie[0, 0].m_cpLT.x - cp.x, 2) + Math.Pow(m_dieMap.m_aDie[0, 0].m_cpLT.y - cp.y, 2));
                    CPoint dp = cp - m_dieMap.m_aDie[0, 0].m_cpLT;
                    double fTheta = Math.Atan2(dp.y, dp.x) - (m_fTheta[nMode] * Math.PI / 180);
                    dX = Math.Abs(Math.Cos(fTheta) * fL);
                    dY = Math.Abs(Math.Sin(fTheta) * fL);
                    if (AddChippingDefect(dX, dY, chippingDie, island.m_strCode, nMode))
                    {
                        m_log.Popup("Defect Mapping Is fail !!");
                        continue;
                    }
                }
            }
            m_log.Add("AOI Mapping : " + sw.Check().ToString());
        }

        bool AddDefect(double dX, double dY, int nDefect, string strCode = "000", int nRotateMode = 0, eDefType defType = eDefType.None)
        {
            CPoint cpIndex;
            cpIndex.x = (int)(dX / (m_dieMap.m_rpPitch.x / m_fResolution));
            cpIndex.y = (int)(dY / (m_dieMap.m_rpPitch.y / m_fResolution));
            if (cpIndex.x >= m_dieMap.m_aDie.GetLength(1) || cpIndex.y >= m_dieMap.m_aDie.GetLength(0))
                return false;
            if (!m_dieMap.m_aDie[cpIndex.y, cpIndex.x].m_bEnable) return false;
            dX = dX % (m_dieMap.m_rpPitch.x / m_fResolution);
            dY = m_dieMap.m_rpPitch.y / m_fResolution - (dY % (m_dieMap.m_rpPitch.y / m_fResolution));
            if (cpIndex.x < 0 || cpIndex.x > m_dieMap.m_aDie.GetLength(1) - 1 || cpIndex.y < 0 || cpIndex.y > m_dieMap.m_aDie.GetLength(0) - 1) return true;
            if (defType != eDefType.None)
            {
                m_dieMap.m_aDie[cpIndex.y, cpIndex.x].AddDieDefect(defType, strCode); // 큰불량 Add시키기
                return false;
            }
            foreach (Defect defect in m_dieMap.m_aDie[cpIndex.y, cpIndex.x].m_aDefect)
            {
                if (defect.m_nDefIndex == nDefect && defect.m_cpDieIndex == m_dieMap.m_aDie[cpIndex.y, cpIndex.x].m_cpIndex && defect.m_nRotateMode == nRotateMode)
                {
                    if (dX > defect.m_cp0.x) defect.m_cp0.x = (int)dX;
                    if (dY > defect.m_cp0.y) defect.m_cp0.y = (int)dY;
                    if (dX < defect.m_cp1.x) defect.m_cp1.x = (int)dX;
                    if (dY < defect.m_cp1.y) defect.m_cp1.y = (int)dY;
                    defect.m_nSize++;
                    return false;
                }
                //else if (defect.m_strCode == strCode) return false;
            }
            Defect def = new Defect();
            def.m_nSize++;
            def.m_nDefIndex = nDefect;
            def.m_cpDieIndex = m_dieMap.m_aDie[cpIndex.y, cpIndex.x].m_cpIndex;
            def.m_cp0.x = (int)dX;
            def.m_cp0.y = (int)dY;
            def.m_cp1.x = (int)dX;
            def.m_cp1.y = (int)dY;
            def.m_strCode = strCode;
            def.m_nRotateMode = nRotateMode;
            m_dieMap.m_aDie[cpIndex.y, cpIndex.x].m_aDefect.Add(def);
            return false;
        }

        public void AddStainDefect(int nRotate)
        {
            int m;
            double dX, dY, fL;
            CPoint cp;
            azBlob.Island island;
            ezStopWatch sw = new ezStopWatch();
            sw.Start();
            for (m = 0; m < m_aIslandStain[nRotate].Count; m++)
            {
                island = (azBlob.Island)(m_aIslandStain[nRotate])[m];
                cp = (island.m_cp0 + island.m_cp1) / 2;
                fL = Math.Sqrt(Math.Pow(m_dieMap.m_aDie[0, 0].m_cpLT.x - cp.x, 2) + Math.Pow(m_dieMap.m_aDie[0, 0].m_cpLT.y - cp.y, 2));
                CPoint dp = cp - m_dieMap.m_aDie[0, 0].m_cpLT;
                //double fAngleDefect = Math.Atan2(dp.y, dp.x) * 180 / Math.PI;
                double fTheta = Math.Atan2(dp.y, dp.x) - (m_fTheta[nRotate] * Math.PI / 180);
                dX = Math.Abs(Math.Cos(fTheta) * fL);
                dY = Math.Abs(Math.Sin(fTheta) * fL);
                if (AddDefect(dX, dY, m, island.m_strCode, island.m_nRoateMode, eDefType.StainMark))
                {
                    m_log.Popup("Defect Mapping Is fail !!");
                    continue;
                }
            }
            m_log.Add("Stain Mapping : " + sw.Check().ToString());
        }

        bool AddChippingDefect(double dX, double dY, Die dieChipping, string strCode = "000", int nRotateMode = 0)
        {
            CPoint cpIndex;
            cpIndex.x = (int)(dX / (m_dieMap.m_rpPitch.x / m_fResolution));
            cpIndex.y = (int)(dY / (m_dieMap.m_rpPitch.y / m_fResolution));
            if (cpIndex.x >= m_dieMap.m_aDie.GetLength(1) || cpIndex.y >= m_dieMap.m_aDie.GetLength(0))
                return false;
            if (!m_dieMap.m_aDie[cpIndex.y, cpIndex.x].m_bEnable) return false;
            dX = dX % (m_dieMap.m_rpPitch.x / m_fResolution);
            dY = m_dieMap.m_rpPitch.y / m_fResolution - (dY % (m_dieMap.m_rpPitch.y / m_fResolution));
            if (cpIndex.x < 0 || cpIndex.x > m_dieMap.m_aDie.GetLength(1) - 1 || cpIndex.y < 0 || cpIndex.y > m_dieMap.m_aDie.GetLength(0) - 1) return true;
            Defect def = new Defect();
            def.m_cpDieIndex = m_dieMap.m_aDie[cpIndex.y, cpIndex.x].m_cpIndex;
            def.m_strCode = strCode;
            def.m_nRotateMode = nRotateMode;
            def.m_eDefType = eDefType.Edge;
            def.m_dieChipping = dieChipping;
            m_dieMap.m_aDie[cpIndex.y, cpIndex.x].m_aDefect.Add(def);
            return false;
        }

        void MappingRingMark()
        {
            int x, y;
            double nDieSize;
            string strRingMarkIndexs = "RingMark : ";
            if (!m_bRingMark) return;
            nDieSize = Math.Sqrt((m_dieMap.m_rpPitch.x * m_dieMap.m_rpPitch.x) + (m_dieMap.m_rpPitch.y * m_dieMap.m_rpPitch.y));
            for (y = 0; y < m_dieMap.m_aDie.GetLength(0); y++)
            {
                for (x = 0; x < m_dieMap.m_aDie.GetLength(1); x++)
                {
                    if (m_dieMap.m_aDie[y, x].GetDieCenter().GetL(m_cpCenter[0]) > m_nSearchRing - m_nRingWidth
                        && m_dieMap.m_aDie[y, x].GetDieCenter().GetL(m_cpCenter[0]) < m_nSearchRing + m_nRingWidth)
                    {
                        m_dieMap.m_aDie[y, x].AddDieDefect(eDefType.RingMark, m_strRingCode);
                        strRingMarkIndexs += m_dieMap.m_aDie[y, x].m_cpIndex.ToString() + "\t";
                    }
                }
            }
            m_log.Add(strRingMarkIndexs);
        }

        void MappingCoarseGrindMark()
        {
            int x, y;
            double nDieSize;
            if (!m_bCorseGrindMark) return;
            nDieSize = Math.Sqrt((m_dieMap.m_rpPitch.x * m_dieMap.m_rpPitch.x) + (m_dieMap.m_rpPitch.y * m_dieMap.m_rpPitch.y));
            for (y = 0; y < m_dieMap.m_aDie.GetLength(0); y++)
            {
                for (x = 0; x < m_dieMap.m_aDie.GetLength(1); x++)
                {
                    if (m_dieMap.m_aDie[y, x].m_bEnable)
                    {
                        m_dieMap.m_aDie[y, x].AddDieDefect(eDefType.CoarseGrindMark, m_strCoarseGrindCode);
                    }
                }
            }
        }

        public void AfterInspect(HW_Klarf klarf, Info_Wafer infoWafer, Info_Carrier infoCarrier, int nRotate, bool bRotate = true)
        {
            if (klarf.m_bBusy)
            {
                m_log.Add(infoWafer.m_strWaferID + " : Klarf File Save Fail !!");
                return;
            }
            if (!bRotate)
            {
                AddStainDefect(0);
                if (m_bCorseGrindMark) MappingCoarseGrindMark();
                if (m_bRingMark) MappingRingMark();
                MappingDefect();
                RemoveDuplicateDefect();
                SetKlarf(klarf, infoWafer, infoCarrier, nRotate);
                PrintKlarf(klarf, infoWafer, infoCarrier, nRotate, bRotate);
                Clear();
            }
            if (nRotate == 2) // Rotate 0, 90 모두 검사
            {
                //Relocate(); // Rotate0, 90 검사일때 해야됨 -> 이미지를 돌리기때문에 필요없음
                //if (IsInspectDone())
                //{
                    if (MergeImage(0))
                    {
                        m_log.Add(infoWafer.m_strWaferID + "_0 Image Merge Fail !!");
                        return;
                    }

                    AddStainDefect(0);
                    AddStainDefect(1);
                    if (m_bCorseGrindMark) MappingCoarseGrindMark();
                    if (m_bRingMark) MappingRingMark();
                    MappingDefect();
                    RemoveDuplicateDefect();
                    SetKlarf(klarf, infoWafer, infoCarrier, nRotate);
                    PrintKlarf(klarf, infoWafer, infoCarrier, nRotate);
                    Clear();
                //}
            }
            else
            {
                AddStainDefect(nRotate);
            }
            //if (m_bRingMark) MappingRingMark();
            //MappingDefect();
            //RemoveDuplicateDefect();
            //MappingDownImg();
//            if (nRotate == 2)
  //          {
    //            SetKlarf(klarf, infoWafer, infoCarrier, nRotate);
      //          PrintKlarf(klarf, infoWafer, infoCarrier, nRotate);
        //        Clear();
          //  }
            //m_bReady = true;
        }

        void RemoveDuplicateDefect()
        {
            bool bRemove = false;
            CPoint cpDiff;
            for (int y = 0; y < m_dieMap.m_aDie.GetLength(0); y++)
            {
                for (int x = 0; x < m_dieMap.m_aDie.GetLength(1); x++)
                {
                    if (m_dieMap.m_aDie[y, x].m_bEnable && m_dieMap.m_aDie[y, x].m_aDefect.Count > 1)
                    {
                        for (int n = 1; n < m_dieMap.m_aDie[y, x].m_aDefect.Count; n++)
                        {
                            bRemove = false;
                            for (int m = 0; m < m_dieMap.m_aDie[y, x].m_aDefect.Count - 1; m++)
                            {
                                if (n == m) break;
                                if (((Defect)m_dieMap.m_aDie[y, x].m_aDefect[n]).m_nSize >= ((Defect)m_dieMap.m_aDie[y, x].m_aDefect[m]).m_nSize)
                                {
                                    cpDiff = ((Defect)m_dieMap.m_aDie[y, x].m_aDefect[n]).GetCenter() - ((Defect)m_dieMap.m_aDie[y, x].m_aDefect[m]).GetCenter();
                                    if (cpDiff.GetL(new CPoint(0, 0)) < m_nMergeOffset)
                                    {
                                        m_dieMap.m_aDie[y, x].m_aDefect.RemoveAt(m);
                                        m--;
                                        bRemove = true;
                                    }
                                }
                                else
                                {
                                    cpDiff = ((Defect)m_dieMap.m_aDie[y, x].m_aDefect[n]).GetCenter() - ((Defect)m_dieMap.m_aDie[y, x].m_aDefect[m]).GetCenter();
                                    if (cpDiff.GetL(new CPoint(0, 0)) < m_nMergeOffset)
                                    {
                                        m_dieMap.m_aDie[y, x].m_aDefect.RemoveAt(n);
                                        n--;
                                        bRemove = true;
                                    }
                                }
                                if (bRemove) break;
                            }
                            if (bRemove) break;
                        }
                    }
                }
            }
        }

        void SetKlarf(HW_Klarf klarf, Info_Wafer infoWafer, Info_Carrier infoCarrier, int nRotate)
        {
            m_infoCarrier = infoCarrier;
            if (m_infoCarrier == null) m_infoCarrier = new Info_Carrier(-1);
            klarf.m_img[0] = m_imgInspect[0, 0];
            klarf.m_img[1] = m_imgInspect[1, 0];
            klarf.m_img90[0] = m_imgInspect[0, 1];
            klarf.m_img90[1] = m_imgInspect[1, 1];
            klarf.m_imgDown[0] = m_imgDown[0, 0];
            klarf.m_imgDown[1] = m_imgDown[1, 0];
            klarf.m_imgDown90[0] = m_imgDown[0, 1];
            klarf.m_imgDown90[1] = m_imgDown[1, 1];
            klarf.m_strSetupID = m_infoCarrier.m_strRecipe;
            klarf.m_strLotID = m_infoCarrier.m_strLotID;
            klarf.m_strCSTID = m_infoCarrier.m_strCarrierID;
            klarf.m_strTimeResult = "";
            klarf.m_strWaferID = infoWafer.m_strWaferID;
            klarf.m_strRecipe = infoCarrier.m_strRecipe;
            klarf.m_strTiffFileName = "";
            klarf.m_strSampleTestPlan = "";
            klarf.m_strAreaPerTest = "";

            klarf.m_nSampleSize = 300;
            klarf.m_nFileVer1 = 1;
            klarf.m_nFileVer2 = 1;
            klarf.m_nKlarfRow = m_dieMap.m_cpMap.y;
            klarf.m_nKlarfCol = m_dieMap.m_cpMap.x;
            klarf.m_nSlot = infoWafer.m_nSlot;
            klarf.m_nInspectionTest = 1;
            klarf.m_nSampleTestCnt = 1;
            klarf.m_nDefectDieCnt = 0;
            klarf.m_nCenterX = 0;
            klarf.m_nCenterY = 0;

            klarf.m_fResolution = 26.0;
            klarf.m_fDiePitchX = m_dieMap.m_rpPitch.x;
            klarf.m_fDiePitchY = m_dieMap.m_rpPitch.y;
            klarf.m_fDieOriginX = 0.000000e+000;
            klarf.m_fDieOriginY = 0.000000e+000;
            if (m_dieMap.m_cpMap.x % 2 == 1) klarf.m_fSampleCenterLocationX = klarf.m_fDiePitchX / 2;
            else klarf.m_fSampleCenterLocationX = 0.000000e+005;
            if (m_dieMap.m_cpMap.x % 2 == 1) klarf.m_fSampleCenterLocationY = klarf.m_fDiePitchY / 2;
            else klarf.m_fSampleCenterLocationY = 0.000000e+005;
            klarf.m_nDown = m_nDown;
        }

        void PrintKlarf(HW_Klarf klarf, Info_Wafer infoWafer, Info_Carrier infoCarrier, int nRotate, bool bRotate = true)
        {
            int x, y;
            string strFile, strPath;
            if (infoCarrier == null) infoCarrier = new Info_Carrier(-1);
            //double fTheta = (m_aoiData.m_fTheta + 360) % 360;
            //if (fTheta < m_aoiData.m_fAngleLimit) fTheta = 0;
            //else if (fTheta > 90 - m_aoiData.m_fAngleLimit && fTheta < 90 + m_aoiData.m_fAngleLimit) fTheta = 90;
            //else if (fTheta > 180 - m_aoiData.m_fAngleLimit && fTheta < 180 + m_aoiData.m_fAngleLimit) fTheta = 180;
            //else if (fTheta > 270 - m_aoiData.m_fAngleLimit && fTheta < 270 + m_aoiData.m_fAngleLimit) fTheta = 270;
            ezStopWatch sw = new ezStopWatch();
            Die[,] tempDie = new Die[m_dieMap.m_aDie.GetLength(0), m_dieMap.m_aDie.GetLength(1)];
            for (x = 0; x < tempDie.GetLength(1); x++) for (y = 0; y < tempDie.GetLength(0); y++) tempDie[y, x] = m_dieMap.m_aDie[y, x].CloneDie();
            Directory.CreateDirectory(m_strPath);
            Directory.CreateDirectory(m_strPath + "\\" + m_strModel);
            Directory.CreateDirectory(m_strPath + "\\" + m_strModel + "\\" + infoCarrier.m_strLotID);
            strPath = m_strPath + "\\" + m_strModel + "\\" + infoCarrier.m_strLotID + "\\";
            strFile = infoCarrier.m_strLotID + "_" + infoWafer.m_strWaferID;
            klarf.m_strWaferID = infoWafer.m_strWaferID;
            klarf.m_strTiffFileName = strFile + ".tif";
            klarf.SaveKlarf(strPath + strFile, true, tempDie, m_fTheta[nRotate % 2]);
            if (!bRotate) SaveWFImage(strPath + strFile, m_imgDown[0, 0], nRotate, m_fTheta[0]);
            else SaveWFImage(strPath + strFile, m_imgMerge, nRotate, m_fTheta[0]); 
            m_log.Add("Save Klarf <" + strPath + strFile + "> : " + sw.Check().ToString());
        }

        public void SaveWFImage(string strFile, ezImg img, int nRotate, double fAngle)
        {
            //불량위치 따라서 x방향 확인 필요!
            if (img.m_szImg.x == 0 || img.m_szImg.y == 0) return;
            ezImg imgRotate = new ezImg("RotateImg", m_log);
            imgRotate.ReAllocate(img);
            ipp.ippiRotate(imgRotate, imgRotate.m_szImg, new Rectangle(0, 0, imgRotate.m_szImg.x, imgRotate.m_szImg.y), img, img.m_szImg, new Rectangle(0, 0, img.m_szImg.x, img.m_szImg.y), fAngle, img.m_szImg.x / 2, img.m_szImg.y / 2, 4);
            Bitmap bitmap = new Bitmap(imgRotate.m_szImg.x, imgRotate.m_szImg.y);
            Graphics dc;
            dc = Graphics.FromImage(bitmap);
            dc.DrawImage(imgRotate.GetBitmap(), new Point(0, 0));
            fAngle = (fAngle + 360) % 360;
            //Bitmap bitimg = img.GetBitmap();
//            if (fAngle >= 45 + 30 && fAngle < 135 - 30) 
//                bitmap.RotateFlip(RotateFlipType.Rotate270FlipXY);
//            else if (fAngle >= 135 + 30 && fAngle < 225 - 30)
//                bitmap.RotateFlip(RotateFlipType.Rotate180FlipXY);
//            else if (fAngle >= 225 + 30 && fAngle < 315 - 30)
//                bitmap.RotateFlip(RotateFlipType.Rotate90FlipXY);
//            else if ((fAngle >= 0 + 30 && fAngle < 45 - 30) || (fAngle >= 315 + 30 && fAngle <= 360 - 30))
//                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipXY);
            //else
                //dc.RotateTransform((float)fAngle);
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipXY);
            switch (m_eOrientation)
            {
                //Wrong notch Orientation
                case eOrientation.R0:
                    if (fAngle > 15 && fAngle < 345)
                    {
                        dc.DrawString("Wrong Notch Orientation (" + (360 - fAngle).ToString("0.0") + ")", new Font("Arial", 15), new SolidBrush(Color.Red), new PointF(img.m_szImg.x / 3, img.m_szImg.x / 3 * 2));
                    }
                    break;
                case eOrientation.R90:
                case eOrientation.R180:
                case eOrientation.R270:
                    if (Math.Abs(((int)m_eOrientation * 90) - fAngle) > 15)
                    {
                        dc.DrawString("Wrong Notch Orientation (" + (360 - fAngle).ToString("0.0") + " dgree)", new Font("Arial", 15), new SolidBrush(Color.Red), new PointF(img.m_szImg.x / 3, img.m_szImg.x / 3 * 2));
                    }
                    break;
            }
           

            if (m_bOffCenter)
            {
                dc.DrawLine(new Pen(new SolidBrush(Color.Yellow), 2), new Point(img.m_szImg.x / 2 - 20, img.m_szImg.y / 2), new Point(img.m_szImg.x / 2 + 20, img.m_szImg.y / 2));
                dc.DrawLine(new Pen(new SolidBrush(Color.Yellow), 2), new Point(img.m_szImg.x / 2, img.m_szImg.y / 2 - 20), new Point(img.m_szImg.x / 2, img.m_szImg.y / 2 + 20));
                dc.DrawString("OffCenter " + m_cpOffCenter.ToString() + " um", new Font("Arial", 15), new SolidBrush(Color.Red), img.m_szImg.x / 2 + 20, img.m_szImg.y / 2);
            }

            bitmap.Save(strFile + ".bmp");
            // ing 170417
            /*if (fAngle >= 45 && fAngle < 135)
                bitmap.RotateFlip(RotateFlipType.Rotate90FlipXY);
            else if (fAngle >= 135 && fAngle < 225)
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipXY);
            else if (fAngle >= 225 && fAngle < 315)
                bitmap.RotateFlip(RotateFlipType.Rotate270FlipXY);
            else if ((fAngle >= 0 && fAngle < 45) || (fAngle >= 315 && fAngle <= 360))
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipXY);
             * */
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipXY);
        }

        /*void MappingDownImg()
        {
            int nMode, nRotate, nCount;
            for (nMode = 0; nMode < 2; nMode++)
            {
                m_aDefectRect[nMode].Clear();
                for (nRotate = 0; nRotate < 2; nRotate++)
                {
                    for (nCount = 0; nCount < 3; nCount++)
                    {
                        foreach (azBlob.Island island in m_aIslandSobel[nMode, nRotate, nCount])
                        {
                            DefectRect defectRT = new DefectRect();
                            defectRT.m_rt = new Rectangle((island.m_cp0 / m_nDown).ToPoint(), new Size(Math.Abs(island.m_cp0.x - island.m_cp1.x) / m_nDown, Math.Abs(island.m_cp0.y - island.m_cp1.y) / m_nDown));
                            defectRT.m_color = m_colorSoble[nMode, nCount];
                            m_aDefectRect[nMode].Add(defectRT);
                        }
                        foreach (azBlob.Island island in m_aIslandBlob[nMode, nRotate, nCount])
                        {
                            DefectRect defectRT = new DefectRect();
                            defectRT.m_rt = new Rectangle((island.m_cp0 / m_nDown).ToPoint(), new Size(Math.Abs(island.m_cp0.x - island.m_cp1.x) / m_nDown, Math.Abs(island.m_cp0.y - island.m_cp1.y) / m_nDown));
                            defectRT.m_color = m_colorBlob[nMode, nCount];
                            m_aDefectRect[nMode].Add(defectRT);
                        }
    //                    for (int nID = 0; nID < 2; nID++) foreach (azBlob.Island island in m_aoiStain[nID].m_aIsland)
    //                        {
    //                            DefectRect defectRT = new DefectRect();
    //                            defectRT.m_rt = new Rectangle((island.m_cp0 / m_aoiData.m_nDown).ToPoint(), new Size(Math.Abs(island.m_cp0.x - island.m_cp1.x) / m_aoiData.m_nDown, Math.Abs(island.m_cp0.y - island.m_cp1.y) / m_aoiData.m_nDown));
    //                            defectRT.m_color = m_aoiSobel[m, n].m_color;
    //                            m_aDefectRect.Add(defectRT);
    //                        }
    //                    foreach (azBlob.Island island in m_aoiStain[1].m_aIsland)
    //                    {
    //                        DefectRect defectRT = new DefectRect();
    //                        defectRT.m_rt = new Rectangle((island.m_cp0 / m_aoiData.m_nDown).ToPoint(), new Size(Math.Abs(island.m_cp0.x - island.m_cp1.x) / m_aoiData.m_nDown, Math.Abs(island.m_cp0.y - island.m_cp1.y) / m_aoiData.m_nDown));
    //                        defectRT.m_color = m_aoiStain[1].m_color;
    //                        m_aDefectRect.Add(defectRT);
    //                    }
                    }
                }
            }
        }*/

        public bool IsInspectDone()
        {
            return m_bInspDone0 && m_bInspDone90;
        }

        public bool IsReady()
        {
            return !m_bInspDone0 && !m_bInspDone90;
        }
    }
}
