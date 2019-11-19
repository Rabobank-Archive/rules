using SecurePipelineScan.VstsService.Response;
using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public interface IPipelineEvaluator
    {
        Task<bool?> EvaluateAsync(Project project, BuildDefinition buildPipeline, IPipelineHasTaskRule rule);
    }
}