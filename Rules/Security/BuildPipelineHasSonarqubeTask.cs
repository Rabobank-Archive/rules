using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Security
{
    public class BuildPipelineHasSonarqubeTask : IPipelineHasTaskRule, IBuildPipelineRule
    {
        private readonly PipelineEvaluatorFactory _pipelineEvaluatorFactory;

        public BuildPipelineHasSonarqubeTask(IVstsRestClient client)
        {
            this._pipelineEvaluatorFactory = new PipelineEvaluatorFactory(client);
        }

        public string TaskId => "15b84ca1-b62f-4a2a-a403-89b77a063157";
        public string TaskName => "SonarQubeAnalyze";
        public string StepName => "";
        public Dictionary<string, string> Inputs => new Dictionary<string, string>();

        [ExcludeFromCodeCoverage] string IRule.Description => "Build pipeline contains an enabled SonarQube task";
        [ExcludeFromCodeCoverage] string IRule.Link => "https://confluence.dev.somecompany.nl/x/RShFD";

        public async Task<bool?> EvaluateAsync(Project project, BuildDefinition buildPipeline)
        {
            return await this._pipelineEvaluatorFactory.Create(buildPipeline).EvaluateAsync(project, buildPipeline, this).ConfigureAwait(false);
        }
    }
}