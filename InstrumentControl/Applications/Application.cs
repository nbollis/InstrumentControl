using System;
using System.Collections.Generic;
using TaskInterfaces;

namespace InstrumentControl
{
    public abstract class Application : IApplication
    {   
        public Application()
        {
            
        }
        public abstract List<InstrumentControlTask> TaskList { get; set; }
        public abstract void ProcessScans();

    }
    public class ScanAveragingApp : Application
    {
        public override List<InstrumentControlTask> TaskList { get; set; }
        public override void ProcessScans()
        {
            // needs: 
            // normalization
            // standardization
            // averaging 
            throw new NotImplementedException();
        }

    }
    public class ScanAveragingAppOptions : IApplicationOptions, 
    {
        public bool Live { get; set; }

    }
}
