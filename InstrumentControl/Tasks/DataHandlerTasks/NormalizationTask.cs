using InstrumentControl.Tasks.ScanProducerTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl.Tasks.DataHandlerTasks
{
    public class NormalizationTask : DataHandlerTask
    {
        public NormalizationTask(TaskType taskType, ref DataHandlerTaskResult spectraData) : base(taskType, ref spectraData)
        {
            SpectraData = spectraData;
        }

        public override DataHandlerTaskResult SpectraData { get; set; }

        public override void RunSpecific()
        {
            double maxCurrent = SpectraData.TotalIonCurrent.Max();
            var AverageIntensity = new double[SpectraData.TotalIonCurrent.Length];
            var AverageIntensityAfter = new double[SpectraData.TotalIonCurrent.Length];
            for (int i = 0; i < SpectraData.TotalIonCurrent.Length; i++)
            {
                double current = SpectraData.TotalIonCurrent[i];
                AverageIntensity[i] = SpectraData.YArrays[i].Average();
                for (int j = 0; j < SpectraData.YArrays[i].Length; j++)
                {
                    SpectraData.YArrays[i][j] = SpectraData.YArrays[i][j] / current * maxCurrent;
                }
                AverageIntensityAfter[i] = SpectraData.YArrays[i].Average();
            }
        }
    }
}
