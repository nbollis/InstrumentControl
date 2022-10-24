using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommLibrary
{
    /// <summary>
    /// Class used to pass scans when the scan queue threshold is triggered. 
    /// </summary>
    public class ScanQueueThresholdReachedEventArgs : EventArgs
    {
        public IEnumerable<SingleScanDataObject> ListSsdo { get; set; }

        public ScanQueueThresholdReachedEventArgs(IEnumerable<SingleScanDataObject> listSsdo)
        {
            ListSsdo = listSsdo;
        }
    }
}
