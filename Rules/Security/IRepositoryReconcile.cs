namespace SecurePipelineScan.Rules.Security
{
    public interface IRepositoryReconcile
    {
        void Reconcile(string projectId, string repositoryId);
        
        string[] Impact { get; }
    }
}