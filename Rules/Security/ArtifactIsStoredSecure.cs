using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public class ArtifactIsStoredSecure : IBuildPipelineRule
    {
        private readonly PipelineEvaluatorFactory _pipelineEvaluatorFactory;

        public ArtifactIsStoredSecure(IVstsRestClient client)
        {
            this._pipelineEvaluatorFactory = new PipelineEvaluatorFactory(client);
        }

        private readonly IPipelineHasTaskRule[] _rules =
        {
            new PipelineHasTaskRule("2ff763a7-ce83-4e1f-bc89-0ae63477cebe")
            {
                TaskName = "PublishBuildArtifacts",
            },
            new PipelineHasTaskRule("ecdc45f6-832d-4ad9-b52b-ee49e94659be")
            {
                TaskName = "PublishPipelineArtifact",
            }
        };

        [ExcludeFromCodeCoverage] string IRule.Description => "Artifact is stored in secure artifactory (SOx)";
        [ExcludeFromCodeCoverage] string IRule.Link => "https://confluence.dev.somecompany.nl/x/TI8AD";

        public async Task<bool?> EvaluateAsync(Project project, BuildDefinition buildPipeline)
        {
            foreach (var rule in _rules)
            {
                var result = await _pipelineEvaluatorFactory.Create(buildPipeline)
                    .EvaluateAsync(project, buildPipeline, rule)
                    .ConfigureAwait(false);
                
                if (result == null)
                    return null;

                if (result.Value)
                    return true;
            }
            return false; 
        }
    }
}