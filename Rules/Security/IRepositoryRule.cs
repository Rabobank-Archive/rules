namespace SecurePipelineScan.Rules.Security
{
    public interface IRepositoryRule
    {
        string Description { get; }
        string Why { get; }
        bool Evaluate(string project, string repositoryId);
    }
}