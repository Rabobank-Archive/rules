using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public class BuildPipelineHasFortifyTask : IPipelineHasTaskRule, IBuildPipelineRule
    {
        private readonly PipelineEvaluatorFactory _pipelineEvaluatorFactory;

        public BuildPipelineHasFortifyTask(IVstsRestClient client)
        {
            this._pipelineEvaluatorFactory = new PipelineEvaluatorFactory(client);
        }


        public string TaskId => "818386e5-c8a5-46c3-822d-954b3c8fb130";
        public string TaskName => "FortifySCA";
        public string StepName => "";
        public Dictionary<string, string> Inputs => new Dictionary<string, string>();

        [ExcludeFromCodeCoverage] string IRule.Description => "Build pipeline contains an enabled Fortify task";
        [ExcludeFromCodeCoverage] string IRule.Link => "https://confluence.dev.somecompany.nl/x/9w1TD";

        public async Task<bool?> EvaluateAsync(Project project, BuildDefinition buildPipeline)
        {
            return await this._pipelineEvaluatorFactory.Create(buildPipeline).EvaluateAsync(project, buildPipeline, this).ConfigureAwait(false);
        }
    }
}