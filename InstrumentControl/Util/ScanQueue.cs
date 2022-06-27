using IMSScanClassExtensions;
using InstrumentControlIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;

namespace InstrumentControl
{
    internal class ScanQueue
    {
        Queue<SingleScanDataObject> DataToProcess { get; set; }
        int Threshold { get; set; }
        public bool ExportToJson { get; set; } = false;
        public event EventHandler<ThresholdReachedEventArgs>? ThresholdReached;

        public ScanQueue(int processingThreshold)
        {
            Threshold = processingThreshold;
            DataToProcess = new Queue<SingleScanDataObject>(100);
        }

        protected virtual void OnThresholdReached(ThresholdReachedEventArgs e)
        {
            ThresholdReached?.Invoke(this, e);
        }

        public void AddValueToQueue(object? sender, MsScanEventArgs e)
        {
            using (IMsScan scan = e.GetScan())
            {
                DataToProcess.Enqueue(new SingleScanDataObject(scan));

                // saves scans in json string format
                if (ExportToJson)
                {
                    string scanFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"IMsScanStrings.txt");
                    JsonSerializerDeserializer.SerializeAndAppend(scan, scanFilePath);
                }

                // if queue has reached capacity to begin processing
                if (DataToProcess.Count > Threshold)
                {
                    ISpectraProcesor.SetStandardizationRange(scan.Header["Scan Low Mass"], scan.Header["Scan High Mass"]);
                    ThresholdReachedEventArgs args = new ThresholdReachedEventArgs(DataToProcess, Threshold);
                    OnThresholdReached(args);
                }
            }
        }
    }
}
