using MassSpectrometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    public class StandardizationTask : InstrumentControlTask
    {

        public StandardizationTask() 
        {
        }

        public override void RunSpecific<T, U>(T options, U data)
        {

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
