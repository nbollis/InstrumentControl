using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpectralAveraging;

namespace WorkflowServer
{
    public class DdaWorkflowOptions : IWorkflowOptions
    {
        public int TopNPeaks { get; private set; }
        public bool PerformAveraging { get; private set; }
        public SpectralAveragingOptions? SpectralAveragingOptions { get; private set; }
        public DataDependentScanOptions DataDependentScanOptions { get; private set; }

        public DdaWorkflowOptions(int n, DataDependentScanOptions ddaOptions, bool performAveraging, SpectralAveragingOptions? options = null)
        {
            TopNPeaks = n;
            DataDependentScanOptions = ddaOptions;
            PerformAveraging = performAveraging;
            SpectralAveragingOptions = options;
        }
    }
}
