using InstrumentControl;
using MassSpectrometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public static class TestWholeChrgeEnvelopeApplication
    {

        [Test]
        public static void ThrowDataAtIt()
        {
            string filepath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"DataFiles\TDYeastFractionMS1.mzML");
            List<MsDataScan> scans = MS1DatabaseParser.LoadAllScansFromFile(filepath);
            List<MzSpectrum> spectra = scans.Select(p => p.MassSpectrum).ToList();
            double[] totalIonCurrent = scans.Select(p => p.TotalIonCurrent).ToArray();
            List<MzSpectrum> averagedSpectra = new List<MzSpectrum>();
            int spectraToSend = 5;

            WholeChargeEnvelopeFragmentationApplication app = new WholeChargeEnvelopeFragmentationApplication();

            for (int i = 0; i < scans.Count; i += spectraToSend)
            {
                var spectraForApp = spectra.GetRange(i, i + spectraToSend);
                app.TestItOut(spectra.GetRange(i, i + spectraToSend), totalIonCurrent[i..(i + spectraToSend)]);
            }

        }

    }
}
