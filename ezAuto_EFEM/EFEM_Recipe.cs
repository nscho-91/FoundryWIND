using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ezAutoMom;
using ezTools;  

namespace ezAuto_EFEM
{
    class EFEM_Recipe : Recipe_Mom
    {

        public EFEM_Recipe()
        {
           
        }

        public override void Init(string id, Auto_Mom auto)
        {
            base.Init(id, auto);
            //m_wafer = m_align.m_wafer; 
            RunGrid(eGrid.eInit);
        }

        public override void RunRAC(string strName, eGrid eMode, ezJob job = null)
        {
            base.RunGrid(eMode, job);
            m_grid.Set(ref strName, "RecipeData", "Recipe Name", "Recipe Name");
            m_wafer.size_RAC = GetRACVal_str(strName, "Size");
            m_wafer.RunGrid(true, m_grid, eMode, "RecipeData", "Wafer Size");
            m_grid.Set(ref m_fAngleAlign, "RecipeData", "Loading Angle", "Loading Angle (deg)");

            if ((m_align != null) && (m_align.m_strAlignModel == "ATI"))
            {
                ((HW_Aligner_ATI)m_align).RecipeGrid(true, m_grid, eMode, job);
            }

            if (m_eBCR != eType_BCR.Disable)
            {
                m_grid.Set(ref m_bUseBCR, "BCR", "Use", "Use BCR");
                m_grid.Set(ref m_sTypeBCR, m_sTypeBCRs, "BCR", "Type", "BCR Type");
                m_grid.Set(ref m_bDirectionNotch, "BCR", "Base", "Base of Direction (Notch = true)");
                m_grid.Set(ref m_fAngleBCR, "BCR", "Angle", "BCR Angle (deg)");
                m_grid.Set(ref m_fAngleChar, "BCR", "CharAngle", "Character Angle (deg)");
                m_grid.Set(ref m_fdRBCR, "BCR", "dR", "BCR delta R (mm)");
            }

            if (m_eOCR != eType_OCR.Disable)
            {
                m_grid.Set(ref m_bUseOCR, "OCR", "Use", "Use OCR");
                m_grid.Set(ref m_fAngleOCR, "OCR", "Angle", "OCR Angle (deg)");
                m_grid.Set(ref m_fdROCR, "OCR", "dR", "OCR delta R (mm)");
                m_grid.Set(ref m_fOCRReadScore, "OCR", "Read Score", "OCR Read Pass Socre");
            }
            m_bUseVision = Convert.ToBoolean(GetRACVal_str(strName, "VisionUse"));
            m_grid.Set(ref m_bUseVision, "Vision", "Use", "Use Vision");

            if ((m_backSide != null) && m_backSide.m_bEnable)
            {
                m_grid.Set(ref m_bUseBackSide, "BackSide", "Use", "Use BackSide");
            }
            else
            {
                m_bUseBackSide = false;
            }
            m_grid.Refresh();
        }

        public override void RunGrid(eGrid eMode, ezJob job = null)
        {
            bool bUseWaferID = false;
            base.RunGrid(eMode, job);
            m_grid.Set(ref m_sRecipe, "RecipeData", "Recipe Name", "Recipe Name");
            m_wafer.RunGrid(false,m_grid, eMode, "RecipeData", "Wafer Size"); 
            m_grid.Set(ref m_fAngleAlign, "RecipeData", "Loading Angle", "Loading Angle (deg)");

            if (m_align != null)
            {
                if (m_align.m_strAlignModel == "ATI") ((HW_Aligner_ATI)m_align).RecipeGrid(false, m_grid, eMode, job);
                if (m_align.m_strAlignModel == "EBR") ((HW_Aligner_EBR)m_align).RecipeGrid(m_grid, eMode, job);
                if (m_align.m_strAlignModel == "EBRFix") ((HW_Aligner_EBRFix)m_align).RecipeGrid(m_grid, eMode, job);
                if (m_align.m_strAlignModel == "LineScan") ((HW_Aligner_LineScan)m_align).RecipeGrid(m_grid, eMode, job);
            }

            if (m_eBCR != eType_BCR.Disable)
            {
                m_grid.Set(ref m_bUseBCR, "BCR", "Use", "Use BCR");
                m_grid.Set(ref m_sTypeBCR, m_sTypeBCRs, "BCR", "Type", "BCR Type");
                m_grid.Set(ref m_bDirectionNotch, "BCR", "Base", "Base of Direction (Notch = true)");
                m_grid.Set(ref m_fAngleBCR, "BCR", "Angle", "BCR Angle (deg)");
                m_grid.Set(ref m_fAngleChar, "BCR", "CharAngle", "Character Angle (deg)");
                m_grid.Set(ref m_fdRBCR, "BCR", "dR", "BCR delta R (mm)");
                m_grid.Set(ref m_align.m_nTryBCR, "BCR", "Try", "Count Of Try Reading");
                m_grid.Set(ref m_align.m_dPeriod, "BCR", "Period", "Period Between Try Reading");
                if (m_align.m_nTryBCR > 10) m_align.m_nTryBCR = 10;
                if (m_align.m_dPeriod > 10) m_align.m_dPeriod = 10;
            }

            if (m_eOCR != eType_OCR.Disable)
            {
                m_grid.Set(ref m_bUseOCR, "OCR", "Use", "Use OCR");
                if (m_eOCRBottom == eType_OCR.CognexCam)
                {
                    string sOCRMode = m_eOCRDir.ToString();
                    m_grid.Set(ref sOCRMode, m_sOCRDir, "OCR", "Dir", "OCR Direction");
                    for (int n = 0; n < m_sOCRDir.Length; n++)
                    {
                        if (sOCRMode == ((eInspDir)n).ToString())
                        {
                            m_eOCRDir = (eInspDir)n;
                            break;
                        }
                    }
                }
                m_grid.Set(ref m_fAngleOCR, "OCR", "Angle", "OCR Angle (deg)");
                m_grid.Set(ref m_fdROCR, "OCR", "dR", "OCR delta R (mm)");
                m_grid.Set(ref m_fOCRReadScore, "OCR", "Read Score", "OCR Read Pass Score");
            }

            if (m_auto.m_strWork == EFEM.eWork.MSB.ToString())
            {
                for (int i = 0; i < ((EFEM_Handler)m_handler).m_nLoadPort; i++)
                {
                    if (((EFEM_Handler)m_handler).m_loadport[i].m_infoCarrier.m_wafer.m_bEnable[(int)Wafer_Size.eSize.mm300_RF])
                    {
                        bUseWaferID = true;
                    }
                }
            }
            if (bUseWaferID)
            {
                m_grid.Set(ref m_sWaferIDType, m_sWaferIDTypes, "WaferID", "WaferIDType", "Select Wafer ID Type For RingFrame");
                if (m_sWaferIDType == "PM") m_grid.Set(ref m_sPMWaferID, "WaferID", "PM_WaferID", "Input WaferID For PM");
            }

            m_grid.Set(ref m_bUseVision, "Vision", "Use", "Use Vision"); 

            if ((m_backSide != null) && m_backSide.m_bEnable)
            {
                if (m_wafer.m_eSize == Wafer_Size.eSize.mm300_RF) m_grid.Set(ref m_bUseBackSide, "BackSide", "Use", "Use BackSide");
                else m_bUseBackSide = false;
            }
            else
            {
                m_bUseBackSide = false; 
            }

            //m_grid.Set()

            if (m_auto.m_strWork == EFEM.eWork.MSB.ToString())
            {
                string sInspMode = m_eInspMode.ToString();
                m_grid.Set(ref sInspMode, m_sInspMode, "InspMode", "InspMode", "Inspect Mode");
                int n = 0;
                foreach (string str in m_sInspMode)
                {
                    if (str == sInspMode)
                    {
                        m_eInspMode = (eInspMode)n;
                        break;
                    }
                    n++;
                }

                switch (m_eInspMode)
                {
                    case eInspMode.FullSlot:
                        break;
                    case eInspMode.CountinuousSlot:
                        string sInspDir = m_eInspDIr.ToString();
                        m_grid.Set(ref sInspDir, m_sInspDir, "InspMode", "InspDir", "Inspect Direction");
                        int nn = 0;
                        foreach (string str in m_sInspDir)
                        {
                            if (str == sInspDir)
                            {
                                m_eInspDIr = (eInspDir)nn;
                                break;
                            }
                            nn++;
                        }
                        m_grid.Set(ref m_nInspCount, "InspMode", "InspCount", "Inspect Slot Count");
                        break;
                    case eInspMode.SelectedSlot:
                        for (int i = 0; i < 13; i++)
                        {
                            m_grid.Set(ref m_aInspSelect[i], "InspMode", "Slot " + (i + 1).ToString(), "Inspect Slot Select");
                        }
                        break;
                }

                for (n = 0; n < m_bUseCode.Length; n++)
                {
                    m_grid.Set(ref m_bUseCode[n], "BinCode", "Code_" + (n + 1).ToString("000"), "Use Bin Code_" + (n + 1).ToString("000"));
                }
            }
            m_grid.Refresh();
        }

        public override void CreateDefaltRCP(string strNewRcp)
        {
            string strRcpBody = "";
            string strOldDateTime = "";
            int nDateTimeStart;
            if (m_strDefaultRCP == null || m_strDefaultRCP == "")
            {
                m_log.Popup(strNewRcp + " Recipe Create Fail, Cause Default Recipe Name Is Invalid !!");
                return;
            }
            if (!File.Exists(GetFileName(m_strDefaultRCP)))
            {
                m_log.Popup(strNewRcp + " Recipe Create Fail, Cause Default Recipe Is Not Exist !!");
                return;
            }
            if (File.Exists(GetFileName(strNewRcp)))
            {
                m_log.Add(strNewRcp + " Recipe Don't Need Create, Cause The Recipe Is Already Exist");
                return;
            }
            File.Copy(GetFileName(m_strDefaultRCP), GetFileName(strNewRcp));
            try
            {
                StreamReader srRcp = new StreamReader(GetFileName(strNewRcp));
                strRcpBody = srRcp.ReadToEnd();
                srRcp.Close();
            }
            catch (Exception ex)
            {
                m_log.Add(ex.Message);
                m_log.Popup(strNewRcp + " Recipe Create / Read Fail, Cause The Recipe File Has Some Problem !!");
                return;
            }
            strRcpBody = strRcpBody.Replace("Recipe,RecipeDataRecipe Name,string," + m_strDefaultRCP, "Recipe,RecipeDataRecipe Name,string," + strNewRcp);
            nDateTimeStart = strRcpBody.IndexOf("Recipe,DateTime,string,");
            if (nDateTimeStart >= 0)
            {
                strOldDateTime = strRcpBody.Substring(nDateTimeStart, ("Recipe,DateTime,string,").Length + DateTime.Now.ToString("yy/MM/dd HH:mm:ss").Length);
                strRcpBody = strRcpBody.Replace(strOldDateTime, "Recipe,DateTime,string," + DateTime.Now.ToString("yy/MM/dd HH:mm:ss"));
            }
            try
            {
                StreamWriter swRcp = new StreamWriter(GetFileName(strNewRcp));
                swRcp.Write(strRcpBody);
                swRcp.Close();
            }
            catch (Exception ex)
            {
                m_log.Add(ex.Message);
                m_log.Popup(strNewRcp + " Recipe Modify Fail, Cause The Recipe File Has Some Problem !!");
                return;
            }
            m_log.Add(strNewRcp + " Recipe Create");
            ReadRecipeDate();
            this.Invalidate();
        }

        public override void SetInfoWafer(Info_Wafer infoWafer, Info_Carrier infoCarrier)
        {
            HW_Aligner_Mom aligner = m_auto.ClassHandler().ClassAligner();
            infoWafer.m_bDirectionNotch = m_bDirectionNotch; 
            infoWafer.m_nOCRDir = (int)m_eOCRDir; 
            infoWafer.m_sRecipe = m_sRecipe;
            infoWafer.m_fAngleChar = m_fAngleChar; 
            infoWafer.m_fAngleAlign = m_fAngleAlign;
            infoWafer.m_bUseBCR = m_bUseBCR;
            infoWafer.m_fAngleBCR = m_fAngleBCR;
            infoWafer.m_fdRBCR = m_fdRBCR;
            infoWafer.m_sTypeBCR = m_sTypeBCR;
            infoWafer.m_bUseOCR = m_bUseOCR;
            infoWafer.m_fAngleOCR = m_fAngleOCR;
            infoWafer.m_fdROCR = m_fdROCR;
            infoWafer.m_bUseVision = m_bUseVision;
            if(infoCarrier != null)
                infoCarrier.m_bUseVision = m_bUseVision;
            if (m_wafer.IsRingFrame()) // ing 171017
            {
                infoWafer.m_bUseBackSide = m_bUseBackSide;
                infoWafer.m_bRotate180 = m_auto.ClassHandler().ClassAligner().m_bRotate180;
            }
            infoWafer.m_bRunEdge = m_bRunEdge;
            infoWafer.m_fOCRReadScore = m_fOCRReadScore;    //KDG 161025 Add OCR Score
            infoWafer.m_bOnlyInkMark = m_bOnlyInkMark;
            infoWafer.m_sWaferIDType = m_sWaferIDType;
            if (m_sWaferIDType == "PM") infoWafer.m_strWaferID = m_sPMWaferID; // ing 170410
            infoWafer.m_bUseAligner = aligner.m_bEnableAlign; // ing 170531
        }


    }
}
