using MassSpectrometry;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;
using MzLibUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{

    // Workflow: Take in set of scans --> MAYBE: Normalize intensity to TotalIonCurrent --> isolate an array of intensities for each mass --> reject outliers --> calculate weights of each point ---> calculate a weighted average
    // TODO deal with scans when they enter e.g. convert to an array of intensities for each m/z value
    // TODO consider whether normaliztion is neededs

    /// <summary>
    /// Class which takes input of n scans and averages them based upon the parameters set
    /// </summary>
    public class SpectrumAveragingTask : InstrumentControlTask, ISpectraAverager
    {
        #region Public Properties

        // settings
        public static RejectionType RejectionType { get; set; } = RejectionType.NoRejection;
        public static WeightingType WeightingType { get; set; } = WeightingType.NoWeight;
        public static SpectrumMergingType SpectrumMergingType { get; set; } = SpectrumMergingType.SpectrumBinning;
        public static double Percentile { get; set; } = 0.9;
        public static double MinSigmaValue { get; set; } = 1.3;
        public static double MaxSigmaValue { get; set; } = 1.3;
        public static double BinSize { get; set; } = 0.02;

        #endregion

        #region Constructor

        public SpectrumAveragingTask(TaskType taskType) : base(taskType)
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Performs the main operations of this task
        /// </summary>
        /// <returns></returns>
        public override void RunSpecific()
        {
            // Average Spectrum
            ISpectraAverager.CompositeSpectrum = CombineSpectra(ISpectraProcesor.XArrays, ISpectraProcesor.YArrays, ISpectraProcesor.ScansToProcess);
        }

        /// <summary>
        /// Main Engine of this class, processes a single array of intesnity values for a single mz and returns their average
        /// </summary>
        /// <param name="mzInitialArray"></param>
        /// <returns></returns>
        public static double ProcessSingleMzArray(double[] mzInitialArray)
        {
            double[] trimmedArray = RejectOutliers(mzInitialArray);
            double[] weights = CalculateWeights(trimmedArray, WeightingType);
            double average = MergePeakValuesToAverage(trimmedArray, weights);

            return average;
        }

        /// <summary>
        /// Calculates the weighted average value for each m/z point passed to it
        /// </summary>
        /// <param name="mzValues">array of mz values to evaluate </param>
        /// <param name="weights">relative weight assigned to each of the mz values</param>
        /// <returns></returns>
        public static double MergePeakValuesToAverage(double[] mzValues, double[] weights)
        {
            double numerator = 0;
            for (int i = 0; i < mzValues.Count(); i++)
            {
                numerator += mzValues[i] * weights[i];
            }
            double average = numerator / weights.Sum();
            return average;
        }

        /// <summary>
        /// Can be used to set the values of the static class in one method call
        /// </summary>
        /// <param name="rejectionType">rejection type to be used</param>
        /// <param name="percentile">percentile for percentile clipping rejection type</param>
        /// <param name="sigma">sigma value for sigma clipping rejection types</param>
        public static void SetValues(RejectionType rejectionType = RejectionType.NoRejection, WeightingType intensityWeighingType = WeightingType.NoWeight, double percentile = 0.9, double minSigma = 1.3, double maxSigma = 1.3, double binSize = 0.02)
        {
            RejectionType = rejectionType;
            WeightingType = intensityWeighingType;
            Percentile = percentile;
            MinSigmaValue = minSigma;
            MaxSigmaValue = maxSigma;
            BinSize = binSize;
        }

        /// <summary>
        /// Method used to reset all static values to their default
        /// </summary>
        public static void ResetValues()
        {
            RejectionType = RejectionType.NoRejection;
            WeightingType = WeightingType.NoWeight;
            Percentile = 0.9;
            MinSigmaValue = 1.3;
            MaxSigmaValue = 1.3;
            BinSize = 0.02;
        }

        #endregion

        #region Rejection Functions

        /// <summary>
        /// Calls the specific rejection function based upon the current static field RejectionType
        /// </summary>
        /// <param name="mzValues">list of mz values to evaluate<</param>
        /// <returns></returns>
        public static double[] RejectOutliers(double[] mzValues, int scanCount = 0)
        {
            double[] trimmedMzValues = mzValues;
            switch (RejectionType)
            {
                case RejectionType.NoRejection:
                    break;

                case RejectionType.MinMaxClipping:
                    trimmedMzValues = MinMaxClipping(mzValues);
                    break;

                case RejectionType.PercentileClipping:
                    trimmedMzValues = PercentileClipping(mzValues, Percentile);
                    break;

                case RejectionType.SigmaClipping:
                    trimmedMzValues = SigmaClipping(mzValues, MinSigmaValue, MaxSigmaValue);
                    break;

                case RejectionType.WinsorizedSigmaClipping:
                    trimmedMzValues = WinsorizedSigmaClipping(mzValues, MinSigmaValue, MaxSigmaValue);
                    break;

                case RejectionType.AveragedSigmaClipping:
                    trimmedMzValues = AveragedSigmaClipping(mzValues, MinSigmaValue, MaxSigmaValue);
                    break;

                case RejectionType.BelowThresholdRejection:
                    trimmedMzValues = BelowThresholdRejection(mzValues, scanCount);
                    break;
            }
            return trimmedMzValues;
        }

        /// <summary>
        /// Reject the max and min of the set
        /// </summary>
        /// <param name="initialValues">array of mz values to evaluate</param>
        /// <returns>list of mz values with outliers rejected</returns>
        public static double[] MinMaxClipping(double[] initialValues)
        {
            double max = initialValues.Max();
            double min = initialValues.Min();

            double[] clippedValues = initialValues.Where(p => p < max && p > min).ToArray();
            CheckValuePassed(clippedValues);
            return clippedValues;
        }

        /// <summary>
        /// Removes values that fall outside of the central value by the defined percentile exclusively
        /// </summary>
        /// <param name="initialValues">list of mz values to evaluate</param>
        /// <param name="percentile"></param>
        /// <returns>list of mz values with outliers rejected</returns>
        public static double[] PercentileClipping(double[] initialValues, double percentile)
        {
            double median = CalculateMedian(initialValues);
            double[] clippedValues = initialValues.Where(p => (median - p) / median > -percentile && (median - p) / median < percentile).ToArray();
            CheckValuePassed(clippedValues);
            return clippedValues;
        }

        /// <summary>
        /// Itteratively removes values that fall outside of the central value by the defined StandardDeviation amount
        /// </summary>
        /// <param name="initialValues">list of mz values to evaluate</param>
        /// <param name="sValueMin">the lower limit of inclusion in sigma (standard deviation) units</param>
        /// <param name="sValueMax">the higher limit of inclusion in sigma (standard deviation) units</param>
        /// <returns></returns>
        public static double[] SigmaClipping(double[] initialValues, double sValueMin, double sValueMax)
        {
            List<double> values = initialValues.ToList();
            int n = 0;
            do
            {
                double median = CalculateMedian(values);
                double standardDeviation = CalculateStandardDeviation(values);
                n = 0;
                for (int i = 0; i < values.Count; i++)
                {
                    if (SigmaClipping(values[i], median, standardDeviation, sValueMin, sValueMax))
                    {
                        values.RemoveAt(i);
                        n++;
                        i--;
                    }
                }
            } while (n > 0);
            double[] val = values.ToArray();
            CheckValuePassed(val);
            return val;
        }

        /// <summary>
        /// Itteratively replaces values that fall outside of the central value by the defined StandardDeviation amount with the values of the median * that standard deviation amount
        /// </summary>
        /// <param name="initialValues">list of mz values to evaluate</param>
        /// <param name="sValueMin">the lower limit of inclusion in sigma (standard deviation) units</param>
        /// <param name="sValueMax">the higher limit of inclusion in sigma (standard deviation) units</param>
        /// <returns></returns>
        public static double[] WinsorizedSigmaClipping(double[] initialValues, double sValueMin, double sValueMax)
        {
            List<double> values = initialValues.ToList();
            int n = 0;
            double iterationLimitforHuberLoop = 0.0005;
            double averageAbsoluteDeviation = Math.Sqrt(2 / Math.PI) * (sValueMax + sValueMin) / 2;
            double medianLeftBound;
            double medianRightBound;
            double windsorizedStandardDeviation;
            do
            {
                double median = CalculateMedian(values);
                double standardDeviation = CalculateStandardDeviation(values);
                double[] toProcess = values.ToArray();
                do // calculates a new median and standard deviation based on the values to do sigma clipping with (Huber loop)
                {
                    medianLeftBound = median - sValueMin * standardDeviation;
                    medianRightBound = median + sValueMax * standardDeviation;
                    Winsorize(toProcess, medianLeftBound, medianRightBound);
                    median = CalculateMedian(toProcess);
                    windsorizedStandardDeviation = standardDeviation;
                    standardDeviation = averageAbsoluteDeviation > 1 ? CalculateStandardDeviation(toProcess) * averageAbsoluteDeviation : CalculateStandardDeviation(toProcess) * 1.05;
                    double value = Math.Abs(standardDeviation - windsorizedStandardDeviation) / windsorizedStandardDeviation;

                } while (Math.Abs(standardDeviation - windsorizedStandardDeviation) / windsorizedStandardDeviation > iterationLimitforHuberLoop);

                n = 0;
                for (int i = 0; i < values.Count; i++)
                {
                    if (SigmaClipping(values[i], median, standardDeviation, sValueMin, sValueMax))
                    {
                        values.RemoveAt(i);
                        n++;
                        i--;
                    }
                }
            } while (n > 0);
            double[] val = values.ToArray();
            CheckValuePassed(val);
            return val;
        }

        /// <summary>
        /// Iteratively removes values that fall outside of a calculated deviation based upon the median of the values
        /// </summary>
        /// <param name="initialValues">list of mz values to evaluate</param>
        /// <param name="sValueMin">the lower limit of inclusion in sigma (standard deviation) units</param>
        /// <param name="sValueMax">the higher limit of inclusion in sigma (standard deviation) units</param>
        /// <returns></returns>
        public static double[] AveragedSigmaClipping(double[] initialValues, double sValueMin, double sValueMax)
        {
            List<double> values = initialValues.ToList();
            double median = CalculateMedian(initialValues);
            double deviation = CalculateStandardDeviation(initialValues, median);
            int n = 0;
            double standardDeviation;
            do
            {
                median = CalculateMedian(values);
                standardDeviation = deviation * Math.Sqrt(median) / 10;

                n = 0;
                for (int i = 0; i < values.Count; i++)
                {
                    if (SigmaClipping(values[i], median, standardDeviation, sValueMin, sValueMax))
                    {
                        values.RemoveAt(i);
                        n++;
                        i--;
                    }
                }
            } while (n > 0);
            double[] val = values.ToArray();
            CheckValuePassed(val);
            return val;
        }

        /// <summary>
        /// Sets the array of mz values to null if they have 25% or fewer values than the number of scans
        /// </summary>
        /// <param name="initialValues">array of mz values to evaluate</param>
        /// <param name="scanCount">number of scans used to create initialValues</param>
        /// <returns></returns>
        public static double[] BelowThresholdRejection(double[] initialValues, int scanCount)
        {
            double cutoffValue = 0.20;
            if (initialValues.Count() <= scanCount * cutoffValue)
            {
                initialValues = null;
            }
            else if (initialValues.Where(p => p != 0).Count() <= scanCount * cutoffValue)
            {
                initialValues = null;
            }
            return initialValues;
        }

        #endregion

        #region Weighing Functions

        /// <summary>
        /// Calls the specicic funtion based upon the settings to calcuate the weight for each value when averaging
        /// </summary>
        /// <param name="mzValues"></param>
        public static double[] CalculateWeights(double[] mzValues, WeightingType weightingType)
        {
            double[] weights = new double[mzValues.Length];

            switch (weightingType)
            {
                case WeightingType.NoWeight:
                    for (int i = 0; i < weights.Length; i++)
                        weights[i] = 1;
                    break;

                case WeightingType.NormalDistribution:
                    WeightByNormalDistribution(mzValues, ref weights);
                    break;

                case WeightingType.CauchyDistribution:
                    WeightByCauchyDistribution(mzValues, ref weights);
                    break;

                case WeightingType.PoissonDistribution:
                    WeightByPoissonDistribution(mzValues, ref weights);
                    break;

                case WeightingType.GammaDistribution:
                    WeightByGammaDistribution(mzValues, ref weights);
                    break;
            }
            return weights;
        }


        /// <summary>
        /// Weights the mzValues based upon a normal distribution
        /// </summary>
        /// <param name="mzValues">intensities for a single mz value</param>
        /// <param name="weights">calculated weights for each intensity</param>
        public static void WeightByNormalDistribution(double[] mzValues, ref double[] weights)
        {
            double standardDeviation = CalculateStandardDeviation(mzValues);
            double mean = mzValues.Average();

            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = Normal.PDF(mean, standardDeviation, mzValues[i]);
            }
        }

        /// <summary>
        /// Weights the mzValues based upon a cauchy distribution
        /// </summary>
        /// <param name="mzValues">intensities for a single mz value</param>
        /// <param name="weights">calculated weights for each intensity</param>
        public static void WeightByCauchyDistribution(double[] mzValues, ref double[] weights)
        {
            double standardDeviation = CalculateStandardDeviation(mzValues);
            double mean = mzValues.Average();

            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = Cauchy.PDF(mean, standardDeviation, mzValues[i]);
            }
        }

        /// <summary>
        /// Weights the mzValues based upon a gamma distribution
        /// </summary>
        /// <param name="mzValues">intensities for a single mz value</param>
        /// <param name="weights">calculated weights for each intensity</param>
        public static void WeightByGammaDistribution(double[] mzValues, ref double[] weights)
        {
            double standardDeviation = CalculateStandardDeviation(mzValues);
            double mean = mzValues.Average();
            double rate = mean / Math.Pow(standardDeviation, 2);
            double shape = mean * rate;

            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = Gamma.PDF(shape, rate, mzValues[i]);
            }
        }

        /// <summary>
        /// Weights the mzValues based upon a poisson distribution
        /// </summary>
        /// <param name="mzValues">intensities for a single mz value</param>
        /// <param name="weights">calculated weights for each intensity</param>
        public static void WeightByPoissonDistribution(double[] mzValues, ref double[] weights)
        {
            double mean = mzValues.Average();

            for (int i = 0; i < weights.Length; i++)
            {
                if (mzValues[i] > mean)
                    weights[i] = 1 - Poisson.CDF(mean, mzValues[i]);
                else if (mzValues[i] < mean)
                    weights[i] = Poisson.CDF(mean, mzValues[i]);
            }
        }

        #endregion

        #region Merging Functions

        /// <summary>
        /// Calls the specific merging function based upon the current static field SpecrimMergingType
        /// </summary>
        /// <param name="scans"></param>
        public static MzSpectrum CombineSpectra(double[][] xArrays, double[][] yArrays, int numSpectra)
        {
            MzSpectrum compositeSpectrum = null;
            switch (SpectrumMergingType)
            {
                case SpectrumMergingType.SpectrumBinning:
                    compositeSpectrum = SpectrumBinning(xArrays, yArrays, BinSize, numSpectra);
                    break;


                case SpectrumMergingType.MostSimilarSpectrum:
                    MostSimilarSpectrum();
                    break;
            }
            return compositeSpectrum;
        }

        /// <summary>
        /// Merges spectra into a two dimensional array of (m/z, int) values based upon their bin 
        /// </summary>
        /// <param name="scans">scans to be combined</param>
        /// <returns>MSDataScan with merged values</returns>
        public static MzSpectrum SpectrumBinning(double[][] xArrays, double[][] yArrays, double binSize, int numSpectra)
        {
            // calculate the bins to be utilizied
            double min = 100000;
            double max = 0;
            for (int i = 0; i < numSpectra; i++)
            {
                min = Math.Min(xArrays[i][0], min);
                max = Math.Max(xArrays[i].Max(), max);
            }
            int numberOfBins = (int)Math.Ceiling((max - min) * (1 / binSize));

            double[][] xValuesArray = new double[numberOfBins][];
            double[][] yValuesArray = new double[numberOfBins][];
            // go through each scan and place each (m/z, int) from the spectra into a jagged array
            for (int i = 0; i < numSpectra; i++)
            {
                for (int j = 0; j < xArrays[i].Length; j++)
                {
                    int binIndex = (int)Math.Floor((xArrays[i][j] - min) / binSize);
                    if (xValuesArray[binIndex] == null)
                    {
                        xValuesArray[binIndex] = new double[numSpectra];
                        yValuesArray[binIndex] = new double[numSpectra];
                    }
                    xValuesArray[binIndex][i] = xArrays[i][j];
                    yValuesArray[binIndex][i] = yArrays[i][j];
                }
            }

            // remove null and any bins below the threshold (currently 20%) i.e. only one scan had a peak in that bin
            xValuesArray = xValuesArray.Where(p => p != null).ToArray();
            yValuesArray = yValuesArray.Where(p => p != null).ToArray();
            RejectionType temp = RejectionType;
            RejectionType = RejectionType.BelowThresholdRejection;
            for (int i = 0; i < yValuesArray.Length; i++)
            {
                yValuesArray[i] = RejectOutliers(yValuesArray[i], numSpectra);
                if (yValuesArray[i] == null)
                {
                    xValuesArray[i] = null;
                }
                else
                {
                    yValuesArray[i] = yValuesArray[i].Where(p => p != 0).ToArray();
                }
            }
            temp = RejectionType;
            xValuesArray = xValuesArray.Where(p => p != null).ToArray();
            yValuesArray = yValuesArray.Where(p => p != null).ToArray();

            // average the remaining arrays to create the composite spectrum
            // this will clipping and avereraging for y values as indicated in the settings
            double[] xArray = new double[xValuesArray.Length];
            double[] yArray = new double[yValuesArray.Length];
            for (int i = 0; i < yValuesArray.Length; i++)
            {
                xArray[i] = xValuesArray[i].Where(p => p != 0).Average();
                yArray[i] = ProcessSingleMzArray(yValuesArray[i]);
            }

            // Create new MsDataScan to return
            MzRange range = new(min, max);
            MzSpectrum mergedSpectra = new(xArray, yArray, false);

            return mergedSpectra;
        }
        public static MzSpectrum MostSimilarSpectrum()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Ensures that there are values within the output of a clipping function
        /// </summary>
        private static void CheckValuePassed(double[] toCheck)
        {
            if (toCheck == null)
                throw new ArgumentNullException("All values were removed by clipping");

            bool valueFound = false;
            foreach (double value in toCheck)
            {
                if (value > 0)
                    valueFound = true;
            }

            if (!valueFound)
                throw new Exception("All values were removed by clipping");
        }

        /// <summary>
        /// Helper delegate method for sigma clipping
        /// </summary>
        /// <param name="value">the value in question</param>
        /// <param name="median">median of the dataset</param>
        /// <param name="standardDeviation">standard dev of the dataset</param>
        /// <param name="sValueMin">the lower limit of inclusion in sigma (standard deviation) units</param>
        /// <param name="sValueMax">the higher limit of inclusion in sigma (standard deviation) units</param>
        /// <returns></returns>
        private static bool SigmaClipping(double value, double median, double standardDeviation, double sValueMin, double sValueMax)
        {
            if ((median - value) / standardDeviation > sValueMin)
            {
                return true;
            }
            else if ((value - median) / standardDeviation > sValueMax)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Helper method to mutate the array of doubles based upon the median value
        /// </summary>
        /// <param name="initialValues">initial values to process</param>
        /// <param name="medianLeftBound">minimum the element in the dataset is allowed to be</param>
        /// <param name="medianRightBound">maxamum the element in the dataset is allowed to be</param>
        /// <returns></returns>
        private static double[] Winsorize(double[] initialValues, double medianLeftBound, double medianRightBound)
        {
            for (int i = 0; i < initialValues.Length; i++)
            {
                if (initialValues[i] < medianLeftBound)
                {
                    initialValues[i] = medianLeftBound;
                }
                else if (initialValues[i] > medianRightBound)
                {
                    initialValues[i] = medianRightBound;
                }
            }
            return initialValues;
        }

        /// <summary>
        /// Calculates the median of an array of doubles
        /// </summary>
        /// <param name="toCalc">initial array to calculate from</param>
        /// <returns>double representation of the median</returns>
        private static double CalculateMedian(double[] toCalc)
        {
            double median = 0;
            if (toCalc.Any())
            {
                if (toCalc.Length % 2 == 0)
                {
                    median = (toCalc[toCalc.Length / 2] + toCalc[toCalc.Length / 2 - 1]) / 2;
                }
                else
                {
                    median = toCalc[(toCalc.Length - 1) / 2];
                }
            }
            return median;
        }

        /// <summary>
        /// Calculates the median of a list of doubles
        /// </summary>
        /// <param name="toCalc">initial list to calculate from</param>
        /// <returns>double representation of the median</returns>
        private static double CalculateMedian(List<double> toCalc)
        {
            double median = 0;
            if (toCalc.Any())
            {
                if (toCalc.Count % 2 == 0)
                {
                    median = (toCalc[toCalc.Count / 2] + toCalc[toCalc.Count / 2 - 1]) / 2;
                }
                else
                {
                    median = toCalc[(toCalc.Count - 1) / 2];
                }
            }
            return median;
        }

        /// <summary>
        /// Calculates the standard deviation of an array of doubles
        /// </summary>
        /// <param name="toCalc">initial array to calculate from</param>
        /// <param name="average">passable value for the average</param>
        /// <returns>double representation of the standard deviation</returns>
        private static double CalculateStandardDeviation(double[] toCalc, double average = 0)
        {
            double deviation = 0;

            if (toCalc.Any())
            {
                average = average == 0 ? toCalc.Average() : average;
                double sum = toCalc.Sum(p => Math.Pow(p - average, 2));
                deviation = Math.Sqrt(sum / toCalc.Count() - 1);
                double test = toCalc.Average();
            }
            return deviation;
        }

        /// <summary>
        /// Calculates the standard deviation of a list of doubles
        /// </summary>
        /// <param name="toCalc">initial list to calculate from</param>
        /// <param name="average">passable value for the average</param>
        /// <returns>double representation of the standard deviation</returns>
        private static double CalculateStandardDeviation(List<double> toCalc, double average = 0)
        {
            double deviation = 0;

            if (toCalc.Any())
            {
                average = average == 0 ? toCalc.Average() : average;
                double sum = toCalc.Sum(x => Math.Pow(x - average, 2));
                deviation = Math.Sqrt(sum / toCalc.Count() - 1);
            }
            return deviation;
        }

        #endregion
    }



    public enum RejectionType
    {
        NoRejection,
        MinMaxClipping,
        PercentileClipping,
        SigmaClipping,
        WinsorizedSigmaClipping,
        AveragedSigmaClipping,
        BelowThresholdRejection
    }

    public enum WeightingType
    {
        NoWeight,
        NormalDistribution,
        CauchyDistribution,
        GammaDistribution,
        PoissonDistribution,
    }

    public enum SpectrumMergingType
    {
        SpectrumBinning,
        MostSimilarSpectrum
    }
}
