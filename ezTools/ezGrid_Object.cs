using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyGridEx;

namespace ezTools
{
    public interface ezGrid_Object
    {
        void JobOpen(ezJob job, string strGroup, string strID);
        void JobSave(ezJob job, string strGroup, string strID);
        void RegRead(ezRegistry reg, string strID);
        void RegWrite(ezRegistry reg, string strID);
        void Update(string strID, object objOld, object objNew);
        BrowsableTypeConverter.LabelStyle GetBrowsableType(); 
    }
}
