using System;
using Normalization; 

namespace Data
{
    public static class MultiScanDataObjectExtensions
    {
        public static void NormalizeSpectraToTic(this MultiScanDataObject scans, NormalizationOptions options)
        {
            for(int i = 0; i < scans.YArrays.GetLength(0) ; i++)
            {
                SpectrumNormalization.NormalizeSpectrumToTic(ref scans.YArrays[i],
                    scans.TotalIonCurrent[i]); 
            }
        }
    }
}
