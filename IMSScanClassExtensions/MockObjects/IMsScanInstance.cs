using IO.MzML;
using MassSpectrometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;
using Thermo.Interfaces.SpectrumFormat_V1;

namespace IMSScanClassExtensions
{
    public class IMsScanInstance : IMsScan
    {
        #region Not Yet Implemented

        public IChargeEnvelope[] ChargeEnvelopes { get; set; }
        public int? NoiseCount { get; set; }
        public IEnumerable<INoiseNode> NoiseBand { get; set; }
        public IDictionary<string, string> Header { get; set; }
        public IInformationSourceAccess TuneData { get; set; }
        public IInformationSourceAccess Trailer { get; set; }
        public IInformationSourceAccess StatusLog { get; set; }

        #endregion

        public string DetectorName { get; set; }
        public int? CentroidCount { get; set; }
        public IEnumerable<ICentroid> Centroids { get; set; }

        #region Constructors

        public IMsScanInstance()
        {

        }

        public IMsScanInstance(MsDataScan scan)
        {
            int centroidCount = scan.MassSpectrum.XArray.Length;
            DetectorName = scan.MzAnalyzer.ToString();
            CentroidCount = centroidCount;
            Centroids = new List<ICentroid>();
            for (int i = 0; i < centroidCount; i++)
            {
                ICentroidInstance centroid = new(scan.MassSpectrum.XArray[i], scan.MassSpectrum.YArray[i]);
                ((List<ICentroid>)Centroids).Add(centroid);
            }
            Centroids = (IEnumerable<ICentroid>)Centroids;

            Header = new Dictionary<string, string>();
            Header.Add("MassAnalyzer", scan.MzAnalyzer.ToString());
            Header.Add("IonizationMode", "NSI");
            Header.Add("ScanRate", "Normal");
            Header.Add("ScanMode", "Full || SIM || Msn");
            Header.Add("StartTime", scan.RetentionTime.ToString());
            Header.Add("Scan", scan.OneBasedScanNumber.ToString());
            Header.Add("TIC", scan.TotalIonCurrent.ToString());
            Header.Add("BasePeakIntensity", scan.MassSpectrum.YArray.First().ToString("0.0"));
            Header.Add("BasePeakMass", scan.MassSpectrum.XArray.First().ToString("0.00"));
            Header.Add("CycleNumber", "Not sure what this is");
            Header.Add("Polarity", scan.Polarity.ToString());
            Header.Add("Microscans", "Not found in MSDataScan");
            Header.Add("InjectTime", scan.InjectionTime.ToString());
            Header.Add("ScanData", "Centroid");
            Header.Add("Segments", "1");
            Header.Add("Monoisotopic", "0");
            Header.Add("MasterScan", scan.MsnOrder == 1 ? "0" : scan.OneBasedPrecursorScanNumber.ToString());
            Header.Add("FirstMass", scan.ScanWindowRange.Minimum.ToString("0"));
            Header.Add("LastMass", scan.ScanWindowRange.Maximum.ToString("0"));
            Header.Add("Checksum", "Idk what this is");
            Header.Add("MSOrder", scan.MsnOrder.ToString());
            Header.Add("Average", "Not sure what this is");
            Header.Add("Dependent", "False");
            Header.Add("MSX", "Not sure what this is");
            Header.Add("SourceFragmentation", scan.DissociationType == null ? "off" : "on");
            Header.Add("SourceFragmentationEnergy", scan.HcdEnergy);
        }

        #endregion

        /// <summary>
        /// Returns a IMsScanInstance from the data randomly selecting a scan
        /// </summary>
        public static IMsScanInstance GetRandomScan()
        {
            string filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"DataFiles\TDYeastFractionMS1.mzML");
            List<MsDataScan> scans = Mzml.LoadAllStaticData(filepath).GetAllScansList().Where(p => p.MsnOrder == 1).ToList();
            Random rand = new();
            MsDataScan scan = scans[rand.Next(scans.Count)];

            return new IMsScanInstance(scan);
        }

        public void Dispose()
        {
            return;
        }
    }



   
}
