namespace SecurePipelineScan.Rules
{
    public interface IProjectRule
    {
        string Description { get; }
        bool Evaluate(string project);
    }
}