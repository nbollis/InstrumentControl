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
        Queue<IMsScan> DataToProcess { get; set; } // convert to mz spectrum 
        int Threshold { get; set; }
        public event EventHandler<ThresholdReachedEventArgs>? ThresholdReached;

        public ScanQueue(int processingThreshold)
        {
            Threshold = processingThreshold;
            DataToProcess = new Queue<IMsScan>(100);
        }
        protected virtual void OnThresholdReached(ThresholdReachedEventArgs e)
        {
            ThresholdReached?.Invoke(this, e);
        }
        public void AddValueToQueue(object? sender, MsScanEventArgs e)
        {
            DataToProcess.Enqueue(e.GetScan());
            if (DataToProcess.Count > Threshold)
            {
                ThresholdReachedEventArgs args = new ThresholdReachedEventArgs(DataToProcess, Threshold);
                OnThresholdReached(args);
            }
        }

        // unused method from the original RealTimeSampleProcessingExample
        public void MSScanContainer_MsScanArrived(object sender, MsScanEventArgs e)
        {
            // put scan in queue
            // Keeping IMScan open will hog the memory resources, so you need to get the  

            using (IMsScan scan = e.GetScan())
            {
                // need to quickly convert to something else
                // ScanProcessingQueue.Enqueue(scan);
                //ScanProcessingQueue.Enqueue(new MsDataScan(scan));				LOOK HERE!!! This line is commented out until we switch from MsDataScans to MzSpectrum objects
            }
            // Perform data processing below here. For clarity, use the minimum
            // number of method calls possible, preferablly a single method,
            // for example, ProteoformIdentifcationEngine(ProteoformIDParameters params);
            // Engine should return either void or an ICustomScan object, but I need to implement the
            // code to facilitate the building and sending of the ICustomScan. 

            // TODO: Implement ICustomScan wrapper to facilitate custom scan sending.
            // May need to wait till Chris Rose has figured everything with Thermo out. 
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


}
