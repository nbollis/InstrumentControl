using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;
using ClientServerCommunication;
using Microsoft.Extensions.DependencyInjection;
using MzLibUtil;
using WorkflowServer;

namespace Tests
{
    public static class TestActivities
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
            SsdoList.Add(new SingleScanDataObject(xArray, y1) { ScanOrder = 1, RetentionTime = 2});
            SsdoList.Add(new SingleScanDataObject(xArray, y2) { ScanOrder = 1 });
            SsdoList.Add(new SingleScanDataObject(xArray, y3) { ScanOrder = 1 });
            SsdoList.Add(new SingleScanDataObject(xArray, y4) { ScanOrder = 1 });
        }

        private static async Task RunAsync(DefaultActivityRunner<IActivityContext> runner, IActivityCollection<IActivityContext> activities, IActivityContext context)
        {
            await runner.RunAsync(activities, context);
        }

        [Test]
        public static void TestTest()
        {
            SingleScanDataObject scan = SsdoList.First();
            scan.ScanInstructions = new ScanInstructions();
            MassTargetList targetList = new();
            targetList.Add(1, 2);

            IActivityContext context = new SpectraActivityContext(scan, targetList);

            // construct the provider
            IServiceProvider provider = new ServiceCollection().BuildServiceProvider();

            // configure the collection
            IActivityCollection<IActivityContext> activities =
                new DefaultActivityCollectionBuilder<IActivityContext>(provider)
                    .Then(new CheckInclusionListActivity<IActivityContext>(2, new PpmTolerance(10)))
                    .WhenAsync((ctx) => ((SpectraActivityContext)ctx).MassTargetList.FoundTargets,
                        (a) => a.Then(new DataDependentScanActivity<IActivityContext>(2)))
                    .Build();

            var runner = new DefaultActivityRunner<IActivityContext>(provider);
            RunAsync(runner, activities, context);


        }
    }
}




