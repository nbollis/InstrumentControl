using MzLibUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowServer;

namespace Tests
{
    public class TestExclusionList
    {
        [Test]
        public static void TestExclusionListItem()
        {
            var item = new MassTargetListItem(15000, 150, 155);
            Assert.That(item.WithinTimeSpan(154));
            Assert.That(!item.WithinTimeSpan(144));
        }

        [Test]
        public static void TestExclusionListAddition()
        {
            double[] testData = new double[] { 15, 47, 200, 36 };
            double[] currentTime = new double[] { 15000, 17000, 18000, 20000 };
            MassTargetList massTargetList = new();
            for (int i = 0; i < testData.Length; i++)
            {
                massTargetList.Add(testData[i], currentTime[i]);
            }
            Assert.That(massTargetList.MassTargets.Count, Is.EqualTo(testData.Length));
        }

        [Test]
        public static void TestExclusionListExcludeMassAtCurrentTime()
        {
            ScanProductionGlobalVariables.ExclusionMatchingPpmTolerance = 10;
            ScanProductionGlobalVariables.TimeToExcludeInMilliseconds = 1000;
            double[] testData = new double[] { 15, 47, 200, 36 };
            double[] currentTime = new double[] { 15000, 17000, 18000, 20000 };
            MassTargetList massTargetList = new();
            for (int i = 0; i < testData.Length; i++)
            {
                massTargetList.Add(testData[i], currentTime[i]);
            }

            // mass found and within time
            Assert.That(massTargetList.CheckMassAtCurrentTime(15, 15500));
            Assert.That(massTargetList.CheckMassAtCurrentTime(36.000001, 20001));
            Assert.That(massTargetList.CheckMassAtCurrentTime(15, 14000));
            Assert.That(massTargetList.CheckMassAtCurrentTime(15, 16000));

            // mass found not within time
            Assert.That(!massTargetList.CheckMassAtCurrentTime(15, 20000));
            Assert.That(!massTargetList.CheckMassAtCurrentTime(15, 13999));
            Assert.That(!massTargetList.CheckMassAtCurrentTime(15, 16001));

        }
    }
}
