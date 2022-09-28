using ClientServerCommLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WorkflowServer
{
    /// <summary>
    /// Normalizes the intensities of each peak to the total ion current of the scan
    /// All peak intensities should sum to one
    /// </summary>
    public class NormalizationTask : InstrumentControlTask
    {
        public override int ScansOutputted => ScansToAccept;

        public NormalizationTask(int acceptScanOrder, int scansToAccept) : base(acceptScanOrder, scansToAccept)
        {

        }

        protected override IEnumerable<SingleScanDataObject> ExecuteSpecific(IEnumerable<SingleScanDataObject> scansToProcess)
        {
            //SpectrumNormalization.NormalizeSpectrumToTic(data as SingleScanDataObject);
            throw new NotImplementedException();
        }
    }
}
