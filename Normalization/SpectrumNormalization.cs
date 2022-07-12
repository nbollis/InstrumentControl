using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalization
{
    public static class SpectrumNormalization
    {
        public static void NormalizeSpectrumToTic(ref double[] intensityArray, double ticVal)
        {
            for(int i = 0; i < intensityArray.Length; i++)
            {
                intensityArray[i] /= ticVal; 
            }
        }

        public static void NormalizeSpectrumToTic(double[] intensityArray, double ticVal)
        {
            for (int i = 0; i < intensityArray.Length; i++)
            {
                intensityArray[i] /= ticVal;
            }
        }

        /// <summary>
        /// Normalization method overload for MultiScanDataObjects
        /// </summary>
        /// <param name="scans"></param>
        public static void NormalizeSpectrumToTic(MultiScanDataObject scans)
        {
            for (int i = 0; i < scans.YArrays.GetLength(0); i++)
            {
                NormalizeSpectrumToTic(ref scans.YArrays[i], scans.TotalIonCurrent[i]);
            }
        }

        /// <summary>
        /// Normalization method overload for SingleScanDataObjects
        /// </summary>
        /// <param name="scan"></param>
        public static void NormalizeSpectrumToTic(SingleScanDataObject scan)
        {
            NormalizeSpectrumToTic(scan.YArray, scan.TotalIonCurrent);
        }
    }
}
