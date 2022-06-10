using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;

namespace InstrumentControl
{
    public abstract class Application
    {
        protected MyApplication ApplicationType { get; set; }
        
        public Application(MyApplication applicationType)
        {
            ApplicationType = applicationType;
        }

        public void EnqueueSelectTasks()
        {
            foreach (var task in TaskList)
            {
                TaskQueue.Enqueue(task);
            }
        }

        // to be executed with each iteration
        public abstract Queue<InstrumentControlTask> TaskQueue { get; set; }
        public abstract List<InstrumentControlTask> TaskList { get; set; }
        public abstract void ProcessScans(object? sender, ThresholdReachedEventArgs e);

    }
}
