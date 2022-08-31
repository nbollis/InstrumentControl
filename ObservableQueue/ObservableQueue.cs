using System; 
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.VisualBasic;

namespace ObservableQueue
{
    public class ObservableQueue<T> : Queue<T>, IObservable<T>, INotifyCollectionChanged
    {
        private List<IObserver<T>> _observers;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public virtual void Enqueue(T item)
        {
            base.Enqueue(item);
            if (CollectionChanged != null)
                CollectionChanged(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add)); 
        }

        public int Count
        {
            get { return base.Count; }
        }
        public virtual T Dequeue()
        {
            T item = base.Dequeue();
            if (CollectionChanged != null)
            {
                CollectionChanged(this, 
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
            }
            return item; 
        }
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if(!_observers.Contains(observer))
                _observers.Add(observer);
            return new Unsubscriber(_observers, observer); 
        }

        public new void Clear()
        {
            base.Clear();
            if (CollectionChanged != null)
            {
                CollectionChanged(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)); 
            }
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<T>> _observers; 
            private IObserver<T> _observer;
            public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
            {
                this._observers = observers;
                this._observer = observer; 
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer); 
            }
        }
        
        
    }
}