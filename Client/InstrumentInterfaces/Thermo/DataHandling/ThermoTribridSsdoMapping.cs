using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.InstrumentInterfaces.Thermo.DataHandling
{
    public static class ThermoTribridSsdoMapping
    {
        public static readonly Dictionary<string, string> TrbridToSsdoMapping = new Dictionary<string, string>
        {
            // key = ssdo; value = Tribrid
            {"MinX", "FirstMass"},
            {"MaxX", "LastMass"},
            {"AnalyzerType", "Analyzer"},
            {"ScanType", "ScanType"},
            {"SourceEnergy", "SourceCIDEnergy"},
            {"SrcRFLens", "SrcRFLens"},
            {"Polarity", "Polarity"},
            {"DataType", "DataType"},
            {"IsolationType", "IsolationMode"},
            {"AgcTarget", "AGCTarget"},
            {"MaxIt", "MaxIT"},
            {"Microscans", "Microscans"},
            {"OrbitrapResolution", "OrbitrapResolution"},
            {"IonTrapScanRate", "ScanRate"},
            {"CollisionEnergy", "CollisionEnergy"},
            {"IsolationWidth", "IsolationWidth"},
            {"ActivationType", "ActivationType"},
            {"ChargeStates", "ChargeStates"},
            {"ActivationQ", "ActivationQ"},
            {"PrecursorMass", "PrecursorMass"},
            {"ReactionTime", "ReactionTime"},
            {"ReagentMaxIT", "ReagentMaxIT"}, 
            {"ReagentAGCTarget", "ReagentAGCTarget"}
        };
    }
}
