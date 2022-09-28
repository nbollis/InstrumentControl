using ClientServerCommLibrary;
using MassSpectrometry;
using MzLibUtil;
using SpectralAveraging;

namespace WorkflowServer
{
    /// <summary>
    /// Class which takes input of n scans and averages them based upon the parameters set
    /// </summary>
    //public class SpectralAveragingTask : InstrumentControlTask
    //{
    //    public MzLibSpectralAveragingOptions Options { get; private set; }
    //    public override int ScansOutputted => ScansToAccept / Options.NumScansToAverage;

    //    #region Constructor

    //    public SpectralAveragingTask(int acceptScanOrder, int scansToAccept, MzLibSpectralAveragingOptions options) : base(acceptScanOrder, scansToAccept)
    //    {
    //        Options = options;
    //    }

    //    #endregion


    //    protected override IEnumerable<SingleScanDataObject> ExecuteSpecific(IEnumerable<SingleScanDataObject> scansToProcess)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}


}
