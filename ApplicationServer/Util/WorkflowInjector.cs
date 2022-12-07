using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;
using Microsoft.Extensions.DependencyInjection;

namespace WorkflowServer
{
    /// <summary>
    /// Class for creating workflows to be injected into the app server pipe
    /// GUI will replace the need for this
    /// </summary>
    public class WorkflowInjector
    {
        public static IActivityCollection<IActivityContext> GetDDAActivityCollection()
        {
            AcceptScansActivity<IActivityContext> acceptScansActivity = new(1, 1);
            TopNPeakSelectionActivity<IActivityContext> topNPeakSelectionActivity = new(3, false, false);
            SendDDAScanInstructionsActivity<IActivityContext> sendDdaScanInstructionsActivity =
                new(GetBaseMS2Scan());

            IActivityCollection<IActivityContext> collection = new DefaultActivityCollectionBuilder<IActivityContext>(new ServiceCollection().BuildServiceProvider())
                .Then(acceptScansActivity)
                .Then(topNPeakSelectionActivity)
                .Then(sendDdaScanInstructionsActivity)
                .Build();

            return collection;
        }

        public static SpectraActivityContext GetSpectraActivityContext()
        {
            SpectraActivityContext context = new();

            // Can add inclusion and exclusion lists here

            return context;
        }

        public static ScanInstructions GetBaseMS2Scan()
        {
            ScanInstructions instructions = new();
            instructions.FirstMass = 300;
            instructions.LastMass = 2000;
            instructions.AnalyzerType = AnalyzerType.Orbitrap;
            instructions.ScanType = ScanType.MSn;
            instructions.SourceEnergy = 15;
            instructions.Polarity = Polarity.Postive;
            instructions.DataType = DataType.Profile;
            instructions.IsolationType = IsolationType.Quadrupole;
            instructions.AgcTarget = 200;
            instructions.MaxIt = 200;
            instructions.Microscans = 5;
            instructions.OrbitrapResolution = OrbitrapResolution.X_30000;
            instructions.CollisionEnergy = 30;
            instructions.IsolationWidth = 4;
            instructions.ActivationType = ActivationType.HCD;

            return instructions;
        }

    }
}
