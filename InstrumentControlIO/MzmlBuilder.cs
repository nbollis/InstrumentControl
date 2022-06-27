using IO.MzML;
using MassSpectrometry;
using MzLibUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControlIO
{
    public static class MzmlBuilder
    {
        public static List<MsDataScan> ScanList { get; set; }
        


        public static void CreateAveragedMsDataScan()
        {
            // create scan
            //MsDataScan scan = new();

        }

        public static void ExportAsMzml(string outputPath)
        {
            // will need to adjust the source file
            SourceFile sourceFile = new(null, null, null, null, null);
            MsDataFile msDataFile = new(ScanList.ToArray(), sourceFile);
            MzmlMethods.CreateAndWriteMyMzmlWithCalibratedSpectra(msDataFile, outputPath, false);
        }
    }
}
