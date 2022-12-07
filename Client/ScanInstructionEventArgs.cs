using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;

namespace InstrumentClient
{
    public class ScanInstructionsEventArgs : EventArgs
    {
        public ScanInstructions ScanInstructions { get; set; }
        public ScanInstructionsEventArgs(ScanInstructions scanInstructions)
        {
            ScanInstructions = scanInstructions;
        }
    }
}
