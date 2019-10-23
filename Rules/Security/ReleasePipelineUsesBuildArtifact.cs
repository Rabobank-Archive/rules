using System;
using System.Linq;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService.Response;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Security
{
    public class ReleasePipelineUsesBuildArtifact : IReleasePipelineRule
    {
        string IRule.Description => "Release pipeline uses solely build artifacts";
        string IRule.Link => "https://confluence.dev.somecompany.nl/x/aY8AD";
        bool IRule.IsSox => true;

        public Task<bool> EvaluateAsync(string projectId, string stageId, ReleaseDefinition releasePipeline)
        {
            if (releasePipeline == null)
            {
                throw new ArgumentNullException(nameof(releasePipeline));
            }

            return Task.FromResult(releasePipeline.Artifacts.Count > 0 &&
                                   releasePipeline.Artifacts
                                       .Select(a => a.Type)
                                       .All(t => t == "Build"));
        }
    }
}