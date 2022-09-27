using System;

namespace WorkflowServer
{
    [Serializable]
    public class ScanInstructions
    {
        public int FragmentationType { get; set; }
        public int ScanType { get; set; }
        public double MzToIsolate { get; set; }
        public double IsolationWidth { get; set; }
        public double HoldTime { get; set; }
        public ScanInstructions(int fragmentationType, int scanType, double mzToIsolate, double isolationWidth, double holdTime)
        {
            FragmentationType = fragmentationType;
            ScanType = scanType;
            MzToIsolate = mzToIsolate;
            IsolationWidth = isolationWidth;
            HoldTime = holdTime;
        }
    }
}
