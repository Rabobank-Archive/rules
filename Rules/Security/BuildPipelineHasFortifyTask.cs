using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using System;
using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public class BuildPipelineHasFortifyTask : PipelineHasTaskRuleBase, IBuildPipelineRule
    {
        public BuildPipelineHasFortifyTask(IVstsRestClient client) : base(client)
        {
            //nothing
        }

        protected override string TaskId => "818386e5-c8a5-46c3-822d-954b3c8fb130";
        protected override string TaskName => "FortifySCA@5";
        protected override string StepName => "";

        string IRule.Description => "Build pipeline contains an enabled Fortify task";
        string IRule.Link => null;
        bool IRule.IsSox => false;
    }
}