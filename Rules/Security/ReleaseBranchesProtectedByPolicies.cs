using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using Response = SecurePipelineScan.VstsService.Response;
using Requests = SecurePipelineScan.VstsService.Requests;

namespace SecurePipelineScan.Rules.Security
{
    public class ReleaseBranchesProtectedByPolicies : IRepositoryRule, IReconcile
    {
        private readonly IVstsRestClient _client;

        public ReleaseBranchesProtectedByPolicies(IVstsRestClient client)
        {
            _client = client;
        }

        private const int MinimumApproverCount = 2;

        public string Description => "Release branches are protected by policies (SOx)";
        public string Link => "https://confluence.dev.somecompany.nl/x/Po8AD";
        public bool IsSox => true;
        public bool RequiresStageId => false;
        string[] IReconcile.Impact => new[] {
            "Require a minimum number of reviewers policy is created or updated.",
            "Minimum number of reviewers is set to at least 2",
            "Reset code reviewer votes when there are new changes is enabled.",
            "Policy is blocking the PR."
        };

        public Task<bool> EvaluateAsync(
            string projectId, string repositoryId, IEnumerable<Response.MinimumNumberOfReviewersPolicy> policies)
        {
            return Task.FromResult(HasRequiredReviewerPolicy(repositoryId, policies));
        }

        public async Task ReconcileAsync(string projectId, string itemId, string stageId, string userId, object data = null)
        {
            var policies = _client.Get(Requests.Policies.MinimumNumberOfReviewersPolicies(projectId));
            var policy = Find(policies, itemId).SingleOrDefault();

            if (policy != null)
            {
                await _client.PutAsync(Requests.Policies.Policy(projectId, policy.Id), UpdatePolicy(policy))
                    .ConfigureAwait(false);
            }
            else
            {
                await _client.PostAsync(Requests.Policies.Policy(projectId), InitializeMinimumNumberOfReviewersPolicy(itemId))
                    .ConfigureAwait(false);
            }
        }

        private static bool HasRequiredReviewerPolicy(string repositoryId, IEnumerable<Response.MinimumNumberOfReviewersPolicy> policies) =>
            Find(policies, repositoryId).Any(p => p.IsEnabled &&
                                                  p.IsBlocking &&
                                                  p.Settings.ResetOnSourcePush &&
                                                  p.Settings.MinimumApproverCount >= MinimumApproverCount);

        private static IEnumerable<Response.MinimumNumberOfReviewersPolicy> Find(IEnumerable<Response.MinimumNumberOfReviewersPolicy> policies, string repositoryId) =>
            policies.Where(p => p.Settings.Scope.Any(scope => scope.RepositoryId.ToString() == repositoryId &&
                                                              scope.RefName == "refs/heads/master"));

        private static Response.MinimumNumberOfReviewersPolicy UpdatePolicy(Response.MinimumNumberOfReviewersPolicy policy)
        {
            UpdateSettings(policy.Settings);
            policy.IsEnabled = true;
            policy.IsBlocking = true;

            return policy;
        }

        private static void UpdateSettings(Response.MinimumNumberOfReviewersPolicySettings settings)
        {
            if (settings.MinimumApproverCount < MinimumApproverCount)
            {
                settings.MinimumApproverCount = MinimumApproverCount;
                settings.CreatorVoteCounts = true;
            }

            settings.ResetOnSourcePush = true;
        }

        private static Response.MinimumNumberOfReviewersPolicy InitializeMinimumNumberOfReviewersPolicy(string repositoryId)
        {
            return UpdatePolicy(new Response.MinimumNumberOfReviewersPolicy
            {
                Settings = new Response.MinimumNumberOfReviewersPolicySettings
                {
                    Scope = new[]
                    {
                        new Response.Scope
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