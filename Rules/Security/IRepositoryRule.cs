using SecurePipelineScan.VstsService.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public interface IRepositoryRule
    {
        string Description { get; }
        string Why { get; }
        bool IsSox { get; }
        Task<bool> EvaluateAsync(string projectId, string repositoryId,
            IEnumerable<MinimumNumberOfReviewersPolicy> policies);
    }
}