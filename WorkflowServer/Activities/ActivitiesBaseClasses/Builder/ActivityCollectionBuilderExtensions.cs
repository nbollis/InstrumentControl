using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer.Activities
{
    /// <summary>
    /// Methods used to construct the workflow
    /// </summary>
    public static class ActivityCollectionBuilderExtensions
    {
        public delegate IActivityCollectionBuilder<TContext> ConfigureActivities<TContext>(IActivityCollectionBuilder<TContext> configure)
            where TContext : IActivityContext;

        public static IActivityCollection<TContext> ConfigureCollection<TContext>(
            this IActivityCollectionBuilder<TContext> builder,
            ConfigureActivities<TContext> configure) where TContext : IActivityContext
            => configure(builder.StartNew()).Build();

        /// <summary>
        /// Runs Lambda activity async once previous one finishes
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="builder"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        public static IActivityCollectionBuilder<TContext> ThenAsync<TContext>(this IActivityCollectionBuilder<TContext> builder, Func<TContext, Task> activity)
            where TContext : IActivityContext
            => builder.Then(new AsyncLambdaActivity<TContext>(activity));

        /// <summary>
        /// Runs Lambda activity once previous one finishes
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="builder"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        public static IActivityCollectionBuilder<TContext> Then<TContext>(this IActivityCollectionBuilder<TContext> builder, Action<TContext> activity)
            where TContext : IActivityContext
            => builder.Then(new LambdaActivity<TContext>(activity));

        /// <summary>
        /// Runs conditional activity async after previous one finishes
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="builder"></param>
        /// <param name="predicate"></param>
        /// <param name="configureThen"></param>
        /// <returns></returns>
        public static IActivityCollectionBuilder<TContext> WhenAsync<TContext>(
            this IActivityCollectionBuilder<TContext> builder,
            Func<TContext, bool> predicate,
            ConfigureActivities<TContext> configureThen) where TContext : IActivityContext
            => builder.Then(new AsyncIfThenElseActivity<TContext>(
                predicate,
                builder.ConfigureCollection(configureThen))
            );

        /// <summary>
        /// Runs conditional activity after previous one finishes
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="builder"></param>
        /// <param name="predicate"></param>
        /// <param name="configureThen"></param>
        /// <returns></returns>
        public static IActivityCollectionBuilder<TContext> WhenAsync<TContext>(
            this IActivityCollectionBuilder<TContext> builder,
            Func<TContext, bool> predicate,
            ConfigureActivities<TContext> configureThen,
            ConfigureActivities<TContext> configureElse) where TContext : IActivityContext
            => builder.Then(new AsyncIfThenElseActivity<TContext>(
                predicate,
                builder.ConfigureCollection(configureThen),
                builder.ConfigureCollection(configureElse))
            );

        /// <summary>
        /// Repeat an activity N number of times where N is count
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="builder"></param>
        /// <param name="count">How many times to repeat the activity</param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IActivityCollectionBuilder<TContext> Repeat<TContext>(
            this IActivityCollectionBuilder<TContext> builder,
            int count,
            ConfigureActivities<TContext> configure) where TContext : IActivityContext
            => builder.Then(new RepeatActivity<TContext>(count, builder.ConfigureCollection(configure)));

        /// <summary>
        /// Repeat an activity until the count function is satisfied
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="builder"></param>
        /// <param name="countFunc"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IActivityCollectionBuilder<TContext> Repeat<TContext>(
            this IActivityCollectionBuilder<TContext> builder,
            Func<TContext, int> countFunc,
            ConfigureActivities<TContext> configure) where TContext : IActivityContext
            => builder.Then(new RepeatActivity<TContext>(countFunc, builder.ConfigureCollection(configure)));

        public static IActivityCollectionBuilder<TContext> Try<TContext>(
            this IActivityCollectionBuilder<TContext> builder,
            ConfigureActivities<TContext> tryConfigure,
            Func<CatchActivityCollection<TContext>, CatchActivityCollection<TContext>> catchConfigure) where TContext : IActivityContext
        {
            var tryActivities = builder.ConfigureCollection(tryConfigure);
            var catchActivities = catchConfigure(new CatchActivityCollection<TContext>(builder));

            return builder.Then(new TryCatchActivity<TContext>(tryActivities, catchActivities, null));
        }

        public static IActivityCollectionBuilder<TContext> Try<TContext>(
            this IActivityCollectionBuilder<TContext> builder,
            ConfigureActivities<TContext> tryConfigure,
            Func<CatchActivityCollection<TContext>, CatchActivityCollection<TContext>> catchConfigure,
            ConfigureActivities<TContext> finallyConfigure) where TContext : IActivityContext
        {
            var tryActivities = builder.ConfigureCollection(tryConfigure);
            var catchActivities = catchConfigure(new CatchActivityCollection<TContext>(builder));
            var finallyActivities = builder.ConfigureCollection(finallyConfigure);

            return builder.Then(new TryCatchActivity<TContext>(tryActivities, catchActivities, finallyActivities));
        }

    }
}
