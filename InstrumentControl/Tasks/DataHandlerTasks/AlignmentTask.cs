using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl.Tasks.DataHandlerTasks
{
    public class AlignmentTask : DataHandlerTask
    {
        public AlignmentTask(TaskType taskType, ref DataHandlerTaskResult spectraData) : base(taskType, ref spectraData)
        {
        }

        public override DataHandlerTaskResult SpectraData { get; set; }

        public override void RunSpecific()
        {
            int breakpoint = 0;
            throw new NotImplementedException();
        }
    }
}
