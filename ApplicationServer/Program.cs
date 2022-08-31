using System.IO.Pipes;
using ClientServerCommunication;
using Data;
using Newtonsoft;
using Newtonsoft.Json;

namespace ApplicationServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // start server async
            Queue<SingleScanDataObject> queue = new();
            bool pipeOpen = false; 
            ServerPipe server = new(args[0], p => p.StartByteReaderAsync(), PipeTransmissionMode.Byte);
            server.PipeConnected += (obj, sender) =>
            {
                Console.WriteLine("Client connected.");
            };
            server.DataReceived += (obj, sender) =>
            {
                queue.Enqueue(JsonConvert.DeserializeObject<SingleScanDataObject>(Convert.ToString(sender.Data)));
            };
            server.PipeClosed += (obj, sender) =>
            {
                Console.Write("Client disconnected. Closing after acknowledgment.");
                Console.ReadLine(); 
                server.Close();
            }; 
            
            while (pipeOpen)
            {
                // break if client terminates. 
                if (!pipeOpen) break;
                // break if user hits escape. 
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape) break; 
            }

            Console.ReadLine(); 
        }
    }
    // TODO: Create God.
}