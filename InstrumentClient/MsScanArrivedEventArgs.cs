using ClientServerCommLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentClient
{
    public class MsScanArrivedEventArgs : EventArgs
    {
        public SingleScanDataObject Ssdo { get; set; }
        public MsScanArrivedEventArgs(SingleScanDataObject ssdo)
        {
            Ssdo = ssdo;
        }
    }
}
