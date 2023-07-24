using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ezAutoMom
{
    public interface HW_RnR_Mom
    {
        void UpdateRnRState(int nLP);                      //170213 SDH ADD LoadPort 별 RNR 갱신 (횟수 다차면 RnR 종료)
        void SetInit(int nLP);                             //170213 SDH ADD RnR 시작시 처음 Setting
    }
}
