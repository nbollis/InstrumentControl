using ScanProduction;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TaskInterfaces;


namespace InstrumentControl
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
