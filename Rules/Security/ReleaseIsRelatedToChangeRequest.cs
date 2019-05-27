namespace SecurePipelineScan.Rules.Security
{
    public class ReleaseIsRelatedToChangeRequest : IRule, IReconcile
    {
        public string Description => "Release is related to a change request in SM9";
        public string Why => "To create traceability for audit, every release should have a tag with the corresponding SM9 change id";
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