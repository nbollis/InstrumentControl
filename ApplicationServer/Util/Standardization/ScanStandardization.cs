using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer
{
    public static class ScanStandardization
    {
       // public static double[] GetStandardizedYArray(double )


        public static double[] GetStandardizedYArray(double[] originalYArray, StandardizationOptions options)
        {
            double[] newXArray = CreateStandardMZAxis((options.MinMass, options.MaxMass), options.Delta);
            double[] newYArray = new double[originalYArray.Length];
            ResampleDataAndInterpolate(originalYArray, ref newYArray);
            return newYArray;
        }

        public static double[] CreateStandardMZAxis((double, double) range, double massAccuracy)
        {
            // mz space between the high and low values
            int lowValue = (int)Math.Floor(range.Item1);
            int highValue = (int)Math.Ceiling(range.Item2 + 1);
            int diff = highValue - lowValue;
            int numberOfMzElements = (int)Math.Ceiling(diff / massAccuracy);

            double[] standardMzAxis = new double[numberOfMzElements];
            // this is faster than the equivalent Linq method
            for (int i = 0; i < numberOfMzElements; i++)
            {
                standardMzAxis[i] = lowValue + i * massAccuracy;
            }
            return standardMzAxis;
        }
        /// <summary>
        /// Used to take the original data to the standardized mz axis before interpolation 
        /// </summary>
        /// <param name="source"></param><summary>The original y array</summary>
        /// <param name="destination"></param><summary>The new y array with length of the standard mz axis.</summary>
        public static void ResampleDataAndInterpolate(double[] source, ref double[] destination)
        {
            destination[0] = source[0];
            int previousJ = 0;
            int j = 0;
            int i = 1;
            for (; i < source.Length; i++)
            {
                j = i * (destination.Length - 1) / (source.Length - 1);
                Interpolate(destination, previousJ, j, source[i - 1], source[i]);

                previousJ = j;
            }
            destination[destination.Length - 1] = source[(int)i - 1];
        }

        public static void Interpolate(double[] dest, int destFrom, int destTo,
            double valueFrom, double valueTo)
        {
            double destLength = destTo - destFrom;
            double valueLength = valueTo - valueFrom;
            for (int i = 0; i < destLength; i++)
            {
                dest[(int)destFrom + i] = valueFrom + valueLength * i / destLength;
            }
        }

        public static double LinearInterpolate(double x1, double y1, double x2, double y2, double x)
        {
            return y1 + (y2 - y1) * (x - x1) / (x2 - x1);
        }
    }
}
