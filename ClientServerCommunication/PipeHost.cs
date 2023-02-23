using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommLibrary
{
    public abstract class PipeHost
    {
        public NamedPipeClientStream ReadDataPipe { get; }
        public NamedPipeServerStream SendDataPipe { get; }

        protected PipeHost(NamedPipeClientStream readDataPipe, NamedPipeServerStream sendDataPipe)
        {
            ReadDataPipe = readDataPipe;
            SendDataPipe = sendDataPipe;
        }


    }
}
