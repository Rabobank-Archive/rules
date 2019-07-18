using System.Collections.Generic;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Builds
    {
        public static IVstsRequest<Multiple<BuildDefinition>> BuildDefinitions(string projectId, bool includeAllProperties)
        {
            return new VstsRequest<Multiple<BuildDefinition>>(
                $"{projectId}/_apis/build/definitions", new Dictionary<string, object>
                {
                    {"includeAllProperties", $"{includeAllProperties}"},
                    {"api-version", "5.0-preview.7"}
                });
        }

        public static IVstsRequest<Multiple<BuildDefinition>> BuildDefinitions(string projectId)
        {
            return BuildDefinitions(projectId, false);
        }

        public static IVstsRequest<Multiple<BuildArtifact>> Artifacts(string project, string id)
        {
            return new VstsRequest<Multiple<BuildArtifact>>($"{project}/_apis/build/builds/{id}/artifacts");
        }

        public static IVstsRequest<Timeline> Timeline(string project, string id)
        {
            return new VstsRequest<Timeline>($"{project}/_apis/build/builds/{id}/timeline");
        }

        public static IVstsRequest<Build> Build(string project, string id)
        {
            return new VstsRequest<Build>($"{project}/_apis/build/builds/{id}");
        }
        
        public static IVstsRequest<Multiple<Build>> All(string project)
        {
            return new VstsRequest<Multiple<Build>>($"{project}/_apis/build/builds");
        }

    }
}