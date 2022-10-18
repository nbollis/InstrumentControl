using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace WorkflowServer
{
    public class ServiceProviderActivity<TContext> : IActivity<TContext>
        where TContext : IActivityContext
    {
        private readonly IServiceProvider _Provider;
        private readonly Type _ActivityType;

        public ServiceProviderActivity(IServiceProvider provider, Type activityType)
        {
            _Provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _ActivityType = activityType ?? throw new ArgumentNullException(nameof(activityType));
        }

        public Task ExecuteAsync(TContext context)
        {
            IActivity<TContext> activity = (IActivity<TContext>)ActivatorUtilities.CreateInstance(_Provider, _ActivityType);
            return activity.ExecuteAsync(context);
        }
    }
}
