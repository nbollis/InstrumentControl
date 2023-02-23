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
        /// <param name="baseMs1"></param>
        /// <param name="baseMs2">standard info for how to collect ms2</param>
        /// <param name="useExclusionList"></param>
        /// <returns></returns>
        public static IActivityCollection<IActivityContext> GetDdaActivityCollection(int scansToProcess = 1,
            int topN = 3, ScanInstructions? baseMs1 = null, ScanInstructions? baseMs2 = null, bool useExclusionList = false)
        {
            CaptureMs1Activity<IActivityContext> captureMs1Activity = new(1, scansToProcess, baseMs1 ?? GetBaseMs1Scan());
            TopNPeakSelectionActivity<IActivityContext> topNPeakSelectionActivity = new(topN, false, useExclusionList);
            SendDDAScanInstructionsActivity<IActivityContext> sendDdaScanInstructionsActivity =
                new(baseMs2 ?? GetBaseMs2Scan());

            IActivityCollection<IActivityContext> collection = new DefaultActivityCollectionBuilder<IActivityContext>(new ServiceCollection().BuildServiceProvider())
                .Then(captureMs1Activity)
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

        public static ScanInstructions GetBaseMs2Scan()
        {
            ScanInstructions instructions = new()
            {
                FirstMass = 300,
                LastMass = 1500,
                Analyzer = AnalyzerType.Orbitrap,
                ScanType = ScanType.MSn,
                SourceCIDEnergy = 15,
                Polarity = Polarity.Positive,
                DataType = DataType.Profile,
                IsolationMode = IsolationMode.Quadrupole,
                AGCTarget = 100,
                MaxIT = 200,
                Microscans = 5,
                OrbitrapResolution = OrbitrapResolution.X_60000,
                CollisionEnergy = 30,
                IsolationWidth = 4,
                ActivationType = ActivationType.HCD
            };

            return instructions;
        }

        public static ScanInstructions GetBaseMs1Scan()
        {
            ScanInstructions instructions = new()
            {
                FirstMass = 200,
                LastMass = 2000,
                Analyzer = AnalyzerType.Orbitrap,
                ScanType = ScanType.Full,
                SourceCIDEnergy = 15,
                Polarity = Polarity.Positive,
                DataType = DataType.Profile,
                IsolationMode = IsolationMode.Quadrupole,
                AGCTarget = 100,
                MaxIT = 200,
                Microscans = 5,
                OrbitrapResolution = OrbitrapResolution.X_120000
            };

            return instructions;
        }

    }
}
