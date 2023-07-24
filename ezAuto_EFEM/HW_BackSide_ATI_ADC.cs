using ezTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ezAuto_EFEM
{
    class HW_BackSide_ATI_ADC
    {
        #region Singleton
        private static HW_BackSide_ATI_ADC _instance;
        private HW_BackSide_ATI_ADC()
        {
            #region Defect
            sDefectName[0] = "Abnormal Grind Mark";
            sDefectCode[0] = "80";
            sDefectName[1] = "Broken Wafer";
            sDefectCode[1] = "79";
            sDefectName[2] = "Crack";
            sDefectCode[2] = "79";
            sDefectName[3] = "Dimple";
            sDefectCode[3] = "83";
            sDefectName[4] = "Contaimnation";
            sDefectCode[4] = "83";
            sDefectName[5] = "Scratch";
            sDefectCode[5] = "82";
            sDefectName[6] = "Bubble";
            sDefectCode[6] = "82";
            sDefectName[7] = "Particle";
            sDefectCode[7] = "82";
            sDefectName[8] = "Center Spot Mark";
            sDefectCode[8] = "80";
            sDefectName[9] = "Edge Chipping";
            sDefectCode[9] = "79";
            sDefectName[10] = "V Chipping";
            sDefectCode[10] = "79";
            sDefectName[11] = "Stain Mark";
            sDefectCode[11] = "80";
            sDefectName[12] = "Burn Mark";
            sDefectCode[12] = "80";
            sDefectName[13] = "Coarse Grind Mark";
            sDefectCode[13] = "80";
            sDefectName[14] = "Ring Mark";
            sDefectCode[14] = "80";
            sDefectName[15] = "Good";
            sDefectCode[15] = "1";
            sDefectName[16] = "Unkown";
            sDefectCode[16] = "85";
            #endregion
        }
        public static HW_BackSide_ATI_ADC Instance
        {
            get
            {
                return (null == _instance) ? _instance = new HW_BackSide_ATI_ADC() : _instance;
            }
        }
        #endregion
        bool bUseAdc = false;

        public bool UseAdc
        {
            get { return bUseAdc; }
            set { bUseAdc = value; }
        }
        string sRecipe = @"C:\Avis\Recipe.vrws";

        public string Recipe
        {
            get { return sRecipe; }
            set { sRecipe = value; }
        }

        double dScore = 70.0f;

        public double Score
        {
            get { return dScore; }
            set { dScore = value; }
        }

        public string[] sDefectName = new string[17];
        public string[] sDefectCode = new string[17];
        public string m_id;

        Dictionary<string, string> Dic = new Dictionary<string, string>();
        public void Init(string id)
        {
            m_id = id;
        }

        public string GetRoughbin(string name)
        {
            string sReturn = "0";
            Dic.TryGetValue(name.ToLower().Trim(), out sReturn);
            return sReturn + " ";
        }

        private void UpdateDic()
        {
            Dic.Clear();

            for (int i = 0; i < sDefectName.Length; i++)
            {
                if (!Dic.Keys.Contains(sDefectName[i].ToLower().Trim()))
                {
                    Dic.Add(sDefectName[i].ToLower().Trim(), sDefectCode[i]);
                }
            }
        }

        public void RunRecipeGrid(ezGrid grid, eGrid eMode, ezJob job = null)
        {
            grid.Set(ref bUseAdc, m_id, "Use", "Use");
            grid.Set(ref sRecipe, m_id, "Recipe Path", "ViDi Recipe Path");
            grid.Set(ref dScore, m_id, "Classify Score", "Classify Score");
        }

        public void RunGrid(ezGrid grid, eGrid eMode, ezJob job = null)
        {
            for (int i = 0; i < sDefectName.Length; i++)
            {
                string sName = "Defect Name" + (i + 1);
                string sCode = "Roughbin Code" + (i + 1);

                grid.Set(ref sDefectName[i], m_id, sName, "ViDi Defect Name");
                grid.Set(ref sDefectCode[i], m_id, sCode, "Rough Bin Code");
            }
            UpdateDic();
        }
    }
}
