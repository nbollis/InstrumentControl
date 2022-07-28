using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;
using MassSpectrometry;
using IMSScanClassExtensions;
using TaskInterfaces;

namespace Data
{
    public class SingleScanDataObject : IData
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

        public SingleScanDataObject(MsDataScan scan)
        {
            XArray = scan.MassSpectrum.XArray;
            YArray = scan.MassSpectrum.YArray;
            TotalIonCurrent = scan.TotalIonCurrent;
            MinX = scan.MassSpectrum.XArray.Min();
            MaxX = scan.MassSpectrum.XArray.Max();
        }
        public void UpdateYarray(double[] newYarray)
        {
            YArray = newYarray; 
        }

        public static List<SingleScanDataObject> ConvertMSDataScansInBulk(List<MsDataScan> scans)
        {
            List<SingleScanDataObject> singleScanDataObjects = new List<SingleScanDataObject>();
            foreach (var scan in scans)
            {
                singleScanDataObjects.Add(new SingleScanDataObject(scan));
            }
            return singleScanDataObjects;
        }

        // temp
        public List<double>[] GetChargeStateEnvelopeMz()
        {
            List<double>[] result = new List<double>[1] { new List<double>()};
            return result;
        }
    }
}
