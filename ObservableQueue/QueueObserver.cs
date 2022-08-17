using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservableQueue
{
    public class QueueObserver<T> : IObserver<ObservableQueue<T>>
    {
        private IDisposable _unsubscriber; 
        public string InstName { get; set; }

        public virtual void Subscribe(IObservable<ObservableQueue<T>> observableQueue)
        {
            if (observableQueue != null)
            {
                _unsubscriber = observableQueue.Subscribe(this); 
            }
        }
        public virtual void Unsubscribe()
        {
            _unsubscriber.Dispose();
        }
        public void OnCompleted()
        {
            Unsubscribe();
        }

        public void OnError(Exception e)
        {
            Console.WriteLine("Failure in instructions queue."); 
        }

        public void OnNext(ObservableQueue<T> value)
        {
            
        }
    }
}
