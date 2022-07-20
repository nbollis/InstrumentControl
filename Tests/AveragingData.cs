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

namespace Tests
{
    public class AveragingData
    {

        [Test]
        public static void AverageDDAData()
        {

            string filepath = @"D:\Projects\InstrumentControl\Output Folder\6ProteinStandardDifferentResolutions\222007 Systematic Approach\RawData\220718_6ProteinStandard_60k.mzML";

            List<MsDataScan> ms1Scans = SpectraFileHandler.LoadMS1ScansFromFile(filepath).Take(5).ToList();

            //List<MsDataScan> scans = ms1Scans.Where(p => p.OneBasedScanNumber >= startScanNum && p.OneBasedScanNumber <= endScanNum).ToList();
            List<SingleScanDataObject> singleScanObjects = SingleScanDataObject.ConvertMSDataScansInBulk(ms1Scans);
            MultiScanDataObject multiScan = new(singleScanObjects);


            NormalizationTask normalizationTask = new NormalizationTask();
            NormalizationOptions normalizationOptions = new NormalizationOptions() {PerformNormalization = true };
            SpectrumAveragingTask averagingTask = new SpectrumAveragingTask();
            SpectrumAveragingOptions averagingOptions = new SpectrumAveragingOptions();
            averagingOptions.SetDefaultValues();
            averagingOptions.RejectionType = RejectionType.WinsorizedSigmaClipping;
            averagingOptions.WeightingType = WeightingType.NormalDistribution;
            averagingOptions.BinSize = 0.001;

            normalizationTask.RunSpecific(normalizationOptions, multiScan);
            averagingTask.RunSpecific(averagingOptions, multiScan);



            string outDirectory = @"D:\Projects\InstrumentControl\Output Folder\6ProteinStandardDifferentResolutions\222007 Systematic Approach";
            string filename = @$"{filepath.Split('.')[0].Substring(filepath.Split('.')[0].Length - 3)}_{multiScan.ScansToProcess}Scans_{averagingOptions.RejectionType}_{averagingOptions.WeightingType}_{averagingOptions.BinSize}.svg";
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
                    for (int i = 0; i < multiScan.CompositeSpectrum.XArray.Length; i++)
                    {
                        writer.WriteLine(multiScan.CompositeSpectrum.XArray[i] + "\t" + multiScan.CompositeSpectrum.YArray[i]);
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
        public static void AverageAustinSIDDataWithDifferentAveragingOptions()
        {
            double startRT = 56.72;
            double endRT = 62.99;
            string filepath = @"D:\DataFiles\AustinSIDData\MS1-sidMS2_6ProtMix_R120_SID60_AGC400_20220123082940.raw";
            string thermopath = @"D:\DataFiles\AustinSIDData\MS1-sidMS2_6ProtMix_R120_SID60_AGC400_20220123082940RT56.72-62.99MS1Only.raw";

            MsDataScan thermoAveragedRaw = SpectraFileHandler.LoadAllScansFromFile(thermopath).First();
            List<MsDataScan> ms1Scans = SpectraFileHandler.LoadMS1ScanFromFile(filepath);
            List<MsDataScan> singleChromPeak = ms1Scans.Where(p => p.RetentionTime >= startRT && p.RetentionTime <= endRT).ToList();
            List<SingleScanDataObject> singleScanObjects = SingleScanDataObject.ConvertMSDataScansInBulk(singleChromPeak);
            MultiScanDataObject multiScan = new(singleScanObjects);

            NormalizationTask normalizationTask = new NormalizationTask();
            NormalizationOptions normalizationOptions = new NormalizationOptions() { PerformNormalization = true };
            normalizationTask.RunSpecific(normalizationOptions, multiScan);
            SpectrumAveragingTask averagingTask = new SpectrumAveragingTask();
            SpectrumAveragingOptions averagingOptions = new SpectrumAveragingOptions();
            averagingOptions.SetDefaultValues();

            var rejectionTypes = Enum.GetValues(typeof(RejectionType));
            var weightingTypes = Enum.GetValues(typeof(WeightingType));

            // runs through each of the types of weighting and rejection
            List<AveragedScanAnalyzer> averaged = new();
            foreach (var weight in weightingTypes)
            {
                averagingOptions.WeightingType = (WeightingType)Enum.Parse(typeof(WeightingType), weight.ToString());
                foreach (var reject in rejectionTypes)
                {
                    averagingOptions.RejectionType = (RejectionType)Enum.Parse(typeof(RejectionType), reject.ToString());
                    string paramName = $"{weight}{reject}";

                    if (averagingOptions.RejectionType == RejectionType.PercentileClipping)
                    {
                        //double[] percents = new double[] { 0.7, 0.8, 0.9, 0.95 };
                        //foreach (var percent in percents)
                        //{
                        //    paramName += $"Percentile: {percent}";
                        //    averagingOptions.Percentile = percent;

                        //    averagingTask.RunSpecific(averagingOptions, multiScan);
                        //    averaged.Add(new AveragedScanAnalyzer(paramName, multiScan.CompositeSpectrum));
                        //}
                    }

                    else if (averagingOptions.RejectionType == RejectionType.SigmaClipping ||
                        averagingOptions.RejectionType == RejectionType.WinsorizedSigmaClipping || 
                        averagingOptions.RejectionType == RejectionType.AveragedSigmaClipping)
                    {
                        //double[] sigmas = new double[] { 0.8, 0.9, 1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 2 };
                        //foreach (var minSigma in sigmas)
                        //{
                        //    foreach (var maxSigma in sigmas)
                        //    {
                        //        paramName += $"MinSigma: {minSigma} - MaxSigma: {maxSigma}";
                        //        averagingOptions.MaxSigmaValue = maxSigma;
                        //        averagingOptions.MinSigmaValue = minSigma;

                        //        averagingTask.RunSpecific(averagingOptions, multiScan);
                        //        averaged.Add(new AveragedScanAnalyzer(paramName, multiScan.CompositeSpectrum));
                        //    }
                        //}
                    }
                    //else
                    //{
                        Console.WriteLine($"Starting {paramName}");
                        averagingTask.RunSpecific(averagingOptions, multiScan);
                        averaged.Add(new AveragedScanAnalyzer(paramName, multiScan.CompositeSpectrum));
                    //}
                }
            }

            string outDirectory = @"D:\Projects\InstrumentControl\Output Folder\FirstPassSpectrumAveragingMethods";
            string filename = @$"{startRT}to{endRT}";
            foreach (var spectrum in averaged)
            {
                string outpath = Path.Combine(outDirectory, spectrum.Header + ".svg");
                SpectrumViewer.WritePlotToSvg(spectrum.PlotModel, outpath);
            }
        }

        [Test]
        public static void GetIsotopicDistributionOfProteinFromFasta()
        {
            // Loads in proteins from a scan and creates a List of doubles representing their m/z values with charges up to 60 from their monoisotopic mass

            // load in proteins with their monoisotopic mass as a peptide representation
            Loaders.LoadElements();
            string filename = @"D:\Projects\InstrumentControl\RealTimeProcessingExample\Six_Protein_Standard.fasta";
            var proteinList = ProteinDbLoader.LoadProteinFasta(filename, true, DecoyType.None, false, out var dbErrors);
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
                    masses.Add(((((double[])carbonicAnyhdraseDistribution.Masses)[i] + (1.00784 * charges[j]))/ charges[j], ((double[])carbonicAnyhdraseDistribution.Intensities)[i]));
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
            string filepath = @"D:\Projects\InstrumentControl\Output Folder\6ProteinStandardDifferentResolutions\222007 Systematic Approach\RawData\15k_5Scans_ThermoAveraged.raw";
            MsDataScan scan = SpectraFileHandler.LoadAllScansFromFile(filepath).First();
            var data = SpectraFileHandler.GetMzandIntValuesFromSingleScan(scan);
            string outpath = filepath.Replace(".raw", ".txt");
            using (StreamWriter writer = new StreamWriter(File.Create(outpath)))
            {
                foreach (var item in data)
                {
                    writer.WriteLine(item.Item1 + "\t" + item.Item2);
                }
            }
        }


        public class AveragedScanAnalyzer
        { 
            public string Header { get; set; }
            public MzSpectrum CompositeSpectrum { get; set; }
            public PlotModel PlotModel { get; set; }

            public AveragedScanAnalyzer(string header, MzSpectrum compositeSpectrum)
            {
                Header = header;
                CompositeSpectrum = compositeSpectrum;
                PlotModel = SpectrumViewer.CreatePlotModel(CompositeSpectrum);
            }   
        }
    }
}
