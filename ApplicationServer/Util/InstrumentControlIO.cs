using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Proteomics;

namespace WorkflowServer
{
    /// <summary>
    /// This class is about the implementation of inputs and outputs. 
    /// </summary>
    /// 
    public static class InstrumentControlIO
    {
        private static string pathToCheck = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataFiles");
        /// <summary>
        /// Create a file that contains the serialized object. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">The path the file will be created</param>
        /// <param name="objectToSerialize">The object to serialize</param>
        public static void Serialize<T>(string path, T objectToSerialize)
        {
            string json = JsonConvert.SerializeObject(objectToSerialize);
            var stream = File.Create(path);
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine(json);
            }
            stream.Close();
        }

        /// <summary>
        /// Return a json deserialized object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">The path of file that will be deserialized</param>
        /// <returns>The deserialized object</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T Deserialize<T>(string path)
        {
            string objecttoDeserialize;
            var stream = File.Open(path, FileMode.Open);
            using (StreamReader writer = new StreamReader(stream))
            {
                objecttoDeserialize = writer.ReadLine() ?? throw new ArgumentNullException() ;
            }
            stream.Close();
            var json = JsonConvert.DeserializeObject<T>(objecttoDeserialize) ?? throw new ArgumentNullException();
            return json;
        }

        /// <summary>
        /// Check if the type exist
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>True if it exist, false if it does not</returns>
        public static bool CheckforTypeFolder(Type type)
        {
            string path = Path.Combine(pathToCheck, type.ToString());
            return Directory.Exists(path);      
        }

        /// <summary>
        /// Return the path of the folder (type)
        /// </summary>
        /// <param name="folder">The type to check</param>
        /// <param name="name">The name of the type</param>
        /// <returns></returns>
        public static string GetFilePath(Type folder, string name)
        {
            string txtFilePath = folder.ToString() + "/" + name + ".txt";
            string path = Path.Combine(pathToCheck, txtFilePath);
            return path;
        }

        /// <summary>
        /// Create the type's folder if it does not exist, save it if it exists. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">name of the object to save</param>
        /// <param name="o">object to save</param>
        public static void Save<T>(string name, T o)
        {
            if (!CheckforTypeFolder(o.GetType()))
            { 
                string pathOfFolder = Path.Combine(pathToCheck, o.GetType().ToString());
                Directory.CreateDirectory(pathOfFolder);
            }
            string path = GetFilePath(o.GetType(), name);
            Serialize<T>(path, o);
        }

        /// <summary>
        /// Return an IRnumerable list of all deserialized file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="folder">The folder to check</param>
        /// <returns>IRnumerable list of all deserialized file</returns>
        public static IEnumerable<T> LoadAllOfTypeT<T>(Type folder)
        {
            if (CheckforTypeFolder(folder)){
                string pathOfFolder = Path.Combine(pathToCheck, folder.ToString());
                string[] contentsInFolder = Directory.GetFiles(pathOfFolder);
                foreach (string path in contentsInFolder)
                {
                    yield return Deserialize<T>(path);
                }
            }
        }

        /// <summary>
        /// Check data folder and get all subfolders
        /// </summary>
        /// <returns>a dictionary entry of type and deserialized collection</returns>
        public static Dictionary<Type, List<object>> LoadAll()
        {
            Dictionary<Type, List<object>> dictionary = new();
            string[] folders = Directory.GetDirectories(pathToCheck);
            foreach(string folder in folders)
            {
                //var temp = folder.Split(@"\");
                string folderName = folder.Split(@"\").Last();
                var type = Type.GetType(folderName);
                if (type == null)
                {
                    var list = folderName.Split(@".").Take(folderName.Split('.').Length - 1);
                    string s = string.Join(".", list);
                    type = Type.GetType(folderName + "," + s);
                    //dictionary.Add(Type.GetType(folderName+","+s), LoadAllOfTypeT<object>(Type.GetType(folderName + "," + s)).ToList());
                }
                
                
                    //var temp = Type.GetType("MassSpectrometry.MzSpectrum,MassSpectrometry");
                    dictionary.Add((type), LoadAllOfTypeT<object>(type).ToList());
                
                //LoadAllOfTypeT<object>(Type.GetType(folderName)).ToList();
            }
            return dictionary;
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">type of file to delete</param>
        /// <param name="name">name of the file</param>
        public static void Delete<T>(Type type, string name)
        {
            if (CheckforTypeFolder(type))
            {
                string filePath = GetFilePath(type, name);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        /// <summary>
        /// Returns the names of each saved object of Type passed
        /// </summary>
        /// <param name="type">Type of object to load names for</param>
        /// <returns></returns>
        public static string[] LoadAllNamesOfType(Type type)
        {
            string path = Path.Combine(pathToCheck, type.ToString());
            var folders = Directory.GetFiles(path)
                .Select(p => Path.GetFileNameWithoutExtension(p)).ToArray();
            return folders;
        }
    }
}
namespace WorkflowServer.Test.Testing
{
    public class Dummy
    {
        public int intTest = 2;

        public Dummy()
        {

        }
    }
}