using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Builds
    {
        public static IVstsRestRequest<Multiple<BuildDefinition>> BuildDefinitions(string projectId)
        {
            return new VstsRestRequest<Multiple<BuildDefinition>>(
                $"{projectId}/_apis/build/definitions?api-version=5.0-preview.7");
        }

        public static IVstsRestRequest<Multiple<BuildArtifact>> Artifacts(string project, string id)
        {
            return new VstsRestRequest<Multiple<BuildArtifact>>($"{project}/_apis/build/builds/{id}/artifacts");
        }

        public static IVstsRestRequest<Timeline> Timeline(string project, string id)
        {
            return new VstsRestRequest<Timeline>($"{project}/_apis/build/builds/{id}/timeline");
        }

        public static IVstsRestRequest<Build> Build(string project, string id)
        {
            return new VstsRestRequest<Build>($"{project}/_apis/build/builds/{id}");
        }
    }
}