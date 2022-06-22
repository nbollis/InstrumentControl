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
        public virtual void RunSpecific<T>(T otpions) where T : ITaskOptions<T>
        {

        }

    }
}
