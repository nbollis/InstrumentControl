using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using InstrumentControl.Util;
using CommandLine;
using TaskInterfaces;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Scans;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Methods;
using Thermo.TNG.Factory;

namespace InstrumentControl
{
    public class Program
    { 
        public static IScans MScan { get; set; }
        

        /*
        Main structure: 
        0. Initialize FusionLumosAPI; create Application class; 
        create scanQueue. 
        1. Parse command line. 
           Output: 
               -> App 
               -> App Options 
        2. App.GetOptions(AppOptions)
        3. Initialize instrument connection
        4. subscribe App.ProcessScans to event triggering data processing
        5. Enter while loop
            
         
         */ 
        static void Main(string[] args)
        {
            FusionLumosAPI api = InitializeConnection();
            ScanQueue scanQueue = new ScanQueue(5);

            var parserResult = Parser.Default.ParseArguments<ScanAveragingAppOptions, object>(args)?.Value;
            var app = ((IApplicationOptions)parserResult)
                .InstantiateTask<ScanAveragingApp>();
            app.GetOptions(parserResult);

            scanQueue.ThresholdReached += app.ProcessScans; 

            // main loop 
            while (api.InstAccessContainer.ServiceConnected)
            {


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

        // MsScanArrived is an event.
        // The += means "subscribes to".
        // So the data.Receiver.MSScanContainer_MsScanArrived method is
        // subscribed to the MsScanArrived event and will be called when the event is raised.

        /* Technically, multiple methods can subscribe to the same event, 
         * however, it is likely doing so would require multithreading. 
         * You would need to have multiple objects working with a single IMsScan object, 
         * and I'm not sure how you would pass a single IMsScan to multiple threads yet
         */
    }
}