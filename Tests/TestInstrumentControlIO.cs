using InstrumentControl;
using MassSpectrometry;
using Newtonsoft.Json;
using System.Linq;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;

namespace Tests
{
    public class TestInstrumentControlIO
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
            InstrumentControlIO.SerializeToNewFile(testDub, filepath);
            double testDubReturned = JsonConvert.DeserializeObject<double>(File.ReadLines(filepath).First());
            Assert.That(testDub == testDubReturned);
            testDubReturned = InstrumentControlIO.Deserialize<double>(filepath, true);
            Assert.That(testDub == testDubReturned);

            double[] testArray = new double[] { 14, 23, 37, 42 };
            filepath = Path.Combine(OutputDirectory, @"test2.txt");
            InstrumentControlIO.SerializeToNewFile(testArray, filepath);
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
            InstrumentControlIO.SerializeAndAppend(testDub1, filepath);
            InstrumentControlIO.SerializeAndAppend(testDub2, filepath);
            InstrumentControlIO.SerializeAndAppend(testDub3, filepath);
            InstrumentControlIO.SerializeAndAppend(testDub4, filepath);
            InstrumentControlIO.SerializeAndAppend(testDub5, filepath);
            var returnedArray = InstrumentControlIO.DeserializeCollection<double>(filepath).ToArray();
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
            InstrumentControlIO.SerializeToNewFile(firstSpectra, filepath);
            var returnedSpectrum = InstrumentControlIO.Deserialize<MzSpectrum>(filepath, true);
            Assert.That(returnedSpectrum.XArray.SequenceEqual(firstSpectra.XArray));
            Assert.That(returnedSpectrum.YArray.SequenceEqual(firstSpectra.YArray));

            // serialize and deserialize a collection  of MzSpectra
            filepath = Path.Combine(OutputDirectory, @"scan2.txt");
            List<MzSpectrum> twentySpectra = scans.GetRange(20, 20).Select(p => p.MassSpectrum).ToList();
            InstrumentControlIO.SerializeCollection<MzSpectrum>(twentySpectra, filepath);
            var deserialized = InstrumentControlIO.DeserializeCollection<MzSpectrum>(filepath).ToList();

            for (int i = 0; i < twentySpectra.Count; i++)
            {
                Assert.That(twentySpectra[i].XArray.SequenceEqual(deserialized[i].XArray));
                Assert.That(twentySpectra[i].YArray.SequenceEqual(deserialized[i].YArray));
            }

        }

        [Test]
        public static void SerializeIMsScan()
        {
           
        }



    }
}
