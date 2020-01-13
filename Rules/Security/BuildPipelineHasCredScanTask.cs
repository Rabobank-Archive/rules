using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Security
{
    public class BuildPipelineHasCredScanTask : IPipelineHasTaskRule, IBuildPipelineRule
    {
        private readonly PipelineEvaluatorFactory _pipelineEvaluatorFactory;

        public BuildPipelineHasCredScanTask(IVstsRestClient client)
        {
            _pipelineEvaluatorFactory = new PipelineEvaluatorFactory(client);
        }

        public string TaskId => "f0462eae-4df1-45e9-a754-8184da95ed01";
        public string TaskName => "CredScan";
        public string StepName => "";

        string IRule.Description => "Build pipeline contains credential scan task";
        string IRule.Link => "https://confluence.dev.somecompany.nl/x/LorHDQ";
        bool IRule.IsSox => false;

        public async Task<bool?> EvaluateAsync(Project project, BuildDefinition buildPipeline)
        {
            return await _pipelineEvaluatorFactory.Create(buildPipeline).EvaluateAsync(project, buildPipeline, this)
                .ConfigureAwait(false);
        }
    }
}