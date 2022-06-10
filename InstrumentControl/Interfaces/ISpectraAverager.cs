using MassSpectrometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
