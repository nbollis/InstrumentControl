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
        public double Resolution { get; set; }
        public double NumberOfScans { get; set; }
        public ISpectrumAveragingOptions Options { get; set; }
        public MzSpectrum CompositeSpectrum { get; set; }
        public PlotModel? PlotModel { get; set; }
        public int Score { get; set; }
        public double? PpmAllowed { get; set; }
        public double SigmaAllowed { get; set; }
        public double ScoreIntensityTarget { get; set; }
        public double SignalToNoise { get; set; }

        public double AverageNonZeroYValue
        {
            get { return CompositeSpectrum.YArray.Where(p => p != 0).Average(); }
        }

        public double NonZeroStandardDeviationInY
        {
            get { return SpectrumAveraging.CalculateStandardDeviation(CompositeSpectrum.YArray.Where(p => p != 0)); }
        }

        #region Constructors

        public AveragedScanAnalyzer(string header, MzSpectrum compositeSpectrum)
        {
            Header = header;
            CompositeSpectrum = compositeSpectrum;
            PlotModel = SpectrumViewer.CreatePlotModel(CompositeSpectrum);
            Score = 0;
        }

        // use this one!
        public AveragedScanAnalyzer(ISpectrumAveragingOptions options, MzSpectrum compositeSpectrum, MultiScanDataObject data, double resolution)
        {
            NumberOfScans = data.ScansToProcess;
            Resolution = resolution;
            Options = options;
            CompositeSpectrum = data.CompositeSpectrum;
            Header = $"{Resolution}_{data.ScansToProcess}Scans_{Options.RejectionType}_{Options.WeightingType}_{Options.BinSize}";
            Score = 0;
        }

        // for initializing the object from an outputted txtFile of a specific name formatting
        public AveragedScanAnalyzer(string filepath, bool convertToRelative = false)
        {
            Header = Path.GetFileNameWithoutExtension(filepath);
            string[] splits = Header.Split('_');
            Options = new SpectrumAveragingOptions();
            //if (splits.Length > 2 && !filepath.Contains("-qb"))
            //{
            //    Resolution = double.Parse(splits[0]);
            //    NumberOfScans = double.Parse(splits[1].Split('S')[0]);
            //    Options.RejectionType = (RejectionType)Enum.Parse(typeof(RejectionType), splits[2]);
            //    Options.WeightingType = (WeightingType)Enum.Parse(typeof(WeightingType), splits[3]);
            //    Options.BinSize = double.Parse(splits[4]);
            //}

            string[] lines = File.ReadAllLines(filepath);
            string[] firstline = lines[0].Split("\t");
            if (firstline.Length > 2)
            {
                ScoreIntensityTarget = double.Parse(firstline[2]);
                PpmAllowed = double.Parse(firstline[3]);
                SigmaAllowed = double.Parse(firstline[4]);
                Score = int.Parse(firstline[5]);
            }

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
            
        }

        #endregion

        #region Mutators
        public void ScoreBasedOnRelativeAbundanceOfPeaksWithinTolerance(double[] targetValues, double ppmTolerance, double targetAbundance)
        {
            Score = 0;
            double yMax = CompositeSpectrum.YArray.Max();
            bool relative = CompositeSpectrum.YArray.Sum() > 1 ? false : true;
            PpmTolerance tolerance = new(ppmTolerance);

            foreach (var target in targetValues)
            {
                foreach (var potentialHits in CompositeSpectrum.XArray.Where(p => tolerance.Within(p, target)))
                {
                    int index = Array.IndexOf(CompositeSpectrum.XArray, potentialHits);
                    if (relative ? CompositeSpectrum.YArray[index] > targetAbundance : CompositeSpectrum.YArray[index] / yMax > targetAbundance)
                    {
                        Score++;
                        break;
                    }
                }
            }
        }

        public void ScoreBasedOnRelavantPeaksAboveDeviation(double[] targetValues, double ppmTolerance, double targetSigmasBelowMean)
        {
            Score = 0;
            PpmAllowed = ppmTolerance;
            SigmaAllowed = targetSigmasBelowMean;
            ScoreIntensityTarget = AverageNonZeroYValue - (NonZeroStandardDeviationInY * targetSigmasBelowMean);
            bool relative = CompositeSpectrum.YArray.Sum() > 1 ? false : true;
            PpmTolerance tolerance = new(ppmTolerance);
            //var test = CompositeSpectrum.YArray.Where(p => p != 0).ToList();
            //double test2 = SpectrumAveraging.CalculateStandardDeviation(test);

            foreach (var target in targetValues)
            {
                //var range = tolerance.GetRange(target);
                var potentialHits = CompositeSpectrum.XArray.Where(p => tolerance.Within(p, target));
                foreach (var potentialHit in potentialHits)
                {
                    int index = Array.IndexOf(CompositeSpectrum.XArray, potentialHit);
                    double intensity = CompositeSpectrum.YArray[index];
                    if (intensity > ScoreIntensityTarget)
                    {
                        Score++;
                        break;
                    }
                }
            }
        }

        public void ScoreAndCalculateSignalToNoise(double[] targetValues, double ppmTolerance, double targetSigmasBelowMean)
        {
            Score = 0;
            PpmAllowed = ppmTolerance;
            SigmaAllowed = targetSigmasBelowMean;
            ScoreIntensityTarget = AverageNonZeroYValue - (NonZeroStandardDeviationInY * targetSigmasBelowMean);
            bool relative = CompositeSpectrum.YArray.Sum() > 1 ? false : true;
            PpmTolerance tolerance = new(ppmTolerance);
            List<double> hitPeaks = new();

            foreach (var target in targetValues.Where(p => p >= 1000 && p <= 1400))
            {
                //var range = tolerance.GetRange(target);
                var potentialHits = CompositeSpectrum.XArray.Where(p => tolerance.Within(p, target));
                foreach (var potentialHit in potentialHits)
                {
                    int index = Array.IndexOf(CompositeSpectrum.XArray, potentialHit);
                    double intensity = CompositeSpectrum.YArray[index];
                    if (intensity > ScoreIntensityTarget)
                    {
                        hitPeaks.Add(CompositeSpectrum.YArray[index]);
                        Score++;
                        break;
                    }
                }
            }
            if (hitPeaks.Count > 0)
            {
                double signalValue = hitPeaks.Average();
                double standardDeviation = SpectrumAveraging.CalculateStandardDeviation(CompositeSpectrum.YArray/*.Where(p => p != 0)*/);
                SignalToNoise = signalValue / standardDeviation;
            }
            else
                SignalToNoise = 0;
        }

        #endregion

        #region IO
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

        public void PrintAveragedScanAsTxtWithScore(string outpath, bool relativeAbundance)
        {
            using (StreamWriter writer = new StreamWriter(File.Create(outpath)))
            {
                if (relativeAbundance)
                {
                    double ymax = CompositeSpectrum.YArray.Max();
                    writer.WriteLine(CompositeSpectrum.XArray[0] + "\t" + CompositeSpectrum.YArray[0] / ymax + "\t" + ScoreIntensityTarget + "\t" + PpmAllowed + "\t" + SigmaAllowed + "\t" + Score);
                    for (int i = 1; i < CompositeSpectrum.XArray.Length; i++)
                    {
                        writer.WriteLine(CompositeSpectrum.XArray[i] + "\t" + (CompositeSpectrum.YArray[i] / ymax));
                    }
                }
                else
                {
                    writer.WriteLine(CompositeSpectrum.XArray[0] + "\t" + CompositeSpectrum.YArray[0] + "\t" + ScoreIntensityTarget + "\t" + PpmAllowed + "\t" + SigmaAllowed + "\t" + Score);
                    for (int i = 1; i < CompositeSpectrum.XArray.Length; i++)
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

        #endregion




        public static List<AveragedScanAnalyzer> AverageMultiScanWithManyOptions(List<SpectrumAveragingOptions> allOptions, MultiScanDataObject data, string resolution)
        {
            SpectrumAveragingTask averagingTask = new SpectrumAveragingTask();
            List<AveragedScanAnalyzer> analysis = new List<AveragedScanAnalyzer>();
            foreach (var option in allOptions)
            {
                averagingTask.RunSpecific(option, data);
                analysis.Add(new AveragedScanAnalyzer(option, data.CompositeSpectrum, data, double.Parse(resolution.Split('k')[0])));
                data.CompositeSpectrum = null;
            }
            return analysis;
        }

        public static void IterateThroughScoredTxtAndCreateSummary(string directoryPath, string basicOutPath)
        {
            List<string> entries = new();
            string delim = "\t";
            string header = "Resolution" + delim + "Scans" + delim + "Rejection" + delim + "Weighting" + delim + "BinSize"
                + delim + "MinSigma" + delim + "MaxSigma" + delim + "Percentile" + delim + "Score" + delim +
                "IntensityScoreTarget" + delim + "PpmAllowedInScore" + delim + "SigmaAllowedInScore";
            foreach (var file in Directory.GetFiles(directoryPath).Where(p => p.Contains(".txt")))
            {
                AveragedScanAnalyzer averagedScan = new(file, false);
                string entry = averagedScan.Resolution + delim + averagedScan.NumberOfScans + delim + averagedScan.Options.RejectionType + delim + 
                    averagedScan.Options.WeightingType + delim + averagedScan.Options.BinSize + delim + averagedScan.Options.MinSigmaValue + delim +
                    averagedScan.Options.MaxSigmaValue + delim + averagedScan.Options.Percentile + delim +  averagedScan.Score + delim + averagedScan.ScoreIntensityTarget 
                    + delim + averagedScan.PpmAllowed + delim +  averagedScan.SigmaAllowed;
                entries.Add(entry);
            }
            string outpath = Path.Combine(directoryPath, basicOutPath);
            OutputSummaries(entries, header, outpath);
        }

        public static void IterateThroughScoredAveragedScanAnalyzers(List<AveragedScanAnalyzer> scans, string outpath)
        {
            List<string> entries = new();
            string delim = "\t";
            string header = "Resolution" + delim + "Scans" + delim + "Rejection" + delim + "Weighting" + delim + "BinSize"
                + delim + "MinSigma" + delim + "MaxSigma" + delim + "Percentile" + delim + "Score" + delim +
                "IntensityScoreTarget" + delim + "PpmAllowedInScore" + delim + "SigmaAllowedInScore" + delim + "RelativeInstensityScoreTarget";
            foreach (var averagedScan in scans)
            {
                string entry = averagedScan.Resolution + delim + averagedScan.NumberOfScans + delim + averagedScan.Options.RejectionType + delim +
                    averagedScan.Options.WeightingType + delim + averagedScan.Options.BinSize + delim + averagedScan.Options.MinSigmaValue + delim +
                    averagedScan.Options.MaxSigmaValue + delim + averagedScan.Options.Percentile + delim + averagedScan.Score + delim + averagedScan.ScoreIntensityTarget
                    + delim + averagedScan.PpmAllowed + delim + averagedScan.SigmaAllowed + delim + averagedScan.ScoreIntensityTarget / averagedScan.CompositeSpectrum.YArray.Max();
                entries.Add(entry);
            }
            OutputSummaries(entries, header, outpath);
        }

        public static void OutputSummaries(List<string> entries, string header, string outpath)
        {
            using (StreamWriter writer = new(File.Create(outpath)))
            {
                writer.WriteLine(header);
                foreach (var line in entries)
                {
                    writer.WriteLine(line);
                }
            }
        }
    }

}
