using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;
using MzLibUtil;

namespace WorkflowServer
{
    /// <summary>
    /// Checks the MassTarget inclusions list and sets hit targets masses if found
    /// Will cancel the subsequent operations if no targets are found
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class CheckInclusionListActivity<TContext> : IActivity<TContext>
        where TContext : IActivityContext
    {
        
        private readonly double relativeIntensityCutoff;
        private readonly Tolerance tolerance;

        public CheckInclusionListActivity(double relativeIntCutoff, Tolerance tol)
        {
            relativeIntensityCutoff = relativeIntCutoff;
            tolerance = tol;
        }

        public Task ExecuteAsync(TContext context)
        {
            SpectraActivityContext specContext = context as SpectraActivityContext ?? throw new ArgumentNullException(nameof(context));
            SingleScanDataObject scan = specContext.FirstSingleScanDataObject;
            MassTargetList inclusionList = specContext.MassTargetList ?? throw new ArgumentNullException(nameof(context));

            // find all targets at current time
            var allTargets = inclusionList.GetInclusionListItemsAtSpecificRetentionTime(scan.RetentionTime);
            foreach (var target in allTargets)
            {
                // get the masses that match the target
                var matchingSpectraPeaks = scan.XArray.Where(p => tolerance.Within(target.Mass, p)).ToArray();
                if (matchingSpectraPeaks.Any())
                {
                    int firstIndex = Array.IndexOf(scan.XArray, matchingSpectraPeaks.First());
                    double maxIntensityWithinRange = scan.YArray
                        .Take(new Range(firstIndex, firstIndex + matchingSpectraPeaks.Length)).Max();

                    // check to see if the most intense peak has a high enough relative intensity 
                    // to be selected for fragmentation 
                    if (maxIntensityWithinRange / scan.YArray.Max() >= relativeIntensityCutoff)
                    {
                        int mostIntenseIndex = Array.IndexOf(scan.YArray, maxIntensityWithinRange);
                        inclusionList.AddHitTarget(scan.XArray[mostIntenseIndex]);
                    }
                    else
                    {
                        specContext.Cancel = true;
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
