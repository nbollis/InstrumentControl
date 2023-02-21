using WorkflowServer.Util;

namespace WorkflowServer.Activities
{
    /// <summary>
    /// Selects the Top N peaks and sets the ScanInstructions Masses To Isolate to that field
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class TopNPeakSelectionActivity<TContext> : IActivity<TContext>
        where TContext : IActivityContext
    {
        private readonly int topNPeaksToIsolateAndFragment;
        private readonly bool useInclusionList;
        private readonly bool useExclusionList;

        public TopNPeakSelectionActivity(int nPeaks, bool useIncList, bool useExcList)
        {
            topNPeaksToIsolateAndFragment = nPeaks;
            useInclusionList = useIncList;
            useExclusionList = useExcList;
        }

        public Task ExecuteAsync(TContext context)
        {
            SpectraActivityContext specContext = context as SpectraActivityContext ?? throw new ArgumentNullException(nameof(context));
            MassTargetList targetList = specContext.MassTargetList ?? throw new ArgumentNullException(nameof(context));

            while (specContext.DataToProcess.Any())
            {
                var singleScanDataObject = specContext.DataToProcess.Dequeue();
                List<double> targets = new();
                // identify targets based on use of inclusion and exclusion lists
                if (useInclusionList)
                {
                    var potentialTargets = targetList.GetHitTargets();
                    if (useExclusionList)
                    {
                        var nonExcludedTargets =
                            targetList.GetTargetsNotExcludedAtSpecificRetentionTime(potentialTargets,
                                singleScanDataObject.RetentionTime).ToList();

                        targets = nonExcludedTargets.Count() < topNPeaksToIsolateAndFragment ?
                            nonExcludedTargets :
                            nonExcludedTargets.Take(topNPeaksToIsolateAndFragment).ToList(); ;
                    }
                    else
                    {
                        targets = potentialTargets.Count() < topNPeaksToIsolateAndFragment ?
                            potentialTargets :
                            potentialTargets.Take(topNPeaksToIsolateAndFragment).ToList();
                    }
                }
                else
                {
                    var potentialTargets =
                        singleScanDataObject.FilterByNumberOfMostIntense(topNPeaksToIsolateAndFragment * 10)
                            .OrderByDescending(p => p.intensity);
                    if (useExclusionList)
                    {
                        var nonExcludedTargets =
                            targetList.GetTargetsNotExcludedAtSpecificRetentionTime(
                                potentialTargets.Select(p => p.mass).ToList(),
                                singleScanDataObject.RetentionTime).ToList();

                        targets = nonExcludedTargets.Count() < topNPeaksToIsolateAndFragment ?
                            nonExcludedTargets :
                            nonExcludedTargets.Take(topNPeaksToIsolateAndFragment).ToList(); ;
                    }
                    else
                    {
                        targets = potentialTargets.Count() < topNPeaksToIsolateAndFragment ?
                            potentialTargets.Select(p => p.mass).ToList() :
                            potentialTargets.Select(p => p.mass).Take(topNPeaksToIsolateAndFragment).ToList();
                    }
                }

                // assign targets to be isolated and fragmented
                foreach (var target in targets)
                {
                    specContext.MassesToTarget.Enqueue(new double[] { target });
                    if (useExclusionList)
                    {
                        specContext.MassTargetList.Add(target, singleScanDataObject.RetentionTime, MassTargetList.MassTargetListTypes.Exclusion);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
