using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace ClientServer
{
    public class ScanInstructionsEventArgs : EventArgs
    {
        public Data.ScanInstructions ScanInstructions { get; set; }
        public ScanInstructionsEventArgs(ScanInstructions scanInstructions)
        {
            ScanInstructions = scanInstructions;
        }
    }
}
