using System.Collections.Concurrent;
using ClientServerCommLibrary;

namespace WorkflowServer
{
    public class ScanQueue
    {
        public ConcurrentQueue<SingleScanDataObject> DataToProcess { get; set; }
        int Threshold { get; set; }
        public bool ThresholdReacheda { get; set; } = false;

        public event EventHandler<ScanQueueThresholdReachedEventArgs>? ThresholdReached;

        public ScanQueue(int processingThreshold)
        {
            Threshold = processingThreshold;
            DataToProcess = new ConcurrentQueue<SingleScanDataObject>();
        }

        public void Enqueue(SingleScanDataObject ssdo)
        {
            DataToProcess.Enqueue(ssdo);
            if (DataToProcess.Count >= Threshold)
            {
                ThresholdReacheda = true;
                OnThresholdReached();
            }
        }

        public void Enqueue(IEnumerable<SingleScanDataObject> ssdos)
        {
            foreach (var singleScanDataObject in ssdos)
            {
                Enqueue(singleScanDataObject);
            }
        }

        private void OnThresholdReached()
        {
            List<SingleScanDataObject> ssdoList = new List<SingleScanDataObject>();
            for (int i = 0; i < Threshold; i++)
            {
                if (DataToProcess.TryDequeue(out SingleScanDataObject? result))
                {
                    ssdoList.Add(result);
                }
            }
            ThresholdReached?.Invoke(this, new ScanQueueThresholdReachedEventArgs(ssdoList));
        }
    }
}
