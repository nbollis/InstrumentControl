using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer
{
    /// <summary>
    /// Used to store what type of activity is contained within the IActivity
    /// </summary>
    public class ActivityDescriptor<TContext>
        where TContext : IActivityContext
    {
        public ActivityDescriptor(Type activityType)
        {
            ActivityType = activityType ?? throw new ArgumentNullException(nameof(activityType));
        }

        public ActivityDescriptor(IActivity<TContext> activity)
        {
            Activity = activity ?? throw new ArgumentNullException(nameof(activity));
        }

        public Type? ActivityType { get; }
        public IActivity<TContext>? Activity { get; }
    }
}
