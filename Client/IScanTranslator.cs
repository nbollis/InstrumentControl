using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;

namespace Client
{
    public interface IScanTranslator
    {
        T Translate<T>(SingleScanDataObject ssdo) where T: new(); 
    }
}
