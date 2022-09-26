namespace WorkflowServer
{
    public interface INormalizationOptions : ITaskOptions
    {
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
