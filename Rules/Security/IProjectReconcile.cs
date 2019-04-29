namespace SecurePipelineScan.Rules.Security
{
    public interface IProjectReconcile
    {
        void Reconcile(string project);
        string[] Impact { get; }
    }
}