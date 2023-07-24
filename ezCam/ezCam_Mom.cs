using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ezTools; 

namespace ezCam
{
    interface ezCam_Mom
    {
        bool SetExposure(double ms);
        bool IsBusy();
        bool IsGrabDone();
        bool SetROI(CPoint cp, CPoint sz);
        bool SetTrigger(bool bOn);
        void GetszImage(ref CPoint szImg);
        bool Reset();
    }
}
