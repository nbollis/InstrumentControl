﻿using MzLibUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowServer.Util;

namespace WorkflowServer
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
        public double TimeToExcludeInMilliseconds { get; set; }
        public PpmTolerance Tolerance { get; set; }

        public bool FoundTargets
        {
            get => hitTargets.Any();
        }

        #endregion

        #region Constructor

        public MassTargetList()
        {
            TimeToExcludeInMilliseconds = ScanProductionGlobalVariables.TimeToExcludeInMilliseconds;
            Tolerance = new PpmTolerance(ScanProductionGlobalVariables.ExclusionMatchingPpmTolerance);
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

        /// <summary>
        /// returns all masses to be searched for at a specific retention time
        /// Exclusion --> returns masses not in list
        /// Inclusion --> returns masses in list
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
        /// Checks a list of masses to see if they are in the exclusion list
        /// </summary>
        /// <param name="valuesToCheck"></param>
        /// <param name="retentionTime"></param>
        /// <returns>All masses not found within exclusion list at specific retention time</returns>
        public IEnumerable<double> GetTargetsNotFoundWithinExclusionListAtSpecificRetentionTime(List<double> valuesToCheck, double retentionTime)
        {
            var exclusionListItemsWithinTime = ExclusionList.Where(p => p.WithinTimeSpan(retentionTime));
            foreach (var val in valuesToCheck)
            {
                if (!exclusionListItemsWithinTime.Any(p => Tolerance.Within(p.Mass, val)))
                    yield return val;
            }
        }

        /// <summary>
        /// Adds a value to the exclusion list
        /// </summary>
        /// <param name="mz"></param>
        /// <param name="currentTime"></param>
        public void Add(double mz, double currentTime, MassTargetListTypes listType)
        {
            switch (listType)
            {
                case MassTargetListTypes.Inclusion:
                    InclusionList.Add(new MassTargetListItem(mz, currentTime - TimeToExcludeInMilliseconds, currentTime + TimeToExcludeInMilliseconds));
                    break;
                case MassTargetListTypes.Exclusion:
                    ExclusionList.Add(new MassTargetListItem(mz, currentTime - TimeToExcludeInMilliseconds, currentTime + TimeToExcludeInMilliseconds));
                    break;
            }
        }





        // TODO: Add params to list constructor and remove from global data
        // evaluate if these are necessary
        public void CreateInclusionList()
        {
            InclusionList = new();
        }

        public void CreateExclusionList()
        {
            ExclusionList = new();
        }




        // TODO: Implement these taking in a filestream so it can be called from anywhere
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

        #endregion

    }
}
