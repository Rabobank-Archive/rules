using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Security
{
    public partial class BuildPipelineHasCredScanTask : IBuildPipelineRule
    {
        private readonly PipelineEvaluatorFactory _pipelineEvaluatorFactory;

        public BuildPipelineHasCredScanTask(IVstsRestClient client)
        {
            _pipelineEvaluatorFactory = new PipelineEvaluatorFactory(client);
        }

        private readonly IPipelineHasTaskRule[] _rules =
        {
            new PipelineHasTaskRule("f0462eae-4df1-45e9-a754-8184da95ed01")
            {
                TaskName = "CredScan",
                StepName = ""
            },
            new PipelineHasTaskRule("dbe519ee-a2e4-43f5-8e1a-949bd935b736")
            {
                TaskName = "PostAnalysis",
                StepName = "",
                Inputs = new Dictionary<string, string>{{"CredScan", "true"}}
            }
        };

        [ExcludeFromCodeCoverage] string IRule.Description => "Build pipeline contains credential scan task";
        [ExcludeFromCodeCoverage] string IRule.Link => "https://confluence.dev.somecompany.nl/x/LorHDQ";

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
    }
}