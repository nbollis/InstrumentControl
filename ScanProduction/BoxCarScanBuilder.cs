using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskInterfaces;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Scans;

namespace ScanProduction
{
    public class BoxCarScanBuilder : ScanBuilder, IScanBuilder, IBoxCarScanOptions
    {

        #region Options

        public int AGCTarget { get; set; }
        public int MaxIT { get; set; }
        public double IsolationWidth { get; set; }
        public int Resolution { get; set; }

        #endregion

        #region To Calculate

        public string MsxInjectRanges { get; set; }
        public string MsxInjectTargets { get; set; }
        public string MsxInjectMaxITs { get; set; }

        #endregion

        #region Interface Method
        public void BuildScan<T, U>(T options, U data) where T : ITaskOptions
        {
            BoxCarScanOptions scanOptions = options as BoxCarScanOptions;
            List<double> mzToIsolate = data as List<double>;

            // set properties that were passed as options
            AGCTarget = scanOptions.AGCTarget;
            MaxIT = scanOptions.MaxIT;
            IsolationWidth = scanOptions.IsolationWidth;
            Resolution = scanOptions.Resolution;

            // calculate other necessary properties
            Tuple<double, double>[] boxCarValues = GenerateBoxCarValues(mzToIsolate, IsolationWidth);
            BuildBoxCarStrings(boxCarValues);
        }

        #endregion

        #region Specific Methods

        public void BuildBoxCarStrings(Tuple<double, double>[] boxes)
        {
            string boxRanges = "[";

            foreach (var box in boxes)
            {
                boxRanges += "(";
                boxRanges += (box.Item1 + 2.5).ToString("0.00");
                boxRanges += ",";
                boxRanges += (box.Item2 - 2.5).ToString("0.00");
                boxRanges += "),";
            }
            boxRanges = boxRanges.Remove(boxRanges.Count() - 1);
            boxRanges += "]";
            MsxInjectRanges = boxRanges;

            string boxTargets = "[";
            for (int i = 0; i < boxes.Length; i++)
            {
                boxTargets += AGCTarget / boxes.Length;
                if (i != boxes.Length - 1)
                {
                    boxTargets += ",";
                }
            }
            boxTargets += "]";
            MsxInjectTargets = boxTargets;

            string boxMaxITs = "[";
            for (int i = 0; i < boxes.Length; i++)
            {
                boxMaxITs += MaxIT / boxes.Length;
                if (i != boxes.Length - 1)
                {
                    boxMaxITs += ",";
                }
            }
            boxMaxITs += "]";
            MsxInjectMaxITs = boxMaxITs;
        }

        /// <summary>
        /// Generates BoxCars centered on the list of Mz values passed
        /// </summary>
        /// <param name="mzs"></param>
        /// <param name="isolationRange"></param>
        /// <returns></returns>
        public static Tuple<double, double>[] GenerateBoxCarValues(List<double> mzs, double isolationWidth)
        {
            int boxesToGenerate = mzs.Count;
            Tuple<double, double>[] boxes = new Tuple<double, double>[boxesToGenerate];

            for (int i = 0; i < boxesToGenerate; i++)
            {
                boxes[i] = new Tuple<double, double>(mzs[i] - isolationWidth, mzs[i] + isolationWidth);
            }

            return boxes;
        }

        #endregion
    }
}
