using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using ClientServerCommunication;
using InstrumentControlIO; 


namespace ClientServerCommunication
{
    public static class SingleScanDataObjectExtensions
    {
        public static void WriteSingleScanToPipe(this SingleScanDataObject singleScan, ClientPipe clientPipe)
        {
            byte[] buffer = JsonSerializerDeserializer.SerializeToBytes(singleScan);
            clientPipe.WriteBytes(buffer); 
        }
        
    }
}
