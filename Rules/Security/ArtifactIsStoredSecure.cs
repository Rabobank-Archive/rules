using SecurePipelineScan.VstsService.Response;
using System;
using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public class ArtifactIsStoredSecure : PipelineHasTaskRuleBase, IBuildPipelineRule
    {
        protected override string TaskId => "2ff763a7-ce83-4e1f-bc89-0ae63477cebe";

        string IRule.Description => "Artifact is stored in secure artifactory";
        string IRule.Link => "https://confluence.dev.somecompany.nl/x/TI8AD";
        bool IRule.IsSox => true;

        public Task<bool> EvaluateAsync(string projectId, BuildDefinition buildPipeline)
        {
            if (buildPipeline == null)
                throw new ArgumentNullException(nameof(buildPipeline));

            return base.EvaluateAsync(buildPipeline);
        }
    }
}