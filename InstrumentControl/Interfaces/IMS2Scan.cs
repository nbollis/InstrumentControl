using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl.Interfaces
{
    public interface IMS2Scan : IBaseScan
    {
        // able to only access parameters associated with MS2 scans. 
        public void SetCustomValue<T>(InstrumentSettings setting, T valueToSet)
        {
            if ((int)setting <= 0)
            {
                IBaseScan.SetCustomValue(this, setting, valueToSet);
            }
        }
    }
}
