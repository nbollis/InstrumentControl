﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer
{
    public class StandardMzAxis
    {
        public double[] XAxis { get; set; }
        public StandardMzAxis(double minX, double maxX, double delta)
        {
            XAxis = ScanStandardization.CreateStandardMZAxis((minX, maxX), delta);  
        }
        public StandardMzAxis(StandardizationOptions options)
        {
            XAxis = ScanStandardization.CreateStandardMZAxis((options.MinMass, options.MaxMass), 
                options.Delta); 
        }

    }
}
