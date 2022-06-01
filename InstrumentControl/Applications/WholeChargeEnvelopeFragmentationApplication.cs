using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;

namespace InstrumentControl
{
    public class WholeChargeEnvelopeFragmentationApplication : Application
    {
        // receive scans
        // pull metadata async(List<IMsScan> scans)
        // average scans(List<MzSpectrum> scans.Select(p => p.MassSpectrum).ToList());
        // get envelopes
        // fragment envelopes


        public WholeChargeEnvelopeFragmentationApplication() : base(MyApplication.WholeChargeStateEnvelopeFragmentation)
        {

        }

        public override void ProcessScans(object? sender, ThresholdReachedEventArgs e)
        {





            throw new NotImplementedException();
        }
    }
}
