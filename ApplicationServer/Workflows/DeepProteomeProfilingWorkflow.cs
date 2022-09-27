//using Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using SpectralAveraging;

//namespace WorkflowServer
//{
//    public class DeepProteomeProfilingWorkflow : Workflow
//    {
//        private StandardizationTask standardizationTask { get; set; }
//        private StandardizationOptions standardizationOptions { get; set; }
//        private NormalizationTask normalizationTask { get; set; }
//        private NormalizationOptions normalizationOptions { get; set; } 
//        private SpectralAveragingTask SpectralAveragingTask { get; set; }
//        private SpectrumAveragingOptions spectrumAveragingOptions { get; set; }
//        private DataDependentScanTask dataDependentScanTask { get; set; }
//        private DataDependentScanOptions dataDependentScanOptions { get; set; }

//        public void GetOptions<T>(T options)
//        {
//            standardizationOptions = options as StandardizationOptions;
//            normalizationOptions = options as NormalizationOptions;
//            spectrumAveragingOptions = options as SpectrumAveragingOptions;
//            dataDependentScanOptions = options as DataDependentScanOptions;

//            standardizationTask = new StandardizationTask();
//            normalizationTask = new NormalizationTask();
//            SpectralAveragingTask = new SpectralAveragingTask();
//            dataDependentScanTask = new DataDependentScanTask();
//        }

//        public override void ProcessScans(object sender, ThresholdReachedEventArgs e)
//        {
//            MultiScanDataObject data = new MultiScanDataObject(e.Data);
//        }
//    }

//    public class DeepProteomeProfilingOptions : IWorkflowOptions, 
//        IStandardizationOptions, INormalizationOptions, ISpectrumAveragingOptions,
//        IDataDependentScanOptions
//    {
//        #region IApplicationOptions members
//        public bool Live { get; set; }
//        #endregion

//        #region IStandardizationOptions members
//        public double MinMass { get; set; }
//        public double MaxMass { get; set; }
//        public double Delta { get; set; }
//        #endregion

//        #region INormalizationOptions members
//        public bool PerformNormalization { get; set; }
//        #endregion

//        #region ISpectrumAveragingOptions members
//        public RejectionType RejectionType { get; set; }
//        public WeightingType WeightingType { get; set; }
//        public SpectrumMergingType SpectrumMergingType { get; set; }
//        public double Percentile { get; set; }
//        public double MinSigmaValue { get; set; }
//        public double MaxSigmaValue { get; set; }
//        public double BinSize { get; set; }
//        #endregion

//        #region IDataDependentScanOptions
//        public double IsolationWidth { get; set; }
//        public double Resolution { get; set; }
//        public double MaxIT { get; set; }
//        public double CollisionEnergy { get; set; }
//        public double AGCTarget { get; set; }

//        public double PrecursorMass { get; set; }
//        public string ScanType { get; set; } = "MSn";
//        #endregion
//    }

//}
