using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;
using WorkflowServer.Activities;
using WorkflowServer.Util;

namespace WorkflowServer
{
    /// <summary>
    /// Context that is passed between activities
    /// Acts as a mediator object, abstracting away some of the underlying processing in activities
    /// </summary>
    public class SpectraActivityContext : ActivityContext
    {
        public SingleScanDataObject FirstSingleScanDataObject => DataToProcess.First();
        public Queue<SingleScanDataObject> DataToProcess { get; set; }

        /// <summary>
        /// Contains both inclusion and exclusion lists
        /// </summary>
        public MassTargetList MassTargetList { get; set; }

        /// <summary>
        /// Masses to Target for the ScanSendingActivities
        /// Each array will be a scan to send
        /// DDA will only have one element per array
        /// boxcar will have several elements per array
        /// </summary>
        public Queue<double[]> MassesToTarget { get; set; }


        //TODO: Remove all references
        [Obsolete]
        public SpectraActivityContext(SingleScanDataObject ssdo, MassTargetList massTargetList)
        {
            DataToProcess = new();
            DataToProcess.Enqueue(ssdo);
            MassTargetList = massTargetList;
            MassesToTarget = new();
        }

        public SpectraActivityContext()
        {
            DataToProcess = new();
            MassTargetList = new();
            MassesToTarget = new();
        }

         

    }

}
