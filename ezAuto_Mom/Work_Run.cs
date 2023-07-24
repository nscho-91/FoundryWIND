using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ezTools;

namespace ezAutoMom
{
    public enum eWorkRun 
    { 
        Init, 
        Ready, 
        Run, 
        Home, 
        Warning0,
        Warning1,
        Error, 
        Reset, 
        AutoLoad, 
        AutoUnload, 
        PickerSet
    }; 

    public interface Work_Run
    {
        void ThreadStop();
        void Run(); 
        void Stop(bool bDelay = true);
        void Reset();
        void Home();
        eWorkRun GetState();
        bool IsManual(); 
        void SetError(eAlarm alarm);
        void SetError(eAlarm alarm, Log log, int iError);
        bool IsEnableSW();
        void DoorLock(bool bOn);
        void SetManual(eWorkRun eRun); 
        void SetReady(); 
    }
}
