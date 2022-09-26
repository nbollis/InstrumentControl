using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Averaging.Tasks
{
    internal abstract class Task
    {
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }

        public abstract void RunTask<T, U>(T? options, U? data);
    }
}
