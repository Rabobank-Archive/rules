using System;
using System.Collections.Generic;
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

        public ProductionStageUsesArtifactFromSecureBranch(IVstsRestClient client)
        {
            _client = client;
        }

        public string Description => "Production stage uses artifact from secure branch (SOx)";
        public string Link => "https://confluence.dev.somecompany.nl/x/YY8AD";
        public bool IsSox => true;
        public bool RequiresStageId => true;
        public string[] Impact => new[]
        {
            "For each production stage (as stored in ITSM) ...",
            "-> Under 'Pre-deployment conditions/Triggers/After stage' or 'After release'",
            "-> For each artifact of type 'Build' or 'Azure Repos Git'",
            "-> An artifact filter is added that includes the 'master' branch."
        };

        public Task<bool?> EvaluateAsync(string projectId, string stageId, ReleaseDefinition releasePipeline)
        {
            if (releasePipeline == null)
            {
                throw new ArgumentNullException(nameof(releasePipeline));
            }

            if (string.IsNullOrWhiteSpace(stageId))
            {
                return Task.FromResult((bool?)null);
            }

            var releaseArtifactNames = releasePipeline.Artifacts
                .Where(a => a.Type == "Build")
                .Select(a => a.Alias);

            if (!releaseArtifactNames.Any())
                return Task.FromResult((bool?)null);

            var releaseProductionEnvironment = releasePipeline.Environments
                .SingleOrDefault(e => e.Id == stageId);

            if (releaseProductionEnvironment == null)
                return Task.FromResult((bool?)null);

            var result = releaseArtifactNames
                .All(a => releaseProductionEnvironment.Conditions
                    .Any(n => n.ConditionType == "artifact" &&
                        n.Name == a &&
                        JsonConvert.DeserializeObject<ConditionArtifact>(n.Value).SourceBranch == "master"));

            return Task.FromResult((bool?)result);
        }

        public async Task ReconcileAsync(string projectId, string itemId, string scope, string stageId)
        {
            var definition = await _client.GetAsync(
                    ReleaseManagement.Definition(projectId, itemId).AsJson())
                .ConfigureAwait(false);


            var aliases = ReleaseDefinitionHelper.GetArtifactAliases(definition);
            foreach (var alias in aliases)
            {
                ReleaseDefinitionHelper.AddConditionToEnvironments(definition, alias, stageId);
            }

            await _client.PutAsync(new VsrmRequest<object>($"{projectId}/_apis/release/definitions/{itemId}",
                    new Dictionary<string, object> { { "api-version", "5.1" } }), definition)
                .ConfigureAwait(false);
        }
    }
}