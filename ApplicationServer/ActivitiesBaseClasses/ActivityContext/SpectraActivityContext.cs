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
    /// </summary>
    public class SpectraActivityContext : ActivityContext
    {
        public ScanInstructions ScanInstructions { get; set; }
        public SingleScanDataObject FirstSingleScanDataObject => SingleScanDataObjects.First();
        public List<SingleScanDataObject> SingleScanDataObjects { get; set; }

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

        public ScanQueueManager ScanQueueManager { get; set; }


        public SpectraActivityContext(SingleScanDataObject ssdo, MassTargetList massTargetList)
        {
            SingleScanDataObjects = new();
            SingleScanDataObjects.Add(ssdo);
            ScanInstructions = ssdo.ScanInstructions;
            MassTargetList = massTargetList;
            MassesToTarget = new();
            ScanQueueManager = ScanQueueManager.Instance;
        }

    }
}
