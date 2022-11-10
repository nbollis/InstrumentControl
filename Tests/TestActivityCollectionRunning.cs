using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;
using IO.MzML;
using MassSpectrometry;
using Microsoft.Extensions.DependencyInjection;
using WorkflowServer;

namespace Tests
{
    public static class TestActivityCollectionRunning
    {
        private static NamedPipeServerStream pipeServerStream;
        private static IServiceProvider provider { get; set; }
        public static List<MsDataScan> ActualMS1Scans { get; set; }

        [OneTimeSetUp]
        public static void OneTimeSetup()
        {
            pipeServerStream =
                new NamedPipeServerStream("test",
                    PipeDirection.InOut, 1, PipeTransmissionMode.Byte);

            provider = new ServiceCollection().BuildServiceProvider();

            ActualMS1Scans = Mzml.LoadAllStaticData(Path.Combine(TestContext.CurrentContext.TestDirectory,
                @"TestData\TDYeastFractionMS1.mzML")).GetAllScansList().Where(p => p.MsnOrder == 1).ToList();
        }

        [Test]
        public static void TestActivityCollection()
        {
            IActivityContext context = new SpectraActivityContext();
            ScanQueueManager.BuildQueues(new List<int>() {1});
            var runner = new DefaultActivityRunner<IActivityContext>(provider);

            AcceptScansActivity<IActivityContext> acceptScansActivity = new(1, 5);
            TopNPeakSelectionActivity<IActivityContext> topNActivity = new(3, false, false);
            
            IActivityCollection<IActivityContext> collection = new DefaultActivityCollectionBuilder<IActivityContext>(provider)
                .Then(acceptScansActivity)
                .Then(topNActivity)
                .Build();

            DummyAppServerPipe pipe = new DummyAppServerPipe(pipeServerStream);
            pipe.StartServer(collection, context, provider);
            var scans = ActualMS1Scans.Take(16);

            foreach (var scan in scans.Take(5))
            {
                pipe.HandleDataReceived(new SingleScanDataObject(scan.MsnOrder, scan.OneBasedScanNumber, scan.MassSpectrum.XArray, scan.MassSpectrum.YArray, scan.OneBasedPrecursorScanNumber, null, scan.RetentionTime));
            }

        }

        [Test]
        public static void TestActivityCollection2()
        {
            IActivityContext context = new SpectraActivityContext();

            IActivityCollection<IActivityContext> collection =
                new DefaultActivityCollectionBuilder<IActivityContext>(provider)
                    .Then(new AcceptScansActivity<IActivityContext>(1, 5))
                    .Then(new TopNPeakSelectionActivity<IActivityContext>(3, false, false))
                    .Build();

            // TODO: Set up pipe
        }
    }
}
