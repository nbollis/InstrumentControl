﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WorkflowServer
{
    /// <summary>
    /// Normalizes the intensities of each peak to the total ion current of the scan
    /// All peak intensities should sum to one
    /// </summary>
    //public class NormalizationTask : InstrumentControlTask
    //{
    //    public NormalizationTask()
    //    {

    //    }

    //    public override void RunSpecific<T, U>(T options, U data)
    //    {
    //        if (data == null || options == null)
    //        {
    //            throw new ArgumentException("Arguments cannot be null");
    //        }
    //        if(typeof(T) != typeof(NormalizationOptions))
    //        {
    //            throw new ArgumentException("Invalid options class for NormalizationTask");
    //        }
    //        if((options as NormalizationOptions).PerformNormalization == true)
    //        {
    //            //if (typeof(U) == typeof(MultiScanDataObject))
    //            //{
    //            //    SpectrumNormalization.NormalizeSpectrumToTic(data as MultiScanDataObject, true);
    //            //}
    //            //else if (typeof(U) == typeof(SingleScanDataObject))
    //            //{
    //            //    SpectrumNormalization.NormalizeSpectrumToTic(data as SingleScanDataObject);
    //            //}
    //        }
    //    }
    //}
}
