using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;

namespace WorkflowServer
{
    /// <summary>
    /// Idea is that it gets passed a list of tasks and creates the desired workflow from there
    /// </summary>
    public class WorkflowFactory
    {
        private AppServerPipe ConnectedPipe { get; }
        public event EventHandler<ScanQueueThresholdReachedEventArgs> Ms1ScanQueueThresholdReached;
        public event EventHandler<ScanQueueThresholdReachedEventArgs> Ms2ScanQueueThresholdReached;


        private List<InstrumentControlTask> Tasks { get; }


        public WorkflowFactory(AppServerPipe pipe, List<InstrumentControlTask> tasks)
        {
            ConnectedPipe = pipe;
            Tasks = tasks;
        }

        public void ExecuteWorkflow()
        {

        }
    }
}
