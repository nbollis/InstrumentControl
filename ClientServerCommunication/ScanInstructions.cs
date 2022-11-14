using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommLibrary
{
    [Serializable]
    public class ScanInstructions
    {
        #region ThermoTribrid Settings
        public double? FirstMass { get; set; }
        public double? LastMass { get; set; }
        public AnalyzerType? AnalyzerType { get; set; }
        public ScanType? ScanType { get; set; }
        public double? SourceEnergy { get; set; }
        public double? SrcRFLens { get; set; }
        public Polarity? Polarity { get; set; }
        public DataType? DataType { get; set; }
        public IsolationType? IsolationType { get; set; }
        public double? AgcTarget { get; set; }
        public double? MaxIt { get; set; }
        public int? Microscans { get; set; }
        public OrbitrapResolution? OrbitrapResolution { get; set; }
        public double? IonTrapScanRate { get; set; } //TODO: I changed this from an unknown Enum to double
        public double? CollisionEnergy { get; set; }
        public double? IsolationWidth { get; set; }
        public ActivationType? ActivationType { get; set; }
        public string ChargeStates { get; private set; }
        public double? ActivationQ { get; set; }
        public double? PrecursorMass { get; set; }
        public double? ReactionTime { get; set; }
        public double? ReagentMaxIT { get; set; }
        public double? ReagentAGCTarget { get; set; }
        public void SetChargeStates(int[] chargeStates)
        {
            ChargeStates = string.Join(",", chargeStates);
        }
        #endregion

        public ScanInstructions()
        {
      
        }
    }
}
