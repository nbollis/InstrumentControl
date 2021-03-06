using IMSScanClassExtensions;
using InstrumentControl;
using InstrumentControlIO;
using MassSpectrometry;
using Newtonsoft.Json;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;

namespace Tests
{
    public class TestIO
    {
        public static string OutputDirectory;

        [OneTimeSetUp]
        public static void OneTimeSetup()
        {
            OutputDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, @"SerializerTest");
            if (!Directory.Exists(OutputDirectory))
                Directory.CreateDirectory(OutputDirectory);
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            if (Directory.Exists(OutputDirectory))
                Directory.Delete(OutputDirectory, true);
        }

        [Test]
        public static void SerializationOfSimpleObjects()
        {
            double testDub = 14;
            string filepath = Path.Combine(OutputDirectory, @"test1.txt");
            JsonSerializerDeserializer.SerializeToNewFile(testDub, filepath);
            double testDubReturned = JsonConvert.DeserializeObject<double>(File.ReadLines(filepath).First());
            Assert.That(testDub == testDubReturned);
            testDubReturned = JsonSerializerDeserializer.Deserialize<double>(filepath, true);
            Assert.That(testDub == testDubReturned);

            double[] testArray = new double[] { 14, 23, 37, 42 };
            filepath = Path.Combine(OutputDirectory, @"test2.txt");
            JsonSerializerDeserializer.SerializeToNewFile(testArray, filepath);
            double[] testArrayReturned = (double[])JsonConvert.DeserializeObject(File.ReadAllText(filepath), typeof(double[]));
            Assert.That(testArray.SequenceEqual(testArrayReturned));

            // Serializing and Deserializing appended collection
            double testDub1 = 14;
            double testDub2 = 16;
            double testDub3 = 23;
            double testDub4 = 79;
            double testDub5 = 68.4;
            testArray = new double[] { testDub1, testDub2, testDub3, testDub4, testDub5 };
            filepath = Path.Combine(OutputDirectory, @"test3.txt");
            JsonSerializerDeserializer.SerializeAndAppend(testDub1, filepath);
            JsonSerializerDeserializer.SerializeAndAppend(testDub2, filepath);
            JsonSerializerDeserializer.SerializeAndAppend(testDub3, filepath);
            JsonSerializerDeserializer.SerializeAndAppend(testDub4, filepath);
            JsonSerializerDeserializer.SerializeAndAppend(testDub5, filepath);
            var returnedArray = JsonSerializerDeserializer.DeserializeCollection<double>(filepath).ToArray();
            Assert.That(testArray.SequenceEqual(returnedArray));
        }

        [Test]
        public static void SerializeMzSpectrum()
        {
            string scanspath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"DataFiles\TDYeastFractionMS1.mzML");
            string filepath = Path.Combine(OutputDirectory, @"scan.txt");
            List<MsDataScan> scans = MS1DatabaseParser.LoadAllScansFromFile(scanspath);
            List<MzSpectrum> spectra = scans.Select(p => p.MassSpectrum).ToList();
            MzSpectrum firstSpectra = spectra.First();

            // serialize and deserialize single MzSpectra
            JsonSerializerDeserializer.SerializeToNewFile(firstSpectra, filepath);
            var returnedSpectrum = JsonSerializerDeserializer.Deserialize<MzSpectrum>(filepath, true);
            Assert.That(returnedSpectrum.XArray.SequenceEqual(firstSpectra.XArray));
            Assert.That(returnedSpectrum.YArray.SequenceEqual(firstSpectra.YArray));

            // serialize and deserialize a collection  of MzSpectra
            filepath = Path.Combine(OutputDirectory, @"scan2.txt");
            List<MzSpectrum> twentySpectra = scans.GetRange(20, 20).Select(p => p.MassSpectrum).ToList();
            JsonSerializerDeserializer.SerializeCollection(twentySpectra, filepath);
            var deserialized = JsonSerializerDeserializer.DeserializeCollection<MzSpectrum>(filepath).ToList();

            for (int i = 0; i < twentySpectra.Count; i++)
            {
                Assert.That(twentySpectra[i].XArray.SequenceEqual(deserialized[i].XArray));
                Assert.That(twentySpectra[i].YArray.SequenceEqual(deserialized[i].YArray));
            }

        }

        [Test]
        public static void DeserializeIMsScan()
        {

            string folderpath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"DataFiles\IMsScansSerialized");
            string[] scanFolders = Directory.GetDirectories(folderpath);
            List<IMsScan> scans = new();
            foreach (var folder in scanFolders)
            {
                IMsScan scan = ScanSerialization.ImportIMsScan(folder);
                scans.Add(scan);
            }
            Assert.That(scans.Count() == scanFolders.Length);


            string outputPath = Path.Combine(OutputDirectory, @"IMsScans");
            foreach (var scan in scans)
            {
                Assert.That(scan.Header != null);
                Assert.That(scan.TuneData != null);
                Assert.That(scan.ChargeEnvelopes == null);
                Assert.That(scan.NoiseBand.Count() == 0);
                Assert.That(scan.NoiseCount == 0);
                Assert.That(scan.Centroids != null);
                Assert.That(scan.CentroidCount != null);
                Assert.That(scan.Centroids.Count() == scan.CentroidCount);
                Assert.That(scan.DetectorName != null);
            }
        }

        [Test]
        public static void TestLoadingMsDataScanAsIMsScan()
        {
            string scanspath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"DataFiles\TDYeastFractionMS1.mzML");
            List<MsDataScan> scans = MS1DatabaseParser.LoadAllScansFromFile(scanspath);
            List<IMsScan> imsScans = new List<IMsScan>();

            foreach (var scan in scans)
            {
                IMsScan imsScan = new IMsScanInstance(scan);

                Assert.That(imsScan.TuneData == null);
                Assert.That(imsScan.ChargeEnvelopes == null);
                Assert.That(imsScan.NoiseBand == null);
                Assert.That(imsScan.NoiseCount == null);
                Assert.That(imsScan.Centroids != null);
                Assert.That(imsScan.CentroidCount != null);
                Assert.That(imsScan.CentroidCount == scan.MassSpectrum.XArray.Length);
                Assert.That(imsScan.Centroids.Count() == imsScan.CentroidCount);
                Assert.That(imsScan.DetectorName != null);
                Assert.That(imsScan.Header != null);
                Assert.That(imsScan.Header["MassAnalyzer"] == scan.MzAnalyzer.ToString());
                Assert.That(imsScan.Header["StartTime"] == scan.RetentionTime.ToString());
                Assert.That(imsScan.Header["Scan"] == scan.OneBasedScanNumber.ToString());
                Assert.That(imsScan.Header["TIC"] == scan.TotalIonCurrent.ToString());
                Assert.That(imsScan.Header["BasePeakIntensity"] == scan.MassSpectrum.YArray.First().ToString("0.0"));
                Assert.That(imsScan.Header["BasePeakMass"] == scan.MassSpectrum.XArray.First().ToString("0.00"));
                Assert.That(imsScan.Header["Polarity"] == scan.Polarity.ToString());
                Assert.That(imsScan.Header["InjectTime"] == scan.InjectionTime.ToString());
                Assert.That(imsScan.Header["MSOrder"] == scan.MsnOrder.ToString());

                imsScans.Add(imsScan);
            }
            Assert.That(scans.Count == imsScans.Count);
        }
    }
}
