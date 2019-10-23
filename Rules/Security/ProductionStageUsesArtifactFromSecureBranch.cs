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

        public Task<bool> EvaluateAsync(string projectId, string stageId, ReleaseDefinition releasePipeline)
        {
            if (releasePipeline == null)
                throw new ArgumentNullException(nameof(releasePipeline));

            if (stageId == null)
                throw new ArgumentNullException(nameof(stageId));

            return Task.FromResult(releasePipeline.Environments
                .Where(e => e.Id == stageId)
                .Any(e => e.Conditions
                    .Any(c => c.ConditionType == "artifact" 
                    && JsonConvert.DeserializeObject<ConditionArtifact>(c.Value).SourceBranch == "master")));
            
        }
    }
}