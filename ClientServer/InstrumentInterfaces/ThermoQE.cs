using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class ThermoQE : IInstrument
    {
        public void OpenInstrumentConnection()
        {
            throw new System.NotSupportedException();
        }

        public void CloseInstrumentConnection()
        {
            throw new NotSupportedException(); 
        }
    }
}
