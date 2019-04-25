using System.Collections.Generic;
using System.Linq;
using SecurePipelineScan.Rules.Checks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using Requests = SecurePipelineScan.VstsService.Requests;

namespace SecurePipelineScan.Rules.Security
{
    public class MasterAndReleaseBranchesProtected : IRepositoryRule
    {
        
        private readonly IVstsRestClient _client;
        
        public MasterAndReleaseBranchesProtected(IVstsRestClient client)
        {
            _client = client;
        }

        public string Description => "Master Branch is protected with branch policies";
        
        public bool Evaluate(string project, string repository)
        {
            var repo = _client.Get(Requests.Repository.Repositories(project)).Single(r => r.Name == repository);
            

            var policies = _client.Get(Requests.Policies.MinimumNumberOfReviewersPolicies(project));
            return HasRequiredReviewerPolicy(repo, policies);
        }
        
        public static bool HasRequiredReviewerPolicy(VstsService.Response.Repository repository, IEnumerable<MinimumNumberOfReviewersPolicy> policies)
        {
            return policies.Any(p => p.Settings.Scope.Any(scope => scope.RepositoryId.ToString() == repository.Id && scope.RefName == "refs/heads/master") &&
                                     p.IsEnabled == true &&
                                     p.Settings.CreatorVoteCounts == true &&
                                     p.Settings.ResetOnSourcePush == true &&
                                     p.Settings.MinimumApproverCount > 1);
        }
    }
}