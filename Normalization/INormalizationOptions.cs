
using CommandLine; 
using TaskInterfaces; 

namespace Normalization
{
    public interface INormalizationOptions : ITaskOptions<INormalizationOptions>
    {
        [Option]
        public bool PerformNormalization { get; set; }
    }
    public class NormalizationOptions : INormalizationOptions
    {
        public bool PerformNormalization { get; set; }  
        public NormalizationOptions()
        {

        }
    }
}
