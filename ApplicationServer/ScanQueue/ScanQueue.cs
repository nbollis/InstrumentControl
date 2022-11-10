using System.Collections.Concurrent;
using ClientServerCommLibrary;

namespace WorkflowServer
{
    public class ScanQueue
    {
        public ConcurrentQueue<SingleScanDataObject> Queue { get; set; }

        public int Count => Queue.Count;

        public ScanQueue()
        {
            Queue = new ConcurrentQueue<SingleScanDataObject>();
        }

        /// <summary>
        /// En queues a single SingleScanDataObject
        /// </summary>
        /// <param name="singleScanDataObject"></param>
        public void Enqueue(SingleScanDataObject singleScanDataObject)
        {
            Queue.Enqueue(singleScanDataObject);
        }

        /// <summary>
        /// En queue multiple single scan data objects
        /// </summary>
        /// <param name="singleScanDataObjects"></param>
        public void EnqueueMany(IEnumerable<SingleScanDataObject> singleScanDataObjects)
        {
            foreach (var singleScanDataObject in singleScanDataObjects)
            {
                Enqueue(singleScanDataObject);
            }
        }

        public bool TryDequeue(out SingleScanDataObject singleScanData)
        {
            return Queue.TryDequeue(out singleScanData);
        }

        public IEnumerable<SingleScanDataObject> DequeueMany(int toDequeue)
        {
            for (int i = 0; i < toDequeue; i++)
            {
                if (TryDequeue(out SingleScanDataObject result))
                {
                    yield return result;
                }
            }
        }
    }
}
