using MassSpectrometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    public abstract class AveragingTask : InstrumentControlTask
    {
        public abstract PreProcessingData SpectraData { get; set; }
        public abstract MzSpectrum CompositeSpectrum { get; set; }
        protected AveragingTask(TaskType taskType, ref PreProcessingData spectraData) : base(taskType)
        {

        }
    }
}
