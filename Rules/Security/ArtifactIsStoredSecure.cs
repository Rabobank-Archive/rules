using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using System;
using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public class ArtifactIsStoredSecure : PipelineHasTaskRuleBase, IBuildPipelineRule
    {
        public ArtifactIsStoredSecure(IVstsRestClient client) : base(client)
        {
            //nothing
        }

        protected override string TaskId => "2ff763a7-ce83-4e1f-bc89-0ae63477cebe";
        protected override string TaskName => "PublishBuildArtifacts@1";
        protected override string StepName => "publish";

        string IRule.Description => "Artifact is stored in secure artifactory";
        string IRule.Link => "https://confluence.dev.somecompany.nl/x/TI8AD";
        bool IRule.IsSox => true;
    }
}