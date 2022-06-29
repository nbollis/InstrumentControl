using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Scans;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;
using Thermo.Interfaces.SpectrumFormat_V1;

namespace ScanProduction
{
    public interface IBaseScan
    {
        // Common Settings
        public double FirstMass { get; set; }
        public double LastMass { get; set; }
        public Analyzer Analyzer { get; set; }
        public ScanType ScanType { get; set; }
        public double SourceCIDEnergy { get; set; }
        public double SrcRFLens { get; set; }
        public Polarity Polarity { get; set; }
        public DataType DataType { get; set; }
        public IsolationMode IsolationMode { get; set; }
        public double AGCTarget { get; set; }
        public double MaxIT { get; set; }
        public int MicroScans { get; set; }
        public OrbitrapResolution OrbitrapResolution { get; set; }


        /// <summary>
        /// Allows user to set value of the properties dynamically. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setting"></param><summary>Enum value used to specify setting of the value to be changed.</summary>
        /// <param name="valueToSet"></param><summary>Value the scan setting should be changed to. </summary>
        /// <exception cref="ArgumentException"></exception>
        protected static void SetCustomValue<T>(IBaseScan scan, InstrumentSettings setting, T valueToSet)
        {
            // get string from the instrument setting enum 
            string propertyName = Enum.GetName(setting);
            // set the value of the property with the value passed. 
            if (propertyName is not null)
            {
                // false positive warning. Checking for null reference, so this error will 
                // not occur. 
                // dynamically gets the type of the property. 
                var propertyType = scan.GetType().GetProperty(propertyName).PropertyType;
                if (propertyType.HasElementType)
                {
                    // Sets the property value using Convert.ChangeType to perform any required casting of input value to property value. 
                    // Note: You should probably do your best to add the correct type. 
                    scan.GetType().GetProperty(propertyName).SetValue(scan, Convert.ChangeType(valueToSet, propertyType.GetType()));
                }
            }
            else
            {
                throw new ArgumentException("Invalid property name");
            }
        }
        public Dictionary<string, string> BuildDictionary()
        {
            Dictionary<string, string> scanDict = new();

            foreach (var property in GetType().GetProperties())
            {
                // property name = string to set to key in dictionary
                // value = orbitrap resolution value
                // name = all other enum values. 

                var objectType = property.PropertyType;
                string propertyName = property.Name;

                if (objectType.IsEnum)
                {
                    if (propertyName == "OrbitrapResolution")
                    {
                        string value = ((int)property.GetValue(this, null)).ToString();
                        scanDict.Add("OrbitrapResolution", value);
                    }
                    string val = property.GetValue(this).ToString();
                    string name = propertyName;
                    scanDict.Add(name, val);
                }
                else if (objectType.IsValueType)
                {
                    string val = property.GetValue(this).ToString();
                    scanDict.Add(propertyName, val);
                }
                else
                {
                    throw new ArgumentException("Error in object conversion to dictionary. " +
                        "Unable to return correctly formatted object.");
                }
            }
            return scanDict;
        }
        
        public void CheckDefaults<T>(InstrumentSettings setting, T value)
        {
            // TODO: Implement generic validity testing for all values. 
            // Probably use a switch and the InstrumentSettings enum. 
            throw new NotImplementedException();
        }

        public void SetValuesToCustomScan(ICustomScan customScan);



    }

}
