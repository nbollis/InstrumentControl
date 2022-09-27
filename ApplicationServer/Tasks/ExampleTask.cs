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
        public override int ScansOutputted => 1;

        public ExampleTask2(int acceptScanOrder, int scansToAccept) : base(acceptScanOrder, scansToAccept)
        {

        }

        protected override IEnumerable<SingleScanDataObject> ExecuteSpecific(IEnumerable<SingleScanDataObject> scansToProcess)
        {
            return new List<SingleScanDataObject>() { scansToProcess.First() };
        }
    }
}
