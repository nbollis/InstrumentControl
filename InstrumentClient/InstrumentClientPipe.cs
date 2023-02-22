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
        public event EventHandler<PipeEventArgs> PipeDataReceived;

        // Public properties
        public NamedPipeClientStream PipeClient { get; }
        public Queue<SingleScanDataObject> ScanInstructionsQueue { get; }
        public bool InstrumentConnectedBool { get; private set; }
        public int[] ScanOrdersToSendToServer { get; }

        // Constructors
        public ClientPipe(NamedPipeClientStream pipeClient, int[] scanOrdersToSend)
        {
            PipeClient = pipeClient;
            ScanInstructionsQueue = new Queue<SingleScanDataObject>();
            ScanOrdersToSendToServer = scanOrdersToSend;
        }

        public void StartClient(IInstrument instrument)
        {
            BeginInstrumentConnection(instrument);

            bool readyToReceiveScan = true;
            instrument.ScanReceived += SendDataToServer;
            instrument.ReadyToReceiveScanInstructions += (obj, sender) =>
            {
                readyToReceiveScan = true;
            };


            while (PipeClient.IsConnected)
            {
                while (ScanInstructionsQueue.Count > 0 && readyToReceiveScan)
                {
                    readyToReceiveScan = false;
                    instrument.SendScanAction(ScanInstructionsQueue.Dequeue());
                    PrintoutMessage.Print(MessageSource.Client, "Instructions sent to instrument");
                }
            }
        }

        ///// <summary>
        ///// Wrapper around enqueue method that also check to see if the scan 
        ///// added to the queue ends up triggering the ScanQueueThresholdReached event. 
        ///// </summary>
        //public void EnqueueInstrumentScan(SingleScanDataObject ssdo)
        //{
        //    InstrumentScanQueue.Enqueue(ssdo); 
        //    if(InstrumentScanQueue.Count >= ScanQueueThreshold)
        //    {
        //        var ssdoList = InstrumentScanQueue.DequeueChunk(chunkSize: ScanQueueThreshold); 
        //        var handler = ScanQueueThresholdReached;
        //        if(handler != null)
        //        {
        //            handler.Invoke(this, new ScanQueueThresholdReachedEventArgs(ssdoList));
        //        }
        //    }
        //}

        #region Client To Server Methods

        /// <summary>
        /// Handles server to client data transfer. 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventArgs"></param>
        private void HandleDataReceivedFromServer(object obj, PipeEventArgs eventArgs)
        {
            // convert the PipeEventArgs to a SingleScanDataObject
            var ssdo = eventArgs.ToSingleScanDataObject();
            if (ssdo == null)
            {
                return;
            }

            ScanInstructionsQueue.Enqueue(ssdo);
            PrintoutMessage.Print(MessageSource.Client, "Instructions received from server");
        }

        /// <summary>
        /// Method for returning data to the server
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sender"></param>
        public void SendDataToServer(object obj, MsScanArrivedEventArgs sender)
        {
            if (!ScanOrdersToSendToServer.Contains(sender.Ssdo.MsNOrder)) return;

            string temp = JsonConvert.SerializeObject(sender.Ssdo);
            byte[] buffer = Encoding.UTF8.GetBytes(temp);
            byte[] length = BitConverter.GetBytes(buffer.Length);
            byte[] finalBuffer = length.Concat(buffer).ToArray();
            PipeClient.Write(finalBuffer, 0, finalBuffer.Length);
            PipeClient.WaitForPipeDrain();

            PrintoutMessage.Print(MessageSource.Client, $"Scan sent to server - Scan Number {sender.Ssdo.ScanNumber}");
        }

        public void ConnectClientToServer()
        {
            var connectionResult = PipeClient.ConnectAsync();
            connectionResult.Wait();
            PrintoutMessage.Print(MessageSource.Client, "Instrument client connected to workflow server.");

            StartReaderAsync();
            PipeDataReceived += HandleDataReceivedFromServer;
        }

        public void StartReaderAsync()
        {
            StartByteReaderAsync((b) =>
                PipeDataReceived?.Invoke(this, new PipeEventArgs(b)));
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
            instr.InstrumentConnected += (obj, sender) => { InstrumentConnectedBool = true; };
            instr.InstrumentDisconnected += (obj, sender) => { InstrumentConnectedBool = false; };
            instr.OpenInstrumentConnection();
        }
        #endregion

       
    }
}
