using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;
using Microsoft.Extensions.DependencyInjection;
using WorkflowServer.Activities;
using WorkflowServer.Util;
using ActivationType = ClientServerCommLibrary.ActivationType;
using DataType = ClientServerCommLibrary.DataType;
using OrbitrapResolution = ClientServerCommLibrary.OrbitrapResolution;
using Polarity = ClientServerCommLibrary.Polarity;
using ScanType = ClientServerCommLibrary.ScanType;

namespace WorkflowServer
{
    /// <summary>
    /// Class for creating workflows to be injected into the app server pipe
    /// GUI will replace the need for this
    /// </summary>
    public class WorkflowInjector
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scansToProcess">number of ms1 scans to select top n peaks from</param>
        /// <param name="topN">how many peaks to select per ms1</param>
        /// <param name="baseMs2">standard info for how to collect ms2</param>
        /// <param name="timeToExclude">how long to exclude in milliseconds</param>
        /// <param name="exclusionPpm">mz ppm to match something to exclusion list</param>
        /// <returns></returns>
        public static IActivityCollection<IActivityContext> GetDdaActivityCollection(int scansToProcess = 1,
            int topN = 3, ScanInstructions? baseMs2 = null, bool useExclusionList = false)
        {
            AcceptScansActivity<IActivityContext> acceptScansActivity = new(1, scansToProcess);
            TopNPeakSelectionActivity<IActivityContext> topNPeakSelectionActivity = new(topN, false, useExclusionList);
            SendDDAScanInstructionsActivity<IActivityContext> sendDdaScanInstructionsActivity =
                new(baseMs2 ??= GetBaseMS2Scan());

            IActivityCollection<IActivityContext> collection = new DefaultActivityCollectionBuilder<IActivityContext>(new ServiceCollection().BuildServiceProvider())
                .Then(acceptScansActivity)
                .Then(topNPeakSelectionActivity)
                .Then(sendDdaScanInstructionsActivity)
                .Build();

            return collection;
        }

        public static SpectraActivityContext GetSpectraActivityContext(double timeToExclude = 0, double exclusionPpmMatch = 0)
        {
            SpectraActivityContext context = new();

            if (timeToExclude != 0 && exclusionPpmMatch != 0)
            {
                context.MassTargetList = new(timeToExclude, exclusionPpmMatch);
            }

            return context;
        }

        public static ScanInstructions GetBaseMS2Scan()
        {
            ScanInstructions instructions = new();
            instructions.FirstMass = 300;
            instructions.LastMass = 2000;
            instructions.Analyzer = AnalyzerType.Orbitrap;
            instructions.ScanType = ScanType.MSn;
            instructions.SourceCIDEnergy = 15;
            instructions.Polarity = Polarity.Postive;
            instructions.DataType = DataType.Profile;
            instructions.IsolationMode = IsolationMode.Quadrupole;
            instructions.AGCTarget = 200;
            instructions.MaxIT = 200;
            instructions.Microscans = 5;
            instructions.OrbitrapResolution = OrbitrapResolution.X_30000;
            instructions.CollisionEnergy = 30;
            instructions.IsolationWidth = 4;
            instructions.ActivationType = ActivationType.HCD;

            return instructions;
        }

    }
}
