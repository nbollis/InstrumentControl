using ClientServerCommLibrary;

namespace WorkflowServer
{
    public class ScanQueue
    {
        public Queue<SingleScanDataObject> DataToProcess { get; set; }
        int Threshold { get; set; }

        public event EventHandler<ScanQueueThresholdReachedEventArgs>? ThresholdReached;

        public ScanQueue(int processingThreshold)
        {
            Threshold = processingThreshold;
            DataToProcess = new Queue<SingleScanDataObject>(100);
        }

        public void Enqueue(SingleScanDataObject ssdo)
        {
            DataToProcess.Enqueue(ssdo);
            if (DataToProcess.Count >= Threshold)
            {
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
            ThresholdReached?.Invoke(this, new ScanQueueThresholdReachedEventArgs(DataToProcess.DequeueChunk(Threshold)));
        }
    }
}
