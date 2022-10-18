using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer
{
    public sealed class DefaultActivityCollectionBuilder<TContext> : IActivityCollectionBuilder<TContext>
        where TContext : IActivityContext
    {
        private readonly IServiceProvider _Provider;
        private IActivityCollection<TContext> _Activities;

        public DefaultActivityCollectionBuilder(IServiceProvider provider)
        {
            _Provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _Activities = new DefaultActivityCollection();
        }

        public IActivityCollectionBuilder<TContext> StartNew()
            => new DefaultActivityCollectionBuilder<TContext>(_Provider);

        public IActivityCollectionBuilder<TContext> Then<TActivity>() where TActivity : IActivity<TContext>
        {
            _Activities.Add(new ServiceProviderActivity<TContext>(_Provider, typeof(TActivity)));
            return this;
        }

        public IActivityCollectionBuilder<TContext> Then(IActivity<TContext> activity)
        {
            _Activities.Add(activity);
            return this;
        }

        public IActivityCollection<TContext> Build()
        {
            var results = _Activities;
            _Activities = new DefaultActivityCollection();
            return results;
        }

        private class DefaultActivityCollection : List<IActivity<TContext>>, IActivityCollection<TContext>
        { }
    }
}
