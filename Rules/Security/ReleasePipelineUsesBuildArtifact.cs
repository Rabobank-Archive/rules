using System;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Security
{
    public class ReleasePipelineUsesBuildArtifact : IReleasePipelineRule
    {
        string IRule.Description => "Release pipeline uses solely build artifacts";
        string IRule.Link => "https://confluence.dev.somecompany.nl/x/aY8AD";
        bool IRule.IsSox => true;

        public Task<bool?> EvaluateAsync(string projectId, string stageId,
            ReleaseDefinition releasePipeline)
        {
            if (releasePipeline == null)
                throw new ArgumentNullException(nameof(releasePipeline));

            bool? result = releasePipeline.Artifacts.Any() && 
                releasePipeline.Artifacts.All(a => a.Type == "Build");

            return Task.FromResult(result);
        }
    }
}