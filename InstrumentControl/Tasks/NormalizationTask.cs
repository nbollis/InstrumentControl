using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    /// <summary>
    /// Normalizes the intensities of each peak to the total ion current of the scan
    /// All peak intensities should sum to one
    /// </summary>
    public class NormalizationTask : InstrumentControlTask, ISpectraProcesor
    {
        public NormalizationTask(TaskType taskType) : base(taskType)
        {

        }

        public override void RunSpecific()
        {
            for (int i = 0; i < ISpectraProcesor.ScansToProcess; i++)
            {
                for (int j = 0; j < ISpectraProcesor.YArrays[i].Length; j++)
                {
                    ISpectraProcesor.YArrays[i][j] = ISpectraProcesor.YArrays[i][j] / ISpectraProcesor.TotalIonCurrent[i];
                }
            }
        }
    }
}
