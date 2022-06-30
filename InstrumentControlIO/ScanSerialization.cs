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

			filepath = Path.Combine(folderpath, @"CentroidMzInt.txt");
			foreach (var centroid in scan.Centroids)
			{
				Tuple<double, double> mzInt = new(centroid.Mz, centroid.Intensity);
				JsonSerializerDeserializer.SerializeAndAppend<Tuple<double,double>>(mzInt, filepath);
			}

			filepath = Path.Combine(folderpath, @"ChargeEnvelopes.txt");
            JsonSerializerDeserializer.SerializeAndAppend<IChargeEnvelope[]>(scan.ChargeEnvelopes, filepath);

			filepath = Path.Combine(folderpath, @"NoiseCount.txt");
			JsonSerializerDeserializer.SerializeAndAppend<int?>(scan.NoiseCount, filepath);

			filepath = Path.Combine(folderpath, @"NoiseBand.txt");
			JsonSerializerDeserializer.SerializeAndAppend<IEnumerable<INoiseNode>>(scan.NoiseBand, filepath);

		}

        public static IMsScan ImportIMsScan(string folderpath)
        {
			if (!Directory.Exists(folderpath))
            {
				throw new ArgumentException("Folder does not exist");
            }

			var centroidValues = JsonSerializerDeserializer.DeserializeCollection<Tuple<double, double>>(Path.Combine(folderpath, @"CentroidMzInt.txt"));
			List<ICentroid> centroids = new();           
			foreach (var value in centroidValues)
            {
				centroids.Add(new ICentroidInstance(value.Item1, value.Item2));
            }

			
            var scan = new IMsScanInstance() 
			{
				CentroidCount = JsonSerializerDeserializer.Deserialize<int?>(Path.Combine(folderpath, @"CentroidCount.txt"), true),
				Header = JsonSerializerDeserializer.Deserialize<IDictionary<string, string>>(Path.Combine(folderpath, @"Header.txt"), true),
				StatusLog = JsonSerializerDeserializer.Deserialize<IInformationSourceAccess>(Path.Combine(folderpath, @"StatusLog.txt"), true),
				Trailer = JsonSerializerDeserializer.Deserialize<IInformationSourceAccess>(Path.Combine(folderpath, @"Trailer.txt"), true),
				TuneData = JsonSerializerDeserializer.Deserialize<IInformationSourceAccess> (Path.Combine(folderpath, @"TuneData.txt"), true),
				Centroids = centroids,
				ChargeEnvelopes = JsonSerializerDeserializer.Deserialize<IChargeEnvelope[]>(Path.Combine(folderpath, @"ChargeEnvelopes.txt"), true),
				NoiseCount = JsonSerializerDeserializer.Deserialize<int?>(Path.Combine(folderpath, @"NoiseCount.txt"), true),
				NoiseBand = JsonSerializerDeserializer.Deserialize<IEnumerable<INoiseNode>>(Path.Combine(folderpath, @"NoiseBand.txt"), true),
			};

			return (IMsScan)scan;
        }
    }
}
