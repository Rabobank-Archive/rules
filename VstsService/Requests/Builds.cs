using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Builds
    {
        public static IVstsRequest<Multiple<BuildDefinition>> BuildDefinitions(string projectId, bool includeAllProperties = false)
        {
            return new VstsRequest<Multiple<BuildDefinition>>(
                $"{projectId}/_apis/build/definitions?includeAllProperties={includeAllProperties}&api-version=5.0-preview.7");
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
    }
}