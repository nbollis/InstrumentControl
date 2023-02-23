using ClientServerCommLibrary;
using Easy.Common.Extensions;
using WorkflowServer.Activities;

namespace WorkflowServer.Activities
{
    public class CaptureMs1Activity<TContext> : IActivity<TContext>, IMs1ScanReceiver
        where TContext : IActivityContext
    {
        public int MsNOrder { get; }
        public ScanInstructions BaseScanInstructions { get; }

        private readonly int scansToDequeue;

        public event EventHandler<ProcessingCompletedEventArgs> SendScan;

        public CaptureMs1Activity(int msNOrder, int toDequeue, ScanInstructions baseMs1Instructions)
        {
            MsNOrder = msNOrder;
            scansToDequeue = toDequeue;
            BaseScanInstructions = baseMs1Instructions;
        }

        public async Task ExecuteAsync(TContext context)
        {
            SpectraActivityContext specContext =
                context as SpectraActivityContext ?? throw new NullReferenceException();

            // send instructions to capture a ms1
            ScanQueueManager.InstructionQueue.Enqueue(BaseScanInstructions);

            IEnumerable<SingleScanDataObject> singleScanDataObjects;
            while (!ScanQueueManager.TryDequeueMany(MsNOrder, scansToDequeue,
                out singleScanDataObjects))
            {
                // 30kD scan on fusion lumos orbitrap scans at a rate of 15 Hz
                // this translates to 66ms, hence the 25 ms timeout
                // this number may need to be dynamic based upon the type of instrument attached
                // but this would violate our encapsulation of server/client
            }

            singleScanDataObjects.ForEach(p => specContext.DataToProcess.Enqueue(p));
        }
    }
}



