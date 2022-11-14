using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;

namespace WorkflowServer
{
    public interface IScanSender
    {
        public ScanInstructions ScanInstructions { get; set; }
        public event EventHandler<ProcessingCompletedEventArgs> SendScan;
    }
}
