using System.IO.Pipes;
using ClientServerCommunication;
using Data;
using InstrumentControl;
using Newtonsoft;
using Newtonsoft.Json;

namespace ApplicationServer
{
    delegate void WorkflowDelegateMethod(List<SingleScanDataObject> scansList);
    internal class Program
    {
        // TODO: Workflow options should be an abstract factory 
        // TODO: string args[] options should correspond to workflow options unless Nic can GUI-fy this more easily. 
        public static WorkflowOptions WorkflowOptions { get; set; }
        public static WorkflowDelegateMethod Workflow { get; set; }
        public static void Main(string[] args)
        {

            // set the workflow 
            switch (args[0])
            {
                case "simple":
                    Workflow = SimpleWorkflowDelegate; 
                    break; 
            }

            ScanQueue scanQueue = new(5); 
            // start server async
            bool pipeOpen = false; 
            
            // initialize the server pipe stream. 
            // in the initialization, the server will wait for the client pipe to connect. 
            // TODO: Add a time-out for client waiting. also, the waiting for client connection should 
            // probably not be happening in the server's initialization. 
            ServerPipe server = new(args[0], p => p.StartByteReaderAsync(), PipeTransmissionMode.Byte);
            
            server.PipeConnected += (obj, sender) =>
            {
                Console.WriteLine("Client connected.");
                pipeOpen = true; 
            };
            // anonymous function that puts the received data into the ScanQueue for processing. 
            server.DataReceived += (obj, sender) =>
            { 
                scanQueue.DataToProcess
                    .Enqueue(JsonConvert
                    .DeserializeObject<SingleScanDataObject>(Convert.ToString(sender.Data)));
            };
            // This event is handled by the processing workflow method. 
            // The processing method takes a delegate. The delegate is set by the options first argument.  
            scanQueue.ThresholdReached += ProcessingWorkflowHandler; 
            // disconnect. ReadLine() is there so that it takes user input to break the program. 
            server.PipeClosed += (obj, sender) =>
            {
                Console.Write("Client disconnected. Closing after acknowledgment.");
                Console.ReadLine(); 
                server.Close();
            }; 
            // enter while loop. 
            while (pipeOpen)
            {
                // break if client terminates. 
                if (!pipeOpen) break;
                // break if user hits escape. 
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape) break; 
            }

            Console.ReadLine(); 
        }

        public static void ProcessingWorkflowHandler(object? o, ThresholdReachedEventArgs thresholdReachedEventArgs)
        {
            Workflow(thresholdReachedEventArgs.Data); 
        }

        public static void SimpleWorkflowDelegate(List<SingleScanDataObject> listScans)
        {
            foreach (SingleScanDataObject data in listScans)
            {
                Console.WriteLine(data.MinX + "; " + data.MaxX);
            }
        }
    }

    public class WorkflowOptions
    {
        public List<string> Options { get; set; } = new List<string>();
    }
}