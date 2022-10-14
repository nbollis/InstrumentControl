using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer
{
    /// <summary>
    /// Pass in two sets of activities and a condition
    /// Condition determines which collection to run
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class AsyncIfThenElseActivity<TContext> : IActivity<TContext>
        where TContext : IActivityContext
    {
        private readonly Func<TContext, bool> _Condition;
        private readonly IActivityCollection<TContext> _ThenActivities;
        private readonly IActivityCollection<TContext>? _ElseActivities;

        public AsyncIfThenElseActivity(Func<TContext, bool> condition, IActivityCollection<TContext> thenActivities)
        {
            _Condition = condition ?? throw new ArgumentNullException(nameof(condition));
            _ThenActivities = thenActivities ?? throw new ArgumentNullException(nameof(thenActivities));
            _ElseActivities = null;
        }

        public AsyncIfThenElseActivity(Func<TContext, bool> condition, IActivityCollection<TContext> thenActivities, IActivityCollection<TContext> elseActivities)
        {
            _Condition = condition ?? throw new ArgumentNullException(nameof(condition));
            _ThenActivities = thenActivities ?? throw new ArgumentNullException(nameof(thenActivities));
            _ElseActivities = elseActivities ?? throw new ArgumentNullException(nameof(elseActivities));
        }

        public async Task ExecuteAsync(TContext context)
        {
            if (_Condition(context))
                await RunActivities(_ThenActivities, context);
            else if (_ElseActivities != null)
                await RunActivities(_ElseActivities, context);
        }

        private async Task RunActivities(IActivityCollection<TContext> activities, TContext context)
        {
            foreach (var activity in activities)
                await activity.ExecuteAsync(context);
        }
    }
}
