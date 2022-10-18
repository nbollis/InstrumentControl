using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer
{
    /// <summary>
    /// Extension class for ActivityCollection that allows one to run after another
    /// </summary>
    public static class xxActivityCollectionExtensions
    {
        //public static IActivityCollection<TContext> Then<TActivity, TContext>(this IActivityCollection<TContext> activities)
        //    where TActivity : IActivity<TContext>
        //    where TContext : IActivityContext
        //{
        //    activities.Add(new ActivityDescriptor<TContext>(typeof(TActivity)));
        //    return activities;
        //}

        //public static IActivityCollection<TContext> Then<TContext>(this IActivityCollection<TContext> activities, IActivity<TContext> activity)
        //    where TContext : IActivityContext
        //{
        //    activities.Add(new ActivityDescriptor<TContext>(activity));
        //    return activities;
        //}
    }
}
