using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;


namespace WorkflowServer
{
    public class AcceptScansActivity<TContext> : IActivity<TContext>
        where TContext : IActivityContext
    {
        private readonly int msNOrder;
        private readonly int scansToDequeue;

        private AutoResetEvent readyToStop;

        public AcceptScansActivity(int msNOrder, int scansToDequeue)
        {
            this.msNOrder = msNOrder;
            this.scansToDequeue = scansToDequeue;
            readyToStop = new AutoResetEvent(false);
        }

        public async Task ExecuteAsync(TContext context)
        {
            SpectraActivityContext specContext =
                context as SpectraActivityContext ?? throw new NullReferenceException();

            // check if queue has enough to process
            Task task = new Task(() => WaitForScans(specContext));

            // spin this up in new background thread
            Thread thread = new Thread(task.Start);
            thread.IsBackground = true;
            thread.Start();

            // wait until there is enough to dequeue
            await task;
        }

        /// <summary>
        /// Task that will check if the scan queue has enough of the target
        /// type to dequeue, then add them to the processing queue
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Task WaitForScans(SpectraActivityContext context)
        {

            while (!ScanQueueManager.CheckQueue(msNOrder, scansToDequeue))
            {
                // 30kD scan on fusion lumos orbitrap scans at a rate of 15 Hz
                // this translates to 66ms, hence the 25 ms timeout
                // this number may need to be dynamic based upon the type of instrument attached
                // but this would violate our encapsulation of server/client
                Thread.Sleep(25);
            }

            ScanQueueManager.TryDequeueMany(msNOrder, scansToDequeue,
                out IEnumerable<SingleScanDataObject> singleScanDataObjects);

            context.DataToProcess.AddRange(singleScanDataObjects);

            return Task.CompletedTask;
        }
    }
}



