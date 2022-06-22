using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskInterfaces;
using CommandLine; 

namespace Standardization
{
    public class StandardizationOptions : ITaskOptions<StandardizationOptions>
    {
        [Option]
        public double MinMass { get; set; }
        [Option]
        public double MaxMass { get; set; }
        [Option]
        public double Delta { get; set; }
    }
}
