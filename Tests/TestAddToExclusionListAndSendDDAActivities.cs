using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;
using Microsoft.Extensions.DependencyInjection;
using WorkflowServer;
using WorkflowServer.Activities;

namespace Tests
{
    [ExcludeFromCodeCoverage]
    public static class TestAddToExclusionListAndSendDDAActivities
    {
        private static NamedPipeServerStream pipeServerStream;
        public static List<SingleScanDataObject> SsdoList { get; set; }
        public static IServiceProvider provider { get; set; }

        [OneTimeSetUp]
        public static void OneTimeSetup()
        {
            double[] x1 = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            double[] x2 = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            double[] x3 = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            double[] x4 = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            double[] y1 = new double[] { 0, 100, 90, 80, 70, 20, 20, 60, 50, 0 };
            double[] y2 = new double[] { 0, 100, 90, 80, 70, 20, 20, 60, 50, 0 };
            double[] y3 = new double[] { 0, 100, 90, 80, 70, 20, 20, 60, 50, 0 };
            double[] y4 = new double[] { 0, 10, 10, 10, 100, 20, 20, 10, 10, 0 };

            SsdoList = new();
            SsdoList.Add(new SingleScanDataObject(x1, y1) { MsNOrder = 1, RetentionTime = 2000 });
            SsdoList.Add(new SingleScanDataObject(x2, y2) { MsNOrder = 1, RetentionTime = 3000 });
            SsdoList.Add(new SingleScanDataObject(x3, y3) { MsNOrder = 1, RetentionTime = 4000 });
            SsdoList.Add(new SingleScanDataObject(x4, y4) { MsNOrder = 1, RetentionTime = 5000 });

            provider = new ServiceCollection().BuildServiceProvider();

            pipeServerStream =
                new NamedPipeServerStream("test",
                    PipeDirection.InOut, 1, PipeTransmissionMode.Byte);
        }

        [Test]
        public static void TestExclusionList()
        {
            var top6 = SsdoList.First().FilterByNumberOfMostIntense(6);
            TopNPeakSelectionActivity<IActivityContext> topNActivity = new(3, false, true);

            SpectraActivityContext context = new SpectraActivityContext();
            context.DataToProcess.Enqueue(SsdoList.First());

            Assert.That(context.MassTargetList.ExclusionList.Count == 0);
            topNActivity.ExecuteAsync(context);

            Assert.That(context.MassesToTarget.Count == 3);
            Assert.That(context.MassTargetList.ExclusionList.Count == 3);
        }

        [Test]
        public static async Task TestExclusionListWithDDASelection()
        {
            // initial setup
            var top6 = SsdoList.First().FilterByNumberOfMostIntense(6).ToList();
            CaptureMs1Activity<IActivityContext> captureMs1Activity = new(1, 1, WorkflowInjector.GetBaseMs1Scan());
            TopNPeakSelectionActivity<IActivityContext> topNActivity = new(3, false, true);
            SendDDAScanInstructionsActivity<IActivityContext> sendDdaScanInstructionsActivity =
                new(WorkflowInjector.GetBaseMs2Scan());
            SpectraActivityContext context = new SpectraActivityContext();
            ScanQueueManager.BuildQueue(1);
            ScanQueueManager.EnqueueScan(SsdoList.First());
            ScanQueueManager.EnqueueScan(SsdoList[1]);
            ScanQueueManager.EnqueueScan(SsdoList[2]);

            Assert.That(context.DataToProcess.Count == 0);
            Assert.That(ScanQueueManager.ScanQueues[1].Count == 3);
            Assert.That(context.MassesToTarget.Count == 0);
            Assert.That(context.MassTargetList.ExclusionList.Count == 0);

            // testing activities
            // accept scan moves from queue to data to process
            await captureMs1Activity.ExecuteAsync(context);
            Assert.That(context.DataToProcess.Count == 1);
            Assert.That(ScanQueueManager.ScanQueues[1].Count == 2);
            Assert.That(context.MassesToTarget.Count == 0);
            Assert.That(context.MassTargetList.ExclusionList.Count == 0);

            // top N removes from data to process
            // selects top 3 then moves those to masses to target
            // should also exclude those three masses
            await topNActivity.ExecuteAsync(context);
            Assert.That(context.DataToProcess.Count == 0);
            Assert.That(ScanQueueManager.ScanQueues[1].Count == 2);
            Assert.That(context.MassesToTarget.Count == 3);
            Assert.That(context.MassTargetList.ExclusionList.Count == 3);
            for (int i = 0; i < context.MassesToTarget.Count; i++)
            {
                Assert.That(Math.Abs(context.MassesToTarget.ElementAt(i).First() - top6[i].mass) < 0.001);
                Assert.That(Math.Abs(context.MassTargetList.ExclusionList[i].Mass - top6[i].mass) < 0.001);
                Assert.That(Math.Abs(context.MassTargetList.ExclusionList[i].EndTime -
                                     (SsdoList[0].RetentionTime + context.MassTargetList.TimeToExcludeInMilliseconds)) < 0.001);
                Assert.That(Math.Abs(context.MassTargetList.ExclusionList[i].StartTime -
                                     (SsdoList[0].RetentionTime - context.MassTargetList.TimeToExcludeInMilliseconds)) < 0.001);
            }

            // send DDa should remove each mass to target
            await sendDdaScanInstructionsActivity.ExecuteAsync(context);
            Assert.That(context.DataToProcess.Count == 0);
            Assert.That(ScanQueueManager.ScanQueues[1].Count == 2);
            Assert.That(context.MassesToTarget.Count == 0);
            Assert.That(context.MassTargetList.ExclusionList.Count == 3);

            // process second scan
            await captureMs1Activity.ExecuteAsync(context);
            Assert.That(context.DataToProcess.Count == 1);
            Assert.That(ScanQueueManager.ScanQueues[1].Count == 1);
            Assert.That(context.MassesToTarget.Count == 0);
            Assert.That(context.MassTargetList.ExclusionList.Count == 3);

            // top N should select 3 for fragmentation
            // they should be a different 3 masses than the first
            await topNActivity.ExecuteAsync(context);
            Assert.That(context.DataToProcess.Count == 0);
            Assert.That(ScanQueueManager.ScanQueues[1].Count == 1);
            Assert.That(context.MassesToTarget.Count == 3);
            Assert.That(context.MassTargetList.ExclusionList.Count == 6);
            for (int i = 0; i < context.MassesToTarget.Count; i++)
            {
                Assert.That(Math.Abs(context.MassesToTarget.ElementAt(i).First() - top6[i + 3].mass) < 0.001);
                Assert.That(Math.Abs(context.MassTargetList.ExclusionList[i + 3].Mass - top6[i + 3].mass) < 0.001);
                Assert.That(Math.Abs(context.MassTargetList.ExclusionList[i + 3].EndTime -
                                     (SsdoList[1].RetentionTime + context.MassTargetList.TimeToExcludeInMilliseconds)) < 0.001);
                Assert.That(Math.Abs(context.MassTargetList.ExclusionList[i + 3].StartTime -
                                     (SsdoList[1].RetentionTime - context.MassTargetList.TimeToExcludeInMilliseconds)) < 0.001);
            }

            // send DDa should remove each mass to target
            await sendDdaScanInstructionsActivity.ExecuteAsync(context);
            Assert.That(context.DataToProcess.Count == 0);
            Assert.That(ScanQueueManager.ScanQueues[1].Count == 1);
            Assert.That(context.MassesToTarget.Count == 0);
            Assert.That(context.MassTargetList.ExclusionList.Count == 6);


            // process third scan
            await captureMs1Activity.ExecuteAsync(context);
            Assert.That(context.DataToProcess.Count == 1);
            Assert.That(ScanQueueManager.ScanQueues[1].Count == 0);
            Assert.That(context.MassesToTarget.Count == 0);
            Assert.That(context.MassTargetList.ExclusionList.Count == 6);

            // top N should select 3 for fragmentation
            // they should be a same 3 masses than the first
            await topNActivity.ExecuteAsync(context);
            Assert.That(context.DataToProcess.Count == 0);
            Assert.That(ScanQueueManager.ScanQueues[1].Count == 0);
            Assert.That(context.MassesToTarget.Count == 3);
            Assert.That(context.MassTargetList.ExclusionList.Count == 9);
            for (int i = 0; i < context.MassesToTarget.Count; i++)
            {
                Assert.That(Math.Abs(context.MassesToTarget.ElementAt(i).First() - top6[i].mass) < 0.001);
                Assert.That(Math.Abs(context.MassTargetList.ExclusionList[i].Mass - top6[i].mass) < 0.001);
                Assert.That(Math.Abs(context.MassTargetList.ExclusionList[i + 6].EndTime -
                                     (SsdoList[2].RetentionTime + context.MassTargetList.TimeToExcludeInMilliseconds)) < 0.001);
                Assert.That(Math.Abs(context.MassTargetList.ExclusionList[i + 6].StartTime -
                                     (SsdoList[2].RetentionTime - context.MassTargetList.TimeToExcludeInMilliseconds)) < 0.001);
            }

            // send DDa should remove each mass to target
            await sendDdaScanInstructionsActivity.ExecuteAsync(context);
            Assert.That(context.DataToProcess.Count == 0);
            Assert.That(ScanQueueManager.ScanQueues[1].Count == 0);
            Assert.That(context.MassesToTarget.Count == 0);
            Assert.That(context.MassTargetList.ExclusionList.Count == 9);
        }



        [Test]
        public static void TestExclusionListActivityInWorkflow()
        {
            var top6 = SsdoList.First().FilterByNumberOfMostIntense(6);
            CaptureMs1Activity<IActivityContext> captureMs1Activity = new(1, 1, WorkflowInjector.GetBaseMs1Scan());
            TopNPeakSelectionActivity<IActivityContext> topNActivity = new(3, false, true);
            SendDDAScanInstructionsActivity<IActivityContext> sendDdaScanInstructionsActivity =
                new(WorkflowInjector.GetBaseMs2Scan());

            SpectraActivityContext context = new SpectraActivityContext();
            IActivityCollection<IActivityContext> collection = new DefaultActivityCollectionBuilder<IActivityContext>(provider)
                .Then(captureMs1Activity)
                .Then(topNActivity)
                .Then(sendDdaScanInstructionsActivity)
                .Build();

            DummyAppServerPipe pipe = new(pipeServerStream);
            collection.ConnectPipe(pipe);
            pipe.StartServer(collection, context, provider);


            Assert.That(context.MassTargetList.ExclusionList.Count == 0);
            Assert.That(!context.MassesToTarget.Any());
            Assert.That(!context.DataToProcess.Any());

            pipe.HandleDataReceived(SsdoList.First());
            Task.Delay(5000);

            Assert.That(context.MassTargetList.ExclusionList.Count == 3);
            Assert.That(!context.MassesToTarget.Any());
            Assert.That(!context.DataToProcess.Any());
        }
    }
}
