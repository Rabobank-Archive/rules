using System;
using System.Collections.Generic;
using System.Linq;
using SecurePipelineScan.Rules.Checks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using Requests = SecurePipelineScan.VstsService.Requests;

namespace SecurePipelineScan.Rules.Security
{
    public class ReleaseBranchesProtectedByPolicies : IRule, IReconcile
    {
        private readonly IVstsRestClient _client;

        public ReleaseBranchesProtectedByPolicies(IVstsRestClient client)
        {
            _client = client;
        }

        string IRule.Description => "Release branches are protected by policies";

        string IRule.Why =>
            "To prevent from hijacking a PR, the minimum number of reviewers must be (at least) 2 and reset code reviewer votes for new changes must be enabled. Self approving changes is then allowed.";

        string[] IReconcile.Impact => new[] {
            "Require a minimum number of reviewers policy is created or updated.",
            "Minimum number of reviewers is set to at least 2",
            "Reset code reviewer votes when there are new changes is enabled."
        };

        public bool Evaluate(string project, string repositoryId)
        {
            var policies = _client.Get(Requests.Policies.MinimumNumberOfReviewersPolicies(project));
            return HasRequiredReviewerPolicy(repositoryId, policies);
        }

        public void Reconcile(string projectId, string id)
        {
            var policies = _client.Get(Requests.Policies.MinimumNumberOfReviewersPolicies(projectId));
            var policy = Find(policies, id).SingleOrDefault(x => x.Settings.Scope.Any(s => s.RepositoryId == new Guid(id)));

            if (policy != null)
            {
                UpdateSettings(policy.Settings);
                _client.Put(Requests.Policies.Policy(projectId, policy.Id), policy);
            }
            else
            {
                 policy = InitializeMinimumNumberOfReviewersPolicy(id);
                 UpdateSettings(policy.Settings);
                 
                _client.Post(new VstsPostRequest<MinimumNumberOfReviewersPolicy>(Requests.Policies.All(projectId).Uri, policy));
            }
        }

        private static bool HasRequiredReviewerPolicy(string repositoryId, IEnumerable<MinimumNumberOfReviewersPolicy> policies) =>
            Find(policies, repositoryId).Any(p => p.IsEnabled &&
                                                  p.Settings.ResetOnSourcePush &&
                                                  p.Settings.MinimumApproverCount >= 2);

        private static IEnumerable<MinimumNumberOfReviewersPolicy> Find(IEnumerable<MinimumNumberOfReviewersPolicy> policies, string repositoryId) =>
            policies.Where(p => p.Settings.Scope.Any(scope => scope.RepositoryId.ToString() == repositoryId &&
                                                              scope.RefName == "refs/heads/master"));

        private static void UpdateSettings(MinimumNumberOfReviewersPolicySettings settings)
        {
            if (settings.MinimumApproverCount < 2)
            {
                settings.MinimumApproverCount = 2;
                settings.CreatorVoteCounts = true;
            }

            settings.ResetOnSourcePush = true;
        }

        private static MinimumNumberOfReviewersPolicy InitializeMinimumNumberOfReviewersPolicy(string repositoryId)
        {
            return new MinimumNumberOfReviewersPolicy
            {
                IsEnabled =  true,
                Settings =  new MinimumNumberOfReviewersPolicySettings
                {
                    Scope = new[]
                    {
                        new Scope
                        {
                            RefName = "refs/heads/master",
                            MatchKind = "exact",
                            RepositoryId = new Guid(repositoryId)
                        }
                    }
                            
                }
            };
        }
}
}