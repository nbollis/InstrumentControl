using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;

namespace WorkflowServer
{
    /// <summary>
    /// Singleton Class which manages the scan queues for the server
    /// </summary>
    public static class ScanQueueManager
    {
        static ScanQueueManager()
        {
            ScanQueues = new Dictionary<int, ScanQueue>();
        }

        #region Public Properties

        public static Dictionary<int, ScanQueue> ScanQueues { get; set; }

        #endregion

        /// <summary>
        /// Builds a queue for each msnScanOrder 
        /// </summary>
        /// <param name="msNOrders">Each MsnOrder expected in this workflow</param>
        public static void BuildQueues(IEnumerable<int> msNOrders)
        {
            var nOrders = msNOrders as int[] ?? msNOrders.ToArray();
            for (var i = 0; i < nOrders.Count(); i++)
            {
                BuildQueue(nOrders[i]);
            }
        }

        public static void BuildQueue(int msnOrder)
        {
            ScanQueues.TryAdd(msnOrder, new ScanQueue());
        }

        /// <summary>
        /// Adds scan to the respective queue based upon its MsnOrder
        /// </summary>
        /// <param name="ssdo">scan to be en queued</param>
        public static void EnqueueScan(SingleScanDataObject ssdo)
        {
            if(ScanQueues.TryGetValue(ssdo.MsNOrder, out ScanQueue queue))
            {
                queue.Enqueue(ssdo);
            }
            else
            {
                ScanQueue newQueue = new ScanQueue();
                ScanQueues.TryAdd(ssdo.MsNOrder, newQueue);
            }
        }

        /// <summary>
        /// Dequeue many scans from the queue
        /// </summary>
        /// <param name="msnOrder">Order of scans to dequeue</param>
        /// <param name="scansToDequeue">Number of scans to dequeue</param>
        /// <param name="singleScanDataObjects">Dequeued scans</param>
        /// <returns>true if there is enough scans to dequeue, false otherwise</returns>
        public static bool TryDequeueMany(int msnOrder, int scansToDequeue,
            out IEnumerable<SingleScanDataObject> singleScanDataObjects)
        {
            singleScanDataObjects = new List<SingleScanDataObject>();
            var queue = ScanQueues[msnOrder];
            if (queue.Count < scansToDequeue)
            {
                return false;
            }

            // if enough to dequeue
            singleScanDataObjects = queue.DequeueMany(scansToDequeue);
            return true;
        }

        /// <summary>
        /// Checks to see if the specific queue has enough objects to warrant a dequeue
        /// </summary>
        /// <param name="msnOrder"></param>
        /// <param name="scansToDequeue"></param>
        /// <returns></returns>
        public static bool CheckQueue(int msnOrder, int scansToDequeue)
        {
            var queue = ScanQueues[msnOrder];
            if (queue.Count < scansToDequeue)
                return false;
            else
                return true;
        }
    }
}
