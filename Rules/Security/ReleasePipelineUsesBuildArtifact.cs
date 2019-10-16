using System;
using System.Linq;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Security
{
    public class ReleasePipelineUsesBuildArtifact : IReleasePipelineRule
    {
        string IRule.Description => "Release pipeline uses solely build artifacts";
        string IRule.Link => "https://confluence.dev.somecompany.nl/x/aY8AD";
        bool IRule.IsSox => true;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<bool> EvaluateAsync(string projectId, ReleaseDefinition releasePipeline)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (releasePipeline == null)
                throw new ArgumentNullException(nameof(releasePipeline));

            return releasePipeline.Artifacts.Count > 0 && 
                releasePipeline.Artifacts
                    .Select(a => a.Type)
                    .All(t => t == "Build");
        }
    }
}