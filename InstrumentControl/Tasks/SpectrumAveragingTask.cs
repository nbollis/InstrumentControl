using MassSpectrometry;
using MzLibUtil;
using InstrumentControl.Interfaces; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{

    // Workflow: Take in set of scans --> MAYBE: Normalize intensity to TotalIonCurrent --> isolate an array of intensities for each mass --> reject outliers --> calculate weights of each point ---> calculate a weighted average
    // TODO deal with scans when they enter e.g. convert to an array of intensities for each m/z value
    // TODO consider whether normaliztion is neededs

    /// <summary>
    /// Class which takes input of n scans and averages them based upon the parameters set
    /// </summary>
    public class SpectrumAveragingTask : ISpectraAverager, ITask
    {
        #region Public Properties

        // settings
        public static RejectionType RejectionType { get; set; } = RejectionType.NoRejection;
        public static WeightingType WeightingType { get; set; } = WeightingType.NoWeight;
        public static SpectrumMergingType SpectrumMergingType { get; set; } = SpectrumMergingType.SpectrumBinning;
        public static double Percentile { get; set; } = 0.9;
        public static double MinSigmaValue { get; set; } = 1.3;
        public static double MaxSigmaValue { get; set; } = 1.3;
        public static double BinSize { get; set; } = 0.02;

        #endregion

        #region Constructor

        public SpectrumAveragingTask(TaskType taskType) : base(taskType)
        {

        }

        #endregion
        /// <summary>
        /// Performs the main operations of this task
        /// </summary>
        /// <returns></returns>
        public override void RunSpecific()
        {
                      
        }

    }


}
