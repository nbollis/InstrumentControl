using System;
using System.Runtime.CompilerServices;
using System.Threading;
using ClientServerCommunication;
using Data; 
using Thermo.Interfaces.FusionAccess_V1;
using Thermo.Interfaces.FusionAccess_V1.MsScanContainer;
using Thermo.Interfaces.InstrumentAccess_V1.Control;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Acquisition;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;
using Thermo.TNG.Factory;
using ScanInstructions = ClientServerCommunication.ScanInstructions;

namespace ClientServer
{
    public class ThermoTribrid : IInstrument
    {
        public string InstrumentID { get; private set; }
        public string InstrumentName { get; private set; }
        public IFusionInstrumentAccessContainer InstAccessContainer { get; private set; }
        public IFusionInstrumentAccess InstAccess { get; private set; }
        public IFusionMsScanContainer MsScanContainer { get; private set; }
        public IAcquisition InstAcq { get; private set; }
        public IControl InstControl { get; private set; }
        public ClientPipe ClientPipe { get; set; }
        // public event ServiceConnected 
        // public event InstrumentConnected
        public ThermoTribrid()
        {
            InstAccessContainer = Factory<IFusionInstrumentAccessContainer>.Create(); 
        }
        
        public void OpenInstrumentConnection()
        {
            InstAccessContainer.StartOnlineAccess();
            // hold while instrument is connecting
            while (!InstAccessContainer.ServiceConnected)
            {
            }
        }

        public void CloseInstrumentConnection()
        {
            InstAccessContainer.Dispose();
        }
        public void EnterMainLoop()
        {
            int p = 1;
            GetInstAccess(p);
            // link delegates to events
            MsScanContainer.MsScanArrived += MsScanArrived;
            
        }

        private void GetInstAccess(int p)
        {
            InstAccess = InstAccessContainer.Get(p);
            // do not change order. InstAccess must be filled first as the other
            // properties depend on it to be filled themselves.
            InstControl = InstAccess.Control;
            InstAcq = InstControl.Acquisition;
            InstrumentID = InstAccess.InstrumentId.ToString();
            InstrumentName = InstAccess.InstrumentName;
            MsScanContainer = InstAccess.GetMsScanContainer(0); 
        }

        private void MsScanArrived(object sender, MsScanEventArgs e)
        {
            ProcessScan(e); 
        }
        private void ProcessScan(MsScanEventArgs e)
        {
            SingleScanDataObject scan = e.GetScan().ConvertToSingleScanDataObject(); 
            
            scan.WriteSingleScanToPipe(ClientPipe);
        }
        public void SendScanToInstrument(ScanInstructions scanInstructions)
        {
            throw new NotImplementedException(); 
        }
        public void MsScanArrived()
        {

        }
        public void AddClientPipe(ClientPipe clientPipe)
        {
            ClientPipe = clientPipe; 
        }
    }
}
