using Data;
using ScanProduction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Scans;

namespace InstrumentControl
{
    public class DataDependentScanTask : InstrumentControlTask
    {
        public DataDependentScanTask()
        {

        }

        public override void RunSpecific<T, U>(T options, U? data) where U : default
        {
            if (options == null || data == null)
            {
                throw new ArgumentException("Data or options passed to method is null.");
            }
            if (typeof(T) != typeof(DataDependentScanOptions))
            {
                throw new ArgumentException("Invalid options class for DataDependentScanTask");
            }
            if (typeof(U) != typeof(SingleScanDataObject))
            {
                throw new ArgumentException("Invalid DataObject for DataDependentScanTask");
            }

            // get mzVaules to isolate
            int scansSent = 0;
            double mzToIsolate;
            double[] topNintValues = (data as SingleScanDataObject).YArray.OrderByDescending(p => p).Take(ScanProductionGlobalVariables.TopN).ToArray();
            double[] mzsToIsolate = new double[topNintValues.Length];
            for (int i = 0; i < mzsToIsolate.Length; i++)
            {
                mzsToIsolate[i] = (data as SingleScanDataObject).XArray[Array.IndexOf((data as SingleScanDataObject).YArray, topNintValues[i])];
            }

            // send back custom scan
            ICustomScan scan = Program.MScan.CreateCustomScan();
            DataDependentScanBuilder dataBuilder = new DataDependentScanBuilder();
            ScanProducer producer = new(dataBuilder);
            // for each value to isolate and fragment
            foreach (var mz in mzsToIsolate)
            {
                producer.BuildScan(options, mz);
                producer.SetValuesToScan(scan);
                Program.MScan.SetCustomScan(scan);
            }
        }
    }
}
