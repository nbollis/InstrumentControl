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
        
        public Queue<DataHandlerTask> DataHandlerTasks = new Queue<DataHandlerTask>();
        public DataHandlerTaskResult DataHandlerTaskResults { get; set; }

        public WholeChargeEnvelopeFragmentationApplication() : base(MyApplication.WholeChargeStateEnvelopeFragmentation)
        {
            DataHandlerTaskResult results = new();
            DataHandlerTasks.Enqueue(new NormalizationTask(TaskType.Normalization, ref results));
            DataHandlerTasks.Enqueue(new AlignmentTask(TaskType.Normalization, ref results));
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

            // pull metadata async(List<IMsScan> scans)
            TaskResults metaData;
            
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

            // perform scan creation tasks

            //TaskResults combinedSpectra = new SpectrumAveragingTask(spectra, totalIonCurrents).RunSpecific();
            //TaskResults combinedSpectra = new SpectrumAveragingTask(xArrays, yArrays, totalIonCurrents).RunSpecific();

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
        }
    }
}
