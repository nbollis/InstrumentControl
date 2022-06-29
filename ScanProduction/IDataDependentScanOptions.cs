using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskInterfaces;

namespace ScanProduction
{
    public interface IDataDependentScanOptions : ITaskOptions
    {
        [Option]
        public double IsolationWidth { get; set; }
        [Option]
        public double Resolution { get; set; }
        [Option]
        public double MaxIT { get; set; }
        [Option]
        public double NCE { get; set; }
        [Option]
        public int NCE_NormCharge { get; set; }
        [Option]
        public double AGC_Target { get; set; }

        public double IsolationRangeLow { get; set; }
        public double IsolationRangeHight { get; set; }
    }

    public class DataDependentScanOptions : IDataDependentScanOptions
    {
        public double IsolationWidth { get; set; }
        public double Resolution { get; set; }
        public double MaxIT { get; set; }
        public double NCE { get; set; }
        public int NCE_NormCharge { get; set; }
        public double AGC_Target { get; set; }

        public double IsolationRangeLow { get; set; }
        public double IsolationRangeHight { get; set; }
    }

}
