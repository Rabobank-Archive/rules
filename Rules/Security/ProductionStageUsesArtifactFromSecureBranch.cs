using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Security
{
    public class ProductionStageUsesArtifactFromSecureBranch : IReleasePipelineRule, IReconcile
    {
        private readonly IVstsRestClient _client;
        private readonly IProductionItemsResolver _productionItemsResolver;

        public ProductionStageUsesArtifactFromSecureBranch(IVstsRestClient client, IProductionItemsResolver productionItemsResolver)
        {
            _client = client;
            _productionItemsResolver = productionItemsResolver;
        }

        [ExcludeFromCodeCoverage] public string Description => "Production stage uses artifact from secure branch (SOx)";
        [ExcludeFromCodeCoverage] public string Link => "https://confluence.dev.somecompany.nl/x/YY8AD";

        [ExcludeFromCodeCoverage]
        public string[] Impact => new[]
        {
            "For each production stage (as stored in ITSM) ...",
            "-> Under 'Pre-deployment conditions/Triggers/After stage' or 'After release'",
            "-> For each artifact of type 'Build' or 'Azure Repos Git'",
            "-> An artifact filter is added that includes the 'master' branch."
        };

        public async Task<bool?> EvaluateAsync(string projectId, ReleaseDefinition releasePipeline)
        {
            if (releasePipeline == null)
            {
                throw new ArgumentNullException(nameof(releasePipeline));
            }

            var stages = await _productionItemsResolver.ResolveAsync(projectId, releasePipeline.Id);

            var results = stages.Select(stageId =>
            {
                var releaseArtifactNames = releasePipeline.Artifacts
                                                .Where(a => a.Type == "Build")
                                                .Select(a => a.Alias);

                if (!releaseArtifactNames.Any())
                    return (bool?)null;

                var releaseProductionEnvironment = releasePipeline.Environments
                    .SingleOrDefault(e => e.Id == stageId);

                if (releaseProductionEnvironment == null)
                    return (bool?)null;

                var result = releaseArtifactNames
                    .All(a => releaseProductionEnvironment.Conditions
                        .Any(n => n.ConditionType == "artifact" &&
                            n.Name == a &&
                            JsonConvert.DeserializeObject<ConditionArtifact>(n.Value).SourceBranch == "master"));

                return result;
            });

            return results.Any(x => x == false) ? false : results.All(x => x == null) ? (bool?)null : true;
        }

        public async Task ReconcileAsync(string projectId, string itemId)
        {
            var stages = await _productionItemsResolver.ResolveAsync(projectId, itemId);
            var definition = await _client.GetAsync(
                    ReleaseManagement.Definition(projectId, itemId).AsJson())
                .ConfigureAwait(false);

            var aliases = ReleaseDefinitionHelper.GetArtifactAliases(definition);

            foreach (var stage in stages)
                foreach (var alias in aliases)
                    ReleaseDefinitionHelper.AddConditionToEnvironments(definition, alias, stage);

            await _client.PutAsync(new VsrmRequest<object>($"{projectId}/_apis/release/definitions/{itemId}",
                    new Dictionary<string, object> { { "api-version", "5.1" } }), definition)
                .ConfigureAwait(false);
        }
    }
}