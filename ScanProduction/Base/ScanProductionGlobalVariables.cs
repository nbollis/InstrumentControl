using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanProduction
{
    /// <summary>
    /// Class to hold variables that do not need to change throughout a run, but are 
    /// important to scan production
    /// </summary>
    public static class ScanProductionGlobalVariables
    {
        public static int TopN = 5;

        // dda exclusion stuff that may become its own options class
        public static double TimeToExcludeInMilliseconds = 1000;
        public static double ExclusionMatchingPpmTolerance = 10;
    }
}
