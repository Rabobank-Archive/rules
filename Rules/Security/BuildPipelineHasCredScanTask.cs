using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Security
{
    public class BuildPipelineHasCredScanTask : IBuildPipelineRule
    {
        private readonly PipelineEvaluatorFactory _pipelineEvaluatorFactory;

        public BuildPipelineHasCredScanTask(IVstsRestClient client)
        {
            _pipelineEvaluatorFactory = new PipelineEvaluatorFactory(client);
        }

        private readonly IPipelineHasTaskRule[] _rules =
        {
            new PipelineHasTaskRule
            {
                TaskId = "f0462eae-4df1-45e9-a754-8184da95ed01",
                TaskName = "CredScan",
                StepName = ""
            },
            new PipelineHasTaskRule
            {
                TaskId = "dbe519ee-a2e4-43f5-8e1a-949bd935b736",
                TaskName = "PostAnalysis",
                StepName = ""
            }
        };

        string IRule.Description => "Build pipeline contains credential scan task";
        string IRule.Link => "https://confluence.dev.somecompany.nl/x/LorHDQ";

        public async Task<bool?> EvaluateAsync(Project project, BuildDefinition buildPipeline)
        {
            foreach (var rule in _rules)
            {
                var result = await _pipelineEvaluatorFactory.Create(buildPipeline)
                    .EvaluateAsync(project, buildPipeline, rule)
                    .ConfigureAwait(false);

                if (result == null)
                {
                    return null;
                }

                if (!result.Value)
                {
                    return false;
                }
            }

            return true;
        }

        public class PipelineHasTaskRule : IPipelineHasTaskRule
        {
            public string TaskId { get; set; }
            public string TaskName { get; set; }
            public string StepName { get; set; }
        }
    }
}