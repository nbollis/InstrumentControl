using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommLibrary
{
    /// <summary>
    /// Used to pass a single scan data object from the workflow to the
    /// method used to communicate with the client. 
    /// </summary>
    public class ProcessingCompletedEventArgs : EventArgs
    {
        public SingleScanDataObject Ssdo { get; set; }
    }
}
