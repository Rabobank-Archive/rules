using System.Diagnostics;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public class Builds
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

        public static IVstsRestRequest<Build> Build(string project, string id)
        {
            return new VstsRestRequest<Build>($"{project}/_apis/build/builds/{id}");
        }
    }
}