using System.Collections.Generic;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Security
{
    public interface IPoliciesResolver
    {
        IEnumerable<MinimumNumberOfReviewersPolicy> Resolve(string projectId);
    }
}