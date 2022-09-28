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
    public class Workflow
    {
        private AppServerPipe ConnectedPipe { get; }
        public event EventHandler<ProcessingCompletedEventArgs> ProcessingCompleted;
        public Dictionary<int, ScanQueue> ScanQueues { get; set; }
        public List<InstrumentControlTask> Tasks { get; }

        public Workflow(AppServerPipe pipe, List<InstrumentControlTask> tasks)
        {
            ConnectedPipe = pipe;
            Tasks = tasks;
            ScanQueues = new();
            CheckTaskListValidity(Tasks);

            ProcessingCompleted += ConnectedPipe.SendDataThroughPipe;

            foreach (var task in Tasks)
            {
                ScanQueues.TryAdd(task.AcceptScanOrder, new ScanQueue(task.ScansToAccept));
                ScanQueues[task.AcceptScanOrder].ThresholdReached += (sender, args) =>
                {
                    ProcessingCompleted?.Invoke(this, new ProcessingCompletedEventArgs(task.ExecuteTask(args.ListSsdo)));
                };
            }
        }

        /// <summary>
        /// Inputs the received SingleScanDataObject into the proper Queue
        /// </summary>
        /// <param name="ssdo"></param>
        /// <exception cref="Exception"></exception>
        public void ReceiveData(SingleScanDataObject ssdo)
        {
            try
            {
                ScanQueues[ssdo.ScanOrder].Enqueue(ssdo);
            }
            catch (Exception e)
            {
                throw new Exception($"Error queueing to dictionary of queue with MSnOrder: {ssdo.ScanOrder}" + e.Message);
            }
        }

        /// <summary>
        /// Ensures the tasks can be operated in the order received
        /// </summary>
        /// <param name="tasks"></param>
        /// <exception cref="ArgumentException"></exception>
        private static void CheckTaskListValidity(List<InstrumentControlTask> tasks)
        {
            int msnOrder = tasks.First().AcceptScanOrder;
            if (msnOrder != 1)
                throw new ArgumentException("First task must accept Ms1 Scans");

            for (int i = 1; i < tasks.Count; i++)
            {
                if (tasks[i].AcceptScanOrder <= msnOrder)
                    throw new ArgumentException("All tasks must have sequential MsnOrders");
                msnOrder = tasks[i].AcceptScanOrder;
            }
        }
    }
}
