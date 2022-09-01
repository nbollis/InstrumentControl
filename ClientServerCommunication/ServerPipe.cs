using System;
using System.IO.Pipes;
using Data; 
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace ClientServerCommunication
{
    public class ServerPipe : BasicPipe
    {
        // public event EventHandler<EventArgs> Connected;
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
            IAsyncResult result = ServerPipeStream.BeginWaitForConnection(new AsyncCallback(PipeConnectedCallback), null);
            result.AsyncWaitHandle.WaitOne();
            result.AsyncWaitHandle.Close();
            Console.WriteLine("Pipe connected"); 
        }

        public ServerPipe()
        {

        }

        protected void PipeConnectedCallback(IAsyncResult ar)
        {
            ServerPipeStream.EndWaitForConnection(ar);
            base.Connected(); 
            asyncReaderStart(this);
        }
    }
}

