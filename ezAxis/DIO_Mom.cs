using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ezAxis
{
    public interface DIO_Mom
    {
        bool GetInputBit(int n);
        bool GetOutputBit(int n);
        void WriteOutputBit(int n, bool bOn);
        void SetDICaption(int nDI, string str);
        void SetDOCaption(int nDO, string str);
    }
}
