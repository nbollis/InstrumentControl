using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;
using ClientServerCommunication;

namespace WorkflowServer
{
    public class DataDependentScanActivity<TContext> : IActivity<TContext>
        where TContext : IActivityContext
    {
        private readonly int topNPeaksToIsolateAndFragment;

        public DataDependentScanActivity(int nPeaks)
        {
            topNPeaksToIsolateAndFragment = nPeaks;
        }

        public Task ExecuteAsync(TContext context)
        {
            SpectraActivityContext specContext = context as SpectraActivityContext ?? throw new ArgumentNullException(nameof(context));
            ScanInstructions instructions = specContext.ScanInstructions;
            MassTargetList targetList = specContext.MassTargetList ?? throw new ArgumentNullException(nameof(context));

            instructions.MassesToIsolate = targetList.GetHitTargets().Take(topNPeaksToIsolateAndFragment).ToList();

            return Task.CompletedTask;
        }
    }
}
