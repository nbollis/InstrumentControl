using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommunication
{
    public class ScanInstructions
    {


        public List<double> MassesToIsolate { get; set; }


        public ScanInstructions()
        {
            MassesToIsolate = new List<double>();
        }
    }
}
