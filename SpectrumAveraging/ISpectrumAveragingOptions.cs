using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using TaskInterfaces; 

namespace SpectrumAveraging
{
    public interface ISpectrumAveragingOptions : ITaskOptions<ISpectrumAveragingOptions>
    {
        [Option('r', "rejection")]
        public RejectionType RejectionType { get; set; }
        [Option('w', "weighting")]
        public WeightingType WeightingType { get; set; }
        [Option('m', "merging")]
        public SpectrumMergingType SpectrumMergingType { get; set; } 
        [Option('p', "percentile")]
        public double Percentile { get; set; } 
        [Option("minsigma")]
        public double MinSigmaValue { get; set; } 
        [Option("maxsigma")]
        public double MaxSigmaValue { get; set; } 
        [Option('b', "binsize")]
        public double BinSize { get; set; } 
    }
    public class SpectrumAveragingOptions : ISpectrumAveragingOptions
    {
        public RejectionType RejectionType { get; set; }
        public WeightingType WeightingType { get; set; }
        public SpectrumMergingType SpectrumMergingType { get; set; }
        public double Percentile { get; set; }
        public double MinSigmaValue { get; set; }
        public double MaxSigmaValue { get; set; }
        public double BinSize { get; set; }
        public SpectrumAveragingOptions()
        {

        }
    }
}
