using SecurePipelineScan.VstsService.Response;
using System.Collections.Generic;
using System.Linq;

using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Checks
{
    public static class Repository
    {
        /// <summary>
        /// Is True when: At least 1 policy is for this repository with;
        ///
        /// - Minimum number of reviewers is 2 or more
        /// - Allow users to approve their own changes is true
        /// - Reset code reviewer votes when there are new changes is true
        /// - Policy is enabled
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="policies"></param>
        /// <returns></returns>
        public static bool HasRequiredReviewerPolicy(this Response.Repository repository, IEnumerable<MinimumNumberOfReviewersPolicy> policies)
        {
            return policies.Any(p => p.Settings.Scope?[0].RepositoryId.ToString() == repository.Id &&
                                     p.IsEnabled == true &&
                                     p.Settings.CreatorVoteCounts == true &&
                                     p.Settings.ResetOnSourcePush == true &&
                                     p.Settings.MinimumApproverCount > 1);
        }
    }
}