using Data;
using ScanProdution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    internal class BoxCarScanTask : InstrumentControlTask
    {
        public BoxCarScanTask()
        {

        }

        public override void RunSpecific<T, U>(T otpions, U? data) where U : default
        {
            // not sure how to handle this, this is temparary
            // checks to make sure the correct types were passed in
            if (typeof(T) != typeof(BoxCarScanOptions))
            {
                throw new Exception("Wrong options for BoxCarScanTask you fool");
            }

            if (typeof(U) != typeof(SingleScanDataObject))
            {
                throw new Exception("Wrong data type for BoxCarScanTask you fool");
            }

            // TODO: pull whole charge state data from single scan data object as List<double>

            // TODO: get boxcars (NB working on it)

            // TODO: add boxcars to scan and send it off

        }
    }
}
