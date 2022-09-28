using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;
using WorkflowServer;

namespace Tests
{
    public class TempTests
    {
        public static List<SingleScanDataObject> SsdoList { get; set; }

        [OneTimeSetUp]
        public static void OneTimeSetup()
        {
            double[] xArray = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            double[] y1 = new double[] { 0, 100, 10, 10, 10, 20, 20, 10, 10, 0 };
            double[] y2 = new double[] { 0, 10, 100, 10, 10, 20, 20, 10, 10, 0 };
            double[] y3 = new double[] { 0, 10, 10, 100, 10, 20, 20, 10, 10, 0 };
            double[] y4 = new double[] { 0, 10, 10, 10, 100, 20, 20, 10, 10, 0 };

            SsdoList = new();
            SsdoList.Add(new SingleScanDataObject(xArray, y1) { ScanOrder = 1 });
            SsdoList.Add(new SingleScanDataObject(xArray, y2) { ScanOrder = 1 });
            SsdoList.Add(new SingleScanDataObject(xArray, y3) { ScanOrder = 1 });
            SsdoList.Add(new SingleScanDataObject(xArray, y4) { ScanOrder = 1 });
        }


        [Test]
        public static void BuildingWorkflowTaskStructure()
        {
            ExampleTask example1 = new(1, 4);
            ExampleTask2 example2 = new(-1, 2);
            example1.AddNextTask(example2);

            // nesting structure is correct in ExecuteTask and ExecuteSpecific
            var result = example1.ExecuteTask(SsdoList);
            Assert.That(result.Count() == 2);
        }

        [Test]
        public static void TestScanQueue()
        {
            ScanQueue queue = new(3);
            queue.Enqueue(SsdoList.Take(2));
            queue.Enqueue(SsdoList[2]);
        }

 
    }
}
