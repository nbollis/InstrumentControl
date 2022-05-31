using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    public class NumberAverager
    {
        public double Average { get; set; }
        public NumberAverager(List<double> doubleList)
        {
            Average = doubleList.Average();
        }
    }
}
