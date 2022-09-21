using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClientServerCommunication;
using Data; 

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

            string instrumentType = args[0];
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
                clientPipe.ConnectClientToServer(); 
                clientPipe.BeginInstrumentConnection(instrumentApi);
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
}
