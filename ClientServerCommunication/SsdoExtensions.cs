using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ClientServerCommLibrary
{
    public static class SsdoExtensions
    {
        public static byte[] CreateSerializedSingleScanDataObject(this SingleScanDataObject ssdo)
        {
            byte[] buffer = SerializeAndCreateBuffer(ssdo);
            int buffLength = buffer.Length;
            byte[] lenghtInByteFormat = BitConverter.GetBytes(buffLength);

            // combine the two arrays 
            byte[] resultBuffer = new byte[lenghtInByteFormat.Length + buffLength];
            Buffer.BlockCopy(lenghtInByteFormat, 0, resultBuffer, 0, lenghtInByteFormat.Length);
            Buffer.BlockCopy(buffer, 0, resultBuffer, lenghtInByteFormat.Length, buffLength);
            return resultBuffer;
        }
        private static byte[] SerializeAndCreateBuffer<T>(T obj)
        {
            string jsonString = JsonConvert.SerializeObject(obj);
            return Encoding.UTF8.GetBytes(jsonString);
        }
    }

}
