namespace SecurePipelineScan.Rules.Security
{
    public interface IRule
    {
        string Description { get; }
        string Why { get; }
        bool Evaluate(string project, string id);
    }
}