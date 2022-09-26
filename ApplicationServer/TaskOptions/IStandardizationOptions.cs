
namespace WorkflowServer
{
    public interface IStandardizationOptions : ITaskOptions
    {
        public double MinMass { get; set; }
        public double MaxMass { get; set; }
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
