using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            RunSpecific();
        }

        public abstract void RunSpecific();

    }
}
