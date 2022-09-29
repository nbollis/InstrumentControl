using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;

namespace Client
{
    public abstract class ScanTranslator
    {
        public abstract void Translate(SingleScanDataObject ssdo); 
    }
}
