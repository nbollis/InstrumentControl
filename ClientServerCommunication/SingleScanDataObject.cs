using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommunication;

namespace ClientServerCommLibrary
{
    [Serializable]
    public class SingleScanDataObject
    {
        public ScanInstructions ScanInstructions { get; set; }
        public int ScanOrder { get; set; }
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
        public SingleScanDataObject(int scanOrder, int scanNumber,
            double[] xArray,
            double[] yArray,
            int? precursorScanNumber = null,
            double? mzPrecursor = null)
        {
            ScanOrder = scanOrder;
            ScanNumber = scanNumber;
            XArray = xArray;
            YArray = yArray;
            PrecursorScanNumber = precursorScanNumber;
            MzPrecursor = mzPrecursor;
        }
    }
}
