using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskInterfaces;

namespace ScanProduction
{
    public class DataDependentScanBuilder : ScanBuilder, IScanBuilder, IDataDependentScanOptions
    {
        #region Options
        public double IsolationWidth { get; set; }
        public double Resolution { get; set; }
        public double MaxIT { get; set; }
        public double NCE { get; set; }
        public int NCE_NormCharge { get; set; }
        public double AGC_Target { get; set; }

        #endregion

        #region To Calculate
        public double IsolationRangeLow { get; set; }
        public double IsolationRangeHight { get; set; }
        public double PrecursorMass { get; set; }
        public string ScanType { get; set; }

        #endregion

        #region Interface Methods
        public void BuildScan<T, U>(T options, U data) where T : ITaskOptions
        {
            DataDependentScanOptions scanOptions = options as DataDependentScanOptions;
            PrecursorMass = Math.Round(double.Parse(data.ToString()), 3);
            
            // set properties that were passed as options
            IsolationWidth = scanOptions.IsolationWidth;
            Resolution = scanOptions.Resolution;
            MaxIT = scanOptions.MaxIT;
            NCE = scanOptions.NCE;
            AGC_Target = scanOptions.AGC_Target;

            // calculate other necessary properties
            IsolationRangeLow = PrecursorMass - IsolationWidth;
            IsolationRangeHight = PrecursorMass + IsolationWidth;
        }

        #endregion

        #region Specific Methods

        #endregion
    }
}
