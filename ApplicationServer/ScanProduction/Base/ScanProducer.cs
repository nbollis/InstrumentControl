using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer
{
    /// <summary>
    /// Directory class following the Builder pattern
    /// COnstructs objects of the IScanBuilder interface
    /// </summary>
    public class ScanProducer : IScanBuilder
    {
        public ScanBuilder Builder { get; set; }

        public ScanProducer(ScanBuilder builder)
        {
            Builder = builder;
        }

        #region Interface Members

        /// <summary>
        /// Performs the calculations and sets the associated values within the builder
        /// e.g. sets the boxcar strings within a boxcar scan
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        public void BuildScan<T, U>(T options, U data) where T : ITaskOptions
        {
            (Builder as IScanBuilder).BuildScan(options, data);
        }

        // TODO: Fix this so that it is not reliant on ICustomScan
        ///// <summary>
        ///// Converts the interanal calculated values to a dictionary and sets them in the custom scan
        ///// </summary>
        ///// <param name="customScan"></param>
        //public void SetValuesToScan(ICustomScan customScan)
        //{
        //    Dictionary<string, string> valueDictionary = Builder.BuildDictionary();
        //    foreach (var key in valueDictionary.Keys)
        //    {
        //        customScan.Values[key] = valueDictionary[key];
        //    }
        //}

        #endregion


    }
}
