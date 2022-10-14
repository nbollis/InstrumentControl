using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer
{
    public static class ActivityRunnerExtensions
    {
        public static Task RunAsync<TContext>(this IActivityRunner<TContext> runner, IActivityCollection<TContext> activities, Func<TContext> initializer)
            where TContext : IActivityContext
        {
            if (runner == null)
                throw new ArgumentNullException(nameof(runner));
            if (activities == null)
                throw new ArgumentNullException(nameof(activities));
            if (initializer == null)
                throw new ArgumentNullException(nameof(initializer));

            TContext context = initializer();
            if (context == null)
                throw new InvalidOperationException("Initializer returned an invalid context for use be the activity runner [null].");

            return runner.RunAsync(activities, context);
        }

        public static Task RunAsync<TContext>(this IActivityRunner<TContext> runner, IActivityCollection<TContext> activities)
            where TContext : IActivityContext, new()
        {
            if (runner == null)
                throw new ArgumentNullException(nameof(runner));
            if (activities == null)
                throw new ArgumentNullException(nameof(activities));

            var context = new TContext();
            return runner.RunAsync(activities, context);
        }
    }
}
