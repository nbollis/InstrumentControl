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
        protected event EventHandler<PipeEventArgs> PipeDataReceived;
        protected NamedPipeClientStream ReadDataPipe { get; }
        protected NamedPipeServerStream SendDataPipe { get; }

        protected PipeHost(NamedPipeClientStream readDataPipe, NamedPipeServerStream sendDataPipe)
        {
            ReadDataPipe = readDataPipe;
            SendDataPipe = sendDataPipe;
        }

        protected abstract void HandleDataReceived(object obj, PipeEventArgs eventArgs);


        protected void StartReaderAsync()
        {
            StartByteReaderAsync((b) =>
                PipeDataReceived?.Invoke(this, new PipeEventArgs(b)));
        }
        protected void StartByteReaderAsync(Action<byte[]> packetReceived)
        {
            byte[] byteDataLength = new byte[sizeof(int)];

            ReadDataPipe.ReadAsync(byteDataLength, 0, sizeof(int))
                .ContinueWith(t =>
                {
                    int len = t.Result;

                    int dataLength = BitConverter.ToInt32(byteDataLength, 0);
                    byte[] data = new byte[dataLength];

                    ReadDataPipe.ReadAsync(data, 0, dataLength)
                        .ContinueWith(t2 =>
                        {
                            len = t2.Result;
                            packetReceived(data);
                            StartByteReaderAsync(packetReceived);
                        });
                });
        }
    }
}
