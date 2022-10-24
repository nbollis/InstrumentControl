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
                clientPipe.ConnectClientToServer(); 
                clientPipe.BeginInstrumentConnection(instrumentApi);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            pipeClient.Close(); 
        }
        
    }
}
