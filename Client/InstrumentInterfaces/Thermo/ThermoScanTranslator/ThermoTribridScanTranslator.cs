using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;

namespace Client
{
    public class ThermoTribridScanTranslator : IScanTranslator
    {
        public T Translate<T>(SingleScanDataObject ssdo) where T : new()
        {
            if (typeof(T) != typeof(Dictionary<string, string>))
            {
                throw new ArgumentException("Invalid return object type for Thermo Tribrid instrument class!"); 
            }
            
            
        }

        private Dictionary<string, string> TranslateSsdo(SingleScanDataObject ssdo)
        {

        }
    }
}
