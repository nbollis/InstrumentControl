using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using InstrumentControl.Util;

namespace InstrumentControl
{
    class Program
    { 
        static void Main(string[] args)
        {
            FusionLumosAPI api = InitializeConnection();
            Application application;
            ScanQueue scanQueue = new ScanQueue(5);
            switch (args[0])
            {
                case "WholeChargeEnvelopeFragmentation":
                    application = new WholeChargeEnvelopeFragmentationApplication();
                    break;

                case "TestApplicationToSaveAndAverageData":
                    application = new TestApplicationToSaveAndAverageData();
                    scanQueue.ExportToJson = false; 
                    break;

                default:
                    Debugger.Break();
                    return;
            }
            scanQueue.ThresholdReached += application.ProcessScans;

            // main loop 
            while (api.InstAccessContainer.ServiceConnected)
            {
                // MsScanArrived is an event.
                // The += means "subscribes to".
                // So the data.Receiver.MSScanContainer_MsScanArrived method is
                // subscribed to the MsScanArrived event and will be called when the event is raised.

                /* Technically, multiple methods can subscribe to the same event, 
				 * however, it is likely doing so would require multithreading. 
				 * You would need to have multiple objects working with a single IMsScan object, 
				 * and I'm not sure how you would pass a single IMsScan to multiple threads yet
				 */

                api.MSScanContainer.MsScanArrived += scanQueue.AddValueToQueue;
            }            
        }

        // Instantiates the FusionLumosAPI and starts both online access and instrument access
        private static FusionLumosAPI InitializeConnection()
        {
            FusionLumosAPI api = new();
            api.StartOnlineAccess(); // call to start online connection. Need to add event and error handling. 
            api.GetInstAccess(1); // access instrument and fills the FusionLumosAPI class properties. 
            return api;
        }
        /*
         New code base structure: 
            Applications combine tasks
                Tasks contain data processing. 

        Verb (application) -> Applications contain tasks (options) -> Data Processing Options (data processing settings)
        Scan averaging: 
            Verb/Application Layer: Average
                Options: Normalization, Standardization, Averaging
                    Data processing options: Averaging type, rejection, etc. 
         */
    }
}