using SecurePipelineScan.VstsService.Response;
using System;
using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public class BuildPipelineHasFortifyTask : PipelineHasTaskRuleBase, IBuildPipelineRule
    {
        protected override string TaskId => "818386e5-c8a5-46c3-822d-954b3c8fb130";

        string IRule.Description => "Build pipeline contains an enabled Fortify task";
        string IRule.Link => null;
        bool IRule.IsSox => false;

        public Task<bool?> EvaluateAsync(string projectId, BuildDefinition buildPipeline)
        {
            if (buildPipeline == null)
                throw new ArgumentNullException(nameof(buildPipeline));

            return base.EvaluateAsync(buildPipeline);
        }
    }
}