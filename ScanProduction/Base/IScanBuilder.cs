using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskInterfaces;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Scans;

namespace ScanProduction
{
    /// <summary>
    /// Interface for all specific builder classes to implement
    /// Allows them to be constructed by the Scan Producer class
    /// </summary>
    public interface IScanBuilder : ITaskOptions
    {
        public void BuildScan<T, U>(T options, U data) where T : ITaskOptions;
    }
}
