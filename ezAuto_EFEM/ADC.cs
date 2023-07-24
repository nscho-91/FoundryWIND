using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ezTools;

namespace ezAuto_EFEM
{
    public class ADC
    {
        #region Singleton
        private static ADC _instance;
        private string m_strPath = "";
        private ADC() { }
        public static ADC Instance
        {
            get
            {
                return (null == _instance) ? _instance = new ADC() : _instance;
            }
        }
        #endregion

        public void Classify(String strPath, Bitmap img, out string name, out double score, Log log = null)
        {
            name = "";
            score = 0.0;
        }

        void Init(string strPath)
        {
        }

        public void ThreadStop()
        {
        }
    }
}
