using System.IO.Pipes;
using ClientServerCommLibrary;
using ClientServerCommunication;
using Newtonsoft;
using Newtonsoft.Json;

namespace WorkflowServer
{
    public class Program
    {
        // args should contain {Number of Ms1 Scans to queue, Number of Ms2 Scans to queue,
        //                      Workflow type, Workflow Options}
        public static void Main(string[] args)
        {
            NamedPipeServerStream pipeServer =
                new NamedPipeServerStream("test",
                    PipeDirection.InOut, 1, PipeTransmissionMode.Byte);

            // parse information from scan the arguments
            int ms1toQueue = int.Parse(args[0]);
            int ms2toQueue = int.Parse(args[1]);
            Workflows workflowType = Enum.Parse<Workflows>(args[2]);
            IWorkflowOptions options = JsonConvert.DeserializeObject<IWorkflowOptions>(args[3]) ?? throw new ArgumentNullException();

            // set up the workflow to be executed
            // TODO: Set up delegates based upon workflow type
            Workflow? workflow = null;
            switch (workflowType)
            {
                case Workflows.DeepProteomeProfilingWorkflow:
                    throw new NotImplementedException();
                    break;

                case Workflows.WholeChargeStateEnvelopeFragmentation:
                    throw new NotImplementedException();
                    break;

                case Workflows.DataDependentAcquisiton:
                   // workflow = new DataDependentWorkflow(options);
                    break;

            }

            AppServerPipe serverPipe = new AppServerPipe(pipeServer, ms1toQueue, ms2toQueue);

            try
            {
                serverPipe.StartServer(workflow ?? throw new ArgumentNullException());
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