using System;
using NUnit.Framework;
using MassSpectrometry.MzSpectra;
using System.Linq;
using MassSpectrometry;
using InstrumentControl;
using UsefulProteomicsDatabases;
using MzLibUtil;
using InstrumentControl.Tasks;

namespace Tests
{
    /*public class SpectraStandardizationTest 
    {
        public StandardizationTask StandardizationTask { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {

            string filepath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"DataFiles\TDYeastFractionMS1.mzML");
            List<MsDataScan> scans = MS1DatabaseParser.LoadAllScansFromFile(filepath);

            ISpectraProcesor.SetData(scans.Where(p => p.MsnOrder == 1).ToList().GetRange(10, 5));
            StandardizationTask = new StandardizationTask(TaskType.Standardization);
        }

        [Test]
        [TestCase(500.0, 1750.0, 0.001, 1251000)]
        public void TestCreateStandardMZAxis(double lowVal, double highVal, double massAccuraccy, 
            int expectedResult)
        {
            (double,double) range = new (lowVal, highVal);
            double[] resultStdArray = StandardizationTask.CreateStandardMZAxis(range, massAccuraccy);
            // expected result with this range is 1251000
            Assert.That(resultStdArray.Length, Is.EqualTo(expectedResult));
            Assert.That(resultStdArray[100] - resultStdArray[99], Is.EqualTo(massAccuraccy).Within(0.0005)); 
        }

        [Test]
        public void TestLinearInterpolate()
        {
            double x1, y1, x2, y2, x;
            x1 = 500.0;
            y1 = 1.2E6;
            x2 = 500.002;
            y2 = 1E6;
            x = 500.001;

            double result = StandardizationTask.LinearInterpolate(x1, y1, x2, y2, x);
            Assert.That(result, Is.EqualTo(1.1E6).Within(0.01)); 
        }

        [Test]
        public void TestCreateNewAxes()
        {
            var function = Enumerable.Range(0, 1000).Select(i => Math.Pow((double)i /100, 0.5)).ToArray();
            // create new array to hold upsample data. 
            double[] resampledData = new double[1200];

            StandardizationTask.ResampleDataAndInterpolate(function, ref resampledData);
            // Need to assert some things. 
            Assert.That(function[0] == resampledData[0]);
            Assert.That(function[function.Length / 2] == resampledData[resampledData.Length / 2]);
            Assert.That(function[function.Length - 1] == resampledData[resampledData.Length - 1]);
        }

        [Test]
        public void TestCreateStandardScan()
        {
            Loaders.LoadElements();
            double[] testIntVals = Enumerable.Range(0, 1000).Select(i => Math.Pow((double)i / 100, 0.5)).ToArray();
            double[] mzVals = Enumerable.Range(0, 1000).Select(i => (double)i / 100D).ToArray();
            MzSpectrum testSpectrum = new(mzVals, testIntVals, true);
            double[] newXAxis = Enumerable.Range(0, 1200).Select(i => (double)i).ToArray();
            
        }
    }
    */
}
