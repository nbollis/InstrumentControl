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
    public class ScanQueueManager
    {
        #region Singleton Initialization

        private static readonly Lazy<ScanQueueManager> lazy = new Lazy<ScanQueueManager>(() => new ScanQueueManager());

        public static ScanQueueManager Instance => lazy.Value;

        private ScanQueueManager()
        {
            ScanQueues = new Dictionary<int, ScanQueue>();
        }

        #endregion

        #region Public Properties

        public Dictionary<int, ScanQueue> ScanQueues { get; set; }

        #endregion


        public void BuildQueues(List<int> msNOrder, List<int> scansToAccept)
        {
            for (var i = 0; i < msNOrder.Count; i++)
            {
                ScanQueues.TryAdd(msNOrder[1], new ScanQueue(scansToAccept[i]));
            }
        }

        public void EnqueueScan(SingleScanDataObject ssdo)
        {
            if(ScanQueues.TryGetValue(ssdo.MsNOrder, out ScanQueue queue))
            {
                queue.Enqueue(ssdo);
            }
            else
            {
                ScanQueue newQueue = new ScanQueue(5);
                ScanQueues.TryAdd(ssdo.MsNOrder, newQueue);
            }
        }



 



    }
}
