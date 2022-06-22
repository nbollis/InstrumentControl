using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine; 

namespace SpectrumAveraging
{
    public class SpectrumAveragingOptions
    {
        [Option('r', "rejection")]
        public RejectionType RejectionType { get; set; } = RejectionType.NoRejection;
        [Option('w', "weighting")]
        public WeightingType WeightingType { get; set; } = WeightingType.NoWeight;
        [Option('m', "merging")]
        public SpectrumMergingType SpectrumMergingType { get; set; } = SpectrumMergingType.SpectrumBinning;
        [Option('p', "percentile")]
        public double Percentile { get; set; } = 0.9;
        [Option("minsigma")]
        public double MinSigmaValue { get; set; } = 1.3;
        [Option("maxsigma")]
        public double MaxSigmaValue { get; set; } = 1.3;
        [Option('b', "binsize")]
        public double BinSize { get; set; } = 0.02;
    }
}
