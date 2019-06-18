using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using Requests = SecurePipelineScan.VstsService.Requests;
using Task = System.Threading.Tasks.Task;

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
            "Reset code reviewer votes when there are new changes is enabled.",
            "Policy is blocking the PR."
        };

        public Task<bool> Evaluate(string project, string repositoryId)
        {
            var policies = _client.Get(Requests.Policies.MinimumNumberOfReviewersPolicies(project));
            return Task.FromResult(HasRequiredReviewerPolicy(repositoryId, policies));
        }

        public async Task Reconcile(string projectId, string id)
        {
            var policies = _client.Get(Requests.Policies.MinimumNumberOfReviewersPolicies(projectId));
            var policy = Find(policies, id).SingleOrDefault();

            if (policy != null)
            {
                await _client.PutAsync(Requests.Policies.Policy(projectId, policy.Id), UpdatePolicy(policy));
            }
            else
            {
                await _client.PostAsync(Requests.Policies.Policy(projectId), InitializeMinimumNumberOfReviewersPolicy(id));
            }
        }

        private static bool HasRequiredReviewerPolicy(string repositoryId, IEnumerable<MinimumNumberOfReviewersPolicy> policies) =>
            Find(policies, repositoryId).Any(p => p.IsEnabled &&
                                                  p.IsBlocking &&
                                                  p.Settings.ResetOnSourcePush &&
                                                  p.Settings.MinimumApproverCount >= 2);

        private static IEnumerable<MinimumNumberOfReviewersPolicy> Find(IEnumerable<MinimumNumberOfReviewersPolicy> policies, string repositoryId) =>
            policies.Where(p => p.Settings.Scope.Any(scope => scope.RepositoryId.ToString() == repositoryId &&
                                                              scope.RefName == "refs/heads/master"));

        private static MinimumNumberOfReviewersPolicy UpdatePolicy(MinimumNumberOfReviewersPolicy policy)
        {
            UpdateSettings(policy.Settings);
            policy.IsEnabled = true;
            policy.IsBlocking = true;

            return policy;
        }

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
            return UpdatePolicy(new MinimumNumberOfReviewersPolicy
            {
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
            });
        }
    }
}