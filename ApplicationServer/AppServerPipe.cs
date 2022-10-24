using System;
using System.IO.Pipes; 
using System.Text; 
using Newtonsoft.Json; 
using ClientServerCommLibrary;

namespace WorkflowServer
{
    /// <summary>
    /// Class is used to communicate and run processing workflows on data received from the client. 
    /// </summary>
    internal class AppServerPipe
    {
        public event EventHandler<EventArgs> PipeConnected;
        public event EventHandler<PipeEventArgs> PipeDataReceived;
        public event EventHandler<ScanQueueThresholdReachedEventArgs> Ms1ScanQueueThresholdReached;
        public event EventHandler<ScanQueueThresholdReachedEventArgs> Ms2ScanQueueThresholdReached;
        public event EventHandler<ProcessingCompletedEventArgs> Ms1ProcessingCompleted;
        public event EventHandler<ProcessingCompletedEventArgs> Ms2ProcessingCompleted;
        public NamedPipeServerStream PipeServer { get; set; }
        public Queue<SingleScanDataObject> ScanQueueMS1 { get; set; }
        public int Ms1ScanQueueThreshold { get; set; }
        public Queue<SingleScanDataObject> ScanQueueMS2 { get; set; }
        public int Ms2ScanQueueThreshold { get; set; }
        private ProcessMs1ScansDelegate? Ms1Delegate { get; set; }
        private ProcessMs2ScansDelegate? Ms2Delegate { get; set; }
        // static class with processing workflows? 
        public AppServerPipe(NamedPipeServerStream pipeServer,
            int ms1ScanQueueThreshold,
            int ms2ScanQueueThreshold,
            ProcessMs1ScansDelegate ms1Delegate = null,
            ProcessMs2ScansDelegate ms2Delegate = null)
        {
            PipeServer = pipeServer;
            ScanQueueMS1 = new();
            ScanQueueMS2 = new();
            Ms1ScanQueueThreshold = ms1ScanQueueThreshold;
            Ms2ScanQueueThreshold = ms2ScanQueueThreshold;
            Ms1Delegate = ms1Delegate;
            Ms2Delegate = ms2Delegate;
        }

        /// <summary>
        /// Begins the server connection; starts the async buffer reader; connects event handler methods to events. 
        /// </summary>
        public void StartServer()
        {
            PipeConnected += (obj, sender) =>
            {
                Console.WriteLine("Pipe client connected. Sent from event.");
            };
  
            PipeDataReceived += HandleDataReceived;
            Ms1ScanQueueThresholdReached += Ms1Delegate.Invoke;
            Ms2ScanQueueThresholdReached += Ms2Delegate.Invoke;
            Ms1ProcessingCompleted += (object? obj, ProcessingCompletedEventArgs sender) =>
            {

                Console.WriteLine("Ms1 Processing Completed"); 
            };
            Ms2ProcessingCompleted += (object? obj, ProcessingCompletedEventArgs sender) =>
            {

                Console.WriteLine("Ms2 Processing Completed."); 
            };

            var connectionResult = PipeServer.BeginWaitForConnection(Connected, null);
            // wait for the connection to occur before proceeding. 
            connectionResult.AsyncWaitHandle.WaitOne();
            connectionResult.AsyncWaitHandle.Close();
            StartReaderAsync();
        }
        /// <summary>
        /// Used to determine which queue scan that was received from server should go into. 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventArgs"></param>
        /// <exception cref="ArgumentException"></exception>
        private void HandleDataReceived(object? obj, PipeEventArgs eventArgs)
        {
            // convert pipeeventargs to single scan data object
            SingleScanDataObject ssdo = eventArgs.ToSingleScanDataObject();
            if (ssdo == null) throw new ArgumentException("single scan data object is null");

            // TODO: refactor and make handling more robust downstream.
            if (ssdo.ScanOrder == 1)
            {
                ScanQueueMS1.Enqueue(ssdo);
                if (ScanQueueMS1.Count == Ms1ScanQueueThreshold)
                {
                    // method raises event
                    OnMs1QueueThresholdReached(ScanQueueMS1);
                }
            }
            else if (ssdo.ScanOrder == 2)
            {
                ScanQueueMS2.Enqueue(ssdo);
                if (ScanQueueMS2.Count == Ms2ScanQueueThreshold)
                {
                    // method raises handler
                    OnMs2QueueThresholdReached(ScanQueueMS2);
                }
            } // TODO: Handle scan order > 2. 
            Console.WriteLine("\n");
        }
        /// <summary>
        /// Triggers event that begins processing of Ms1 queue. 
        /// </summary>
        /// <param name="queue"></param>
        private void OnMs1QueueThresholdReached(Queue<SingleScanDataObject> queue)
        {
            Ms1ScanQueueThresholdReached?.Invoke(this,
                new ScanQueueThresholdReachedEventArgs(queue.DequeueChunk(Ms1ScanQueueThreshold)));
        }
        /// <summary>
        ///  Triggers event that begins processing of Ms2 queue. 
        /// </summary>
        /// <param name="queue"></param>
        private void OnMs2QueueThresholdReached(Queue<SingleScanDataObject> queue)
        {
            Ms2ScanQueueThresholdReached?.Invoke(this,
                new ScanQueueThresholdReachedEventArgs(queue.DequeueChunk(Ms2ScanQueueThreshold)));
        }
        /// <summary>
        /// Callback method used to wait for client pipe connection. 
        /// </summary>
        /// <param name="ar"></param>
        private void Connected(IAsyncResult ar)
        {
            OnConnection();
            PipeServer.EndWaitForConnection(ar);
        }
        private void OnConnection()
        {
            PipeConnected?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// Begins asynchronous buffer reading. Converts the first four bytes into the integer size of the object contained in the rest of the buffer.
        /// Then creates the buffer of that size and reads the data into the buffer. 
        ///
        /// DO NOT MODIFY OR THINK ABOUT
        /// MODIFYING THIS METHOD UNLESS YOU 100% KNOW WHAT IT DOES AND WHAT YOU ARE CHANGING. 
        /// </summary>
        /// <param name="packetReceived">The Action delegate determines what is done with the byte buffer that is received.</param>
        private void StartByteReaderAsync(Action<byte[]> packetReceived)
        {
            byte[] byteDataLength = new byte[sizeof(int)];
            PipeServer.ReadAsync(byteDataLength, 0, sizeof(int))
                .ContinueWith(t =>
                {
                    int len = t.Result;
                    int dataLength = BitConverter.ToInt32(byteDataLength, 0);
                    byte[] data = new byte[dataLength];

                    PipeServer.ReadAsync(data, 0, dataLength)
                        .ContinueWith(t2 =>
                        {
                            len = t2.Result;
                            packetReceived(data);
                            StartByteReaderAsync(packetReceived);
                        });
                });
        }
        /// <summary>
        /// Starts the reader asynchronously and triggers the PipeDataReceived event when data is received from the pipe. 
        /// </summary>
        public void StartReaderAsync()
        {
            StartByteReaderAsync((b) =>
                PipeDataReceived?.Invoke(this, new PipeEventArgs(b)));
        }

        private void SendScanToClient(object obj, ProcessingCompletedEventArgs sender)
        {
            byte[] buffer = sender.Ssdo.CreateSerializedSingleScanDataObject();
            PipeServer.WriteAsync(buffer, 0, buffer.Length); 
            PipeServer.WaitForPipeDrain();
        }
    }
}
