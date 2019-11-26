using System;
using System.Linq;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Security
{
    public class ReleasePipelineHasDeploymentMethod : IReleasePipelineRule
    {
        public string Description => "Release pipeline has deployment method";
        public string Link => "https://confluence.dev.somecompany.nl/x/????"; // TODO 
        public bool IsSox => false;
        public bool RequiresStageId => true;
        public string[] Impact => new string[0];

        public Task<bool?> EvaluateAsync(string projectId, string stageId, ReleaseDefinition releasePipeline)
        {
            if (releasePipeline == null)
                throw new ArgumentNullException(nameof(releasePipeline));

            var stageExists = stageId == null 
                ? (bool?) null 
                : releasePipeline.Environments.Any(e => e.Id == stageId); 

            return Task.FromResult(stageExists);
        }
    }
}