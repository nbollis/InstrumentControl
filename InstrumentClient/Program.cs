﻿using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClientServerCommLibrary;
using Newtonsoft.Json;

namespace InstrumentClient
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            NamedPipeClientStream pipeClient;
            string instrumentType;
            int[] acceptableScanOrders;

            if (args.Length > 1)
            {
                pipeClient =
                new NamedPipeClientStream(args[0], args[1],
                    PipeDirection.InOut,
                    PipeOptions.Asynchronous);
                instrumentType = args[2];
                acceptableScanOrders = JsonConvert.DeserializeObject<int[]>(args[3]);
            }
            else
            {
                pipeClient =
                new NamedPipeClientStream(".", "test",
                    PipeDirection.InOut,
                    PipeOptions.Asynchronous);
                instrumentType = args[0];
                acceptableScanOrders = new[] { 1 };
            }

            ClientPipe clientPipe = new ClientPipe(pipeClient, acceptableScanOrders);
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

                clientPipe.StartClient(instrumentApi);
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
    }
}
