 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer
{
    public class ActivityCollection<TContext> : List<ActivityDescriptor<TContext>>, IActivityCollection<TContext>
        where TContext : IActivityContext
    {
        IActivity<TContext> IList<IActivity<TContext>>.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(IActivity<TContext> item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(IActivity<TContext> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IActivity<TContext>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(IActivity<TContext> item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, IActivity<TContext> item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IActivity<TContext> item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<IActivity<TContext>> IEnumerable<IActivity<TContext>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

}
