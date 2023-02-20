using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer.Activities
{
    public interface IActivityCollection<TContext> : IList<IActivity<TContext>>
        where TContext : IActivityContext
    { }
}
