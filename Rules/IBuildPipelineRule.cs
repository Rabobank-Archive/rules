using System.Threading.Tasks;
using SecurePipelineScan.VstsService.Response;

namespace AzureDevOps.Compliance.Rules
{
    public interface IBuildPipelineRule : IRule
    {
        Task<bool?> EvaluateAsync(Project project, BuildDefinition buildPipeline);
    }
}