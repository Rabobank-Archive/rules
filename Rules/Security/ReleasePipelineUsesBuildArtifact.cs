using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Security
{
    public class ReleasePipelineUsesBuildArtifact : IReleasePipelineRule
    {
        public string Description => "The release pipeline should solely use build artifacts";

        public string Why => "To make sure artifacts can't be modified when a build is completed";

        public bool IsSox => true;

        public async Task<bool> EvaluateAsync(string projectId, ReleaseDefinition releasePipeline)
        {
            if (releasePipeline == null)
                throw new ArgumentNullException(nameof(releasePipeline));

            return releasePipeline.Artifacts
                .Select(a => a.Type)
                .All(t => t == "Build");
        }
    }
}