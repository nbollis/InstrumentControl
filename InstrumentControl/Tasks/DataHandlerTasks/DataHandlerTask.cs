using InstrumentControl.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    public abstract class DataHandlerTask : InstrumentControlTask
    {
        public abstract DataHandlerTaskResult SpectraData { get; set; }
        protected DataHandlerTask(TaskType taskType, ref DataHandlerTaskResult spectraData) : base(taskType)
        {
            SpectraData = spectraData;
        }
    }
}
