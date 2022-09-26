using System;
using System.Collections.Generic;
using Data;
using SpectralAveraging;

namespace WorkflowServer
{
    public abstract class Application : IApplication
    {   
        public Application()
        {
            
        }
        public abstract void ProcessScans(object sender, ThresholdReachedEventArgs e); 

    }
    public class ScanAveragingApp : Application
    {
        private SpectralAveragingOptions AveragingOptions { get; set; }
        private StandardizationOptions StandardizationOptions { get; set; }
        private NormalizationOptions NormalizationOptions { get; set; }
   
        public void GetOptions<T>(T options)
        {
            AveragingOptions = options as SpectralAveragingOptions;
            StandardizationOptions = options as StandardizationOptions;
            NormalizationOptions = options as NormalizationOptions; 
        }
        public override void ProcessScans(object sender, ThresholdReachedEventArgs e)
        {
            Main(e.Data); 
        }
        private void Main(List<SingleScanDataObject> data)
        {
            // perform scan standardization
            // perform scan normalization
            // perform specrum averaging 
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
