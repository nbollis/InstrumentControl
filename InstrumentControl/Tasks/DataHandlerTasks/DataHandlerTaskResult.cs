using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    public class DataHandlerTaskResult : TaskResults
    {
        public double[][] XArrays { get; set; }
        public double[][] YArrays { get; set; }
        public double[] TotalIonCurrent { get; set; }

        public void SetData(double[][] xarr, double[][] yarr, double[] tic)
        {
            XArrays = xarr;
            YArrays = yarr;
            TotalIonCurrent = tic;
        }
    }
}
