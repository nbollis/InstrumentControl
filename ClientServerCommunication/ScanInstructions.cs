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
        public CustomOrRepeatingScan? CustomOrRepeating {get ; set; }
        public double? FirstMass { get; set; }
        public double? LastMass { get; set; }
        public AnalyzerType? Analyzer { get; set; }
        public ScanType? ScanType { get; set; }
        public double? SourceCIDEnergy { get; set; }
        public double? SrcRFLens { get; set; }
        public Polarity? Polarity { get; set; }
        public DataType? DataType { get; set; }
        public IsolationMode? IsolationMode { get; set; }
        public double? AGCTarget { get; set; }
        public double? MaxIT { get; set; }
        public int? Microscans { get; set; }
        public OrbitrapResolution? OrbitrapResolution { get; set; }
        public ScanRate? ScanRate { get; set; } //TODO: I changed this from an unknown Enum to double
        public double? CollisionEnergy { get; set; }
        public double? IsolationWidth { get; set; }
        public ActivationType? ActivationType { get; set; }
        public string ChargeStates { get; private set; }
        public double? ActivationQ { get; set; }
        public double? PrecursorMass { get; set; }
        public double? ReactionTime { get; set; }
        public double? ReagentMaxIT { get; set; }
        public double? ReagentAGCTarget { get; set; }
        private static string[] valueNames = {"CollisionEnergy",
                                    "ScanRate",
                                    "FirstMass",
                                    "LastMass",
                                    "Analyzer",
                                    "ScanType",
                                    "Polarity",
                                    "DataType",
                                    "SrcRFLens",
                                    "SourceCIDEnergy",
                                    "IsolationMode",
                                    "OrbitrapResolution",
                                    "IsolationWidth",
                                    "ActivationType",
                                    "ChargeStates",
                                    "ActivationQ",
                                    "AGCTarget",
                                    "MaxIT",
                                    "Microscans",
                                    "PrecursorMass",
                                    "ReactionTime",
                                    "ReagentMaxIT",
                                    "ReagentAGCTarget"};
        public void SetChargeStates(int[] chargeStates)
        {
            ChargeStates = string.Join(",", chargeStates);
        }
        #endregion

        public ScanInstructions()
        {
      
        }
        public IDictionary<string,string> ToThermoTribridCompatibleDictionary()
        {

            Dictionary<string, string> thermoDict = new Dictionary<string, string>();
            var properties = typeof(ScanInstructions).GetProperties(); 
            foreach (string key in valueNames)
            {
                object temp = properties.First(i => i.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase)).GetValue(this); 
                if(temp != null)
                {
                    if (key == "OrbitrapResolution")
                    {
                        temp = (int)temp; 
                    }
                    // skip CustomOrRepeatingScan property
                    if(key == "CustomOrRepeatingScan")
                    {
                        continue; 
                    }

                    thermoDict.Add(key, temp.ToString()); 
                }
            }
            return thermoDict; 

                        
        }
    }
}
