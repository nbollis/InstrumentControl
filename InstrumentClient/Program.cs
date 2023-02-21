using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClientServerCommLibrary; 

namespace InstrumentClient
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            NamedPipeClientStream pipeClient;
            string instrumentType;

            if (args.Length > 1)
            {
                pipeClient =
                new NamedPipeClientStream(args[0], args[1],
                    PipeDirection.InOut,
                    PipeOptions.Asynchronous);
                instrumentType = args[2];
            }
            else
            {
                pipeClient =
                new NamedPipeClientStream(".", "test",
                    PipeDirection.InOut,
                    PipeOptions.Asynchronous);
                instrumentType = args[0];
            }

            ClientPipe clientPipe = new ClientPipe(pipeClient);
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

            try
            {
                // the general architectural guideline is that if the instrument throws an 
                // event, then the client needs to respond to it. This adds a layer of abstraction between the instrument and
                // this client that ensures we can extend it to other instruments in the future.
                //
                // It is the responsibility of the person implementing the instrument to client interface to only expose
                // the absolutely necessary information to this instrument client. The instrument client should only provide the absolutely necessary information to the 
                // application server. This prevents bloat and future issues. 

                clientPipe.ConnectClientToServer();
                Thread.Sleep(1000); 
                
                clientPipe.BeginInstrumentConnection(instrumentApi);
                clientPipe.ScanQueueThreshold = 1;


                bool readyToReceiveScan = true;

                instrumentApi.ScanReceived += (obj, sender) =>
                {
                    Console.WriteLine("Client: Scan received from instrument"); 
                    clientPipe.EnqueueInstrumentScan(sender.Ssdo);
                };
                instrumentApi.ReadyToReceiveScanInstructions += (obj, sender) =>
                {
                    readyToReceiveScan = true;
                };
                clientPipe.ScanQueueThresholdReached += (obj, sender) =>
                {

                    if (readyToReceiveScan && clientPipe.ScanInstructionsQueue.Count > 0)
                    {
                        readyToReceiveScan = false;
                        instrumentApi.SendScanAction(clientPipe.ScanInstructionsQueue.Dequeue());
                        Console.WriteLine("Client: Instructions sent to instrument");
                    }

                    var listSsdo = sender.ListSsdo.ToList(); 
                    foreach(var list in listSsdo)
                    {
                        clientPipe.SendDataThroughPipe(list);
                        Console.WriteLine("Client: Scan sent to server"); 
                    }
                };
                while (true)
                {

                }; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                pipeClient.Dispose();
            }
        }


        //static void Main(string[] args)
        //{
        //    // fire up the pipe client
        //    string serverPipeName = args[0];
        //    string pipeName = args[1];
        //    ClientPipe clientPipe = new ClientPipe(serverPipeName, pipeName,
        //        p => p.StartByteReaderAsync());

        //    string instrumentType = args[2];
        //    IInstrumentFactory factory = null; 
        //    switch (instrumentType)
        //    {
        //        case "qe":
        //            factory = new ThermoQEFactory();
        //            break;
        //        case "tribrid":
        //            factory = new ThermoTribridFactory();
        //            break; 
        //    }

        //    try
        //    {
        //        IInstrument instrumentApi = factory?.CreateInstrumentApi();
        //        instrumentApi.OpenInstrumentConnection(); 
        //        instrumentApi.PipeClient = clientPipe;
        //        instrumentApi?.EnterMainLoop();
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //    Console.ReadLine(); 
        //}
        //// TODO: Write method to query what type of instrument is attached. 


    }
    public class MsScanArrivedEventArgs : EventArgs
    {
        public SingleScanDataObject Ssdo { get; set; }
        public MsScanArrivedEventArgs(SingleScanDataObject ssdo)
        {
            Ssdo = ssdo;
        }
    }
}
