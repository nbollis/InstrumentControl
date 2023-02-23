using System.IO.Pipes;
using ClientServerCommLibrary;
using Newtonsoft;
using Newtonsoft.Json;

namespace WorkflowServer
{
    public class Program
    {
        // args should contain a serialized ActivityCollection
        public static void Main(string[] args)
        {
            // Reading and writing between two processes requires that each side of the process has a NamedPipeServerStream to do the data writing 
            // and a NamedPipeClientStream to do the listening. The pipe names are "[process doing the writing] + Write". 
            var writePipe =
                new NamedPipeServerStream("workflowServerWrite", PipeDirection.Out, 1, PipeTransmissionMode.Byte);
            var readPipe =
                new NamedPipeClientStream(".", "instrumentClientWrite", PipeDirection.In, PipeOptions.Asynchronous);

            AppServerPipe serverPipe = new AppServerPipe(readPipe, writePipe);
            

            try
            {
                PrintoutMessage.Print(MessageSource.Server, "Startup Initiated");
                serverPipe.ConnectServerToClient();
                Thread.Sleep(1000);

                serverPipe.StartServer(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                readPipe.Dispose();
                writePipe.Dispose();
            }
        }
    }
}