using MassSpectrometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer
{
    public class WholeChargeEnvelopeFragmentationWorkflow : Workflow
    {
        public override void ProcessScans(object sender, ThresholdReachedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
