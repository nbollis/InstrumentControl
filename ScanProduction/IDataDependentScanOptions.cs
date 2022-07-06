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
        public double CollisionEnergy { get; set; }
        [Option]
        public double AGCTarget { get; set; }

        public double PrecursorMass { get; set; }
        public string ScanType { get; set; }
    }

    public class DataDependentScanOptions : IDataDependentScanOptions
    {
        public double IsolationWidth { get; set; }
        public double Resolution { get; set; }
        public double MaxIT { get; set; }
        public double CollisionEnergy { get; set; }
        public double AGCTarget { get; set; }

        public double PrecursorMass { get; set; }
        public string ScanType { get; set; } = "MSn";
    }

}
