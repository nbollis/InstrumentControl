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
        public event EventHandler<PipeEventArgs> PipeDataReceived;


        public NamedPipeClientStream ReadDataPipe { get; }
        public NamedPipeServerStream SendDataPipe { get; }

        private IActivityCollection<IActivityContext> activityCollection;
        private SpectraActivityContext spectraActivityContext;
        private ServiceProvider serviceProvider;

        public AppServerPipe(NamedPipeClientStream readDataPipe, NamedPipeServerStream sendDataPipe)
        {
            SendDataPipe = sendDataPipe;
            ReadDataPipe = readDataPipe;
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

            DefaultActivityRunner<IActivityContext> runner = new(serviceProvider);
            while (SendDataPipe.IsConnected)
            {
                await runner.RunAsync(activityCollection, spectraActivityContext);
            }
        }

        #region Pipe Methods

        /// <summary>
        /// Method for returning data to the client
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sender"></param>
        public void SendInstructionToClient(object? obj, ProcessingCompletedEventArgs sender)
        {
            SingleScanDataObject wrapperSsdo = new SingleScanDataObject()
            {
                ScanInstructions = sender.ScanInstructions
            };

            string temp = JsonConvert.SerializeObject(wrapperSsdo);
            byte[] buffer = Encoding.UTF8.GetBytes(temp);
            byte[] length = BitConverter.GetBytes(buffer.Length);
            byte[] finalBuffer = length.Concat(buffer).ToArray();
            SendDataPipe.Write(finalBuffer, 0, finalBuffer.Length);
            SendDataPipe.WaitForPipeDrain();
            PrintoutMessage.Print(MessageSource.Server, "Sent instruction to client");
        }

        /// <summary>
        /// Receives data from the client
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventArgs"></param>
        /// <exception cref="ArgumentException"></exception>
        public void HandleDataReceivedFromClient(object? obj, PipeEventArgs eventArgs)
        {
            // convert PipeEventArgs to single scan data object
            SingleScanDataObject ssdo = eventArgs.ToSingleScanDataObject();
            if (ssdo == null) throw new ArgumentException("single scan data object is null");

            ScanQueueManager.EnqueueScan(ssdo);
            PrintoutMessage.Print(MessageSource.Server, $"Received scan from client - Scan Number {ssdo.ScanNumber}");
        }

        public void ConnectServerToClient()
        {
            var readerConnectResultsAsync = ReadDataPipe.ConnectAsync().ContinueWith(_ =>
            {
                PrintoutMessage.Print(MessageSource.Client, "Workflow Server read pipe connected to client.");
            });

            var senderConnectionResult = SendDataPipe.WaitForConnectionAsync().ContinueWith(_ =>
            {
                PrintoutMessage.Print(MessageSource.Client, "Worklflow Server write pipe connected to client.");
            });
            readerConnectResultsAsync.Wait();
            senderConnectionResult.Wait();

            StartReaderAsync();
            PipeDataReceived += HandleDataReceivedFromClient;
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

    }

    public interface IPipe
    {
        public void SendInstructionToClient(object? obj, ProcessingCompletedEventArgs sender);
    }
}
