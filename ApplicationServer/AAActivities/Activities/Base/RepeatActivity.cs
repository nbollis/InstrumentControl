using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer
{
    /// <summary>
    /// Run the same set of activities multiple times
    /// Either with a static count or based on a property on the context
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class RepeatActivity<TContext> : IActivity<TContext>
        where TContext : IActivityContext
    {
        private readonly Func<TContext, int> _CountFunction;
        private readonly IActivityCollection<TContext> _Activities;

        public RepeatActivity(Func<TContext, int> countFunction, IActivityCollection<TContext> activities)
        {
            _CountFunction = countFunction ?? throw new ArgumentNullException(nameof(countFunction));
            _Activities = activities ?? throw new ArgumentNullException(nameof(activities));
        }

        public RepeatActivity(int count, IActivityCollection<TContext> activities)
        {
            if (count < 1)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be 1 or greater.");

            _CountFunction = (ctx) => count;
            _Activities = activities ?? throw new ArgumentNullException(nameof(activities));
        }

        public async Task ExecuteAsync(TContext context)
        {
            int count = _CountFunction(context);

            for (int i = 0; i < count; i++)
                await RunActivities(_Activities, context);
        }

        private async Task RunActivities(IActivityCollection<TContext> activities, TContext context)
        {
            foreach (var activity in activities)
                await activity.ExecuteAsync(context);
        }
    }
}
