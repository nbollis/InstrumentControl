﻿using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer
{
    public class DataDependentScanBuilder : ScanBuilder, IScanBuilder, IDataDependentScanOptions
    {
        #region Options
        public double IsolationWidth { get; set; }
        public double Resolution { get; set; }
        public double MaxIT { get; set; }
        public double CollisionEnergy { get; set; }
        public double AGCTarget { get; set; }

        #endregion

        #region To Calculate
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
            CollisionEnergy = scanOptions.CollisionEnergy;
            AGCTarget = scanOptions.AGCTarget;

        }

        #endregion
    }
}
