using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer
{
    public interface IApplication
    {
        virtual void GetOptions<T>(T options)
        {

        }
    }
}
