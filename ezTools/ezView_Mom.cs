using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ezTools
{
    public interface ezView_Mom
    {
        void InvalidView(bool bInvalidate);
        ezImg ClassImage();
        ezImgView ClassImageView();
    }
}
