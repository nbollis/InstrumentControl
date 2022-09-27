using System;
using System.Collections.Generic;
using ClientServerCommLibrary;
using SpectralAveraging;

namespace WorkflowServer
{
    public abstract class Workflow
    {   
        public Workflow()
        {
            
        }
        public Workflows WorkflowType { get; set; }
        public IWorkflowOptions WorkflowOptions { get; set; }
        public ProcessMs1ScansDelegate? Ms1ScanDelegate;
        public ProcessMs2ScansDelegate? Ms2ScanDelegate;

        public abstract void ProcessScans(object sender, ThresholdReachedEventArgs e);
        

    }
}
