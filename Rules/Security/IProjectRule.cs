namespace SecurePipelineScan.Rules.Security
{
    public interface IProjectRule
    {
        string Description { get; }
        string Why { get; }
        bool Evaluate(string project);
    }
}