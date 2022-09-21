using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommLibrary
{
    public delegate void ProcessMs1ScansDelegate(object obj,
        ScanQueueThresholdReachedEventArgs scans);

    public delegate void ProcessMs2ScansDelegate(object obj,
        ScanQueueThresholdReachedEventArgs scans);
}
