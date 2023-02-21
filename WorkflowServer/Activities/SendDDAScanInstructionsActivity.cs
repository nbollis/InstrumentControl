using ClientServerCommLibrary;
using WorkflowServer.Activities;

namespace WorkflowServer.Activities
{
    /// <summary>
    /// Takes the masses to target list and sends the scan instructions to the client
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class SendDDAScanInstructionsActivity<TContext> : IActivity<TContext>, IScanSender
        where TContext : IActivityContext
    {
        public ScanInstructions ScanInstructions { get; set; }
        public event EventHandler<ProcessingCompletedEventArgs> SendScan;

        public SendDDAScanInstructionsActivity(ScanInstructions baseScan)
        {
            ScanInstructions = baseScan;
        }

        public Task ExecuteAsync(TContext context)
        {
            SpectraActivityContext specContext =
                context as SpectraActivityContext ?? throw new NullReferenceException();
            while (specContext.MassesToTarget.Any())
            {
                var target = specContext.MassesToTarget.Dequeue();
                if (target.Length == 1)
                {
                    ScanInstructions.PrecursorMass = target.First();
                    SendScan?.Invoke(null, new ProcessingCompletedEventArgs(ScanInstructions));
                }
                else
                {
                    throw new ArgumentException("DDAScanSender only accepts one precursor mass to isolate per scan");
                }
            }
            return Task.CompletedTask;
        }
    }
}
