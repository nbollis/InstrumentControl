using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommunication;

namespace ClientServer
{
    internal class Program
    {
        private ClientPipe _clientPipe; 
        event EventHandler<ScanInstructionsReceivedEventArgs> 
        void Main(string[] args)
        {
            // fire up the pipe client
            string pipeName = args[0];
            StartClientPipe(pipeName); 
            string instrumentType = args[1];
            IInstrumentFactory factory = null; 
            switch (instrumentType)
            {
                case "qe":
                    factory = new ThermoQEFactory();
                    break;
                case "tribrid":
                    factory = new ThermoTribridFactory();
                    break; 
            }

            IInstrument instrumentApi = factory?.CreateInstrumentApi(); 
            instrumentApi?.AddClientPipe(_clientPipe);
            instrumentApi?.OpenInstrumentConnection();
            instrumentApi?.EnterMainLoop();
            instrumentApi?.CloseInstrumentConnection();
        }

        private void StartClientPipe(string pipeName)
        {
            _clientPipe = new ClientPipe(".", pipeName,
                p => p.StartByteReaderAsync());
            _clientPipe.DataReceived += (sender, args) =>
            {
                // deserialize to an instructions object
                _clientPipe.AddScansToQueue(_clientPipe.DeserializeByteStream<ScanInstructions>(args.Data));
                // if( scanInstructions != null ) clientPipe.CreateAndRunCustomScan(scanInstructions); 
            }; 
        }

        public void OnScanInstructionsReceived()
        {

        }
    }

    public class ScanInstructionsReceivedEventArgs : EventArgs
    {
        public ScanInstructions ScanInstr { get; set; }
    }
}
