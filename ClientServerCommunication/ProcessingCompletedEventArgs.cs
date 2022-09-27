using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommLibrary
{
    public class ProcessingCompletedEventArgs : EventArgs
    {
        public IEnumerable<SingleScanDataObject> ssdo { get; set; }

        public ProcessingCompletedEventArgs(IEnumerable<SingleScanDataObject> ssdo)
        {
            this.ssdo = ssdo;
        }
    }
}
