using Data;
using MassSpectrometry;
using Standardization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskInterfaces;

namespace InstrumentControl
{
    public class StandardizationTask : InstrumentControlTask
    {

        public StandardizationTask() 
        {
        }

        public override void RunSpecific<T, U>(T options, U data)
        {

            if (data == null || options == null)
            {
                throw new ArgumentException("Arguments cannot be null");
            }
            if (typeof(T) != typeof(StandardizationOptions))
            {
                throw new ArgumentException("Invalid options class for StandardizationTask");
            }
            if (typeof(U) == typeof(MultiScanDataObject))
            {
                for (int i = 0; i < (data as MultiScanDataObject).ScansToProcess; i++)
                {
                    (data as SingleScanDataObject).UpdateYarray(ScanStandardization.GetStandardizedYArray( (data as SingleScanDataObject).YArray, options as StandardizationOptions));
                }
            }
            else if (typeof(U) == typeof(SingleScanDataObject))
            {
                (data as SingleScanDataObject).UpdateYarray(ScanStandardization.GetStandardizedYArray((data as SingleScanDataObject).YArray, options as StandardizationOptions));
            }
            //for (int i = 0; i < ISpectraProcesor.ScansToProcess; i++)
            //{
            //    double[] yarrayNew = new double[ISpectraProcesor.YArrays[i].Length];
            //    double[] xarrayNew = CreateStandardMZAxis((ISpectraProcesor.MinX, ISpectraProcesor.MaxX), 0.001);
            //    ResampleDataAndInterpolate(ISpectraProcesor.YArrays[i], ref yarrayNew);
            //    ISpectraProcesor.YArrays[i] = yarrayNew;
            //}

        }


    }
}
