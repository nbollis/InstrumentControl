using MassSpectrometry;
using MzLibUtil;
using InstrumentControl.Interfaces; 
using SpectrumAveraging;
using TaskInterfaces; 


namespace InstrumentControl
{

    // Workflow: Take in set of scans --> MAYBE: Normalize intensity to TotalIonCurrent --> isolate an array of intensities for each mass --> reject outliers --> calculate weights of each point ---> calculate a weighted average
    // TODO deal with scans when they enter e.g. convert to an array of intensities for each m/z value
    // TODO consider whether normaliztion is neededs

    /// <summary>
    /// Class which takes input of n scans and averages them based upon the parameters set
    /// </summary>
    public class SpectrumAveragingTask : InstrumentControlTask
    {
        #region Public Properties
        public SpectrumAveragingOptions AveragingOptions { get; set; }
        // add property to include the data to be processed. 

        #endregion

        #region Constructor

        public SpectrumAveragingTask(SpectrumAveragingOptions options)
        {
            AveragingOptions = options;
        }
        #endregion

        #region Public Methods

        public override void RunSpecific<T>(T options)
        {
            if(typeof(T) != typeof(SpectrumAveragingOptions))
            {
                throw new ArgumentException("Invalid options class for this class and method."); 
            }
            // implement the specific running here
        }

        #endregion
    }


}
