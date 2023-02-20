using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer.Activities
{
    public class DefaultActivityRunner<TContext> : IActivityRunner<TContext>
        where TContext : IActivityContext
    {
        private readonly IServiceProvider _Provider;

        public DefaultActivityRunner(IServiceProvider provider)
        {
            _Provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public async Task RunAsync(IActivityCollection<TContext> activities, TContext context)
        {
            foreach (IActivity<TContext> activity in activities)
            {
                if (await RunActivity(activity, context))
                    break;
            }
        }

        private async Task<bool> RunActivity(IActivity<TContext> activity, TContext context)
        {
            await activity.ExecuteAsync(context);
            return context.Cancel;
        }
    }
}

