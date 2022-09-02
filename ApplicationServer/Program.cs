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
        public static bool PipeOpen { get; set; } 
        public static void Main(string[] args)
        {
            PipeOpen = true;
            // set the workflow 
            switch (args[0])
            {
                case "simple":
                    Workflow = SimpleWorkflowDelegate; 
                    break; 
            }

            ScanQueue scanQueue = new(5); 
            // start server async
            
            // initialize the server pipe stream. 
            // in the initialization, the server will wait for the client pipe to connect. 
            // TODO: Add a time-out for client waiting. also, the waiting for client connection should 
            // probably not be happening in the server's initialization. 
            ServerPipe server = new(args[1], p => p.StartByteReaderAsync(), PipeTransmissionMode.Byte);
            
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
            while (PipeOpen)
            {
                // anonymous function that puts the received data into the ScanQueue for processing. 
                server.DataReceived += (sender, args) =>
                {
                    OnDataReceived(sender, args);
                }; 
            }

            Console.ReadLine(); 
        }
        private static void OnDataReceived(object sender, PipeEventArgs args)
        {
            // TODO: Convert to concurrent queue. 
            // to prevent multiple writes to the queue in the event 
            // of multiple ScanInstructionObjects getting sent in the buffer rapidly.
            // enqueue ScanInstructionObject

            // The listener for CanAcceptNextCustomScan 
            // Creates a delegate to receive the EventArgs from the CanAcceptNextCustomScan event. 

            // TODO: Refactor to pull out the AutoResentEvent and EventHandler from this method. 
            AutoResetEvent waitHandle = new AutoResetEvent(false);
            EventHandler ev = delegate (object o, EventArgs e)
            {
                waitHandle.Set();
            };
            // Delegate listens for the CanAcceptNextCustomScan
            Task.Run(() =>
                {
                    // TODO: add conversion from ScanInstructions to ICustomScan. 
                    Console.WriteLine(args.Length); 
                }
            );
            // Blocks thread from continuing until it receives the CanAcceptNextCustomScanEvent. 
            //waitHandle.WaitOne();
        }

        public static void DataReceived(object obj, PipeEventArgs args)
        {
            Console.WriteLine(args.Length);
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