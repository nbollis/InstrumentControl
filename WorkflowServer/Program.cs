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
            NamedPipeServerStream pipeServer =
                new NamedPipeServerStream("test",
                    PipeDirection.InOut, 1, PipeTransmissionMode.Byte);

            AppServerPipe serverPipe = new AppServerPipe(pipeServer);

            try
            {
                serverPipe.StartServer(args);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                pipeServer.Dispose();
            }
        }
    }
}