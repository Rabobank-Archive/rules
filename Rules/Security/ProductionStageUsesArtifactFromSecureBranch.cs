using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SecurePipelineScan.VstsService.Response;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Security
{
    public class ProductionStageUsesArtifactFromSecureBranch : IReleasePipelineRule
    {
        public string Description => "Production stage uses artifact from secure branch";
        public string Link => "https://confluence.dev.somecompany.nl/x/YY8AD";
        public bool IsSox => true;

        public Task<bool?> EvaluateAsync(string projectId, string stageId, ReleaseDefinition releasePipeline)
        {
            if (releasePipeline == null)
                throw new ArgumentNullException(nameof(releasePipeline));            

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
                    .Any(n=> n.ConditionType == "artifact" &&
                        n.Name == a &&
                        JsonConvert.DeserializeObject<ConditionArtifact>(n.Value).SourceBranch == "master"));

            return Task.FromResult((bool?)result);
            
        }
    }
}