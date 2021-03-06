using IO.MzML;
using IO.ThermoRawFileReader;
using MassSpectrometry;
using MzLibUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControlIO
{
    public static class SpectraFileHandler
    {
		/// <summary>
		/// Creates a List of MsDataScans from a spectra file. Currently supports MzML and raw
		/// </summary>
		/// <param name="filepath"></param>
		/// <returns></returns>
		/// <exception cref="MzLibException"></exception>
		public static List<MsDataScan> LoadAllScansFromFile(string filepath)
		{
			List<MsDataScan> scans = new();
			if (filepath.EndsWith(".mzML"))
				scans = Mzml.LoadAllStaticData(filepath).GetAllScansList();
			else if (filepath.EndsWith(".raw"))
				scans = ThermoRawFileReader.LoadAllStaticData(filepath).GetAllScansList();
			else
			{
				throw new MzLibException("Cannot load spectra");
			}
			return scans;
		}

		/// <summary>
		/// Creates a List of MsDataScans from a spectra file
		/// </summary>
		/// <param name="filepath"></param>
		/// <param name="start">OneBasedScanNumber of the first scan</param>
		/// <param name="end">Optional: will return only one scan if blank</param>
		/// <returns></returns>
		public static List<MsDataScan> LoadSelectScansFromFile(string filepath, int start, int end = -1)
		{
			if (end == -1)
			{
				end = start + 1;
			}
			List<MsDataScan> scans = LoadAllScansFromFile(filepath);
			List<MsDataScan> trimmedScans = scans.GetRange(start - 1, (end - start));
			return trimmedScans;
		}

		/// <summary>
		/// returns the MS1's only from a file
		/// </summary>
		/// <param name="filepath"></param>
		/// <returns></returns>
		public static List<MsDataScan> LoadMS1ScansFromFile(string filepath)
        {
			return LoadAllScansFromFile(filepath).Where(p => p.MsnOrder == 1).ToList();
        }






		// TODO: Make it so the list of combined scans can be exported as an openable Mzml
		public static void SaveMergedScanAsMzml(List<MsDataScan> combinedScans, string outputPath)
        {
            SourceFile temp = new SourceFile(null, null, null, null, null);
            MsDataFile combinedScansFile = new MsDataFile(combinedScans.ToArray(), temp);
            MzmlMethods.CreateAndWriteMyMzmlWithCalibratedSpectra(combinedScansFile, outputPath, false);
        }
    }
}
