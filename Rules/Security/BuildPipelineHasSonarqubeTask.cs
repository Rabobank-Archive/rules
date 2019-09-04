using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Security
{
    public class BuildPipelineHasSonarqubeTask : PipelineHasTaskRuleBase, IRule
    {
        protected override string TaskId => "6d01813a-9589-4b15-8491-8164aeb38055";

        public BuildPipelineHasSonarqubeTask(IVstsRestClient client) : base(client)
        {
            //nothing
        }

        string IRule.Description => "Build pipeline contains an enabled SonarQube task";
        string IRule.Why => "To make sure a static code analysis is executed for each build";
        bool IRule.IsSox => false;
    }
}