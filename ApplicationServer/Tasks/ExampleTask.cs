using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;

namespace WorkflowServer
{
    public class ExampleTask : InstrumentControlTask
    {
        public ExampleTask(int acceptScanOrder, int scansToAccept) : base(acceptScanOrder, scansToAccept)
        {

        }

        protected override IEnumerable<SingleScanDataObject> ExecuteSpecific(IEnumerable<SingleScanDataObject> scansToProcess)
        {
            List<SingleScanDataObject> processedScans = new();

            return scansToProcess;
        }

        
    }

    public class ExampleTask2 : InstrumentControlTask
    {
        // Fill in required fields for performing the task at hand and add them to the constructor


        // Override this virtual int with how many it should output
        public override int ScansOutputted => 1;

        public ExampleTask2(int acceptScanOrder, int scansToAccept) : base(acceptScanOrder, scansToAccept)
        {

        }

        // Define what to do when it receives a scan/group of scans
        // should return some single scan data objects which will either be used in downstream processing if
        // NextTask is not null and the AcceptedScanOrder is -1
        // otherwise it will be sent to the client to inform the next scan to be captured
        protected override IEnumerable<SingleScanDataObject> ExecuteSpecific(IEnumerable<SingleScanDataObject> scansToProcess)
        {
            return new List<SingleScanDataObject>() { scansToProcess.First() };
        }
    }
}
