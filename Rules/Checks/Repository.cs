using SecurePipelineScan.VstsService.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Response = SecurePipelineScan.VstsService.Response;


namespace SecurePipelineScan.Rules.Checks
{
    public static class Repository
    {
        public static bool HasRequiredReviewerPolicy(this Response.Repository repository, IEnumerable<MinimumNumberOfReviewersPolicy> policies)
        {
            if (policies.Any(p => p.Settings.Scope[0].RepositoryId.ToString() == repository.Id &&
                                  p.IsEnabled.HasValue &&
                                  p.IsEnabled.Value == true))
            {
                return true;
            }
            return false;
        }
    }
}