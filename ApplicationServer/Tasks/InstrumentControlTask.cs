using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace WorkflowServer
{
    public class InstrumentControlTask
    { 
        public virtual void RunSpecific<T, U>(T options, U? data) 
            where T : ITaskOptions
            where U : IData
        {

        }

    }
}
