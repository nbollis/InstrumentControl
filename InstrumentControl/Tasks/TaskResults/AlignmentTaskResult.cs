using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    public class AlignmentTaskResult : TaskResults, ISpectraManipulator
    {
        public AlignmentTaskResult(AlignmentTask task) : base(task)
        {
            XArrays = task.XArrays;
            YArrays = task.YArrays;
            TotalIonCurrent = task.TotalIonCurrent;
        }

        public double[][] XArrays { get; set; }
        public double[][] YArrays { get; set; }
        public double[] TotalIonCurrent { get; set; }
    }
}
