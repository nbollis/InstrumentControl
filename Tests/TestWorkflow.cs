using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;
using Easy.Common.Extensions;
using WorkflowServer;

namespace Tests
{
    public class TestWorkflow
    {
        public static AppServerPipe Pipe { get; set; }
        public static WorkflowFactory BasicWorkflow { get; set; }
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

            NamedPipeServerStream pipeServer =
                new NamedPipeServerStream("test",
                    PipeDirection.InOut, 1, PipeTransmissionMode.Byte);
            Pipe = new AppServerPipe(pipeServer);

            ExampleTask example1 = new(1, 4);
            BasicWorkflow = new(Pipe, new List<InstrumentControlTask>() { example1 });
        }

        [Test]
        public static void TestBasicWorkflowConstructor()
        {
            Assert.That(BasicWorkflow.ScanQueues.Count == 1);
            Assert.That(BasicWorkflow.Tasks.Count == 1);
        }

        [Test]
        public static void TestReceiveData()
        {
            ExampleTask example1 = new(1, 4);
            ExampleTask example2 = new(2, 4);
            ExampleTask example3 = new(3, 4);

            WorkflowFactory workflow = new(Pipe, 
                new List<InstrumentControlTask>() { example1, example2, example3 });
            Assert.That(workflow.ScanQueues.Count == 3);
            Assert.That(workflow.Tasks.Count == 3);

            Assert.That(workflow.ScanQueues.Values.All(p => p.DataToProcess.Count == 0));
            workflow.ReceiveData(SsdoList.First());
            Assert.That(workflow.ScanQueues[1].DataToProcess.Count == 1);
            Assert.That(workflow.ScanQueues[2].DataToProcess.Count == 0);
            Assert.That(workflow.ScanQueues[3].DataToProcess.Count == 0);
        }

        [Test]
        public static void TestCheckTaskListValidity()
        {
            ExampleTask example1 = new(1, 4);
            ExampleTask example2 = new(2, 4);
            ExampleTask example3 = new(3, 4);

            List<InstrumentControlTask> correctOrder = new List<InstrumentControlTask>()
                { example1, example2, example3 };
            try
            {
                var workflow = new WorkflowFactory(Pipe, correctOrder);
            }
            catch (Exception e)
            {
                Assert.That(false);
            }

            List<InstrumentControlTask> noMs1First = new List<InstrumentControlTask>() 
                { example2, example1, example3 };
            try
            {
                var workflow = new WorkflowFactory(Pipe, noMs1First);
                Assert.That(false);
            }
            catch (ArgumentException e)
            {
                Assert.That(e.Message == "First task must accept Ms1 Scans");
            }
            catch (Exception e)
            {
                Assert.That(false);
            }

            List<InstrumentControlTask> outOfOrder = new List<InstrumentControlTask>()
                { example1, example3, example2 };
            try
            {
                var workflow = new WorkflowFactory(Pipe, outOfOrder);
                Assert.That(false);
            }
            catch (ArgumentException e)
            {
                Assert.That(e.Message == "All tasks must have sequential MsnOrders");
            }
            catch (Exception e)
            {
                Assert.That(false);
            }
        }

        [Test]
        public static void TestWorkflowWithOneTask()
        {
            ExampleTask example1 = new(1, 4);
            ExampleTask2 example2 = new(-1, 2);
            example1.AddNextTask(example2);

            List<InstrumentControlTask> tasks = new List<InstrumentControlTask>() { example1 };
            WorkflowFactory workflow = new WorkflowFactory(Pipe, tasks);
        }
    }
}
