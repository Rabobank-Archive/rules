using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Security
{
    public class BuildPipelineHasFortifyTask : PipelineHasTaskRuleBase, IRule
    {
        protected override string TaskId => "818386e5-c8a5-46c3-822d-954b3c8fb130";

        public BuildPipelineHasFortifyTask(IVstsRestClient client) : base(client)
        {
            //nothing
        }

        string IRule.Description => "Build pipeline contains an enabled Fortify task";
        string IRule.Why => "To make sure a static code analysis is executed for each build";
        bool IRule.IsSox => false;
    }
}