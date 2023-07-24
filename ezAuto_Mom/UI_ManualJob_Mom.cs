using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ezAutoMom; 

namespace ezAutoMom
{
    public interface UI_ManualJob_Mom
    {
        void Init(string id, Auto_Mom auto, int nLP, bool MapdataUse, string strPath);
        void SetRNRMode();
        void SetXGemMode(int nLP);
        DialogResult ShowModal(); 
    }
}
