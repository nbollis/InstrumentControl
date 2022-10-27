using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;


namespace WorkflowServer
{
    public class AcceptScansActivity<TContext> : IActivity<TContext>
        where TContext : IActivityContext
    {
        private readonly int msNOrder;
        private readonly int scansToDequeue;

        public AcceptScansActivity(int msNOrder, int scansToDequeue)
        {
            this.msNOrder = msNOrder;
            this.scansToDequeue = scansToDequeue;
        }

        public Task ExecuteAsync(TContext context)
        {

            throw new NotImplementedException();
            return Task.CompletedTask;
        }

        private async Task WaitForScans()
        {

        }


    }
}



