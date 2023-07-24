using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ezTools; 

namespace ezAutoMom
{
    public interface Control_Child
    {
        void ControlGrid(Control_Mom control, ezGrid rGrid, eGrid eMode); 
    }
}
