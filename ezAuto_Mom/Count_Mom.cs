using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ezTools;

namespace ezAutoMom
{
    public enum eResult { eInit, eInspect, eCheck, eMark, eError = 100000, eRework, eVerify }

    public interface Count_Mom //forget
    {
        void Init(string id, Auto_Mom auto);
        void Start();
        void ThreadStop();
        void Clear();
        bool IsReady();
        bool IsFull();
        bool IsMatchSensor();
        void Print();
        void FileSave();
        int FindTray(int nXout, ref CPoint cpTray);
        void Add(Info_Strip infoStrip);
        int GetNeedPaperTray();
        CPoint GetSize(); 
        void ReDraw(); 
        bool GetDynamic(); 
        int GetMaxLoad();

        void CheckClear();
        void InitTray();
        void ClearTray();
        void ShowTray(CPoint cp);
        int FindTray(eTrayType eType, int nXout, bool bMatch);
        void RunThread();
        void RunThreadLED();
        void CalcTime();
        bool IsDead();
        void ReadReg();
        void SaveReg();
        void SaveReg(int nXout);
        Count_Tray GetTray(int index);
    }
}
