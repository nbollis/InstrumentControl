using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommunication;
using InstrumentControlIO;


namespace ClientServer
{
    internal class Program
    {
        private ClientPipe _clientPipe;
        private Queue<ScanInstructions> _scanInstrQueue = new Queue<ScanInstructions>();  
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
            _clientPipe.Connect();
            _clientPipe.PipeConnected += (obj, e) =>
            {
                Console.WriteLine("Client connected to server.");
            };
            
            ScanAddedToQueue += (obj, sender) =>
            {
                instrumentApi?.SendScanToInstrument(_scanInstrQueue.Dequeue());
            };
            try
            {
                instrumentApi?.EnterMainLoop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                instrumentApi?.CloseInstrumentConnection();
            }
        }
        private void StartClientPipe(string pipeName)
        {
            _clientPipe = new ClientPipe(".", pipeName,
                p => p.StartByteReaderAsync());
            _clientPipe.DataReceived += (sender, args) =>
            {
                // deserialize to an instructions object
                _clientPipe.AddScansToQueue(_clientPipe.DeserializeByteStream<ScanInstructions>(args.Data));
                OnScanAddedToQueue(EventArgs.Empty);
                // if( scanInstructions != null ) clientPipe.CreateAndRunCustomScan(scanInstructions); 
            }; 
        } 
        public void OnScanAddedToQueue(EventArgs e)
        {
            EventHandler handler = ScanAddedToQueue; 
            handler?.Invoke(this, e);
        }

        public event EventHandler ScanAddedToQueue; 
    }

    public class ScanInstructionsReceivedEventArgs : EventArgs
    {
        public ScanInstructions ScanInstr { get; set; }
    }
}
