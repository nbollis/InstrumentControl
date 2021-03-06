using InstrumentControl;
using MzLibUtil;
using ScanProduction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class TestExclusionList
    {
        [Test]
        public static void TestExclusionListItem()
        {
            var item = new ExclusionListItem(15000, 150, 155);
            Assert.That(item.WithinTimeSpan(154));
            Assert.That(!item.WithinTimeSpan(144));
        }

        [Test]
        public static void TestExclusionListAddition()
        {
            double[] testData = new double[] { 15, 47, 200, 36 };
            double[] currentTime = new double[] { 15000, 17000, 18000, 20000 };
            ExclusionList exclusionList = new();
            for (int i = 0; i < testData.Length; i++)
            {
                exclusionList.Add(testData[i], currentTime[i]);
            }
            Assert.That(exclusionList.TheExclusionList.Count, Is.EqualTo(testData.Length));
        }

        [Test]
        public static void TestExclusionListExcludeMassAtCurrentTime()
        {
            ScanProductionGlobalVariables.ExclusionMatchingPpmTolerance = 10;
            ScanProductionGlobalVariables.TimeToExcludeInMilliseconds = 1000;
            double[] testData = new double[] { 15, 47, 200, 36 };
            double[] currentTime = new double[] { 15000, 17000, 18000, 20000 };
            ExclusionList exclusionList = new();
            for (int i = 0; i < testData.Length; i++)
            {
                exclusionList.Add(testData[i], currentTime[i]);
            }

            // mass found and within time
            Assert.That(exclusionList.ExcludeMassAtCurrentTime(15, 15500));
            Assert.That(exclusionList.ExcludeMassAtCurrentTime(36.000001, 20001));
            Assert.That(exclusionList.ExcludeMassAtCurrentTime(15, 14000));
            Assert.That(exclusionList.ExcludeMassAtCurrentTime(15, 16000));

            // mass found not within time
            Assert.That(!exclusionList.ExcludeMassAtCurrentTime(15, 20000));
            Assert.That(!exclusionList.ExcludeMassAtCurrentTime(15, 13999));
            Assert.That(!exclusionList.ExcludeMassAtCurrentTime(15, 16001));

        }
    }
}
