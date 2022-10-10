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
            NamedPipeClientStream pipeClient =
                new NamedPipeClientStream(".", "test",
                    PipeDirection.InOut,
                    PipeOptions.Asynchronous);

            ClientPipe clientPipe = new ClientPipe(pipeClient);

            string instrumentType = "tribrid";
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
                System.Timers.Timer timer = new System.Timers.Timer();
                int i = 0; 

                clientPipe.ConnectClientToServer(); 
                clientPipe.BeginInstrumentConnection(instrumentApi);
                Console.WriteLine(instrumentApi.GetSystemState(1));

                while (i < 6)
                {
                    switch (i)
                    {
                        case 0:
                            instrumentApi.StartAcquisition("test.raw");
                            break;
                        case 1:
                            instrumentApi.PauseAcquisition();
                            break;
                        case 2:
                            instrumentApi.CancelAcquisition();
                            break;
                        case 3:
                            instrumentApi.InstrumentStandby();
                            break;
                        case 4:
                            break;
                        default:
                            Console.WriteLine("Test completed");
                            break;
                    }
                    Console.WriteLine(instrumentApi.GetSystemState(1) + "\n");
                    i++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                
            }
            pipeClient.Close(); 
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
}
