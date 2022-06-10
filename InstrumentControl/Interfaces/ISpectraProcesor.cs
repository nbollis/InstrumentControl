using MassSpectrometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    public interface ISpectraProcesor
    {
        protected static int ScansToProcess { get; set; }
        protected static double MinX { get; set; }
        protected static double MaxX { get; set; }
        protected static double[][] XArrays { get; set; }
        protected static double[][] YArrays { get; set; }
        protected static double[] TotalIonCurrent { get; set; }

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
