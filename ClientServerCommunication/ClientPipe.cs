using System;
using System.IO.Pipes;
using Data; 
using System.Collections.Generic;

namespace ClientServerCommunication
{
    public class ClientPipe : BasicPipe
    {
        protected NamedPipeClientStream ClientPipeStream;

        public ClientPipe(string serverName, string pipeName, Action<BasicPipe> asyncReaderStart)
        {
            this.asyncReaderStart = asyncReaderStart;
            ClientPipeStream = new NamedPipeClientStream(serverName, pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            PipeStream = ClientPipeStream;
        }
        public ClientPipe()
        {

        }
        public void Connect()
        {
            ClientPipeStream.Connect();
            Connected();
            asyncReaderStart(this);
        }
    }
}
