using MassSpectrometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;

namespace InstrumentControl
{
    public class WholeChargeEnvelopeFragmentationApplication : Application
    {
        
        public override Queue<InstrumentControlTask> TaskQueue { get; set; } = new Queue<InstrumentControlTask>();
        public override List<InstrumentControlTask> TaskList { get; set; } = new List<InstrumentControlTask>();

        public InstrumentControlTask AveragingTask { get; set; }

        public WholeChargeEnvelopeFragmentationApplication() : base(MyApplication.WholeChargeStateEnvelopeFragmentation)
        {
            // set spectra preprocessing tasks
            TaskList.Add(new NormalizationTask(TaskType.Normalization));
            TaskList.Add(new StandardizationTask(TaskType.Standardization));

            // set spectra averaging tasks
            AveragingTask = new SpectrumAveragingTask(TaskType.SpectrumAveraging);
            TaskList.Add(AveragingTask);

            // set scan returning tasks

            EnqueueSelectTasks();
        }

        /// <summary>
        /// Event that is fired when the ScanQueue reaches its threshold and this is the selected application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Data contains a list of IMsScans to be processed</param>
        /// <exception cref="NotImplementedException"></exception>
        public override void ProcessScans(object? sender, ThresholdReachedEventArgs e)
        {
            // pull out relevant scan data
            ISpectraProcesor.ProccessDataQueue(e.Data);

            // perform data handling tasks
            while (TaskQueue.Count > 0)
            {
                TaskQueue.Dequeue().Run();
            }

            MzSpectrum compositeSpectra = ISpectraAverager.CompositeSpectrum;

            // perform scan creation tasks

            // get envelopes

            // fragment envelopes

            // reset scans for next run
            EnqueueSelectTasks();
        }


        public void TestItOut(List<MzSpectrum> spectra, double[] tic)
        {
            // pull out relevant scan data
            int scans = spectra.Count;
            double[][] xArrays = new double[scans][];
            double[][] yArrays = new double[scans][];
            double[] totalIonCurrents = new double[scans];
            for (int i = 0; i < scans; i++)
            {
                xArrays[i] = spectra[i].XArray;
                yArrays[i] = spectra[i].YArray;
                totalIonCurrents[i] = tic[i];
            }

            // perform data handling tasks
            ISpectraProcesor.SetData(xArrays, yArrays, totalIonCurrents);
            while (TaskQueue.Count > 0)
            {
                TaskQueue.Dequeue().Run();
            }


            MzSpectrum compositeSpectra = ISpectraAverager.CompositeSpectrum;
        }
    }
}
