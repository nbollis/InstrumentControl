using ClientServerCommunication;
using System; 
using Data;
using Thermo.Interfaces.InstrumentAccess_V2.MsScanContainer;
using ScanInstructions = Data.ScanInstructions;

namespace Client
{
    /// <summary>
    /// Defines the common interface to all instrument types. 
    /// </summary>
    public interface IInstrument
    {
        ClientPipe PipeClient { get; set; }
        void OpenInstrumentConnection();
        void CloseInstrumentConnection();
        void MsScanReadyToSend(MsScanReadyToSendEventArgs scanEventArgs);
        void EnterMainLoop();
        void SendScanInstructionsToInstrument(); 
        // scan is going to require yet another abstract factory
        void SendScanToServer(object sender, MsScanReadyToSendEventArgs eventArgs); 
        // TODO: Write the LCMS loading methods. 
    }

    public class MsScanReadyToSendEventArgs : EventArgs
    {
        public SingleScanDataObject ScanData { get; set; }

        public MsScanReadyToSendEventArgs(SingleScanDataObject scan)
        {
            ScanData = scan; 
        }
    }

    public class ScanInstructionsEventArgs : EventArgs
    {
        public Data.ScanInstructions ScanInstructions { get; set; }
        public ScanInstructionsEventArgs(ScanInstructions scanInstructions)
        {
            ScanInstructions = scanInstructions;
        }
    }
}