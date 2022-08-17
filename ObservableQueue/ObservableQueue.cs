namespace ObservableQueue
{
    public class ObservableQueue<T> : Queue<T>, IObservable<T>
    {
        private static readonly Queue<T> _queue = new Queue<T>();
        private List<IObserver<T>> _observers;

        public virtual void Enqueue(T item)
        {
            _queue.Enqueue(item);
            _observers = new List<IObserver<T>>(); 
        }

        public int Count
        {
            get { return _queue.Count; }
        }
        public virtual T Dequeue()
        {
            T item = _queue.Dequeue();
            return item; 
        }
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if(!_observers.Contains(observer))
                _observers.Add(observer);
            return new Unsubscriber(_observers, observer); 
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