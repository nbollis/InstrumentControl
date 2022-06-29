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
    public class BoxCarScanTask : InstrumentControlTask
    {
        public BoxCarScanTask()
        {

        }

        public override void RunSpecific<T, U>(T options, U data) 
        {
            if (options == null || data == null)
            {
                throw new ArgumentException("Data or options passed to method is null.");
            }
            if (typeof(T) != typeof(BoxCarScanOptions))
            {
                throw new ArgumentException("Invalid options class for BoxCarScanTask");
            }
            if (typeof(U) != typeof(SingleScanDataObject))
            {
                throw new ArgumentException("Invalid DataObject for BoxCarScanTask");
            }

            
            // get mzValues to isolate
            List<double>[] mzValues = (data as SingleScanDataObject).GetChargeStateEnvelopeMz(); // NOT A REAL METHOD YET

            // send back custom scan
            ICustomScan scan = Program.MScan.CreateCustomScan();
            BoxCarScanBuilder boxBuilder = new();
            ScanProducer producer = new(boxBuilder);
            // for each set of charge states desired to be isolated
            foreach (var mz in mzValues)
            {
                producer.BuildScan(options, data);
                producer.SetValuesToScan(scan);
                Program.MScan.SetCustomScan(scan);
            }
        }
    }
}
