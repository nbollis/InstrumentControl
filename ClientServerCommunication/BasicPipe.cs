using System;
using System.Text;
using System.IO.Pipes;
using System.Linq;
using System.Threading.Tasks;
using InstrumentControlIO;

namespace ClientServerCommunication
{
    public abstract class BasicPipe
    {
        public event EventHandler<PipeEventArgs> DataReceived;
        public event EventHandler<EventArgs> PipeClosed;
        public event EventHandler<EventArgs> PipeConnected;
        protected PipeStream PipeStream { get; set; }
        protected Action<BasicPipe> asyncReaderStart;
        protected delegate T3 QueueProcessing<in T1, in T2, out T3>(T1 scansEnumerable, T2 workflowParams) where T3 : new();

        public BasicPipe()
        {

        }
        public void Close()
        {
            PipeStream.WaitForPipeDrain();
            PipeStream.Close();
            PipeStream.Dispose();
            PipeStream = null;
        }
        public void Flush()
        {
            PipeStream.Flush();
        }
        protected void Connected()
        {
            PipeConnected?.Invoke(this, EventArgs.Empty);
        }
        protected void StartByteReaderAsync(Action<byte[]> packetReceived)
        {
            int intSize = sizeof(int);
            byte[] bDataLength = new byte[intSize];

            PipeStream.ReadAsync(bDataLength, 0, intSize).ContinueWith(t =>
            {
                int len = t.Result;

                if (len == 0)
                {
                    PipeClosed?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    int dataLength = BitConverter.ToInt32(bDataLength, 0);
                    byte[] data = new byte[dataLength];

                    PipeStream.ReadAsync(data, 0, dataLength).ContinueWith(t2 =>
                    {
                        len = t2.Result;

                        if (len == 0)
                        {
                            PipeClosed?.Invoke(this, EventArgs.Empty);
                        }
                        else
                        {
                            packetReceived(data);
                            StartByteReaderAsync(packetReceived);
                        }
                    });
                }
            });
        }
        public Task WriteString(string str)
        {
            return WriteBytes(Encoding.UTF8.GetBytes(str));
        }

        public Task WriteBytes(byte[] bytes)
        {
            var blength = BitConverter.GetBytes(bytes.Length);
            var bfull = blength.Concat(bytes).ToArray();

            return PipeStream.WriteAsync(bfull, 0, bfull.Length);
        }
        /// <summary>
        /// Reads an array of bytes, where the first [n] bytes (based on the server's intsize) indicates the number of bytes to read
        /// to complete the packet.
        /// </summary>
        public void StartByteReaderAsync()
        {
            StartByteReaderAsync((b) =>
              DataReceived?.Invoke(this, new PipeEventArgs(b, b.Length)));
        }

        /// <summary>
        /// Reads an array of bytes, where the first [n] bytes (based on the server's intsize) indicates the number of bytes to read
        /// to complete the packet, and invokes the DataReceived event with a string converted from UTF8 of the byte array.
        /// </summary>
        public void StartStringReaderAsync()
        {
            StartByteReaderAsync((b) =>
            {
                string str = Encoding.UTF8.GetString(b).TrimEnd('\0');
                DataReceived?.Invoke(this, new PipeEventArgs(str));
            });
        }
        public void StartReaderAsync(Action<byte[]> handler = null)
        {
            StartByteReaderAsync((b) =>
            {
                DataReceived?.Invoke(this, new PipeEventArgs(b, b.Length));
                handler?.Invoke(b);
            });
        }

        public virtual T OnDataReceived<T>(object sender, PipeEventArgs e) where T : new()
        {
            // deserialization specifics go here
            return new T();
        }

        public T DeserializeByteStream<T>(byte[] buffer)
        {
            string jsonStr = Encoding.UTF8.GetString(buffer);
            return JsonSerializerDeserializer.Deserialize<T>(jsonStr, false);
        }

        protected virtual T3 RunQueueProcessing<T1, T2, T3>(T1 scans, T2 workflowParams,
            Func<T1, T2, T3> processingWorkflow)
        {
            return processingWorkflow(scans, workflowParams);
        }
    }
}
