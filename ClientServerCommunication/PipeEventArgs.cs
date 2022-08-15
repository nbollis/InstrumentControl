using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommunication
{
    public class PipeEventArgs
    {
        public byte[] Data { get; set; }
        public int Length { get; set; }
        public PipeEventArgs(byte[] bytes, int length)
        {
            Data = bytes;
            Length = length;
        }
        public PipeEventArgs(string str)
        {
            StringToBytes(str);
        }
        private void StringToBytes(string str)
        {
            Data = Encoding.UTF8.GetBytes(str);
            Length = Data.Length;
        }
    }
}
