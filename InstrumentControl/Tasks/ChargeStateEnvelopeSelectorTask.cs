using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    public class ChargeStateEnvelopeSelectorTask : InstrumentControlTask
    {
        public ChargeStateEnvelopeSelectorTask() : base(TaskType.ChargeStateEnvelope)
        {
        }

        public override TaskResults RunSpecific()
        {
            throw new NotImplementedException();
        }
    }
}
