using System;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Security
{
    public class BuildPipelineHasSonarqubeTask : PipelineHasTaskRuleBase, IBuildPipelineRule
    {
        public BuildPipelineHasSonarqubeTask(IVstsRestClient client) : base(client)
        {
            //nothing
        }
        protected override string TaskId => "15b84ca1-b62f-4a2a-a403-89b77a063157";
        protected override string TaskName => "";

        string IRule.Description => "Build pipeline contains an enabled SonarQube task";
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