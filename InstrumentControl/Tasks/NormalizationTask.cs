using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Normalization;
using Data; 


namespace InstrumentControl
{
    /// <summary>
    /// Normalizes the intensities of each peak to the total ion current of the scan
    /// All peak intensities should sum to one
    /// </summary>
    public class NormalizationTask : InstrumentControlTask
    {

        public NormalizationTask()
        {

        }

        public override void RunSpecific<T, U>(T options, U data)
        {
            if(typeof(T) != typeof(NormalizationOptions))
            {
                throw new ArgumentException("Invalid options class for NormalizationTask");
            }
            if((options as NormalizationOptions).PerformNormalization == true)
            {
                if (typeof(U) == typeof(MultiScanDataObject))
                {
                    MultiScanDataObjectExtensions.NormalizeSpectraToTic(data as MultiScanDataObject, options as NormalizationOptions);
                }
                else if (typeof(U) == typeof(SingleScanDataObject))
                {
                    SingleScanDataObject scan = data as SingleScanDataObject;
                    var yarray = scan.YArray;
                    SpectrumNormalization.NormalizeSpectrumToTic(ref yarray,
                        (data as SingleScanDataObject).TotalIonCurrent);
                    scan.UpdateYarray(yarray);
                }
            }
        }
    }
}
