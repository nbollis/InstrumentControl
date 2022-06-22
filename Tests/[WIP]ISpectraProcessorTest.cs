using InstrumentControl;
using MassSpectrometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class _WIP_ScanQueueTest
    {
        [Test]
        public static void TempTest()
        {
            string filepath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"DataFiles\TDYeastFractionMS1.mzML");
            List<MsDataScan> scans = MS1DatabaseParser.LoadAllScansFromFile(filepath).Where(p => p.MsnOrder == 1).ToList();
            List<SingleScanDataObject> data = new();
            foreach (MsDataScan scan in scans)
            {
                data.Add(new SingleScanDataObject(scan, 1));
            }

            ISpectraProcesor.ProccessDataQueue(data.GetRange(0, 5));

        }


    }
}
