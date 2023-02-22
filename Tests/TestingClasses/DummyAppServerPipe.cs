using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;
using Microsoft.Extensions.DependencyInjection;
using WorkflowServer;
using WorkflowServer.Activities;

namespace Tests
{
    public class DummyAppServerPipe : IPipe
    {
        private IActivityCollection<IActivityContext> activityCollection;
        private SpectraActivityContext spectraActivityContext;
        private ServiceProvider serviceProvider;
        public NamedPipeServerStream PipeServer { get; set; }

        public DummyAppServerPipe(NamedPipeServerStream pipeServer)
        {
            PipeServer = pipeServer;
        }

        public async Task StartServer(IActivityCollection<IActivityContext> actCollection,
            IActivityContext context, IServiceProvider provider)
        {
            activityCollection = actCollection;
            spectraActivityContext = (SpectraActivityContext)context;
            serviceProvider = (ServiceProvider)provider;

            DefaultActivityRunner<IActivityContext> runner = new(serviceProvider);
            while (true)
            {
                await runner.RunAsync(activityCollection, spectraActivityContext);
            }
        }

        public void HandleDataReceived(SingleScanDataObject ssdo)
        {
            ScanQueueManager.EnqueueScan(ssdo);

            //Workflow.ReceiveData(ssdo);
            Console.WriteLine("\n");
        }

        public void SendInstructionToClient(object? obj, ProcessingCompletedEventArgs sender)
        {

        }
    }
}
