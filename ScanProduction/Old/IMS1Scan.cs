using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Scans;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;

namespace ScanProduction
{
    public interface IMS1Scan : IBaseScan
    {

        // able to only access parameters associated with MS1 scans. 
        public void SetCustomValue<T>(InstrumentSettings setting, T valueToSet)
        {
            // filter any InstrumentSettings that are disallowed
            if ((int)setting >= 14)
            {
                throw new ArgumentException("Disallowed scan setting for MS1 scan.");
            }
            else
            {
                SetCustomValue(this, setting, valueToSet);
            }
        }


        public void SetMS1ValuesBasedOnPreviousScan(IMsScan scan)
        {
            // all MS1 values defined above are below in order
            FirstMass = double.Parse(scan.Header["FirstMass"]);
            LastMass = double.Parse(scan.Header["LastMass"]);
            //Analyzer = 
            //ScanType =
            SourceCIDEnergy = double.Parse(scan.Header["SourceCID"]);
            //SrcRFLens = scan.Header[];
            SetCustomValue(this, InstrumentSettings.Polarity, scan.Header["Polarity"]);
            DataType = scan.CentroidCount > 0 ? DataType.Centroid : DataType.Profile;
            //IsolationMode = scan.Header[];
            AGCTarget = double.Parse(scan.Header["AGC_Target"]);
            MaxIT = double.Parse(scan.Header["MaxIT"]);
            MicroScans = int.Parse(scan.Header["Microscans"]);
            SetCustomValue(this, InstrumentSettings.OrbitrapResolution, scan.Header["Resolution"]);


            // TODO: Implement a method that takes an IScans and sets the properties of this
            // to the original scan.  
            throw new NotImplementedException();
        }
    }
}
