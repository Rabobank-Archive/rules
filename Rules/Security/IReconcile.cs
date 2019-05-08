namespace SecurePipelineScan.Rules.Security
{
    public interface IReconcile
    {
        void Reconcile(string projectId, string id);
        
        string[] Impact { get; }
    }
}