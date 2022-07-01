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
        public bool? IsExceptional { get; set; }

        public bool? IsReferenced { get; set; }

        public bool? IsMerged { get; set; }

        public bool? IsFragmented { get; set; }

        public int? Charge { get; set; }

        public IMassIntensity[] Profile => throw new NotImplementedException();

        public double? Resolution { get; set; }

        public int? ChargeEnvelopeIndex { get; set; }

        public bool? IsMonoisotopic { get; set; }

        public bool? IsClusterTop { get; set; }

        public double Mz { get; set; }
        public double Intensity { get; set; }

        #region Constructors

        public ICentroidInstance()
        {

        }
        public ICentroidInstance(double mz, double intensity)
        {
            Mz = mz;
            Intensity = intensity;
        }

        #endregion


    }
}
