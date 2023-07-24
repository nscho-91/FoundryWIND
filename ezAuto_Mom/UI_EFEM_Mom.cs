using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;

namespace ezAutoMom
{

    public interface UI_EFEM_Mom
    {
        void Init(Auto_Mom auto);
        IDockContent GetIDockContent(string persistString);
        IDockContent GetContentFromPersistString(string persistString);
        void ShowPanel(DockPanel dockPanel);
        void SetEnable(bool bEnable);
        void UpdateLoadport(int nID);
        void AddBacksideUI();
        void AddImageVSUI();
        void SetLP();
        void Invalidate();
        void SetWTRMotion(eWTRSimnulMotion Motion);
        void AddRunData(Work_Mom.cRunData RunData);
        void AddRunData2();
        void AddStopData(Work_Mom.cStopData StopData);
        void ThreadStop(); 
    }

    public enum eWTRSimnulMotion
    {
        None,
        Home,
        LP1GetUArm,
        LP2GetUArm,
        LP1PutUArm,
        LP2PutUArm,
        AlignerGetUArm,
        AlignerPutUArm,
        VisionGetUArm,
        VisionPutUArm,
        LP1GetLArm,
        LP2GetLArm,
        LP1PutLArm,
        LP2PutLArm,
        AlignerGetLArm,
        AlignerPutLArm,
        VisionGetLArm,
        VisionPutLArm,
    }
}
