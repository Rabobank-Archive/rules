using System.Collections.Generic;
using SecurePipelineScan.VstsService.Response;

namespace AzureDevOps.Compliance.Rules
{
    public interface IPoliciesResolver
    {
        IEnumerable<MinimumNumberOfReviewersPolicy> Resolve(string projectId);

        void Clear(string projectId);
    }
}