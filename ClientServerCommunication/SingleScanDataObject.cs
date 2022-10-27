using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommunication;
using MathNet.Numerics.Statistics;

namespace ClientServerCommLibrary
{
    [Serializable]
    public class SingleScanDataObject
    {
        public ScanInstructions ScanInstructions { get; set; }
        public int MsNOrder { get; set; }
        public int ScanNumber { get; set; }
        public int? PrecursorScanNumber { get; set; }
        public double? MzPrecursor { get; set; }
        public double[] XArray { get; set; }
        public double[] YArray { get; set; }
        public double RetentionTime { get; set; }

        public SingleScanDataObject()
        {

        }
        public SingleScanDataObject(double[] xArray, double[] yArray)
        {
            XArray = xArray;
            YArray = yArray;
        }
        public SingleScanDataObject(int msNOrder, int scanNumber,
            double[] xArray,
            double[] yArray,
            int? precursorScanNumber = null,
            double? mzPrecursor = null)
        {
            MsNOrder = msNOrder;
            ScanNumber = scanNumber;
            XArray = xArray;
            YArray = yArray;
            PrecursorScanNumber = precursorScanNumber;
            MzPrecursor = mzPrecursor;
        }

        public IEnumerable<(double mass, double intensity)> FilterByNumberOfMostIntense(int topNPeaks)
        {
            var quantile = 1.0 - (double)topNPeaks / YArray.Length;
            quantile = Math.Max(0, quantile);
            quantile = Math.Min(1, quantile);
            double cutoffYvalue = YArray.Quantile(quantile);

            for (int i = 0; i < YArray.Length; i++)
            {
                if (YArray[i] >= cutoffYvalue)
                {
                    yield return (XArray[i], YArray[i]);
                }
            }
        }
    }
}
