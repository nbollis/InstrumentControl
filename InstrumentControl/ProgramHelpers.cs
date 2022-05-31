using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    public class ProgramHelpers
    {
        Queue<double> DataToProcess { get; set; }
        int Threshold { get; set; }
        public event EventHandler<ThresholdReachedEventArgs>? ThresholdReached;
        
        public ProgramHelpers(int processingThreshold)
        {
            Threshold = processingThreshold;
            DataToProcess = new Queue<double>(100);
        }
        protected virtual void OnThresholdReached(ThresholdReachedEventArgs e)
        {
            ThresholdReached?.Invoke(this, e);
        }
        public void AddValueToQueue(double value)
        {
            DataToProcess.Enqueue(value);
            if (DataToProcess.Count > Threshold)
            {
                ThresholdReachedEventArgs args = new ThresholdReachedEventArgs(DataToProcess, Threshold);
                OnThresholdReached(args);
            }
        }
    }
}

// IBoxcarScan : IMS1Scan
// BoxcarFraggerTask : IBoxcarScan, IMS2
// ChargeStateEnvelopeSelectorTask
// ScanAveragerTask

/*
    - Application
        - Task
            -Interfaces
 */