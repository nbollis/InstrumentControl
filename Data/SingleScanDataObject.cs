using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Data
{
    [Serializable]
    public class SingleScanDataObject
    {
        public double[] XArray { get; set; }
        public double[] YArray { get; set; }
        public double TotalIonCurrent { get; set; }
        public double MinX { get; set; }
        public double MaxX { get; set; }

        public double Resolution { get; set; }


        public void UpdateYarray(double[] newYarray)
        {
            YArray = newYarray; 
        }

        // temp
        public List<double>[] GetChargeStateEnvelopeMz()
        {
            List<double>[] result = new List<double>[1] { new List<double>()};
            return result;
        }
        public SingleScanDataObject(double[] xArray, double[] yArray, double totalIonCurrent, double minX, double maxX, double resolution)
        {
            XArray = xArray;
            YArray = yArray;
            TotalIonCurrent = totalIonCurrent;
            MinX = minX;
            MaxX = maxX;
            Resolution = resolution;
        }
        public SingleScanDataObject()
        {

        }
    }
}
