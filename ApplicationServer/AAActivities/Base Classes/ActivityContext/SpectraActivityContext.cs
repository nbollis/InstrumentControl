using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;
using ClientServerCommunication;

namespace WorkflowServer
{
    public class SpectraActivityContext : ActivityContext
    {
        public ScanInstructions ScanInstructions { get; set; }
        public SingleScanDataObject SingleScanDataObject { get; set; }



        public MassTargetList? MassTargetList { get; set; }




        public SpectraActivityContext(SingleScanDataObject ssdo, MassTargetList? massTargetList)
        {
            SingleScanDataObject = ssdo;
            ScanInstructions = ssdo.ScanInstructions;
            MassTargetList = massTargetList;
        }
    }
}
