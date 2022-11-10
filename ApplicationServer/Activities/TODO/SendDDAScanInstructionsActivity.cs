using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer
{
    /// <summary>
    /// Takes the masses to target list and sends the scan instructions to the client
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class SendDDAScanInstructionsActivity<TContext> : IActivity<TContext>
        where TContext : IActivityContext
    {
        public Task ExecuteAsync(TContext context)
        {
            throw new NotImplementedException();
        }
    }
}
