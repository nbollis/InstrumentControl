using System;
using System.IO.Pipes;
using Data; 
using System.Collections.Generic;


namespace ClientServerCommunication
{
    public class ServerPipe : BasicPipe
    {
        private Queue<SingleScanDataObject> _processingQueue = new Queue<SingleScanDataObject>();
        public event EventHandler<EventArgs> Connected;
        protected NamedPipeServerStream ServerPipeStream;
        protected string PipeName { get; set; }

        public ServerPipe(string pipeName, Action<BasicPipe> asyncReaderStart, PipeTransmissionMode mode)
        {
            this.asyncReaderStart = asyncReaderStart;
            PipeName = pipeName;

            ServerPipeStream = new NamedPipeServerStream(
                pipeName,
                PipeDirection.InOut,
                maxNumberOfServerInstances: NamedPipeServerStream.MaxAllowedServerInstances,
                transmissionMode: mode);
            PipeStream = ServerPipeStream;
            ServerPipeStream.BeginWaitForConnection(new AsyncCallback(PipeConnected), null);
        }

        public ServerPipe()
        {

        }

        protected void PipeConnected(IAsyncResult ar)
        {
            ServerPipeStream.EndWaitForConnection(ar);
            Connected?.Invoke(this, EventArgs.Empty);
            asyncReaderStart(this);
        }

        public void AddScanToProcessingQueue(SingleScanDataObject scan)
        {
            _processingQueue.Enqueue(scan);
        }
    }
}

