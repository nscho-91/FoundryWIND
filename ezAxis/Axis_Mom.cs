using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ezTools; 

namespace ezAxis
{
    enum eAxis { eInit, eReady, eHome, eMove }; 

    public interface Axis_Mom
    {
        void ServoOn(bool bOn);
        void ResetAlarm();
        bool Move(double fPos, double fSlow = 1.0);
        bool Move(double fPos, double fV, double fAcc); 
        bool MoveV(double fPos, double fV);
        bool MoveV(double fPos, double fPosV, double fV);
        void Jog(double fScale);
        void StopAxis(bool bEmg = false);
        bool HomeSearch();
        bool IsReady();
        bool IsAxisMove();
        void SetTrigger(double fPos0, double fPos1, double dPos, bool bCmd, double dTrigTime = 2);
        void StopTrigger();
        double GetPos(bool bCmd);
        void SetPos(bool bCmd, double fPos); 
        void SetCaption(string strID);
        void OverrideVel(double fV);
        bool WaitReady(double dp = -1);
        bool IsHome();
        bool IsPlusLimit();
        bool IsMinusLimit();
        string GetID();
        double GetPosDst();
        void LoadAxis(); 
        void SaveAxis();
        void ThreadStop();
        bool IsServoOn();
        void StartRepeat();
        void RunGrid(ezGrid rGrid, eGrid eMode, ezJob job = null); 
    }
}
