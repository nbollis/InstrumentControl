using System.IO.Pipes;
using ClientServerCommLibrary;
using ClientServerCommunication;
using Newtonsoft;
using Newtonsoft.Json;

namespace WorkflowServer
{
    public class Program
    {
        // args should contain {a list of tasks}
        public static void Main(string[] args)
        {
            NamedPipeServerStream pipeServer =
                new NamedPipeServerStream("test",
                    PipeDirection.InOut, 1, PipeTransmissionMode.Byte);

            List<InstrumentControlTask> tasks = JsonConvert.DeserializeObject<List<InstrumentControlTask>>(args[0]) ?? throw new NullReferenceException();
            AppServerPipe serverPipe = new AppServerPipe(pipeServer);
            Workflow workflow = new(serverPipe, tasks);

            try
            {
                serverPipe.StartServer(workflow);
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