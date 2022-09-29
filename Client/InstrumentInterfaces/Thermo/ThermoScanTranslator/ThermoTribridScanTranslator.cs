using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;

namespace Client
{
    public class ThermoTribridScanTranslator : ScanTranslator
    {
        public Dictionary<string,string> InstrumentCompatibleScanInstructions { get; private set; }
        public override void Translate(SingleScanDataObject ssdo)
        {
            throw new NotImplementedException();
        }
        private Dictionary<string, string> TranslateSsdo(SingleScanDataObject ssdo)
        {
            var result = new Dictionary<string, string>();
            PropertyInfo[] properties = typeof(ScanInstructions).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                // set result key by matching the property name to the constant dictionary key 
                // and returning the value. 
                string newKey = ThermoTribridSsdoMapping.TrbridToSsdoMapping[property.Name];
                var propType = property.GetType();
                string valConvertToString = String.Empty;

                switch (propType.Name)
                {
                    case nameof(Double):
                        break;
                    case nameof(Int32):
                        break;
                    case nameof(Enum):
                        break; 
                }

            }

            return result; 
        }
        private static string GetEnumString<T>(T enumOfType) where T : Enum
        {
            var enumType = typeof(T);
            if (enumType == typeof(OrbitrapResolution))
            {
                var temp = (int)Enum.Parse(typeof(OrbitrapResolution), enumOfType.ToString());
                return temp.ToString();
            }
            return Enum.Parse(typeof(T), enumOfType.ToString()).ToString();
        }
    }
}
