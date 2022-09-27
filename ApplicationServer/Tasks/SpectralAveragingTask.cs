//using ClientServerCommLibrary;
//using MassSpectrometry;
//using MzLibUtil;
//using SpectralAveraging;

//namespace WorkflowServer
//{
//    /// <summary>
//    /// Class which takes input of n scans and averages them based upon the parameters set
//    /// </summary>
//    public class SpectralAveragingTask : InstrumentControlTask
//    {
//        public SpectralAveragingOptions Options { get; private set; }

//        #region Constructor

//        public SpectralAveragingTask(SpectralAveragingOptions options)
//        {
//            Options = options;
//        }

//        #endregion

//        public override void RunSpecific<T, U>(T options, U data)
//        {
//            if(options == null || data == null)
//            {
//                throw new ArgumentException("Data or options passed to method is null."); 
//            }
//            if(typeof(T) != typeof(SpectralAveragingOptions))
//            {
//                throw new ArgumentException("Invalid options class for this class and method."); 
//            }
  

//            //SpectrumAveraging.CombineSpectra(data as MultiScanDataObject, options as SpectralAveragingOptions);
//        }

//        public T RunTask<T>(T scansToAverage)
//        {
//            // TODO: link this bad boi up once spectral averaging mzlib is fixed
//            return scansToAverage;
//        }

//        protected override IEnumerable<SingleScanDataObject> ExecuteSpecific(IEnumerable<SingleScanDataObject> scansToProcess)
//        {
//            throw new NotImplementedException();
//        }
//    }


//}
