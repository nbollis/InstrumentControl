using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientInstrument
{
    public interface IInstrumentFactory
    {
        IInstrument Api { get; }
        IInstrument CreateInstrumentApi();
    }
}
