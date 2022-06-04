using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    public class AlignmentTask : InstrumentControlTask, ISpectraManipulator
    {
        public AlignmentTask(TaskType taskType, ISpectraManipulator results) : base(taskType)
        {
            XArrays = results.XArrays;
            YArrays = results.YArrays;
            TotalIonCurrent = results.TotalIonCurrent;
        }

        public double[][] XArrays { get; set; }
        public double[][] YArrays { get; set; }
        public double[] TotalIonCurrent { get; set; }

        public override TaskResults RunSpecific()
        {
            throw new NotImplementedException();
        }
    }
}
