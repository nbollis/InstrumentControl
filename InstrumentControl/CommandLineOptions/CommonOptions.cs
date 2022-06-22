using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine; 

namespace InstrumentControl.CommandLineOptions
{
    public class CommonOptions
    {

    }
    public class SpectrumAveragerOptions : CommonOptions
    {
        [Option(Default = (string)"NoRejection")]
        public string? RejectionType { get; set; }
        [Option(Default = (string)"NoWeight")]
        public string? WeightingType { get; set; }
        [Option(Default = (string)"SpectrumBinning")]
        public string? SpectrumMergeType { get; set; }
        [Option(Default = (double)0.9)]
        public double Percentile { get; set; }
        [Option(Default = (double)1.3)]
        public double MinSigmaValue { get; set; }
        [Option(Default = (double)1.3)]
        public double MaxSigmaValue { get; set; }   
        [Option(Default = (double)0.02)]
        public double BinSize { get; set; }
        
    }


}
