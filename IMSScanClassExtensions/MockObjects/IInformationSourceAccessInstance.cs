using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.SpectrumFormat_V1;

namespace IMSScanClassExtensions
{
    public class IInformationSourceAccessInstance : IInformationSourceAccess
    {
        public IEnumerable<string> ItemNames {get; set;}

        public bool Available { get; set; }

        public bool Valid { get; set; }

        public bool TryGetRawValue(string name, out object value)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string name, out string value)
        {
            throw new NotImplementedException();
        }
    }
}
