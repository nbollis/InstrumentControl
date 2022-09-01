using System;
using Thermo.TNG.Factory;
using Thermo.Interfaces.FusionAccess_V1;
using Thermo.Interfaces.FusionAccess_V1.MsScanContainer;
using Thermo.Interfaces.InstrumentAccess_V1.Control;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Acquisition;


namespace Client
{
    public class ThermoQEFactory : IInstrumentFactory
    {
        public IInstrument Api { get; }
        public ThermoQEFactory()
        {
            Api = CreateInstrumentApi();
        }
        public IInstrument CreateInstrumentApi()
        {
            return new ThermoQE();
        }
    }
}
