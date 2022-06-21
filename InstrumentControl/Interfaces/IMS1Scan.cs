using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl.Interfaces
{
    public interface IMS1Scan : IBaseScan
    {
        // able to only access parameters associated with MS1 scans. 
        public void SetCustomValue<T>(InstrumentSettings setting, T valueToSet)
        {
            // filter any InstrumentSettings that are disallowed
            if ((int)setting >= 14)
            {
                throw new ArgumentException("Disallowed scan setting for MS1 scan.");
            }
            else
            {
                IBaseScan.SetCustomValue(this, setting, valueToSet);
            }
        }
    }
}
