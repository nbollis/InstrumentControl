using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ClientServerCommLibrary;


namespace WorkflowServer
{
    public abstract class InstrumentControlTask
    {
        public int AcceptScanOrder { get; }
        public int ScansToAccept { get; }
        public virtual int ScansOutputted => ScansToAccept;
        public bool Terminate { get; private set; } = false;
        public InstrumentControlTask? NextTask { get; private set; }


        public InstrumentControlTask(int acceptScanOrder, int scansToAccept)
        {
            AcceptScanOrder = acceptScanOrder;
            ScansToAccept = scansToAccept;
        }


        protected abstract IEnumerable<SingleScanDataObject> ExecuteSpecific(IEnumerable<SingleScanDataObject> scansToProcess);
        public IEnumerable<SingleScanDataObject> ExecuteTask(IEnumerable<SingleScanDataObject> scansToProcess)
        {
            List<SingleScanDataObject> processedScans = this.ExecuteSpecific(scansToProcess).ToList();

            // if next task is meant to accept the output of the previous task
            if (NextTask is { AcceptScanOrder: -1 } && !Terminate)
            {
                Queue<SingleScanDataObject> processingQueue = new Queue<SingleScanDataObject>(processedScans);
                int scansToSend = ScansOutputted / NextTask.ScansToAccept;
                int totalScans = processedScans.Count;
                processedScans.Clear();
                for (int i = 0; i < totalScans; i += scansToSend)
                {
                    processedScans.AddRange(NextTask.ExecuteTask(processingQueue.DequeueChunk(scansToSend)));
                }
            }
            return processedScans;
        }

        public void AddNextTask(InstrumentControlTask task)
        {
            NextTask = task;
        }
    }
}
