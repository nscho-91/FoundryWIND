using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ezAutoMom
{

    public class InitState
    {

        public enum eInitModule{
            MotorIO,
            WTR,
            Loadport,
            Aligner,
            Vision,
            OHT,
            SECSGEM
        }

        string sPath = @"C:\AVIS\Init\InitState.ini";
        public InitState()
        {
          
        }

        public void Init()
        {
            for (int i = 0; i < Enum.GetNames(typeof(eInitModule)).Length; i++) {
                string sName = Enum.GetName(typeof(eInitModule), i).ToString();
                IniFile.G_IniWriteBoolValue("InitModule", sName, false, sPath);
            }
        }

        public void SetInitDone(eInitModule InitModule)
        {
            string sName = Enum.GetName(typeof(eInitModule), InitModule).ToString();
            IniFile.G_IniWriteBoolValue("InitModule",sName, true, sPath);
        }



    }
}
