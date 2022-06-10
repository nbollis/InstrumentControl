using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl.Tasks
{
    public class BoxcarFraggerTask : InstrumentControlTask
    {
        public BoxcarFraggerTask(TaskType taskType) : base(TaskType.BoxcarFragger)
        {
        }

        public override void RunSpecific()
        {
            throw new NotImplementedException();
        }
    }
}
