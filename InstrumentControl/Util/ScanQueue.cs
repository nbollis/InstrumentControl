using IMSScanClassExtensions;
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
        bool ExportToJson { get; set; } = false;
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
                    IMsScanExtensions.JsonSerializeScan(scan);
                }

                // if queue has reached capacity to begin processing
                if (DataToProcess.Count > Threshold)
                {
                    ThresholdReachedEventArgs args = new ThresholdReachedEventArgs(DataToProcess, Threshold);
                    OnThresholdReached(args);
                }
            }
        }
    }
}
