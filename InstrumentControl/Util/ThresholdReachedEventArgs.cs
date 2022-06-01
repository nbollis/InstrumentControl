using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;

namespace InstrumentControl
{
    public class ThresholdReachedEventArgs : EventArgs
    {
        public List<IMsScan> Data { get; set; }
        public ThresholdReachedEventArgs(Queue<IMsScan> dataQueue, int numberElementToDequeue)
        {
            Data = new List<IMsScan>(numberElementToDequeue);
            DequeueAndSendToList(dataQueue, numberElementToDequeue);
        }
        private void DequeueAndSendToList(Queue<IMsScan> dataQueue, int numberToDequeue)
        {
            Data = DequeueMany<IMsScan>(dataQueue, numberToDequeue).ToList();
        }
        private IEnumerable<T> DequeueMany<T>(Queue<T> queue, int size)
        {
            for (int i = 0; i < size; i++)
            {
                yield return queue.Dequeue();
            }
        }
    }
}
