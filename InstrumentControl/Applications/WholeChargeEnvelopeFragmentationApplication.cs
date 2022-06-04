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
        


        public WholeChargeEnvelopeFragmentationApplication() : base(MyApplication.WholeChargeStateEnvelopeFragmentation)
        {

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
            
            // average scans
            int scans = e.Data.Count;
            MzSpectrum[] spectra = new MzSpectrum[scans];
            double[][] xArrays = new double[scans][];
            double[][] yArrays = new double[scans][];
            double[] totalIonCurrents = new double[scans];
            for (int i = 0; i < scans; i++)
            {
                xArrays[i] = e.Data[i].Centroids.Select(c => c.Mz).ToArray();
                yArrays[i] = e.Data[i].Centroids.Select(c => c.Intensity).ToArray();
                totalIonCurrents[i] = double.Parse(e.Data[i].Header["Total Ion Current"]); 
            }
            NormalizationTask normalization = new NormalizationTask(TaskType.Normalization);
            normalization.XArrays = xArrays;
            normalization.YArrays = yArrays;
            normalization.TotalIonCurrent = totalIonCurrents;
            ISpectraManipulator normResult = (ISpectraManipulator)normalization.RunSpecific();
            AlignmentTask alignment = new AlignmentTask(TaskType.Alignment, normResult);
            ISpectraManipulator alignmentResult = (ISpectraManipulator)alignment.RunSpecific();


            //TaskResults combinedSpectra = new SpectrumAveragingTask(spectra, totalIonCurrents).RunSpecific();
            TaskResults combinedSpectra = new SpectrumAveragingTask(xArrays, yArrays, totalIonCurrents).RunSpecific();

            // get envelopes


            // fragment envelopes

        }
    }
}
