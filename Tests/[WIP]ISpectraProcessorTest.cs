using InstrumentControl;
using MassSpectrometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using ScanProduction;
using TaskInterfaces;
using Moq;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;

namespace Tests
{
    public class _WIP_ScanQueueTest
    {
        [Test]
        public static void TempTest()
        {
            BoxCarScanBuilder builder = new();
            var builderProp = builder.GetType().GetProperties();
            var bulderDict = builder.BuildDictionary();
            
        }

        [Test]
        public static void TestDDATemp()
        {
            string filepath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"DataFiles\TDYeastFractionMS1.mzML");
            List<MsDataScan> scans = MS1DatabaseParser.LoadAllScansFromFile(filepath);
            List<SingleScanDataObject> data = new();
            foreach (var scan in scans)
            {
                data.Add(new SingleScanDataObject(scan));
            }

            DataDependentScanOptions options = new()
            {
                IsolationWidth = 0.7,
                Resolution = 15000,
                MaxIT = 25,
                NCE = 27,
                NCE_NormCharge = 2,
                AGC_Target = 100000
            };

            DataDependentScanTask ddaTask = new();
            foreach (var dataItem in data)
            {
                ddaTask.RunSpecific(options, dataItem);
            }

        }

        [Test]
        public static void IDKMan()
        {
            var mock = new Mock<IMsScan>();
            var obj = mock.Object;
        }


    }
}
