using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.TNG.Factory;

namespace ClientServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string instrumentType = args[0];
            IInstrumentFactory factory = null; 
            switch (instrumentType)
            {
                case "qe":
                    factory = new ThermoQEFactory();
                    break;
                case "tribrid":
                    factory = new ThermoTribridFactory();
                    break; 
            }

            IInstrument instrumentApi = factory?.CreateInstrumentApi(); 
            instrumentApi?.OpenInstrumentConnection();


        }
    }
}
