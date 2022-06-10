using MassSpectrometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    public class PreProcessingData 
    {
        public int ScansToProcess { get; set; }
        public double MinX { get; set; }
        public double MaxX { get; set; }
        public double[][] XArrays { get; set; }
        public double[][] YArrays { get; set; }
        public double[] TotalIonCurrent { get; set; }
        
        public void SetData(double[][] xarr, double[][] yarr, double[] tic)
        {
            XArrays = xarr;
            YArrays = yarr;
            TotalIonCurrent = tic;
            ScansToProcess = xarr.Count();
        }

        public void SetData(List<MsDataScan> scans)
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
    }
}
