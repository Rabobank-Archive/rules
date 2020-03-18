using System.Threading.Tasks;
using SecurePipelineScan.VstsService.Response;

namespace AzureDevOps.Compliancy.Rules
{
    public interface IPipelineEvaluator
    {
        Task<bool?> EvaluateAsync(Project project, BuildDefinition buildPipeline, IPipelineHasTaskRule rule);
    }
}