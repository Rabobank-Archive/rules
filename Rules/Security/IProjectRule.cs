namespace SecurePipelineScan.Rules.Security
{
    public interface IProjectRule
    {
        string Description { get; }
        bool Evaluate(string project);
    }
}