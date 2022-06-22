using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine; 
namespace InstrumentControl
{
    public abstract class InstrumentControlTask
    {

        protected TaskType TaskType { get; set; }

        public InstrumentControlTask(TaskType taskType)
        {
            TaskType = taskType;
        }

        public void Run()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            RunSpecific();
            watch.Stop();
            Console.WriteLine("Executed {0} Task in {1} ms", TaskType, watch.ElapsedMilliseconds);
        }

        public virtual void RunSpecific() { }
    }
}
