using MassSpectrometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;

namespace InstrumentControl
{
    /// <summary>
    /// Object to contain the information of a single MS1 scan received from the 
    /// instrument to be stored in a queue
    /// </summary>
    public class SingleScanDataObject
    {
        public double[] XArray { get; private set; }
        public double[] YArray { get; private set; }
        public double TotalIonCurrent { get; private set; }

        public SingleScanDataObject(IMsScan scan)
        {
            XArray = scan.Centroids.Select(c => c.Mz).ToArray();
            YArray = scan.Centroids.Select(c => c.Intensity).ToArray();
            TotalIonCurrent = double.Parse(scan.Header["Total Ion Current"]);
        }

        public SingleScanDataObject(MsDataScan scan, int tempToAllowCompiling)
        {
            XArray = scan.MassSpectrum.XArray;
            YArray = scan.MassSpectrum.YArray;
            TotalIonCurrent = scan.TotalIonCurrent;
        }
    }
}
