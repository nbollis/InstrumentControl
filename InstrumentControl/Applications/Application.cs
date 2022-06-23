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
        public abstract List<InstrumentControlTask> TaskList { get; set; }
        public abstract void ProcessScans();

    }
    public class ScanAveragingApp : Application
    {
        public override List<InstrumentControlTask> TaskList { get; set; }
        public override void ProcessScans()
        {
            // needs: 
            // normalization
            // standardization
            // averaging 
            throw new NotImplementedException();
        }

    }
    public class ScanAveragingAppOptions : IApplicationOptions, IStandardizationOptions, 
        INormalizationOptions, ISpectrumAveragingOptions
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
