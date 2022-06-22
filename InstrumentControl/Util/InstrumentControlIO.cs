using MassSpectrometry;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    public static class InstrumentControlIO
    {

        /// <summary>
        /// Serializes the incoming object to a new  file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <param name="filepath"></param>
        public static void SerializeToNewFile<T>(T objectToSerialize, string filepath)
        {
            using (StreamWriter streamWriter = File.CreateText(filepath))
            {
                streamWriter.WriteLine(JsonConvert.SerializeObject(objectToSerialize));
            }
        }

        /// <summary>
        /// Serializes the incoming object and appends it to an existing file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <param name="filepath"></param>
        public static void SerilaizeAndAppend<T>(T objectToSerialize, string filepath)
        {
            using (StreamWriter streamWriter = File.AppendText(filepath))
            {
                streamWriter.WriteLine(JsonConvert.SerializeObject(objectToSerialize));
            }
        }

        /// <summary>
        /// Converts a txt file of json strings into an Enumerable of the specified object type
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<T> DeserializeCollection<T>(string filepath)
        {
            string[] lines = File.ReadAllLines(filepath);
            foreach (var line in lines)
            {
                yield return Deserialize<T>(line, false);
            }
        }

        /// <summary>
        /// Converts an array of json strings to an enumerable of the specified object type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonStrings"></param>
        /// <returns></returns>
        public static IEnumerable<T> DeserializeCollection<T>(string[] jsonStrings)
        {
            foreach (var json in jsonStrings)
            {
                yield return Deserialize<T>(json, false);
            }
        }

        /// <summary>
        /// Converts a txt file with a single json string to the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string jsonString, bool stringIsFilePath)
        {
            if (stringIsFilePath)
            {
                jsonString = File.ReadAllText(jsonString);
            }



            if (typeof(T) == typeof(MzSpectrum))
            {
                var obj = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(jsonString);
                object mzSpectrum = new MzSpectrum(obj.GetValue("XArray").ToObject<double[]>(), obj.GetValue("YArray").ToObject<double[]>(), true);
                return (T)mzSpectrum;
            }
            else
            {
                object obj = JsonConvert.DeserializeObject(jsonString, typeof(T));
                return (T)obj;
            }
        }



    }
}
