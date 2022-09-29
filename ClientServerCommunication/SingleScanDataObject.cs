using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommLibrary
{
    // TODO: Write validation methods for SingleScanDataObject
    [Serializable]
    public class SingleScanDataObject
    {
        public int ScanOrder { get; set; }
        public int ScanNumber { get; set; }
        public int? PrecursorScanNumber { get; set; }
        public double? MzPrecursor { get; set; }
        public double[] XArray { get; set; }
        public double[] YArray { get; set; }
        public double TotalIonCurrent { get; set; }
        public double MinX { get; set; }
        public double MaxX { get; set; }
        public ScanInstructions ScanInstructions { get; private set; }

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
        public void SetScanInstructions(ScanInstructions si, string instrument)
        {
            if (!ValidateScanInstructionsForInstrument(si, instrument))
            {
                throw new ArgumentException(
                    "Invalid scan instructions for instrument type: {0}", 
                    instrument); 
            }
            SetScanInstructions(si);
        }
        private void SetScanInstructions(ScanInstructions si)
        {
            ScanInstructions = si;
        }
        // TODO: Fill out the method. 
        private bool ValidateScanInstructionsForInstrument(ScanInstructions si, string instrument)
        {
            return true; 
        }
    }

    public enum IsolationType
    {
        Quadropole, 
        IonTrap
    }

    public enum AnalyzerType
    {
        Quadropole, 
        TOF, 
        Orbitrap, 
        IonTrap
    }

    public enum CustomScanType
    {
        RepeatingScan, 
        CustomScan
    }

    public enum InstrumentType
    {
        ThermoTribrid, 
        ThermoQE, 
        WatersTOF,
        BrukerTOF
    }

    public enum ScanType
    {
        Full, 
        SIM, 
        MSn
    }

    public enum Polarity
    {
        Positive, 
        Negative
    }

    public enum DataType
    {
        Profile, 
        Centroid
    }
    public enum OrbitrapResolution : int
    {
        X_7500 = 7500,
        X_15000 = 15000,
        X_30000 = 30000,
        X_50000 = 50000,
        X_60000 = 60000,
        X_120000 = 120000,
        X_240000 = 240000,
        X_500000 = 500000
    }
    public enum IonTrapScanRate
    {
        Normal,
        Enhanced,
        Zoom,
        Rapid,
        Turbo
    }
    public enum ActivationType
    {
        CID,
        HCD,
        ETD,
        EThcD,
        ETciD
    }
}
