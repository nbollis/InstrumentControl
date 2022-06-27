using IMSScanClassExtensions;
using MassSpectrometry;
using MzLibUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;

namespace InstrumentControl
{
    public interface ISpectraAverager : ISpectraProcesor
    {
        public static MzSpectrum CompositeSpectrum
        {
            get { return  _compositeSpectrum;  }
            set { _compositeSpectrum = value; }
        }
        protected static MzSpectrum _compositeSpectrum;



    }
}
