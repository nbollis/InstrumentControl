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
        protected void ParseAppOptions<T>(out T taskOpt)
            // new requires the class to have a parameterless constructor
                        where T : class, ITaskOptions<T>, new()
        {
            taskOpt = this as T; 
        }
    }
}
