using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;
using System.IO.Pipes;
using Microsoft.Win32;
using Newtonsoft.Json;


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
        public Queue<SingleScanDataObject> InstrumentScanQueue { get; set; }
        public int ScanQueueThreshold { get; set; }
        // Private properties
        public bool InstrumentConnectedBool { get; set; }
        // Constructors
        public ClientPipe(NamedPipeClientStream pipeClient,
            ProcessMs1ScansDelegate ms1Delegate = null,
            ProcessMs2ScansDelegate ms2Delegate = null)
        {
            PipeClient = pipeClient;
            ScanInstructionsQueue = new Queue<SingleScanDataObject>();
            InstrumentScanQueue = new Queue<SingleScanDataObject>();
        }
        /// <summary>
        /// Wrapper around enqueue method that also check to see if the scan 
        /// added to the queue ends up triggering the ScanQueueThresholdReached event. 
        /// </summary>
        public void EnqueueInstrumentScan(SingleScanDataObject ssdo)
        {
            InstrumentScanQueue.Enqueue(ssdo); 
            if(InstrumentScanQueue.Count >= ScanQueueThreshold)
            {
                var ssdoList = InstrumentScanQueue.DequeueChunk(chunkSize: ScanQueueThreshold); 
                var handler = ScanQueueThresholdReached;
                if(handler != null)
                {
                    handler.Invoke(this, new ScanQueueThresholdReachedEventArgs(ssdoList));
                }
            }
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
            // convert the PipeEventArgs to a SingleScanDataObject
            var ssdo = eventArgs.ToSingleScanDataObject();
            if(ssdo == null)
            {
                return; 
            }

            ScanInstructionsQueue.Enqueue(ssdo);
            Console.WriteLine("Client: Instructions received from server");
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

        }
        #endregion
        /// <summary>
        /// Method for returning data to the client
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sender"></param>
        public void SendDataThroughPipe(SingleScanDataObject ssdo)
        {
            string temp = JsonConvert.SerializeObject(ssdo);
            byte[] buffer = Encoding.UTF8.GetBytes(temp);
            byte[] length = BitConverter.GetBytes(buffer.Length);
            byte[] finalBuffer = length.Concat(buffer).ToArray();
            PipeClient.Write(finalBuffer, 0, finalBuffer.Length);
            PipeClient.WaitForPipeDrain();
        }
    }
}
