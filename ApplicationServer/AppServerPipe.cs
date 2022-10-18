using System;
using System.IO.Pipes; 
using System.Text; 
using Newtonsoft.Json; 
using ClientServerCommLibrary;

namespace WorkflowServer
{
    public class AppServerPipe
    {
        public event EventHandler<EventArgs> PipeConnected;
        public event EventHandler<PipeEventArgs> PipeDataReceived;
        public NamedPipeServerStream PipeServer { get; set; }

        private Workflow Workflow { get; set; }

        
        public AppServerPipe(NamedPipeServerStream pipeServer)
        {
            PipeServer = pipeServer;
            PipeDataReceived += HandleDataReceived;
        }

        public void StartServer(Workflow workflow)
        {
            Workflow = workflow;
            PipeConnected += (obj, sender) =>
            {
                Console.WriteLine("Pipe client connected. Sent from event.");
            };

            var connectionResult = PipeServer.BeginWaitForConnection(Connected, null);
            // wait for the connection to occur before proceeding. 
            connectionResult.AsyncWaitHandle.WaitOne();
            connectionResult.AsyncWaitHandle.Close();
            StartReaderAsync();
            while (PipeServer.IsConnected)
            {

            }
        }

        public void SendDataThroughPipe(object? obj, ProcessingCompletedEventArgs sender)
        {
            string temp = JsonConvert.SerializeObject(sender.ssdo);
            byte[] buffer = Encoding.UTF8.GetBytes(temp);
            byte[] length = BitConverter.GetBytes(buffer.Length);
            byte[] finalBuffer = length.Concat(buffer).ToArray();
            PipeServer.Write(finalBuffer, 0, finalBuffer.Length);
            PipeServer.WaitForPipeDrain();
        }

        private void HandleDataReceived(object? obj, PipeEventArgs eventArgs)
        {
            // convert PipeEventArgs to single scan data object
            SingleScanDataObject ssdo = eventArgs.ToSingleScanDataObject();
            if (ssdo == null) throw new ArgumentException("single scan data object is null");

            //TODO add these to the correct scan queue


            //Workflow.ReceiveData(ssdo);
            Console.WriteLine("\n");
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
