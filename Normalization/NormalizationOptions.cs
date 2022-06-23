
using CommandLine; 
using TaskInterfaces; 

namespace Normalization
{
    public interface INormalizationOptions : ITaskOptions<INormalizationOptions>
    {
        [Option]
        public bool PerformNormalization { get; set; }
    }
}
