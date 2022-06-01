using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;

namespace InstrumentControl
{
    public abstract class Application
    {
        #region Properties

        protected MyApplication ApplicationType { get; set; }

        #endregion


        public Application(MyApplication applicationType)
        {
            ApplicationType = applicationType;
        }

        public abstract void ProcessScans(object? sender, ThresholdReachedEventArgs e);
    }
}
