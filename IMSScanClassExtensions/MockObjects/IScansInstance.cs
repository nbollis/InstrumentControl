using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.Control;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Scans;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;
using Thermo.Interfaces.SpectrumFormat_V1;

namespace IMSScanClassExtensions
{
    public class IScansInstance : IScans
    {

        public event EventHandler PossibleParametersChanged;
        public event EventHandler CanAcceptNextCustomScan;

        #region Not Yet Implemented
        public IParameterDescription[] PossibleParameters => throw new NotImplementedException();

        public bool CancelCustomScan()
        {
            throw new NotImplementedException();
        }

        public bool CancelRepetition()
        {
            throw new NotImplementedException();
        }
        public IRepeatingScan CreateRepeatingScan()
        {
            throw new NotImplementedException();
        }
        public bool SetRepetitionScan(IRepeatingScan scan)
        {
            throw new NotImplementedException();
        }
        #endregion

        public ICustomScan CreateCustomScan()
        {
            return (ICustomScan) new ICustomScanInstance();
        }

        public void Dispose()
        {
            return;
        }

        public bool SetCustomScan(ICustomScan scan)
        {
            return true;
        }

       
    }
}
