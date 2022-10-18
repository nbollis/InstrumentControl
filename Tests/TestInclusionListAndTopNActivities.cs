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
    public static class TestInclusionListAndTopNActivities
    {
        public static List<SingleScanDataObject> SsdoList { get; set; }
        public static IServiceProvider provider { get; set; }

        [OneTimeSetUp]
        public static void OneTimeSetup()
        {
            double[] xArray = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            double[] y1 = new double[] { 0, 100, 10, 10, 10, 20, 20, 10, 10, 0 };
            double[] y2 = new double[] { 0, 10, 100, 10, 10, 20, 20, 10, 10, 0 };
            double[] y3 = new double[] { 0, 10, 10, 100, 10, 20, 20, 10, 10, 0 };
            double[] y4 = new double[] { 0, 10, 10, 10, 100, 20, 20, 10, 10, 0 };

            SsdoList = new();
            SsdoList.Add(new SingleScanDataObject(xArray, y1) { ScanOrder = 1, RetentionTime = 2000});
            SsdoList.Add(new SingleScanDataObject(xArray, y2) { ScanOrder = 1, RetentionTime = 3000 });
            SsdoList.Add(new SingleScanDataObject(xArray, y3) { ScanOrder = 1, RetentionTime = 4000 });
            SsdoList.Add(new SingleScanDataObject(xArray, y4) { ScanOrder = 1, RetentionTime = 5000 });

            provider = new ServiceCollection().BuildServiceProvider();
        }

        private static async Task RunAsync(DefaultActivityRunner<IActivityContext> runner, IActivityCollection<IActivityContext> activities, IActivityContext context)
        {
            await runner.RunAsync(activities, context);
        }

        [Test]
        public static void TestTopNSelectionActivity()
        {
            SingleScanDataObject scan = SsdoList[2];
            scan.ScanInstructions = new();

            IActivityContext context = new SpectraActivityContext(scan, new MassTargetList());
            TopNPeakSelectionActivity<IActivityContext> topNActivity = new(3, false, false);
            topNActivity.ExecuteAsync(context);

            var selectedPeaks = ((SpectraActivityContext)context).MassesToTarget;
            Assert.That(selectedPeaks.Count() == 3);
            Assert.That(selectedPeaks.Dequeue()[0] == 3);
            Assert.That(selectedPeaks.Dequeue()[0] == 5);
            Assert.That(selectedPeaks.Dequeue()[0] == 6);
        }

        [Test]
        public static void TestTopNSelectionActivityWithExclusionList()
        {
            SingleScanDataObject scan = SsdoList[2];
            scan.ScanInstructions = new();
            MassTargetList targetList = new();
            targetList.Add(5, 4000, MassTargetList.MassTargetListTypes.Exclusion);

            IActivityContext context = new SpectraActivityContext(scan, targetList);
            TopNPeakSelectionActivity<IActivityContext> topNActivity = new(3, false, true);

            // exclude one of the top values
            topNActivity.ExecuteAsync(context);
            var selectedPeaks = ((SpectraActivityContext)context).MassesToTarget;
            Assert.That(selectedPeaks.Count() == 3);
            Assert.That(selectedPeaks.Dequeue()[0] == 3);
            Assert.That(selectedPeaks.Dequeue()[0] == 6);
            Assert.That(selectedPeaks.Dequeue()[0] == 1);

            // exclude one of the top values at the wrong time
            targetList.ExclusionList.Clear();
            targetList.Add(5, 7000, MassTargetList.MassTargetListTypes.Exclusion);
            topNActivity.ExecuteAsync(context);
            selectedPeaks = ((SpectraActivityContext)context).MassesToTarget;
            Assert.That(selectedPeaks.Count() == 3);
            Assert.That(selectedPeaks.Dequeue()[0] == 3);
            Assert.That(selectedPeaks.Dequeue()[0] == 5);
            Assert.That(selectedPeaks.Dequeue()[0] == 6);
        }

        [Test]
        public static void TestInclusionListActivity()
        {
            SingleScanDataObject scan = SsdoList.First();
            scan.ScanInstructions = new ScanInstructions();
            MassTargetList targetList = new();
            targetList.Add(1, 2000, MassTargetList.MassTargetListTypes.Inclusion);

            IActivityContext context = new SpectraActivityContext(scan, targetList);
            var activities = new DefaultActivityCollectionBuilder<IActivityContext>(provider)
                .Then(new CheckInclusionListActivity<IActivityContext>(0.5, new PpmTolerance(10)))
                .Build();

            var runner = new DefaultActivityRunner<IActivityContext>(provider);
            RunAsync(runner, activities, context);
            Assert.That(((SpectraActivityContext)context).MassTargetList.GetHitTargets().Count() == 1);
            Assert.That(!((SpectraActivityContext)context).MassTargetList.GetHitTargets().Any());

            CheckInclusionListActivity<IActivityContext> activity = new CheckInclusionListActivity<IActivityContext>(0.5, new PpmTolerance(10));
            activity.ExecuteAsync(context);
            Assert.That(((SpectraActivityContext)context).MassTargetList.GetHitTargets().Count() == 1);
        }

        [Test]
        public static void TestInclusionListTopNSelection()
        {
            SingleScanDataObject scan = SsdoList[2];
            scan.ScanInstructions = new();
            MassTargetList targetList = new();
            targetList.Add(7, 4000, MassTargetList.MassTargetListTypes.Inclusion);

            CheckInclusionListActivity<IActivityContext> inclusionActivity = new(0.1, new PpmTolerance(10));
            TopNPeakSelectionActivity<IActivityContext> topNActivity = new(3, true, false);

            IActivityContext context = new SpectraActivityContext(scan, targetList);
            IActivityCollection<IActivityContext> activities =
                new DefaultActivityCollectionBuilder<IActivityContext>(provider)
                    .Then(inclusionActivity)
                    .WhenAsync((ctx) => ((SpectraActivityContext)ctx).MassesToTarget.Any(),
                        (a) => a.Then(topNActivity))
                    .Build();

            var runner = new DefaultActivityRunner<IActivityContext>(provider);
            runner.RunAsync(activities, context);

        }
    }
}




