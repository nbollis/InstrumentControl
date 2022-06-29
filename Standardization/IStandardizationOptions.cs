using TaskInterfaces;
using CommandLine; 

namespace TaskInterfaces
{
    public interface IStandardizationOptions : ITaskOptions
    {
        [Option]
        public double MinMass { get; set; }
        [Option]
        public double MaxMass { get; set; }
        [Option]
        public double Delta { get; set; }
    }
    public class StandardizationOptions : IStandardizationOptions
    {
        public double MinMass { get; set; }
        public double MaxMass { get; set; }
        public double Delta { get; set; }
        public StandardizationOptions()
        {

        }
    }
}
