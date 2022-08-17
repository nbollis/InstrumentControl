using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientInstrument
{
    public class ThermoTribridFactory : IInstrumentFactory
    {
        public IInstrument Api { get; }
        public ThermoTribridFactory()
        {
            Api = CreateInstrumentApi();
        }
        public IInstrument CreateInstrumentApi()
        {
            return new ThermoTribrid();
        }
    }
}
