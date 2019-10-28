using SecurePipelineScan.VstsService.Response;
using System.Collections;
using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public interface IReleasePipelineRule : IRule
    {
        Task<bool?> EvaluateAsync(string projectId, string stageId, ReleaseDefinition releasePipeline);
    }
}