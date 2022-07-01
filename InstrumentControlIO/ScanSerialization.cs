using IMSScanClassExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;
using Thermo.Interfaces.SpectrumFormat_V1;

namespace InstrumentControlIO
{
    public class ScanSerialization
    {
		public static void ExportIMsScan(IMsScan scan, string folderpath)
		{
			string filepath;
			if (!Directory.Exists(folderpath))
			{
				Directory.CreateDirectory(folderpath);
			}

			filepath = Path.Combine(folderpath, @"CentroidCount.txt");
			JsonSerializerDeserializer.SerializeAndAppend<int?>(scan.CentroidCount, filepath);

			filepath = Path.Combine(folderpath, @"Header.txt");
			JsonSerializerDeserializer.SerializeAndAppend<IDictionary<string, string>>(scan.Header, filepath);

			filepath = Path.Combine(folderpath, @"StatusLog.txt");
			JsonSerializerDeserializer.SerializeAndAppend<IInformationSourceAccess>(scan.StatusLog, filepath);

			filepath = Path.Combine(folderpath, @"Trailer.txt");
			JsonSerializerDeserializer.SerializeAndAppend<IInformationSourceAccess>(scan.Trailer, filepath);

			filepath = Path.Combine(folderpath, @"TuneData.txt");
			JsonSerializerDeserializer.SerializeAndAppend<IInformationSourceAccess>(scan.TuneData, filepath);

			filepath = Path.Combine(folderpath, @"ChargeEnvelopes.txt");
			JsonSerializerDeserializer.SerializeAndAppend<IChargeEnvelope[]>(scan.ChargeEnvelopes, filepath);

			filepath = Path.Combine(folderpath, @"NoiseCount.txt");
			JsonSerializerDeserializer.SerializeAndAppend<int?>(scan.NoiseCount, filepath);

			filepath = Path.Combine(folderpath, @"NoiseBand.txt");
			JsonSerializerDeserializer.SerializeAndAppend<IEnumerable<INoiseNode>>(scan.NoiseBand, filepath);

			filepath = Path.Combine(folderpath, @"DetectorName.txt");
			JsonSerializerDeserializer.SerializeAndAppend<string>(scan.DetectorName, filepath);

			string centroidFolderpath = Path.Combine(folderpath, @"centroids");
			Directory.CreateDirectory(centroidFolderpath);
			foreach (var centroid in scan.Centroids)
			{
				Tuple<double, double> mzInt = new(centroid.Mz, centroid.Intensity);
				filepath = Path.Combine(centroidFolderpath, @"CentroidMzInt.txt");
				JsonSerializerDeserializer.SerializeAndAppend<Tuple<double,double>>(mzInt, filepath);

				bool?[] bools = new bool?[6];
				bools[0] = centroid.IsExceptional;
				bools[1] = centroid.IsReferenced;
				bools[2] = centroid.IsMerged;
				bools[3] = centroid.IsFragmented;
				bools[4] = centroid.IsMonoisotopic;
				bools[5] = centroid.IsClusterTop;
				filepath = Path.Combine(centroidFolderpath, @"Bools.txt");
                JsonSerializerDeserializer.SerializeAndAppend<bool?[]>(bools, filepath);

				int?[] ints = new int?[2];
				ints[0] = centroid.Charge;
				ints[1] = centroid.ChargeEnvelopeIndex;
				filepath = Path.Combine(centroidFolderpath, @"Ints.txt");
				JsonSerializerDeserializer.SerializeAndAppend<int?[]>(ints, filepath);
			}
		}

        public static IMsScan ImportIMsScan(string folderpath)
        {
			if (!Directory.Exists(folderpath))
            {
				throw new ArgumentException("Folder does not exist");
            }

			string centroidFolderpath = Path.Combine(folderpath, @"centroids");
			Tuple<double, double>[] centroidValues = JsonSerializerDeserializer.DeserializeCollection<Tuple<double, double>>(Path.Combine(centroidFolderpath, @"CentroidMzInt.txt")).ToArray();
			bool?[][] bools = JsonSerializerDeserializer.DeserializeCollection<bool?[]>(Path.Combine(centroidFolderpath, @"Bools.txt")).ToArray();
			int?[][] ints = JsonSerializerDeserializer.DeserializeCollection<int?[]>(Path.Combine(centroidFolderpath, @"Ints.txt")).ToArray();

			List<ICentroid> centroids = new();           
			for (int i = 0; i < centroidValues.Count(); i++)
            {
				ICentroidInstance centroid = new()
				{
                    Mz = centroidValues[i].Item1,
					Intensity = centroidValues[i].Item2,
					IsExceptional = bools[i][0],
					IsReferenced = bools[i][1],
					IsMerged = bools[i][2],
					IsFragmented = bools[i][3],
					IsMonoisotopic = bools[i][4],
					IsClusterTop = bools[i][5],
					Charge = ints[i][0],
					ChargeEnvelopeIndex = ints[1][1]
				};
				centroids.Add(centroid);
            }
			
            var scan = new IMsScanInstance() 
			{
				CentroidCount = JsonSerializerDeserializer.Deserialize<int?>(Path.Combine(folderpath, @"CentroidCount.txt"), true),
				Header = JsonSerializerDeserializer.Deserialize<IDictionary<string, string>>(Path.Combine(folderpath, @"Header.txt"), true),
				StatusLog = JsonSerializerDeserializer.Deserialize<IInformationSourceAccessInstance>(Path.Combine(folderpath, @"StatusLog.txt"), true),
				Trailer = JsonSerializerDeserializer.Deserialize<IInformationSourceAccessInstance>(Path.Combine(folderpath, @"Trailer.txt"), true),
				TuneData = JsonSerializerDeserializer.Deserialize<IInformationSourceAccessInstance> (Path.Combine(folderpath, @"TuneData.txt"), true),
				ChargeEnvelopes = JsonSerializerDeserializer.Deserialize<IChargeEnvelope[]>(Path.Combine(folderpath, @"ChargeEnvelopes.txt"), true),
				NoiseCount = JsonSerializerDeserializer.Deserialize<int?>(Path.Combine(folderpath, @"NoiseCount.txt"), true),
				NoiseBand = JsonSerializerDeserializer.Deserialize<IEnumerable<INoiseNode>>(Path.Combine(folderpath, @"NoiseBand.txt"), true),
				DetectorName = JsonSerializerDeserializer.Deserialize<string>(Path.Combine(folderpath, @"DetectorName.txt"), true),
				Centroids = centroids,
			};

			return (IMsScan)scan;
        }
    }
}
