using System;
using System.Collections.Generic;
using System.Linq;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using Requests = SecurePipelineScan.VstsService.Requests;

namespace SecurePipelineScan.Rules.Security
{
    public class PipelineHasRequiredRetentionPolicy : IRule, IReconcile
    {
        private readonly IVstsRestClient _client;

        public PipelineHasRequiredRetentionPolicy(IVstsRestClient client)
        {
            _client = client;
        }

        //TODO: Once we can get information from SM9 regarding the production pipeline/stage,
        //      we only have to check/reconcile the retention of this stage.
        string IRule.Description => "Production releases are retained for at least 15 months";

        string IRule.Why =>
            "To make sure production releases are auditable for at least 15 months";

        string[] IReconcile.Impact => new[] {
            "In project settings the maximum retention policy is set to 450 days.",
            "On the pipeline the days to retain a release is set to 450 days for every stage.",
            "On the pipeline the checkbox to retain associated artifacts is enabled for every stage."
        };

        public bool Evaluate(string project, string pipelineId)
        {
            var releasePipeline = _client.Get(Requests.ReleaseManagement.Definition(project, pipelineId));
            return HasRequiredRetentionPolicy(releasePipeline);
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
                 
                _client.Post(Requests.Policies.Policy(projectId), policy);
            }
        }

        private static bool HasRequiredRetentionPolicy(ReleaseDefinition releasePipeline) =>
            releasePipeline
                .Environments
                .Select(r => r.RetentionPolicy)
                .Any(d => d.DaysToKeep >= 450 && d.RetainBuild);

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