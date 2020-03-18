using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using Requests = SecurePipelineScan.VstsService.Requests;
using Task = System.Threading.Tasks.Task;

namespace AzureDevOps.Compliance.Rules
{
    public class PipelineHasRequiredRetentionPolicy : IReleasePipelineRule, IReconcile
    {
        private readonly IVstsRestClient _client;
        private readonly int RequiredRetentionDays = 450;

        public PipelineHasRequiredRetentionPolicy(IVstsRestClient client) => _client = client;

        [ExcludeFromCodeCoverage] string IRule.Description => "All releases are retained for at least 15 months";
        [ExcludeFromCodeCoverage] string IRule.Link => "https://github.com/azure-devops-compliance/rules/wiki/Rules-PipelineHasRequiredRetentionPolicy";

        [ExcludeFromCodeCoverage]
        string[] IReconcile.Impact => new[] {
            "In project settings the maximum retention policy is set to 450 days.",
            "On the pipeline the days to retain a release is set to 450 days for every stage.",
            "On the pipeline the checkbox to retain associated artifacts is enabled for every stage."
        };

        public Task<bool?> EvaluateAsync(string projectId,
            ReleaseDefinition releasePipeline)
        {
            if (releasePipeline == null)
                throw new ArgumentNullException(nameof(releasePipeline));

            bool? result = releasePipeline
                .Environments
                .Select(e => e.RetentionPolicy)
                .All(r => r.DaysToKeep >= RequiredRetentionDays && r.RetainBuild);

            return Task.FromResult(result);
        }

        public async Task ReconcileAsync(string projectId, string itemId)
        {
            var releaseSettings = await _client.GetAsync(Requests.ReleaseManagement.Settings(projectId))
                .ConfigureAwait(false);
            if (!HasRequiredReleaseSettings(releaseSettings))
            {
                await _client.PutAsync(Requests.ReleaseManagement.Settings(projectId),
                    UpdateReleaseSettings(releaseSettings))
                    .ConfigureAwait(false);
            }

            var releasePipeline = await _client.GetAsync(new VsrmRequest<object>($"{projectId}/_apis/release/definitions/{itemId}")
                .AsJson())
                .ConfigureAwait(false);
            await _client.PutAsync(new VsrmRequest<object>($"{projectId}/_apis/release/definitions/{itemId}", new Dictionary<string, object> { { "api-version", "5.0" } }),
                UpdateReleaseDefinition(releasePipeline))
                .ConfigureAwait(false);
        }

        private bool HasRequiredReleaseSettings(ReleaseSettings settings) =>
            settings.RetentionSettings.MaximumEnvironmentRetentionPolicy.DaysToKeep >= RequiredRetentionDays;

        private ReleaseSettings UpdateReleaseSettings(ReleaseSettings settings)
        {
            if (settings.RetentionSettings.MaximumEnvironmentRetentionPolicy.DaysToKeep < RequiredRetentionDays)
                settings.RetentionSettings.MaximumEnvironmentRetentionPolicy.DaysToKeep = RequiredRetentionDays;
            settings.RetentionSettings.MaximumEnvironmentRetentionPolicy.RetainBuild = true;
            return settings;
        }

        private JToken UpdateReleaseDefinition(JToken pipeline)
        {
            pipeline
                .SelectTokens("environments[*].retentionPolicy.daysToKeep")
                .ToList()
                .ForEach(t => t.Replace(RequiredRetentionDays));
            pipeline
                .SelectTokens("environments[*].retentionPolicy.retainBuild")
                .ToList()
                .ForEach(t => t.Replace(true));

            return pipeline;
        }
    }
}