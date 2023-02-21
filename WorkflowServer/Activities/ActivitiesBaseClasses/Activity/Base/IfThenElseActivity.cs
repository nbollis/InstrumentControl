using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer.Activities
{
    /// <summary>
    /// Pass in two sets of activities and a condition
    /// Condition determines which collection to run
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class AsyncIfThenElseActivity<TContext> : IActivity<TContext>
        where TContext : IActivityContext
    {
        private readonly Func<TContext, bool> condition;
        private readonly IActivityCollection<TContext> thenActivities;
        private readonly IActivityCollection<TContext>? elseActivities;

        public AsyncIfThenElseActivity(Func<TContext, bool> condition,
            IActivityCollection<TContext> thenActivities)
        {
            this.condition = condition ?? throw new ArgumentNullException(nameof(condition));
            this.thenActivities = thenActivities ?? throw new ArgumentNullException(nameof(thenActivities));
            elseActivities = null;
        }

        public AsyncIfThenElseActivity(Func<TContext, bool> condition,
            IActivityCollection<TContext> thenActivities, IActivityCollection<TContext> elseActivities)
        {
            this.condition = condition ?? throw new ArgumentNullException(nameof(condition));
            this.thenActivities = thenActivities ?? throw new ArgumentNullException(nameof(thenActivities));
            this.elseActivities = elseActivities ?? throw new ArgumentNullException(nameof(elseActivities));
        }

        public async Task ExecuteAsync(TContext context)
        {
            if (condition(context))
                await RunActivities(thenActivities, context);
            else if (elseActivities != null)
                await RunActivities(elseActivities, context);
        }

        private async Task RunActivities(IActivityCollection<TContext> activities, TContext context)
        {
            foreach (var activity in activities)
                await activity.ExecuteAsync(context);
        }
    }
}
