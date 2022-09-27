//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Cryptography.X509Certificates;
//using System.Text;
//using System.Threading.Tasks;
//using ClientServerCommLibrary;

//namespace WorkflowServer
//{
//    public class DataDependentWorkflow : Workflow
//    {
//        //private DataDependentScanTask dataDependentScanTask;
//        private SpectralAveragingTask spectralAveragingTask;
//        public DataDependentWorkflow(IWorkflowOptions options)
//        {
//            // check passed values
//            if (options == null) throw new ArgumentNullException(nameof(options));
//            if (options.GetType() != typeof(DdaWorkflowOptions))
//                throw new ArgumentException("Invalid options class for DdaWorkflow");

//            // set required values
//            WorkflowOptions = options;
//            WorkflowType = Workflows.DataDependentAcquisiton;
//            DdaWorkflowOptions ddaOptions = WorkflowOptions as DdaWorkflowOptions ?? throw new ArgumentNullException();
//            dataDependentScanTask = new DataDependentScanTask(ddaOptions.DataDependentScanOptions);
//            if (ddaOptions.PerformAveraging)
//            {
//                spectralAveragingTask =
//                    new SpectralAveragingTask(ddaOptions.SpectralAveragingOptions ?? throw new ArgumentNullException());
//            }

//            // set values based upon the options
//            Ms1ScanDelegate = (o, scans) =>
//            {
//                IEnumerable<SingleScanDataObject> scanData = scans.ListSsdo;
//                if (spectralAveragingTask != null)
//                {
//                    scanData = new List<SingleScanDataObject>();
//                    ((List<SingleScanDataObject>)scanData).AddRange(spectralAveragingTask.RunTask(scanData));
//                }

//                var temp = dataDependentScanTask.RunTask(scanData);

//            };
//            Ms2ScanDelegate = null;
//        }

//        public override void ProcessScans(object sender, ThresholdReachedEventArgs e)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
