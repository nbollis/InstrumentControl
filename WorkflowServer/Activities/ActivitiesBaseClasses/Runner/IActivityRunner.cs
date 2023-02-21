using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer.Activities
{
    /// <summary>
    /// Runs Activities
    /// </summary>
    public interface IActivityRunner<TContext>
        where TContext : IActivityContext
    {
        Task RunAsync(IActivityCollection<TContext> activities, TContext context);
    }
}
