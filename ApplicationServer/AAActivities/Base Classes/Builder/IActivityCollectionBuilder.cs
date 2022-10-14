using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer
{
    public interface IActivityCollectionBuilder<TContext>
        where TContext : IActivityContext
    {
        IActivityCollectionBuilder<TContext> StartNew();

        IActivityCollectionBuilder<TContext> Then<TActivity>()
            where TActivity : IActivity<TContext>;
        IActivityCollectionBuilder<TContext> Then(IActivity<TContext> activity);

        IActivityCollection<TContext> Build();
    }
}
