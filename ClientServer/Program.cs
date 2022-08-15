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
        static void Main(string[] args)
        {
            // fire up the pipe client
            string serverPipeName = args[0];
            string pipeName = args[1];
            ClientPipe clientPipe = new ClientPipe(serverPipeName, pipeName,
                p => p.StartByteReaderAsync()); 

            string instrumentType = args[2];
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
            instrumentApi?.OpenInstrumentConnection();
            instrumentApi?.EnterMainLoop();

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
            }
        }
        
    }
}
