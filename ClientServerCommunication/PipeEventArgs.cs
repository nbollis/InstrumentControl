using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json; 

namespace ClientServerCommLibrary
{
    /// <summary>
    /// Class used to pass buffer sent from client to server or server to client. 
    /// </summary>
    public class PipeEventArgs : EventArgs
    {
        public byte[] Buffer;

        public PipeEventArgs(byte[] buff)
        {
            Buffer = buff;
        }
    }

    public static class PipeEventArgsOverrides
    {
        public static SingleScanDataObject ToSingleScanDataObject(this PipeEventArgs args)
        {
            // get the string: 
            string jsonziedObject = Encoding.UTF8.GetString(args.Buffer);
            return JsonConvert.DeserializeObject<SingleScanDataObject>(jsonziedObject);
        }
    }
}
