using System.Threading.Tasks;
using SecurePipelineScan.VstsService.Response;

namespace AzureDevOps.Compliance.Rules
{
    public interface IReleasePipelineRule : IRule
    {
        Task<bool?> EvaluateAsync(string projectId, ReleaseDefinition releasePipeline);
    }
}