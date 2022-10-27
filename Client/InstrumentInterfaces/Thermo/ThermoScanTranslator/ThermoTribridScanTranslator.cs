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
        public static Dictionary<string, string> TranslateSsdo(SingleScanDataObject ssdo)
        {
            var result = new Dictionary<string, string>();
            PropertyInfo[] properties = typeof(ScanInstructions).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                // set result key by matching the property name to the constant dictionary key 
                // and returning the value. 
                
                // get the key 
                string newKey = ThermoTribridSsdoMapping.TrbridToSsdoMapping[property.Name];
                string valConvertToString = String.Empty;

                // get the value
                string value = (string)property.GetValue(ssdo); 

                result.Add(newKey, value);
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
