using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;
using Data; 

namespace InstrumentControl
{
    public class ThresholdReachedEventArgs : EventArgs
    {
        public List<SingleScanDataObject> Data { get; set; }
        public ThresholdReachedEventArgs(Queue<SingleScanDataObject> dataQueue, int numberElementToDequeue)
        {
            Data = new List<SingleScanDataObject>(numberElementToDequeue);
            DequeueAndSendToList(dataQueue, numberElementToDequeue);
        }
        private void DequeueAndSendToList(Queue<SingleScanDataObject> dataQueue, int numberToDequeue)
        {
            Data = DequeueMany<SingleScanDataObject>(dataQueue, numberToDequeue).ToList();
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
