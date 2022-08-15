using System;

namespace Data
{
    [Serializable]
    public class ScanInstructions
    {
        public int FragmentationType { get; set; }
        public int ScanType { get; set; }
        public double MzToIsolate { get; set; }
        public double IsolationWidth { get; set; }
        public ScanInstructions(int fragmentationType, int scanType, double mzToIsolate, double isolationWidth)
        {
            FragmentationType = fragmentationType;
            ScanType = scanType;
            MzToIsolate = mzToIsolate;
            IsolationWidth = isolationWidth;
        }
    }
}
