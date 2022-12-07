using System;
using System.Collections;
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
            double? mzPrecursor = null,
            double retentionTime = 0)
        {
            MsNOrder = msNOrder;
            ScanNumber = scanNumber;
            XArray = xArray;
            YArray = yArray;
            PrecursorScanNumber = precursorScanNumber;
            MzPrecursor = mzPrecursor;
            RetentionTime = retentionTime;
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

        public override bool Equals(object x)
        {
            var scanDataObject = x as SingleScanDataObject;
            if (scanDataObject == null ) return false;
            if (!scanDataObject.XArray.SequenceEqual(XArray)) return false;
            if (!scanDataObject.YArray.SequenceEqual(YArray)) return false;
            if (scanDataObject.MsNOrder != MsNOrder) return false;
            if (scanDataObject.ScanNumber != ScanNumber) return false;   
            if (Math.Abs(scanDataObject.RetentionTime - RetentionTime) > 0.001) return false;
            if (scanDataObject.PrecursorScanNumber != PrecursorScanNumber) return false;
            if (scanDataObject.MzPrecursor != MzPrecursor) return false;
            return true;
        }

        public int GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
