namespace SecurePipelineScan.Rules.Security
{
    public class ReleaseHasChangeIdTag : IRule, IReconcile
    {
        public string Description => "Releases have change id tag from SM9";
        public string Why => "To create traceability for audit, every release should have a SM9 tag";
        public bool Evaluate(string project, string id)
        {
            throw new System.NotImplementedException();
        }

        public void Reconcile(string projectId, string id)
        {
            throw new System.NotImplementedException();
        }

        public string[] Impact { get; }
    }
}