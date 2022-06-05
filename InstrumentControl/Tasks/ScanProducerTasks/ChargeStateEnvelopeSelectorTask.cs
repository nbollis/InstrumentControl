using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl.Tasks.ScanProducerTasks
{
    public class ChargeStateEnvelopeSelectorTask : InstrumentControlTask
    {
        public ChargeStateEnvelopeSelectorTask() : base(TaskType.ChargeStateEnvelope)
        {
        }

        public override void RunSpecific()
        {
            throw new NotImplementedException();
        }
    }
}
