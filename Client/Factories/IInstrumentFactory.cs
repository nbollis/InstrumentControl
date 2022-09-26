using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentClient
{
    public interface IInstrumentFactory
    {
        IInstrument Api { get; }
        IInstrument CreateInstrumentApi();
    }
}
