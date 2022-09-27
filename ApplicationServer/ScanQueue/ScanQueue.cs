using ClientServerCommLibrary;

namespace WorkflowServer
{
    public class ScanQueue
    {
        public Queue<SingleScanDataObject> DataToProcess { get; set; }
        int Threshold { get; set; }

        public event EventHandler<ThresholdReachedEventArgs>? ThresholdReached;

        public ScanQueue(int processingThreshold)
        {
            Threshold = processingThreshold;
            DataToProcess = new Queue<SingleScanDataObject>(100);
        }

        protected virtual void OnThresholdReached(ThresholdReachedEventArgs e)
        {
            // make .NET Framework 4.8 compatible just in case this ever gets ported to 
            // client in any manner. 
            EventHandler<ThresholdReachedEventArgs> handler = ThresholdReached;
            if (handler != null)
            {
                handler(this, e); 
            }
        }
    }
}
