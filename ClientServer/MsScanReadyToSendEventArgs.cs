using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using System.Runtime; 

namespace Client
{
    public class MsScanReadyToSendEventArgs : EventArgs
    {
        public SingleScanDataObject ScanData { get; set; }

        public MsScanReadyToSendEventArgs(SingleScanDataObject scan)
        {
            ScanData = scan;
        }
    }
}
