using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;

namespace WorkflowServer
{
    public class DataDependentScanTask : InstrumentControlTask
    {
        public int TopNPeaks { get; set; }
        public override int ScansOutputted => ScansToAccept * TopNPeaks;

        public DataDependentScanTask(int acceptScanOrder, int scansToAccept, int topNPeaks) : base(acceptScanOrder, scansToAccept)
        {
            TopNPeaks = topNPeaks;
        }

        protected override IEnumerable<SingleScanDataObject> ExecuteSpecific(IEnumerable<SingleScanDataObject> scansToProcess)
        {
            List<SingleScanDataObject> processedScans = new();
            foreach (var scan in scansToProcess)
            {
                var topNIntvalues = scan.YArray.OrderByDescending(p => p).Take(TopNPeaks).ToArray();
                double[] mzsToIsolate = new double[topNIntvalues.Count()];
                for (int i = 0; i < topNIntvalues.Count(); i++)
                {
                    mzsToIsolate[i] = scan.XArray[Array.IndexOf(scan.YArray, topNIntvalues[i])];
                }

                // TODO: Create SingleScanDataObjects from the mzs to Isolate and add to processedScans to be returned
            }

            return processedScans;
        }
    }
}
