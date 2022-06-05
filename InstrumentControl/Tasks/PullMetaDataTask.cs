using InstrumentControl.Tasks;
using InstrumentControl.Tasks.ScanProducerTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    public class PullMetaDataTask : InstrumentControlTask
    {
        public PullMetaDataTask(TaskType taskType) : base(taskType)
        {
        }

        public override void RunSpecific()
        {
            throw new NotImplementedException();
        }
    }
}
