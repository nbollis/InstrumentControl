using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    public class PullMetaDataTask : InstrumentControlTask
    {
        public PullMetaDataTask(MyTask taskType) : base(taskType)
        {
        }

        public override TaskResults RunSpecific()
        {
            throw new NotImplementedException();
        }
    }
}
