using MassSpectrometry;
using MzLibUtil;
using Averaging;
using TaskInterfaces;
using Data; 


namespace InstrumentControl
{
    /// <summary>
    /// Class which takes input of n scans and averages them based upon the parameters set
    /// </summary>
    public class SpectrumAveragingTask : InstrumentControlTask
    {
        #region Constructor

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

            SpectrumAveraging.CombineSpectra(data as MultiScanDataObject, options as SpectrumAveragingOptions);
        }
        // No return type yet. 
        private void Main(MultiScanDataObject scans, SpectrumAveragingOptions options)
        {

        }
    }


}
