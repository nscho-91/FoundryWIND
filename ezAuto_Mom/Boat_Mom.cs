using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ezAutoMom
{
    public enum eBoat 
    { 
        Init, 
        Home, 
        Ready, 
        Done, 
        Run, 
        RunReady, 
        Start,
        Error
    };

    public interface Boat_Mom
    {
        Info_Strip GetInfoStrip();
        string GetID();
        eBoat GetState(); 
        void RunVac(bool bVac);
        bool IsVac();
        void StartHome(); 
        void SetReady(bool bWait);
        void SetRun(Info_Strip infoStrip, bool bRun);
        bool IsReady(); 
        bool IsDone();
        bool IsConnect(); 
        long GetRunTime(bool bLoad);
        void GetGVSlope(ref double fSlope, ref double fOffset);
        bool RunInit();
        bool IsRun();
        bool CheckInitOK();
        bool MoveMark(int nPos, bool bWait);
    }
}
