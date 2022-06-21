using InstrumentControl;
using InstrumentControl.Tasks;
using MassSpectrometry;
using MathNet.Numerics.Distributions;
using NUnit.Framework;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Tests
{
    public static class TestSpectrumAveragingTask
    {

		// TESTING IDEA: take scans, group them into five, send them through with different parametes
		// and run a classic search on teh resulting composit scans. Print a txt file that has the 
		// RejectionType, WeightingType, (paramertes of weighting if applicable), PSMs, peptides, proteins
		// also save the psmtsv from the searches with identifying names.
		// This should allow a simple way to optimize parameters

		// Used to initialte the main method of the class, will need to turn into an actual test in the future
		[Test]
		public static void TestSpectrumBinning()
		{
			string outputPath = @"C:\Users\Nic\Desktop\OuputFolder\InstrumentControl\TestingSpectraAveraging";
			string filepath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"DataFiles\TDYeastFractionMS1.mzML");
			List<MsDataScan> scans = MS1DatabaseParser.LoadAllScansFromFile(filepath).Where(p => p.MsnOrder == 1).ToList();
			List<MsDataScan> reducedScans = scans.GetRange(400, 5);
			SpectrumAveragingTask averagingTask = new(TaskType.SpectrumAveraging);
			List<MzSpectrum> compositeSpectra = new();
			int spectraToSend = 5;

			if (false)
            {
				// average all scans in groups of five from the yeast fraction
				for (int i = 0; i < scans.Count; i += spectraToSend)
				{
					ISpectraProcesor.SetData(scans.GetRange(i, spectraToSend));
					averagingTask.Run();
					compositeSpectra.Add(ISpectraAverager.CompositeSpectrum);
				}
			}

			// print each of the scans as png
			string path;
            foreach (var scan in reducedScans)
            {
				// convert to model and export as png
				path = Path.Combine(outputPath, "raw_scan" + scan.OneBasedScanNumber + ".png");
				PlotModel plot = SpectrumViewer.PlotMSDataScan(scan);
				SpectrumViewer.WritePlotToPng(plot, path);

				// export the mass spectra as txt
				string txtpath = Path.Combine(outputPath, "raw_scan" + scan.OneBasedScanNumber + ".txt");
				StringBuilder builder = new();
				for (int i = 0; i < scan.MassSpectrum.XArray.Length; i++)
				{
					builder.Append(scan.MassSpectrum.XArray[i] + ":" + scan.MassSpectrum.YArray[i] + "\n");
				}
				File.WriteAllText(txtpath, builder.ToString());
			}

			// average and print the average as png
			path = Path.Combine(outputPath, "CompositeSpectrum.png");
			ISpectraProcesor.SetData(reducedScans);
			averagingTask.Run();
			PlotModel compositePlot = SpectrumViewer.CreatePlotModel(ISpectraAverager.CompositeSpectrum);
			SpectrumViewer.WritePlotToPng(compositePlot, path);

			MzSpectrum compositeSpectrum = ISpectraAverager.CompositeSpectrum;
			path = Path.Combine(outputPath, "CompositeSpectra.txt");
			StringBuilder sb = new();
            for (int i = 0; i < compositeSpectrum.XArray.Length; i++)
            {
				sb.Append(compositeSpectrum.XArray[i] + ":" + compositeSpectrum.YArray[i] + "\n");
            }
			File.WriteAllText(path, sb.ToString());

			

			SpectrumAveragingTask.ResetValues();
		}

		[Test]
		public static void TestClippingFunctions()
        {
			#region Min Max Clipping

			double[] test = { 10, 9, 8, 7, 6, 5 };
			double[] expected = { 9, 8, 7, 6 };
			double[] minMaxClipped = SpectrumAveragingTask.MinMaxClipping(test);
			Assert.That(minMaxClipped, Is.EqualTo(expected));

			#endregion

			#region Percentile Clipping

			test = new double[] { 100, 95, 90, 80, 70, 60, 50, 40, 30, 20, 10, 5, 0 };
			expected = new double[] { 90, 80, 70, 60, 50, 40, 30, 20, 10 };
			double[] percentileClipped = SpectrumAveragingTask.PercentileClipping(test, 0.9);
			Assert.That(percentileClipped, Is.EqualTo(expected));

			test = new double[] { 100, 80, 60, 50, 40, 30, 20, 10, 0 };
			expected = new double[] { 60, 50, 40, 30, 20, 10 };
			percentileClipped = SpectrumAveragingTask.PercentileClipping(test, 0.9);
			Assert.That(percentileClipped, Is.EqualTo(expected));

			#endregion

			#region Sigma Clipping

			test = new double[] { 100, 80, 60, 50, 40, 30, 20, 10, 0 };
			double[] sigmaClipped = SpectrumAveragingTask.SigmaClipping(test, 1.5, 1.5);
			expected = new double[] { 50, 40, 30, 20, 10 };
			Assert.That(sigmaClipped, Is.EqualTo(expected));

			test = new double[] { 100, 95, 90, 80, 70, 60, 50, 40, 30, 20, 10, 5, 0 };
			sigmaClipped = SpectrumAveragingTask.SigmaClipping(test, 1, 1);
			expected = new double[] { 50 };
			Assert.That(sigmaClipped, Is.EqualTo(expected));

			sigmaClipped = SpectrumAveragingTask.SigmaClipping(test, 1.3, 1.3);
			expected = new double[] { 60, 50, 40 };
			Assert.That(sigmaClipped, Is.EqualTo(expected));

			sigmaClipped = SpectrumAveragingTask.SigmaClipping(test, 1.5, 1.5);
			expected = new double[] { 100, 95, 90, 80, 70, 60, 50, 40, 30, 20, 10, 5, 0 };
			Assert.That(sigmaClipped, Is.EqualTo(expected));

			#endregion

			#region Windsorized Sigma Clipping

			test = new double[] { 100, 80, 60, 50, 40, 30, 20, 10, 0 };
			double[] windsorizedSigmaClipped = SpectrumAveragingTask.WinsorizedSigmaClipping(test, 1.5, 1.5);
			expected = new double[] { 60, 50, 40, 30, 20, 10, 0 };
			Assert.That(windsorizedSigmaClipped, Is.EqualTo(expected));

			test = new double[] { 100, 95, 90, 80, 70, 60, 50, 40, 30, 20, 10, 5, 0 };
			windsorizedSigmaClipped = SpectrumAveragingTask.WinsorizedSigmaClipping(test, 1, 1);
			expected = new double[] { 60, 50, 40 };
			Assert.That(windsorizedSigmaClipped, Is.EqualTo(expected));

			windsorizedSigmaClipped = SpectrumAveragingTask.WinsorizedSigmaClipping(test, 1.3, 1.3);
			expected = new double[] { 60, 50, 40 };
			Assert.That(windsorizedSigmaClipped, Is.EqualTo(expected));

			windsorizedSigmaClipped = SpectrumAveragingTask.WinsorizedSigmaClipping(test, 1.5, 1.5);
			expected = new double[] { 100, 95, 90, 80, 70, 60, 50, 40, 30, 20, 10, 5, 0 };
			Assert.That(windsorizedSigmaClipped, Is.EqualTo(expected));

			#endregion

			#region Averaged Sigma Clipping

			test = new double[] { 100, 80, 60, 50, 40, 30, 20, 10, 0 };
			double[] averagedSigmaClipping = SpectrumAveragingTask.AveragedSigmaClipping(test, 3, 3);
			expected = new double[] { 80, 60, 50, 40, 30, 20, 10, 0 };
			Assert.That(averagedSigmaClipping, Is.EqualTo(expected));

			test = new double[] { 100, 95, 90, 80, 70, 60, 50, 40, 30, 20, 10, 5, 0 };
			averagedSigmaClipping = SpectrumAveragingTask.AveragedSigmaClipping(test, 1, 1);
			expected = new double[] { 70, 60, 50, 40, 30, };
			Assert.That(averagedSigmaClipping, Is.EqualTo(expected));

			averagedSigmaClipping = SpectrumAveragingTask.AveragedSigmaClipping(test, 1.3, 1.3);
			expected = new double[] { 80, 70, 60, 50, 40, 30, 20 };
			Assert.That(averagedSigmaClipping, Is.EqualTo(expected));

			averagedSigmaClipping = SpectrumAveragingTask.AveragedSigmaClipping(test, 1.5, 1.5);
			expected = new double[] { 80, 70, 60, 50, 40, 30, 20 };
			Assert.That(averagedSigmaClipping, Is.EqualTo(expected));

			#endregion

		}

		[Test]
		public static void TestWeightedAverage()
        {
			double[] values = new double[] { 10, 0 };
			double[] weights = new double[] { 8, 2 };
			double average = SpectrumAveragingTask.MergePeakValuesToAverage(values, weights);
			Assert.That(average, Is.EqualTo(8));

			values = new double[] { 10, 2, 0 };
			weights = new double[] { 9, 1, 0 };
			average = SpectrumAveragingTask.MergePeakValuesToAverage(values, weights);
			Assert.That(Math.Round(average,4), Is.EqualTo(9.200));
        }

		[Test]
		public static void TestCalculatingWeights()
        {
			double[] test = new double[] { 10, 8, 6, 5, 4, 3, 2, 1 };
			double[] weights = new double[test.Length];
			SpectrumAveragingTask.WeightByNormalDistribution(test, ref weights);
			double weightedAverage = SpectrumAveragingTask.MergePeakValuesToAverage(test, weights);
			Assert.That(Math.Round(weightedAverage,4), Is.EqualTo(4.5460));

			weights = new double[test.Length];
			SpectrumAveragingTask.WeightByCauchyDistribution(test, ref weights);
			weightedAverage = SpectrumAveragingTask.MergePeakValuesToAverage(test, weights);
			Assert.That(Math.Round(weightedAverage, 4), Is.EqualTo(4.6411));

			weights = new double[test.Length];
			SpectrumAveragingTask.WeightByPoissonDistribution(test, ref weights);
			weightedAverage = SpectrumAveragingTask.MergePeakValuesToAverage(test, weights);
			Assert.That(Math.Round(weightedAverage, 4), Is.EqualTo(4.2679));

			weights = new double[test.Length];
			SpectrumAveragingTask.WeightByGammaDistribution(test, ref weights);
			weightedAverage = SpectrumAveragingTask.MergePeakValuesToAverage(test, weights);
			Assert.That(Math.Round(weightedAverage, 4), Is.EqualTo(4.1638));
        }

		[Test]
		public static void TestSetValuesAndRejectOutliersSwitch()
        {
			SpectrumAveragingTask.ResetValues();
			Assert.That(SpectrumAveragingTask.RejectionType == RejectionType.NoRejection);
			Assert.That(0.9, Is.EqualTo(SpectrumAveragingTask.Percentile));
			Assert.That(1.3, Is.EqualTo(SpectrumAveragingTask.MinSigmaValue));
			Assert.That(1.3, Is.EqualTo(SpectrumAveragingTask.MaxSigmaValue));

			SpectrumAveragingTask.SetValues(RejectionType.MinMaxClipping, WeightingType.NoWeight, .8, 2, 4);
			Assert.That(SpectrumAveragingTask.RejectionType == RejectionType.MinMaxClipping);
			Assert.That(0.8, Is.EqualTo(SpectrumAveragingTask.Percentile));
			Assert.That(2, Is.EqualTo(SpectrumAveragingTask.MinSigmaValue));
			Assert.That(4, Is.EqualTo(SpectrumAveragingTask.MaxSigmaValue));

			SpectrumAveragingTask.ResetValues();
			double[] test = new double[] { 10, 8, 6, 5, 4, 2, 0 };
			double[] output = SpectrumAveragingTask.RejectOutliers(test);
			double[] expected = new double[] { 10, 8, 6, 5, 4, 2, 0 };
			Assert.That(output, Is.EqualTo(expected));

			SpectrumAveragingTask.SetValues(rejectionType: RejectionType.MinMaxClipping);
			Assert.That(SpectrumAveragingTask.RejectionType == RejectionType.MinMaxClipping);
			output = SpectrumAveragingTask.RejectOutliers(test);
			expected = new double[] { 8, 6, 5, 4, 2 };
			Assert.That(output, Is.EqualTo(expected));

			SpectrumAveragingTask.SetValues(rejectionType: RejectionType.PercentileClipping);
			Assert.That(SpectrumAveragingTask.RejectionType == RejectionType.PercentileClipping);
			SpectrumAveragingTask.SetValues(rejectionType: RejectionType.SigmaClipping);
			Assert.That(SpectrumAveragingTask.RejectionType == RejectionType.SigmaClipping);
			SpectrumAveragingTask.SetValues(rejectionType: RejectionType.WinsorizedSigmaClipping);
			Assert.That(SpectrumAveragingTask.RejectionType == RejectionType.WinsorizedSigmaClipping);
			SpectrumAveragingTask.SetValues(rejectionType: RejectionType.AveragedSigmaClipping);
			Assert.That(SpectrumAveragingTask.RejectionType == RejectionType.AveragedSigmaClipping);
			SpectrumAveragingTask.ResetValues();
		}

	}
}
