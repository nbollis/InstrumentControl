using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskInterfaces
{
    public interface IApplicationOptions
    {
        public bool Live { get; set; }
        public U InstantiateTask<U>()
            where U : class, new()
        {
            return new U(); 
        }
        public void TestMethod() => Console.WriteLine("Test"); 
    }
}
