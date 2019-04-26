using System.Collections.Generic;
using System.Linq;
using SecurePipelineScan.Rules.Checks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using Requests = SecurePipelineScan.VstsService.Requests;

namespace SecurePipelineScan.Rules.Security
{
    public class ReleaseBranchesProtectedByPolicies : IRepositoryRule
    {
        
        private readonly IVstsRestClient _client;
        
        public ReleaseBranchesProtectedByPolicies(IVstsRestClient client)
        {
            _client = client;
        }

        public string Description => "Release branches are protected by policies";
        
        public bool Evaluate(string project, string repositoryId)
        {
            var policies = _client.Get(Requests.Policies.MinimumNumberOfReviewersPolicies(project));
            return HasRequiredReviewerPolicy(repositoryId, policies);
        }

        private static bool HasRequiredReviewerPolicy(string repositoryId, IEnumerable<MinimumNumberOfReviewersPolicy> policies)
        {
            return policies.Any(p => p.Settings.Scope.Any(scope => scope.RepositoryId.ToString() == repositoryId && scope.RefName == "refs/heads/master") &&
                                     p.IsEnabled == true &&
                                     p.Settings.CreatorVoteCounts == true &&
                                     p.Settings.ResetOnSourcePush == true &&
                                     p.Settings.MinimumApproverCount >= 2);
        }
    }
}