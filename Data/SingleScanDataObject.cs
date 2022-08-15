using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;

namespace Data
{
    [Serializable]
    public class SingleScanDataObject
    {
        public double[] XArray { get; private set; }
        public double[] YArray { get; private set; }
        public double TotalIonCurrent { get; private set; }
        public double MinX { get; private set; }
        public double MaxX { get; private set; }

        public double Resolution { get; private set; }

        public SingleScanDataObject(IMsScan scan)
        {
            XArray = scan.Centroids.Select(c => c.Mz).ToArray();
            YArray = scan.Centroids.Select(c => c.Intensity).ToArray();
            TotalIonCurrent = double.Parse(scan.Header["Total Ion Current"]);
            MinX = scan.GetValueFromHeaderDict<double>("FirstMass");
            MaxX = scan.GetValueFromHeaderDict<double>("LastMass");
            Resolution = scan.GetValueFromHeaderDict<double>("Orbitrap Resolution");
        }
        public void UpdateYarray(double[] newYarray)
        {
            YArray = newYarray; 
        }

        // temp
        public List<double>[] GetChargeStateEnvelopeMz()
        {
            List<double>[] result = new List<double>[1] { new List<double>()};
            return result;
        }
        public SingleScanDataObject(double[] xArray, double[] yArray, double totalIonCurrent, double minX, double maxX, double resolution)
        {
            XArray = xArray;
            YArray = yArray;
            TotalIonCurrent = totalIonCurrent;
            MinX = minX;
            MaxX = maxX;
            Resolution = resolution;
        }
        public SingleScanDataObject()
        {

        }
    }
}
