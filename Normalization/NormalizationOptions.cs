
using CommandLine; 
using TaskInterfaces; 

namespace Normalization
{
    public class NormalizationOptions : ITaskOptions<NormalizationOptions>
    {
        [Option]
        public bool PerformNormalization { get; set; }
    }
}
