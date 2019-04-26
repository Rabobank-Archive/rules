namespace SecurePipelineScan.Rules.Security
{
    public interface IRepositoryRule
    {
        string Description { get; }
        bool Evaluate(string project, string repositoryId);
    }
}