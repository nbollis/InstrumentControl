using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentClient
{
    public class ThermoTribridFactory : IInstrumentFactory
    {
        public IInstrument Api { get; }
        public ThermoTribridFactory()
        {
            
        }
        public IInstrument CreateInstrumentApi()
        {
            return new ThermoTribrid();
        }
    }
}
