using MassSpectrometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    /// <summary>
    /// Interface to be inherited by all tasks who process spectral data before it is 
    /// utilized to create a custom scan. The static members will be shared by all who
    /// inherit. 
    /// </summary>
    public interface ISpectraProcesor
    {
        protected static int ScansToProcess { get; set; }
        protected static double MinX { get; set; }
        protected static double MaxX { get; set; }
        protected static double[][] XArrays { get; set; }
        protected static double[][] YArrays { get; set; }
        protected static double[] TotalIonCurrent { get; set; }

        public static void ProccessDataQueue(List<SingleScanDataObject> scanDataObjects)
        {
            ScansToProcess = scanDataObjects.Count;
            for (int i = 0; i < ScansToProcess; i++)
            {
                XArrays[i] = scanDataObjects[i].XArray;
                YArrays[i] = scanDataObjects[i].YArray;
                TotalIonCurrent[i] = scanDataObjects[i].TotalIonCurrent;
            }
        }



        // all below will likely be removed or changed in the future 
        public static void SetData(List<MsDataScan> scans)
        {
            ScansToProcess = scans.Count;
            XArrays = scans.Select(p => p.MassSpectrum.XArray).ToArray();
            YArrays = scans.Select(p => p.MassSpectrum.YArray).ToArray();
            TotalIonCurrent = scans.Select(p => p.TotalIonCurrent).ToArray();
            double? min = scans.Select(p => p.MassSpectrum.FirstX).Average();
            double? max = scans.Select(p => p.MassSpectrum.LastX).Average();
            MinX = min ??= 200;
            MaxX = max ??= 2000;
        }

        public static void SetData(double[][] xarr, double[][] yarr, double[] tic)
        {
            XArrays = xarr;
            YArrays = yarr;
            TotalIonCurrent = tic;
            ScansToProcess = xarr.Count();
        }
    }
}
