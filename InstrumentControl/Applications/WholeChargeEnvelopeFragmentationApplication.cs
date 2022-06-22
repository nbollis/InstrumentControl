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
        public override List<InstrumentControlTask> TaskList { get; set; } = new List<InstrumentControlTask>();

        public WholeChargeEnvelopeFragmentationApplication() : base(MyApplication.WholeChargeStateEnvelopeFragmentation)
        {
            // set spectra preprocessing tasks
            TaskList.Add(new NormalizationTask(TaskType.Normalization));
            TaskList.Add(new StandardizationTask(TaskType.Standardization));
            TaskList.Add(new SpectrumAveragingTask(TaskType.SpectrumAveraging));

            // set scan returning tasks

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

            foreach (var task in TaskList)
            {
                task.Run();
            }


            MzSpectrum compositeSpectra = ISpectraAverager.CompositeSpectrum;

            // perform scan creation tasks

            // get envelopes

            // fragment envelopes


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
            foreach (var task in TaskList)
            {
                task.Run();
            }


            MzSpectrum compositeSpectra = ISpectraAverager.CompositeSpectrum;
        }
    }
}
