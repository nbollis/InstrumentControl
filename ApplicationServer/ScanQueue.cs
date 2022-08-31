using IMSScanClassExtensions;
using InstrumentControlIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;
using Data; 

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
    }
}
