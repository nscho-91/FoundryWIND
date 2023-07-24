using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ezAutoMom;
using ezTools;

namespace ezAuto_EFEM
{
    class HW_Aligner_ATI_AOI_Data
    {
        public enum eInspect
        {
            WhiteWafer,
            White300, 
            BlackWafer,
            Black_Blob,
//            Black_Dicing
        }
        public eInspect m_eInspect = eInspect.Black_Blob;
        string[] m_sInspects = Enum.GetNames(typeof(eInspect));

        public enum eShape
        {
            Notch,
            Flat
        }
        public eShape m_eShape = eShape.Notch;
        string[] m_sShapes = Enum.GetNames(typeof(eShape));

        string m_id;
        Log m_log; 

        public int m_nGV = 50;
        public int c_nNotchSize = 45;

        public int m_nMargin = 200;
        public int m_dxROI = 0;

        public int m_minR = 3000;
        public int m_nErosion = 0; // Erosion 하기위한 변수,  0 == Unuse
        public int m_nNotchThreadhold = 20;

        public int m_nChipping = 0;

        public bool m_bLiveGrab = false;
        public double m_degAlign = 0.08;

        public void Init(string id, Log log)
        {
            m_id = id;
            m_log = log; 
            m_imgNotch = new ezImg("Notch", m_log);
        }

        public void RunGrid(ezGrid rGrid, eGrid eMode)
        {
            rGrid.Set(ref m_bLiveGrab, "Inspect", "LiveGrab", "LiveGrab");
            if (eMode == eGrid.eRegRead) m_bLiveGrab = false;
            rGrid.Set(ref m_degAlign, "Inspect", "Align", "Aligner Align (Degree)");
            rGrid.Set(ref m_minR, "Inspect", "minR", "Minimum Radius of Wafer (Pixel)");
            rGrid.Set(ref m_nMargin, "Inspect", "Margin", "The Margin Size of Inspection");
            rGrid.Set(ref m_dxROI, "Inspect", "dxROI", "The Margin Size of Inspection");
            rGrid.Set(ref m_nErosion, "Notch", "Erosion", "The Number of Pixel for BlackWafer Erosion");
        }

        public void RecipeGrid(bool bRAC, ezGrid rGrid, eGrid eMode, ezJob job, Recipe_Mom m_Recipe)
        {
            if (bRAC)
            {
                var File = job.m_strTitle.Split('\\');
                m_eInspect = (eInspect)Enum.Parse(typeof(eInspect), m_Recipe.GetRACVal_str(File[2], "AlignInspect"));
                m_eShape = (eShape)Enum.Parse(typeof(eShape), m_Recipe.GetRACVal_str(File[2], "AlignShape"));
                m_nGV = (int)m_Recipe.GetRACVal_Num(File[2], "AlignGV");
            }
            RecipeGrid(rGrid, eMode, job, ref m_eInspect);
            RecipeGrid(rGrid, eMode, job, ref m_eShape);
            rGrid.Set(ref m_nGV, "Align", "GV", "GV (0 ~ 255)");
        }

        public void RecipeGrid(ezGrid rGrid, eGrid eMode, ezJob job, ref eInspect nInspect)
        {
            string sInspect = nInspect.ToString();
            rGrid.Set(ref sInspect, m_sInspects, "Align", "Inspect", "Inspect Mode");
            int n = 0;
            foreach (string str in m_sInspects)
            {
                if (str == sInspect)
                {
                    nInspect = (eInspect)n;
                    return;
                }
                n++;
            }
            if (sInspect == "Sobel") nInspect = eInspect.Black_Blob;
            if (sInspect == "Sobel2") nInspect = eInspect.Black_Blob;
            if (sInspect == "Black_Down") nInspect = eInspect.Black_Blob;
        }

        public void RecipeGrid(ezGrid rGrid, eGrid eMode, ezJob job, ref eShape nShape)
        {
            string sShape = nShape.ToString();
            rGrid.Set(ref sShape, m_sShapes, "Align", "Shape", "Wafer Shape");
            int n = 0;
            foreach (string str in m_sShapes)
            {
                if (str == sShape)
                {
                    nShape = (eShape)n;
                    return;
                }
                n++;
            }
        }

        public ezImg m_imgNotch = null;
        public string m_sRecipe = "";
        public bool m_bNotch = false;
        public CPoint c_szNotchImg = new CPoint(200, 100);
        public CPoint c_szNotchImg2 = new CPoint(100, 50);
        public CPoint c_szNotchROI = new CPoint(300, 150);
        public CPoint c_szNotchROI2 = new CPoint(150, 75);

        public void SetRecipe(string sRecipe)
        {
            m_bNotch = false;
            //if (sRecipe == m_sRecipe) return;
            m_sRecipe = sRecipe;

            if (m_imgNotch.FileOpen("c:\\TestImg\\Notch\\" + sRecipe + ".bmp"))
            {
                m_bNotch = false;
                if (m_imgNotch.FileOpen("c:\\TestImg\\Notch\\Notch.bmp"))
                {
                    m_bNotch = true;
                }
                else
                {
                    m_bNotch = true;
                    m_log.Add("Use CrossCorr : " + sRecipe);
                }
            }
            else
            {
                m_bNotch = true;
                m_log.Add("Use CrossCorr : " + sRecipe);
            }
        }
    }
}
