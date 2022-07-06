using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskInterfaces;

namespace ScanProduction
{
    public class IAllScanOptions : ITaskOptions
    {
        public string CollisionEnergy { get; set; }
        public ScanRate ScanRate { get; set; }
        public string FirstMass { get; set; }
        public string LastMass { get; set; }
        public Analyzer Analyzer { get; set; }  
        public ScanType ScanType { get; set; }
        public Polarity Polarity { get; set; }
        public DataType DataType { get; set; }
        public double SrcRFLens { get; set; }
        public double SourceCIDEnergy { get; set; }
        public IsolationMode IsolationMode { get; set; }
        public OrbitrapResolution OrbitrapResolution { get; set; }
        public double IsolationWidth { get; set; }
        public ActivationType ActivationType { get; set; }
        public string ChargeStates { get; set; }
        public string ActivationQ { get; set; }
        public int AGCTarget { get; set; }
        public int Microscans { get; set; }
        public string? PrecursorMass { get; set; }
        public string? ReactionTime { get; set; }
        public string? ReagentMaxIT { get; set; }
        public string? ReagentAGCTarget { get; set; }
    }

    public class AllScanOptions : IAllScanOptions 
    {
        public string CollisionEnergy { get; set; }
        public ScanRate ScanRate { get; set; }
        public string FirstMass { get; set; }
        public string LastMass { get; set; }
        public Analyzer Analyzer { get; set; }
        public ScanType ScanType { get; set; }
        public Polarity Polarity { get; set; }
        public DataType DataType { get; set; }
        public double SrcRFLens { get; set; }
        public double SourceCIDEnergy { get; set; }
        public IsolationMode IsolationMode { get; set; }
        public OrbitrapResolution OrbitrapResolution { get; set; }
        public string IsolationWidth { get; set; }
        public ActivationType ActivationType { get; set; }
        public string ChargeStates { get; set; }
        public string ActivationQ { get; set; }
        public int AGCTarget { get; set; }
        public double MaxIT { get; set; }
        public int Microscans { get; set; }
        public string? PrecursorMass { get; set; }
        public string? ReactionTime { get; set; }
        public string? ReagentMaxIT { get; set; }
        public string? ReagentAGCTarget { get; set; }

        public void SetDefaults()
        {
            ScanRate = ScanRate.Normal;
            FirstMass = "150";
            LastMass = "2000";
            Analyzer = Analyzer.IonTrap;
            ScanType = ScanType.Full;
            Polarity = Polarity.Postive;
            DataType = DataType.Centroid;
            SrcRFLens = 60;
            SourceCIDEnergy = 0;
            IsolationMode = IsolationMode.Quadrupole;
            OrbitrapResolution = OrbitrapResolution.X_120000;
            IsolationWidth = "0.7";
            ActivationType = ActivationType.CID;
            ActivationQ = "0.25";
            AGCTarget = 3000;
            MaxIT = 100;
            Microscans = 1;
            ReactionTime = "10";
            ReagentAGCTarget = "30000";
        }
    }

}
