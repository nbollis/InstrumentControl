using ClientServerCommLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MzLibUtil;

namespace WorkflowServer
{
    public class TargetedAcquisitionTask : InstrumentControlTask
    {
        public List<double> MassesToTarget { get; private set; }
        public Tolerance Tolerance { get; private set; }
        public double RelativeIntensityCutoff { get; private set; }
        public override int ScansOutputted => AcceptScanOrder;

        public TargetedAcquisitionTask(int acceptedScanOrder, int scansToAccept, List<double> massesToTarget,
            Tolerance tolerance, double relativeIntensityCutoff) : base(acceptedScanOrder, scansToAccept)
        {
            MassesToTarget = massesToTarget;
            Tolerance = tolerance;
            RelativeIntensityCutoff = relativeIntensityCutoff;
        }


        // TODO: Idea: have the NextTask be a DDA task and figure out the details in a bit

        protected override IEnumerable<SingleScanDataObject> ExecuteSpecific(IEnumerable<SingleScanDataObject> scansToProcess)
        {
            List<SingleScanDataObject> processedScans = new();
            foreach (var scan in scansToProcess)
            {
                if (scan.XArray.Any(p => MassesToTarget.Any(m => Tolerance.Within(p, m))))
                {
                    var temp = scan.XArray.Where(p => MassesToTarget.Any(m => Tolerance.Within(p, m)));
                    var maxInt = scan.YArray.Max();
                    foreach (var peak in temp)
                    {
                        int index = Array.IndexOf(scan.XArray, peak);
                        if (scan.YArray[index] / maxInt > RelativeIntensityCutoff)
                        {
                            processedScans.Add(scan);
                            // TODO: Add the peak to the masses to target section of scan instructions
                        }
                    }
                }
            }

            return processedScans;
        }
    }
}
