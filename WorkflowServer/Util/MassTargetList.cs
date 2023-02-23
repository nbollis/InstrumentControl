using MzLibUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer.Util
{
    /// <summary>
    /// A list of mass targets to check for inclusion or exclusion
    /// private hitTargets field will be what to select fragment
    ///    if they are found by an external class
    /// </summary>
    public class MassTargetList
    {
        #region Private Properties

        private List<double> hitTargets;

        #endregion

        #region Public Properties

        public List<MassTargetListItem> InclusionList { get; set; }
        public List<MassTargetListItem> ExclusionList { get; set; }

        public enum MassTargetListTypes
        {
            Exclusion,
            Inclusion
        }
        public double TimeToExcludeInSeconds { get; set; }
        public PpmTolerance Tolerance { get; set; }

        /// <summary>
        /// If inclusion list was searched and targets were identified
        /// </summary>
        public bool FoundTargets
        {
            get => hitTargets.Any();
        }

        #endregion

        #region Constructor

        public MassTargetList(double bufferTimeOnLists = 60, double ppmTolerance = 10)
        {
            TimeToExcludeInSeconds = bufferTimeOnLists / 60.0;
            Tolerance = new PpmTolerance(ppmTolerance);
            hitTargets = new();
            ExclusionList = new();
            InclusionList = new();
        }

        #endregion

        #region Public Methods

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

        public bool IsExcluded(double mz, double rt)
        {
            foreach (var excludedItem in GetExclusionListItemsAtSpecificRetentionTime(rt))
            {
                if (Tolerance.Within(mz, excludedItem.Mass))
                    return true;
            }

            RemoveOutdatedItems(rt);

            return false;
        }

        private void RemoveOutdatedItems(double currentRT)
        {
            var outDateExclusionListItems = ExclusionList.Where(p => p.EndTime < currentRT).ToList();
            foreach (var outDatedItem in outDateExclusionListItems)
            {
                ExclusionList.Remove(outDatedItem);
            }
        }

        /// <summary>
        /// returns all masses to be searched for at a specific retention time
        /// </summary>
        /// <param name="retentionTime"></param>
        /// <returns></returns>
        public IEnumerable<MassTargetListItem> GetInclusionListItemsAtSpecificRetentionTime(double retentionTime)
        {
            foreach (var item in InclusionList)
            {
                if (item.WithinTimeSpan(retentionTime))
                    yield return item;
            }
        }

        /// <summary>
        /// returns all masses to be excluded for at a specific retention time
        /// </summary>
        /// <param name="retentionTime"></param>
        /// <returns></returns>
        public IEnumerable<MassTargetListItem> GetExclusionListItemsAtSpecificRetentionTime(double retentionTime)
        {
            foreach (var item in ExclusionList.Where(p => p.WithinTimeSpan(retentionTime)))
            {
                yield return item;
            }
        }

        /// <summary>
        /// Checks a list of masses to see if they are in the exclusion list
        /// </summary>
        /// <param name="valuesToCheck"></param>
        /// <param name="retentionTime"></param>
        /// <returns>All masses not found within exclusion list at specific retention time</returns>
        public IEnumerable<double> GetTargetsNotExcludedAtSpecificRetentionTime(List<double> valuesToCheck, double retentionTime)
        {
            var exclusionListItemsWithinTime = ExclusionList.Where(p => p.WithinTimeSpan(retentionTime));
            foreach (var val in valuesToCheck.Where(m => !exclusionListItemsWithinTime.Any(p => Tolerance.Within(p.Mass, m))))
            {
                yield return val;
            }
        }

        /// <summary>
        /// Adds a value to the exclusion or inclusion list
        /// </summary>
        /// <param name="mz">mass of species</param>
        /// <param name="currentTime">retention time of species</param>
        public void Add(double mz, double currentTime, MassTargetListTypes listType)
        {
            switch (listType)
            {
                case MassTargetListTypes.Inclusion:
                    InclusionList.Add(new MassTargetListItem(mz, currentTime - TimeToExcludeInSeconds, currentTime + TimeToExcludeInSeconds));
                    break;
                case MassTargetListTypes.Exclusion:
                    ExclusionList.Add(new MassTargetListItem(mz, currentTime - TimeToExcludeInSeconds, currentTime + TimeToExcludeInSeconds));
                    break;
            }
        }


        // TODO: Implement these taking in a filestream so it can be called from anywhere
        /// <summary>
        /// Exports exclusion list in json format for use in later runs
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void ExportList(Stream stream, MassTargetListTypes listType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Imports an exclusion list and adds it to the current list
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void ImportList(Stream stream, MassTargetListTypes listType)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
