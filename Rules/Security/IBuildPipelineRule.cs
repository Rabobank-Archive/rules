using SecurePipelineScan.VstsService.Response;
using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public interface IBuildPipelineRule : IRule
    {
        Task<bool?> EvaluateAsync(Project project, BuildDefinition buildPipeline);
    }
}