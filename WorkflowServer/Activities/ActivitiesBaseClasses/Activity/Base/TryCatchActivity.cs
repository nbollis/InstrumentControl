using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer.Activities
{
    /// <summary>
    /// Alter functionality based upon a specific exception
    /// </summary>
    public class TryCatchActivity<TContext> : IActivity<TContext>
    where TContext : IActivityContext
    {
        private readonly IActivityCollection<TContext> _TryActivities;
        private readonly CatchActivityCollection<TContext> _CatchActivities;
        private readonly IActivityCollection<TContext> _FinallyActivities;

        public TryCatchActivity(IActivityCollection<TContext> tryActivities, CatchActivityCollection<TContext> catchActivities, IActivityCollection<TContext> finallyActivities)
        {
            _TryActivities = tryActivities ?? throw new ArgumentNullException(nameof(tryActivities));
            _CatchActivities = catchActivities ?? throw new ArgumentNullException(nameof(catchActivities));
            _FinallyActivities = finallyActivities;
        }

        public async Task ExecuteAsync(TContext context)
        {
            try
            {
                await RunActivities(_TryActivities, context);
            }
            catch (Exception ex)
            {
                IActivityCollection<TContext>? catchActivities = GetCatchActivities(ex.GetType());
                if (catchActivities == null)
                    throw;

                await RunActivities(catchActivities, context);
            }
            finally
            {
                if (_FinallyActivities != null)
                    await RunActivities(_FinallyActivities, context);
            }
        }

        private IActivityCollection<TContext>? GetCatchActivities(Type type)
        {
            var found = _CatchActivities.FirstOrDefault(t => t.Type.IsAssignableFrom(type));
            if (found != null)
                return found.Activities;

            return null;
        }

        private async Task RunActivities(IActivityCollection<TContext> activities, TContext context)
        {
            foreach (var activity in activities)
                await activity.ExecuteAsync(context);
        }
    }

    public sealed class CatchActivityCollection<TContext> : List<CatchActivityEntry<TContext>>
        where TContext : IActivityContext
    {
        private readonly IActivityCollectionBuilder<TContext> _Builder;

        public CatchActivityCollection(IActivityCollectionBuilder<TContext> builder)
        {
            _Builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        public CatchActivityCollection<TContext> Throws<TException>(ActivityCollectionBuilderExtensions.ConfigureActivities<TContext> configure)
            where TException : Exception
        {
            Add(new CatchActivityEntry<TContext>
            {
                Type = typeof(TException),
                Activities = _Builder.StartNew().ConfigureCollection(configure)
            });
            return this;
        }
    }

    public class CatchActivityEntry<TContext>
        where TContext : IActivityContext
    {
        public Type Type { get; set; }
        public IActivityCollection<TContext> Activities { get; set; }
    }
}
