using System;
using System.Collections.Generic;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Builds
    {
        public static IEnumerableRequest<BuildDefinition> BuildDefinitions(string projectId, bool includeAllProperties) =>
            new VstsRequest<BuildDefinition>(
                $"{projectId}/_apis/build/definitions", new Dictionary<string, object>
                {
                    {"includeAllProperties", $"{includeAllProperties}"},
                    {"api-version", "5.0-preview.7"}
                }).AsEnumerable();

        public static IEnumerableRequest<BuildDefinition> BuildDefinitions(string projectId) => 
            BuildDefinitions(projectId, false);

        public static IVstsRequest<BuildDefinition> BuildDefinition(string projectId, string id) =>
            new VstsRequest<BuildDefinition>($"{projectId}/_apis/build/definitions/{id}");

        public static IEnumerableRequest<BuildArtifact> Artifacts(string project, string id) => 
            new VstsRequest<BuildArtifact>($"{project}/_apis/build/builds/{id}/artifacts").AsEnumerable();

        public static IVstsRequest<Timeline> Timeline(string project, string id) => 
            new VstsRequest<Timeline>($"{project}/_apis/build/builds/{id}/timeline");

        public static IVstsRequest<Build> Build(string project, string id) => 
            new VstsRequest<Build>($"{project}/_apis/build/builds/{id}");

        public static IEnumerableRequest<Build> LongRunningBuilds(string project) =>
            new VstsRequest<Build>($"{project}/_apis/build/builds/", new Dictionary<string, object>
        {
            {"queryOrder", "startTimeAscending"},
            { "minTime", $"{DateTime.UtcNow.AddHours(-6).ToString("O")}"},
            {"api-version", "5.1"}
        }).AsEnumerable();

        public static IEnumerableRequest<Build> All(string project) => 
            new VstsRequest<Build>($"{project}/_apis/build/builds").AsEnumerable();
    }
}