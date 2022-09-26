using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer
{
    public interface IBoxCarScanOptions : ITaskOptions
    {

        public int AGCTarget { get; set; }
        public int MaxIT { get; set; }
        public double IsolationWidth { get; set; }
        public int Resolution { get; set; }
        public string MsxInjectRanges { get; set; }
        public string MsxInjectTargets { get; set; }
        public string MsxInjectMaxITs { get; set; }
    }

    public class BoxCarScanOptions : IBoxCarScanOptions
    {
        public int AGCTarget { get; set; }
        public int MaxIT { get; set; }
        public double IsolationWidth { get; set; }
        public int Resolution { get; set; }
        public string MsxInjectRanges { get; set; }
        public string MsxInjectTargets { get; set; }
        public string MsxInjectMaxITs { get; set; }

        public BoxCarScanOptions()
        {

        }

    }
}
