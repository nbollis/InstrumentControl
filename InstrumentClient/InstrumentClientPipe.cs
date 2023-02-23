using ClientServerCommLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace InstrumentClient
{
    public class ClientPipe
    {
        // Events
        public event EventHandler<PipeEventArgs> PipeDataReceived;

        // Public properties
        public NamedPipeClientStream ReadDataPipe { get; }
        public NamedPipeServerStream SendDataPipe { get; }
        public ConcurrentQueue<SingleScanDataObject> ScanInstructionsQueue { get; }
        public bool InstrumentConnectedBool { get; private set; }
        public int[] ScanOrdersToSendToServer { get; }
        private int ScansInOut { get; set; } = 0; 

        public event EventHandler DataSentToServer;
        public event EventHandler InstructionsReceivedFromServer;
        // Constructors
        public ClientPipe(NamedPipeClientStream readDataPipe, NamedPipeServerStream sendDataPipe, int[] scanOrdersToSend)
        {
            ReadDataPipe = readDataPipe;
            ScanInstructionsQueue = new ConcurrentQueue<SingleScanDataObject>();
            ScanOrdersToSendToServer = scanOrdersToSend;
            SendDataPipe = sendDataPipe; 
        }

        public void StartClient(IInstrument instrument)
        {
            BeginInstrumentConnection(instrument);
            instrument.ScanReceived += ScanReceivedFromInstrument;

            InstructionsReceivedFromServer += (o, e) =>
            {
                ScansInOut++; 
            };

            DataSentToServer += (o, e) =>
            {
                ScansInOut--; 
            }; 

            while (ReadDataPipe.IsConnected)
            {
                while (ScanInstructionsQueue.Count > 0)
                {
                    bool success = ScanInstructionsQueue.TryDequeue(out SingleScanDataObject ssdo);
                    if (success)
                        instrument.SendScanAction(ssdo);
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

            // increments when scan instructions were received from the server
            var handler = InstructionsReceivedFromServer; 
            if(handler != null)
            {
                handler.Invoke(this, EventArgs.Empty); 
            }

            ScanInstructionsQueue.Enqueue(ssdo);
            PrintoutMessage.Print(MessageSource.Client, "Instructions received from server");
        }

        /// <summary>
        /// Method for returning data to the server
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sender"></param>
        public void SendScanToServer(SingleScanDataObject ssdo)
        {
            string temp = JsonConvert.SerializeObject(ssdo);
            byte[] buffer = Encoding.UTF8.GetBytes(temp);
            byte[] length = BitConverter.GetBytes(buffer.Length);
            byte[] finalBuffer = length.Concat(buffer).ToArray();
            SendDataPipe.Write(finalBuffer, 0, finalBuffer.Length);
            SendDataPipe.WaitForPipeDrain();

            PrintoutMessage.Print(MessageSource.Client, $"Scan sent to server - Scan Number {ssdo.ScanNumber}");
        }

        public async Task ConnectClientToServer()
        {
            var readerConnectResultsAsync = ReadDataPipe.ConnectAsync().ContinueWith(_ =>
            {
                PrintoutMessage.Print(MessageSource.Client, "Instrument client ready to read data from workflow server.");
            });
            

            var senderConnectionResult = SendDataPipe.WaitForConnectionAsync().ContinueWith(_ =>
            {
                PrintoutMessage.Print(MessageSource.Client, "Instrument client ready to send data to workflow server.");
            });
            readerConnectResultsAsync.Wait();
            senderConnectionResult.Wait(); 

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

            ReadDataPipe.ReadAsync(byteDataLength, 0, sizeof(int))
                .ContinueWith(t =>
                {
                    int len = t.Result;

                    int dataLength = BitConverter.ToInt32(byteDataLength, 0);
                    byte[] data = new byte[dataLength];

                    ReadDataPipe.ReadAsync(data, 0, dataLength)
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

        public void ScanReceivedFromInstrument(object obj, MsScanArrivedEventArgs sender)
        {
            var ssdo = sender.Ssdo;
            if (ScanOrdersToSendToServer.Contains(ssdo.MsNOrder))
            {
                // this means that the scan is MS1
                SendScanToServer(ssdo);
            }
            else
            {
                // fire event if not MS1
                var handler = DataSentToServer;
                if (handler != null)
                {
                    handler.Invoke(this, EventArgs.Empty);
                }
            }
            
        }

        #endregion


    }
}
