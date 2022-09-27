using System;
using NUnit.Framework;
using MassSpectrometry.MzSpectra;
using System.Linq;
using ClientServerCommLibrary;
using MassSpectrometry;
using UsefulProteomicsDatabases;
using MzLibUtil;
using IO.MzML;
using WorkflowServer;

namespace Tests
{
    //public class TestSpectraStandardization
    //{
    //    public StandardizationTask StandardizationTask { get; set; }
    //    public StandardizationOptions StandardizationOptions { get; set; }
    //    public SingleScanDataObject SingleScanDataObject { get; set; }
    //    public MultiScanDataObject MultiScanDataObject { get; set; }

    //    [OneTimeSetUp]
    //    public void OneTimeSetup()
    //    {
    //        Loaders.LoadElements();
    //        string filepath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"DataFiles\TDYeastFractionMS1.mzML");
    //        List<MsDataScan> scans = Mzml.LoadAllStaticData(filepath).GetAllScansList().Where(p => p.MsnOrder == 1)
    //            .ToList();
    //        List<SingleScanDataObject> singleScans = new();
    //        foreach (var scan in scans)
    //        {
    //            singleScans.Add(new SingleScanDataObject(scan.MassSpectrum.XArray,
    //                scan.MassSpectrum.YArray, scan.TotalIonCurrent, (double)scan.MassSpectrum.FirstX, (double)scan.MassSpectrum.LastX, 0));
    //        }
    //        SingleScanDataObject = singleScans[0];
    //        MultiScanDataObject = new MultiScanDataObject(singleScans.GetRange(1, 5));
    //        StandardizationOptions = new StandardizationOptions()
    //        {
    //            MinMass = SingleScanDataObject.MinX,
    //            MaxMass = SingleScanDataObject.MaxX,
    //            Delta = 0.001
    //        };

    //        StandardizationTask = new StandardizationTask();
    //    }

    //    [Test]
    //    [TestCase(500.0, 1750.0, 0.001, 1251000)]
    //    public void TestCreateStandardMZAxis(double lowVal, double highVal, double massAccuraccy,
    //        int expectedResult)
    //    {
    //        (double, double) range = new(lowVal, highVal);
    //        double[] resultStdArray = ScanStandardization.CreateStandardMZAxis(range, massAccuraccy);
    //        // expected result with this range is 1251000
    //        Assert.That(resultStdArray.Length, Is.EqualTo(expectedResult));
    //        Assert.That(resultStdArray[100] - resultStdArray[99], Is.EqualTo(massAccuraccy).Within(0.0005));
    //    }

    //    [Test]
    //    public void TestLinearInterpolate()
    //    {
    //        double x1, y1, x2, y2, x;
    //        x1 = 500.0;
    //        y1 = 1.2E6;
    //        x2 = 500.002;
    //        y2 = 1E6;
    //        x = 500.001;

    //        double result = ScanStandardization.LinearInterpolate(x1, y1, x2, y2, x);
    //        Assert.That(result, Is.EqualTo(1.1E6).Within(0.01));
    //    }

    //    [Test]
    //    public void TestCreateNewAxes()
    //    {
    //        var function = Enumerable.Range(0, 1000).Select(i => Math.Pow((double)i / 100, 0.5)).ToArray();
    //        // create new array to hold upsample data. 
    //        double[] resampledData = new double[1200];

    //        ScanStandardization.ResampleDataAndInterpolate(function, ref resampledData);
    //        // Need to assert some things. 
    //        Assert.That(function[0] == resampledData[0]);
    //        Assert.That(function[function.Length / 2] == resampledData[resampledData.Length / 2]);
    //        Assert.That(function[function.Length - 1] == resampledData[resampledData.Length - 1]);
    //    }

    //    [Test] // not a test
    //    public void TestCreateStandardScan()
    //    {
    //        double[] testIntVals = Enumerable.Range(0, 1000).Select(i => Math.Pow((double)i / 100, 0.5)).ToArray();
    //        double[] mzVals = Enumerable.Range(0, 1000).Select(i => (double)i / 100D).ToArray();
    //        MzSpectrum testSpectrum = new(mzVals, testIntVals, true);
    //        double[] newXAxis = Enumerable.Range(0, 1200).Select(i => (double)i).ToArray();
    //    }

    //    [Test]
    //    public void TestGetStandardizedArray()
    //    {
    //        double[] yarray = SingleScanDataObject.YArray;
    //        var test = ScanStandardization.GetStandardizedYArray(
    //            SingleScanDataObject.YArray, StandardizationOptions);
    //        Assert.That(SingleScanDataObject.YArray.SequenceEqual(yarray));
    //    }
    //}

}
