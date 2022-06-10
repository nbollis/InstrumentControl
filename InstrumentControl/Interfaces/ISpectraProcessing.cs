using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    public interface ISpectraProcessing
    {
        public static int ScansToProcess { get; set; }
        public static double MinX { get; set; }
        public static double MaxX { get; set; }
        public static double[][] XArrays { get; set; }
        public static double[][] YArrays { get; set; }
        public static double[] TotalIonCurrent { get; set; }
    }
}
