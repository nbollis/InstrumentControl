using System;
using System.Collections.Generic;
using TaskInterfaces;
using Normalization;
using Standardization;
using SpectrumAveraging; 

namespace InstrumentControl
{
    public abstract class Application : IApplication
    {   
        public Application()
        {
            
        }

    }
    public class ScanAveragingApp : Application
    {
        public SpectrumAveragingOptions AveragingOptions { get; set; }
        public StandardizationOptions StandardizationOptions { get; set; }
        public NormalizationOptions NormalizationOptions { get; set; }
   
        public void GetOptions<T>(T options)
        {
            AveragingOptions = options as SpectrumAveragingOptions;
            StandardizationOptions = options as StandardizationOptions;
            NormalizationOptions = options as NormalizationOptions; 
        }
        public void ProcessScans(object sender, ThresholdReachedEventArgs e)
        {

        }

    }
    public class ScanAveragingAppOptions : IApplicationOptions,
        IStandardizationOptions, INormalizationOptions, ISpectrumAveragingOptions
    {
        #region IApplicationOptions members
        public bool Live { get; set; }
        #endregion
        
        #region IStandardizationOptions members
        public double MinMass { get; set; }
        public double MaxMass { get; set; }
        public double Delta { get; set; }
        #endregion

        #region INormalizationOptions members
        public bool PerformNormalization { get; set; }

        #endregion

        #region ISpectrumAveragingOptions members
        public RejectionType RejectionType { get; set; }
        public WeightingType WeightingType { get; set; }
        public SpectrumMergingType SpectrumMergingType { get; set; }
        public double Percentile { get; set; }
        public double MinSigmaValue { get; set; }
        public double MaxSigmaValue { get; set; }
        public double BinSize { get; set; }
        #endregion
         
    }
}
