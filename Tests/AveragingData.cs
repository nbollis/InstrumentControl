using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstrumentControlIO;
using MassSpectrometry;
using Data;
using Normalization;
using Averaging;
using InstrumentControl;
using OxyPlot;
using Proteomics.AminoAcidPolymer;
using UsefulProteomicsDatabases;
using Chemistry;
using Proteomics;

namespace Tests
{
    public class AveragingData
    {
        #region  Data Injectors

        public static double[] GetCarbonicAnhydraseTargets()
        {
            string targetTxtPath = @"D:\Projects\InstrumentControl\Output Folder\AveragingCarbonicAnhydrase\carbonicAnhydrase.txt";
            MzSpectrum carbonicAnhydraseTheoretical = new AveragedScanAnalyzer(targetTxtPath, true).CompositeSpectrum;

            List<double> targets = new();
            for (int i = 0; i < carbonicAnhydraseTheoretical.YArray.Length; i++)
            {
                if (carbonicAnhydraseTheoretical.YArray[i] == 1)
                    targets.Add(carbonicAnhydraseTheoretical.XArray[i]);
            }
            return targets.ToArray();
        }

        #endregion

        [SetUp]
        public static void Setup()
        {
            Loaders.LoadElements();
        }

        [Test]
        public static void AverageDDAData()
        {
            double resolution = 15;
            string filepath = @$"D:\Projects\InstrumentControl\Output Folder\6ProteinStandardDifferentResolutions\222007 Systematic Approach\RawData\220718_6ProteinStandard_{resolution}k.mzML";

            List<MsDataScan> ms1Scans = SpectraFileHandler.LoadMS1ScansFromFile(filepath).Take(5).ToList();

            //List<MsDataScan> scans = ms1Scans.Where(p => p.OneBasedScanNumber >= startScanNum && p.OneBasedScanNumber <= endScanNum).ToList();
            List<SingleScanDataObject> singleScanObjects = SingleScanDataObject.ConvertMSDataScansInBulk(ms1Scans);
            MultiScanDataObject multiScan = new(singleScanObjects);


            NormalizationTask normalizationTask = new NormalizationTask();
            NormalizationOptions normalizationOptions = new NormalizationOptions() { PerformNormalization = true };
            SpectrumAveragingTask averagingTask = new SpectrumAveragingTask();
            SpectrumAveragingOptions averagingOptions = new SpectrumAveragingOptions();
            averagingOptions.SetDefaultValues();
            averagingOptions.RejectionType = RejectionType.SigmaClipping;
            averagingOptions.WeightingType = WeightingType.NormalDistribution;
            averagingOptions.BinSize = 0.05;
            normalizationTask.RunSpecific(normalizationOptions, multiScan);
            averagingTask.RunSpecific(averagingOptions, multiScan);



            string outDirectory = @"D:\Projects\InstrumentControl\Output Folder\6ProteinStandardDifferentResolutions\222007 Systematic Approach";
            string filename = @$"{filepath.Split('.')[0].Substring(filepath.Split('.')[0].Length - 3)}_ZeroComOneNorm_{multiScan.ScansToProcess}Scans_{averagingOptions.RejectionType}_{averagingOptions.WeightingType}_{averagingOptions.BinSize}.svg";
            string outpath = Path.Combine(outDirectory, filename);
            // export plot model
            if (false)
            {
                PlotModel averagedModel = SpectrumViewer.CreatePlotModel(multiScan.CompositeSpectrum);
                SpectrumViewer.WritePlotToSvg(averagedModel, outpath);
            }

            // export to txt for excel analysis
            if (true)
            {
                using (StreamWriter writer = new StreamWriter(File.Create(outpath.Replace(".svg", ".txt"))))
                {
                    double ymax = multiScan.CompositeSpectrum.YArray.Max();
                    for (int i = 0; i < multiScan.CompositeSpectrum.XArray.Length; i++)
                    {
                        writer.WriteLine(multiScan.CompositeSpectrum.XArray[i] + "\t" + (multiScan.CompositeSpectrum.YArray[i] / ymax));
                    }
                }
            }

        }

        [Test]
        public static void AverageAustinSIDData()
        {
            double startRT = 45.15;
            double endRT = 45.29;
            string filepath = @"D:\DataFiles\AustinSIDData\AVC_12-13-2109_yeast_fraction2_Res30k_Cycle2s_SID60 - Copy.raw";

            List<MsDataScan> ms1Scans = SpectraFileHandler.LoadAllScansFromFile(filepath);
            List<MsDataScan> singleChromPeak = ms1Scans.Where(p => p.RetentionTime >= startRT && p.RetentionTime <= endRT && p.ScanFilter == "ITMS + p NSI sid=15.00 Full ms [500.0000-2000.0000]").ToList();
            List<SingleScanDataObject> singleScanObjects = SingleScanDataObject.ConvertMSDataScansInBulk(singleChromPeak);
            MultiScanDataObject multiScan = new(singleScanObjects);

            NormalizationTask normalizationTask = new NormalizationTask();
            NormalizationOptions normalizationOptions = new NormalizationOptions() { PerformNormalization = true };
            SpectrumAveragingTask averagingTask = new SpectrumAveragingTask();
            SpectrumAveragingOptions averagingOptions = new SpectrumAveragingOptions();
            averagingOptions.SetDefaultValues();
            averagingOptions.BinSize = 0.001;
            averagingOptions.RejectionType = RejectionType.MinMaxClipping;

            normalizationTask.RunSpecific(normalizationOptions, multiScan);
            averagingTask.RunSpecific(averagingOptions, multiScan);

            string outDirectory = @"D:\Projects\InstrumentControl\Output Folder\AveragingRandomLowResYeastSID\DifferentSettings";
            string filename = @$"RejectionRT{startRT}to{endRT}-{singleScanObjects.Count}Yeastscans{averagingOptions.RejectionType}.svg";
            string outpath = Path.Combine(outDirectory, filename);

            // export plot model
            if (false)
            {
                PlotModel averagedModel = SpectrumViewer.CreatePlotModel(multiScan.CompositeSpectrum);
                SpectrumViewer.WritePlotToSvg(averagedModel, outpath);
            }

            // export as data points
            if (true)
            {
                using (StreamWriter writer = new StreamWriter(File.Create(outpath.Replace(".svg", ".txt"))))
                {
                    for (int i = 0; i < multiScan.CompositeSpectrum.XArray.Length; i++)
                    {
                        writer.WriteLine(multiScan.CompositeSpectrum.XArray[i] + "\t" + multiScan.CompositeSpectrum.YArray[i]);
                    }
                }
            }
        }


        [Test]
        public static void GetIsotopicDistributionOfProteinFromFasta()
        {
            // Loads in proteins from a scan and creates a List of doubles representing their m/z values with charges up to 60 from their monoisotopic mass

            // load in proteins with their monoisotopic mass as a peptide representation
            Loaders.LoadElements();
            string filename = @"D:\Projects\InstrumentControl\RealTimeProcessingExample\Six_Protein_Standard.fasta";
            List<Protein> proteinList = ProteinDbLoader.LoadProteinFasta(filename, true, DecoyType.None, false, out var dbErrors);
            List<Peptide> peptideRepresentation = new();
            List<IsotopicDistribution> distributions = new();
            foreach (var protein in proteinList)
            {
                Peptide prot = new(protein.BaseSequence);
                distributions.Add(IsotopicDistribution.GetDistribution(prot.GetChemicalFormula()));
                peptideRepresentation.Add(prot);
            }

            int proteinNum = 3;

            Peptide carbonicAnyhdrase = peptideRepresentation[proteinNum];
            IsotopicDistribution carbonicAnyhdraseDistribution = distributions[proteinNum];

            int[] charges = Enumerable.Range(1, 60).ToArray();
            List<(double, double)> masses = new();
            for (int j = 0; j < charges.Length; j++)
            {
                for (int i = 0; i < carbonicAnyhdraseDistribution.Masses.Count(); i++)
                {
                    masses.Add(((((double[])carbonicAnyhdraseDistribution.Masses)[i] + (1.00784 * charges[j])) / charges[j], ((double[])carbonicAnyhdraseDistribution.Intensities)[i]));
                }
            }
            masses = masses.Where(p => p.Item1 > 450 && p.Item1 < 2000).OrderBy(p => p.Item1).ToList();

            masses = masses.Where(p => p.Item2 > 10E-10).ToList();
            if (true)
            {
                string outpath = @$"D:\Projects\InstrumentControl\Output Folder\AveragingCarbonicAnhydrase\protein{proteinNum}Trimmed.txt";
                using (StreamWriter writer = new StreamWriter(File.Create(outpath)))
                {
                    foreach (var mass in masses)
                    {
                        writer.WriteLine(mass.Item1 + "\t" + mass.Item2);
                    }
                }
            }
        }

        [Test]
        public static void GetSpectrumTxtFromOneScan()
        {
            string filepath = @"D:\Projects\InstrumentControl\Output Folder\6ProteinBackToBack\Data\220725_6ProteinStandard_15k_5-qb.raw";
            MsDataScan scan = SpectraFileHandler.LoadAllScansFromFile(filepath).First();
            var data = SpectraFileHandler.GetMzandIntValuesFromSingleScan(scan);
            string outpath = filepath.Replace(".raw", ".txt");
            using (StreamWriter writer = new StreamWriter(File.Create(outpath)))
            {
                double yMax = data.Select(p => p.Item2).Max();
                foreach (var item in data)
                {
                    writer.WriteLine(item.Item1 + "\t" + (item.Item2 ));
                }
            }
        }


        [Test]
        public static void DoManyMethodsAndOutputSystematicallySmallerScale()
        {
            #region Value Initialization

            //string directoryPath = @"D:\Projects\InstrumentControl\Output Folder\6ProteinStandardDifferentResolutions\220722IterationAndScoring\TestingBigAlgorithmicChanges";
            string directoryPath = @"D:\Projects\InstrumentControl\Output Folder\6ProteinBackToBack\FirstPassAtScoring";
            string[] resolutions = new string[] { "15k", "30k", "45k", "60k" };
            int[] scansToAverage = new int[] { 5 };
            List<string> mzMLpaths = new List<string>();
            foreach (var resolution in resolutions)
            {
                mzMLpaths.Add(@$"D:\Projects\InstrumentControl\Output Folder\6ProteinBackToBack\Data\220725_6ProteinStandard_{resolution}_1.mzML");
            }
            List<AveragedScanAnalyzer> analysis = new List<AveragedScanAnalyzer>();
            NormalizationTask normalizationTask = new NormalizationTask();
            NormalizationOptions normalizationOptions = new NormalizationOptions() { PerformNormalization = true };
            SpectrumAveragingTask averagingTask = new SpectrumAveragingTask();

            #endregion

            #region Generating Options

            List<SpectrumAveragingOptions> averagingOptions = new();
            List<RejectionType> rejectionTypes = new();
            rejectionTypes.AddRange(Enum.GetValues<RejectionType>().Where(p => p != RejectionType.Thermo));

            List<WeightingType> weightingTypes = new();
            weightingTypes.AddRange(Enum.GetValues<WeightingType>());

            double[] binSizes = new double[] { 0.5, 0.1, 0.05, 0.01, 0.005, 0.001 };
            double[] sigmas = new double[] { 1.1, 1.2, 1.3, 1.4, 1.5, 2, 3, 4, 5, 6 };
            double[] percentiles = new double[] { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9 };
            foreach (var rejection in rejectionTypes)
            {
                foreach (var weight in weightingTypes)
                {
                    foreach (var bin in binSizes)
                    {

                        if (rejection == RejectionType.SigmaClipping || rejection == RejectionType.WinsorizedSigmaClipping || rejection == RejectionType.AveragedSigmaClipping)
                        {
                            foreach (var sigma in sigmas)
                            {
                                foreach (var innerSigma in sigmas)
                                {
                                    SpectrumAveragingOptions innerOption = new();
                                    innerOption.SetDefaultValues();
                                    innerOption.RejectionType = rejection;
                                    innerOption.WeightingType = weight;
                                    innerOption.BinSize = bin;

                                    innerOption.MinSigmaValue = sigma;
                                    innerOption.MaxSigmaValue = innerSigma;
                                    averagingOptions.Add(innerOption);
                                }
                            }
                        }
                        else if (rejection == RejectionType.PercentileClipping)
                        {
                            foreach (var percentile in percentiles)
                            {
                                SpectrumAveragingOptions innerOption = new();
                                innerOption.SetDefaultValues();
                                innerOption.RejectionType = rejection;
                                innerOption.WeightingType = weight;
                                innerOption.BinSize = bin;

                                innerOption.Percentile = percentile;
                                averagingOptions.Add(innerOption);
                            }
                        }
                        else
                        {
                            SpectrumAveragingOptions averagingOption = new();
                            averagingOption.SetDefaultValues();
                            averagingOption.RejectionType = rejection;
                            averagingOption.WeightingType = weight;
                            averagingOption.BinSize = bin;

                            averagingOptions.Add(averagingOption);
                        }
                    }
                }
            }

            #endregion

            // iterate through options, averaging as you go
            foreach (var filepath in mzMLpaths)
            {
                foreach (var scansToTake in scansToAverage)
                {
                    List<MsDataScan> ms1Scans = SpectraFileHandler.LoadMS1ScansFromFile(filepath);
                    List<MsDataScan> trimmedScans = ms1Scans.Take(scansToTake).ToList();
                    List<SingleScanDataObject> singleScanObjects = SingleScanDataObject.ConvertMSDataScansInBulk(trimmedScans);

                    MultiScanDataObject multiScan = new(singleScanObjects);
                    normalizationTask.RunSpecific(normalizationOptions, multiScan);
                    analysis.AddRange(AveragedScanAnalyzer.AverageMultiScanWithManyOptions(averagingOptions, multiScan, filepath.Split('.')[0].Substring(filepath.Split('.')[0].Length - 5, 3)));
                }
            }

            // add the thermo scans to it
            string dataDirectory = @"D:\Projects\InstrumentControl\Output Folder\6ProteinBackToBack\Data";
            foreach (var file in Directory.GetFiles(dataDirectory).Where(p => p.Contains(".txt")))
            {
                AveragedScanAnalyzer thermo = new(file);
                thermo.Options.RejectionType = RejectionType.Thermo;
                analysis.Add(thermo);
            }

            // output basic txt of results
            string type = $"ZerosInDistribution";
            string outpath = "";
            foreach (var item in analysis)
            {
                //outpath = Path.Combine(directoryPath, "AbsoluteAbundance", item.Header + $"{type}.txt");
                //item.PrintAveragedScanAnalyzerAsTxtOfSpectrum(outpath, false);

                //outpath = Path.Combine(directoryPath, "RelativeAbundance", item.Header + $"{type}.txt");
                //item.PrintAveragedScanAnalyzerAsTxtOfSpectrum(outpath, true);

                outpath = Path.Combine(directoryPath, "Figures", item.Header + $"{type}.svg");
                item.PlotModel = SpectrumViewer.CreatePlotModel(item.CompositeSpectrum);
                SpectrumViewer.WritePlotToSvg(item.PlotModel, outpath);
            }

            // Score the results with numerous methods
            double[] ppms = new double[] { 100, 75, 50, 25, 10 };
            double[] sigmasDecreaseFromMean = new double[] { 1.1, 1, 0.9, 0.75, 0.5 };
            foreach (var ppm in ppms)
            {
                foreach (var sigma in sigmasDecreaseFromMean)
                {
                    foreach (var item in analysis)
                    {
                        outpath = Path.Combine(directoryPath, "AbsoluteAbundance", item.Header + $"{type}.txt");
                        item.ScoreBasedOnRelavantPeaksAboveDeviation(GetCarbonicAnhydraseTargets(), ppm, sigma);
                        item.PrintAveragedScanAsTxtWithScore(outpath, false);
                    }
                    outpath = Path.Combine(directoryPath, "Scores", $"CompiledScoredResults_{ppm}_{sigma}_{type}.csv");
                    AveragedScanAnalyzer.IterateThroughScoredAveragedScanAnalyzers(analysis, outpath);
                }
            }
        }

        [Test]
        public static void ReorderByScore()
        {
            string directoryPath = @"D:\Projects\InstrumentControl\Output Folder\6ProteinBackToBack\FirstPassAtScoring\Scores";
            foreach (var file in Directory.GetFiles(directoryPath))
            {
                string[] lines = File.ReadAllLines(file);
                var data = lines.Skip(1);
                var sorted = data.Select(line => new
                {
                    SortKey = Double.Parse(line.Split("\t")[8]),
                    SecondSort = double.Parse(line.Split("\t")[9]),
                    Line = line
                }).OrderByDescending(p => p.SortKey).ThenByDescending(p => p.SecondSort).Select(p => p.Line);
                File.WriteAllLines(Path.Combine(directoryPath, "Sorted", Path.GetFileNameWithoutExtension(file) + ".csv") , lines.Take(1).Concat(sorted));
            }
        }

        [Test]
        public static void TakeTop3FromEachScoredResult()
        {
            string directoryPath = @"D:\Projects\InstrumentControl\Output Folder\6ProteinBackToBack\FirstPassAtScoring\Scores\Sorted";
            List<string> top3 = new();
            string[] header = new string[Directory.GetFiles(directoryPath).Count() + 1];
            foreach (var file in Directory.GetFiles(directoryPath))
            {
                string[] lines = File.ReadAllLines(file);
                header[0] = lines[0];
                
                top3.AddRange(lines.Take(new Range(1,3)));
            }

            var sorted = top3.Select(line => new
            {
                SortKey = Double.Parse(line.Split("\t")[8]),
                Line = line
            }).OrderByDescending(p => p.SortKey).Select(p => p.Line);


            File.WriteAllLines(Path.Combine(directoryPath, "Top3FromEach.csv"), header.Concat(top3));
        }
    }
}
