using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ezTools; 

namespace ezAutoMom
{
    public interface WTR_Child
    {
        string GetID();
        int GetID(string sLocate); 
        bool IsPutOK(string sLocate, InfoWafer infoWaferPut, ref int nPos, ref int nID);
        bool IsGetOK(string sLocate, ref InfoWafer infoWaferGet, ref int nPos, ref int nID);
        void RunChildGrid(ezGrid rGrid, eGrid eMode);
        bool SetInfoWafer(int nID, InfoWafer infoWafer);
        InfoWafer GetInfoWafer(int nID);
        bool Is2Step(int nID);
        bool BeforeGet(string sLocate, bool bWait);
        bool BeforePut(string sLocate, bool bWait);
        bool AfterGet(string sLocate, bool bWait);
        bool AfterPut(string sLocate, bool bWait);
    }
}
