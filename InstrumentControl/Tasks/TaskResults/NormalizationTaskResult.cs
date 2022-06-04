using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    public class NormalizationTaskResult : TaskResults, ISpectraManipulator
    {
        public NormalizationTaskResult(NormalizationTask task) : base(task)
        {
            XArrays = task.XArrays;
            YArrays = task.YArrays;
            TotalIonCurrent = task.TotalIonCurrent;
        }

        public double[][] XArrays { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double[][] YArrays { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double[] TotalIonCurrent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
