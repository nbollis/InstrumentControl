using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;
using System.IO.Pipes;
using Microsoft.Win32;


namespace InstrumentClient
{
    public class ClientPipe
    {
        // Events
        public event EventHandler<EventArgs> PipeConnected;
        public event EventHandler<PipeEventArgs> PipeDataReceived;
        public event EventHandler<ScanQueueThresholdReachedEventArgs> ScanQueueThresholdReached;

        // Public properties
        public NamedPipeClientStream PipeClient { get; set; }
        public Queue<SingleScanDataObject> ScanInstructionsQueue { get; set; }
        public int ScanQueueThreshold { get; set; }
        // Private properties
        private bool InstrumentConnectedBool { get; set; }
        // Constructors
        public ClientPipe(NamedPipeClientStream pipeClient,
            int ms1ScanQueueThreshold = 0,
            int ms2ScanQueueThreshold = 0,
            ProcessMs1ScansDelegate ms1Delegate = null,
            ProcessMs2ScansDelegate ms2Delegate = null)
        {
            PipeClient = pipeClient;
        }

        #region Client To Server Methods
        public void ConnectClientToServer()
        {
            PipeConnected += (obj, sender) =>
            {
                Console.WriteLine("Client pipe connected to server.");
            };
            var connectionResult = PipeClient.ConnectAsync();
            connectionResult.Wait();
            // invokes the pipe connected event
            connectionResult.ContinueWith(i => OnPipeConnected());

            StartReaderAsync();
            PipeDataReceived += HandleDataReceived;
        }
        public void StartReaderAsync()
        {
            StartByteReaderAsync((b) =>
                PipeDataReceived?.Invoke(this, new PipeEventArgs(b)));
        }
        public void OnPipeConnected()
        {
            var handler = new EventHandler<EventArgs>(PipeConnected);
            if (handler != null)
            {
                handler.Invoke(this, EventArgs.Empty);
                Console.WriteLine("Instrument client connected to workflow server."); 
            }
        }
        /// <summary>
        /// Handles server to client data transfer. 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventArgs"></param>
        private void HandleDataReceived(object obj, PipeEventArgs eventArgs)
        {
            Console.WriteLine("Client: Handling data received...");
            // convert the PipeEventArgs to a SingleScanDataObject
            var ssdo = eventArgs.ToSingleScanDataObject();
            if(ssdo == null)
            {
                return; 
            }

            ScanInstructionsQueue.Enqueue(ssdo);
        }

        private void StartByteReaderAsync(Action<byte[]> packetReceived)
        {
            byte[] byteDataLength = new byte[sizeof(int)];

            PipeClient.ReadAsync(byteDataLength, 0, sizeof(int))
                .ContinueWith(t =>
                {
                    int len = t.Result;

                    int dataLength = BitConverter.ToInt32(byteDataLength, 0);
                    byte[] data = new byte[dataLength];

                    PipeClient.ReadAsync(data, 0, dataLength)
                        .ContinueWith(t2 =>
                        {
                            len = t2.Result;
                            packetReceived(data);
                            StartByteReaderAsync(packetReceived);
                        });
                });
        }
        #endregion


        #region Client to Instrument Methods
        public void BeginInstrumentConnection(IInstrument instr)
        {
            bool instrReadyToReceiveScan = false; 
            instr.OpenInstrumentConnection();
            instr.InstrumentConnected += (obj, sender) => { InstrumentConnectedBool = true; };
            instr.InstrumentDisconnected += (obj, sender) => { InstrumentConnectedBool = false; };
           
            instr.ScanReceived += (obj, sender) =>
            {
                // send scan to the server. 

            };
            
            ScanQueueThresholdReached += (obj, sender) =>
            {
                // send the scan to the instrument
            };

            // enter instrument main routine: 
            while (InstrumentConnectedBool)
            {

            }
        }
        #endregion
    }
}
