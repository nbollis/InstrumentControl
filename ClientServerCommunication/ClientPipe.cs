using System;
using System.IO.Pipes;
using Data; 
using System.Collections.Generic;

namespace ClientServerCommunication
{
    public class ClientPipe : BasicPipe
    {
        protected NamedPipeClientStream ClientPipeStream;
        private Queue<ScanInstructions> _scanQueue = new Queue<ScanInstructions>();

        public ClientPipe(string serverName, string pipeName, Action<BasicPipe> asyncReaderStart)
        {
            this.asyncReaderStart = asyncReaderStart;
            ClientPipeStream = new NamedPipeClientStream(serverName, pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            pipeStream = ClientPipeStream;
        }

        public void Connect()
        {
            ClientPipeStream.Connect();
            Connected();
            asyncReaderStart(this);
        }

        public void AddScansToQueue(ScanInstructions instr)
        {
            _scanQueue.Enqueue(instr);
        }
    }
}
