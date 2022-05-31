using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    public class ThresholdReachedEventArgs : EventArgs
    {
        public List<double> Data { get; set; }
        public ThresholdReachedEventArgs(Queue<double> dataQueue, int numberElementToDequeue)
        {
            Data = new List<double>(numberElementToDequeue);
            DequeueAndSendToList(dataQueue, numberElementToDequeue);
        }
        private void DequeueAndSendToList(Queue<double> dataQueue, int numberToDequeue)
        {
            Data = DequeueMany<double>(dataQueue, numberToDequeue).ToList();
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
