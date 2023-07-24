using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ezAutoMom
{
    public interface If_InlineFlow
    {
        bool IsReady(string strReady);
        bool IsDone(string strDone);
        bool StartLoad(Info_Strip infoStrip);
        bool Send(); 
        Info_Strip GetStrip();
        bool SetReady();
        bool SetDone(Info_Strip infoStrip); 
    }
}
