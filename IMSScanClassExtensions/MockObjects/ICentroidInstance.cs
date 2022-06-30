using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;
using Thermo.Interfaces.SpectrumFormat_V1;

namespace IMSScanClassExtensions
{
    public class ICentroidInstance : ICentroid
    {
        #region Not Yet Implemented
        public bool? IsExceptional => throw new NotImplementedException();

        public bool? IsReferenced => throw new NotImplementedException();

        public bool? IsMerged => throw new NotImplementedException();

        public bool? IsFragmented => throw new NotImplementedException();

        public int? Charge => throw new NotImplementedException();

        public IMassIntensity[] Profile => throw new NotImplementedException();

        public double? Resolution => throw new NotImplementedException();

        public int? ChargeEnvelopeIndex => throw new NotImplementedException();

        public bool? IsMonoisotopic => throw new NotImplementedException();

        public bool? IsClusterTop => throw new NotImplementedException();

        #endregion

        public double Mz { get; set; }
        public double Intensity { get; set; }

        public ICentroidInstance(double mz, double intensity)
        {
            Mz = mz;
            Intensity = intensity;
        }
    }
}
