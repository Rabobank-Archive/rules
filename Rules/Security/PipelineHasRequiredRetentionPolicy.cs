using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using Requests = SecurePipelineScan.VstsService.Requests;

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

        public void Reconcile(string project, string pipelineId)
        {
            var releaseSettings = _client.Get(Requests.ReleaseManagement.Settings(project));
            if (!HasRequiredReleaseSettings(releaseSettings))
            {
                _client.Put(Requests.ReleaseManagement.Settings(project),
                    UpdateReleaseSettings(releaseSettings));
            }

            var releasePipeline = _client.Get(new VsrmRequest<object>($"{project}/_apis/release/definitions/{pipelineId}")
                .AsJson());
            _client.Put(new VsrmRequest<object>($"{project}/_apis/release/definitions/{pipelineId}?api-version=5.0"),
                UpdateReleaseDefinition(releasePipeline));
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

        private JObject UpdateReleaseDefinition(JObject pipeline)
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