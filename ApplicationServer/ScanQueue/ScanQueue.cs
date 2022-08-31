using Data; 

namespace ApplicationServer
{
    public class ScanQueue
    {
        public Queue<SingleScanDataObject> DataToProcess { get; set; }
        int Threshold { get; set; }
        public bool ExportToJson { get; set; } = false;
        public event EventHandler<ThresholdReachedEventArgs>? ThresholdReached;

        public ScanQueue(int processingThreshold)
        {
            Threshold = processingThreshold;
            DataToProcess = new Queue<SingleScanDataObject>(100);
        }

        protected virtual void OnThresholdReached(ThresholdReachedEventArgs e)
        {
            ThresholdReached?.Invoke(this, e);
        }
    }
}
