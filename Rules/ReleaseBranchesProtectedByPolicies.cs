using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using Response = SecurePipelineScan.VstsService.Response;
using Requests = SecurePipelineScan.VstsService.Requests;
using Task = System.Threading.Tasks.Task;

namespace AzureDevOps.Compliance.Rules
{
    public class ReleaseBranchesProtectedByPolicies : IRepositoryRule, IReconcile
    {
        private readonly IVstsRestClient _client;
        private readonly IPoliciesResolver _policiesResolver;

        public ReleaseBranchesProtectedByPolicies(IVstsRestClient client, IPoliciesResolver policiesResolver)
        {
            _policiesResolver = policiesResolver;
            _client = client;
        }

        private const int MinimumApproverCount = 2;

        [ExcludeFromCodeCoverage] public string Description => "Release branches are protected by policies";
        [ExcludeFromCodeCoverage] public string Link => "https://github.com/azure-devops-compliance/rules/wiki/Rules-ReleaseBranchesProtectedByPolicies";

        [ExcludeFromCodeCoverage]
        string[] IReconcile.Impact => new[] {
            "Require a minimum number of reviewers policy is created or updated.",
            "Minimum number of reviewers is set to at least 2",
            "Reset code reviewer votes when there are new changes is enabled.",
            "Policy is blocking the PR."
        };

        public Task<bool?> EvaluateAsync(string projectId, string repositoryId) =>
            HasMasterBranch(projectId, repositoryId)
                ? Task.FromResult(HasRequiredReviewerPolicy(repositoryId, _policiesResolver.Resolve(projectId)))
                : Task.FromResult<bool?>(null);

        public async Task ReconcileAsync(string projectId, string itemId)
        {
            var policies = _policiesResolver.Resolve(projectId);
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

        private bool HasMasterBranch(string projectId, string repositoryId)
        {
            var gitRefs = _client.Get(Requests.Repository.Refs(projectId, repositoryId)).ToList();
            return gitRefs.Any(r => r.Name == "refs/heads/master");
        }
        
        private static bool? HasRequiredReviewerPolicy(string repositoryId, IEnumerable<Response.MinimumNumberOfReviewersPolicy> policies) =>
            Find(policies, repositoryId).Any(p => p.IsEnabled &&
                                                  p.IsBlocking &&
                                                  p.Settings.ResetOnSourcePush &&
                                                  p.Settings.MinimumApproverCount >= MinimumApproverCount);

        private static IEnumerable<Response.MinimumNumberOfReviewersPolicy> Find(IEnumerable<Response.MinimumNumberOfReviewersPolicy> policies, string repositoryId) =>
            policies.Where(p => p.Settings.Scope.Any(scope => scope.RepositoryId?.ToString() == repositoryId &&
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