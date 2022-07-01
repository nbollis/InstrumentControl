using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Scans;

namespace IMSScanClassExtensions
{
    public class ICustomScanInstance : ICustomScan
    {
        public double SingleProcessingDelay { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IDictionary<string, string> Values { get; set; }

        public long RunningNumber { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICustomScanInstance()
        {
            Values = new Dictionary<string, string>();
        }
    }
}
