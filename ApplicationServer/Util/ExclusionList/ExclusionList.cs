using MzLibUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer
{
    public class ExclusionList 
    {
        public List<ExclusionListItem> TheExclusionList { get; set; }
        public double TimeToExcludeInMilliseconds { get; set; }
        public PpmTolerance Tolerance { get; set; }

        public ExclusionList()
        {
            TheExclusionList = new List<ExclusionListItem>();
            TimeToExcludeInMilliseconds = ScanProductionGlobalVariables.TimeToExcludeInMilliseconds;
            Tolerance = new PpmTolerance(ScanProductionGlobalVariables.ExclusionMatchingPpmTolerance);
        }
        
        /// <summary>
        /// Adds a value to the exclusion list
        /// </summary>
        /// <param name="mz"></param>
        /// <param name="currentTime"></param>
        public void Add(double mz, double currentTime)
        {
            TheExclusionList.Add(new ExclusionListItem(mz, currentTime - TimeToExcludeInMilliseconds, currentTime + TimeToExcludeInMilliseconds));
        }

        /// <summary>
        /// Checks to see if the mass is found and meant to be excluded at the current time
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="currentTime"></param>
        /// <returns>True if the mass is meant to be excluded</returns>
        public bool ExcludeMassAtCurrentTime(double mass, double currentTime)
        {
            var massesWithinTime = TheExclusionList.Where(p => p.WithinTimeSpan(currentTime));
            if (massesWithinTime.Any(p => Tolerance.Within(mass, p.Mass)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Exports exclusion list in json format for use in later runs
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void ExportExclusionList()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Imports an exclusion list and adds it to the current list
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void ImportExclusionList()
        {
            throw new NotImplementedException();
        }

    }
}
