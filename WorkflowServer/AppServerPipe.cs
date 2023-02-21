using System;
using System.IO.Pipes;
using System.Text;
using Newtonsoft.Json;
using ClientServerCommLibrary;
using Microsoft.Extensions.DependencyInjection;
using WorkflowServer.Activities;

namespace WorkflowServer
{
    public class AppServerPipe : IPipe
    {
        public event EventHandler<EventArgs> PipeConnected;
        public event EventHandler<PipeEventArgs> PipeDataReceived;

        public NamedPipeServerStream PipeServer { get; set; }

        private IActivityCollection<IActivityContext> activityCollection;
        private SpectraActivityContext spectraActivityContext;
        private ServiceProvider serviceProvider;

        public AppServerPipe(NamedPipeServerStream pipeServer)
        {
            PipeServer = pipeServer;
            PipeDataReceived += HandleDataReceived;
            serviceProvider = new ServiceCollection().BuildServiceProvider();
        }

        public async Task StartServer(string[] startupContext)
        {
            // if the startup context contains the number of required elements
            if (startupContext.Length == 2)
            {
                ParseStartupContext(startupContext);
            }
            else
            {
                GenerateStartupContext();
            }

            PipeConnected += (obj, sender) =>
            {
                Console.WriteLine("Pipe client connected. Sent from event.");
            };
            
            var asyncResult = PipeServer.BeginWaitForConnection(Connected, null);
            asyncResult.AsyncWaitHandle.WaitOne();
            asyncResult.AsyncWaitHandle.Close(); 
            
            // wait for the connection to occur before proceeding. 
            StartReaderAsync();

            DefaultActivityRunner<IActivityContext> runner = new(serviceProvider);
            while (PipeServer.IsConnected)
            {
                await runner.RunAsync(activityCollection, spectraActivityContext);
            }
        }

        /// <summary>
        /// Method for returning data to the client
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sender"></param>
        public void SendDataThroughPipe(object? obj, ProcessingCompletedEventArgs sender)
        {
            SingleScanDataObject wrapperSsdo = new SingleScanDataObject()
            {
                ScanInstructions = sender.ScanInstructions
            };

            string temp = JsonConvert.SerializeObject(wrapperSsdo);
            byte[] buffer = Encoding.UTF8.GetBytes(temp);
            byte[] length = BitConverter.GetBytes(buffer.Length);
            byte[] finalBuffer = length.Concat(buffer).ToArray();
            PipeServer.Write(finalBuffer, 0, finalBuffer.Length);
            PipeServer.WaitForPipeDrain();
            Console.WriteLine("Server: Sent instruciton to client");
        }


        /// <summary>
        /// Receives data from the client
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventArgs"></param>
        /// <exception cref="ArgumentException"></exception>
        public void HandleDataReceived(object? obj, PipeEventArgs eventArgs)
        {
            // convert PipeEventArgs to single scan data object
            SingleScanDataObject ssdo = eventArgs.ToSingleScanDataObject();
            if (ssdo == null) throw new ArgumentException("single scan data object is null");

            ScanQueueManager.EnqueueScan(ssdo);
            Console.WriteLine("Server: Received scan from client.");
        }


        /// <summary>
        /// Takes the args from the program and parses them into the proper fields
        /// </summary>
        /// <param name="context">args passed from god to server</param>
        /// <exception cref="ArgumentException">Thrown if deserialization results in null values</exception>
        private void ParseStartupContext(string[] context)
        {
            activityCollection = JsonConvert.DeserializeObject<IActivityCollection<IActivityContext>>(context[0]) ??
                                 throw new ArgumentException("Activity Collection not properly deserialized");
            activityCollection.ConnectPipe(this);

            spectraActivityContext = JsonConvert.DeserializeObject<SpectraActivityContext>(context[1]) ??
                                     throw new ArgumentException(
                                         "Spectra Activity Context not properly deserialized");
        }

        /// <summary>
        /// Generates startup context if no parameters are passed in
        /// </summary>
        private void GenerateStartupContext()
        {
            activityCollection = WorkflowInjector.GetDdaActivityCollection();
            activityCollection.ConnectPipe(this);
            spectraActivityContext = WorkflowInjector.GetSpectraActivityContext();
        }



        #region private pipe methods
        public void StartReaderAsync()
        {
            StartByteReaderAsync((b) =>
                PipeDataReceived?.Invoke(this, new PipeEventArgs(b)));
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

        #endregion
    }

    public interface IPipe
    {
        public void SendDataThroughPipe(object? obj, ProcessingCompletedEventArgs sender);
    }
}
