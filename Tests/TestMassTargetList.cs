using MzLibUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowServer;
using WorkflowServer.Util;

namespace Tests
{
    public class TestMassTargetList
    {
        [Test]
        public static void TestMassTargetListItem()
        {
            var item = new MassTargetListItem(15000, 150, 155);
            Assert.That(item.Mass == 15000);
            Assert.That(item.StartTime == 150);
            Assert.That(item.EndTime == 155);
            Assert.That(item.WithinTimeSpan(154));
            Assert.That(!item.WithinTimeSpan(144));
            Assert.That(item.ToString() == "15000 : 150-155");

            List<double> masses = new List<double>() { 10, 20, 30, 40 };
            List<double> retentionTimes = new List<double>() { 1000, 3000, 4000, 6000 };
            List<MassTargetListItem> targets = MassTargetListItem.CreateListItemsInBulk(masses, retentionTimes, 1000);
            Assert.That(targets.Count, Is.EqualTo(4));
        }

        [Test]
        public static void TestExclusionList()
        {
            double[] testData = new double[] { 15, 47, 200, 36 };
            double[] currentTime = new double[] { 15000, 17000, 18000, 20000 };
            MassTargetList massTargetList = new();
            for (int i = 0; i < testData.Length; i++)
            {
                massTargetList.Add(testData[i], currentTime[i], MassTargetList.MassTargetListTypes.Exclusion);
            }
            Assert.That(massTargetList.ExclusionList.Count, Is.EqualTo(testData.Length));

            var notExcluded = massTargetList.GetTargetsNotExcludedAtSpecificRetentionTime(testData.ToList(), 17000);
            Assert.That(notExcluded.Count() == 2);
            Assert.That(notExcluded.Contains(15));
            Assert.That(notExcluded.Contains(36));

            notExcluded = massTargetList.GetTargetsNotExcludedAtSpecificRetentionTime(testData.ToList(), 16000);
            Assert.That(notExcluded.Count() == 2);
            Assert.That(notExcluded.Contains(200));
            Assert.That(notExcluded.Contains(36));

            notExcluded = massTargetList.GetTargetsNotExcludedAtSpecificRetentionTime(testData.ToList(), 16001);
            Assert.That(notExcluded.Count() == 3);
            Assert.That(notExcluded.Contains(15));
            Assert.That(notExcluded.Contains(200));
            Assert.That(notExcluded.Contains(36));
        }

        [Test]
        public static void TestInclusionList()
        {
            double[] testData = new double[] { 15, 47, 200, 36 };
            double[] currentTime = new double[] { 15000, 17000, 18000, 20000 };
            MassTargetList massTargetList = new();
            for (int i = 0; i < testData.Length; i++)
            {
                massTargetList.Add(testData[i], currentTime[i], MassTargetList.MassTargetListTypes.Inclusion);
            }
            Assert.That(massTargetList.InclusionList.Count, Is.EqualTo(testData.Length));

            var itemsFound = massTargetList.GetInclusionListItemsAtSpecificRetentionTime(16000).Select(p => p.Mass);
            Assert.That(itemsFound.Count() == 2);
            Assert.That(itemsFound.Contains(15));
            Assert.That(itemsFound.Contains(47));

            itemsFound = massTargetList.GetInclusionListItemsAtSpecificRetentionTime(16001).Select(p => p.Mass);
            Assert.That(itemsFound.Count() == 1);
            Assert.That(itemsFound.Contains(47));

            itemsFound = massTargetList.GetInclusionListItemsAtSpecificRetentionTime(17000).Select(p => p.Mass);
            Assert.That(itemsFound.Count() == 2);
            Assert.That(itemsFound.Contains(47));
            Assert.That(itemsFound.Contains(200));
        }

        [Test]
        public static void TestListImportExport()
        {

        }

    }
}
