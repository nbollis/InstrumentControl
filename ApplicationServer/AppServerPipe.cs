using System;
using System.IO.Pipes; 
using System.Text; 
using Newtonsoft.Json; 
using ClientServerCommLibrary;

namespace WorkflowServer
{
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

        public void StartServer()
        {
            PipeConnected += (obj, sender) =>
            {
                Console.WriteLine("Pipe client connected. Sent from event.");
            };
            // delegate for processing needs to be used as a function. 
            //ProcessMs1ScansDelegate ms1Del = (o, scans) =>
            //{
            //    // select highest m/z from the scans and send a singlescandataobject back to client
            //    List<double> mzPrecursors = new();
            //    foreach (var sc in scans.ListSsdo)
            //    {
            //        double max = sc.YArray.Max();
            //        int posX = Array.IndexOf(sc.YArray, max);
            //        mzPrecursors.Add(sc.XArray[posX]);
            //    }

            //    foreach (var mz in mzPrecursors)
            //    {
            //        SingleScanDataObject ssdoTemp = new()
            //        {
            //            ScanOrder = 2,
            //            ScanNumber = 10,
            //            PrecursorScanNumber = 3,
            //            MzPrecursor = 15,
            //            XArray = new double[] { 0, 0 },
            //            YArray = new double[] { 0, 0 }
            //        };
            //        string temp = JsonConvert.SerializeObject(ssdoTemp);
            //        byte[] buffer = Encoding.UTF8.GetBytes(temp);
            //        byte[] length = BitConverter.GetBytes(buffer.Length);
            //        byte[] finalBuffer = length.Concat(buffer).ToArray();
            //        PipeServer.Write(finalBuffer, 0, finalBuffer.Length);
            //    }

            //};

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
        private void OnMs1QueueThresholdReached(Queue<SingleScanDataObject> queue)
        {
            Ms1ScanQueueThresholdReached?.Invoke(this,
                new ScanQueueThresholdReachedEventArgs(queue.DequeueChunk(Ms1ScanQueueThreshold)));
        }
        private void OnMs2QueueThresholdReached(Queue<SingleScanDataObject> queue)
        {
            Ms2ScanQueueThresholdReached?.Invoke(this,
                new ScanQueueThresholdReachedEventArgs(queue.DequeueChunk(Ms2ScanQueueThreshold)));
        }
        private void Connected(IAsyncResult ar)
        {
            OnConnection();
            PipeServer.EndWaitForConnection(ar);
        }
        private void OnConnection()
        {
            PipeConnected?.Invoke(this, EventArgs.Empty);
        }
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
        public void StartReaderAsync()
        {
            StartByteReaderAsync((b) =>
                PipeDataReceived?.Invoke(this, new PipeEventArgs(b)));
        }
    }
}
