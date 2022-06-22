using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalization
{
    public static class SpectrumNormalization
    {
        public static void NormalizeSpectrumToTic(ref double[] intensityArray, double ticVal)
        {
            for(int i = 0; i < intensityArray.Length; i++)
            {
                intensityArray[i] /= ticVal; 
            }
        }
    }
}
