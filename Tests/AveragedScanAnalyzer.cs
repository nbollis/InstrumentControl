using Averaging;
using MassSpectrometry;
using OxyPlot;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstrumentControl;
using MzLibUtil;
using UsefulProteomicsDatabases;

namespace Tests
{
    public class AveragedScanAnalyzer
    {
        public string Header { get; set; }

        #region Header Components

        public string Resolution { get { return Header.Split("_")[0]; } }
        public string NumberOfScans { get { return Header.Split("_")[1]; } }
        public string RejectionType { get { return Header.Split("_")[2]; } }
        public string WeightingType { get { return Header.Split("_")[3]; } }
        public string BinSize { get { return Header.Split("_")[4]; } }

        #endregion

        public MzSpectrum CompositeSpectrum { get; set; }
        public PlotModel? PlotModel { get; set; }
        public int Score { get; set; }

        public AveragedScanAnalyzer(string header, MzSpectrum compositeSpectrum)
        {
            Header = header;
            CompositeSpectrum = compositeSpectrum;
            PlotModel = SpectrumViewer.CreatePlotModel(CompositeSpectrum);
            Score = 0;
        }

        // for initializing the object from an outputted txtFile of a specific name formatting
        public AveragedScanAnalyzer(string filepath, bool convertToRelative = false)
        {
            Header = Path.GetFileNameWithoutExtension(filepath);
            string[] lines = File.ReadAllLines(filepath);
            double[] xValues = new double[lines.Length];
            double[] yValues = new double[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                string[] values = lines[i].Split("\t");
                xValues[i] = double.Parse(values[0]);
                yValues[i] = double.Parse(values[1]);
            }

            if (convertToRelative)
            {
                double yMax = yValues.Max();
                yValues = yValues.Select(p => p / yMax).ToArray();
            }

            CompositeSpectrum = new MzSpectrum(xValues, yValues, true);
            Score = 0;
        }


        public void ScoreBasedOnRelativeAbundanceOfPeaksWithinTolerance(double[] targetValues, double ppmTolerance, double targetAbundance)
        {
            double yMax = CompositeSpectrum.YArray.Max();
            bool relative = CompositeSpectrum.YArray.Sum() > 1 ? false : true;
            PpmTolerance tolerance = new(ppmTolerance);

            foreach (var target in targetValues)
            {
                foreach (var potentialHits in CompositeSpectrum.XArray.Where(p => tolerance.Within(p, target)))
                {
                    var test = tolerance.GetRange(potentialHits);
                    int index = Array.IndexOf(CompositeSpectrum.XArray, potentialHits);
                    if (relative ? CompositeSpectrum.YArray[index] > targetAbundance : CompositeSpectrum.YArray[index] / yMax > targetAbundance)
                    {
                        Score++;
                    }
                }
            }
        }

        public void ScoreBasedOnRelavantPeaksAboveDeviation(double[] targetValues, double ppmTolerance, double targetSigmasBelowMean)
        {
            double yMax = CompositeSpectrum.YArray.Max();
            double mean = CompositeSpectrum.YArray.Average();
            double deviation = SpectrumAveraging.CalculateStandardDeviation(CompositeSpectrum.YArray);
            bool relative = CompositeSpectrum.YArray.Sum() > 1 ? false : true;
            PpmTolerance tolerance = new(ppmTolerance);
        }


        public void PrintAveragedScanAnalyzerAsTxtOfSpectrum(string outpath, bool relativeAbundance)
        {
            using (StreamWriter writer = new StreamWriter(File.Create(outpath)))
            {
                if (relativeAbundance)
                {
                    double ymax = CompositeSpectrum.YArray.Max();
                    for (int i = 0; i < CompositeSpectrum.XArray.Length; i++)
                    {
                        writer.WriteLine(CompositeSpectrum.XArray[i] + "\t" + (CompositeSpectrum.YArray[i] / ymax));
                    }
                }
                else
                {
                    for (int i = 0; i < CompositeSpectrum.XArray.Length; i++)
                    {
                        writer.WriteLine(CompositeSpectrum.XArray[i] + "\t" + CompositeSpectrum.YArray[i]);
                    }
                }
            }
        }

        public bool TrySavePlotModelAsSvg(string directoryPath, out string? error)
        {
            string outpath = Path.Combine(directoryPath, Header + ".svg");
            try
            {
                SpectrumViewer.WritePlotToSvg(PlotModel, outpath);
                error = null;
                return true;
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }            
        }

        public static List<AveragedScanAnalyzer> AverageMultiScanWithManyOptions(List<SpectrumAveragingOptions> allOptions, MultiScanDataObject data, string resolution)
        {
            SpectrumAveragingTask averagingTask = new SpectrumAveragingTask();
            List<AveragedScanAnalyzer> analysis = new List<AveragedScanAnalyzer>();
            foreach (var option in allOptions)
            {
                averagingTask.RunSpecific(option, data);
                string header = $"{resolution}_{data.ScansToProcess}Scans_" +
                    $"{option.RejectionType}_{option.WeightingType}_{option.BinSize}";
                analysis.Add(new AveragedScanAnalyzer(header, data.CompositeSpectrum));
                data.CompositeSpectrum = null;
            }
            return analysis;
        }

        public static List<AveragedScanAnalyzer> LoadMany(string[] files)
        {
            List<AveragedScanAnalyzer> averagedScanAnalyzers = new();
            foreach (var file in files)
            {
                var averagedScanAnalyzer = new AveragedScanAnalyzer(file);
                averagedScanAnalyzers.Add(averagedScanAnalyzer);
            }
            return averagedScanAnalyzers;
        }

    }

}
