using IMSScanClassExtensions;
using MassSpectrometry;
using MzLibUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;

namespace InstrumentControlIO
{
    public class MsDataScanBuilder
    {
        public static MzSpectrum CompositeSpectrum;
        public static int oneBasedScanNumber;
        public static int msnOrder;
        public static bool isCentroid;
        public static MassSpectrometry.Polarity polarity;
        public static double retentionTime;
        public static MzRange scanWindowRange;
        public static string scanFilter;
        public static MZAnalyzerType mzAnalyzer;
        public static double totalIonCurrent;
        public static double injectionTime;
        public static double[,] noiseData;
        public static string nativeId;

        public static void SetMsDataScanFields(IMsScan scan)
        {
            //    oneBasedScanNumber = scan.GetValueFromHeaderDict<int>("Scan Number");
            //    msnOrder = scan.GetValueFromHeaderDict<int>();
            //    isCentroid = scan.GetValueFromHeaderDict<int>();
            //    polarity = scan.GetValueFromHeaderDict<int>();
            //    retentionTime = scan.GetValueFromHeaderDict<int>();
            //    scanWindowRange = new MzRange(MinX, MaxX);
            //    scanFilter = scan.GetValueFromHeaderDict<int>();
            //    mzAnalyzer = scan.GetValueFromHeaderDict<int>();
            //    totalIonCurrent = scan.GetValueFromHeaderDict<int>();
            //    injectionTime = scan.GetValueFromHeaderDict<double>("Ion Injection Time (ms)");
            //    noiseData = scan.GetValueFromHeaderDict<int>();
            //    nativeId = scan.GetValueFromHeaderDict<int>();
        }


        public static void ExportCompositeAsMzML()
        {
            MsDataScan scan = new(CompositeSpectrum, oneBasedScanNumber, msnOrder, isCentroid, polarity, retentionTime,
                scanWindowRange, scanFilter, mzAnalyzer, totalIonCurrent, injectionTime, noiseData, nativeId);
        }
    }
}
