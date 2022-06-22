using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using InstrumentControl.Interfaces;
using TaskInterfaces;


namespace InstrumentControl
{
    public class InstrumentControlTask
    { 
        public virtual void RunSpecific<T, U>(T otpions, U? data) 
            where T : ITaskOptions<T>
            where U : IData<T>
        {

        }

    }
}
