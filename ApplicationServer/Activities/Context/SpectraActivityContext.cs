using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;
using ClientServerCommunication;
using WorkflowServer.Util;

namespace WorkflowServer
{
    /// <summary>
    /// Context that is passed between activities
    /// Acts as a mediator object, abstracting away some of the underlying processing in activities
    /// </summary>
    public class SpectraActivityContext : ActivityContext
    {
        public ScanInstructions ScanInstructions { get; set; }
        public SingleScanDataObject FirstSingleScanDataObject => DataToProcess.First();
        public List<SingleScanDataObject> DataToProcess { get; set; }

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

        //TODO: Change constructor to take the IActivityCollection<IActivityContext> as the only input parameter
        //TODO: Make this obsolete
        public SpectraActivityContext(SingleScanDataObject ssdo, MassTargetList massTargetList)
        {
            DataToProcess = new();
            DataToProcess.Add(ssdo);
            ScanInstructions = ssdo.ScanInstructions;
            MassTargetList = massTargetList;
            MassesToTarget = new();
        }

        public SpectraActivityContext()
        {
            DataToProcess = new();
            ScanInstructions = new();
            MassTargetList = new();
            MassesToTarget = new();
        }

    }

}
