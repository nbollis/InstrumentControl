using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer
{
    /// <summary>
    /// Provide a placeholder that will be executed at runtime asynchronously 
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class AsyncLambdaActivity<TContext> : IActivity<TContext>
        where TContext : IActivityContext
    {
        private readonly Func<TContext, Task> _Action;
        public AsyncLambdaActivity(Func<TContext, Task> action)
        {
            _Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public Task ExecuteAsync(TContext context)
            => _Action(context);
    }

    /// <summary>
    /// Provide a placeholder that will be executed at runtime 
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class LambdaActivity<TContext> : IActivity<TContext>
        where TContext : IActivityContext
    {
        private readonly Action<TContext> _Action;
        public LambdaActivity(Action<TContext> action)
        {
            _Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public Task ExecuteAsync(TContext context)
        {
            _Action(context);
            return Task.CompletedTask;
        }
    }
}
