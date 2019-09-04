using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Security
{
    public class ArtifactIsStoredSecure : PipelineHasTaskRuleBase, IRule
    {
        protected override string TaskId => "2ff763a7-ce83-4e1f-bc89-0ae63477cebe";

        public ArtifactIsStoredSecure(IVstsRestClient client) : base(client)
        {
            //nothing
        }

        string IRule.Description => "Artifact is stored in secure artifactory";
        string IRule.Why => "To make sure artifacts can't be modified when a build is completed";
        bool IRule.IsSox => true;
    }
}