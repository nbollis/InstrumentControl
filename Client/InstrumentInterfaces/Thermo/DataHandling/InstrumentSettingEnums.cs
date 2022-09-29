using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class InstrumentSettingEnums
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

            // ms1 is only defined by the common settings

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
        
        
        public enum Analyzer
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
        public enum Polarity
        {
            Postive,
            Negative
        }
        public enum DataType
        {
            Centroid,
            Profile
        }
        public enum IsolationMode
        {
            None,
            Quadrupole,
            IonTrap
        }
        
    }
}
