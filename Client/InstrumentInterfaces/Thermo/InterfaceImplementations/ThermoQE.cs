using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommunication;

namespace InstrumentClient
{
    internal class ThermoQE : IInstrument
    {
        public ClientPipe PipeClient { get; set; }

        public void OpenInstrumentConnection()
        {
            throw new System.NotSupportedException();
        }

        public void CloseInstrumentConnection()
        {
            throw new NotSupportedException(); 
        }

        public void MsScanReadyToSend(MsScanReadyToSendEventArgs scanEventArgs)
        {
            throw new NotImplementedException();
        }

        public void EnterMainLoop()
        {
            throw new NotImplementedException();
        }

        public void SendScanInstructionsToInstrument()
        {
            throw new NotImplementedException();
        }

        public void SendScanToServer(object sender, MsScanReadyToSendEventArgs eventArgs)
        {
            throw new NotImplementedException();
        }
    }
}
