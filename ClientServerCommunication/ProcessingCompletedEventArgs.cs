using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommLibrary
{
    public class ProcessingCompletedEventArgs : EventArgs
    {
        public ScanInstructions ScanInstructions { get; private set; }

        public ProcessingCompletedEventArgs(ScanInstructions scanInstructions)
        {
            ScanInstructions = scanInstructions;
        }
    }
}
