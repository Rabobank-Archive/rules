using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public class BuildPipelineHasNexusIqTask : IPipelineHasTaskRule, IBuildPipelineRule
    {
        private readonly PipelineEvaluatorFactory _pipelineEvaluatorFactory;

        public BuildPipelineHasNexusIqTask(IVstsRestClient client)
        {
            this._pipelineEvaluatorFactory = new PipelineEvaluatorFactory(client);
        }
        
        public string TaskId => "4f40d1a2-83b0-4ddc-9a77-e7f279eb1802";
        public string TaskName => "SonatypeIntegrations.nexus-iq-azure-extension.nexus-iq-azure-pipeline-task.NexusIqPipelineTask";
        public string StepName => "";
        string IRule.Description => "Build pipeline contains an enabled Nexus IQ task";
        string IRule.Link => "https://confluence.dev.somecompany.nl/x/JSNFD";
        bool IRule.IsSox => false;

        public async Task<bool?> EvaluateAsync(Project project, BuildDefinition buildPipeline)
        {
            return await this._pipelineEvaluatorFactory
                .Create(buildPipeline)
                .EvaluateAsync(project, buildPipeline, this)
                .ConfigureAwait(false);
        }
    }
}