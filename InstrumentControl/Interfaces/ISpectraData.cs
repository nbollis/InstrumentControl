using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    public interface ISpectraData
    {
        public double[][] XArrays { get; set; }
        public double[][] YArrays { get; set; }
        public double[] TotalIonCurrent { get; set; }
        

    }
}
