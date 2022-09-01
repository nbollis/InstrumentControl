using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClientServerCommunication;
using Data; 

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // fire up the pipe client
            string serverPipeName = args[0];
            string pipeName = args[1];
            ClientPipe clientPipe = new ClientPipe(serverPipeName, pipeName,
                p => p.StartByteReaderAsync());

            string instrumentType = args[2];
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

            try
            {
                IInstrument instrumentApi = factory?.CreateInstrumentApi();
                instrumentApi.PipeClient = clientPipe;
                instrumentApi?.OpenInstrumentConnection();
                instrumentApi?.EnterMainLoop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Console.ReadLine(); 
        }
        // TODO: Write method to query what type of instrument is attached. 

        
    }
}
