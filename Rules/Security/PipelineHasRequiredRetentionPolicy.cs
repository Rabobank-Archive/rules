using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using Requests = SecurePipelineScan.VstsService.Requests;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Security
{
    public class PipelineHasRequiredRetentionPolicy : IRule, IReconcile
    {
        private readonly IVstsRestClient _client;
        private readonly int RequiredRetentionDays = 450;

        public PipelineHasRequiredRetentionPolicy(IVstsRestClient client)
        {
            _client = client;
        }

        string IRule.Description => "Production releases are retained for at least 15 months";

        string IRule.Why =>
            "To make sure production releases are auditable for at least 15 months";
        bool IRule.IsSox => true;
        string[] IReconcile.Impact => new[] {
            "In project settings the maximum retention policy is set to 450 days.",
            "On the pipeline the days to retain a release is set to 450 days for every stage.",
            "On the pipeline the checkbox to retain associated artifacts is enabled for every stage."
        };

        public async Task<bool> EvaluateAsync(string project, string releasePipelineId) //NOSONAR
        {
            var releasePipeline = await _client.GetAsync(Requests.ReleaseManagement.Definition(project, releasePipelineId))
                .ConfigureAwait(false);
            return HasRequiredRetentionPolicy(releasePipeline);
        }

        public async Task ReconcileAsync(string projectId, string releasePipelineId) //NOSONAR
        {
            var releaseSettings = await _client.GetAsync(Requests.ReleaseManagement.Settings(projectId))
                .ConfigureAwait(false);
            if (!HasRequiredReleaseSettings(releaseSettings))
            {
                await _client.PutAsync(Requests.ReleaseManagement.Settings(projectId),
                    UpdateReleaseSettings(releaseSettings))
                    .ConfigureAwait(false);
            }

            var releasePipeline = await _client.GetAsync(new VsrmRequest<object>($"{projectId}/_apis/release/definitions/{releasePipelineId}")
                .AsJson())
                .ConfigureAwait(false);
            await _client.PutAsync(new VsrmRequest<object>($"{projectId}/_apis/release/definitions/{releasePipelineId}", new Dictionary<string, object> { {"api-version", "5.0" }}), 
                UpdateReleaseDefinition(releasePipeline))
                .ConfigureAwait(false);
        }

        private bool HasRequiredRetentionPolicy(ReleaseDefinition releasePipeline) =>
            releasePipeline
                .Environments
                .Select(e => e.RetentionPolicy)
                .Any(r => r.DaysToKeep >= RequiredRetentionDays && r.RetainBuild);

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