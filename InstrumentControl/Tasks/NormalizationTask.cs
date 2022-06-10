using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    public class NormalizationTask : InstrumentControlTask, ISpectraProcesor
    {
        public NormalizationTask(TaskType taskType) : base(taskType)
        {

        }

        //public override PreProcessingData SpectraData { get; set; }

        // TODO: Actually normalize the spectra to TIC, instead of just filling space
        public override void RunSpecific()
        {
            double maxCurrent = ISpectraProcesor.TotalIonCurrent.Max();

            // for testing
            var AverageIntensity = new double[ISpectraProcesor.TotalIonCurrent.Length];
            var AverageIntensityAfter = new double[ISpectraProcesor.TotalIonCurrent.Length];

            for (int i = 0; i < ISpectraProcesor.ScansToProcess; i++)
            {
                double current = ISpectraProcesor.TotalIonCurrent[i];
                AverageIntensity[i] = ISpectraProcesor.YArrays[i].Average();
                for (int j = 0; j < ISpectraProcesor.YArrays[i].Length; j++)
                {
                    ISpectraProcesor.YArrays[i][j] = ISpectraProcesor.YArrays[i][j] / current * maxCurrent;
                }

                // for testing
                AverageIntensityAfter[i] = ISpectraProcesor.YArrays[i].Average();
            }

        }

    }
}
