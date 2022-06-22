using IMSScanClassExtensions;
using MassSpectrometry;
using MzLibUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;

namespace InstrumentControl
{
    public interface ISpectraAverager : ISpectraProcesor
    {
        public static MzSpectrum CompositeSpectrum
        {
            get { return  _compositeSpectrum;  }
            set { _compositeSpectrum = value; }
        }
        protected static MzSpectrum _compositeSpectrum;

        #region MsDataScan Variables

        private static int oneBasedScanNumber;
        private static int msnOrder;
        private static bool isCentroid;
        private static MassSpectrometry.Polarity polarity;
        private static double retentionTime;
        private static MzRange scanWindowRange;
        private static string scanFilter;
        private static MZAnalyzerType mzAnalyzer;
        private static double totalIonCurrent;
        private static double injectionTime;
        private static double[,] noiseData;
        private static string nativeId;

        #endregion

        //public static void SetMsDataScanParams(IMsScan scan)
        //{
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
        //}

        public static void ExportCompositeAsMzML()
        {
            MsDataScan scan = new(_compositeSpectrum, oneBasedScanNumber, msnOrder, isCentroid, polarity, retentionTime, 
                scanWindowRange, scanFilter, mzAnalyzer, totalIonCurrent, injectionTime, noiseData, nativeId);
        }

    }
}
