using MassSpectrometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    /// <summary>
    /// Class used to funnel data out of each task to be utilized after the task is executed
    /// </summary>
    public class TaskResults
    {
        #region Public Properties
        public MzSpectrum CompositeSpectrum { get; set; }

        #endregion

        /* Currently, one constructor will be designed for each task type 
        There may be a better way of going about this, but I think this
        will help make the future drag and drop GUI easier to execute if
        the return type for each task is the same */
        #region Constructors

        public TaskResults(SpectrumAveragingTask averageTask)
        {
            CompositeSpectrum = averageTask.CompositeSpectrum;
        }

        #endregion


    }
}
