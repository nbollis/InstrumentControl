using MassSpectrometry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    public class TestApplicationToSaveAndAverageData : Application
    {
        public override List<InstrumentControlTask> TaskList { get; set; } = new List<InstrumentControlTask>();


        public TestApplicationToSaveAndAverageData() : base(MyApplication.TestApplicationToSaveAndAverageData)
        {
            TaskList.Add(new NormalizationTask(TaskType.Normalization));
            TaskList.Add(new StandardizationTask(TaskType.Standardization));
            TaskList.Add(new SpectrumAveragingTask(TaskType.SpectrumAveraging));
        }

        public override void ProcessScans(object? sender, ThresholdReachedEventArgs e)
        {
            ISpectraProcesor.ProccessDataQueue(e.Data);

            foreach (var task in TaskList)
            {
                task.Run();
            }
            string MzSpectrumFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"AveragedMzSpectrumStrings.txt");
            MzSpectrum compositeSpectra = ISpectraAverager.CompositeSpectrum;
            InstrumentControlIO.SerializeAndAppend<MzSpectrum>(compositeSpectra, MzSpectrumFilePath);
        }

    }
}
