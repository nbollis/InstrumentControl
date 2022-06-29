using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanProduction
{
    public interface IMS2Scan : IBaseScan
    {
        public ActivationType ActivationType { get; set; }
        public double CollisionEnergy { get; set; }
        public double IsolationWidth { get; set; }
        public int[] ChargeStates { get; set; }
        public double ActivationQ { get; set; }
        public double PrecursorMass { get; set; }
        public double ReactionTime { get; set; }
        public double ReagentMaxIT { get; set; }
        public double ReagentAGCTarget { get; set; }


        // able to only access parameters associated with MS2 scans. 
        public void SetCustomValue<T>(InstrumentSettings setting, T valueToSet)
        {
            if ((int)setting <= 0)
            {
                SetCustomValue(this, setting, valueToSet);
            }
        }
    }
}
