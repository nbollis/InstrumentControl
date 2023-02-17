using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommLibrary
{
    public enum InstrumentSettings
    {
        // common
        FirstMass,
        LastMass,
        Analyzer,
        ScanType,
        SourceCIDEnergy,
        SrcRFLens,
        Polarity,
        DataType,
        IsolationMode,
        AGCTarget,
        MaxIT,
        Microscans,

        // detector resolutions
        OrbitrapResolution,
        ScanRate,

        // ms1 only 

        // ms2 only
        CollisionEnergy,
        IsolationWidth,
        ActivationType,
        ChargeStates,
        ActivationQ,
        PrecursorMass,
        ReactionTime,
        ReagentMaxIT,
        ReagentAGCTarget
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
    public enum ScanRate
    {
        Normal,
        Enhanced,
        Zoom,
        Rapid,
        Turbo
    }
    public enum AnalyzerType
    {
        IonTrap,
        Orbitrap
    }
    public enum ScanType
    {
        Full,
        SIM,
        MSn
    }
    public enum DataType
    {
        Centroid,
        Profile
    }
    public enum IsolationType
    {
        None,
        Quadrupole,
        IonTrap
    }
    public enum ActivationType
    {
        CID,
        HCD,
        ETD,
        EThcD,
        ETciD
    }
    public enum Polarity
    {
        Postive,
        Negative
    }
    public enum CustomOrRepeatingScan
    {
        Custom, 
        Repeating
    }
}
