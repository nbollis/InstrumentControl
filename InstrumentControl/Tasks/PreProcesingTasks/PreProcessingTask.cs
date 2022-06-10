using InstrumentControl.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    public abstract class PreProcessingTask : InstrumentControlTask
    {
        public abstract PreProcessingData SpectraData { get; set; }
        protected PreProcessingTask(TaskType taskType, ref PreProcessingData spectraData) : base(taskType)
        {
            SpectraData = spectraData;
        }
    }
}
