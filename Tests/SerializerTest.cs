using InstrumentControl;
using MassSpectrometry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class SerializerTest
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
        public static void SerializeMoreComplexObject()
        {
            string scanspath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"DataFiles\TDYeastFractionMS1.mzML");
            string filepath = Path.Combine(OutputDirectory, @"scan.txt");
            List<MsDataScan> scans = MS1DatabaseParser.LoadAllScansFromFile(scanspath);


            string jsonData;
            foreach (var scan in scans)
            {
                jsonData = JsonConvert.SerializeObject(scan);
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(jsonData);
                }
            }

            string[] jsonDataRecovered = File.ReadAllLines(filepath);


            int breakpoint = 0;
        }

    }
}
