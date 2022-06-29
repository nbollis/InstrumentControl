using MassSpectrometry;
using MzLibUtil;
using SpectrumAveraging;
using TaskInterfaces;
using Data; 


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
        public SpectrumAveragingOptions? AveragingOptions { get; set; }
        // add property to include the data to be processed. 

        #endregion

        #region Constructor

        public SpectrumAveragingTask(SpectrumAveragingOptions options)
        {
            AveragingOptions = options;
        }
        public SpectrumAveragingTask()
        {

        }
        #endregion

        public override void RunSpecific<T, U>(T options, U data)
        {
            if(options == null || data == null)
            {
                throw new ArgumentException("Data or options passed to method is null."); 
            }
            if(typeof(T) != typeof(SpectrumAveragingOptions))
            {
                throw new ArgumentException("Invalid options class for this class and method."); 
            }
            if(typeof(U) != typeof(MultiScanDataObject))
            {
                throw new ArgumentException("Invalid data class for this class and method."); 
            }
            // implement the specific running here
            SpectrumAveragingOptions opt = options as SpectrumAveragingOptions; 
            MultiScanDataObject scans = data as MultiScanDataObject;

            

        }
        // No return type yet. 
        private void Main(MultiScanDataObject scans, SpectrumAveragingOptions options)
        {

        }
    }


}
