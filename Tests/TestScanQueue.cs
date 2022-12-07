using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;
using WorkflowServer;

namespace Tests
{
    [ExcludeFromCodeCoverage]
    public static class TestScanQueue
    {
        public static List<SingleScanDataObject> SsdoList;

        [OneTimeSetUp]
        public static void OneTimeSetup()
        {
            double[] xArray = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            double[] y1 = new double[] { 0, 100, 10, 10, 10, 20, 20, 10, 10, 0 };
            double[] y2 = new double[] { 0, 10, 100, 10, 10, 20, 20, 10, 10, 0 };
            double[] y3 = new double[] { 0, 10, 10, 100, 10, 20, 20, 10, 10, 0 };
            double[] y4 = new double[] { 0, 10, 10, 10, 100, 20, 20, 10, 10, 0 };

            SsdoList = new();
            SsdoList.Add(new SingleScanDataObject(xArray, y1) { MsNOrder = 1, RetentionTime = 2000 });
            SsdoList.Add(new SingleScanDataObject(xArray, y2) { MsNOrder = 1, RetentionTime = 3000 });
            SsdoList.Add(new SingleScanDataObject(xArray, y3) { MsNOrder = 1, RetentionTime = 4000 });
            SsdoList.Add(new SingleScanDataObject(xArray, y4) { MsNOrder = 1, RetentionTime = 5000 });
        }

        [Test]
        public static void TestScanQueues()
        {
            // test basic properties
            ScanQueue queue = new ScanQueue();
            Assert.That(queue.Queue != null);
            Assert.That(queue.Count, Is.EqualTo(queue.Queue.Count));
            Assert.That(queue.Count, Is.EqualTo(0));

            // test enqueue methods
            queue.Enqueue(SsdoList.First());
            Assert.That(queue.Count, Is.EqualTo(1));
            queue.EnqueueMany(SsdoList);
            Assert.That(queue.Count, Is.EqualTo(5));

            // test dequeue methods
            Assert.That(queue.TryDequeue(out SingleScanDataObject ssdo));
            Assert.That(ssdo.Equals(SsdoList.First()));
            Assert.That(queue.Count == 4);
            var ssdos = queue.DequeueMany(4).ToList();
            Assert.That(ssdos.Count() == 4);
            for (int i = 0; i < ssdos.Count(); i++)
            {
                Assert.That(ssdos[i].Equals(SsdoList[i]));
            }
            Assert.That(queue.Count == 0);
            Assert.That(queue.TryDequeue(out SingleScanDataObject ssdo2), Is.False);
        }

        [Test]
        public static void TestScanQueueManager()
        {
            Assert.That(ScanQueueManager.ScanQueues != null);

            var msnOrders = new int[] { 1, 2, 3 };
            ScanQueueManager.BuildQueues(msnOrders);
            Assert.That(ScanQueueManager.ScanQueues, Has.Count.EqualTo(3));

            var ms1Scan = SsdoList.First();
            var ms2Scan = SsdoList[1];
            ms2Scan.MsNOrder = 2;
            var ms3Scan = SsdoList[2];
            ms3Scan.MsNOrder = 3;

            ScanQueueManager.EnqueueScan(ms1Scan);
            Assert.That(ScanQueueManager.ScanQueues[1].Count == 1);
            Assert.That(ScanQueueManager.ScanQueues[2].Count == 0);
            Assert.That(ScanQueueManager.ScanQueues[3].Count == 0);

            ScanQueueManager.EnqueueScan(ms1Scan);
            Assert.That(ScanQueueManager.ScanQueues[1].Count == 2);
            Assert.That(ScanQueueManager.ScanQueues[2].Count == 0);
            Assert.That(ScanQueueManager.ScanQueues[3].Count == 0);

            ScanQueueManager.EnqueueScan(ms2Scan);
            Assert.That(ScanQueueManager.ScanQueues[1].Count == 2);
            Assert.That(ScanQueueManager.ScanQueues[2].Count == 1);
            Assert.That(ScanQueueManager.ScanQueues[3].Count == 0);

            ScanQueueManager.EnqueueScan(ms3Scan);
            Assert.That(ScanQueueManager.ScanQueues[1].Count == 2);
            Assert.That(ScanQueueManager.ScanQueues[2].Count == 1);
            Assert.That(ScanQueueManager.ScanQueues[3].Count == 1);

            Assert.That(!ScanQueueManager.CheckQueue(1, 3));
            Assert.That(ScanQueueManager.CheckQueue(1, 2));

            Assert.That(!ScanQueueManager.TryDequeueMany(1, 3, out IEnumerable<SingleScanDataObject> tempList));
            Assert.That(!tempList.Any());
            Assert.That(ScanQueueManager.TryDequeueMany(1, 2, out tempList));
            Assert.That(tempList.Count() == 2);

            Assert.That(ScanQueueManager.ScanQueues[1].Count == 0);
            Assert.That(ScanQueueManager.ScanQueues[2].Count == 1);
            Assert.That(ScanQueueManager.ScanQueues[3].Count == 1);


        }
    }
}
