using InstrumentControl.Tasks.DataHandlerTasks;
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
        
        public Queue<PreProcessingTask> DataHandlerTasks = new Queue<PreProcessingTask>();
        public PreProcessingData DataHandlerTaskResults { get; set; }
        public AveragingTask AveragingTask { get; set; }

        public WholeChargeEnvelopeFragmentationApplication() : base(MyApplication.WholeChargeStateEnvelopeFragmentation)
        {
            // set data preprocessing tasks
            PreProcessingData results = new();
            DataHandlerTasks.Enqueue(new NormalizationTask(TaskType.Normalization, ref results));
            DataHandlerTasks.Enqueue(new StandardizationTask(TaskType.Standardization, ref results));

            // set data averaging tasks
            AveragingTask = new SpectrumAveragingTask(TaskType.SpectrumAveraging, ref results);

            // set scan returning tasks

            // save reference as instance data so it doesnt get garbage collected
            DataHandlerTaskResults = results;
        }

        /// <summary>
        /// Event that is fired when the ScanQueue reaches its threshold and this is the selected application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Data contains a list of IMsScans to be processed</param>
        /// <exception cref="NotImplementedException"></exception>
        public override void ProcessScans(object? sender, ThresholdReachedEventArgs e)
        {

            // TODO: pull metadata async(List<IMsScan> scans)
            
            // pull out relevant scan data
            int scans = e.Data.Count;
            double[][] xArrays = new double[scans][];
            double[][] yArrays = new double[scans][];
            double[] totalIonCurrents = new double[scans];
            for (int i = 0; i < scans; i++)
            {
                xArrays[i] = e.Data[i].Centroids.Select(c => c.Mz).ToArray();
                yArrays[i] = e.Data[i].Centroids.Select(c => c.Intensity).ToArray();
                totalIonCurrents[i] = double.Parse(e.Data[i].Header["Total Ion Current"]); 
            }

            // perform data handling tasks
            DataHandlerTaskResults.SetData(xArrays, yArrays, totalIonCurrents);
            while (DataHandlerTasks.Count > 0)
            {
                DataHandlerTasks.Dequeue().Run();
            }

            // perform translator task
            AveragingTask.Run();
            MzSpectrum compositeSpectra = AveragingTask.CompositeSpectrum;

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
            DataHandlerTaskResults.SetData(xArrays, yArrays, totalIonCurrents);
            while (DataHandlerTasks.Count > 0)
            {
                DataHandlerTasks.Dequeue().Run();
            }

            AveragingTask.Run();
            MzSpectrum compositeSpectra = AveragingTask.CompositeSpectrum;
        }
    }
}
