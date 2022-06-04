using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    public class NormalizationTask : InstrumentControlTask, ISpectraManipulator
    {
        public NormalizationTask(TaskType taskType) : base(taskType)
        {

        }

        public double[][] XArrays {get; set;}
        public double[][] YArrays { get; set; }
        public double[] TotalIonCurrent { get; set; }

        public override TaskResults RunSpecific()
        {
            double maxCurrent = TotalIonCurrent.Max();
            var AverageIntensity = new double[TotalIonCurrent.Length];
            var AverageIntensityAfter = new double[TotalIonCurrent.Length];
            for (int i = 0; i < TotalIonCurrent.Length; i++)
            {
                double current = TotalIonCurrent[i];
                AverageIntensity[i] = YArrays[i].Average();
                for (int j = 0; j < YArrays[i].Length; j++)
                {
                    YArrays[i][j] = YArrays[i][j] / current * maxCurrent;
                }
                AverageIntensityAfter[i] = YArrays[i].Average();
            }
            TaskResults result = new NormalizationTaskResult(this);
            return result;
        }
    }
}
