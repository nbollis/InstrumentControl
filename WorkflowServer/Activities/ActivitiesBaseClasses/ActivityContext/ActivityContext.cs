using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer.Activities
{
    public abstract class ActivityContext : IActivityContext
    {
        public bool Cancel { get; set; } = false;
    }
}
