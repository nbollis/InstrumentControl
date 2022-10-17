using MzLibUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer
{
    /// <summary>
    /// A list of mass targets to check for inclusion or exclusion
    /// private hitTargets field will be what to select fragment
    ///    if they are found by an external class
    /// </summary>
    public class MassTargetList 
    {
        private List<double> hitTargets;

        public List<MassTargetListItem> MassTargets { get; set; }
        public double TimeToExcludeInMilliseconds { get; set; }
        public PpmTolerance Tolerance { get; set; }

        public bool FoundTargets
        {
            get => hitTargets.Any();
        }

        public MassTargetList()
        {
            MassTargets = new List<MassTargetListItem>();
            TimeToExcludeInMilliseconds = ScanProductionGlobalVariables.TimeToExcludeInMilliseconds;
            Tolerance = new PpmTolerance(ScanProductionGlobalVariables.ExclusionMatchingPpmTolerance);
            hitTargets = new();
        }

        /// <summary>
        /// Returns and clears all hit targets
        /// Should be called by a scan producing task
        /// </summary>
        /// <returns></returns>
        public List<double> GetHitTargets()
        {
            double[] targetsCopy = new double[hitTargets.Count];
            hitTargets.CopyTo(targetsCopy);
            hitTargets.Clear();
            return targetsCopy.ToList();
        }

        /// <summary>
        /// Add a mass to the hit targets list for downstream selection for fragmentation
        /// </summary>
        /// <param name="target"></param>
        public void AddHitTarget(double target)
        {
            hitTargets.Add(target);
        }

        /// <summary>
        /// returns all masses to be searched for at a specific retention time
        /// </summary>
        /// <param name="retentionTime"></param>
        /// <returns></returns>
        public IEnumerable<MassTargetListItem> GetTargetsToSearchForAtSpecificTime(double retentionTime)
        {
            foreach (var item in MassTargets)
            {
                if (item.WithinTimeSpan(retentionTime))
                    yield return item;
            }
        }




        // BELOW -> Old Implementation
        // TODO: Fix this all so its nice






        /// <summary>
        /// Adds a value to the exclusion list
        /// </summary>
        /// <param name="mz"></param>
        /// <param name="currentTime"></param>
        public void Add(double mz, double currentTime)
        {
            MassTargets.Add(new MassTargetListItem(mz, currentTime - TimeToExcludeInMilliseconds, currentTime + TimeToExcludeInMilliseconds));
        }

        /// <summary>
        /// Checks to see if the mass is found at the current time
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="currentTime"></param>
        /// <returns>True if the mass is meant to be excluded</returns>
        public bool CheckMassAtCurrentTime(double mass, double currentTime)
        {
            var massesWithinTime = MassTargets.Where(p => p.WithinTimeSpan(currentTime));
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
