using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ezAutoMom
{
    public interface HW_LoadEV_Mom : Control_Child
    {
        void LoadUp();
        void Down(int msDown);
        void Blow(bool bBlow);
        bool IsPaper();
        void RunInit();
        void Stop();
        bool IsDone();
        bool IsReady(); // ing 171130
        bool IsPaperBoxFull(); 
        bool IsPaperBoxCheck(); 
    }
}
