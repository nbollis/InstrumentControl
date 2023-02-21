using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer.Activities
{
    /// <summary>
    /// Activity that performs a discrete task
    /// </summary>
    public interface IActivity<TContext>
        where TContext : IActivityContext
    {
        Task ExecuteAsync(TContext context);
    }
}
